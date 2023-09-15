// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Random = UnityEngine.Random;

namespace Doozy.Runtime.Common
{
    [Serializable]
    public class RandomFloat01
    {
        private const float TOLERANCE = 0.000001f;
        
        /// <summary>
        /// Previous the random value
        /// </summary>
        public float previousValue { get; private set; }
        
        /// <summary> Returns a random float number between 0.0 [inclusive] and 1.0 [inclusive] (Read Only) </summary>
        public float randomValue
        {
            get
            {
                previousValue = m_TempPreviousValue;
                m_TempPreviousValue = getRandomValue;
                while (Math.Abs(m_TempPreviousValue - previousValue) > TOLERANCE)
                    m_TempPreviousValue = getRandomValue;
                return m_TempPreviousValue;
            }
        }

        private float m_TempPreviousValue;

        private float getRandomValue =>
            Random.value;


    }
}
