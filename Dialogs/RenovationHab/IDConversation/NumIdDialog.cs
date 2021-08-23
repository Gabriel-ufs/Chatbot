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
using CoreBot.Models.Generate;
using CoreBot.Models;

namespace Microsoft.BotBuilderSamples.Dialogs
{
    public class NumIdDialog : CancelAndHelpDialog
    {
        RenovationFields RenovationFields;

        public NumIdDialog()
        : base(nameof(NumIdDialog))

        {

            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt), null, "pt-br"));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(new EmissaoDialog());

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                NumIdRequest,
                ValidationNumId,
                DigitoResquest,
                ValidationDigit,
            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);

        }
        /// <summary>
        /// Passo responsável por captar o contexto do diálogo pai (RootRenovationDialog), informar ao usuário a necessidade dos dados da identidade e perguntar o número da identidade
        /// </summary>
        /// <param name="stepContext">Contexto do RootRenovationDialog</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<DialogTurnResult> NumIdRequest (WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // Busca pelo contexto do diálogo pai.
            WaterfallStepContext contextParent = (WaterfallStepContext)stepContext.Parent;
            RenovationFields = new RenovationFields();
            RenovationFields = (RenovationFields)contextParent.Values["RenovationFields"];
            stepContext.Values["RenovationFields"] = RenovationFields;

            if(RenovationFields.txtNumId == 0)
            {
                RenovationFields.txtNumId++;
                await stepContext.Context.SendActivityAsync("Agora irei precisar das informações da sua identidade para podermos dar continuidade");
            }

            await stepContext.Context.SendActivityAsync("Por favor, me informe o número da sua identidade, sem o dígito verificador");

            var identidade = MessageFactory.Text(null, InputHints.ExpectingInput);

            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = identidade }, cancellationToken);
        }

        /// <summary>
        /// Passo responsável por receber o número da identidade e valida-lo, caso seja um valor valido o dialogo continua caso não será perguntado novamente a identidade do usuário
        /// </summary>
        /// <param name="stepContext">Contexto do RootRenovationDialog</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<DialogTurnResult> ValidationNumId (WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var RenovationFields = (RenovationFields)stepContext.Values["RenovationFields"];
            RenovationFields.identidade = (string)stepContext.Result;

            if (RenovationFields.IsNumeric(RenovationFields.identidade) && RenovationFields.identidade.Length > 3)
            {
                return await stepContext.ContinueDialogAsync(cancellationToken);
            }

            else if(RenovationFields.contID < 3)
            {
                RenovationFields.contID++;
                await stepContext.Context.SendActivityAsync("Por favor, digite um registro de identidade válido");
                return await stepContext.ReplaceDialogAsync(nameof(NumIdDialog), default, cancellationToken);
            }
            else
            {
                await stepContext.Context.SendActivityAsync("Número da identidade informado é inválido. Verifique seus dados e tente novamente.");
                return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
            }
        }

        /// <summary>
        /// Passo responsável por perguntar ao usuário o digito verificador referente a identidade informada
        /// </summary>
        /// <param name="stepContext">Contexto do RootRenovationDialog</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// 
        private async Task<DialogTurnResult> DigitoResquest (WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            await stepContext.Context.SendActivityAsync("Por favor, agora informe o dígito verificador da sua identidade");
            await stepContext.Context.SendActivityAsync("Caso sua identidade não possua dígito, basta digitar 'sd'");

            var digito = MessageFactory.Text(null, InputHints.ExpectingInput);

            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = digito}, cancellationToken);
        }

        /// <summary>
        /// Passo responsável por receber o valor do digito verificador e valida-lo caso seja valido é chamado o dialogo EmissaoDialog
        /// caso não seja o usuário irá informar o número da identidade novamente e o digito
        /// </summary>
        /// <param name="stepContext">Contexto do RootRenovationDialog</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// 
        private async Task<DialogTurnResult> ValidationDigit (WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var RenovationFields = (RenovationFields)stepContext.Values["RenovationFields"];
            RenovationFields.digitoID = (string)stepContext.Result;

            if((RenovationFields.IsNumeric(RenovationFields.digitoID) && RenovationFields.digitoID.Length <= 2) || RenovationFields.digitoID.ToLower() == "sd")
            {
                return await stepContext.BeginDialogAsync(nameof(EmissaoDialog), RenovationFields, cancellationToken);
            }

            else if (RenovationFields.contDigit < 3)
            {
                RenovationFields.contDigit++;
                await stepContext.Context.SendActivityAsync("Dígito da identidade informado é inválido. Por favor, digite corretamente.");
                return await stepContext.ReplaceDialogAsync(nameof(NumIdDialog), default, cancellationToken);
            }

            else
            {
                await stepContext.Context.SendActivityAsync("Dígito informado é inválido. Verifique seus dados e tente novamente.");
                return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
            }
        }


    }
}