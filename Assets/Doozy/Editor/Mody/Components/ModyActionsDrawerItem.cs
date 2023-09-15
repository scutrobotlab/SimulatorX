// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Runtime.UIElements.Extensions;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Editor.Mody.Components
{
    public class ModyActionsDrawerItem
    {
        public string actionName
        {
            get => actionNameProperty.stringValue;
            set
            {
                if (actionNameProperty.stringValue.Equals(value)) return;
                actionNameProperty.stringValue = value;
                actionProperty.serializedObject.ApplyModifiedProperties();
            }
        }
        public bool actionEnabled
        {
            get => actionEnabledProperty.boolValue;
            set
            {
                if (actionEnabledProperty.boolValue == value) return;
                actionEnabledProperty.boolValue = value;
                actionProperty.serializedObject.ApplyModifiedProperties();
            }
        }

        public SerializedProperty actionProperty { get; }
        public SerializedProperty actionNameProperty { get; }
        public SerializedProperty actionEnabledProperty { get; }

        public FluidAnimatedContainer animatedContainer { get; }

        public ModyActionsDrawer parentDrawer { get; set; }

        public ModyActionsDrawerItem(SerializedProperty actionProperty)
        {
            this.actionProperty = actionProperty;
            actionNameProperty = this.actionProperty.FindPropertyRelative("ActionName");
            actionEnabledProperty = this.actionProperty.FindPropertyRelative("ActionEnabled");
            animatedContainer = new FluidAnimatedContainer().SetName(actionNameProperty.stringValue);

            animatedContainer.OnShowCallback = () =>
            {
                animatedContainer
                    .AddContent
                    (
                        DesignUtils.NewPropertyField(actionProperty.propertyPath)
                            .SetStyleMarginBottom(DesignUtils.k_Spacing)
                    );


                animatedContainer.Bind(actionProperty.serializedObject);
            };

            animatedContainer.Toggle(actionEnabledProperty.boolValue, false);

            Toggle invisibleToggle = DesignUtils.NewToggle(actionEnabledProperty.propertyPath, true);
            animatedContainer.Add(invisibleToggle);
            invisibleToggle.RegisterValueChangedCallback(evt =>
            {
                // Debug.Log($"{actionName}: {(evt.newValue ? "Show" : "Hide")}");
                animatedContainer.Toggle(evt.newValue, false);
                parentDrawer?.ItemUpdated(this, evt.newValue);
            });
        }

        public void ShowItem(bool animateChange = true) =>
            animatedContainer?.Show(animateChange);

        public void HideItem(bool animateChange = true) =>
            animatedContainer?.Hide(animateChange);

        public void ToggleItem(bool show, bool animateChange = true) =>
            animatedContainer?.Toggle(show, animateChange);

    }
}
