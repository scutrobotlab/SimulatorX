// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using System.Reflection;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.Components.Internal;
using Doozy.Editor.EditorUI.ScriptableObjects.Colors;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Editor.Mody.Components;
using Doozy.Runtime.Common.Extensions;
using Doozy.Runtime.Mody;
using Doozy.Runtime.UIElements.Extensions;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Editor.Mody
{
    public class ModyModuleEditor<T> : EditorUIEditor<T> where T : ModyModule
    {
        public override Color accentColor => EditorColors.Mody.Module;
        public override EditorSelectableColorInfo selectableAccentColor => EditorSelectableColors.Mody.Module;
        public sealed override List<Texture2D> iconTextures => EditorMicroAnimations.Mody.Icons.ModyModule;

        private ModyModuleStateIndicator m_StateIndicator;

        protected VisualElement settingsContainer { get; private set; }

        protected ModyActionsDrawer actionsDrawer { get; private set; }
        protected VisualElement actionsContainer { get; private set; }

        protected SerializedProperty moduleNameProperty { get; private set; }
        protected TextField moduleNameTextField { get; private set; }
        protected FluidField moduleNameField { get; private set; }
        protected FluidAnimatedContainer moduleNameAnimatedContainer { get; private set; }
        protected FluidToggleButtonTab moduleNameTab { get; private set; }

        protected virtual void OnDisable()
        {
            moduleNameTab?.Recycle();
            moduleNameField?.Recycle();
            moduleNameAnimatedContainer?.Dispose();
            m_StateIndicator?.Dispose();
        }

        protected override void CreateEditor()
        {
            base.CreateEditor();

            actionsDrawer = new ModyActionsDrawer();

            settingsContainer = new VisualElement();
            actionsContainer = new VisualElement();

            moduleNameProperty = serializedObject.FindProperty("ModuleName");
            moduleNameTextField = DesignUtils.NewTextField(moduleNameProperty).SetStyleFlexGrow(1);
            moduleNameTextField.RegisterValueChangedCallback(evt => UpdateComponentTypeText(evt.newValue));
            moduleNameField =
                FluidField.Get()
                    .SetLabelText("Module Name")
                    .SetTooltip("Name of the Module")
                    .AddFieldContent(moduleNameTextField)
                    .SetIcon(EditorMicroAnimations.EditorUI.Icons.Label)
                    .SetStyleMarginTop(DesignUtils.k_Spacing)
                    .SetStyleMarginBottom(DesignUtils.k_EndOfLineSpacing);

            moduleNameAnimatedContainer =
                new FluidAnimatedContainer()
                    .SetClearOnHide(false)
                    .Hide(false)
                    .AddContent(moduleNameField);

            moduleNameTab = DesignUtils.NameTab()
                .SetLabelText("Module Name")
                .SetOnValueChanged(evt => moduleNameAnimatedContainer.Toggle(evt.newValue));
            
            void UpdateComponentTypeText(string nameOfTheModule)
            {
                nameOfTheModule = nameOfTheModule.IsNullOrEmpty() ? string.Empty : $" - {nameOfTheModule}";
                fluidHeader.SetComponentTypeText($"{nameof(Module)}{nameOfTheModule}");
            }

            UpdateComponentTypeText(moduleNameProperty.stringValue);

            fluidHeader.SetIcon(iconTextures);
            fluidHeader.SetElementSize(ElementSize.Small);

            m_StateIndicator = new ModyModuleStateIndicator().SetStyleMarginLeft(DesignUtils.k_Spacing2X);
            castedTarget.UpdateState();
            m_StateIndicator.UpdateState(castedTarget.state);

            EnumField invisibleStateEnum = DesignUtils.NewEnumField("ModuleCurrentState", true); 
            root.Add(invisibleStateEnum);
            invisibleStateEnum?.RegisterValueChangedCallback(evt =>
            {
                if (evt?.newValue == null) return;
                m_StateIndicator?.UpdateState((ModuleState)evt.newValue);
            });

            if (EditorApplication.isPlayingOrWillChangePlaymode)
                fluidHeader.AddElement(m_StateIndicator);
        }

        protected void AddActionToDrawer(ModyActionsDrawerItem item)
        {
            actionsDrawer.AddItem(item);
            actionsContainer.AddChild(item.animatedContainer);
            
         
        }

        protected void Compose()
        {
            root
                .AddChild
                (
                    DesignUtils.row
                        .SetStyleMargins(DesignUtils.k_Spacing2X, -10, DesignUtils.k_Spacing2X, DesignUtils.k_Spacing)
                        .AddChild(DesignUtils.flexibleSpace)
                        .AddChild(moduleNameTab)
                )
                .AddChild(moduleNameAnimatedContainer)
                .AddChild(settingsContainer)
                .AddChild(DesignUtils.spaceBlock2X)
                .AddChild
                (
                    DesignUtils.row
                        .AddChild(actionsDrawer)
                )
                .AddChild(DesignUtils.spaceBlock)
                .AddChild(actionsContainer)
                .AddChild(DesignUtils.endOfLineBlock);

            actionsDrawer.schedule.Execute(() => actionsDrawer.Update());
        }
    }
}
