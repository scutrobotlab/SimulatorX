// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.Components.Internal;
using Doozy.Editor.EditorUI.ScriptableObjects.Colors;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Editor.EditorUI.Windows.Internal;
using Doozy.Editor.Nody.Automation.Generators;
using Doozy.Runtime.Nody;
using Doozy.Runtime.UIElements.Extensions;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.UIElements;
using EditorStyles = Doozy.Editor.EditorUI.EditorStyles;
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Editor.Nody
{
    public class NodyWindow : FluidWindow<NodyWindow>
    {
        private const string WINDOW_TITLE = "Nody";
        public const string k_WindowMenuPath = "Tools/Doozy/Nody/";

        [MenuItem(k_WindowMenuPath + "Window", priority = -900)]
        public static void Open()
        {
            InternalOpenWindow(WINDOW_TITLE);
            _ = NodyInspectorWindow.instance;
        }

        [MenuItem(k_WindowMenuPath + "Refresh", priority = -800)]
        public static void Refresh()
        {
            FlowNodeViewExtensionGenerator.Run();
            NodyNodeSearchWindowGenerator.Run();
            UnityEditor.Compilation.CompilationPipeline.RequestScriptCompilation();
        }

        [OnOpenAsset]
        public static bool OnOpenAsset(int instanceId, int line)
        {
            if (!(Selection.activeObject is FlowGraph))
                return false;

            Open();
            return true;

        }

        public static NodyInspectorWindow inspector => NodyInspectorWindow.instance;

        public static Color accentColor => EditorColors.Nody.Color;
        public static EditorSelectableColorInfo selectableAccentColor => EditorSelectableColors.Nody.Color;

        private TemplateContainer templateContainer { get; set; }
        private VisualElement layoutContainer { get; set; }
        private VisualElement sideMenuContainer { get; set; }
        private VisualElement graphView { get; set; }

        private FlowGraphView flowGraphView { get; set; }

        private NodyMiniMap miniMap { get; set; }

        private FluidSideMenu sideMenu { get; set; }
        private FluidToggleButtonTab showMinimapButton { get; set; }
        private FluidToggleButtonTab loadGraphButton { get; set; }
        private FluidToggleButtonTab saveGraphButton { get; set; }
        private FluidToggleButtonTab openInspectorButton { get; set; }
        private FluidToggleButtonTab closeGraphButton { get; set; }

        private Label openedGraphPathLabel { get; set; }
        private FluidButton pingOpenGraphButton { get; set; }

        protected override void CreateGUI()
        {
            root.Add(templateContainer = EditorLayouts.Nody.NodyWindow.CloneTree());

            templateContainer
                .SetStyleFlexGrow(1)
                .AddStyle(EditorStyles.Nody.NodyWindow);

            layoutContainer = templateContainer.Q<VisualElement>(nameof(layoutContainer));
            sideMenuContainer = layoutContainer.Q<VisualElement>(nameof(sideMenuContainer));
            graphView = layoutContainer.Q<VisualElement>(nameof(graphView));

            CreateFlowGraphView();
            CreateMiniMap();
            CreateSideMenu();

            Compose();

            OnSelectionChange();
        }

        /// <summary> Loads a graph from the given path </summary>
        /// <param name="path"> Asset path where the graph can be found </param>
        public static T LoadGraph<T>(string path) where T : FlowGraph
        {
            if (string.IsNullOrEmpty(path)) return null;
            T graph = AssetDatabase.LoadAssetAtPath<T>(FileUtil.GetProjectRelativePath(path));
            return graph == null ? null : graph;
        }

        /// <summary> Opens a file panel set to filter only the given Graph type and returns the selected asset file </summary>
        public static T LoadGraphWithDialog<T>() where T : FlowGraph
        {
            string path = EditorUtility.OpenFilePanelWithFilters($"Load {ObjectNames.NicifyVariableName(typeof(T).Name)}", "", new[] { typeof(T).Name, "asset" });
            return LoadGraph<T>(path);
        }

        private void CreateSideMenu()
        {
            sideMenu =
                new FluidSideMenu()
                    .SetMenuLevel(FluidSideMenu.MenuLevel.Level_1)
                    .SetCustomWidth(160)
                    .IsCollapsable(true)
                    .CollapseMenu(false);

            #region Load Graph Button

            loadGraphButton =
                sideMenu.AddButton("Load", selectableAccentColor, false)
                    .SetIcon(EditorMicroAnimations.EditorUI.Icons.Load)
                    .SetOnClick(() =>
                    {
                        saveGraphButton.schedule.Execute(() => loadGraphButton.SetIsOn(false));
                        FlowGraph flowGraph = LoadGraphWithDialog<FlowGraph>();
                        if (flowGraph != null) OpenGraph(flowGraph);
                    });

            #endregion

            #region Save Graph Button

            saveGraphButton =
                sideMenu.AddButton("Save", selectableAccentColor, false)
                    .SetIcon(EditorMicroAnimations.EditorUI.Icons.Save)
                    .SetIsOn(false)
                    .SetOnClick(() =>
                    {
                        saveGraphButton.schedule.Execute(() => saveGraphButton.SetIsOn(false));

                        if (flowGraphView.flowGraph == null)
                            return;

                        EditorUtility.SetDirty(flowGraphView.flowGraph);
                        flowGraphView.flowGraph.nodes.ForEach(EditorUtility.SetDirty);
                        AssetDatabase.SaveAssets();
                    });

            #endregion

            #region Save Graph Button

            closeGraphButton =
                sideMenu.AddButton("Close", selectableAccentColor, false)
                    .SetIcon(EditorMicroAnimations.EditorUI.Icons.Close)
                    .SetIsOn(false)
                    .SetOnClick(() =>
                    {
                        closeGraphButton.schedule.Execute(() => closeGraphButton.SetIsOn(false));

                        if (flowGraphView.flowGraph == null)
                            return;

                        EditorUtility.SetDirty(flowGraphView.flowGraph);
                        flowGraphView.flowGraph.nodes.ForEach(EditorUtility.SetDirty);
                        AssetDatabase.SaveAssets();
                        CloseGraph();
                    });

            #endregion

            #region Open Nody Inspector

            openInspectorButton =
                sideMenu.AddButton("Inspector", selectableAccentColor, false)
                    .SetIcon(EditorMicroAnimations.Nody.Icons.Nody)
                    .SetIsOn(NodyInspectorWindow.isOpen)
                    .SetOnClick(() =>
                    {
                        if (NodyInspectorWindow.isOpen)
                        {
                            NodyInspectorWindow.instance.Focus();
                            return;
                        }
                        _ = NodyInspectorWindow.instance;

                        if (flowGraphView == null) return;
                        if (flowGraphView.selection.Count != 1) return;
                        if (flowGraphView.selection[0] is FlowNodeView nodeView)
                        {
                            NodyInspectorWindow.instance.UpdateSelection(nodeView);
                        }

                    })
                    .SetStyleMarginTop(DesignUtils.k_Spacing4X);

            openInspectorButton.schedule.Execute(() =>
            {
                openInspectorButton.SetIsOn(NodyInspectorWindow.isOpen);
            }).Every(200);

            #endregion

            #region Show MiniMap Button

            showMinimapButton =
                sideMenu
                    .AddButton("MiniMap", selectableAccentColor, false)
                    .SetIcon(EditorMicroAnimations.Nody.Icons.Minimap)
                    .SetIsOn(miniMap.isVisible)
                    .SetOnValueChanged(evt =>
                    {
                        miniMap.SetIsVisible(evt.newValue);
                        UpdateShowMinimapButton();
                    })
                    .SetStyleMarginTop(DesignUtils.k_Spacing4X);

            UpdateShowMinimapButton();

            void UpdateShowMinimapButton()
            {
                showMinimapButton.buttonLabel.SetText
                (
                    miniMap.isVisible
                        ? "Hide MiniMap"
                        : "Show MiniMap"
                );
                showMinimapButton.SetTooltip(sideMenu.isCollapsed ? showMinimapButton.buttonLabel.text : string.Empty);
            }

            #endregion


        }

        private void CreateFlowGraphView()
        {
            flowGraphView =
                new FlowGraphView()
                {
                    OnNodeSelected = OnNodeSelectionChanged,
                };

            openedGraphPathLabel =
                DesignUtils.NewFieldNameLabel("Graph Path")
                    .SetStyleTextAlign(TextAnchor.MiddleLeft);
            pingOpenGraphButton =
                DesignUtils.GetNewTinyButton
                    (
                        string.Empty,
                        EditorMicroAnimations.EditorUI.Icons.Location,
                        EditorSelectableColors.Default.ButtonIcon,
                        "Ping the opened graph in the Project view"
                    )
                    .SetButtonStyle(ButtonStyle.Clear)
                    .SetOnClick(() =>
                    {
                        if (flowGraphView == null) return;
                        if (flowGraphView.flowGraph == null) return;
                        if (!AssetDatabase.IsMainAsset(flowGraphView.flowGraph)) return;
                        EditorGUIUtility.PingObject(flowGraphView.flowGraph);
                    });
        }

        private void CreateMiniMap()
        {
            miniMap = new NodyMiniMap();

            root.schedule.Execute(() =>
            {
                miniMap
                    .SetPosition
                    (
                        new Rect
                        (
                            DesignUtils.k_Spacing,
                            position.height - 200 - DesignUtils.k_Spacing,
                            200,
                            200
                        )
                    );

                miniMap.SetIsVisible(NodyMiniMap.GetPrefs_IsVisible());
            });
        }

        private void Compose()
        {
            sideMenuContainer
                .AddChild(sideMenu);


            graphView
                .AddChild
                (
                    flowGraphView
                        .AddChild(miniMap)
                )
                .AddChild
                (
                    DesignUtils.row
                        .SetStyleHeight(24)
                        .SetStyleMarginTop(-24)
                        .SetStyleTop(-DesignUtils.k_Spacing)
                        .SetStyleMarginLeft(DesignUtils.k_Spacing)
                        .SetStyleAlignItems(Align.Center)
                        .SetStyleFlexGrow(0)
                        .AddChild(pingOpenGraphButton)
                        .AddChild(openedGraphPathLabel)
                );
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            EditorApplication.playModeStateChanged -= PlayModeStateChanged;
            EditorApplication.playModeStateChanged += PlayModeStateChanged;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            EditorApplication.playModeStateChanged -= PlayModeStateChanged;

            if (NodyInspectorWindow.isOpen)
                NodyInspectorWindow.instance.ClearInspector();
        }

        private void PlayModeStateChanged(PlayModeStateChange playModeState)
        {
            switch (playModeState)
            {
                case PlayModeStateChange.EnteredEditMode:
                    CloseGraph();
                    OnSelectionChange();
                    break;
                case PlayModeStateChange.ExitingEditMode:
                    break;
                case PlayModeStateChange.EnteredPlayMode:
                    OnSelectionChange();
                    break;
                case PlayModeStateChange.ExitingPlayMode:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(playModeState), playModeState, null);
            }
        }

        private void OnSelectionChange()
        {
            if (flowGraphView?.flowGraph == null)
                CloseGraph();

            if (Selection.activeObject == null)
                return;

            if (Selection.activeObject is FlowGraph graph)
            {
                if (flowGraphView?.flowGraph == graph)
                    return;

                if (!EditorUtility.IsPersistent(graph))
                    return;

                OpenGraph(graph);
                return;
            }

            if (Selection.activeGameObject == null)
                return;

            FlowController controller = Selection.activeGameObject.GetComponent<FlowController>();

            if (controller != null)
            {
                if (controller.flow == null)
                    return;
                if (flowGraphView?.flowGraph == controller.flow)
                    return;
                OpenGraph(controller.flow);
                return;
            }

        }

        public void CloseGraph()
        {
            openedGraphPathLabel?
                .SetStyleDisplay(DisplayStyle.None)
                .SetText(string.Empty);

            pingOpenGraphButton?
                .SetStyleDisplay(DisplayStyle.None);

            if (flowGraphView != null)
            {
                if (flowGraphView.flowGraph != null)
                    AssetDatabase.SaveAssetIfDirty(flowGraphView.flowGraph);
                
                flowGraphView.flowGraph = null;
                flowGraphView.ClearGraphView();
            }

            if (NodyInspectorWindow.isOpen)
                inspector.ClearInspector();
        }

        public void OpenGraph(FlowGraph flowGraph)
        {
            CloseGraph();

            if (flowGraph == null)
            {
                if (Selection.activeGameObject == null)
                    return;

                FlowController controller = Selection.activeGameObject.GetComponent<FlowController>();

                if (controller == null)
                    return;

                flowGraph = controller.flow;
            }

            if (Application.isPlaying)
            {
                if (flowGraph != null && flowGraphView != null)
                {
                    flowGraphView.PopulateView(flowGraph);
                }

                return;
            }

            if (flowGraph != null & AssetDatabase.CanOpenForEdit(flowGraph))
            {
                openedGraphPathLabel?
                    .SetStyleDisplay(DisplayStyle.Flex)
                    .SetText(AssetDatabase.GetAssetPath(flowGraph));
                pingOpenGraphButton?
                    .SetStyleDisplay(DisplayStyle.Flex);

                flowGraph.ResetGraph();

                if (flowGraphView == null) return;
                flowGraphView.PopulateView(flowGraph);
                root.schedule.Execute(() => flowGraphView.FrameAll()).ExecuteLater(100);
                root.schedule.Execute(() => flowGraphView.FrameAll()).ExecuteLater(300);
                root.schedule.Execute(() => flowGraphView.FrameAll()).ExecuteLater(500);
            }
        }

        private static void OnNodeSelectionChanged(FlowNodeView nodeView)
        {
            if (NodyInspectorWindow.isOpen)
                inspector.UpdateSelection(nodeView);
        }
    }
}
