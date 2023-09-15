// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Editor.Reactor.Internal;
using Doozy.Editor.UIElements;
using Doozy.Runtime.Colors;
using Doozy.Runtime.Reactor.Easings;
using Doozy.Runtime.Reactor.Extensions;
using Doozy.Runtime.Reactor.Internal;
using Doozy.Runtime.Reactor.Reactions;
using Doozy.Runtime.UIElements.Extensions;
using UnityEngine;
using UnityEngine.UIElements;
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Editor.EditorUI.Components
{
    public class FluidCircularGauge : PoolableElement<FluidCircularGauge>
    {
        public TemplateContainer templateContainer { get; }
        public VisualElement layoutContainer { get; }
        public Image body { get; }
        public Image fillBackground { get; }
        public Image fill { get; }

        public Texture2DReaction fillReaction { get; }

        public sealed override void Reset()
        {
            body.RecycleAndClear();
            SetAccentColor(EditorColors.EditorUI.Amber);
            fillReaction.SetProgressAtZero();
        }

        public override void Recycle(bool debug = false)
        {
            base.Recycle(debug);
            fillReaction?.Recycle();
        }

        public FluidCircularGauge()
        {
            Add(templateContainer = EditorLayouts.EditorUI.FluidCircularGauge.CloneTree());

            this
                .AddStyle(EditorStyles.EditorUI.FluidCircularGauge);

            layoutContainer = templateContainer.Q<VisualElement>(nameof(layoutContainer));
            body = templateContainer.Q<Image>(nameof(body));
            fillBackground = templateContainer.Q<Image>(nameof(fillBackground));
            fill = templateContainer.Q<Image>(nameof(fill));

            body
                .SetStyleBackgroundImage(EditorTextures.EditorUI.Widgets.CircularGaugeBody)
                .SetStyleBackgroundImageTintColor(EditorColors.Default.BoxBackground);

            fillBackground
                .SetStyleBackgroundImage(EditorTextures.EditorUI.Widgets.CircularGaugeFillBackground);

            fillReaction =
                fill.GetTexture2DReaction()
                    .SetEditorHeartbeat()
                    .SetTextures(EditorMicroAnimations.EditorUI.Widgets.CircularGaugeFillBackground)
                    .SetDuration(0.5f);


            Reset();
        }
        
        public FluidCircularGauge SetAccentColor(Color color) =>
            SetFillBackgroundColor(color.WithAlpha(0.15f))
                .SetFillColor(color);


        public FluidCircularGauge SetBodyColor(Color color)
        {
            body.SetStyleBackgroundImageTintColor(color);
            return this;
        }

        public FluidCircularGauge SetFillBackgroundColor(Color color)
        {
            fillBackground.SetStyleBackgroundImageTintColor(color);
            return this;
        }

        public FluidCircularGauge SetFillColor(Color color)
        {
            fill.SetStyleBackgroundImageTintColor(color);
            return this;
        }

        public FluidCircularGauge SetAnimationDuration(float duration)
        {
            fillReaction.SetDuration(duration);
            return this;
        }

        public FluidCircularGauge SetAnimationEase(Ease ease)
        {
            fillReaction.SetEase(ease);
            return this;
        }

        public FluidCircularGauge PlayToProgress(float progress)
        {
            fillReaction.PlayToProgress(progress);
            return this;
        }

    }
}
