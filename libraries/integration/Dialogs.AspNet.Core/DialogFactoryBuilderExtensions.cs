using System;
using Microsoft.Bot.Builder.Dialogs;

namespace Microsoft.Bot.Builder.Integration.AspNet.Core
{
    public static class DialogFactoryBuilderExtensions
    {
        public static DialogFactoryBuilder AddRootDialog(this DialogFactoryBuilder builder, Type dialogType, params object[] additionalDialogArguments) =>
            builder.AddDialog(Dialog.RootDialogId, dialogType, additionalDialogArguments);

        public static DialogFactoryBuilder AddRootDialog(this DialogFactoryBuilder builder, Type dialogType) =>
            builder.AddDialog(Dialog.RootDialogId, dialogType, additionalDialogArguments: null);

        public static DialogFactoryBuilder AddRootDialog<TDialog>(this DialogFactoryBuilder builder)
            where TDialog : Dialog =>
            builder.AddDialog(Dialog.RootDialogId, typeof(TDialog), additionalDialogArguments: null);

        public static DialogFactoryBuilder AddRootDialog<TDialog>(this DialogFactoryBuilder builder, params object[] additionalDialogArguments)
            where TDialog : Dialog =>
            builder.AddDialog(Dialog.RootDialogId, typeof(TDialog), additionalDialogArguments);

        public static DialogFactoryBuilder AddRootDialog(this DialogFactoryBuilder builder, Func<IServiceProvider, Dialog> dialogFactory) =>
            builder.AddDialog(Dialog.RootDialogId, dialogFactory);

        public static DialogFactoryBuilder AddDialog<TDialog>(this DialogFactoryBuilder builder, string dialogId, params object[] additionalDialogArguments)
            where TDialog : Dialog =>
            builder.AddDialog(dialogId, typeof(TDialog), additionalDialogArguments);

        public static DialogFactoryBuilder AddDialog<TDialog>(this DialogFactoryBuilder builder, string dialogId)
            where TDialog : Dialog =>
            builder.AddDialog(dialogId, typeof(TDialog), additionalDialogArguments: null);

        public static DialogFactoryBuilder AddPrompt<TPrompt, TPromptValue>(this DialogFactoryBuilder builder, string promptId)
            where TPrompt : Prompt<TPromptValue> =>
            builder.AddDialog<TPrompt>(promptId, additionalDialogArguments: null);

        public static DialogFactoryBuilder AddPrompt<TPrompt, TPromptValue>(this DialogFactoryBuilder builder, string promptId, PromptValidator<TPromptValue> promptValidator)
            where TPrompt : Prompt<TPromptValue> =>
            builder.AddDialog<TPrompt>(promptId, additionalDialogArguments: promptValidator);

        public static DialogFactoryBuilder AddPrompt<TPromptValue>(this DialogFactoryBuilder builder, Prompt<TPromptValue> prompt) =>
            builder.AddDialog(prompt);
    }
}
