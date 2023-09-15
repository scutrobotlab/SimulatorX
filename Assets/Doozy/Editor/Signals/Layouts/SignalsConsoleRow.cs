// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.ScriptableObjects.Colors;
using Doozy.Editor.Reactor.Internal;
using Doozy.Runtime.Common.Extensions;
using Doozy.Runtime.Reactor.Easings;
using Doozy.Runtime.Reactor.Internal;
using Doozy.Runtime.Reactor.Reactions;
using Doozy.Runtime.Signals;
using Doozy.Runtime.UIElements.Extensions;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using PlayMode = Doozy.Runtime.Reactor.PlayMode;
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Editor.Signals.Layouts
{
    public class SignalsConsoleRow : PoolableElement<SignalsConsoleRow>
    {
        private const string NONE = "---";
        
        public static SignalsConsoleRow Get(Signal targetSignal) =>
            Get().SetSignal(targetSignal);

        public Signal signal { get; private set; }

        private TemplateContainer templateContainer { get; }

        public VisualElement layoutContainer { get; }
        
        public Label timestampLabel { get; }
        
        public Image signalIcon { get; }
        public Image streamIcon { get; }
        
        public VisualElement detailsContainer { get; }
        public VisualElement signalInfoContainer { get; }
        public VisualElement messageContainer { get; }
        public VisualElement sourceGameObjectContainer { get; }
        public VisualElement signalProviderContainer { get; }
        public VisualElement signalSenderObjectContainer { get; }
        public VisualElement signalValueTypeContainer { get; }
        public VisualElement signalValueContainer { get; }
        public VisualElement streamInfoContainer { get; }
        public VisualElement streamSignalProviderContainer { get; }
        public VisualElement streamSignalProviderGameObjectContainer { get; }
        public VisualElement streamCategoryContainer { get; }
        public VisualElement streamNameContainer { get; }
        public VisualElement streamReceiversCountContainer { get; }
        public VisualElement streamInfoMessageContainer { get; }
        public VisualElement streamGuidContainer { get; }
        
        public Label messageLabel { get; }
        public Label sourceGameObjectLabel { get; }
        public Label signalProviderLabel { get; }
        public Label signalSenderObjectLabel { get; }
        public Label signalValueTypeLabel { get; }
        public Label signalValueLabel { get; }
        public Label streamSignalProviderLabel { get; }
        public Label streamSignalProviderGameObjectLabel { get; }
        public Label streamCategoryLabel { get; }
        public Label streamNameLabel { get; }
        public Label streamReceiversCountLabel { get; }
        public Label streamInfoMessageLabel { get; }
        public Label streamGuidLabel { get; }

        public FluidButton buttonPingSignalProvider { get; }
        public FluidButton buttonPingSenderObject { get; }
        public FluidButton buttonPingSourceGameObject { get; }
        public FluidButton buttonPingStreamProvider { get; }

        private Texture2DReaction signalIconReaction { get; set; }
        private Texture2DReaction streamIconReaction { get; set; }
        private ColorReaction colorReaction { get; set; }

        private static IEnumerable<Texture2D> signalTextures => EditorMicroAnimations.Signals.Icons.Signal;
        private static IEnumerable<Texture2D> metaSignalTextures => EditorMicroAnimations.Signals.Icons.MetaSignal;
        private static IEnumerable<Texture2D> streamTextures => EditorMicroAnimations.Signals.Icons.SignalStream;
        private static IEnumerable<Texture2D> locationTextures => EditorMicroAnimations.EditorUI.Icons.Location;

        private static Color layoutContainerNormalColor => EditorColors.Default.FieldBackground;
        private static Color layoutContainerHoverColor => EditorColors.Default.WindowHeaderBackground;
        private static Color initialContainerColor => EditorGUIUtility.isProSkin ? layoutContainerNormalColor * signalColor : layoutContainerNormalColor + signalColor * 0.3f;

        private static Color signalColor => EditorColors.Signals.Signal;
        private static EditorSelectableColorInfo signalSelectableColor => EditorSelectableColors.Signals.Signal;

        private static Color streamColor => EditorColors.Signals.Stream;
        private static EditorSelectableColorInfo streamSelectableColor => EditorSelectableColors.Signals.Stream;

        private static Color infoLabelColor => EditorColors.Default.TextTitle;
        private static Font infoLabelFont => EditorFonts.Ubuntu.Light;

        private static Color titleLabelColor => EditorColors.Default.TextDescription;
        private static Font titleLabelFont => EditorFonts.Ubuntu.Light;

        public SignalsConsoleRow()
        {
            this.SetStyleFlexGrow(1);
            
            Add(templateContainer = EditorLayouts.Signals.SignalsConsoleRow.CloneTree());

            templateContainer
                .SetStyleFlexGrow(1)
                .AddStyle(EditorUI.EditorStyles.Signals.SignalsConsoleRow);

            layoutContainer = templateContainer.Q<VisualElement>(nameof(layoutContainer));

            timestampLabel = layoutContainer.Q<Label>(nameof(timestampLabel));

            //ICONS
            signalIcon = layoutContainer.Q<Image>(nameof(signalIcon));
            streamIcon = layoutContainer.Q<Image>(nameof(streamIcon));

            //CONTAINERS
            detailsContainer = layoutContainer.Q<VisualElement>(nameof(detailsContainer));
            signalInfoContainer = layoutContainer.Q<VisualElement>(nameof(signalInfoContainer));
            messageContainer = layoutContainer.Q<VisualElement>(nameof(messageContainer));
            sourceGameObjectContainer = layoutContainer.Q<VisualElement>(nameof(sourceGameObjectContainer));
            signalProviderContainer = layoutContainer.Q<VisualElement>(nameof(signalProviderContainer));
            signalSenderObjectContainer = layoutContainer.Q<VisualElement>(nameof(signalSenderObjectContainer));
            signalValueTypeContainer = layoutContainer.Q<VisualElement>(nameof(signalValueTypeContainer));
            signalValueContainer = layoutContainer.Q<VisualElement>(nameof(signalValueContainer));
            streamInfoContainer = layoutContainer.Q<VisualElement>(nameof(streamInfoContainer));
            streamSignalProviderContainer = layoutContainer.Q<VisualElement>(nameof(streamSignalProviderContainer));
            streamSignalProviderGameObjectContainer = layoutContainer.Q<VisualElement>(nameof(streamSignalProviderGameObjectContainer));
            streamNameContainer = layoutContainer.Q<VisualElement>(nameof(streamNameContainer));
            streamCategoryContainer = layoutContainer.Q<VisualElement>(nameof(streamCategoryContainer));
            streamReceiversCountContainer = layoutContainer.Q<VisualElement>(nameof(streamReceiversCountContainer));
            streamInfoMessageContainer = layoutContainer.Q<VisualElement>(nameof(streamInfoMessageContainer));
            streamGuidContainer = layoutContainer.Q<VisualElement>(nameof(streamGuidContainer));

            //INFO LABELS
            var infoLabels = new List<Label>
            {
                (messageLabel = layoutContainer.Q<Label>(nameof(messageLabel))),
                (sourceGameObjectLabel = layoutContainer.Q<Label>(nameof(sourceGameObjectLabel))),
                (signalProviderLabel = layoutContainer.Q<Label>(nameof(signalProviderLabel))),
                (signalSenderObjectLabel = layoutContainer.Q<Label>(nameof(signalSenderObjectLabel))),
                (signalValueTypeLabel = layoutContainer.Q<Label>(nameof(signalValueTypeLabel))),
                (signalValueLabel = layoutContainer.Q<Label>(nameof(signalValueLabel))),
                (streamSignalProviderLabel = layoutContainer.Q<Label>(nameof(streamSignalProviderLabel))),
                (streamSignalProviderGameObjectLabel = layoutContainer.Q<Label>(nameof(streamSignalProviderGameObjectLabel))),
                (streamNameLabel = layoutContainer.Q<Label>(nameof(streamNameLabel))),
                (streamCategoryLabel = layoutContainer.Q<Label>(nameof(streamCategoryLabel))),
                (streamReceiversCountLabel = layoutContainer.Q<Label>(nameof(streamReceiversCountLabel))),
                (streamInfoMessageLabel = layoutContainer.Q<Label>(nameof(streamInfoMessageLabel))),
                (streamGuidLabel = layoutContainer.Q<Label>(nameof(streamGuidLabel)))
            };

            //TITLE LABELS
            const string titleLabelClassName = "TitleLabel";
            var titleLabels = new List<Label>
            {
                messageContainer.Q<Label>(null, titleLabelClassName),
                sourceGameObjectContainer.Q<Label>(null, titleLabelClassName),
                signalProviderContainer.Q<Label>(null, titleLabelClassName),
                signalSenderObjectContainer.Q<Label>(null, titleLabelClassName),
                signalValueTypeContainer.Q<Label>(null, titleLabelClassName),
                signalValueContainer.Q<Label>(null, titleLabelClassName),
                streamSignalProviderContainer.Q<Label>(null, titleLabelClassName),
                streamSignalProviderGameObjectContainer.Q<Label>(null, titleLabelClassName),
                streamNameContainer.Q<Label>(null, titleLabelClassName),
                streamCategoryContainer.Q<Label>(null, titleLabelClassName),
                streamReceiversCountContainer.Q<Label>(null, titleLabelClassName),
                streamInfoMessageContainer.Q<Label>(null, titleLabelClassName),
                streamGuidContainer.Q<Label>(null, titleLabelClassName)
            };

            layoutContainer.SetStyleBackgroundColor(initialContainerColor);
            signalIcon.SetStyleBackgroundImageTintColor(signalColor);
            streamIcon.SetStyleBackgroundImageTintColor(streamColor);

            foreach (Label infoLabel in infoLabels)
            {
                infoLabel
                    .ResetLayout()
                    .SetStyleColor(infoLabelColor)
                    .SetStyleUnityFont(infoLabelFont);
            }

            foreach (Label titleLabel in titleLabels)
            {
                titleLabel
                    .ResetLayout()
                    .SetStyleColor(titleLabelColor)
                    .SetStyleUnityFont(titleLabelFont);
            }

            FluidButton GetLocationButton(EditorSelectableColorInfo accentColor) =>
                FluidButton.Get(locationTextures)
                    .SetAccentColor(accentColor)
                    .SetElementSize(ElementSize.Tiny);

            buttonPingSourceGameObject = GetLocationButton(signalSelectableColor);
            buttonPingSignalProvider = GetLocationButton(signalSelectableColor);
            buttonPingSenderObject = GetLocationButton(signalSelectableColor);
            buttonPingStreamProvider = GetLocationButton(streamSelectableColor);

            const string pingButtonContainerClassName = "PingButtonContainer";
            sourceGameObjectContainer.Q<VisualElement>(null, pingButtonContainerClassName).AddChild(buttonPingSourceGameObject);
            signalProviderContainer.Q<VisualElement>(null, pingButtonContainerClassName).AddChild(buttonPingSignalProvider);
            signalSenderObjectContainer.Q<VisualElement>(null, pingButtonContainerClassName).AddChild(buttonPingSenderObject);
            streamSignalProviderContainer.Q<VisualElement>(null, pingButtonContainerClassName).AddChild(buttonPingStreamProvider);
        }

        public SignalsConsoleRow SetSignal(Signal targetSignal)
        {
            Reset();
            signal = targetSignal;

            if (signal == null) return this;

            //REACTIONS
            layoutContainer.RegisterCallback<MouseEnterEvent>(evt => layoutContainer.SetStyleBackgroundColor(layoutContainerHoverColor));
            layoutContainer.RegisterCallback<MouseLeaveEvent>(evt => layoutContainer.SetStyleBackgroundColor(layoutContainerNormalColor));

            const float reactionDuration = 0.6f;
            signalIconReaction = Reaction.Get<Texture2DReaction>()
                .SetEditorHeartbeat().SetDuration(reactionDuration)
                .SetSetter(value => signalIcon.SetStyleBackgroundImage(value))
                .SetTextures(signal.hasValue ? metaSignalTextures : signalTextures);

            streamIconReaction = Reaction.Get<Texture2DReaction>()
                .SetEditorHeartbeat().SetDuration(reactionDuration)
                .SetSetter(value => streamIcon.SetStyleBackgroundImage(value))
                .SetTextures(streamTextures);

            colorReaction = Reaction.Get<ColorReaction>()
                .SetEditorHeartbeat().SetDuration(reactionDuration)
                .SetPlayMode(PlayMode.Normal).SetEase(Ease.Linear)
                .SetSetter(value => layoutContainer.SetStyleBackgroundColor(value))
                .SetOnFinishCallback(() => layoutContainer.SetStyleBackgroundColor(layoutContainerNormalColor));

            colorReaction.SetFrom(initialContainerColor);
            colorReaction.SetTo(layoutContainerNormalColor);

            signalIconReaction.Play();
            streamIconReaction.Play();
            colorReaction.Play();

            UpdateRow();
            return this;
        }

        
        private void UpdateRow()
        {
            if (signal == null) return;
            timestampLabel.SetText($"{DateTime.Now.TimeOfDay}");

            sourceGameObjectContainer.SetStyleDisplay(signal.hasSourceGameObject ? DisplayStyle.Flex : DisplayStyle.None);
            sourceGameObjectLabel.SetText(signal.hasSourceGameObject ? signal.sourceGameObject.name : NONE);

            signalProviderContainer.SetStyleDisplay(signal.hasProvider ? DisplayStyle.Flex : DisplayStyle.None);
            signalProviderLabel.SetText(signal.hasProvider ? ObjectNames.NicifyVariableName(signal.signalProvider.GetType().Name) : NONE);

            signalSenderObjectContainer.SetStyleDisplay(signal.hasSenderObject ? DisplayStyle.Flex : DisplayStyle.None);
            signalSenderObjectLabel.SetText(signal.hasSenderObject ? ObjectNames.NicifyVariableName(signal.signalSenderObject.GetType().Name) : NONE);

            messageContainer.SetStyleDisplay(signal.message.IsNullOrEmpty() ? DisplayStyle.None : DisplayStyle.Flex);
            messageLabel.SetText(signal.message.IsNullOrEmpty() ? NONE : signal.message);

            bool signalHasValue = signal.hasValue & signal.valueAsObject != null;

            // signalValueTypeContainer.SetStyleDisplay(signalHasValue ? DisplayStyle.Flex : DisplayStyle.None);
            signalValueTypeLabel.SetText(signalHasValue ? ObjectNames.NicifyVariableName(signal.valueType.Name) : NONE);

            string cleanValue = NONE;
            if (signalHasValue)
            {
                string valueToString = signal.valueAsObject.ToString();
                valueToString = Regex.Replace(valueToString, @"<b>|</b>", "");
                cleanValue = Regex.Replace(valueToString, @"\t|\r|", "");
                cleanValue = Regex.Replace(cleanValue, @"\n", " ");
            }

            signalValueContainer.SetStyleDisplay(signalHasValue ? DisplayStyle.Flex : DisplayStyle.None);
            signalValueLabel.SetText(cleanValue);

            streamSignalProviderContainer.SetStyleDisplay(signal.stream.hasProvider ? DisplayStyle.Flex : DisplayStyle.None);
            streamSignalProviderLabel.SetText(signal.stream.hasProvider ? ObjectNames.NicifyVariableName(signal.stream.signalProvider.GetType().Name) : NONE);

            streamSignalProviderGameObjectContainer.SetStyleDisplay(signal.stream.hasProvider ? DisplayStyle.Flex : DisplayStyle.None);
            streamSignalProviderGameObjectLabel.SetText(signal.stream.hasProvider ? signal.stream.signalProvider.gameObject.name : NONE);

            streamNameContainer.SetStyleDisplay(signal.stream.name.IsNullOrEmpty() || signal.stream.name.Equals(SignalStream.k_DefaultName) ? DisplayStyle.None : DisplayStyle.Flex);
            streamNameLabel.SetText(signal.stream.name);

            streamCategoryContainer.SetStyleDisplay(signal.stream.category.IsNullOrEmpty() || signal.stream.category.Equals(SignalStream.k_DefaultCategory) ? DisplayStyle.None : DisplayStyle.Flex);
            streamCategoryLabel.SetText(signal.stream.category);

            streamReceiversCountLabel.SetText(signal.stream.receiversCount.ToString());

            streamInfoMessageContainer.SetStyleDisplay(signal.stream.infoMessage.IsNullOrEmpty() ? DisplayStyle.None : DisplayStyle.Flex);
            streamInfoMessageLabel.SetText(signal.stream.infoMessage.IsNullOrEmpty() ? NONE : signal.stream.infoMessage);

            streamGuidLabel.SetText(signal.stream.key.ToString());
            
            buttonPingSourceGameObject.SetOnClick(() => EditorGUIUtility.PingObject(signal.sourceGameObject));
            buttonPingSignalProvider.SetOnClick(() => EditorGUIUtility.PingObject(signal.signalProvider));
            buttonPingSenderObject.SetOnClick(() => EditorGUIUtility.PingObject(signal.signalSenderObject));
            buttonPingStreamProvider.SetOnClick(() => EditorGUIUtility.PingObject(signal.stream.signalProvider));
        }

        public override void Dispose()
        {
            base.Dispose();

            signalIconReaction?.Recycle();
            streamIconReaction?.Recycle();
            colorReaction?.Recycle();

            buttonPingSignalProvider?.Recycle();
            buttonPingSenderObject?.Recycle();
            buttonPingSourceGameObject?.Recycle();
            buttonPingStreamProvider?.Recycle();
        }

        public override void Reset()
        {
            signalIconReaction?.Finish();
            streamIconReaction?.Finish();
            colorReaction?.Finish();

            signal = null;
        }
    }
}
