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
using Microsoft.Bot.Schema;

namespace Microsoft.BotBuilderSamples.Dialogs
{
    /// <summary>
    /// Diálogo final.
    /// </summary>
    public class FinalDialog : CancelAndHelpDialog
    {
        public FinalDialog()
            : base(nameof(FinalDialog))
        {

            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt), null, "pt-br"));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                AskStepAsync,
                FinalStepAsync,
                //AvaliationStepAsync,
            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        /// <summary>
        /// Passo responsável por questionar o usuário se deseja algo mais. É chamado ao finalizar os serviços já realizados.
        /// </summary>
        /// <param name="stepContext">Variável contendo o contexto do MainDialog.</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<DialogTurnResult> AskStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // Adição do efeito de digitação com delay de 1s.
            await stepContext.Context.SendActivitiesAsync(new Activity[]
           {
                new Activity { Type = ActivityTypes.Typing },
                new Activity { Type = "delay", Value = 1000},
           }, cancellationToken);

            var promptOptions = new PromptOptions
            {
                Prompt = MessageFactory.Text(TextGlobal.Ajuda + TextGlobal.Choice),
                RetryPrompt = MessageFactory.Text(TextGlobal.Desculpe + TextGlobal.Ajuda + TextGlobal.ChoiceDig),
                Choices = ChoiceFactory.ToChoices(new List<string> { "Sim", "Não" }),
            };

            return await stepContext.PromptAsync(nameof(ChoicePrompt), promptOptions, cancellationToken);
        }

        /// <summary>
        /// Passo responsável por receber a resposta do passo AskStepAsync, gerar mensagem de agradecimento e encerrar a conversação.
        /// </summary>
        /// <param name="stepContext">Variável contendo o contexto do MainDialog.</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["choice"] = ((FoundChoice)stepContext.Result).Value;

            if (stepContext.Values["choice"].ToString().ToLower() == "sim")
            {
                return await stepContext.ReplaceDialogAsync(nameof(MainDialog), cancellationToken);
            }
            else
            {
                await stepContext.Context.SendActivityAsync(MessageFactory.Text(TextGlobal.Agradecimento, InputHints.IgnoringInput), cancellationToken);
                return await stepContext.EndDialogAsync(cancellationToken);
                //var promptOptions = new PromptOptions
                //{
                //    Prompt = MessageFactory.Text($"De 1 a 5, qual nota você daria para meu atendimento?"),
                //    Choices = ChoiceFactory.ToChoices(new List<string> { "1 - Péssimo", "2 - Ruim", "3 - Regular", "4 - Bom", "5 - Excelente" }),
                //};
                //return await stepContext.PromptAsync(nameof(ChoicePrompt), promptOptions, cancellationToken);
            }

            //if (stepContext.Result != null)
            //{
            //    var result = stepContext.Result;
            //    await stepContext.Context.SendActivityAsync(MessageFactory.Text(result.ToString()), cancellationToken);
            //}
            //else
            //{
            //await stepContext.Context.SendActivityAsync(MessageFactory.Text("Obrigada."), cancellationToken);
            //}
        }

        /// <summary>
        /// Passo responsável por receber a avaliação do usuário. Não utilizada nesta versão.
        /// </summary>
        /// <param name="stepContext">Variável contendo o contexto do MainDialog.</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<DialogTurnResult> AvaliationStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["choice"] = ((FoundChoice)stepContext.Result).Value;

            await stepContext.Context.SendActivityAsync(MessageFactory.Text(TextGlobal.Agradecimento), cancellationToken);
            return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
        }
    }

}
