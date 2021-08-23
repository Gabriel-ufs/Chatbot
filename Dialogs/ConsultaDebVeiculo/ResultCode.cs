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
    public class ResultCode : CancelAndHelpDialog
    {
        ConsultaDebitosFields ConsultaDebitosFields;

        public ResultCode()
            : base(nameof(ResultCode))
        {


            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt), null, "pt-br"));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(new SpecificationConsult());
            AddDialog(new ResultLicense());


            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                OptionStepAsync,
                ValidationCod,
                ResultConsult,
                
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
            WaterfallStepContext contextParent = (WaterfallStepContext)stepContext.Parent;
            ConsultaDebitosFields = new ConsultaDebitosFields();
            ConsultaDebitosFields = (ConsultaDebitosFields)contextParent.Values["ConsultaDebitosFields"];
            stepContext.Values["ConsultaDebitosFields"] = ConsultaDebitosFields;


            // Geração da imagem do codigo de seguranca.
            ImageCard ImageCard = new ImageCard();
            await stepContext.Context.SendActivityAsync(ImageCard.addImageCardSecureCode(), cancellationToken);
            //Botão para ampliar a imagem de onde localizar o codigo de seguranca
            ButtonCard expandButton = new ButtonCard();
            await stepContext.Context.SendActivityAsync(expandButton.addButtonExpandSecureCode2(ConsultaDebitosFields), cancellationToken);

            await stepContext.Context.SendActivityAsync("Por favor, informe o código de segurança do seu veículo");

            var codSeguranca = MessageFactory.Text(null, InputHints.ExpectingInput);
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = codSeguranca }, cancellationToken);
        }


        private async Task<DialogTurnResult> ValidationCod (WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var ConsultaDebitosFields = (ConsultaDebitosFields)stepContext.Values["ConsultaDebitosFields"];
            ConsultaDebitosFields.codSeguranca = (string)stepContext.Result;

            if((ConsultaDebitosFields.codSeguranca.Length == 11 || ConsultaDebitosFields.codSeguranca.Length == 10 || ConsultaDebitosFields.codSeguranca == "0") && ConsultaDebitosFields.IsNumeric(ConsultaDebitosFields.codSeguranca))
            {
                return await stepContext.ContinueDialogAsync(cancellationToken);
            }
            else if(ConsultaDebitosFields.contSecCode < 3)
            {
                ConsultaDebitosFields.contSecCode++;
                await stepContext.Context.SendActivityAsync("Código de segurança inválido, tente novamente");
                return await stepContext.ReplaceDialogAsync(nameof(ResultCode), default, cancellationToken);
            }
            else
            {
                await stepContext.Context.SendActivityAsync("Código de segurança informado é inválido. Verifique no documento e tente novamente");
                return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
            }
        }


        private async Task<DialogTurnResult> ResultConsult (WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var ConsultaDebitosFields = (ConsultaDebitosFields)stepContext.Values["ConsultaDebitosFields"];
            ConsultaDebitosFields.codSeguranca = (string)stepContext.Result;

            VehicleSituacao situacao = new VehicleSituacao();

            var webresult = await situacao.ObterSituacaoVeiculo(ConsultaDebitosFields.plataforma, ConsultaDebitosFields.renavam, ConsultaDebitosFields.codSeguranca);

            ConsultaDebitosFields.totalregistros = webresult.totRegistros;
            ConsultaDebitosFields.vetDescDebitos = webresult.vetDescDebitos;
            ConsultaDebitosFields.vetValorCotaUnica = webresult.vetValorCotaunica;

            if (ConsultaDebitosFields.totalregistros == 0 || ConsultaDebitosFields.vetValorCotaUnica.Sum() == 0)
            {
                await stepContext.Context.SendActivityAsync("Não há débitos débitos pendentes para este veículo");
                return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
            }


            List<string> debitos = new List<string>();
            foreach (string value in ConsultaDebitosFields.vetDescDebitos)
            {
                if (value != null && value != "")
                {
                    debitos.Add(value);
                }
            }

            List<string> valor = new List<string>();
            foreach (decimal value in ConsultaDebitosFields.vetValorCotaUnica)
            {
                if (value != 0)
                {
                    valor.Add(value.ToString());
                }
            }

            await stepContext.Context.SendActivityAsync("**Resultado da consulta de situação do veículo**");
            for (int i = 0; i < ConsultaDebitosFields.totalregistros; i++)
            {
                await stepContext.Context.SendActivityAsync($"{debitos[i]}:     R$ {valor[i]}");
            }

            await stepContext.Context.SendActivityAsync($"**TOTAL:** **R$** **{ConsultaDebitosFields.vetValorCotaUnica.Sum()}**");

            return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
        }

    }
}