// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Runtime.Reactor.Animations;
using Doozy.Runtime.UIElements.Extensions;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Doozy.Editor.Reactor.Components
{
    public class UIAnimatorIndicators : VisualElement, IDisposable
    {
        public void Dispose()
        {
            moveIndicator?.Recycle();
            rotateIndicator?.Recycle();
            scaleIndicator?.Recycle();
            fadeIndicator?.Recycle();
        }

        private EnabledIndicator moveIndicator { get; }
        private EnabledIndicator rotateIndicator { get; }
        private EnabledIndicator scaleIndicator { get; }
        private EnabledIndicator fadeIndicator { get; }

        public Func<bool> moveEnabledGetter { get; set; }
        public Func<bool> rotateEnabledGetter { get; set; }
        public Func<bool> scaleEnabledGetter { get; set; }
        public Func<bool> fadeEnabledGetter { get; set; }
        
        private IVisualElementScheduledItem scheduledItem { get; }

        public UIAnimatorIndicators(Func<bool> moveEnabledGetter, Func<bool> rotateEnabledGetter, Func<bool> scaleEnabledGetter, Func<bool> fadeEnabledGetter) : this()
        {
            this.moveEnabledGetter = moveEnabledGetter;
            this.rotateEnabledGetter = rotateEnabledGetter;
            this.scaleEnabledGetter = scaleEnabledGetter;
            this.fadeEnabledGetter = fadeEnabledGetter;
        }

        public const float k_IndicatorHeight = 2f;
        
        public UIAnimatorIndicators()
        {
            EnabledIndicator GetIndicator(Color enabledColor) =>
                EnabledIndicator.Get()
                    .SetEnabledColor(enabledColor)
                    .SetStyleFlexGrow(1)
                    .SetStyleHeight(k_IndicatorHeight);

            moveIndicator = GetIndicator(EditorColors.Reactor.Move).SetStyleMarginRight(k_IndicatorHeight);
            rotateIndicator = GetIndicator(EditorColors.Reactor.Rotate).SetStyleMarginRight(k_IndicatorHeight);
            scaleIndicator = GetIndicator(EditorColors.Reactor.Scale).SetStyleMarginRight(k_IndicatorHeight);
            fadeIndicator = GetIndicator(EditorColors.Reactor.Fade);

            this
                .SetStyleFlexDirection(FlexDirection.Row)
                .AddChild(moveIndicator)
                .AddChild(rotateIndicator)
                .AddChild(scaleIndicator)
                .AddChild(fadeIndicator);

            scheduledItem = schedule.Execute(AutoUpdate).Every(200);
            scheduledItem.Pause();
        }

        private void AutoUpdate()
        {
            bool moveEnabled = moveEnabledGetter?.Invoke() ?? false;
            bool rotateEnabled = rotateEnabledGetter?.Invoke() ?? false;
            bool scaleEnabled = scaleEnabledGetter?.Invoke() ?? false;
            bool fadeEnabled = fadeEnabledGetter?.Invoke() ?? false;

            UpdateIndicator(moveEnabled, rotateEnabled, scaleEnabled, fadeEnabled, true);
        }

        public void StartAutoUpdate() =>
            scheduledItem.Resume();

        public void StopAutoUpdate() =>
            scheduledItem.Pause();

        public void UpdateMoveIndicator(bool enabled, bool animateChange) =>
            moveIndicator?.Toggle(enabled, animateChange);

        public void UpdateRotateIndicator(bool enabled, bool animateChange) =>
            rotateIndicator?.Toggle(enabled, animateChange);

        public void UpdateScaleIndicator(bool enabled, bool animateChange) =>
            scaleIndicator?.Toggle(enabled, animateChange);

        public void UpdateFadeIndicator(bool enabled, bool animateChange) =>
            fadeIndicator?.Toggle(enabled, animateChange);

        public void UpdateIndicator(bool moveEnabled, bool rotateEnabled, bool scaleEnabled, bool fadeEnabled, bool animateChange)
        {
            UpdateMoveIndicator(moveEnabled, animateChange);
            UpdateRotateIndicator(rotateEnabled, animateChange);
            UpdateScaleIndicator(scaleEnabled, animateChange);
            UpdateFadeIndicator(fadeEnabled, animateChange);
        }
        
        public static void InitializeTab(SerializedProperty uiAnimationProperty, UIAnimatorIndicators tabIndicators, FluidToggleButtonTab tabButton, VisualElement tabContainer)
        {
            tabContainer.SetStyleFlexGrow(0).AddChild(tabIndicators).AddChild(tabButton);

            SerializedProperty moveEnabledProperty = uiAnimationProperty.FindPropertyRelative($"{nameof(UIAnimation.Move)}.Enabled");
            SerializedProperty rotateEnabledProperty = uiAnimationProperty.FindPropertyRelative($"{nameof(UIAnimation.Rotate)}.Enabled");
            SerializedProperty scaleEnabledProperty = uiAnimationProperty.FindPropertyRelative($"{nameof(UIAnimation.Scale)}.Enabled");
            SerializedProperty fadeEnabledProperty = uiAnimationProperty.FindPropertyRelative($"{nameof(UIAnimation.Fade)}.Enabled");

            tabIndicators.moveEnabledGetter = () => moveEnabledProperty.boolValue;
            tabIndicators.rotateEnabledGetter = () => rotateEnabledProperty.boolValue;
            tabIndicators.scaleEnabledGetter = () => scaleEnabledProperty.boolValue;
            tabIndicators.fadeEnabledGetter = () => fadeEnabledProperty.boolValue;

            tabIndicators.UpdateIndicator
            (
                moveEnabledProperty.boolValue,
                rotateEnabledProperty.boolValue,
                scaleEnabledProperty.boolValue,
                fadeEnabledProperty.boolValue,
                false
            );

            tabIndicators.StartAutoUpdate();
        }

    }
    
    public static class UIAnimatorIndicatorsExtensions
    {
        public static T Initialize<T>(this T target, SerializedProperty uiAnimationProperty, FluidToggleButtonTab tabButton, VisualElement tabContainer) where T : UIAnimatorIndicators
        {
            tabContainer.SetStyleFlexGrow(0).AddChild(target).AddChild(tabButton);

            SerializedProperty moveEnabledProperty = uiAnimationProperty.FindPropertyRelative($"{nameof(UIAnimation.Move)}.Enabled");
            SerializedProperty rotateEnabledProperty = uiAnimationProperty.FindPropertyRelative($"{nameof(UIAnimation.Rotate)}.Enabled");
            SerializedProperty scaleEnabledProperty = uiAnimationProperty.FindPropertyRelative($"{nameof(UIAnimation.Scale)}.Enabled");
            SerializedProperty fadeEnabledProperty = uiAnimationProperty.FindPropertyRelative($"{nameof(UIAnimation.Fade)}.Enabled");

            target.moveEnabledGetter = () => moveEnabledProperty.boolValue;
            target.rotateEnabledGetter = () => rotateEnabledProperty.boolValue;
            target.scaleEnabledGetter = () => scaleEnabledProperty.boolValue;
            target.fadeEnabledGetter = () => fadeEnabledProperty.boolValue;

            target.UpdateIndicator
            (
                moveEnabledProperty.boolValue,
                rotateEnabledProperty.boolValue,
                scaleEnabledProperty.boolValue,
                fadeEnabledProperty.boolValue,
                false
            );

            target.StartAutoUpdate();
            return target;
        }
    }
}
