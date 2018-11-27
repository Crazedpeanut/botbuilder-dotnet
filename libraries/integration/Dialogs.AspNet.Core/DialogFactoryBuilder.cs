using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Bot.Builder.Integration.AspNet.Core
{
    public sealed class DialogFactoryBuilder
    {
        private readonly Dictionary<string, DialogFactory> _dialogFactories;
        private readonly string _scopeId;

        public DialogFactoryBuilder()
            : this(string.Empty, new Dictionary<string, DialogFactory>())
        {
        }

        private DialogFactoryBuilder(string scopeId, Dictionary<string, DialogFactory> dialogFactories)
        {
            _scopeId = scopeId ?? throw new ArgumentNullException(nameof(scopeId));
            _dialogFactories = dialogFactories;
        }

        internal Dictionary<string, DialogFactory> DialogFactories => _dialogFactories;

        internal string ScopeId => _scopeId;

        public DialogFactoryBuilder AddDialog(Dialog dialog)
        {
            if (dialog == null)
            {
                throw new ArgumentNullException(nameof(dialog));
            }

            if (string.IsNullOrEmpty(dialog.Id))
            {
                throw new ArgumentException($"Singleton {nameof(Dialog)} instances must have their {nameof(Dialog.Id)} properties set to a non-null/empty value.");
            }

            return AddDialogFactory(
                dialog.Id,
                (sp, dialogId) => dialog);
        }

        public DialogFactoryBuilder AddDialogScope(string scopeId, Action<DialogFactoryBuilder> scopeBuilder)
        {
            if (string.IsNullOrWhiteSpace(scopeId))
            {
                throw new ArgumentException("Expected non-null/empty string.", nameof(scopeId));
            }

            if (scopeBuilder == null)
            {
                throw new ArgumentNullException(nameof(scopeBuilder));
            }

            scopeBuilder(new DialogFactoryBuilder(ScopeId + "/" + scopeId, _dialogFactories));

            return this.AddDialog<DialogScope>(scopeId);
        }

        public DialogFactoryBuilder AddDialog(string dialogId, Type dialogType, params object[] additionalDialogArguments)
        {
            if (string.IsNullOrEmpty(dialogId))
            {
                throw new ArgumentException("Expected non-null/empty value.", nameof(dialogId));
            }

            if (dialogType == null)
            {
                throw new ArgumentNullException(nameof(dialogType));
            }

            if (!typeof(Dialog).IsAssignableFrom(dialogType))
            {
                throw new ArgumentException($"Type must be a subtype of {nameof(Dialog)}.", nameof(dialogType));
            }

            var dialogFactory = default(DialogFactory);

            if (additionalDialogArguments != null && additionalDialogArguments.Length > 0)
            {
                var objectFactory = ActivatorUtilities.CreateFactory(dialogType, additionalDialogArguments?.Select(o => o.GetType()).ToArray() ?? Type.EmptyTypes);

                dialogFactory = (sp, did) =>
                {
                    var dialog = objectFactory(sp, additionalDialogArguments) as Dialog;

                    dialog.Id = did;

                    return dialog;
                };
            }
            else
            {
                dialogFactory = (sp, did) =>
                {
                    var dialog = ActivatorUtilities.GetServiceOrCreateInstance(sp, dialogType) as Dialog;

                    dialog.Id = did;

                    return dialog;
                };
            }

            return AddDialogFactory(dialogId, dialogFactory);
        }

        public DialogFactoryBuilder AddDialog(string dialogId, Func<IServiceProvider, Dialog> dialogFactory)
        {
            if (string.IsNullOrEmpty(dialogId))
            {
                throw new ArgumentException("Expected non-null/empty value.", nameof(dialogId));
            }

            if (dialogFactory == null)
            {
                throw new ArgumentNullException(nameof(dialogFactory));
            }

            return AddDialogFactory(dialogId, (sp, did) =>
            {
                var dialog = dialogFactory(sp);

                dialog.Id = did;

                return dialog;
            });
        }

        private DialogFactoryBuilder AddDialogFactory(string dialogId, DialogFactory dialogFactory)
        {
            _dialogFactories.Add(_scopeId + "/" + dialogId, dialogFactory);

            return this;
        }
    }
}
