// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using System.Linq;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Editor.Reactor.Components;
using Doozy.Editor.UIManager.Editors.Animators.Internal;
using Doozy.Runtime.UIElements.Extensions;
using Doozy.Runtime.UIManager;
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
    [CustomEditor(typeof(UISelectableUIAnimator), true)]
    public class UISelectableUIAnimatorEditor : BaseUISelectableAnimatorEditor
    {
        public static IEnumerable<Texture2D> uiAnimatorIconTextures => EditorMicroAnimations.Reactor.Icons.UIAnimator;
        public static IEnumerable<Texture2D> uiAnimationIconTextures => EditorMicroAnimations.Reactor.Icons.UIAnimation;

        public UISelectableUIAnimator castedTarget => (UISelectableUIAnimator)target;
        public IEnumerable<UISelectableUIAnimator> castedTargets => targets.Cast<UISelectableUIAnimator>();

        private VisualElement normalTabContainer { get; set; }
        private VisualElement highlightedTabContainer { get; set; }
        private VisualElement pressedTabContainer { get; set; }
        private VisualElement selectedTabContainer { get; set; }
        private VisualElement disabledTabContainer { get; set; }

        private UIAnimatorIndicators normalTabIndicators { get; set; }
        private UIAnimatorIndicators highlightedTabIndicators { get; set; }
        private UIAnimatorIndicators pressedTabIndicators { get; set; }
        private UIAnimatorIndicators selectedTabIndicators { get; set; }
        private UIAnimatorIndicators disabledTabIndicators { get; set; }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            normalTabButton?.Recycle();
            highlightedTabButton?.Recycle();
            pressedTabButton?.Recycle();
            selectedTabButton?.Recycle();
            disabledTabButton?.Recycle();

            normalTabIndicators?.Dispose();
            highlightedTabIndicators?.Dispose();
            pressedTabIndicators?.Dispose();
            selectedTabIndicators?.Dispose();
            disabledTabIndicators?.Dispose();
        }

        protected override void InitializeEditor()
        {
            base.InitializeEditor();
            
            componentHeader
                    .SetComponentTypeText("UI Animator")
                    .SetIcon(uiAnimatorIconTextures.ToList())
                    .AddManualButton("https://doozyentertainment.atlassian.net/wiki/spaces/DUI4/pages/1048543277/UISelectable+UI+Animator?atlOrigin=eyJpIjoiMDNjZTc2YWZhZjlhNDU0N2E1YzdmOThjMzQwODJmMWIiLCJwIjoiYyJ9")
                    .AddYouTubeButton();
            
            // foreach (UISelectionState state in UISelectable.uiSelectionStates)
            //     castedTarget.GetAnimation(state).SetTarget(castedTarget.rectTransform);
            
            normalAnimatedContainer.AddOnShowCallback(() => normalAnimatedContainer.Bind(serializedObject));
            highlightedAnimatedContainer.AddOnShowCallback(() => highlightedAnimatedContainer.Bind(serializedObject));
            pressedAnimatedContainer.AddOnShowCallback(() => pressedAnimatedContainer.Bind(serializedObject));
            selectedAnimatedContainer.AddOnShowCallback(() => selectedAnimatedContainer.Bind(serializedObject));
            disabledAnimatedContainer.AddOnShowCallback(() => disabledAnimatedContainer.Bind(serializedObject));
        }

        protected override void InitializeTabs()
        {
            base.InitializeTabs();

            const float tabWidth = 68f;

            VisualElement GetTabContainer() =>
                new VisualElement()
                    .SetStyleWidth(tabWidth);

            normalTabContainer = GetTabContainer();
            highlightedTabContainer = GetTabContainer();
            pressedTabContainer = GetTabContainer();
            selectedTabContainer = GetTabContainer();
            disabledTabContainer = GetTabContainer();

            normalTabButton = GetTab(UISelectionState.Normal, normalAnimatedContainer, selectableAccentColor);
            highlightedTabButton = GetTab(UISelectionState.Highlighted, highlightedAnimatedContainer, selectableAccentColor);
            pressedTabButton = GetTab(UISelectionState.Pressed, pressedAnimatedContainer, selectableAccentColor);
            selectedTabButton = GetTab(UISelectionState.Selected, selectedAnimatedContainer, selectableAccentColor);
            disabledTabButton = GetTab(UISelectionState.Disabled, disabledAnimatedContainer, selectableAccentColor);
           
            normalTabIndicators = new UIAnimatorIndicators().Initialize(propertyNormalAnimation, normalTabButton, normalTabContainer);
            highlightedTabIndicators = new UIAnimatorIndicators().Initialize(propertyHighlightedAnimation, highlightedTabButton, highlightedTabContainer);
            pressedTabIndicators = new UIAnimatorIndicators().Initialize(propertyPressedAnimation, pressedTabButton, pressedTabContainer);
            selectedTabIndicators = new UIAnimatorIndicators().Initialize(propertySelectedAnimation, selectedTabButton, selectedTabContainer);
            disabledTabIndicators = new UIAnimatorIndicators().Initialize(propertyDisabledAnimation, disabledTabButton, disabledTabContainer);

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
    }
}
