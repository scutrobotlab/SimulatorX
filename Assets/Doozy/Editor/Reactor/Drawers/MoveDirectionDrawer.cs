// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.UIElements;
using Doozy.Runtime.Colors;
using Doozy.Runtime.Reactor;
using Doozy.Runtime.UIElements.Extensions;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace Doozy.Editor.Reactor.Drawers
{
    [CustomPropertyDrawer(typeof(MoveDirection), true)]
    public class MoveDirectionDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {}


        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            UnityAction<MoveDirection> action = delegate {};

            var drawer = new VisualElement();

            var directionEnum = new EnumField().BindToProperty(property).ResetLayout().SetStyleMinWidth(120);
            var directionField = FluidField.Get(property.displayName, directionEnum).LayoutContainerResetLayout();

            const int cellSize = 7;
            const int cellSpacing = 3;
            const int cellCornerRadius = 4;
            Color normalColor = EditorColors.Default.Selection;
            Color selectedColor = EditorColors.Default.TextTitle.WithAlpha(0.7f);
            Color hoverColor = selectedColor.WithAlpha(0.4f);


            var customTopLeft = GetElement(cellSize, cellSize, MoveDirection.CustomPosition, cellCornerRadius, 0, cellCornerRadius);
            var customTopRight = GetElement(cellSize, cellSize, MoveDirection.CustomPosition, 0, cellCornerRadius, 0, cellCornerRadius);
            var customBottomRight = GetElement(cellSize, cellSize, MoveDirection.CustomPosition, cellCornerRadius, 0, cellCornerRadius);
            var customBottomLeft = GetElement(cellSize, cellSize, MoveDirection.CustomPosition, 0, cellCornerRadius, 0, cellCornerRadius);

            var top = GetElement(cellSize * 3 + cellSpacing * 2, cellSize, MoveDirection.Top);
            var bottom = GetElement(cellSize * 3 + cellSpacing * 2, cellSize, MoveDirection.Bottom);
            var left = GetElement(cellSize, cellSize * 3 + cellSpacing * 2, MoveDirection.Left);
            var right = GetElement(cellSize, cellSize * 3 + cellSpacing * 2, MoveDirection.Right);

            var topLeft = GetElement(cellSize, cellSize, MoveDirection.TopLeft, cellCornerRadius);
            var topCenter = GetElement(cellSize, cellSize, MoveDirection.TopCenter);
            var topRight = GetElement(cellSize, cellSize, MoveDirection.TopRight, 0, cellCornerRadius);

            var middleLeft = GetElement(cellSize, cellSize, MoveDirection.MiddleLeft);
            var middleCenter = GetElement(cellSize, cellSize, MoveDirection.MiddleCenter, cellCornerRadius, cellCornerRadius, cellCornerRadius, cellCornerRadius);
            var middleRight = GetElement(cellSize, cellSize, MoveDirection.MiddleRight);

            var bottomLeft = GetElement(cellSize, cellSize, MoveDirection.BottomLeft, 0, 0, 0, cellCornerRadius);
            var bottomCenter = GetElement(cellSize, cellSize, MoveDirection.BottomCenter);
            var bottomRight = GetElement(cellSize, cellSize, MoveDirection.BottomRight, 0, 0, cellCornerRadius);

            var widget = new VisualElement();

            widget.AddChild
                (
                    new VisualElement()
                        .SetStyleFlexDirection(FlexDirection.Row)
                        .AddChild(customTopLeft)
                        .AddChild(top.SetStyleMargins(cellSpacing, 0, cellSpacing, 0))
                        .AddChild(customTopRight)
                )
                .AddChild
                (
                    new VisualElement()
                        .SetStyleMarginTop(cellSpacing)
                        .SetStyleFlexDirection(FlexDirection.Row)
                        .AddChild(left)
                        .AddChild
                        (
                            new VisualElement()
                                .SetStyleMargins(cellSpacing, 0, cellSpacing, 0)
                                .SetStyleFlexDirection(FlexDirection.Column)
                                .AddChild
                                (
                                    new VisualElement()
                                        .SetStyleFlexDirection(FlexDirection.Row)
                                        .AddChild(topLeft)
                                        .AddChild(topCenter.SetStyleMargins(cellSpacing, 0, cellSpacing, 0))
                                        .AddChild(topRight)
                                )
                                .AddChild
                                (
                                    new VisualElement()
                                        .SetStyleMarginTop(cellSpacing)
                                        .SetStyleFlexDirection(FlexDirection.Row)
                                        .AddChild(middleLeft)
                                        .AddChild(middleCenter.SetStyleMargins(cellSpacing, 0, cellSpacing, 0))
                                        .AddChild(middleRight)
                                )
                                .AddChild
                                (
                                    new VisualElement()
                                        .SetStyleMarginTop(cellSpacing)
                                        .SetStyleFlexDirection(FlexDirection.Row)
                                        .AddChild(bottomLeft)
                                        .AddChild(bottomCenter.SetStyleMargins(cellSpacing, 0, cellSpacing, 0))
                                        .AddChild(bottomRight)
                                )
                        )
                        .AddChild(right)
                )
                .AddChild
                (
                    new VisualElement()
                        .SetStyleMarginTop(cellSpacing)
                        .SetStyleFlexDirection(FlexDirection.Row)
                        .AddChild(customBottomLeft)
                        .AddChild(bottom.SetStyleMargins(cellSpacing, 0, cellSpacing, 0))
                        .AddChild(customBottomRight)
                );

            drawer.Add
            (
                new VisualElement()
                    .SetStyleFlexDirection(FlexDirection.Row)
                    .AddChild(widget)
                    .AddSpace(cellSpacing * 2, 0)
                    .AddChild(directionField)
            );

            VisualElement GetElement(int width, int height, MoveDirection moveDirection, int radiusTopLeft = 0, int radiusTopRight = 0, int radiusBottomRight = 0, int radiusBottomLeft = 0)
            {
                VisualElement element =
                    new VisualElement()
                        .SetStyleSize(width, height)
                        .SetStyleBorderRadius(radiusTopLeft, radiusTopRight, radiusBottomRight, radiusBottomLeft)
                        .SetTooltip(moveDirection.ToString());
                element.AddManipulator(new Clickable(() =>
                {
                    property.enumValueIndex = (int)moveDirection;
                    property.serializedObject.ApplyModifiedProperties();
                }));

                element.RegisterCallback<MouseEnterEvent>(evt => element.SetStyleBackgroundColor(hoverColor));
                element.RegisterCallback<MouseLeaveEvent>(evt => element.SetStyleBackgroundColor((MoveDirection)property.enumValueIndex == moveDirection ? selectedColor : normalColor));

                action += direction => element.SetStyleBackgroundColor(direction == moveDirection ? selectedColor : normalColor);
                return element;
            }

            action.Invoke((MoveDirection)property.enumValueIndex);
            directionEnum.RegisterValueChangedCallback(value =>
            {
                try
                {
                    action?.Invoke((MoveDirection)value.newValue);
                }
                catch
                {
                    // ignored
                }
            });
            // action += direction => property.enumValueIndex = (int)direction;
            return drawer;
        }

    }
}
