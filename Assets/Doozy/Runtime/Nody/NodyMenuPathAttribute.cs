// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Doozy.Runtime.Nody
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class NodyMenuPathAttribute : Attribute
    {
        public string category { get; }
        public string name { get; }
        public NodyMenuPathAttribute(string category, string name)
        {
            this.category = category;
            this.name = name;
        }
    }
}
