// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CoreBot.Fields;
using CoreBot.Models;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;

namespace Microsoft.BotBuilderSamples.Dialogs
{
    /// <summary>
    /// Diálogo responsável por tratar quando o usuário possui ou não mais de um ano para pagar suas pendências.
    /// </summary>
    public class PendencyDialog : CancelAndHelpDialog
    {
        LicenseFields LicenseFields;
         
        public PendencyDialog()
            : base(nameof(PendencyDialog))
        {
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt)));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                PendencyStepAsync,
                ConfirmValidationStepAsync,
                Pendency_2StepAsync,
                Pendency_3StepAsync
            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        /// <summary>
        /// Classe responsável por resgatar ano atual e o ano anterior.
        /// </summary>
        class ano
        {
            public static string anoAnterior = ((DateTime.Now.Year) - 1).ToString();
            public static string anoAtual = DateTime.Now.Year.ToString();
        }

        /// <summary>
        /// Passo responsável por demonstrar os valores ao usuário e:
        /// 1) Caso possua 1 ano perguntar se deseja prosseguir.
        /// 2) Caso possua 2 anos perguntar qual ano deseja pagar.
        /// </summary>
        /// <param name="stepContext">Contexto do PendencyDialog.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>DialogTurnResult com qual ano o usuário deseja licenciar (Caso possua 2 anos) ou se deseja prosseguir. [1º ano, 2º ano, Sim, Não].</returns>
        private async Task<DialogTurnResult> PendencyStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            WaterfallStepContext contextParent = (WaterfallStepContext)stepContext.Parent;
            LicenseFields = (LicenseFields)contextParent.Values["LicenseFields"];
            stepContext.Values["LicenseFields"] = LicenseFields;

            // Caso possua 2 anos
            if (VehicleLicense.ValidationYear(LicenseFields.contadorAnoLicenciamento) == true) 
            {
                await stepContext.Context.SendActivityAsync("Detectei também que você pode optar por licenciar o ano anterior");
                await stepContext.Context.SendActivityAsync("Ano: " + LicenseFields.anoLicenciamento[0] + "\r\n" +
                                                            "Valor a ser pago: R$ " + LicenseFields.totalCotaUnica);

                List<string> anos = new List<string>();
                for (int i = 0; i < LicenseFields.contadorAnoLicenciamento; i++)
                {
                    if (LicenseFields.anoLicenciamento[i] != 0)
                    {
                        anos.Add(LicenseFields.anoLicenciamento[i].ToString());
                    }
                }

                var promptOptions = new PromptOptions
                {
                    Prompt = MessageFactory.Text($"Qual ano deseja licenciar seu veículo? (A escolha do ano atual já traz acumulado o ano anterior)"),
                    Choices = ChoiceFactory.ToChoices(choices: anos),
                };
                return await stepContext.PromptAsync(nameof(ChoicePrompt), promptOptions, cancellationToken);

            }
            // Caso possua 1 ano
            else
            {
                await stepContext.Context.SendActivityAsync("Ano: " + LicenseFields.anoLicenciamento[0] + "\r\n" +
                                                            "Valor a ser pago: R$ " + LicenseFields.totalCotaUnica);
                LicenseFields.exercicio = LicenseFields.anoLicenciamento[0];

                var Options = new PromptOptions
                {
                    Prompt = MessageFactory.Text(TextGlobal.Prosseguir),
                    RetryPrompt = MessageFactory.Text(TextGlobal.Desculpe + "Deseja Prosseguir?" + TextGlobal.ChoiceDig),
                    Choices = ChoiceFactory.ToChoices(new List<string> { "Sim", "Não" }),
                };

                return await stepContext.PromptAsync(nameof(ChoicePrompt), Options, cancellationToken);
            }
        }

        /// <summary>
        /// Passo responsável por receber os valores do passo PendencyStepAsync. Trata caso os valores sejam "Sim", "Não", AnoAnterior, AnoAtual.
        /// </summary>
        /// <param name="stepContext">Contexto do PendencyDialog.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>
        /// DialogTurnResult: 
        /// "Sim" - Retorna EndDialog retornando ao diálogo de Specifications para gerar o pdf no passo VehicleStepAsync.
        /// "Não" - Retorna EndDialog que será tratado no diálogo Specifications no passo VehicleStepAsync.
        /// "AnoAtual" - Retorna ContinueDialog para que a resposta seja tratada no passo seguinte.
        /// "AnoAnterior" - Retorna EndDialog retornando ao diálogo de Specifications para gerar o pdf no passo VehicleStepAsync.
        /// </returns>
        private async Task<DialogTurnResult> ConfirmValidationStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            LicenseFields = (LicenseFields)stepContext.Values["LicenseFields"];
            stepContext.Values["choice"] = ((FoundChoice)stepContext.Result).Value;

            // Caso o passo anterior retorne "Sim".
            if (stepContext.Values["choice"].ToString().ToLower() == "sim")
            {
                await stepContext.Context.SendActivitiesAsync(new Activity[]
                {
                    MessageFactory.Text("Estou verificando seus dados para gerar o documento. Por favor, aguarde um momento..."),
                    //new Activity { Type = ActivityTypes.Typing },
                }, cancellationToken);

                LicenseFields.exercicio = LicenseFields.anoLicenciamento[0];

                return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
            }
            else
            {
                // Caso o passo anterior retorne "Não".
                if (LicenseFields.anoLicenciamento[0] == LicenseFields.exercicio && stepContext.Values["choice"].ToString().ToLower() == "não")
                {
                    return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
                }

                LicenseFields.exercicio = Convert.ToInt32(stepContext.Values["choice"]);

                // Caso o passo anterior retorne "AnoAnterior".
                if (stepContext.Values["choice"].ToString().ToLower() == LicenseFields.anoLicenciamento[0].ToString())
                {
                    await stepContext.Context.SendActivitiesAsync(new Activity[]
                    {
                        MessageFactory.Text("Estou verificando seus dados para gerar o documento. Por favor, aguarde um momento..."),
                       //new Activity { Type = ActivityTypes.Typing },
                    }, cancellationToken);
                    return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
                }
                else
                // Caso o passo anterior retorne "AnoAtual".
                {
                    return await stepContext.ContinueDialogAsync(cancellationToken);
                }
            }
        }

        /// <summary>
        /// Passo responsável por receber o valor do "AnoAtual", chamar o WebService para obter os valores desejados,
        /// mostrar os valores para o usuário e questioná-lo se deseja prosseguir.
        /// </summary>
        /// <param name="stepContext">Contexto do PendencyDialog.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>DialogTurnResult questionando se o usuário deseja prosseguir com o ano atual.</returns>
        private async Task<DialogTurnResult> Pendency_2StepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            LicenseFields = (LicenseFields)stepContext.Values["LicenseFields"];

            VehicleLicense vehicle = new VehicleLicense();

            // Verificação se o ano em LicenseFields é igual ao anoLicenciamento na posição do AnoAtual.
            if (LicenseFields.exercicio == LicenseFields.anoLicenciamento[1])
            {
                var webResult = await vehicle.ValidationSecureCodeLicenciamento(LicenseFields.codSegurancaOut, LicenseFields.tipoDocumentoIn, LicenseFields.exercicio, LicenseFields.plataforma);
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

                await stepContext.Context.SendActivityAsync("Ano: " + LicenseFields.anoLicenciamento[0] + "\r\n" +
                                                            "Valor a ser pago: R$ " + LicenseFields.totalCotaUnica);
                var Options = new PromptOptions
                {
                    Prompt = MessageFactory.Text($"Deseja prosseguir? (Caso não deseje, gerarei o documento de " + ano.anoAnterior +")" + TextGlobal.Choice),
                    RetryPrompt = MessageFactory.Text(TextGlobal.Desculpe + "Deseja prosseguir?" + TextGlobal.ChoiceDig),
                    Choices = ChoiceFactory.ToChoices(new List<string> { "Sim", "Não" }),
                };
                return await stepContext.PromptAsync(nameof(ChoicePrompt), Options, cancellationToken);
            }
            // Caso o ano em LicenseFields seja diferente ao anoLicenciamento na posição do AnoAtual.
            else
            {
                return await stepContext.ContinueDialogAsync(cancellationToken);
            }
        }

        /// <summary>
        /// Passo responsável por receber o valor do passo Pendency_2StepAsync.
        /// </summary>
        /// <param name="stepContext">Contexto do PendencyDialog.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>
        /// DialogTurnResult: 
        /// "Sim" - Retorna ContinueDialog, indo para o diálogo Specificarions no passo VehicleStepAsync.
        /// "Não" - Verifica o ano anterior novamente e retorna ContinueDialog com o ano anterior, que será tratado no diálogo Specifications no passo VehicleStepAsync.
        /// </returns>
        private async Task<DialogTurnResult> Pendency_3StepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            LicenseFields = (LicenseFields)stepContext.Values["LicenseFields"];
            stepContext.Values["choice"] = ((FoundChoice)stepContext.Result).Value;

            // Caso o passo anterior retorne "Sim".
            if (stepContext.Values["choice"].ToString().ToLower() == "sim")
            {
                await stepContext.Context.SendActivitiesAsync(new Activity[]
                {
                    MessageFactory.Text("Estou verificando seus dados para gerar o documento. Por favor, aguarde um momento..."),
                    //new Activity { Type = ActivityTypes.Typing },
                }, cancellationToken);

                return await stepContext.ContinueDialogAsync(cancellationToken);
            }
            // Caso o passo anterior retorne "Não"
            else
            {
                LicenseFields.exercicio -= 1;
                VehicleLicense vehicle = new VehicleLicense();

                var webResult = await vehicle.ValidationSecureCodeLicenciamento(LicenseFields.codSegurancaOut, LicenseFields.tipoDocumentoIn, LicenseFields.exercicio, LicenseFields.plataforma);
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
                return await stepContext.ContinueDialogAsync(cancellationToken);
            }
        }
    }
}
