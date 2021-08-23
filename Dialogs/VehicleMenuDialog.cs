// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CoreBot.Dialogs;
using CoreBot.Dialogs.Calendario;
using CoreBot.Fields;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;


namespace Microsoft.BotBuilderSamples.Dialogs
{
    /// <summary>
    /// Diálogo raiz da aplicação.
    /// </summary>
    public class VehicleMenuDialog : ComponentDialog
    {
        MenuFields MenuFields;

        public VehicleMenuDialog()
            : base(nameof(VehicleMenuDialog))
        {

            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt), null, "pt-PT"));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(new RootLicenseDialog());
            AddDialog(new RootCRLVeDialog());
            AddDialog(new RootLicenseDialog());
            AddDialog(new Calendario());
            AddDialog(new RootConsultaDeb());
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

            await stepContext.Context.SendActivityAsync("**Bem-vindo ao Menu de serviços para veículo!**");
            await stepContext.Context.SendActivityAsync("Aqui você pode escolher qual serviço relacionado a veículos que você deseja realizar");
            await stepContext.Context.SendActivityAsync("A qualquer momento, você pode digitar **CANCELAR** para parar o processo e retornar ao menu de opções");

            return await stepContext.ContinueDialogAsync(cancellationToken);
        }


        private async Task<DialogTurnResult> ChoiceStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {

            List<string> options = new List<string> {

                "Licenciamento Anual (BANESE)",
                "Licenciamento Anual (Outros Bancos)",
                "Emitir Documento de Circulação (CRLV-e)",
                "Consultar Data de Licenciamento",
                "Consultar Situação de Veículo (Débitos)",
                "Nenhuma das alternativas",
            };

            // Caso o usuário escreva qualquer outro número ou somente letras, este passo se repetirá.
            var promptOptions = new PromptOptions
            {
                Prompt = MessageFactory.Text($"Com qual destas opções eu posso te ajudar? "),
                RetryPrompt = MessageFactory.Text("Por Favor, digite um número de 1 a 5"),
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
            var CalendarioFields = new CalendarioFields();
            var ConsultaDebitosFields = new ConsultaDebitosFields();


            // Recebimento da resposta do passo IntroStepAsync.
            var MenuFields = (MenuFields)stepContext.Values["MenuFields"];
            stepContext.Values["menu"] = ((FoundChoice)stepContext.Result).Value;

            // Captação da plataforma em que o bot está sendo utilizado
            LicenseFields.plataforma = stepContext.Context.Activity.ChannelId.ToUpper();
            CRLVeFields.plataforma = stepContext.Context.Activity.ChannelId.ToUpper();


            if (stepContext.Context.Activity.Text.Length > 1)
            {
                await stepContext.Context.SendActivityAsync("Por favor, digite somente o número da opção desejada (De 1 a 5).");

                return await stepContext.ReplaceDialogAsync(nameof(MainDialog), default, cancellationToken);
            }
            else
            {

                switch (stepContext.Values["menu"].ToString().ToLower())
                {
                    case "licenciamento anual (banese)":
                        LicenseFields.tipoDocumentoIn = "D";
                        LicenseFields.Banco = "Banese";
                        stepContext.Values["LicenseFields"] = LicenseFields;
                        return await stepContext.BeginDialogAsync(nameof(RootLicenseDialog), LicenseFields, cancellationToken);

                    case "licenciamento anual (outros bancos)":
                        LicenseFields.tipoDocumentoIn = "F";
                        LicenseFields.Banco = "Outros Bancos";
                        stepContext.Values["LicenseFields"] = LicenseFields;
                        return await stepContext.BeginDialogAsync(nameof(RootLicenseDialog), LicenseFields, cancellationToken);

                    case "emitir documento de circulação (crlv-e)":
                        stepContext.Values["CRLVeFields"] = CRLVeFields;
                        return await stepContext.BeginDialogAsync(nameof(RootCRLVeDialog), CRLVeFields, cancellationToken);
                    
                    case "consultar data de licenciamento":
                        stepContext.Values["CalendarioFields"] = CalendarioFields;
                        return await stepContext.BeginDialogAsync(nameof(Calendario), CalendarioFields, cancellationToken);
                    case "consultar situação de veículo (débitos)":
                        stepContext.Values["ConsultaDebitosFields"] = ConsultaDebitosFields;
                        return await stepContext.BeginDialogAsync(nameof(RootConsultaDeb), ConsultaDebitosFields, cancellationToken);

                    case "nenhuma das alternativas":
                        return await stepContext.BeginDialogAsync(nameof(RootOthersServicesDialog), LicenseFields, cancellationToken);

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


    }
}