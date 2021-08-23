// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using CoreBot.Models;
using CoreBot.Fields;
using CoreBot.Services.WSDLService;
using CoreBot.Components.Widgets;
using CoreBot.Components.Functions;

namespace Microsoft.BotBuilderSamples.Dialogs
{
    /// <summary>
    /// Diálogo responsável por obter o código de segurança do usuário.
    /// </summary>
    public class SecureCodeDialog : CancelAndHelpDialog
    {
        LicenseFields LicenseFields;
        public SecureCodeDialog()
            : base(nameof(SecureCodeDialog))
        {
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt), null, "Pt-br"));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(new SpecificationsDialog());
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                SecureCodeRequiredStepAsync,
                VerificationSecureCodeStepAsync

            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        /// <summary>
        /// Passo inicial responsável por obter o contexto do diálogo pai (RootLicenseDialog) e
        /// solicitar que o usuário escreva o código de segurança.
        /// </summary>
        /// <param name="stepContext">Contexto de SecureCode.</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<DialogTurnResult> SecureCodeRequiredStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // Busca pelo contexto do diálogo pai.
            WaterfallStepContext contextParent = (WaterfallStepContext)stepContext.Parent;
            LicenseFields = new LicenseFields();
            LicenseFields = (LicenseFields)contextParent.Values["LicenseFields"];
            stepContext.Values["LicenseFields"] = LicenseFields;

            // Geração da imagem do CRLVe.
            ImageCard ImageCard = new ImageCard();
            await stepContext.Context.SendActivityAsync(ImageCard.addImageCardSecureCode(), cancellationToken);

            // Botão com link.
            ButtonCard expandButton = new ButtonCard();
            await stepContext.Context.SendActivityAsync(expandButton.addButtonExpandSecureCode(LicenseFields), cancellationToken);

            await stepContext.Context.SendActivityAsync(MessageFactory.Text("Informe o CÓDIGO DE SEGURANÇA"), cancellationToken);
            var secureCode = MessageFactory.Text(null, InputHints.ExpectingInput);
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = secureCode }, cancellationToken);
        }

        /// <summary>
        /// Passo responsável por receber o valor do código de segurança informado no passo SecureCodeRequiredStepAsync,
        /// Chamar o WebService e Atribuir aos valores de LicenseFields dinamicamente (Tais valores serão passados como objeto LicenseFields e recuperados no próximo diálogo via stepContext.Parent). 
        /// </summary>
        /// <param name="stepContext">Contexto de SecureCode.</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<DialogTurnResult> VerificationSecureCodeStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            LicenseFields = (LicenseFields)stepContext.Values["LicenseFields"];
            LicenseFields.codSegurancaIn = stepContext.Result.ToString();

            await stepContext.Context.SendActivitiesAsync(new Activity[]
            {
                MessageFactory.Text("Estou verificando o código de segurança informado. Por favor, aguarde um momento..." + Emojis.Rostos.Sorriso),
                //new Activity { Type = ActivityTypes.Typing },
            }, cancellationToken);

            VehicleLicense vehicle = new VehicleLicense();

            // Caso o código de segurança seja no formato válido.
            if (vehicle.ValidationString(LicenseFields.codSegurancaIn) == true)
            {
                wsDetranChatBot.wsChatbotSoapClient wsClient = Authentication.WsClient();
                wsDetranChatBot.autenticacao auth = Authentication.Auth();

                var webResult = await vehicle.ValidationSecureCodeLicenciamento(LicenseFields.codSegurancaIn, LicenseFields.tipoDocumentoIn, LicenseFields.plataforma);

                lock (LicenseFields)
                {
                    LicenseFields.codigoRetorno = webResult.codigoRetorno;
                    LicenseFields.erroCodigo = webResult.erro.codigo;
                    LicenseFields.erroMensagem = webResult.erro.mensagem;
                    LicenseFields.erroTrace = webResult.erro.trace;
                    LicenseFields.codSegurancaOut = webResult.codSegurancaOut.ToString();
                    LicenseFields.renavamOut = webResult.renavamOut.ToString();
                    LicenseFields.placa = webResult.placa;
                    LicenseFields.nomeProprietario = webResult.nomeProprietario;
                    LicenseFields.temRNTRC = webResult.temRNTRC;
                    LicenseFields.tipoAutorizacaoRNTRCOut = webResult.tipoAutorizacaoRNTRC;
                    LicenseFields.nroAutorizacaoRNTRCOut = webResult.nroAutorizacaoRNTRC;
                    LicenseFields.temIsençãoIPVA = webResult.temIsencaoIPVA;
                    LicenseFields.restricao = webResult.restricao;
                    LicenseFields.anoLicenciamento = webResult.anoLicenciamento;
                    LicenseFields.totalCotaUnica = webResult.totalCotaUnica;
                    LicenseFields.contadorAnoLicenciamento = webResult.contadorAnoLicenciamento;
                    LicenseFields.recallCodigo = webResult.recallPendente.codigo;
                    LicenseFields.recallMensagem = webResult.recallPendente.mensagem;
                    LicenseFields.recallDescricao = new string[] { webResult.recallPendente.listaRecall.ToString() };
                }

                if (LicenseFields.erroCodigo == 1)
                {
                    await stepContext.Context.SendActivityAsync(LicenseFields.erroMensagem);
                    if (LicenseFields.SecureCodeBool == true || LicenseFields.Count < 3)
                    {
                        // Contador que garante 3 tentativas
                        Counter cont = new Counter();
                        return await cont.ThreeTimes(LicenseFields, stepContext, nameof(SecureCodeDialog), cancellationToken, "o Código de Segurança");
                    }
                    else
                    {
                        return await stepContext.ReplaceDialogAsync(nameof(MainDialog), LicenseFields, cancellationToken);
                    }
                } 
                // Caso erro 2 <= x <= 900
                else if (LicenseFields.erroCodigo >= 2 && LicenseFields.erroCodigo <= 900)
                {
                    await stepContext.Context.SendActivityAsync(LicenseFields.erroMensagem);
                    return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
                }
                // Caso retorne nenhum erro, mas tenha falha na conexao
                else if (LicenseFields.erroCodigo == 0 && LicenseFields.codigoRetorno == 0)
                {
                    await stepContext.Context.SendActivityAsync("Estou realizando correções em meu sistema. Por favor, volte mais tarde para efetuar seu serviço" +
                                                                ", tente pelo nosso portal ou entre em contato com nossa equipe de atendimento.");
                    return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
                }
                // Caso não haja erros
                else if (LicenseFields.erroCodigo == 0 && LicenseFields.codigoRetorno == 1)
                {
                    LicenseFields.Count = 0;
                    return await stepContext.BeginDialogAsync(nameof(SpecificationsDialog), LicenseFields, cancellationToken);
                }
                // Erro crítico (Sistema fora)
                else
                {
                    await stepContext.Context.SendActivityAsync("Estou realizando correções em meu sistema. Por favor, volte mais tarde para efetuar seu serviço" +
                                                                ", tente pelo nosso portal ou entre em contato com nossa equipe de atendimento.");
                    return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
                }
            }
            // Se a string for inválida
            else
            {
                await stepContext.Context.SendActivityAsync("Este código de segurança é inválido!");

                // Contador que garante 3 tentativas
                Counter cont = new Counter();
                return await cont.ThreeTimes(LicenseFields, stepContext, nameof(SecureCodeDialog), cancellationToken, "o Código de Segurança"); 
            }
        }
    }
}
