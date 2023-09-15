// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using System.Linq;
using Doozy.Editor.EditorUI.Components;
using Doozy.Runtime.Common.Extensions;
using Doozy.Runtime.Common.Utils;
using Doozy.Runtime.UIElements.Extensions;
using UnityEditor;
using UnityEngine.UIElements;

namespace Doozy.Editor.EditorUI.Windows.Internal
{
    public class EditorUIDatabaseWindow<T> : FluidWindow<T> where T : EditorWindow
    {
        protected override void CreateGUI()
        {
            windowLayout = GetFluidWindowLayout($"{GetType().Name}");
            if (windowLayout == null)
                return;
            windowLayout.SetStyleFlexGrow(1);
            root.AddChild(windowLayout);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            ((FluidWindowLayout) windowLayout).Dispose();
        }

        private static VisualElement GetFluidWindowLayout(string layoutName)
        {
            if (layoutName.IsNullOrEmpty()) return null;
            IEnumerable<Type> results = ReflectionUtils.GetTypesThatImplementInterface<IEditorUIDatabaseWindowLayout>();
            var layoutTypes = results.Where(result => result.Name.Contains(layoutName)).ToList();
            if (!layoutTypes.Any()) return null;
            return (VisualElement)Activator.CreateInstance(layoutTypes[0]);
        }
    }
}
