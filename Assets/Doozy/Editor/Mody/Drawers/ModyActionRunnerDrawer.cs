// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using Doozy.Editor.Common.Extensions;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Editor.UIElements;
using Doozy.Runtime.Mody;
using Doozy.Runtime.UIElements.Extensions;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using EditorStyles = Doozy.Editor.EditorUI.EditorStyles;

namespace Doozy.Editor.Mody.Drawers
{
    [CustomPropertyDrawer(typeof(ModyActionRunner), true)]
    public class ModyActionRunnerDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {}

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var modyActionRunner = property.GetTargetObjectOfProperty() as ModyActionRunner;

            SerializedProperty moduleProperty = property.FindPropertyRelative(nameof(ModyActionRunner.Module));
            SerializedProperty actionNameProperty = property.FindPropertyRelative(nameof(ModyActionRunner.ActionName));
            SerializedProperty runProperty = property.FindPropertyRelative(nameof(ModyActionRunner.Run));
            SerializedProperty ignoreCooldownProperty = property.FindPropertyRelative(nameof(ModyActionRunner.IgnoreCooldown));

            SerializedProperty boolValueProperty = property.FindPropertyRelative(nameof(ModyActionRunner.BoolValue));
            SerializedProperty colorValueProperty = property.FindPropertyRelative(nameof(ModyActionRunner.ColorValue));
            SerializedProperty doubleValueProperty = property.FindPropertyRelative(nameof(ModyActionRunner.DoubleValue));
            SerializedProperty floatValueProperty = property.FindPropertyRelative(nameof(ModyActionRunner.FloatValue));
            SerializedProperty gameObjectValueProperty = property.FindPropertyRelative(nameof(ModyActionRunner.GameObjectValue));
            SerializedProperty genericValueProperty = property.FindPropertyRelative(nameof(ModyActionRunner.GenericValue));
            SerializedProperty intValueProperty = property.FindPropertyRelative(nameof(ModyActionRunner.IntValue));
            SerializedProperty longValueProperty = property.FindPropertyRelative(nameof(ModyActionRunner.LongValue));
            SerializedProperty monoBehaviourValueProperty = property.FindPropertyRelative(nameof(ModyActionRunner.MonoBehaviourValue));
            SerializedProperty scriptableObjectValueProperty = property.FindPropertyRelative(nameof(ModyActionRunner.ScriptableObjectValue));
            SerializedProperty spriteValueProperty = property.FindPropertyRelative(nameof(ModyActionRunner.SpriteValue));
            SerializedProperty stringValueProperty = property.FindPropertyRelative(nameof(ModyActionRunner.StringValue));
            SerializedProperty texture2DValueProperty = property.FindPropertyRelative(nameof(ModyActionRunner.Texture2DValue));
            SerializedProperty textureValueProperty = property.FindPropertyRelative(nameof(ModyActionRunner.TextureValue));
            SerializedProperty vector2ValueProperty = property.FindPropertyRelative(nameof(ModyActionRunner.Vector2Value));
            SerializedProperty vector3ValueProperty = property.FindPropertyRelative(nameof(ModyActionRunner.Vector3Value));
            SerializedProperty vector4ValueProperty = property.FindPropertyRelative(nameof(ModyActionRunner.Vector4Value));

            VisualElement layoutContainer;
            Label moduleNameLabel;
            ObjectField moduleObjectField;
            VisualElement actionRow;
            Label actionNameLabel;
            VisualElement actionNameContainer;
            PopupField<string> actionNamesPopupField;
            VisualElement valueColumn;
            Label valueLabel;
            VisualElement valueContainer;
            Label runLabel;
            EnumField runEnumField;
            Label ignoreCooldownLabel;
            VisualElement ignoreCooldownContainer;

            ModyAction selectedAction = null;

