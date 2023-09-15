// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using static UnityEngine.Mathf;

namespace Doozy.Runtime.Reactor.Easings
{
    public static class EasingFunction
    {
        #region Constants
        // ReSharper disable UnusedMember.Local
        
        private const float PI = (float)Math.PI;

        /// <summary> 1.57079632679 </summary>
        private const float HALF_PI = PI * 0.5f;

        /// <summary> 6.28318530718 </summary>
        private const float TWO_PI = PI * 2f;

        private const float C1 = 1.70158f;
        private const float C2 = C1 * 1.525f;
        private const float C3 = C1 + 1f;
        private const float C4 = 2f * PI / 3f;
        private const float C5 = 2f * PI / 4.5f;

        // ReSharper restore UnusedMember.Local
        #endregion
        
        #region Linear interpolation (no easing)

        /// <summary>
        /// Linear interpolation (no easing)
        /// <para/> Modeled after the line y = x
        /// </summary>
        /// <param name="progress"> Value clamped between 0 and 1 </param>
        public static float LinearInterpolation(float progress)
        {
            progress = Clamp01(progress);
            return progress;
        }

        #endregion

        #region Quadratic easing; p^2

        /// <summary>
        /// Quadratic easing; p^2
        /// <para/> Modeled after the parabola y = x^2
        /// </summary>
        /// <param name="progress"> Value clamped between 0 and 1 </param>
        public static float QuadraticEaseIn(float progress)
        {
            progress = Clamp01(progress);
            return Pow(progress, 2);
        }

        /// <summary>
        /// Quadratic easing; p^2
        /// <para/> Modeled after the parabola y = -x^2 + 2x
        /// </summary>
        /// <param name="progress"> Value clamped between 0 and 1 </param>
        public static float QuadraticEaseOut(float progress)
        {
            progress = Clamp01(progress);
            return -(progress * (progress - 2f));
        }

        /// <summary>
        /// Quadratic easing; p^2
        /// <para/> Modeled after the piecewise quadratic
        /// <para/> y = (1/2)((2x)^2)             ; [0, 0.5)
        /// <para/> y = -(1/2)((2x-1)*(2x-3) - 1) ; [0.5, 1]
        /// </summary>
        /// <param name="progress"> Value clamped between 0 and 1 </param>
        public static float QuadraticEaseInOut(float progress)
        {
            progress = Clamp01(progress);

            if (progress < 0.5f)
            {
                return 2f * Pow(progress, 2);
            }
            else
            {
                return -2f * Pow(progress, 2) + 4 * progress - 1;
            }
        }

        #endregion

        #region Cubic easing; p^3

        /// <summary>
        /// Cubic easing; p^3
        /// <para/> Modeled after the cubic y = x^3
        /// </summary>
        /// <param name="progress"> Value clamped between 0 and 1 </param>
        public static float CubicEaseIn(float progress)
        {
            progress = Clamp01(progress);
            return Pow(progress, 3);
        }

        /// <summary>
        /// Cubic easing; p^3
        /// <para/> Modeled after the cubic y = (x - 1)^3 + 1
        /// </summary>
        /// <param name="progress"> Value clamped between 0 and 1 </param>
        public static float CubicEaseOut(float progress)
        {
            progress = Clamp01(progress);
            float value = progress - 1f;
            return Pow(value, 3) + 1f;
        }

        /// <summary>
        /// Cubic easing; p^3
        /// <para/> Modeled after the piecewise cubic
        /// <para/> y = (1/2)((2x)^3)       ; [0, 0.5)
        /// <para/> y = (1/2)((2x-2)^3 + 2) ; [0.5, 1]
        /// </summary>
        /// <param name="progress"> Value clamped between 0 and 1 </param>
        public static float CubicEaseInOut(float progress)
        {
            progress = Clamp01(progress);
            if (progress < 0.5f)
            {
                return 4f * Pow(progress, 3);
            }
            else
            {
                float value = 2f * progress - 2f;
                return 0.5f * Pow(value, 3) + 1f;
            }
        }

        #endregion

        #region Quartic easing; p^4

        /// <summary>
        /// Quartic easing; p^4
        /// <para/> Modeled after the quartic y = x^4
        /// </summary>
        /// <param name="progress"> Value clamped between 0 and 1 </param>
        public static float QuarticEaseIn(float progress)
        {
            progress = Clamp01(progress);
            return Pow(progress, 4);
        }

