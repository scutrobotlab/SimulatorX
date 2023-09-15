// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using Doozy.Runtime.Reactor.Internal;
using Doozy.Runtime.Reactor.Reactions;
using Doozy.Runtime.UIElements.Extensions;
using UnityEngine;
using UnityEngine.UIElements;

namespace Doozy.Runtime.Reactor.Extensions
{
    public static class VisualElementExtensions
    {
        public static ColorReaction GetColorReaction(this VisualElement target, PropertySetter<Color> setter) =>
            Reaction.Get<ColorReaction>().SetTargetObject(target).SetSetter(setter);

        public static FloatReaction GetFloatReaction(this VisualElement target, PropertySetter<float> setter) =>
            Reaction.Get<FloatReaction>().SetTargetObject(target).SetSetter(setter);

        public static Texture2DReaction GetTexture2DReaction(this VisualElement target, IEnumerable<Texture2D> textures = null)
        {
            Texture2DReaction reaction = Reaction.Get<Texture2DReaction>().SetTargetObject(target).SetSetter(value => target.SetStyleBackgroundImage(value));
            if (textures != null) reaction.SetTextures(textures);
            return reaction;
        }

        public static IntReaction GetIntReaction(this VisualElement target, PropertySetter<int> setter) =>
            Reaction.Get<IntReaction>().SetTargetObject(target).SetSetter(setter);

        public static Vector2Reaction GetVector2Reaction(this VisualElement target, PropertySetter<Vector2> setter) =>
            Reaction.Get<Vector2Reaction>().SetTargetObject(target).SetSetter(setter);

        public static Vector3Reaction GetVector3Reaction(this VisualElement target, PropertySetter<Vector3> setter) =>
            Reaction.Get<Vector3Reaction>().SetTargetObject(target).SetSetter(setter);
    }
}
