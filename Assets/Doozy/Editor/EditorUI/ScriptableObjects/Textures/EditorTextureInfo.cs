// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Runtime.Common.Extensions;
using UnityEngine;

namespace Doozy.Editor.EditorUI.ScriptableObjects.Textures
{
    [Serializable]
    public class EditorTextureInfo
    {
        public string TextureName;
        public Texture2D TextureReference;
        
        public void ValidateName() =>
            TextureName = TextureName.RemoveWhitespaces().RemoveAllSpecialCharacters();
    }
}
