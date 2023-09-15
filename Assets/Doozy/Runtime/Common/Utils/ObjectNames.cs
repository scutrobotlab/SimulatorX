// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Text.RegularExpressions;
using Doozy.Runtime.Common.Extensions;

namespace Doozy.Runtime.Common.Utils
{
    public static class ObjectNames
    {
        /// <summary> Make a displayable name for a variable </summary>
        /// <param name="name"> Object name </param>
        public static string NicifyVariableName(string name)
        {
            if (name[0] == 'k') name = name.Right(name.Length - 1);
            name = name.Replace("m_", "").Replace("_", " ");
            name = Regex.Replace(name, "[A-Z]", " $0");
            name = name.TrimStart().TrimEnd();
            return name;
        }
    }
}
