// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using System.Linq;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.ScriptableObjects.Colors;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Editor.UIManager.Editors.Components;
using Doozy.Runtime.UIElements.Extensions;
using Doozy.Runtime.UIManager;
using Doozy.Runtime.UIManager.Components;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Editor.UIManager.Editors.Animators.Internal
{
    public abstract class BaseUISelectableAnimatorEditor : BaseTargetComponentAnimatorEditor
    {
        protected static IEnumerable<Texture2D> uiSelectableIconTextures => UISelectableEditor.selectableIconTextures;
        protected static IEnumerable<Texture2D> uiSelectableAnimatorIconTextures => EditorMicroAnimations.UIManager.Icons.UISelectableAnimator;

        protected FluidToggleButtonTab normalTabButton { get; set; }
        protected FluidToggleButtonTab highlightedTabButton { get; set; }
        protected FluidToggleButtonTab pressedTabButton { get; set; }
        protected FluidToggleButtonTab selectedTabButton { get; set; }
        protected FluidToggleButtonTab disabledTabButton { get; set; }

        protected FluidAnimatedContainer normalAnimatedContainer { get; set; }
        protected FluidAnimatedContainer highlightedAnimatedContainer { get; set; }
        protected FluidAnimatedContainer pressedAnimatedContainer { get; set; }
        protected FluidAnimatedContainer selectedAnimatedContainer { get; set; }
        protected FluidAnimatedContainer disabledAnimatedContainer { get; set; }

        // protected FluidToggleCheckbox isOnCheckbox { get; set; }
        // protected FluidField isOnField { get; set; }

        protected EnumField toggleCommandEnumField { get; set; }
        protected FluidField toggleCommandField { get; set; }

        protected SerializedProperty propertyToggleCommand { get; set; }
        protected SerializedProperty propertyNormalAnimation { get; set; }
        protected SerializedProperty propertyHighlightedAnimation { get; set; }
        protected SerializedProperty propertyPressedAnimation { get; set; }
        protected SerializedProperty propertySelectedAnimation { get; set; }
        protected SerializedProperty propertyDisabledAnimation { get; set; }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            normalTabButton?.Recycle();
            highlightedTabButton?.Recycle();
            pressedTabButton?.Recycle();
            selectedTabButton?.Recycle();
            disabledTabButton?.Recycle();

            toggleCommandField?.Recycle();

            normalAnimatedContainer?.Dispose();
            highlightedAnimatedContainer?.Dispose();
            pressedAnimatedContainer?.Dispose();
            selectedAnimatedContainer?.Dispose();
            disabledAnimatedContainer?.Dispose();
        }

        protected override void FindProperties()
        {
            base.FindProperties();
            propertyToggleCommand = serializedObject.FindProperty("ToggleCommand");
            propertyNormalAnimation = serializedObject.FindProperty("NormalAnimation");
            propertyHighlightedAnimation = serializedObject.FindProperty("HighlightedAnimation");
            propertyPressedAnimation = serializedObject.FindProperty("PressedAnimation");
            propertySelectedAnimation = serializedObject.FindProperty("SelectedAnimation");
            propertyDisabledAnimation = serializedObject.FindProperty("DisabledAnimation");
        }

        protected override void InitializeEditor()
        {
            base.InitializeEditor();

            componentHeader
                .SetComponentNameText(ObjectNames.NicifyVariableName(nameof(UISelectable)))
                .SetIcon(uiSelectableAnimatorIconTextures.ToList());
        }

        protected override void InitializeAnimatedContainers()
        {
            base.InitializeAnimatedContainers();
            normalAnimatedContainer = GetAnimatedContainer(propertyNormalAnimation);
            highlightedAnimatedContainer = GetAnimatedContainer(propertyHighlightedAnimation);
            pressedAnimatedContainer = GetAnimatedContainer(propertyPressedAnimation);
            selectedAnimatedContainer = GetAnimatedContainer(propertySelectedAnimation);
            disabledAnimatedContainer = GetAnimatedContainer(propertyDisabledAnimation);
        }

        protected override void ComposeAnimatedContainers()
        {
            animatedContainers
                .AddChild(normalAnimatedContainer)
                .AddChild(highlightedAnimatedContainer)
                .AddChild(pressedAnimatedContainer)
                .AddChild(selectedAnimatedContainer)
                .AddChild(disabledAnimatedContainer);
        }

        protected override void InitializeController()
        {
            controllerObjectField =
                DesignUtils.NewObjectField(propertyController, typeof(UISelectable))
                    .SetTooltip($"{ObjectNames.NicifyVariableName(nameof(UISelectable))} controller")
                    .SetStyleFlexGrow(1);

            toggleCommandEnumField =
                DesignUtils.NewEnumField(propertyToggleCommand)
                    .SetStyleWidth(50, 50, 50)
                    .SetStyleAlignSelf(Align.Center)
                    .SetStyleMarginRight(DesignUtils.k_Spacing);

            void ShowToggleCommand(bool show) =>
                toggleCommandEnumField.SetStyleDisplay(show ? DisplayStyle.Flex : DisplayStyle.None);

            ShowToggleCommand(propertyController.objectReferenceValue != null && ((UISelectable)propertyController.objectReferenceValue).isToggle);
            controllerObjectField.RegisterValueChangedCallback(evt =>
            {
                if (evt.newValue == null)
                {
                    ShowToggleCommand(false);
                    return;
                }

                ShowToggleCommand(((UISelectable)evt.newValue).isToggle);
            });

            controllerField =
                FluidField.Get()
                    .SetLabelText($"Controller")
                    .SetIcon(uiSelectableIconTextures)
                    .SetStyleMinWidth(200)
                    .AddFieldContent
                    (
                        DesignUtils.row
                            .SetStyleFlexGrow(0)
                            .AddChild(toggleCommandEnumField)
                            .AddChild(controllerObjectField)
                    );
        }


        protected static FluidToggleButtonTab GetTab(UISelectionState state, FluidAnimatedContainer targetContainer, EditorSelectableColorInfo selectableColor) =>
            GetTab(targetContainer, state.ToString(), $"{state} selection state animation", selectableColor);
    }
}
