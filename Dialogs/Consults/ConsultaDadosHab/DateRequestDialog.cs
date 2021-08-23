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
    public class DateRequestDialog : CancelAndHelpDialog
    {
        ConsultFields ConsultFields;


        public DateRequestDialog()
            : base(nameof(DateRequestDialog))
        {


            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt), null, "pt-br"));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(new SpecificationConsult());


            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                DateRequest,
                DateValidation,
                ConfirmationData,
                OptionValidationConfirmData

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
        private async Task<DialogTurnResult> DateRequest(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            WaterfallStepContext contextParent = (WaterfallStepContext)stepContext.Parent;
            ConsultFields = new ConsultFields();
            ConsultFields = (ConsultFields)contextParent.Values["ConsultFields"];
            stepContext.Values["ConsultFields"] = ConsultFields;

            await stepContext.Context.SendActivityAsync(MessageFactory.Text("Por Favor, agora informe a sua data de nascimento"), cancellationToken);
            var date = MessageFactory.Text(null, InputHints.ExpectingInput);
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = date }, cancellationToken);
        }


        /// <summary>
        /// Passo responsável por validar a data passada pelo usuário, caso a data seja válida o dialogo continua, caso não seja, o contexto atual é destruido e volta para o RootConsultChoice.
        /// </summary>
        /// <param name="stepContext">Contexto do RootConsulteDialog</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>

        private async Task<DialogTurnResult> DateValidation(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var ConsultFields = (ConsultFields)stepContext.Values["ConsultFields"];
            ConsultFields.date = (string)stepContext.Result;

            DateTime dataValida;

            if (DateTime.TryParse(ConsultFields.date, CultureInfo.InstalledUICulture, DateTimeStyles.AssumeLocal, out dataValida))

            {
                return await stepContext.ContinueDialogAsync(cancellationToken);
            }

            else
            {
                await stepContext.Context.SendActivityAsync("Esta Data tem formato inválido! \n Digite uma data no formato correto (DD/MM/AAAA)");
                return await stepContext.ReplaceDialogAsync(nameof(DateRequestDialog), default, cancellationToken);
            }
        }

        private async Task<DialogTurnResult> ConfirmationData(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var ConsultFields = (ConsultFields)stepContext.Values["ConsultFields"];


            await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Confirme se seus dados estão corretos: " +
                                $"\r\n Número de registro: {ConsultFields.register} \r\n Número da CNH: {ConsultFields.CNH} \r\n Data de nascimento: {ConsultFields.date}"));

            var promptOptions = new PromptOptions
            {
                Prompt = MessageFactory.Text("Os dados acima estão corretos?  \r\n1. Sim (Continuar) \n2. Não (Preencher dados novamente)"),
                RetryPrompt = MessageFactory.Text(TextGlobal.Desculpe + TextGlobal.Prosseguir),
                Choices = ChoiceFactory.ToChoices(new List<string> { "Sim", "Não" }),
            };

            return await stepContext.PromptAsync(nameof(ChoicePrompt), promptOptions, cancellationToken);
        }

        /// <summary>
        /// Passo responsável por receber a resposta obtida em ConfirmationData. Caso seja "sim", continua para o próximo passo, senão destrói o contexto atual e volta para RootConsultChoice.
        /// </summary>
        /// <param name="stepContext">Contexto do RootConsulteDialog1</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>

        private async Task<DialogTurnResult> OptionValidationConfirmData(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            ConsultFields = (ConsultFields)stepContext.Values["ConsultFields"];
            stepContext.Values["choice"] = ((FoundChoice)stepContext.Result).Value;


            if (stepContext.Values["choice"].ToString().ToLower() == "sim")
            {
                stepContext.Values["ConsultFields"] = ConsultFields;
                return await stepContext.BeginDialogAsync(nameof(SpecificationConsult), ConsultFields, cancellationToken);
            }
            else return await stepContext.ReplaceDialogAsync(nameof(RootConsultChoice), default, cancellationToken);
        }

    }
}