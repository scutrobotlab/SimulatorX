// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Runtime.Reactor.Internal;
using Doozy.Runtime.Reactor.Ticker;

namespace Doozy.Editor.Reactor.Ticker
{
    /// <summary> Registers to the Editor Ticker Service and ticks a target callback (when registered) </summary>
    public class EditorHeartbeat : Heartbeat
    {
        /// <summary> Construct an editor Heartbeat with no target callback </summary>
        public EditorHeartbeat() : base(null) {}
        
        /// <summary> Construct an editor Heartbeat with a target callback </summary>
        /// <param name="onTickCallback"> Target callback </param>
        public EditorHeartbeat(ReactionCallback onTickCallback) : base(onTickCallback) {}
        
        /// <summary> EditorApplication.timeSinceStartup </summary>
        public override double timeSinceStartup => EditorTicker.timeSinceStartup;
       
        /// <summary> Register to the Editor Ticker Service <para/> Start ticking the callback </summary>
        public override void RegisterToTickService()
        {
            base.RegisterToTickService();
            EditorTicker.service.Register(this);
        }

        /// <summary> Unregister from the Runtime Ticker Service <para/> Stop ticking the callback </summary>
        public override void UnregisterFromTickService()
        {
            base.UnregisterFromTickService();
            EditorTicker.service.Unregister(this);
        }
    }
}