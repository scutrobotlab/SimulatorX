// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Linq;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.Components.Internal;
using Doozy.Editor.EditorUI.ScriptableObjects.Colors;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Editor.Reactor.Internal;
using Doozy.Runtime.Reactor.Extensions;
using Doozy.Runtime.Reactor.Internal;
using Doozy.Runtime.Signals;
using Doozy.Runtime.UIElements.Extensions;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Doozy.Editor.Signals.Drawers
{
    [CustomPropertyDrawer(typeof(ProviderId), true)]
    public class ProviderIdDrawer : PropertyDrawer
    {
        private static Color accentColor => EditorColors.Mody.Trigger;
        private static EditorSelectableColorInfo selectableAccentColor => EditorSelectableColors.Mody.Trigger;
        private static ElementSize tabSize => ElementSize.Small;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {}

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var typeSerializedProperty = property.FindPropertyRelative(nameof(ProviderId.Type));
            var categorySerializedProperty = property.FindPropertyRelative(nameof(ProviderId.Category));
            var nameSerializedProperty = property.FindPropertyRelative(nameof(ProviderId.Name));

            ProviderType CurrentProviderType() => (ProviderType)typeSerializedProperty.enumValueIndex;

            var drawer = DesignUtils.row;

            var typeInvisibleEnumField = DesignUtils.NewEnumField(typeSerializedProperty.propertyPath, true);

            var triggerIconImage = NewTriggerIconImage();
            var triggerIconReaction = triggerIconImage.GetTexture2DReaction(EditorMicroAnimations.Mody.Icons.ModyTrigger).SetEditorHeartbeat().SetDuration(0.4f);
            triggerIconImage.AddManipulator(new Clickable(() => triggerIconReaction?.Play()));

            var buttonLocalProvider = NewProviderButton(ProviderType.Local).SetTabPosition(TabPosition.TabOnTop).SetTooltip("Local Signal Provider\nAttached to this GameObject");
            var buttonGlobalProvider = NewProviderButton(ProviderType.Global).SetTabPosition(TabPosition.TabOnBottom).SetTooltip($"Global Signal Provider\nAttached to the '{nameof(Signals)}' GameObject");
            var providerTypeButtonsContainer = DesignUtils.column.SetStyleFlexGrow(0).SetStyleAlignSelf(Align.Center).AddChild(buttonLocalProvider).AddChild(buttonGlobalProvider);

            var providerCategories = SignalProvider.GetProviderCategories(CurrentProviderType()).ToList();
            var providerNames = SignalProvider.GetProviderNames(CurrentProviderType(), providerCategories[0]).ToList();

            if (!providerCategories.Contains(categorySerializedProperty.stringValue))
            {
                categorySerializedProperty.stringValue = providerCategories[0];
                nameSerializedProperty.stringValue = providerNames[0];
                property.serializedObject.ApplyModifiedProperties();
            }
            else if (!providerNames.Contains(nameSerializedProperty.stringValue))
            {
                nameSerializedProperty.stringValue = providerNames[0];
                property.serializedObject.ApplyModifiedProperties();
            }

            var providerCategoriesPopup = new PopupField<string>(providerCategories, categorySerializedProperty.stringValue).ResetLayout();
            var providerNamesPopup = new PopupField<string>(providerNames, nameSerializedProperty.stringValue).ResetLayout();

            providerCategoriesPopup.RegisterValueChangedCallback(evt =>
            {
                categorySerializedProperty.stringValue = evt.newValue;
                providerNames.Clear();
                providerNames.AddRange(SignalProvider.GetProviderNames(CurrentProviderType(), evt.newValue));
                nameSerializedProperty.stringValue = providerNames[0];
                property.serializedObject.ApplyModifiedProperties();
            });

            providerNamesPopup.RegisterValueChangedCallback(evt =>
            {
                nameSerializedProperty.stringValue = evt.newValue;
                property.serializedObject.ApplyModifiedProperties();
            });

            buttonLocalProvider.isOn = CurrentProviderType() == ProviderType.Local;
            buttonGlobalProvider.isOn = CurrentProviderType() == ProviderType.Global;

            buttonLocalProvider.SetOnClick(() => SelectProviderType(buttonLocalProvider, ProviderType.Local));
            buttonGlobalProvider.SetOnClick(() => SelectProviderType(buttonGlobalProvider, ProviderType.Global));

            typeInvisibleEnumField.RegisterValueChangedCallback(evt => OnProviderTypeChanged((ProviderType)evt.newValue));

            void OnProviderTypeChanged(ProviderType type)
            {
                buttonLocalProvider.isOn = type == ProviderType.Local;
                buttonGlobalProvider.isOn = type == ProviderType.Global;
                triggerIconReaction?.Play();

                providerCategories.Clear();
                providerCategories.AddRange(SignalProvider.GetProviderCategories(type));
                providerCategoriesPopup.value = providerCategories[0];
                categorySerializedProperty.stringValue = providerCategories[0];

                providerNames.Clear();
                providerNames.AddRange(SignalProvider.GetProviderNames(type, providerCategories[0]));
                providerNamesPopup.value = providerNames[0];
                nameSerializedProperty.stringValue = providerNames[0];

                property.serializedObject.ApplyModifiedProperties();
            }

            void SelectProviderType(FluidToggleButtonTab button, ProviderType type)
            {
                if ((ProviderType)typeInvisibleEnumField.value == type)
                {
                    button.schedule.Execute(() => button.SetIsOn(true));
                    return;
                }
                typeInvisibleEnumField.value = type;
            }

            drawer
                .AddChild(typeInvisibleEnumField)
                .AddChild(providerTypeButtonsContainer)
                .AddChild(triggerIconImage)
                .AddChild
                (
                    DesignUtils.column
                        .AddChild(providerCategoriesPopup)
                        .AddChild(providerNamesPopup)
                );

            drawer.RegisterCallback<DetachFromPanelEvent>(evt =>
            {
                buttonLocalProvider.Recycle();
                buttonGlobalProvider.Recycle();
            });
            
            return drawer;
        }

        private static FluidToggleButtonTab NewProviderButton(ProviderType type)
        {
            FluidToggleButtonTab tab =
                FluidToggleButtonTab.Get(type.ToString())
                    .ResetLayout()
                    .SetStyleWidth(40)
                    .SetToggleAccentColor(selectableAccentColor)
                    .SetContainerColorOff(EditorColors.Default.Background)
                    .SetElementSize(ElementSize.Small);
            tab.buttonLabel.SetStyleTextAlign(TextAnchor.MiddleCenter);
            return tab;
        }

        private static Image NewTriggerIconImage() =>
            new Image().SetStyleBackgroundImageTintColor(accentColor)
                .SetStyleFlexGrow(0).SetStyleFlexShrink(0)
                .SetStyleSize(32).SetStyleAlignSelf(Align.Center)
                .SetStyleMarginLeft(DesignUtils.k_Spacing / 2f)
                .SetStyleMarginRight(DesignUtils.k_Spacing);
    }
}
