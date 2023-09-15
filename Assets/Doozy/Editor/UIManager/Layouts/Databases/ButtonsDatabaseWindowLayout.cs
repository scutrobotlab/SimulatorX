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
    public sealed class ButtonsDatabaseWindowLayout : CategoryNameGroupWindowLayout, IUIManagerDatabaseWindowLayout
    {
        public override string layoutName => "Buttons";
        public override List<Texture2D> animatedIconTextures => EditorMicroAnimations.UIManager.Icons.ButtonsDatabase;
        public override Color accentColor => EditorColors.UIManager.UIComponent;
        public override EditorSelectableColorInfo selectableAccentColor => EditorSelectableColors.UIManager.UIComponent;

        protected override Object targetObject => UIButtonIdDatabase.instance;
        protected override UnityAction onUpdateCallback => UIButtonIdDatabase.instance.onUpdateCallback; 
        protected override CategoryNameGroup<CategoryNameItem> database => UIButtonIdDatabase.instance.database;
        protected override string groupTypeName => "Button";

        protected override Func<string, List<string>, bool> exportDatabaseHandler => UIButtonIdDatabase.instance.ExportRoamingDatabase;
        protected override Func<List<ScriptableObject>, bool> importDatabaseHandler => UIButtonIdDatabase.instance.ImportRoamingDatabases;
        protected override string roamingDatabaseTypeName => nameof(UIButtonIdRoamingDatabase);

        protected override UnityAction runEnumGenerator => () => UIButtonIdExtensionGenerator.Run(true, false, true);
        
        public ButtonsDatabaseWindowLayout()
        {
            AddHeader("Buttons Database", "UIButton Ids", animatedIconTextures);
            sideMenu.SetMenuLevel(FluidSideMenu.MenuLevel.Level_2);
            sideMenu.IsCollapsable(false);
            sideMenu.SetAccentColor(selectableAccentColor);
            Initialize();
        }
    }
}
