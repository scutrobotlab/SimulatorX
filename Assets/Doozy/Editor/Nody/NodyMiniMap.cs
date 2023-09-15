// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Reflection;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Runtime.UIElements.Extensions;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Editor.Nody
{
    public class NodyMiniMap : MiniMap
    {
        public static string editorPrefsKeyIsVisible => $"{nameof(NodyMiniMap)}_{nameof(isVisible)}";
        public static bool GetPrefs_IsVisible() => EditorPrefs.GetBool(editorPrefsKeyIsVisible, true);
        public static void SetPrefs_IsVisible(bool value) => EditorPrefs.SetBool(editorPrefsKeyIsVisible, value);
        
        private bool m_IsVisible;
        /// <summary> MiniMap visibility </summary>
        public bool isVisible
        {
            get => m_IsVisible;
            set
            {
                m_IsVisible = value;
                this.SetStyleDisplay(value ? DisplayStyle.Flex : DisplayStyle.None);
                SetPrefs_IsVisible(value);
            }
        }
        
        public NodyMiniMap()
        {
            m_IsVisible = GetPrefs_IsVisible();
            
            anchored = true;

            this
                .SetStyleDisplay(DisplayStyle.None)
                .SetStyleBorderRadius(DesignUtils.k_Spacing2X)
                .SetStyleBackgroundColor(EditorColors.Nody.MiniMapBackground);

            typeof(MiniMap).GetField("m_ViewportColor", BindingFlags.NonPublic | BindingFlags.Instance)?
                .SetValue(this, EditorColors.Nody.Color);

            typeof(MiniMap).GetField("m_SelectedChildrenColor", BindingFlags.NonPublic | BindingFlags.Instance)?
                .SetValue(this, EditorColors.Nody.Selection);

            typeof(MiniMap).GetField("m_PlacematBorderColor", BindingFlags.NonPublic | BindingFlags.Instance)?
                .SetValue(this, EditorColors.Nody.NodeIcon);

            this.Q<Label>()
                .SetStyleUnityFont(DesignUtils.fieldNameTextFont)
                .SetStyleFontSize(DesignUtils.k_FieldLabelFontSize)
                .SetStyleColor(DesignUtils.fieldNameTextColor);
        }

        /// <summary> Set MiniMap visibility </summary>
        /// <param name="value"> True is visible </param>
        public NodyMiniMap SetIsVisible(bool value = true)
        {
            isVisible = value;
            return this;
        }
    }
}
