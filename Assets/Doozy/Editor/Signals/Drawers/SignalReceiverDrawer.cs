// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Runtime.Colors;
using Doozy.Runtime.Signals;
using Doozy.Runtime.UIElements.Extensions;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Doozy.Editor.Signals.Drawers
{
    [CustomPropertyDrawer(typeof(SignalReceiver), true)]
    public class SignalReceiverDrawer : PropertyDrawer

    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {}

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            // var signalReceiver = property.GetTargetObjectOfProperty() as SignalReceiver;

            VisualElement drawer =
                new VisualElement()
                    .SetStyleBackgroundColor(Color.black.WithAlpha(0.1f))
                    .SetStyleBorderRadius(DesignUtils.k_Spacing * 2)
                    .SetStylePadding(DesignUtils.k_Spacing * 2)
                    .SetStyleMargins(DesignUtils.k_Spacing);

            SerializedProperty connectionSerializedProperty = property.FindPropertyRelative("ConnectionMode");

            EnumField signalStreamConnectionEnum = DesignUtils.NewEnumField(connectionSerializedProperty.propertyPath).SetStyleFlexGrow(1);
            PropertyField signalProviderIdPropertyField = DesignUtils.NewPropertyField("SignalProviderId");
            ObjectField providerReferenceObjectField = DesignUtils.NewObjectField("ProviderReference", typeof(SignalProvider)).SetStyleFlexGrow(1);
            PropertyField signalStreamIdPropertyField = DesignUtils.NewPropertyField("StreamId");

            var fieldConnection = FluidField.Get("Stream Connection Mode", signalStreamConnectionEnum);
            var fieldSignalProviderId = FluidField.Get(signalProviderIdPropertyField);
            var fieldProviderReference = FluidField.Get("Signal Provider", providerReferenceObjectField);
            var fieldStreamId = FluidField.Get(signalStreamIdPropertyField);

            drawer
                .AddChild
                (
                    DesignUtils.row
                        .AddChild(fieldSignalProviderId.SetStyleMarginRight(DesignUtils.k_Spacing))
                        .AddChild(fieldProviderReference.SetStyleMarginRight(DesignUtils.k_Spacing))
                        .AddChild(fieldStreamId.SetStyleMarginRight(DesignUtils.k_Spacing))
                        .AddChild(fieldConnection)
                );



            //Refresh on SignalStreamConnection changed
            signalStreamConnectionEnum.RegisterValueChangedCallback(evt => drawer.schedule.Execute(Refresh));

            //Refresh on SignalProviderId mouse leave
            signalProviderIdPropertyField.RegisterCallback<MouseLeaveEvent>(evt => drawer.schedule.Execute(Refresh));

            Refresh();

            drawer.RegisterCallback<DetachFromPanelEvent>(evt =>
            {
                fieldConnection?.Recycle();
                fieldSignalProviderId?.Recycle();
                fieldProviderReference?.Recycle();
                fieldStreamId?.Recycle();
            });


            void Refresh()
            {
                var streamConnection = (StreamConnection)connectionSerializedProperty.enumValueIndex;

                fieldSignalProviderId.SetStyleDisplay(streamConnection == StreamConnection.ProviderId ? DisplayStyle.Flex : DisplayStyle.None);
                fieldProviderReference.SetStyleDisplay(streamConnection == StreamConnection.ProviderReference ? DisplayStyle.Flex : DisplayStyle.None);
                fieldStreamId.SetStyleDisplay(streamConnection == StreamConnection.StreamId ? DisplayStyle.Flex : DisplayStyle.None);

                switch (streamConnection)
                {
                    case StreamConnection.None:
                        fieldConnection.style.minWidth = new StyleLength(StyleKeyword.Auto);
                        fieldConnection.style.width = new StyleLength(StyleKeyword.Auto);
                        fieldConnection.style.maxWidth = new StyleLength(StyleKeyword.Auto);
                        break;
                    case StreamConnection.ProviderId:
                    case StreamConnection.StreamId:
                    case StreamConnection.ProviderReference:
                        const float width = 140f;
                        fieldConnection.SetStyleWidth(width, width, width);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return drawer;
        }
    }
}
