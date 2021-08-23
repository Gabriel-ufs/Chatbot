
using System.Threading;
using System.Threading.Tasks;
using CoreBot.Fields;
using Microsoft.Bot.Builder.Dialogs;


namespace Microsoft.BotBuilderSamples.Dialogs
{
    public class PortalDialog : CancelAndHelpDialog
    {
        RenovationFields RenovationFields;

        public PortalDialog()
        : base(nameof(PortalDialog))

        {

            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt), null, "pt-br"));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                ShowCourses,

            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> ShowCourses(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            WaterfallStepContext contextParent = (WaterfallStepContext)stepContext.Parent;
            RenovationFields = new RenovationFields();
            RenovationFields = (RenovationFields)contextParent.Values["RenovationFields"];
            stepContext.Values["RenovationFields"] = RenovationFields;

            await stepContext.Context.SendActivityAsync("Para sua atualização de dados você precisará acessar o link abaixo para o portal do Detran");
            await stepContext.Context.SendActivityAsync("No portal você poderá finalizar todo processo de renovação da habilitação");
                
            await stepContext.Context.SendActivityAsync("https://www.detran.se.gov.br/portal/?pg=cnh_renovacao");

            return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
        }


    }
}
            