            TemplateContainer templateContainer = EditorLayouts.Mody.ModyActionRunner.CloneTree();
            layoutContainer = templateContainer.Q<VisualElement>(nameof(layoutContainer));
            moduleNameLabel = layoutContainer.Q<Label>(nameof(moduleNameLabel));
            moduleObjectField = layoutContainer.Q<ObjectField>(nameof(moduleObjectField));
            actionRow = layoutContainer.Q<VisualElement>(nameof(actionRow));
            actionNameLabel = layoutContainer.Q<Label>(nameof(actionNameLabel));
            actionNameContainer = layoutContainer.Q<VisualElement>(nameof(actionNameContainer));
            valueColumn = layoutContainer.Q<VisualElement>(nameof(valueColumn));
            valueLabel = layoutContainer.Q<Label>(nameof(valueLabel));
            valueContainer = layoutContainer.Q<VisualElement>(nameof(valueContainer));
            runLabel = layoutContainer.Q<Label>(nameof(runLabel));
            runEnumField = layoutContainer.Q<EnumField>(nameof(runEnumField));
            ignoreCooldownLabel = layoutContainer.Q<Label>(nameof(ignoreCooldownLabel));
            ignoreCooldownContainer = layoutContainer.Q<VisualElement>(nameof(ignoreCooldownContainer));

            templateContainer.AddStyle(EditorStyles.Mody.ModyActionRunner);


            VisualElement drawer = new VisualElement().AddChild(templateContainer);

            layoutContainer.SetStyleBackgroundColor(DesignUtils.fieldBackgroundColor);

            moduleNameLabel
                .SetStyleColor(EditorColors.Default.TextTitle)
                .SetStyleUnityFont(EditorFonts.Ubuntu.Light);

            var fieldNameLabels = new List<Label>
            {
                actionNameLabel,
                runLabel,
                ignoreCooldownLabel
            };

            foreach (Label label in fieldNameLabels)
            {
                label
                    .SetStyleColor(EditorColors.Default.TextDescription)
                    .SetStyleUnityFont(EditorFonts.Ubuntu.Light);
            }


            moduleObjectField
                .ResetLayout()
                .SetStyleFlexGrow(1)
                .SetBindingPath(moduleProperty.propertyPath)
                .SetObjectType(typeof(ModyModule))
                .SetTooltip("Reference to the target Module");


            var actionNames = new List<string> { string.Empty };
            actionNameContainer
                .AddChild
                (
                    actionNamesPopupField =
                        new PopupField<string>(actionNames, 0)
                            .ResetLayout()
                            .SetTooltip("Name of the Action on the target Module")
                );

            runEnumField
                .ResetLayout()
                .SetBindingPath(runProperty.propertyPath)
                .SetTooltip("Operation to run")
                .SetStyleWidth(60);

            FluidToggleCheckbox ignoreCooldownCheckbox =
                FluidToggleCheckbox.Get()
                    .BindToProperty(ignoreCooldownProperty)
                    .SetTooltip("If the Action is in cooldown, should it be ignored and run anyway");

            ignoreCooldownContainer
                .AddChild(ignoreCooldownCheckbox);



            actionNamesPopupField.RegisterValueChangedCallback(evt =>
            {
                actionNameProperty.stringValue = actionNamesPopupField.value;
                property.serializedObject.ApplyModifiedProperties();
                UpdateActionNames(modyActionRunner != null && modyActionRunner.Module != null ? modyActionRunner.Module : null);
            });

            moduleObjectField.RegisterValueChangedCallback(evt =>
            {
                UpdateModuleNameLabel(evt.newValue != null ? (ModyModule)evt.newValue : null);
                UpdateActionNames((ModyModule)evt.newValue);
            });

            TextField invisibleActionNameTextField = DesignUtils.NewTextField(actionNameProperty, false, true);
            invisibleActionNameTextField.RegisterValueChangedCallback(evt =>
            {
                if (evt.newValue == actionNamesPopupField.value)
                    return;

                if (actionNames.Contains(evt.newValue))
                {
                    actionNamesPopupField.value = actionNames[actionNames.IndexOf(evt.newValue)];
                    return;
                }

                UpdateActionNames(modyActionRunner != null && modyActionRunner.Module != null ? modyActionRunner.Module : null);
            });

