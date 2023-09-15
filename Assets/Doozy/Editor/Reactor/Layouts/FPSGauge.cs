// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Editor.Reactor.Internal;
using Doozy.Runtime.Reactor;
using Doozy.Runtime.Reactor.Extensions;
using Doozy.Runtime.Reactor.Internal;
using Doozy.Runtime.Reactor.Reactions;
using Doozy.Runtime.UIElements.Extensions;
using UnityEngine;
using UnityEngine.UIElements;
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Editor.Reactor.Layouts
{
    public class FPSGauge : FluidCircularGauge
    {

        public static Color accentColor => EditorColors.Reactor.Red;

        private Image icon { get; }
        private Texture2DReaction iconReaction { get; }

        private Label titleLabel { get; }
        private Label valueLabel { get; }
        private Label typeLabel { get; }

        public FPSGauge()
        {
            icon =
                new Image()
                    .SetName("Icon")
                    .ResetLayout()
                    .SetStyleSize(64)
                    .SetStyleBackgroundImageTintColor(accentColor);

            iconReaction =
                icon.GetTexture2DReaction().SetEditorHeartbeat()
                    .SetDuration(0.5f)
                    .SetTextures(EditorMicroAnimations.Reactor.Icons.Heartbeat);


            titleLabel = new Label()
                .SetName("Title")
                .SetText("Title")
                .ResetLayout()
                .SetStyleUnityFont(EditorFonts.Ubuntu.Light)
                .SetStyleFontSize(16)
                .SetStyleTextAlign(TextAnchor.MiddleCenter)
                .SetStyleColor(accentColor);

            valueLabel = new Label()
                .SetName("Value")
                .SetText("Value")
                .ResetLayout()
                .SetStyleFontSize(32)
                .SetStyleTextAlign(TextAnchor.MiddleCenter)
                .SetStyleUnityFont(EditorFonts.Ubuntu.Regular)
                .SetStyleColor(EditorColors.Default.TextTitle);

            typeLabel = new Label()
                .SetName("Type")
                .SetText("Type")
                .ResetLayout()
                .SetStyleFontSize(12)
                .SetStyleTextAlign(TextAnchor.MiddleCenter)
                .SetStyleUnityFont(EditorFonts.Ubuntu.Light)
                .SetStyleColor(EditorColors.Default.TextTitle);

            body
                .AddChild
                (
                    DesignUtils.column
                        .SetStyleFlexGrow(1)
                        .SetStyleJustifyContent(Justify.Center)
                        .SetStyleAlignItems(Align.Center)
                        .AddChild(icon)
                        .AddChild(titleLabel.SetStyleMarginTop(-DesignUtils.k_Spacing))
                        .AddChild(valueLabel.SetStyleMarginTop(DesignUtils.k_Spacing3X))
                        .AddChild(typeLabel)
                );

            SetAccentColor(accentColor);

            this.AddManipulator(new Clickable(() => iconReaction.Play()));
        }

        public FPSGauge SetIcon(IEnumerable<Texture2D> textures)
        {
            iconReaction.SetTextures(textures);
            return this;
        }

        public FPSGauge SetTitle(string title)
        {
            titleLabel.SetText(title);
            return this;
        }

        public FPSGauge SetValue(string value)
        {
            valueLabel.SetText(value);
            return this;
        }

        public FPSGauge SetType(string typeName)
        {
            typeLabel.SetText(typeName);
            return this;
        }

        public FPSGauge SetFPS(FPS fps, int customFPS = 1)
        {
            float progress;
            switch (fps)
            {
                case FPS.FPS_120:
                    progress = 120f / 120f;
                    break;
                case FPS.FPS_90:
                    progress = 90f / 120f;
                    break;
                case FPS.FPS_60:
                    progress = 60f / 120f;
                    break;
                case FPS.FPS_30:
                    progress = 30f / 120f;
                    break;
                case FPS.FPS_24:
                    progress = 24f / 120f;
                    break;
                case FPS.CustomFPS:
                    progress = Mathf.Max(customFPS, 1) / 120f;
                    break;
                case FPS.MaxFPS:
                    progress = 1f;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(fps), fps, null);
            }

            PlayToProgress(progress);
            iconReaction.Play();

            switch (fps)
            {
                case FPS.FPS_120:
                    valueLabel.SetText("120");
                    break;
                case FPS.FPS_90:
                    valueLabel.SetText("90");
                    break;
                case FPS.FPS_60:
                    valueLabel.SetText("60");
                    break;
                case FPS.FPS_30:
                    valueLabel.SetText("30");
                    break;
                case FPS.FPS_24:
                    valueLabel.SetText("24");
                    break;
                case FPS.CustomFPS:
                    valueLabel.SetText($"{customFPS}");
                    break;
                case FPS.MaxFPS:
                    valueLabel.SetText("MAX");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(fps), fps, null);
            }

            return this;
        }
    }
}
