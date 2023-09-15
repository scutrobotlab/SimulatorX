// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Editor.Mody;
using Doozy.Editor.Mody.Components;
using Doozy.Editor.Reactor.Components;
using Doozy.Runtime.UIElements.Extensions;
using Doozy.Runtime.UIManager.Modules;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Doozy.Editor.UIManager.Editors.Modules
{
    [CustomEditor(typeof(AnimatorModule), true)]
    public class AnimatorModuleEditor : ModyModuleEditor<AnimatorModule>
    {
        public override List<Texture2D> secondaryIconTextures => EditorMicroAnimations.Reactor.Icons.UIAnimator;

        private List<SerializedProperty> itemsSource { get; set; }
        private SerializedProperty arrayProperty { get; set; }
        private FluidListView fluidListView { get; set; }

        public override VisualElement CreateInspectorGUI()
        {
            CreateEditor();
            return root;
        }

        protected override void CreateEditor()
        {
            base.CreateEditor();

            //MODULE HEADER
            fluidHeader.SetSecondaryIcon(secondaryIconTextures);
            fluidHeader.SetComponentNameText(AnimatorModule.k_DefaultModuleName);

            #region ListView

            itemsSource = new List<SerializedProperty>();
            arrayProperty = serializedObject.FindProperty(nameof(AnimatorModule.Animators));
            fluidListView = new FluidListView();
            const string animationName = "Animator";
            fluidListView
                // .SetListTitle($"{uiAnimationName}s")
                .SetListDescription($"List of {animationName}s controlled by this module")
                .UseSmallEmptyListPlaceholder(true)
                .HideFooter(true)
                .ShowItemIndex(false);
            
            fluidListView.emptyListPlaceholder.SetIcon(EditorMicroAnimations.EditorUI.Placeholders.EmptyListViewSmall);
            fluidListView.listView.selectionType = SelectionType.None;
            fluidListView.listView.itemsSource = itemsSource;
            fluidListView.listView.makeItem = () => new AnimatorFluidListViewItem(fluidListView).SetStylePaddingLeft(DesignUtils.k_Spacing);
            fluidListView.listView.bindItem = (element, i) =>
            {
                var item = (AnimatorFluidListViewItem)element;
                item.Update(i, itemsSource[i]);
                item.OnRemoveButtonClick = property =>
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
                };
            };
            
            #if UNITY_2021_2_OR_NEWER
            fluidListView.listView.fixedItemHeight = 30;
            fluidListView.SetPreferredListHeight((int)fluidListView.listView.fixedItemHeight * 5);
            #else
            fluidListView.listView.itemHeight = 30;
            fluidListView.SetPreferredListHeight(fluidListView.listView.itemHeight * 5);
            #endif
            
            fluidListView.SetDynamicListHeight(false);
            
            //ADD ITEM BUTTON (plus button)
            fluidListView.AddNewItemButtonCallback += () =>
            {
                arrayProperty.InsertArrayElementAtIndex(0);
                arrayProperty.GetArrayElementAtIndex(0).objectReferenceValue = null;
                arrayProperty.serializedObject.ApplyModifiedProperties();
                UpdateItemsSource();
            };

            UpdateItemsSource();
            
            int arraySize = arrayProperty.arraySize;
            fluidListView.schedule.Execute(() =>
            {
                if (arrayProperty == null) return;
                if (arrayProperty.arraySize == arraySize) return;
                arraySize = arrayProperty.arraySize;
                UpdateItemsSource();

            }).Every(100);
            
            void UpdateItemsSource()
            {
                itemsSource.Clear();
                for (int i = 0; i < arrayProperty.arraySize; i++)
                    itemsSource.Add(arrayProperty.GetArrayElementAtIndex(i));

                fluidListView?.Update();
            }
            #endregion
            
            
            //MODULE SETTINGS
            settingsContainer.AddChild(fluidListView);
            
            //MODULE ACTIONS
            AddActionToDrawer(new ModyActionsDrawerItem(serializedObject.FindProperty(nameof(AnimatorModule.PlayForward))));
            AddActionToDrawer(new ModyActionsDrawerItem(serializedObject.FindProperty(nameof(AnimatorModule.PlayReverse))));
            AddActionToDrawer(new ModyActionsDrawerItem(serializedObject.FindProperty(nameof(AnimatorModule.Stop))));
            AddActionToDrawer(new ModyActionsDrawerItem(serializedObject.FindProperty(nameof(AnimatorModule.Finish))));
            AddActionToDrawer(new ModyActionsDrawerItem(serializedObject.FindProperty(nameof(AnimatorModule.Reverse))));
            AddActionToDrawer(new ModyActionsDrawerItem(serializedObject.FindProperty(nameof(AnimatorModule.Rewind))));
            AddActionToDrawer(new ModyActionsDrawerItem(serializedObject.FindProperty(nameof(AnimatorModule.Pause))));
            AddActionToDrawer(new ModyActionsDrawerItem(serializedObject.FindProperty(nameof(AnimatorModule.Resume))));
            AddActionToDrawer(new ModyActionsDrawerItem(serializedObject.FindProperty(nameof(AnimatorModule.SetProgressAt))));
            AddActionToDrawer(new ModyActionsDrawerItem(serializedObject.FindProperty(nameof(AnimatorModule.SetProgressAtZero))));
            AddActionToDrawer(new ModyActionsDrawerItem(serializedObject.FindProperty(nameof(AnimatorModule.SetProgressAtOne))));
            AddActionToDrawer(new ModyActionsDrawerItem(serializedObject.FindProperty(nameof(AnimatorModule.PlayToProgress))));
            AddActionToDrawer(new ModyActionsDrawerItem(serializedObject.FindProperty(nameof(AnimatorModule.PlayFromProgress))));
            AddActionToDrawer(new ModyActionsDrawerItem(serializedObject.FindProperty(nameof(AnimatorModule.UpdateValues))));

            //COMPOSE
            Compose();
        }
    }
}
