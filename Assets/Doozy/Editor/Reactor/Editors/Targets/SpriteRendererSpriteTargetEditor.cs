// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Editor.UIElements;
using Doozy.Runtime.Reactor.Targets;
using UnityEditor;
using UnityEngine;

namespace Doozy.Editor.Reactor.Editors.Targets
{
    [CustomEditor(typeof(SpriteRendererSpriteTarget))]
    public class SpriteRendererSpriteTargetEditor : ReactorSpriteTargetEditor
    {
        protected override void InitializeEditor()
        {
            base.InitializeEditor();

            componentHeader
                .SetComponentNameText(ObjectNames.NicifyVariableName(nameof(SpriteRenderer)))
                .AddManualButton("https://doozyentertainment.atlassian.net/wiki/spaces/DUI4/pages/1046544484/SpriteRenderer+Sprite+Target?atlOrigin=eyJpIjoiYzNlYmUxM2NhOGM1NGU2MThlNTNiNTA2MGJkNjY1Y2YiLCJwIjoiYyJ9")
                .AddYouTubeButton();

            targetObjectField
                .SetObjectType(typeof(SpriteRenderer));
        }
    }
}
