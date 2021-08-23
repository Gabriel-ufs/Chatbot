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
    /// Diálogo raiz da aplicação.
    /// </summary>
    public class HabilitacaoMenuDialog : ComponentDialog
    {
        MenuFields MenuFields;

        public HabilitacaoMenuDialog()
            : base(nameof(HabilitacaoMenuDialog))
        {

            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt), null, "pt-PT"));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(new RootLicenseDialog());
            AddDialog(new RootCRLVeDialog());
            AddDialog(new RootLicenseDialog());
            AddDialog(new RootRenovationDialog());
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                OptionsAsync,
                ChoiceStepAsync,
                ActStepAsync,

            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> OptionsAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {


            // Busca pelo contexto do diálogo pai.
            WaterfallStepContext contextParent = (WaterfallStepContext)stepContext.Parent;
            MenuFields = new MenuFields();
            MenuFields = (MenuFields)contextParent.Values["MenuFields"];
            stepContext.Values["MenuFields"] = MenuFields;

            await stepContext.Context.SendActivityAsync("**Bem-vindo ao Menu de serviços para habilitação!**");
            await stepContext.Context.SendActivityAsync("Aqui você pode escolher qual serviço relacionado a habilitação que você deseja realizar");
            await stepContext.Context.SendActivityAsync("A qualquer momento, você pode digitar **CANCELAR** para parar o processo e retornar ao menu de opções");

            return await stepContext.ContinueDialogAsync(cancellationToken);
        }


        private async Task<DialogTurnResult> ChoiceStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {

            List<string> options = new List<string> {

                "Consulta de processo",
                "Renovação de Habilitação (BANESE)",
                "Renovação de Habilitação (Outros bancos)",
                "Nenhuma das alternativas",
                
            };

            // Caso o usuário escreva qualquer outro número ou somente letras, este passo se repetirá.
            var promptOptions = new PromptOptions
            {
                Prompt = MessageFactory.Text($"Com qual destas opções eu posso te ajudar? "),
                RetryPrompt = MessageFactory.Text("Por Favor, digite um número de 1 a 3"),
                Choices = ChoiceFactory.ToChoices(options),

            };

            return await stepContext.PromptAsync(nameof(ChoicePrompt), promptOptions, cancellationToken);
        }


        private async Task<DialogTurnResult> ActStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // Criação de instância dos objetos utilizados durante todo o fluxo de diálogo.
            var LicenseFields = new LicenseFields();
            var CRLVeFields = new CRLVeFields();
            var ConsultFields = new ConsultFields();
            var RenovationFields = new RenovationFields();


            // Recebimento da resposta do passo IntroStepAsync.
            var MenuFields = (MenuFields)stepContext.Values["MenuFields"];
            stepContext.Values["menu"] = ((FoundChoice)stepContext.Result).Value;

            // renovação de habilitação
            stepContext.Values["RenovationFields"] = ((FoundChoice)stepContext.Result).Value;

            // Captação da plataforma em que o bot está sendo utilizado.
            ConsultFields.plataforma = stepContext.Context.Activity.ChannelId.ToUpper();
            RenovationFields.plataforma = stepContext.Context.Activity.ChannelId.ToUpper();


            if (stepContext.Context.Activity.Text.Length > 1)
            {
                await stepContext.Context.SendActivityAsync("Por favor, digite somente o número da opção desejada (De 1 a 3).");

                return await stepContext.ReplaceDialogAsync(nameof(MainDialog), default, cancellationToken);
            }
            else
            {

                switch (stepContext.Values["menu"].ToString().ToLower())
                {

                    case "consulta de processo":
                        stepContext.Values["ConsultFields"] = ConsultFields;
                        return await stepContext.BeginDialogAsync(nameof(RootConsultChoice), ConsultFields, cancellationToken);

                    case "nenhuma das alternativas":
                        return await stepContext.BeginDialogAsync(nameof(RootOthersServicesDialog), LicenseFields, cancellationToken);

                    case "renovação de habilitação (banese)":
                        RenovationFields.tipoDocumentoIn = "D";
                        RenovationFields.banco = "banese";
                        RenovationFields.txtTipoPagamento = "DOCUMENTO ÚNICO DE ARRECADAÇÃO - DUA (PAGÁVEL POR TODOS OS MEIOS DISPONIBILIZADOS PELO BANESE,COMPENSAÇÃO EM 24 HORAS E SEM CUSTO ADICIONAL)";
                        stepContext.Values["RenovationFields"] = RenovationFields;
                        return await stepContext.BeginDialogAsync(nameof(RootRenovationDialog), RenovationFields, cancellationToken);

                    case "renovação de habilitação (outros bancos)":
                        RenovationFields.tipoDocumentoIn = "F";
                        RenovationFields.banco = "outros bancos";
                        RenovationFields.txtTipoPagamento = "Ficha de Compensação - (Pagável em qualquer banco, compensação em 4 dias ÚTEIS e custo adicional de R$2,00)";
                        stepContext.Values["RenovationFields"] = RenovationFields;
                        return await stepContext.BeginDialogAsync(nameof(RootRenovationDialog), RenovationFields, cancellationToken);

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

        private static async Task<DialogTurnResult> NewMethod(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.ReplaceDialogAsync(nameof(MainDialog), default, cancellationToken);
        }
    }
}