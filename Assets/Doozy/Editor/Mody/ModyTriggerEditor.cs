// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.ScriptableObjects.Colors;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Editor.Mody.Components;
using Doozy.Editor.UIElements;
using Doozy.Runtime.Signals;
using Doozy.Runtime.UIElements.Extensions;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Editor.Mody
{
    public class ModyTriggerEditor<T> : EditorUIEditor<T> where T : SignalProvider
    {
        public override Color accentColor => EditorColors.Mody.Trigger;
        public override EditorSelectableColorInfo selectableAccentColor => EditorSelectableColors.Mody.Trigger;
        public sealed override List<Texture2D> iconTextures => EditorMicroAnimations.Mody.Icons.ModyTrigger;

        private ModyProviderStateIndicator m_StateIndicator;
        
        protected virtual void OnDisable()
        {
            m_StateIndicator?.Dispose();
        }
        
        protected override void CreateEditor()
        {
            base.CreateEditor();
            
            fluidHeader.SetIcon(iconTextures);
            fluidHeader.SetElementSize(ElementSize.Small);
            fluidHeader.SetComponentNameText($"{castedTarget.attributes.id.Category}.{castedTarget.attributes.id.Name}");
            fluidHeader.SetComponentTypeText($"{castedTarget.attributes.id.Type} Trigger");

            m_StateIndicator = new ModyProviderStateIndicator().SetStyleMarginLeft(DesignUtils.k_Spacing2X);
            m_StateIndicator.UpdateState(castedTarget, castedTarget.currentState);
            castedTarget.onStateChanged = state => m_StateIndicator.UpdateState(castedTarget, state);
            
            if (EditorApplication.isPlayingOrWillChangePlaymode)
                fluidHeader.AddElement(m_StateIndicator);
        }

        protected void Compose()
        {
            root
                .AddChild
                (
                    DesignUtils.row
                        .AddChild(DesignUtils.flexibleSpace)
                        .AddChild(DesignUtils.spaceBlock)
                        .AddChild(GetCooldownFluidField(serializedObject))
                        .AddChild(DesignUtils.spaceBlock)
                        .AddChild(GetTimescaleFluidField(serializedObject))
                )
                .AddChild(DesignUtils.spaceBlock);
        }
        
        protected static FluidField GetCooldownFluidField(SerializedObject serializedObject) =>
            TimeField
            (
                serializedObject,
                "SignalCooldown",
                ModyProviderStateIndicator.cooldownColor,
                "Cooldown",
                "Cooldown\n\nCooldown time after a signal was sent. During this time, no Signal will be sent",
                EditorMicroAnimations.EditorUI.Icons.Cooldown
            );
        
        protected static FluidField GetTimescaleFluidField(SerializedObject serializedObject)
        {
            SerializedProperty targetProperty = serializedObject.FindProperty("SignalTimescale");
            EnumField enumField =
                new EnumField()
                    .ResetLayout()
                    .BindToProperty(targetProperty)
                    .SetStyleMinWidth(94);

            FluidField field =
                FluidField.Get()
                    // .SetLabelText("Timescale")
                    .SetTooltip
                    (
                        "Timescale" +
                        "\n\nDetermine if the Signal's timers will be affected by the application's timescale" +
                        "\n\nTimescale.Independent - (Realtime)\nNot affected by the application's timescale value" +
                        "\n\nTimescale.Dependent - (Application Time)\nAffected by the application's timescale value"
                    )
                    .SetElementSize(ElementSize.Small)
                    .SetIcon(EditorMicroAnimations.EditorUI.Icons.TimeScale)
                    .SetStyleMaxWidth(160)
                    .AddFieldContent(enumField);
            return field;
        }
        
        protected static FluidField TimeField(SerializedObject serializedObject, string targetPropertyName, Color enabledColor, string fieldLabelText, string fieldTooltip, IEnumerable<Texture2D> fieldIconTextures)
        {
            SerializedProperty targetProperty = serializedObject.FindProperty(targetPropertyName);
            EnabledIndicator indicator = EnabledIndicator.Get().SetIcon(fieldIconTextures).SetEnabledColor(enabledColor).SetSize(18);
            FloatField floatField = new FloatField().ResetLayout().BindToProperty(targetProperty).SetStyleFlexGrow(1);
            floatField.RegisterValueChangedCallback(evt => indicator.Toggle(evt.newValue > 0, true));
            indicator.Toggle(targetProperty.floatValue > 0, false);

            return FluidField.Get()
                // .SetLabelText(fieldLabelText)
                .SetTooltip(fieldTooltip)
                .SetElementSize(ElementSize.Tiny)
                .SetStyleMaxWidth(120)
                .AddFieldContent
                (
                    DesignUtils.row.SetStyleFlexGrow(0)
                        .AddChild(indicator.SetStyleMarginRight(DesignUtils.k_Spacing))
                        .AddChild(floatField)
                );
        }
    }
}
