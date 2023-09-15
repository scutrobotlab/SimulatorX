// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Runtime.Common;
using Doozy.Runtime.Reactor.Easings;
using UnityEngine;
using static UnityEngine.Mathf;

namespace Doozy.Runtime.Reactor
{
    [Serializable]
    public class ReactionSettings
    {
        public const int k_InfiniteLoops = -1;
        public const PlayMode k_PlayMode = PlayMode.Normal;
        public const EaseMode k_EaseMode = EaseMode.Ease;
        public const Ease k_Ease = Ease.Easy;
        public const float k_StartDelay = 0f;
        public const float k_Duration = 1f;
        public const int k_Loops = 0;
        public const float k_LoopDelay = 0f;
        public const float k_Strength = 1f;
        public const int k_Vibration = 8;
        public const float k_Elasticity = 1f;

        [SerializeField] private PlayMode PlayMode;
        /// <summary>
        /// Determines what type of computation is used when the reaction is playing
        /// </summary>
        public PlayMode playMode
        {
            get => PlayMode;
            set => PlayMode = value;
        }

        [SerializeField] private EaseMode EaseMode;
        /// <summary>
        /// Determines the evaluation method used to update the eased progress
        /// <para/> EaseMode.Ease - uses the set ease to evaluate the current reaction and update the eased progress
        /// <para/> EaseMode.AnimationCurve - uses the set curve to evaluate the current project and update the eased progress
        /// </summary>
        public EaseMode easeMode
        {
            get => EaseMode;
            set => EaseMode = value;
        }

        [SerializeField] private Ease Ease;
        /// <summary>
        /// Easing used to update the eased progress when the easeMode is set to EaseMode.Ease
        /// </summary>
        public Ease ease
        {
            get => Ease;
            set
            {
                Ease = value;
                EaseMode = EaseMode.Ease;
            }
        }

        private IEasing easing => EaseFactory.GetEase(Ease);

        [SerializeField] private AnimationCurve Curve;
        /// <summary>
        /// Animation curve used to update the eased progress when the easeMode is set to EaseMode.AnimationCurve
        /// </summary>
        public AnimationCurve curve
        {
            get => Curve;
            set
            {
                Curve = value;
                EaseMode = EaseMode.AnimationCurve;
            }
        }

        [SerializeField] private float StartDelay;
        /// <summary>
        /// Reaction start delay time interval
        /// </summary>
        public float startDelay
        {
            get => StartDelay;
            set => StartDelay = Max(0, value);
        }

        [SerializeField] private float Duration;
        /// <summary>
        /// Reaction playing time interval
        /// </summary>
        public float duration
        {
            get => Duration;
            set => Duration = Max(0, value);
        }

        [SerializeField] private int Loops;
        /// <summary>
        /// Number of times the reaction replays (restarts)
        /// <para/> -1 - infinite loops
        /// <para/> 0 - no loops (plays once)
        /// <para/> > 0 - replays (restarts) for the given number of loops
        /// </summary>
        public int loops
        {
            get => Loops;
            set => Loops = Max(-1, value);
        }

        [SerializeField] private float LoopDelay;
        /// <summary>
        /// Reaction delay between loops time interval
        /// </summary>
        public float loopDelay
        {
            get => LoopDelay;
            set => LoopDelay = Max(0, value);
        }

        [SerializeField] private bool UseRandomStartDelay;
        /// <summary>
        /// Determines if the reaction's start delay time interval is a fixed or a random value
        /// <para/> Default value: FALSE
        /// </summary>
        public bool useRandomStartDelay
        {
            get => UseRandomStartDelay;
            set => UseRandomStartDelay = value;
        }

        [SerializeField] private bool UseRandomDuration;
        /// <summary>
        /// Determines if the reaction's duration time interval is a fixed or a random value
        /// <para/> Default value: FALSE
        /// </summary>
        public bool useRandomDuration
        {
            get => UseRandomDuration;
            set => UseRandomDuration = value;
        }

        [SerializeField] private bool UseRandomLoops;
        /// <summary>
        /// Determines if the reaction should play for a fixed or a random number of loops
        /// <para/> Default value: FALSE
        /// </summary>
        public bool useRandomLoops
        {
            get => UseRandomLoops;
            set => UseRandomLoops = value;
        }

        [SerializeField] private bool UseRandomLoopDelay;
        /// <summary>
        /// Determines if the reaction's delay between loops interval is a fixed or a random value
        /// <para/> Default value: FALSE
        /// </summary>
        public bool useRandomLoopDelay
        {
            get => UseRandomLoopDelay;
            set => UseRandomLoopDelay = value;
        }

