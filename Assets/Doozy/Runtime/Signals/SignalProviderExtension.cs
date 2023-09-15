// Copyright (c) 2015 - 2021 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

//.........................
//.....Generated Class.....
//.........................
//.......Do not edit.......
//.........................

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
// ReSharper disable RedundantNameQualifier
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable ConvertSwitchStatementToSwitchExpression

namespace Doozy.Runtime.Signals
{
    public abstract partial class SignalProvider
    {
        public static IEnumerable<string> GetProviderCategories(ProviderType providerType)
        {
            return providerType switch
                   {
                       ProviderType.Global => Global.GetProviderCategories(),
                       ProviderType.Local  => Local.GetProviderCategories(),
                       _                   => throw new ArgumentOutOfRangeException(nameof(providerType), providerType, null)
                   };
        }

        public static IEnumerable<string> GetProviderNames(ProviderType providerType, string category)
        {
            return providerType switch
                   {
                       ProviderType.Global => Global.GetProviderNames(category),
                       ProviderType.Local  => Local.GetProviderNames(category),
                       _                   => throw new ArgumentOutOfRangeException(nameof(providerType), providerType, null)
                   };
        }

        public static Type GetProviderType(ProviderId providerId)
        {
            switch (providerId.Type)
            {
                case ProviderType.Global:
                    IEnumerable<ProviderAttributes> globalAttributes = Global.GetAttributesList(providerId.Category).Where(a => a.id == providerId);
                    return globalAttributes.First().typeOfProvider;
                case ProviderType.Local:
                    IEnumerable<ProviderAttributes> localAttributes = Local.GetAttributesList(providerId.Category).Where(a => a.id == providerId);
                    return localAttributes.First().typeOfProvider;
                default:
                    throw new Exception($"There is no {providerId.Category} {providerId.Name} {nameof(SignalProvider)} registered in the {nameof(SignalProvider)}");
            }

        }

        public static class Local
        {
            public const ProviderType k_ProviderType = ProviderType.Local;

            public static IEnumerable<string> GetProviderCategories() =>
                new List<string>
                {
                    Pointer.k_ProviderCategory,
                    UI.k_ProviderCategory,

                };

            public static IEnumerable<string> GetProviderNames(string category)
            {
                switch (category)
                {
                    case Pointer.k_ProviderCategory: return Pointer.GetProviderNames();
                    case UI.k_ProviderCategory: return UI.GetProviderNames();

                }

                throw new Exception($"There is no {k_ProviderType} {nameof(SignalProvider)} '{category}' category registered in the {nameof(SignalProvider)}");
            }

            public static IEnumerable<ProviderAttributes> GetAttributesList(string category)
            {
                switch (category)
                {
                    case Pointer.k_ProviderCategory: return Pointer.AttributesList;
                    case UI.k_ProviderCategory: return UI.AttributesList;

                }
                
                throw new Exception($"There is no {k_ProviderType} {nameof(SignalProvider)} '{category}' category registered in the {nameof(SignalProvider)}");
            }
            
            public static class Pointer
            {
                public const string k_ProviderCategory = nameof(Pointer);

                public static IEnumerable<string> GetProviderNames() => Enum.GetValues(typeof(Name)).Cast<Name>().Select(name => name.ToString());
                public static ProviderId GetProviderId(Name providerName) => new ProviderId(k_ProviderType, k_ProviderCategory, providerName.ToString());
                public static ISignalProvider Get(Name providerName, GameObject signalSource) => SignalsService.GetProvider(GetProviderId(providerName), signalSource);

                public enum Name
                {
                    Click,
                    DoubleClick,
                    Down,
                    Enter,
                    Exit,
                    LeftClick,
                    LongClick,
                    MiddleClick,
                    RightClick,
                    Up,
                }

