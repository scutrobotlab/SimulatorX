// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Runtime.Common.Attributes;
using Doozy.Runtime.Reactor.ScriptableObjects;
using Doozy.Runtime.Reactor.Ticker;
using UnityEditor;
// ReSharper disable UnusedMember.Local

namespace Doozy.Editor.Reactor.Ticker
{
    /// <summary> Editor Tick Service - ticks all registered targets in the editor </summary>
    [InitializeOnLoad]
    public static class EditorTicker
    {
        static EditorTicker() => Initialize();

        [ClearOnReload]
        private static TickService s_service;
        public static TickService service
        {
            get
            {
                if (!initialized) Initialize();
                return s_service ?? (s_service = new TickService(ReactorSettings.editorFPS));
            }
        }
        
        [ExecuteOnReload]
        private static void OnReload()
        {
            ResetTime();
        }

        /// <summary> EditorApplication.timeSinceStartup </summary>
        public static float timeSinceStartup => (float)EditorApplication.timeSinceStartup;
        private static float tickInterval => service.tickInterval;
        private static double s_elapsedTime, s_lastTickTime;
        private static bool initialized { get; set; }

        private static void Initialize()
        {
            if (initialized) return;
            initialized = true;
            ResetTime();
            EditorApplication.update -= Update;
            EditorApplication.update += Update;
        }

        private static void ResetTime()
        {
            s_elapsedTime = 0;
            s_lastTickTime = timeSinceStartup;
        }

        private static void Update()
        {
            if (!service.hasRegisteredTargets)
            {
                // Debug.Log($"{nameof(EditorTicker)}.{nameof(Update)}()");
                ResetTime();
                return;
            }

            s_elapsedTime += timeSinceStartup - s_lastTickTime;
            s_lastTickTime = timeSinceStartup;
            if (tickInterval < TickService.maxFPS && s_elapsedTime < tickInterval)
                return;
            s_elapsedTime = 0;
            service.Tick();

            // Debug.Log($"{nameof(EditorTicker)}.{nameof(Update)}() - Registered Targets: {service.registeredTargetsCount}");
        }

        public static void UpdateTicker()
        {
            
        }
    }
}
