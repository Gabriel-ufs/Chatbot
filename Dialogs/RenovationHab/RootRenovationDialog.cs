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
using ValidacoesLibrary;

namespace Microsoft.BotBuilderSamples.Dialogs
{
    public class RootRenovationDialog : CancelAndHelpDialog
    {
        RenovationFields RenovationFields;

        public RootRenovationDialog()
        : base(nameof(RootRenovationDialog))

           {

            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt), null, "pt-br"));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(new NumIdDialog());

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                OptionStepAsync,
                OptionValidationStepAsync,
                RegisterRenewalData,
                ValidateRenewalData,
            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);

        }

        /// <summary>
        /// Passo responsável por captar o contexto do diálogo pai (MainDialog), apresentar introdução do serviço de Renovação de Habilitação
        /// e perguntar se o usuário deseja continuar.
        /// </summary>
        /// <param name="stepContext">Contexto do RootRenovationDialog</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>

        private async Task<DialogTurnResult> OptionStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // Busca pelo contexto do diálogo pai.
            WaterfallStepContext contextParent = (WaterfallStepContext)stepContext.Parent;
            RenovationFields = new RenovationFields();
            RenovationFields = (RenovationFields)contextParent.Values["RenovationFields"];
            stepContext.Values["RenovationFields"] = RenovationFields;



            if(RenovationFields.controler == 0)
            {
                RenovationFields.controler++;

                await stepContext.Context.SendActivityAsync("**Bem-vindo ao serviço de Renovação da Carteira Nacional de Habilitação - CNH!**");
                if (RenovationFields.banco == "banese")
                {
                    await stepContext.Context.SendActivityAsync("Aqui você pode dar entrada no seu processo de renovação da CNH e gerar o documento para pagar a sua renovação. \r\n" +
                                                                "O documento gerado aqui é o Documento de Arrecadação (DUA) (Pagável somente no BANESE) \r\n" +
                                                                "Compensação em 24 horas e sem custo adicional");
                }

                else
                {
                    await stepContext.Context.SendActivityAsync("Aqui você pode dar entrada no seu processo de renovação da CNH e gerar o documento para pagar a sua renovação. \r\n" +
                                                                "O documento gerado aqui é o Boleto bancário ou Ficha de Compensação \r\n" +
                                                                "(Pagável em qualquer banco, compensação em 4 dias ÚTEIS e custo adicional de R$2,00)");
                }


                await stepContext.Context.SendActivityAsync("Até às 18 horas, este documento será gerado para ser pago hoje. Após esse horário será gerado para ser pago no próximo dia útil.");
                await stepContext.Context.SendActivityAsync("A qualquer momento, você pode digitar **CANCELAR** para parar o processo e retornar ao menu de opções");
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
        /// <param name="stepContext">Contexto do RootRenovationDialog</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
    
        private async Task<DialogTurnResult> OptionValidationStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            RenovationFields = (RenovationFields)stepContext.Values["RenovationFields"];

            stepContext.Values["choice"] = ((FoundChoice)stepContext.Result).Value;
            if (stepContext.Values["choice"].ToString().ToLower() == "sim") return await stepContext.ContinueDialogAsync(cancellationToken);
            else return await stepContext.ReplaceDialogAsync(nameof(MainDialog), default, cancellationToken);
        }

        /// <summary>
        /// Passo responsável por informar a necessidade de dados e a sua digitação obrigatória.
        /// </summary>
        /// <param name="stepContext">Contexto do RootRenovationDialog</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>


        private async Task<DialogTurnResult> RegisterRenewalData (WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var text = "Para iniciarmos o processo Renovação de CNH vou precisar de algumas informações";
            await stepContext.Context.SendActivityAsync(MessageFactory.Text(text), cancellationToken);

            await stepContext.Context.SendActivityAsync(MessageFactory.Text("Por Favor, informe o seu CPF (SEM CARACTERES ESPECIAIS) \n EX: 00000000000"), cancellationToken);
            var register = MessageFactory.Text(null, InputHints.ExpectingInput);
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = register }, cancellationToken);
        }

        /// <summary>
        /// Passo responsável por validar  o CPF  informado pelo usuário, caso seja um valor válido o diálogo RenachDialog é chamado, caso não seja destrói o contexto atual e volta para o início de RootrenovationDialog.
        /// </summary>
        /// <param name="stepContext">Contexto do RootRenovationDialog</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>


        private async Task<DialogTurnResult> ValidateRenewalData(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var RenovationFields = (RenovationFields)stepContext.Values["RenovationFields"];
            RenovationFields.cpf = (string)stepContext.Result;

            if (Validacoes.ValidaCPF(RenovationFields.cpf))
            {
                return await stepContext.BeginDialogAsync(nameof(NumIdDialog), RenovationFields, cancellationToken);
            }

            else if (RenovationFields.contCpf < 3)
            {
                RenovationFields.contCpf++;

                await stepContext.Context.SendActivityAsync(MessageFactory.Text("CPF inválido."));
                return await stepContext.ReplaceDialogAsync(nameof(RootRenovationDialog), default, cancellationToken);
            }

            else
            {
                await stepContext.Context.SendActivityAsync(MessageFactory.Text("O CPF informado é inválido. Verifique seus dados e tente novamente"));
                return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
            }
        }

    }

}

