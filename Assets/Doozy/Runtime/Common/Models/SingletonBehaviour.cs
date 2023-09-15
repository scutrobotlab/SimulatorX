// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Runtime.Common.Attributes;
using UnityEngine;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable CheckNamespace

namespace Doozy.Runtime.Common
{
    public abstract class SingletonBehaviour<T> : MonoBehaviour where T : MonoBehaviour
    {
        // ReSharper disable once StaticMemberInGenericType
        /// <summary> Internal variable used as a flag when the application is quitting </summary>
        [ClearOnReload(resetValue: false)]
        protected static bool applicationIsQuitting { get; set; }

        [ClearOnReload]
        private static T s_instance;

        public static T instance
        {
            get
            {
                // Debug.Log($"Instance");
                if (applicationIsQuitting) return null;
                if (s_instance != null) return s_instance;
                s_instance = FindObjectOfType<T>();
                if (s_instance != null) return s_instance;
                // DontDestroyOnLoad(SceneUtils.AddToScene<T>(typeof(T).Name, true).gameObject);
                s_instance = new GameObject(typeof(T).Name).AddComponent<T>();
                return s_instance;
            }
        }

        // [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        // private static void RunOnStart() =>
        // 	Application.quitting += () => applicationIsQuitting = false;

        protected virtual void OnApplicationQuit() =>
            applicationIsQuitting = true;

        protected virtual void OnDestroy() =>
            applicationIsQuitting = true;

        protected virtual void Awake()
        {
            if (s_instance != null && s_instance != this)
            {
                Debug.Log($"There cannot be two '{typeof(T).Name}' active at the same time. Destroying the '{gameObject.name}' GameObject!");
                Destroy(gameObject);
                return;
            }

            // s_instance = FindObjectOfType<T>();
            s_instance = GetComponent<T>();
            DontDestroyOnLoad(gameObject);
        }
    }
}
