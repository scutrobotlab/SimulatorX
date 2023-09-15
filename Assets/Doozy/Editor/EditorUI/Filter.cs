// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Text.RegularExpressions;
using Doozy.Runtime.Common.Extensions;
using UnityEngine.Events;
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Editor.EditorUI
{
    public class Filter
    {
        private string m_Pattern;
        public string pattern
        {
            get => m_Pattern;
            set
            {
                m_Pattern = value;
                onPatternChanged?.Invoke(value);
            }
        }
        
        public bool isActive => !m_Pattern.IsNullOrEmpty();

        public UnityAction<string> onPatternChanged { get; set; }
        

        public Filter SetPattern(string value)
        {
            pattern = value;
            return this;
        }

        public Filter Clear()
        {
            pattern = string.Empty;
            return this;
        }

        public Filter SetOnPatternChanged(UnityAction<string> callback)
        {
            onPatternChanged += callback;
            return this;
        }

        public bool IsMatch(string input) =>
            isActive && IsMatch(input, pattern);

        private static bool IsMatch(string input, string pattern) =>
            Regex.IsMatch(input, pattern, RegexOptions.IgnoreCase);
    }
}
