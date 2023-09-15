// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using System.Linq;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.Components.Internal;
using Doozy.Editor.EditorUI.ScriptableObjects.Colors;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Editor.Reactor.Internal;
using Doozy.Editor.UIElements;
using Doozy.Runtime.Common.Extensions;
using Doozy.Runtime.Nody;
using Doozy.Runtime.Reactor.Extensions;
using Doozy.Runtime.Reactor.Reactions;
using Doozy.Runtime.UIElements.Extensions;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable MemberCanBeProtected.Global

namespace Doozy.Editor.Nody.Nodes.Internal
{
    public abstract class FlowNodeEditor : UnityEditor.Editor
    {
        protected static IEnumerable<Texture2D> customNodeIconTextures => EditorMicroAnimations.Nody.Icons.CustomNode;

        public virtual Color nodeAccentColor => EditorColors.Nody.Color;
        public virtual EditorSelectableColorInfo nodeSelectableAccentColor => EditorSelectableColors.Nody.Color;
        public virtual IEnumerable<Texture2D> nodeIconTextures => EditorMicroAnimations.Nody.Icons.CustomNode;

        protected VisualElement root { get; set; }
        protected FluidComponentHeader componentHeader { get; set; }

        protected FluidField nodeNameField { get; set; }
        protected FluidField nodeDescriptionField { get; set; }

        protected FluidButton copyFlowGraphIdButton { get; set; }
        protected FluidButton copyNodeIdButton { get; set; }
        protected FluidButton copyNodeNameButton { get; set; }

        protected TextField nodeNameTextField { get; set; }
        protected TextField nodeDescriptionTextField { get; set; }

        protected FluidField flowOptionsField { get; set; }
        protected FluidToggleSwitch passthroughSwitch { get; set; }
        protected FluidToggleSwitch clearGraphHistorySwitch { get; set; }

        public Image deleteLockIcon { get; private set; }
        public Texture2DReaction deleteLockIconReaction { get; private set; }

        private VisualElement m_PortsContainer;
        protected VisualElement portsContainer => m_PortsContainer ??= new VisualElement();

        protected SerializedProperty propertyFlowGraphId { get; set; }
        protected SerializedProperty propertyNodeId { get; set; }
        protected SerializedProperty propertyNodeType { get; set; }
        protected SerializedProperty propertyNodeName { get; set; }
        protected SerializedProperty propertyDescription { get; set; }
        protected SerializedProperty propertyInputPorts { get; set; }
        protected SerializedProperty propertyOutputPorts { get; set; }
        protected SerializedProperty propertyNodeState { get; set; }
        protected SerializedProperty propertyPosition { get; set; }
        protected SerializedProperty propertyPassthrough { get; set; }
        protected SerializedProperty propertyClearGraphHistory { get; set; }

        protected FlowNode flowNode => (FlowNode)target;
        public FlowNodeView nodeView { get; set; }

        protected bool canBeDeleted => serializedObject.targetObject != null && ((FlowNode)serializedObject.targetObject).canBeDeleted;

        protected virtual void OnPortConnected()
        {
            RefreshNodeEditor();
        }

        protected virtual void OnPortDisconnected()
        {
            RefreshNodeEditor();
        }

        protected virtual void OnNodeRefresh()
        {
            RefreshNodeEditor();
        }

        public virtual void RefreshNodeEditor()
        {
            portsContainer?.RecycleAndClear();
        }

        public override VisualElement CreateInspectorGUI()
        {
            InitializeEditor();
            Compose();
            root.Bind(serializedObject);

            flowNode.ports.ForEach(p =>
            {
                p.onConnected += other => OnPortConnected();
                p.onDisconnected += other => OnPortDisconnected();
            });

            flowNode.refreshNodeEditor -= OnNodeRefresh;
            flowNode.refreshNodeEditor += OnNodeRefresh;

            return root;
        }

