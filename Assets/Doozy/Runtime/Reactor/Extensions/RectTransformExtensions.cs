// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Runtime.Reactor.Reactions;
using UnityEngine;
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global

namespace Doozy.Runtime.Reactor.Extensions
{
    public static class RectTransformExtensions
    {
        public static Vector3Reaction AnchorPosition3DReaction(this RectTransform target, Vector3 targetValue, float duration, bool relative) =>
            Reactor.To
            (
                () => target.anchoredPosition3D,
                value => target.anchoredPosition3D = value,
                targetValue,
                duration,
                relative
            );

        public static Vector3Reaction RotationReaction(this RectTransform target, Vector3 targetValue, float duration, bool relative) =>
            Reactor.To
            (
                () => target.localEulerAngles,
                value => target.localRotation = Quaternion.Euler(value),
                targetValue,
                duration,
                relative
            );
    }
}
