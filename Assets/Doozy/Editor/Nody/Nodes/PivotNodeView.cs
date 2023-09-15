// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Runtime.Nody;
using Doozy.Runtime.Nody.Nodes;
using Doozy.Runtime.UIElements.Extensions;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using EditorStyles = Doozy.Editor.EditorUI.EditorStyles;

namespace Doozy.Editor.Nody.Nodes
{
    public sealed class PivotNodeView : FlowNodeView
    {
        public override void Dispose()
        {
            base.Dispose();
            orientationButton?.Recycle();
        }

        public override Type nodeType => typeof(PivotNode);
        public override IEnumerable<Texture2D> nodeIconTextures => EditorMicroAnimations.Nody.Icons.PivotNode;

        private FluidButton orientationButton
        {
            get;
            set;
        }

        private SerializedProperty propertyPivotOrientation { get; set; }

        public PivotNodeView(FlowGraphView graphView, FlowNode node) : base(graphView, node)
        {
            showInMiniMap = false;
            this.AddStyle(EditorStyles.Nody.PivotNode);
        }

        protected override void FindProperties()
        {
            base.FindProperties();
            propertyPivotOrientation = serializedObject.FindProperty("PivotOrientation");
        }

        protected override void InitializeView()
        {
            base.InitializeView();
            
            EnumField invisibleEnumField = DesignUtils.NewEnumField(propertyPivotOrientation, true);
            Add(invisibleEnumField);
            OnOrientationChanged((PivotNode.Orientation)propertyPivotOrientation.enumValueIndex);

            orientationButton =
                FluidButton.Get()
                    .SetIcon(EditorMicroAnimations.EditorUI.Icons.Reset)
                    .SetElementSize(ElementSize.Tiny)
                    .SetButtonStyle(ButtonStyle.Clear)
                    .SetOnClick(() =>
                    {
                        PivotNode.Orientation nextOrientation;
                        switch ((PivotNode.Orientation)propertyPivotOrientation.enumValueIndex)
                        {
                            case PivotNode.Orientation.Horizontal:
                                nextOrientation = PivotNode.Orientation.HorizontalReversed;
                                break;
                            case PivotNode.Orientation.HorizontalReversed:
                                nextOrientation = PivotNode.Orientation.Vertical;
                                break;
                            case PivotNode.Orientation.Vertical:
                                nextOrientation = PivotNode.Orientation.VerticalReversed;
                                break;
                            case PivotNode.Orientation.VerticalReversed:
                                nextOrientation = PivotNode.Orientation.Horizontal;
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }

                        invisibleEnumField.value = nextOrientation;
                    });

            invisibleEnumField.RegisterValueChangedCallback(evt =>
            {
                if (evt?.newValue == null) return;
                OnOrientationChanged((PivotNode.Orientation)evt.newValue);
            });

            top.Insert(1, orientationButton);
            top.AddChild(invisibleEnumField);
            top.Bind(serializedObject);
        }

        public void OnOrientationChanged(PivotNode.Orientation orientation)
        {
            switch (orientation)
            {
                case PivotNode.Orientation.Horizontal:
                    top.SetStyleFlexDirection(FlexDirection.Row);
                    break;
                case PivotNode.Orientation.HorizontalReversed:
                    top.SetStyleFlexDirection(FlexDirection.RowReverse);
                    break;
                case PivotNode.Orientation.Vertical:
                    top.SetStyleFlexDirection(FlexDirection.Column);
                    break;
                case PivotNode.Orientation.VerticalReversed:
                    top.SetStyleFlexDirection(FlexDirection.ColumnReverse);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(orientation), orientation, null);
            }
        }
    }
}
