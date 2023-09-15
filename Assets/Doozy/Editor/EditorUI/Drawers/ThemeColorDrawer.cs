// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Runtime.Colors;
using Doozy.Runtime.UIElements.Extensions;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Doozy.Editor.EditorUI.Drawers
{
    [CustomPropertyDrawer(typeof(ThemeColor), true)]
    public class ThemeColorDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {}

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            TemplateContainer container = EditorLayouts.EditorUI.ThemeColor.CloneTree();
            container.AddStyle(EditorStyles.EditorUI.ThemeColor);
            return container;
        }
    }
}
