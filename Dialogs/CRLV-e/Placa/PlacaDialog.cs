// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using AdaptiveCards;
using Microsoft.Bot.Builder.Dialogs.Choices;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using CoreBot.Fields;
using CoreBot.Models.Methods;
using CoreBot.Components.Widgets;
using CoreBot.Components.Functions;

namespace Microsoft.BotBuilderSamples.Dialogs
{
    /// <summary>
    /// Diálogo responsável por solicitar e validar a Placa.
    /// </summary>
    public class PlacaDialog : CancelAndHelpDialog
    {
        private CRLVeFields CRLVeFields;

        public PlacaDialog()
            : base(nameof(PlacaDialog))
        {
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt), null, "Pt-br"));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(new SecureCodeCRLVeDialog());
            AddDialog(new SpecificationsCRLVeDialog());
            AddDialog(new RequiredSecureCodeCRLVeDialog());
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                RenavamStepAsync,
                ValidationPlacaStepAsync,
                OptionValidationStepAsync

            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        /// <summary>
        /// Passo responsável por receber o contexto do diálogo pai (RootCRLVeDialog) e solicitar a Placa para usuário.
        /// </summary>
        /// <param name="stepContext">Contexto do PlacaDialog.</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<DialogTurnResult> RenavamStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {

            WaterfallStepContext contextParent = (WaterfallStepContext)stepContext.Parent;
            CRLVeFields = (CRLVeFields)contextParent.Values["CRLVeFields"];
            stepContext.Values["CRLVeFields"] = CRLVeFields;

            // Geração da imagem
            ImageCard ImageCard = new ImageCard();
            await stepContext.Context.SendActivityAsync(ImageCard.addImageCardRenavamPlaca(), cancellationToken);

            // Geração de botão com link
            ButtonCard expandButton = new ButtonCard();
            await stepContext.Context.SendActivityAsync(expandButton.addButtonExpandRenavam(CRLVeFields), cancellationToken);

            await stepContext.Context.SendActivityAsync(MessageFactory.Text("Por favor, informe a PLACA do seu veículo"), cancellationToken);
            var renavam = MessageFactory.Text(null, InputHints.ExpectingInput);
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = renavam }, cancellationToken);
        }

        /// <summary>
        /// Passo responsável por receber o valor do código de segurança informado no passo RenavamStepAsync,
        /// Chamar o WebService e Atribuir aos valores de CRLVeFields dinamicamente (Tais valores serão passados como objeto CRLVeFields e recuperados no próximo diálogo via stepContext.Parent).
        /// </summary>
        /// <param name="stepContext">Contexto do PlacaDialog</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<DialogTurnResult> ValidationPlacaStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            CRLVeFields = (CRLVeFields)stepContext.Values["CRLVeFields"];

            await stepContext.Context.SendActivitiesAsync(new Activity[]
            {
                MessageFactory.Text("Estou verificando a placa informada. Por favor, aguarde um momento..."),
                //new Activity { Type = ActivityTypes.Typing },
            }, cancellationToken);

            CRLVeFields.placaIn = stepContext.Result.ToString();

            VehicleCRLV vehicle = new VehicleCRLV();

            // Caso o formato da placa seja válido
            if (vehicle.ValidationStringPlaca(CRLVeFields.placaIn) == true)
            {
                var webResult = await vehicle.ValidationPlaca(CRLVeFields.placaIn, CRLVeFields.plataforma);

                CRLVeFields.codigoRetorno = webResult.codigoRetorno;
                CRLVeFields.codSegurançaOut = webResult.codSegurancaOut.ToString();
                CRLVeFields.renavam = webResult.renavam.ToString();
                CRLVeFields.nomeProprietario = webResult.nomeProprietario;
                CRLVeFields.placaOut = webResult.placaOut;
                CRLVeFields.codigoRetorno = webResult.codigoRetorno;
                CRLVeFields.documentoCRLVePdf = webResult.documentoCRLVePdf;
                CRLVeFields.erroCodigo = webResult.erro.codigo;
                CRLVeFields.erroMensagem = webResult.erro.mensagem;
                CRLVeFields.erroTrace = webResult.erro.trace;

                stepContext.Values["CRLVeFields"] = CRLVeFields;

                switch (CRLVeFields.erroCodigo == 0 && CRLVeFields.codigoRetorno == 1 ? "Ok" :
                        CRLVeFields.erroCodigo == 1 ? "Incorreto" :
                        CRLVeFields.erroCodigo >= 2 && CRLVeFields.erroCodigo <= 900 ? "ErroSistema" :
                        CRLVeFields.erroCodigo == 0 && CRLVeFields.codigoRetorno == 0 ? "ErroConexao" :
                        null)
                {
                    case "Ok":
                        CRLVeFields.Count = 0;
                        if (VehicleCRLV.ExistSecureCodePlaca(CRLVeFields.codSegurançaOut) == true)
                        {
                            await stepContext.Context.SendActivityAsync("Em nossos sistemas você possui código de segurança, vou precisar dessa informação");

                            // Geração da imagem
                            ImageCard ImageCard = new ImageCard();
                            await stepContext.Context.SendActivityAsync(ImageCard.addImageCardSecureCode(), cancellationToken);

                            // Geração de botão com link
                            ButtonCard expandButton = new ButtonCard();
                            await stepContext.Context.SendActivityAsync(expandButton.addButtonExpandSecureCode(CRLVeFields), cancellationToken);

                            var promptOptions = new PromptOptions
                            {
                                Prompt = MessageFactory.Text("Você localizou?" + TextGlobal.Choice),
                                RetryPrompt = MessageFactory.Text(TextGlobal.Desculpe + "Você localizou?" + TextGlobal.ChoiceDig),
                                Choices = ChoiceFactory.ToChoices(new List<string> { "Sim", "Não" }),
                            };
                            return await stepContext.PromptAsync(nameof(ChoicePrompt), promptOptions, cancellationToken);
                        }
                        else
                        {
                            return await stepContext.BeginDialogAsync(nameof(SpecificationsCRLVeDialog), CRLVeFields, cancellationToken);
                        }
                    case "Incorreto":
                        await stepContext.Context.SendActivityAsync(CRLVeFields.erroMensagem);
                        // Contador que garante 3 tentativas
                        Counter cont = new Counter();
                        return await cont.ThreeTimes(CRLVeFields, stepContext, nameof(PlacaDialog), cancellationToken, "a Placa");
                    case "ErroSistema":
                        await stepContext.Context.SendActivityAsync(CRLVeFields.erroMensagem);
                        return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
                    case "ErroConexao":
                        await stepContext.Context.SendActivityAsync("Estou realizando correções em meu sistema. Por favor, volte mais tarde para efetuar seu serviço" +
                                                                    ", tente pelo nosso portal ou entre em contato com nossa equipe de atendimento.");
                        return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
                    default:
                        await stepContext.Context.SendActivityAsync("Estou realizando correções em meu sistema. Por favor, volte mais tarde para efetuar seu serviço" +
                                                                ", tente pelo nosso portal ou entre em contato com nossa equipe de atendimento.");
                        return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
                }
            }
            // Se a string for inválida
            else
            {
                await stepContext.Context.SendActivityAsync("Esta placa é inválida!");

                // Contador que garante 3 tentativas
                Counter cont = new Counter();
                return await cont.ThreeTimes(CRLVeFields, stepContext, nameof(PlacaDialog), cancellationToken, "o Renavam");
            }
        }

        /// <summary>
        /// Passo responsável por, caso o usuário tenha Código de seguraça, receber o valor de "Você localizou?".
        /// </summary>
        /// <param name="stepContext">Contexto do PlacaDialog.</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<DialogTurnResult> OptionValidationStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            CRLVeFields = (CRLVeFields)stepContext.Values["CRLVeFields"];

            stepContext.Values["choice"] = ((FoundChoice)stepContext.Result).Value;

            if (stepContext.Values["choice"].ToString().ToLower() == "sim")
                return await stepContext.ReplaceDialogAsync(nameof(RequiredSecureCodeCRLVeDialog), CRLVeFields, cancellationToken);
            else
            {
                await stepContext.Context.SendActivityAsync("Infelizmente preciso dessa informação para prosseguir. " +
                                                            "Nesse caso, será necessário entrar em contato com o nosso atendimento!");

                // Geração de botão
                ButtonCard button = new ButtonCard();
                await stepContext.Context.SendActivityAsync(button.addButtonExpand(CRLVeFields, "Ir para o site", "https://www.detran.se.gov.br/portal/?pg=atend_agendamento&pCod=1"), cancellationToken);

                return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);

            }
        }
    }
}