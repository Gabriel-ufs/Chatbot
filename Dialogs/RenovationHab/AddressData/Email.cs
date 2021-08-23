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
using System.Text.RegularExpressions;
using ValidacoesLibrary;

namespace Microsoft.BotBuilderSamples.Dialogs
{
    public class Email : CancelAndHelpDialog
    {
        RenovationFields RenovationFields;

        public Email()
        : base(nameof(Email))

        {

            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt), null, "pt-br"));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(new PortalDialog());
            AddDialog(new LocalChoiceDialog());


            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                EmailRequest,
                ValidationEmail,
                ConfimationAddress,
                ValidationData,

            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);

        }

        /// <summary>
        /// Passo responsável por captar o contexto do diálogo pai (Phone), e perguntar o endereço de Email dele
        /// </summary>
        /// <param name="stepContext">Contexto do RootRenovationDialog</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        
        private async Task<DialogTurnResult> EmailRequest (WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            WaterfallStepContext contextParent = (WaterfallStepContext)stepContext.Parent;
            RenovationFields = new RenovationFields();
            RenovationFields = (RenovationFields)contextParent.Values["RenovationFields"];
            stepContext.Values["RenovationFields"] = RenovationFields;

            await stepContext.Context.SendActivityAsync("Por favor, informe o seu endereço de Email ");

            var email = MessageFactory.Text(null, InputHints.ExpectingInput);

            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = email }, cancellationToken);
        }

        private async Task<DialogTurnResult> ValidationEmail(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var RenovationFields = (RenovationFields)stepContext.Values["RenovationFields"];
            RenovationFields.email = (string)stepContext.Result;


            var validarEmail = new Validacoes();

            if (validarEmail.ValidarEmail(RenovationFields.email))
            {
                return await stepContext.ContinueDialogAsync(cancellationToken);
            }
            else if (validarEmail.ValidarEmail(RenovationFields.email) == false && RenovationFields.contEmail < 3)
            {
                RenovationFields.contEmail++;
                await stepContext.Context.SendActivityAsync("Endereço de E-mail inválido, favor informar um E-mail válido");
                return await stepContext.ReplaceDialogAsync(nameof(Email), RenovationFields, cancellationToken);
            }
            else
            {
                await stepContext.Context.SendActivityAsync("O endereço de E-mail é inválido. Verifique e tente novamente");
                return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
            }
        }

        /// <summary>
        /// Passo responsável por receber o endereço de email informado anteriormente e pedir a confirmação dos dados informado pelo usuário
        /// </summary>
        /// <param name="stepContext">Contexto do RootRenovationDialog</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<DialogTurnResult> ConfimationAddress(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {

            var RenovationFields = (RenovationFields)stepContext.Values["RenovationFields"];


            await stepContext.Context.SendActivityAsync("Por favor, confira se seus dados estão corretos");
            await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Numero residencial: {RenovationFields.numeroEndereco} " +
                                                                        $"\r\n Complemento: {RenovationFields.complemento} " +
                                                                        $"\r\n Telefone: {RenovationFields.telefone}" +
                                                                        $"\r\n Email: {RenovationFields.email}"));

            var promptOptions = new PromptOptions
            {
                Prompt = MessageFactory.Text("Seus dados estão todos corretos? \r\n   1. Sim, estão todos corretos(Continuar) \r\n   2. Não, preciso corrigi-los (Preencher dados novamente)"),
                RetryPrompt = MessageFactory.Text(TextGlobal.Desculpe + "Por favor, se seus dados estão corretos \r\n 1. Sim \r\n 2. Não"),
                Choices = ChoiceFactory.ToChoices(new List<string> { "Sim", "Não" }),
            };

            return await stepContext.PromptAsync(nameof(ChoicePrompt), promptOptions, cancellationToken);

        }

        /// <summary>
        /// Passo responsável por receber a confirmação do passo anterior caso seja confirmado inicia o dialogo LocalChoiceDialog
        /// caso nao seja reinicia a o dialogo AddressData
        /// </summary>
        /// <param name="stepContext">Contexto do RootRenovationDialog</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        
        private async Task<DialogTurnResult> ValidationData(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            RenovationFields = (RenovationFields)stepContext.Values["RenovationFields"];
            stepContext.Values["choice"] = ((FoundChoice)stepContext.Result).Value;


            if (stepContext.Values["choice"].ToString().ToLower() == "sim")
            {
                stepContext.Values["RenovationFields"] = RenovationFields;
                return await stepContext.BeginDialogAsync(nameof(LocalChoiceDialog), RenovationFields, cancellationToken);
            }

            else return await stepContext.ReplaceDialogAsync(nameof(AddressData), RenovationFields, cancellationToken);
        }

    }
}