        protected virtual void OnDestroy()
        {

            if (target != null && flowNode != null)
            {
                flowNode.refreshNodeEditor -= OnNodeRefresh;
                flowNode.ports.ForEach(p =>
                {
                    p.onConnected -= other => OnPortConnected();
                    p.onDisconnected -= other => OnPortDisconnected();
                });

                EditorUtility.SetDirty((FlowNode)target);
            }

            componentHeader?.Recycle();
            nodeNameField?.Recycle();
            copyFlowGraphIdButton?.Recycle();
            copyNodeIdButton?.Recycle();
            copyNodeNameButton?.Recycle();
            flowOptionsField?.Recycle();
            passthroughSwitch?.Recycle();
            clearGraphHistorySwitch?.Recycle();

            try
            {
                serializedObject.ApplyModifiedProperties();
            }
            catch
            {
                // ignored
            }
        }

        protected virtual void FindProperties()
        {
            propertyFlowGraphId = serializedObject.FindProperty("FlowGraphId");

            propertyNodeId = serializedObject.FindProperty("NodeId");
            propertyNodeType = serializedObject.FindProperty("NodeType");

            propertyNodeName = serializedObject.FindProperty("NodeName");
            propertyDescription = serializedObject.FindProperty("NodeDescription");

            propertyInputPorts = serializedObject.FindProperty("InputPorts");
            propertyOutputPorts = serializedObject.FindProperty("OutputPorts");

            propertyNodeState = serializedObject.FindProperty("NodeState");
            propertyPosition = serializedObject.FindProperty("Position");

            propertyPassthrough = serializedObject.FindProperty("Passthrough");
            propertyClearGraphHistory = serializedObject.FindProperty("ClearGraphHistory");
        }

