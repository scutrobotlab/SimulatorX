// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Runtime.UIElements.Extensions;
using UnityEngine.UIElements;

namespace Doozy.Editor.EditorUI.Fields
{
    public class FieldInfoLabel : Label
    {
        public FieldInfoLabel() : this(string.Empty) {}

        public FieldInfoLabel(string text)
        {
            this.AddClass(nameof(FieldInfoLabel))
                .AddStyle(EditorStyles.EditorUI.FieldInfoLabel)
                .SetPickingMode(PickingMode.Ignore)
                .SetStyleColor(EditorColors.Default.TextDescription)
                .SetStyleUnityFont(EditorFonts.Ubuntu.Light)
                .SetText(text);
        }
    }
}
