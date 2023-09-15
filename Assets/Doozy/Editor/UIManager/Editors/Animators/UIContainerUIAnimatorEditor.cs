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
    [CustomEditor(typeof(UIContainerUIAnimator), true)]
    public class UIContainerUIAnimatorEditor : BaseUIContainerAnimatorEditor
    {
        public static IEnumerable<Texture2D> uiAnimatorIconTextures => EditorMicroAnimations.Reactor.Icons.UIAnimator;

        public UIContainerUIAnimator castedTarget => (UIContainerUIAnimator)target;
        public IEnumerable<UIContainerUIAnimator> castedTargets => targets.Cast<UIContainerUIAnimator>();

        private VisualElement showTabContainer { get; set; }
        private VisualElement hideTabContainer { get; set; }

        private UIAnimatorIndicators showTabIndicators { get; set; }
        private UIAnimatorIndicators hideTabIndicators { get; set; }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            showTabIndicators?.Dispose();
            hideTabIndicators?.Dispose();
        }

        protected override void InitializeEditor()
        {
            base.InitializeEditor();

            componentHeader
                .SetComponentTypeText("UI Animator")
                .SetIcon(uiAnimatorIconTextures.ToList())
                .AddManualButton("https://doozyentertainment.atlassian.net/wiki/spaces/DUI4/pages/1048707091/UIContainer+UI+Animator?atlOrigin=eyJpIjoiZGRjNzc3ZGFkZDljNGQwMGJmMDMzMTMwMmFmYTNjNTUiLCJwIjoiYyJ9")
                .AddYouTubeButton();

            // castedTarget.showAnimation.SetTarget(castedTarget.rectTransform);
            // castedTarget.hideAnimation.SetTarget(castedTarget.rectTransform);

            showAnimatedContainer.AddOnShowCallback(() => showAnimatedContainer.Bind(serializedObject));
            hideAnimatedContainer.AddOnShowCallback(() => hideAnimatedContainer.Bind(serializedObject));
        }

        protected override void InitializeTabs()
        {
            base.InitializeTabs();

            const float tabWidth = 68f;

            VisualElement GetTabContainer() =>
                new VisualElement()
                    .SetStyleWidth(tabWidth);

            showTabContainer = GetTabContainer();
            hideTabContainer = GetTabContainer();

            showTabButton = GetTab(showAnimatedContainer, "Show", "Show animation", selectableAccentColor);
            hideTabButton = GetTab(hideAnimatedContainer, "Hide", "Hide animation", selectableAccentColor);

            showTabIndicators = new UIAnimatorIndicators().Initialize(propertyShowAnimation, showTabButton, showTabContainer);
            hideTabIndicators = new UIAnimatorIndicators().Initialize(propertyHideAnimation, hideTabButton, hideTabContainer);

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
    }
}
