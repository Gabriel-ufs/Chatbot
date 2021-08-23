// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;
using Microsoft.Recognizers.Text.DataTypes.TimexExpression;
using CoreBot.Models;
using AdaptiveCards;
using Microsoft.Extensions.Options;
using System.Linq;
using Newtonsoft.Json.Linq;
using CoreBot.Models.MethodsValidation.License;
using CoreBot.Fields;
using CoreBot.Components.Widgets;
using CoreBot.Components.Functions;

namespace Microsoft.BotBuilderSamples.Dialogs
{
    /// <summary>
    /// Diálogo responsável por solicitar e validar o Renavam.
    /// </summary>
    public class RenavamDialog : CancelAndHelpDialog
    {
        LicenseFields LicenseFields;

        public RenavamDialog()
            : base(nameof(RenavamDialog))
        {
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt), null, "Pt-br"));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(new SecureCodeDialog());
            AddDialog(new RequiredSecureCodeLicenseDialog());
            AddDialog(new SpecificationsDialog());
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                RenavamStepAsync,
                ValidationRenavamStepAsync,
                OptionValidationStepAsync,

            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        /// <summary>
        /// Passo responsável por receber o contexto do diálogo pai (RootLicenseDialog) e solicitar o Renavam para usuário.
        /// </summary>
        /// <param name="stepContext">Contexto do RenavamDialog</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<DialogTurnResult> RenavamStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // Busca pelo contexto do diálogo pai.
            WaterfallStepContext contextParent = (WaterfallStepContext)stepContext.Parent;
            LicenseFields = new LicenseFields();
            LicenseFields = (LicenseFields)contextParent.Values["LicenseFields"];
            stepContext.Values["LicenseFields"] = LicenseFields;

            // Geração da imagem
            ImageCard ImageCard = new ImageCard();
            await stepContext.Context.SendActivityAsync(ImageCard.addImageCardRenavamPlaca(), cancellationToken);

            // Geração de botão com link
            ButtonCard expandButton = new ButtonCard();
            await stepContext.Context.SendActivityAsync(expandButton.addButtonExpandRenavam(LicenseFields), cancellationToken);

            await stepContext.Context.SendActivityAsync(MessageFactory.Text("Por favor, informe o RENAVAM do seu veículo"), cancellationToken);
            var renavam = MessageFactory.Text(null, InputHints.ExpectingInput);
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = renavam }, cancellationToken);
        }

        /// <summary>
        /// Passo responsável por receber o valor do código de segurança informado no passo RenavamStepAsync,
        /// Chamar o WebService e Atribuir aos valores de LicenseFields dinamicamente (Tais valores serão passados como objeto LicenseFields e recuperados no próximo diálogo via stepContext.Parent).
        /// </summary>
        /// <param name="stepContext">Contexto do RenavamDialog</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<DialogTurnResult> ValidationRenavamStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            LicenseFields = (LicenseFields)stepContext.Values["LicenseFields"];

            await stepContext.Context.SendActivitiesAsync(new Activity[]
            {
                MessageFactory.Text("Estou verificando o Renavam informado. Por favor, aguarde um momento..."),
                //new Activity { Type = ActivityTypes.Typing },
            }, cancellationToken);

            LicenseFields.renavamIn = stepContext.Result.ToString();

            VehicleLicense vehicle = new VehicleLicense();

            // Caso o formato do Renavam seja válido
            if (vehicle.ValidationString(LicenseFields.renavamIn) == true)
            {
                var webResult = await vehicle.ValidationRenavam(LicenseFields.renavamIn, LicenseFields.tipoDocumentoIn, LicenseFields.plataforma);
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
                stepContext.Values["LicenseFields"] = LicenseFields;

                if (LicenseFields.erroCodigo == 1)
                {
                    await stepContext.Context.SendActivityAsync(LicenseFields.erroMensagem);
                    if (LicenseFields.SecureCodeBool == true || LicenseFields.Count < 3)
                    {
                        // Contador que garante 3 tentativas
                        Counter cont = new Counter();
                        return await cont.ThreeTimes(LicenseFields, stepContext, nameof(RenavamDialog), cancellationToken, "o Renavam");
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
                else if(LicenseFields.erroCodigo == 0 && LicenseFields.codigoRetorno == 1)
                {
                    LicenseFields.Count = 0;
                    if (LicenseFields.codSegurancaOut != "0")
                    {
                        await stepContext.Context.SendActivityAsync("Em nossos sistemas você possui código de segurança, para prosseguir será necessário informá-lo.");

                        // Geração da imagem
                        ImageCard ImageCard = new ImageCard();
                        await stepContext.Context.SendActivityAsync(ImageCard.addImageCardSecureCode(), cancellationToken);

                        // Botão com link
                        ButtonCard expandButton = new ButtonCard();
                        await stepContext.Context.SendActivityAsync(expandButton.addButtonExpandSecureCode(LicenseFields), cancellationToken);

                        var promptOptions = new PromptOptions
                        {
                            Prompt = MessageFactory.Text("Você localizou?" + TextGlobal.Choice),
                            RetryPrompt = MessageFactory.Text(TextGlobal.Desculpe + "Você localizou?" + TextGlobal.ChoiceDig),
                            Choices = ChoiceFactory.ToChoices(new List<string> { "Sim", "Não" }),
                        };

                        return await stepContext.PromptAsync(nameof(ChoicePrompt), promptOptions, cancellationToken);
                    }
                    LicenseFields.codSegurancaIn = "0";
                    return await stepContext.ReplaceDialogAsync(nameof(SpecificationsDialog), LicenseFields, cancellationToken);
                }
                // Erro crítico (Sistema fora)
                else
                {
                    await stepContext.Context.SendActivityAsync("Estou realizando correções em meu sistema. Por favor, volte mais tarde para efetuar seu serviço" +
                                                                ", tente pelo nosso portal ou entre em contato com nossa equipe de atendimento.");
                    return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
                }

            }
            else
            {
                // Se a string for inválida
                await stepContext.Context.SendActivityAsync("Este Renavam é inválido!");
                // Contador que garante 3 tentativas
                Counter cont = new Counter();
                return await cont.ThreeTimes(LicenseFields, stepContext, nameof(RenavamDialog), cancellationToken, "o Renavam");
            }
        }

        /// <summary>
        /// Passo responsável por, caso o usuário tenha Código de seguraça, receber o valor de "Você localizou?".
        /// </summary>
        /// <param name="stepContext">Contexto do RenavamDialog</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Fim do diálogo atual ou chama o diálogo RequiredSecureCodeLicenseDialog</returns>
        private async Task<DialogTurnResult> OptionValidationStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {

            LicenseFields = (LicenseFields)stepContext.Values["LicenseFields"];

            stepContext.Values["choice"] = ((FoundChoice)stepContext.Result).Value;
            if (stepContext.Values["choice"].ToString().ToLower() == "sim")
            {
                return await stepContext.ReplaceDialogAsync(nameof(RequiredSecureCodeLicenseDialog), LicenseFields, cancellationToken);
            }
            else
            {
                await stepContext.Context.SendActivityAsync("Infelizmente preciso dessa informação para prosseguir. " +
                                                            "Nesse caso, será necessário entrar em contato com o nosso atendimento!");

                // Geração de botão
                ButtonCard button = new ButtonCard();
                await stepContext.Context.SendActivityAsync(button.addButtonExpand(LicenseFields, "Ir para o site", "https://www.detran.se.gov.br/portal/?pg=atend_agendamento&pCod=1"), cancellationToken);

                return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
            }
        }
    }
}
