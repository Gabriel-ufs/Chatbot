// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CoreBot.Fields;
using CoreBot.Models;
using CoreBot.Models.Generate;
using CoreBot.Models.MethodsValidation.License;
using CoreBot.Services.WSDLService.efetuarServicoLicenciamento;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;

namespace Microsoft.BotBuilderSamples.Dialogs
{
    /// <summary>
    /// Diálogo responsável por: 
    /// 1) Exibir informações do usuário e confirmar se dados estão certos. 
    /// 2) Verificar se veículo possui RNTRC. 
    /// 3) Verificar se veículo possui chamada para Recall. 
    /// 4) Verificar se veículo possui isenção. 
    /// 5) Verificar se veículo possui pendências. 
    /// 6) Efetuar o Licenciamento de acordo com as opções verificadas. 
    /// 7) Gerar e disponibilizar PDF e código de barras. 
    /// </summary>
    public class SpecificationsDialog : CancelAndHelpDialog
    {
        LicenseFields LicenseFields;

        public SpecificationsDialog()
            : base(nameof(SpecificationsDialog))
        {
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt)));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(new RNTRCDialog());
            AddDialog(new RecallDialog());
            AddDialog(new ExemptionDialog());
            AddDialog(new PendencyDialog());
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                InfoStepAsync,
                ConfirmDataAsync,
                TypeVehicleAsync,
                RecallVehicleStepAsync,
                ExemptionVehicleStepAsync,
                InvoiceVehicleStepAsync,
                VehicleStepAsync,
                FinalStepAsync

            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        /// <summary>
        /// Passo responsável por resgatar o contexto do diálogo pai (SecureCode ou RequiredSecureCodeLicenses), apresentar informações ao usuário e solicitar confirmação.
        /// </summary>
        /// <param name="stepContext">Contexto de Specifications.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Solicitação de confirmação dos dados.</returns>
        private async Task<DialogTurnResult> InfoStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // Busca pelo contexto do diálogo pai.
            WaterfallStepContext contextParent = (WaterfallStepContext)stepContext.Parent;
            LicenseFields = new LicenseFields();
            LicenseFields = (LicenseFields)contextParent.Values["LicenseFields"];

            // Exibição de mensagem com informações ao usuário.
            await stepContext.Context.SendActivityAsync(MessageFactory.Text("\r\nPlaca: " + LicenseFields.placa +
                                                                            "\r\nProprietário: " + LicenseFields.nomeProprietario),
                                                                            cancellationToken);
            stepContext.Values["LicenseFields"] = LicenseFields;
            var promptOptions = new PromptOptions
            {
                Prompt = MessageFactory.Text($"Seus dados estão corretos?" + TextGlobal.Choice),
                RetryPrompt = MessageFactory.Text("Seus dados estão corretos?" + TextGlobal.ChoiceDig),
                Choices = ChoiceFactory.ToChoices(new List<string> { "Sim", "Não" }),
            };

            return await stepContext.PromptAsync(nameof(ChoicePrompt), promptOptions, cancellationToken);
        }

        /// <summary>
        /// Passo responsável por continuar o diálogo ou reiniciar de acordo com a resposta obtida de InfoStepAsync.
        /// </summary>
        /// <param name="stepContext">Contexto de Specifications.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>DialogTurnResult com continuação do diálogo ou início do diálogo MainDialog.</returns>
        private async Task<DialogTurnResult> ConfirmDataAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            LicenseFields = (LicenseFields)stepContext.Values["LicenseFields"];
            stepContext.Values["choice"] = ((FoundChoice)stepContext.Result).Value;

            if (stepContext.Values["choice"].ToString().ToLower() == "sim")
            {
                return await stepContext.ContinueDialogAsync(cancellationToken);
            }
            else
            {
                await stepContext.Context.SendActivityAsync("Se os dados não estão corretos, teremos que repetir o processo.\r\n" +
                                                            "Caso o problema persista, entre em contato com nossa equipe de atendimento");
                return await stepContext.ReplaceDialogAsync(nameof(MainDialog), default, cancellationToken);
            }
        }

        /// <summary>
        /// Passo responsável por verificar se veículo possui RNTRC. 
        /// </summary>
        /// <param name="stepContext">Contexto de Specifications.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>DialogTurnResult com continuação do diálogo ou início do diálogo RNTRCDialog.</returns>
        private async Task<DialogTurnResult> TypeVehicleAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            LicenseFields = (LicenseFields)stepContext.Values["LicenseFields"];

            if (VehicleLicenseRNTRC.ValidationVehicleType(LicenseFields.temRNTRC) == true)
                return await stepContext.BeginDialogAsync(nameof(RNTRCDialog), LicenseFields, cancellationToken);
            else
            {
                LicenseFields.dataValidadeRNTRC = "0";
                return await stepContext.ContinueDialogAsync(cancellationToken);
            }
        }

        /// <summary>
        /// Passo responsável por verificar se veículo possui chamada para Recall.
        /// </summary>
        /// <param name="stepContext">Contexto de Specifications.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>DialogTurnResult com continuação do diálogo ou início do diálogo RecallDialog.</returns>
        private async Task<DialogTurnResult> RecallVehicleStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            LicenseFields = (LicenseFields)stepContext.Values["LicenseFields"];
            string x = VehicleLicenseRNTRC.ValidationDate(stepContext.Result.ToString());

            LicenseFields.dataValidadeRNTRC = x;

            if (VehicleLicenseRecall.ValidationVehicleRecall(LicenseFields.recallCodigo) == true)
                return await stepContext.BeginDialogAsync(nameof(RecallDialog), LicenseFields, cancellationToken);
            else
                return await stepContext.ContinueDialogAsync(cancellationToken);

        }

        /// <summary>
        /// Passo responsável por verificar se veículo possui isenção.
        /// </summary>
        /// <param name="stepContext">Contexto de Specifications.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>DialogTurnResult com continuação do diálogo ou início do diálogo ExemptionDialog.</returns>
        private async Task<DialogTurnResult> ExemptionVehicleStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            LicenseFields = (LicenseFields)stepContext.Values["LicenseFields"];

            if (VehicleLicenseExemption.Exemption(LicenseFields.temIsençãoIPVA) == true)
            {
                return await stepContext.BeginDialogAsync(nameof(ExemptionDialog), LicenseFields, cancellationToken);
            }
            else
                return await stepContext.ContinueDialogAsync(cancellationToken);
        }

        /// <summary>
        /// Passo responsável por verificar se veículo possui pendências.
        /// </summary>
        /// <param name="stepContext">Contexto de Specifications.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>DialogTurnResult com continuação do diálogo ou início do diálogo PendencyDialog.</returns>
        private async Task<DialogTurnResult> InvoiceVehicleStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            LicenseFields = (LicenseFields)stepContext.Values["LicenseFields"];

            var answer = stepContext.Parent.Context.Activity.Text.ToLower();
            if (answer == "sim" || answer == "1")
                LicenseFields.IsencaoIPVA = "S";
            else
                LicenseFields.IsencaoIPVA = "N";


            if (VehicleLicense.Pendency(LicenseFields.anoLicenciamento) == true)
                return await stepContext.BeginDialogAsync(nameof(PendencyDialog), LicenseFields, cancellationToken);
            else
                return await stepContext.ContinueDialogAsync(cancellationToken);
        }

        /// <summary>
        /// Passo responsável por Efetuar Licenciamento. 
        /// </summary>
        /// <param name="stepContext">Contexto de Specifications.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>DialogTurnResult com continuação do diálogo ou finalização de diálogo (Retorno ao passo AskStepAsync em MainDialog)</returns>
        private async Task<DialogTurnResult> VehicleStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            LicenseFields = (LicenseFields)stepContext.Values["LicenseFields"];

            var answer = stepContext.Parent.Context.Activity.Text.ToLower();

            // Verifica o contexto oriundo do passo anterior ou do PendencyDialog
            if ((answer == "sim" || answer == "1") && LicenseFields.contadorAnoLicenciamento == 1) // "Sim" vindo de PendencyStepAsync caso possua 1 ano
                LicenseFields.exercicio = 2021;
            else if ((answer == "2020" || answer == "1") && LicenseFields.contadorAnoLicenciamento != 1) // "2020" ou "1" vindo de 
                LicenseFields.exercicio = 2020;
            else if ((answer == "não" || answer == "2") && LicenseFields.contadorAnoLicenciamento != 1)
                LicenseFields.exercicio = 2020;
            else if ((answer == "não" || answer == "2") && LicenseFields.contadorAnoLicenciamento == 1)
                return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);


            var result = await EfetuarServicoLicenciamento.efetuarServicoLicenciamento(
            LicenseFields.plataforma,
            Convert.ToDouble(LicenseFields.renavamOut),
            Convert.ToDouble(LicenseFields.codSegurancaOut),
            LicenseFields.restricao,
            LicenseFields.exercicio,
            LicenseFields.tipoAutorizacaoRNTRCOut,
            Convert.ToDouble(LicenseFields.nroAutorizacaoRNTRCOut),
            LicenseFields.dataValidadeRNTRC,
            LicenseFields.IsencaoIPVA,
            LicenseFields.tipoDocumentoIn
            );

            if (result.erro.codigo != 0)
            {
                await stepContext.Context.SendActivityAsync("Estou realizando correções em meu sistema e não foi possível gerar o PDF. Por favor, volte mais tarde para efetuar seu serviço" +
                                                                ", tente pelo nosso portal ou entre em contato com nossa equipe de atendimento.");
                return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
            }
            else
            {
                lock (LicenseFields)
                {
                    LicenseFields.codigoRetorno = result.codigoRetorno;
                    LicenseFields.erroCodigo = result.erro.codigo;
                    LicenseFields.erroMensagem = result.erro.mensagem;
                    LicenseFields.erroTrace = result.erro.trace;
                    LicenseFields.cpfProcurador = result.cpfProcurador;
                    LicenseFields.numeroDocumento = result.numeroDocumento;
                    LicenseFields.tipoDocumentoOut = result.tipoDocumento;
                    LicenseFields.cor = result.cor;
                    LicenseFields.vetTaxas = result.vetTaxas;
                    LicenseFields.vetDescDebitos = result.vetDescDebitos;
                    LicenseFields.dataProcessamento = result.dataProcessamento;
                    LicenseFields.exercicio = result.exercicio;
                    LicenseFields.ind = result.ind;
                    LicenseFields.marcaModelo = result.marcaModelo;
                    LicenseFields.nome = result.nome;
                    LicenseFields.placa = result.placa;
                    LicenseFields.renavamOut = result.renavam.ToString();
                    LicenseFields.tipo = result.tipo;
                    LicenseFields.vetValorA = result.vetValorA;
                    LicenseFields.valorApagar = result.valorApagar;
                    LicenseFields.vencimento = result.vencimento;
                    LicenseFields.agencia = result.agencia;
                    LicenseFields.mensagem1 = result.mensagem1;
                    LicenseFields.mensagem2 = result.mensagem2;
                    LicenseFields.mensagem3 = result.mensagem3;
                    LicenseFields.mensagem4 = result.mensagem4;
                    LicenseFields.mensagem5 = result.mensagem5;
                    LicenseFields.totalA = result.mensagem5;
                    LicenseFields.linhaDig = result.linhaDig;
                    LicenseFields.linhaCodBarra = result.linhaCodBarra;
                    LicenseFields.codBarra = result.codBarra;
                    LicenseFields.asBace1 = result.asBace1;
                    LicenseFields.indDescricao = result.indDescricao;
                    LicenseFields.vetDescInfracao = result.vetDescInfracao;
                    LicenseFields.indMensagem = result.indMensagem;
                    LicenseFields.vetDuaMensagem = result.vetDuaMensagem;
                    LicenseFields.chassiSNG = result.chassiSNG;
                    LicenseFields.tituloVenc = result.tituloVenc;
                    LicenseFields.datsVenc = result.datsVenc;
                    LicenseFields.indParc = result.indParc;
                    LicenseFields.vetDuaParc = result.vetDuaParc;
                    LicenseFields.vetValorA1Parc = result.vetValorA1Parc;
                    LicenseFields.vetLinhaDigParc = result.vetLinhaDigParc;
                    LicenseFields.vetLinhaCodBarra = result.vetLinhaCodBarra;
                    LicenseFields.vetCodBarraParc = result.vetCodBarraParc;
                    LicenseFields.vetASBACE1Parc = result.vetASBACE1Parc;
                    LicenseFields.vetValorA2Parc = result.vetValorA2Parc;
                    LicenseFields.vetValorA3Parc = result.vetValorA3Parc;
                    LicenseFields.vetTotalAParc = result.vetTotalAParc;
                    LicenseFields.vetVencimentoParc = result.vetVencimentoParc;
                    LicenseFields.flagParc1A = result.flagParc1A;
                    LicenseFields.flagParc2A = result.flagParc2A;
                    LicenseFields.flagParc3A = result.flagParc3A;
                    LicenseFields.cpfCnpjPagador = result.cpfCnpjPagador;
                    LicenseFields.enderecoPagador = result.enderecoPagador;
                    LicenseFields.cepPagador = result.cepPagador;
                    LicenseFields.bairroPagador = result.bairroPagador;
                    LicenseFields.municipioPagador = result.municipioPagador;
                    LicenseFields.ufPagador = result.ufPagador;
                    LicenseFields.nossoNumero = result.nossoNumero;
                    LicenseFields.codBarra = result.codBarra;
                    LicenseFields.linhaDig = result.linhaDig;
                    LicenseFields.linhaCodBarra = result.linhaCodBarra;
                }
                stepContext.Values["LicenseFields"] = LicenseFields;
            }

            return await stepContext.ContinueDialogAsync(cancellationToken);
        }

        /// <summary>
        /// Passo responsável por gerar e disponibilizar PDF e código de barras. 
        /// </summary>
        /// <param name="stepContext">Contexto de Specifications.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>DialogTurnResult com continuação do diálogo ou início do diálogo PendencyDialog.</returns>
        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {

            LicenseFields = (LicenseFields)stepContext.Values["LicenseFields"];

            var info = "Aqui está sua via para pagamento no " + LicenseFields.Banco + "!\r\n" +
                        "Estou disponibilizando seu documento ou diretamente o código de barras para facilitar seu pagamento!\r\n" +
                        "Após a compensação do pagamento você pode voltar aqui para emitir seu Documento de Circulação (CRLV-e).";

            var codeF = LicenseFields.codBarra;
            var codeD = LicenseFields.linhaDig;

            if (LicenseFields.tipoDocumentoIn == "F")
            {
                var reply = MessageFactory.Text(info);
                reply.Attachments = new List<Attachment>() { PdfProvider.Disponibilizer(GeneratePdfCompensacao.GenerateInvoice2(LicenseFields), "Ficha_de_compensacao") };
                await stepContext.Context.SendActivityAsync(reply);
                await stepContext.Context.SendActivityAsync(codeF);
            }
            else
            {
                var reply = MessageFactory.Text(info);
                reply.Attachments = new List<Attachment>() { PdfProvider.Disponibilizer(GeneratePdfDUA.GenerateInvoice2(LicenseFields), "DUA") };
                await stepContext.Context.SendActivityAsync(reply);
                await stepContext.Context.SendActivityAsync(codeD);
            }
            return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
        }

    }
}
