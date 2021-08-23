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
using Refit;
using CoreBot.Models.MethodsValidation.License;
using CoreBot.Models;
using CoreBot.Services.WSDLService;

namespace Microsoft.BotBuilderSamples.Dialogs
{
    public class AddressConfirm : CancelAndHelpDialog
    {
        RenovationFields RenovationFields;

        public AddressConfirm()
        : base(nameof(AddressConfirm))

        {

            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt), null, "pt-br"));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(new PortalDialog());
            AddDialog(new LocalChoiceDialog());
            AddDialog(new AddressData());


            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                askCep,
                StepAuthCep,
                //ConfimationAddress,
                //ValidationAddress,

            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);

        }

        /// <summary>
        /// Passo responsável por captar o contexto do diálogo pai (EmissaoDialog), e perguntar ao usuário o CEP para atualização de endereço
        /// </summary>
        /// <param name="stepContext">Contexto do RootRenovationDialog</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<DialogTurnResult> askCep (WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            WaterfallStepContext contextParent = (WaterfallStepContext)stepContext.Parent;
            RenovationFields = new RenovationFields();
            RenovationFields = (RenovationFields)contextParent.Values["RenovationFields"];
            stepContext.Values["RenovationFields"] = RenovationFields;

            await stepContext.Context.SendActivityAsync("Por favor, informe o seu CEP sem caracteres especiais (EX: 00000000)");

            var cep = MessageFactory.Text(null, InputHints.ExpectingInput);

            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = cep }, cancellationToken);

        }

        /// <summary>
        /// Passo responsável por receber o CEP informado no passo anterior e acessar o webService coletando as informações referentes ao CEP informado e exibe as iformações para usuário
        /// logo depois chama o diálogo AddressData
        /// </summary>
        /// <param name="stepContext">Contexto do RootRenovationDialog</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<DialogTurnResult> StepAuthCep(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var RenovationFields = (RenovationFields)stepContext.Values["RenovationFields"];
            RenovationFields.cep = (string)stepContext.Result;

            if (RenovationFields.IsNumeric(RenovationFields.cep) == false || RenovationFields.cep.Length != 8 && RenovationFields.cepCont < 3)
            {
                RenovationFields.cepCont++;
                await stepContext.Context.SendActivityAsync("CEP invalido, confira se o CEP informado está correto e tente novamente");
                return await stepContext.ReplaceDialogAsync(nameof(AddressConfirm), default, cancellationToken);
            }

            if(RenovationFields.cepCont >= 3)
            {
                await stepContext.Context.SendActivityAsync("Endereço referente ao CEP não encontrado, por favor acesse o portal através do link abaixo para que o senhor(a) possa fazer a atualização de endereço manualmente");
                await stepContext.Context.SendActivityAsync("https://www.detran.se.gov.br/portal/?pg=cnh_renovacao");
                return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
            }

            CnhRenovation cep = new CnhRenovation();

            var webResult = await cep.obterCEP(RenovationFields.plataforma,Convert.ToInt32(RenovationFields.cep));

            RenovationFields.TipoEndereco = webResult.tipo_logradouro;
            RenovationFields.endereco = webResult.logradouro;
            RenovationFields.bairro = webResult.bairro;
            RenovationFields.municipio = webResult.municipio;


            if (webResult.tipo_logradouro == "" && webResult.logradouro == "" && RenovationFields.cepCont < 3)
            {
                RenovationFields.cepCont++;
                await stepContext.Context.SendActivityAsync("CEP não encontrado, confira se o CEP informado está correto e tente novamente");
                return await stepContext.ReplaceDialogAsync(nameof(AddressConfirm), default, cancellationToken);
            }

            else if(webResult == null)
            {
                RenovationFields.cepCont++;
                await stepContext.Context.SendActivityAsync("CEP não encontrado, confira se o CEP informado está correto e tente novamente");
                return await stepContext.ReplaceDialogAsync(nameof(AddressConfirm), default, cancellationToken);
            }

            else if(RenovationFields.TipoEndereco == "" && RenovationFields.endereco == "" && RenovationFields.cepCont >= 3)
            {
                await stepContext.Context.SendActivityAsync("Endereço referente ao CEP não encontrado, por favor acesse o portal através do link abaixo para realizar a atualização do seu endereço manualmente");
                await stepContext.Context.SendActivityAsync("https://www.detran.se.gov.br/portal/?pg=cnh_renovacao");
                return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
            }

            else
            {
                await stepContext.Context.SendActivityAsync($"Logradouro: {RenovationFields.TipoEndereco} {RenovationFields.endereco} \r\nBairro: {RenovationFields.bairro} \r\nCidade: {RenovationFields.municipio}");

                return await stepContext.BeginDialogAsync(nameof(AddressData), RenovationFields, cancellationToken);
            }

        }

    }
}