
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CoreBot.Fields;
using CoreBot.Models;
using CoreBot.Models.Generate;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;
using CoreBot.Services.WSDLService.realizarRenovacaoHabilitacao;
using System;

namespace Microsoft.BotBuilderSamples.Dialogs
{
    public class MailAndPayDialog : CancelAndHelpDialog
    {
        RenovationFields RenovationFields;

        public MailAndPayDialog()
        : base(nameof(MailAndPayDialog))

        {

            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt), null, "pt-br"));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                MailStep,
                WorkDeclaration,
                ValidationWorkDeclararion,
                realizaRenov,
                DuaGeneration,


            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        /// <summary>
        /// Passo responsável por captar o contexto do diálogo pai (LocalChoiceDialog), e perguntar ao usuário qual o local ele deseja pegar sua CNH
        /// </summary>
        /// <param name="stepContext">Contexto do Consult</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>

        private async Task<DialogTurnResult> MailStep (WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            WaterfallStepContext contextParent = (WaterfallStepContext)stepContext.Parent;
            RenovationFields = new RenovationFields();
            RenovationFields = (RenovationFields)contextParent.Values["RenovationFields"];
            stepContext.Values["RenovationFields"] = RenovationFields;


            await stepContext.Context.SendActivityAsync("Escolha a seguir o local onde deseja receber sua CNH.");

            //RenovationFields.vetDescricaoSetor.SetValue("DETRAN SEDE SETAC", 0);


            List<string> optionsCon = new List<string>();
            foreach (string value in RenovationFields.vetDescricaoSetor)
            {
                if (value != null && value != "")
                {
                    optionsCon.Add(value);
                }
            }
            

            var promptOptions = new PromptOptions
            {
                Prompt = MessageFactory.Text("Escolha entre: "),
                RetryPrompt = MessageFactory.Text(TextGlobal.Desculpe + "Escolha entre (digite um número de 1 a 14): "),
                Choices = ChoiceFactory.ToChoices(optionsCon),
            };

            return await stepContext.PromptAsync(nameof(ChoicePrompt), promptOptions, cancellationToken);
        }


        /// <summary>
        /// Passo responsável por receber a resposta do passo anterior e por perguntar se o usuário tem interesse em exercer atividade remunerada 
        /// </summary>
        /// <param name="stepContext">Contexto do Consult</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>

        private async Task<DialogTurnResult> WorkDeclaration (WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {

            RenovationFields = (RenovationFields)stepContext.Values["RenovationFields"];
            RenovationFields.localEntrega = ((FoundChoice)stepContext.Result).Value;

            int index = Array.IndexOf(RenovationFields.vetDescricaoSetor, RenovationFields.localEntrega);
            RenovationFields.setorEntrega = RenovationFields.vetCodigoSetor[index];


            await stepContext.Context.SendActivityAsync("Declaração de Exercício de atividade remunerada");

            var promptOptions = new PromptOptions
            {
                Prompt = MessageFactory.Text("Você exerce ou deseja exercer atividade de transporte remunerada?  \r\n  1. Sim   \r\n  2. Não"),
                RetryPrompt = MessageFactory.Text(TextGlobal.Desculpe + "Você exerce ou deseja exercer atividade de transporte remunerada?  \r\n  1. Sim   \r\n  2. Não"),
                Choices = ChoiceFactory.ToChoices(new List<string> { "Sim", "Não" }),
            };

            return await stepContext.PromptAsync(nameof(ChoicePrompt), promptOptions, cancellationToken);
        }


        private async Task<DialogTurnResult> ValidationWorkDeclararion (WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            RenovationFields = (RenovationFields)stepContext.Values["RenovationFields"];
            RenovationFields.trabalhoRemunerado = ((FoundChoice)stepContext.Result).Value;

            if (RenovationFields.trabalhoRemunerado == "Sim")
            {
                RenovationFields.trabalhoRemunerado = "S";
                RenovationFields.txtAtvRemunerada = "EXERÇO";
            }
            else
            {
                RenovationFields.trabalhoRemunerado = "N";
                RenovationFields.txtAtvRemunerada = "NÃO EXERÇO";
            }



            return await stepContext.ContinueDialogAsync(cancellationToken);
        }


        private async Task<DialogTurnResult> realizaRenov (WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var result = await realizarRenovacaoHabilitacao.realizarServicoRenovacaoHabilitacao(
                
                    RenovationFields.plataforma,
                    RenovationFields.cpf,
                    RenovationFields.tipoDocumentoIn,
                    RenovationFields.localEntrega,
                    RenovationFields.TipoEndereco,
                    RenovationFields.endereco,
                    RenovationFields.numeroEndereco,
                    RenovationFields.complemento,
                    RenovationFields.bairro,
                    RenovationFields.municipio,
                    RenovationFields.cep,
                    RenovationFields.uf,
                    RenovationFields.telefone,
                    Convert.ToInt32(RenovationFields.setorEntrega),// ---
                    RenovationFields.email,
                    RenovationFields.ddd,
                    RenovationFields.nomeUsuario,
                    RenovationFields.categoria,
                    RenovationFields.TipoDocPrimeiraHab,//
                    RenovationFields.NumeroDocPrimeiraHab,//
                    RenovationFields.DigitoPrimeiraHab,//
                    RenovationFields.orgaoEmissorPrimeiraHab,//
                    RenovationFields.UfDocPrimeiraHab,//
                    RenovationFields.nomeMae,
                    RenovationFields.nomePai,
                    RenovationFields.sexoOut,
                    Convert.ToInt32(RenovationFields.nacionalidadeOut),
                    RenovationFields.ufNaturalidade,
                    RenovationFields.localidadeNasc,
                    RenovationFields.naturalidade,//
                    Convert.ToInt32(RenovationFields.dataNasc),
                    Convert.ToInt32(RenovationFields.escolaridadeCod),//
                    RenovationFields.deficienciaFisica,
                    RenovationFields.ProvaCursoRenovacao,//
                    RenovationFields.trabalhoRemunerado,
                    RenovationFields.UfPrimeiraHab,//
                    RenovationFields.datapriemirahab,
                    RenovationFields.SetorVirtual //
                    
                );


            RenovationFields.numeroDocumento = result.vNumeroDocumento;
            RenovationFields.dataProcessamento = result.vDataProcessamento;
            RenovationFields.vDataEmissaoCnhField = result.vDataEmissaoCnh;
            RenovationFields.dataVenc = result.vVencimento;
            RenovationFields.renach = result.vFormularioRenach;
            RenovationFields.dataNasc = result.vDataNascimento;

            RenovationFields.vDocArrecadacaoField = result.vDocArrecadacao;


            RenovationFields.vVetDescricaoDebitosField = result.vVetDescricaoDebitos;
            RenovationFields.vVetTaxasField = result.vVetTaxas;
            RenovationFields.linhaCodBarra = result.vLinhaCodBarra; // numero do codigo de barra sem espaços
            RenovationFields.vLinhaDigField = result.vLinhaDig; // numero do codigo de barra com espaços
            RenovationFields.agencia = result.vAgencia;
            RenovationFields.valorPagar = result.vValorApagar;


            RenovationFields.vProcessoField = result.vProcesso;
            RenovationFields.vFlagSenhaProcessoField = result.vFlagSenhaProcesso;
            RenovationFields.identidade = result.vIdentidade;


            return await stepContext.ContinueDialogAsync(cancellationToken);

        }

        /// <summary>
        /// Passo responsável por disponibilizar o pdf do documento para que o usuário possa efetuar o pagamento do serviço de renovação da CNH
        /// </summary>
        /// <param name="stepContext">Contexto do Consult</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>


        private async Task<DialogTurnResult> DuaGeneration (WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            RenovationFields = (RenovationFields)stepContext.Values["RenovationFields"];

            await stepContext.Context.SendActivityAsync("Estes são os dados para que você possa acompanhar o seu serviço no portal");
            await stepContext.Context.SendActivityAsync($"Senha: {RenovationFields.vFlagSenhaProcessoField} \r\n Número do Processo: {RenovationFields.vProcessoField}");

            var info = "Aqui está o documento para efetuar o pagamento da sua renovação";
            var message = "Aqui está o comprovante de requerimento de serviço disponível para download";

            if (RenovationFields.tipoDocumentoIn == "F")
            {
                //pdf do boleto
                var reply = MessageFactory.Text(info);
                reply.Attachments = new List<Attachment>() { PdfProvider.Disponibilizer(GenerateBoletoRenov.GenerateInvoice2(RenovationFields), "Ficha_de_compensacao") };
                await stepContext.Context.SendActivityAsync(reply);
                //pdf do requerimento
                var requerimento = MessageFactory.Text(message);
                requerimento.Attachments = new List<Attachment>() { PdfProvider.Disponibilizer(RequerimentoRenov.GenerateInvoice2(RenovationFields), "Requerimento") };
                await stepContext.Context.SendActivityAsync(requerimento);

            }

            else
            {
                //pdf DUA
                var reply = MessageFactory.Text(info);
                reply.Attachments = new List<Attachment>() { PdfProvider.Disponibilizer(GenerateDuaRenov.GenerateInvoice2(RenovationFields), "DUA") };
                await stepContext.Context.SendActivityAsync(reply);
                //pdf requerimento
                var requerimento = MessageFactory.Text(message);
                requerimento.Attachments = new List<Attachment>() { PdfProvider.Disponibilizer(RequerimentoRenov.GenerateInvoice2(RenovationFields), "Requerimento") };
                await stepContext.Context.SendActivityAsync(requerimento);

            }
            return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
        }

    }
}