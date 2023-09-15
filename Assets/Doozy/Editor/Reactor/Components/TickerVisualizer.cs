// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using Doozy.Editor.EditorUI;
using Doozy.Editor.Reactor.Internal;
using Doozy.Editor.Reactor.Ticker;
using Doozy.Runtime.Reactor.Easings;
using Doozy.Runtime.Reactor.Internal;
using Doozy.Runtime.Reactor.Reactions;
using Doozy.Runtime.Reactor.Ticker;
using Doozy.Runtime.UIElements.Extensions;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Editor.Reactor.Components
{
    public class TickerVisualizer : VisualElement, IDisposable
    {
        public enum TickerType
        {
            Editor,
            Runtime
        }

        private TickerType m_TickerType = TickerType.Runtime;

        private TemplateContainer templateContainer { get; }
        public VisualElement layoutContainer { get; }
        public VisualElement reactorIconContainer { get; }
        public Image reactorIconImage { get; }
        public VisualElement heartIconContainer { get; }
        public Image heartIconImage { get; }
        public VisualElement labelsContainer { get; }
        public Label tickerNameLabel { get; }
        public Label stateLabel { get; }

        public Texture2DReaction reactorIconReaction { get; }
        public Texture2DReaction heartIconReaction { get; }
        private List<Texture2D> heartIconTextures { get; set; }

        public static Color backgroundNormalColor => EditorColors.Default.WindowHeaderBackground;
        public static Color backgroundHoverColor =>
            EditorGUIUtility.isProSkin
                ? backgroundNormalColor * reactorColor
                : backgroundNormalColor + reactorColor * 0.3f;

        public static Color reactorColor => EditorColors.Reactor.Red;
        public static Color disabledIconColor => EditorColors.Default.Placeholder;
        public static Color tickerNameTextColor => EditorColors.Default.TextTitle;
        public static Color stateTextColor => EditorColors.Default.TextSubtitle;

        public static Font tickerNameFont => EditorFonts.Ubuntu.Regular;
        public static Font stateFont => EditorFonts.Ubuntu.Medium;

        public UnityAction OnClick;

        private TickService service { get; set; }
        private bool isConnected => service != null;
        private bool isActive => m_TickerType == TickerType.Editor || EditorApplication.isPlaying;

        private IVisualElementScheduledItem m_Refresher;

        public TickerVisualizer(TickerType tickerType, UnityAction onClick)
        {
            this.SetStyleFlexGrow(1);

            Add(templateContainer = EditorLayouts.Reactor.TickerVisualizer.CloneTree());

            templateContainer
                .SetStyleFlexGrow(1)
                .AddStyle(EditorUI.EditorStyles.Reactor.TickerVisualizer);

            layoutContainer = templateContainer.Q<VisualElement>("LayoutContainer");

            reactorIconContainer = layoutContainer.Q<VisualElement>("ReactorIconContainer");
            reactorIconImage = reactorIconContainer.Q<Image>("ReactorIcon");

            heartIconContainer = layoutContainer.Q<VisualElement>("HeartIconContainer");
            heartIconImage = heartIconContainer.Q<Image>("HeartIcon");

            labelsContainer = layoutContainer.Q<VisualElement>("LabelsContainer");
            tickerNameLabel = labelsContainer.Q<Label>("TickerName");
            stateLabel = labelsContainer.Q<Label>("State");

            //COLORS
            layoutContainer.SetStyleBackgroundColor(backgroundNormalColor);

            reactorIconImage.SetStyleBackgroundImageTintColor(reactorColor);
            heartIconImage.SetStyleBackgroundImageTintColor(reactorColor);


            tickerNameLabel.SetStyleColor(tickerNameTextColor);
            stateLabel.SetStyleColor(stateTextColor);

            //FONTS
            tickerNameLabel.SetStyleUnityFont(tickerNameFont);
            stateLabel.SetStyleUnityFont(stateFont);

            reactorIconReaction = Reaction.Get<Texture2DReaction>()
                .SetEditorHeartbeat().SetDuration(0.5f).SetEase(Ease.Linear)
                .SetTextures(EditorMicroAnimations.Reactor.Icons.Reactor)
                .SetSetter(t => reactorIconImage.SetStyleBackgroundImage(t));
            reactorIconReaction.SetFirstFrame();


            heartIconReaction = Reaction.Get<Texture2DReaction>()
                .SetEditorHeartbeat().SetDuration(0.5f).SetEase(Ease.Linear)
                .SetSetter(t => heartIconImage.SetStyleBackgroundImage(t));

            SetTickerType(tickerType);
            UpdateState();

            if (onClick != null)
            {
                layoutContainer.RegisterCallback<PointerEnterEvent>(evt => layoutContainer.SetStyleBackgroundColor(backgroundHoverColor));
                layoutContainer.RegisterCallback<PointerLeaveEvent>(evt => layoutContainer.SetStyleBackgroundColor(backgroundNormalColor));

                layoutContainer.AddManipulator(new Clickable(() =>
                {
                    reactorIconReaction?.Play();
                    onClick();
                }));
            }

            EditorApplication.playModeStateChanged += state =>
            {
                if (m_TickerType != TickerType.Runtime) return;

                switch (state)
                {

                    case PlayModeStateChange.EnteredPlayMode:
                        ConnectTickService(RuntimeTicker.service);
                        break;
                    default:
                        Disconnect();
                        break;
                }
            };

            if (tickerType == TickerType.Editor)
            {
                if (isConnected) return;
                ConnectTickService(EditorTicker.service);
                return;
            }
            if (tickerType == TickerType.Runtime && EditorApplication.isPlaying)
            {
                if (isConnected) return;
                ConnectTickService(RuntimeTicker.service);
            }
        }

        public void OnTick()
        {
            if (!isConnected) return;
            UpdateState();

            if (heartIconReaction.isActive) return;

            switch (m_TickerType)
            {

                case TickerType.Editor:
                    if (service.registeredTargetsCount > 1)
                        heartIconReaction.Play();
                    break;
                case TickerType.Runtime:
                    heartIconReaction.Play();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private Color currentIconColor => isConnected && isActive ? reactorColor : disabledIconColor;

        private void UpdateState()
        {
            reactorIconImage.SetStyleBackgroundImageTintColor(currentIconColor);
            heartIconImage.SetStyleBackgroundImageTintColor(currentIconColor);

            stateLabel.SetText
            (
                isConnected & isActive
                    ? service.hasRegisteredTargets
                        ? $"{service.registeredTargetsCount} reactions @ {service.fps}FPS"
                        : "Idle"
                    : "Disabled"
            );
        }

        private void ConnectTickService(TickService tickService)
        {
            if (isConnected) return;
            if (tickService == null) return;
            service = tickService;
            service.OnTick += OnTick;
            OnTick();
            m_Refresher = schedule.Execute(OnTick).Every(10000);
        }

        private void Disconnect()
        {
            if (!isConnected) return;
            m_Refresher.Pause();
            m_Refresher = null;
            service.OnTick -= OnTick;
            service = null;
            UpdateState();
        }

        public void Dispose()
        {
            RemoveFromHierarchy();
            if (isConnected) service.OnTick -= OnTick;
            reactorIconReaction?.Recycle();
            heartIconReaction?.Recycle();
        }

        public void SetTickerType(TickerType type)
        {
            m_TickerType = type;
            switch (type)
            {
                case TickerType.Editor:
                    heartIconTextures = EditorMicroAnimations.Reactor.Icons.EditorHeartbeat;
                    break;
                case TickerType.Runtime:
                    heartIconTextures = EditorMicroAnimations.Reactor.Icons.Heartbeat;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }

            heartIconReaction?.SetTextures(heartIconTextures);
            tickerNameLabel.SetText($"{type} Heartbeat");
        }
    }
}
