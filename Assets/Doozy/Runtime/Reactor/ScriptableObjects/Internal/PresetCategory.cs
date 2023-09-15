// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using System.Linq;

namespace Doozy.Runtime.Reactor.ScriptableObjects.Internal
{
    [Serializable]
    public class PresetCategory
    {
        public string Category;
        public List<string> Names;

        public PresetCategory(string category)
        {
            Category = category;
        }
        
        public PresetCategory AddName(string value)
        {
            Names ??= new List<string>();
            Names.Add(value);
            Names = Names.Distinct().ToList();
            Names.Sort();
            return this;
        }
    }
}
