// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using Doozy.Editor.Common.Extensions;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.Components.Internal;
using Doozy.Editor.EditorUI.ScriptableObjects.Colors;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Runtime.Mody;
using Doozy.Runtime.UIElements.Extensions;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Doozy.Editor.Mody.Drawers
{
    [CustomPropertyDrawer(typeof(ModyEvent), true)]
    public class ModyEventDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {}

        protected static Color accentColor => EditorColors.Mody.Action;
        protected static EditorSelectableColorInfo selectableAccentColor => EditorSelectableColors.Mody.Action;

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement drawer = DesignUtils.row;

            SerializedProperty enabledProperty = property.FindPropertyRelative(nameof(ModyEvent.Enabled));
            SerializedProperty eventNameProperty = property.FindPropertyRelative(nameof(ModyEvent.EventName));
            SerializedProperty runnersProperty = property.FindPropertyRelative(nameof(ModyEvent.Runners));
            SerializedProperty eventProperty = property.FindPropertyRelative(nameof(ModyEvent.Event));

            FluidToggleSwitch enabledSwitch = FluidToggleSwitch.Get().BindToProperty(enabledProperty.propertyPath);
            PropertyField eventPropertyField = DesignUtils.NewPropertyField(eventProperty.propertyPath);

            var foldout = new FluidFoldout(eventNameProperty.stringValue);
            foldout.SetStyleFlexGrow(1).SetEnabled(enabledProperty.boolValue);

            foldout.animatedContainer
                .ClearContent()
                .SetClearOnHide(true)
                .SetOnShowCallback(() =>
                {
                    foldout.AddContent(ActionRunnersListView(runnersProperty));
                    foldout.AddContent(DesignUtils.spaceBlock2X);
                    foldout.AddContent(eventPropertyField);
                    foldout.Bind(property.serializedObject);
                });

            enabledSwitch.SetOnValueChanged(evt =>
            {
                foldout.SetEnabled(evt.newValue);
                if (!evt.newValue && foldout.isOn)
                {
                    foldout.Close();
                }
            });

            drawer
                .AddChild(enabledSwitch.SetStyleMarginRight(DesignUtils.k_Spacing))
                .AddChild(foldout);

            return drawer;
        }

        public static FluidListView ActionRunnersListView(SerializedProperty runnersProperty)
        {
            var itemsSource = new List<SerializedProperty>();
            FluidListView fluidListView =
                new FluidListView()
                    .SetItemsSource(itemsSource)
                    .ShowEmptyListPlaceholder(true)
                    .UseSmallEmptyListPlaceholder(true)
                    .HideFooterWhenEmpty(true)
                    .HideFooter(true)
                    .ShowItemIndex(false)
                    .SetListDescription("Action Runners");

            fluidListView.emptyListPlaceholder.SetIcon(EditorMicroAnimations.EditorUI.Placeholders.EmptyListViewSmall);

            fluidListView.listView.selectionType = SelectionType.None;
            fluidListView.listView.makeItem = () =>
                new PropertyFluidListViewItem(fluidListView);

            fluidListView.listView.bindItem = (element, i) =>
            {
                var item = (PropertyFluidListViewItem)element;
                item.Update(i, itemsSource[i]);
                item.OnRemoveButtonClick += itemProperty =>
                {
                    int propertyIndex = 0;
                    for (int j = 0; j < runnersProperty.arraySize; j++)
                    {
                        if (itemProperty.propertyPath != runnersProperty.GetArrayElementAtIndex(j).propertyPath)
                            continue;
                        propertyIndex = j;
                        break;
                    }
                    runnersProperty.DeleteArrayElementAtIndex(propertyIndex);
                    runnersProperty.serializedObject.ApplyModifiedProperties();

                    UpdateItemsSource();
                };
            };

            #if UNITY_2021_2_OR_NEWER
            fluidListView.listView.fixedItemHeight = 84;
            fluidListView.SetPreferredListHeight((int)fluidListView.listView.fixedItemHeight * 4);
            #else
            fluidListView.listView.itemHeight = 84;
            fluidListView.SetPreferredListHeight(fluidListView.listView.itemHeight * 4);
            #endif

            fluidListView.SetDynamicListHeight(false);

            //ADD ITEM BUTTON (plus button)
            fluidListView.AddNewItemButtonCallback += () =>
            {
                runnersProperty.InsertArrayElementAtIndex(0);
                runnersProperty.serializedObject.ApplyModifiedProperties();
                UpdateItemsSource();
                var runner = runnersProperty.GetArrayElementAtIndex(0).GetTargetObjectOfProperty() as ModyActionRunner;
                runner?.Reset();
            };

            UpdateItemsSource();

            int arraySize = runnersProperty.arraySize;

            fluidListView.schedule.Execute(() =>
            {
                if (runnersProperty.arraySize == arraySize) return;
                arraySize = runnersProperty.arraySize;
                UpdateItemsSource();

            }).Every(100);

            void UpdateItemsSource()
            {
                itemsSource.Clear();
                for (int i = 0; i < runnersProperty.arraySize; i++)
                    itemsSource.Add(runnersProperty.GetArrayElementAtIndex(i));

                fluidListView.Update();
            }

            return fluidListView;
        }

    }
}
