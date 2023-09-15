// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Editor.EditorUI.ScriptableObjects.Colors;
using Doozy.Runtime.UIElements.Extensions;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Doozy.Editor.EditorUI.Drawers
{
    [CustomPropertyDrawer(typeof(EditorColorInfo))]
    public class EditorColorInfoDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {}

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            TemplateContainer drawer = EditorLayouts.EditorUI.EditorColorInfo.CloneTree();
            drawer.AddStyle(EditorStyles.EditorUI.EditorColorInfo);
            drawer.Q<VisualElement>("LayoutContainer").SetStyleBackgroundColor(EditorColors.Default.FieldBackground);
            return drawer;
        }
    }
}
