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
using ValidacoesLibrary;

namespace Microsoft.BotBuilderSamples.Dialogs
{
    public class Phone : CancelAndHelpDialog
    {
        RenovationFields RenovationFields;

        public Phone()
        : base(nameof(Phone))

        {

            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt), null, "pt-br"));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(new PortalDialog());
            AddDialog(new Email());


            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                PhoneRequest,
                PhoneValidation,
                

            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);

        }

        /// <summary>
        /// Passo responsável por captar o contexto do diálogo pai (AddrressData), e perguntar o número de telefone dele 
        /// </summary>
        /// <param name="stepContext">Contexto do RootRenovationDialog</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// 
        private async Task<DialogTurnResult> PhoneRequest(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            WaterfallStepContext contextParent = (WaterfallStepContext)stepContext.Parent;
            RenovationFields = new RenovationFields();
            RenovationFields = (RenovationFields)contextParent.Values["RenovationFields"];
            stepContext.Values["RenovationFields"] = RenovationFields;

            await stepContext.Context.SendActivityAsync("Por favor,informe o seu número de telefone com o DDD, sem o 0 (EX: 79999999999)");

            var phone = MessageFactory.Text(null, InputHints.ExpectingInput);

            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = phone }, cancellationToken);

        }

        /// <summary>
        /// Passo responsável por receber o numero de telefone informado no passo anterior e valida-lo caso seja válido inicia o dialogo Email 
        /// caso seja invalido destroi o contexto atual e reinicia o dialogo Phone
        /// </summary>
        /// <param name="stepContext">Contexto do RootRenovationDialog</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<DialogTurnResult> PhoneValidation (WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var RenovationFields = (RenovationFields)stepContext.Values["RenovationFields"];
            RenovationFields.telefone = (string)stepContext.Result;

            var validarPhone = new Validacoes();


            if (validarPhone.ValidarPhone(RenovationFields.telefone))
            {
                RenovationFields.ddd = RenovationFields.telefone.Substring(0, 2);
                RenovationFields.telefone = RenovationFields.telefone.Substring(2, 9);

                return await stepContext.BeginDialogAsync(nameof(Email), RenovationFields, cancellationToken);
            }

            else if (validarPhone.ValidarPhone(RenovationFields.telefone) == false && RenovationFields.contPhone < 3)
            {
                RenovationFields.contPhone++;
                await stepContext.Context.SendActivityAsync("Número de telefone inválido, favor informar um número válido");
                return await stepContext.ReplaceDialogAsync(nameof(Phone), RenovationFields, cancellationToken);
            }

            else
            {
                await stepContext.Context.SendActivityAsync("O número de telefone informado é inválido. Verifique e tente novamente");
                return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
            }
            
        }
    }
}