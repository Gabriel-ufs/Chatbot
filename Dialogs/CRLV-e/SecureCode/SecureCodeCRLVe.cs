// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using AdaptiveCards;
using CoreBot.Components.Functions;
using CoreBot.Components.Widgets;
using CoreBot.Fields;
using CoreBot.Models;
using CoreBot.Models.Methods;
using CoreBot.Models.MethodsValidation.License;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Schema;
using Newtonsoft.Json.Linq;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.BotBuilderSamples.Dialogs
{
    /// <summary>
    /// Diálogo responsável por obter o código de segurança do usuário.
    /// </summary>
    public class SecureCodeCRLVeDialog : CancelAndHelpDialog
    {
        CRLVeFields CRLVeFields;

        public SecureCodeCRLVeDialog()
            : base(nameof(SecureCodeCRLVeDialog))
        {
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt), null, "Pt-br"));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(new SpecificationsCRLVeDialog());
            AddDialog(new RequiredSecureCodeCRLVeDialog());
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                SecureCodeRequiredStepAsync,
                VerificationSecureCodeStepAsync

            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        /// <summary>
        /// Passo inicial responsável por obter o contexto do diálogo pai (RootCRLVeDialog) e
        /// solicitar que o usuário escreva o código de segurança.
        /// </summary>
        /// <param name="stepContext">Contexto de SecureCodeCRLVe.</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<DialogTurnResult> SecureCodeRequiredStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            WaterfallStepContext contextParent = (WaterfallStepContext)stepContext.Parent;
            CRLVeFields = (CRLVeFields)contextParent.Values["CRLVeFields"];
            stepContext.Values["CRLVeFields"] = CRLVeFields;

            // Geração da imagem do CRLVe.
            ImageCard ImageCard = new ImageCard();
            await stepContext.Context.SendActivityAsync(ImageCard.addImageCardSecureCode(), cancellationToken);

            // Geração de botão com link
            ButtonCard expandButton = new ButtonCard();
            await stepContext.Context.SendActivityAsync(expandButton.addButtonExpandSecureCode(CRLVeFields), cancellationToken);

            await stepContext.Context.SendActivityAsync(MessageFactory.Text("Informe o CÓDIGO DE SEGURANÇA"), cancellationToken);
            var secureCode = MessageFactory.Text(null, InputHints.ExpectingInput);
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = secureCode }, cancellationToken);
        }

        /// <summary>
        /// Passo responsável por receber o valor do código de segurança informado no passo SecureCodeRequiredStepAsync,
        /// Chamar o WebService e Atribuir aos valores de CRLVeFields dinamicamente (Tais valores serão passados como objeto CRLVeFields e recuperados no próximo diálogo via stepContext.Parent). 
        /// </summary>
        /// <param name="stepContext">Contexto de SecureCodeCRLVe.</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<DialogTurnResult> VerificationSecureCodeStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            CRLVeFields = (CRLVeFields)stepContext.Values["CRLVeFields"];
            CRLVeFields.codSegurancaIn = stepContext.Result.ToString();

            await stepContext.Context.SendActivitiesAsync(new Activity[]
            {
                MessageFactory.Text("Estou verificando o código de segurança informado. Por favor, aguarde um momento..."),
                //new Activity { Type = ActivityTypes.Typing },
            }, cancellationToken);

            VehicleCRLV vehicle = new VehicleCRLV();

            // Caso o formato seja válido.
            if (vehicle.ValidationString(CRLVeFields.codSegurancaIn) == true)
            {
                var webResult = await vehicle.ValidationSecureCode(CRLVeFields.codSegurancaIn, CRLVeFields.plataforma);

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

                switch (CRLVeFields.erroCodigo == 0 && CRLVeFields.codigoRetorno == 1 ? "Ok":
                        CRLVeFields.erroCodigo == 1 ? "Incorreto" :
                        CRLVeFields.erroCodigo >= 2 && CRLVeFields.erroCodigo <= 900 ? "ErroSistema" :
                        CRLVeFields.erroCodigo == 0 && CRLVeFields.codigoRetorno == 0 ? "ErroConexao" :
                        null)
                {
                    case "Ok":
                        CRLVeFields.Count = 0;
                        return await stepContext.BeginDialogAsync(nameof(SpecificationsCRLVeDialog), CRLVeFields, cancellationToken);
                    case "Incorreto":
                        await stepContext.Context.SendActivityAsync(CRLVeFields.erroMensagem);

                        // Contador que garante 3 tentativas
                        Counter cont = new Counter();
                        return await cont.ThreeTimes(CRLVeFields, stepContext, nameof(SecureCodeCRLVeDialog), cancellationToken, "o Código de Segurança");
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
            else
            {
                // Se a string for inválida
                await stepContext.Context.SendActivityAsync("Este código de segurança é inválido!");

                // Contador que garante 3 tentativas
                Counter cont = new Counter();
                return await cont.ThreeTimes(CRLVeFields, stepContext, nameof(SecureCodeCRLVeDialog), cancellationToken, "o Código de Segurança");
            }
        }
    }
}