        /// <summary>
        /// Quartic easing; p^4
        /// <para/> Modeled after the quartic y = 1 - (x - 1)^4
        /// </summary>
        /// <param name="progress"> Value clamped between 0 and 1 </param>
        public static float QuarticEaseOut(float progress)
        {
            progress = Clamp01(progress);
            float value = progress - 1f;
            return Pow(value, 3) * (1f - progress) + 1f;
        }

        /// <summary>
        /// Quartic easing; p^4
        /// <para/> Modeled after the piecewise quartic
        /// <para/> y = (1/2)((2x)^4)        ; [0, 0.5)
        /// <para/> y = -(1/2)((2x-2)^4 - 2) ; [0.5, 1]
        /// </summary>
        /// <param name="progress"> Value clamped between 0 and 1 </param>
        public static float QuarticEaseInOut(float progress)
        {
            progress = Clamp01(progress);
            if (progress < 0.5f)
            {
                return 8f * Pow(progress, 4);
            }
            else
            {
                float value = progress - 1f;
                return -8f * Pow(value, 4) + 1f;
            }
        }

        #endregion

        #region Quintic easing; p^5

        /// <summary>
        /// Quintic easing; p^5
        /// <para/> Modeled after the quintic y = x^5
        /// </summary>
        /// <param name="progress"> Value clamped between 0 and 1 </param>
        public static float QuinticEaseIn(float progress)
        {
            progress = Clamp01(progress);
            return Pow(progress, 5);
        }

        /// <summary>
        /// Quintic easing; p^5
        /// <para/> Modeled after the quintic y = (x - 1)^5 + 1
        /// </summary>
        /// <param name="progress"> Value clamped between 0 and 1 </param>
        public static float QuinticEaseOut(float progress)
        {
            progress = Clamp01(progress);
            float value = progress - 1f;
            return Pow(value, 5) + 1f;
        }

        /// <summary>
        /// Quintic easing; p^5
        /// <para/> Modeled after the piecewise quintic
        /// <para/> y = (1/2)((2x)^5)       ; [0, 0.5)
        /// <para/> y = (1/2)((2x-2)^5 + 2) ; [0.5, 1]
        /// </summary>
        /// <param name="progress"> Value clamped between 0 and 1 </param>
        public static float QuinticEaseInOut(float progress)
        {
            progress = Clamp01(progress);
            if (progress < 0.5f)
            {
                return 16f * Pow(progress, 5);
            }
            else
            {
                float value = 2f * progress - 2f;
                return 0.5f * Pow(value, 5) + 1f;
            }
        }

        #endregion

        #region Sine wave easing; sin(p * PI/2)

        /// <summary>
        /// Sine wave easing; sin(p * PI/2)
        /// <para/> Modeled after quarter-cycle of sine wave
        /// </summary>
        /// <param name="progress"> Value clamped between 0 and 1 </param>
        public static float SineEaseIn(float progress)
        {
            progress = Clamp01(progress);
            return Sin((progress - 1f) * HALF_PI) + 1f;
        }

        /// <summary>
        /// Sine wave easing; sin(p * PI/2)
        /// <para/> Modeled after quarter-cycle of sine wave (different phase)
        /// </summary>
        /// <param name="progress"> Value clamped between 0 and 1 </param>
        public static float SineEaseOut(float progress)
        {
            progress = Clamp01(progress);
            return Sin(progress * HALF_PI);
        }

        /// <summary>
        /// Sine wave easing; sin(p * PI/2)
        /// <para/>  Modeled after half sine wave
        /// </summary>
        /// <param name="progress"> Value clamped between 0 and 1 </param>
        public static float SineEaseInOut(float progress)
        {
            progress = Clamp01(progress);
            return 0.5f * (1f - Cos(progress * PI));
        }

        #endregion

        #region Circular easing; sqrt(1 - p^2)

        /// <summary>
        /// Circular easing; sqrt(1 - p^2)
        /// <para/> Modeled after shifted quadrant IV of unit circle
        /// </summary>
        /// <param name="progress"> Value clamped between 0 and 1 </param>
        public static float CircularEaseIn(float progress)
        {
            progress = Clamp01(progress);
            return 1f - Sqrt(1 - Pow(progress, 2));
        }

