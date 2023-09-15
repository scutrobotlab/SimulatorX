// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using System.Linq;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.Components.Internal;
using Doozy.Editor.EditorUI.ScriptableObjects.Colors;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Editor.Reactor.Components;
using Doozy.Runtime.Common.Extensions;
using Doozy.Runtime.Reactor;
using Doozy.Runtime.Reactor.Animators.Internal;
using Doozy.Runtime.UIElements.Extensions;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
// ReSharper disable MemberCanBeProtected.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Editor.Reactor.Editors.Animators.Internal
{
    public abstract class BaseReactorAnimatorEditor : UnityEditor.Editor
    {
        public static Color accentColor => EditorColors.Reactor.Red;
        public static EditorSelectableColorInfo selectableAccentColor => EditorSelectableColors.Reactor.Red;

        public static IEnumerable<Texture2D> reactorIconTextures => EditorMicroAnimations.Reactor.Icons.Reactor;

        protected VisualElement root { get; set; }
        protected FluidComponentHeader componentHeader { get; set; }
        protected ReactionControls controls { get; set; }

        protected FluidField onStartBehaviourFluidField { get; set; }
        protected FluidField onEnableBehaviourFluidField { get; set; }

        protected PropertyField animationPropertyField { get; set; }
        protected TextField animatorNameTextField { get; private set; }
        protected FluidField animatorNameField { get; private set; }

        protected FluidToggleGroup tabsGroup { get; private set; }
        protected FluidToggleButtonTab animationTabButton { get; private set; }
        protected FluidToggleButtonTab settingsTab { get; private set; }
        protected FluidToggleButtonTab animatorNameTab { get; private set; }

        protected FluidAnimatedContainer animationAnimatedContainer { get; private set; }
        protected FluidAnimatedContainer behaviourAnimatedContainer { get; private set; }
        protected FluidAnimatedContainer animatorNameAnimatedContainer { get; private set; }

        protected SerializedProperty propertyAnimatorName { get; set; }
        protected SerializedProperty propertyAnimation { get; set; }
        protected SerializedProperty propertyOnStartBehaviour { get; set; }
        protected SerializedProperty propertyOnEnableBehaviour { get; set; }

        public override VisualElement CreateInspectorGUI()
        {
            InitializeEditor();
            Compose();
            return root;
        }
        
        protected virtual void OnDestroy()
        {
            componentHeader?.Recycle();
            onStartBehaviourFluidField?.Recycle();
            onEnableBehaviourFluidField?.Recycle();

            tabsGroup?.Recycle();

            animationTabButton?.Recycle();
            settingsTab?.Recycle();
            animatorNameTab?.Recycle();

            animatorNameField?.Recycle();

            animationAnimatedContainer?.Dispose();
            behaviourAnimatedContainer?.Dispose();
            animatorNameAnimatedContainer?.Dispose();
        }

        protected virtual void FindProperties()
        {
            propertyAnimatorName = serializedObject.FindProperty("AnimatorName");
            propertyAnimation = serializedObject.FindProperty("Animation");
            propertyOnStartBehaviour = serializedObject.FindProperty(nameof(ReactorAnimator.OnStartBehaviour));
            propertyOnEnableBehaviour = serializedObject.FindProperty(nameof(ReactorAnimator.OnEnableBehaviour));
        }

        protected virtual void InitializeEditor()
        {
            FindProperties();

            root = new VisualElement();

            componentHeader = FluidComponentHeader.Get()
                .SetAccentColor(accentColor)
                .SetComponentNameText("Reactor")
                .SetComponentTypeText("Animator")
                .SetIcon(reactorIconTextures.ToList())
                .SetElementSize(ElementSize.Small);

            animationAnimatedContainer = new FluidAnimatedContainer().SetClearOnHide(false).Show(false);
            behaviourAnimatedContainer = new FluidAnimatedContainer().SetClearOnHide(false).Hide(false);
            animatorNameAnimatedContainer = new FluidAnimatedContainer().SetClearOnHide(false).Hide(false);

            animationTabButton =
                DesignUtils.GetTabButtonForComponentSection(reactorIconTextures)
                    .SetLabelText("Animation")
                    .SetOnValueChanged(evt => animationAnimatedContainer.Toggle(evt.newValue));

            settingsTab =
                DesignUtils.GetTabButtonForComponentSection(EditorMicroAnimations.EditorUI.Icons.Settings)
                    .SetLabelText("Behaviour")
                    .SetOnValueChanged(evt => behaviourAnimatedContainer.Toggle(evt.newValue));


            animatorNameTab = 
                DesignUtils.NameTab()
                .SetLabelText("Animator Name")
                .SetOnValueChanged(evt => animatorNameAnimatedContainer.Toggle(evt.newValue));

            tabsGroup = FluidToggleGroup.Get().SetControlMode(FluidToggleGroup.ControlMode.OneToggleOnEnforced);
            animationTabButton.AddToToggleGroup(tabsGroup);
            settingsTab.AddToToggleGroup(tabsGroup);
            animatorNameTab.AddToToggleGroup(tabsGroup);

            animatorNameTextField = DesignUtils.NewTextField(propertyAnimatorName).SetStyleFlexGrow(1);
            animatorNameTextField.RegisterValueChangedCallback(evt => UpdateComponentTypeText(evt.newValue));
            animatorNameField =
                FluidField.Get()
                    .SetLabelText("Animator Name")
                    .SetTooltip("Name of the Animator")
                    .AddFieldContent(animatorNameTextField)
                    .SetIcon(EditorMicroAnimations.EditorUI.Icons.Label)
                    .SetStyleMarginTop(DesignUtils.k_Spacing);

            void UpdateComponentTypeText(string nameOfTheAnimator)
            {
                nameOfTheAnimator = nameOfTheAnimator.IsNullOrEmpty() ? string.Empty : $" - {nameOfTheAnimator}";
                componentHeader.SetComponentTypeText(nameOfTheAnimator);
            }

            UpdateComponentTypeText(propertyAnimatorName.stringValue);

            animationPropertyField = DesignUtils.NewPropertyField(propertyAnimation);

            EnumField onStartBehaviourEnumField = DesignUtils.NewEnumField(propertyOnStartBehaviour.propertyPath).SetStyleFlexGrow(1);
            onStartBehaviourFluidField =
                FluidField.Get("OnStart Behaviour")
                    .SetElementSize(ElementSize.Tiny)
                    .AddFieldContent(onStartBehaviourEnumField);

            onStartBehaviourEnumField.RegisterValueChangedCallback(evt =>
            {
                if (evt.newValue == null) return;
                UpdateInitialBehaviourTooltip(onStartBehaviourFluidField, (AnimatorBehaviour)evt.newValue);
            });
            UpdateInitialBehaviourTooltip(onStartBehaviourFluidField, (AnimatorBehaviour)propertyOnStartBehaviour.enumValueIndex);

            EnumField onEnableBehaviourEnumField = DesignUtils.NewEnumField(propertyOnEnableBehaviour.propertyPath).SetStyleFlexGrow(1);
            onEnableBehaviourFluidField = FluidField.Get("OnEnable Behaviour").SetElementSize(ElementSize.Tiny).AddFieldContent(onEnableBehaviourEnumField);
            onEnableBehaviourEnumField.RegisterValueChangedCallback(evt =>
            {
                if (evt.newValue == null) return;
                UpdateInitialBehaviourTooltip(onEnableBehaviourFluidField, (AnimatorBehaviour)evt.newValue);
            });
            UpdateInitialBehaviourTooltip(onEnableBehaviourFluidField, (AnimatorBehaviour)propertyOnEnableBehaviour.enumValueIndex);

            void UpdateInitialBehaviourTooltip(VisualElement targetElement, AnimatorBehaviour value)
            {
                string message;
                switch (value)
                {
                    case AnimatorBehaviour.Disabled:
                        message = "Do nothing";
                        break;
                    case AnimatorBehaviour.PlayForward:
                        message = "Play the animation forward (from 0 to 1)";
                        break;
                    case AnimatorBehaviour.PlayReverse:
                        message = "Play the animation in reverse (from 1 to 0)";
                        break;
                    case AnimatorBehaviour.SetFromValue:
                        message = "Set the animation at 'from' value (at the start value of the animation)";
                        break;
                    case AnimatorBehaviour.SetToValue:
                        message = "Set the animation at 'to' value (at the end value of the animation) ";
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(value), value, null);
                }

                targetElement.SetTooltip(message);
            }

            controls =
                new ReactionControls()
                    .SetStyleDisplay(EditorApplication.isPlaying ? DisplayStyle.Flex : DisplayStyle.None);
        }
        
        protected virtual void Compose()
        {
            const float marginLeft = 35;

            animationAnimatedContainer
                .fluidContainer
                .AddChild(animationPropertyField.SetStyleMarginLeft(marginLeft));

            behaviourAnimatedContainer
                .fluidContainer
                .AddChild
                (
                    DesignUtils.row
                        .SetStyleMarginLeft(marginLeft)
                        .AddChild(onStartBehaviourFluidField)
                        .AddChild(DesignUtils.spaceBlock)
                        .AddChild(onEnableBehaviourFluidField)
                )
                .AddChild(DesignUtils.endOfLineBlock);


            animatorNameAnimatedContainer
                .fluidContainer
                .AddChild(animatorNameField.SetStyleMarginLeft(marginLeft))
                .AddChild(DesignUtils.endOfLineBlock);

           
        }
    }
}
