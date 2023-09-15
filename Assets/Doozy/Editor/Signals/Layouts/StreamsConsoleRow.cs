// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
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
using UnityEngine.Events;
using UnityEngine.UIElements;
using PlayMode = Doozy.Runtime.Reactor.PlayMode;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Local

namespace Doozy.Editor.Signals.Layouts
{
    public class StreamsConsoleRow : PoolableElement<StreamsConsoleRow>
    {
        private const string NONE = "---";

        public static StreamsConsoleRow Get(SignalStream targetStream) =>
            Get().SetStream(targetStream);

        public bool isHiddenByFilter { get; private set; }

        public SignalStream stream { get; private set; }

        private TemplateContainer templateContainer { get; }

        public VisualElement layoutContainer { get; }
        public Image signalIcon { get; }
        public Image streamIcon { get; }

        public VisualElement detailsContainer { get; }
        public VisualElement detailsTopRow { get; }
        public VisualElement streamSignalProviderContainer { get; }
        public VisualElement streamSignalProviderGameObjectContainer { get; }
        public VisualElement streamCategoryContainer { get; }
        public VisualElement streamNameContainer { get; }
        public VisualElement detailsBottomRow { get; }
        public VisualElement streamReceiversCountContainer { get; }
        public VisualElement streamInfoMessageContainer { get; }
        public VisualElement streamGuidContainer { get; }

        public Label signalsCounterLabel { get; }
        public Label streamSignalProviderLabel { get; }
        public Label streamSignalProviderGameObjectLabel { get; }
        public Label streamCategoryLabel { get; }
        public Label streamNameLabel { get; }
        public Label streamReceiversCountLabel { get; }
        public Label streamInfoMessageLabel { get; }
        public Label streamGuidLabel { get; }

        public FluidButton buttonPingStreamProvider { get; }

        private Texture2DReaction signalIconSignalReaction { get; set; }
        private Texture2DReaction signalIconMetaSignalReaction { get; set; }
        private Texture2DReaction streamIconReaction { get; set; }
        private ColorReaction colorReaction { get; set; }

        private static IEnumerable<Texture2D> signalTextures => EditorMicroAnimations.Signals.Icons.SignalOnOff;
        private static IEnumerable<Texture2D> metaSignalTextures => EditorMicroAnimations.Signals.Icons.MetaSignalOnOff;
        private static IEnumerable<Texture2D> streamTextures => EditorMicroAnimations.Signals.Icons.SignalStream;
        private static IEnumerable<Texture2D> locationTextures => EditorMicroAnimations.EditorUI.Icons.Location;

        public bool pinned { get; set; }
        public UnityAction<StreamsConsoleRow> OnPinned;

        private static Color layoutContainerNormalColor => EditorColors.Default.FieldBackground;
        private static Color layoutContainerHoverColor => EditorColors.Default.WindowHeaderBackground;

        private static Color signalColor => EditorColors.Signals.Signal;
        private static EditorSelectableColorInfo signalSelectableColor => EditorSelectableColors.Signals.Signal;

        private static Color streamColor => EditorColors.Signals.Stream;
        private static EditorSelectableColorInfo streamSelectableColor => EditorSelectableColors.Signals.Stream;

        private static Color infoLabelColor => EditorColors.Default.TextTitle;
        private static Font infoLabelFont => EditorFonts.Ubuntu.Light;

        private static Color titleLabelColor => EditorColors.Default.TextDescription;
        private static Font titleLabelFont => EditorFonts.Ubuntu.Light;


