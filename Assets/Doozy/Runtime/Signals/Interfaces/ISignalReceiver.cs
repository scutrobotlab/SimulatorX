// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

// ReSharper disable UnusedMemberInSuper.Global

namespace Doozy.Runtime.Signals
{
	public interface ISignalReceiver
	{
		/// <summary> Stream connection mode </summary>
		StreamConnection streamConnection { get; }
		
		/// <summary> Identifier used to connect to a signal provider, when stream connection mode is set to ProvideId </summary>
		ProviderId providerId { get; }
		
		/// <summary> Signal provider reference, used when the stream connection mode is set to ProviderReference </summary>
		SignalProvider providerReference { get; }
		
		/// <summary> Identifier used to connect to a signal stream, when stream connection mode is set to StreamId </summary>
		StreamId streamId { get; }

		/// <summary> Signal stream reference that this receiver is connected to (when not connected it is null) </summary>
		SignalStream stream { get; }
		
		/// <summary> TRUE is this receiver is connected to a signal stream </summary>
		bool isConnected { get; }

		/// <summary> Connect to signal stream using the selected connection mode </summary>
		void Connect();
		
		/// <summary> Disconnect from signal stream (if connected) </summary>
		void Disconnect();
		
		/// <summary> Invoked every time the stream sends a signal through </summary>
		/// <param name="signal"> Signal (can be a MetaSignal as well) </param>
		void OnSignal(Signal signal);
	}
}