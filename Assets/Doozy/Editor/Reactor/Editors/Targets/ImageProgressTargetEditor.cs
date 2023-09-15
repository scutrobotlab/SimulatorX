// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using Doozy.Editor.EditorUI;
using Doozy.Editor.UIElements;
using Doozy.Runtime.Reactor.Targets.ProgressTargets;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
namespace Doozy.Editor.Reactor.Editors.Targets
{
    [CustomEditor(typeof(ImageProgressTarget), true)]
    public sealed class ImageProgressTargetEditor : ProgressTargetEditor
    {
        public override IEnumerable<Texture2D> targetIconTextures => EditorMicroAnimations.Reactor.Icons.ImageProgressTarget;

        protected override void InitializeEditor()
        {
            base.InitializeEditor();
            
            componentHeader
                .SetComponentNameText(ObjectNames.NicifyVariableName(nameof(Image)))
                .AddManualButton()
                .AddYouTubeButton();

            targetObjectField
                .SetObjectType(typeof(Image));
        }        
    }
}
