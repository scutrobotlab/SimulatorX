// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
namespace Doozy.Runtime.Reactor.Targets.ProgressTargets
{
    [AddComponentMenu("Doozy/Reactor/Targets/Text Progress Target")]
    public class TextProgressTarget : MetaProgressTarget<Text>
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

        [SerializeField] private string Prefix;
        /// <summary> Text added before the converted string value </summary>
        public string prefix
        {
            get => Prefix;
            set => Prefix = value;
        }

        [SerializeField] private string Suffix = "%";
        /// <summary> Text added after the converted string value </summary>
        public string suffix
        {
            get => Suffix;
            set => Suffix = value;
        }

        /// <summary> Internal variable used to keep track if this progress target's variables have been initialized </summary>
        private bool m_Initialized;

        /// <summary> Internal variable used to get the updated target value </summary>
        private float m_TargetValue;

        /// <summary> Internal variable used to lower the string operations memory allocations </summary>
        private StringBuilder m_StringBuilder = new StringBuilder();

        #if UNITY_EDITOR
        private void Reset()
        {
            Target = Target ? Target : GetComponent<Text>();
            if (!Enum.IsDefined(typeof(Mode), targetMode))
                targetMode = Mode.Value;
        }

        private void OnValidate()
        {
            if (!Enum.IsDefined(typeof(Mode), targetMode))
                targetMode = Mode.Value;
        }
        #endif

        public override void UpdateTarget(Progressor progressor)
        {
            if (!m_Initialized) Init();
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

            target.text =
                m_StringBuilder
                    .Remove(0, m_StringBuilder.Length)
                    .Append(Prefix)
                    .Append(m_TargetValue)
                    .Append(Suffix)
                    .ToString();
        }
        
        private void Init()
        {
            m_StringBuilder ??= new StringBuilder();
            m_Initialized = true;
        }
    }
}
