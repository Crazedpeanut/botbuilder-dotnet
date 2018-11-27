using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;

namespace Microsoft.Bot.Builder.Integration.AspNet.Core
{
    public class DialogsBot : IBot
    {
        private readonly IDialogFactory _dialogFactory;
        private readonly IStatePropertyAccessor<DialogState> _dialogStateAccessor;

        public DialogsBot(IDialogFactory dialogFactory, IStatePropertyAccessor<DialogState> dialogStateAccessor)
        {
            _dialogFactory = dialogFactory ?? throw new ArgumentNullException(nameof(dialogFactory));
            _dialogStateAccessor = dialogStateAccessor ?? throw new ArgumentNullException(nameof(dialogStateAccessor));
        }

        public async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            var dialogState = await _dialogStateAccessor.GetAsync(
                turnContext,
                () => new DialogState(),
                cancellationToken).ConfigureAwait(false);

            var dialogContext = new DialogContext(_dialogFactory, turnContext, dialogState);

            if (await OnBeforeExecuteNextDialogTurnAsync(dialogContext, cancellationToken).ConfigureAwait(false))
            {

                var dialogTurnResult = default(DialogTurnResult);

                if (dialogContext.ActiveDialog != null)
                {
                    dialogTurnResult = await dialogContext.ContinueDialogAsync(cancellationToken).ConfigureAwait(false);
                }
                else
                {
                    dialogTurnResult = await dialogContext.BeginDialogAsync(Dialog.RootDialogId, cancellationToken: cancellationToken).ConfigureAwait(false);
                }
            }

            await OnAfterExecuteDialogTurnAsync(dialogContext, cancellationToken).ConfigureAwait(false);
        }

        protected virtual Task<bool> OnBeforeExecuteNextDialogTurnAsync(DialogContext dialogContext, CancellationToken cancellationToken) => Task.FromResult(true);

        protected virtual Task OnAfterExecuteDialogTurnAsync(DialogContext dialogContext, CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
