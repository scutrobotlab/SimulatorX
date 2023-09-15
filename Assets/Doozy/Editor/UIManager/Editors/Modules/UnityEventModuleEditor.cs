// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Editor.Mody;
using Doozy.Editor.Mody.Components;
using Doozy.Runtime.UIElements.Extensions;
using Doozy.Runtime.UIManager.Modules;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Doozy.Editor.UIManager.Editors.Modules
{
    [CustomEditor(typeof(UnityEventModule), true)]
    public sealed class UnityEventModuleEditor : ModyModuleEditor<UnityEventModule>
    {
        public override List<Texture2D> secondaryIconTextures => EditorMicroAnimations.EditorUI.Icons.UnityEvent;

        public override VisualElement CreateInspectorGUI()
        {
            CreateEditor();
            return root;
        }

        protected override void CreateEditor()
        {
            base.CreateEditor();

            //MODULE HEADER
            fluidHeader.SetSecondaryIcon(secondaryIconTextures);
            fluidHeader.SetComponentNameText(UnityEventModule.k_DefaultModuleName);
            
            //MODULE SETTINGS
            settingsContainer
                .AddChild(DesignUtils.NewPropertyField(serializedObject.FindProperty(nameof(UnityEventModule.Event))));

            //MODULE ACTIONS
            AddActionToDrawer(new ModyActionsDrawerItem(serializedObject.FindProperty(nameof(UnityEventModule.InvokeEvent))));
            
            //COMPOSE
            Compose();
        }
    }
}
