// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Editor.Common.Drawers;
using Doozy.Editor.EditorUI;
using Doozy.Editor.UIManager.ScriptableObjects;
using Doozy.Editor.UIManager.Windows;
using Doozy.Runtime.UIManager;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Doozy.Editor.UIManager.Drawers
{
    [CustomPropertyDrawer(typeof(UIToggleId), true)]
    public class UIToggleIdDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {}
        
        public override VisualElement CreatePropertyGUI(SerializedProperty property) =>
            CategoryNameIdUtils.CreateDrawer
            (
                property,
                () => UIToggleIdDatabase.instance.database.GetCategories(),
                targetCategory => UIToggleIdDatabase.instance.database.GetNames(targetCategory),
                EditorMicroAnimations.UIManager.Icons.TogglesDatabase,
                TogglesDatabaseWindow.Open,
                "Open Toggles Database Window",
                UIToggleIdDatabase.instance,
                EditorSelectableColors.UIManager.UIComponent
            );
    }
}
