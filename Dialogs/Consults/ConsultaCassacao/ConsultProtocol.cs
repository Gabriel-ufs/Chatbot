// Copyright(c) Microsoft Corporation.All rights reserved.
// Licensed under the MIT License.

using System;
using System.Globalization;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CoreBot.Fields;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;

namespace Microsoft.BotBuilderSamples.Dialogs
{
    /// <summary>
    /// Diálogo raiz de geração de CRLVe.
    /// </summary>
    public class ConsultProtocol : CancelAndHelpDialog
    {
        ConsultFields ConsultFields;


        public ConsultProtocol()
            : base(nameof(ConsultProtocol))
        {


            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt), null, "pt-br"));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(new SpecificationConsult());


            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                ProtocolRequest,
                ProtocolValidation,
                ConfirmData,
                OptionValidationConfirmData,

            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        /// <summary>
        /// Passo responsável por receber o número de protocolo do usuário.
        /// </summary>
        /// <param name="stepContext">Contexto do RootConsulteDialog</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<DialogTurnResult> ProtocolRequest(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            WaterfallStepContext contextParent = (WaterfallStepContext)stepContext.Parent;
            var ConsultFields = new ConsultFields();
            ConsultFields = (ConsultFields)contextParent.Values["ConsultFields"];
            stepContext.Values["ConsultFields"] = ConsultFields;

            await stepContext.Context.SendActivityAsync(MessageFactory.Text("Informe o número de protcolo "), cancellationToken);
            var protocol = MessageFactory.Text(null, InputHints.ExpectingInput);
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = protocol }, cancellationToken);
        }

        private async Task<DialogTurnResult> ProtocolValidation(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var ConsultFields = (ConsultFields)stepContext.Values["ConsultFields"];
            ConsultFields.protocol = (string)stepContext.Result;

            if (ConsultFields.protocol.Length > 7 && ConsultFields.IsNumeric(ConsultFields.protocol))
            {
                return await stepContext.ContinueDialogAsync(cancellationToken);
            }

            else if (ConsultFields.contProtocol < 3)
            {
                ConsultFields.contProtocol++;
                await stepContext.Context.SendActivityAsync("O número de protocolo informado é inválido. Verifique-o e tente mais tarde");
                return await stepContext.ReplaceDialogAsync(nameof(ConsultProtocol), default, cancellationToken);
            }

            else
            {
                await stepContext.Context.SendActivityAsync(MessageFactory.Text("Número de protocolo com formato inválido"), cancellationToken);
                return await stepContext.ReplaceDialogAsync(nameof(RootConsultDialog), default, cancellationToken);
            }
        }

        /// <summary>
        /// Passo responsável por perguntar ao usuário se as informações coletadas estão corretas.
        /// </summary>
        /// <param name="stepContext">Contexto do RootConsulteDialog </param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>


        private async Task<DialogTurnResult> ConfirmData(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var ConsultFields = (ConsultFields)stepContext.Values["ConsultFields"];

            await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Número de protocolo: {ConsultFields.protocol}  \n Data de nascimento: {ConsultFields.date}"), cancellationToken);

            var promptOptions = new PromptOptions
            {
                Prompt = MessageFactory.Text("Os dados acima estão corretos?  \r\n1. Sim (Continuar) \n2. Não (Preencher dados novamente)"),
                RetryPrompt = MessageFactory.Text(TextGlobal.Desculpe + TextGlobal.Prosseguir),
                Choices = ChoiceFactory.ToChoices(new List<string> { "Sim", "Não" }),
            };

            return await stepContext.PromptAsync(nameof(ChoicePrompt), promptOptions, cancellationToken);

        }


        /// <summary>
        /// Passo responsável por receber a resposta obtida em ConfirmData. Caso seja "sim", continua para o próximo passo, senão volta para o início do dialogo RootConsultDialog.
        /// </summary>
        /// <param name="stepContext">Contexto do RootConsulteDialog</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>

        private async Task<DialogTurnResult> OptionValidationConfirmData(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            ConsultFields = (ConsultFields)stepContext.Values["ConsultFields"];

            stepContext.Values["choice"] = ((FoundChoice)stepContext.Result).Value;
            if (stepContext.Values["choice"].ToString().ToLower() == "sim") return await stepContext.BeginDialogAsync(nameof(SpecificationConsult), ConsultFields, cancellationToken);
            else return await stepContext.ReplaceDialogAsync(nameof(RootConsultDialog), default, cancellationToken);
        }


    }
}