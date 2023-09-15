// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using System.Linq;
using Doozy.Runtime.UIManager.Components;
using UnityEngine;

namespace Doozy.Runtime.UIManager
{
    [Serializable]
    public class UIBehaviours
    {
        [SerializeField] private List<UIBehaviour> Behaviours;
        public List<UIBehaviour> behaviours => Behaviours;

        [SerializeField] private GameObject SignalSource;
        public GameObject signalSource => SignalSource;

        [SerializeField] private UISelectable Selectable;
        public UISelectable selectable => Selectable;

        public UIBehaviours() : this(null) {}

        public UIBehaviours(GameObject signalSource)
        {
            SignalSource = signalSource;
            Behaviours = new List<UIBehaviour>();
        }

        public UIBehaviours Connect()
        {
            if (signalSource == null) return this;
            foreach (UIBehaviour behaviour in Behaviours)
                behaviour?
                    .SetSelectable(selectable)
                    .SetSignalSource(signalSource)
                    .Connect();
            return this;
        }

        public UIBehaviours Disconnect()
        {
            foreach (UIBehaviour behaviour in Behaviours)
                behaviour?.Disconnect();
            return this;
        }

        public UIBehaviour AddBehaviour(UIBehaviour.Name behaviourName)
        {
            if (HasBehaviour(behaviourName))
                return GetBehaviour(behaviourName);

            UIBehaviour newBehaviour =
                new UIBehaviour(behaviourName, signalSource)
                    .SetSelectable(selectable);

            Behaviours.Add(newBehaviour);

            var temp = (from UIBehaviour.Name name in Enum.GetValues(typeof(UIBehaviour.Name)) select GetBehaviour(name) into b where b != null select b).ToList();
            Behaviours.Clear();
            Behaviours.AddRange(temp);

            return newBehaviour;
        }

        public void RemoveBehaviour(UIBehaviour.Name behaviourName)
        {
            UIBehaviour behaviour = GetBehaviour(behaviourName);
            if (behaviour == null) return;
            behaviour.Disconnect();
            Behaviours.Remove(behaviour);
        }

        public bool HasBehaviour(UIBehaviour.Name behaviourName) =>
            Behaviours.Any(b => b.behaviourName == behaviourName);

        public UIBehaviour GetBehaviour(UIBehaviour.Name behaviourName) =>
            Behaviours.FirstOrDefault(b => b.behaviourName == behaviourName);

        public UIBehaviours SetSignalSource(GameObject target)
        {
            SignalSource = target;
            foreach (UIBehaviour behaviour in Behaviours)
                behaviour.SetSignalSource(target);
            return this;
        }

        public UIBehaviours SetSelectable(UISelectable uiSelectable)
        {
            Selectable = uiSelectable;
            foreach (UIBehaviour behaviour in behaviours)
                behaviour.SetSelectable(selectable);
            return this;
        }

        public UIBehaviours ClearSelectable() =>
            SetSelectable(null);
    }
}
