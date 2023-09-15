// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using System.Linq;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.ScriptableObjects.Colors;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Editor.Reactor.Internal;
using Doozy.Runtime.Common.Extensions;
using Doozy.Runtime.Nody;
using Doozy.Runtime.Reactor;
using Doozy.Runtime.Reactor.Easings;
using Doozy.Runtime.Reactor.Extensions;
using Doozy.Runtime.Reactor.Internal;
using Doozy.Runtime.Reactor.Reactions;
using Doozy.Runtime.UIElements.Extensions;
using UnityEditor;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using EditorStyles = Doozy.Editor.EditorUI.EditorStyles;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable CollectionNeverQueried.Global
// ReSharper disable MemberCanBeProtected.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable VirtualMemberCallInConstructor
// ReSharper disable VirtualMemberNeverOverridden.Global
// ReSharper disable UnusedMember.Global

namespace Doozy.Editor.Nody
{
    public partial class FlowNodeView : Node, IDisposable
    {
        public virtual void Dispose()
        {
            addOutputButton?.Recycle();
            activeStateLineIndicatorReaction?.Recycle();
            deleteLockIconReaction?.Recycle();
            nodeIconReaction?.Recycle();
            runningStateIconIndicatorReaction?.Recycle();
            activeStateBorderReaction?.Recycle();
            pingBorderReaction?.Recycle();

            flowNode.ping -= Ping;
            flowNode.refreshNodeView -= RefreshNodeView;
            flowNode.refreshNodeView -= RefreshPortsViews;
        }

        public virtual Type nodeType => null;
        public virtual Texture2D nodeIconTexture => EditorTextures.Nody.Icons.Infinity;
        public virtual IEnumerable<Texture2D> nodeIconTextures => EditorMicroAnimations.Nody.Icons.Nody;
        public virtual Color nodeAccentColor => EditorColors.Nody.Color;
        public virtual EditorSelectableColorInfo nodeSelectableAccentColor => EditorSelectableColors.Nody.Color;

        public FlowGraphView graphView { get; }
        public FlowNode flowNode { get; }

        public Action<FlowNodeView> OnNodeSelected;

        public List<FlowPortView> inputPortViews { get; set; }
        public List<FlowPortView> outputPortViews { get; set; }

        protected FluidButton addOutputButton { get; set; }

        private static string s_uxmlPath;
        private static string uxmlPath => !s_uxmlPath.IsNullOrEmpty() ? s_uxmlPath : (s_uxmlPath = AssetDatabase.GetAssetPath(EditorLayouts.Nody.NodeView));

        public ColorReaction activeStateBorderReaction { get; internal set; }
        public ColorReaction pingBorderReaction { get; internal set; }
        public Image activeStateLineIndicator { get; private set; }
        public Image deleteLockIcon { get; private set; }
        public Image nodeIcon { get; private set; }
        public Image runningStateIconIndicator { get; private set; }
        public Label nodeDescriptionLabel { get; private set; }
        public Label nodeTitleLabel { get; private set; }
        public Texture2DReaction activeStateLineIndicatorReaction { get; internal set; }
        public Texture2DReaction deleteLockIconReaction { get; private set; }
        public Texture2DReaction nodeIconReaction { get; internal set; }
        public Texture2DReaction runningStateIconIndicatorReaction { get; internal set; }
        public VisualElement activeStateBorder { get; private set; }
        public VisualElement nodeBorder { get; private set; }
        public VisualElement nodeContent { get; private set; }
        public VisualElement nodeInfo { get; private set; }
        public VisualElement nodeSelectionBorder { get; private set; }
        public VisualElement pingBorder { get; private set; }
        public VisualElement portDivider { get; private set; }
        public VisualElement top { get; private set; }

