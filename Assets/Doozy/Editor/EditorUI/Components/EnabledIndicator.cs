// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using Doozy.Editor.Reactor.Internal;
using Doozy.Runtime.Reactor;
using Doozy.Runtime.Reactor.Extensions;
using Doozy.Runtime.Reactor.Internal;
using Doozy.Runtime.Reactor.Reactions;
using Doozy.Runtime.UIElements.Extensions;
using UnityEngine;
using UnityEngine.UIElements;

namespace Doozy.Editor.EditorUI.Components
{
    public class EnabledIndicator : PoolableElement<EnabledIndicator>
    {
        public override void Reset()
        {
            colorReaction?.Stop();
            iconReaction?.Stop();

            this.SetStyleFlexGrow(0);
            SetDisabledColor(defaultDisabledColor);
            SetEnabledColor(defaultEnabledColor);
            ClearIcon();
            SetDisabled(false);
            ResetSize();
            isOn = false;
        }

        public override void Dispose()
        {
            base.Dispose();

            colorReaction?.Recycle();
            iconReaction?.Recycle();
        }

        public static Color defaultDisabledColor => EditorColors.Default.Placeholder;
        public static Color defaultEnabledColor => EditorColors.Default.Action;

        public Color disabledColor { get; private set; }
        public Color enabledColor { get; private set; }

        private ColorReaction m_ColorReaction;
        public ColorReaction colorReaction => m_ColorReaction = m_ColorReaction ?? GetColorReaction();
        public Texture2DReaction iconReaction { get; private set; }

        public bool hasIcon { get; private set; }
        public bool iconIsLooping { get; private set; }
        public bool isOn { get; private set; }

        private ColorReaction GetColorReaction() =>
            Reaction.Get<ColorReaction>().SetEditorHeartbeat().SetSetter(SetColor).SetDuration(0.4f);

        public EnabledIndicator()
        {
            SetDisabledColor(defaultDisabledColor);
            SetEnabledColor(defaultEnabledColor);

            ClearIcon();
            SetDisabled(false);
            ResetSize();
        }

        public EnabledIndicator SetEnabledColor(Color color, bool animateChange = false)
        {
            enabledColor = color;
            if (isOn) SetEnabled(animateChange);
            return this;
        }

        public EnabledIndicator SetDisabledColor(Color color, bool animateChange = false)
        {
            disabledColor = color;
            if (!isOn) SetDisabled(animateChange);
            return this;
        }

        public EnabledIndicator Toggle(bool enabled, bool animateChange)
        {
            if (enabled)
            {
                SetEnabled(animateChange);
                return this;
            }

            SetDisabled(animateChange);
            return this;
        }

        public EnabledIndicator SetEnabled(bool animateChange = true, bool forced = false)
        {
            if (!forced && isOn) return this;

            isOn = true;

            if (animateChange)
            {
                colorReaction.Play(GetColor(), enabledColor);
                iconReaction?.SetLoops(iconIsLooping ? -1 : 0).Play(PlayDirection.Forward);
            }
            else
            {
                SetColor(enabledColor);
                iconReaction?.SetLoops(iconIsLooping ? -1 : 0).SetProgressAtOne();
            }

            return this;
        }

        public EnabledIndicator SetDisabled(bool animateChange = true, bool forced = false)
        {
            if (!forced && !isOn) return this;

            isOn = false;

            if (animateChange)
            {
                colorReaction.Play(GetColor(), disabledColor);
                iconReaction?.SetLoops(0).Play(PlayDirection.Reverse);
            }
            else
            {
                SetColor(disabledColor);
                iconReaction?.SetLoops(0).SetProgressAtZero();
            }

            return this;
        }

        public EnabledIndicator IconIsLooping(bool isLooping)
        {
            iconIsLooping = isLooping;
            return this;
        }

        public EnabledIndicator SetIcon(IEnumerable<Texture2D> textures, bool isLooping = false)
        {
            hasIcon = true;
            if (iconReaction == null)
                iconReaction = this.GetTexture2DReaction(textures).SetEditorHeartbeat().SetDuration(0.6f);
            else
                iconReaction.SetTextures(textures);

            IconIsLooping(isLooping);
            Update();
            return this;
        }

        public EnabledIndicator SetIcon(Texture2D iconTexture2D)
        {
            hasIcon = true;
            iconReaction?.Recycle();
            iconReaction = null;
            this.SetStyleBackgroundImage(iconTexture2D);
            IconIsLooping(false);
            Update();
            return this;
        }

        public EnabledIndicator ClearIcon()
        {
            hasIcon = false;
            iconReaction?.Recycle();
            iconReaction = null;
            this.SetStyleBackgroundImage((Texture2D)null);
            ResetSize();
            IconIsLooping(false);
            Update();
            return this;
        }

        public EnabledIndicator SetSize(int width, int height)
        {
            this.SetStyleSize(width, height);
            return this;
        }

        public EnabledIndicator SetSize(int size)
        {
            this.SetStyleSize(size);
            return this;
        }

        public EnabledIndicator ResetSize()
        {
            this.SetStyleWidth(StyleKeyword.Auto);
            this.SetStyleHeight(1);
            return this;
        }

        private void SetColor(Color color)
        {
            this.SetStyleBackgroundColor(hasIcon ? Color.clear : color);
            this.SetStyleBackgroundImageTintColor(hasIcon ? color : Color.clear);
        }

        private Color GetColor() =>
            hasIcon ? this.GetStyleBackgroundImageTintColor() : this.GetStyleBackgroundColor();

        public void Update()
        {
            SetColor(isOn ? enabledColor : disabledColor);
        }
    }
}
