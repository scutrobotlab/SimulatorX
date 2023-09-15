// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.Components.Internal;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Editor.EditorUI.Windows.Internal;
using Doozy.Editor.Reactor.Internal;
using Doozy.Editor.UIElements;
using Doozy.Runtime.Common.Extensions;
using Doozy.Runtime.Reactor.Extensions;
using Doozy.Runtime.Reactor.Reactions;
using Doozy.Runtime.UIElements.Extensions;
using UnityEditor;
using UnityEngine.UIElements;
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Editor.UIManager.UIMenu
{
    public class UIMenuWindow : FluidWindow<UIMenuWindow>, ISearchable
    {
        private const string WINDOW_TITLE = "UI Menu";
        public const string k_WindowMenuPath = "Tools/Doozy/UI Menu";

        [MenuItem(k_WindowMenuPath, priority = -1000)]
        public static void Open() => InternalOpenWindow(WINDOW_TITLE);

        public Image uiMenuHeader { get; private set; }
        public Texture2DReaction uiMenuHeaderReaction { get; private set; }

        public ScrollView contentScroller { get; private set; }

        public FluidSearchBox searchBox { get; private set; }

        public FluidToggleButtonTab autoSelectTab { get; private set; }
        public FluidToggleButtonTab defaultInstantiateModeTab { get; private set; }
        public FluidToggleButtonTab cloneInstantiateModeTab { get; private set; }
        public FluidToggleButtonTab linkInstantiateModeTab { get; private set; }

        public FluidButton refreshButton { get; set; }
        public FluidButton regenerateButton { get; set; }

        private List<UIMenuPrefabType> prefabTypes { get; set; }
        private List<UIMenuItem> items { get; set; }
        private List<UIMenuItem> searchItems { get; set; }
        public VisualElement itemsContainer { get; set; }

        protected override void CreateGUI()
        {
            Initialize();
            Compose();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            Undo.undoRedoPerformed += UndoRedoPerformed;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            Undo.undoRedoPerformed -= UndoRedoPerformed;
        }

        private void UndoRedoPerformed()
        {
            UpdateBottomTabs(true);
        }

        private void Initialize()
        {
            uiMenuHeader =
                new Image()
                    .SetStyleBackgroundImageTintColor(EditorColors.Default.WindowHeaderTitle)
                    .SetStyleMarginLeft(DesignUtils.k_Spacing3X);
            
            uiMenuHeaderReaction =
                uiMenuHeader
                    .GetTexture2DReaction(EditorMicroAnimations.UIManager.UIMenu.UIMenuHeader)
                    .SetEditorHeartbeat();

            uiMenuHeaderReaction.Play();

            float headerWidth = EditorMicroAnimations.UIManager.UIMenu.UIMenuHeader[0].width;
            float headerHeight = EditorMicroAnimations.UIManager.UIMenu.UIMenuHeader[0].height;
            uiMenuHeader.SetStyleSize(headerWidth * 0.6f, headerHeight * 0.6f);

            uiMenuHeader.RegisterCallback<PointerEnterEvent>(evt => uiMenuHeaderReaction?.Play());
            uiMenuHeader.AddManipulator(new Clickable(() => uiMenuHeaderReaction?.Play()));

            searchBox =
                new FluidSearchBox()
                    .SetStyleMargins(DesignUtils.k_Spacing2X)
                    .SetStyleAlignSelf(Align.Center);

            searchBox.AddSearchable(this);

            regenerateButton =
                FluidButton.Get()
                    .SetTooltip("Regenerate the UI Menu")
                    .SetIcon(EditorMicroAnimations.EditorUI.Icons.Reset)
                    .SetElementSize(ElementSize.Tiny)
                    .SetOnClick(() =>
                    {
                        UIMenuItemsDatabase.instance.RefreshDatabase();
                        UpdateItems();
                    });
            
            refreshButton =
                FluidButton.Get()
                    .SetTooltip("Refresh the UI Menu")
                    .SetIcon(EditorMicroAnimations.EditorUI.Icons.Refresh)
                    .SetElementSize(ElementSize.Tiny)
                    .SetOnClick(UpdateItems);

            FluidToggleButtonTab GetInstantiateTab(string labelText) =>
                FluidToggleButtonTab.Get()
                    .SetLabelText(labelText)
                    .SetContainerColorOff(DesignUtils.tabButtonColorOff)
                    .SetToggleAccentColor(EditorSelectableColors.UIManager.UIComponent);

            autoSelectTab =
                GetInstantiateTab("Auto Select")
                    .SetTooltip("Auto select newly created objects")
                    .SetOnClick(() =>
                    {
                        Undo.RecordObject(UIMenuSettings.instance, "Update Auto Select");
                        UIMenuSettings.instance.SelectNewlyCreatedObjects = !UIMenuSettings.instance.SelectNewlyCreatedObjects;
                        EditorUtility.SetDirty(UIMenuSettings.instance);
                        AssetDatabase.SaveAssetIfDirty(UIMenuSettings.instance);
                        // AssetDatabase.SaveAssets();
                        UpdateBottomTabs(true);
                    });

            cloneInstantiateModeTab =
                GetInstantiateTab(PrefabInstantiateModeSetting.Clone.ToString())
                    .SetTooltip("Clone Instantiate Mode\n\nIgnore the default menu item behaviour when instantiating its target object and create a clone")
                    .SetTabPosition(TabPosition.TabOnLeft)
                    .SetOnClick(() =>
                    {
                        Undo.RecordObject(UIMenuSettings.instance, "Update InstantiateMode");
                        UIMenuSettings.instance.InstantiateMode = PrefabInstantiateModeSetting.Clone;
                        EditorUtility.SetDirty(UIMenuSettings.instance);
                        AssetDatabase.SaveAssetIfDirty(UIMenuSettings.instance);
                        // AssetDatabase.SaveAssets();
                        UpdateBottomTabs(true);
                    });

            defaultInstantiateModeTab =
                GetInstantiateTab(PrefabInstantiateModeSetting.Default.ToString())
                    .SetTooltip("Default Instantiate Mode\n\nUse the default menu item behaviour when instantiating its target object")
                    .SetTabPosition(TabPosition.TabInCenter)
                    .SetOnClick(() =>
                    {
                        Undo.RecordObject(UIMenuSettings.instance, "Update InstantiateMode");
                        UIMenuSettings.instance.InstantiateMode = PrefabInstantiateModeSetting.Default;
                        EditorUtility.SetDirty(UIMenuSettings.instance);
                        AssetDatabase.SaveAssetIfDirty(UIMenuSettings.instance);
                        // AssetDatabase.SaveAssets();
                        UpdateBottomTabs(true);
                    });


            linkInstantiateModeTab =
                GetInstantiateTab(PrefabInstantiateModeSetting.Link.ToString())
                    .SetTooltip("Link Instantiate Mode\n\nIgnore the default menu item behaviour when instantiating its target object and create a prefab link")
                    .SetTabPosition(TabPosition.TabOnRight)
                    .SetOnClick(() =>
                    {
                        Undo.RecordObject(UIMenuSettings.instance, "Update InstantiateMode");
                        UIMenuSettings.instance.InstantiateMode = PrefabInstantiateModeSetting.Link;
                        EditorUtility.SetDirty(UIMenuSettings.instance);
                        AssetDatabase.SaveAssetIfDirty(UIMenuSettings.instance);
                        // AssetDatabase.SaveAssets();
                        UpdateBottomTabs(true);
                    });


            UpdateBottomTabs(false);

            contentScroller =
                new ScrollView()
                    .SetStyleFlexGrow(1);

            prefabTypes = new List<UIMenuPrefabType>();
            items = new List<UIMenuItem>();
            searchItems = new List<UIMenuItem>();

            itemsContainer =
                new VisualElement();

            searchResults =
                new VisualElement()
                    .SetStyleFlexDirection(FlexDirection.Row)
                    .SetStyleFlexWrap(Wrap.Wrap)
                    .SetStyleDisplay(DisplayStyle.None)
                    .SetStylePadding(DesignUtils.k_Spacing);

            contentScroller.contentContainer
                .AddChild(itemsContainer)
                .AddChild(searchResults);

            UpdateItems();
        }

        private void UpdateBottomTabs(bool animateChange)
        {
            root.schedule.Execute(() =>
            {
                UIMenuSettings settings = UIMenuSettings.instance;
                autoSelectTab?.SetIsOn(settings.SelectNewlyCreatedObjects, animateChange);
                cloneInstantiateModeTab?.SetIsOn(settings.InstantiateMode == PrefabInstantiateModeSetting.Clone, animateChange);
                defaultInstantiateModeTab?.SetIsOn(settings.InstantiateMode == PrefabInstantiateModeSetting.Default, animateChange);
                linkInstantiateModeTab?.SetIsOn(settings.InstantiateMode == PrefabInstantiateModeSetting.Link, animateChange);
            });
        }

        private void UpdateItems()
        {
            prefabTypes.Clear();
            items.Clear();
            itemsContainer.RecycleAndClear();

            foreach (string prefabTypeName in UIMenuItemsDatabase.GetPrefabTypes())
            {
                prefabTypes.Add(new UIMenuPrefabType(prefabTypeName));
                UIMenuPrefabType currentType = prefabTypes.Last();

                foreach (string categoryName in UIMenuItemsDatabase.GetCategories(prefabTypeName))
                {
                    currentType.AddCategory(categoryName);
                    UIMenuCategory currentCategory = currentType.categories.Last();

                    foreach (string prefabName in UIMenuItemsDatabase.GetPrefabNames(prefabTypeName, categoryName))
                    {
                        currentCategory.AddButton(prefabName);
                        items.Add(currentCategory.buttons.Last().menuItem);
                    }
                }

                itemsContainer.AddChild(currentType);
            }
        }

        private void Compose()
        {
            root
                .AddChild(DesignUtils.spaceBlock2X)
                .AddChild
                (
                    DesignUtils.row
                        .SetStyleFlexGrow(0)
                        .SetStyleAlignItems(Align.Center)
                        .AddChild(uiMenuHeader.SetStyleFlexShrink(0))
                        .AddChild(DesignUtils.flexibleSpace)
                        .AddChild(searchBox.SetStyleFlexShrink(0))
                )
                .AddChild(DesignUtils.spaceBlock2X)
                .AddChild(contentScroller)
                .AddChild
                (
                    DesignUtils.row
                        .SetStyleFlexShrink(0)
                        .SetStyleFlexGrow(0)
                        .SetStyleAlignItems(Align.Center)
                        .SetStyleBackgroundColor(EditorColors.Default.BoxBackground)
                        .SetStylePadding(DesignUtils.k_Spacing2X)
                        .AddChild(regenerateButton)
                        .AddChild(DesignUtils.spaceBlock)
                        .AddChild(refreshButton)
                        .AddChild(DesignUtils.spaceBlock2X)
                        .AddChild(autoSelectTab)
                        .AddChild(DesignUtils.flexibleSpace)
                        .AddChild(DesignUtils.spaceBlock2X)
                        .AddChild(cloneInstantiateModeTab)
                        .AddSpace(2, 0)
                        .AddChild(defaultInstantiateModeTab)
                        .AddSpace(2, 0)
                        .AddChild(linkInstantiateModeTab)
                )
                ;
        }

        #region Search

        public bool isSearching => searchBox.isSearching;
        public string searchPattern => searchBox.searchPattern;
        public bool hasSearchResults { get; private set; }
        public VisualElement searchResults { get; set; }

        public void ClearSearch()
        {
            hasSearchResults = false;
            Search(string.Empty);
        }

        public void Search(string pattern)
        {
            UpdateSearchResults();
        }

        public void UpdateSearchResults()
        {
            searchResults.RecycleAndClear();
            searchItems.Clear();

            if (isSearching)
            {
                foreach (UIMenuItem item in items)
                {
                    bool foundMatch = false;

                    //check prefab type
                    foundMatch = Regex.IsMatch(item.prefabTypeName, searchBox.searchPattern, RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
                    if (foundMatch)
                    {
                        searchItems.Add(item);
                        continue;
                    }
                    
                    //check prefab category
                    foundMatch = Regex.IsMatch(item.prefabCategory, searchBox.searchPattern, RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
                    if (foundMatch)
                    {
                        searchItems.Add(item);
                        continue;
                    }
                    
                    //check prefab name
                    foundMatch = Regex.IsMatch(item.prefabName, searchBox.searchPattern, RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
                    if (foundMatch)
                    {
                        searchItems.Add(item);
                        continue;
                    }

                    //check tags
                    foreach (string tag in item.tags)
                    {
                        foundMatch = Regex.IsMatch(tag, searchBox.searchPattern, RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
                        if (foundMatch)
                        {
                            searchItems.Add(item);
                            break;
                        }
                    }
                }
            }

            hasSearchResults = searchItems.Count > 0;

            itemsContainer.SetStyleDisplay(isSearching ? DisplayStyle.None : DisplayStyle.Flex);
            searchResults.SetStyleDisplay(isSearching ? DisplayStyle.Flex : DisplayStyle.None);


            if (hasSearchResults)
            {
                foreach (UIMenuItem item in searchItems)
                    searchResults.AddChild(new UIMenuItemButton().SetUIMenuItem(item));
                return;
            }

            if (isSearching & !hasSearchResults)
            {
                searchResults.AddChild(searchBox.EmptySearchPlaceholderElement().SetStyleMarginTop(DesignUtils.k_Spacing2X));
                return;
            }

            UpdateItems();
        }

        #endregion
        
        private class UIMenuPrefabType : FluidFoldout
        {
            public override void Dispose()
            {
                base.Dispose();

                foreach (UIMenuCategory category in categories)
                    category?.Dispose();
            }

            private string editorPrefsKey => $"UIMenu.{prefabTypeName.RemoveWhitespaces().RemoveAllSpecialCharacters()}";

            public string prefabTypeName { get; }
            private VisualElement categoriesContainer { get; set; }
            public List<UIMenuCategory> categories { get; private set; }

            public UIMenuPrefabType(string prefabTypeName)
            {
                this.prefabTypeName = prefabTypeName;
                categories = new List<UIMenuCategory>();

                categoriesContainer =
                    new VisualElement()
                        .SetStyleMarginBottom(DesignUtils.k_Spacing2X);

                this.AddContent(categoriesContainer);

                this
                    .SetName(prefabTypeName)
                    .SetLabelText(prefabTypeName)
                    .SetElementSize(ElementSize.Normal)
                    .SetContentPadding(0)
                    .SetContentLeftPadding()
                    .SetStyleMarginBottom(DesignUtils.k_Spacing2X)
                    .SetIsOn(EditorPrefs.GetBool(editorPrefsKey), false);

                animatedContainer.Toggle(EditorPrefs.GetBool(editorPrefsKey), false);
                tabButton.OnValueChanged += value => EditorPrefs.SetBool(editorPrefsKey, value.newValue);
            }

            public UIMenuPrefabType AddCategory(string categoryName)
            {
                if (categoryName.IsNullOrEmpty()) return this;
                bool containsCategory = categories.Any(c => c.categoryName.Equals(categoryName));
                if (containsCategory) return this;
                categories.Add(new UIMenuCategory(prefabTypeName, categoryName));
                categories = categories.OrderBy(c => c.categoryName).ToList();
                categoriesContainer.AddChild(categories.Last());
                return this;
            }
        }

        private class UIMenuCategory : FluidFoldout
        {
            public override void Dispose()
            {
                base.Dispose();

                foreach (UIMenuItemButton button in buttons)
                    button?.Dispose();
            }

            private string editorPrefsKey => $"UIMenu.{prefabTypeName.RemoveWhitespaces().RemoveAllSpecialCharacters()}.{categoryName.RemoveWhitespaces().RemoveAllSpecialCharacters()}";

            public string prefabTypeName { get; }
            public string categoryName { get; }
            private VisualElement buttonsContainer { get; set; }
            public List<UIMenuItemButton> buttons { get; private set; }

            public UIMenuCategory(string prefabTypeName, string categoryName)
            {
                this.prefabTypeName = prefabTypeName;
                this.categoryName = categoryName;
                buttons = new List<UIMenuItemButton>();

                buttonsContainer =
                    new VisualElement()
                        .SetStyleFlexDirection(FlexDirection.Row)
                        .SetStyleFlexWrap(Wrap.Wrap)
                        .SetStyleMarginBottom(DesignUtils.k_EndOfLineSpacing);

                animatedContainer.AddContent(buttonsContainer);

                this
                    .SetName(categoryName)
                    .SetLabelText(categoryName)
                    .SetElementSize(ElementSize.Small)
                    .SetContentPadding()
                    .SetStyleMarginTop(DesignUtils.k_Spacing)
                    .SetIsOn(EditorPrefs.GetBool(editorPrefsKey), false);

                animatedContainer.Toggle(EditorPrefs.GetBool(editorPrefsKey), false);
                tabButton.OnValueChanged += value => EditorPrefs.SetBool(editorPrefsKey, value.newValue);
            }

            public UIMenuCategory AddButton(string prefabName)
            {
                UIMenuItem item = UIMenuItemsDatabase.GetMenuItem(prefabTypeName, categoryName, prefabName);
                return item == null ? this : AddButton(new UIMenuItemButton().SetUIMenuItem(item));
            }

            public UIMenuCategory AddButton(UIMenuItemButton button)
            {
                if (button == null) return this;
                if (buttons.Contains(button)) return this;
                buttons.Add(button);
                buttonsContainer.Add(button);
                return this;
            }

        }
    }
}