        public Color stateColor
        {
            get
            {
                switch (flowNode.nodeState)
                {
                    case NodeState.Idle:
                        return EditorColors.Nody.StateIdle;
                    case NodeState.Running:
                        return EditorColors.Nody.StateRunning;
                    case NodeState.Active:
                        return EditorColors.Nody.StateActive;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public SerializedObject serializedObject { get; }
        public SerializedProperty propertyFlowGraphId { get; private set; }
        public SerializedProperty propertyNodeId { get; private set; }
        public SerializedProperty propertyNodeType { get; private set; }
        public SerializedProperty propertyNodeName { get; private set; }
        public SerializedProperty propertyDescription { get; private set; }
        public SerializedProperty propertyInputPorts { get; private set; }
        public SerializedProperty propertyOutputPorts { get; private set; }
        public SerializedProperty propertyNodeState { get; private set; }
        public SerializedProperty propertyPosition { get; private set; }
        public SerializedProperty propertyPassthrough { get; private set; }
        public SerializedProperty propertyClearGraphHistory { get; private set; }

        protected FlowNodeView(FlowGraphView graphView, FlowNode node) : base(uxmlPath)
        {
            this.flowNode = node;
            this.graphView = graphView;

            serializedObject = new SerializedObject(node);
            FindProperties();

            node.ping += Ping;
            node.refreshNodeView += RefreshNodeView;
            node.refreshNodeView += RefreshPortsViews;

            RegisterCallback<PointerEnterEvent>(evt => nodeIconReaction?.Play());
            RegisterCallback<PointerDownEvent>(evt => nodeIconReaction?.Play());

            inputPortViews ??= new List<FlowPortView>();
            outputPortViews ??= new List<FlowPortView>();

            InitializeView();
            RefreshNodeView();
            RefreshPortsViews();
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

        #region Ping

        public void Ping(FlowDirection direction)
        {
            Color directionColor;
            switch (direction)
            {
                case FlowDirection.Forward:
                    directionColor = EditorColors.Nody.StateActive;
                    break;
                case FlowDirection.Back:
                    directionColor = EditorColors.Nody.BackFlow;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }

            pingBorderReaction.Stop();
            pingBorderReaction.SetFrom(directionColor);
            pingBorderReaction.SetTo(Color.clear);
            if (direction == FlowDirection.Back)
                pingBorderReaction.Play();


        }

        public void Ping(Color color)
        {
            pingBorderReaction.Stop();
            pingBorderReaction.SetFrom(color);
            pingBorderReaction.SetTo(Color.clear);
            pingBorderReaction.Play();
        }

        #endregion

        /// <summary> Refresh the node (call when node's data changes) </summary>
        public virtual void RefreshNodeView()
        {
            viewDataKey = flowNode.nodeId;
            title = flowNode.name;

            nodeTitleLabel.SetText(flowNode.nodeName);
            nodeDescriptionLabel.SetText(flowNode.nodeDescription);
            nodeDescriptionLabel.SetStyleDisplay(flowNode.nodeDescription.IsNullOrEmpty() ? DisplayStyle.None : DisplayStyle.Flex);

            style.left = flowNode.position.x;
            style.top = flowNode.position.y;

            // UpdateNodeState(node.nodeState);
        }

        public virtual void RefreshPortsViews()
        {
            inputPortViews.ForEach(p => p?.Dispose());
            inputPortViews.Clear();
            flowNode.inputPorts.ForEach(port =>
            {
                port.refreshPortView -= RefreshData;
                port.refreshPortView += RefreshData;
                AddPortView(port);
            });

            outputPortViews.ForEach(p => p?.Dispose());
            outputPortViews.Clear();
            flowNode.outputPorts.ForEach(port =>
            {
                port.refreshPortView -= RefreshData;
                port.refreshPortView += RefreshData;
                AddPortView(port);
            });

            graphView.RefreshEdges();

            RefreshPorts();
        }

        public virtual void RefreshData()
        {

        }


        /// <summary> Called every time the node state changes its state </summary>
        /// <param name="newState"> New node state </param>
        protected virtual void OnNodeStateChanged(NodeState newState) =>
            UpdateNodeState(newState);

        /// <summary> Update the visual state of the node according to the new node state </summary>
        /// <param name="newState"> New node state </param>
        public void UpdateNodeState(NodeState newState = NodeState.Idle)
        {
            switch (newState)
            {
                case NodeState.Idle:
                    runningStateIconIndicatorReaction.SetFirstFrame();
                    
                    activeStateBorderReaction.SetProgressAtOne();
                    activeStateBorderReaction.Play(PlayDirection.Reverse);
                    activeStateLineIndicatorReaction.SetFirstFrame();
                    break;
                case NodeState.Running:
                    runningStateIconIndicatorReaction.Play();

                    if (flowNode.nodeType == NodeType.Global)
                    {
                        activeStateBorderReaction.SetProgressAtOne();
                        activeStateBorderReaction.Play(PlayDirection.Reverse);
                        activeStateLineIndicatorReaction.SetFirstFrame();
                    }
                    
                    break;
                case NodeState.Active:
                    activeStateBorderReaction.Play(PlayDirection.Forward);
                    activeStateLineIndicatorReaction.Play();
                    nodeIconReaction?.Play();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
            }
        }

        protected virtual void InitializeView()
        {
            this
                .AddStyle(EditorStyles.Nody.NodeView)
                .SetStyleMargins(0)
                .SetStylePadding(0);

            nodeBorder = this.Q<VisualElement>("node-border");
            nodeSelectionBorder = this.Q<VisualElement>("selection-border");
            nodeIcon = this.Q<Image>(nameof(nodeIcon));
            nodeTitleLabel = this.Q<Label>("title-label");
            nodeInfo = this.Q<VisualElement>(nameof(nodeInfo));
            nodeDescriptionLabel = this.Q<Label>(nameof(nodeDescriptionLabel));
            nodeContent = this.Q<VisualElement>(nameof(nodeContent));
            top = this.Q<VisualElement>("top");
            runningStateIconIndicator = this.Q<Image>(nameof(runningStateIconIndicator)).SetPickingMode(PickingMode.Ignore);
            activeStateLineIndicator = this.Q<Image>(nameof(activeStateLineIndicator)).SetPickingMode(PickingMode.Ignore);
            activeStateBorder = this.Q<VisualElement>(nameof(activeStateBorder)).SetPickingMode(PickingMode.Ignore);
            pingBorder = this.Q<VisualElement>(nameof(pingBorder)).SetPickingMode(PickingMode.Ignore);
            portDivider = this.Q<VisualElement>(nameof(portDivider));

            // nodeIcon.transform.rotation = Quaternion.Euler(0,0,-20);
            titleContainer
                .SetStyleBorderColor(EditorColors.Nody.LineColor)
                .SetStyleBackgroundColor(EditorColors.Nody.LineColor);

            nodeIcon
                .SetStyleBackgroundImageTintColor(nodeAccentColor);

            nodeTitleLabel
                .SetStyleFlexGrow(1)
                .SetStyleColor(EditorColors.Nody.NodeTitle)
                .SetStyleUnityFont(EditorFonts.Ubuntu.Light);

            nodeDescriptionLabel
                .SetStyleBackgroundColor(EditorColors.Nody.LineColor)
                .SetStyleBorderColor(EditorColors.Nody.LineColor)
                .SetStyleColor(EditorColors.Nody.NodeTitle)
                .SetStyleUnityFont(EditorFonts.Inter.Light);

            nodeBorder
                .SetStyleBackgroundColor(EditorColors.Nody.NodeBackground)
                .SetStyleBorderColor(EditorColors.Nody.LineColor)
                .SetStyleOverflow(Overflow.Visible);

            nodeSelectionBorder
                .SetStyleBorderColor(EditorColors.Nody.Selection)
                .SetPickingMode(PickingMode.Ignore);

            inputContainer
                .SetStyleBorderColor(EditorColors.Nody.LineColor)
                .SetStyleBackgroundColor(EditorColors.Nody.LineColor);

            outputContainer
                .SetStyleBorderColor(EditorColors.Nody.LineColor)
                .SetStyleBackgroundColor(EditorColors.Nody.LineColor);

            if (!flowNode.canBeDeleted)
            {
                deleteLockIcon =
                    new Image()
                        .SetName(nameof(deleteLockIcon))
                        .ResetLayout()
                        .SetStyleSize(12)
                        .SetStyleAlignSelf(Align.Center)
                        .SetStyleBackgroundImageTintColor(DesignUtils.placeholderColor)
                        .SetTooltip("This node cannot be deleted");

                deleteLockIconReaction =
                    deleteLockIcon
                        .GetTexture2DReaction()
                        .SetEditorHeartbeat()
                        .SetTextures(EditorMicroAnimations.EditorUI.Icons.Locked);

                deleteLockIcon.RegisterCallback<PointerEnterEvent>(evy => deleteLockIconReaction?.Play());

                nodeInfo.AddChild(deleteLockIcon);
            }

            AddStateIndicators();

            this.SetIcon(nodeIconTextures);
        }

        private void AddStateIndicators()
        {
            #region Running State Indicator

            runningStateIconIndicator
                .SetStyleDisplay(DisplayStyle.Flex)
                .SetStyleBackgroundImageTintColor(nodeAccentColor);

            runningStateIconIndicatorReaction =
                runningStateIconIndicator.GetTexture2DReaction().SetEditorHeartbeat()
                    .SetLoops(-1).SetDuration(1f)
                    .SetTextures(EditorMicroAnimations.Nody.Effects.NodeStateRunning);

            nodeIcon.AddChild(runningStateIconIndicator);

            #endregion

            #region Active State Indicator

            activeStateLineIndicator
                .SetStyleDisplay(DisplayStyle.Flex)
                .SetStyleBackgroundImageTintColor(EditorColors.Nody.StateActive);

            activeStateLineIndicatorReaction =
                activeStateLineIndicator.GetTexture2DReaction().SetEditorHeartbeat()
                    .SetLoops(-1)
                    .SetDuration(1f)
                    .SetTextures(EditorMicroAnimations.Nody.Effects.NodeStateActive);

            nodeContent.Insert(0, activeStateLineIndicator);

            #endregion

            #region Active State Border

            activeStateBorderReaction =
                activeStateBorder
                    .GetColorReaction(value => activeStateBorder.SetStyleBorderColor(value)).SetEditorHeartbeat()
                    .SetDuration(0.5f);

            activeStateBorderReaction.SetFrom(Color.clear);
            activeStateBorderReaction.SetTo(EditorColors.Nody.StateActive);

            #endregion

            #region Ping Border

            pingBorderReaction =
                pingBorder
                    .GetColorReaction(value => pingBorder.SetStyleBorderColor(value))
                    .SetEditorHeartbeat()
                    .SetEase(Ease.InExpo)
                    .SetDuration(0.5f);

            #endregion


            flowNode.onStart += () => runningStateIconIndicatorReaction.Play();
            flowNode.onStop += () => runningStateIconIndicatorReaction.Stop();
            flowNode.onStateChanged += OnNodeStateChanged;
        }

        protected void InjectAddOutputButton()
        {
            addOutputButton?.Recycle();

            addOutputButton =
                DesignUtils.SystemButton(EditorMicroAnimations.EditorUI.Icons.Plus)
                    .SetName(nameof(addOutputButton))
                    .SetStyleAlignSelf(Align.FlexEnd)
                    .SetOnClick(() =>
                    {
                        Undo.RecordObject(flowNode, "Add Port");                    //record undo
                        flowNode.AddOutputPort();                                   //add a new output port
                        FlowPort port = flowNode.outputPorts.Last();                //get a reference to the newly created port (it's the last one in the list)
                        var portView = new FlowPortView(graphView, this, port); //create a port view for the new port
                        outputPortViews.Add(portView);                          //add port view to the port view list
                        int indexOf = outputContainer.IndexOf(addOutputButton); //get + button index
                        outputContainer.Insert(indexOf, portView);              //insert the port view before the + button (so that the button is always drawn last)
                        EditorUtility.SetDirty(flowNode);                           //mark the node as dirty
                        EditorUtility.SetDirty(graphView.flowGraph);            //mark the graph as dirty
                        RefreshPorts();                                         //refresh ports

                        AssetDatabase.SaveAssetIfDirty(flowNode);                //save assets
                        AssetDatabase.SaveAssetIfDirty(graphView.flowGraph); //save assets

                        flowNode.RefreshNodeEditor();
                        flowNode.RefreshNodeView();

                        port.RefreshPortEditor();
                        port.RefreshPortView();
                    });

            outputContainer.Add(addOutputButton);

            // RefreshExpandedState();
            RefreshPorts();
        }

        public override void OnSelected()
        {
            base.OnSelected();
            OnNodeSelected?.Invoke(this);
        }

        public override void SetPosition(Rect newPos)
        {
            base.SetPosition(newPos);
            Undo.RecordObject(flowNode, "Set Position");
            flowNode.SetPosition(newPos.min);
            EditorUtility.SetDirty(flowNode);
        }

        protected virtual FlowPortView AddPortView(FlowPort flowPort)
        {
            var portView = new FlowPortView(graphView, this, flowPort);

            switch (portView.direction)
            {
                case Direction.Input:
                    inputPortViews.Add(portView);
                    inputContainer
                        .SetStyleDisplay(DisplayStyle.Flex)
                        .AddChild(portView);
                    break;
                case Direction.Output:
                    outputPortViews.Add(portView);
                    outputContainer
                        .SetStyleDisplay(DisplayStyle.Flex)
                        .AddChild(portView);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            RefreshExpandedState();
            RefreshPorts();

            return portView;
        }
    }

    public static class FlowNodeViewExtensions
    {
        #region Icon

        /// <summary> Set Animated Icon </summary>
        /// <param name="target"> Target NodeView </param>
        /// <param name="textures"> Icon textures </param>
        public static T SetIcon<T>(this T target, IEnumerable<Texture2D> textures) where T : FlowNodeView
        {
            if (target.nodeIconReaction == null)
            {
                target.nodeIconReaction = target.nodeIcon.GetTexture2DReaction(textures).SetEditorHeartbeat().SetDuration(0.6f);
            }
            else
            {
                target.nodeIconReaction.SetTextures(textures);
            }
            target.nodeIcon.SetStyleDisplay(DisplayStyle.Flex);
            return target;
        }

        /// <summary> Set Static Icon </summary>
        /// <param name="target"> Target Button </param>
        /// <param name="iconTexture2D"> Icon texture </param>
        public static T SetIcon<T>(this T target, Texture2D iconTexture2D) where T : FlowNodeView
        {
            target.nodeIconReaction?.Recycle();
            target.nodeIconReaction = null;
            target.nodeIcon.SetStyleBackgroundImage(iconTexture2D);
            target.nodeIcon.SetStyleDisplay(DisplayStyle.Flex);
            return target;
        }

        /// <summary> Clear the icon. If the icon is animated, its reaction will get recycled </summary>
        /// <param name="target"> Target Button </param>
        public static T ClearIcon<T>(this T target) where T : FlowNodeView
        {
            target.nodeIconReaction?.Recycle();
            target.nodeIconReaction = null;
            target.nodeIcon.SetStyleBackgroundImage((Texture2D)null);
            target.nodeIcon.SetStyleDisplay(DisplayStyle.None);
            return target;
        }

        #endregion
    }
}
