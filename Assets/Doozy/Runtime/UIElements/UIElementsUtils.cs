// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using System.Linq;
using Doozy.Runtime.Common.Extensions;
using Doozy.Runtime.UIElements.Extensions;
using UnityEngine.UIElements;

namespace Doozy.Runtime.UIElements
{
    public static class UIElementsUtils
    {
        public static void AddClass(string className, IEnumerable<VisualElement> elements, bool removeNulls = true)
        {
            if (className.IsNullOrEmpty())
                return;

            if (elements == null)
                return;

            if (removeNulls)
                elements = elements.Where(item => item != null);

            foreach (VisualElement element in elements)
                element.AddClass(className);
        }

        public static void RemoveClass(string className, IEnumerable<VisualElement> elements, bool removeNulls = true)
        {
            if (className.IsNullOrEmpty())
                return;

            if (elements == null)
                return;

            if (removeNulls)
                elements = elements.Where(item => item != null);

            foreach (VisualElement element in elements)
                element.RemoveClass(className);
        }
    }
}
