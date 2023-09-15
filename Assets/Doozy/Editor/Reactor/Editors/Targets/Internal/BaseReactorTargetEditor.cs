// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using System.Linq;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.ScriptableObjects.Colors;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Runtime.UIElements.Extensions;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable MemberCanBeProtected.Global

namespace Doozy.Editor.Reactor.Editors.Targets
{
    public abstract class BaseReactorTargetEditor : UnityEditor.Editor
    {
        public static Color accentColor => EditorColors.Reactor.Red;
        public static EditorSelectableColorInfo selectableAccentColor => EditorSelectableColors.Reactor.Red;

        public virtual IEnumerable<Texture2D> targetIconTextures => EditorMicroAnimations.Reactor.Icons.Reactor;
        public static IEnumerable<Texture2D> linkIconTextures => EditorMicroAnimations.EditorUI.Icons.Link;
        public static IEnumerable<Texture2D> unlinkIconTextures => EditorMicroAnimations.EditorUI.Icons.Unlink;
        
        protected VisualElement root { get; set; }
        protected FluidComponentHeader componentHeader { get; set; }

        protected ObjectField targetObjectField { get; set; }
        protected FluidField targetFluidField { get; set; }
        protected SerializedProperty propertyTarget { get; set; }
        
        protected virtual void OnDestroy()
        {
            componentHeader?.Recycle();
            targetFluidField?.Recycle();
        }

        public override VisualElement CreateInspectorGUI()
        {
            InitializeEditor();
            Compose();
            return root;
        }

        protected virtual void FindProperties()
        {
            propertyTarget = serializedObject.FindProperty("Target");
        }
        
        protected virtual void InitializeEditor()
        {
            FindProperties();

            root = new VisualElement();

            componentHeader = FluidComponentHeader.Get()
                .SetAccentColor(accentColor)
                .SetSecondaryIcon(targetIconTextures.ToList())
                .SetElementSize(ElementSize.Tiny);
        }

        protected virtual void Compose()
        {
            root
                .AddChild(componentHeader)
                .AddChild(DesignUtils.spaceBlock)
                .AddChild(targetFluidField)
                .AddChild(DesignUtils.spaceBlock2X);
        }
    }
}
