using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using CoreBot.Fields;
using CoreBot.Components.Widgets;
using CoreBot.Components.Functions;

namespace Microsoft.BotBuilderSamples.Dialogs
{
    /// <summary>
    /// Diálogo responsável por solicitar o Código de Segurança caso o usuário tenha entrado com Renavam e possua o Código de Segurança.
    /// </summary>
    public class RequiredSecureCodeLicenseDialog : CancelAndHelpDialog
    {
        LicenseFields LicenseFields;
        public RequiredSecureCodeLicenseDialog()
            : base(nameof(RequiredSecureCodeLicenseDialog))
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
        /// Passo inicial responsável por obter o contexto do diálogo pai (SecureCode) e
        /// solicitar que o usuário escreva o código de segurança.
        /// </summary>
        /// <param name="stepContext">Contexto de RequiredSecureCodeLicenses.</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<DialogTurnResult> SecureCodeRequiredStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // Busca pelo contexto do diálogo pai.
            WaterfallStepContext contextParent = (WaterfallStepContext)stepContext.Parent;
            LicenseFields = (LicenseFields)contextParent.Values["LicenseFields"];
            stepContext.Values["LicenseFields"] = LicenseFields;

            // Geração da imagem do CRLVe.
            ImageCard ImageCard = new ImageCard();
            await stepContext.Context.SendActivityAsync(ImageCard.addImageCardSecureCode(), cancellationToken);

            // Geração de botão com link.
            ButtonCard expandButton = new ButtonCard();
            await stepContext.Context.SendActivityAsync(expandButton.addButtonExpandSecureCode(LicenseFields), cancellationToken);

            await stepContext.Context.SendActivityAsync(MessageFactory.Text("Informe o CÓDIGO DE SEGURANÇA"), cancellationToken);
            var secureCode = MessageFactory.Text(null, InputHints.ExpectingInput);
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = secureCode }, cancellationToken);
        }

        /// <summary>
        /// Passo responsável por Comparar o código de Segurança digitado pelo usuário com o retornado pelo WebService.
        /// </summary>
        /// <param name="stepContext">Contexto de RequiredSecureCodeLicenses.</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<DialogTurnResult> VerificationSecureCodeStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            LicenseFields = (LicenseFields)stepContext.Values["LicenseFields"];
            LicenseFields.codSegurancaIn = stepContext.Result.ToString();

            // Se o código digitado pelo usuário for igual ao presente no WebService.
            if (LicenseFields.codSegurancaIn == LicenseFields.codSegurancaOut)
            {
                LicenseFields.Count = 0;
                return await stepContext.BeginDialogAsync(nameof(SpecificationsDialog), LicenseFields, cancellationToken);
            }
            else
            {
                await stepContext.Context.SendActivityAsync("Este CÓDIGO DE SEGURANÇA é inválido!");

                // Contador que garante 3 tentativas
                Counter cont = new Counter();
                return await cont.ThreeTimes(LicenseFields, stepContext, nameof(RequiredSecureCodeLicenseDialog), cancellationToken, "o Código de Segurança");
            }
        }
    }
}
