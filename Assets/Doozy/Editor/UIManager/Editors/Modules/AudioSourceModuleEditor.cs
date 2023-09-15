// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.Mody;
using Doozy.Editor.Mody.Components;
using Doozy.Runtime.UIElements.Extensions;
using Doozy.Runtime.UIManager.Modules;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Doozy.Editor.UIManager.Editors.Modules
{
    [CustomEditor(typeof(AudioSourceModule), true)]
    public sealed class AudioSourceModuleEditor : ModyModuleEditor<AudioSourceModule>
    {
        public override List<Texture2D> secondaryIconTextures => EditorMicroAnimations.EditorUI.Icons.Sound;

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
            fluidHeader.SetComponentNameText(AudioSourceModule.k_DefaultModuleName);

            //MODULE SETTINGS
            settingsContainer
                .AddChild(FluidField.Get<PropertyField>(nameof(AudioSourceModule.Source), "Source Reference", "Audio Source Reference", true));

            //MODULE ACTIONS
            AddActionToDrawer(new ModyActionsDrawerItem(serializedObject.FindProperty(nameof(AudioSourceModule.Play))));
            AddActionToDrawer(new ModyActionsDrawerItem(serializedObject.FindProperty(nameof(AudioSourceModule.Stop))));
            AddActionToDrawer(new ModyActionsDrawerItem(serializedObject.FindProperty(nameof(AudioSourceModule.Mute))));
            AddActionToDrawer(new ModyActionsDrawerItem(serializedObject.FindProperty(nameof(AudioSourceModule.Unmute))));
            AddActionToDrawer(new ModyActionsDrawerItem(serializedObject.FindProperty(nameof(AudioSourceModule.Pause))));
            AddActionToDrawer(new ModyActionsDrawerItem(serializedObject.FindProperty(nameof(AudioSourceModule.Unpause))));

            //COMPOSE
            Compose();
        }
    }
}
