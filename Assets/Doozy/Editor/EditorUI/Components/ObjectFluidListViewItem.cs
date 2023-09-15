// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Editor.EditorUI.Components.Internal;
using Doozy.Runtime.UIElements.Extensions;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.Events;
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Editor.EditorUI.Components
{
    public class ObjectFluidListViewItem : FluidListViewItem
    {
        public ObjectField objectField { get; }
        public UnityAction<SerializedProperty> OnRemoveButtonClick;

        public ObjectFluidListViewItem(FluidListView listView, Type objectType)
            : base(listView)
        {
            itemContentContainer.Clear();
            objectField = new ObjectField { objectType = objectType };
            itemContentContainer.Add
            (
                new ComponentField
                    (
                        ComponentField.Size.Small,
                        string.Empty,
                        objectField
                    )
                    .SetStyleMargins(0, 2, 0, 2)
            );
        }

        public void Update(int index, SerializedProperty property)
        {
            //UPDATE INDEX
            showItemIndex = listView.showItemIndex;
            UpdateItemIndex(index);

            //UPDATE PROPERTY
            objectField.BindProperty(property);

            //UPDATE REMOVE BUTTON
            itemRemoveButton.SetOnClick(() => OnRemoveButtonClick?.Invoke(property));
        }
    }
}
