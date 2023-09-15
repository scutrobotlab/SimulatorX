// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Editor.UIElements;
using Doozy.Runtime.Reactor.Targets;
using UnityEditor;
using UnityEngine.UI;

namespace Doozy.Editor.Reactor.Editors.Targets
{
    [CustomEditor(typeof(ImageColorTarget), true)]
    public sealed class ImageColorTargetEditor : ReactorColorTargetEditor
    {
        protected override void InitializeEditor()
        {
            base.InitializeEditor();
            
            componentHeader
                .SetComponentNameText(ObjectNames.NicifyVariableName(nameof(Image)))
                .AddManualButton("https://doozyentertainment.atlassian.net/wiki/spaces/DUI4/pages/1048969271/Image+Color+Target?atlOrigin=eyJpIjoiNjhlNWEyZjE2ZTVjNDY2OWJhZDJjYTFmNmZlOWUwOGQiLCJwIjoiYyJ9")
                .AddYouTubeButton();

            targetObjectField
                .SetObjectType(typeof(Image));
        }
    }
}