        public StreamsConsoleRow()
        {
            this.SetStyleFlexGrow(1);

            Add(templateContainer = EditorLayouts.Signals.StreamsConsoleRow.CloneTree());

            templateContainer
                .SetStyleFlexGrow(1)
                .AddStyle(EditorUI.EditorStyles.Signals.StreamsConsoleRow);

            layoutContainer = templateContainer.Q<VisualElement>(nameof(layoutContainer));

            //ICONS
            signalIcon = layoutContainer.Q<Image>(nameof(signalIcon));
            streamIcon = layoutContainer.Q<Image>(nameof(streamIcon));

            //CONTAINERS
            detailsContainer = layoutContainer.Q<VisualElement>(nameof(detailsContainer));
            detailsTopRow = layoutContainer.Q<VisualElement>(nameof(detailsTopRow));
            streamSignalProviderContainer = layoutContainer.Q<VisualElement>(nameof(streamSignalProviderContainer));
            streamSignalProviderGameObjectContainer = layoutContainer.Q<VisualElement>(nameof(streamSignalProviderGameObjectContainer));
            streamCategoryContainer = layoutContainer.Q<VisualElement>(nameof(streamCategoryContainer));
            streamNameContainer = layoutContainer.Q<VisualElement>(nameof(streamNameContainer));
            streamReceiversCountContainer = layoutContainer.Q<VisualElement>(nameof(streamReceiversCountContainer));
            detailsBottomRow = layoutContainer.Q<VisualElement>(nameof(detailsBottomRow));
            streamInfoMessageContainer = layoutContainer.Q<VisualElement>(nameof(streamInfoMessageContainer));
            streamGuidContainer = layoutContainer.Q<VisualElement>(nameof(streamGuidContainer));

            //INFO LABELS
            var infoLabels = new List<Label>
            {
                (signalsCounterLabel = layoutContainer.Q<Label>(nameof(signalsCounterLabel))),
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
                streamSignalProviderContainer.Q<Label>(null, titleLabelClassName),
                streamSignalProviderGameObjectContainer.Q<Label>(null, titleLabelClassName),
                streamNameContainer.Q<Label>(null, titleLabelClassName),
                streamCategoryContainer.Q<Label>(null, titleLabelClassName),
                streamReceiversCountContainer.Q<Label>(null, titleLabelClassName),
                streamInfoMessageContainer.Q<Label>(null, titleLabelClassName),
                streamGuidContainer.Q<Label>(null, titleLabelClassName)
            };

            layoutContainer.SetStyleBackgroundColor(layoutContainerNormalColor);
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

            buttonPingStreamProvider = GetLocationButton(streamSelectableColor);

            const string pingButtonContainerClassName = "PingButtonContainer";
            streamSignalProviderContainer.Q<VisualElement>(null, pingButtonContainerClassName).AddChild(buttonPingStreamProvider);
        }

        private StreamsConsoleRow SetStream(SignalStream targetStream)
        {
            Reset();
            stream = targetStream;

            if (stream == null) return this;

            //REACTIONS
            layoutContainer.RegisterCallback<MouseEnterEvent>(evt => layoutContainer.SetStyleBackgroundColor(layoutContainerHoverColor));
            layoutContainer.RegisterCallback<MouseLeaveEvent>(evt => layoutContainer.SetStyleBackgroundColor(layoutContainerNormalColor));
            layoutContainer.AddManipulator(new Clickable(() =>
            {
                pinned = !pinned;
                OnPinned?.Invoke(this);
            }));

            const float reactionDuration = 0.6f;
            signalIconSignalReaction = Reaction.Get<Texture2DReaction>()
                .SetEditorHeartbeat().SetDuration(reactionDuration)
                .SetSetter(value =>
                {
                    signalIconMetaSignalReaction?.Stop();
                    signalIcon.SetStyleBackgroundImage(value);
                })
                .SetTextures(signalTextures);

            signalIconMetaSignalReaction = Reaction.Get<Texture2DReaction>()
                .SetEditorHeartbeat().SetDuration(reactionDuration)
                .SetSetter(value =>
                {
                    signalIconSignalReaction?.Stop();
                    signalIcon.SetStyleBackgroundImage(value);
                })
                .SetTextures(metaSignalTextures);

            streamIconReaction = Reaction.Get<Texture2DReaction>()
                .SetEditorHeartbeat().SetDuration(reactionDuration)
                .SetSetter(value => streamIcon.SetStyleBackgroundImage(value))
                .SetTextures(streamTextures)
                .SetLastFrame();

            colorReaction = Reaction.Get<ColorReaction>()
                .SetEditorHeartbeat().SetDuration(reactionDuration)
                .SetPlayMode(PlayMode.Normal).SetEase(Ease.InExpo)
                .SetSetter(c => layoutContainer.SetStyleBackgroundColor(c))
                .SetOnFinishCallback(() => layoutContainer.SetStyleBackgroundColor(layoutContainerNormalColor));

            colorReaction.SetFrom
            (
                EditorGUIUtility.isProSkin
                    ? layoutContainerNormalColor * streamColor
                    : layoutContainerNormalColor + streamColor * 0.3f
            );
            colorReaction.SetTo(layoutContainerNormalColor);

            stream.OnSignal += OnSignal;
            stream.OnReceiverConnected += OnReceiversUpdate;
            stream.OnReceiverDisconnected += OnReceiversUpdate;

            UpdateRow();
            return this;
        }

