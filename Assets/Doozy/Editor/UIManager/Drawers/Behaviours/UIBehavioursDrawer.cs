// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using System.Linq;
using Doozy.Editor.Common.Extensions;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Runtime.UIElements.Extensions;
using Doozy.Runtime.UIManager;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Doozy.Editor.UIManager.Drawers
{
    [CustomPropertyDrawer(typeof(UIBehaviours), true)]
    public class UIBehavioursDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {}

        private static IEnumerable<Texture2D> emptyPlaceholderTextures => EditorMicroAnimations.EditorUI.Placeholders.Empty;
        private static string drawerTitle => "Available Behaviours";
        
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var uiBehaviours = property.GetTargetObjectOfProperty() as UIBehaviours;
            var drawer = new VisualElement();

            FluidButton addBehaviourButton = FluidButton.Get()
                .SetIcon(EditorMicroAnimations.EditorUI.Icons.Plus)
                .SetAccentColor(EditorSelectableColors.Default.Add)
                .SetLabelText("Add Behaviour")
                .SetButtonStyle(ButtonStyle.Contained)
                .SetElementSize(ElementSize.Tiny);
            
            var availableBehaviours = new List<UIBehaviour.Name>(Enum.GetValues(typeof(UIBehaviour.Name)).Cast<UIBehaviour.Name>());
            var availableBehavioursPopupField = new PopupField<UIBehaviour.Name>(availableBehaviours, UIBehaviour.Name.PointerClick);

            FluidField availableBehavioursField = FluidField.Get(drawerTitle);
            FluidPlaceholder placeholder = FluidPlaceholder.Get().SetIcon(emptyPlaceholderTextures).ResizeToHeight(35);
            FluidField placeholderField = FluidField.Get().AddFieldContent(placeholder);
            placeholderField.AddManipulator(new Clickable(() => placeholder?.Play()));
            
            SerializedProperty signalSourceProperty = property.FindPropertyRelative("SignalSource");
            SerializedProperty behavioursProperty = property.FindPropertyRelative("Behaviours");

            availableBehavioursPopupField.RegisterValueChangedCallback(evt =>
            {
                var behaviourName = evt.newValue;
                availableBehavioursField.SetIcon(UIBehaviourDrawer.GetBehaviourTextures(behaviourName));
                availableBehavioursField.iconReaction.Play();
                addBehaviourButton.SetEnabled(uiBehaviours != null && !uiBehaviours.HasBehaviour(behaviourName));
                UpdateAddBehaviourButtonTooltip(behaviourName);
            });
            availableBehavioursPopupField.schedule.Execute(() => UpdateAddBehaviourButtonTooltip(availableBehavioursPopupField.value));
            
            addBehaviourButton.SetEnabled(uiBehaviours != null && !uiBehaviours.HasBehaviour(availableBehavioursPopupField.value));

            availableBehavioursField
                .SetIcon(UIBehaviourDrawer.GetBehaviourTextures(availableBehavioursPopupField.value))
                .AddFieldContent(availableBehavioursPopupField)
                .AddInfoElement(DesignUtils.flexibleSpace)
                .AddInfoElement(addBehaviourButton);

            FluidAnimatedContainer behavioursContainer =
                new FluidAnimatedContainer()
                    .SetClearOnHide(false)
                    .Show(false)
                    .SetStyleMarginTop(DesignUtils.k_Spacing)
                    .SetStyleMarginBottom(DesignUtils.k_Spacing);

            var minusButtons = new List<FluidButton>();
            Refresh();

            void Refresh()
            {
                foreach (FluidButton minusButton in minusButtons)
                    minusButton.Recycle();
                minusButtons.Clear();

                availableBehaviours.Clear();
                availableBehaviours.AddRange(Enum.GetValues(typeof(UIBehaviour.Name)).Cast<UIBehaviour.Name>());

                behavioursContainer.ClearContent();
                for (int i = 0; i < behavioursProperty.arraySize; i++)
                {
                    SerializedProperty behaviourProperty = behavioursProperty.GetArrayElementAtIndex(i);
                    var behaviourName = (UIBehaviour.Name)behaviourProperty.FindPropertyRelative("BehaviourName").enumValueIndex;

                    availableBehaviours.Remove(behaviourName);

                    minusButtons.Add
                    (
                        GetMinusButton()
                            .SetTooltip($"Remove '{ObjectNames.NicifyVariableName(behaviourName.ToString())}' Behaviour")
                            .SetOnClick(() =>
                            {
                                if (property.serializedObject == null) return;
                                if (uiBehaviours == null) return;
                                Undo.RecordObject(property.serializedObject.targetObject, $"Remove Behaviour");
                                uiBehaviours.RemoveBehaviour(behaviourName);
                                Refresh();
                            })
                    );

                    behavioursContainer.AddContent
                    (
                        DesignUtils.row
                            .SetStyleMarginBottom(DesignUtils.k_Spacing)
                            .AddChild(DesignUtils.NewPropertyField(behaviourProperty))
                            .AddChild(DesignUtils.spaceBlock)
                            .AddChild(minusButtons[minusButtons.Count - 1])
                    );
                }
                behavioursContainer.Bind(property.serializedObject);


                placeholderField.SetStyleDisplay(availableBehaviours.Count == 0 ? DisplayStyle.Flex : DisplayStyle.None);
                placeholder.Toggle(availableBehaviours.Count == 0);
                availableBehavioursField.SetStyleDisplay(availableBehaviours.Count == 0 ? DisplayStyle.None : DisplayStyle.Flex);
                
                if (availableBehaviours.Count > 0)
                    availableBehavioursPopupField.value = availableBehaviours.First();
                addBehaviourButton.SetEnabled(!uiBehaviours.HasBehaviour(availableBehavioursPopupField.value));
            }

            addBehaviourButton
                .SetOnClick(() =>
                {
                    if (property.serializedObject == null) return;
                    if (uiBehaviours == null) return;
                    Undo.RecordObject(property.serializedObject.targetObject, "Add Behaviour");
                    uiBehaviours.AddBehaviour(availableBehavioursPopupField.value);
                    Refresh();
                });

            
            void UpdateAddBehaviourButtonTooltip(UIBehaviour.Name behaviourName) =>
                addBehaviourButton.SetTooltip($"Add the '{ObjectNames.NicifyVariableName(behaviourName.ToString())}' behaviour and remove it from the {drawerTitle} drawer");

            int arraySize = -1;
            drawer.schedule.Execute(() =>
            {
                if (arraySize == behavioursProperty.arraySize) return;
                arraySize = behavioursProperty.arraySize;
                Refresh();

            }).Every(200);

            drawer.RegisterCallback<DetachFromPanelEvent>(evt =>
            {
                availableBehavioursField?.Recycle();
                addBehaviourButton?.Recycle();
                behavioursContainer?.Dispose();
                
            });

            drawer
                .AddChild(availableBehavioursField)
                .AddChild(placeholderField)
                .AddChild(behavioursContainer);

            return drawer;
        }

        private static FluidButton GetMinusButton() =>
            FluidButton.Get()
                .SetIcon(EditorMicroAnimations.EditorUI.Icons.Minus)
                .SetAccentColor(EditorSelectableColors.Default.Remove)
                .SetElementSize(ElementSize.Small);
    }
}
