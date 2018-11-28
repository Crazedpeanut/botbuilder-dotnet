// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Diagnostics;

namespace Microsoft.Bot.Builder.Diagnostics
{
    internal static class BotAdapterDiagnosticSourceExtensions
    {
        private const string RunPipelineAsyncEventName = "Microsoft.BotBuilder.Core.BotAdapter.RunPipelineAsync";
        private const string RunPipelineAsyncTurnStateKey = "BotBuilder.Activities.BotAdapter::RunPipelineAsync";

        public static void StartRunPipelineAsyncActivity(this DiagnosticSource diagnosticSource, ITurnContext turnContext)
        {
            if (diagnosticSource.IsEnabled(RunPipelineAsyncEventName, turnContext))
            {
                StartActivity();
            }

            void StartActivity()
            {
                var activity = turnContext.Activity;
                
                var runPipelineAsyncDiagnosticActivity = new Activity("BotAdapter::RunPipelineAsync")
                    .AddBaggage("BotBuilder.Activity.Id", activity.Id)
                    .AddBaggage("BotBuilder.Activity.Conversation.Id", activity.Conversation.Id)
                    .AddTag("BotBuilder.Activity.Type", activity.Type)
                    .AddTag("BotBuilder.Activity.ChannelId", activity.ChannelId);

                diagnosticSource.StartActivity(runPipelineAsyncDiagnosticActivity, new { TurnContext = turnContext });

                turnContext.TurnState.Add(RunPipelineAsyncTurnStateKey, runPipelineAsyncDiagnosticActivity);
            }
        }

        public static void StopBotAdapterRunPipelineAsyncActivity(this DiagnosticSource diagnosticSource, ITurnContext turnContext, Exception exception = null)
        {
            if (diagnosticSource.IsEnabled(RunPipelineAsyncEventName, turnContext))
            {
                StopActivity();
            }

            void StopActivity()
            {
                var turnState = turnContext.TurnState;
                var runPipelineAsyncDiagnosticActivity = turnState[RunPipelineAsyncTurnStateKey] as Activity;
                turnState.Remove(RunPipelineAsyncTurnStateKey);

                runPipelineAsyncDiagnosticActivity.AddTag("BotBuilder.TurnContext.Responded", turnContext.Responded.ToString());

                diagnosticSource.StopActivity(runPipelineAsyncDiagnosticActivity, new { TurnContext = turnContext });
            }
        }
    }
}
