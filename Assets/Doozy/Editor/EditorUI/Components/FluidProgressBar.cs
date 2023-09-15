// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Editor.Reactor.Internal;
using Doozy.Runtime.Reactor.Internal;
using Doozy.Runtime.Reactor.Reactions;
using Doozy.Runtime.UIElements.Extensions;
using UnityEngine;
using UnityEngine.UIElements;
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Editor.EditorUI.Components
{
    public class FluidProgressBar : PoolableElement<FluidProgressBar>
    {
        public override void Reset()
        {
            SetBackgroundColor(EditorColors.Default.Background);
            SetIndicatorColor(EditorColors.Default.Action);
            this.SetStyleHeight(1);

            reaction?.Stop();
        }

        public override void Dispose()
        {
            base.Dispose();
            reaction?.Recycle();
        }

        public FloatReaction reaction { get; }
        public VisualElement indicator { get; }

        public Color backgroundColor { get; private set; }
        public Color indicatorColor { get; private set; }

        public FluidProgressBar()
        {
            indicator = new VisualElement().SetStyleFlexGrow(1);
            indicator.visible = false;

            this.SetStyleFlexGrow(1)
                .SetStyleHeight(2)
                .SetStyleOverflow(Overflow.Hidden)
                .AddChild(indicator);

            SetBackgroundColor(EditorColors.Default.Placeholder);
            SetIndicatorColor(EditorColors.Default.Action);

            reaction = Reaction.Get<FloatReaction>().SetEditorHeartbeat()
                .SetSetter(value =>
                {
                    indicator.visible = value > 0.01f;
                    indicator.SetStyleLeft((1f - value) * -resolvedStyle.width);
                });

            schedule.Execute(() =>
            {
                reaction.SetProgressAtZero();
                indicator.visible = true;
            });
        }

        public FluidProgressBar Play(bool inReverse = false)
        {
            reaction?.Play(inReverse);
            return this;
        }

        public FluidProgressBar Stop()
        {
            reaction?.Stop();
            return this;
        }

        public FluidProgressBar SetAtZero()
        {
            reaction?.SetProgressAtZero();
            return this;
        }

        public FluidProgressBar SetAtOne()
        {
            reaction?.SetProgressAtOne();
            return this;
        }

        public FluidProgressBar SetBackgroundColor(Color color)
        {
            backgroundColor = color;
            this.SetStyleBackgroundColor(color);
            return this;
        }

        public FluidProgressBar SetIndicatorColor(Color color)
        {
            indicatorColor = color;
            indicator.SetStyleBackgroundColor(color);
            return this;
        }

        public FluidProgressBar SetDuration(float duration)
        {
            reaction?.SetDuration(duration);
            return this;
        }

    }
}
