// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using Doozy.Editor.Common.Layouts;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.ScriptableObjects.Colors;
using Doozy.Editor.Signals.Automation.Generators;
using Doozy.Editor.Signals.ScriptableObjects;
using Doozy.Editor.UIManager;
using Doozy.Runtime.Common;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;
// ReSharper disable ClassNeverInstantiated.Global

namespace Doozy.Editor.Signals.Layouts
{
    public sealed class StreamsDatabaseWindowLayout : CategoryNameGroupWindowLayout, IUIManagerDatabaseWindowLayout
    {
        public override string layoutName => "Streams"; 
        public override Color accentColor => EditorColors.Signals.Stream;
        public override EditorSelectableColorInfo selectableAccentColor => EditorSelectableColors.Signals.Stream;
        public override List<Texture2D> animatedIconTextures => EditorMicroAnimations.Signals.Icons.StreamDatabase;
        
        protected override Object targetObject => StreamIdDatabase.instance;
        protected override UnityAction onUpdateCallback => StreamIdDatabase.instance.onUpdateCallback;
        protected override CategoryNameGroup<CategoryNameItem> database => StreamIdDatabase.instance.database;
        protected override string groupTypeName => "Stream";
        
        protected override Func<string, List<string>, bool> exportDatabaseHandler => StreamIdDatabase.instance.ExportRoamingDatabase;
        protected override Func<List<ScriptableObject>, bool> importDatabaseHandler => StreamIdDatabase.instance.ImportRoamingDatabases;
        protected override string roamingDatabaseTypeName => nameof(StreamIdRoamingDatabase);

        protected override UnityAction runEnumGenerator => () => StreamIdExtensionGenerator.Run(true, false, true);
        
        public StreamsDatabaseWindowLayout()
        {
            AddHeader("Streams Database", "Stream Ids", animatedIconTextures);
            sideMenu.SetMenuLevel(FluidSideMenu.MenuLevel.Level_2);
            sideMenu.IsCollapsable(false);
            sideMenu.SetAccentColor(selectableAccentColor);
            Initialize();
        }
    }
}
