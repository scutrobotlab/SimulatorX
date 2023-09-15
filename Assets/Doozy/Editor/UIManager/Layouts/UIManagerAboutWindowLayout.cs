// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using Doozy.Editor.Common;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.ScriptableObjects.Colors;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Editor.Reactor.Internal;
using Doozy.Runtime.Reactor.Extensions;
using Doozy.Runtime.Reactor.Internal;
using Doozy.Runtime.Reactor.Reactions;
using Doozy.Runtime.UIElements.Extensions;
using UnityEngine;
using UnityEngine.UIElements;

namespace Doozy.Editor.UIManager.Layouts
{
    public sealed class UIManagerAboutWindowLayout : FluidWindowLayout, IUIManagerWindowLayout
    {
        public int order => 1000;

        public override string layoutName => "About";
        public override List<Texture2D> animatedIconTextures => EditorMicroAnimations.EditorUI.Icons.Info;

        public override Color accentColor => EditorColors.Default.UnityThemeInversed;
        public override EditorSelectableColorInfo selectableAccentColor => EditorSelectableColors.Default.UnityThemeInversed;

        private Image doozyUiIconImage { get; set; }
        private Texture2DReaction doozyUiIconReaction { get; set; }
        private Label copyrightLabel { get; set; }
        private Label doozyUiVersionLabel { get; set; }

        private ProductInfo m_DoozyProductInfo;
        private ProductInfo doozyProductInfo =>
            m_DoozyProductInfo ? m_DoozyProductInfo : m_DoozyProductInfo = ProductInfo.Get("Doozy UI Manager");
        
        public UIManagerAboutWindowLayout()
        {
            AddHeader("About", "Hello World", animatedIconTextures);
            content.ResetLayout();
            sideMenu.Dispose();
            Initialize();
            Compose();
        }

        private void Initialize()
        {
            doozyUiIconImage =
                new Image()
                    .ResetLayout()
                    .SetStyleSize(128);

            doozyUiIconReaction =
                doozyUiIconImage.GetTexture2DReaction().SetEditorHeartbeat()
                    .SetDuration(0.6f)
                    .SetTextures(EditorMicroAnimations.UIManager.Icons.DoozyUI);

            doozyUiVersionLabel =
                DesignUtils.fieldLabel
                    .SetText(doozyProductInfo.nameAndVersion)
                    .SetStyleTextAlign(TextAnchor.MiddleCenter)
                    .SetStyleFontSize(11);

            copyrightLabel =
                DesignUtils.fieldLabel
                    .SetText("Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.");

            doozyUiIconImage.RegisterCallback<PointerEnterEvent>(evt => doozyUiIconReaction?.Play());
            doozyUiIconImage.AddManipulator(new Clickable(() => doozyUiIconReaction?.Play()));
        }

        private void Compose()
        {
            content
                .AddChild
                (
                    DesignUtils.column
                        .SetStyleJustifyContent(Justify.Center)
                        .SetStyleAlignItems(Align.Center)
                        .AddChild(doozyUiIconImage)
                        .AddChild(DesignUtils.spaceBlock2X)
                        .AddChild(doozyUiVersionLabel)
                );

            footerLabel
                .SetStyleDisplay(DisplayStyle.None);

            footer
                .SetStyleDisplay(DisplayStyle.Flex)
                .SetStyleHeight(32)
                .AddChild
                (
                    DesignUtils.row
                        .SetStyleBackgroundColor(EditorColors.Default.WindowHeaderBackground)
                        .SetStyleAlignItems(Align.Center)
                        .SetStylePadding(DesignUtils.k_Spacing2X)
                        .AddChild(DesignUtils.flexibleSpace)
                        .AddChild(copyrightLabel)
                        .AddChild(DesignUtils.flexibleSpace)
                );

        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            doozyUiIconReaction?.Recycle();
        }

    }
}
