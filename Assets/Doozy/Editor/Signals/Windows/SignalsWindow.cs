// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using Doozy.Editor.Common.Layouts;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.Components.Internal;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Editor.EditorUI.Windows.Internal;
using Doozy.Editor.Signals.Layouts;
using Doozy.Runtime.UIElements.Extensions;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Doozy.Editor.Signals.Windows
{
    public class SignalsWindow : FluidWindow<SignalsWindow>
    {
        private const string WINDOW_TITLE = "Signals";
        public const string k_WindowMenuPath = "Tools/Doozy/Signals/";

        [MenuItem(k_WindowMenuPath + "Window", priority = -750)]
        public static void Open() => InternalOpenWindow(WINDOW_TITLE);

        [MenuItem(k_WindowMenuPath + "Refresh Providers", priority = -650)]
        public static void RefreshProviders() => SignalsUtils.RefreshProviders();

        private TemplateContainer templateContainer { get; set; }
        private VisualElement sideMenuContainer { get; set; }
        private VisualElement contentContainer { get; set; }

        private FluidSideMenu sideMenu { get; set; }

        protected override void OnEnable()
        {
            base.OnEnable();
            minSize = new Vector2(600, 400);
        }

        private List<VisualElement> layouts { get; set; }

        private StreamsConsoleWindowLayout streamsLayout { get; set; }
        private SignalsConsoleWindowLayout signalsLayout { get; set; }
        private CategoryNameGroupWindowLayout streamsDatabaseLayout { get; set; }
        private SignalsSettingsWindowLayout settingsLayout { get; set; }

        protected override void CreateGUI()
        {
            layouts = new List<VisualElement>();

            root.Add(templateContainer = EditorLayouts.Signals.SignalsWindow.CloneTree());
            templateContainer
                .AddStyle(EditorUI.EditorStyles.Signals.SignalsWindow)
                .SetStyleFlexGrow(1);

            sideMenuContainer = templateContainer.Q<VisualElement>("SideMenuContainer");
            contentContainer = templateContainer.Q<VisualElement>("ContentContainer");

            sideMenuContainer.Add(sideMenu =
                new FluidSideMenu()
                    .SetMenuLevel(FluidSideMenu.MenuLevel.Level_0));

            layouts.Add(streamsLayout = streamsLayout ?? Activator.CreateInstance<StreamsConsoleWindowLayout>());
            layouts.Add(signalsLayout = signalsLayout ?? Activator.CreateInstance<SignalsConsoleWindowLayout>());
            layouts.Add(streamsDatabaseLayout = streamsDatabaseLayout ?? Activator.CreateInstance<StreamsDatabaseWindowLayout>());
            layouts.Add(settingsLayout = settingsLayout ?? Activator.CreateInstance<SignalsSettingsWindowLayout>());

            var buttonStreamsConsole =
                sideMenu
                    .AddButton("Streams Console", streamsLayout.selectableAccentColor).SetIcon(streamsLayout.animatedIconTextures)
                    .SetOnValueChanged(evt =>
                    {
                        if (!evt.newValue) return;
                        SetLayout(streamsLayout);
                    });

            var buttonSignalsConsole =
                sideMenu
                    .AddButton("Signals Console", signalsLayout.selectableAccentColor).SetIcon(signalsLayout.animatedIconTextures)
                    .SetOnValueChanged(evt =>
                    {
                        if (!evt.newValue) return;
                        SetLayout(signalsLayout);
                    });

            sideMenu.buttonsScrollViewContainer.AddSpace(0, DesignUtils.k_Spacing * 4);

            var buttonStaticStreams =
                sideMenu
                    .AddButton("Streams Database", streamsDatabaseLayout.selectableAccentColor).SetIcon(streamsDatabaseLayout.animatedIconTextures)
                    .SetOnValueChanged(evt =>
                    {
                        if (!evt.newValue) return;
                        SetLayout(streamsDatabaseLayout);
                    });

            sideMenu.buttonsScrollViewContainer.AddSpace(0, DesignUtils.k_Spacing * 4);

            // var buttonSettings =
            //     sideMenu
            //         .AddButton("Settings", settingsLayout.selectableAccentColor).SetIcon(settingsLayout.animatedIconTextures)
            //         .SetOnValueChanged(evt =>
            //         {
            //             if (!evt.newValue) return;
            //             SetLayout(settingsLayout);
            //         });
        }

        private void HideLayouts()
        {
            foreach (VisualElement layout in layouts)
                layout.SetStyleDisplay(DisplayStyle.None);
        }

        private void SetLayout(VisualElement layout)
        {
            HideLayouts();
            contentContainer.Clear();
            contentContainer.AddChild(layout.SetStyleDisplay(DisplayStyle.Flex));
        }


        protected override void OnDestroy()
        {
            base.OnDestroy();
            streamsLayout?.OnDestroy();
            signalsLayout?.OnDestroy();
        }
    }
}
