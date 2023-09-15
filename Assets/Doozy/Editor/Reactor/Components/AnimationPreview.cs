// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Editor.EditorUI;
using Doozy.Editor.Reactor.Internal;
using Doozy.Runtime.Colors;
using Doozy.Runtime.Reactor;
using Doozy.Runtime.Reactor.Easings;
using Doozy.Runtime.Reactor.Internal;
using Doozy.Runtime.Reactor.Reactions;
using Doozy.Runtime.UIElements.Extensions;
using UnityEngine;
using UnityEngine.UIElements;
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Editor.Reactor.Components
{
    public class AnimationPreview : VisualElement
    {
        public TemplateContainer templateContainer { get; }
        public VisualElement layoutContainer { get; }
        public VisualElement easedPointerContainer { get; }
        public Image easedPointer { get; }
        public Label easedPointerLabel { get; }
        public VisualElement progressBarContainer { get; }
        public VisualElement progressBar { get; }
        public VisualElement linearPointerContainer { get; }
        public Image linearPointer { get; }
        public Label linearPointerLabel { get; }

        public FloatReaction easedReaction { get; }
        public FloatReaction linearReaction { get; }

        private float pointerTravelDistance => progressBar.resolvedStyle.width;
        private float easedPointerLeftPosition => 0f;
        private float easedPointerRightPosition => pointerTravelDistance + easedPointerLeftPosition;
        private float linearPointerLeftPosition => 0f;
        private float linearPointerRightPosition => pointerTravelDistance;

        public Color accentColor { get; private set; }
        private float easedPointerLabelColorAlpha => 0.6f;

        public AnimationPreview()
        {
            Add(templateContainer = EditorLayouts.Reactor.AnimationPreview.CloneTree());
            templateContainer
                .AddStyle(EditorStyles.Reactor.AnimationPreview);

            layoutContainer = templateContainer.Q<VisualElement>("LayoutContainer");
            easedPointerContainer = layoutContainer.Q<VisualElement>("EasedPointerContainer");
            easedPointer = easedPointerContainer.Q<Image>("EasedPointer");
            easedPointerLabel = easedPointerContainer.Q<Label>("EasedPointerLabel");
            progressBarContainer = layoutContainer.Q<VisualElement>("ProgressBarContainer");
            progressBar = layoutContainer.Q<VisualElement>("ProgressBar");
            linearPointerContainer = layoutContainer.Q<VisualElement>("LinearPointerContainer");
            linearPointer = linearPointerContainer.Q<Image>("LinearPointer");
            linearPointerLabel = linearPointerContainer.Q<Label>("LinearPointerLabel");

            accentColor = EditorColors.Default.TextTitle;
            Color backgroundColor = EditorColors.Default.Background;
            Color genericColor = EditorColors.Default.Selection;

            //COLORS
            layoutContainer.SetStyleBackgroundColor(backgroundColor);
            easedPointer.SetStyleBackgroundImageTintColor(accentColor);
            progressBar.SetStyleBackgroundColor(genericColor);
            linearPointer.SetStyleBackgroundImageTintColor(genericColor);

            //TEXT COLOR
            easedPointerLabel.SetStyleColor(accentColor.WithAlpha(easedPointerLabelColorAlpha));
            linearPointerLabel.SetStyleColor(genericColor);

            Font font = EditorFonts.Ubuntu.Regular;
            //TEXT FONTS
            easedPointerLabel.SetStyleUnityFont(font);
            linearPointerLabel.SetStyleUnityFont(font);

            //SET POINTER IMAGES
            easedPointer.SetStyleBackgroundImage(EditorTextures.EditorUI.Pointers.PointerDown);
            linearPointer.SetStyleBackgroundImage(EditorTextures.EditorUI.Pointers.PointerUp);

            easedReaction = Reaction.Get<FloatReaction>().SetEditorHeartbeat().SetDuration(1f)
                .SetSetter(value => easedPointer.SetStyleLeft(value));

            linearReaction = Reaction.Get<FloatReaction>().SetEditorHeartbeat().SetDuration(1f)
                .SetSetter(value => linearPointer.SetStyleLeft(value));

            // layoutContainer.RegisterCallback<PointerEnterEvent>(evt => StartStopAnimationPreview());
            // layoutContainer.AddManipulator(new Clickable(StartStopAnimationPreview));

            StopAnimationPreview();
        }


        public void StartStopAnimationPreview()
        {
            if (easedReaction.isActive)
            {
                StopAnimationPreview();
                return;
            }
            StartAnimationPreview();
        }


        public AnimationPreview StartAnimationPreview()
        {
            easedReaction.SetFrom(easedPointerLeftPosition);
            easedReaction.SetTo(easedPointerRightPosition);
            easedReaction.Play();

            linearReaction.SetFrom(linearPointerLeftPosition);
            linearReaction.SetTo(linearPointerRightPosition);
            linearReaction.Play();

            return this;
        }

        public AnimationPreview StopAnimationPreview()
        {
            easedReaction.SetValue(easedPointerLeftPosition);
            linearReaction.SetValue(linearPointerLeftPosition);
            // easedAnimation.StopAnimationAndSetStartValue();
            // linearAnimation.StopAnimationAndSetStartValue();
            return this;
        }

        public AnimationPreview SetAccentColor(Color color)
        {
            accentColor = color;
            easedPointer.SetStyleBackgroundImageTintColor(accentColor);
            easedPointerLabel.SetStyleColor(accentColor.WithAlpha(easedPointerLabelColorAlpha));
            return this;
        }

        public void ApplyReactionSettings(ReactionSettings settings)
        {
            easedReaction.SetReactionSettings(new ReactionSettings(settings));
            easedPointerLabel.SetText($"{(settings.easeMode == EaseMode.Ease ? easedReaction.settings.ease.ToString() : "Animation Curve")}");

            linearReaction.SetReactionSettings(new ReactionSettings(settings));
            linearReaction.SetEase(Ease.Linear);
            linearPointerLabel.SetText($"{(settings.easeMode == EaseMode.Ease ? linearReaction.settings.ease.ToString() : "Animation Curve")}");
        }
    }
}
