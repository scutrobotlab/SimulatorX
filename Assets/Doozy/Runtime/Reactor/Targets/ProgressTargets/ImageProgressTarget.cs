// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Mathf;

namespace Doozy.Runtime.Reactor.Targets.ProgressTargets
{
    [AddComponentMenu("Doozy/Reactor/Targets/Image Progress Target")]
    public class ImageProgressTarget : MetaProgressTarget<Image>
    {
        #if UNITY_EDITOR
        private void Reset()
        {
            Target = Target ? Target : GetComponent<Image>();
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
            if(target == null) return;
            if (!Enum.IsDefined(typeof(Mode), targetMode))
                targetMode = Mode.Value;
            switch (targetMode)
            {
                case Mode.Progress:
                    target.fillAmount = Clamp01(progressor.progress);
                    break;
                case Mode.Value:
                    target.fillAmount = Clamp01(progressor.currentValue);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
