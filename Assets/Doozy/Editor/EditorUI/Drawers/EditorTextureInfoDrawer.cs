// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Linq;
using Doozy.Editor.Common.Extensions;
using Doozy.Editor.EditorUI.ScriptableObjects.Textures;
using Doozy.Runtime.Colors;
using Doozy.Runtime.UIElements.Extensions;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Doozy.Editor.EditorUI.Drawers
{
    [CustomPropertyDrawer(typeof(EditorTextureInfo))]
    public class EditorTextureInfoDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {}

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var target = property.GetTargetObjectOfProperty() as EditorTextureInfo;

            SerializedProperty referenceProperty = property.FindPropertyRelative("TextureReference");

            TemplateContainer drawer = EditorLayouts.EditorUI.EditorTextureInfo.CloneTree();
            drawer.AddStyle(EditorStyles.EditorUI.EditorTextureInfo);

            drawer.Q<VisualElement>("LayoutContainer")
                .SetStyleBackgroundColor(EditorColors.Default.FieldBackground)
                .SetStyleBorderColor(EditorColors.Default.Selection);

            drawer.Q<Image>("PreviewImageContainer")
                .SetStyleBackgroundImage(EditorTextures.EditorUI.Placeholders.TransparencyGrid)
                .SetStyleBackgroundImageTintColor(Color.gray.WithAlpha(0.5f));

            Image previewImage = drawer.Q<Image>("PreviewImage");

            VisualElement textureNameContainer = drawer.Q<VisualElement>("TextureNameContainer");
            
            Label pathLabel = drawer.Q<Label>("Path")
                .SetStyleColor(EditorColors.Default.TextDescription)
                .SetStyleUnityFont(EditorFonts.Inter.Regular);

            Label textureSizeLabel = drawer.Q<Label>("TextureSize")
                .SetStyleColor(EditorColors.Default.TextDescription)
                .SetStyleUnityFont(EditorFonts.Inter.Light);

            ObjectField textureObjectField = drawer.Q<ObjectField>("TextureReference");
            textureObjectField.objectType = typeof(Texture);
            textureObjectField.RegisterValueChangedCallback(OnValueChanged);

            UpdateDrawer((Texture2D)referenceProperty?.objectReferenceValue);

            drawer.RegisterCallback<AttachToPanelEvent>(evt =>
            {
                drawer.RegisterCallback<DragUpdatedEvent>(OnDragUpdate);
                drawer.RegisterCallback<DragPerformEvent>(OnDragPerformEvent);
            });

            drawer.RegisterCallback<DetachFromPanelEvent>(evy =>
            {
                drawer.UnregisterCallback<DragUpdatedEvent>(OnDragUpdate);
                drawer.UnregisterCallback<DragPerformEvent>(OnDragPerformEvent);
            });

            return drawer;

            void OnValueChanged(ChangeEvent<Object> evt = null) =>
                UpdateDrawer((Texture2D)evt?.newValue);

            void UpdateDrawer(Texture2D texture)
            {
                bool hasReference = texture != null;

                previewImage.SetStyleBackgroundImage(texture);
                
                DisplayStyle displayStyle = hasReference ? DisplayStyle.Flex : DisplayStyle.None;
                textureNameContainer.SetStyleDisplay(displayStyle);
                textureSizeLabel.SetStyleDisplay(displayStyle);
                pathLabel.SetStyleDisplay(displayStyle);

                if (!hasReference)
                    return;

                textureSizeLabel.SetText($"W: {texture.width}px\nH: {texture.height}px");
                pathLabel.SetText($"{AssetDatabase.GetAssetPath(texture)}");
            }

            void OnDragUpdate(DragUpdatedEvent evt)
            {
                bool isDraggingTexture = DragAndDrop.objectReferences.Any(item => item is Texture);
                if (!isDraggingTexture) return;
                DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
            }

            void OnDragPerformEvent(DragPerformEvent evt)
            {
                var texture = DragAndDrop.objectReferences.First(item => item != null && item is Texture) as Texture;
                if (referenceProperty == null) return;
                referenceProperty.objectReferenceValue = texture;
                referenceProperty.serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
