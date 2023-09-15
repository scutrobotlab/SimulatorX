// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using Doozy.Editor.Common.Extensions;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.Components.Internal;
using Doozy.Editor.UIElements;
using Doozy.Runtime.Reactor;
using Doozy.Runtime.Reactor.Easings;
using Doozy.Runtime.UIElements.Extensions;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using PlayMode = Doozy.Runtime.Reactor.PlayMode;

namespace Doozy.Editor.Reactor.Drawers
{
    [CustomPropertyDrawer(typeof(ReactionSettings), true)]
    public class ReactionSettingsDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {}

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var target = property.GetTargetObjectOfProperty() as ReactionSettings;

            var drawer = new VisualElement();

            var startDelayField = newStartDelayField;
            var durationField = newDurationField;
            var loopsField = newLoopsField;
            var loopDelayField = newLoopDelayField;

            var playModeEnum = new EnumField().SetBindingPath("PlayMode").ResetLayout();
            var playModeField = FluidField.Get("Play Mode", playModeEnum).SetStyleFlexGrow(0).SetStyleMinWidth(160);

            var easeModeEnum = new EnumField().SetBindingPath("EaseMode").ResetLayout().SetTooltip("Animation ease mode can be either an Ease or an AnimationCurve");
            var easeModeField = FluidField.Get("Ease Mode", easeModeEnum).SetStyleFlexGrow(0).SetStyleMinWidth(128);

            var easeEnum = new EnumField().SetBindingPath("Ease").ResetLayout().SetTooltip("Ease for the animation");
            var easeField = FluidField.Get("Ease", easeEnum);

            var animationCurve = new CurveField().SetBindingPath("Curve").ResetLayout().SetTooltip("AnimationCurve for the animation");
            var animationCurveField = FluidField.Get("Animation Curve", animationCurve);

            var strength = new FloatField().SetBindingPath("Strength").ResetLayout().SetTooltip("Multiplier applied to the current value for spring and shake play modes");
            var strengthField = FluidField.Get("Strength", strength);

            var vibration = new IntegerField().SetBindingPath("Vibration").ResetLayout().SetTooltip("Minimum number of oscillations for spring and shake play modes.\nHigher value means more oscillations during the reaction's duration");
            var vibrationField = FluidField.Get("Vibration", vibration);

            var elasticity = new FloatField().SetBindingPath("Elasticity").ResetLayout().SetTooltip("Spring elasticity controls how much the current value goes back beyond the start value when contracting.\n0 - current value does not go beyond the start value\n1 - current value goes beyond the start value at maximum elastic force");
            var elasticityField = FluidField.Get("Elasticity", elasticity);

            var fadeOutShake = FluidToggleSwitch.Get(EditorSelectableColors.Reactor.Red).SetBindingPath("FadeOutShake").SetLabelText("Fade Out Shake").SetTooltip("Fade out the shake animation, by easing the last 20% of cycles (shakes) into a semi-smooth transition");

            {
                VisualElement row = new VisualElement().SetStyleFlexDirection(FlexDirection.Row);
                drawer.Add(row);
                row.Add(startDelayField);
                row.AddSpace(4, 0);
                row.Add(durationField);
                row.AddSpace(4, 0);
                row.Add(loopsField);
                row.AddSpace(4, 0);
                row.Add(loopDelayField);
            }
            drawer.AddSpace(0, 4);
            {
                VisualElement row = new VisualElement().SetStyleFlexDirection(FlexDirection.Row);
                drawer.Add(row);

                row.Add(playModeField);
                row.AddSpace(4, 0);
                row.Add(easeModeField);
                row.AddSpace(4, 0);
                row.Add(easeField);
                row.Add(animationCurveField);
            }

            VisualElement extraRow = new VisualElement().SetStyleFlexDirection(FlexDirection.Row).SetStyleMargins(0, 4, 0, 4);
            {
                extraRow.Add(strengthField);
                extraRow.AddSpace(4, 0);
                extraRow.Add(vibrationField);
                extraRow.AddSpace(4, 0);
                extraRow.Add(elasticityField);
                extraRow.AddSpace(4, 0);
                extraRow.Add(fadeOutShake);
            }
            drawer.Add(extraRow);

