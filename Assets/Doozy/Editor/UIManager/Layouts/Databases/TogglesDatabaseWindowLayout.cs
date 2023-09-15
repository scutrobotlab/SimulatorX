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
    public sealed class TogglesDatabaseWindowLayout : CategoryNameGroupWindowLayout, IUIManagerDatabaseWindowLayout
    {
        public override string layoutName => "Toggles";
        public override List<Texture2D> animatedIconTextures => EditorMicroAnimations.UIManager.Icons.TogglesDatabase;
        public override Color accentColor => EditorColors.UIManager.UIComponent;
        public override EditorSelectableColorInfo selectableAccentColor => EditorSelectableColors.UIManager.UIComponent;
        
        protected override Object targetObject => UIToggleIdDatabase.instance;
        protected override UnityAction onUpdateCallback => UIToggleIdDatabase.instance.onUpdateCallback;
        protected override CategoryNameGroup<CategoryNameItem> database => UIToggleIdDatabase.instance.database;
        protected override string groupTypeName => "Toggle";

        protected override Func<string, List<string>, bool> exportDatabaseHandler => UIToggleIdDatabase.instance.ExportRoamingDatabase;
        protected override Func<List<ScriptableObject>, bool> importDatabaseHandler => UIToggleIdDatabase.instance.ImportRoamingDatabases;
        protected override string roamingDatabaseTypeName => nameof(UIToggleIdRoamingDatabase);

        protected override UnityAction runEnumGenerator => () => UIToggleIdExtensionGenerator.Run(true, false, true);
        
        public TogglesDatabaseWindowLayout()
        {
            AddHeader("Toggles Database", "UIToggle Ids", animatedIconTextures);
            sideMenu.SetMenuLevel(FluidSideMenu.MenuLevel.Level_2);
            sideMenu.IsCollapsable(false);
            sideMenu.SetAccentColor(selectableAccentColor);
            Initialize();
        }
    }
}
