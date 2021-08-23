using CoreBot.Fields;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;
using Microsoft.BotBuilderSamples.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CoreBot.Services.WSDLService;
using CoreBot.Components.Widgets;
using CoreBot.Components.Functions;
using CoreBot.Models;

namespace CoreBot.Dialogs.Calendario
{
    public class Calendario : CancelAndHelpDialog
    {
        CalendarioFields CalendarioFields;

        public Calendario()
            : base(nameof(Calendario))
        {


            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt), null, "pt-br"));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(new SpecificationConsult());
            AddDialog(new ResultLicense());


            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                OptionStepAsync,
                OptionValidationStepAsync,
                PlacaRequest,
                PlacaValidation,
                
                
               // OptionValidationConfirmData,

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
        private async Task<DialogTurnResult> OptionStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // Busca pelo contexto do diálogo pai.
            WaterfallStepContext contextParent = (WaterfallStepContext)stepContext.Parent;
            CalendarioFields = new CalendarioFields();
            CalendarioFields = (CalendarioFields)contextParent.Values["CalendarioFields"];
            stepContext.Values["CalendarioFields"] = CalendarioFields;


            if(CalendarioFields.cont == 0)
            {
                await stepContext.Context.SendActivityAsync("**Bem-vindo ao serviço de Consulta da data de Licenciamento.**");
                await stepContext.Context.SendActivityAsync("Aqui você poderá consultar a data de licenciamento referente ao ano corrente ou anteriores");
                await stepContext.Context.SendActivityAsync("A qualquer momento, você pode digitar **CANCELAR** para parar o processo e retornar ao menu de opções");
                CalendarioFields.cont++;
            }

            else
            {
                await stepContext.Context.SendActivityAsync("Vamos tentar novamente");
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
            CalendarioFields = (CalendarioFields)stepContext.Values["CalendarioFields"];

            stepContext.Values["choice"] = ((FoundChoice)stepContext.Result).Value;
            if (stepContext.Values["choice"].ToString().ToLower() == "sim") return await stepContext.ContinueDialogAsync(cancellationToken);
            else return await stepContext.ReplaceDialogAsync(nameof(VehicleMenuDialog), default, cancellationToken);

        }

        private async Task<DialogTurnResult> PlacaRequest(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            await stepContext.Context.SendActivityAsync(MessageFactory.Text("Por favor, informe o último dígito da placa."), cancellationToken);
            var placa = MessageFactory.Text(null, InputHints.ExpectingInput);
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = placa }, cancellationToken);
        }

        /// <summary>
        /// Passo responsável por validar o protocolo informado no passo anterior(ProtocolRequest)
        /// </summary>
        /// <param name="stepContext"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<DialogTurnResult> PlacaValidation(WaterfallStepContext stepContext, CancellationToken cancellationToken) //contador
        {
            var CalendarioFields = (CalendarioFields)stepContext.Values["CalendarioFields"];
            CalendarioFields.finalPlaca = (string)stepContext.Result;
            CalendarioFields.contFinalPLaca++;

            if (CalendarioFields.finalPlaca.Length == 1 && CalendarioFields.IsNumeric(CalendarioFields.finalPlaca))
            {
                return await stepContext.BeginDialogAsync(nameof(ResultLicense), CalendarioFields, cancellationToken);
            }

            else if (CalendarioFields.contFinalPLaca < 3)
            {
                
                await stepContext.Context.SendActivityAsync(MessageFactory.Text("Número de final de placa inválido (Digite um valor de 0 a 9)"), cancellationToken);
                return await stepContext.ReplaceDialogAsync(nameof(Calendario), default, cancellationToken);
            }
            else
            {
                await stepContext.Context.SendActivityAsync(MessageFactory.Text("O dígito informado é inválido. Verifique o final da placa e tente novamente."), cancellationToken); //adicionar texto de numero de tentativas excedidad
                return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
            }
        }

    }
}