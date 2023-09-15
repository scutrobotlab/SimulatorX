// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.Components.Internal;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Editor.Nody.Nodes.Internal;
using Doozy.Editor.UIElements;
using Doozy.Editor.UIManager.Nodes.PortData;
using Doozy.Runtime.Colors;
using Doozy.Runtime.UIElements.Extensions;
using Doozy.Runtime.UIManager.Nodes;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Doozy.Editor.UIManager.Nodes
{
    [CustomEditor(typeof(UINode))]
    public class UINodeEditor : FlowNodeEditor
    {
        public override IEnumerable<Texture2D> nodeIconTextures => EditorMicroAnimations.Nody.Icons.UINode;

        private FluidComponentHeader enterNodeHeader { get; set; }
        private FluidComponentHeader exitNodeHeader { get; set; }
        private FluidToggleSwitch onEnterHideAllViewsSwitch { get; set; }
        private FluidToggleSwitch onExitHideAllViewsSwitch { get; set; }

        private SerializedProperty propertyOnEnterShowViews { get; set; }
        private SerializedProperty propertyOnEnterHideViews { get; set; }
        private SerializedProperty propertyOnExitShowViews { get; set; }
        private SerializedProperty propertyOnExitHideViews { get; set; }
        private SerializedProperty propertyOnEnterHideAllViews { get; set; }
        private SerializedProperty propertyOnExitHideAllViews { get; set; }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            enterNodeHeader?.Recycle();
            exitNodeHeader?.Recycle();
            onEnterHideAllViewsSwitch?.Recycle();
            onExitHideAllViewsSwitch?.Recycle();
        }

        protected override void FindProperties()
        {
            base.FindProperties();

            propertyOnEnterShowViews = serializedObject.FindProperty("OnEnterShowViews");
            propertyOnEnterHideViews = serializedObject.FindProperty("OnEnterHideViews");
            propertyOnExitShowViews = serializedObject.FindProperty("OnExitShowViews");
            propertyOnExitHideViews = serializedObject.FindProperty("OnExitHideViews");
            propertyOnEnterHideAllViews = serializedObject.FindProperty("OnEnterHideAllViews");
            propertyOnExitHideAllViews = serializedObject.FindProperty("OnExitHideAllViews");
        }

        protected override void InitializeEditor()
        {
            base.InitializeEditor();

            componentHeader
                .SetComponentNameText(ObjectNames.NicifyVariableName(nameof(UINode)))
                .AddManualButton("https://doozyentertainment.atlassian.net/wiki/spaces/DUI4/pages/1048215593/UI+Node?atlOrigin=eyJpIjoiNTJjN2ZjZjgwYjJkNDM0YTk1ZjViZDk5MTYwN2RhZmIiLCJwIjoiYyJ9")
                .AddYouTubeButton();

            enterNodeHeader =
                GetHeader()
                    .SetComponentNameText("On Enter Node")
                    .SetIcon(EditorMicroAnimations.Nody.Icons.EnterNode)
                    .SetAccentColor(EditorColors.Nody.Input);

            onEnterHideAllViewsSwitch =
                FluidToggleSwitch.Get()
                    .SetToggleAccentColor(EditorSelectableColors.Nody.Input)
                    .SetLabelText("Hide All Views")
                    .BindToProperty(propertyOnEnterHideAllViews);

            exitNodeHeader =
                GetHeader()
                    .SetComponentNameText("On Exit Node")
                    .SetIcon(EditorMicroAnimations.Nody.Icons.ExitNode)
                    .SetAccentColor(EditorColors.Nody.Output);

            onExitHideAllViewsSwitch =
                FluidToggleSwitch.Get()
                    .SetToggleAccentColor(EditorSelectableColors.Nody.Output)
                    .SetLabelText("Hide All Views")
                    .BindToProperty(propertyOnExitHideAllViews);

            RefreshNodeEditor();
        }

        public override void RefreshNodeEditor()
        {
            base.RefreshNodeEditor();
            flowNode.outputPorts.ForEach(p => portsContainer.AddChild(new UIOutputPortDataEditor(p, nodeView)));
        }

        protected override void Compose()
        {
            base.Compose();

            root
                .AddChild(DesignUtils.spaceBlock2X)
                .AddChild(portsContainer)
                .AddChild(DesignUtils.spaceBlock3X)
                .AddChild(enterNodeHeader)
                .AddChild(DesignUtils.spaceBlock)
                .AddChild
                (
                    showHideContainer
                        .SetStyleBorderColor(EditorColors.Nody.Input.WithAlpha(0.4f))
                        .AddChild(onEnterHideAllViewsSwitch)
                        .AddChild(DesignUtils.spaceBlock2X)
                        .AddChild(GetListView(propertyOnEnterShowViews).SetListDescription("Show Views"))
                        .AddChild(DesignUtils.spaceBlock2X)
                        .AddChild(GetListView(propertyOnEnterHideViews).SetListDescription("Hide Views"))
                )
                .AddChild(DesignUtils.spaceBlock4X)
                .AddChild(exitNodeHeader)
                .AddChild(DesignUtils.spaceBlock)
                .AddChild
                (
                    showHideContainer
                        .SetStyleBorderColor(EditorColors.Nody.Output.WithAlpha(0.4f))
                        .AddChild(onExitHideAllViewsSwitch)
                        .AddChild(DesignUtils.spaceBlock2X)
                        .AddChild(GetListView(propertyOnExitShowViews).SetListDescription("Show Views"))
                        .AddChild(DesignUtils.spaceBlock2X)
                        .AddChild(GetListView(propertyOnExitHideViews).SetListDescription("Hide Views"))
                )
                .AddChild(DesignUtils.spaceBlock2X)
                ;
        }

        private static FluidListView GetListView(SerializedProperty arrayProperty)
        {
            var itemsSource = new List<SerializedProperty>();
            var lv = new FluidListView();
            lv.listView.selectionType = SelectionType.None;
            lv.listView.itemsSource = itemsSource;
            lv.listView.makeItem = () => new PropertyFluidListViewItem(lv);
            lv.listView.bindItem = (element, i) =>
            {
                var item = (PropertyFluidListViewItem)element;
                item.propertyField.TryToHideLabel();
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
                };
            };

            #if UNITY_2021_2_OR_NEWER
            lv.listView.fixedItemHeight = 80;
            lv.SetPreferredListHeight((int)lv.listView.fixedItemHeight * 10);
            #else
            lv.listView.itemHeight = 80;
            lv.SetPreferredListHeight(lv.listView.itemHeight * 10);
            #endif

            lv.SetDynamicListHeight(false);
            lv.UseSmallEmptyListPlaceholder(true);
            lv.HideFooter(true);
            lv.ShowItemIndex(false);
            lv.emptyListPlaceholder.SetIcon(EditorMicroAnimations.EditorUI.Placeholders.EmptyListViewSmall);

            //ADD ITEM BUTTON (plus button)
            lv.AddNewItemButtonCallback += () =>
            {
                arrayProperty.InsertArrayElementAtIndex(0);
                // propertyTags.GetArrayElementAtIndex(0).objectReferenceValue = null;
                arrayProperty.serializedObject.ApplyModifiedProperties();
                UpdateItemsSource();
            };

            int arraySize = -1;
            lv.schedule.Execute(() =>
            {
                if (arrayProperty.arraySize == arraySize) return;
                arraySize = arrayProperty.arraySize;
                UpdateItemsSource();

            }).Every(100);

            void UpdateItemsSource()
            {
                itemsSource.Clear();

                for (int i = 0; i < arrayProperty.arraySize; i++)
                    itemsSource.Add(arrayProperty.GetArrayElementAtIndex(i));

                lv?.Update();
            }

            UpdateItemsSource();

            return lv;
        }

        private static VisualElement showHideContainer =>
            DesignUtils.column
                .SetStyleBorderRadius(6)
                .SetStyleBorderWidth(1)
                .SetStylePadding(DesignUtils.k_Spacing2X);

        private static FluidComponentHeader GetHeader() => FluidComponentHeader.Get().SetElementSize(ElementSize.Small);
    }
}
