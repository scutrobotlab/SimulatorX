// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using System.Linq;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.Components.Internal;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Editor.UIManager.Editors.Animators.Internal;
using Doozy.Runtime.Reactor.Targets;
using Doozy.Runtime.UIElements.Extensions;
using Doozy.Runtime.UIManager;
using Doozy.Runtime.UIManager.Animators;
using Doozy.Runtime.UIManager.Components;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Editor.UIManager.Editors.Animators
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(UISelectableSpriteAnimator), true)]
    public class UISelectableSpriteAnimatorEditor : BaseUISelectableAnimatorEditor
    {
        public UISelectableSpriteAnimator castedTarget => (UISelectableSpriteAnimator)target;
        public IEnumerable<UISelectableSpriteAnimator> castedTargets => targets.Cast<UISelectableSpriteAnimator>();

        public static IEnumerable<Texture2D> spriteAnimatorIconTextures => EditorMicroAnimations.Reactor.Icons.SpriteAnimator;
        public static IEnumerable<Texture2D> spriteAnimationIconTextures => EditorMicroAnimations.Reactor.Icons.SpriteAnimation;
        public static IEnumerable<Texture2D> spriteTargetIconTextures => EditorMicroAnimations.Reactor.Icons.SpriteTarget;

        private ObjectField spriteTargetObjectField { get; set; }
        private FluidField spriteTargetFluidField { get; set; }
        private SerializedProperty propertySpriteTarget { get; set; }
        private IVisualElementScheduledItem targetFinder { get; set; }

        private VisualElement normalTabContainer { get; set; }
        private VisualElement highlightedTabContainer { get; set; }
        private VisualElement pressedTabContainer { get; set; }
        private VisualElement selectedTabContainer { get; set; }
        private VisualElement disabledTabContainer { get; set; }

        private EnabledIndicator normalTabIndicator { get; set; }
        private EnabledIndicator highlightedTabIndicator { get; set; }
        private EnabledIndicator pressedTabIndicator { get; set; }
        private EnabledIndicator selectedTabIndicator { get; set; }
        private EnabledIndicator disabledTabIndicator { get; set; }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            spriteTargetFluidField?.Recycle();
            
            normalTabButton?.Recycle();
            highlightedTabButton?.Recycle();
            pressedTabButton?.Recycle();
            selectedTabButton?.Recycle();
            disabledTabButton?.Recycle();

            normalTabIndicator?.Recycle();
            highlightedTabIndicator?.Recycle();
            pressedTabIndicator?.Recycle();
            selectedTabIndicator?.Recycle();
            disabledTabIndicator?.Recycle();
        }

        protected override void FindProperties()
        {
            base.FindProperties();
            propertySpriteTarget = serializedObject.FindProperty("SpriteTarget");
        }

        protected override void InitializeEditor()
        {
            base.InitializeEditor();

            componentHeader
                .SetComponentTypeText("Sprite Animator")
                .SetIcon(spriteAnimatorIconTextures.ToList())
                .AddManualButton("https://doozyentertainment.atlassian.net/wiki/spaces/DUI4/pages/1048281113/UISelectable+Sprite+Animator?atlOrigin=eyJpIjoiNjQxNmZkOTY3NThmNDZiZmE5ODkwYzVkZDRhODUxZmQiLCJwIjoiYyJ9")
                .AddYouTubeButton();

            InitializeSpriteTarget();

            normalAnimatedContainer.AddOnShowCallback(() => normalAnimatedContainer.Bind(serializedObject));
            highlightedAnimatedContainer.AddOnShowCallback(() => highlightedAnimatedContainer.Bind(serializedObject));
            pressedAnimatedContainer.AddOnShowCallback(() => pressedAnimatedContainer.Bind(serializedObject));
            selectedAnimatedContainer.AddOnShowCallback(() => selectedAnimatedContainer.Bind(serializedObject));
            disabledAnimatedContainer.AddOnShowCallback(() => disabledAnimatedContainer.Bind(serializedObject));
            
            targetFinder = root.schedule.Execute(() =>
            {
                if (castedTarget == null)
                    return;

                if (castedTarget.spriteTarget != null)
                {
                    foreach (UISelectionState state in UISelectable.uiSelectionStates)
                        castedTarget.GetAnimation(state).SetTarget(castedTarget.spriteTarget);
                    
                    targetFinder.Pause();
                    return;
                }

                castedTarget.FindTarget();

            }).Every(1000);
        }

        private void InitializeSpriteTarget()
        {
            spriteTargetObjectField =
                DesignUtils.NewObjectField(propertySpriteTarget, typeof(ReactorSpriteTarget))
                    .SetStyleFlexGrow(1)
                    .SetTooltip("Animation sprite target");
            spriteTargetFluidField =
                FluidField.Get()
                    .SetLabelText("Sprite Target")
                    .SetIcon(spriteTargetIconTextures)
                    .SetStyleFlexShrink(0)
                    .AddFieldContent(spriteTargetObjectField);
        }
      
        protected override void InitializeTabs()
        {
            base.InitializeTabs();

            (normalTabButton, normalTabIndicator, normalTabContainer) = DesignUtils.GetTabButtonForComponentSectionWithEnabledIndicator(null, selectableAccentColor, accentColor);
            (highlightedTabButton, highlightedTabIndicator, highlightedTabContainer) = DesignUtils.GetTabButtonForComponentSectionWithEnabledIndicator(null, selectableAccentColor, accentColor);
            (pressedTabButton, pressedTabIndicator, pressedTabContainer) = DesignUtils.GetTabButtonForComponentSectionWithEnabledIndicator(null, selectableAccentColor, accentColor);
            (selectedTabButton, selectedTabIndicator, selectedTabContainer) = DesignUtils.GetTabButtonForComponentSectionWithEnabledIndicator(null, selectableAccentColor, accentColor);
            (disabledTabButton, disabledTabIndicator, disabledTabContainer) = DesignUtils.GetTabButtonForComponentSectionWithEnabledIndicator(null, selectableAccentColor, accentColor);

            const float tabWidth = 68f;
            normalTabContainer.SetStyleWidth(tabWidth);
            highlightedTabContainer.SetStyleWidth(tabWidth);
            pressedTabContainer.SetStyleWidth(tabWidth);
            selectedTabContainer.SetStyleWidth(tabWidth);
            disabledTabContainer.SetStyleWidth(tabWidth);

            void InitializeTab(UISelectionState state, FluidToggleButtonTab tab, FluidAnimatedContainer animatedContainer)
            {
                tab
                    .SetLabelText(state.ToString())
                    .SetOnValueChanged(evt => animatedContainer.Toggle(evt.newValue));
            }

            InitializeTab(UISelectionState.Normal, normalTabButton, normalAnimatedContainer);
            InitializeTab(UISelectionState.Highlighted, highlightedTabButton, highlightedAnimatedContainer);
            InitializeTab(UISelectionState.Pressed, pressedTabButton, pressedAnimatedContainer);
            InitializeTab(UISelectionState.Selected, selectedTabButton, selectedAnimatedContainer);
            InitializeTab(UISelectionState.Disabled, disabledTabButton, disabledAnimatedContainer);

            SerializedProperty propertyNormalAnimationEnabled = propertyNormalAnimation.FindPropertyRelative("Animation.Enabled");
            SerializedProperty propertyHighlightedAnimationEnabled = propertyHighlightedAnimation.FindPropertyRelative("Animation.Enabled");
            SerializedProperty propertyPressedAnimationEnabled = propertyPressedAnimation.FindPropertyRelative("Animation.Enabled");
            SerializedProperty propertySelectedAnimationEnabled = propertySelectedAnimation.FindPropertyRelative("Animation.Enabled");
            SerializedProperty propertyDisabledAnimationEnabled = propertyDisabledAnimation.FindPropertyRelative("Animation.Enabled");

            UpdateIndicators(false);
            root.schedule.Execute(() => UpdateIndicators(true)).Every(200);

            void UpdateIndicators(bool animateChange)
            {
                normalTabIndicator.Toggle(propertyNormalAnimationEnabled.boolValue, animateChange);
                highlightedTabIndicator.Toggle(propertyHighlightedAnimationEnabled.boolValue, animateChange);
                pressedTabIndicator.Toggle(propertyPressedAnimationEnabled.boolValue, animateChange);
                selectedTabIndicator.Toggle(propertySelectedAnimationEnabled.boolValue, animateChange);
                disabledTabIndicator.Toggle(propertyDisabledAnimationEnabled.boolValue, animateChange);
            }

            tabsGroup = FluidToggleGroup.Get().SetControlMode(FluidToggleGroup.ControlMode.OneToggleOn);
            normalTabButton.AddToToggleGroup(tabsGroup);
            highlightedTabButton.AddToToggleGroup(tabsGroup);
            pressedTabButton.AddToToggleGroup(tabsGroup);
            selectedTabButton.AddToToggleGroup(tabsGroup);
            disabledTabButton.AddToToggleGroup(tabsGroup);
        }
        
        protected override void ComposeTabs()
        {
            tabsContainer
                .AddChild(normalTabContainer)
                .AddChild(DesignUtils.spaceBlock2X)
                .AddChild(highlightedTabContainer)
                .AddChild(DesignUtils.spaceBlock2X)
                .AddChild(pressedTabContainer)
                .AddChild(DesignUtils.spaceBlock2X)
                .AddChild(selectedTabContainer)
                .AddChild(DesignUtils.spaceBlock2X)
                .AddChild(disabledTabContainer)
                .AddChild(DesignUtils.spaceBlock2X)
                .AddChild(DesignUtils.flexibleSpace)
                .AddChild(DesignUtils.spaceBlock)
                .AddChild
                (
                    DesignUtils.SystemButton_SortComponents
                    (
                        castedTarget.gameObject,
                        nameof(RectTransform),
                        nameof(Canvas),
                        nameof(CanvasGroup),
                        nameof(GraphicRaycaster)
                    )
                );
        }

        protected override void Compose()
        {
            ComposeTabs();
            ComposeAnimatedContainers();

            root
                .AddChild(componentHeader)
                .AddChild(tabsContainer)
                .AddChild(DesignUtils.spaceBlock)
                .AddChild(animatedContainers)
                .AddChild
                (
                    DesignUtils.row
                        .AddChild(controllerField)
                        .AddChild(DesignUtils.spaceBlock)
                        .AddChild(spriteTargetFluidField)
                )
                .AddChild(DesignUtils.endOfLineBlock);
        }
    }
}
