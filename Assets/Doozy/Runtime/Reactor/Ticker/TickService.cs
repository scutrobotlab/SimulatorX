// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using Doozy.Runtime.Reactor.ScriptableObjects;
using UnityEngine;
using UnityEngine.Events;
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Runtime.Reactor.Ticker
{
    /// <summary> Tick Service system that ticks every registered IUseTickService interfaces </summary>
    public class TickService
    {
        public static int minFPS => 1;
        public static int maxFPS => (int)FPS.MaxFPS;

        public float tickInterval { get; private set; }
        public int fps { get; private set; }

        private readonly List<IUseTickService> m_Targets = new List<IUseTickService>();
        private List<IUseTickService> safeTargets { get; }
        public UnityAction OnTick;
        
        public TickService(int fps)
        {
            SetFPS(fps);
            safeTargets = new List<IUseTickService>();
        }

        public void SetFPS(int value)
        {
            fps = Mathf.Max(minFPS, value);
            tickInterval = ReactorSettings.GetTickInterval(value);
        }

        public void SetFPS(FPS value)
        {
            SetFPS((int)value);
        }

        public int registeredTargetsCount =>
            m_Targets.Count;

        public bool hasRegisteredTargets =>
            registeredTargetsCount > 0;

        public void Register(IUseTickService target)
        {
            if (target == null) return;
            if (m_Targets.Contains(target)) return;
            RemoveNullTargets();
            m_Targets.Add(target);
        }

        public void Unregister(IUseTickService target)
        {
            if (target == null) return;
            if (!m_Targets.Contains(target)) return;
            RemoveNullTargets();
            m_Targets.Remove(target);
        }

        public void Tick()
        {
            RemoveNullTargets();
            safeTargets.Clear();
            safeTargets.AddRange(m_Targets);
            foreach (IUseTickService target in safeTargets) target.Tick();
            OnTick?.Invoke();
        }

        private void RemoveNullTargets()
        {
            for (int i = m_Targets.Count - 1; i >= 0; i--)
            {
                if (m_Targets[i] != null)
                    continue;
                m_Targets.RemoveAt(i);
            }
        }
    }
}
