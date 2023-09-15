// Copyright (c) 2015 - 2021 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

//.........................
//.....Generated Class.....
//.........................
//.......Do not edit.......
//.........................

using System.Diagnostics.CodeAnalysis;
using Doozy.Editor.EditorUI.ScriptableObjects.Styles;
using UnityEngine.UIElements;

namespace Doozy.Editor.EditorUI
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public static class EditorStyles
    {
        public static class Common
        {
            private static EditorDataStyleGroup s_styleGroup;
            private static EditorDataStyleGroup styleGroup =>
                s_styleGroup != null
                    ? s_styleGroup
                    : s_styleGroup = EditorDataStyleDatabase.GetStyleGroup("Common");

            public static StyleSheet GetStyleSheet(StyleName styleName) =>
                styleGroup.GetStyleSheet(styleName.ToString());

            public enum StyleName
            {
                CategoryNameItemCategoryRow,
                CategoryNameItemNameRow
            }
            
            private static StyleSheet s_CategoryNameItemCategoryRow;
            public static StyleSheet CategoryNameItemCategoryRow => s_CategoryNameItemCategoryRow ? s_CategoryNameItemCategoryRow : s_CategoryNameItemCategoryRow = GetStyleSheet(StyleName.CategoryNameItemCategoryRow);
            private static StyleSheet s_CategoryNameItemNameRow;
            public static StyleSheet CategoryNameItemNameRow => s_CategoryNameItemNameRow ? s_CategoryNameItemNameRow : s_CategoryNameItemNameRow = GetStyleSheet(StyleName.CategoryNameItemNameRow);            
        }


        public static class EditorUI
        {
            private static EditorDataStyleGroup s_styleGroup;
            private static EditorDataStyleGroup styleGroup =>
                s_styleGroup != null
                    ? s_styleGroup
                    : s_styleGroup = EditorDataStyleDatabase.GetStyleGroup("EditorUI");

            public static StyleSheet GetStyleSheet(StyleName styleName) =>
                styleGroup.GetStyleSheet(styleName.ToString());

            public enum StyleName
            {
                ComponentField,
                EditorColorInfo,
                EditorMicroAnimationInfo,
                EditorSelectableColorInfo,
                EditorTextureInfo,
                EditorUIWindow,
                FieldIcon,
                FieldInfoLabel,
                FieldName,
                FluidAnimatedContainer,
                FluidButton,
                FluidCircularGauge,
                FluidComponentHeader,
                FluidContainer,
                FluidField,
                FluidFoldout,
                FluidListView,
                FluidListViewItem,
                FluidPlaceholder,
                FluidSearchBox,
                FluidSideMenu,
                FluidSystemWindow,
                FluidToggle,
                FluidToggleButton,
                FluidToggleIconButton,
                FluidWindowLayout,
                LayoutContainer,
                ListView,
                ListViewItem,
                ThemeColor,
                WindowHeader
            }
            
            private static StyleSheet s_ComponentField;
            public static StyleSheet ComponentField => s_ComponentField ? s_ComponentField : s_ComponentField = GetStyleSheet(StyleName.ComponentField);
            private static StyleSheet s_EditorColorInfo;
            public static StyleSheet EditorColorInfo => s_EditorColorInfo ? s_EditorColorInfo : s_EditorColorInfo = GetStyleSheet(StyleName.EditorColorInfo);
            private static StyleSheet s_EditorMicroAnimationInfo;
            public static StyleSheet EditorMicroAnimationInfo => s_EditorMicroAnimationInfo ? s_EditorMicroAnimationInfo : s_EditorMicroAnimationInfo = GetStyleSheet(StyleName.EditorMicroAnimationInfo);
            private static StyleSheet s_EditorSelectableColorInfo;
            public static StyleSheet EditorSelectableColorInfo => s_EditorSelectableColorInfo ? s_EditorSelectableColorInfo : s_EditorSelectableColorInfo = GetStyleSheet(StyleName.EditorSelectableColorInfo);
            private static StyleSheet s_EditorTextureInfo;
            public static StyleSheet EditorTextureInfo => s_EditorTextureInfo ? s_EditorTextureInfo : s_EditorTextureInfo = GetStyleSheet(StyleName.EditorTextureInfo);
            private static StyleSheet s_EditorUIWindow;
            public static StyleSheet EditorUIWindow => s_EditorUIWindow ? s_EditorUIWindow : s_EditorUIWindow = GetStyleSheet(StyleName.EditorUIWindow);
            private static StyleSheet s_FieldIcon;
            public static StyleSheet FieldIcon => s_FieldIcon ? s_FieldIcon : s_FieldIcon = GetStyleSheet(StyleName.FieldIcon);
            private static StyleSheet s_FieldInfoLabel;
            public static StyleSheet FieldInfoLabel => s_FieldInfoLabel ? s_FieldInfoLabel : s_FieldInfoLabel = GetStyleSheet(StyleName.FieldInfoLabel);
            private static StyleSheet s_FieldName;
            public static StyleSheet FieldName => s_FieldName ? s_FieldName : s_FieldName = GetStyleSheet(StyleName.FieldName);
            private static StyleSheet s_FluidAnimatedContainer;
            public static StyleSheet FluidAnimatedContainer => s_FluidAnimatedContainer ? s_FluidAnimatedContainer : s_FluidAnimatedContainer = GetStyleSheet(StyleName.FluidAnimatedContainer);
            private static StyleSheet s_FluidButton;
            public static StyleSheet FluidButton => s_FluidButton ? s_FluidButton : s_FluidButton = GetStyleSheet(StyleName.FluidButton);
            private static StyleSheet s_FluidCircularGauge;
            public static StyleSheet FluidCircularGauge => s_FluidCircularGauge ? s_FluidCircularGauge : s_FluidCircularGauge = GetStyleSheet(StyleName.FluidCircularGauge);
            private static StyleSheet s_FluidComponentHeader;
            public static StyleSheet FluidComponentHeader => s_FluidComponentHeader ? s_FluidComponentHeader : s_FluidComponentHeader = GetStyleSheet(StyleName.FluidComponentHeader);
            private static StyleSheet s_FluidContainer;
            public static StyleSheet FluidContainer => s_FluidContainer ? s_FluidContainer : s_FluidContainer = GetStyleSheet(StyleName.FluidContainer);
            private static StyleSheet s_FluidField;
            public static StyleSheet FluidField => s_FluidField ? s_FluidField : s_FluidField = GetStyleSheet(StyleName.FluidField);
            private static StyleSheet s_FluidFoldout;
            public static StyleSheet FluidFoldout => s_FluidFoldout ? s_FluidFoldout : s_FluidFoldout = GetStyleSheet(StyleName.FluidFoldout);
            private static StyleSheet s_FluidListView;
            public static StyleSheet FluidListView => s_FluidListView ? s_FluidListView : s_FluidListView = GetStyleSheet(StyleName.FluidListView);
            private static StyleSheet s_FluidListViewItem;
            public static StyleSheet FluidListViewItem => s_FluidListViewItem ? s_FluidListViewItem : s_FluidListViewItem = GetStyleSheet(StyleName.FluidListViewItem);
            private static StyleSheet s_FluidPlaceholder;
            public static StyleSheet FluidPlaceholder => s_FluidPlaceholder ? s_FluidPlaceholder : s_FluidPlaceholder = GetStyleSheet(StyleName.FluidPlaceholder);
            private static StyleSheet s_FluidSearchBox;
            public static StyleSheet FluidSearchBox => s_FluidSearchBox ? s_FluidSearchBox : s_FluidSearchBox = GetStyleSheet(StyleName.FluidSearchBox);
            private static StyleSheet s_FluidSideMenu;
            public static StyleSheet FluidSideMenu => s_FluidSideMenu ? s_FluidSideMenu : s_FluidSideMenu = GetStyleSheet(StyleName.FluidSideMenu);
            private static StyleSheet s_FluidSystemWindow;
            public static StyleSheet FluidSystemWindow => s_FluidSystemWindow ? s_FluidSystemWindow : s_FluidSystemWindow = GetStyleSheet(StyleName.FluidSystemWindow);
            private static StyleSheet s_FluidToggle;
            public static StyleSheet FluidToggle => s_FluidToggle ? s_FluidToggle : s_FluidToggle = GetStyleSheet(StyleName.FluidToggle);
            private static StyleSheet s_FluidToggleButton;
            public static StyleSheet FluidToggleButton => s_FluidToggleButton ? s_FluidToggleButton : s_FluidToggleButton = GetStyleSheet(StyleName.FluidToggleButton);
            private static StyleSheet s_FluidToggleIconButton;
            public static StyleSheet FluidToggleIconButton => s_FluidToggleIconButton ? s_FluidToggleIconButton : s_FluidToggleIconButton = GetStyleSheet(StyleName.FluidToggleIconButton);
            private static StyleSheet s_FluidWindowLayout;
            public static StyleSheet FluidWindowLayout => s_FluidWindowLayout ? s_FluidWindowLayout : s_FluidWindowLayout = GetStyleSheet(StyleName.FluidWindowLayout);
            private static StyleSheet s_LayoutContainer;
            public static StyleSheet LayoutContainer => s_LayoutContainer ? s_LayoutContainer : s_LayoutContainer = GetStyleSheet(StyleName.LayoutContainer);
            private static StyleSheet s_ListView;
            public static StyleSheet ListView => s_ListView ? s_ListView : s_ListView = GetStyleSheet(StyleName.ListView);
            private static StyleSheet s_ListViewItem;
            public static StyleSheet ListViewItem => s_ListViewItem ? s_ListViewItem : s_ListViewItem = GetStyleSheet(StyleName.ListViewItem);
            private static StyleSheet s_ThemeColor;
            public static StyleSheet ThemeColor => s_ThemeColor ? s_ThemeColor : s_ThemeColor = GetStyleSheet(StyleName.ThemeColor);
            private static StyleSheet s_WindowHeader;
            public static StyleSheet WindowHeader => s_WindowHeader ? s_WindowHeader : s_WindowHeader = GetStyleSheet(StyleName.WindowHeader);            
        }


        public static class Mody
        {
            private static EditorDataStyleGroup s_styleGroup;
            private static EditorDataStyleGroup styleGroup =>
                s_styleGroup != null
                    ? s_styleGroup
                    : s_styleGroup = EditorDataStyleDatabase.GetStyleGroup("Mody");

            public static StyleSheet GetStyleSheet(StyleName styleName) =>
                styleGroup.GetStyleSheet(styleName.ToString());

            public enum StyleName
            {
                ModyActionRunner,
                ModyStateIndicator
            }
            
            private static StyleSheet s_ModyActionRunner;
            public static StyleSheet ModyActionRunner => s_ModyActionRunner ? s_ModyActionRunner : s_ModyActionRunner = GetStyleSheet(StyleName.ModyActionRunner);
            private static StyleSheet s_ModyStateIndicator;
            public static StyleSheet ModyStateIndicator => s_ModyStateIndicator ? s_ModyStateIndicator : s_ModyStateIndicator = GetStyleSheet(StyleName.ModyStateIndicator);            
        }


        public static class Nody
        {
            private static EditorDataStyleGroup s_styleGroup;
            private static EditorDataStyleGroup styleGroup =>
                s_styleGroup != null
                    ? s_styleGroup
                    : s_styleGroup = EditorDataStyleDatabase.GetStyleGroup("Nody");

            public static StyleSheet GetStyleSheet(StyleName styleName) =>
                styleGroup.GetStyleSheet(styleName.ToString());

            public enum StyleName
            {
                FlowPort,
                NodeView,
                NodyInspectorWindow,
                NodyWindow,
                PivotNode
            }
            
            private static StyleSheet s_FlowPort;
            public static StyleSheet FlowPort => s_FlowPort ? s_FlowPort : s_FlowPort = GetStyleSheet(StyleName.FlowPort);
            private static StyleSheet s_NodeView;
            public static StyleSheet NodeView => s_NodeView ? s_NodeView : s_NodeView = GetStyleSheet(StyleName.NodeView);
            private static StyleSheet s_NodyInspectorWindow;
            public static StyleSheet NodyInspectorWindow => s_NodyInspectorWindow ? s_NodyInspectorWindow : s_NodyInspectorWindow = GetStyleSheet(StyleName.NodyInspectorWindow);
            private static StyleSheet s_NodyWindow;
            public static StyleSheet NodyWindow => s_NodyWindow ? s_NodyWindow : s_NodyWindow = GetStyleSheet(StyleName.NodyWindow);
            private static StyleSheet s_PivotNode;
            public static StyleSheet PivotNode => s_PivotNode ? s_PivotNode : s_PivotNode = GetStyleSheet(StyleName.PivotNode);            
        }


        public static class Reactor
        {
            private static EditorDataStyleGroup s_styleGroup;
            private static EditorDataStyleGroup styleGroup =>
                s_styleGroup != null
                    ? s_styleGroup
                    : s_styleGroup = EditorDataStyleDatabase.GetStyleGroup("Reactor");

            public static StyleSheet GetStyleSheet(StyleName styleName) =>
                styleGroup.GetStyleSheet(styleName.ToString());

            public enum StyleName
            {
                AnimationInfo,
                AnimationPreview,
                ReactionControls,
                ReactorWindow,
                TickerVisualizer
            }
            
            private static StyleSheet s_AnimationInfo;
            public static StyleSheet AnimationInfo => s_AnimationInfo ? s_AnimationInfo : s_AnimationInfo = GetStyleSheet(StyleName.AnimationInfo);
            private static StyleSheet s_AnimationPreview;
            public static StyleSheet AnimationPreview => s_AnimationPreview ? s_AnimationPreview : s_AnimationPreview = GetStyleSheet(StyleName.AnimationPreview);
            private static StyleSheet s_ReactionControls;
            public static StyleSheet ReactionControls => s_ReactionControls ? s_ReactionControls : s_ReactionControls = GetStyleSheet(StyleName.ReactionControls);
            private static StyleSheet s_ReactorWindow;
            public static StyleSheet ReactorWindow => s_ReactorWindow ? s_ReactorWindow : s_ReactorWindow = GetStyleSheet(StyleName.ReactorWindow);
            private static StyleSheet s_TickerVisualizer;
            public static StyleSheet TickerVisualizer => s_TickerVisualizer ? s_TickerVisualizer : s_TickerVisualizer = GetStyleSheet(StyleName.TickerVisualizer);            
        }


        public static class Signals
        {
            private static EditorDataStyleGroup s_styleGroup;
            private static EditorDataStyleGroup styleGroup =>
                s_styleGroup != null
                    ? s_styleGroup
                    : s_styleGroup = EditorDataStyleDatabase.GetStyleGroup("Signals");

            public static StyleSheet GetStyleSheet(StyleName styleName) =>
                styleGroup.GetStyleSheet(styleName.ToString());

            public enum StyleName
            {
                ProvidersConsoleRow,
                SignalsConsoleRow,
                SignalsWindow,
                StreamsConsoleRow
            }
            
            private static StyleSheet s_ProvidersConsoleRow;
            public static StyleSheet ProvidersConsoleRow => s_ProvidersConsoleRow ? s_ProvidersConsoleRow : s_ProvidersConsoleRow = GetStyleSheet(StyleName.ProvidersConsoleRow);
            private static StyleSheet s_SignalsConsoleRow;
            public static StyleSheet SignalsConsoleRow => s_SignalsConsoleRow ? s_SignalsConsoleRow : s_SignalsConsoleRow = GetStyleSheet(StyleName.SignalsConsoleRow);
            private static StyleSheet s_SignalsWindow;
            public static StyleSheet SignalsWindow => s_SignalsWindow ? s_SignalsWindow : s_SignalsWindow = GetStyleSheet(StyleName.SignalsWindow);
            private static StyleSheet s_StreamsConsoleRow;
            public static StyleSheet StreamsConsoleRow => s_StreamsConsoleRow ? s_StreamsConsoleRow : s_StreamsConsoleRow = GetStyleSheet(StyleName.StreamsConsoleRow);            
        }


        public static class UIManager
        {
            private static EditorDataStyleGroup s_styleGroup;
            private static EditorDataStyleGroup styleGroup =>
                s_styleGroup != null
                    ? s_styleGroup
                    : s_styleGroup = EditorDataStyleDatabase.GetStyleGroup("UIManager");

            public static StyleSheet GetStyleSheet(StyleName styleName) =>
                styleGroup.GetStyleSheet(styleName.ToString());

            public enum StyleName
            {
                UIManagerWindow,
                UIMenuItemButton
            }
            
            private static StyleSheet s_UIManagerWindow;
            public static StyleSheet UIManagerWindow => s_UIManagerWindow ? s_UIManagerWindow : s_UIManagerWindow = GetStyleSheet(StyleName.UIManagerWindow);
            private static StyleSheet s_UIMenuItemButton;
            public static StyleSheet UIMenuItemButton => s_UIMenuItemButton ? s_UIMenuItemButton : s_UIMenuItemButton = GetStyleSheet(StyleName.UIMenuItemButton);            
        }

    }
}
