// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Runtime.Reactor;
using Doozy.Runtime.UIElements.Extensions;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
namespace Doozy.Editor.Reactor.Editors.Targets
{
    public abstract class ProgressTargetEditor : BaseReactorTargetEditor
    {
        public override IEnumerable<Texture2D> targetIconTextures => EditorMicroAnimations.Reactor.Icons.ProgressTarget;
        
        protected EnumField targetModeEnumField { get; set; }
        protected FluidField targetModeFluidField { get; set; }
        protected SerializedProperty propertyTargetMode { get; set; }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            targetModeFluidField?.Recycle();
        }

        protected override void FindProperties()
        {
            base.FindProperties();
            propertyTargetMode = serializedObject.FindProperty("TargetMode");
        }
        
        protected override void InitializeEditor()
        {
            base.InitializeEditor();

            componentHeader
                .SetComponentTypeText("Progress Target");

            targetObjectField =
                DesignUtils.NewObjectField(propertyTarget, typeof(ProgressTarget))
                    .SetStyleFlexGrow(1)
                    .SetTooltip("Progress target");

            targetFluidField =
                FluidField.Get()
                    .SetIcon(propertyTarget.objectReferenceValue != null ? linkIconTextures : unlinkIconTextures)
                    .AddFieldContent(targetObjectField);

            targetObjectField.RegisterValueChangedCallback(evt =>
            {
                targetFluidField.SetIcon(evt.newValue != null ? linkIconTextures : unlinkIconTextures);
                targetFluidField.iconReaction?.Play();
            });

            if (!Enum.IsDefined(typeof(ProgressTarget.Mode), propertyTargetMode.enumValueIndex))
            {
                Debug.Log("WTF");
            }
            
            targetModeEnumField =
                DesignUtils.NewEnumField(propertyTargetMode)
                    .SetStyleFlexGrow(1);
            
            targetModeFluidField =
                FluidField.Get()
                    .SetLabelText("Target Mode")
                    .AddFieldContent(targetModeEnumField);
        }

        protected override void Compose()
        {
            base.Compose();

            root
                .AddChild(targetModeFluidField)
                .AddChild(DesignUtils.spaceBlock2X);
        }
    }
}
