// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.Components.Internal;
using Doozy.Editor.EditorUI.ScriptableObjects.Colors;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Runtime.UIElements.Extensions;
using UnityEngine;
using UnityEngine.UIElements;
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Editor.Reactor.Components
{
    public class ColorAnimationTab : PoolableElement<ColorAnimationTab>
    {
        public const float k_TabMinWidth = 68f;
        public const float k_ColorReferenceWidth = 60f;

        public override void Dispose()
        {
            base.Dispose();
            tabButton?.Recycle();
        }

        public override void Reset()
        {
            SetTabAccentColor(EditorSelectableColors.Default.ButtonIcon);
            SetReferenceColor(Color.clear);

            tabButton
                .Reset();

            tabButton
                .SetContainerColorOff(DesignUtils.tabButtonColorOff)
                .SetTabPosition(TabPosition.TabOnBottom)
                .SetElementSize(ElementSize.Small);
        }

        public VisualElement referenceColorElement { get; }
        public FluidToggleButtonTab tabButton { get; }

        public Color referenceColor { get; private set; }

        public ColorAnimationTab()
        {
            tabButton = DesignUtils.GetTabButtonForComponentSection();
            referenceColorElement = GetReferenceColorElement();

            this
                .SetStyleFlexGrow(0)
                .SetStyleMinWidth(k_TabMinWidth)
                .AddChild(tabButton)
                .AddChild(referenceColorElement);
        }

        public ColorAnimationTab SetTabAccentColor(EditorSelectableColorInfo selectableAccentColor)
        {
            tabButton.SetToggleAccentColor(selectableAccentColor);
            return this;
        }

        public ColorAnimationTab SetReferenceColor(Color color)
        {
            referenceColor = color;
            referenceColorElement.SetStyleBackgroundColor(referenceColor);
            return this;
        }
        
        public static VisualElement GetReferenceColorElement() =>
            new VisualElement()
                .SetStyleHeight(6)
                .SetStyleWidth(k_ColorReferenceWidth)
                .SetStyleAlignSelf(Align.Center)
                .SetStyleBorderRadius(3)
                .SetStyleMarginTop(DesignUtils.k_Spacing);
    }
}
