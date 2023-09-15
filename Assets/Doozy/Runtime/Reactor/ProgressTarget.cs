// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using UnityEngine;

namespace Doozy.Runtime.Reactor
{
    public abstract class ProgressTarget : MonoBehaviour
    {
        public enum Mode
        {
            /// <summary> Reaction progress value </summary>
            Progress,
            /// <summary> Progressor current value </summary>
            Value
        }

        [SerializeField] private Mode TargetMode = Mode.Value;
        public Mode targetMode
        {
            get => TargetMode;
            set => TargetMode = value;
        }

        /// <summary> Method used by a Progressor to when the current Value has changed </summary>
        /// <param name="progressor"> The Progressor that triggered this update </param>
        public abstract void UpdateTarget(Progressor progressor);
    }
}
