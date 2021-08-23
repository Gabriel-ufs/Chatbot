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
using CoreBot.Models.Generate;
using CoreBot.Models;
using System.Linq;

namespace Microsoft.BotBuilderSamples.Dialogs
{
    public class EmissaoDialog : CancelAndHelpDialog
    {
        RenovationFields RenovationFields;

        public EmissaoDialog()
        : base(nameof(EmissaoDialog))

        {

            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt), null, "pt-br"));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(new ConfirmData());

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                UfRequest,
                ValidationUF,
                OrgaoEmissorStep,
                DescricaoOrgao,
                ValidationOrgao,

            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);

        }

        /// <summary>
        /// Passo responsável por captar o contexto do diálogo pai (NumIdDialog), e perguntar ao usuário a UF de emissão da identidade
        /// </summary>
        /// <param name="stepContext">Contexto do RootRenovationDialog</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<DialogTurnResult> UfRequest (WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // Busca pelo contexto do diálogo pai.
            WaterfallStepContext contextParent = (WaterfallStepContext)stepContext.Parent;
            RenovationFields = new RenovationFields();
            RenovationFields = (RenovationFields)contextParent.Values["RenovationFields"];
            stepContext.Values["RenovationFields"] = RenovationFields;

            await stepContext.Context.SendActivityAsync("Por favor, informe a UF de emissão da sua identidade, Ex: SE");
            var uf = MessageFactory.Text(null, InputHints.ExpectingInput);
            uf.ToString().ToUpper();

            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = uf}, cancellationToken);
        }


        /// <summary>
        /// Passo responsável por receber a UF informada além de valida-la caso seja válida o dialogo continua, caso não o usuário informa uma novamente a UF
        /// </summary>
        /// <param name="stepContext">Contexto do RootRenovationDialog</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<DialogTurnResult> ValidationUF(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var RenovationFields = (RenovationFields)stepContext.Values["RenovationFields"];
            RenovationFields.ufIdentidadeIn = (string)stepContext.Result;

            

            if (RenovationFields.ufSiglas.Contains(RenovationFields.ufIdentidadeIn.ToUpper()))
            {
                return await stepContext.ContinueDialogAsync(cancellationToken);
            }

            else if(RenovationFields.contUF < 3)
            {
                RenovationFields.contUF++;
                await stepContext.Context.SendActivityAsync("Por favor, digite uma UF válida");
                return await stepContext.ReplaceDialogAsync(nameof(EmissaoDialog), default, cancellationToken);
            }

            else
            {
                await stepContext.Context.SendActivityAsync("A UF informada é inválida. Verifique no documento e tente novamente.");
                return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
            }
        }

        /// <summary>
        /// Passo responsável por perguntar o orgão emissor da identidade
        /// </summary>
        /// <param name="stepContext">Contexto do RootRenovationDialog</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<DialogTurnResult> OrgaoEmissorStep (WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {


            await stepContext.Context.SendActivityAsync("Escolha a seguir qual o orgão emissor da sua identidade");

            string[] vetOrgaosEmissores = { "SSP", "Conselhos", "Institutos", "Ministérios", "Polícia", "Outras Secretarias", "Outros" };


            var promptOptions = new PromptOptions
            {
                Prompt = MessageFactory.Text("Escolha entre: "),
                RetryPrompt = MessageFactory.Text(TextGlobal.Desculpe + "Escolha entre as opções abaixo (digite um número de 1 a 7):"),
                Choices = ChoiceFactory.ToChoices(vetOrgaosEmissores),
            };

            return await stepContext.PromptAsync(nameof(ChoicePrompt), promptOptions, cancellationToken);

        }

        /// <summary>
        /// Passo responsável por receber o tipo de orgão emissor selecionado pelo usuário, caso seja ssp se inicia o dialogo ConfirmData
        /// caso seja qualquer outra opção o usuário irá selecionar uma das opções listadas antes de iniciar o diáçogo ConfirmData
        /// </summary>
        /// <param name="stepContext">Contexto do RootRenovationDialog</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<DialogTurnResult> DescricaoOrgao (WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var RenovationFields = (RenovationFields)stepContext.Values["RenovationFields"];
            RenovationFields.tipoOrgao = ((FoundChoice)stepContext.Result).Value;

            switch (RenovationFields.tipoOrgao)
            {
                case "SSP":
                    RenovationFields.orgaoDescricao = "SSP - SECRETARIA DE SEGURANCA PUBLICA";
                    return await stepContext.BeginDialogAsync(nameof(ConfirmData), RenovationFields, cancellationToken);

                case "Conselhos":
                    List<string> optionsCon = new List<string>();
                    foreach (string value in RenovationFields.ConselhosIn)
                    {
                        if (value != "")
                        {
                            optionsCon.Add(value);
                        }
                    }

                    var promptOptionsCon = new PromptOptions
                    {
                        Prompt = MessageFactory.Text($"Escolha o orgão de emissão da sua Identidade "),
                        RetryPrompt = MessageFactory.Text("Por Favor, digite um numero de 1 a 33"),
                        Choices = ChoiceFactory.ToChoices(optionsCon),

                    };
                    return await stepContext.PromptAsync(nameof(ChoicePrompt), promptOptionsCon, cancellationToken);

                case "Ministérios":
                    List<string> optionsMin = new List<string>();
                    foreach (string value in RenovationFields.MinisteriosIn)
                    {
                        if (value != "")
                        {
                            optionsMin.Add(value);
                        }
                    }
                    
                    var promptOptionsMin = new PromptOptions
                    {
                        Prompt = MessageFactory.Text($"Escolha o orgão de emissão da sua Identidade "),
                        RetryPrompt = MessageFactory.Text("Por Favor, digite um numero de 1 a 9"),
                        Choices = ChoiceFactory.ToChoices(optionsMin),

                    };
                    return await stepContext.PromptAsync(nameof(ChoicePrompt), promptOptionsMin, cancellationToken);

                case "Institutos":
                    List<string> optionsIns = new List<string>();
                    foreach (string value in RenovationFields.InstitutosIn)
                    {
                        if (value != "")
                        {
                            optionsIns.Add(value);
                        }
                    }

                    var promptOptionsIns = new PromptOptions
                    {
                        Prompt = MessageFactory.Text($"Escolha o orgão de emissão da sua Identidade "),
                        RetryPrompt = MessageFactory.Text("Por Favor, digite um numero de 1 a 14"),
                        Choices = ChoiceFactory.ToChoices(optionsIns),

                    };
                    return await stepContext.PromptAsync(nameof(ChoicePrompt), promptOptionsIns, cancellationToken);



                case "Polícia":
                    List<string> optionsPol = new List<string>();
                    foreach (string value in RenovationFields.PoliciaIn)
                    {
                        if (value != "")
                        {
                            optionsPol.Add(value);
                        }
                    }

                    var promptOptionsPol = new PromptOptions
                    {
                        Prompt = MessageFactory.Text($"Escolha o orgão de emissão da sua Identidade "),
                        RetryPrompt = MessageFactory.Text("Por Favor, digite um numero de 1 a 18"),
                        Choices = ChoiceFactory.ToChoices(optionsPol),

                    };
                    return await stepContext.PromptAsync(nameof(ChoicePrompt), promptOptionsPol, cancellationToken);


                case "Outras Secretarias":
                    List<string> optionsSec = new List<string>();
                    foreach (string value in RenovationFields.OutrasSecsIn)
                    {
                        if (value != "")
                        {
                            optionsSec.Add(value);
                        }
                    }

                    var promptOptionsSec = new PromptOptions
                    {
                        Prompt = MessageFactory.Text($"Escolha o orgão de emissão da sua Identidade "),
                        RetryPrompt = MessageFactory.Text("Por Favor, digite um numero de 1 a 40"),
                        Choices = ChoiceFactory.ToChoices(optionsSec),

                    };
                    return await stepContext.PromptAsync(nameof(ChoicePrompt), promptOptionsSec, cancellationToken);

                case "Outros":
                    List<string> optionsOutros = new List<string>();
                    foreach (string value in RenovationFields.OutrasSecsIn)
                    {
                        if (value != "")
                        {
                            optionsOutros.Add(value);
                        }
                    }

                    var promptOptionsOutros = new PromptOptions
                    {
                        Prompt = MessageFactory.Text($"Escolha o orgão de emissão da sua Identidade "),
                        RetryPrompt = MessageFactory.Text("Por Favor, digite um numero de 1 a 40"),
                        Choices = ChoiceFactory.ToChoices(optionsOutros),

                    };
                    return await stepContext.PromptAsync(nameof(ChoicePrompt), promptOptionsOutros, cancellationToken);



                default:
                    return await stepContext.ReplaceDialogAsync(nameof(EmissaoDialog), default, cancellationToken);
            }
        }

        /// <summary>
        /// Passo responsável por receber a descrição do orgão emissor e chamar o dialogo ConfirmData
        /// </summary>
        /// <param name="stepContext">Contexto do RootRenovationDialog</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
         

        private async Task<DialogTurnResult> ValidationOrgao (WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var RenovationFields = (RenovationFields)stepContext.Values["RenovationFields"];

            if ((FoundChoice)stepContext.Result == null) 
            {
                return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
            }
            else
            {
                RenovationFields.orgaoDescricao = ((FoundChoice)stepContext.Result).Value;
            }
            

            return await stepContext.BeginDialogAsync(nameof(ConfirmData), RenovationFields, cancellationToken);
        }

    }
}