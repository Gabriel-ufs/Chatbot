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
    public class ResultLicense : CancelAndHelpDialog
    {
        CalendarioFields CalendarioFields;

        public ResultLicense()
            : base(nameof(ResultLicense))
        {


            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt), null, "pt-br"));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(new SpecificationConsult());


            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                AnoRequest,
                AnoValidation,
                ResultCalendario
                

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
        private async Task<DialogTurnResult> AnoRequest(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // Busca pelo contexto do diálogo pai.
            WaterfallStepContext contextParent = (WaterfallStepContext)stepContext.Parent;
            CalendarioFields = new CalendarioFields();
            CalendarioFields = (CalendarioFields)contextParent.Values["CalendarioFields"];
            stepContext.Values["CalendarioFields"] = CalendarioFields;

            await stepContext.Context.SendActivityAsync(MessageFactory.Text("Por favor, informe o ano do calendário desejado:"), cancellationToken);
            var ano = MessageFactory.Text(null, InputHints.ExpectingInput);
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = ano }, cancellationToken);
        }



        private async Task<DialogTurnResult> AnoValidation(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var CalendarioFields = (CalendarioFields)stepContext.Values["CalendarioFields"];
            CalendarioFields.anoLicenciamentoIn = (string)stepContext.Result;

            CalendarioFields.contAno++;

            if (CalendarioFields.anoLicenciamentoIn.Length == 4 && CalendarioFields.IsNumeric(CalendarioFields.anoLicenciamentoIn))
            {
                return await stepContext.ContinueDialogAsync(cancellationToken);
            }

            else if(CalendarioFields.contAno < 3)
            {
                await stepContext.Context.SendActivityAsync(MessageFactory.Text("Ano inválido, tente novamente"), cancellationToken);
                return await stepContext.ReplaceDialogAsync(nameof(ResultLicense), default, cancellationToken);
            }

            else
            {
                DateTime thisDay = DateTime.Now;
                await stepContext.Context.SendActivityAsync(MessageFactory.Text("O formato do ano é inválido. Tente novamente informando conforme o exemplo: " + thisDay.ToString("yyyy")), cancellationToken);
                return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
            }
        }



        private async Task<DialogTurnResult> ResultCalendario(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            VehicleCalendar calendario = new VehicleCalendar();

            var CalendarioFields = (CalendarioFields)stepContext.Values["CalendarioFields"];

            //DateTime hoje = new DateTime();

            int Ano = Convert.ToInt32(DateTime.Now.Year.ToString());
            int Limite = Ano - 7;
            int info = Convert.ToInt32(CalendarioFields.anoLicenciamentoIn);

            if (info < Limite && CalendarioFields.contAno < 3)
            {
                CalendarioFields.contAno++;
                await stepContext.Context.SendActivityAsync(MessageFactory.Text("O Ano solicitado ultrapassa 8 anos"), cancellationToken);
                return await stepContext.ReplaceDialogAsync(nameof(ResultLicense), default, cancellationToken);
            }

            else if (info > Ano && CalendarioFields.contAno <= 3)
            {
                CalendarioFields.contAno++;
                await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Não é possível consultar o Licencenciamento do ano {info}"));
                return await stepContext.ReplaceDialogAsync(nameof(ResultLicense), default, cancellationToken);
            }
            else if(CalendarioFields.contAno > 3)
            {
                await stepContext.Context.SendActivityAsync(MessageFactory.Text("O ano informado é inválido. Verifique o ano desejado e tente novamente"), cancellationToken);
                return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
            }

            else
            {
                var webResult = await calendario.ObtainCalendarLicenciamento(CalendarioFields.plataforma, Convert.ToInt32(CalendarioFields.finalPlaca), Convert.ToInt32(CalendarioFields.anoLicenciamentoIn));
                
                CalendarioFields.dataDesconto = webResult.DataDesconto;
                CalendarioFields.dataPagToSemDesconto = webResult.DataPagTotSemDesconto;
                CalendarioFields.dataFiscalizacao = webResult.DataFiscalizacao;
                

                await stepContext.Context.SendActivityAsync($"**Resultado da Consulta da data de Licenciamento** \r\n **Final de Placa**: {CalendarioFields.finalPlaca} \r\n **Data com desconto**: {CalendarioFields.dataDesconto}  \r\n **Data sem desconto**: {CalendarioFields.dataPagToSemDesconto} \r\n **Data de Fiscalização**: {CalendarioFields.dataFiscalizacao}  \r\n");
                return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
            }
        }
    }
}