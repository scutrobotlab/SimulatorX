// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Editor.Enums;
using UnityEngine;

namespace Doozy.Editor.EditorUI.ScriptableObjects.Fonts
{
    [Serializable]
    public class EditorFontInfo
    {
        public Font FontReference;
        public GenericFontWeight Weight;

        public EditorFontInfo()
        {
            FontReference = null;
            Weight = GenericFontWeight.Regular;
        }
    }
}
