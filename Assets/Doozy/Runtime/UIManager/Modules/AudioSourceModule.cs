// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Runtime.Common.Extensions;
using Doozy.Runtime.Mody;
using Doozy.Runtime.Mody.Actions;
using UnityEngine;
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Runtime.UIManager.Modules
{
	[AddComponentMenu("Doozy/UI/Modules/AudioSource Module")]
	public class AudioSourceModule : ModyModule
	{
		public const string k_DefaultModuleName = "AudioSource";
		
		public AudioSource Source;
		public bool hasSource => Source != null;

		public SimpleModyAction Play;
		public SimpleModyAction Stop;
		public SimpleModyAction Mute;
		public SimpleModyAction Unmute;
		public SimpleModyAction Pause;
		public SimpleModyAction Unpause;
		
		public AudioSourceModule() : this(k_DefaultModuleName) { }

		public AudioSourceModule(AudioSource audioSource) : this(k_DefaultModuleName, audioSource) { }

		public AudioSourceModule(string moduleName, AudioSource audioSource) : this(moduleName.IsNullOrEmpty() ? k_DefaultModuleName : moduleName)
		{
			Source = audioSource;
		}

		public AudioSourceModule(string moduleName) : base(moduleName) { }

		protected override void SetupActions()
		{
			this.AddAction(Play ??= new SimpleModyAction(this, nameof(Play), ExecuteSourcePlay));
			this.AddAction(Stop ??= new SimpleModyAction(this, nameof(Stop), ExecuteSourceStop));
			this.AddAction(Mute ??= new SimpleModyAction(this, nameof(Mute), ExecuteSourceMute));
			this.AddAction(Unmute ??= new SimpleModyAction(this, nameof(Unmute), ExecuteSourceUnmute));
			this.AddAction(Pause ??= new SimpleModyAction(this, nameof(Pause), ExecutePauseSource));
			this.AddAction(Unpause ??= new SimpleModyAction(this, nameof(Unpause), ExecuteSourceUnpause));
		}

		public void ExecuteSourcePlay()
		{
			if (!hasSource) return;
			Source.Play();
		}

		public void ExecuteSourceStop()
		{
			if (!hasSource) return;
			Source.Stop();
		}
		
		public void ExecuteSourceMute()
		{
			if (!hasSource) return;
			Source.mute = true;
		}
		
		public void ExecuteSourceUnmute()
		{
			if (!hasSource) return;
			Source.mute = false;
		}
		
		public void ExecutePauseSource()
		{
			if (!hasSource) return;
			Source.Pause();
		}
		
		public void ExecuteSourceUnpause()
		{
			if (!hasSource) return;
			Source.UnPause();
		}
	}
}