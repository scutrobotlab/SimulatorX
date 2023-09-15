// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.Components.Internal;
using Doozy.Editor.EditorUI.ScriptableObjects.Colors;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Runtime.UIElements.Extensions;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Editor.UIManager.Editors.Animators.Internal
{
    public abstract class BaseTargetComponentAnimatorEditor : UnityEditor.Editor
    {
        protected const float k_MarginLeft = 35;

        protected virtual Color accentColor => EditorColors.Reactor.Red;
        protected virtual EditorSelectableColorInfo selectableAccentColor => EditorSelectableColors.Reactor.Red;

        protected VisualElement root { get; set; }
        protected FluidComponentHeader componentHeader { get; set; }

        protected FluidField controllerField { get; set; }
        protected FluidToggleGroup tabsGroup { get; set; }

        protected ObjectField controllerObjectField { get; set; }

        protected SerializedProperty propertyController { get; set; }

        protected VisualElement animatedContainers { get; set; }
        protected VisualElement tabsContainer { get; set; }

        public override VisualElement CreateInspectorGUI()
        {
            InitializeEditor();
            Compose();
            return root;
        }

        protected virtual void OnDestroy()
        {
            componentHeader?.Recycle();

            controllerField?.Recycle();

            tabsGroup?.Recycle();
        }

        protected virtual void FindProperties()
        {
            propertyController = serializedObject.FindProperty("Controller");
        }

        protected virtual void InitializeEditor()
        {
            FindProperties();

            root = new VisualElement();

            componentHeader =
                FluidComponentHeader.Get()
                    .SetAccentColor(accentColor)
                    .SetElementSize(ElementSize.Small);

            InitializeController();
            InitializeAnimatedContainers();
            InitializeTabs();
        }

        protected virtual void InitializeAnimatedContainers()
        {
            animatedContainers =
                new VisualElement()
                    .SetName("Animated Containers");
        }
        
        protected abstract void ComposeAnimatedContainers();

        protected virtual void InitializeTabs()
        {
            tabsContainer =
                DesignUtils.row
                    .SetName("Tabs Container")
                    .SetStyleMargins(42, -2, DesignUtils.k_Spacing2X, DesignUtils.k_Spacing2X);
        }
        
        protected abstract void ComposeTabs();

        protected abstract void InitializeController();

        protected virtual void Compose()
        {
            ComposeTabs();
            ComposeAnimatedContainers();

            root
                .AddChild(componentHeader)
                .AddChild(tabsContainer)
                .AddChild(DesignUtils.spaceBlock)
                .AddChild(animatedContainers)
                .AddChild(controllerField)
                .AddChild(DesignUtils.endOfLineBlock);
        }

        protected FluidAnimatedContainer GetAnimatedContainer(SerializedProperty animationProperty)
        {
            FluidAnimatedContainer container = new FluidAnimatedContainer().SetClearOnHide(true).Hide(false).SetStyleMarginLeft(k_MarginLeft);
            container.SetOnShowCallback(() =>
            {
                container.AddContent(DesignUtils.NewPropertyField(animationProperty));
                container.AddContent(DesignUtils.endOfLineBlock);
                container.Bind(serializedObject);
            });

            return container;
        }

        protected static FluidToggleButtonTab GetTab(FluidAnimatedContainer targetContainer, string labelText, string tooltip, EditorSelectableColorInfo selectableColor) =>
            DesignUtils.GetTabButtonForComponentSection(null, selectableColor)
                .SetLabelText(labelText)
                .SetTooltip(tooltip)
                .SetOnValueChanged(evt => targetContainer.Toggle(evt.newValue));
    }
}
