// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Editor.Reactor.Ticker;
using Doozy.Runtime.Reactor;
using Doozy.Runtime.Reactor.ScriptableObjects;
using Doozy.Runtime.Reactor.Ticker;
using Doozy.Runtime.UIElements.Extensions;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
// ReSharper disable ClassNeverInstantiated.Global

namespace Doozy.Editor.Reactor.Layouts
{
    public sealed class ReactorSettingsWindowLayout : FluidWindowLayout
    {
        public override List<Texture2D> animatedIconTextures => EditorMicroAnimations.EditorUI.Icons.Settings;

        private SerializedObject serializedObject { get; set; }

        private EnumField runtimeFPSEnumField { get; set; }
        private IntegerField runtimeCustomFPSIntegerField { get; set; }

        private EnumField editorFPSEnumField { get; set; }
        private IntegerField editorCustomFPSIntegerField { get; set; }

        private FPSGauge editorFPSGauge { get; }
        private FPSGauge runtimeFPSGauge { get; }

        public ReactorSettingsWindowLayout()
        {
            ReactorSettings settings = ReactorSettings.instance;
            serializedObject = new SerializedObject(settings);

            AddHeader("Reactor", "Global Settings", animatedIconTextures);

            menu.SetStyleDisplay(DisplayStyle.None);

            editorFPSGauge =
                new FPSGauge()
                    .SetIcon(EditorMicroAnimations.Reactor.Icons.EditorHeartbeat)
                    .SetTitle("Editor\nHeartbeat")
                    .SetValue("30")
                    .SetType("fps")
                    .SetFPS(settings.EditorFPS, settings.CustomEditorFPS);

            runtimeFPSGauge =
                new FPSGauge()
                    .SetIcon(EditorMicroAnimations.Reactor.Icons.Heartbeat)
                    .SetTitle("Runtime\nHeartbeat")
                    .SetValue("30")
                    .SetType("fps")
                    .SetFPS(settings.RuntimeFPS, settings.CustomRuntimeFPS);

            runtimeFPSEnumField =
                new EnumField { bindingPath = "RuntimeFPS" }
                    .ResetLayout()
                    .SetStyleWidth(120);

            runtimeCustomFPSIntegerField =
                new IntegerField { bindingPath = "CustomRuntimeFPS" }
                    .ResetLayout()
                    .SetStyleWidth(60)
                    .SetStyleDisplay(settings.RuntimeFPS == FPS.CustomFPS ? DisplayStyle.Flex : DisplayStyle.None);

            editorFPSEnumField =
                new EnumField { bindingPath = "EditorFPS" }
                    .ResetLayout()
                    .SetStyleWidth(120);

            editorCustomFPSIntegerField =
                new IntegerField { bindingPath = "CustomEditorFPS" }
                    .ResetLayout()
                    .SetStyleWidth(60)
                    .SetStyleDisplay(settings.EditorFPS == FPS.CustomFPS ? DisplayStyle.Flex : DisplayStyle.None);

            editorCustomFPSIntegerField.SetStyleDisplay(settings.EditorFPS == FPS.CustomFPS ? DisplayStyle.Flex : DisplayStyle.None);

            editorCustomFPSIntegerField.RegisterValueChangedCallback(evt =>
            {
                int cleanValue = Mathf.Max(evt.newValue, 1);
                editorCustomFPSIntegerField.SetValueWithoutNotify(cleanValue);
                editorFPSGauge.SetFPS((FPS)editorFPSEnumField.value, cleanValue);
                EditorUtility.SetDirty(settings);
                schedule.Execute(() => EditorTicker.service.SetFPS(ReactorSettings.editorFPS));
            });

            editorFPSEnumField.RegisterValueChangedCallback(evt =>
            {
                editorCustomFPSIntegerField.SetStyleDisplay((FPS)evt.newValue == FPS.CustomFPS ? DisplayStyle.Flex : DisplayStyle.None);
                editorFPSGauge.SetFPS((FPS)evt.newValue, editorCustomFPSIntegerField.value);
                EditorUtility.SetDirty(settings);
                schedule.Execute(() => EditorTicker.service.SetFPS(ReactorSettings.editorFPS));
            });

            runtimeCustomFPSIntegerField.RegisterValueChangedCallback(evt =>
            {
                int cleanValue = Mathf.Max(evt.newValue, 1);
                runtimeCustomFPSIntegerField.SetValueWithoutNotify(cleanValue);
                runtimeFPSGauge.SetFPS((FPS)runtimeFPSEnumField.value, cleanValue);
                EditorUtility.SetDirty(settings);
                AssetDatabase.SaveAssetIfDirty(settings);
                if (Application.isPlaying)
                {
                    schedule.Execute(() => RuntimeTicker.service.SetFPS(ReactorSettings.runtimeFPS));
                }
            });

            runtimeFPSEnumField.RegisterValueChangedCallback(evt =>
            {
                runtimeCustomFPSIntegerField.SetStyleDisplay((FPS)evt.newValue == FPS.CustomFPS ? DisplayStyle.Flex : DisplayStyle.None);
                runtimeFPSGauge.SetFPS((FPS)evt.newValue, editorCustomFPSIntegerField.value);
                EditorUtility.SetDirty(settings);
                AssetDatabase.SaveAssetIfDirty(settings);
                if (Application.isPlaying)
                {
                    schedule.Execute(() => RuntimeTicker.service.SetFPS(ReactorSettings.runtimeFPS));
                }
            });



            Compose();
            content.Bind(serializedObject);
        }


