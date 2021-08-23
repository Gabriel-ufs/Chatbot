// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdaptiveCards;
using CoreBot.Components.Widgets;
using CoreBot.Fields;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;
using Newtonsoft.Json.Linq;

namespace Microsoft.BotBuilderSamples.Dialogs
{
    /// <summary>
    /// Diálogo responsável por alertar ao usuário que o veículo possui uma chamada para Recall.
    /// </summary>
    public class RecallDialog : CancelAndHelpDialog
    {
        LicenseFields LicenseFields;
        public RecallDialog()
            : base(nameof(RecallDialog))
        {
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt)));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                IntroStepAsync,
                AuthorizationStepAsync,
                ConfirmStepAsync,
            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        /// <summary>
        /// Passo responsável por informar a existência de uma chamada da montadora e montar um link para o portal.
        /// </summary>
        /// <param name="stepContext">Contexto de Recall</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<DialogTurnResult> IntroStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            WaterfallStepContext contextParent = (WaterfallStepContext)stepContext.Parent;
            LicenseFields = (LicenseFields)contextParent.Values["LicenseFields"];
            stepContext.Values["LicenseFields"] = LicenseFields;

            await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Há uma chamada da montadora de seu veículo para RECALL! Para mais informações acesse o site do detran e verifique a situação do seu veículo."), cancellationToken);

            // Botão com link.
            ButtonCard expandButton = new ButtonCard();
            await stepContext.Context.SendActivityAsync(expandButton.addButtonExpand(LicenseFields, "Ir para o site", "https://www.detran.se.gov.br/portal/?menu=1"), cancellationToken);

            return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Passo responsável por solicitar confirmação do usuário caso esteja ciente da informação.
        /// </summary>
        /// <param name="stepContext">Contexto de Recall</param>
        /// <param name="cancellationToken"></param>
        /// <returns>DialogTurnResult com choicePrompt de Sim ou Não.</returns>
        private async Task<DialogTurnResult> AuthorizationStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var promptOptions = new PromptOptions
            {
                Prompt = MessageFactory.Text($"Esta ciente dessa informação?" + TextGlobal.Choice),
                RetryPrompt = MessageFactory.Text(TextGlobal.Desculpe + "Esta ciente dessa informação?" + TextGlobal.ChoiceDig),
                Choices = ChoiceFactory.ToChoices(new List<string> { "Sim", "Não" }),
            };

            return await stepContext.PromptAsync(nameof(ChoicePrompt), promptOptions, cancellationToken);
        }

        /// <summary>
        /// Passo responsável por receber a resposta oriunda do passo AuthorizationStepAsync e verificar se o usuário está ciente.
        /// </summary>
        /// <param name="stepContext">Contexto de Recall</param>
        /// <param name="cancellationToken"></param>
        /// <returns>ContinueDialogAsync, que retorna para o diálogo Specifications ou ReplaceDialogAsync retornando para o menu inicial.</returns>
        private async Task<DialogTurnResult> ConfirmStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            LicenseFields = (LicenseFields)stepContext.Values["LicenseFields"];
            stepContext.Values["choice"] = ((FoundChoice)stepContext.Result).Value;

            if (stepContext.Values["choice"].ToString().ToLower() == "sim")
                return await stepContext.ContinueDialogAsync(cancellationToken);
            else
            {
                await stepContext.Context.SendActivityAsync("Infelizmente não podemos continuar sem esta confirmação!");
                return await stepContext.ReplaceDialogAsync(nameof(MainDialog), LicenseFields, cancellationToken);
            }
        }

    }
}
