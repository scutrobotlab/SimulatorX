// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using UnityEngine;
using UnityEngine.UI;

namespace Doozy.Runtime.Reactor.Targets
{
    [Serializable]
    public abstract class ReactorSpriteTarget : MonoBehaviour
    {
        public abstract Type targetType { get; }
        
        public int currentFrame { get; set; }
        public Sprite current { get; set; }
        
        public Sprite sprite
        {
            get => GetSprite();
            set => SetSprite(value);
        }

        public abstract Sprite GetSprite();
        public abstract void SetSprite(Sprite value);
        
        //ToDo: [Maybe] create an automated system that writes code to generate sprite targets (a factory pattern)
        public static ReactorSpriteTarget FindTarget(GameObject gameObject)
        {
            ReactorSpriteTarget[] array = gameObject.GetComponents<ReactorSpriteTarget>();
            ReactorSpriteTarget target = array != null && array.Length > 0 ? array[0] : null;

            if (target != null)
                return target;

            Image image = gameObject.GetComponent<Image>(); //check for Image
            SpriteMask spriteMask = gameObject.GetComponent<SpriteMask>(); //check for SpriteMask
            SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>(); //check for SpriteRenderer

            if (image)
                return gameObject.AddComponent<ImageSpriteTarget>();

            if (spriteMask)
                return gameObject.AddComponent<SpriteMaskSpriteTarget>();
            
            if (spriteRenderer)
                return gameObject.AddComponent<SpriteRendererSpriteTarget>();

            return gameObject.GetComponent<ReactorSpriteTarget>();
        }
    }
}
