// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.ScriptableObjects.Styles;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Doozy.Editor.EditorUI.Drawers
{
    [CustomPropertyDrawer(typeof(EditorStyleInfo))]
    public class EditorStyleInfoDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {}

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            SerializedProperty value = property.FindPropertyRelative("UssReference");
            
            var root = new ComponentField
            (
                ComponentField.Size.Small,
                string.Empty,
                new ObjectField {bindingPath = "UssReference", objectType = typeof(StyleSheet)}
            );
            return root;
        }
    }
}
