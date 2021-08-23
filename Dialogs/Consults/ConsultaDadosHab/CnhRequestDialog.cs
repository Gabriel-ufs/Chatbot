// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Globalization;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CoreBot.Fields;
using CoreBot.Components.Widgets;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;

namespace Microsoft.BotBuilderSamples.Dialogs
{
    /// <summary>
    /// Diálogo raiz de geração de CRLVe.
    /// </summary>
    public class CnhRequestDialog : CancelAndHelpDialog
    {
        ConsultFields ConsultFields;


        public CnhRequestDialog()
            : base(nameof(CnhRequestDialog))
        {


            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt), null, "pt-br"));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(new DateRequestDialog());


            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                CNHRequest,
                ValidationCNH,

            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        /// <summary>
        /// Passo responsável por captar o contexto do diálogo pai (RootConsultChoice), apresentar introdução do serviço de Consulta de processo
        /// e perguntar se o usuário deseja continuar.
        /// </summary>
        /// <param name="stepContext">Contexto do RootConsultDialog1 </param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<DialogTurnResult> CNHRequest(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            WaterfallStepContext contextParent = (WaterfallStepContext)stepContext.Parent;
            ConsultFields = new ConsultFields();
            ConsultFields = (ConsultFields)contextParent.Values["ConsultFields"];
            stepContext.Values["ConsultFields"] = ConsultFields;

            //Geração de imagem
            ImageCard ImageCard = new ImageCard();
            await stepContext.Context.SendActivityAsync(ImageCard.addImageCnhNumber(), cancellationToken);

            await stepContext.Context.SendActivityAsync(MessageFactory.Text("Por Favor, informe o número da sua CNH "), cancellationToken);
            var cnh = MessageFactory.Text(null, InputHints.ExpectingInput);

            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = cnh }, cancellationToken);
        }

        private async Task<DialogTurnResult> ValidationCNH(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var ConsultFields = (ConsultFields)stepContext.Values["ConsultFields"];
            ConsultFields.CNH = (string)stepContext.Result;

            if (ConsultFields.CNH.Length > 7 && ConsultFields.IsNumeric(ConsultFields.CNH))
            {
                return await stepContext.BeginDialogAsync(nameof(DateRequestDialog), ConsultFields, cancellationToken);
            }
            else if (ConsultFields.contCnh < 3)
            {
                await stepContext.Context.SendActivityAsync(MessageFactory.Text("Número de CNH inválido! Tente novamente"));
                return await stepContext.ReplaceDialogAsync(nameof(CnhRequestDialog), default, cancellationToken);
            }
            else
            {
                await stepContext.Context.SendActivityAsync(MessageFactory.Text("Número de CNH inválido! Tente novamente"));
                return await stepContext.EndDialogAsync(cancellationToken:cancellationToken);
            }
        }
    }
}