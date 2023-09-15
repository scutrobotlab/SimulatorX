// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Runtime.Mody;
using Doozy.Runtime.Signals;

namespace Doozy.Runtime.UIManager.Listeners.Internal
{
    public abstract class BaseListener : BaseStreamListener
    {
        public ModyEvent Callback;

        protected BaseListener() =>
            Callback = new ModyEvent("Callback").SetEnabled(true);

        protected override void ProcessSignal(Signal signal) =>
            Callback?.Execute(signal);
    }
}
