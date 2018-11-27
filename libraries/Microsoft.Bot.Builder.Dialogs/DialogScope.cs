using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Bot.Builder.Dialogs
{
    public class DialogScope : Dialog
    {
        internal const string DialogScopeStateKey = "__MS_BOTFRAMEWORK_DIALOGSCOPE_STATE";

        public DialogScope()
        {
        }

        public override async Task<DialogTurnResult> BeginDialogAsync(DialogContext outerDialogContext, object options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (outerDialogContext == null)
            {
                throw new ArgumentNullException(nameof(outerDialogContext));
            }

            var scopeDialogState = new DialogState();

            outerDialogContext.ActiveDialog.State.Add(DialogScopeStateKey, scopeDialogState);

            var scopeDialogContext = new DialogContext(ScopeId + "/" + Id, outerDialogContext.DialogFactory, outerDialogContext.Context, scopeDialogState);
            var turnResult = await scopeDialogContext.BeginDialogAsync(Dialog.RootDialogId, options, cancellationToken).ConfigureAwait(false);

            if (turnResult.Status != DialogTurnStatus.Waiting)
            {
                return await outerDialogContext.EndDialogAsync(turnResult.Result, cancellationToken).ConfigureAwait(false);
            }

            return Dialog.EndOfTurn;
        }

        public override async Task<DialogTurnResult> ContinueDialogAsync(DialogContext outerDialogContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (outerDialogContext == null)
            {
                throw new ArgumentNullException(nameof(outerDialogContext));
            }

            var scopeDialogState = (DialogState)outerDialogContext.ActiveDialog.State[DialogScopeStateKey];
            var scopeDialogContext = new DialogContext(outerDialogContext.DialogFactory, outerDialogContext.Context, scopeDialogState);
            var turnResult = await scopeDialogContext.ContinueDialogAsync(cancellationToken).ConfigureAwait(false);

            if (turnResult.Status != DialogTurnStatus.Waiting)
            {
                return await outerDialogContext.EndDialogAsync(turnResult.Result, cancellationToken).ConfigureAwait(false);
            }

            return Dialog.EndOfTurn;
        }

        public override async Task RepromptDialogAsync(DialogContext outerDialogContext, DialogInstance instance, CancellationToken cancellationToken = default(CancellationToken))
        {
            var scopeDialogState = (DialogState)instance.State[DialogScopeStateKey];
            var scopeDialogContext = new DialogContext(outerDialogContext.DialogFactory, outerDialogContext.Context, scopeDialogState);

            await scopeDialogContext.RepromptDialogAsync(cancellationToken).ConfigureAwait(false);
        }

        public override async Task<DialogTurnResult> ResumeDialogAsync(DialogContext outerDialogContext, DialogReason reason, object result = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            await RepromptDialogAsync(outerDialogContext, outerDialogContext.ActiveDialog, cancellationToken).ConfigureAwait(false);

            return Dialog.EndOfTurn;
        }
    }
}
