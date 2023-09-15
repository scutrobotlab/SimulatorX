// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using TMPro;
using UnityEngine;

namespace Doozy.Runtime.Reactor.Targets
{
    /// <summary>
    /// Connects a TextMeshPro component with a Reactor animator.
    /// </summary>
    [Serializable]
    [AddComponentMenu("Doozy/Reactor/Targets/TextMeshPro Color Target")]
    public class TextMeshProColorTarget : ReactorMetaColorTarget<TMP_Text>
    {
        #if UNITY_EDITOR
        private void Reset()
        {
            Target = Target ? Target : GetComponent<TMP_Text>();
        }
        #endif
        
        public override Type targetType => typeof(TMP_Text);
        
        public override Color GetColor() =>
            Target == null ? Color.magenta : Target.color;
        
        public override void SetColor(Color value)
        {
            if(Target == null)
                return;

            Target.color = value;
        }
    }
}
