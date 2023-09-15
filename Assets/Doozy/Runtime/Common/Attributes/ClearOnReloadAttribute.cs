// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable ClassNeverInstantiated.Global

namespace Doozy.Runtime.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Event | AttributeTargets.Field | AttributeTargets.Property)]
    public class ClearOnReloadAttribute : Attribute
    {
        public readonly object ValueOnReload;
        public readonly bool CreateNewInstance;

        /// <summary> On reload, clear event, field or property </summary>
        public ClearOnReloadAttribute()
        {
            ValueOnReload = null;
            CreateNewInstance = false;
        }

        /// <summary> On reload, set the given value to a target field or property (does not work for events) </summary>
        /// <param name="resetValue"> Value set to target field or property. Value type needs to match the field/property type. Does not work on events. </param>
        public ClearOnReloadAttribute(object resetValue)
        {
            ValueOnReload = resetValue;
            CreateNewInstance = false;
        }

        /// <summary> On reload, clear or re-initialize target field or property </summary>
        /// <param name="newInstance"> Create a new instance of the target field or property, if TRUE. Does not work on events. </param>
        public ClearOnReloadAttribute(bool newInstance)
        {
            ValueOnReload = null;
            CreateNewInstance = newInstance;
        }
    }
}