        [SerializeField] private RandomFloat RandomStartDelay = new RandomFloat();
        /// <summary>
        /// Random start delay interval value, used if useRandomStartDelay is TRUE
        /// <para/> Returns a new random value (between min and max)
        /// </summary>
        public RandomFloat randomStartDelay
        {
            get => RandomStartDelay;
            set => RandomStartDelay = value;
        }

        [SerializeField] private RandomFloat RandomDuration = new RandomFloat();
        /// <summary>
        /// Random duration interval value, used if useRandomDuration is TRUE
        /// <para/> Returns a new random value (between min and max)
        /// </summary>
        public RandomFloat randomDuration
        {
            get => RandomDuration;
            set => RandomDuration = value;
        }

        [SerializeField] private RandomInt RandomLoops = new RandomInt();
        /// <summary>
        /// Random number of loops value, used if useRandomLoops is TRUE
        /// <para/> Returns a new random value (between min and max)
        /// </summary>
        public RandomInt randomLoops
        {
            get => RandomLoops;
            set => RandomLoops = value;
        }

        [SerializeField] private RandomFloat RandomLoopDelay = new RandomFloat();
        /// <summary>
        /// Random delay between loops interval value, used if useRandomLoopDelay is TRUE
        /// <para/> Returns a new random value (between min and max)
        /// </summary>
        public RandomFloat randomLoopDelay
        {
            get => RandomLoopDelay;
            set => RandomLoopDelay = value;
        }

        [SerializeField] private float Strength;
        /// <summary>
        /// Multiplier applied to the current value for spring and shake play modes
        /// <para/> Default value: 1f
        /// </summary>
        public float strength
        {
            get => Strength;
            set => Strength = value;
        }

        [SerializeField] private int Vibration;
        /// <summary>
        /// Represents the minimum number of oscillations for spring and shake play modes
        /// <para/> Higher value means more oscillations during the reaction's duration
        /// </summary>
        public int vibration
        {
            get => Vibration;
            set => Vibration = Max(0, value);
        }

        [SerializeField] private float Elasticity;
        /// <summary>
        /// Spring elasticity controls how much the current value goes back beyond the start value when contracting
        /// <para/> 0 - current value does not go beyond the start value
        /// <para/> 1 - current value goes beyond the start value at maximum elastic force
        /// </summary>
        public float elasticity
        {
            get => Elasticity;
            set => Elasticity = Clamp01(value);
        }

        [SerializeField] private bool FadeOutShake;
        /// <summary>
        /// Fade out the shake animation, by easing the last 20% of cycles (shakes) into a semi-smooth transition
        /// </summary>
        public bool fadeOutShake
        {
            get => FadeOutShake;
            set => FadeOutShake = value;
        }

        /// <summary> Returns TRUE is useRandomLoops is true or if loops not 0 (zero) </summary>
        public bool hasLoops => useRandomLoops || loops != 0;

        /// <summary>
        /// Get the start delay value, for the reaction's current start delay setting
        /// <para/> useRandomStartDelay: TRUE - returns randomStartDelay value
        /// <para/> useRandomStartDelay: FALSE - returns startDelay value
        /// </summary>
        public float GetStartDelay() => useRandomStartDelay ? randomStartDelay.randomValue : startDelay;

        /// <summary> Set a random start delay interval [min,max] </summary>
        /// <param name="min"> Min value </param>
        /// <param name="max"> Max value </param>
        /// <param name="useRandomValue"> If TRUE, the reaction will use a random start delay value, in the given [min,max] interval </param>
        public void SetRandomStartDelay(float min, float max, bool useRandomValue = true)
        {
            useRandomStartDelay = useRandomValue;
            min = Max(0, min);
            max = Max(0, max);
            randomStartDelay = new RandomFloat(min, max);

            if (!Approximately(min, max)) return;
            // min == max >>> makes no sense to use a random value
            useRandomStartDelay = false;
            startDelay = min;
        }

        /// <summary>
        /// Get the duration value, for the reaction's current duration setting
        /// <para/> useRandomDuration: TRUE - returns randomDuration value
        /// <para/> useRandomDuration: FALSE - returns duration value
        /// </summary>
        public float GetDuration() => useRandomDuration ? randomDuration.randomValue : duration;

        /// <summary> Set a random duration interval [min,max] </summary>
        /// <param name="min"> Min value </param>
        /// <param name="max"> Max value </param>
        /// <param name="useRandomValue"> If TRUE, the reaction will use a random duration value, in the given [min,max] interval </param>
        public void SetRandomDuration(float min, float max, bool useRandomValue = true)
        {
            useRandomDuration = useRandomValue;
            min = Max(0, min);
            max = Max(0, max);
            randomDuration = new RandomFloat(min, max);

            if (!Approximately(min, max)) return;
            // min == max >>> makes no sense to use a random value
            useRandomDuration = false;
            duration = min;
        }

