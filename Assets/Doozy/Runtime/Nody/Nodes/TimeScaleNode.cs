// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Runtime.Nody.Nodes.Internal;
using Doozy.Runtime.Reactor.Easings;
using Doozy.Runtime.Reactor.Internal;
using Doozy.Runtime.Reactor.Reactions;
using UnityEngine;
// ReSharper disable RedundantOverriddenMember

namespace Doozy.Runtime.Nody.Nodes
{
    /// <summary>
    /// The TimeScale Node sets the scale at which the time is passing (it updates Time.timeScale). This can be used for slow motion effects.
    /// It does that either instantly or over a set duration (animated).
    /// The node can wait until the current Time.timeScale value has reached the target value, before activating the next node in the Graph.
    /// </summary>
    [Serializable]
    [NodyMenuPath("Time", "TimeScale")]
    public sealed class TimeScaleNode : SimpleNode
    {
        public static string timescaleAnimationId => $"{nameof(TimeScaleNode)} TimeScale Animation";

        private static FloatReaction s_timeScaleReaction;
        public static FloatReaction timeScaleReaction
        {
            get
            {
                if (s_timeScaleReaction != null)
                    return s_timeScaleReaction;

                return
                    s_timeScaleReaction =
                        Reaction
                            .Get<FloatReaction>()
                            .SetStringId(timescaleAnimationId)
                            .SetSetter(value => Time.timeScale = value)
                            .SetGetter(() => Time.timeScale);
            }
        }

        public float TargetValue;
        public bool AnimateValue;
        public float AnimationDuration;
        public Ease AnimationEase;
        public bool WaitForAnimationToFinish;

        public TimeScaleNode()
        {
            TargetValue = 1f;
            AnimateValue = false;
            AnimationDuration = 1f;
            AnimationEase = Ease.Linear;
            WaitForAnimationToFinish = false;
            
            AddInputPort()
                .SetCanBeDeleted(false)
                .SetCanBeReordered(false);

            AddOutputPort()
                .SetCanBeDeleted(false)
                .SetCanBeReordered(false);
        }

        public override void OnEnter(FlowNode previousNode = null, FlowPort previousPort = null)
        {
            base.OnEnter(previousNode, previousPort);
            StartTimer();
        }

        public override void OnExit()
        {
            base.OnExit();
            StopTimer();
        }

        private void StartTimer()
        {
            timeScaleReaction.Stop();
            timeScaleReaction.SetEase(AnimationEase);
            if (AnimateValue && AnimationDuration > 0)
            {
                timeScaleReaction.settings.duration = AnimationDuration;
                timeScaleReaction.SetFrom(Time.timeScale);
                timeScaleReaction.SetTo(TargetValue);
                timeScaleReaction.Play();
                if (WaitForAnimationToFinish)
                {
                    timeScaleReaction.ClearOnFinishCallback();
                    timeScaleReaction.AddOnFinishCallback(() => GoToNextNode(firstOutputPort));
                    return;
                }
                GoToNextNode(firstOutputPort);
                return;
            }
            Time.timeScale = TargetValue;
        }

        private void StopTimer()
        {
            timeScaleReaction.Finish();
            timeScaleReaction.ClearOnFinishCallback();
        }


    }
}
