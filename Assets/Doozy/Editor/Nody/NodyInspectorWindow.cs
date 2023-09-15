// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.ScriptableObjects.Colors;
using Doozy.Editor.EditorUI.Windows.Internal;
using Doozy.Editor.Nody.Nodes.Internal;
using Doozy.Editor.UIElements;
using Doozy.Runtime.UIElements.Extensions;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using EditorStyles = Doozy.Editor.EditorUI.EditorStyles;
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Editor.Nody
{
    public class NodyInspectorWindow : FluidWindow<NodyInspectorWindow>
    {
        private const string WINDOW_TITLE = "Nody Inspector";
        public const string k_WindowMenuPath = "Tools/Doozy/Nody/";

        [MenuItem(k_WindowMenuPath + "Inspector", priority = -850)]
        public static void Open() => InternalOpenWindow(WINDOW_TITLE);

        public static Color accentColor => EditorColors.Nody.Color;
        public static EditorSelectableColorInfo selectableAccentColor => EditorSelectableColors.Nody.Color;

        private TemplateContainer templateContainer { get; set; }
        private VisualElement layoutContainer { get; set; }
        private ScrollView inspectorScrollView { get; set; }
        private VisualElement placeholderContainer { get; set; }

        private FluidPlaceholder emptySelectionPlaceholder { get; set; }

        public UnityEditor.Editor editor { get; private set; }
        public FlowNodeView nodeView { get; private set; }

        protected override void OnEnable()
        {
            base.OnEnable();
            titleContent = new GUIContent(WINDOW_TITLE);
            minSize = new Vector2(200, 200);
        }

        protected override void CreateGUI()
        {
            root.Add(templateContainer = EditorLayouts.Nody.NodyInspectorWindow.CloneTree());

            templateContainer
                .SetStyleFlexGrow(1)
                .AddStyle(EditorStyles.Nody.NodyInspectorWindow);

            layoutContainer = templateContainer.Q<VisualElement>(nameof(layoutContainer));
            inspectorScrollView = layoutContainer.Q<ScrollView>(nameof(inspectorScrollView));
            placeholderContainer = layoutContainer.Q<VisualElement>(nameof(placeholderContainer));

            inspectorScrollView.viewDataKey = $"{nameof(NodyInspectorWindow)}_{nameof(inspectorScrollView)}";

            emptySelectionPlaceholder =
                FluidPlaceholder.Get()
                    .SetStyleAlignSelf(Align.Center)
                    .SetIcon(EditorMicroAnimations.Nody.Icons.Nody);

            placeholderContainer
                .AddChild(emptySelectionPlaceholder);

        }

        /// <summary> Clear the inspector and show the placeholder </summary>
        /// <param name="showPlaceholder"> True to show the placeholder </param>
        public void ClearInspector(bool showPlaceholder = true)
        {
            if (inspectorScrollView == null)
                return;

            inspectorScrollView.contentContainer.RecycleAndClear();
            if (editor != null) DestroyImmediate(editor);
            placeholderContainer.SetStyleDisplay(showPlaceholder ? DisplayStyle.Flex : DisplayStyle.None);
            if (showPlaceholder) emptySelectionPlaceholder.Play();
        }

        public void UpdateSelection(FlowNodeView view)
        {
            nodeView = view;
            bool showPlaceholder = view == null; //node view is null -> nothing selected -> show placeholder
            ClearInspector(showPlaceholder);
            if (showPlaceholder) return;

            try
            {
                editor = UnityEditor.Editor.CreateEditor(view.flowNode);   //TARGET OBJECT CUSTOM EDITOR
                ((FlowNodeEditor)editor).nodeView = view;                  //inject node view reference
                VisualElement visualElement = editor.CreateInspectorGUI(); // <<< EDITOR GETS INJECTED HERE
                inspectorScrollView.contentContainer.Add(visualElement);   //add editor to the inspector
            }
            catch
            {
                // ignored
            }
        }

        public void Refresh()
        {
            UpdateSelection(nodeView);
        }
    }
}
