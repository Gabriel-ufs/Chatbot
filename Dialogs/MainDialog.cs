// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CoreBot.Fields;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using CoreBot.Models;
using System;
using CoreBot.Services.WSDLService;

namespace Microsoft.BotBuilderSamples.Dialogs
{
    /// <summary>
    /// Diálogo raiz da aplicação.
    /// </summary>
    public class MainDialog : ComponentDialog
    {
        protected readonly IConfiguration Configuration;
        protected readonly ILogger Logger;

        public MainDialog(IConfiguration configuration, ILogger<MainDialog> logger)
            : base(nameof(MainDialog))
        {
            Configuration = configuration;
            Logger = logger;

            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt), null, "pt-PT"));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(new RootCRLVeDialog());
            AddDialog(new RootLicenseDialog());
            AddDialog(new RootQnaMakerDialog());
            AddDialog(new RootOthersServicesDialog());
            AddDialog(new RootConsultChoice());
            AddDialog(new VehicleMenuDialog());
            AddDialog(new HabilitacaoMenuDialog());
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                IntroStepAsync,
                ActStepAsync,
                AskStepAsync,
                FinalStepAsync,
                //AvaliationStepAsync,
            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        /// <summary>
        /// Introdução raiz da aplicação. Finaliza apresentando as opções de menu
        /// </summary>
        /// <param name="stepContext">Variável contendo o contexto do MainDialog.</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<DialogTurnResult> IntroStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {

            /*CnhRenovation cep = new CnhRenovation();

            var webResult = await cep.obterCEP("EMULATOR", 49030549);*/



            List<string> options = new List<string> {
                
                "Serviços de Veículo",
                "Serviços de Habilitação",
                "Dúvidas frequentes",
                "Nenhuma das alternativas"

            };

            // Caso o usuário escreva qualquer outro número ou somente letras, este passo se repetirá.
            var promptOptions = new PromptOptions
            {
                Prompt = MessageFactory.Text($"Com qual destas opções eu posso te ajudar? "),
                RetryPrompt = MessageFactory.Text("Por Favor, digite um numero de 1 a 4"),
                Choices = ChoiceFactory.ToChoices(options),

            };

            return await stepContext.PromptAsync(nameof(ChoicePrompt), promptOptions, cancellationToken);
        }

        /// <summary>
        /// Passo responsável por receber a resposta do passo IntroStepAsync e repassar para o diálogo desejado.
        /// </summary>
        /// <param name="stepContext">Variável contendo o contexto do MainDialog, capta a resposta do passo anterior.</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        
        private async Task<DialogTurnResult> ActStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // Criação de instância dos objetos utilizados durante todo o fluxo de diálogo.
            var LicenseFields = new LicenseFields();
            var CRLVeFields = new CRLVeFields();
            var ConsultFields = new ConsultFields();
            var MenuFields = new  MenuFields();

            // Recebimento da resposta do passo IntroStepAsync.
            stepContext.Values["LicenseFields"] = ((FoundChoice)stepContext.Result).Value;

            // Captação da plataforma em que o bot está sendo utilizado.
            MenuFields.plataforma = stepContext.Context.Activity.ChannelId.ToUpper();

            // Caso o usuário escreva algo maior que um número.
            if (stepContext.Context.Activity.Text.Length > 1)
            {
                await stepContext.Context.SendActivityAsync("Por favor, digite somente o número da opção desejada (De 1 a 4).");
                return await stepContext.ReplaceDialogAsync(nameof(MainDialog), default, cancellationToken);
            }
            else
            {
                //stepContext.FindDialog(nameof(RootLicenseDialog));

                switch (stepContext.Values["LicenseFields"].ToString().ToLower())
                {
                    case "nenhuma das alternativas":
                        return await stepContext.BeginDialogAsync(nameof(RootOthersServicesDialog), LicenseFields, cancellationToken);

                    case "serviços de habilitação":
                        stepContext.Values["MenuFields"] = MenuFields;
                        return await stepContext.BeginDialogAsync(nameof(HabilitacaoMenuDialog), MenuFields, cancellationToken);

                    case "serviços de veículo":
                        stepContext.Values["MenuFields"] = MenuFields;
                        return await stepContext.BeginDialogAsync(nameof(VehicleMenuDialog), MenuFields, cancellationToken);

                    case "dúvidas frequentes":
                        return await stepContext.BeginDialogAsync(nameof(RootQnaMakerDialog), CRLVeFields, cancellationToken);

                    default:
                        stepContext.Values["LicenseFields"] = LicenseFields;
                        var promptOption2 = new PromptOptions
                        {
                            RetryPrompt = MessageFactory.Text("")
                        };

                        return await stepContext.ReplaceDialogAsync(nameof(MainDialog), default, cancellationToken);
                }
            }
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
                return await stepContext.ReplaceDialogAsync(nameof(MainDialog), default, cancellationToken);
            }
            else
            {
                await stepContext.Context.SendActivityAsync(MessageFactory.Text(TextGlobal.Agradecimento, InputHints.IgnoringInput), cancellationToken);
                return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
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