        /// <summary>
        /// Circular easing; sqrt(1 - p^2)
        /// <para/> Modeled after shifted quadrant II of unit circle
        /// </summary>
        /// <param name="progress"> Value clamped between 0 and 1 </param>
        public static float CircularEaseOut(float progress)
        {
            progress = Clamp01(progress);
            return Sqrt((2 - progress) * progress);
        }

        /// <summary>
        /// Circular easing; sqrt(1 - p^2)
        /// <para/> Modeled after the piecewise circular function
        /// <para/> y = (1/2)(1 - sqrt(1 - 4x^2))           ; [0, 0.5)
        /// <para/> y = (1/2)(sqrt(-(2x - 3)*(2x - 1)) + 1) ; [0.5, 1]
        /// </summary>
        /// <param name="progress"> Value clamped between 0 and 1 </param>
        public static float CircularEaseInOut(float progress)
        {
            progress = Clamp01(progress);
            if (progress < 0.5f)
            {
                return 0.5f * (1f - Sqrt(1f - 4f * Pow(progress, 2)));
            }
            else
            {
                return 0.5f * (Sqrt(-(2f * progress - 3f) * (2f * progress - 1f)) + 1f);
            }
        }

        #endregion

        #region Exponential easing, base 2

        /// <summary>
        /// Exponential easing, base 2
        /// <para/> Modeled after the exponential function y = 2^(10(x - 1))
        /// </summary>
        /// <param name="progress"> Value clamped between 0 and 1 </param>
        public static float ExponentialEaseIn(float progress)
        {
            progress = Clamp01(progress);
            return
                Approximately(progress, 0f)
                    ? progress :
                    Pow(2f, 10f * (progress - 1f));
        }

        /// <summary>
        /// Exponential easing, base 2
        /// <para/> Modeled after the exponential function y = -2^(-10x) + 1
        /// </summary>
        /// <param name="progress"> Value clamped between 0 and 1 </param>
        public static float ExponentialEaseOut(float progress)
        {
            progress = Clamp01(progress);
            return
                Approximately(progress, 1f)
                    ? progress
                    : 1f - Pow(2f, -10f * progress);
        }

        /// <summary>
        /// Exponential easing, base 2
        /// <para/> Modeled after the piecewise exponential
        /// <para/> y = (1/2)2^(10(2x - 1))         ; [0,0.5)
        /// <para/> y = -(1/2)*2^(-10(2x - 1))) + 1 ; [0.5,1]
        /// </summary>
        /// <param name="progress"> Value clamped between 0 and 1 </param>
        public static float ExponentialEaseInOut(float progress)
        {
            progress = Clamp01(progress);
            if (Approximately(progress, 0f) || Approximately(progress, 1f)) return progress;

            if (progress < 0.5f)
            {
                return 0.5f * Pow(2f, 20f * progress - 10f);
            }
            else
            {
                return -0.5f * Pow(2f, -20f * progress + 10f) + 1f;
            }

        }

        #endregion

        #region Exponentially-damped sine wave easing (Elastic)

        /// <summary>
        /// Exponentially-damped sine wave easing (Elastic)
        /// <para/> Modeled after the damped sine wave y = sin(13pi/2*x)*pow(2, 10 * (x - 1))
        /// </summary>
        /// <param name="progress"> Value clamped between 0 and 1 </param>
        public static float ElasticEaseIn(float progress)
        {
            progress = Clamp01(progress);
            return Sin(13f * HALF_PI * progress) * Pow(2f, 10f * (progress - 1f));
        }

        /// <summary>
        /// Exponentially-damped sine wave easing (Elastic)
        /// <para/> Modeled after the damped sine wave y = sin(-13pi/2*(x + 1))*pow(2, -10x) + 1
        /// </summary>
        /// <param name="progress"> Value clamped between 0 and 1 </param>
        public static float ElasticEaseOut(float progress)
        {
            progress = Clamp01(progress);
            return Sin(-13f * HALF_PI * (progress + 1f)) * Pow(2f, -10f * progress) + 1f;
        }

