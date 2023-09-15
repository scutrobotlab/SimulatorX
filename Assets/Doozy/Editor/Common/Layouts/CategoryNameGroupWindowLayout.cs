// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.Components.Internal;
using Doozy.Editor.EditorUI.ScriptableObjects.Colors;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Editor.UIElements;
using Doozy.Runtime.Common;
using Doozy.Runtime.Common.Extensions;
using Doozy.Runtime.UIElements.Extensions;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Editor.Common.Layouts
{
    public abstract class CategoryNameGroupWindowLayout : FluidWindowLayout
    {
        protected virtual UnityEngine.Object targetObject => null;
        protected virtual UnityAction onUpdateCallback => null;
        protected virtual CategoryNameGroup<CategoryNameItem> database => null;
        protected virtual string groupTypeName => "Group Type Name";

        protected virtual Func<string, List<string>, bool> exportDatabaseHandler => null;
        protected virtual Func<List<ScriptableObject>, bool> importDatabaseHandler => null;
        protected virtual string roamingDatabaseTypeName => null;
        private FluidPlaceholder placeholderNoRoamingDatabaseFound { get; set; }
        private FluidPlaceholder placeholderExportEmptyDatabase { get; set; }

        protected bool regenerateEnumOnDisable { get; set; }
        protected virtual UnityAction runEnumGenerator => null;
        private FluidButton generateEnumButton { get; set; }

        private static string defaultCategory => CategoryNameItem.k_DefaultCategory;
        private static string defaultName => CategoryNameItem.k_DefaultName;

        private bool databaseIsEmpty => database.isEmpty;

        private FluidPlaceholder placeholderEmptyDatabase { get; set; }
        private Dictionary<string, FluidToggleButtonTab> categoryButtons { get; set; }

        private string selectedCategoryName { get; set; }

        private FluidListView fluidListView { get; set; }
        private readonly List<CategoryNameItem> m_Items = new List<CategoryNameItem>();

        private static Color titleTextColor => EditorColors.Default.TextDescription;
        private static Font titleFont => EditorFonts.Ubuntu.Light;
        private static int titleFontSize => 16;

        private bool initialized { get; set; }

        public override void OnDisable()
        {
            base.OnDisable();

            if (regenerateEnumOnDisable)
                runEnumGenerator?.Invoke();
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            Undo.undoRedoPerformed -= UndoRedoPerformed;
            RecycleNames(searchResults.contentContainer, true);
        }

        private void UndoRedoPerformed()
        {
            string currentCategoryName = selectedCategoryName;
            {
                UpdateSearchResults();
                UpdateDatabase();
            }

            if (buttonImportExport.isOn) return;
            if (sideMenu.searchBox.isSearching) return;
            if (!database.ContainsCategory(currentCategoryName)) return;
            categoryButtons[currentCategoryName].SetIsOn(true);
        }

        protected void Initialize()
        {
            if (!initialized)
            {
                InitializeCreateNewIdContainer();
                InitializeImportExport();

                sideMenu.toolbarContainer.SetStyleDisplay(DisplayStyle.Flex);
                sideMenu.toolbarContainer.Add(buttonImportExport);

                generateEnumButton =
                    FluidButton.Get()
                        .SetLabelText("Regenerate Enums")
                        .SetTooltip("Regenerate enum database accessors")
                        .SetIcon(EditorMicroAnimations.EditorUI.Icons.EmptyList)
                        .SetElementSize(ElementSize.Tiny)
                        .SetButtonStyle(ButtonStyle.Contained)
                        .SetOnClick(() => runEnumGenerator?.Invoke());

                sideMenu.toolbarContainer.Add(DesignUtils.spaceBlock2X);
                sideMenu.toolbarContainer.Add(generateEnumButton);


                placeholderEmptyDatabase =
                    FluidPlaceholder.Get("Empty Database", EditorMicroAnimations.EditorUI.Placeholders.EmptyDatabase)
                        .Hide();

                selectedCategoryName = string.Empty;
                categoryButtons = new Dictionary<string, FluidToggleButtonTab>();


                fluidListView = new FluidListView();
                fluidListView.listView.selectionType = SelectionType.None;
                fluidListView.listView.makeItem = () => CategoryNameItemNameRow.Get();
                fluidListView.listView.bindItem = (element, i) =>
                    ((CategoryNameItemNameRow)element)
                    .SetTarget(m_Items[i])
                    .SetSaveHandler(ItemSaveHandler)
                    .SetRemoveHandler(ItemRemoveHandler);

                #if UNITY_2021_2_OR_NEWER
                fluidListView.listView.fixedItemHeight = 30;
                #else
                fluidListView.listView.itemHeight = 30;
                #endif

                fluidListView
                    .SetItemsSource(m_Items)
                    .SetDynamicListHeight(true)
                    .HideAddNewItemButton(); //HIDE ADD NEW ITEM BUTTON (plus button)


                Undo.undoRedoPerformed += UndoRedoPerformed;
                initialized = true;
            }

            UpdateDatabase();
        }

        private bool ShowEmptyDatabase()
        {
            if (!databaseIsEmpty)
                return false; //database is NOT empty

            content.Clear();
            content
                .AddChild(createNewIdContainer.SetStyleMarginBottom(DesignUtils.k_Spacing * 4))
                .AddChild(placeholderEmptyDatabase);

            UpdateCreateNewIdContainer();

            return true; // database is empty
        }

        private void UpdateDatabase()
        {
            // Debug.Log($"{nameof(UpdateDatabase)}");

            database.CleanDatabase();
            onUpdateCallback?.Invoke();

            foreach (FluidToggleButtonTab categoryButton in categoryButtons.Values) categoryButton?.Recycle();
            categoryButtons.Clear();

            sideMenu.searchBox.SetEnabled(!databaseIsEmpty);

            if (ShowEmptyDatabase())
                return;

            foreach (string category in database.GetCategories())
            {
                if (category.Equals(defaultCategory))
                    continue;

                FluidToggleButtonTab buttonTab = sideMenu.AddButton(category, selectableAccentColor);
                buttonTab.SetOnValueChanged(evt =>
                {
                    if (!evt.newValue) return;
                    selectedCategoryName = category;
                    content.Clear();
                    m_Items.Clear();
                    foreach (CategoryNameItem item in database.items.Where(item => item.category.Equals(category))) m_Items.Add(item);
                    content
                        .AddChild(createNewIdContainer.SetStyleMarginBottom(DesignUtils.k_Spacing * 4))
                        .AddChild(new CategoryNameItemCategoryRow().SetTarget(category).SetRemoveHandler(CategoryRemoveHandler))
                        .AddChild(fluidListView);
                    fluidListView.Update();

                    UpdateCreateNewIdContainer();

                    if (buttonImportExport.isOn) buttonImportExport.SetIsOn(false);
                });
                categoryButtons.Add(category, buttonTab);
            }
        }

        private static void RecycleNames(VisualElement targetContainer, bool clearTargetContainer)
        {
            targetContainer.RecycleIPoolableChildren(clearTargetContainer);
        }

        #region Import/Export

        private FluidToggleButtonTab buttonImportExport { get; set; }

        private VisualElement importExportContainer { get; set; }

        private VisualElement exportColumn { get; set; }
        private FluidButton buttonExport { get; set; }
        private TextField exportDatabaseNameTextField { get; set; }
        private FluidField exportField { get; set; }
        private FluidToggleGroup exportToggleGroup { get; set; }
        private ScrollView exportScrollableContent { get; set; }
        private Dictionary<string, FluidToggleCheckbox> exportToggles { get; set; }

        private VisualElement importColumn { get; set; }
        private FluidButton buttonImport { get; set; }
        private FluidButton buttonSearchForRoamingDatabases { get; set; }
        private FluidToggleGroup importToggleGroup { get; set; }
        private ScrollView importScrollableContent { get; set; }
        private Dictionary<ScriptableObject, FluidToggleCheckbox> importToggles { get; set; }

        private static EditorSelectableColorInfo selectableActionColor => EditorSelectableColors.Default.Action;

        private static FluidButton NewButtonImportExport(string labelText, string tooltipText, IEnumerable<Texture2D> textures) =>
            FluidButton.Get()
                .SetLabelText(labelText)
                .SetTooltip(tooltipText)
                .SetIcon(textures)
                .SetElementSize(ElementSize.Normal)
                .SetButtonStyle(ButtonStyle.Contained)
                .SetStyleFlexGrow(0)
                .SetStyleAlignSelf(Align.Center)
                .SetAccentColor(selectableActionColor);

        private static FluidButton NewPingAssetButton() =>
            FluidButton.Get()
                .SetStyleAlignSelf(Align.Center)
                .SetElementSize(ElementSize.Tiny)
                .SetIcon(EditorMicroAnimations.EditorUI.Icons.Location)
                .SetTooltip("Ping the asset in the Project view");

        private static FluidButton NewDeleteAssetButton() =>
            FluidButton.Get()
                .SetStyleAlignSelf(Align.Center)
                .SetElementSize(ElementSize.Tiny)
                .SetIcon(EditorMicroAnimations.EditorUI.Icons.Minus)
                .SetAccentColor(EditorSelectableColors.Default.Remove)
                .SetTooltip("Delete Asset");

        private void InitializeImportExport()
        {
            placeholderNoRoamingDatabaseFound =
                FluidPlaceholder.Get("No roaming databases found", EditorMicroAnimations.EditorUI.Placeholders.EmptySearch)
                    .Hide();

            placeholderExportEmptyDatabase =
                FluidPlaceholder.Get("Empty Database", EditorMicroAnimations.EditorUI.Placeholders.EmptyDatabase)
                    .Hide();

            buttonImportExport = FluidToggleButtonTab.Get()
                .SetLabelText("Import/Export").SetTooltip("Import/Export roaming databases")
                .SetIcon(EditorMicroAnimations.EditorUI.Icons.GenericDatabase)
                .SetElementSize(ElementSize.Small)
                .SetOnValueChanged(change =>
                {
                    sideMenu.buttonsScrollViewContainer.SetEnabled(!change.newValue);
                    sideMenu.searchBox.SetEnabled(!change.newValue);

                    if (change.newValue)
                    {
                        content.Clear();
                        content.Add(importExportContainer);
                        UpdateExport();
                        UpdateImport();
                        return;
                    }

                    if (databaseIsEmpty)
                    {
                        UpdateDatabase();
                        return;
                    }

                    string currentCategoryName = selectedCategoryName;
                    sideMenu.searchBox.ClearSearch();
                    if (categoryButtons.ContainsKey(currentCategoryName))
                    {
                        categoryButtons[currentCategoryName].SetIsOn(true);
                        return;
                    }
                    UpdateDatabase();
                });

            buttonExport =
                NewButtonImportExport("Export", "Export selected categories", EditorMicroAnimations.EditorUI.Icons.Export)
                    .SetOnClick(() =>
                    {
                        if (exportToggles == null || exportToggles.Values.Count == 0)
                        {
                            EditorUtility.DisplayDialog("Export", "There is nothing to export, the database is empty...", "Ok");
                            return;
                        }

                        _ = exportDatabaseHandler ?? throw new NullReferenceException(nameof(exportDatabaseHandler));

                        var categoriesForExport = exportToggles.Keys.Where(category => exportToggles[category].isOn).ToList();
                        bool success = exportDatabaseHandler.Invoke(exportDatabaseNameTextField.value, categoriesForExport);
                        if (!success) return;
                        foreach (FluidToggleCheckbox toggle in exportToggles.Values)
                            toggle.SetIsOn(false);
                        exportDatabaseNameTextField.value = string.Empty;
                        UpdateImport();
                    });

            buttonImport = NewButtonImportExport("Import", "Import selected roaming databases", EditorMicroAnimations.EditorUI.Icons.Import)
                .SetOnClick(() =>
                {
                    if (importToggles == null || importToggles.Values.Count == 0)
                    {
                        EditorUtility.DisplayDialog("Import", "There is nothing to import.\n\nNo roaming databases were found in this project...", "Ok");
                        return;
                    }

                    _ = importDatabaseHandler ?? throw new NullReferenceException(nameof(importDatabaseHandler));

                    var databasesForImport = importToggles.Keys.Where(roamingDatabase => importToggles[roamingDatabase].isOn).ToList();
                    bool success = importDatabaseHandler.Invoke(databasesForImport);
                    if (!success) return;
                    foreach (FluidToggleCheckbox toggle in importToggles.Values)
                        toggle.SetIsOn(false);
                    UpdateDatabase();
                    buttonImportExport.isOn = true;
                    UpdateExport();
                });

            buttonSearchForRoamingDatabases =
                FluidButton.Get("Search for Databases", "Search through the entire project for roaming databases")
                    .SetStyleFlexGrow(0)
                    .SetStyleAlignSelf(Align.Center)
                    .SetStyleMargins(DesignUtils.k_Spacing)
                    .SetElementSize(ElementSize.Tiny)
                    .SetButtonStyle(ButtonStyle.Contained)
                    .SetIcon(EditorMicroAnimations.EditorUI.Icons.Search)
                    .SetOnClick(UpdateImport);

            exportDatabaseNameTextField = new TextField().ResetLayout();
            exportField = FluidField.Get("Database Name").AddFieldContent(exportDatabaseNameTextField);

            exportToggleGroup = new FluidToggleGroup().SetLabelText("Select All").SetControlMode(FluidToggleGroup.ControlMode.Passive);
            importToggleGroup = new FluidToggleGroup().SetLabelText("Select All").SetControlMode(FluidToggleGroup.ControlMode.Passive).SetStyleDisplay(DisplayStyle.None);

            exportScrollableContent = new ScrollView();
            importScrollableContent = new ScrollView();

            exportToggles = new Dictionary<string, FluidToggleCheckbox>();
            importToggles = new Dictionary<ScriptableObject, FluidToggleCheckbox>();

            importExportContainer =
                DesignUtils.row
                    .AddChild(exportColumn = DesignUtils.column)
                    .AddChild(DesignUtils.column
                        .SetStyleBackgroundColor(EditorColors.Default.FieldBackground)
                        .SetStyleWidth(1, 1, 1)
                        .SetStyleMargins(DesignUtils.k_Spacing * 4))
                    .AddChild(importColumn = DesignUtils.column);

            exportColumn
                .AddChild
                (
                    DesignUtils.row.SetStyleFlexGrow(0).SetStyleHeight(40)
                        .AddChild(exportField)
                        .AddSpace(DesignUtils.k_Spacing, 0)
                        .AddChild(buttonExport)
                )
                .AddSpace(0, DesignUtils.k_Spacing * 2)
                .AddChild(exportToggleGroup)
                .AddSpace(0, DesignUtils.k_Spacing)
                .AddChild(exportScrollableContent)
                .AddChild(placeholderExportEmptyDatabase);


            exportDatabaseNameTextField.RegisterValueChangedCallback(evt => OnExportTextFieldValueChanges(evt.newValue));
            OnExportTextFieldValueChanges(exportDatabaseNameTextField.value);

            void OnExportTextFieldValueChanges(string newValue)
            {
                bool isValid = !newValue.RemoveWhitespaces().RemoveAllSpecialCharacters().IsNullOrEmpty();
                exportToggleGroup.SetEnabled(isValid);
                exportScrollableContent.SetEnabled(isValid);
            }

            importColumn
                .AddChild
                (
                    DesignUtils.row.SetStyleFlexGrow(0).SetStyleHeight(40)
                        .AddChild(buttonImport)
                        .AddChild(DesignUtils.flexibleSpace)
                        .AddChild(buttonSearchForRoamingDatabases)
                )
                .AddSpace(0, DesignUtils.k_Spacing * 2)
                .AddChild(importToggleGroup)
                .AddSpace(0, DesignUtils.k_Spacing)
                .AddChild(importScrollableContent)
                .AddChild(placeholderNoRoamingDatabaseFound);
        }

        private void UpdateExport()
        {
            foreach (FluidToggleCheckbox exportToggle in exportToggles.Values)
                exportToggle?.Recycle();

            exportScrollableContent.Clear();
            exportToggles.Clear();
            exportDatabaseNameTextField.value = string.Empty;

            foreach (string category in database.GetCategories())
            {
                if (category.Equals(defaultCategory)) continue;
                FluidToggleCheckbox toggle = FluidToggleCheckbox.Get().SetLabelText(category).SetToggleAccentColor(selectableActionColor);
                toggle.AddToToggleGroup(exportToggleGroup);
                exportScrollableContent.AddChild(toggle);
                exportToggles.Add(category, toggle);
            }

            bool isEmpty = exportToggles.Values.Count == 0;
            exportToggleGroup.SetStyleDisplay(isEmpty ? DisplayStyle.None : DisplayStyle.Flex);
            placeholderExportEmptyDatabase.Toggle(isEmpty);
            if (isEmpty) placeholderExportEmptyDatabase.Play();
        }

        private void UpdateImport()
        {
            foreach (FluidToggleCheckbox importToggle in importToggles.Values)
                importToggle?.Recycle();

            importScrollableContent.Clear();
            importToggles.Clear();

            string[] guids = AssetDatabase.FindAssets($"t:{roamingDatabaseTypeName}");
            bool isEmpty = guids.Length == 0;
            if (!isEmpty)
                importScrollableContent
                    .AddChild
                    (
                        DesignUtils.flexibleSpace
                            .SetStyleBackgroundColor(EditorColors.Default.FieldBackground)
                            .SetStyleHeight(1, 1, 1)
                            .SetStyleMargins(DesignUtils.k_Spacing / 2f)
                    );

            foreach (string guid in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                ScriptableObject asset = AssetDatabase.LoadAssetAtPath<ScriptableObject>(assetPath);
                string assetName = asset.name;
                string assetPrettyName = assetName.RemoveAllSpecialCharacters().Replace(roamingDatabaseTypeName, "");
                FluidToggleCheckbox toggle = FluidToggleCheckbox.Get().SetLabelText(assetPrettyName).SetToggleAccentColor(selectableActionColor);
                FluidButton pingAssetButton = NewPingAssetButton().SetOnClick(() => EditorGUIUtility.PingObject(asset));
                FluidButton deleteAssetButton = NewDeleteAssetButton().SetOnClick(() =>
                {
                    if (!EditorUtility.DisplayDialog("Delete Asset", $"Are you sure you want to delete the '{assetPrettyName}' database?", "Yes", "Cancel"))
                        return;
                    AssetDatabase.MoveAssetToTrash(assetPath);
                    Debug.Log($"Asset '{assetPrettyName}' database moved to trash! ({assetPath})");
                    UpdateImport();
                });
                toggle.AddToToggleGroup(importToggleGroup);
                importScrollableContent
                    .AddChild
                    (
                        DesignUtils.row
                            .AddChild(toggle)
                            .AddChild(DesignUtils.flexibleSpace)
                            .AddChild(pingAssetButton)
                            .AddChild(deleteAssetButton)
                    )
                    .AddChild(DesignUtils.flexibleSpace
                        .SetStyleBackgroundColor(EditorColors.Default.FieldBackground)
                        .SetStyleHeight(1, 1, 1)
                        .SetStyleMargins(DesignUtils.k_Spacing / 2f));
                importToggles.Add(asset, toggle);
            }

            placeholderNoRoamingDatabaseFound.Toggle(isEmpty);
            if (isEmpty) placeholderNoRoamingDatabaseFound.Play();
        }

        #endregion

        #region Handlers

        private void CategoryRemoveHandler(string targetCategory)
        {
            bool canRemoveCategory;
            string message;
            (canRemoveCategory, message) = database.CanRemoveCategory(targetCategory);
            if (!canRemoveCategory)
            {
                EditorUtility.DisplayDialog("Attention Required", message, "Ok");
                return;
            }
            if (!EditorUtility.DisplayDialog
                (
                    "Confirmation",
                    $"Are you sure you want to remove the '{targetCategory}' category",
                    "Yes",
                    "Cancel"
                )
            ) return;

            regenerateEnumOnDisable = true;
            Undo.RecordObject(targetObject, "Remove Category");
            database.RemoveCategory(targetCategory);
            EditorUtility.SetDirty(targetObject);
            AssetDatabase.SaveAssetIfDirty(targetObject);
            // AssetDatabase.SaveAssets();
            UpdateDatabase();
            schedule.Execute(() => sideMenu.buttons.First()?.SetIsOn(true));
        }

        private void ItemRemoveHandler(CategoryNameItem targetItem)
        {
            bool canRemoveName;
            string message;
            string targetCategory = targetItem.category;
            (canRemoveName, message) = database.CanRemoveName(targetCategory, targetItem.name);
            if (!canRemoveName)
            {
                EditorUtility.DisplayDialog("Attention Required", message, "Ok");
                return;
            }
            if (!EditorUtility.DisplayDialog
                (
                    "Confirmation",
                    $"Are you sure you want to remove the '{targetItem.name}' from the '{targetItem.category}' category",
                    "Yes",
                    "Cancel"
                )
            ) return;

            regenerateEnumOnDisable = true;
            Undo.RecordObject(targetObject, "Remove Item");
            database.RemoveName(targetCategory, targetItem.name);
            EditorUtility.SetDirty(targetObject);
            AssetDatabase.SaveAssetIfDirty(targetObject);
            // AssetDatabase.SaveAssets();
            onUpdateCallback?.Invoke();
            if (sideMenu.searchBox.isSearching)
            {
                UpdateSearchResults();
                return;
            }

            if (database.ContainsCategory(targetCategory))
            {
                categoryButtons[targetCategory].SetIsOn(true);
                return;
            }

            UpdateDatabase();
            schedule.Execute(() => sideMenu.buttons.First()?.SetIsOn(true));
        }

        private bool ItemSaveHandler(CategoryNameItem targetItem, string newName)
        {
            bool canAddName;
            string message;
            string targetCategory = targetItem.category;
            (canAddName, message) = database.CanAddName(targetCategory, newName);
            if (!canAddName)
            {
                EditorUtility.DisplayDialog("Attention Required", message, "Ok");
                return false;
            }

            regenerateEnumOnDisable = true;
            Undo.RecordObject(targetObject, "Add Item");
            database.AddName(targetCategory, newName);
            database.RemoveName(targetCategory, targetItem.name);
            EditorUtility.SetDirty(targetObject);
            AssetDatabase.SaveAssetIfDirty(targetObject);
            // AssetDatabase.SaveAssets();
            if (sideMenu.searchBox.isSearching)
            {
                UpdateSearchResults();
                return true;
            }

            UpdateDatabase();
            categoryButtons[targetCategory].SetIsOn(true);
            return true;
        }

        #endregion

        #region Create New Id Container

        private static EditorSelectableColorInfo selectableAddColor => EditorSelectableColors.Default.Add;
        private static EditorSelectableColorInfo selectableRemoveColor => EditorSelectableColors.Default.Remove;

        private VisualElement createNewIdContainer { get; set; }
        private List<string> categoriesList { get; set; }
        private PopupField<string> newIdCategoryPopupField { get; set; }
        private TextField newIdCategoryTextField { get; set; }
        private TextField newIdNameTextField { get; set; }
        private FluidToggleButtonTab buttonNewIdEditCategoryName { get; set; }
        private FluidField newIdCategoryField { get; set; }
        private FluidField newIdNameField { get; set; }
        private FluidButton buttonCreateNewId { get; set; }

        private void InitializeCreateNewIdContainer()
        {
            categoriesList = new List<string> { string.Empty };
            newIdCategoryPopupField = new PopupField<string>(categoriesList, 0).ResetLayout();
            newIdCategoryTextField = new TextField().ResetLayout();
            newIdNameTextField = new TextField().ResetLayout();

            newIdNameTextField.RegisterCallback<KeyUpEvent>(keyUpEvent =>
            {
                if (!newIdNameTextField.IsFocused()) return;
                switch (keyUpEvent.keyCode)
                {
                    case KeyCode.Return:
                        buttonCreateNewId?.ExecuteOnClick();
                        return;
                    case KeyCode.Escape:
                        if (buttonNewIdEditCategoryName.isOn)
                            newIdCategoryTextField.value = string.Empty;
                        newIdNameTextField.value = string.Empty;
                        return;
                    default:
                        return;
                }
            });

            buttonNewIdEditCategoryName =
                FluidToggleButtonTab.Get()
                    .SetElementSize(ElementSize.Tiny)
                    .SetIcon(EditorMicroAnimations.EditorUI.Icons.Edit)
                    .SetToggleAccentColor(selectableAddColor)
                    .SetTooltip($"Create a new {groupTypeName} category")
                    .SetStyleMarginLeft(DesignUtils.k_Spacing);

            newIdCategoryField =
                FluidField.Get($"{groupTypeName} Category")
                    .AddFieldContent
                    (
                        DesignUtils.row.SetStyleAlignItems(Align.Center)
                            .AddChild
                            (
                                DesignUtils.column
                                    .AddChild(newIdCategoryPopupField)
                                    .AddChild(newIdCategoryTextField)
                            )
                            .AddChild(buttonNewIdEditCategoryName)
                    );

            newIdNameField = FluidField.Get($"New {groupTypeName} Name")
                .AddFieldContent(newIdNameTextField);


            buttonCreateNewId =
                FluidButton.Get().SetButtonStyle(ButtonStyle.Contained).SetElementSize(ElementSize.Small).SetStyleAlignSelf(Align.Center)
                    .SetIcon(EditorMicroAnimations.EditorUI.Icons.Plus)
                    .SetAccentColor(selectableAddColor)
                    .SetTooltip($"Create a new {groupTypeName} Id")
                    .SetOnClick(() =>
                    {
                        bool canAddNewStreamId;
                        string message;
                        (canAddNewStreamId, message) = database.CanAddName(newIdCategoryTextField.value, newIdNameTextField.value);

                        if (!canAddNewStreamId)
                        {
                            EditorUtility.DisplayDialog($"New {groupTypeName} Id", message, "Ok");
                            return;
                        }

                        regenerateEnumOnDisable = true;
                        Undo.RecordObject(targetObject, $"New {groupTypeName} Id");
                        string categoryName = newIdCategoryTextField.value;
                        bool createdNewCategory = !database.ContainsCategory(categoryName);
                        bool result = database.AddName(categoryName, newIdNameTextField.value);
                        if (!result) return;
                        newIdNameTextField.value = string.Empty;
                        EditorUtility.SetDirty(targetObject);
                        AssetDatabase.SaveAssetIfDirty(targetObject);
                        // AssetDatabase.SaveAssets();
                        string currentCategoryName = createdNewCategory ? categoryName : selectedCategoryName;
                        UpdateDatabase();
                        categoryButtons[currentCategoryName].SetIsOn(true);
                    });

            createNewIdContainer = new VisualElement().SetStyleMargins(2, 0, 2, 0);

            createNewIdContainer.RegisterCallback<GeometryChangedEvent>(evt =>
                createNewIdContainer.SetStyleFlexDirection(createNewIdContainer.resolvedStyle.width > 400 ? FlexDirection.Row : FlexDirection.Column));

            createNewIdContainer
                .AddChild(newIdCategoryField.SetStyleMargins(DesignUtils.k_Spacing / 2f))
                .AddChild(newIdNameField.SetStyleMargins(DesignUtils.k_Spacing / 2f))
                .AddChild(buttonCreateNewId.SetStyleMargins(DesignUtils.k_Spacing));


            newIdCategoryTextField.RegisterValueChangedCallback(evt =>
            {
                bool categoryIsValid = !buttonNewIdEditCategoryName.isOn || buttonNewIdEditCategoryName.isOn & !evt.newValue.Trim().IsNullOrEmpty();
                bool nameIsValid = !newIdNameTextField.value.Trim().IsNullOrEmpty();
                newIdNameTextField.SetEnabled(categoryIsValid | nameIsValid);
                buttonCreateNewId.SetEnabled(categoryIsValid & nameIsValid);
            });

            newIdNameTextField.RegisterValueChangedCallback(evt =>
            {
                bool categoryIsValid = !buttonNewIdEditCategoryName.isOn || !newIdCategoryTextField.value.Trim().IsNullOrEmpty();
                bool nameIsValid = !evt.newValue.Trim().IsNullOrEmpty();
                buttonCreateNewId.SetEnabled(categoryIsValid & nameIsValid);
            });

            newIdCategoryPopupField.RegisterValueChangedCallback(evt =>
            {
                newIdCategoryTextField.value = evt.newValue;
            });

            buttonNewIdEditCategoryName.SetOnValueChanged(change =>
            {
                newIdCategoryPopupField.SetStyleDisplay(databaseIsEmpty || change.newValue ? DisplayStyle.None : DisplayStyle.Flex);
                newIdCategoryTextField.SetStyleDisplay(databaseIsEmpty || change.newValue ? DisplayStyle.Flex : DisplayStyle.None);
                newIdCategoryField.SetLabelText($"{(databaseIsEmpty | change.newValue ? "New " : "")}{groupTypeName} Category");

                if (!change.newValue & databaseIsEmpty)
                {
                    buttonNewIdEditCategoryName.isOn = true;
                    newIdCategoryTextField.value = string.Empty;
                    return;
                }

                if (change.newValue && databaseIsEmpty)
                    newIdCategoryTextField.Focus();

            });
        }

        private void UpdateCreateNewIdContainer()
        {
            categoriesList.Clear();
            if (databaseIsEmpty) categoriesList.Add(string.Empty);
            else categoriesList.AddRange(database.GetCategories());

            newIdCategoryPopupField.value = databaseIsEmpty ? string.Empty : selectedCategoryName;
            buttonNewIdEditCategoryName.isOn = databaseIsEmpty;

            newIdNameTextField.value = string.Empty;
            newIdNameTextField.SetEnabled(!databaseIsEmpty);
            buttonCreateNewId.SetEnabled(false);
        }

        #endregion

        #region Search

        private List<CategoryNameItem> m_SearchResultsItems = new List<CategoryNameItem>();

        protected override void UpdateSearchResults()
        {
            if (!sideMenu.searchBox.isSearching)
                return;

            //CLEAR << CONTENT container
            content.Clear();
            //CLEAR << SEARCH RESULTS container
            RecycleNames(searchResults.contentContainer, true);

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

            m_SearchResultsItems.Clear();
            foreach (CategoryNameItem item in database.items)
            {
                if (!Regex.IsMatch(item.name, $"{sideMenu.searchBox.searchPattern}", RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace))
                    continue;
                m_SearchResultsItems.Add(item);
            }

            bool hasSearchResults = m_SearchResultsItems.Count > 0;

            //CHECK FLAG for SEARCH RESULTS
            if (!hasSearchResults)
            {
                //NO SEARCH RESULTS <<< show search visual
                content.Add(sideMenu.searchBox.EmptySearchPlaceholderElement());
                //STOP
                return;
            }

            m_SearchResultsItems =
                m_SearchResultsItems
                    .OrderBy(item => item.category)
                    .ThenBy(item => item.name)
                    .ToList();

            string category = string.Empty;
            foreach (CategoryNameItem resultsItem in m_SearchResultsItems)
            {
                if (!category.Equals(resultsItem.category))
                {
                    if (!category.IsNullOrEmpty()) searchResults.AddSpace(0, DesignUtils.k_Spacing * 4);
                    category = resultsItem.category;
                    CategoryNameItemCategoryRow categoryRow = new CategoryNameItemCategoryRow().SetTarget(category).HideRemoveCategoryButton();
                    categoryRow.layoutContainer.SetStyleMarginBottom(DesignUtils.k_Spacing);
                    searchResults.Add(categoryRow);
                }

                searchResults.Add(CategoryNameItemNameRow.Get(resultsItem, ItemSaveHandler, ItemRemoveHandler));
            }

            //ADD SEARCH RESULTS to CONTENT container
            content.Add(searchResults);
        }

        #endregion
    }
}
