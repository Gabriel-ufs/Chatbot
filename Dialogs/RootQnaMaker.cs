// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdaptiveCards;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.AI.QnA;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;
using Newtonsoft.Json.Linq;

namespace Microsoft.BotBuilderSamples.Dialogs
{
    /// <summary>
    /// Diálogo responsável por realizar a ligação entre o bot com o QnA Maker.
    /// </summary>
    public class RootQnaMakerDialog : CancelAndHelpDialog
    {
        public RootQnaMakerDialog()
            : base(nameof(RootQnaMakerDialog))
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
                IntroStepAsync,
                QnaConnectStepAsync,
                QuestionStepAsync,
                AnswerStepAsync,
            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        /// <summary>
        /// Passo responsável por questionar o que o usuário deseja.
        /// </summary>
        /// <param name="stepContext"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<DialogTurnResult> IntroStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var question = new PromptOptions
            {
                Prompt = MessageFactory.Text($"Em que posso ajudá-lo?", InputHints.ExpectingInput)
            };

            return await stepContext.PromptAsync(nameof(TextPrompt), question, cancellationToken);
        }

        /// <summary>
        /// Passo responsável por receber a resposta do usuário.
        /// </summary>
        /// <param name="stepContext"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<DialogTurnResult> QnaConnectStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            //var answer = stepContext.Result.ToString();
            //await stepContext.Context.SendActivityAsync(MessageFactory.Text("Você disse: " + answer), cancellationToken);

            var qnaMaker = new QnAMaker(new QnAMakerEndpoint
            {
                KnowledgeBaseId = "823dec12-27f5-4c85-a1c3-ba28f865c21a",
                EndpointKey = "33174ef7-a35d-4d4c-a9ed-e81788312def",
                Host = "https://qnamakerbotdetran.azurewebsites.net/qnamaker"
            });

            var options = new QnAMakerOptions { Top = 1 };


            // The actual call to the QnA Maker service.
            try
            {
                var response = await qnaMaker.GetAnswersAsync(stepContext.Context, options);

                if (response != null && response.Length > 0)
                {
                    await stepContext.Context.SendActivityAsync(MessageFactory.Text(response[0].Answer), cancellationToken);
                }
                else
                {
                    await stepContext.Context.SendActivityAsync(MessageFactory.Text("No momento não sei como respondê-lo. Entre em contato com nossa equipe de atendimento!"), cancellationToken);
                }

                return await stepContext.ContinueDialogAsync(cancellationToken: cancellationToken);
            } catch
            {
                await stepContext.Context.SendActivityAsync(MessageFactory.Text("No momento não sei como respondê-lo. Entre em contato com nossa equipe de atendimento!"), cancellationToken);
                return await stepContext.ContinueDialogAsync(cancellationToken: cancellationToken);
            }
            
            
        }

        private async Task<DialogTurnResult> QuestionStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var promptOptions = new PromptOptions
            {
                Prompt = MessageFactory.Text($"Deseja fazer outra pergunta?"),
                RetryPrompt = MessageFactory.Text("Por favor, utilize 1 para SIM ou 2 para NÃO."),
                Choices = ChoiceFactory.ToChoices(new List<string> { "Sim", "Não" }),
            };

            return await stepContext.PromptAsync(nameof(ChoicePrompt), promptOptions, cancellationToken);
        }

        private async Task<DialogTurnResult> AnswerStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["choice"] = ((FoundChoice)stepContext.Result).Value;

            if (stepContext.Values["choice"].ToString().ToLower() == "sim")
                return await stepContext.ReplaceDialogAsync(nameof(RootQnaMakerDialog), default, cancellationToken);
            else return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
        }
    }

}
