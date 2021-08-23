// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;
using Microsoft.Recognizers.Text.DataTypes.TimexExpression;
using CoreBot.Models;
using AdaptiveCards;
using Microsoft.Extensions.Options;
using CoreBot.Fields;
using Newtonsoft.Json.Linq;
using CoreBot.Components.Widgets;
using CoreBot.Components.Functions;

namespace Microsoft.BotBuilderSamples.Dialogs
{
    /// <summary>
    /// Diálogo responsável por solicitar o Código de Segurança caso o usuário tenha entrado com Renavam e possua o Código de Segurança.
    /// </summary>
    public class RequiredSecureCodeCRLVeDialog : CancelAndHelpDialog
    {
        private CRLVeFields CRLVeFields;

        public RequiredSecureCodeCRLVeDialog()
            : base(nameof(RequiredSecureCodeCRLVeDialog))
        {
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt), null, "Pt-br"));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(new SpecificationsCRLVeDialog());
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                SecureCodeRequiredStepAsync,
                VerificationSecureCodeStepAsync

            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        /// <summary>
        /// Passo inicial responsável por obter o contexto do diálogo pai (SecureCodeCRLVe) e
        /// solicitar que o usuário escreva o código de segurança.
        /// </summary>
        /// <param name="stepContext">Contexto de RequiredSecureCodeCRLV.</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<DialogTurnResult> SecureCodeRequiredStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            WaterfallStepContext contextParent = (WaterfallStepContext)stepContext.Parent;
            CRLVeFields = (CRLVeFields)contextParent.Values["CRLVeFields"];
            stepContext.Values["CRLVeFields"] = CRLVeFields;

            // Geração da imagem do CRLVe.
            ImageCard ImageCard = new ImageCard();
            await stepContext.Context.SendActivityAsync(ImageCard.addImageCardSecureCode(), cancellationToken);

            // Geração de botão com link
            ButtonCard expandButton = new ButtonCard();
            await stepContext.Context.SendActivityAsync(expandButton.addButtonExpandSecureCode(CRLVeFields), cancellationToken);

            await stepContext.Context.SendActivityAsync(MessageFactory.Text("Informe o CÓDIGO DE SEGURANÇA"), cancellationToken);
            var secureCode = MessageFactory.Text(null, InputHints.ExpectingInput);
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = secureCode }, cancellationToken);
        }

        /// <summary>
        /// Passo responsável por comparar o código de Segurança digitado pelo usuário com o retornado pelo WebService.
        /// </summary>
        /// <param name="stepContext">Contexto de RequiredSecureCodeCRLV.</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<DialogTurnResult> VerificationSecureCodeStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            CRLVeFields = (CRLVeFields)stepContext.Values["CRLVeFields"];
            CRLVeFields.codSegurancaIn = stepContext.Result.ToString();

            // Se o código digitado pelo usuário for igual ao presente no WebService.
            if (CRLVeFields.codSegurancaIn == CRLVeFields.codSegurançaOut)
            {
                CRLVeFields.Count = 0;
                return await stepContext.BeginDialogAsync(nameof(SpecificationsCRLVeDialog), CRLVeFields, cancellationToken);
            }
            // Caso não esteja correto
            else
            {
                await stepContext.Context.SendActivityAsync("Este CÓDIGO DE SEGURANÇA é inválido!");

                // Contador que garante 3 tentativas
                Counter cont = new Counter();
                return await cont.ThreeTimes(CRLVeFields, stepContext, nameof(RequiredSecureCodeCRLVeDialog), cancellationToken, "o Código de Segurança");
            }
        }
    }
}
