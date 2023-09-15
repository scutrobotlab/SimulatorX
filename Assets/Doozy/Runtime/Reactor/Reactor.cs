// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Runtime.Reactor.Internal;
using Doozy.Runtime.Reactor.Reactions;
using UnityEngine;

namespace Doozy.Runtime.Reactor
{
    public static class Reactor
    {
        public static ColorReaction To
        (
            PropertyGetter<Color> getter, PropertySetter<Color> setter,
            Color targetValue, float duration, bool relative = false, bool startReaction = false
        )
        {
            ColorReaction reaction = 
                Reaction.Get<ColorReaction>()
                    .SetRuntimeHeartbeat()
                    .SetDuration(duration)
                    .SetGetter(getter)
                    .SetSetter(setter);
           
            reaction.SetValue(getter.Invoke());
            if (startReaction) reaction.PlayToValue(targetValue, relative);
            else reaction.SetTo(targetValue, relative);
            return reaction;
        }

        public static FloatReaction To
        (
            PropertyGetter<float> getter, PropertySetter<float> setter,
            float targetValue, float duration, bool relative = false, bool startReaction = false
        )
        {
            FloatReaction reaction = Reaction.Get<FloatReaction>().SetRuntimeHeartbeat().SetDuration(duration);
            reaction.getter = getter;
            reaction.setter = setter;
            reaction.SetValue(getter.Invoke());
            if (startReaction) reaction.PlayToValue(targetValue, relative);
            else reaction.SetTo(targetValue, relative);
            return reaction;
        }

        public static IntReaction To
        (
            PropertyGetter<int> getter, PropertySetter<int> setter,
            int targetValue, float duration, bool relative = false, bool startReaction = false
        )
        {
            IntReaction reaction = Reaction.Get<IntReaction>().SetRuntimeHeartbeat().SetDuration(duration);
            reaction.getter = getter;
            reaction.setter = setter;
            reaction.SetValue(getter.Invoke());
            if (startReaction) reaction.PlayToValue(targetValue, relative);
            else reaction.SetTo(targetValue, relative);
            return reaction;
        }

        public static Vector2Reaction To
        (
            PropertyGetter<Vector2> getter, PropertySetter<Vector2> setter,
            Vector2 targetValue, float duration, bool relative = false, bool startReaction = false
        )
        {
            Vector2Reaction reaction = Reaction.Get<Vector2Reaction>().SetRuntimeHeartbeat().SetDuration(duration);
            reaction.getter = getter;
            reaction.setter = setter;
            reaction.SetValue(getter.Invoke());
            if (startReaction) reaction.PlayToValue(targetValue, relative);
            else reaction.SetTo(targetValue, relative);
            return reaction;
        }

        public static Vector3Reaction To
        (
            PropertyGetter<Vector3> getter, PropertySetter<Vector3> setter,
            Vector3 targetValue, float duration, bool relative = false, bool startReaction = false
        )
        {
            Vector3Reaction reaction = Reaction.Get<Vector3Reaction>().SetRuntimeHeartbeat().SetDuration(duration);
            reaction.getter = getter;
            reaction.setter = setter;
            reaction.SetValue(getter.Invoke());
            if (startReaction) reaction.PlayToValue(targetValue, relative);
            else reaction.SetTo(targetValue, relative);
            return reaction;
        }
    }
}