                public static readonly List<ProviderAttributes> AttributesList = new List<ProviderAttributes>
                {
                    new ProviderAttributes(k_ProviderType, k_ProviderCategory, Name.Click.ToString(), typeof(Doozy.Runtime.UIManager.Triggers.PointerClickTrigger)),
                    new ProviderAttributes(k_ProviderType, k_ProviderCategory, Name.DoubleClick.ToString(), typeof(Doozy.Runtime.UIManager.Triggers.PointerDoubleClickTrigger)),
                    new ProviderAttributes(k_ProviderType, k_ProviderCategory, Name.Down.ToString(), typeof(Doozy.Runtime.UIManager.Triggers.PointerDownTrigger)),
                    new ProviderAttributes(k_ProviderType, k_ProviderCategory, Name.Enter.ToString(), typeof(Doozy.Runtime.UIManager.Triggers.PointerEnterTrigger)),
                    new ProviderAttributes(k_ProviderType, k_ProviderCategory, Name.Exit.ToString(), typeof(Doozy.Runtime.UIManager.Triggers.PointerExitTrigger)),
                    new ProviderAttributes(k_ProviderType, k_ProviderCategory, Name.LeftClick.ToString(), typeof(Doozy.Runtime.UIManager.Triggers.PointerLeftClickTrigger)),
                    new ProviderAttributes(k_ProviderType, k_ProviderCategory, Name.LongClick.ToString(), typeof(Doozy.Runtime.UIManager.Triggers.PointerLongClickTrigger)),
                    new ProviderAttributes(k_ProviderType, k_ProviderCategory, Name.MiddleClick.ToString(), typeof(Doozy.Runtime.UIManager.Triggers.PointerMiddleClickTrigger)),
                    new ProviderAttributes(k_ProviderType, k_ProviderCategory, Name.RightClick.ToString(), typeof(Doozy.Runtime.UIManager.Triggers.PointerRightClickTrigger)),
                    new ProviderAttributes(k_ProviderType, k_ProviderCategory, Name.Up.ToString(), typeof(Doozy.Runtime.UIManager.Triggers.PointerUpTrigger)),
                };
            }

            public static class UI
            {
                public const string k_ProviderCategory = nameof(UI);

                public static IEnumerable<string> GetProviderNames() => Enum.GetValues(typeof(Name)).Cast<Name>().Select(name => name.ToString());
                public static ProviderId GetProviderId(Name providerName) => new ProviderId(k_ProviderType, k_ProviderCategory, providerName.ToString());
                public static ISignalProvider Get(Name providerName, GameObject signalSource) => SignalsService.GetProvider(GetProviderId(providerName), signalSource);

                public enum Name
                {
                    Deselected,
                    Selected,
                    Submit,
                }

                public static readonly List<ProviderAttributes> AttributesList = new List<ProviderAttributes>
                {
                    new ProviderAttributes(k_ProviderType, k_ProviderCategory, Name.Deselected.ToString(), typeof(Doozy.Runtime.UIManager.Triggers.UIDeselectedTrigger)),
                    new ProviderAttributes(k_ProviderType, k_ProviderCategory, Name.Selected.ToString(), typeof(Doozy.Runtime.UIManager.Triggers.UISelectedTrigger)),
                    new ProviderAttributes(k_ProviderType, k_ProviderCategory, Name.Submit.ToString(), typeof(Doozy.Runtime.UIManager.Triggers.UISubmitTrigger)),
                };
            }


        }

        public static class Global
        {
            public const ProviderType k_ProviderType = ProviderType.Global;

            public static IEnumerable<string> GetProviderCategories() =>
                new List<string>
                {
                    Input.k_ProviderCategory,
                     
                };

            public static IEnumerable<string> GetProviderNames(string category)
            {
                switch (category)
                {
                    case Input.k_ProviderCategory: return Input.GetProviderNames();
                       
                }

                throw new Exception($"There is no {k_ProviderType} {nameof(SignalProvider)} '{category}' category registered in the {nameof(SignalProvider)}");
            }

            public static IEnumerable<ProviderAttributes> GetAttributesList(string category)
            {
                switch (category)
                {
                    case Input.k_ProviderCategory: return Input.AttributesList;
                       
                }
                
                throw new Exception($"There is no {k_ProviderType} {nameof(SignalProvider)} '{category}' category registered in the {nameof(SignalProvider)}");
            }

            public static class Input
            {
                public const string k_ProviderCategory = nameof(Input);

                public static IEnumerable<string> GetProviderNames() => Enum.GetValues(typeof(Name)).Cast<Name>().Select(name => name.ToString());
                public static ProviderId GetProviderId(Name providerName) => new ProviderId(k_ProviderType, k_ProviderCategory, providerName.ToString());
                public static ISignalProvider Get(Name providerName, GameObject signalSource) => SignalsService.GetProvider(GetProviderId(providerName), signalSource);

                public enum Name
                {
                    BackButton,
                }

                public static readonly List<ProviderAttributes> AttributesList = new List<ProviderAttributes>
                {
                    new ProviderAttributes(k_ProviderType, k_ProviderCategory, Name.BackButton.ToString(), typeof(Doozy.Runtime.UIManager.Triggers.InputBackButtonTrigger)),
                };
            }

            
           
        }
    }
}