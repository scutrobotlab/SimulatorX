// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using System.Linq;

namespace Doozy.Runtime.Signals
{
	[Serializable]
	public abstract class MultiSignalsReceiver<T> where T : SignalReceiver
	{
		public List<T> SignalsReceivers = new List<T>();

		protected abstract void OnSignal(Signal signal);

		public bool isConnected
		{
			get
			{
				return 
					SignalsReceivers != null && 
					SignalsReceivers.Count != 0 && 
					SignalsReceivers.Cast<SignalReceiver>()
						.Where(receiver => receiver != null)
						.Any(receiver => receiver.isConnected);
			}
		}

		public virtual void ConnectReceivers()
		{
			// Debugger.Log($"{nameof(MultiSignalReceiver)} > ConnectReceivers");
			foreach (T receiver in SignalsReceivers)
			{
				receiver.onSignal += OnSignal;
				receiver.Connect();
			}
		}

		public virtual void DisconnectReceivers()
		{
			// Debugger.Log($"{nameof(MultiSignalReceiver)} > DisconnectReceivers");
			foreach (T receiver in SignalsReceivers)
			{
				receiver.onSignal -= OnSignal;
				receiver.Disconnect();
			}
		}
	}
}