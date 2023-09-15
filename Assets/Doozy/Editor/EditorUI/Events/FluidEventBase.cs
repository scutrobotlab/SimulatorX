// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using UnityEngine;

namespace Doozy.Editor.EditorUI.Events
{
    public abstract class FluidEventBase<T> : IFluidEvent<T>
    {
        public T previousValue { get; }
        public T newValue { get; }
        public bool animateChange { get; }
        
        public bool used { get; set; }
        public float timestamp { get; }

        protected FluidEventBase(T previousValue, T newValue, bool animateChange)
        {
            this.previousValue = previousValue;
            this.newValue = newValue;
            this.animateChange = animateChange;
            
            used = false;
            timestamp = Time.time;
        }
    }
}
