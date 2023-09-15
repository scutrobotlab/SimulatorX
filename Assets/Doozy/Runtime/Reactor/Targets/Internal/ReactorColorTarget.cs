// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Doozy.Runtime.Reactor.Targets
{
    /// <summary> Base class for all ColorTargets animated by a color reaction </summary>
    [Serializable]
    public abstract class ReactorColorTarget : MonoBehaviour
    {
        public abstract Type targetType { get; }
        
        /// <summary> ColorTarget's color </summary>
        public Color color
        {
            get => GetColor();
            set => SetColor(value);
        }

        /// <summary> Get ColorTarget's color </summary>
        public abstract Color GetColor();
        
        /// <summary> Set ColorTarget's color </summary>
        /// <param name="value"> New color </param>
        public abstract void SetColor(Color value);

        //ToDo: [Maybe] create an automated system that writes code to generate color targets (a factory pattern)
        
        /// <summary> Find a ColorTarget on the given target GameObject </summary>
        /// <param name="gameObject"> Target GameObject </param>
        public static ReactorColorTarget FindTarget(GameObject gameObject)
        {
            ReactorColorTarget[] array = gameObject.GetComponents<ReactorColorTarget>();
            ReactorColorTarget target = array != null && array.Length > 0 ? array[0] : null;

            if (target != null)
                return target;

            Image image = gameObject.GetComponent<Image>(); //check for Image
            TMP_Text tmp = gameObject.GetComponent<TMP_Text>(); //check for TextMeshPro
            Text text = gameObject.GetComponent<Text>(); //check for Text

            if (image)
                return gameObject.AddComponent<ImageColorTarget>();

            if (tmp)
                return gameObject.AddComponent<TextMeshProColorTarget>();
            
            if (text)
                return gameObject.AddComponent<TextColorTarget>();

            return gameObject.GetComponent<ReactorColorTarget>();
        }
    }
}