        /// <summary>
        /// Get the number of loops, the reaction will replay (restart) until it finishes
        /// <para/> useRandomLoops: TRUE - returns randomLoops value
        /// <para/> useRandomLoops: FALSE - returns loops value
        /// </summary>
        public int GetLoops() => useRandomLoops ? randomLoops.randomValue : loops;

        /// <summary> Set a random loops interval [min,max] </summary>
        /// <param name="min"> Min value </param>
        /// <param name="max"> Max value </param>
        /// <param name="useRandomValue"> If TRUE, the reaction will use a random loops value, in the given [min,max] interval </param>
        public void SetRandomLoops(int min, int max, bool useRandomValue = true)
        {
            useRandomLoops = useRandomValue;
            min = Max(0, min);
            max = Max(1, max);
            randomLoops = new RandomInt(min, max);

            if (min != max) return;
            // min == max >>> makes no sense to use a random value
            useRandomLoops = false;
            loops = min;
        }

        /// <summary>
        /// Get the delay between loops delay value, for the reaction's current loop delay setting
        /// <para/> useRandomLoopDelay: TRUE - returns randomLoopDelay value
        /// <para/> useRandomLoopDelay: FALSE - returns loopDelay value
        /// </summary>
        public float GetLoopDelay() => useRandomLoopDelay ? randomLoopDelay.randomValue : loopDelay;

        /// <summary> Set a random loop delay interval [min,max] </summary>
        /// <param name="min"> Min value </param>
        /// <param name="max"> Max value </param>
        /// <param name="useRandomValue"> If TRUE, the reaction will use a random loop delay value, in the given [min,max] interval </param>
        public void SetRandomLoopDelay(float min, float max, bool useRandomValue = true)
        {
            useRandomLoopDelay = useRandomValue;
            min = Max(0, min);
            max = Max(0, max);
            randomLoopDelay = new RandomFloat(min, max);

            if (!Approximately(min, max)) return;
            // min == max >>> makes no sense to use a random value
            useRandomLoopDelay = false;
            loopDelay = min;
        }

        public ReactionSettings()
        {
            Reset();
        }

        public ReactionSettings(ReactionSettings other)
        {
            Reset();

            playMode = other.playMode;
            curve = other.curve;
            ease = other.ease;
            easeMode = other.easeMode;
            startDelay = other.startDelay;
            duration = other.duration;
            loops = other.loops;
            loopDelay = other.loopDelay;
            strength = other.strength;
            vibration = other.vibration;
            elasticity = other.elasticity;

            UseRandomStartDelay = other.useRandomStartDelay;
            UseRandomDuration = other.useRandomDuration;
            UseRandomLoops = other.useRandomLoops;
            UseRandomLoopDelay = other.UseRandomLoopDelay;

            RandomStartDelay = new RandomFloat(other.randomStartDelay);
            RandomDuration = new RandomFloat(other.randomDuration);
            RandomLoops = new RandomInt(other.randomLoops);
            RandomLoopDelay = new RandomFloat(other.randomLoopDelay);
        }

        /// <summary> Reset settings to the default values </summary>
        public void Reset()
        {
            playMode = k_PlayMode;
            curve = AnimationCurve.Linear(0, 0, 1, 1);
            ease = k_Ease;
            easeMode = k_EaseMode;
            startDelay = k_StartDelay;
            duration = k_Duration;
            loops = k_Loops;
            loopDelay = k_LoopDelay;
            strength = k_Strength;
            vibration = k_Vibration;
            elasticity = k_Elasticity;

            UseRandomStartDelay = false;
            UseRandomDuration = false;
            UseRandomLoops = false;
            UseRandomLoopDelay = false;

            RandomStartDelay.Reset();
            RandomDuration.Reset();
            RandomLoops.Reset();
            RandomLoopDelay.Reset();
        }

        /// <summary>
        /// Validate settings
        /// </summary>
        public void Validate()
        {
            StartDelay = startDelay;
            Duration = duration;
            Loops = loops;
            LoopDelay = loopDelay;

            Strength = strength;
            Vibration = vibration;
            Elasticity = elasticity;
        }

        /// <summary>
        /// Get the calculated eased progress value for the given progress value
        /// </summary>
        /// <param name="progress"> Progress value </param>
        public float CalculateEasedProgress(float progress)
        {
            switch (easeMode)
            {
                case EaseMode.Ease: return easing.Evaluate(progress);
                case EaseMode.AnimationCurve: return curve.Evaluate(progress);
                default: throw new ArgumentOutOfRangeException();
            }
        }
    }
}
