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
using Doozy.Runtime.UIElements.Extensions;
using UnityEngine;
using UnityEngine.UIElements;
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Editor.Mody.Components
{
    public class ModyActionStateIndicator : VisualElement, IDisposable
    {
        public void Dispose()
        {
            startDelayProgressBar?.Recycle();
            runningProgressBar?.Recycle();
            cooldownProgressBar?.Recycle();

            onStartIndicator?.Recycle();
            onFinishIndicator?.Recycle();

            startDelayIndicator?.Recycle();
            runningIndicator?.Recycle();
            cooldownIndicator?.Recycle();
        }

        public static Color idleColor => EditorColors.Mody.StateIdle;
        public static Color startDelayColor => EditorColors.Mody.StateIdle;
        public static Color runningColor => EditorColors.Mody.StateActive;
        public static Color cooldownColor => EditorColors.Mody.StateCooldown;

        public FluidProgressBar startDelayProgressBar { get; }
        public FluidProgressBar runningProgressBar { get; }
        public FluidProgressBar cooldownProgressBar { get; }

        public EnabledIndicator startDelayIndicator { get; }
        public EnabledIndicator onStartIndicator { get; }
        public EnabledIndicator runningIndicator { get; }
        public EnabledIndicator onFinishIndicator { get; }
        public EnabledIndicator cooldownIndicator { get; }

        public Label stateNameLabel { get; }

        private VisualElement startDelay { get; }
        private VisualElement running { get; }
        private VisualElement cooldown { get; }

        public ModyActionStateIndicator()
        {
            onStartIndicator = GetIndicator(EditorMicroAnimations.EditorUI.Icons.EventsOnStart).SetEnabledColor(runningColor).SetSize(22);
            onFinishIndicator = GetIndicator(EditorMicroAnimations.EditorUI.Icons.EventsOnFinish).SetEnabledColor(runningColor).SetSize(22);

            startDelayIndicator = GetIndicator(EditorMicroAnimations.EditorUI.Icons.StartDelay).SetEnabledColor(startDelayColor).IconIsLooping(true);
            runningIndicator = GetIndicator(EditorMicroAnimations.EditorUI.Icons.Duration).SetEnabledColor(runningColor).IconIsLooping(true);
            cooldownIndicator = GetIndicator(EditorMicroAnimations.EditorUI.Icons.Cooldown).SetEnabledColor(cooldownColor).IconIsLooping(true);

            startDelayProgressBar = FluidProgressBar.Get().SetIndicatorColor(startDelayColor);
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

            startDelay =
                DesignUtils.column.SetStyleFlexGrow(0).SetStyleMarginRight(2)
                    .AddChild(startDelayIndicator)
                    .AddSpace(2, 2)
                    .AddChild(startDelayProgressBar);
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
                .AddChild(startDelay)
                .AddChild(onStartIndicator.SetStyleMarginRight(2))
                .AddChild(running)
                .AddChild(onFinishIndicator.SetStyleMarginLeft(2))
                .AddChild(cooldown);
        }

        public void UpdateTriggeredState(ModyAction action, ModyAction.TriggeredActionState triggeredState)
        {
            // Debug.Log($"{nameof(UpdateTriggeredState)}: {triggeredState}");

            UpdateStateNameLabel(action.currentState);
            
            bool isDisabled = !action.enabled;
            if (isDisabled)
            {
                startDelay.SetStyleDisplay(DisplayStyle.None);
                onStartIndicator.SetStyleDisplay(DisplayStyle.None);
                running.SetStyleDisplay(DisplayStyle.None);
                onFinishIndicator.SetStyleDisplay(DisplayStyle.None);
                cooldown.SetStyleDisplay(DisplayStyle.None);

                return;
            }

            bool hasStartDelay = action.startDelay > 0;
            bool hasOnStart = action.onStartEvents != null && action.onStartEvents.Enabled;
            bool hasOnFinish = action.onFinishEvents != null && action.onFinishEvents.Enabled;
            bool hasCooldown = action.cooldown > 0;
            bool instantAction = action.duration == 0;

            startDelay.SetStyleDisplay(hasStartDelay ? DisplayStyle.Flex : DisplayStyle.None);
            onStartIndicator.SetStyleDisplay(hasOnStart ? DisplayStyle.Flex : DisplayStyle.None);
            running.SetStyleDisplay(DisplayStyle.Flex);
            onFinishIndicator.SetStyleDisplay(hasOnFinish ? DisplayStyle.Flex : DisplayStyle.None);
            cooldown.SetStyleDisplay(hasCooldown ? DisplayStyle.Flex : DisplayStyle.None);

            switch (triggeredState)
            {
                case ModyAction.TriggeredActionState.Disabled:
                case ModyAction.TriggeredActionState.Idle:

                    startDelayProgressBar.SetAtZero();
                    cooldownProgressBar.SetAtZero();

                    if (instantAction)
                    {
                        runningProgressBar.Stop().SetDuration(0.1f).Play(true);
                    }
                    else
                    {
                        runningProgressBar.SetAtZero();
                    }

                    if (startDelayIndicator.isOn) startDelayIndicator.SetDisabled();
                    if (onStartIndicator.isOn) onStartIndicator.SetDisabled();
                    if (runningIndicator.isOn) runningIndicator.SetDisabled();
                    if (onFinishIndicator.isOn) onFinishIndicator.SetDisabled();
                    if (cooldownIndicator.isOn) cooldownIndicator.SetDisabled();
                    
                    UpdateStateNameLabel(action.currentState);
                    break;
                case ModyAction.TriggeredActionState.StartDelay:
                    if (!hasStartDelay) break;
                    if (onStartIndicator.isOn) onStartIndicator.SetDisabled(false);
                    if (runningIndicator.isOn) runningIndicator.SetDisabled(false);
                    if (onFinishIndicator.isOn) onFinishIndicator.SetDisabled(false);
                    if (cooldownIndicator.isOn) cooldownIndicator.SetDisabled(false);
                    runningProgressBar.SetAtZero();
                    cooldownProgressBar.SetAtZero();
                    
                    startDelayProgressBar.SetDuration(action.startDelay);
                    startDelayProgressBar.reaction.SetOnUpdateCallback(() => stateNameLabel.SetText(ReactionDurationLabel("Start Delay", startDelayProgressBar.reaction)));
                    startDelayProgressBar.reaction.SetOnStopCallback(() =>
                    {
                        startDelayProgressBar.reaction.ClearOnUpdateCallback();
                        UpdateStateNameLabel(action.currentState);
                    });
                    startDelayProgressBar.Play();
                    startDelayIndicator.SetEnabled();
                    break;
                case ModyAction.TriggeredActionState.OnStart:
                    if (!hasOnStart) return;
                    onStartIndicator.SetEnabled(false);
                    break;
                case ModyAction.TriggeredActionState.Run:
                    if (startDelayIndicator.isOn) startDelayIndicator.SetDisabled();
                    runningProgressBar.SetDuration(action.duration);
                    runningProgressBar.reaction.SetOnUpdateCallback(() => stateNameLabel.SetText(ReactionDurationLabel("Running", runningProgressBar.reaction)));
                    runningProgressBar.reaction.SetOnStopCallback(() =>
                    {
                        runningProgressBar.reaction.ClearOnUpdateCallback();
                        UpdateStateNameLabel(action.currentState);
                    });
                    runningProgressBar.Play();
                    runningIndicator.SetEnabled(false);
                    break;
                case ModyAction.TriggeredActionState.OnFinish:
                    if (!hasOnFinish) return;
                    onFinishIndicator.SetEnabled(false);
                    break;
                case ModyAction.TriggeredActionState.Cooldown:
                    if (!hasCooldown) return;
                    if (onStartIndicator.isOn) onStartIndicator.SetDisabled();
                    if (runningIndicator.isOn) runningIndicator.SetDisabled();
                    if (onFinishIndicator.isOn) onFinishIndicator.SetDisabled();
                    cooldownProgressBar.SetDuration(action.cooldown);
                    cooldownProgressBar.reaction.SetOnUpdateCallback(() => stateNameLabel.SetText(ReactionDurationLabel("Cooldown", cooldownProgressBar.reaction)));
                    cooldownProgressBar.reaction.SetOnStopCallback(() =>
                    {
                        cooldownProgressBar.reaction.ClearOnUpdateCallback();
                        UpdateStateNameLabel(action.currentState);
                    });
                    cooldownProgressBar.Play();
                    cooldownIndicator.SetEnabled(false);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(triggeredState), triggeredState, null);
            }
        }
        
        private void UpdateStateNameLabel(ActionState state)
        {
            stateNameLabel.SetStyleDisplay(state != ActionState.Idle ? DisplayStyle.Flex : DisplayStyle.None);
            switch (state)
            {
                case ActionState.Disabled:
                    stateNameLabel.SetText("Disabled");
                    stateNameLabel.SetStyleColor(DesignUtils.disabledTextColor);
                    break;
                case ActionState.Idle:
                    stateNameLabel.SetText("Idle");
                    stateNameLabel.SetStyleColor(idleColor);
                    break;
                case ActionState.InStartDelay:
                    stateNameLabel.SetText("Start Delay");
                    stateNameLabel.SetStyleColor(startDelayColor);
                    break;
                case ActionState.IsRunning:
                    stateNameLabel.SetText("Running");
                    stateNameLabel.SetStyleColor(runningColor);
                    break;
                case ActionState.InCooldown:
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
