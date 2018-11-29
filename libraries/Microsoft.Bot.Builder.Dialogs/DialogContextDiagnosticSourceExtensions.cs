// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Diagnostics;

namespace Microsoft.Bot.Builder.Dialogs.Diagnostics
{
    public static class DialogContextDiagnosticSourceExtensions
    {
        private const string BaseDialogContextEventName = "Microsoft.BotBuilder.Dialogs.DialogContext";
        private const string BeginDialogAsyncDiagnosticEventName = BaseDialogContextEventName + ".BeginDialogAsync";

        private const string BeginningDialogDiagnosticEventName = BaseDialogContextEventName + ".BeginningDialog";
        private const string ResumingDialogDiagnosticEventName = BaseDialogContextEventName + ".ResumingDialog";
        private const string ContinuingDialogDiagnosticEventName = BaseDialogContextEventName + ".ContinuingDialog";
        private const string CancelingDialogDiagnosticEventName = BaseDialogContextEventName + ".CancelingDialog";
        private const string EndingDialogDiagnosticEventName = BaseDialogContextEventName + ".EndingDialog";
        private const string ReplacingDialogDiagnosticsEventName = BaseDialogContextEventName + ".ReplacingDialog";
        private const string RepromptingDialogDiagnosticsEventName = BaseDialogContextEventName + ".RepromptingDialog";

        public static Activity StartBeginDialogAsyncActivity(this DiagnosticSource diagnosticSource, DialogContext dialogContext, string dialogId)
        {
            if (!diagnosticSource.IsEnabled(BeginDialogAsyncDiagnosticEventName, dialogContext))
            {
                return null;
            }

            var beginDialogAsyncDiagnosticActivity = new Activity(BeginDialogAsyncDiagnosticEventName);

            return diagnosticSource.StartActivity(beginDialogAsyncDiagnosticActivity, new { DialogContext = dialogContext, DialogId = dialogId });
        }

        public static void StopBeginDialogAsyncActivity(this DiagnosticSource diagnosticSource, Activity beginDialogAsyncDiagnosticActivity, DialogContext dialogContext, DialogTurnResult result)
        {
            if (beginDialogAsyncDiagnosticActivity != null)
            {
                beginDialogAsyncDiagnosticActivity.AddTag("BotBuilder.Dialogs.Result.Status", result.Status.ToString());

                diagnosticSource.StopActivity(beginDialogAsyncDiagnosticActivity, new { DialogContext = dialogContext });
            }
        }

        public static void WriteBeginningDialog(this DiagnosticSource diagnosticSource, DialogContext dialogContext, DialogInstance dialogInstance) =>
            WriteDialogLifecycleEvent(BeginningDialogDiagnosticEventName, diagnosticSource, dialogContext, dialogInstance);

        public static void WriteContinuingDialog(this DiagnosticSource diagnosticSource, DialogContext dialogContext, DialogInstance dialogInstance) =>
            WriteDialogLifecycleEvent(ContinuingDialogDiagnosticEventName, diagnosticSource, dialogContext, dialogInstance);

        public static void WriteResumingDialog(this DiagnosticSource diagnosticSource, DialogContext dialogContext, DialogInstance dialogInstance) =>
            WriteDialogLifecycleEvent(ResumingDialogDiagnosticEventName, diagnosticSource, dialogContext, dialogInstance);

        public static void WriteCancelingDialog(this DiagnosticSource diagnosticSource, DialogContext dialogContext, DialogInstance dialogInstance) =>
            WriteDialogLifecycleEvent(CancelingDialogDiagnosticEventName, diagnosticSource, dialogContext, dialogInstance);

        public static void WriteEndingDialog(this DiagnosticSource diagnosticSource, DialogContext dialogContext, DialogInstance dialogInstance) =>
            WriteDialogLifecycleEvent(EndingDialogDiagnosticEventName, diagnosticSource, dialogContext, dialogInstance);

        public static void WriteReplacingDialog(this DiagnosticSource diagnosticSource, DialogContext dialogContext, DialogInstance dialogInstance) =>
            WriteDialogLifecycleEvent(ReplacingDialogDiagnosticsEventName, diagnosticSource, dialogContext, dialogInstance);

        public static void WriteRepromptingDialog(this DiagnosticSource diagnosticSource, DialogContext dialogContext, DialogInstance dialogInstance) =>
            WriteDialogLifecycleEvent(RepromptingDialogDiagnosticsEventName, diagnosticSource, dialogContext, dialogInstance);

        private static void WriteDialogLifecycleEvent(string eventName, DiagnosticSource diagnosticSource, DialogContext dialogContext, DialogInstance dialogInstance)
        {
            if (!diagnosticSource.IsEnabled(eventName, dialogContext))
            {
                return;
            }

            diagnosticSource.Write(eventName, new { DialogContext = dialogContext, DialogInstance = dialogInstance });
        }
    }
}