            #region PlayMode

            var invisibleLoopsField = new IntegerField { bindingPath = "Loops" }.SetStyleDisplay(DisplayStyle.None);
            invisibleLoopsField.RegisterValueChangedCallback(evt => UpdatePlayMode((PlayMode)playModeEnum.value));
            drawer.AddChild(invisibleLoopsField);

            var invisibleUseRandomLoopsField = new Toggle { bindingPath = "UseRandomLoops" }.SetStyleDisplay(DisplayStyle.None);
            invisibleUseRandomLoopsField.RegisterValueChangedCallback(evt => UpdatePlayMode((PlayMode)playModeEnum.value));
            drawer.AddChild(invisibleUseRandomLoopsField);

            playModeEnum.RegisterValueChangedCallback(evt =>
            {
                try
                {
                    UpdatePlayMode((PlayMode)evt.newValue);
                }
                catch
                {
                    // ignored
                }
            });
            playModeField.schedule.Execute(() => UpdatePlayMode((PlayMode)playModeEnum.value));

            void UpdatePlayMode(PlayMode playMode)
            {
                bool hasLoops = target != null && target.hasLoops;

                loopDelayField.SetEnabled(hasLoops);
                extraRow.SetStyleDisplay(playMode == PlayMode.Spring || playMode == PlayMode.Shake ? DisplayStyle.Flex : DisplayStyle.None);
                elasticityField.SetStyleDisplay(playMode == PlayMode.Spring ? DisplayStyle.Flex : DisplayStyle.None);
                fadeOutShake.SetStyleDisplay(playMode == PlayMode.Shake ? DisplayStyle.Flex : DisplayStyle.None);

                List<Texture2D> animatedIconTextures;
                switch (playMode)
                {
                    case PlayMode.Normal:
                        animatedIconTextures = hasLoops ? EditorMicroAnimations.EditorUI.Icons.Loop : EditorMicroAnimations.EditorUI.Icons.OneShot;
                        playModeEnum.SetTooltip("Normal - current value goes from start value and target value");
                        break;
                    case PlayMode.PingPong:
                        animatedIconTextures = hasLoops ? EditorMicroAnimations.EditorUI.Icons.PingPong : EditorMicroAnimations.EditorUI.Icons.PingPongOnce;
                        playModeEnum.SetTooltip("PingPong - current value goes between start and target values back and forth");
                        break;
                    case PlayMode.Spring:
                        animatedIconTextures = EditorMicroAnimations.EditorUI.Icons.Spring;
                        playModeEnum.SetTooltip("Spring - current value goes between start and target values back and forth and returns to the start value as if connected by a spring");
                        break;
                    case PlayMode.Shake:
                        animatedIconTextures = EditorMicroAnimations.EditorUI.Icons.Shake;
                        playModeEnum.SetTooltip("Shake - current value randomly 'shakes' between start and target values back and forth and returns to the start value");
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(playMode), playMode, null);
                }

