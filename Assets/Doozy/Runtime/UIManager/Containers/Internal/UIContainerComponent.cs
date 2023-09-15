// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using Doozy.Runtime.Common.Attributes;
// ReSharper disable MemberCanBeProtected.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Runtime.UIManager.Containers.Internal
{
    public abstract class UIContainerComponent<T> : UIContainer where T : UIContainer
    {
        [ClearOnReload]
        public static HashSet<T> database { get; } = new HashSet<T>();

        public T component { get; private set; }

        protected override void Awake()
        {
            database.Add(component = GetComponent<T>());
            base.Awake();
        }

        protected override void OnEnable()
        {
            CleanDatabase();
            base.OnEnable();
        }

        protected override void OnDestroy()
        {
            database.Remove(component);
            CleanDatabase();
            base.OnDestroy();
        }

        protected static void CleanDatabase() =>
            database.Remove(null);
    }
}
