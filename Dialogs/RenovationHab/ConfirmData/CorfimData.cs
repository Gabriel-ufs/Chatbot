// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Globalization;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CoreBot.Fields;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using CoreBot.Models.MethodsValidation.License;
using CoreBot.Models;

namespace Microsoft.BotBuilderSamples.Dialogs
{
    public class ConfirmData : CancelAndHelpDialog
    {
        RenovationFields RenovationFields;

        public ConfirmData()
        : base(nameof(ConfirmData))

        {

            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt), null, "pt-br"));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(new PortalDialog());
            AddDialog(new AddressConfirm());

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                ConfirmationData,
                OptionValidationConfirmData,

            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);

        }

        /// <summary>
        /// Passo responsável por captar o contexto do diálogo pai (EmissaoDialog), chama o webservice para a identificação do condutor e coleta de dados sobre o mesmo
        /// </summary>
        /// <param name="stepContext">Contexto do Consult</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>

        private async Task<DialogTurnResult> ConfirmationData (WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {

            WaterfallStepContext contextParent = (WaterfallStepContext)stepContext.Parent;
            RenovationFields = new RenovationFields();
            RenovationFields = (RenovationFields)contextParent.Values["RenovationFields"];
            stepContext.Values["RenovationFields"] = RenovationFields;

            CnhRenovation cnh = new CnhRenovation();

            var webResult = await cnh.obterRenovacao(RenovationFields.plataforma, RenovationFields.cpf, RenovationFields.identidade, RenovationFields.digitoID, RenovationFields.orgaoDescricao, RenovationFields.ufIdentidadeIn, RenovationFields.tipoDocumentoIn);


            RenovationFields.cursos = webResult.msgCurso;
            RenovationFields.escolaridade = webResult.vDescricaoEscolaridade;
            RenovationFields.mensagem = webResult.erro.mensagem;
            RenovationFields.erro = webResult.erro.codigo;
            
            if(RenovationFields.erro == 4 || RenovationFields.erro == 5)
            {
                await stepContext.Context.SendActivityAsync($"Ouve um erro na validação do seu processo: \r\n {RenovationFields.mensagem}");
                await stepContext.Context.SendActivityAsync("Para mais detalhes acesse ao portal através do link abaixo \r\n https://www.detran.se.gov.br/portal/?pg=atend_agendamento&pCod=338");

                return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
            }

            if(RenovationFields.cursos != "")
            {
                await stepContext.Context.SendActivityAsync("**ATENÇÃO!** \r\n Notei que o senhor(a) possui cursos especializados, para realizar sua renovação de cnh, acesse ao portal através do link abaixo");
                await stepContext.Context.SendActivityAsync("https://www.detran.se.gov.br/portal/?pg=cnh_renovacao");

                return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);

            }
            
            
            RenovationFields.sexo = webResult.vDescricaoSexo;
            RenovationFields.escolaridade = webResult.vDescricaoEscolaridade;
            RenovationFields.nacionalidade = webResult.vDescricaoNacionalidade;
            RenovationFields.ufNaturalidade = webResult.vUFnaturalidade;
            RenovationFields.localidadeNasc = webResult.vLocalidadeNascimento;
            RenovationFields.nomeUsuario = webResult.vNome;
            RenovationFields.categoria = webResult.vCategoria;
            RenovationFields.dataNasc = webResult.vDataNascimento;
            RenovationFields.escolaridadeCod = webResult.vCodigoEscolaridade;
            //RenovationFields.nomeMae = webResult.vNomeMae;
            //RenovationFields.nomePai = webResult.vNomePai;
            RenovationFields.sexoOut = webResult.vCodigoSexo;
            RenovationFields.nacionalidadeOut = webResult.vCodigoNacionalidade;
            RenovationFields.ufNaturalidade = webResult.vUFnaturalidade;
            RenovationFields.naturalidade = webResult.vLocalidadeNascimentoDescricao;
            RenovationFields.vetDescricaoLocal = webResult.vVetDescricaoLocaisProvasExames;
            RenovationFields.vetDescricaoSetor = webResult.vVetDescricaoSetor;
            RenovationFields.contadorSetor = Convert.ToInt32(webResult.vContadorSetor);


            RenovationFields.vetCodigoLocal = webResult.vVetCodigoLocaisProvasExames;
            RenovationFields.vetCodigoSetor = webResult.vVetCodigoSetor;


            switch (RenovationFields.categoria)
            {
                case "D":

                    await stepContext.Context.SendActivityAsync($"Notei que a categoria da sua CNH é {RenovationFields.categoria}. Caso tenha interesse em fazer uma redução de categoria, por favor acesse o portal a partir do link abaixo, lá o senhor(a) poderá fazer sua redução de categoria e finalizar o seu processo");
                    await stepContext.Context.SendActivityAsync("https://www.detran.se.gov.br/portal/?pg=cnh_renovacao");
                    await stepContext.Context.SendActivityAsync("Caso contrário basta continuar o diálogo");


                    await stepContext.Context.SendActivityAsync(MessageFactory.Text("Por favor, confira se seus dados estão corretos"));
                    await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Sexo: {RenovationFields.sexo} " +
                                                                                $"\r\n Escolaridade: {RenovationFields.escolaridade} " +
                                                                                $"\r\n Nacionalidade: {RenovationFields.nacionalidade}" +
                                                                                $"\r\n UF de Nascimento: {RenovationFields.ufNaturalidade} " +
                                                                                $"\r\n Município de Nascimento: {RenovationFields.naturalidade}"));

                    var promptOptionsD = new PromptOptions
                    {
                        Prompt = MessageFactory.Text("Seus dados estão precisando ser atualizados? \r\n   1. Sim, preciso atualiza-los (Ir ao Portal) \r\n   2. Não, meus dados estão corretos (Continuar)"),
                        RetryPrompt = MessageFactory.Text(TextGlobal.Desculpe + "Por favor, informe se seus dados precisam ser atualizados"),
                        Choices = ChoiceFactory.ToChoices(new List<string> { "Sim", "Não" }),
                    };

                    return await stepContext.PromptAsync(nameof(ChoicePrompt), promptOptionsD, cancellationToken);


                case "E":

                    await stepContext.Context.SendActivityAsync($"Notei que a categoria da sua CNH é {RenovationFields.categoria}. Caso tenha interesse em fazer uma redução de categoria, por favor acesse o portal a partir do link abaixo, lá o senhor(a) poderá fazer sua redução de categoria e finalizar o seu processo");
                    await stepContext.Context.SendActivityAsync("https://www.detran.se.gov.br/portal/?pg=cnh_renovacao");
                    await stepContext.Context.SendActivityAsync("Caso contrário basta continuar o diálogo");


                    await stepContext.Context.SendActivityAsync(MessageFactory.Text("Por favor, confira se seus dados estão corretos"));
                    await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Sexo: {RenovationFields.sexo} " +
                                                                                $"\r\n Escolaridade: {RenovationFields.escolaridade} " +
                                                                                $"\r\n Nacionalidade: {RenovationFields.nacionalidade}" +
                                                                                $"\r\n UF de Nascimento: {RenovationFields.ufNaturalidade} " +
                                                                                $"\r\n Município de Nascimento: {RenovationFields.naturalidade}"));

                    var promptOptionsE = new PromptOptions
                    {
                        Prompt = MessageFactory.Text("Seus dados estão precisando ser atualizados? \r\n   1. Sim, preciso atualiza-los (Ir ao Portal) \r\n   2. Não, meus dados estão corretos (Continuar)"),
                        RetryPrompt = MessageFactory.Text(TextGlobal.Desculpe + "Por favor, informe se seus dados precisam ser atualizados"),
                        Choices = ChoiceFactory.ToChoices(new List<string> { "Sim", "Não" }),
                    };

                    return await stepContext.PromptAsync(nameof(ChoicePrompt), promptOptionsE, cancellationToken);


                case "C":

                    await stepContext.Context.SendActivityAsync($"Notei que a categoria da sua CNH é {RenovationFields.categoria}. Caso tenha interesse em fazer uma redução de categoria, por favor acesse o portal a partir do link abaixo, lá o senhor(a) poderá fazer sua redução de categoria e finalizar o seu processo");
                    await stepContext.Context.SendActivityAsync("https://www.detran.se.gov.br/portal/?pg=cnh_renovacao");
                    await stepContext.Context.SendActivityAsync("Caso contrário basta continuar o diálogo");


                    await stepContext.Context.SendActivityAsync(MessageFactory.Text("Por favor, confira se seus dados estão corretos"));
                    await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Sexo: {RenovationFields.sexo} " +
                                                                                $"\r\n Escolaridade: {RenovationFields.escolaridade} " +
                                                                                $"\r\n Nacionalidade: {RenovationFields.nacionalidade}" +
                                                                                $"\r\n UF de Nascimento: {RenovationFields.ufNaturalidade} " +
                                                                                $"\r\n Município de Nascimento: {RenovationFields.naturalidade}"));

                    var promptOptionsC = new PromptOptions
                    {
                        Prompt = MessageFactory.Text("Seus dados estão precisando ser atualizados? \r\n   1. Sim, preciso atualiza-los (Ir ao Portal) \r\n   2. Não, meus dados estão corretos (Continuar)"),
                        RetryPrompt = MessageFactory.Text(TextGlobal.Desculpe + "Por favor, informe se seus dados precisam ser atualizados"),
                        Choices = ChoiceFactory.ToChoices(new List<string> { "Sim", "Não" }),
                    };

                    return await stepContext.PromptAsync(nameof(ChoicePrompt), promptOptionsC, cancellationToken);


                default:
                    await stepContext.Context.SendActivityAsync(MessageFactory.Text("Por favor, confira se seus dados estão corretos"));
                    await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Sexo: {RenovationFields.sexo} " +
                                                                                $"\r\n Escolaridade: {RenovationFields.escolaridade} " +
                                                                                $"\r\n Nacionalidade: {RenovationFields.nacionalidade}" +
                                                                                $"\r\n UF de Nascimento: {RenovationFields.ufNaturalidade} " +
                                                                                $"\r\n Município de Nascimento: {RenovationFields.naturalidade}"));

                    var promptOptions = new PromptOptions
                    {
                        Prompt = MessageFactory.Text("Seus dados estão precisando ser atualizados? \r\n   1. Sim, preciso atualiza-los (Ir ao Portal) \r\n   2. Não, meus dados estão corretos (Continuar)"),
                        RetryPrompt = MessageFactory.Text(TextGlobal.Desculpe + "Por favor, informe se seus dados precisam ser atualizados"),
                        Choices = ChoiceFactory.ToChoices(new List<string> { "Sim", "Não" }),
                    };

                    return await stepContext.PromptAsync(nameof(ChoicePrompt), promptOptions, cancellationToken);
            }

        }


        /// <summary>
        /// Passo responsável por receber o resultado do passo anterior que valida os dados informados se a resposta for "não" chama o o dialogo AddressConfirm
        /// caso seja "sim" é iniciado o dialogo PortalDialog que encaminha o usuário para o Portal de atendimento
        /// </summary>
        /// <param name="stepContext">Contexto do Consult</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// 

        private async Task<DialogTurnResult> OptionValidationConfirmData(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            RenovationFields = (RenovationFields)stepContext.Values["RenovationFields"];
            stepContext.Values["choice"] = ((FoundChoice)stepContext.Result).Value;


            if (stepContext.Values["choice"].ToString().ToLower() == "sim")
            {
                stepContext.Values["RenovationFields"] = RenovationFields;
                return await stepContext.BeginDialogAsync(nameof(PortalDialog), RenovationFields, cancellationToken);
            }
            else return await stepContext.ReplaceDialogAsync(nameof(AddressConfirm), default, cancellationToken);
        }

    }
}