                playModeField.SetIcon(animatedIconTextures);
                playModeField.iconReaction.Play();
            }

            #endregion

            #region EaseMode

            easeModeEnum?.RegisterValueChangedCallback(evt =>
            {
                try
                {
                    UpdateEaseMode((EaseMode)evt.newValue);
                }
                catch
                {
                    // ignored
                }
            });
            easeModeField.schedule.Execute(() =>
            {
                if (easeModeEnum != null)
                    UpdateEaseMode((EaseMode)easeModeEnum.value);
            });

            void UpdateEaseMode(EaseMode easeMode)
            {
                easeField.SetStyleDisplay(easeMode == EaseMode.Ease ? DisplayStyle.Flex : DisplayStyle.None);
                animationCurveField.SetStyleDisplay(easeMode == EaseMode.AnimationCurve ? DisplayStyle.Flex : DisplayStyle.None);
            }

            #endregion

            return drawer;
        }

        private static FluidField newStartDelayField
        {
            get
            {
                var value =
                    new FloatField { bindingPath = "StartDelay" }
                        .SetTooltip("Animation start delay (seconds)");

                var randomValue =
                    new PropertyField { bindingPath = "RandomStartDelay" }
                        .SetTooltip("Random start delay interval [min]...[max] for the animation");

                var useRandom = new Toggle { bindingPath = "UseRandomStartDelay" };

                FluidField field = NewRandomField
                (
                    "Start Delay",
                    value,
                    randomValue,
                    useRandom
                );

                // field.SetAnimatedIcon(Generic.GetTextures(Generic.AnimationName.StartDelay));

                return field;
            }
        }

        private static FluidField newDurationField
        {
            get
            {
                var value =
                    new FloatField { bindingPath = "Duration" }
                        .SetTooltip("Animation duration (seconds)");

                var randomValue =
                    new PropertyField { bindingPath = "RandomDuration" }
                        .SetTooltip("Random animation duration interval [min]...[max] for the animation");

                var useRandom = new Toggle { bindingPath = "UseRandomDuration" };

                FluidField field = NewRandomField
                (
                    "Duration",
                    value,
                    randomValue,
                    useRandom
                );

                // field.SetAnimatedIcon(Generic.GetTextures(Generic.AnimationName.Duration));

                return field;
            }
        }

        private static FluidField newLoopsField
        {
            get
            {
                var value =
                    new IntegerField { bindingPath = "Loops" }
                        .SetTooltip("Number of loops the animation needs to perform before it stops playing and finishes (-1 means infinite loops)");

                var randomValue =
                    new PropertyField { bindingPath = "RandomLoops" }
                        .SetTooltip("Random number of loops interval [min]...[max] for the animation");

                var useRandom = new Toggle { bindingPath = "UseRandomLoops" };

                FluidField field = NewRandomField
                (
                    "Loops",
                    value,
                    randomValue,
                    useRandom
                );

                // field.SetAnimatedIcon(EditorMicroAnimations.Dween. GetTextures(EditorMicroAnimations.Dween. AnimationName.Loop));

                return field;
            }
        }

        private static FluidField newLoopDelayField
        {
            get
            {
                var value =
                    new FloatField { bindingPath = "LoopDelay" }
                        .SetTooltip("Time delay between loops (seconds)");

                var randomValue =
                    new PropertyField { bindingPath = "RandomLoopDelay" }
                        .SetTooltip("Random delay interval [min]...[max] between loops");

                var useRandom = new Toggle { bindingPath = "UseRandomLoopDelay" };

                FluidField field = NewRandomField
                (
                    "Loop Delay",
                    value,
                    randomValue,
                    useRandom
                );

                // field.SetAnimatedIcon(Generic.GetTextures(Generic.AnimationName.DelayBetweenLoops));

                return field;
            }
        }

        private static FluidField NewRandomField
        (
            string fieldName,
            VisualElement value,
            VisualElement randomValue,
            Toggle useRandom
        )
        {
            
            value.ResetLayout();
            useRandom.SetStyleDisplay(DisplayStyle.None);

            FluidToggleButtonTab randomButton =
                FluidToggleButtonTab.Get(EditorMicroAnimations.EditorUI.Icons.Dice)
                    .SetTooltip("Random Value")
                    .SetElementSize(ElementSize.Normal)
                    .SetTabPosition(TabPosition.FloatingTab)
                    .SetOnClick(() => useRandom.value = !useRandom.value);

            FluidField field =
                FluidField.Get()
                    .AddFieldContent(value)
                    .AddFieldContent(randomValue)
                    .AddFieldContent(useRandom)
                    .AddInfoElement(randomButton);

            useRandom.RegisterValueChangedCallback(evt => VisualUpdate(evt.newValue));
            VisualUpdate(useRandom.value);
            field.schedule.Execute(() => VisualUpdate(useRandom.value));
            return field;

            void VisualUpdate(bool useRandomValue)
            {
                field.SetLabelText($"{(useRandomValue ? "Random " : "")}{fieldName}");
                value.SetStyleDisplay(useRandomValue ? DisplayStyle.None : DisplayStyle.Flex);
                randomValue.SetStyleDisplay(useRandomValue ? DisplayStyle.Flex : DisplayStyle.None);
                randomButton.isOn = useRandomValue;
            }
        }
    }
}