        protected virtual void InitializeEditor()
        {
            FindProperties();

            root = new VisualElement();

            componentHeader =
                FluidComponentHeader.Get()
                    .SetElementSize(ElementSize.Large)
                    .SetAccentColor(nodeAccentColor)
                    .SetIcon(nodeIconTextures.ToList());

            if (!canBeDeleted)
            {
                deleteLockIcon =
                    new Image()
                        .SetName(nameof(deleteLockIcon))
                        .ResetLayout()
                        .SetStyleSize(24)
                        .SetStyleAlignSelf(Align.Center)
                        .SetStyleBackgroundImageTintColor(DesignUtils.placeholderColor)
                        .SetTooltip("This node cannot be deleted");

                deleteLockIconReaction =
                    deleteLockIcon
                        .GetTexture2DReaction()
                        .SetEditorHeartbeat()
                        .SetTextures(EditorMicroAnimations.EditorUI.Icons.Locked);

                deleteLockIcon.RegisterCallback<PointerEnterEvent>(evy => deleteLockIconReaction?.Play());
            }

            passthroughSwitch =
                FluidToggleSwitch.Get()
                    .SetLabelText("Passthrough")
                    .SetTooltip("Allow the graph to bypass this node when going back")
                    .SetToggleAccentColor(nodeSelectableAccentColor)
                    .BindToProperty(propertyPassthrough)
                    .SetStyleDisplay(flowNode.showPassthroughInEditor ? DisplayStyle.Flex : DisplayStyle.None);

            clearGraphHistorySwitch =
                FluidToggleSwitch.Get()
                    .SetLabelText("Clear Graph History")
                    .SetTooltip("OnEnter, clear graph history and remove the possibility of being able to go back to previously active nodes")
                    .SetToggleAccentColor(nodeSelectableAccentColor)
                    .BindToProperty(propertyClearGraphHistory)
                    .SetStyleDisplay(flowNode.showClearGraphHistoryInEditor ? DisplayStyle.Flex : DisplayStyle.None);

            flowOptionsField =
                FluidField.Get()
                    .SetLabelText("Flow Options")
                    .AddFieldContent
                    (
                        DesignUtils.column
                            .AddChild(passthroughSwitch)
                            .AddChild(DesignUtils.spaceBlock)
                            .AddChild(clearGraphHistorySwitch)
                    )
                    .SetStyleMarginBottom(DesignUtils.k_Spacing2X)
                    .SetStyleDisplay(flowNode.showPassthroughInEditor || flowNode.showClearGraphHistoryInEditor ? DisplayStyle.Flex : DisplayStyle.None);

            FluidButton GetIdButton()
            {
                FluidButton button =
                    FluidButton.Get()
                        .SetElementSize(ElementSize.Small)
                        .SetAccentColor(EditorSelectableColors.Default.ButtonIcon)
                        .SetIcon(EditorMicroAnimations.EditorUI.Icons.Copy);
                button.SetStyleTextAlign(TextAnchor.MiddleLeft);
                return button;
            }

            copyFlowGraphIdButton =
                GetIdButton()
                    .SetLabelText($"Graph Id: {propertyFlowGraphId.stringValue}")
                    .SetTooltip("Copy Graph Id to Clipboard")
                    .SetOnClick(() =>
                    {
                        GUIUtility.systemCopyBuffer = propertyFlowGraphId.stringValue;
                        Debug.Log($"Graph Id: '{propertyFlowGraphId.stringValue}' added to clipboard");
                    });

            copyNodeIdButton =
                GetIdButton()
                    .SetLabelText($"Node Id: {propertyNodeId.stringValue}")
                    .SetTooltip("Copy Node Id to Clipboard")
                    .SetOnClick(() =>
                    {
                        GUIUtility.systemCopyBuffer = propertyNodeId.stringValue;
                        Debug.Log($"Node Id: '{propertyNodeId.stringValue}' added to clipboard");
                    });

            nodeNameTextField =
                DesignUtils.NewTextField(propertyNodeName)
                    .SetStyleFlexGrow(1);

            nodeNameTextField.RegisterValueChangedCallback(evt =>
            {
                if (nodeView == null) return;
                nodeView.nodeTitleLabel.text = evt.newValue;
            });

            copyNodeNameButton =
                GetIdButton()
                    .SetTooltip("Copy Node Name to Clipboard")
                    .SetOnClick(() =>
                    {
                        GUIUtility.systemCopyBuffer = propertyNodeName.stringValue;
                        Debug.Log($"Node Name: '{propertyNodeName.stringValue}' added to clipboard");
                    });

            nodeNameField =
                FluidField.Get()
                    .SetLabelText("Node Name")
                    .AddFieldContent(nodeNameTextField)
                    .AddInfoElement
                    (
                        DesignUtils.column
                            .AddChild(DesignUtils.flexibleSpace)
                            .AddChild
                            (
                                copyNodeNameButton
                                    .SetStyleTop(2)
                            )
                    );

            nodeDescriptionTextField =
                DesignUtils.NewTextField(propertyDescription)
                    .SetMultiline(true)
                    .SetStyleFlexGrow(1);

            nodeDescriptionTextField.RegisterValueChangedCallback(evt =>
            {
                if (nodeView == null) return;
                nodeView.nodeDescriptionLabel.text = evt.newValue;
                nodeView.nodeDescriptionLabel.SetStyleDisplay(evt.newValue.IsNullOrEmpty() ? DisplayStyle.None : DisplayStyle.Flex);
            });

            nodeDescriptionField =
                FluidField.Get()
                    .SetLabelText("Node Description")
                    .AddFieldContent(nodeDescriptionTextField);

            //if visible in the Unity Inspector -> disable it to avoid letting developers messing up things outside Nody
            root.schedule.Execute(() =>
            {
                if (nodeView != null)
                    return;
                root.DisableElement();
            });

        }

        protected virtual void Compose()
        {
            root
                .AddChild(componentHeader)
                .AddChild
                (
                    DesignUtils.row
                        .SetStyleMargins(50, -4, DesignUtils.k_Spacing2X, DesignUtils.k_Spacing2X)
                        .AddChild(DesignUtils.flexibleSpace)
                        .AddChild(deleteLockIcon)
                )
                .AddChild(flowOptionsField)
                .AddChild(nodeNameField)
                .AddChild(DesignUtils.spaceBlock)
                .AddChild(nodeDescriptionField)
                .AddChild(DesignUtils.spaceBlock2X)
                .AddChild(portsContainer);
        }
    }
}
