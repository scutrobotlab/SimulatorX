// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.ScriptableObjects.Colors;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Runtime.UIElements.Extensions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Editor.Reactor.Components
{
    public class ReactionControls : VisualElement
    {
        public static IEnumerable<Texture2D> firstFrameTextures => EditorMicroAnimations.EditorUI.Icons.FirstFrame;
        public static IEnumerable<Texture2D> playForwardTextures => EditorMicroAnimations.EditorUI.Icons.PlayForward;
        public static IEnumerable<Texture2D> stopTextures => EditorMicroAnimations.EditorUI.Icons.Stop;
        public static IEnumerable<Texture2D> playReverseTextures => EditorMicroAnimations.EditorUI.Icons.PlayReverse;
        public static IEnumerable<Texture2D> reverseTextures => EditorMicroAnimations.EditorUI.Icons.Reverse;
        public static IEnumerable<Texture2D> lastFrameTextures => EditorMicroAnimations.EditorUI.Icons.LastFrame;
        
        public static EditorSelectableColorInfo accentColor => EditorSelectableColors.Reactor.Red;
        
        public TemplateContainer templateContainer { get; }

        public VisualElement layoutContainer { get; }
        public VisualElement content { get; }

        public FluidButton playForwardButton { get; }
        public FluidButton stopButton { get; }
        public FluidButton playReverseButton { get; }
        public FluidButton firstFrameButton { get; }
        public FluidButton lastFrameButton { get; }
        public FluidButton reverseButton { get; }

        public ReactionControls()
        {
            Add(templateContainer = EditorLayouts.Reactor.ReactionControls.CloneTree());
            templateContainer
                .AddStyle(EditorStyles.Reactor.ReactionControls);

            layoutContainer = templateContainer.Q<VisualElement>("LayoutContainer");
            content = layoutContainer.Q<VisualElement>("Content");

            Color backgroundColor = EditorColors.Default.Background;
            content.SetStyleBackgroundColor(backgroundColor);


            content
                .AddChild(firstFrameButton = GetNewButton(firstFrameTextures,  "Start"))
                .AddChild(DesignUtils.spaceBlock2X)
                .AddChild(playForwardButton = GetNewButton(playForwardTextures,  "Play Forward"))
                .AddChild(DesignUtils.spaceBlock)
                .AddChild(stopButton = GetNewButton(stopTextures,  "Stop"))
                .AddChild(DesignUtils.spaceBlock)
                .AddChild(playReverseButton = GetNewButton(playReverseTextures,  "Play in Reverse"))
                .AddChild(DesignUtils.spaceBlock2X)
                .AddChild(reverseButton = GetNewButton(reverseTextures, "Reverse"))
                .AddChild(DesignUtils.spaceBlock2X)
                .AddChild(lastFrameButton = GetNewButton(lastFrameTextures,  "End"));
        }

        public ReactionControls SetFirstFrameButtonCallback(UnityAction callback)
        {
            firstFrameButton.SetOnClick(callback);
            return this;
        }

        public ReactionControls SetPlayForwardButtonCallback(UnityAction callback)
        {
            playForwardButton.SetOnClick(callback);
            return this;
        }

        public ReactionControls SetStopButtonCallback(UnityAction callback)
        {
            stopButton.SetOnClick(callback);
            return this;
        }

        public ReactionControls SetPlayReverseButtonCallback(UnityAction callback)
        {
            playReverseButton.SetOnClick(callback);
            return this;
        }

        public ReactionControls SetReverseButtonCallback(UnityAction callback)
        {
            reverseButton.SetOnClick(callback);
            return this;
        }

        public ReactionControls SetLastFrameButtonCallback(UnityAction callback)
        {
            lastFrameButton.SetOnClick(callback);
            return this;
        }

        private static FluidButton GetNewButton(IEnumerable<Texture2D> textures, string buttonTooltip) =>
            FluidButton.Get()
                .SetIcon(textures)
                .SetAccentColor(accentColor)
                .SetTooltip(buttonTooltip)
                .SetElementSize(ElementSize.Tiny);
    }
}
