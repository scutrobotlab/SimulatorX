// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using System.Linq;
using Doozy.Runtime.Signals;

namespace Doozy.Runtime.Mody
{
    [Serializable]
    public abstract class ModyEventBase
    {
        public const string k_DefaultEventName = "Unnamed";

        /// <summary> Enabled state for the event. If FALSE it will not Execute </summary>
        public bool Enabled;

        /// <summary> Name of the event </summary>
        public string EventName;

        /// <summary> List of action runners that trigger set actions on referenced modules </summary>
        public List<ModyActionRunner> Runners;

        /// <summary> Returns TRUE if the Runners count is greater than zero </summary>
        public bool hasRunners => Runners.Count > 0;

        protected ModyEventBase() : this(k_DefaultEventName) {}

        protected ModyEventBase(string eventName)
        {
            Enabled = false;
            EventName = eventName;
            Runners = new List<ModyActionRunner>();
        }

        /// <summary> Execute the event. Note that if the mody event is not enabled, this method does nothing </summary>
        public virtual void Execute(Signal signal = null)
        {
            if (!Enabled) return;
            foreach (ModyActionRunner runner in Runners)
                runner?.Execute();
        }

        public bool RunsAction(ModyModule module, string actionName) =>
            Runners.Where(runner => runner.Module == module).Any(runner => runner.ActionName.Equals(actionName));

        public bool RunsModule(ModyModule module) =>
            Runners.Any(runner => runner.Module == module);
    }

    public static class ModyEventBaseExtensions
    {
        public static T SetEnabled<T>(this T target, bool enabled) where T : ModyEventBase
        {
            target.Enabled = enabled;
            return target;
        }

        public static T SetEventName<T>(this T target, string eventName) where T : ModyEventBase
        {
            target.EventName = eventName;
            return target;
        }
    }
}
