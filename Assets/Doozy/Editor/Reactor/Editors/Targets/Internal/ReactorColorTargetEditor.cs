// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Runtime.Reactor.Targets;
using Doozy.Runtime.UIElements.Extensions;
using UnityEngine;
using UnityEngine.UIElements;

namespace Doozy.Editor.Reactor.Editors.Targets
{
    public abstract class ReactorColorTargetEditor : BaseReactorTargetEditor
    {
        public override IEnumerable<Texture2D> targetIconTextures => EditorMicroAnimations.Reactor.Icons.ColorTarget;

        protected override void InitializeEditor()
        {
            base.InitializeEditor();

            componentHeader
                .SetComponentTypeText("Color Target");

            targetObjectField =
                DesignUtils.NewObjectField(propertyTarget, typeof(ReactorColorTarget))
                    .SetStyleFlexGrow(1)
                    .SetTooltip("Animation color target");

            targetFluidField =
                FluidField.Get()
                    .SetIcon(propertyTarget.objectReferenceValue != null ? linkIconTextures : unlinkIconTextures)
                    .AddFieldContent(targetObjectField);

            targetObjectField.RegisterValueChangedCallback(evt =>
            {
                targetFluidField.SetIcon(evt.newValue != null ? linkIconTextures : unlinkIconTextures);
                targetFluidField.iconReaction?.Play();
            });
        }
    }
}