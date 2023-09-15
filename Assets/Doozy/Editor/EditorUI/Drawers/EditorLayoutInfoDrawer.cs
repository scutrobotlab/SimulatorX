// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.ScriptableObjects.Layouts;
using Doozy.Runtime.UIElements.Extensions;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Doozy.Editor.EditorUI.Drawers
{
    [CustomPropertyDrawer(typeof(EditorLayoutInfo))]
    public class EditorLayoutInfoDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {}

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            ComponentField root =
                new ComponentField
                    (
                        ComponentField.Size.Small,
                        string.Empty,
                        new ObjectField { bindingPath = "UxmlReference", objectType = typeof(VisualTreeAsset) }
                    )
                    .SetStyleMargins(0, 2, 0, 2);
            return root;
        }
    }
}
