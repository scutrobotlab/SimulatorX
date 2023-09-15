// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;

namespace Doozy.Runtime.Signals
{
	[Serializable]
	public struct ProviderAttributes
	{
		public ProviderId id { get; }
		public Type typeOfProvider { get; }

		public ProviderAttributes(ProviderId id, Type typeOfProvider)
		{
			this.id = id;
			this.typeOfProvider = typeOfProvider;
		}

		public ProviderAttributes(ProviderType providerType, string providerCategory, string providerName, Type typeOfProvider)
			: this(new ProviderId(providerType, providerCategory, providerName), typeOfProvider) { }

		public override string ToString() =>
			$"{id.ToString()}";
	}
}