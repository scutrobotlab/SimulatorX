// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Editor.Mody.Drawers;
using Doozy.Runtime.Mody;
using Doozy.Runtime.UIElements.Extensions;
using Doozy.Runtime.UIManager;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Doozy.Editor.UIManager.Drawers
{
    [CustomPropertyDrawer(typeof(UISelectableState), true)]
    public class UISelectableStateDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {}

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            SerializedProperty stateTypeProperty = property.FindPropertyRelative("StateType");
            SerializedProperty stateNameProperty = property.FindPropertyRelative("StateName");
            SerializedProperty stateEventProperty = property.FindPropertyRelative("StateEvent");
            SerializedProperty runnersProperty = stateEventProperty.FindPropertyRelative(nameof(ModyEvent.Runners));
            SerializedProperty eventProperty = stateEventProperty.FindPropertyRelative(nameof(ModyEvent.Event));

            PropertyField eventPropertyField = DesignUtils.NewPropertyField(eventProperty.propertyPath);

            VisualElement drawer = DesignUtils.row;
            FluidFoldout foldout = new FluidFoldout()
                .SetStyleFlexGrow(1)
                .SetElementSize(ElementSize.Normal)
                .SetLabelText(ObjectNames.NicifyVariableName(stateNameProperty.stringValue));

            foldout.animatedContainer.SetClearOnHide(true);

            foldout.animatedContainer.SetOnShowCallback(() =>
            {
                foldout.AddContent(ModyEventDrawer.ActionRunnersListView(runnersProperty));
                foldout.AddContent(DesignUtils.spaceBlock2X);
                foldout.AddContent(eventPropertyField);
                foldout.Bind(property.serializedObject);
            });

            drawer.RegisterCallback<DetachFromPanelEvent>(evt =>
            {
                foldout.Dispose();
            });

            drawer
                // .AddChild(behaviourIcon)
                .AddChild(foldout);

            return drawer;
        }
    }
}
