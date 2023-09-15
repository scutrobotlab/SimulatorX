// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Editor.Common.Extensions;
using Doozy.Editor.EditorUI.Components;
using Doozy.Runtime.Mody.Actions;
using Doozy.Runtime.UIElements.Extensions;
using UnityEditor;
using UnityEngine.UIElements;

namespace Doozy.Editor.Mody.Drawers.ModyActions
{
    [CustomPropertyDrawer(typeof(FloatModyAction))]
    public class FloatModyActionDrawer : BaseModyActionDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var modyAction = property.GetTargetObjectOfProperty() as FloatModyAction;
            
            VisualElement drawer = new VisualElement().SetStyleOverflow(Overflow.Hidden);
            FluidComponentHeader header = NewActionHeader(modyAction);
            FluidAnimatedContainer animatedContainer = new FluidAnimatedContainer().SetClearOnHide(true).Hide(false);
            FluidToggleIconButton expandCollapseButton = NewExpandCollapseActionButton(header, animatedContainer);
            ConnectHeaderToExpandCollapseButton(header, expandCollapseButton);
            FluidToggleSwitch disableSwitch = NewDisableActionSwitch(property);

            drawer
                .AddChild(GetDrawerHeader(expandCollapseButton, header, disableSwitch))
                .AddChild(animatedContainer);

            animatedContainer.OnShowCallback += () =>
            {
                animatedContainer.fluidContainer.SetStylePadding(animatedContainerFluidContainerPadding);
                animatedContainer.AddContent(AnimatedContainerContent(property));
            };

            return drawer;
        }
    }
}
