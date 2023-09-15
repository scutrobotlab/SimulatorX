// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Runtime.UIManager.Animators;
using UnityEngine;
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Runtime.UIManager.Audio
{
    /// <summary>
    /// Specialized audio component used to play a set AudioClip by listening to a UIContainer (controller) show/hide commands.
    /// </summary>
    [AddComponentMenu("Doozy/UI/Audio/Container/UI Container Audio")]
    public class UIContainerAudio : BaseUIContainerAnimator
    {
        [SerializeField] private AudioSource AudioSource;
        /// <summary> Reference to a target Audio source </summary>
        public AudioSource audioSource => AudioSource;

        /// <summary> Check if a AudioSource is referenced or not </summary>
        public bool hasAudioSource => AudioSource != null;

        [SerializeField] private AudioClip ShowAudioClip;
        /// <summary> Container Show AudioClip </summary>
        public AudioClip showAudioClip => ShowAudioClip;

        [SerializeField] private AudioClip HideAudioClip;
        /// <summary> Container Hide AudioClip </summary>
        public AudioClip hideAudioClip => HideAudioClip;

        public override void StopAllReactions()
        {
            if (!hasAudioSource)
                return;

            audioSource.Stop();
        }

        public override void Show()
        {
            if (!hasAudioSource)
                return;

            audioSource.Stop();
            audioSource.clip = showAudioClip;
            if (audioSource.clip != null)
                audioSource.Play();
        }

        public override void ReverseShow() =>
            Hide();

        public override void Hide()
        {
            if (!hasAudioSource)
                return;

            audioSource.Stop();
            audioSource.clip = hideAudioClip;
            if (audioSource.clip != null)
                audioSource.Play();
        }

        public override void ReverseHide() =>
            Show();

        public override void UpdateSettings() {} //ignored
        public override void InstantShow() {}    //ignored
        public override void InstantHide() {}    //ignored
    }
}
