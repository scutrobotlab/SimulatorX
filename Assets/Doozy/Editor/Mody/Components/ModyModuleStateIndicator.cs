// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Editor.EditorUI;
using Doozy.Editor.Reactor.Internal;
using Doozy.Runtime.Mody;
using Doozy.Runtime.Reactor.Extensions;
using Doozy.Runtime.Reactor.Internal;
using Doozy.Runtime.Reactor.Reactions;
using Doozy.Runtime.UIElements.Extensions;
using UnityEngine;
using UnityEngine.UIElements;
using static Doozy.Runtime.Mody.ModuleState;
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Editor.Mody.Components
{
    public class ModyModuleStateIndicator : VisualElement, IDisposable
    {
        public void Dispose()
        {
            iconReaction?.Recycle();
        }

        public TemplateContainer templateContainer { get; }
        public VisualElement layoutContainer { get; }
        public Label indicatorText { get; }
        public VisualElement barContainer { get; }
        public VisualElement indicatorBar { get; }

        public Texture2DReaction iconReaction { get; }

        public ModyModuleStateIndicator()
        {
            Add(templateContainer = EditorLayouts.Mody.ModyStateIndicator.CloneTree());

            templateContainer
                .AddStyle(EditorStyles.Mody.ModyStateIndicator);

            layoutContainer = templateContainer.Q<VisualElement>(nameof(layoutContainer));
            indicatorText = layoutContainer.Q<Label>(nameof(indicatorText));
            barContainer = layoutContainer.Q<VisualElement>(nameof(barContainer));
            indicatorBar = layoutContainer.Q<VisualElement>(nameof(indicatorBar));

            iconReaction =
                indicatorBar
                    .GetTexture2DReaction(EditorMicroAnimations.Mody.Effects.Running)
                    .SetEditorHeartbeat().SetLoops(-1);

            UpdateState(Disabled);
        }

        public void UpdateState(ModuleState state)
        {
            Color stateColor;
            DisplayStyle barDisplayStyle = DisplayStyle.None;

            switch (state)
            {
                case Disabled:
                    stateColor = EditorColors.Mody.StateDisabled;
                    iconReaction?.Stop();
                    break;
                case Idle:
                    stateColor = EditorColors.Mody.StateIdle;
                    iconReaction?.Stop();
                    break;
                case Active:
                    stateColor = EditorColors.Mody.StateActive;
                    barDisplayStyle = DisplayStyle.Flex;
                    iconReaction?.Play();
                    break;
                default: throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }

            indicatorText
                .SetText(state.ToString())
                .SetStyleColor(stateColor);

            indicatorBar
                .SetStyleBackgroundImageTintColor(stateColor)
                .SetStyleDisplay(barDisplayStyle);
        }

    }
}
