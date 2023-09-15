// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Doozy.Editor.EditorUI.Windows.Internal
{
    public abstract class FluidWindow<T> : EditorWindow where T : EditorWindow
    {
        protected string EditorPrefsKey(string variableName) => $"{GetType().FullName} - {variableName}";

        protected VisualElement root => rootVisualElement;
        protected VisualElement windowLayout { get; set; }

        public static bool isOpen { get; private set; }

        #region Instance

        private static T s_instance;

        public static T instance
        {
            get
            {
                if (s_instance != null) return s_instance;
                s_instance = window;
                if (s_instance != null) return s_instance;
                s_instance = GetWindow<T>();
                return s_instance;
            }
        }

        /*
       * An alternative way to get Window, because
       * GetWindow<T>() forces window to be active and present
       */
        private static T window
        {
            get
            {
                T[] windows = Resources.FindObjectsOfTypeAll<T>();
                return windows.Length > 0 ? windows[0] : null;
            }
        }

        #endregion

        protected static void InternalOpenWindow(string windowTitle)
        {
            instance.Show();
            instance.titleContent.text = windowTitle;
        }

        protected virtual void OnEnable()
        {
            isOpen = true;
        }

        protected virtual void OnDisable()
        {
            isOpen = false;
        }

        protected virtual void OnDestroy() {}

        protected abstract void CreateGUI();

    }
}
