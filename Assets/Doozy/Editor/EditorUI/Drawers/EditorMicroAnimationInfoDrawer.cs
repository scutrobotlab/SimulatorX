// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Editor.Common.Extensions;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.ScriptableObjects.MicroAnimations;
using Doozy.Editor.Reactor.Internal;
using Doozy.Runtime.Colors;
using Doozy.Runtime.Common.Extensions;
using Doozy.Runtime.Reactor.Extensions;
using Doozy.Runtime.Reactor.Reactions;
using Doozy.Runtime.UIElements.Extensions;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Color = UnityEngine.Color;

namespace Doozy.Editor.EditorUI.Drawers
{
    [CustomPropertyDrawer(typeof(EditorMicroAnimationInfo))]
    public class EditorMicroAnimationInfoDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {}

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            Texture2DReaction reaction = null;

            var target = property.GetTargetObjectOfProperty() as EditorMicroAnimationInfo;

            SerializedProperty animationNameProperty = property.FindPropertyRelative("AnimationName");
            SerializedProperty texturesProperty = property.FindPropertyRelative("Textures");

            TemplateContainer drawer = EditorLayouts.EditorUI.EditorMicroAnimationInfo.CloneTree();
            drawer.AddStyle(EditorStyles.EditorUI.EditorMicroAnimationInfo);

            drawer.Q<VisualElement>("LayoutContainer")
                .SetStyleBackgroundColor(EditorColors.Default.FieldBackground)
                .SetStyleBorderColor(EditorColors.Default.Selection);

            Image previewImageContainer =
                drawer.Q<Image>("PreviewImageContainer")
                    .SetStyleBackgroundImage(EditorTextures.EditorUI.Placeholders.TransparencyGrid)
                    .SetStyleBackgroundImageTintColor(Color.gray.WithAlpha(0.5f));

            previewImageContainer.RegisterCallback<MouseEnterEvent>(evt =>
            {
                if (reaction == null) return;
                if (reaction.isActive) return;
                reaction.Play();
            });

            previewImageContainer.RegisterCallback<MouseUpEvent>(evt =>
            {
                if (reaction == null) return;

                const int leftMouseButton = 0;
                const int rightMouseButton = 1;

                switch (evt.button)
                {
                    case leftMouseButton:
                        reaction.Play(); //play forward
                        break;
                    case rightMouseButton:
                        reaction.Play(true); //play in reverse
                        break;
                }
            });

            Image previewImage = drawer.Q<Image>("PreviewImage");

            Label pathLabel = drawer.Q<Label>("Path")
                .SetStyleColor(EditorColors.Default.TextDescription)
                .SetStyleUnityFont(EditorFonts.Inter.Regular);

            Label numberOfFramesLabel = drawer.Q<Label>("NumberOfFrames")
                .SetStyleColor(EditorColors.Default.TextDescription)
                .SetStyleUnityFont(EditorFonts.Inter.Light);

            Label textureSizeLabel = drawer.Q<Label>("TextureSize")
                .SetStyleColor(EditorColors.Default.TextDescription)
                .SetStyleUnityFont(EditorFonts.Inter.Light);

            int frameCount = texturesProperty.arraySize;
            Texture2D firstTexture = frameCount > 0
                ? (Texture2D)texturesProperty.GetArrayElementAtIndex(0).objectReferenceValue
                : null;

            int playerSpacing = 2;

            VisualElement controlsLine = new VisualElement().SetStyleFlexDirection(FlexDirection.Row).SetStylePadding(playerSpacing);
            VisualElement player = drawer.Q<VisualElement>("Player")
                .AddChild(controlsLine);

            void UpdateFramesLabel()
            {
                numberOfFramesLabel.text =
                    reaction == null
                        ? "-/- Frames"
                        : $"{reaction.currentFrame + 1}/{reaction.numberOfFrames} Frames";
            }

            Slider playerSlider =
                new Slider(0f, 1f)
                    .SetStyleFlexGrow(1)
                    .SetStyleMargins(playerSpacing)
                    .SetStylePadding(0);

