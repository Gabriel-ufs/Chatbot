// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CoreBot.Fields;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;

namespace Microsoft.BotBuilderSamples.Dialogs
{
    /// <summary>
    /// Diálogo raiz de geração de CRLVe.
    /// </summary>
    public class RootConsultChoice : CancelAndHelpDialog
    {
        ConsultFields ConsultFields;


        public RootConsultChoice()
            : base(nameof(RootConsultChoice))
        {


            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt), null, "pt-br"));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(new RootConsultDialog());
            AddDialog(new RootConsultDialog1());


            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                ChoiceStepAsync,
                ValidationChoiceAsync,

            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }




        private async Task<DialogTurnResult> ChoiceStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // Busca pelo contexto do diálogo pai.
            WaterfallStepContext contextParent = (WaterfallStepContext)stepContext.Parent;
            ConsultFields = new ConsultFields();
            ConsultFields = (ConsultFields)contextParent.Values["ConsultFields"];
            stepContext.Values["ConsultFields"] = ConsultFields;

            await stepContext.Context.SendActivityAsync("**Bem-vindo ao serviço de Consulta de Dados de habilitação!**");
            await stepContext.Context.SendActivityAsync("Aqui você pode visualizar as informações referentes aos Dados do documento de habilitação ou Dados do Processo Suspensão/Cassação do Direito de Dirigir");
            await stepContext.Context.SendActivityAsync("A qualquer momento, você pode digitar **CANCELAR** para parar o processo e retornar ao menu de opções");

            var promptOptions = new PromptOptions
            {
                Prompt = MessageFactory.Text("Por favor, escolha entre: "),
                RetryPrompt = MessageFactory.Text(TextGlobal.Desculpe + "Escolha as opções 1 ou 2, por favor"),
                Choices = ChoiceFactory.ToChoices(new List<string> { "Dados do documento de habilitação", "Dados do Processo Suspensão/Cassação do Direito de Dirigir" }),
            };

            return await stepContext.PromptAsync(nameof(ChoicePrompt), promptOptions, cancellationToken);
        }




        private async Task<DialogTurnResult> ValidationChoiceAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            ConsultFields = (ConsultFields)stepContext.Values["ConsultFields"];

            stepContext.Values["choice"] = ((FoundChoice)stepContext.Result).Value;

            if (stepContext.Values["choice"].ToString().ToLower() == "dados do processo suspensão/cassação do direito de dirigir")
            {
                stepContext.Values["ConsultFields"] = ConsultFields;
                return await stepContext.BeginDialogAsync(nameof(RootConsultDialog), ConsultFields, cancellationToken);
            }

            else if (stepContext.Values["choice"].ToString().ToLower() == "dados do documento de habilitação")
            {
                stepContext.Values["ConsultFields"] = ConsultFields;
                return await stepContext.BeginDialogAsync(nameof(RootConsultDialog1), ConsultFields, cancellationToken);
            }
            else
            {
                return await stepContext.ReplaceDialogAsync(nameof(MainDialog), default, cancellationToken);
            }
        }

    }
}
