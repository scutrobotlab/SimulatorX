// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Runtime.Common.Extensions;
// ReSharper disable NonReadonlyMemberInGetHashCode

namespace Doozy.Runtime.Signals
{
    [Serializable]
    public struct ProviderId : IEquatable<ProviderId>
    {
        public ProviderType Type;
        public string Category;
        public string Name;

        public ProviderId(ProviderType providerType, string providerCategory, string providerName)
        {
            Type = providerType;
            Category = providerCategory.RemoveWhitespaces().RemoveAllSpecialCharacters();
            Name = providerName.RemoveWhitespaces().RemoveAllSpecialCharacters();
        }

        public override string ToString() => $"{Type} {Category}.{Name}";
        public static bool operator==(ProviderId a, ProviderId b) => a.Equals(b);
        public static bool operator!=(ProviderId a, ProviderId b) => !(a == b);
        public bool Equals(ProviderId other) => Type == other.Type && Category == other.Category && Name == other.Name;
        public override bool Equals(object obj) => obj is ProviderId other && Equals(other);
        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (int)Type;
                hashCode = (hashCode * 397) ^ (Category != null ? Category.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Name != null ? Name.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}
