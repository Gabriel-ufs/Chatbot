// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdaptiveCards;
using CoreBot.Fields;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;
using Newtonsoft.Json.Linq;

namespace Microsoft.BotBuilderSamples.Dialogs
{
    /// <summary>
    /// Diálogo responsável por verificar se o usuário deseja utilizar isenção no pagamento do IPVA.
    /// </summary>
    public class ExemptionDialog : CancelAndHelpDialog
    {
        //private LicenseFields LicenseFields;
        LicenseFields LicenseFields;
        public ExemptionDialog()
            : base(nameof(ExemptionDialog))
        {
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt)));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(new RecallDialog());
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                ExemptionStepAsync,
                ExemptionConfirmStepAsync,

            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        /// <summary>
        /// Passo responsável por receber o contexto do diálogo pai (Specifications) e
        /// perguntar se usuário deseja utilizar isenção de IPVA.
        /// </summary>
        /// <param name="stepContext">Contexto de Exemption.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>DialogTurnResult: ChoicePrompt com "Sim" ou "Não".</returns>
        private async Task<DialogTurnResult> ExemptionStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            WaterfallStepContext contextParent = (WaterfallStepContext)stepContext.Parent;
            LicenseFields = (LicenseFields)contextParent.Values["LicenseFields"];
            stepContext.Values["LicenseFields"] = LicenseFields;

            var promptOptions = new PromptOptions
            {
                Prompt = MessageFactory.Text("Deseja utilizar isenção de IPVA neste veículo?" + TextGlobal.Choice),
                RetryPrompt = MessageFactory.Text(TextGlobal.Desculpe + "Deseja utilizar isenção de IPVA neste veículo?" + TextGlobal.ChoiceDig),
                Choices = ChoiceFactory.ToChoices(new List<string> { "Sim", "Não" }),
            };

            return await stepContext.PromptAsync(nameof(ChoicePrompt), promptOptions, cancellationToken);
        }

        /// <summary>
        /// Passo responsável por receber a resposta solicitada em ExemptionStepAsync e retornar a variável IsencaoIPVA.
        /// </summary>
        /// <param name="stepContext"></param>
        /// <param name="cancellationToken">Contexto de Exemption.</param>
        /// <returns>ContinueDialogAsync com o valor de IsencaoIPVA modificados caso o usuário queira utilizar a isenção.</returns>
        private async Task<DialogTurnResult> ExemptionConfirmStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            LicenseFields = (LicenseFields)stepContext.Values["LicenseFields"];
            stepContext.Values["choice"] = ((FoundChoice)stepContext.Result).Value;

            if (stepContext.Values["choice"].ToString().ToLower() == "sim")
                return await stepContext.ContinueDialogAsync(cancellationToken);
            else
                return await stepContext.ContinueDialogAsync(cancellationToken);
        }

    }
}