        private void OnSignal(Signal signal)
        {
            UpdateRow();
            streamIconReaction?.Play();
            colorReaction?.Play();

            if (signal.hasValue)
            {
                signalIconMetaSignalReaction?.Play();
                return;
            }

            signalIconSignalReaction?.Play();
        }

        private void OnReceiversUpdate(ISignalReceiver receiver) =>
            UpdateRow();

        private void UpdateRow()
        {
            if (stream == null) return;

            signalsCounterLabel.SetText(stream.signalsCounter.ToString());

            streamSignalProviderContainer.SetStyleDisplay(stream.hasProvider ? DisplayStyle.Flex : DisplayStyle.None);
            streamSignalProviderLabel.SetText(stream.hasProvider ? ObjectNames.NicifyVariableName(stream.signalProvider.GetType().Name) : NONE);

            streamSignalProviderGameObjectContainer.SetStyleDisplay(stream.hasProvider ? DisplayStyle.Flex : DisplayStyle.None);
            streamSignalProviderGameObjectLabel.SetText(stream.hasProvider ? stream.signalProvider.gameObject.name : NONE);

            streamNameContainer.SetStyleDisplay(stream.name.IsNullOrEmpty() || stream.name.Equals(SignalStream.k_DefaultName) ? DisplayStyle.None : DisplayStyle.Flex);
            streamNameLabel.SetText(stream.name);

            streamCategoryContainer.SetStyleDisplay(stream.category.IsNullOrEmpty() || stream.category.Equals(SignalStream.k_DefaultCategory) ? DisplayStyle.None : DisplayStyle.Flex);
            streamCategoryLabel.SetText(stream.category);

            streamReceiversCountLabel.SetText(stream.receivers.Count.ToString());

            streamInfoMessageContainer.SetStyleDisplay(stream.infoMessage.IsNullOrEmpty() ? DisplayStyle.None : DisplayStyle.Flex);
            streamInfoMessageLabel.SetText(stream.infoMessage.IsNullOrEmpty() ? NONE : stream.infoMessage);

            streamGuidLabel.SetText(stream.key.ToString());

            buttonPingStreamProvider.SetOnClick(() => EditorGUIUtility.PingObject(stream.signalProvider));
        }

        public override void Dispose()
        {
            base.Dispose();

            signalIconSignalReaction?.Recycle();
            signalIconMetaSignalReaction?.Recycle();
            streamIconReaction?.Recycle();
            colorReaction?.Recycle();
        }

        public override void Reset()
        {
            signalIconSignalReaction?.Finish();
            signalIconMetaSignalReaction?.Finish();
            streamIconReaction?.Finish();
            colorReaction?.Finish();

            OnPinned = null;

            if (stream == null) return;

            stream.OnSignal -= OnSignal;
            stream.OnReceiverConnected -= OnReceiversUpdate;
            stream.OnReceiverDisconnected -= OnReceiversUpdate;

            stream = null;
        }

        public void ApplyFilter(Filter filter, string input)
        {
            if (!filter.isActive) return;
            ApplyFilter(filter.IsMatch(input));
        }

        public void ApplyFilter(bool showRow)
        {
            isHiddenByFilter = !showRow;
            this.SetStyleDisplay(pinned || showRow ? DisplayStyle.Flex : DisplayStyle.None);
        }

        public void ClearFilter()
        {
            ApplyFilter(true);
        }

    }
}
