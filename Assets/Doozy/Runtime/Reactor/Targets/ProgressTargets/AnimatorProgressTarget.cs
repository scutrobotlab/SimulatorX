// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using UnityEngine;
namespace Doozy.Runtime.Reactor.Targets.ProgressTargets
{
    [AddComponentMenu("Doozy/Reactor/Targets/Animator Progress Target")]
    public class AnimatorProgressTarget : MetaProgressTarget<Animator>
    {
        [SerializeField] private string ParameterName = "Progress";
        /// <summary>
        /// Target parameter name (defined in the Animator as a float parameter)
        /// This parameter needs to be selected as the 'Normalized Time' parameter in the target animation.
        /// </summary>
        public string parameterName
        {
            get => ParameterName;
            set => ParameterName = value;
        }

        #if UNITY_EDITOR
        private void Reset()
        {
            Target = Target ? Target : GetComponent<Animator>();
            if (!Enum.IsDefined(typeof(Mode), targetMode))
                targetMode = Mode.Progress;
            targetMode = Mode.Progress;
        }

        private void OnValidate()
        {
            if (!Enum.IsDefined(typeof(Mode), targetMode))
                targetMode = Mode.Progress;
        }
        #endif

        public override void UpdateTarget(Progressor progressor)
        {
            if (target == null) return;
            if (!target.gameObject.activeSelf) return;
            if (!target.isActiveAndEnabled) return;
            if (!Enum.IsDefined(typeof(Mode), targetMode))
                targetMode = Mode.Progress;
            switch (targetMode)
            {
                case Mode.Progress:
                    target.SetFloat(ParameterName, progressor.progress);
                    break;
                case Mode.Value:
                    target.SetFloat(ParameterName, progressor.currentValue);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
