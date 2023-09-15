// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using System.Linq;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.ScriptableObjects.Colors;
using Doozy.Editor.EditorUI.ScriptableObjects.Fonts;
using Doozy.Editor.EditorUI.ScriptableObjects.Layouts;
using Doozy.Editor.EditorUI.ScriptableObjects.MicroAnimations;
using Doozy.Editor.EditorUI.ScriptableObjects.Styles;
using Doozy.Editor.EditorUI.ScriptableObjects.Textures;
using Doozy.Editor.EditorUI.Windows.Internal;
using Doozy.Runtime.UIElements.Extensions;
using UnityEditor;
using UnityEngine.UIElements;

namespace Doozy.Editor.EditorUI.Windows
{
    public class EditorUIWindow : FluidWindow<EditorUIWindow>
    {
        private const string WINDOW_TITLE = "EditorUI";
        public const string k_WindowMenuPath = "Tools/Doozy/EditorUI/";

        [MenuItem(k_WindowMenuPath + "Window", priority = -600)]
        public static void Open() => InternalOpenWindow(WINDOW_TITLE);

        [MenuItem(k_WindowMenuPath + "Refresh", priority = -450)]
        public static void Refresh()
        {
            EditorDataLayoutDatabase.instance.RefreshDatabase();
            EditorDataStyleDatabase.instance.RefreshDatabase();
            
            EditorDataColorDatabase.instance.RefreshDatabase();
            EditorDataSelectableColorDatabase.instance.RefreshDatabase();
            
            EditorDataFontDatabase.instance.RefreshDatabase();
            
            EditorDataTextureDatabase.instance.RefreshDatabase();
            EditorDataMicroAnimationDatabase.instance.RefreshDatabase();
        }

        public static EditorSelectableColorInfo buttonAccentColor => EditorSelectableColors.EditorUI.Amber;

        public TemplateContainer templateContainer { get; private set; }
        public VisualElement layoutContainer { get; private set; }
        public VisualElement sideMenuContainer { get; private set; }
        public VisualElement contentContainer { get; private set; }

        private FluidSideMenu sideMenu { get; set; }

        protected override void CreateGUI()
        {
            root.Add(templateContainer = EditorLayouts.EditorUI.EditorUIWindow.CloneTree());

            templateContainer
                .SetStyleFlexGrow(1)
                .AddStyle(EditorStyles.EditorUI.EditorUIWindow);

            layoutContainer = templateContainer.Q<VisualElement>(nameof(layoutContainer));
            sideMenuContainer = layoutContainer.Q<VisualElement>(nameof(sideMenuContainer));
            contentContainer = layoutContainer.Q<VisualElement>(nameof(contentContainer));

            sideMenuContainer.Add(sideMenu = new FluidSideMenu());

            foreach (IEditorUIDatabaseWindowLayout layout in GetLayouts())
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

                sideMenuButton.OnValueChanged += evt =>
                {
                    if (!evt.newValue) return;
                    contentContainer.Clear();
                    contentContainer.Add(customWindowLayout);
                };
            }
        }

        private static IEnumerable<IEditorUIDatabaseWindowLayout> GetLayouts()
        {
            TypeCache.TypeCollection results = TypeCache.GetTypesDerivedFrom(typeof(IEditorUIDatabaseWindowLayout));
            // IEnumerable<Type> results = ReflectionUtils.GetTypesThatImplementInterface<IEditorUIDatabaseWindowLayout>(ReflectionUtils.doozyEditorAssembly);
            return results.Select(type => (IEditorUIDatabaseWindowLayout)Activator.CreateInstance(type)).ToList();
            ;
        }
    }
}
