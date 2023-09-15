// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using System.Linq;
using Doozy.Editor.Signals.Automation.Generators;
using Doozy.Runtime.Common.Utils;
using Doozy.Runtime.Signals;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Doozy.Editor.Signals
{
    public static class SignalsUtils
    {
        private static List<ProviderAttributes> s_providerAttributesSet;
        public static List<ProviderAttributes> providerAttributesSet => GetProviderAttributesSet();

        // [MenuItem("Doozy/Signals/Refresh Providers")]
        public static void RefreshProviders()
        {
            SignalProviderExtensionGenerator.Run();
        }

        private static List<ProviderAttributes> GetProviderAttributesSet()
        {
            if (s_providerAttributesSet != null)
                return s_providerAttributesSet;

            s_providerAttributesSet = new List<ProviderAttributes>();

            IEnumerable<Type> providerTypes = TypeUtils.GetDerivedTypesOfType(typeof(SignalProvider));

            var gameObject = new GameObject { hideFlags = HideFlags.HideAndDontSave };

            foreach (Type providerType in providerTypes)
            {
                var provider = (SignalProvider)gameObject.AddComponent(providerType);
                s_providerAttributesSet.Add(provider.attributes);
                Object.DestroyImmediate(provider);
            }

            Object.DestroyImmediate(gameObject);

            s_providerAttributesSet =
                s_providerAttributesSet
                    .Distinct()
                    .OrderBy(a => a.id.Type)
                    .ThenBy(a => a.id.Category)
                    .ThenBy(a => a.id.Name)
                    .ToList();

            return s_providerAttributesSet;
        }
    }
}
