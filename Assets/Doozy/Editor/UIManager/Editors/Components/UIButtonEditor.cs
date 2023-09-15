// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using System.Linq;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.ScriptableObjects.Colors;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Editor.UIManager.Editors.Components.Internal;
using Doozy.Runtime.UIElements.Extensions;
using Doozy.Runtime.UIManager.Components;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Editor.UIManager.Editors.Components
{
    [CustomEditor(typeof(UIButton), true)]
    [CanEditMultipleObjects]
    public sealed class UIButtonEditor : UISelectableBaseEditor
    {
        public override Color accentColor => EditorColors.UIManager.UIComponent;
        public override EditorSelectableColorInfo selectableAccentColor => EditorSelectableColors.UIManager.UIComponent;

        public UIButton castedTarget => (UIButton)target;
        public IEnumerable<UIButton> castedTargets => targets.Cast<UIButton>();

        public static IEnumerable<Texture2D> buttonIconTextures => EditorMicroAnimations.UIManager.Icons.Buttons;

        private FluidField idField { get; set; }

        private SerializedProperty propertyId { get; set; }
        private SerializedProperty propertyBehaviours { get; set; }

        protected override void FindProperties()
        {
            base.FindProperties();

            propertyId = serializedObject.FindProperty(nameof(UIButton.Id));
            propertyBehaviours = serializedObject.FindProperty("Behaviours");
        }

        protected override void InitializeEditor()
        {
            base.InitializeEditor();

            componentHeader
                .SetAccentColor(accentColor)
                .SetComponentNameText((ObjectNames.NicifyVariableName(nameof(UIButton))))
                .SetIcon(buttonIconTextures.ToList())
                .AddManualButton("https://doozyentertainment.atlassian.net/wiki/spaces/DUI4/pages/1048117262/UIButton?atlOrigin=eyJpIjoiOTY4ZDg1Yjk5NDYwNDRmMmFmZDg4OWQyM2VlMjBmNmQiLCJwIjoiYyJ9")
                .AddYouTubeButton();

            idField = 
                FluidField.Get()
                    .AddFieldContent(DesignUtils.NewPropertyField(propertyId));
        }

        protected override void Compose()
        {
            if (castedTarget == null)
                return;

            root
                .AddChild(componentHeader)
                .AddChild
                (
                    DesignUtils.row
                        .SetStyleMargins(50, -4, DesignUtils.k_Spacing2X, DesignUtils.k_Spacing2X)
                        .AddChild(settingsTabButton)
                        .AddChild(DesignUtils.spaceBlock2X)
                        .AddChild(statesTabButton)
                        .AddChild(DesignUtils.spaceBlock)
                        .AddChild(navigationTabButton)
                        .AddChild(DesignUtils.flexibleSpace)
                        .AddChild(DesignUtils.spaceBlock2X)
                        .AddChild
                        (
                            DesignUtils.SystemButton_RenameComponent
                            (
                                castedTarget.gameObject,
                                () => $"Button - {castedTarget.Id.Category} {castedTarget.Id.Name}"
                            )
                        )
                        .AddChild(DesignUtils.spaceBlock)
                        .AddChild
                        (
                            DesignUtils.SystemButton_SortComponents
                            (
                                castedTarget.gameObject,
                                nameof(RectTransform),
                                nameof(Canvas),
                                nameof(CanvasGroup),
                                nameof(GraphicRaycaster),
                                nameof(UIButton)
                            )
                        )
                )
                .AddChild(DesignUtils.spaceBlock)
                .AddChild
                (
                    settingsAnimatedContainer
                        .AddContent
                        (
                            DesignUtils.column
                                .AddChild(DesignUtils.spaceBlock)
                                .AddChild
                                (
                                    DesignUtils.row
                                        .AddChild(interactableCheckbox)
                                        .AddChild(DesignUtils.spaceBlock)
                                        .AddChild(deselectAfterPressCheckbox)
                                        .AddChild(DesignUtils.spaceBlock)
                                        .AddChild(GetStateButtons())
                                )
                                .AddChild(DesignUtils.spaceBlock)
                                .AddChild(idField)
                                .AddChild(DesignUtils.spaceBlock)
                                .AddChild(DesignUtils.NewPropertyField(propertyBehaviours))
                                .AddChild(DesignUtils.endOfLineBlock)
                        )
                )
                .AddChild(statesAnimatedContainer)
                .AddChild(navigationAnimatedContainer);
        }

    }
}
