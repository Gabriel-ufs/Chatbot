using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CoreBot.Fields;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using CoreBot.Models;
using System;

namespace Microsoft.BotBuilderSamples.Dialogs
{
    public class LocalChoiceDialog : CancelAndHelpDialog
    {
        RenovationFields RenovationFields;

        public LocalChoiceDialog()
        : base(nameof(LocalChoiceDialog))

        {

            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt), null, "pt-br"));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(new MailAndPayDialog());

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                EmailAutStep,
                OptionValidationEmailAut,
                ShowPlacesStep,
                DeclarationDeficient,
                OptionStepAsync,

            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        /// <summary>
        /// Passo responsável por captar o contexto do diálogo pai (Email), e perguntar ao usuário se ele deseja receber emails com as informações referentes ao andamento do seu processo de renovação da CNH
        /// </summary>
        /// <param name="stepContext">Contexto do Consult</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        
        private async Task<DialogTurnResult> EmailAutStep(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            WaterfallStepContext contextParent = (WaterfallStepContext)stepContext.Parent;
            RenovationFields = new RenovationFields();
            RenovationFields = (RenovationFields)contextParent.Values["RenovationFields"];
            stepContext.Values["RenovationFields"] = RenovationFields;

            var promptOptions = new PromptOptions
            {
                Prompt = MessageFactory.Text("Você deseja receber informações sobre o andamento do processo em seu e-mail? \r\n1. Sim, quero receber emails com atualizações \r\r2. Não, não quero receber e-mail algum"),
                RetryPrompt = MessageFactory.Text(TextGlobal.Desculpe + "Você deseja receber informações sobre o andamento do processo em seu e-mail?"),
                Choices = ChoiceFactory.ToChoices(new List<string> { "Sim", "Não" }),
            };

            return await stepContext.PromptAsync(nameof(ChoicePrompt), promptOptions, cancellationToken);
        }


        private async Task<DialogTurnResult> OptionValidationEmailAut(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            RenovationFields = (RenovationFields)stepContext.Values["RenovationFields"];
            RenovationFields.emailAut = ((FoundChoice)stepContext.Result).Value;

            if (RenovationFields.emailAut.ToLower() == "sim")
            {
                RenovationFields.emailAut = "S";
            }
            else
            {
                RenovationFields.emailAut = "N";
            }


            return await stepContext.ContinueDialogAsync(cancellationToken);
        }

        /// <summary>
        /// Passo responsável por perguntar ao usuário qual o local ele deseja realizar seus exames e provas
        /// </summary>
        /// <param name="stepContext">Contexto do Consult</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>

        private async Task<DialogTurnResult> ShowPlacesStep (WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {


            await stepContext.Context.SendActivityAsync("Escolha a seguir o local para a realização de exames médicos/psicológicos e/ou provas teóricas e/ou de direção veicular.");
            await stepContext.Context.SendActivityAsync("ATENÇÃO! As provas de direção veícular da localidade de Nossa Senhora do Socorro, serão realizadas em Aracaju.");


            var options = RenovationFields.vetDescricaoLocal;

           

            var promptOptions = new PromptOptions
            {
                Prompt = MessageFactory.Text("Escolha entre: "),
                RetryPrompt = MessageFactory.Text(TextGlobal.Desculpe + "Escolha entre (digite um número de 1 a 9): "),
                Choices = ChoiceFactory.ToChoices(options),
            };


            return await stepContext.PromptAsync(nameof(ChoicePrompt), promptOptions, cancellationToken);

        }


        /// <summary>
        /// Passo responsável por captar o valor da resposta anterior e por perguntar ao usuário se ele possui algum tipo de deficiência
        /// </summary>
        /// <param name="stepContext">Contexto do Consult</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>

        private async Task<DialogTurnResult> DeclarationDeficient (WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            RenovationFields = (RenovationFields)stepContext.Values["RenovationFields"];
            RenovationFields.localProve = ((FoundChoice)stepContext.Result).Value;

            int index = Array.IndexOf(RenovationFields.vetDescricaoLocal, RenovationFields.localProve);
            RenovationFields.SetorVirtual = RenovationFields.vetCodigoLocal[index];
            

            var promptOptions = new PromptOptions
            {
                Prompt = MessageFactory.Text("O senhor(a) possui alguma deficiência?  \r\n  1. Sim   \r\n  2. Não"),
                RetryPrompt = MessageFactory.Text(TextGlobal.Desculpe + "O senhor(a) possui alguma deficiência? \r\n  1. Sim   \r\n  2. Não"),
                Choices = ChoiceFactory.ToChoices(new List<string> { "Sim", "Não" }),
            };

            return await stepContext.PromptAsync(nameof(ChoicePrompt), promptOptions, cancellationToken);
        }


        private async Task<DialogTurnResult> OptionStepAsync (WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            RenovationFields = (RenovationFields)stepContext.Values["RenovationFields"];
            RenovationFields.deficienciaFisica = ((FoundChoice)stepContext.Result).Value;

            if (RenovationFields.deficienciaFisica.ToLower() == "sim")
            {
                RenovationFields.deficienciaFisica = "S";
            }
            else
            {
                RenovationFields.deficienciaFisica = "N";
            }


            return await stepContext.BeginDialogAsync(nameof(MailAndPayDialog), RenovationFields, cancellationToken);

        }


    }
}