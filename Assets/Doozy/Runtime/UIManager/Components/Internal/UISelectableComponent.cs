// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections;
using System.Collections.Generic;
using Doozy.Runtime.Common.Attributes;
using UnityEngine;
using UnityEngine.EventSystems;
// ReSharper disable MemberCanBeProtected.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Runtime.UIManager.Components.Internal
{
    public abstract class UISelectableComponent<T> : UISelectable where T : UISelectable
    {
        public static HashSet<T> database { get; private set; } = new HashSet<T>();

        [ExecuteOnReload]
        private static void OnReload()
        {
            database = new HashSet<T>();
        }
        
        public T component { get; private set; }

        [SerializeField] private UIBehaviours Behaviours;
        public UIBehaviours behaviours => Behaviours;

        public bool isSelected => EventSystem.current.currentSelectedGameObject == gameObject;
        
        protected UISelectableComponent()
        {
            Behaviours =
                new UIBehaviours()
                    .SetSelectable(this);
        }

        protected override void Awake()
        {
            database.Add(component = GetComponent<T>());
            base.Awake();
            Behaviours
                .SetSelectable(component)
                .SetSignalSource(gameObject);
        }

        protected override void OnEnable()
        {
            if(!Application.isPlaying)
                return;
            
            CleanDatabase();
            base.OnEnable();
            StartCoroutine(ConnectBehaviours());
        }

        protected override void OnDestroy()
        {
            database.Remove(component);
            CleanDatabase();
            base.OnDestroy();
        }

        protected static void CleanDatabase() =>
            database.Remove(null);
        
        private IEnumerator ConnectBehaviours()
        {
            yield return null;
            behaviours?
                .SetSelectable(component)
                .SetSignalSource(gameObject)
                .Connect();
        }
    }
}
