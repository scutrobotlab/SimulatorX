// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using UnityEngine;
using UnityEngine.Audio;
using static UnityEngine.Mathf;
namespace Doozy.Runtime.Reactor.Targets.ProgressTargets
{
    [AddComponentMenu("Doozy/Reactor/Targets/AudioMixer Progress Target")]
    public class AudioMixerProgressTarget : MetaProgressTarget<AudioMixer>
    {
        #region Constants

        private const float MIN_VALUE = 0.0001f;
        private const float MAX_VALUE = 1f;

        #endregion

        [SerializeField] private string ExposedParameterName;
        /// <summary> Name of exposed parameter in the target AudioMixer </summary>
        public string exposedParameterName
        {
            get => ExposedParameterName;
            set => ExposedParameterName = value;
        }
        
        [SerializeField] private bool UseLogarithmicConversion = true;
        /// <summary>
        /// Lower the sensitivity of the slider by using a logarithmic conversion.
        /// <para/> Should be TRUE if, for example, setting the volume level (the attenuation) for a AudioMixerGroup.
        /// <para/> If TRUE the progressor. Progress value will be used (converted to its logarithmic value), if FALSE progressor.Value value will be used (as is).
        /// </summary>
        public bool useLogarithmicConversion
        {
            get => UseLogarithmicConversion;
            set => UseLogarithmicConversion = value;
        }

        #if UNITY_EDITOR
        private void Reset()
        {
            if (!Enum.IsDefined(typeof(Mode), targetMode))
                targetMode = Mode.Value;
            targetMode = Mode.Value;
        }

        private void OnValidate()
        {
            if (!Enum.IsDefined(typeof(Mode), targetMode))
                targetMode = Mode.Value;
        }
        #endif
        
        private void Awake()
        {
            targetMode = Mode.Value;
        }

        public override void UpdateTarget(Progressor progressor)
        {
            if(target == null) return;
            if (!Enum.IsDefined(typeof(Mode), targetMode))
                targetMode = Mode.Value;
            if (UseLogarithmicConversion)
            {
                target.SetFloat(ExposedParameterName, GetLogarithmicValue(progressor.progress));
                return;
            }
            
            target.SetFloat(ExposedParameterName, progressor.currentValue);
        }

        private static float GetLogarithmicValue(float value) =>
            Log10(Clamp(value, MIN_VALUE, MAX_VALUE)) * 20;
    }
}
