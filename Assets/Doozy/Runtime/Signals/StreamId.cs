// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Runtime.Common;

namespace Doozy.Runtime.Signals
{
    [Serializable]
    public partial class StreamId : CategoryNameId
    {
        public StreamId() {}
        public StreamId(string category, string name, bool custom = false) : base(category, name, custom) {}
        
        public void SetStream(SignalStream stream)
        {
            if (stream == null) return;
            Category = stream.category;
            Name = stream.name;
            Custom = true;
        }
    }
}
