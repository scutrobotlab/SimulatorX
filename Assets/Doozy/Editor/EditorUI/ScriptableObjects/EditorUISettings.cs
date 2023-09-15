using System.Collections.Generic;
using System.Linq;
using Doozy.Editor.Common.ScriptableObjects;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.ScriptableObjects.Colors;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Editor.EditorUI.Windows;
using Doozy.Runtime.UIElements.Extensions;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
// ReSharper disable MemberCanBeMadeStatic.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Editor.EditorUI.ScriptableObjects
{
    public class EditorUISettings : SingletonEditorScriptableObject<EditorUISettings>
    {
        public bool AutoRefresh;

        public void Refresh() => 
            EditorUIWindow.Refresh();
    }

    [CustomEditor(typeof(EditorUISettings))]
    public class EditorUISettingsEditor : UnityEditor.Editor
    {
        private static IEnumerable<Texture2D> iconTextures => EditorMicroAnimations.EditorUI.Icons.EditorSettings;
        private static Color accentColor => EditorColors.EditorUI.Amber;
        private static EditorSelectableColorInfo selectableAccentColor => EditorSelectableColors.EditorUI.Amber;

        private VisualElement root { get; set; }
        private FluidComponentHeader componentHeader { get; set; }

        private FluidToggleCheckbox autoRefreshCheckbox { get; set; }
        private FluidButton saveButton { get; set; }

        private SerializedProperty propertyAutoRefresh { get; set; }

        public override VisualElement CreateInspectorGUI()
        {
            InitializeEditor();
            Compose();
            return root;
        }

        private void OnDestroy()
        {
            componentHeader?.Recycle();
            autoRefreshCheckbox?.Recycle();
            saveButton?.Recycle();
        }

        private void FindProperties()
        {
            propertyAutoRefresh = serializedObject.FindProperty("AutoRefresh");
        }

        private void InitializeEditor()
        {
            FindProperties();

            root = new VisualElement();

            componentHeader =
                FluidComponentHeader.Get()
                    .SetElementSize(ElementSize.Large)
                    .SetAccentColor(accentColor)
                    .SetComponentNameText("EditorUI Settings")
                    .SetIcon(iconTextures.ToList());

            autoRefreshCheckbox =
                FluidToggleCheckbox.Get()
                    .SetLabelText("Auto Refresh")
                    .BindToProperty(propertyAutoRefresh);

            saveButton =
                FluidButton.Get()
                    .SetLabelText("Save")
                    .SetIcon(EditorMicroAnimations.EditorUI.Icons.Save)
                    .SetButtonStyle(ButtonStyle.Contained)
                    .SetElementSize(ElementSize.Small)
                    .SetOnClick(() =>
                    {
                        EditorUtility.SetDirty(serializedObject.targetObject);
                        AssetDatabase.SaveAssets();
                    });
        }

        private void Compose()
        {
            root
                .AddChild(componentHeader)
                .AddChild(DesignUtils.spaceBlock)
                .AddChild
                (
                    DesignUtils.row
                        .SetStyleAlignItems(Align.Center)
                        .AddChild(autoRefreshCheckbox)
                        .AddChild(DesignUtils.flexibleSpace)
                        .AddChild(saveButton)
                        .AddChild(DesignUtils.spaceBlock)
                );
        }
    }
}
