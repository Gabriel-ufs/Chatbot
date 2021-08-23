// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CoreBot.Components.Widgets;
using CoreBot.Fields;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;

namespace Microsoft.BotBuilderSamples.Dialogs
{
    /// <summary>
    /// Diálogo raiz de Licenciamento.
    /// </summary>
    public class RootLicenseDialog : CancelAndHelpDialog
    {

        LicenseFields LicenseFields;
        public RootLicenseDialog()
            : base(nameof(RootLicenseDialog))
        {

            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt), null, "pt-BR"));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(new RNTRCDialog());
            AddDialog(new RenavamDialog());
            AddDialog(new SpecificationsDialog());
            AddDialog(new SecureCodeDialog());

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                OptionStepAsync,
                OptionValidationStepAsync,
                SecureCodeQuestionStepAsync,
                SecureCodeStepAsync,

            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        /// <summary>
        /// Passo responsável por captar o contexto do diálogo pai (MainDialog), apresentar introdução dos serviços de licenciamento
        /// e perguntar se o usuário deseja continuar.
        /// </summary>
        /// <param name="stepContext">Contexto do RootLicenseDialog</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<DialogTurnResult> OptionStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // Busca pelo contexto do diálogo pai.
            WaterfallStepContext contextParent = (WaterfallStepContext)stepContext.Parent;
            LicenseFields = new LicenseFields();
            LicenseFields = (LicenseFields)contextParent.Values["LicenseFields"];
            stepContext.Values["LicenseFields"] = LicenseFields;

            await stepContext.Context.SendActivityAsync("Bem-vindo ao serviço de Licenciamento Anual! " + Emojis.Veiculos.Carro + "\r\n");
            if (LicenseFields.Banco == "Banese")
            {
                await stepContext.Context.SendActivityAsync("Aqui você pode gerar o documento para pagar o licenciamento do seu veículo.\r\n" +
                                                            "O documento gerado aqui é o Documento de Arrecadação (DUA).");
            }
            else
            {
                await stepContext.Context.SendActivityAsync("Aqui você pode gerar o documento para pagar o licenciamento do seu veículo.\r\n" +
                                                            "O documento gerado aqui é o Boleto Bancário ou Ficha de Compensação. \r\n" +
                                                            "(Pagável em qualquer banco, compensação em 4 dias ÚTEIS e custo adicional de R$2,00)");
            }
            await stepContext.Context.SendActivityAsync("Até às 18 horas, este documento será gerado para ser pago hoje. Após esse horário será gerado para ser pago no próximo dia útil.");
            await stepContext.Context.SendActivityAsync("A qualquer momento, você pode digitar CANCELAR para parar o processo e retornar ao menu de opções");

            var promptOptions = new PromptOptions
            {
                Prompt = MessageFactory.Text(TextGlobal.Prosseguir),
                RetryPrompt = MessageFactory.Text(TextGlobal.Desculpe + Emojis.Rostos.SorrisoSuor + TextGlobal.Prosseguir),
                Choices = ChoiceFactory.ToChoices(new List<string> { "Sim", "Não" }),
            };

            return await stepContext.PromptAsync(nameof(ChoicePrompt), promptOptions, cancellationToken);
        }

        /// <summary>
        /// Passo responsável por receber a resposta obtida em OptionStepAsync. Caso seja "sim", continua para o próximo passo, senão destrói o contexto atual e reinicia MainDialog.
        /// </summary>
        /// <param name="stepContext">Contexto do RootLicenseDialog</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<DialogTurnResult> OptionValidationStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            LicenseFields = (LicenseFields)stepContext.Values["LicenseFields"];

            stepContext.Values["choice"] = ((FoundChoice)stepContext.Result).Value;
            if (stepContext.Values["choice"].ToString().ToLower() == "sim") return await stepContext.ContinueDialogAsync(cancellationToken);
            else return await stepContext.ReplaceDialogAsync(nameof(MainDialog), default, cancellationToken);
        }

        /// <summary>
        /// Passo responsável por dar início ao serviço de Licenciamento solicitando se o usuário pode informar o código de segurança.
        /// </summary>
        /// <param name="stepContext">Contexto do RootLicenseDialog</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<DialogTurnResult> SecureCodeQuestionStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Para iniciarmos o processo, vou precisar de algumas informações."), cancellationToken);

            // Geração da imagem do CRLVe
            ImageCard ImageCard = new ImageCard();
            await stepContext.Context.SendActivityAsync(ImageCard.addImageCardSecureCode(), cancellationToken);

            // Geração de botão com link
            ButtonCard expandButton = new ButtonCard();
            await stepContext.Context.SendActivityAsync(expandButton.addButtonExpandSecureCode(LicenseFields), cancellationToken);


            var promptOptions = new PromptOptions
            {
                Prompt = MessageFactory.Text($"Você pode informar o CÓDIGO DE SEGURANÇA?" + TextGlobal.Choice),
                RetryPrompt = MessageFactory.Text(TextGlobal.Desculpe + "Você pode informar o CÓDIGO DE SEGURANÇA?" + TextGlobal.ChoiceDig),
                Choices = ChoiceFactory.ToChoices(new List<string> { "Sim", "Não" }),
            };

            return await stepContext.PromptAsync(nameof(ChoicePrompt), promptOptions, cancellationToken);
        }

        /// <summary>
        /// Passo responsável por receber a resposta de SecureCodeQuestionStepAsync. 
        /// Caso "sim" inicia o diálogo SecureCodeDialog, senão inicia o diálogo RenavamDialog.
        /// </summary>
        /// <param name="stepContext">Contexto do RootLicenseDialog</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<DialogTurnResult> SecureCodeStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            LicenseFields = (LicenseFields)stepContext.Values["LicenseFields"];
            stepContext.Values["choice"] = ((FoundChoice)stepContext.Result).Value;

            if (stepContext.Values["choice"].ToString().ToLower() == "sim") 
                return await stepContext.BeginDialogAsync(nameof(SecureCodeDialog), LicenseFields, cancellationToken);
            else 
                return await stepContext.BeginDialogAsync(nameof(RenavamDialog), LicenseFields, cancellationToken);
        }
    }
}
