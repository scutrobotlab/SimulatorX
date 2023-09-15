// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Editor.Nody.Nodes.Internal;
using Doozy.Editor.Reactor.Internal;
using Doozy.Editor.UIElements;
using Doozy.Runtime.Reactor.Extensions;
using Doozy.Runtime.Reactor.Reactions;
using Doozy.Runtime.UIElements.Extensions;
using Doozy.Runtime.UIManager.Nodes;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Doozy.Editor.UIManager.Nodes
{
    [CustomEditor(typeof(PortalNode))]
    public class PortalNodeEditor : FlowNodeEditor
    {
        public override IEnumerable<Texture2D> nodeIconTextures => EditorMicroAnimations.Nody.Icons.PortalNode;

        private Image icon { get; set; }
        private Texture2DReaction iconReaction { get; set; }

        private EnumField triggerEnumField { get; set; }
        private EnumField commandToggleEnumField { get; set; }
        private EnumField commandShowHideEnumField { get; set; }

        private PropertyField signalPayloadPropertyField { get; set; }
        private PropertyField buttonIdPropertyField { get; set; }
        private PropertyField toggleIdPropertyField { get; set; }
        private PropertyField viewIdPropertyField { get; set; }

        private VisualElement settingsContainer { get; set; }

        private SerializedProperty propertyTrigger { get; set; }
        private SerializedProperty propertyCommandToggle { get; set; }
        private SerializedProperty propertyCommandShowHide { get; set; }

        private SerializedProperty propertySignalPayload { get; set; }
        private SerializedProperty propertyButtonId { get; set; }
        private SerializedProperty propertyToggleId { get; set; }
        private SerializedProperty propertyViewId { get; set; }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            iconReaction?.Recycle();
        }

        protected override void FindProperties()
        {
            base.FindProperties();

            propertyTrigger = serializedObject.FindProperty("Trigger");
            propertyCommandToggle = serializedObject.FindProperty("CommandToggle");
            propertyCommandShowHide = serializedObject.FindProperty("CommandShowHide");

            propertySignalPayload = serializedObject.FindProperty("SignalPayload");
            propertyButtonId = serializedObject.FindProperty("ButtonId");
            propertyToggleId = serializedObject.FindProperty("ToggleId");
            propertyViewId = serializedObject.FindProperty("ViewId");
        }

        protected override void InitializeEditor()
        {
            base.InitializeEditor();

            componentHeader
                .SetComponentNameText(ObjectNames.NicifyVariableName(nameof(PortalNode)))
                .AddManualButton("https://doozyentertainment.atlassian.net/wiki/spaces/DUI4/pages/1048182826/Portal+Node?atlOrigin=eyJpIjoiNTJhMWI3NTBiNTBhNDg2M2I2Mzc2ZTQyOGIzZjY5MmMiLCJwIjoiYyJ9")
                .AddYouTubeButton();

            icon =
                new Image()
                    .SetName("Icon")
                    .ResetLayout()
                    .SetStyleFlexShrink(0)
                    .SetStyleSize(26)
                    .SetStyleAlignSelf(Align.Center)
                    .SetStyleBackgroundImageTintColor(EditorColors.Default.FieldIcon);

            iconReaction =
                icon.GetTexture2DReaction().SetEditorHeartbeat();

            triggerEnumField =
                DesignUtils.NewEnumField(propertyTrigger)
                    .SetStyleFlexGrow(1)
                    .SetStyleMarginLeft(DesignUtils.k_Spacing2X);

            triggerEnumField.RegisterValueChangedCallback(evt => root.schedule.Execute(RefreshData));

            commandToggleEnumField =
                DesignUtils.NewEnumField(propertyCommandToggle)
                    .SetStyleFlexShrink(0)
                    .SetStyleDisplay(DisplayStyle.None)
                    .SetStyleMarginLeft(DesignUtils.k_Spacing)
                    .SetStyleWidth(48);

            commandShowHideEnumField =
                DesignUtils.NewEnumField(propertyCommandShowHide)
                    .SetStyleFlexShrink(0)
                    .SetStyleDisplay(DisplayStyle.None)
                    .SetStyleMarginLeft(DesignUtils.k_Spacing)
                    .SetStyleWidth(56);

            signalPayloadPropertyField = DesignUtils.NewPropertyField(propertySignalPayload);
            buttonIdPropertyField = DesignUtils.NewPropertyField(propertyButtonId);
            toggleIdPropertyField = DesignUtils.NewPropertyField(propertyToggleId);
            viewIdPropertyField = DesignUtils.NewPropertyField(propertyViewId);

            settingsContainer =
                new VisualElement()
                    .SetStyleFlexGrow(1)
                    .SetStyleMarginLeft(-2)
                    .SetStyleMarginTop(DesignUtils.k_Spacing);

            root.schedule.Execute(RefreshData);
        }


        protected override void Compose()
        {
            base.Compose();

            root
                .AddChild(DesignUtils.spaceBlock2X)
                .AddChild
                (
                    DesignUtils.column
                        .AddChild
                        (
                            DesignUtils.row
                                .AddChild(icon)
                                .AddChild(DesignUtils.spaceBlock)
                                .AddChild(triggerEnumField)
                                .AddChild(commandToggleEnumField)
                                .AddChild(commandShowHideEnumField)
                        )
                        .AddChild
                        (
                            DesignUtils.row
                                .AddChild(settingsContainer)
                        )
                )
                ;

        }

        private void RefreshData()
        {
            var trigger = (PortalNode.TriggerCondition)propertyTrigger.enumValueIndex;

            settingsContainer.RecycleAndClear();
            commandToggleEnumField.SetStyleDisplay(trigger == PortalNode.TriggerCondition.UIToggle ? DisplayStyle.Flex : DisplayStyle.None);
            commandShowHideEnumField.SetStyleDisplay(trigger == PortalNode.TriggerCondition.UIView ? DisplayStyle.Flex : DisplayStyle.None);

            iconReaction.SetTextures(GetTextures(trigger));

            settingsContainer.AddChild
            (
                trigger == PortalNode.TriggerCondition.Signal ? GetSignalContainer() :
                trigger == PortalNode.TriggerCondition.UIButton ? GetUIButtonContainer() :
                trigger == PortalNode.TriggerCondition.UIToggle ? GetUIToggleContainer() :
                trigger == PortalNode.TriggerCondition.UIView ? GetUIViewContainer() : throw new ArgumentOutOfRangeException()
            );

            settingsContainer.Bind(serializedObject);
        }

        private VisualElement GetSignalContainer()
        {
            return signalPayloadPropertyField;
        }

        private VisualElement GetUIButtonContainer()
        {
            return buttonIdPropertyField;
        }

        private VisualElement GetUIToggleContainer()
        {
            return toggleIdPropertyField;
        }

        private VisualElement GetUIViewContainer()
        {
            return viewIdPropertyField;
        }

        private static IEnumerable<Texture2D> GetTextures(PortalNode.TriggerCondition trigger)
        {
            switch (trigger)
            {
                case PortalNode.TriggerCondition.Signal:
                    return EditorMicroAnimations.Signals.Icons.SignalStream;
                case PortalNode.TriggerCondition.UIButton:
                    return EditorMicroAnimations.UIManager.Icons.Buttons;
                case PortalNode.TriggerCondition.UIToggle:
                    return EditorMicroAnimations.UIManager.Icons.UIToggleCheckbox;
                case PortalNode.TriggerCondition.UIView:
                    return EditorMicroAnimations.UIManager.Icons.Views;
                default:
                    throw new ArgumentOutOfRangeException(nameof(trigger), trigger, null);
            }
        }
    }
}
