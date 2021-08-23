// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Threading;
using System.Threading.Tasks;
using CoreBot.Fields;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;

namespace Microsoft.BotBuilderSamples.Dialogs
{
    public class CancelAndHelpDialog : ComponentDialog
    {
        LicenseFields LicenseFields;

        public CancelAndHelpDialog(string id)
            : base(id)
        {
        }

        protected override async Task<DialogTurnResult> OnBeginDialogAsync(DialogContext innerDc, object options, CancellationToken cancellationToken = default(CancellationToken))
        {
            var result = await InterruptAsync(innerDc, cancellationToken);
            if (result != null)
            {
                return result;
            }

            return await base.OnBeginDialogAsync(innerDc, options, cancellationToken);
        }

        protected override async Task<DialogTurnResult> OnContinueDialogAsync(DialogContext innerDc, CancellationToken cancellationToken)
        {
            var result = await InterruptAsync(innerDc, cancellationToken);
            if (result != null)
            {
                return result;
            }

            return await base.OnContinueDialogAsync(innerDc, cancellationToken);
        }

        private async Task<DialogTurnResult> InterruptAsync(DialogContext innerDc, CancellationToken cancellationToken)
        {
            if (innerDc.Context.Activity.Type == ActivityTypes.Message)
            {
                var text = innerDc.Context.Activity.Text.ToLowerInvariant();

                switch (text)
                {
                    case "ajuda":
                    case "help":
                    case "?":
                        await innerDc.Context.SendActivityAsync($"Exibir Ajuda...", cancellationToken: cancellationToken);
                        return new DialogTurnResult(DialogTurnStatus.Waiting);

                    case "sair":
                    case "cancelar":
                    case "quit":
                        return await innerDc.ReplaceDialogAsync(nameof(MainDialog), LicenseFields, cancellationToken);
                    //await innerDc.Context.SendActivityAsync($"Cancelando", cancellationToken: cancellationToken);
                    //return await innerDc.CancelAllDialogsAsync();

                    case "menu":
                    case "voltar":
                    case "reiniciar":
                        //await innerDc.Context.SendActivityAsync($"Exibir Ajuda...", cancellationToken: cancellationToken);
                        return await innerDc.ReplaceDialogAsync(nameof(MainDialog), LicenseFields, cancellationToken);

                }
            }

            return null;
        }
    }
}
