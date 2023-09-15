// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using System.Linq;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.ScriptableObjects.Colors;
using Doozy.Runtime.Reactor.Ticker;
using Doozy.Runtime.UIElements.Extensions;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Editor.Reactor.Editors
{
    [CustomEditor(typeof(RuntimeTicker), true)]
    public class RuntimeTickerEditor : UnityEditor.Editor
    {
        public RuntimeTicker castedTarget => (RuntimeTicker)target;

        public static Color accentColor => EditorColors.Reactor.Red;
        public static EditorSelectableColorInfo selectableAccentColor => EditorSelectableColors.Reactor.Red;
        
        public static IEnumerable<Texture2D> heartbeatIconTextures => EditorMicroAnimations.Reactor.Icons.Heartbeat;
        
        private VisualElement root { get; set; }
        private FluidComponentHeader componentHeader { get; set; }
        
        public override VisualElement CreateInspectorGUI()
        {
            CreateEditor();
            Compose();
            return root;
        }
        
        private void OnDestroy()
        {
            componentHeader?.Recycle();
        }

        private void CreateEditor()
        {
            root = new VisualElement();

            componentHeader = FluidComponentHeader.Get()
                .SetAccentColor(accentColor)
                .SetComponentNameText(ObjectNames.NicifyVariableName(nameof(RuntimeTicker)))
                .SetIcon(heartbeatIconTextures.ToList())
                .SetElementSize(ElementSize.Large)
                .AddManualButton("www.bit.ly/DoozyKnowledgeBase4")
                .AddYouTubeButton();
        }

        private void Compose()
        {
            root
                .AddChild(componentHeader);
        }
    }
}
