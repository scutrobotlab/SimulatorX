// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.ScriptableObjects.Colors;
using Doozy.Runtime.UIElements.Extensions;
using UnityEngine;
using UnityEngine.UIElements;
// ReSharper disable ClassNeverInstantiated.Global

namespace Doozy.Editor.Signals.Layouts
{
    public sealed class SignalsSettingsWindowLayout : FluidWindowLayout
    {
        public override Color accentColor => EditorColors.Signals.Signal;
        public override EditorSelectableColorInfo selectableAccentColor => EditorSelectableColors.Signals.Signal;
        public override List<Texture2D> animatedIconTextures => EditorMicroAnimations.EditorUI.Icons.Settings;

        public SignalsSettingsWindowLayout()
        {
            AddHeader("Settings", "Signals Global Settings", animatedIconTextures);
            menu.SetStyleDisplay(DisplayStyle.None);
            Initialize();
        }

        private void Initialize()
        {
            
        }
    }
}
