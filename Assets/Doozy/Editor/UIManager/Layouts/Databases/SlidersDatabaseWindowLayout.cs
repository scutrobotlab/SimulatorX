// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using Doozy.Editor.Common.Layouts;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.ScriptableObjects.Colors;
using Doozy.Editor.UIManager.Automation.Generators;
using Doozy.Editor.UIManager.ScriptableObjects;
using Doozy.Runtime.Common;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;

namespace Doozy.Editor.UIManager.Layouts.Databases
{
    public sealed class SlidersDatabaseWindowLayout  : CategoryNameGroupWindowLayout, IUIManagerDatabaseWindowLayout
    {
        public override string layoutName => "Sliders";
        public override List<Texture2D> animatedIconTextures => EditorMicroAnimations.UIManager.Icons.SlidersDatabase;
        public override Color accentColor => EditorColors.UIManager.UIComponent;
        public override EditorSelectableColorInfo selectableAccentColor => EditorSelectableColors.UIManager.UIComponent;

        protected override Object targetObject => UISliderIdDatabase.instance;
        protected override UnityAction onUpdateCallback => UISliderIdDatabase.instance.onUpdateCallback; 
        protected override CategoryNameGroup<CategoryNameItem> database => UISliderIdDatabase.instance.database;
        protected override string groupTypeName => "Slider";

        protected override Func<string, List<string>, bool> exportDatabaseHandler => UISliderIdDatabase.instance.ExportRoamingDatabase;
        protected override Func<List<ScriptableObject>, bool> importDatabaseHandler => UISliderIdDatabase.instance.ImportRoamingDatabases;
        protected override string roamingDatabaseTypeName => nameof(UISliderIdRoamingDatabase);

        protected override UnityAction runEnumGenerator => () => UISliderIdExtensionGenerator.Run(true, false, true);
        
        public SlidersDatabaseWindowLayout()
        {
            AddHeader("Sliders Database", "UISlider Ids", animatedIconTextures);
            sideMenu.SetMenuLevel(FluidSideMenu.MenuLevel.Level_2);
            sideMenu.IsCollapsable(false);
            sideMenu.SetAccentColor(selectableAccentColor);
            Initialize();
        }
    }
}
