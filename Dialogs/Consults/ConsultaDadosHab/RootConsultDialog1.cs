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
    public class RootConsultDialog1 : CancelAndHelpDialog
    {
        ConsultFields ConsultFields;


        public RootConsultDialog1()
            : base(nameof(RootConsultDialog1))
        {


            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt), null, "pt-br"));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(new CnhRequestDialog());
            

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                OptionStepAsync,
                OptionValidationStepAsync,
                RegisterRequest,
                ValidationRegister,

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
        private async Task<DialogTurnResult> OptionStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // Busca pelo contexto do diálogo pai.
            WaterfallStepContext contextParent = (WaterfallStepContext)stepContext.Parent;
            ConsultFields = new ConsultFields();
            ConsultFields = (ConsultFields)contextParent.Values["ConsultFields"];
            stepContext.Values["ConsultFields"] = ConsultFields;
            if (ConsultFields.cont == 0)
            {
                await stepContext.Context.SendActivityAsync("Aqui você pode visualizar as informações referentes aos Dados da sua Habilitação");
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
        /// <param name="stepContext">Contexto do RootConsulteDialog1</param>
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
        /// Passo responsável por informar a necessidade de dados ao usuário e perguntar o número do registro 
        /// </summary>
        /// <param name="stepContext">Contexto do RootConsulteDialog1</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>

        private async Task<DialogTurnResult> RegisterRequest (WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            await stepContext.Context.SendActivityAsync("Para iniciarmos o processo de Consulta a dados da Habilitação vou precisar de algumas informações");

            //Geração de Imagem
            ImageCard ImageCard = new ImageCard();
            await stepContext.Context.SendActivityAsync(ImageCard.addImageResgister(), cancellationToken);

            await stepContext.Context.SendActivityAsync(MessageFactory.Text("Por Favor, informe o número do registro da sua CNH "), cancellationToken);
            var register = MessageFactory.Text(null, InputHints.ExpectingInput);
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions {Prompt = register}, cancellationToken);
        }

        /// <summary>
        /// Passo responsável por validar o numero da CNH informado pelo usuário, caso seja um valor válido o diálogo continua, caso não seja destrói o contexto atual e volta para RootConsutChoice.
        /// </summary>
        /// <param name="stepContext">Contexto do RootConsulteDialog1</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
       


        private async Task<DialogTurnResult> ValidationRegister (WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var ConsultFields = (ConsultFields)stepContext.Values["ConsultFields"];
            ConsultFields.register = (string)stepContext.Result;

            if (ConsultFields.register.Length > 7 && ConsultFields.IsNumeric(ConsultFields.register))
            {
                return await stepContext.BeginDialogAsync(nameof(CnhRequestDialog), ConsultFields, cancellationToken);
            }
            else if (ConsultFields.contTry < 3)
            {
                ConsultFields.contTry++;
                await stepContext.Context.SendActivityAsync("O número de registro é inálido. Verifique-o e tente novamente");
                return await stepContext.ReplaceDialogAsync(nameof(RootConsultDialog1), default, cancellationToken);
            }
            else
            {
                await stepContext.Context.SendActivityAsync("O número de registro é inálido. Verifique-o e tente novamente");
                return await stepContext.EndDialogAsync(cancellationToken:cancellationToken);
            } 
        }

    }
        
}
