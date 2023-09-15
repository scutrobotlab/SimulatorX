// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Editor.EditorUI.Windows.Internal;
using Doozy.Editor.Signals.Layouts;
using Doozy.Runtime.UIElements.Extensions;
using UnityEditor;
using UnityEngine;

namespace Doozy.Editor.Signals.Windows
{
    public class StreamsDatabaseWindow : FluidWindow<StreamsDatabaseWindow>
    {
        private const string WINDOW_TITLE = "Streams Database";
        
        [MenuItem(SignalsWindow.k_WindowMenuPath + WINDOW_TITLE, priority = -700)]
        public static void Open() => InternalOpenWindow(WINDOW_TITLE);

        protected override void OnEnable()
        {
            base.OnEnable();
            minSize = new Vector2(500, 500);
        }

        protected override void CreateGUI() =>
            root.AddChild(windowLayout = Activator.CreateInstance<StreamsDatabaseWindowLayout>());

        protected override void OnDestroy()
        {
            base.OnDestroy(); 
            ((StreamsDatabaseWindowLayout) windowLayout).OnDestroy();
        }
    }
}
