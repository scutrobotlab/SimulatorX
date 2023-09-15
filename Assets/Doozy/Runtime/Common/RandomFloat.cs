// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Doozy.Runtime.Common
{
    [Serializable]
    public class RandomFloat
    {
        /// <summary> Minimum value for the interval </summary>
        [SerializeField] private float MIN;
        public float min
        {
            get => MIN;
            set => MIN = value;
        }

        /// <summary> Maximum value for the interval </summary>
        [SerializeField] private float MAX;
        public float max
        {
            get => MAX;
            set => MAX = value;
        }

        /// <summary>
        /// Current random value from the [MIN,MAX] interval
        /// <para/> Value updated every time 'randomValue' is used
        /// </summary>
        public float currentValue { get; private set; }

        /// <summary>
        /// Previous random value
        /// <para/> Used to make sure no two consecutive random values are used
        /// </summary>
        public float previousValue { get; private set; }

        /// <summary>
        /// Random number between MIN [inclusive] and MAX [inclusive] (Read Only)
        /// <para/> Updates both the currentValue and the previousValue
        /// </summary>
        public float randomValue
        {
            get
            {
                previousValue = currentValue;
                currentValue = random;
                int counter = 100; //fail-safe counter to avoid infinite loops (if min = max)
                while (Mathf.Approximately(currentValue, previousValue) && counter > 0)
                {
                    currentValue = random;
                    counter--;
                }
                return currentValue;
            }
        }

        /// <summary> Random value from the [MIN,MAX] interval </summary>
        private float random => Random.Range(MIN, MAX);

        public RandomFloat(RandomFloat other) : this(other.min, other.max) {}

        public RandomFloat() : this(0, 1) {}

        public RandomFloat(float minValue, float maxValue)
        {
            Reset(minValue, maxValue);
        }

        public void Reset(float minValue = 0, float maxValue = 1)
        {
            MIN = minValue;
            MAX = maxValue;
            previousValue = currentValue = minValue;
            // previousValue = random; //set a random previous value
            // currentValue = randomValue; //init a current random value
        }
    }
}
