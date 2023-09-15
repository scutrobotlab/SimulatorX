// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using Doozy.Editor.EditorUI.ScriptableObjects.Colors;
using Doozy.Editor.Reactor.Internal;
using Doozy.Runtime.Colors;
using Doozy.Runtime.Reactor.Easings;
using Doozy.Runtime.Reactor.Extensions;
using Doozy.Runtime.Reactor.Internal;
using Doozy.Runtime.Reactor.Reactions;
using Doozy.Runtime.UIElements.Extensions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace Doozy.Editor.EditorUI.Components
{
    public sealed class FluidSideMenu : VisualElement
    {
        public const float k_ExpandCollapseDuration = 0.3f;
        public const Ease k_ExpandCollapseEase = Ease.InOutExpo;
        
        #region MenuState

        private enum MenuState
        {
            Expanded,
            IsExpanding,
            Collapsed,
            IsCollapsing
        }

        private MenuState currentMenuState { get; set; }
        private bool areButtonLabelsHidden { get; set; }

        #endregion

        #region MenuLevel

        // ReSharper disable InconsistentNaming
        public enum MenuLevel
        {
            Level_0 = 0,
            Level_1 = 1,
            Level_2 = 2
        }
        // ReSharper restore InconsistentNaming

        public MenuLevel menuLevel { get; private set; }

        private Color MenuBackgroundColor()
        {
            switch (menuLevel)
            {
                case MenuLevel.Level_0: return EditorColors.Default.MenuBackgroundLevel0;
                case MenuLevel.Level_1: return EditorColors.Default.MenuBackgroundLevel1;
                case MenuLevel.Level_2: return EditorColors.Default.MenuBackgroundLevel2;
                default: throw new ArgumentOutOfRangeException();
            }
        }

        private EditorSelectableColorInfo ButtonContainerColor()
        {
            switch (menuLevel)
            {
                case MenuLevel.Level_0: return EditorSelectableColors.Default.MenuButtonBackgroundLevel0;
                case MenuLevel.Level_1: return EditorSelectableColors.Default.MenuButtonBackgroundLevel1;
                case MenuLevel.Level_2: return EditorSelectableColors.Default.MenuButtonBackgroundLevel2;
                default: throw new ArgumentOutOfRangeException();
            }
        }

        public FluidSideMenu SetMenuLevel(MenuLevel level)
        {
            menuLevel = level;
            UpdateColors();
            UpdateButtonSizes();
            return this;
        }

        private static ElementSize GetButtonSize(MenuLevel level)
        {
            switch (level)
            {
                case MenuLevel.Level_0: return ElementSize.Large;
                case MenuLevel.Level_1: return ElementSize.Normal;
                case MenuLevel.Level_2: return ElementSize.Small;
                default: throw new ArgumentOutOfRangeException();
            }
        }

        private void UpdateButtonSizes()
        {
            foreach (FluidToggleButtonTab button in buttons)
                button.SetElementSize(GetButtonSize(menuLevel));
        }

        #endregion

        //REFERENCES
        public TemplateContainer templateContainer { get; }
        public VisualElement layoutContainer { get; }
        public VisualElement headerContainer { get; }
        public VisualElement expandCollapseButtonContainer { get; }
        public VisualElement menuInfoContainer { get; }
        public Image menuInfoIcon { get; }
        public Label menuInfoLabel { get; }
        public VisualElement searchBoxContainer { get; }
        public VisualElement toolbarContainer { get; }
        public ScrollView buttonsScrollViewContainer { get; }
        public VisualElement footerContainer { get; }

        public Texture2DReaction menuInfoIconReaction { get; set; }

        public FluidToggleGroup toggleGroup { get; }
        public List<FluidToggleButtonTab> buttons { get; }

        //SETTINGS
        public int selectedMenuIndex { get; private set; }
        public bool isExpanded => currentMenuState == MenuState.Expanded || currentMenuState == MenuState.IsExpanding;
        public bool isCollapsed => currentMenuState == MenuState.Collapsed || currentMenuState == MenuState.IsCollapsing;
        public bool hideToolbarWhenCollapsed { get; private set; }
        public bool hasToolbar { get; private set; }
        public bool showMenuInfoWhenCollapsed { get; set; }

        #region Accent Color

        private EditorSelectableColorInfo m_AccentColor;

        public FluidSideMenu SetAccentColor(EditorSelectableColorInfo selectableColor)
        {
            if (selectableColor == null) return this;
            m_AccentColor = selectableColor;

            if (hasSearch)
            {
                searchBox.SetAccentColor(m_AccentColor);
            }

            if (isCollapsable)
            {
                expandButton.SetAccentColor(m_AccentColor);
                collapseButton.SetAccentColor(m_AccentColor);
            }
            return this;
        }

        #endregion

        #region Search

        public bool hasSearch => searchBox != null;
        public FluidSearchBox searchBox { get; private set; }

        public FluidSideMenu AddSearch()
        {
            //CLEAR SEARCH BOX CONTAINER
            searchBoxContainer.Clear();
            //DISPLAY SEARCH BOX CONTAINER
            searchBoxContainer.SetStyleDisplay(DisplayStyle.Flex);
            //CREATE a new SEARCH BOX and add it to the SEARCH BOX CONTAINER 
            searchBoxContainer.Add(searchBox = new FluidSearchBox());
            //SET SEARCH BOX ACCENT COLOR
            searchBox.SetAccentColor(m_AccentColor);

            //SET ALL THE BUTTONS TO CLEAR SEARCH WHEN CLICKED
            foreach (FluidToggleButtonTab button in buttons)
                button.OnClick += () => searchBox?.ClearSearch();

            //CONNECT SEARCH BOX TO SIDE MENU TOGGLE GROUP
            searchBox.ConnectToToggleGroup(toggleGroup);

            //ADD SEARCH TAB BUTTON TO THE BUTTONS CONTAINER (a tab to view search results)
            AddSearchButtonToButtonsContainer();

            //SET CALLBACK - that when a search is over to select the previously selected button
            searchBox.OnShowSearchResultsCallback += showResults =>
            {
                if (showResults == false && buttons.Count > 0)
                    buttons[selectedMenuIndex].isOn = true;
            };

            //SET CALLBACK - that when the collapse button is clicked to clear any ongoing search
            collapseButton.OnClick += searchBox.ClearSearch;

            return this;
        }

        public void SelectTheButtonThatWasSelectedBeforeSearchWasInitiated()
        {
            if (buttons.Count < 1)
                return;
            buttons[selectedMenuIndex].isOn = true;
        }

        public FluidSideMenu RemoveSearch()
        {
            searchBoxContainer.Clear();
            searchBoxContainer.SetStyleDisplay(DisplayStyle.None);
            searchBox?.OnShowSearchResultsCallback?.Invoke(false);
            searchBox?.DisconnectFromToggleGroup();
            if (buttons.Contains(searchBox?.searchTabButton))
                buttons.Remove(searchBox?.searchTabButton);
            RemoveSearchButtonFromButtonsContainer();
            if (searchBox != null)
                collapseButton.OnClick -= searchBox.ClearSearch;
            searchBox = null;
            return this;
        }

        #endregion

        #region Expand Collapse

        public FluidButton expandButton { get; }
        public FluidButton collapseButton { get; }
        public FloatReaction expandCollapseReaction { get; }

        public UnityAction OnExpand;
        public UnityAction OnCollapse;

        public FluidSideMenu SetOnExpand(UnityAction callback)
        {
            OnExpand = callback;
            return this;
        }

        public FluidSideMenu AddOnExpand(UnityAction callback)
        {
            OnExpand += callback;
            return this;
        }

        public FluidSideMenu ClearOnExpand()
        {
            OnExpand = null;
            return this;
        }

        public FluidSideMenu SetOnCollapse(UnityAction callback)
        {
            OnCollapse = callback;
            return this;
        }

        public FluidSideMenu AddOnCollapse(UnityAction callback)
        {
            OnCollapse += callback;
            return this;
        }

        public FluidSideMenu ClearOnCollapse()
        {
            OnCollapse = null;
            return this;
        }

        public bool isCollapsable
        {
            get => expandCollapseButtonContainer.GetStyleDisplay() == DisplayStyle.Flex;
            set => expandCollapseButtonContainer.SetStyleDisplay(value ? DisplayStyle.Flex : DisplayStyle.None);
        }

        public FluidSideMenu IsCollapsable(bool canCollapse)
        {
            isCollapsable = canCollapse;
            return this;
        }

        public bool hasCustomWidth { get; private set; }
        private int customWidth { get; set; }

        public FluidSideMenu SetCustomWidth(int width)
        {
            hasCustomWidth = true;
            customWidth = Mathf.Max(CollapsedWidth() * 2, width);
            UpdateVisualState();
            return this;
        }

        public FluidSideMenu ClearCustomWidth()
        {
            hasCustomWidth = false;
            UpdateVisualState();
            return this;
        }


        private int ExpandedWidth()
        {
            int value = CollapsedWidth();
            if (hasCustomWidth) return customWidth - value;
            switch (menuLevel)
            {
                case MenuLevel.Level_0: return 208 - value;
                case MenuLevel.Level_1: return 200 - value;
                case MenuLevel.Level_2: return 194 - value;
                default: throw new ArgumentOutOfRangeException();
            }
        }

        private int CollapsedWidth()
        {
            switch (menuLevel)
            {

                case MenuLevel.Level_0: return 54;
                case MenuLevel.Level_1: return 48;
                case MenuLevel.Level_2: return 40;
                default: throw new ArgumentOutOfRangeException();
            }
        }

        #endregion

        public FluidSideMenu SetMenuInfo(string menuName, IEnumerable<Texture2D> textures)
        {
            showMenuInfoWhenCollapsed = true;
            menuInfoContainer.SetStyleDisplay(DisplayStyle.Flex);
            menuInfoLabel.SetText(menuName);
            if (menuInfoIconReaction == null)
            {
                menuInfoIconReaction = menuInfoIcon.GetTexture2DReaction(textures).SetEditorHeartbeat().SetDuration(0.6f);
            }
            else
            {
                menuInfoIconReaction.SetTextures(textures);
            }
            return this;
        }

        public FluidSideMenu ClearMenuInfo()
        {
            showMenuInfoWhenCollapsed = true;
            menuInfoContainer.SetStyleDisplay(DisplayStyle.None);
            menuInfoLabel.SetText(string.Empty);
            menuInfoIconReaction?.Recycle();
            menuInfoIconReaction = null;
            menuInfoIcon.SetStyleBackgroundImage((Texture2D)null);
            return this;
        }

        public FluidSideMenu()
        {
            this.SetStyleFlexGrow(1);

            Add(templateContainer = EditorLayouts.EditorUI.FluidSideMenu.CloneTree());
            templateContainer
                .AddStyle(EditorStyles.EditorUI.FluidSideMenu)
                .SetStyleFlexGrow(1);

            layoutContainer = templateContainer.Q<VisualElement>(nameof(layoutContainer));
            headerContainer = layoutContainer.Q<VisualElement>(nameof(headerContainer));
            expandCollapseButtonContainer = layoutContainer.Q<VisualElement>(nameof(expandCollapseButtonContainer));
            menuInfoContainer = layoutContainer.Q<VisualElement>(nameof(menuInfoContainer));
            menuInfoIcon = layoutContainer.Q<Image>(nameof(menuInfoIcon));
            menuInfoLabel = layoutContainer.Q<Label>(nameof(menuInfoLabel));
            searchBoxContainer = layoutContainer.Q<VisualElement>(nameof(searchBoxContainer));
            toolbarContainer = layoutContainer.Q<VisualElement>(nameof(toolbarContainer));
            buttonsScrollViewContainer = layoutContainer.Q<ScrollView>(nameof(buttonsScrollViewContainer));
            footerContainer = layoutContainer.Q<VisualElement>(nameof(footerContainer));


            layoutContainer.AddManipulator(new Clickable(() =>
            {
                if (isCollapsed)
                {
                    expandButton.ExecuteOnClick();
                    expandButton.SetSelectionState(SelectionState.Normal);
                    return;
                }
                collapseButton.ExecuteOnClick();
            }));

            layoutContainer.RegisterCallback<PointerEnterEvent>(evt =>
            {
                if (isExpanded) return;
                menuInfoIconReaction?.Play();
                expandButton.iconReaction?.Play();
                expandButton.SetSelectionState(SelectionState.Highlighted);
            });

            menuInfoContainer.RegisterCallback<PointerLeaveEvent>(evt =>
            {
                if (isExpanded) return;
                expandButton.SetSelectionState(SelectionState.Normal);
            });

            menuInfoIcon.SetStyleBackgroundImageTintColor(EditorColors.Default.Placeholder);
            menuInfoLabel.SetStyleColor(EditorColors.Default.Placeholder).SetStyleUnityFont(EditorFonts.Ubuntu.Light);
            menuInfoLabel.transform.rotation = Quaternion.Euler(0, 0, -90);

            toggleGroup = FluidToggleGroup.Get().SetControlMode(FluidToggleGroup.ControlMode.OneToggleOnEnforced);

            toggleGroup.OnValueChanged += value =>
            {
                if (hasSearch && searchBox.isSearching) return;
                for (int i = 0; i < buttons.Count; i++)
                {
                    if (!buttons[i].isOn)
                        continue;
                    selectedMenuIndex = i;
                    break;
                }
            };

            buttons =
                new List<FluidToggleButtonTab>();

            expandCollapseReaction =
                Reaction.Get<FloatReaction>()
                    .SetEditorHeartbeat()
                    .SetDuration(k_ExpandCollapseDuration)
                    .SetEase(k_ExpandCollapseEase);

            expandCollapseReaction.setter = value => UpdateVisualState();
            expandCollapseReaction.SetFrom(0f);
            expandCollapseReaction.SetTo(1f);
            expandCollapseReaction.SetValue(1f);

            #region Expand Collapse Buttons

            IsCollapsable(true);

            FluidButton GetNewExpandCollapseButton(List<Texture2D> textures)
            {
                _ = textures ?? throw new ArgumentNullException(nameof(textures));
                FluidButton button = FluidButton.Get().SetIcon(textures).SetElementSize(ElementSize.Small).SetButtonStyle(ButtonStyle.Clear);
                button.buttonContainer.SetStyleJustifyContent(Justify.FlexEnd);
                return button;
            }


            expandCollapseButtonContainer
                .Add
                (
                    expandButton =
                        GetNewExpandCollapseButton(EditorMicroAnimations.EditorUI.Arrows.ChevronRight)
                            .SetName("ExpandButton")
                );

            expandCollapseButtonContainer
                .Add
                (
                    collapseButton =
                        GetNewExpandCollapseButton(EditorMicroAnimations.EditorUI.Arrows.ChevronLeft)
                            .SetName("CollapseButton")
                );

            expandButton.OnClick += () => ExpandMenu();

            collapseButton.OnClick += () =>
            {
                CollapseMenu();

                if (hasSearch && searchBox.isSearching && buttons.Count > 1)
                {
                    SelectTheButtonThatWasSelectedBeforeSearchWasInitiated();
                    // buttons[selectedMenuIndex].isOn = true;
                }
            };

            bool expanded = expandCollapseReaction.currentValue > 0.5f;
            expandButton.SetStyleDisplay(expanded ? DisplayStyle.None : DisplayStyle.Flex);
            collapseButton.SetStyleDisplay(expanded ? DisplayStyle.Flex : DisplayStyle.None);

            #endregion

            // SetAccentColor(EditorSelectableColors.EditorUI.Amber);
            SetMenuLevel(MenuLevel.Level_0);
        }

        public void Dispose()
        {
            RemoveFromHierarchy();

            searchBox?.Dispose();
            toggleGroup?.Dispose();

            if (buttons != null)
            {
                foreach (FluidToggleButtonTab button in buttons)
                    button?.Recycle();

                buttons.Clear();
            }

            OnExpand = null;
            OnCollapse = null;
        }

        public FluidSideMenu ToggleMenu(bool expandMenu, bool animateChange = true)
        {
            if (expandMenu)
            {
                ExpandMenu(animateChange);
                return this;
            }

            CollapseMenu(animateChange);
            return this;
        }

        public FluidSideMenu ExpandMenu(bool animateChange = true)
        {
            if (!isCollapsable) return this;
            schedule.Execute(() =>
            {
                if (animateChange)
                {
                    expandCollapseReaction.PlayToValue(1f);
                }
                else
                {
                    expandCollapseReaction.SetValue(1f);
                }
            });

            expandButton.SetStyleDisplay(DisplayStyle.None);
            collapseButton.SetStyleDisplay(DisplayStyle.Flex);
            return this;
        }

        public FluidSideMenu CollapseMenu(bool animateChange = true)
        {
            if (!isCollapsable) return this;
            schedule.Execute(() =>
            {
                if (animateChange)
                {
                    expandCollapseReaction.PlayToValue(0f);
                }
                else
                {
                    expandCollapseReaction.SetValue(0f);
                }
            });
            
            expandButton.SetStyleDisplay(DisplayStyle.Flex);
            collapseButton.SetStyleDisplay(DisplayStyle.None);
            return this;
        }

        private void UpdateCurrentState()
        {
            currentMenuState =
                expandCollapseReaction.currentValue > 0.5f
                    ? expandCollapseReaction.isActive
                        ? MenuState.IsExpanding
                        : MenuState.Expanded
                    : expandCollapseReaction.isActive
                        ? MenuState.IsCollapsing
                        : MenuState.Collapsed;
        }

        public void UpdateVisualState()
        {
            MarkDirtyRepaint();
            UpdateCurrentState();

            float expandProgress = expandCollapseReaction.currentValue;

            layoutContainer.SetStyleWidth(CollapsedWidth() + ExpandedWidth() * expandProgress);

            float leftRightPadding = 8 + 8 * expandProgress;
            buttonsScrollViewContainer.SetStylePaddingLeft(leftRightPadding);
            buttonsScrollViewContainer.SetStylePaddingRight(leftRightPadding);

            bool shouldShowButtonLabel = expandProgress > 0.2f && (currentMenuState == MenuState.IsExpanding || currentMenuState == MenuState.Expanded);
            bool shouldHideButtonLabel = expandProgress < 0.2f && (currentMenuState == MenuState.IsCollapsing || currentMenuState == MenuState.Collapsed);

            if (shouldShowButtonLabel)
            {
                OnExpand?.Invoke();
                if (showMenuInfoWhenCollapsed)
                    menuInfoContainer.SetStyleDisplay(DisplayStyle.None);

                if (hasToolbar)
                    toolbarContainer.SetStyleDisplay(DisplayStyle.Flex);
            }

            if (shouldHideButtonLabel)
            {
                OnCollapse?.Invoke();
                if (showMenuInfoWhenCollapsed)
                    menuInfoContainer.SetStyleDisplay(DisplayStyle.Flex);

                if (hideToolbarWhenCollapsed)
                    toolbarContainer.SetStyleDisplay(DisplayStyle.None);
            }

            foreach (FluidToggleButtonTab buttonTab in buttons)
            {
                if (shouldShowButtonLabel && areButtonLabelsHidden)
                {
                    buttonTab.SetTabContent(TabContent.IconAndText);
                    buttonTab.SetTooltip(string.Empty);
                    continue;
                }

                if (shouldHideButtonLabel && areButtonLabelsHidden == false)
                {
                    buttonTab.SetTabContent(TabContent.IconOnly);
                    buttonTab.SetTooltip(buttonTab.buttonLabel.text);
                }
            }

            if (shouldShowButtonLabel && areButtonLabelsHidden)
            {
                // footerContainer.visible = true;
                areButtonLabelsHidden = false;

                if (hasSearch)
                {
                    searchBox.SetStyleDisplay(DisplayStyle.Flex);
                }
            }

            if (shouldHideButtonLabel && areButtonLabelsHidden == false)
            {
                // footerContainer.visible = false;
                areButtonLabelsHidden = true;

                if (hasSearch)
                {
                    searchBox.SetStyleDisplay(DisplayStyle.None);
                }
            }
        }

        private void UpdateColors()
        {
            Color backgroundColor = MenuBackgroundColor();
            templateContainer.SetStyleBackgroundColor(backgroundColor);

            EditorSelectableColorInfo buttonContainerColor = ButtonContainerColor();
            foreach (FluidToggleButtonTab button in buttons)
            {
                button.iconContainerSelectableColor = buttonContainerColor;
                button.fluidElement.StateChanged();
            }

        }

        private void AddSearchButtonToButtonsContainer()
        {
            FluidToggleButtonTab button = searchBox.searchTabButton;
            button.Q<VisualElement>(nameof(FluidToggleButtonTab.buttonContainer)).AddClass($"{nameof(FluidSideMenu)}");
            buttonsScrollViewContainer.Insert(0, button);
            UpdateColors();
        }

        private void RemoveSearchButtonFromButtonsContainer()
        {
            FluidToggleButtonTab button = searchBox?.searchTabButton;
            if (button != null) buttonsScrollViewContainer.Remove(button);
            UpdateColors();
        }

        public FluidToggleButtonTab GetNewSideMenuTabButton(string buttonText, EditorSelectableColorInfo accentColor)
        {
            var tabButton =
                FluidToggleButtonTab.Get(buttonText, accentColor)
                    .AddClass($"{nameof(FluidSideMenu)}")
                    .SetElementSize(GetButtonSize(menuLevel))
                    .SetTabPosition(TabPosition.FloatingTab)
                    .SetStyleMarginBottom(4);

            tabButton.Q<VisualElement>(nameof(FluidToggleButtonTab.buttonContainer)).AddClass($"{nameof(FluidSideMenu)}");

            return tabButton;
        }

        private bool addedFirstButton { get; set; }

        public FluidToggleButtonTab AddButton(string buttonText, EditorSelectableColorInfo accentColor, bool connectToMenu = true)
        {
            FluidToggleButtonTab tabButton = GetNewSideMenuTabButton(buttonText, accentColor);
            if (connectToMenu)
                tabButton.AddToToggleGroup(toggleGroup);
            tabButton.OnClick += () => searchBox?.ClearSearch();
            buttons.Add(tabButton);
            buttonsScrollViewContainer.Add(tabButton);
            UpdateColors();

            if (!connectToMenu)
                return tabButton;

            if (addedFirstButton)
                return tabButton;

            schedule.Execute(() => buttons[0].isOn = true); //enable it
            addedFirstButton = true;

            return tabButton;
        }

        public FluidSideMenu HideToolbarWhenCollapsed(bool hide)
        {
            hideToolbarWhenCollapsed = hide;
            hasToolbar = true;
            if (isCollapsed) toolbarContainer.SetStyleDisplay(DisplayStyle.None);
            return this;
        }

        public FluidSideMenu HasToolbar(bool value)
        {
            hasToolbar = value;
            toolbarContainer.SetStyleDisplay(hasToolbar ? DisplayStyle.Flex : DisplayStyle.None);
            return this;
        }
    }
}
