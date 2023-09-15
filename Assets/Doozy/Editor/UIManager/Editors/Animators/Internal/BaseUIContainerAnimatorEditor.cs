// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using System.Linq;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Runtime.UIElements.Extensions;
using Doozy.Runtime.UIManager.Containers;
using UnityEditor;
using UnityEngine;
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Editor.UIManager.Editors.Animators.Internal
{
    public abstract class BaseUIContainerAnimatorEditor : BaseTargetComponentAnimatorEditor
    {
        protected static IEnumerable<Texture2D> uiContainerIconTextures => EditorMicroAnimations.UIManager.Icons.UIContainer;
        protected static IEnumerable<Texture2D> uiContainerAnimatorIconTextures => EditorMicroAnimations.EditorUI.Icons.Animator;

        protected FluidToggleButtonTab showTabButton { get; set; }
        protected FluidToggleButtonTab hideTabButton { get; set; }

        protected FluidAnimatedContainer showAnimatedContainer { get; set; }
        protected FluidAnimatedContainer hideAnimatedContainer { get; set; }

        protected SerializedProperty propertyShowAnimation { get; set; }
        protected SerializedProperty propertyHideAnimation { get; set; }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            showTabButton?.Recycle();
            hideTabButton?.Recycle();

            showAnimatedContainer?.Dispose();
            hideAnimatedContainer?.Dispose();
        }

        protected override void FindProperties()
        {
            base.FindProperties();

            propertyShowAnimation = serializedObject.FindProperty("ShowAnimation");
            propertyHideAnimation = serializedObject.FindProperty("HideAnimation");
        }

        protected override void InitializeEditor()
        {
            base.InitializeEditor();

            componentHeader
                .SetComponentNameText(ObjectNames.NicifyVariableName(nameof(UIContainer)))
                .SetIcon(uiContainerAnimatorIconTextures.ToList());
        }

        protected override void InitializeAnimatedContainers()
        {
            base.InitializeAnimatedContainers();
            showAnimatedContainer = GetAnimatedContainer(propertyShowAnimation);
            hideAnimatedContainer = GetAnimatedContainer(propertyHideAnimation);
        }

        protected override void ComposeAnimatedContainers()
        {
            animatedContainers
                .AddChild(showAnimatedContainer)
                .AddChild(hideAnimatedContainer);
        }

        protected override void InitializeController()
        {
            controllerObjectField =
                DesignUtils.NewObjectField(propertyController, typeof(UIContainer))
                    .SetTooltip($"{ObjectNames.NicifyVariableName(nameof(UIContainer))} controller")
                    .SetStyleFlexGrow(1);

            controllerField =
                FluidField.Get()
                    .SetLabelText($"Controller")
                    .SetIcon(uiContainerIconTextures)
                    .SetStyleMinWidth(200)
                    .AddFieldContent(controllerObjectField);
        }
    }
}
