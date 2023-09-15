// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Editor.Common.Extensions;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.ScriptableObjects.Colors;
using Doozy.Runtime.Reactor.Internal;
using Doozy.Runtime.UIElements.Extensions;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using PlayMode = Doozy.Runtime.Reactor.PlayMode;

namespace Doozy.Editor.EditorUI.Drawers
{
    [CustomPropertyDrawer(typeof(EditorSelectableColorInfo))]
    public class EditorSelectableColorInfoDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {}

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var target = property.GetTargetObjectOfProperty() as EditorSelectableColorInfo;

            TemplateContainer root = EditorLayouts.EditorUI.EditorSelectableColorInfo.CloneTree();
            root.AddStyle(EditorStyles.EditorUI.EditorSelectableColorInfo);

            root.Q<VisualElement>("LayoutContainer")
                .SetStyleBackgroundColor(EditorColors.Default.FieldBackground);

            const string onDarkColorGeneratorElementName = "OnDarkColorGeneratorButtonContainer";
            root.Q<VisualElement>(onDarkColorGeneratorElementName)
                .AddChild(
                    GetNewColorGeneratorButton(
                        () => target?.GenerateOnDarkColorVariantsFromNormalColor(),
                        onDarkColorGeneratorElementName,
                        "Generate on dark color variants from the Normal color")
                );

            const string onLightColorGeneratorElementName = "OnDarkColorGeneratorButtonContainer";
            root.Q<VisualElement>(onLightColorGeneratorElementName)
                .AddChild(
                    GetNewColorGeneratorButton(
                        () => target?.GenerateOnLightColorVariantsFromNormalColor(),
                        onLightColorGeneratorElementName,
                        "Generate on light color variants from the Normal color")
                );


            return root;
        }

        private static FluidButton GetNewColorGeneratorButton(UnityAction callback, string elementName, string tooltip)
        {

            FluidButton button = 
                FluidButton.Get()
                .SetIcon(EditorMicroAnimations.EditorUI.Icons.SelectableColorGenerator)
                .SetTooltip(tooltip)
                .AddClass("ESColorInfo")
                .SetName(elementName)
                .SetElementSize(ElementSize.Small)
                .SetButtonStyle(ButtonStyle.Clear)
                .SetOnClick(callback);

            button.iconReaction.SetPlayMode(PlayMode.PingPong);

            return button;
        }
    }
}
