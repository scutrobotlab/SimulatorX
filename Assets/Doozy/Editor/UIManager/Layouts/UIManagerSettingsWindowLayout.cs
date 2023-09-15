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
    public sealed class UIManagerSettingsWindowLayout : FluidWindowLayout, IUIManagerWindowLayout
    {
        public int order => 100;
        
        public override string layoutName => "Settings";
        public override List<Texture2D> animatedIconTextures => EditorMicroAnimations.EditorUI.Icons.Settings;

        public override Color accentColor => EditorColors.UIManager.Settings;
        public override EditorSelectableColorInfo selectableAccentColor => EditorSelectableColors.UIManager.Settings;

        public UIManagerSettingsWindowLayout()
        {
            content.ResetLayout();
                
            sideMenu.RemoveSearch();
            sideMenu.IsCollapsable(true);
            sideMenu.SetCustomWidth(200);
            
            foreach (IUIManagerSettingsWindowLayout windowLayout in  GetLayouts().OrderBy(l => l.layoutName))
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
                
                sideMenuButton.SetToggleAccentColor(((IUIManagerSettingsWindowLayout)customWindowLayout).selectableAccentColor);
                
                sideMenuButton.OnValueChanged += evt =>
                {
                    if (!evt.newValue) return;
                    content.Clear();
                    content.Add(customWindowLayout);
                };
            }
        }
        
        private static IEnumerable<IUIManagerSettingsWindowLayout> GetLayouts()
        {
            TypeCache.TypeCollection results = TypeCache.GetTypesDerivedFrom(typeof(IUIManagerSettingsWindowLayout));
            // IEnumerable<Type> results = ReflectionUtils.GetTypesThatImplementInterface<IUIManagerSettingsWindowLayout>(ReflectionUtils.doozyEditorAssembly);
            return results.Select(type => (IUIManagerSettingsWindowLayout)Activator.CreateInstance(type)).ToList();;
        }
       
    }
}
