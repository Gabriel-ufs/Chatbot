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
    /// Diálogo raiz de geração de CRLVe.
    /// </summary>
    public class RootCRLVeDialog : CancelAndHelpDialog
    {
        CRLVeFields CRLVeFields;

        public RootCRLVeDialog()
            : base(nameof(RootCRLVeDialog))
        {

            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt), null, "pt-br"));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(new RNTRCDialog());
            AddDialog(new PlacaDialog());
            AddDialog(new SpecificationsDialog());
            AddDialog(new SecureCodeCRLVeDialog());
      
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
        /// Passo responsável por captar o contexto do diálogo pai (MainDialog), apresentar introdução do serviço de Emissão de CRLVe
        /// e perguntar se o usuário deseja continuar.
        /// </summary>
        /// <param name="stepContext">Contexto do RootCRLVeDialog</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<DialogTurnResult> OptionStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // Busca pelo contexto do diálogo pai.
            WaterfallStepContext contextParent = (WaterfallStepContext)stepContext.Parent;
            CRLVeFields = new CRLVeFields();
            CRLVeFields = (CRLVeFields)contextParent.Values["CRLVeFields"];
            stepContext.Values["CRLVeFields"] = CRLVeFields;

            await stepContext.Context.SendActivityAsync("**Bem-vindo ao serviço de emissão do Documento de Circulação!**");
            await stepContext.Context.SendActivityAsync("Aqui você pode emitir o Documento de Circulação de Porte Obrigatório (CRLV-e).");
            await stepContext.Context.SendActivityAsync("A qualquer momento, você pode digitar **CANCELAR** para parar o processo e retornar ao menu de opções");

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
        /// <param name="stepContext">Contexto do RootCRLVeDialog</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<DialogTurnResult> OptionValidationStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            CRLVeFields = (CRLVeFields)stepContext.Values["CRLVeFields"];

            stepContext.Values["choice"] = ((FoundChoice)stepContext.Result).Value;
            if (stepContext.Values["choice"].ToString().ToLower() == "sim") return await stepContext.ContinueDialogAsync(cancellationToken);
            else return await stepContext.ReplaceDialogAsync(nameof(MainDialog), default, cancellationToken);
        }

        /// <summary>
        /// Passo responsável por dar início ao serviço de Emissão de CRLVe solicitando se o usuário pode informar o código de segurança.
        /// </summary>
        /// <param name="stepContext">Contexto do RootCRLVeDialog</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<DialogTurnResult> SecureCodeQuestionStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var text = "Para iniciarmos o processo de emissão do Documento de Circulação de Porte Obrigatório (CRLV-e), vou precisar de algumas informações.";
            await stepContext.Context.SendActivityAsync(MessageFactory.Text(text), cancellationToken);

            // Geração da imagem do CRLVe
            ImageCard ImageCard = new ImageCard();
            await stepContext.Context.SendActivityAsync(ImageCard.addImageCardSecureCode(), cancellationToken);

            // Geração de botão com link
            ButtonCard expandButton = new ButtonCard();
            await stepContext.Context.SendActivityAsync(expandButton.addButtonExpandSecureCode(CRLVeFields), cancellationToken);

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
        /// <param name="stepContext">Contexto do RootCRLVeDialog</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<DialogTurnResult> SecureCodeStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            CRLVeFields = (CRLVeFields)stepContext.Values["CRLVeFields"];
            stepContext.Values["choice"] = ((FoundChoice)stepContext.Result).Value;

            if (stepContext.Values["choice"].ToString().ToLower() == "sim") return await stepContext.BeginDialogAsync(nameof(SecureCodeCRLVeDialog), CRLVeFields, cancellationToken);
            else return await stepContext.ReplaceDialogAsync(nameof(PlacaDialog), CRLVeFields, cancellationToken);
        }
    }

}
