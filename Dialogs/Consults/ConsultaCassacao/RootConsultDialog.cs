// Copyright (c) Microsoft Corporation. All rights reserved.
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
    public class RootConsultDialog : CancelAndHelpDialog
    {
        ConsultFields ConsultFields;


        public RootConsultDialog()
            : base(nameof(RootConsultDialog))
        {


            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt), null, "pt-br"));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(new ConsultProtocol());


            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                OptionStepAsync,
                OptionValidationStepAsync,
                DateRequest,
                DateValidation,

            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        /// <summary>
        /// Passo responsável por captar o contexto do diálogo pai (MainDialog), apresentar introdução do serviço de Consulta de processo
        /// e perguntar se o usuário deseja continuar.
        /// </summary>
        /// <param name="stepContext">Contexto do Consult</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<DialogTurnResult> OptionStepAsync (WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // Busca pelo contexto do diálogo pai.
            WaterfallStepContext contextParent = (WaterfallStepContext)stepContext.Parent;
            ConsultFields = new ConsultFields();
            ConsultFields = (ConsultFields)contextParent.Values["ConsultFields"];
            stepContext.Values["ConsultFields"] = ConsultFields;

            if(ConsultFields.cont == 0)
            {
                await stepContext.Context.SendActivityAsync("Aqui você pode visualizar as informações referentes aos Dados do Processo da Suspensão/Cassação do Direito de Dirigir");
                await stepContext.Context.SendActivityAsync("A qualquer momento, você pode digitar **CANCELAR** para parar o processo e retornar ao menu de opções");
                ConsultFields.cont++;
            }
            
            var promptOptions = new PromptOptions
            {
                Prompt = MessageFactory.Text(TextGlobal.Prosseguir),
                RetryPrompt = MessageFactory.Text(TextGlobal.Desculpe + TextGlobal.Prosseguir),
                Choices = ChoiceFactory.ToChoices(new List<string> { "Sim", "Não" }),
            };

            return await stepContext.PromptAsync(nameof(ChoicePrompt), promptOptions, cancellationToken);
        }


        /// <summary>
        /// Passo responsável por receber a resposta obtida em OptionStepAsync. Caso seja "sim", continua para o próximo passo, senão destrói o contexto atual e reinicia MainDialog.
        /// </summary>
        /// <param name="stepContext">Contexto do RootConsulteDialog</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>

        private async Task<DialogTurnResult> OptionValidationStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            ConsultFields = (ConsultFields)stepContext.Values["ConsultFields"];

            stepContext.Values["choice"] = ((FoundChoice)stepContext.Result).Value;
            if (stepContext.Values["choice"].ToString().ToLower() == "sim") return await stepContext.ContinueDialogAsync(cancellationToken);
            else return await stepContext.ReplaceDialogAsync(nameof(MainDialog), default, cancellationToken);
        }


        /// <summary>
        /// Passo responsável por receber a data de nascimento do usuário.
        /// </summary>
        /// <param name="stepContext">Contexto do RootConsulteDialog</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>

        private async Task<DialogTurnResult> DateRequest (WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var text = "Para iniciarmos o processo de Consulta a dados da Habilitação vou precisar de algumas informações";
            await stepContext.Context.SendActivityAsync(MessageFactory.Text(text), cancellationToken);

            await stepContext.Context.SendActivityAsync(MessageFactory.Text("Informe a sua data de nascimento (DD/MM/AAAA) "), cancellationToken);
            var date = MessageFactory.Text(null, InputHints.ExpectingInput);
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = date }, cancellationToken);
            
        }


        /// <summary>
        /// Passo responsável por validar a data passada pelo usuário, caso a data seja válida o dialogo continua, caso não seja, o contexto atual é destruido e volta para o MainDialog.
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
                return await stepContext.BeginDialogAsync(nameof(ConsultProtocol), ConsultFields, cancellationToken);
            }

            else if(ConsultFields.contTry < 3)
            {
                ConsultFields.contTry++;
                await stepContext.Context.SendActivityAsync("Esta Data tem formato inválido! \n Digite uma data no formato correto (DD/MM/AAAA)");
                return await stepContext.ReplaceDialogAsync(nameof(RootConsultDialog), default, cancellationToken);
            }

            else 
            {
                await stepContext.Context.SendActivityAsync("O formato do ano informado é inválido. Verifique o ano e tente mais tarde");
                return await stepContext.EndDialogAsync(cancellationToken:cancellationToken);
            }
            
        }


            

    }

}