        private VisualElement simpleContainer => new VisualElement().SetStyleAlignItems(Align.Center).SetStyleJustifyContent(Justify.Center);

        private void Compose()
        {
            content
                .AddChild
                (
                    simpleContainer
                        .SetName("Main Container")
                        .SetStyleFlexDirection(FlexDirection.Row)
                        .SetStyleJustifyContent(Justify.SpaceAround)
                        .SetStyleFlexGrow(1)
                        .AddChild
                        (
                            simpleContainer
                                .AddChild(editorFPSGauge)
                                .AddChild(DesignUtils.spaceBlock4X)
                                .AddChild
                                (
                                    DesignUtils.column
                                        .SetTooltip("Refresh rate for the EditorTicker")
                                        .SetStyleFlexGrow(0)
                                        .AddChild
                                        (
                                            DesignUtils.column.SetStyleFlexGrow(0).SetStyleAlignItems(Align.Center)
                                                .AddChild(DesignUtils.NewFieldNameLabel("Editor FPS").SetStyleFontSize(11))
                                                .AddChild(DesignUtils.NewFieldNameLabel("Frames Per Second").SetStyleFontSize(9)
                                                )
                                                .AddChild
                                                (
                                                    DesignUtils.column
                                                        .SetStyleFlexGrow(0)
                                                        .SetStyleAlignItems(Align.Center)
                                                        .SetStyleMarginTop(DesignUtils.k_Spacing)
                                                        .AddChild(editorFPSEnumField)
                                                        .AddChild(editorCustomFPSIntegerField.SetStyleMarginTop(DesignUtils.k_Spacing))
                                                )
                                        )
                                )
                        )
                        .AddChild
                        (
                            simpleContainer
                                .AddChild(runtimeFPSGauge)
                                .AddChild(DesignUtils.spaceBlock4X)
                                .AddChild
                                (
                                    DesignUtils.column
                                        .SetTooltip("Refresh rate for the RuntimeTicker")
                                        .SetStyleFlexGrow(0)
                                        .AddChild
                                        (
                                            DesignUtils.column.SetStyleFlexGrow(0).SetStyleAlignItems(Align.Center)
                                                .AddChild(DesignUtils.NewFieldNameLabel("Runtime FPS").SetStyleFontSize(11))
                                                .AddChild(DesignUtils.NewFieldNameLabel("Frames Per Second").SetStyleFontSize(9)
                                                )
                                                .AddChild
                                                (
                                                    DesignUtils.column
                                                        .SetStyleFlexGrow(0)
                                                        .SetStyleAlignItems(Align.Center)
                                                        .SetStyleMarginTop(DesignUtils.k_Spacing)
                                                        .AddChild(runtimeFPSEnumField)
                                                        .AddChild(runtimeCustomFPSIntegerField.SetStyleMarginTop(DesignUtils.k_Spacing))
                                                )
                                        )
                                )
                        )
                );
        }
    }
}
