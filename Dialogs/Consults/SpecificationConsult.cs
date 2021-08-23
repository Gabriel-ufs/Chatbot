// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdaptiveCards;
using CoreBot.Fields;
using CoreBot.Models;
using CoreBot.Models.FileManagement;
using CoreBot.Models.Generate;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;
using Newtonsoft.Json.Linq;

namespace Microsoft.BotBuilderSamples.Dialogs
{
    public class SpecificationConsult : CancelAndHelpDialog
    {

        private ConsultFields ConsultFields;


        public SpecificationConsult()
            : base(nameof(SpecificationConsult))
        {
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt)));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                FinalStepAsync,
            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {

            WaterfallStepContext contextParent = (WaterfallStepContext)stepContext.Parent;
            ConsultFields = (ConsultFields)contextParent.Values["ConsultFields"];
            stepContext.Values["ConsultFields"] = ConsultFields;


            ConsultFields = (ConsultFields)stepContext.Values["ConsultFields"];

            var info = "Aqui está o resultado da sua consulta referente aos dados da sua habilitação";

            //var codeF = ProcessFields.categoria;
            //var codeD = ProcessFields.cnh;

            if ("a" == "F")
            {
                var reply = MessageFactory.Text(info);
                reply.Attachments = new List<Attachment>() { PdfProvider.Disponibilizer(GeneratePdfConsultaProcesso.GenerateInvoice2(ConsultFields), "Consulta de Processo") };
                await stepContext.Context.SendActivityAsync(reply);
                //await stepContext.Context.SendActivityAsync(codeF);
            }
            else
            {
                var reply = MessageFactory.Text(info);
                reply.Attachments = new List<Attachment>() { PdfProvider.Disponibilizer(GeneratePdfConsultaProcesso.GenerateInvoice2(ConsultFields), "Consulta de Processo") };
                await stepContext.Context.SendActivityAsync(reply);
                //await stepContext.Context.SendActivityAsync(codeD);
            }
            return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);



        }
    }
}