        /// <summary>
        /// Exponentially-damped sine wave easing (Elastic)
        /// <para/> Modeled after the piecewise exponentially-damped sine wave:
        /// <para/> y = (1/2)*sin(13pi/2*(2*x))*pow(2, 10 * ((2*x) - 1))      ; [0,0.5)
        /// <para/> y = (1/2)*(sin(-13pi/2*((2x-1)+1))*pow(2,-10(2*x-1)) + 2) ; [0.5, 1]
        /// </summary>
        /// <param name="progress"> Value clamped between 0 and 1 </param>
        public static float ElasticEaseInOut(float progress)
        {
            progress = Clamp01(progress);
            if (progress < 0.5)
            {
                return 0.5f * Sin(13f * HALF_PI * (2f * progress)) * Pow(2f, 10f * (2f * progress - 1f));
            }
            else
            {
                return 0.5f * (Sin(-13f * HALF_PI * (2f * progress - 1f + 1f)) * Pow(2f, -10f * (2f * progress - 1f)) + 2);
            }
        }

        #endregion

        #region Overshooting cubic easing (Back)

        /// <summary>
        /// Overshooting cubic easing (Back)
        /// <para/> Modeled after the overshooting cubic y = x^3-x*sin(x*pi)
        /// </summary>
        /// <param name="progress"> Value clamped between 0 and 1 </param>
        public static float BackEaseIn(float progress)
        {
            progress = Clamp01(progress);
            return Pow(progress, 3) - progress * Sin(progress * PI);
        }

        /// <summary>
        /// Overshooting cubic easing (Back)
        /// <para/> Modeled after overshooting cubic y = 1-((1-x)^3-(1-x)*sin((1-x)*pi))
        /// </summary>
        /// <param name="progress"> Value clamped between 0 and 1 </param>
        public static float BackEaseOut(float progress)
        {
            progress = Clamp01(progress);
            float value = 1 - progress;
            return 1 - (Pow(value, 3) - value * Sin(value * PI));
        }

        /// <summary>
        /// Overshooting cubic easing (Back)
        /// <para/> Modeled after the piecewise overshooting cubic function:
        /// <para/> y = (1/2)*((2x)^3-(2x)*sin(2*x*pi))           ; [0, 0.5)
        /// <para/> y = (1/2)*(1-((1-x)^3-(1-x)*sin((1-x)*pi))+1) ; [0.5, 1]
        /// </summary>
        /// <param name="progress"> Value clamped between 0 and 1 </param>
        public static float BackEaseInOut(float progress)
        {
            progress = Clamp01(progress);
            if (progress < 0.5f)
            {
                float value = 2f * progress;
                return 0.5f * (Pow(value, 3) - value * Sin(value * PI));
            }
            else
            {
                float value = 1f - (2f * progress - 1f);
                return 0.5f * (1f - (Pow(value, 3) - value * Sin(value * PI))) + 0.5f;
            }
        }

        #endregion

        #region Exponentially-decaying bounce easing (Bounce)

        /// <summary>
        /// Exponentially-decaying bounce easing (Bounce)
        /// </summary>
        /// <param name="progress"> Value clamped between 0 and 1 </param>
        public static float BounceEaseIn(float progress)
        {
            progress = Clamp01(progress);
            return 1f - BounceEaseOut(1f - progress);
        }

        /// <summary>
        /// Exponentially-decaying bounce easing (Bounce)
        /// </summary>
        /// <param name="progress"> Value clamped between 0 and 1 </param>
        public static float BounceEaseOut(float progress)
        {
            progress = Clamp01(progress);
            if (progress < 4f / 11f)
            {
                return 121f * Pow(progress, 2) / 16f;
            }
            else if (progress < 8f / 11f)
            {
                return 363f / 40f * Pow(progress, 2) - 99f / 10f * progress + 17f / 5f;
            }
            else if (progress < 9 / 10.0)
            {
                return 4356f / 361f * Pow(progress, 2) - 35442f / 1805f * progress + 16061f / 1805f;
            }
            else
            {
                return 54f / 5f * Pow(progress, 2) - 513f / 25f * progress + 268f / 25f;
            }
        }

        /// <summary>
        /// Exponentially-decaying bounce easing (Bounce)
        /// </summary>
        /// <param name="progress"> Value clamped between 0 and 1 </param>
        public static float BounceEaseInOut(float progress)
        {
            progress = Clamp01(progress);
            if (progress < 0.5f)
            {
                return 0.5f * BounceEaseIn(progress * 2f);
            }
            else
            {
                return 0.5f * BounceEaseOut(progress * 2f - 1f) + 0.5f;
            }
        }

        #endregion
    }
}
