// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Microsoft.Bot.Builder.Dialogs.Diagnostics
{
    public static class WaterfallDialogDiagnosticSourceExtensions
    {
        private const string OnStepAsyncEventName = "Microsoft.BotBuilder.Dialogs.WaterfallDialog.OnStepAsync";

        public static Activity StartOnStepAsyncActivity(this DiagnosticSource diagnosticSource, WaterfallStepContext stepContext, string stepName)
        {
            if (!diagnosticSource.IsEnabled(OnStepAsyncEventName, stepContext))
            {
                return null;
            }

            var onStepAsyncDiagnosticActivity = new Activity(OnStepAsyncEventName)
                    .AddTag("BotBuilder.Dialogs.WaterfallDialog.Step.Index", stepContext.Index.ToString())
                    .AddTag("BotBuilder.Dialogs.WaterfallDialog.Step.Name", stepName);

            return diagnosticSource.StartActivity(onStepAsyncDiagnosticActivity, new { StepContext = stepContext });
        }

        public static void StopOnStepAsyncActivity(this DiagnosticSource diagnosticSource, Activity onStepAsyncDiagnosticActivity, WaterfallStepContext stepContext, DialogTurnResult result)
        {
            if (onStepAsyncDiagnosticActivity != null)
            {
                onStepAsyncDiagnosticActivity.AddTag("BotBuilder.Dialogs.WaterfallDialog.Step.Result.Status", result.Status.ToString());

                diagnosticSource.StopActivity(onStepAsyncDiagnosticActivity, new { StepContext = stepContext });
            }
        }
    }
}
