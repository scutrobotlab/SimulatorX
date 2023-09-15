// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.Components.Internal;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Runtime.Common.Extensions;
using Doozy.Runtime.Reactor.Animators;
using Doozy.Runtime.Reactor.Animators.Internal;
using Doozy.Runtime.UIElements.Extensions;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.Events;
using UnityEngine.UIElements;
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Editor.Reactor.Components
{
    public class AnimatorFluidListViewItem : FluidListViewItem
    {
        public ObjectField objectField { get; private set; }
        public Label animationTypeLabel { get; private set; }
        public Label animatorNameLabel { get; private set; }

        public UnityAction<SerializedProperty> OnRemoveButtonClick;

        public AnimatorFluidListViewItem(FluidListView listView) : base(listView)
        {
            this.SetListView(listView);
            itemContentContainer.Clear();

            VisualElement row = DesignUtils.row.SetStyleJustifyContent(Justify.Center);

            animationTypeLabel =
                DesignUtils.NewLabel(string.Empty, 10)
                    .SetStyleColor(EditorColors.Reactor.Red)
                    .SetStyleUnityFont(EditorFonts.Ubuntu.Light)
                    .SetTooltip("UIAnimation Type")
                    .SetStyleFlexShrink(1)
                    .SetStyleDisplay(DisplayStyle.None);

            animatorNameLabel =
                DesignUtils.NewLabel(string.Empty)
                    .SetStyleUnityFont(EditorFonts.Ubuntu.Regular)
                    .SetTooltip("Animator Name")
                    .SetStyleFlexShrink(1)
                    .SetStyleDisplay(DisplayStyle.None);

            objectField = new ObjectField
                {
                    objectType = typeof(ReactorAnimator)
                }
                .ResetLayout()
                .SetTooltip("UIAnimator Reference")
                .SetStyleFlexGrow(1)
                .SetStyleFlexShrink(1);

            row
                .SetStyleMargins(DesignUtils.k_Spacing)
                .AddChild(animatorNameLabel.SetStyleMarginRight(DesignUtils.k_Spacing2X))
                .AddChild(animationTypeLabel.SetStyleMarginRight(DesignUtils.k_Spacing2X))
                .AddChild(objectField);

            itemContentContainer.Add(row);
        }

        public void Update(int index, SerializedProperty property)
        {
            //UPDATE INDEX
            showItemIndex = listView.showItemIndex;
            UpdateItemIndex(index);


            //UPDATE PROPERTY
            objectField.BindProperty(property);


            void UpdateInfo()
            {
                bool hasReference = property.objectReferenceValue != null;
                animationTypeLabel.SetStyleDisplay(hasReference ? DisplayStyle.Flex : DisplayStyle.None);
                animatorNameLabel.SetStyleDisplay(hasReference ? DisplayStyle.Flex : DisplayStyle.None);

                if (!hasReference) return;


                var animator = (ReactorAnimator)property.objectReferenceValue;
                var uiAnimator = property.objectReferenceValue as UIAnimator;
                if (uiAnimator != null)
                {
                    animationTypeLabel.SetText($"{uiAnimator.animation.animationType} Animation");
                }
                else
                {
                    animationTypeLabel.SetStyleDisplay(DisplayStyle.None);
                }

                animatorNameLabel.SetText(animator.AnimatorName);
                animatorNameLabel.SetStyleDisplay(animator.AnimatorName.IsNullOrEmpty() ? DisplayStyle.None : DisplayStyle.Flex);
            }

            objectField.schedule.Execute(UpdateInfo).Every(1000);

            //UPDATE REMOVE BUTTON
            itemRemoveButton.OnClick = () => OnRemoveButtonClick?.Invoke(property);
        }
    }
}
