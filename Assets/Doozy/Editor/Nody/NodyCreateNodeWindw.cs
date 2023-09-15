// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Editor.Common.Utils;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.Components.Internal;
using Doozy.Editor.EditorUI.ScriptableObjects.Colors;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Editor.EditorUI.Windows.Internal;
using Doozy.Editor.Nody.Automation.Generators;
using Doozy.Editor.Reactor.Internal;
using Doozy.Runtime;
using Doozy.Runtime.Common.Extensions;
using Doozy.Runtime.Nody;
using Doozy.Runtime.Reactor.Extensions;
using Doozy.Runtime.Reactor.Reactions;
using Doozy.Runtime.UIElements.Extensions;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Doozy.Editor.Nody
{
    public class NodyCreateNodeWindow : FluidWindow<NodyCreateNodeWindow>
    {
        private const string WINDOW_TITLE = "Create Node";
        public const string k_WindowMenuPath = "Tools/Doozy/Nody/";

        [MenuItem(k_WindowMenuPath + WINDOW_TITLE, priority = -800)]
        public static void Open() => InternalOpenWindow(WINDOW_TITLE);

        public static Color accentColor => EditorColors.Nody.Color;
        public static EditorSelectableColorInfo selectableAccentColor => EditorSelectableColors.Nody.Color;

        public Image nodyIcon { get; private set; }
        public Texture2DReaction nodyIconReaction { get; private set; }

        public FluidToggleButtonTab simpleNodeTabSelector { get; private set; }
        public FluidToggleButtonTab globalNodeTabSelector { get; private set; }

        public FluidToggleButtonTab runUpdateTabSelector { get; private set; }
        public FluidToggleButtonTab runFixedUpdateTabSelector { get; private set; }
        public FluidToggleButtonTab runLateUpdateTabSelector { get; private set; }

        public FluidToggleSwitch canBeDeletedSwitch { get; private set; }

        public FluidField nodeNameField { get; private set; }
        public TextField nodeNameTextField { get; private set; }

        public FluidField runtimePathField { get; private set; }
        public TextField runtimePathTextField { get; private set; }
        public FluidButton runtimePathButton { get; private set; }

        public FluidField editorPathField { get; private set; }
        public TextField editorPathTextField { get; private set; }
        public FluidButton editorPathButton { get; private set; }

        public VisualElement content { get; private set; }

        public FluidButton createNodeButton { get; private set; }

        public Label magicLabel { get; private set; }

        public NodeType nodeType { get; private set; }
        public bool runUpdate { get; private set; } = false;
        public bool runFixedUpdate { get; private set; } = false;
        public bool runLateUpdate { get; private set; } = false;
        public bool canBeDeleted { get; private set; } = true;

        protected override void OnDestroy()
        {
            base.OnDestroy();

            simpleNodeTabSelector?.Recycle();
            globalNodeTabSelector?.Recycle();

            runUpdateTabSelector?.Recycle();
            runFixedUpdateTabSelector?.Recycle();
            runLateUpdateTabSelector?.Recycle();

            canBeDeletedSwitch?.Recycle();

            nodeNameField?.Recycle();

            runtimePathField?.Recycle();
            runtimePathButton?.Recycle();

            editorPathField?.Recycle();
            editorPathButton?.Recycle();

            createNodeButton?.Recycle();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            minSize = new Vector2(480, 310);
            position = new Rect(position.position, minSize);
        }

        protected override void CreateGUI()
        {
            Initialize();
            Compose();
        }

        private void Initialize()
        {
            root
                .SetStyleBackgroundColor(EditorColors.Default.BoxBackground);

            magicLabel =
                DesignUtils.NewFieldNameLabel("I put on my robe and wizard hat")
                    .SetStyleAlignSelf(Align.Center)
                    .SetStyleTextAlign(TextAnchor.MiddleCenter)
                    .SetStyleOpacity(0.5f);

            nodyIcon =
                new Image()
                    .ResetLayout()
                    .SetStyleFlexShrink(0)
                    .SetStyleBackgroundImageTintColor(EditorColors.Default.WindowHeaderTitle)
                    .SetStyleMarginLeft(DesignUtils.k_Spacing3X)
                    .SetStyleSize(64);

            nodyIconReaction =
                nodyIcon
                    .GetTexture2DReaction(EditorMicroAnimations.Nody.Icons.Nody)
                    .SetEditorHeartbeat();

            nodyIconReaction.Play();
            nodyIcon.RegisterCallback<PointerEnterEvent>(evt => nodyIconReaction?.Play());
            nodyIcon.AddManipulator(new Clickable(() => nodyIconReaction?.Play()));


            simpleNodeTabSelector =
                GetSelectorTab(NodeType.Simple.ToString())
                    .SetTooltip("Simple node is a normal node (most nodes are simple nodes)")
                    .SetTabPosition(TabPosition.TabOnLeft)
                    .SetOnClick(() => SelectNodeType(NodeType.Simple));

            globalNodeTabSelector =
                GetSelectorTab(NodeType.Global.ToString())
                    .SetTooltip("Global node is a node that that is always running")
                    .SetTabPosition(TabPosition.TabOnRight)
                    .SetOnClick(() => SelectNodeType(NodeType.Global));

            runUpdateTabSelector =
                GetSelectorTab("Update")
                    .SetTooltip("Run Update when the node is active")
                    .SetTabPosition(TabPosition.TabOnLeft)
                    .SetOnValueChanged(evt => runUpdate = evt.newValue);

            runFixedUpdateTabSelector =
                GetSelectorTab("FixedUpdate")
                    .SetTooltip("Run FixedUpdate when the node is active")
                    .SetTabPosition(TabPosition.TabInCenter)
                    .SetOnValueChanged(evt => runFixedUpdate = evt.newValue);

            runLateUpdateTabSelector =
                GetSelectorTab("LateUpdate")
                    .SetTooltip("Run LateUpdate when the node is active")
                    .SetTabPosition(TabPosition.TabOnRight)
                    .SetOnValueChanged(evt => runLateUpdate = evt.newValue);

            content =
                new VisualElement()
                    .SetStylePadding(DesignUtils.k_Spacing2X)
                    .SetStyleMargins(DesignUtils.k_Spacing2X, 0, DesignUtils.k_Spacing2X, DesignUtils.k_Spacing2X)
                    .SetStyleBackgroundColor(EditorColors.Default.FieldBackground)
                    .SetStyleBorderRadius(4);

            nodeNameTextField =
                new TextField()
                    .SetName(nameof(nodeNameTextField))
                    .ResetLayout()
                    .SetStyleFlexShrink(0)
                    .SetStyleFlexGrow(1);

            canBeDeletedSwitch =
                FluidToggleSwitch.Get("Node can be deleted")
                    .SetTooltip("[Editor] Used to prevent special nodes from being deleted in the editor")
                    .SetToggleAccentColor(selectableAccentColor)
                    .SetIsOn(canBeDeleted)
                    .SetOnValueChanged(evt => canBeDeleted = evt.newValue);

            nodeNameField =
                FluidField.Get()
                    .SetLabelText("Node Name")
                    .SetTooltip("Enter a name for the new node")
                    .AddFieldContent(nodeNameTextField)
                    .AddInfoElement
                    (
                        DesignUtils.column
                            .AddChild(DesignUtils.spaceBlock3X)
                            .AddChild(canBeDeletedSwitch)
                    );

            runtimePathTextField =
                new TextField()
                    .SetName(nameof(runtimePathTextField))
                    .ResetLayout()
                    .SetStyleFlexShrink(0)
                    .SetStyleFlexGrow(1);

            runtimePathButton =
                FluidButton.Get()
                    .SetElementSize(ElementSize.Small)
                    .SetIcon(EditorMicroAnimations.EditorUI.Icons.Load)
                    .SetOnClick(() =>
                    {
                        string rawPath = EditorUtility.OpenFolderPanel("Runtime Path", runtimePathTextField.value, "");
                        runtimePathTextField.value = FileGenerator.CleanPath(rawPath);
                    });

            runtimePathField =
                FluidField.Get()
                    .SetLabelText("Runtime Path (where the node is saved)")
                    .SetTooltip("Set the runtime path where to save the {NodeName}Node.cs file")
                    .AddFieldContent(runtimePathTextField)
                    .AddInfoElement
                    (
                        DesignUtils.column
                            .AddChild(DesignUtils.spaceBlock3X)
                            .AddChild(runtimePathButton)
                    );

            editorPathTextField =
                new TextField()
                    .SetName(nameof(editorPathTextField))
                    .ResetLayout()
                    .SetStyleFlexShrink(0)
                    .SetStyleFlexGrow(1);

            editorPathButton =
                FluidButton.Get()
                    .SetElementSize(ElementSize.Small)
                    .SetIcon(EditorMicroAnimations.EditorUI.Icons.Load)
                    .SetOnClick(() =>
                    {
                        string rawPath = EditorUtility.OpenFolderPanel("Editor Path", editorPathTextField.value, "");
                        editorPathTextField.value = FileGenerator.CleanPath(rawPath);
                    });

            editorPathField =
                FluidField.Get()
                    .SetLabelText("Editor Path (where the node view and node editor are saved)")
                    .SetTooltip("Set the editor path where to save the {NodeName}NodeView.cs and {NodeName}NodeEditor.cs files")
                    .AddFieldContent(editorPathTextField)
                    .AddInfoElement
                    (
                        DesignUtils.column
                            .AddChild(DesignUtils.spaceBlock3X)
                            .AddChild(editorPathButton)
                    );

            createNodeButton =
                FluidButton.Get()
                    .SetLabelText("Create")
                    .SetIcon(EditorMicroAnimations.Nody.Icons.Nody)
                    .SetElementSize(ElementSize.Large)
                    .SetButtonStyle(ButtonStyle.Contained)
                    .SetAccentColor(selectableAccentColor)
                    .SetOnClick(() =>
                    {
                        NewNodeGenerator.CreateNode
                        (
                            nodeNameTextField.value, nodeType,
                            canBeDeleted, runUpdate, runFixedUpdate, runLateUpdate,
                            runtimePathTextField.value, editorPathTextField.value
                        );
                        Close();
                    });

            runtimePathTextField.value = $"{RuntimePath.path}/Nody/Nodes/";
            editorPathTextField.value = $"{EditorPath.path}/Nody/Nodes/";

            SelectNodeType(NodeType.Simple);

            root.schedule.Execute(() =>
            {
                bool setEnabled = !(nodeNameTextField.value.IsNullOrEmpty() ||
                    runtimePathTextField.value.IsNullOrEmpty() ||
                    editorPathTextField.value.IsNullOrEmpty());

                createNodeButton.SetEnabled(setEnabled);
                magicLabel.visible = setEnabled;

            }).Every(100);
        }

        private void Compose()
        {
            root
                .AddChild(DesignUtils.spaceBlock2X)
                // .AddChild
                // (
                //     DesignUtils.row
                //         .SetStyleFlexGrow(0)
                //         .SetStyleAlignItems(Align.Center)
                //         .AddChild(DesignUtils.flexibleSpace)
                //         .AddChild(nodyIcon)
                //         .AddChild(DesignUtils.flexibleSpace)
                //     // .AddChild(searchBox.SetStyleFlexShrink(0))
                // )
                .AddChild(DesignUtils.spaceBlock2X)
                .AddChild
                (
                    DesignUtils.row
                        .SetStyleFlexShrink(0)
                        .SetStyleFlexGrow(0)
                        .SetStyleAlignItems(Align.Center)
                        .SetStyleBackgroundColor(EditorColors.Default.BoxBackground)
                        .SetStylePaddingLeft(DesignUtils.k_Spacing3X)
                        .SetStylePaddingRight(DesignUtils.k_Spacing3X)
                        .AddChild(simpleNodeTabSelector)
                        .AddSpace(2, 0)
                        .AddChild(globalNodeTabSelector)
                        .AddChild(DesignUtils.spaceBlock2X)
                        .AddChild(DesignUtils.flexibleSpace)
                        .AddChild(DesignUtils.spaceBlock2X)
                        .AddChild(runUpdateTabSelector)
                        .AddSpace(2, 0)
                        .AddChild(runFixedUpdateTabSelector)
                        .AddSpace(2, 0)
                        .AddChild(runLateUpdateTabSelector)
                )
                .AddChild(DesignUtils.spaceBlock2X)
                .AddChild
                (
                    content
                        .AddChild(nodeNameField)
                        .AddChild(DesignUtils.spaceBlock2X)
                        .AddChild(runtimePathField)
                        .AddChild(DesignUtils.spaceBlock2X)
                        .AddChild(editorPathField)
                        .AddChild(DesignUtils.spaceBlock2X)
                        .AddChild(DesignUtils.flexibleSpace)
                )
                .AddChild(DesignUtils.spaceBlock4X)
                .AddChild
                (
                    DesignUtils.row
                        .AddChild(DesignUtils.flexibleSpace)
                        .AddChild(createNodeButton)
                        .AddChild(DesignUtils.flexibleSpace)
                )
                .AddChild(magicLabel)
                .AddChild(DesignUtils.flexibleSpace)
                ;
        }

        
    

        internal void SelectNodeType(NodeType nodeType)
        {
            root.schedule.Execute(() =>
            {
                this.nodeType = nodeType;
                simpleNodeTabSelector?.SetIsOn(nodeType == NodeType.Simple);
                globalNodeTabSelector?.SetIsOn(nodeType == NodeType.Global);
            });
        }


        private FluidToggleButtonTab GetSelectorTab(string labelText) =>
            FluidToggleButtonTab.Get()
                .SetLabelText(labelText)
                .SetContainerColorOff(DesignUtils.tabButtonColorOff)
                .SetToggleAccentColor(selectableAccentColor);
    }
}