            playerSlider.RegisterValueChangedCallback(evt =>
            {
                if (reaction == null) return;
                if (reaction.isActive) return;
                reaction.SetFrameAtProgress(Mathf.Clamp01(evt.newValue));
                UpdateFramesLabel();
            });

            playerSlider.RegisterCallback<MouseDownEvent>(evt =>
            {
                if (reaction == null) return;
                if (!reaction.isActive) return;
                reaction.Stop();
            });

            UpdateDrawer(firstTexture, frameCount);

            FluidButton playForwardButton =
                FluidButton.Get()
                    .SetIcon(EditorMicroAnimations.EditorUI.Icons.PlayForward)
                    .SetAccentColor(EditorSelectableColors.EditorUI.LightGreen)
                    .SetTooltip("Play Forward")
                    .SetElementSize(ElementSize.Small)
                    .SetButtonStyle(ButtonStyle.Clear)
                    .SetOnClick(() => reaction.Play());

            FluidButton playReversedButton =
                FluidButton.Get()
                    .SetIcon(EditorMicroAnimations.EditorUI.Icons.PlayReverse)
                    .SetAccentColor(EditorSelectableColors.EditorUI.LightGreen)
                    .SetTooltip("Play Reversed")
                    .SetElementSize(ElementSize.Small)
                    .SetButtonStyle(ButtonStyle.Clear)
                    .SetOnClick(() => reaction.Play(true)); //play in reverse

            controlsLine
                .AddChild(playForwardButton)
                .AddChild(playerSlider.SetStyleMarginLeft(playerSpacing))
                .AddChild(playReversedButton);

            // animation?.StartAnimationForward();

            return drawer;

            void UpdateDrawer(Texture2D texture, int numberOfFrames = 0)
            {
                bool hasReference = texture != null;
                DisplayStyle displayStyle = hasReference ? DisplayStyle.Flex : DisplayStyle.None;

                previewImage.SetStyleBackgroundImage(texture);
                pathLabel.SetStyleDisplay(displayStyle);
                numberOfFramesLabel.SetStyleDisplay(displayStyle);
                textureSizeLabel.SetStyleDisplay(displayStyle);

                if (!hasReference)
                    return;

                numberOfFramesLabel.SetText($"{numberOfFrames} Frames");
                textureSizeLabel.SetText($"W: {texture.width}px\nH: {texture.height}px");

                string assetPath = AssetDatabase.GetAssetPath(texture);
                string[] splitPath = assetPath.Split('/');
                assetPath = string.Empty;
                for (int i = 0; i < splitPath.Length - 1; i++)
                {
                    bool lastItem = i == splitPath.Length - 2;
                    assetPath += $"{splitPath[i]}/";
                }
                assetPath = assetPath.RemoveLastCharacter();

                pathLabel.SetText($"{assetPath}");

                reaction = GetAnimation(previewImage, target);

                void UpdateSliderValue()
                {
                    playerSlider.value = reaction.progress;
                    playerSlider.MarkDirtyRepaint();
                }

                reaction.OnPlayCallback += () =>
                {
                    UpdateSliderValue();
                    UpdateFramesLabel();
                };

                reaction.OnStopCallback += () =>
                {
                    UpdateSliderValue();
                    UpdateFramesLabel();
                };

                reaction.OnUpdateCallback += () =>
                {
                    UpdateFramesLabel();
                    if (!reaction.isActive)
                        return;
                    UpdateSliderValue();
                };
            }
        }

        private static Texture2DReaction GetAnimation(Image previewImage, EditorMicroAnimationInfo editorMicroAnimationInfo) =>
            previewImage.GetTexture2DReaction(editorMicroAnimationInfo.Textures).SetEditorHeartbeat();

        // private static EditorFrameAnimation GetAnimation(Image previewImage, EditorMicroAnimationInfo editorMicroAnimationInfo) =>
        //     new EditorFrameAnimation(previewImage, editorMicroAnimationInfo.Textures);

    }
}
