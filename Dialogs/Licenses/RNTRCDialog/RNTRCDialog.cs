// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdaptiveCards;
using CoreBot.Fields;
using CoreBot.Models.MethodsValidation.License;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;
using Newtonsoft.Json.Linq;

namespace Microsoft.BotBuilderSamples.Dialogs
{
    /// <summary>
    /// Diálogo responsável por:
    /// 1) Solicitar o tipo de autorização.
    /// 2) Solicitar e verificar validade do número da autorização.
    /// 3) Solicitar data de validade da autorização.
    /// 4) Validar as informações.
    /// </summary>
    public class RNTRCDialog : CancelAndHelpDialog
    {
        LicenseFields LicenseFields;
        public RNTRCDialog()
            : base(nameof(RNTRCDialog))
        {
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt)));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                AuthorizationStepAsync,
                AuthorizationNumberStepAsync,
                ValidationTypeAuthorizationStepAsync,
                ValidationAuthorizationNumeroStepAsync,
                AuthorizationDataStepAsync,
                ValidationAuthorizationDataStepAsync,
                AuthorizationValidationStepAsync
            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        /// <summary>
        /// Passo inicial responsável por obter o contexto do diálogo pai (RootLicenseDialog) e
        /// solicitar o tipo de autorização.
        /// </summary>
        /// <param name="stepContext">Contexto RNTRCDialog</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Solicitação de tipo de autorização</returns>
        private async Task<DialogTurnResult> AuthorizationStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            WaterfallStepContext contextParent = (WaterfallStepContext)stepContext.Parent;
            LicenseFields = (LicenseFields)contextParent.Values["LicenseFields"];
            stepContext.Values["LicenseFields"] = LicenseFields;

            var promptOptions = new PromptOptions
            {
                Prompt = MessageFactory.Text($"Qual tipo de autorização você possui?"),
                Choices = ChoiceFactory.ToChoices(new List<string> { "ETC - Empresa de transporte de carga", "CTC - Cooperativa de transporte de carga", "TAC - Transportador autônomo de carga", "Não sei onde encontrar esta informação" }),
                RetryPrompt = MessageFactory.Text("Por favor, insira uma opções abaixo:")
            };

            return await stepContext.PromptAsync(nameof(ChoicePrompt), promptOptions, cancellationToken);
        }

        /// <summary>
        /// Passo responsável por receber o tipo de autorização informado em AuthorizationStepAsync.
        /// </summary>
        /// <param name="stepContext">Contexto RNTRCDialog</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Continuação do diálogo.</returns>
        private async Task<DialogTurnResult> AuthorizationNumberStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            LicenseFields = (LicenseFields)stepContext.Values["LicenseFields"];
            stepContext.Values["choice"] = ((FoundChoice)stepContext.Result).Value;
            LicenseFields.tipoAutorizacaoRNTRCIn = stepContext.Context.Activity.Text;
            return await stepContext.ContinueDialogAsync(cancellationToken);
        }

        /// <summary>
        /// Passo responsável por solicitar o número da autorização.
        /// </summary>
        /// <param name="stepContext">Contexto RNTRCDialog</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Solicitação do núumero autorização</returns>
        private async Task<DialogTurnResult> ValidationTypeAuthorizationStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
        //    if(VehicleLicenseRNTRC.ValidationTypeAuthorization(LicenseFields.tipoAutorizacaoRNTRCIn) == true)
        //    {
                var promptMessage = MessageFactory.Text("Para continuarmos informe o número da autorização", InputHints.ExpectingInput);
                return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
        //    }
        //    else
        //    {
        //        await stepContext.Context.SendActivityAsync("Não é esse o tipo de autorização que consta em nossos sistemas, vamos repetir!");
        //        return await stepContext.ReplaceDialogAsync(nameof(RNTRCDialog), LicenseFields, cancellationToken);
        //    }
        }

        /// <summary>
        /// Passo responsável por receber o número da autorização de ValidationTypeAuthorizationStepAsync e verificar se o retorno é formado somente por números.
        /// </summary>
        /// <param name="stepContext">Contexto RNTRCDialog</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<DialogTurnResult> ValidationAuthorizationNumeroStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            LicenseFields = (LicenseFields)stepContext.Values["LicenseFields"];
            LicenseFields.nroAutorizacaoRNTRCIn = stepContext.Result.ToString();
            if (LicenseFields.Count < 3)
            {
                if (Format.Input.ValidationFormat.IsNumber(LicenseFields.nroAutorizacaoRNTRCIn) == true)
                {
                    LicenseFields.Count = 0;
                    return await stepContext.ContinueDialogAsync(cancellationToken);
                }
                else
                {
                    LicenseFields.Count += 1;
                    await stepContext.Context.SendActivityAsync("Esse número de autorização não é válido, vamos repetir o processo");
                    return await stepContext.ReplaceDialogAsync(nameof(RNTRCDialog), LicenseFields, cancellationToken);
                }
            }
            else
            {
                LicenseFields.Count = 0;
                await stepContext.Context.SendActivityAsync("Seu numero de autorização é inválido!\r\n" +
                                                            "Nesse caso, vou pedir para que procure e volte a falar comigo novamente depois " +
                                                            "ou entre em contato com o DETRAN, para obter mais informações");
                return await stepContext.ReplaceDialogAsync(nameof(MainDialog), LicenseFields, cancellationToken);
            }
            
        }

        /// <summary>
        /// Passo responsável por solicitar a validade da autorização.
        /// </summary>
        /// <param name="stepContext">Contexto RNTRCDialog</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Solicitação da validade da autorização</returns>
        private async Task<DialogTurnResult> AuthorizationDataStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var promptMessage = MessageFactory.Text("Por fim, informe a data de validade da autorização\r\n" +
                                                     "Exemplo: 12/12/2021", InputHints.ExpectingInput);
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
            
        }

        /// <summary>
        /// Passo responsável por receber e validar a data informada pelo usuário.
        /// </summary>
        /// <param name="stepContext">Contexto RNTRCDialog</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<DialogTurnResult> ValidationAuthorizationDataStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {

            LicenseFields = (LicenseFields)stepContext.Values["LicenseFields"];
            LicenseFields.dataValidadeRNTRC = stepContext.Result.ToString();
            LicenseFields.Count += 1;

            if (LicenseFields.Count < 3)
            {
                string data = VehicleLicenseRNTRC.ValidationDate(LicenseFields.dataValidadeRNTRC);
                if (data != null)
                {
                    LicenseFields.dataValidadeRNTRC = data;
                    LicenseFields.Count = 0;
                    return await stepContext.ContinueDialogAsync(cancellationToken);
                }
                else
                {
                    await stepContext.Context.SendActivityAsync("A data informada deve ser maior que a atual, vamos repetir o processo");
                    return await stepContext.ReplaceDialogAsync(nameof(RNTRCDialog), LicenseFields, cancellationToken);
                }
            }
            else
            {
                await stepContext.Context.SendActivityAsync("Sua data é invalida!\r\n" +
                                                            "Nesse caso, vou pedir para que procure e volte a falar comigo novamente depois " +
                                                            "ou entre em contato com o DETRAN, para obter mais informações");
                return await stepContext.ReplaceDialogAsync(nameof(MainDialog), LicenseFields, cancellationToken);
            }
        }

        /// <summary>
        /// Passo responsável por validar as informações passadas pelo usuário.
        /// </summary>
        /// <param name="stepContext">Contexto RNTRCDialog</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<DialogTurnResult> AuthorizationValidationStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            LicenseFields = (LicenseFields)stepContext.Values["LicenseFields"];
            LicenseFields.Count += 1;
            if (LicenseFields.Count < 3)
            {
                ///Valida tipo da autorização
                if (VehicleLicenseRNTRC.ValidationNumber(LicenseFields.nroAutorizacaoRNTRCIn, LicenseFields.nroAutorizacaoRNTRCOut) == true && VehicleLicenseRNTRC.ValidationTypeAuthorization(LicenseFields.tipoAutorizacaoRNTRCIn, LicenseFields.tipoAutorizacaoRNTRCOut))
                {
                    return await stepContext.ContinueDialogAsync(cancellationToken);
                }
                else
                {
                    await stepContext.Context.SendActivityAsync("Os dados informados não estão de acordo com o nosso sistema.\r\n" +
                                                               "Vou ter que repetir algumas perguntas, ok?");
                    return await stepContext.ReplaceDialogAsync(nameof(RNTRCDialog), LicenseFields, cancellationToken);
                }
            }
            else
            {
                await stepContext.Context.SendActivityAsync("Os dados informados não estão de acordo com o nosso sistema.\r\n" +
                                                            "Nesse caso, vou pedir para que procure e volte a falar comigo novamente depois " +
                                                            "ou entre em contato com o DETRAN, para obter mais informações");
                var choices = new[] { "Ir para o site" };
                var card = new AdaptiveCard(new AdaptiveSchemaVersion(1, 0))
                {
                    // Use LINQ to turn the choices into submit actions

                    Actions = choices.Select(choice => new AdaptiveOpenUrlAction
                    {
                        Title = choice,
                        Url = new Uri("https://www.detran.se.gov.br/portal/?menu=1")

                    }).ToList<AdaptiveAction>(),
                };

                // Prompt
                return await stepContext.PromptAsync(nameof(ChoicePrompt), new PromptOptions
                {
                    Prompt = (Activity)MessageFactory.Attachment(new Attachment
                    {
                        ContentType = AdaptiveCard.ContentType,
                        // Convert the AdaptiveCard to a JObject
                        Content = JObject.FromObject(card),
                    }),
                    Choices = ChoiceFactory.ToChoices(choices),
                    // Don't render the choices outside the card
                    Style = ListStyle.None,
                },
                    cancellationToken);
                
            }
        }

    }
}
