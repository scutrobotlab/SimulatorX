// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.Components.Internal;
using Doozy.Editor.EditorUI.ScriptableObjects.Colors;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Editor.EditorUI.Windows.Internal;
using Doozy.Editor.Reactor.Components;
using Doozy.Editor.Reactor.Layouts;
using Doozy.Runtime.Reactor.ScriptableObjects;
using Doozy.Runtime.UIElements.Extensions;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Doozy.Editor.Reactor.Windows
{
    public class ReactorWindow : FluidWindow<ReactorWindow>
    {
        private const string WINDOW_TITLE = "Reactor";
        public const string k_WindowMenuPath = "Tools/Doozy/Reactor/";

        [MenuItem(k_WindowMenuPath + "Window", priority = -750)]
        public static void Open() => InternalOpenWindow(WINDOW_TITLE);
    
        [MenuItem(k_WindowMenuPath + "Refresh", priority = -650)]
        private static void RefreshDatabase() => UIAnimationPresetDatabase.instance.RefreshDatabase();

        private TemplateContainer templateContainer { get; set; }
        private VisualElement sideMenuContainer { get; set; }
        private VisualElement contentContainer { get; set; }

        private FluidSideMenu sideMenu { get; set; }

        private Color accentColor { get; set; }
        private EditorSelectableColorInfo selectableAccentColor { get; set; }

        private TickerVisualizer editorTickerVisualizer { get; set; }
        private TickerVisualizer runtimeTickerVisualizer { get; set; }

        private ReactorPresetsWindowLayout m_PresetsWindowLayout;
        private ReactorSettingsWindowLayout m_SettingsWindowLayout;

        public IEnumerable<Texture2D> reactorIconTextures => EditorMicroAnimations.Reactor.Icons.ReactorIconToFull;

        protected override void OnEnable()
        {
            base.OnEnable();
            minSize = new Vector2(600, 400);
            position = new Rect(position.x, position.y, 800, 500);
        }

        protected override void CreateGUI()
        {
            accentColor = EditorColors.Reactor.Red;
            selectableAccentColor = EditorSelectableColors.Reactor.Red;

            root.Add(templateContainer = EditorLayouts.Reactor.ReactorWindow.CloneTree());
            templateContainer
                .AddStyle(EditorUI.EditorStyles.Reactor.ReactorWindow)
                .SetStyleFlexGrow(1);

            sideMenuContainer = templateContainer.Q<VisualElement>("SideMenuContainer");
            contentContainer = templateContainer.Q<VisualElement>("ContentContainer");

            sideMenuContainer.Add(sideMenu =
                new FluidSideMenu()
                    .SetAccentColor(selectableAccentColor)
                    .SetMenuLevel(FluidSideMenu.MenuLevel.Level_0));

            m_PresetsWindowLayout = m_PresetsWindowLayout ?? Activator.CreateInstance<ReactorPresetsWindowLayout>();
            m_SettingsWindowLayout = m_SettingsWindowLayout ?? Activator.CreateInstance<ReactorSettingsWindowLayout>();

            var buttonPresets =
                sideMenu
                    .AddButton("Presets", selectableAccentColor).SetIcon(m_PresetsWindowLayout.animatedIconTextures)
                    .SetOnValueChanged(evt =>
                    {
                        if (!evt.newValue) return;
                        contentContainer.Clear();
                        contentContainer.Add(m_PresetsWindowLayout);
                    });

            var buttonSettings =
                sideMenu
                    .AddButton("Settings", selectableAccentColor).SetIcon(m_SettingsWindowLayout.animatedIconTextures)
                    .SetOnValueChanged(evt =>
                    {
                        if (!evt.newValue) return;
                        contentContainer.Clear();
                        contentContainer.Add(m_SettingsWindowLayout);
                    });

            sideMenu
                .footerContainer.SetStyleDisplay(DisplayStyle.Flex).SetStylePadding(DesignUtils.k_Spacing)
                .AddChild(runtimeTickerVisualizer = new TickerVisualizer(TickerVisualizer.TickerType.Runtime, ReactorRuntimeTickerWindow.ShowWindow).SetStyleBorderRadius(8, 8, 0, 0))
                .AddSpace(0, DesignUtils.k_Spacing)
                .AddChild(editorTickerVisualizer = new TickerVisualizer(TickerVisualizer.TickerType.Editor, ReactorEditorTickerWindow.ShowWindow).SetStyleBorderRadius(0, 0, 8, 8));

            sideMenu.OnExpand += () =>
            {
                runtimeTickerVisualizer.labelsContainer.SetStyleDisplay(DisplayStyle.Flex);
                editorTickerVisualizer.labelsContainer.SetStyleDisplay(DisplayStyle.Flex);

                runtimeTickerVisualizer.layoutContainer.SetStyleAlignSelf(Align.Auto);
                editorTickerVisualizer.layoutContainer.SetStyleAlignSelf(Align.Auto);
            };

            sideMenu.OnCollapse += () =>
            {
                runtimeTickerVisualizer.labelsContainer.SetStyleDisplay(DisplayStyle.None);
                editorTickerVisualizer.labelsContainer.SetStyleDisplay(DisplayStyle.None);

                runtimeTickerVisualizer.layoutContainer.SetStyleAlignSelf(Align.Center);
                editorTickerVisualizer.layoutContainer.SetStyleAlignSelf(Align.Center);
            };

            buttonPresets.isOn = true;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            m_PresetsWindowLayout?.OnDestroy();
            m_SettingsWindowLayout?.OnDestroy();

            editorTickerVisualizer?.Dispose();
            runtimeTickerVisualizer?.Dispose();
            
            AssetDatabase.SaveAssetIfDirty(ReactorSettings.instance);
            // AssetDatabase.SaveAssets();
        }
    }
}
