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

namespace Microsoft.BotBuilderSamples.Dialogs
{
    public class AddressData : CancelAndHelpDialog
    {
        RenovationFields RenovationFields;

        public AddressData()
        : base(nameof(AddressData))

        {

            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt), null, "pt-br"));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(new PortalDialog());
            AddDialog(new Phone());


            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                NumResquest,
                ValidationNumResquest,
                ComplementRequest,
                StepAsync,

            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);

        }

        /// <summary>
        /// Passo responsável por captar o contexto do diálogo pai (AddressConfirm), e perguntar ao o número da residencia dele
        /// </summary>
        /// <param name="stepContext">Contexto do RootRenovationDialog</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// 
        private async Task<DialogTurnResult> NumResquest(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            WaterfallStepContext contextParent = (WaterfallStepContext)stepContext.Parent;
            RenovationFields = new RenovationFields();
            RenovationFields = (RenovationFields)contextParent.Values["RenovationFields"];
            stepContext.Values["RenovationFields"] = RenovationFields;


            await stepContext.Context.SendActivityAsync("Por favor, informe o número da sua residência, caso não haja número digite s/n");

            var num = MessageFactory.Text(null, InputHints.ExpectingInput);
            

            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = num }, cancellationToken);
            
        }

        private async Task<DialogTurnResult> ValidationNumResquest(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var RenovationFields = (RenovationFields)stepContext.Values["RenovationFields"];
            RenovationFields.numeroEndereco = (string)stepContext.Result;

            if (RenovationFields.numeroEndereco.ToUpper() == "S/N" || RenovationFields.IsNumeric(RenovationFields.numeroEndereco))
            {
                return await stepContext.ContinueDialogAsync(cancellationToken);
            }

            else if (RenovationFields.contNumEnd < 3)
            {
                RenovationFields.contNumEnd++;
                await stepContext.Context.SendActivityAsync("Número de endereço inválido, por favor informe um número válido");
                return await stepContext.ReplaceDialogAsync(nameof(AddressData), RenovationFields, cancellationToken);
            }
            else
            {
                await stepContext.Context.SendActivityAsync("O formato do número de endereço é inválido. Verifique o número residencial e tente novamente");
                return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
            }
        }

        /// <summary>
        /// Passo responsável por perguntar ao usuário o complemento do endereço dele
        /// </summary>
        /// <param name="stepContext">Contexto do RootRenovationDialog</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<DialogTurnResult> ComplementRequest (WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            
            await stepContext.Context.SendActivityAsync("Por favor, informe o complemento do seu endereço, caso não haja basta digitar 'nenhum'");

            var complemento = MessageFactory.Text(null, InputHints.ExpectingInput);

            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = complemento }, cancellationToken);
        }
        /// <summary>
        /// Passo responsável por receber o resultado do passoa anterior e chamar o dialogo Phone
        /// </summary>
        /// <param name="stepContext">Contexto do RootRenovationDialog</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// 
        private async Task<DialogTurnResult> StepAsync (WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var RenovationFields = (RenovationFields)stepContext.Values["RenovationFields"];

            if ((string)stepContext.Result.ToString().ToLower() == "nenhum")
            {
                RenovationFields.complemento = "";
            }

            else
            {
                RenovationFields.complemento = (string)stepContext.Result;
            }

            return await stepContext.BeginDialogAsync(nameof(Phone),RenovationFields, cancellationToken);
        }

    }
}