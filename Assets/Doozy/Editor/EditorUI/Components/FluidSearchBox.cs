// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using Doozy.Editor.EditorUI.Components.Internal;
using Doozy.Editor.EditorUI.ScriptableObjects.Colors;
using Doozy.Editor.Reactor.Internal;
using Doozy.Runtime.Colors;
using Doozy.Runtime.Common.Extensions;
using Doozy.Runtime.Reactor.Extensions;
using Doozy.Runtime.Reactor.Internal;
using Doozy.Runtime.Reactor.Reactions;
using Doozy.Runtime.UIElements.Extensions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace Doozy.Editor.EditorUI.Components
{
    public class FluidSearchBox : VisualElement
    {
        private const string SEARCH_TEXT = "Search...";
        public int minimumNumberOfCharactersToExecuteTheSearch { get; private set; }

        public TemplateContainer templateContainer { get; }
        public VisualElement layoutContainer { get; }
        public TextField searchTextField { get; }
        public VisualElement searchTextFieldInput { get; }
        public VisualElement searchInfoContainer { get; }
        public VisualElement buttonContainer { get; }
        public FluidButton searchButton { get; }
        public FluidButton cancelSearchButton { get; }
        public Label leftLabel { get; }
        public VisualElement flexibleSpaceBetweenLabels { get; }
        public Label rightLabel { get; }

        public FluidToggleButtonTab searchTabButton { get; }

        public bool isSearching => searchPattern.IsNullOrEmpty() == false;

        public string searchPattern
        {
            get => searchTextField.value;
            set => Search(value);
        }

        public UnityAction OnClearSearchCallback;
        public UnityAction<bool> OnShowSearchResultsCallback;
        public UnityAction<string> OnSearchPatternChangedCallback;

        private EditorSelectableColorInfo m_SelectableAccentColor;

        #region Empty Search Placeholder

        private const float PLACEHOLDER_ANIMATION_DURATION = 1f;
        public Image emptySearchPlaceholderImage { get; }
        public Texture2DReaction emptySearchPlaceholderAnimation { get; private set; }
        public static List<Texture2D> emptySearchPlaceholderTextures { get; private set; }

        #endregion

        public FluidSearchBox()
        {
            Add(templateContainer = EditorLayouts.EditorUI.FluidSearchBox.CloneTree());
            templateContainer
                .SetStyleFlexGrow(1)
                .SetStyleJustifyContent(Justify.Center)
                .AddStyle(EditorStyles.EditorUI.FluidSearchBox);

            layoutContainer = templateContainer.Q<VisualElement>(nameof(layoutContainer));
            searchTextField = layoutContainer.Q<TextField>(nameof(searchTextField));
            searchTextFieldInput = searchTextField.Q<VisualElement>("unity-text-input");
            // searchTextFieldInput = searchTextField.Q<VisualElement>(TextField.inputUssClassName);
            searchInfoContainer = layoutContainer.Q<VisualElement>(nameof(searchInfoContainer)).SetPickingMode(PickingMode.Ignore);
            buttonContainer = searchInfoContainer.Q<VisualElement>(nameof(buttonContainer));
            leftLabel = searchInfoContainer.Q<Label>(nameof(leftLabel)).SetPickingMode(PickingMode.Ignore);
            flexibleSpaceBetweenLabels = searchInfoContainer.Q<VisualElement>(nameof(flexibleSpaceBetweenLabels));
            rightLabel = searchInfoContainer.Q<Label>(nameof(rightLabel)).SetPickingMode(PickingMode.Ignore);

            Color placeholderColor = EditorColors.Default.Placeholder;
            emptySearchPlaceholderTextures = emptySearchPlaceholderTextures ?? EditorMicroAnimations.EditorUI.Placeholders.EmptySearch;

            //searchTextFieldInput
            const float searchTextFieldInputPaddingLeft = 22;
            searchTextFieldInput
                .SetStylePadding(searchTextFieldInputPaddingLeft, 1, 0, 0)
                .SetStyleBackgroundColor(EditorColors.Default.BoxBackground)
                .SetStyleBorderRadius(8)
                .SetStyleBorderWidth(0);

            Color infoLabelsTextColor = EditorColors.Default.TextDescription;
            Font infoLabelsFont = EditorFonts.Ubuntu.Light;

            //LeftLabel
            leftLabel
                .SetText(SEARCH_TEXT)
                .SetPickingMode(PickingMode.Ignore)
                .SetStyleColor(infoLabelsTextColor)
                .SetStyleUnityFont(infoLabelsFont);

            //FlexibleSpaceBetweenLabels
            flexibleSpaceBetweenLabels
                .SetPickingMode(PickingMode.Ignore);

            //RightLabel
            rightLabel
                .SetPickingMode(PickingMode.Ignore)
                .SetStyleColor(infoLabelsTextColor)
                .SetStyleUnityFont(infoLabelsFont);

            //EMPTY SEARCH PLACEHOLDER
            emptySearchPlaceholderImage =
                new Image()
                    .SetName("PlaceholderImage")
                    .SetStyleAlignSelf(Align.Center)
                    .SetStyleFlexGrow(0)
                    .SetStyleFlexShrink(0)
                    .SetStyleBackgroundImageTintColor(placeholderColor);

            emptySearchPlaceholderAnimation =
                emptySearchPlaceholderImage
                    .GetTexture2DReaction(emptySearchPlaceholderTextures)
                    .SetEditorHeartbeat()
                    .SetDuration(PLACEHOLDER_ANIMATION_DURATION);

            emptySearchPlaceholderImage.SetStyleSize(emptySearchPlaceholderAnimation.current.width, emptySearchPlaceholderAnimation.current.height); //update placeholder image size to match the animation texture size
            emptySearchPlaceholderImage.AddManipulator(new Clickable(ClearSearch));


            //INJECT SEARCH ICON BUTTON
            buttonContainer.Insert(0,
                searchButton =
                    GetNewSearchButton(EditorMicroAnimations.EditorUI.Icons.Search)
                        .SetOnClick(() => searchTextFieldInput.Focus())
            );

            //INJECT CANCEL SEARCH ICON BUTTON
            buttonContainer.Insert(1,
                cancelSearchButton =
                    GetNewSearchButton(EditorMicroAnimations.EditorUI.Icons.Close)
                        .SetAccentColor(EditorSelectableColors.EditorUI.Red)
                        .SetOnClick(ClearSearch)
            );

            //INITIALIZE SEARCH TAB BUTTON (that gets injected in the menu and is used to show search results content)
            searchTabButton =
                FluidToggleButtonTab.Get(SEARCH_TEXT, EditorMicroAnimations.EditorUI.Icons.Search)
                    .SetElementSize(ElementSize.Small)
                    .SetStyleMarginBottom(8)
                    .SetTabPosition(TabPosition.FloatingTab)
                    .SetStyleDisplay(DisplayStyle.None);


            //ADD CALLBACK to search TextField to trigger an animation on the SEARCH ICON animation, when FocusInEvent is fired 
            searchTextField.RegisterCallback<FocusInEvent>(evt => searchButton.iconReaction.Play());

            //ADD CALLBACK to search TextField to call Search and invoke onSearchPatternChangedCallback, when the TextField value changed
            searchTextField.RegisterValueChangedCallback(evt =>
            {
                Search(evt.newValue);
                OnSearchPatternChangedCallback?.Invoke(evt.newValue);
            });

            //INITIALIZE SEARCHABLES HashSet
            searchables = new HashSet<ISearchable>();

            //INIT other SEARCH BOXES HashSet
            connectedSearchBoxes = new HashSet<FluidSearchBox>();

            //CALL Search to update things
            Search(searchTextField.value);
        }

        public virtual void Dispose()
        {
            RemoveFromHierarchy();

            searchTabButton?.Dispose();
            searchButton?.Dispose();
            cancelSearchButton?.Dispose();

            emptySearchPlaceholderAnimation?.Recycle();
        }


        public void ClearSearch()
        {
            // Debug.Log($"{nameof(FluidSearchBox)}.{nameof(ClearSearch)}()");

            if (!isSearching) return;

            //SET SEARCH PATTERN to string.Empty
            searchPattern = string.Empty;

            //INVOKE CLEAR SEARCH CALLBACK
            OnClearSearchCallback?.Invoke();

            //CALL CLEAR SEARCH IN SEARCHABLES
            searchables.Remove(null);
            if (searchables?.Count > 0)
            {
                foreach (ISearchable searchable in searchables)
                    searchable.ClearSearch();
            }

            //CALL CLEAR SEARCH IN CONNECTED SEARCH BOXES
            connectedSearchBoxes.Remove(null);
            if (connectedSearchBoxes?.Count > 0)
            {
                foreach (FluidSearchBox connectedSearchBox in connectedSearchBoxes)
                    connectedSearchBox.ClearSearch();
            }
        }

        /// <summary> Perform a search for the given pattern </summary>
        /// <param name="pattern"> Search pattern </param>
        private void Search(string pattern)
        {
            //UPDATE SEARCH PATTERN to the given pattern
            searchTextField.value = pattern;

            int patternLength = pattern.Length;
            leftLabel.visible = patternLength == 0;
            UpdateSearchTextFieldConstraintsMessage(patternLength);

            //INVOKE SHOW SEARCH RESULTS CALLBACK
            OnShowSearchResultsCallback?.Invoke(pattern.IsNullOrEmpty() == false);

            //CALL SEARCH IN SEARCHABLES
            searchables.Remove(null);
            if (searchables?.Count > 0)
            {
                foreach (ISearchable searchable in searchables)
                    searchable.Search(pattern);
            }

            //CALL SEARCH IN CONNECTED SEARCH BOXES
            connectedSearchBoxes.Remove(null);
            if (connectedSearchBoxes?.Count > 0)
            {
                foreach (FluidSearchBox connectedSearchBox in connectedSearchBoxes)
                    connectedSearchBox.Search(pattern);
            }

            //SEARCH PATTERN EMPTY (isSearching = false)
            if (pattern.IsNullOrEmpty())
            {
                //HIDE SEARCH TAB BUTTON (that was injected in the menu and is used to show search results content)
                if (searchTabButton.inToggleGroup)
                {
                    searchTabButton.isOn = false;
                    searchTabButton.SetStyleDisplay(DisplayStyle.None);
                    searchTabButton.SetLabelText(SEARCH_TEXT);
                }

                //SHOW SEARCH ICON BUTTON
                searchButton.SetStyleDisplay(DisplayStyle.Flex);
                //ANIMATE SEARCH ICON BUTTON
                searchButton.iconReaction.Play();
                //HIDE CANCEL SEARCH ICON BUTTON
                cancelSearchButton.SetStyleDisplay(DisplayStyle.None);

                return;
            }

            //SHOW SEARCH TAB BUTTON (that was injected in the menu and is used to show search results content)
            if (searchTabButton.inToggleGroup)
            {
                searchTabButton.isOn = true;
                searchTabButton.SetStyleDisplay(DisplayStyle.Flex);
                searchTabButton.SetLabelText($"{pattern}...");
                searchTabButton.iconReaction.Play();
            }

            //HIDE SEARCH ICON BUTTON
            searchButton.SetStyleDisplay(DisplayStyle.None);
            //SHOW CANCEL SEARCH ICON BUTTON
            cancelSearchButton.SetStyleDisplay(DisplayStyle.Flex);
            //ANIMATE CANCEL SEARCH ICON BUTTON
            cancelSearchButton.iconReaction.Play();
            //SET CANCEL SEARCH ICON BUTTON to HIGHLIGHTED STATE (this makes the button red, as it applies the highlighted selectable state color) 
            cancelSearchButton.selectionState = SelectionState.Highlighted;
        }

        /// <summary> Add the searchTabButton to the given toggle group </summary>
        /// <param name="value"> Target toggle group </param>
        public FluidSearchBox ConnectToToggleGroup(IToggleGroup value)
        {
            searchTabButton.AddToToggleGroup(value);
            return this;
        }

        /// <summary> Remove the searchTabButton from any toggle group (it was previously registered to) </summary>
        public FluidSearchBox DisconnectFromToggleGroup()
        {
            searchTabButton.RemoveFromToggleGroup();
            return this;
        }

        /// <summary> Set the accent color for this search box (is applied to all the interactable components) </summary>
        /// <param name="selectableColor"> New selectable accent color </param>
        public FluidSearchBox SetAccentColor(EditorSelectableColorInfo selectableColor)
        {
            if (selectableColor == null) return this;
            m_SelectableAccentColor = selectableColor;
            searchButton.SetAccentColor(m_SelectableAccentColor);
            searchTabButton.SetToggleAccentColor(m_SelectableAccentColor);
            return this;
        }

        /// <summary> Get a new fluid button with the given textures and configured to work as an icon button </summary>
        /// <param name="animatedIconTextures"> Animated Icon Textures </param>
        private static FluidButton GetNewSearchButton(IEnumerable<Texture2D> animatedIconTextures) =>
            FluidButton.Get()
                .SetIcon(animatedIconTextures)
                .SetElementSize(ElementSize.Tiny)
                .SetStyleAlignSelf(Align.Center)
                .SetButtonStyle(ButtonStyle.Clear);

        /// <summary> Set a minimum number of characters needed to start a search (this is used to update the visuals and user messages) </summary>
        /// <param name="value"> 0 (disables the system) and a value of 3 is recommended for a smooth search </param>
        public FluidSearchBox SetMinimumNumberOfCharactersToExecuteSearch(int value)
        {
            minimumNumberOfCharactersToExecuteTheSearch = Mathf.Max(0, value);
            UpdateSearchTextFieldConstraintsMessage(searchPattern.Length);
            return this;
        }

        /// <summary> Updates the RightLabel text and toggles its visibility </summary>
        /// <param name="patternLength"> Search pattern length (number of characters) </param>
        private void UpdateSearchTextFieldConstraintsMessage(int patternLength)
        {
            rightLabel.SetText($"min {minimumNumberOfCharactersToExecuteTheSearch} characters");
            rightLabel.visible = minimumNumberOfCharactersToExecuteTheSearch > 0 && patternLength < minimumNumberOfCharactersToExecuteTheSearch;
        }

        #region Set Callbacks

        /// <summary> Add a callback to onSearchPatternChangedCallback </summary>
        /// <param name="callback"> Callback </param>
        public FluidSearchBox SetOnSearchPatternChangedCallback(UnityAction<string> callback)
        {
            if (callback == null) return this;
            OnSearchPatternChangedCallback += callback;
            return this;
        }

        /// <summary> Add a callback to onClearSearchCallback </summary>
        /// <param name="callback"> Callback </param>
        public FluidSearchBox SetOnClearSearchCallback(UnityAction callback)
        {
            if (callback == null) return this;
            OnClearSearchCallback += callback;
            return this;
        }

        /// <summary> Add a callback to OnShowSearchResultsCallback </summary>
        /// <param name="callback"> Callback </param>
        public FluidSearchBox SetOnShowSearchResultsCallback(UnityAction<bool> callback)
        {
            if (callback == null) return this;
            OnShowSearchResultsCallback += callback;
            return this;
        }

        #endregion

        #region ISearchable

        /// <summary> All ISearchables controlled by this search box </summary>
        public HashSet<ISearchable> searchables { get; }

        /// <summary> Add an ISearchable to this search box </summary>
        /// <param name="searchable"> Target searchable </param>
        public FluidSearchBox AddSearchable(ISearchable searchable)
        {
            if (searchable == null) return this;
            searchables.Add(searchable);
            return this;
        }

        /// <summary> Remove an ISearchable from this search box </summary>
        /// <param name="searchable"> Target searchable </param>
        public FluidSearchBox RemoveSearchable(ISearchable searchable)
        {
            searchables.Remove(searchable);
            return this;
        }

        /// <summary> Clear searchables HashSet </summary>
        public FluidSearchBox ClearSearchables()
        {
            searchables.Clear();
            return this;
        }

        #endregion

        #region OTHER SearchBoxes

        /// <summary> All other SearchBoxes controlled by this search box </summary>
        public HashSet<FluidSearchBox> connectedSearchBoxes { get; }

        /// <summary> Add a search box to be controlled by this search box </summary>
        /// <param name="searchBox"> Other search box </param>
        public FluidSearchBox AddSearchBox(FluidSearchBox searchBox)
        {
            if (searchBox == null) return this;
            if (searchBox == this) return this;
            connectedSearchBoxes.Add(searchBox);
            return this;
        }

        /// <summary> Remove the given search box from the connectedSearchBoxes HashSet of this search box </summary>
        /// <param name="searchBox"> Other search box </param>
        public FluidSearchBox RemoveSearchBox(FluidSearchBox searchBox)
        {
            connectedSearchBoxes.Remove(searchBox);
            return this;
        }

        /// <summary> Clear connectedSearchBoxes HashSet </summary>
        public FluidSearchBox ClearConnectedSearchBoxes()
        {
            connectedSearchBoxes.Clear();
            return this;
        }

        #endregion

        /// <summary> Get a VisualElement containing an empty search visual with the given message </summary>
        /// <param name="message"> Message that appears under the search visual </param>
        public VisualElement EmptySearchPlaceholderElement(string message = "No results found") =>
            EmptySearchPlaceholderElement(EditorColors.Default.TextSubtitle, message);

        /// <summary> Get a VisualElement containing an empty search visual with the given message </summary>
        /// <param name="accentColor"> Visual and Message text accent color </param>
        /// <param name="message"> Message that appears under the search visual </param>
        private VisualElement EmptySearchPlaceholderElement(Color accentColor, string message = "No results found")
        {
            VisualElement element = new VisualElement()
                .SetStyleFlexGrow(1)
                .SetStyleJustifyContent(Justify.Center)
                .AddChild
                (
                    emptySearchPlaceholderImage
                        .SetStyleBackgroundImageTintColor(accentColor)
                        .SetStyleAlignSelf(Align.Center)
                )
                .AddChild
                (
                    new Label(message)
                        .SetStyleMarginTop(8)
                        .SetStyleFontSize(16)
                        .SetStyleUnityFont(EditorFonts.Ubuntu.Light)
                        .SetStyleColor(accentColor)
                        .SetStyleAlignSelf(Align.Center)
                );

            emptySearchPlaceholderAnimation.Play();
            return element;
        }
    }
}
