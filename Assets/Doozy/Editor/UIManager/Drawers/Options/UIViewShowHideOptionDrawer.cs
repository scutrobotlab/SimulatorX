// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Editor.Common.Drawers;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Editor.UIManager.ScriptableObjects;
using Doozy.Editor.UIManager.Windows;
using Doozy.Runtime.UIElements.Extensions;
using Doozy.Runtime.UIManager;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Doozy.Editor.UIManager.Drawers
{
    [CustomPropertyDrawer(typeof(UIViewShowHideOption), true)]
    public class UIViewShowHideOptionDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {}

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            SerializedProperty propertyId = property.FindPropertyRelative("Id");
            SerializedProperty propertyMode = property.FindPropertyRelative("Mode");

            var drawer = new VisualElement();

            VisualElement id =
                CategoryNameIdUtils.CreateDrawer
                (
                    propertyId,
                    () => UIViewIdDatabase.instance.database.GetCategories(),
                    targetCategory => UIViewIdDatabase.instance.database.GetNames(targetCategory),
                    EditorMicroAnimations.UIManager.Icons.ViewsDatabase,
                    ViewsDatabaseWindow.Open,
                    "Open Views Database Window",
                    UIViewIdDatabase.instance,
                    EditorSelectableColors.UIManager.UIComponent
                );

            id.Q<FluidButton>().SetStyleDisplay(DisplayStyle.None);
            id.Q<VisualElement>("drawer").SetStylePaddingLeft(DesignUtils.k_Spacing);
            
            EnumField modeEnumField =
                DesignUtils.NewEnumField(propertyMode)
                    .SetStyleMarginTop(DesignUtils.k_Spacing)
                    .SetStyleMarginLeft(DesignUtils.k_Spacing)
                    .SetStyleMarginRight(92)
                    .SetStyleFlexGrow(1);

            drawer
                .AddChild(id)
                .AddChild(modeEnumField);

            return drawer;
        }
    }
}
