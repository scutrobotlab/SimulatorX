// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using System.Linq;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Editor.UIManager.Editors.Containers.Internal;
using Doozy.Runtime.UIElements.Extensions;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UIView = Doozy.Runtime.UIManager.Containers.UIView;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Doozy.Editor.UIManager.Editors.Containers
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(UIView), true)]
    public class UIViewEditor : BaseUIContainerEditor
    {
        public static IEnumerable<Texture2D> viewsIconTextures => EditorMicroAnimations.UIManager.Icons.Views;

        public UIView castedTarget => (UIView)target;
        public IEnumerable<UIView> castedTargets => targets.Cast<UIView>();

        private FluidField idField { get; set; }

        protected SerializedProperty propertyId { get; set; }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            idField?.Recycle();
        }

        protected override void FindProperties()
        {
            base.FindProperties();

            propertyId = serializedObject.FindProperty(nameof(UIView.Id));
        }

        protected override void InitializeEditor()
        {
            base.InitializeEditor();

            componentHeader
                .SetComponentNameText(nameof(UIView))
                .SetIcon(viewsIconTextures.ToList())
                .AddManualButton("https://doozyentertainment.atlassian.net/wiki/spaces/DUI4/pages/1048281106/UIView?atlOrigin=eyJpIjoiMGIxNThlOTZjNTA3NDIyOWI3NWMzNTQ3MWZkYjE5ZTYiLCJwIjoiYyJ9")
                .AddYouTubeButton();

            #region UIView Id

            idField = FluidField.Get().AddFieldContent(DesignUtils.NewPropertyField(propertyId));

            #endregion
        }

        protected override void Compose()
        {
            root
                .AddChild(componentHeader)
                .AddChild
                (
                    DesignUtils.row
                        .SetStyleMargins(50, -4, DesignUtils.k_Spacing2X, DesignUtils.k_Spacing2X)
                        .AddChild(settingsTabButton)
                        .AddChild(DesignUtils.spaceBlock2X)
                        .AddChild(callbacksTab)
                        .AddChild(DesignUtils.spaceBlock2X)
                        .AddChild(DesignUtils.flexibleSpace)
                        .AddChild(DesignUtils.spaceBlock2X)
                        .AddChild(DesignUtils.SystemButton_RenameComponent
                            (
                                castedTarget.gameObject, () => $"View - {castedTarget.Id.Category} {castedTarget.Id.Name}"
                            )
                        )
                        .AddChild(DesignUtils.spaceBlock)
                        .AddChild
                        (
                            DesignUtils.SystemButton_SortComponents
                            (
                                castedTarget.gameObject,
                                nameof(RectTransform),
                                nameof(Canvas),
                                nameof(CanvasGroup),
                                nameof(GraphicRaycaster),
                                nameof(UIView)
                            )
                        )
                )
                .AddChild(DesignUtils.spaceBlock)
                .AddChild(GetShowHideControls())
                .AddChild(callbacksAnimatedContainer)
                .AddChild
                (
                    settingsAnimatedContainer
                        .AddContent
                        (
                            DesignUtils.column
                                .AddChild(idField)
                                .AddChild(DesignUtils.spaceBlock)
                                .AddChild
                                (
                                    DesignUtils.row
                                        .AddChild(onStartBehaviourField)
                                        .AddChild(DesignUtils.spaceBlock)
                                        .AddChild(autoHideAfterShowField)
                                )
                                .AddChild(DesignUtils.spaceBlock)
                                .AddChild(customStartPositionField)
                                .AddChild(DesignUtils.spaceBlock)
                                .AddChild(whenHiddenField)
                                .AddChild(DesignUtils.spaceBlock)
                                .AddChild
                                (
                                    DesignUtils.row
                                        .AddChild(clearSelectedField)
                                        .AddChild(DesignUtils.spaceBlock)
                                        .AddChild(autoSelectAfterShowField)
                                )
                                .AddChild(DesignUtils.spaceBlock)
                        )
                )
                .AddChild(DesignUtils.endOfLineBlock)
                ;
        }
    }
}
