// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

// ReSharper disable UnusedMemberInSuper.Global

namespace Doozy.Runtime.Signals
{
	public interface ISignalProvider
	{
		ProviderAttributes attributes { get; }
		SignalStream stream { get; }
		bool isConnected { get; }
		
		void OpenStream();
		void CloseStream();

		bool SendSignal();
		bool SendSignal<T>(T signalValue);
	}
}