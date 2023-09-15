// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Runtime.Common.Events;
using UnityEngine;
namespace Doozy.Runtime.Reactor.Targets.ProgressTargets
{
    [AddComponentMenu("Doozy/Reactor/Targets/UnityEvent Progress Target")]
    public class UnityEventProgressTarget : MetaProgressTarget<FloatEvent>
    {
        [SerializeField] private bool WholeNumbers = true;
        /// <summary> Should the target variable value get rounded to the nearest integer? </summary>
        public bool wholeNumbers
        {
            get => WholeNumbers;
            set => WholeNumbers = value;
        }
        
        [SerializeField] private bool UseMultiplier = true;
        /// <summary> Should the target variable value have a multiplier applied? </summary>
        public bool useMultiplier
        {
            get => UseMultiplier;
            set => UseMultiplier = value;
        }

        [SerializeField] private float Multiplier = 100;
        /// <summary>
        /// If UseMultiplier is TRUE, the target variable value will be multiplied by this value.
        /// Useful if you want to convert the Progress from 0.5 to 50% for example. 
        /// </summary>
        public float multiplier
        {
            get => Multiplier;
            set => Multiplier = value;
        }
        
        /// <summary> Internal variable used to get the updated target value </summary>
        private float m_TargetValue;
        
        #if UNITY_EDITOR
        private void OnValidate()
        {
            if (!Enum.IsDefined(typeof(Mode), targetMode))
                targetMode = Mode.Value;
        }
        #endif

        
        public override void UpdateTarget(Progressor progressor)
        {
            if (target == null) return;
            if (!Enum.IsDefined(typeof(Mode), targetMode))
                targetMode = Mode.Value;
            m_TargetValue = 0;
            switch (targetMode)
            {
                case Mode.Progress:
                    m_TargetValue = progressor.progress;
                    break;
                case Mode.Value:
                    m_TargetValue = progressor.currentValue;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            if (UseMultiplier) m_TargetValue *= Multiplier;
            if (WholeNumbers) m_TargetValue = Mathf.Round(m_TargetValue);

            target.Invoke(m_TargetValue);
        }
    }
}