            void UpdateModuleNameLabel(ModyModule module)
            {
                moduleNameLabel.SetStyleDisplay(module != null ? DisplayStyle.Flex : DisplayStyle.None);
                if (module == null) return;
                moduleNameLabel.SetText($"{module.moduleName}");
            }

            void UpdateActionNames(ModyModule module)
            {
                actionNames.Clear();
                bool hasModule = module != null;
                actionRow.SetEnabled(hasModule);
                if (hasModule)
                {
                    actionNames.AddRange(module.actionNames);
                    int actionNameIndex = 0;
                    if (actionNames.Contains(actionNameProperty.stringValue))
                        actionNameIndex = actionNames.IndexOf(actionNameProperty.stringValue);
                    actionNamesPopupField.value = actionNames[actionNameIndex];
                }
                else
                {
                    actionNames.Add(string.Empty);
                    actionNamesPopupField.value = actionNames[0];
                }

                selectedAction = null;
                valueColumn.SetStyleDisplay(DisplayStyle.None);
                valueContainer.RecycleAndClear();

                if (modyActionRunner == null) return;
                if (!hasModule) return;
                selectedAction = module.GetAction(actionNamesPopupField.value);
                if (selectedAction == null) return;
                if (!selectedAction.HasValue) return;

                valueColumn.SetStyleDisplay(DisplayStyle.Flex);
                Type type = selectedAction.ValueType;

                if (type == typeof(int))
                {
                    valueLabel.SetText($"Value <int>");
                    IntegerField field = new IntegerField().ResetLayout().SetBindingPath(intValueProperty.propertyPath).SetStyleMinWidth(80).SetStyleFlexGrow(1).SetName(valueLabel.text);
                    field.isDelayed = true;
                    valueContainer.AddChild(field);
                }
                else if (type == typeof(float))
                {
                    valueLabel.SetText($"Value <float>");
                    FloatField field = new FloatField().ResetLayout().SetBindingPath(floatValueProperty.propertyPath).SetStyleMinWidth(80).SetStyleFlexGrow(1).SetName(valueLabel.text);
                    field.isDelayed = true;
                    valueContainer.AddChild(field);
                }
                else if (type == typeof(double))
                {
                    valueLabel.SetText($"Value <double>");
                    DoubleField field = new DoubleField().ResetLayout().SetBindingPath(doubleValueProperty.propertyPath).SetStyleMinWidth(80).SetStyleFlexGrow(1).SetName(valueLabel.text);
                    field.isDelayed = true;
                    valueContainer.AddChild(field);
                }
                else if (type == typeof(long))
                {
                    valueLabel.SetText($"Value <long>");
                    LongField field = new LongField().ResetLayout().SetBindingPath(longValueProperty.propertyPath).SetStyleMinWidth(80).SetStyleFlexGrow(1).SetName(valueLabel.text);
                    field.isDelayed = true;
                    valueContainer.AddChild(field);
                }
                else if (type == typeof(string))
                {
                    valueLabel.SetText($"Value <string>");
                    TextField field = new TextField().ResetLayout().SetBindingPath(stringValueProperty.propertyPath).SetStyleMinWidth(80).SetStyleFlexGrow(1).SetName(valueLabel.text);
                    field.isDelayed = true;
                    valueContainer.AddChild(field);
                }
                else if (type == typeof(bool))
                {
                    valueLabel.SetText($"Value <bool>");
                    valueContainer.AddChild(FluidToggleCheckbox.Get().BindToProperty(boolValueProperty).SetName(valueLabel.text));
                }
                else if (type == typeof(Color) || type == typeof(Color32))
                {
                    valueLabel.SetText($"Value <Color>");
                    valueContainer.AddChild(new ColorField().ResetLayout().SetBindingPath(colorValueProperty.propertyPath).SetStyleMinWidth(80).SetStyleFlexGrow(1).SetName(valueLabel.text));
                }
                else if (type == typeof(Vector2))
                {
                    valueLabel.SetText($"Value <Vector2>");
                    valueContainer.AddChild(new Vector2Field().ResetLayout().SetBindingPath(vector2ValueProperty.propertyPath).SetStyleMinWidth(80).SetStyleFlexGrow(1).SetName(valueLabel.text));
                }
                else if (type == typeof(Vector3))
                {
                    valueLabel.SetText($"Value <Vector3>");
                    valueContainer.AddChild(new Vector3Field().ResetLayout().SetBindingPath(vector3ValueProperty.propertyPath).SetStyleMinWidth(120).SetStyleFlexGrow(1).SetName(valueLabel.text));
                }
                else if (type == typeof(Vector4))
                {
                    valueLabel.SetText($"Value <Vector4>");
                    valueContainer.AddChild(new Vector4Field().ResetLayout().SetBindingPath(vector4ValueProperty.propertyPath).SetStyleMinWidth(160).SetStyleFlexGrow(1).SetName(valueLabel.text));
                }
                else if (type == typeof(GameObject))
                {
                    valueLabel.SetText($"Value <GameObject>");
                    valueContainer.AddChild(new ObjectField().ResetLayout().SetBindingPath(gameObjectValueProperty.propertyPath).SetObjectType(typeof(GameObject)).SetName(valueLabel.text));
                }
                else if (type == typeof(MonoBehaviour))
                {
                    valueLabel.SetText($"Value <MonoBehaviour>");
                    valueContainer.AddChild(new ObjectField().ResetLayout().SetBindingPath(monoBehaviourValueProperty.propertyPath).SetObjectType(typeof(MonoBehaviour)).SetName(valueLabel.text));
                }
                else if (type == typeof(Sprite))
                {
                    valueLabel.SetText($"Value <Sprite>");
                    valueContainer.AddChild(new ObjectField().ResetLayout().SetBindingPath(spriteValueProperty.propertyPath).SetObjectType(typeof(Sprite)).SetName(valueLabel.text));
                }
                else if (type == typeof(Texture))
                {
                    valueLabel.SetText($"Value <Texture>");
                    valueContainer.AddChild(new ObjectField().ResetLayout().SetBindingPath(textureValueProperty.propertyPath).SetObjectType(typeof(Texture)).SetName(valueLabel.text));
                }
                else if (type == typeof(Texture2D))
                {
                    valueLabel.SetText($"Value <Texture2D>");
                    valueContainer.AddChild(new ObjectField().ResetLayout().SetBindingPath(texture2DValueProperty.propertyPath).SetObjectType(typeof(Texture2D)).SetName(valueLabel.text));
                }
                else if (type == typeof(ScriptableObject))
                {
                    valueLabel.SetText($"Value <{type.Name}>");
                    valueContainer.AddChild(new ObjectField().ResetLayout().SetBindingPath(scriptableObjectValueProperty.propertyPath).SetObjectType(type).SetName(valueLabel.text));
                }
                else
                {
                    valueLabel.SetText($"Value <{type.Name}>");
                    valueContainer.AddChild(new ObjectField().ResetLayout().SetBindingPath(genericValueProperty.propertyPath).SetObjectType(type).SetName(valueLabel.text));
                }

                valueContainer.Bind(property.serializedObject);
            }

            UpdateModuleNameLabel(moduleProperty.objectReferenceValue != null ? (ModyModule)moduleProperty.objectReferenceValue : null);
            UpdateActionNames(modyActionRunner != null && modyActionRunner.Module != null ? modyActionRunner.Module : null);

            return drawer;
        }
    }
}
