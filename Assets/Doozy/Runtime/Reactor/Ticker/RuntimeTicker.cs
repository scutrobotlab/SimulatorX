// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Runtime.Common;
using Doozy.Runtime.Common.Attributes;
using Doozy.Runtime.Reactor.ScriptableObjects;
using UnityEngine;
// ReSharper disable UnusedMember.Local

namespace Doozy.Runtime.Reactor.Ticker
{
    /// <summary> Runtime Tick Service - ticks all registered targets at runtime </summary>
    public class RuntimeTicker : SingletonBehaviour<RuntimeTicker>
    {
        [ClearOnReload]
        private static TickService s_service;
        public static TickService service
        {
            get
            {
                if (applicationIsQuitting) return null;
                if (instance == null) return null;
                if (!instance.initialized) instance.Initialize();
                return s_service ??= new TickService(ReactorSettings.runtimeFPS);
            }
        }

        [ExecuteOnReload]
        private static void OnReload()
        {
            ResetTime();
        }
        
        /// <summary> Time.realtimeSinceStartup </summary>
        public static float timeSinceStartup => Time.realtimeSinceStartup;
        private static float tickInterval => service.tickInterval;
        private static double s_elapsedTime, s_lastTickTime;
        
        private bool initialized { get; set; }

        private void Initialize()
        {
            if (initialized) return;
            initialized = true;
            ResetTime();
        }

        private static void ResetTime()
        {
            s_elapsedTime = 0;
            s_lastTickTime = timeSinceStartup;
        }

        private void Update()
        {
            if (!service.hasRegisteredTargets)
            {
                // Debug.Log($"{nameof(RuntimeTicker)}.{nameof(Update)}()");
                ResetTime();
                return;
            }

            s_elapsedTime += timeSinceStartup - s_lastTickTime;
            s_lastTickTime = timeSinceStartup;

            if (tickInterval < TickService.maxFPS && s_elapsedTime < tickInterval)
                return;
            s_elapsedTime = 0;
            service.Tick();

            // Debug.Log($"{nameof(RuntimeTicker)}.{nameof(Update)}() - Registered Targets: {service.registeredTargetsCount}");
        }
    }
}
