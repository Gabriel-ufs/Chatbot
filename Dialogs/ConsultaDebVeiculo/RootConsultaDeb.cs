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
    public class RootConsultaDeb : CancelAndHelpDialog
    {
        ConsultaDebitosFields ConsultaDebitosFields;

        public RootConsultaDeb()
            : base(nameof(RootConsultaDeb))
        {


            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt), null, "pt-br"));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(new SpecificationConsult());
            AddDialog(new ResultLicense());
            AddDialog(new ResultCode());


            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                OptionStepAsync,
                ConfirmStepAsync,
                RenavamAsk,
                ValidationRenavam,
                
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
            ConsultaDebitosFields = new ConsultaDebitosFields();
            ConsultaDebitosFields = (ConsultaDebitosFields)contextParent.Values["ConsultaDebitosFields"];
            stepContext.Values["ConsultaDebitosFields"] = ConsultaDebitosFields;
           
            if(ConsultaDebitosFields.cont == 0)
            {
                ConsultaDebitosFields.cont++;
                await stepContext.Context.SendActivityAsync("**Bem-vindo ao Serviço de Consulta de Dados do veículo (débitos)**");
                await stepContext.Context.SendActivityAsync("Aqui você poderá consultar os débitos referentes ao seu veículo");
                await stepContext.Context.SendActivityAsync("A qualquer momento, você pode digitar **CANCELAR** para parar o processo e retornar ao menu de opções");
            }
            

            var promptOptions = new PromptOptions
            {
                Prompt = MessageFactory.Text(TextGlobal.Prosseguir),
                RetryPrompt = MessageFactory.Text(TextGlobal.Desculpe + TextGlobal.Prosseguir),
                Choices = ChoiceFactory.ToChoices(new List<string> { "Sim", "Não" }),
            };

            return await stepContext.PromptAsync(nameof(ChoicePrompt), promptOptions, cancellationToken);

        }

        private async Task<DialogTurnResult> ConfirmStepAsync (WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var ConsultaDebitosFields = (ConsultaDebitosFields)stepContext.Values["ConsultaDebitosFields"];
            stepContext.Values["choice"] = ((FoundChoice)stepContext.Result).Value;


            if (stepContext.Values["choice"].ToString().ToLower() == "sim") return await stepContext.ContinueDialogAsync(cancellationToken);
            else return await stepContext.ReplaceDialogAsync(nameof(VehicleMenuDialog), default, cancellationToken);

        }

        private async Task<DialogTurnResult> RenavamAsk (WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var ConsultaDebitosFields = (ConsultaDebitosFields)stepContext.Values["ConsultaDebitosFields"];

            // Geração da imagem
            ImageCard ImageCard = new ImageCard();
            await stepContext.Context.SendActivityAsync(ImageCard.addImageCardRenavamPlaca(), cancellationToken);

            // Geração de botão com link
            ButtonCard expandButton = new ButtonCard();
            await stepContext.Context.SendActivityAsync(expandButton.addButtonExpandRenavamConsult(ConsultaDebitosFields), cancellationToken);

            await stepContext.Context.SendActivityAsync(MessageFactory.Text("Por favor, informe o RENAVAM do veículo."), cancellationToken);
            var renavam = MessageFactory.Text(null, InputHints.ExpectingInput);
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = renavam }, cancellationToken);
        }

        private async Task<DialogTurnResult> ValidationRenavam (WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var ConsultaDebitosFields = (ConsultaDebitosFields)stepContext.Values["ConsultaDebitosFields"];
            ConsultaDebitosFields.renavam = (string)stepContext.Result;

            if (ConsultaDebitosFields.renavam.Length >= 9 && ConsultaDebitosFields.IsNumeric(ConsultaDebitosFields.renavam))
            {
                return await stepContext.BeginDialogAsync(nameof(ResultCode), ConsultaDebitosFields, cancellationToken);
            }
            else if (ConsultaDebitosFields.contRenavam < 3)
            {
                ConsultaDebitosFields.contRenavam++;
                await stepContext.Context.SendActivityAsync("RENAVAM inválido, tente novamente");
                return await stepContext.ReplaceDialogAsync(nameof(RootConsultaDeb), default, cancellationToken);
            }
            else
            {
                await stepContext.Context.SendActivityAsync("RENAVAM informado é inválido. Verifique no documento e tente novamente");
                return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
            }

        }

    }
}