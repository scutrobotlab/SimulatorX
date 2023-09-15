// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using Doozy.Editor.EditorUI.ScriptableObjects.Colors;
using UnityEngine;

namespace Doozy.Editor.EditorUI
{
    public interface IWindowLayout
    {
        string layoutName { get; }
        Texture2D staticIconTexture { get; }
        List<Texture2D> animatedIconTextures { get; }
        Color accentColor { get; }
        EditorSelectableColorInfo selectableAccentColor { get; }
    }
}
