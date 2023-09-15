// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Runtime.UIElements.Extensions;
using UnityEngine;
using UnityEngine.UIElements;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable MemberCanBeProtected.Global

namespace Doozy.Editor.EditorUI.Components.Internal
{
    public abstract class FluidListViewItem : VisualElement
    {
        //REFERENCES
        public TemplateContainer templateContainer { get; }
        public VisualElement layoutContainer { get; }
        public Label itemIndexLabel { get; }
        public VisualElement itemContentContainer { get; }
        public VisualElement itemRemoveButtonContainer { get; }

        //SETTINGS
        public FluidListView listView { get; internal set; }
        public int itemIndex { get; protected set; }
        public FluidButton itemRemoveButton { get; }

        public bool showItemIndex { get; protected set; }
        public bool showRemoveButton { get; protected set; }

        private static Color itemIndexTextColor => EditorColors.Default.TextDescription;

        private static Font indexLabelFont => EditorFonts.Ubuntu.Light;

        protected FluidListViewItem()
        {
            this.SetStyleJustifyContent(Justify.Center);
            
            Add(templateContainer = EditorLayouts.EditorUI.FluidListViewItem.CloneTree());
            templateContainer
                .AddStyle(EditorStyles.EditorUI.FluidListViewItem);

            //REFERENCES
            layoutContainer = templateContainer.Q<VisualElement>("LayoutContainer");
            itemIndexLabel = layoutContainer.Q<Label>("ItemIndex");
            itemContentContainer = layoutContainer.Q<VisualElement>("ItemContentContainer");
            itemRemoveButtonContainer = layoutContainer.Q<VisualElement>("ItemRemoveButtonContainer");

            //TEXT COLORS
            itemIndexLabel.SetStyleColor(itemIndexTextColor);
            
            //TEXT FONTS
            itemIndexLabel.SetStyleUnityFont(indexLabelFont);
        }
        
        protected FluidListViewItem(FluidListView listView) : this()
        {
            this.SetListView(listView);
            itemRemoveButtonContainer.Clear();
            itemRemoveButton = FluidListView.Buttons.removeButton;
            itemRemoveButtonContainer.Add(itemRemoveButton);
        }

        internal virtual void UpdateItemIndex(int value)
        {
            itemIndexLabel.SetStyleDisplay(showItemIndex ? DisplayStyle.Flex : DisplayStyle.None);
            value = Mathf.Max(0, value);
            itemIndex = value;
            itemIndexLabel.text = value.ToString();
        }
    }

    public static class FluidListViewItemExtensions
    {
        internal static T SetListView<T>(this T target, FluidListView value) where T : FluidListViewItem
        {
            target.listView = value;
            return target;
        }
        
        public static T SetItemIndex<T>(this T target, int value) where T : FluidListViewItem
        {
            target.UpdateItemIndex(value);
            return target;
        }

        public static T EnableItemRemoveButton<T>(this T target) where T : FluidListViewItem
        {
            target.itemRemoveButton.EnableElement();
            return target;
        }

        public static T DisableItemRemoveButton<T>(this T target) where T : FluidListViewItem
        {
            target.itemRemoveButton.DisableElement();
            return target;
        }

        public static T ToggleItemRemoveButton<T>(this T target, bool enabled) where T : FluidListViewItem
        {
            return enabled
                ? target.EnableItemRemoveButton()
                : target.DisableItemRemoveButton();
        }
    }
}
