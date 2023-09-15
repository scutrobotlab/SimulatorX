// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Editor.Common.Drawers;
using Doozy.Editor.EditorUI;
using Doozy.Editor.Signals.ScriptableObjects;
using Doozy.Editor.Signals.Windows;
using Doozy.Runtime.Signals;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Doozy.Editor.Signals.Drawers
{
    [CustomPropertyDrawer(typeof(StreamId), true)]
    public class StreamIdDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {}

        public override VisualElement CreatePropertyGUI(SerializedProperty property) =>
            CategoryNameIdUtils.CreateDrawer
            (
                property,
                () => StreamIdDatabase.instance.database.GetCategories(),
                targetCategory => StreamIdDatabase.instance.database.GetNames(targetCategory),
                EditorMicroAnimations.Signals.Icons.StreamDatabase,
                StreamsDatabaseWindow.Open,
                "Open Streams Database Window",
                StreamIdDatabase.instance,
                EditorSelectableColors.Signals.Stream
            );
    }
}
