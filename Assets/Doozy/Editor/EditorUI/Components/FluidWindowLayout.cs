// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using System.Linq;
using Doozy.Editor.EditorUI.Components.Internal;
using Doozy.Editor.EditorUI.ScriptableObjects.Colors;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Runtime.Pooler;
using Doozy.Runtime.UIElements.Extensions;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
// ReSharper disable MemberCanBeProtected.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Editor.EditorUI.Components
{
    public abstract class FluidWindowLayout : VisualElement
    {
        protected string EditorPrefsKey(string variableName) => $"{GetType().FullName} - {variableName}";

        public virtual string layoutName => "Unknown Layout Name";
        public virtual Texture2D staticIconTexture => null;
        public virtual List<Texture2D> animatedIconTextures => null;
        public virtual Color accentColor => EditorColors.EditorUI.Amber;
        public virtual EditorSelectableColorInfo selectableAccentColor => EditorSelectableColors.EditorUI.Amber;

        public TemplateContainer templateContainer { get; protected set; }
        public VisualElement menu { get; protected set; }
        public VisualElement header { get; protected set; }
        public VisualElement content { get; protected set; }
        public VisualElement footer { get; protected set; }
        public Label footerLabel { get; protected set; }

        public FluidSideMenu sideMenu { get; protected set; }

        public ScrollView searchResults { get; }
        public Dictionary<ISearchable, FluidSearchableItem> searchableItems { get; }

        protected virtual int maximumNumberOfItemsVisibleAtOnce => 10;

        protected virtual int spacing => DesignUtils.k_Spacing;

        public readonly List<IPoolable> Poolables;

        public void Dispose()
        {
            if (Poolables != null && Poolables.Count > 0)
                foreach (IPoolable poolable in Poolables)
                    poolable?.Recycle();

            sideMenu.Dispose();
        }

        protected FluidWindowLayout()
        {
            this.SetStyleFlexGrow(1);

            Add(templateContainer = EditorLayouts.EditorUI.FluidWindowLayout.CloneTree());
            templateContainer
                .SetStyleFlexGrow(1)
                .AddStyle(EditorStyles.EditorUI.FluidWindowLayout);

            //REFERENCES
            menu = templateContainer.Q<VisualElement>("MenuContainer");
            header = templateContainer.Q<VisualElement>("HeaderContainer");
            content = templateContainer.Q<VisualElement>("ContentContainer");
            footer = templateContainer.Q<VisualElement>("FooterContainer");
            footerLabel = footer.Q<Label>("FooterLabel");

            content.viewDataKey = "content";

            Poolables = new List<IPoolable>();
            searchResults = new ScrollView { viewDataKey = nameof(searchResults) };
            searchableItems = new Dictionary<ISearchable, FluidSearchableItem>();

            //SIDE MENU <<< ADD search, NOT collapsable, Menu Level 1, ColorName.Amber
            menu.Add(sideMenu = new FluidSideMenu().AddSearch().IsCollapsable(false).SetMenuLevel(FluidSideMenu.MenuLevel.Level_1));

            //SEARCH - clear the searchable items list (used to generate complex search results)
            searchableItems.Clear();

            //SIDE MENU - SEARCH BOX - set minimum number of characters needed to perform a search (needed for a smooth search and a better UX)
            sideMenu.searchBox.SetMinimumNumberOfCharactersToExecuteSearch(3);

            //SIDE MENU - SEARCH BOX - clear the target searchables HashSet
            sideMenu.searchBox.ClearSearchables();
            //SIDE MENU - SEARCH BOX - clear the connected search boxes HashSet
            sideMenu.searchBox.ClearConnectedSearchBoxes();

            //SIDE MENU - SEARCH BOX - callback when a search ended to reselect the previously selected side menu button (and show its content)
            sideMenu.searchBox.OnShowSearchResultsCallback += value =>
            {
                if (value) return;
                if (sideMenu.selectedMenuIndex < 0)
                    return;

                if (sideMenu.selectedMenuIndex > sideMenu.buttons.Count - 1)
                    return;

                FluidToggleButtonTab sideMenuButton = sideMenu.buttons[sideMenu.selectedMenuIndex];
                sideMenuButton?.SetIsOn(true);
            };

            //SIDE MENU - SEARCH BOX - callback when a search is in progress to update the search results
            sideMenu.searchBox.OnSearchPatternChangedCallback += value =>
            {
                if (sideMenu.searchBox.isSearching)
                    UpdateSearchResults();
            };

            //FOOTER
            footer
                .SetStyleBackgroundColor(EditorColors.Default.Background);

            //FOOTER LABEL
            footerLabel
                .SetStyleUnityFont(EditorFonts.Ubuntu.Light)
                .SetStyleColor(EditorColors.Default.TextDescription);


            RegisterCallback<AttachToPanelEvent>(OnAttachToPanel);
            RegisterCallback<DetachFromPanelEvent>(OnDetachFromPanel);
        }


        private FluidWindowHeader fluidWindowHeader { get; set; }

        protected void AddHeader(string titleText, string subtitleText, IEnumerable<Texture2D> iconTextures)
        {
            Poolables.Add(fluidWindowHeader = FluidWindowHeader.Get(titleText, subtitleText, iconTextures));
            header.Add(fluidWindowHeader);
        }

        protected void AddHeader(string titleText, string subtitleText, Texture2D iconTexture)
        {
            Poolables.Add(fluidWindowHeader = FluidWindowHeader.Get(titleText, subtitleText, iconTexture));
            header.Add(fluidWindowHeader);
        }


        public virtual void OnEnable() {}
        public virtual void OnDisable() {}
        public virtual void OnDestroy()
        {
            Dispose();
        }

        protected virtual void OnAttachToPanel(AttachToPanelEvent evt) =>
            schedule.Execute(LoadWindowState).ExecuteLater(100);

        protected virtual void OnDetachFromPanel(DetachFromPanelEvent evt) =>
            SaveWindowState();

        protected virtual void SaveWindowState()
        {
            EditorPrefs.SetInt(EditorPrefsKey(nameof(sideMenu.selectedMenuIndex)), sideMenu.selectedMenuIndex);
            EditorPrefs.SetBool(EditorPrefsKey(nameof(sideMenu.isExpanded)), sideMenu.isExpanded);
        }

        protected virtual void LoadWindowState()
        {
            int selectedMenuIndex = EditorPrefs.GetInt(EditorPrefsKey(nameof(sideMenu.selectedMenuIndex)), 0);
            if (selectedMenuIndex < sideMenu.buttons.Count)
                sideMenu.buttons[selectedMenuIndex].isOn = true;

            sideMenu.ToggleMenu(EditorPrefs.GetBool(EditorPrefsKey(nameof(sideMenu.isExpanded)), true));
        }

        protected void SideMenuButtonClick(FluidListView fluidListView, VisualElement visualElement, FluidButton selectAssetButton)
        {
            //SET LIST VIEW PREFERRED HEIGHT (do not use our dynamic height option as Unity is VERY SLOW in calculating its own listview height --- kill me now!!!)
            #if UNITY_2021_2_OR_NEWER
            fluidListView.SetPreferredListHeight((int)fluidListView.listView.fixedItemHeight * maximumNumberOfItemsVisibleAtOnce);
            #else
            fluidListView.SetPreferredListHeight(fluidListView.listView.itemHeight * maximumNumberOfItemsVisibleAtOnce);
            #endif

            //HIDE LIST VIEW TOOLBAR WHILE SEARCHING
            fluidListView.HideToolbarWhileSearching(false);

            //CLEAR SEARCH (in case it was left in a limbo state)
            fluidListView.searchBox.ClearSearch();

            //CLEAR (window right side) CONTENT container
            content.Clear();

            //CONTENT <<< add SELECT ASSET BUTTON and target object VISUAL ELEMENT (the custom editor with minor adaptations for in window use)
            content
                .AddChild(selectAssetButton)
                .AddChild(visualElement);
        }

        protected virtual void UpdateSearchResults()
        {
            //CLEAR << CONTENT container 
            content.Clear();
            //CLEAR << SEARCH RESULTS container
            searchResults.contentContainer.Clear();

            //SEARCH PATTERN length
            int searchPatternLength = sideMenu.searchBox.searchPattern.Length;
            //MIN SEARCH PATTERN LENGTH CONSTRAINT
            int minimumSearchLength = sideMenu.searchBox.minimumNumberOfCharactersToExecuteTheSearch;

            //UPDATE SEARCH VISUAL - tell the user how many characters are needed to start the search
            if (searchPatternLength < minimumSearchLength)
            {
                //CALCULATE how many characters does the user need to add to the search box to the search to start
                int numberOfCharactersNeeded = minimumSearchLength - searchPatternLength;
                //SHOW SEARCH VISUAL <<< with the info text for the user
                content.Add(sideMenu.searchBox.EmptySearchPlaceholderElement($"Add {numberOfCharactersNeeded} more character{(numberOfCharactersNeeded != 1 ? "s" : "")}..."));
                //STOP
                return;
            }

            //FLAG for SEARCH RESULTS (for this single pass)
            bool hasSearchResults = false;
            //ITERATE through SEARCHABLES for SEARCH RESULTS
            foreach (ISearchable searchable in sideMenu.searchBox.searchables.Where(searchable => searchable.hasSearchResults))
            {
                //ADD SEARCH RESULT from searchable to SEARCH RESULTS container
                searchResults.contentContainer
                    .AddChild(searchableItems[searchable].selectAssetButton)
                    .AddChild(searchableItems[searchable].visualElement)
                    .AddSpace(0, 24);

                //GET SEARCHABLE FluidListView reference
                FluidListView fluidListView = searchableItems[searchable].fluidListView;

                //SET DYNAMIC LIST HEIGHT to FALSE
                fluidListView.SetDynamicListHeight(false);

                //HIDE LIST VIEW TOOLBAR (this is an 'external' search and these options are useless; we are also hiding the search bar)
                fluidListView.HideToolbarWhileSearching(true);

                //UPDATE FluidListView (data and visuals)
                fluidListView.Update();

                //FLAG for SEARCH RESULTS <<< TRUE
                hasSearchResults = true;
            }

            //CHECK FLAG for SEARCH RESULTS
            if (!hasSearchResults)
            {
                //NO SEARCH RESULTS <<< show search visual
                content.Add(sideMenu.searchBox.EmptySearchPlaceholderElement());
                //STOP
                return;
            }

            //ADD SEARCH RESULTS to CONTENT container
            content.Add(searchResults);
        }


        protected FluidToggleButtonTab GetNewTabButton(string buttonText, FluidToggleGroup addToToggleGroup = null)
        {
            FluidToggleButtonTab tab =
                FluidToggleButtonTab.Get(buttonText)
                    .SetElementSize(ElementSize.Normal)
                    .SetTabPosition(TabPosition.FloatingTab);
            tab.AddToToggleGroup(addToToggleGroup);
            return tab;
        }

        protected static Label GetNewTabButtonInfoLabel(string labelText) =>
            new Label(labelText)
                .SetStyleHeight(23)
                .SetStyleTextAlign(TextAnchor.MiddleRight)
                .SetStylePadding(0, 0, 8, 0)
                .SetStyleMarginTop(-23)
                .SetPickingMode(PickingMode.Ignore)
                .SetStyleFontSize(8)
                .SetStyleUnityFont(EditorFonts.Ubuntu.Light)
                .SetStyleColor(EditorColors.Default.TextDescription);

        protected static VisualElement GetTabButtonWithInfoLabel(FluidToggleButtonTab tabButton, string infoLabelText) =>
            new VisualElement().SetStyleMarginBottom(8)
                .AddChild(tabButton)
                .AddChild(GetNewTabButtonInfoLabel(infoLabelText));

        protected static FluidButton GetNewSelectAssetButton(string buttonName, Texture2D buttonIcon, Object asset)
        {
            string assetPath = AssetDatabase.GetAssetPath(asset);
            FluidButton button =
                FluidButton.Get()
                    .SetLabelText($"{buttonName}")
                    .SetIcon(buttonIcon)
                    .SetOnClick(() => Selection.activeObject = asset)
                    .SetButtonStyle(ButtonStyle.Contained)
                    .SetElementSize(ElementSize.Small);

            button.buttonLabel.SetStyleTextAlign(TextAnchor.MiddleLeft); //set button label text alignment

            button.buttonContainer.Add //add the asset path label to the button container
            (
                new Label(assetPath)
                    .SetStyleUnityFont(EditorFonts.Ubuntu.Light)
                    .SetStyleFontSize(9)
                    .SetStyleTextAlign(TextAnchor.MiddleRight)
                    .SetStyleColor(EditorColors.Default.TextDescription)
            );


            return button;
        }

        protected static VisualElement GetNewItemsContainer() =>
            new VisualElement()
                .SetStyleFlexDirection(FlexDirection.Column)
                .SetStyleFlexGrow(1);
    }
}
