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
using Doozy.Runtime.UIManager.Animators;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Editor.UIManager.Editors.Animators
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(UIContainerSpriteAnimator), true)]
    public class UIContainerSpriteAnimatorEditor : BaseUIContainerAnimatorEditor
    {
        public UIContainerSpriteAnimator castedTarget => (UIContainerSpriteAnimator)target;
        public IEnumerable<UIContainerSpriteAnimator> castedTargets => targets.Cast<UIContainerSpriteAnimator>();

        public static IEnumerable<Texture2D> spriteAnimatorIconTextures => EditorMicroAnimations.Reactor.Icons.SpriteAnimator;
        public static IEnumerable<Texture2D> spriteAnimationIconTextures => EditorMicroAnimations.Reactor.Icons.SpriteAnimation;
        public static IEnumerable<Texture2D> spriteTargetIconTextures => EditorMicroAnimations.Reactor.Icons.SpriteTarget;

        private ObjectField spriteTargetObjectField { get; set; }
        private FluidField spriteTargetFluidField { get; set; }
        private SerializedProperty propertySpriteTarget { get; set; }
        private IVisualElementScheduledItem targetFinder { get; set; }

        private VisualElement showTabContainer { get; set; }
        private VisualElement hideTabContainer { get; set; }

        private EnabledIndicator showTabIndicator { get; set; }
        private EnabledIndicator hideTabIndicator { get; set; }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            spriteTargetFluidField?.Recycle();

            showTabIndicator?.Recycle();
            hideTabIndicator?.Recycle();
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
                .AddManualButton("https://doozyentertainment.atlassian.net/wiki/spaces/DUI4/pages/1048903720/UIContainer+Sprite+Animator?atlOrigin=eyJpIjoiOTE2YzJjMjZjMjQwNDhjNWIzY2I3NTEzYzUwNjRkOGUiLCJwIjoiYyJ9")
                .AddYouTubeButton();

            InitializeSpriteTarget();
            
            showAnimatedContainer.AddOnShowCallback(() => showAnimatedContainer.Bind(serializedObject));
            hideAnimatedContainer.AddOnShowCallback(() => hideAnimatedContainer.Bind(serializedObject));
            
            targetFinder = root.schedule.Execute(() =>
            {
                if (castedTarget == null)
                    return;

                if (castedTarget.spriteTarget != null)
                {
                    castedTarget.showAnimation.SetTarget(castedTarget.spriteTarget);
                    castedTarget.hideAnimation.SetTarget(castedTarget.spriteTarget);
                    
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
            
            (showTabButton, showTabIndicator, showTabContainer) = DesignUtils.GetTabButtonForComponentSectionWithEnabledIndicator(null, selectableAccentColor, accentColor);
            (hideTabButton, hideTabIndicator, hideTabContainer) = DesignUtils.GetTabButtonForComponentSectionWithEnabledIndicator(null, selectableAccentColor, accentColor);
            
            const float tabWidth = 68f;
            showTabContainer.SetStyleWidth(tabWidth);
            hideTabContainer.SetStyleWidth(tabWidth);
            
            void InitializeTab(string labelText, FluidToggleButtonTab tab, FluidAnimatedContainer animatedContainer)
            {
                tab
                    .SetLabelText(labelText)
                    .SetOnValueChanged(evt => animatedContainer.Toggle(evt.newValue));
            }
            
            InitializeTab("Show", showTabButton, showAnimatedContainer);
            InitializeTab("Hide", hideTabButton, hideAnimatedContainer);
            
            SerializedProperty propertyShowAnimationEnabled = propertyShowAnimation.FindPropertyRelative("Animation.Enabled");
            SerializedProperty propertyHideAnimationEnabled = propertyHideAnimation.FindPropertyRelative("Animation.Enabled");
            
            UpdateIndicators(false);
            root.schedule.Execute(() => UpdateIndicators(true)).Every(200);
            
            void UpdateIndicators(bool animateChange)
            {
                showTabIndicator.Toggle(propertyShowAnimationEnabled.boolValue, animateChange);
                hideTabIndicator.Toggle(propertyHideAnimationEnabled.boolValue, animateChange);
            }
            
            tabsGroup = FluidToggleGroup.Get().SetControlMode(FluidToggleGroup.ControlMode.OneToggleOn);
            showTabButton.AddToToggleGroup(tabsGroup);
            hideTabButton.AddToToggleGroup(tabsGroup);
        }
        
        protected override void ComposeTabs()
        {
            tabsContainer
                .AddChild(showTabContainer)
                .AddChild(DesignUtils.spaceBlock2X)
                .AddChild(hideTabContainer)
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
