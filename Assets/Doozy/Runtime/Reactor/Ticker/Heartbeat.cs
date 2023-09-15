// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Runtime.Reactor.Internal;

namespace Doozy.Runtime.Reactor.Ticker
{
    /// <summary> Base class that registers to a tick service and ticks a target callback (when registered) </summary>
    public abstract class Heartbeat : IUseTickService
    {
        public bool isActive { get; private set; }
        
        /// <summary> Time updater </summary>
        public virtual double timeSinceStartup => 0f;

        public double lastUpdateTime { get; set; }

        public double deltaTime
        {
            get
            {
                double delta = timeSinceStartup - lastUpdateTime;
                lastUpdateTime = timeSinceStartup;
                return delta;
            }
        }
        
        /// <summary>
        /// Callback invoked every time the tick service (this Heartbeat is registered to) ticks
        /// <para/> Register to a tick service to Start invoking on every tick
        /// <para/> Unregister from the tick service to Stop invoking on every tick
        /// </summary>
        public ReactionCallback onTickCallback { get; internal set; }

        /// <summary> Construct a Heartbeat with a target callback </summary>
        /// <param name="onTickCallback"> Target callback </param>
        protected Heartbeat(ReactionCallback onTickCallback) =>
            this.onTickCallback = onTickCallback;

        /// <summary> Tick the target callback </summary>
        public virtual void Tick() =>
            onTickCallback?.Invoke();

        /// <summary> Register to a Tick Service to Start ticking the callback </summary>
        public virtual void RegisterToTickService()
        {
            isActive = true;
            lastUpdateTime = timeSinceStartup;
        }

        /// <summary> Unregister from the Tick Service to Stop ticking the callback </summary>
        public virtual void UnregisterFromTickService()
        {
            isActive = false;
        }
    }

    public static class HeartbeatExtensions
    {
        /// <summary>
        /// Clear any callbacks from being called by onTickCallback
        /// <para/> This method sets onTickCallback to null
        /// </summary>
        /// <param name="target"> Target Heartbeat </param>
        public static T ClearOnTickCallback<T>(this T target) where T : Heartbeat
        {
            target.onTickCallback = null;
            return target;
        }
        
        /// <summary>
        /// Set a target callback that will be invoked every time the tick service (this Heartbeat is registered to) ticks
        /// <para/> This method clears any other callbacks for onTickCallback and sets a single one
        /// </summary>
        /// <param name="target"> Target Heartbeat </param>
        /// <param name="callback"> Target callback </param>
        public static T SetOnTickCallback<T>(this T target, ReactionCallback callback) where T : Heartbeat
        {
            target.onTickCallback = callback;
            return target;
        }

        /// <summary>
        /// Add a target callback that will be invoked every time the tick service (this Heartbeat is registered to) ticks
        /// <para/> This method allows adding multiple callbacks for onTickCallback 
        /// </summary>
        /// <param name="target"> Target Heartbeat </param>
        /// <param name="callback"> Target callback </param>
        public static T AddOnTickCallback<T>(this T target, ReactionCallback callback) where T : Heartbeat
        {
            target.onTickCallback += callback;
            return target;
        }
    }
}
