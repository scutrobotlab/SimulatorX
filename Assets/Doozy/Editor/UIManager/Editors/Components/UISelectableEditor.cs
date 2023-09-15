// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using System.Linq;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.ScriptableObjects.Colors;
using Doozy.Editor.UIManager.Editors.Components.Internal;
using Doozy.Runtime.UIManager.Components;
using UnityEditor;
using UnityEngine;
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Editor.UIManager.Editors.Components
{
    [CustomEditor(typeof(UISelectable), true)]
    [CanEditMultipleObjects]
    public sealed class UISelectableEditor : UISelectableBaseEditor
    {
        public override Color accentColor => EditorColors.UIManager.UIComponent;
        public override EditorSelectableColorInfo selectableAccentColor => EditorSelectableColors.UIManager.UIComponent;
        
        public static IEnumerable<Texture2D> selectableIconTextures => EditorMicroAnimations.UIManager.Icons.UISelectable;
        
        public UISelectable castedTarget => (UISelectable)target;
        public IEnumerable<UISelectable> castedTargets => targets.Cast<UISelectable>();

        protected override void InitializeEditor()
        {
            base.InitializeEditor();
            
            componentHeader
                .SetAccentColor(accentColor)
                .SetComponentNameText(ObjectNames.NicifyVariableName(nameof(UISelectable)))
                .SetIcon(selectableIconTextures.ToList())
                .AddManualButton("https://doozyentertainment.atlassian.net/wiki/spaces/DUI4/pages/1048707076/UISelectable?atlOrigin=eyJpIjoiOWU1NTFiYjc0ZmRiNGRiYWIwNmU5NGIzZjE1YzZhN2IiLCJwIjoiYyJ9")
                .AddYouTubeButton();
        }

        protected override void Compose()
        {
            if (castedTarget == null)
                return;
            
            base.Compose();
        }
    }
}
