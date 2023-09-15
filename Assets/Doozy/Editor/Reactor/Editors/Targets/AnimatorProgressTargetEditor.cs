// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.Components.Internal;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Editor.UIElements;
using Doozy.Runtime.Reactor.Targets.ProgressTargets;
using Doozy.Runtime.UIElements.Extensions;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
namespace Doozy.Editor.Reactor.Editors.Targets
{
    [CustomEditor(typeof(AnimatorProgressTarget), true)]
    public class AnimatorProgressTargetEditor : ProgressTargetEditor
    {
        public override IEnumerable<Texture2D> targetIconTextures => EditorMicroAnimations.Reactor.Icons.AnimatorProgressTarget;

        private TextField parameterNameTextField { get; set; }
        private FluidField parameterNameFluidField { get; set; }
        private SerializedProperty propertyParameterName { get; set; }


        protected override void OnDestroy()
        {
            base.OnDestroy();

            parameterNameFluidField?.Recycle();
        }

        protected override void FindProperties()
        {
            base.FindProperties();

            propertyParameterName = serializedObject.FindProperty("ParameterName");
        }

        protected override void InitializeEditor()
        {
            base.InitializeEditor();

            componentHeader
                .SetComponentNameText(ObjectNames.NicifyVariableName(nameof(Animator)))
                .AddManualButton()
                .AddYouTubeButton();

            targetObjectField
                .SetObjectType(typeof(Animator));

            parameterNameTextField =
                DesignUtils.NewTextField(propertyParameterName)
                    .SetStyleFlexGrow(1)
                    .SetTooltip("Target parameter name (defined in the Animator as a float parameter)");

            parameterNameFluidField =
                FluidField.Get()
                    .SetLabelText("Parameter Name (float)")
                    .AddFieldContent(parameterNameTextField);
        }

        protected override void Compose()
        {
            base.Compose();
            root
                .AddChild(parameterNameFluidField)
                .AddChild(DesignUtils.spaceBlock2X);
        }
    }
}
