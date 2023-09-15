// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using System.Linq;
using Doozy.Runtime.Signals;

namespace Doozy.Runtime.UIManager
{
    public partial class UIBehaviour
    {
        public enum Name
        {
            PointerEnter,
            PointerExit,
            PointerDown,
            PointerUp,
            PointerClick,
            PointerDoubleClick,
            PointerLongClick,
            PointerLeftClick,
            PointerMiddleClick,
            PointerRightClick,
            Selected,
            Deselected,
            Submit
        }

        private static List<string> s_behaviourNames;
        public static IEnumerable<string> GetBehaviourNames() =>
            s_behaviourNames ??=
                Enum.GetValues(typeof(Name))
                    .Cast<Name>()
                    .Select(name => name.ToString())
                    .ToList();

        private static List<Name> s_behaviours;
        public static IEnumerable<Name> GetBehaviours() =>
            s_behaviours ??=
                Enum.GetValues(typeof(Name))
                    .Cast<Name>()
                    .ToList();

        public static ProviderId GetProvideId(Name behaviourName)
        {
            return behaviourName switch
                   {
                       Name.PointerEnter       => SignalProvider.Local.Pointer.GetProviderId(SignalProvider.Local.Pointer.Name.Enter),
                       Name.PointerExit        => SignalProvider.Local.Pointer.GetProviderId(SignalProvider.Local.Pointer.Name.Exit),
                       Name.PointerDown        => SignalProvider.Local.Pointer.GetProviderId(SignalProvider.Local.Pointer.Name.Down),
                       Name.PointerUp          => SignalProvider.Local.Pointer.GetProviderId(SignalProvider.Local.Pointer.Name.Up),
                       Name.PointerClick       => SignalProvider.Local.Pointer.GetProviderId(SignalProvider.Local.Pointer.Name.Click),
                       Name.PointerDoubleClick => SignalProvider.Local.Pointer.GetProviderId(SignalProvider.Local.Pointer.Name.DoubleClick),
                       Name.PointerLongClick   => SignalProvider.Local.Pointer.GetProviderId(SignalProvider.Local.Pointer.Name.LongClick),
                       Name.PointerLeftClick   => SignalProvider.Local.Pointer.GetProviderId(SignalProvider.Local.Pointer.Name.LeftClick),
                       Name.PointerMiddleClick => SignalProvider.Local.Pointer.GetProviderId(SignalProvider.Local.Pointer.Name.MiddleClick),
                       Name.PointerRightClick  => SignalProvider.Local.Pointer.GetProviderId(SignalProvider.Local.Pointer.Name.RightClick),
                       Name.Selected           => SignalProvider.Local.UI.GetProviderId(SignalProvider.Local.UI.Name.Selected),
                       Name.Deselected         => SignalProvider.Local.UI.GetProviderId(SignalProvider.Local.UI.Name.Deselected),
                       Name.Submit             => SignalProvider.Local.UI.GetProviderId(SignalProvider.Local.UI.Name.Submit),
                       _                       => throw new ArgumentOutOfRangeException(nameof(behaviourName), behaviourName, null)
                   };
        }
    }
}
