// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Runtime.Common.Extensions;
using Doozy.Runtime.Mody;
using Doozy.Runtime.Reactor.Internal;
using Doozy.Runtime.Signals;
using Doozy.Runtime.UIElements.Extensions;
using UnityEngine;
using UnityEngine.UIElements;
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Editor.Mody.Components
{
    public class ModyProviderStateIndicator : VisualElement, IDisposable
    {
        public void Dispose()
        {
            runningProgressBar?.Recycle();
            cooldownProgressBar?.Recycle();

            runningIndicator?.Recycle();
            cooldownIndicator?.Recycle();
        }

        public static Color idleColor => EditorColors.Mody.StateIdle;
        public static Color runningColor => EditorColors.Mody.StateActive;
        public static Color cooldownColor => EditorColors.Mody.StateCooldown;

        public FluidProgressBar runningProgressBar { get; }
        public FluidProgressBar cooldownProgressBar { get; }

        public EnabledIndicator runningIndicator { get; }
        public EnabledIndicator cooldownIndicator { get; }

        public Label stateNameLabel { get; }

        private VisualElement running { get; }
        private VisualElement cooldown { get; }

        public ModyProviderStateIndicator()
        {
            runningIndicator = GetIndicator(EditorMicroAnimations.EditorUI.Icons.Duration).SetEnabledColor(runningColor).IconIsLooping(true);
            cooldownIndicator = GetIndicator(EditorMicroAnimations.EditorUI.Icons.Cooldown).SetEnabledColor(cooldownColor).IconIsLooping(true);

            runningProgressBar = FluidProgressBar.Get().SetIndicatorColor(runningColor);
            cooldownProgressBar = FluidProgressBar.Get().SetIndicatorColor(cooldownColor);

            stateNameLabel = new Label()
                .ResetLayout()
                .SetStyleUnityFont(DesignUtils.fieldNameTextFont)
                .SetStyleFontSize(12)
                .SetStyleAlignSelf(Align.Center)
                .SetStyleDisplay(DisplayStyle.None);

            this.SetStyleFlexDirection(FlexDirection.Row);
            this.SetStyleAlignItems(Align.Center);
            this.SetStyleAlignSelf(Align.Center);

            running =
                DesignUtils.column.SetStyleFlexGrow(0).SetStyleMarginRight(2)
                    .AddChild(runningIndicator)
                    .AddSpace(2, 2)
                    .AddChild(runningProgressBar);
            cooldown =
                DesignUtils.column.SetStyleFlexGrow(0).AddChild(cooldownIndicator)
                    .AddSpace(2, 2)
                    .AddChild(cooldownProgressBar);

            this
                .AddChild(stateNameLabel.SetStyleMarginRight(DesignUtils.k_Spacing * 2))
                .AddChild(running)
                .AddChild(cooldown);
        }

        public void UpdateState(SignalProvider provider, ProviderState state)
        {
            UpdateStateNameLabel(state);

            bool isDisabled = state == ProviderState.Disabled;
            if (isDisabled)
            {
                running.SetStyleDisplay(DisplayStyle.None);
                cooldown.SetStyleDisplay(DisplayStyle.None);

                return;
            }

            bool hasCooldown = provider.cooldown > 0;

            running.SetStyleDisplay(DisplayStyle.Flex);
            cooldown.SetStyleDisplay(hasCooldown ? DisplayStyle.Flex : DisplayStyle.None);

            switch (state)
            {
                case ProviderState.Disabled:
                case ProviderState.Idle:

                    cooldownProgressBar.SetAtZero();
                    runningProgressBar.Stop().SetDuration(0.1f).Play(true);

                    if (runningIndicator.isOn) runningIndicator.SetDisabled();
                    if (cooldownIndicator.isOn) cooldownIndicator.SetDisabled();

                    break;

                case ProviderState.IsRunning:
                    runningProgressBar.SetDuration(0.1f);
                    runningProgressBar.reaction.SetOnUpdateCallback(() => stateNameLabel.SetText(ReactionDurationLabel("Running", runningProgressBar.reaction)));
                    runningProgressBar.reaction.SetOnStopCallback(() => runningProgressBar.reaction.ClearOnUpdateCallback());
                    runningProgressBar.Play();
                    runningIndicator.SetEnabled(false);
                    break;
                case ProviderState.InCooldown:
                    if (!hasCooldown) return;
                    if (runningIndicator.isOn) runningIndicator.SetDisabled();
                    cooldownProgressBar.SetDuration(provider.cooldown);
                    cooldownProgressBar.reaction.SetOnUpdateCallback(() => stateNameLabel.SetText(ReactionDurationLabel("Cooldown", cooldownProgressBar.reaction)));
                    cooldownProgressBar.reaction.SetOnStopCallback(() => cooldownProgressBar.reaction.ClearOnUpdateCallback());
                    cooldownProgressBar.Play();
                    cooldownIndicator.SetEnabled(false);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }

        private void UpdateStateNameLabel(ProviderState state)
        {
            stateNameLabel.SetStyleDisplay(state != ProviderState.Idle ? DisplayStyle.Flex : DisplayStyle.None);
            switch (state)
            {
                case ProviderState.Disabled:
                    stateNameLabel.SetText("Disabled");
                    stateNameLabel.SetStyleColor(DesignUtils.disabledTextColor);
                    break;
                case ProviderState.Idle:
                    stateNameLabel.SetText("Idle");
                    stateNameLabel.SetStyleColor(idleColor);
                    break;
                case ProviderState.IsRunning:
                    stateNameLabel.SetText("Running");
                    stateNameLabel.SetStyleColor(runningColor);
                    break;
                case ProviderState.InCooldown:
                    stateNameLabel.SetText("Cooldown");
                    stateNameLabel.SetStyleColor(cooldownColor);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static string ReactionDurationLabel(string prefix, Reaction reaction)
        {
            float n = Mathf.Max(1, Mathf.Abs((int)reaction.duration));
            string format = string.Empty;
            for (int i = 0; i < Mathf.Floor(Mathf.Log10(n) + 1); i++)
                format += "0";
            format += ".00";
            return $"{prefix} ({reaction.elapsedDuration.Round(2).ToString(format)}/{reaction.duration.ToString(format)})";
        }

        private static EnabledIndicator GetIndicator(IEnumerable<Texture2D> textures) =>
            EnabledIndicator.Get().SetIcon(textures).SetStyleSize(16);
    }
}
