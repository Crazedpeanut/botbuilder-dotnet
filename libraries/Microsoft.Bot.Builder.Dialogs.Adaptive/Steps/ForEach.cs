﻿// Licensed under the MIT License.
// Copyright (c) Microsoft Corporation. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs.Debugging;
using Microsoft.Bot.Builder.Expressions;
using Microsoft.Bot.Builder.Expressions.Parser;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Microsoft.Bot.Builder.Dialogs.Adaptive.Steps
{
    /// <summary>
    /// Executes a set of steps once for each item in an in-memory list or collection.
    /// </summary>
    public class Foreach : DialogCommand, IDialogDependencies
    {
        // Expression used to compute the list that should be enumerated.
        [JsonProperty("ListProperty")]
        public string ListProperty { get; set; }

        // In-memory property that will contain the current items index. Defaults to `dialog.index`.
        [JsonProperty("IndexProperty")]
        public string IndexProperty { get; set; } = "dialog.index";

        // In-memory property that will contain the current items value. Defaults to `dialog.value`.
        [JsonProperty("ValueProperty")]
        public string ValueProperty { get; set; } = "dialog.value";

        // Steps to be run for each of items.
        [JsonProperty("Steps")]
        public List<IDialog> Steps { get; set; } = new List<IDialog>();

        [JsonConstructor]
        public Foreach([CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
            : base()
        {
            this.RegisterSourceLocation(sourceFilePath, sourceLineNumber);
        }

        protected override async Task<DialogTurnResult> OnRunCommandAsync(DialogContext dc, object options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (options is CancellationToken)
            {
                throw new ArgumentException($"{nameof(options)} cannot be a cancellation token");
            }

            // Ensure planning context
            if (dc is PlanningContext planning)
            {
                string listProperty = null;
                int offset = 0;
                if (options != null && options is ForeachOptions)
                {
                    var opt = options as ForeachOptions;
                    listProperty = opt.list;
                    offset = opt.offset;
                }

                if (listProperty == null)
                {
                    listProperty = await new TextTemplate(this.ListProperty).BindToData(dc.Context, dc.State).ConfigureAwait(false);
                }

                var itemList = dc.State.GetValue(listProperty, new JArray());
                var item = this.GetItem(itemList, offset);

                if (item != null)
                {
                    dc.State.SetValue(this.ValueProperty, item);
                    dc.State.SetValue(this.IndexProperty, offset);
                    var changes = new PlanChangeList()
                    {
                        ChangeType = PlanChangeTypes.DoSteps,
                        Steps = new List<PlanStepState>()
                    };
                    this.Steps.ForEach(step => changes.Steps.Add(new PlanStepState(step.Id)));

                    changes.Steps.Add(new PlanStepState()
                    {
                        DialogStack = new List<DialogInstance>(),
                        DialogId = this.Id,
                        Options = new ForeachOptions()
                        {
                            list = listProperty,
                            offset = offset + 1
                        }
                    });
                    planning.QueueChanges(changes);
                }

                return await planning.EndDialogAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
            }
            else
            {
                throw new Exception("`Foreach` should only be used in the context of an adaptive dialog.");
            }
        }

        private object GetItem(object list, int index)
        {
            JToken result = null;
            if (list != null && list.GetType() == typeof(JArray))
            {
                if (index < JArray.FromObject(list).Count)
                {
                    result = JArray.FromObject(list)[index];
                }
            }
            return result;
        }
        protected override string OnComputeId()
        {
            return $"{nameof(Foreach)}({this.ListProperty})";
        }

        public override List<IDialog> ListDependencies()
        {
            return this.Steps;
        }

        public class ForeachOptions
        {
            public string list { get; set; }
            public int offset { get; set; }
        }
    }
}
