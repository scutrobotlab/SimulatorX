// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Runtime.Mody;
using Doozy.Runtime.UIElements.Extensions;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Editor.Mody.Components
{
    public class ModyModuleComponentHeader : FluidComponentHeader
    {
        public readonly ModyModuleStateIndicator StateIndicator;

        public ModyModuleComponentHeader(string moduleName, Texture2D moduleIcon, ModyModule module, SerializedObject serializedObject) : base()
        {
            iconContainer
                .SetStyleBackgroundImageTintColor(EditorColors.Default.Background);

            Image moduleIconImage = new Image()
                .SetStyleBackgroundImage(EditorTextures.Mody.Icons.ModyModule)
                .SetStyleBackgroundImageTintColor(EditorColors.Mody.Module)
                .SetStyleFlexGrow(1)
                .SetStyleSize(32);

            iconContainer.Add(moduleIconImage);

            StateIndicator = new ModyModuleStateIndicator();
            module.UpdateState();
            StateIndicator.UpdateState(module.state);

            EnumField invisibleStateEnum = new EnumField { bindingPath = "ModuleCurrentState" }.SetStyleDisplay(DisplayStyle.None);
            Add(invisibleStateEnum);
            invisibleStateEnum.RegisterValueChangedCallback(evt =>
            {
                StateIndicator.UpdateState((ModuleState)evt.newValue);
            });

            if (EditorApplication.isPlayingOrWillChangePlaymode)
                barRightContainer.Add(StateIndicator);
        }
    }
}
