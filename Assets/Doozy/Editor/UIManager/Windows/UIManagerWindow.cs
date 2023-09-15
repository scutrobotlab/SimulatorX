// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using System.Linq;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.Components.Internal;
using Doozy.Editor.EditorUI.ScriptableObjects.Colors;
using Doozy.Editor.EditorUI.Windows.Internal;
using Doozy.Runtime.UIElements.Extensions;
using UnityEditor;
using UnityEngine.UIElements;
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Editor.UIManager.Windows
{
    public class UIManagerWindow : FluidWindow<UIManagerWindow>
    {
        private const string WINDOW_TITLE = "UI Manager";
        public const string k_WindowMenuPath = "Tools/Doozy/UI Manager/";

        [MenuItem(k_WindowMenuPath + "Window", priority = -750)]
        public static void Open() => InternalOpenWindow(WINDOW_TITLE);

        public static EditorSelectableColorInfo buttonAccentColor => EditorSelectableColors.EditorUI.Blue;

        public TemplateContainer templateContainer { get; private set; }
        public VisualElement layoutContainer { get; private set; }
        public VisualElement sideMenuContainer { get; private set; }
        public VisualElement contentContainer { get; private set; }

        private FluidSideMenu sideMenu { get; set; }

        protected override void CreateGUI()
        {
            root.Add(templateContainer = EditorLayouts.UIManager.UIManagerWindow.CloneTree());

            templateContainer
                .SetStyleFlexGrow(1)
                .AddStyle(EditorUI.EditorStyles.UIManager.UIManagerWindow);

            layoutContainer = templateContainer.Q<VisualElement>(nameof(layoutContainer));
            sideMenuContainer = layoutContainer.Q<VisualElement>(nameof(sideMenuContainer));
            contentContainer = layoutContainer.Q<VisualElement>(nameof(contentContainer));

            sideMenuContainer.Add(sideMenu = new FluidSideMenu());
            sideMenu.IsCollapsable(true);
            sideMenu.SetCustomWidth(200);

            foreach (IUIManagerWindowLayout layout in GetLayouts().OrderBy(l => l.order))
            {
                //SIDE MENU BUTTON
                FluidToggleButtonTab sideMenuButton = sideMenu.AddButton(layout.layoutName, buttonAccentColor);

                //ADD SIDE MENU BUTTON ICON (animated or static)
                if (layout.animatedIconTextures?.Count > 0)
                    sideMenuButton.SetIcon(layout.animatedIconTextures); // <<< ANIMATED ICON
                else if (layout.staticIconTexture != null)
                    sideMenuButton.SetIcon(layout.staticIconTexture); // <<< STATIC ICON

                //WINDOW LAYOUT (added to the content container when the button is pressed)                
                VisualElement customWindowLayout = ((VisualElement)layout).SetStyleFlexGrow(1);

                sideMenuButton.SetToggleAccentColor(((IUIManagerWindowLayout)customWindowLayout).selectableAccentColor);

                sideMenuButton.OnValueChanged += evt =>
                {
                    if (!evt.newValue) return;
                    contentContainer.Clear();
                    contentContainer.Add(customWindowLayout);
                };
            }
        }

        private static IEnumerable<IUIManagerWindowLayout> GetLayouts()
        {
            TypeCache.TypeCollection results = TypeCache.GetTypesDerivedFrom(typeof(IUIManagerWindowLayout));
            // IEnumerable<Type> results = ReflectionUtils.GetTypesThatImplementInterface<IUIManagerWindowLayout>(ReflectionUtils.doozyEditorAssembly);
            return results.Select(type => (IUIManagerWindowLayout)Activator.CreateInstance(type)).ToList();
        }
    }
}
