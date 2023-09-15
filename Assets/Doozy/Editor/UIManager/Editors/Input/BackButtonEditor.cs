// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using System.Linq;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.ScriptableObjects.Colors;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Runtime.UIElements.Extensions;
using Doozy.Runtime.UIManager.Input;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Doozy.Editor.UIManager.Editors.Input
{
    [CustomEditor(typeof(BackButton), true)]
    public class BackButtonEditor : UnityEditor.Editor
    {
        private static Color accentColor => EditorColors.UIManager.UIComponent;
        private static EditorSelectableColorInfo selectableAccentColor => EditorSelectableColors.UIManager.UIComponent;

        private static IEnumerable<Texture2D> backIconTextures => EditorMicroAnimations.EditorUI.Icons.Back;
        private static IEnumerable<Texture2D> disabledEnabledTextures => EditorMicroAnimations.EditorUI.Icons.DisabledEnabled;

        private VisualElement root { get; set; }
        private FluidComponentHeader componentHeader { get; set; }

        private FluidField onBackButtonEventField { get; set; }

        private EnabledIndicator stateIndicator { get; set; }
        private VisualElement stateIndicatorContainer { get; set; }

        public override VisualElement CreateInspectorGUI()
        {
            InitializeEditor();
            Compose();
            return root;
        }

        private void OnDestroy()
        {
            componentHeader?.Recycle();
            stateIndicator?.Recycle();
        }

        private void FindProperties()
        {
        }

        private void InitializeEditor()
        {
            FindProperties();

            root = new VisualElement();

            componentHeader = FluidComponentHeader.Get()
                .SetAccentColor(accentColor)
                .SetComponentNameText(ObjectNames.NicifyVariableName(nameof(BackButton)))
                .SetIcon(backIconTextures.ToList())
                .SetElementSize(ElementSize.Large)
                .AddManualButton("https://doozyentertainment.atlassian.net/wiki/spaces/DUI4/pages/1048477760/Back+Button?atlOrigin=eyJpIjoiMWZiNTE0ZDBkZjZhNDMwZjgzYTY1ZjgzMmQ4OTc5MDUiLCJwIjoiYyJ9")
                .AddYouTubeButton();

            stateIndicator =
                EnabledIndicator.Get()
                    .SetIcon(disabledEnabledTextures)
                    .SetEnabledColor(accentColor)
                    .SetSize(60, 12)
                    .Toggle(BackButton.IsEnabled(), false);

            stateIndicatorContainer =
                new VisualElement()
                    .SetName("State Indicator")
                    .SetStyleFlexDirection(FlexDirection.Row)
                    .SetStyleFlexGrow(1)
                    .AddChild(DesignUtils.flexibleSpace)
                    .AddChild(stateIndicator)
                    .AddChild(DesignUtils.spaceBlock2X)
                    .SetStyleMarginTop(-25)
                    .SetStyleMarginBottom(13)
                    .SetStyleDisplay(EditorApplication.isPlayingOrWillChangePlaymode ? DisplayStyle.Flex : DisplayStyle.None);

            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                root.schedule.Execute(() => stateIndicator.Toggle(BackButton.IsEnabled(), true)).Every(100);
            }
        }

        private void Compose()
        {
            root
                .AddChild(componentHeader)
                .AddChild(stateIndicatorContainer);
        }
    }
}
