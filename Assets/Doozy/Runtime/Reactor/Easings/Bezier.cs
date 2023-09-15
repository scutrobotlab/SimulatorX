// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

//https://pomax.github.io/bezierinfo/
//https://gist.github.com/cjddmut/d789b9eb78216998e95c

using System;
using UnityEngine;
using static UnityEngine.Mathf;
namespace Doozy.Runtime.Reactor.Easings
{
    [Serializable]
    public class Bezier : IEasing
    {
        [SerializeField] private float Ax;
        [SerializeField] private float Ay;
        [SerializeField] private float Bx;
        [SerializeField] private float By;

        public Vector2 a => new Vector2(ax, ay);
        public float ax => Ax;
        public float ay => Ay;

        public Vector2 b => new Vector2(bx, By);
        public float bx => Bx;
        public float by => By;

        public Bezier(float ax, float ay, float bx, float by)
        {
            Ax = ax;
            Ay = ay;
            Bx = bx;
            By = by;
        }

        public Bezier(Vector2 a, Vector2 b)
            : this(a.x, a.y, b.x, b.y)
        {
        }

        public Bezier(Bezier other)
            : this(other.ax, other.ay, other.bx, other.by)
        {
        }

        public float Evaluate(float progress) =>
            Evaluate(a, b, Clamp01(progress));

        private static float A(float x, float y) => 1f - 3f * y + 3f * x;
        private static float B(float x, float y) => 3f * y - 6f * x;
        private static float C(float x) => 3f * x;

        /// <summary>
        /// x(t) given (t, x1, x2) or y(t) given (t, y1, y2)
        /// <para/> if a=x1 then b=x2
        /// <para/> if a=y1 then b=y2
        /// </summary>
        /// <param name="t"> Time </param>
        /// <param name="a"> x1 or y1 </param>
        /// <param name="b"> x2 or y2 </param>
        private static float CalcBezier(float t, float a, float b) =>
            ((A(a, b) * t + B(a, b)) * t + C(a)) * t;

        /// <summary>
        /// dx/dt given (t, x1, x2) or dy/dt given (t, y1, y2)
        /// <para/> if a=x1 then b=x2
        /// <para/> if a=y1 then b=y2
        /// </summary>
        /// <param name="t"> Time </param>
        /// <param name="a"> x1 or y1 </param>
        /// <param name="b"> x2 or y2 </param>
        private static float GetSlope(float t, float a, float b) =>
            3f * A(a, b) * t * t + 2f * B(a, b) * t + C(a);

        /// <summary>
        /// Calculates the time for the given x1,x2 or y1,y2 values
        /// <para/> if a=x1 then b=x2
        /// <para/> if a=y1 then b=y2
        /// </summary>
        /// <param name="t"> Time </param>
        /// <param name="a"> x1 or y1 </param>
        /// <param name="b"> x2 or y2 </param>
        /// <returns></returns>
        private static float CalculateTime(float t, float a, float b)
        {
            float time = t;
            for (int i = 0; i < 4; i++)
            {
                float slope = GetSlope(time, a, b);
                if (slope == 0) return time;
                float currentX = CalcBezier(time, a, b) - t;
                time -= currentX / slope;
            }
            return time;
        }

        private static float Calculate(float ax, float ay, float bx, float by, float t)
        {
            // if (mX1 == mY1 && mX2 == mY2) return aX; //linear
            if (Approximately(ax, ay) && Approximately(bx, by)) return t; //linear
            return CalcBezier(CalculateTime(t, ax, bx), ay, by);
        }

        public static float Evaluate(float ax, float ay, float bx, float by, float t) =>
            Calculate(ax, ay, bx, by, t);

        public static float Evaluate(Vector2 a, Vector2 b, float t) =>
            Calculate(a.x, a.y, b.x, b.y, t);

        public static float Evaluate(Bezier b, float t) =>
            Evaluate(b.a.x, b.a.y, b.b.x, b.b.y, t);

        public static float Evaluate(float progress, Ease ease)
        {
            progress = Clamp01(progress);

            //The switch expression does not handle all possible values of its input type (it is not exhaustive).
            #pragma warning disable 8509
            {
                return ease switch
                       {
                           Ease.Linear     => Evaluate(0f, 0f, 1f, 1f, progress),
                           Ease.Easy       => Evaluate(0.25f, 0.1f, 0.25f, 1.0f, progress),
                           Ease.InEasy     => Evaluate(0.42f, 0f, 1f, 1f, progress),
                           Ease.OutEasy    => Evaluate(0f, 0f, 0.58f, 1f, progress),
                           Ease.InOutEasy  => Evaluate(0.42f, 0, 0.58f, 1f, progress),
                           Ease.InSine     => Evaluate(0.47f, 0, 0.745f, 0.715f, progress),
                           Ease.OutSine    => Evaluate(0.39f, 0.575f, 0.565f, 1f, progress),
                           Ease.InOutSine  => Evaluate(0.445f, 0.05f, 0.55f, 0.95f, progress),
                           Ease.InQuad     => Evaluate(0.55f, 0.085f, 0.68f, 0.53f, progress),
                           Ease.OutQuad    => Evaluate(0.25f, 0.46f, 0.45f, 0.94f, progress),
                           Ease.InOutQuad  => Evaluate(0.455f, 0.03f, 0.515f, 0.955f, progress),
                           Ease.InCubic    => Evaluate(0.55f, 0.055f, 0.675f, 0.19f, progress),
                           Ease.OutCubic   => Evaluate(0.215f, 0.61f, 0.355f, 1f, progress),
                           Ease.InOutCubic => Evaluate(0.645f, 0.045f, 0.355f, 1f, progress),
                           Ease.InQuart    => Evaluate(0.895f, 0.03f, 0.685f, 0.22f, progress),
                           Ease.OutQuart   => Evaluate(0.165f, 0.84f, 0.44f, 1f, progress),
                           Ease.InOutQuart => Evaluate(0.77f, 0f, 0.175f, 1f, progress),
                           Ease.InQuint    => Evaluate(0.755f, 0.05f, 0.855f, 0.06f, progress),
                           Ease.OutQuint   => Evaluate(0.23f, 1f, 0.32f, 1f, progress),
                           Ease.InOutQuint => Evaluate(0.86f, 0f, 0.07f, 1f, progress),
                           Ease.InExpo     => Evaluate(0.95f, 0.05f, 0.795f, 0.035f, progress),
                           Ease.OutExpo    => Evaluate(0.19f, 1f, 0.22f, 1f, progress),
                           Ease.InOutExpo  => Evaluate(1f, 0f, 0f, 1f, progress),
                           Ease.InCirc     => Evaluate(0.6f, 0.04f, 0.98f, 0.335f, progress),
                           Ease.OutCirc    => Evaluate(0.075f, 0.82f, 0.165f, 1f, progress),
                           Ease.InOutCirc  => Evaluate(0.785f, 0.135f, 0.15f, 0.86f, progress),
                           Ease.InBack     => Evaluate(0.8f, -0.4f, 0f, 1f, progress),
                           // Ease.OutBack      => expr,
                           // Ease.InOutBack    => expr,
                           // Ease.InElastic    => expr,
                           // Ease.OutElastic   => expr,
                           // Ease.InOutElastic => expr,
                           // Ease.InBounce     => expr,
                           // Ease.OutBounce    => expr,
                           // Ease.InOutBounce  => expr,
                           // Ease.Spring       => expr,
                           // _                 => throw new ArgumentOutOfRangeException(nameof(ease), ease, null)
                       };
            }
            #pragma warning restore 8509

        }
    }
}
