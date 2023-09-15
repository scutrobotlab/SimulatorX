// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.ScriptableObjects.Colors;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Runtime.Common.Extensions;
using Doozy.Runtime.UIElements.Extensions;
using UnityEditor;
using UnityEngine.UIElements;
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Editor.EditorUI.Editors
{
    [CustomEditor(typeof(EditorDataColorPalette))]
    public class EditorDataColorPaletteEditor : UnityEditor.Editor, ISearchable
    {
        public EditorDataColorPalette castedTarget => (EditorDataColorPalette)target;

        public VisualElement root { get; private set; }
        public FluidField nameComponentField { get; private set; }
        public FluidListView fluidListView { get; private set; }

        private SerializedProperty arrayProperty { get; set; }
        private List<SerializedProperty> itemsSource { get; set; }

        public override VisualElement CreateInspectorGUI()
        {
            InitializeEditor();
            root
                .AddSpace(0, DesignUtils.k_Spacing)
                .AddChild(nameComponentField)
                .AddSpace(0, DesignUtils.k_Spacing * 2)
                .AddChild(fluidListView);
            return root;
        }

        private void OnDestroy()
        {
            nameComponentField?.Recycle();
            
            if (fluidListView == null) return;
            foreach (VisualElement element in fluidListView.toolbarElements)
                if (element is IDisposable disposable)
                    disposable.Dispose();
        }

        private static string ListViewTitle(string paletteName) =>
            $"{(paletteName.IsNullOrEmpty() ? "Unnamed" : paletteName)} Color Palette";

        private void SortByName()
        {
            Undo.RecordObject(castedTarget, "Sort Az");
            castedTarget.SortByColorName();
        }

        private void SortByHue()
        {
            Undo.RecordObject(castedTarget, "Sort Hue");
            castedTarget.SortByHue();
        }

        private void Save() =>
            EditorDataColorDatabase.instance.RefreshDatabaseItem(castedTarget, true, true, true);

        private void InitializeEditor()
        {
            root = new VisualElement();

            nameComponentField = FluidField.Get("Palette Name").SetIcon(EditorMicroAnimations.EditorUI.Components.EditorColorPalette);
            TextField nameTextField = DesignUtils.NewTextField("PaletteName", true).SetStyleFlexGrow(1);
            nameTextField.RegisterValueChangedCallback(evt =>
            {
                fluidListView.SetListTitle(ListViewTitle(evt.newValue));
                nameComponentField?.iconReaction?.Play();
            });
            nameComponentField.AddFieldContent(nameTextField);

            InitializeListView();
        }

        private void InitializeListView()
        {
            arrayProperty = serializedObject.FindProperty("Colors");
            fluidListView = new FluidListView();
            fluidListView.SetListTitle(ListViewTitle(serializedObject.FindProperty("PaletteName").stringValue));
            fluidListView.SetListDescription("List of colors");
            fluidListView.listView.selectionType = SelectionType.None;
            itemsSource = new List<SerializedProperty>();

            fluidListView.listView.itemsSource = itemsSource;
            fluidListView.listView.makeItem = () => new PropertyFluidListViewItem(fluidListView);
            fluidListView.listView.bindItem = (element, i) =>
            {
                var item = (PropertyFluidListViewItem)element;
                item.Update(i, itemsSource[i]);
                item.OnRemoveButtonClick += property =>
                {
                    int propertyIndex = 0;
                    for (int j = 0; j < arrayProperty.arraySize; j++)
                    {
                        if (property.propertyPath != arrayProperty.GetArrayElementAtIndex(j).propertyPath)
                            continue;
                        propertyIndex = j;
                        break;
                    }
                    arrayProperty.DeleteArrayElementAtIndex(propertyIndex);
                    arrayProperty.serializedObject.ApplyModifiedProperties();

                    UpdateItemsSource();

                    if (fluidListView.searchBox.isSearching)
                    {
                        Search(fluidListView.searchBox.searchPattern);
                    }
                };
            };
            
            #if UNITY_2021_2_OR_NEWER
            fluidListView.listView.fixedItemHeight = 40;
            fluidListView.SetPreferredListHeight((int)fluidListView.listView.fixedItemHeight * 10);
            #else
            fluidListView.listView.itemHeight = 40;
            fluidListView.SetPreferredListHeight(fluidListView.listView.itemHeight * 10);
            #endif
            
            fluidListView.SetDynamicListHeight(false);

            //ADD ITEM BUTTON (plus button)
            fluidListView.AddNewItemButtonCallback += () =>
            {
                arrayProperty.InsertArrayElementAtIndex(0);
                // arrayProperty.GetArrayElementAtIndex(0).objectReferenceValue = null;
                arrayProperty.serializedObject.ApplyModifiedProperties();
                UpdateItemsSource();
            };

            //TOOLBAR BUTTONS (sort, save)
            fluidListView
                .AddToolbarElement(FluidListView.Buttons.sortAzButton.SetOnClick(SortByName).SetTooltip("Order colors by color name"))
                .AddToolbarElement(FluidListView.Buttons.sortHueButton.SetOnClick(SortByHue).SetTooltip("Order colors by HUE value"))
                .AddToolbarElement(FluidListView.Buttons.saveButton.SetOnClick(Save))
                .AddToolbarElement(DesignUtils.flexibleSpace);

            //SEARCH (search box)
            fluidListView.HasSearch(true);
            fluidListView.searchBox.AddSearchable(this);

            UpdateItemsSource();

            int arraySize = arrayProperty.arraySize;
            fluidListView.schedule.Execute(() =>
            {
                if (arrayProperty == null) return;
                if (arrayProperty.arraySize == arraySize) return;
                arraySize = arrayProperty.arraySize;
                UpdateItemsSource();

            }).Every(100);
        }

        private void UpdateItemsSource()
        {
            itemsSource.Clear();

            if (!fluidListView.inSearchMode)
            {
                for (int i = 0; i < arrayProperty.arraySize; i++)
                    itemsSource.Add(arrayProperty.GetArrayElementAtIndex(i));

                fluidListView?.Update();
                return;
            }

            UpdateSearchResults();
        }

        #region Search

        public bool isSearching => fluidListView.searchBox.isSearching;
        public string searchPattern => fluidListView.searchBox.searchPattern;
        public bool hasSearchResults { get; private set; }
        public VisualElement searchResults => new VisualElement();

        public void ClearSearch()
        {
            hasSearchResults = false;
            Search(string.Empty);
        }

        public void Search(string pattern)
        {
            fluidListView.searchBox.searchTextField.value = pattern;
            UpdateItemsSource();
            hasSearchResults = itemsSource.Count > 0;
        }

        public void UpdateSearchResults()
        {
            itemsSource.Clear();

            for (int i = 0; i < arrayProperty.arraySize; i++)
            {
                SerializedProperty arrayElementAtIndex = arrayProperty.GetArrayElementAtIndex(i);
                if (!Regex.IsMatch(arrayElementAtIndex.FindPropertyRelative("ColorName").stringValue, $"{fluidListView.searchBox.searchPattern}", RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace))
                    continue;
                itemsSource.Add(arrayElementAtIndex);
            }

            hasSearchResults = itemsSource.Count > 0;

            fluidListView.Update();
        }

        #endregion
    }
}
