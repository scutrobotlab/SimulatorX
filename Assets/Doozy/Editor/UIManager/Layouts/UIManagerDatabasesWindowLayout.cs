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
using Doozy.Runtime.UIElements.Extensions;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Doozy.Editor.UIManager.Layouts
{
    public sealed class UIManagerDatabasesWindowLayout : FluidWindowLayout, IUIManagerWindowLayout
    {
        public int order => 10;
        
        public override string layoutName => "Databases";
        public override Texture2D staticIconTexture => EditorTextures.EditorUI.Icons.GenericDatabase;
        public override List<Texture2D> animatedIconTextures => EditorMicroAnimations.EditorUI.Icons.GenericDatabase;

        public override Color accentColor => EditorColors.UIManager.UIComponent;
        public override EditorSelectableColorInfo selectableAccentColor => EditorSelectableColors.UIManager.UIComponent;

        public UIManagerDatabasesWindowLayout()
        {
            content.ResetLayout();

            sideMenu.RemoveSearch();
            sideMenu.IsCollapsable(true);
            sideMenu.SetCustomWidth(200);

            foreach (IUIManagerDatabaseWindowLayout windowLayout in GetLayouts().OrderBy(l => l.layoutName))
            {
                //SIDE MENU BUTTON
                FluidToggleButtonTab sideMenuButton = sideMenu.AddButton(windowLayout.layoutName, selectableAccentColor);

                //ADD SIDE MENU BUTTON ICON (animated or static)
                if (windowLayout.animatedIconTextures?.Count > 0)
                    sideMenuButton.SetIcon(windowLayout.animatedIconTextures); // <<< ANIMATED ICON
                else if (windowLayout.staticIconTexture != null)
                    sideMenuButton.SetIcon(windowLayout.staticIconTexture); // <<< STATIC ICON

                //WINDOW LAYOUT (added to the content container when the button is pressed)                
                VisualElement customWindowLayout = ((VisualElement)windowLayout).SetStyleFlexGrow(1);

                sideMenuButton.SetToggleAccentColor(((IUIManagerDatabaseWindowLayout)customWindowLayout).selectableAccentColor);

                sideMenuButton.OnValueChanged += evt =>
                {
                    if (!evt.newValue) return;
                    content.Clear();
                    content.Add(customWindowLayout);
                };
            }
        }

        private static IEnumerable<IUIManagerDatabaseWindowLayout> GetLayouts()
        {
            TypeCache.TypeCollection results = TypeCache.GetTypesDerivedFrom(typeof(IUIManagerDatabaseWindowLayout));
            // IEnumerable<Type> results = ReflectionUtils.GetTypesThatImplementInterface<IUIManagerDatabaseWindowLayout>(ReflectionUtils.doozyEditorAssembly);
            return results.Select(type => (IUIManagerDatabaseWindowLayout)Activator.CreateInstance(type)).ToList();
        }
    }
}
