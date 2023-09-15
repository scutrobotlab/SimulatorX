// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections;
using System.Collections.Generic;
using Doozy.Editor.EditorUI.ScriptableObjects.Colors;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Editor.Reactor.Internal;
using Doozy.Runtime.Common.Extensions;
using Doozy.Runtime.Reactor.Easings;
using Doozy.Runtime.Reactor.Internal;
using Doozy.Runtime.Reactor.Reactions;
using Doozy.Runtime.UIElements.Extensions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using PlayMode = Doozy.Runtime.Reactor.PlayMode;
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Editor.EditorUI.Components
{
    public class FluidListView : VisualElement, IDisposable
    {
        public void Dispose()
        {
            m_AddItemButton?.Recycle();

            emptyListPlaceholder?.RemoveManipulator(emptyListPlaceholderClickable);
            emptyListPlaceholder?.Recycle();

            emptySearchPlaceholder?.RemoveManipulator(emptySearchPlaceholderClickable);
            emptySearchPlaceholder?.Recycle();
        }

        private const int REFRESH_INTERVAL = 100;
        private const int LIST_MINIMUM_HEIGHT = 120;
        private const int LIST_ITEM_HEIGHT = 20;
        private const float PLACEHOLDER_ANIMATION_DURATION = 1f;

        //ACTIONS
        public UnityAction AddNewItemButtonCallback;

        //REFERENCES
        public TemplateContainer templateContainer { get; }
        public VisualElement layoutContainer { get; }
        public VisualElement headerContainer { get; }
        public Label listTitle { get; }
        public Label listDescription { get; }
        public VisualElement toolbarContainer { get; }
        public VisualElement listContainer { get; }
        public VisualElement listHeaderContainer { get; }
        public VisualElement columnNamesContainer { get; }
        public VisualElement addItemButtonContainer { get; }
        public VisualElement listViewContainer { get; }
        public ListView listView { get; }
        public VisualElement footerContainer { get; }
        public Label footerLabel { get; }

        private Scroller m_ListViewVerticalScroller;
        private Scroller listViewVerticalScroller =>
            m_ListViewVerticalScroller ??= listView.Q<Scroller>(null, Scroller.verticalVariantUssClassName);

        public List<VisualElement> toolbarElements { get; }

        public int listViewItemsCount => listView.itemsSource?.Count ?? 0;
        public bool listViewIsEmpty => (listView.itemsSource?.Count ?? 0) == 0;

        //SEARCH
        public bool inSearchMode => hasSearch && searchBox.isSearching;
        public FluidSearchBox searchBox { get; private set; }
        public bool hasSearch { get; private set; }
        private FloatReaction m_SearchBoxMinWidthAnimation;
        private const float SEARCH_BOX_MIN_WIDTH = 160f;
        private float searchBoxMinWidthWhenSearching =>
            toolbarContainer.resolvedStyle.width
            - toolbarContainer.resolvedStyle.paddingLeft * 2
            - toolbarContainer.resolvedStyle.paddingRight * 2;

        //BUTTONS
        private FluidButton m_AddItemButton;
        public FluidButton addItemButton => m_AddItemButton ??= Buttons.addButton;

        //EMPTY STATE
        public bool hasToolbar { get; internal set; }
        private bool hideFooterWhenEmpty { get; set; }

        private static IEnumerable<Texture2D> emptyListPlaceholderTextures => EditorMicroAnimations.EditorUI.Placeholders.EmptyListView;
        private static IEnumerable<Texture2D> emptyListSmallPlaceholderTextures => EditorMicroAnimations.EditorUI.Placeholders.EmptySmall;
        private static IEnumerable<Texture2D> emptySearchPlaceholderTextures => EditorMicroAnimations.EditorUI.Placeholders.EmptySearch;

        private bool showEmptyListPlaceholder { get; set; } = true;
        private bool useSmallEmptyListPlaceholder { get; set; } = false;
        public FluidPlaceholder emptyListPlaceholder { get; }
        private Clickable emptyListPlaceholderClickable { get; }

        public FluidPlaceholder emptySearchPlaceholder { get; }
        private Clickable emptySearchPlaceholderClickable { get; }

        //SETTINGS
        public bool hideFooter { get; private set; }
        public bool showItemIndex { get; private set; }
        public int preferredListHeight { get; private set; }
        public bool hideToolbarWhileSearching { get; private set; }
        public bool addNewItemButtonIsHidden { get; private set; }

        //SELECTABLE COLORS
        private static EditorSelectableColorInfo actionSelectableColor => EditorSelectableColors.Default.Action;
        private static EditorSelectableColorInfo addSelectableColor => EditorSelectableColors.Default.Add;
        private static EditorSelectableColorInfo removeSelectableColor => EditorSelectableColors.Default.Remove;

        //COLORS
        private static Color listNameTextColor => EditorColors.Default.TextTitle;
        private static Color listDescriptionTextColor => EditorColors.Default.TextDescription;
        private static Color footerTextColor => EditorColors.Default.TextDescription;
        private static Color backgroundColor => EditorColors.Default.Background;
        private static Color placeholderColor => EditorColors.Default.Placeholder;

        //FONTS
        private static Font listNameFont => EditorFonts.Ubuntu.Light;
        private static Font listDescriptionFont => EditorFonts.Inter.Light;
        private static Font footerLabelFont => EditorFonts.Inter.Regular;

        public FluidListView SetItemsSource(IList list)
        {
            listView.itemsSource = list;
            return this;
        }

        public FluidListView()
        {
            Add(templateContainer = EditorLayouts.EditorUI.FluidListView.CloneTree());
            templateContainer
                .SetStyleFlexGrow(1)
                .AddStyle(EditorStyles.EditorUI.FluidListView);

            //REFERENCES
            layoutContainer = templateContainer.Q<VisualElement>(nameof(layoutContainer));
            headerContainer = layoutContainer.Q<VisualElement>(nameof(headerContainer));
            listTitle = headerContainer.Q<Label>(nameof(listTitle));
            listDescription = headerContainer.Q<Label>(nameof(listDescription));
            toolbarContainer = layoutContainer.Q<VisualElement>(nameof(toolbarContainer));
            listContainer = layoutContainer.Q<VisualElement>(nameof(listContainer));
            listHeaderContainer = listContainer.Q<VisualElement>(nameof(listHeaderContainer));
            columnNamesContainer = listHeaderContainer.Q<VisualElement>(nameof(columnNamesContainer));
            addItemButtonContainer = listHeaderContainer.Q<VisualElement>(nameof(addItemButtonContainer));
            listViewContainer = listContainer.Q<VisualElement>(nameof(listViewContainer));
            listView = listContainer.Q<ListView>(nameof(listView));
            footerContainer = templateContainer.Q<VisualElement>(nameof(footerContainer));
            footerLabel = footerContainer.Q<Label>(nameof(footerLabel));

            //TOOLBAR
            toolbarElements = new List<VisualElement>();

            //SEARCH
            searchBox = new FluidSearchBox()
                .SetStyleHeight(26)
                .SetStyleMinWidth(SEARCH_BOX_MIN_WIDTH)
                .SetStyleMarginRight(2);

            //BACKGROUND COLORS
            layoutContainer.Q<VisualElement>("top").SetStyleBackgroundColor(backgroundColor);
            layoutContainer.Q<VisualElement>("bottom").SetStyleBackgroundColor(backgroundColor);
            toolbarContainer.SetStyleBackgroundColor(backgroundColor);
            listContainer.SetStyleBackgroundColor(backgroundColor);
            footerContainer.SetStyleBackgroundColor(backgroundColor);

            //TEXT COLORS
            listTitle.SetStyleColor(listNameTextColor);
            listDescription.SetStyleColor(listDescriptionTextColor);
            footerLabel.SetStyleColor(footerTextColor);

            //TEXT FONTS
            listTitle.SetStyleUnityFont(listNameFont);
            listDescription.SetStyleUnityFont(listDescriptionFont);
            footerLabel.SetStyleUnityFont(footerLabelFont);

            //INJECTIONS
            addNewItemButtonIsHidden = false;
            addItemButtonContainer.Add(addItemButton);
            addItemButton.OnClick += () =>
            {
                if (inSearchMode) //do not add new items if in search mode 
                    return;
                AddNewItemButtonCallback?.Invoke();
            };

            //LIST VIEW
            preferredListHeight = LIST_MINIMUM_HEIGHT;
            
            #if UNITY_2021_2_OR_NEWER
            listView.fixedItemHeight = LIST_ITEM_HEIGHT;
            #else
            listView.itemHeight = LIST_ITEM_HEIGHT;
            #endif
            
            // listView.RegisterCallback<AttachToPanelEvent>(evt => Undo.undoRedoPerformed += Update);
            // listView.RegisterCallback<DetachFromPanelEvent>(evt => Undo.undoRedoPerformed -= Update);

            //SHOW INDEX LABELS by default
            ShowItemIndex(true);

            //ADD CALLBACK to scroll list view to top when adding a new item
            AddNewItemButtonCallback += () => schedule.Execute(() => listView.ScrollToItem(0));

            //PLACEHOLDER - Empty List
            emptyListPlaceholder = FluidPlaceholder.Get(emptyListPlaceholderTextures).Hide();
            listContainer.Add(emptyListPlaceholder);
            emptyListPlaceholderClickable = new Clickable(() =>
            {
                if (inSearchMode) return;
                AddNewItemButtonCallback?.Invoke(); //set placeholder on click to add a new item (if not in search mode)
            });
            emptyListPlaceholder.AddManipulator(emptyListPlaceholderClickable);


            //PLACEHOLDER - Empty Search
            emptySearchPlaceholder = FluidPlaceholder.Get(emptySearchPlaceholderTextures).Hide();
            listContainer.Add(emptySearchPlaceholder);
            emptySearchPlaceholderClickable = new Clickable(() =>
            {
                if (!hasSearch) return;
                searchBox.ClearSearch(); //set placeholder on click to clear search (if in search mode)
            });
            emptySearchPlaceholder.AddManipulator(emptySearchPlaceholderClickable);

            VisualUpdate();
        }

        public void Update()
        {
            DataUpdate();
            VisualUpdate();
        }

        private void DataUpdate()
        {
            #if UNITY_2021_2_OR_NEWER
            listView.Rebuild();
            #else
            listView.Refresh();
            #endif
        }

        #region Visual Update

        private void VisualUpdate()
        {
            MarkDirtyRepaint();
            VisualUpdate_ListViewHeight();
            VisualUpdate_Toolbar();
            VisualUpdate_FooterText();
            VisualUpdate_UpdateAddItemButton();
            VisualUpdate_EmptyState();
            VisualUpdate_UpdateSearchBoxWidth();
        }

        private void VisualUpdate_Toolbar()
        {
            toolbarContainer.SetStyleDisplay
            (
                !hasToolbar || inSearchMode & hideToolbarWhileSearching
                    ? DisplayStyle.None
                    : DisplayStyle.Flex
            );

            if (!hasToolbar) return;

            if (listViewIsEmpty)
            {
                if (inSearchMode == false) //do not disable toolbar if in search mode
                    toolbarContainer.DisableElement();
            }
            else
            {
                toolbarContainer.EnableElement();
            }
        }

        private void VisualUpdate_ListViewHeight()
        {
            #if UNITY_2021_2_OR_NEWER
            int listHeight = (int)Mathf.Max(listView.fixedItemHeight * listViewItemsCount, LIST_ITEM_HEIGHT);
            #else
            int listHeight = Mathf.Max(listView.itemHeight * listViewItemsCount, LIST_ITEM_HEIGHT);
            #endif

            int dynamicHeight = (int)listViewContainer.resolvedStyle.height;
            int calculatedHeight =
                listViewItemsCount == 0
                    ? LIST_MINIMUM_HEIGHT
                    : Mathf.Min(listHeight, hasDynamicHeight ? dynamicHeight : preferredListHeight);

            listView.SetStyleHeight(calculatedHeight);
            listView.MarkDirtyRepaint();
        }

        private void VisualUpdate_FooterText()
        {
            //update footer text to number of items
            SetFooterLabelText($"{listView.itemsSource?.Count ?? 0} items{(inSearchMode ? " found" : "")}");
        }

        private void VisualUpdate_UpdateAddItemButton()
        {
            //hide (plus) button when in search mode
            // addItemButton.SetStyleDisplay(inSearchMode ? DisplayStyle.None : DisplayStyle.Flex);
            if (inSearchMode)
            {
                addItemButton.DisableElement();
                addItemButton.visible = false;
            }
            else
            {
                addItemButton.EnableElement();
                addItemButton.visible = true;
            }

            //update the (plus) button position if the vertical scroller is visible
            addItemButton.schedule.Execute
                (
                    () => addItemButton.SetStyleMarginRight(listViewVerticalScroller.visible ? 12 : 0)
                )
                .ExecuteLater(10); //add a small delay for safety
        }

        private void VisualUpdate_EmptyState()
        {
            listView.SetStyleDisplay(listViewIsEmpty ? DisplayStyle.None : DisplayStyle.Flex);

            listViewContainer.SetStyleDisplay(listViewIsEmpty & useSmallEmptyListPlaceholder ? DisplayStyle.None : DisplayStyle.Flex);

            footerContainer.SetStyleDisplay(hideFooter ? DisplayStyle.None : DisplayStyle.Flex);
            if (hideFooter == false & hideFooterWhenEmpty)
                footerContainer.SetStyleDisplay(listViewIsEmpty ? DisplayStyle.None : DisplayStyle.Flex);

            emptySearchPlaceholder.Toggle(listViewIsEmpty & inSearchMode);
            emptyListPlaceholder.Toggle(showEmptyListPlaceholder && listViewIsEmpty & !inSearchMode);
        }

        private void VisualUpdate_UpdateSearchBoxWidth()
        {
            if (!hasSearch) return;
            schedule.Execute(() => m_SearchBoxMinWidthAnimation
                .PlayToValue(searchBox.isSearching ? searchBoxMinWidthWhenSearching : SEARCH_BOX_MIN_WIDTH));
        }

        #endregion

        public FluidListView SetFooterLabelText(string text)
        {
            footerLabel.text = text;
            bool hasFooterText = !text.IsNullOrEmpty();
            footerLabel.SetStyleDisplay(hasFooterText ? DisplayStyle.Flex : DisplayStyle.None);
            return this;
        }

        public FluidListView SetListTitle(string text)
        {
            listTitle.text = text;

            bool hasListName = !text.IsNullOrEmpty();
            listTitle.SetStyleDisplay(hasListName ? DisplayStyle.Flex : DisplayStyle.None);
            if (hasListName) headerContainer.SetStyleDisplay(DisplayStyle.Flex);
            return this;
        }

        public FluidListView SetListDescription(string text)
        {
            listDescription.text = text;

            bool hasListDescription = !text.IsNullOrEmpty();
            listDescription.SetStyleDisplay(hasListDescription ? DisplayStyle.Flex : DisplayStyle.None);
            if (hasListDescription) headerContainer.SetStyleDisplay(DisplayStyle.Flex);
            return this;
        }

        public FluidListView SetItemHeight(int itemHeight)
        {
            #if UNITY_2021_2_OR_NEWER
            listView.fixedItemHeight = itemHeight;
            #else
            listView.itemHeight = itemHeight;
            #endif
            
            return this;
        }

        public FluidListView ShowItemIndex(bool show)
        {
            showItemIndex = show;
            
            #if UNITY_2021_2_OR_NEWER
            listView.Rebuild();
            #else
            listView.Refresh();
            #endif
            
            return this;
        }

        public FluidListView SetPreferredListHeight(int height)
        {
            preferredListHeight = Mathf.Max(LIST_MINIMUM_HEIGHT, height);
            VisualUpdate_ListViewHeight();
            return this;
        }

        private bool hasDynamicHeight { get; set; }

        public FluidListView SetDynamicListHeight(bool dynamicHeight)
        {
            schedule.Execute(UpdateHeight);
            schedule.Execute(UpdateHeight).ExecuteLater(50);
            UpdateHeight();

            void UpdateHeight()
            {
                hasDynamicHeight = dynamicHeight;
                this.SetStyleFlexGrow(dynamicHeight ? 1 : 0);

                if (dynamicHeight)
                {
                    listViewContainer.RegisterCallback<GeometryChangedEvent>(_ =>
                    {
                        listViewContainer.schedule.Execute(VisualUpdate_ListViewHeight);
                    });
                }
            }

            return this;
        }

        public FluidListView ShowAddNewItemButton()
        {
            addItemButtonContainer.SetStyleDisplay(DisplayStyle.Flex);
            addNewItemButtonIsHidden = true;
            return this;
        }

        public FluidListView HideAddNewItemButton()
        {
            addItemButtonContainer.SetStyleDisplay(DisplayStyle.None);
            addNewItemButtonIsHidden = false;
            return this;
        }

        public FluidListView AddToolbarElement(VisualElement element)
        {
            hasToolbar = true;
            toolbarContainer.SetStyleDisplay(DisplayStyle.Flex);
            toolbarContainer.Add(element);
            toolbarElements.Add(element);
            if (hasSearch) searchBox.BringToFront();
            return this;
        }

        private VisualElement flexibleSpaceBeforeSearchBox { get; set; }

        public FluidListView HasSearch(bool enabled)
        {
            if (hasSearch == enabled) return this;

            flexibleSpaceBeforeSearchBox ??= DesignUtils.flexibleSpace;

            hasSearch = enabled;
            if (hasSearch)
            {
                toolbarContainer.Add(flexibleSpaceBeforeSearchBox);
                toolbarContainer.Add(searchBox);
                searchBox.BringToFront();
                hasToolbar = true;
                toolbarContainer.SetStyleDisplay(DisplayStyle.Flex);

                toolbarContainer.RegisterCallback<GeometryChangedEvent>(_ => VisualUpdate_UpdateSearchBoxWidth());

                m_SearchBoxMinWidthAnimation =
                    Reaction.Get<FloatReaction>()
                        .SetEditorHeartbeat()
                        .SetDuration(0.25f)
                        .SetPlayMode(PlayMode.Normal)
                        .SetEase(Ease.OutCirc);

                m_SearchBoxMinWidthAnimation.setter = value => searchBox.SetStyleMinWidth(value);
                m_SearchBoxMinWidthAnimation.SetValue(SEARCH_BOX_MIN_WIDTH);

                searchBox.OnShowSearchResultsCallback += _ =>
                {
                    foreach (VisualElement toolbarElement in toolbarElements)
                        toolbarElement.SetStyleDisplay(inSearchMode ? DisplayStyle.None : DisplayStyle.Flex);

                    VisualUpdate_UpdateSearchBoxWidth();
                };
            }
            else //does not have search
            {
                toolbarContainer.Remove(flexibleSpaceBeforeSearchBox);
                toolbarContainer.Remove(searchBox);
                toolbarContainer.UnregisterCallback<GeometryChangedEvent>(_ => VisualUpdate_UpdateSearchBoxWidth());

                m_SearchBoxMinWidthAnimation.setter = null;
                m_SearchBoxMinWidthAnimation.Stop();
                m_SearchBoxMinWidthAnimation = null;
            }

            return this;
        }

        public FluidListView HideToolbarWhileSearching(bool hideWhileSearching)
        {
            hideToolbarWhileSearching = hideWhileSearching;
            return this;
        }

        public FluidListView ShowEmptyListPlaceholder(bool show)
        {
            showEmptyListPlaceholder = show;
            // emptyListPlaceholder.SetStyleDisplay(show ? DisplayStyle.Flex : DisplayStyle.None);
            return this;
        }

        public FluidListView UseSmallEmptyListPlaceholder(bool useSmallPlaceholder)
        {
            useSmallEmptyListPlaceholder = useSmallPlaceholder;

            emptyListPlaceholder
                .ResetLayout()
                .RemoveFromHierarchy();

            if (useSmallPlaceholder)
            {
                emptyListPlaceholder
                    .SetIcon(emptyListSmallPlaceholderTextures)
                    .SetStyleMarginLeft(DesignUtils.k_Spacing2X);

                listHeaderContainer
                    .Insert(0, emptyListPlaceholder);
                return this;
            }

            emptyListPlaceholder.SetIcon(emptyListPlaceholderTextures);
            listContainer.Add(emptyListPlaceholder);

            return this;
        }

        public FluidListView HideFooter(bool hide)
        {
            hideFooter = hide;
            return this;
        }

        public FluidListView HideFooterWhenEmpty(bool hideWhenEmpty)
        {
            hideFooterWhenEmpty = hideWhenEmpty;
            return this;
        }

        public static class Buttons
        {
            private const ElementSize SIZE = ElementSize.Small;
            private const ButtonStyle BUTTON_STYLE = ButtonStyle.Clear;
            private static EditorSelectableColorInfo accentColor => actionSelectableColor;

            internal static FluidButton GetNewToolbarButton(IEnumerable<Texture2D> textures, string tooltip = "") =>
                FluidButton.Get()
                    .SetIcon(textures)
                    .SetElementSize(SIZE)
                    .SetButtonStyle(BUTTON_STYLE)
                    .SetAccentColor(accentColor)
                    .SetTooltip(tooltip);

            public static FluidButton addButton => GetNewToolbarButton(EditorMicroAnimations.EditorUI.Icons.Plus, "Add Item").SetAccentColor(addSelectableColor);
            public static FluidButton removeButton => GetNewToolbarButton(EditorMicroAnimations.EditorUI.Icons.Minus, "Remove Item").SetAccentColor(removeSelectableColor);
            public static FluidButton clearButton => GetNewToolbarButton(EditorMicroAnimations.EditorUI.Icons.Clear, "Clear");
            public static FluidButton sortAzButton => GetNewToolbarButton(EditorMicroAnimations.EditorUI.Icons.SortAz, "Sort AZ");
            public static FluidButton sortZaButton => GetNewToolbarButton(EditorMicroAnimations.EditorUI.Icons.SortZa, "Sort ZA");
            public static FluidButton sortHueButton => GetNewToolbarButton(EditorMicroAnimations.EditorUI.Icons.SortHue, "Sort HUE");
            public static FluidButton saveButton => GetNewToolbarButton(EditorMicroAnimations.EditorUI.Icons.Save, "Save");
        }
    }
}
