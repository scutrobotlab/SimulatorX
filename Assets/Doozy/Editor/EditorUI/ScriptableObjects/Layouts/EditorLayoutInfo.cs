// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using UnityEngine.UIElements;

namespace Doozy.Editor.EditorUI.ScriptableObjects.Layouts
{
    [Serializable]
    public class EditorLayoutInfo
    {
        public VisualTreeAsset UxmlReference;

        public EditorLayoutInfo()
        {
            UxmlReference = null;
        }
    }
}
