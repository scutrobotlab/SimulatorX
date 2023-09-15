// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using System.Linq;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.ScriptableObjects.Colors;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Editor.UIManager.Editors.Animators.Internal;
using Doozy.Runtime.Reactor.Targets;
using Doozy.Runtime.UIElements.Extensions;
using Doozy.Runtime.UIManager.Components;
using Doozy.Runtime.UIManager.Visual;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Doozy.Editor.UIManager.Editors.Visual
{
    [CustomEditor(typeof(UISelectableSpriteSwapper), true)]
    public class UISelectableSpriteSwapperEditor : BaseTargetComponentAnimatorEditor
    {
        public UISelectableSpriteSwapper castedTarget => (UISelectableSpriteSwapper)target;
        public IEnumerable<UISelectableSpriteSwapper> castedTargets => targets.Cast<UISelectableSpriteSwapper>();

        protected override Color accentColor => EditorColors.UIManager.VisualComponent;
        protected override EditorSelectableColorInfo selectableAccentColor => EditorSelectableColors.UIManager.VisualComponent;

        private static IEnumerable<Texture2D> uiSelectableIconTextures => EditorMicroAnimations.UIManager.Icons.UISelectable;
        private static IEnumerable<Texture2D> spriteSwapperIconTextures => EditorMicroAnimations.UIManager.Icons.SpriteSwapper;
        private static IEnumerable<Texture2D> spriteTargetIconTextures => EditorMicroAnimations.Reactor.Icons.SpriteTarget;

        private SerializedProperty propertySpriteTarget { get; set; }
        private SerializedProperty propertyNormalSprite { get; set; }
        private SerializedProperty propertyHighlightedSprite { get; set; }
        private SerializedProperty propertyPressedSprite { get; set; }
        private SerializedProperty propertySelectedSprite { get; set; }
        private SerializedProperty propertyDisabledSprite { get; set; }

        private FluidField spriteTargetFluidField { get; set; }
        private FluidField normalSpriteFluidField { get; set; }
        private FluidField highlightedSpriteFluidField { get; set; }
        private FluidField pressedSpriteFluidField { get; set; }
        private FluidField selectedSpriteFluidField { get; set; }
        private FluidField disabledSpriteFluidField { get; set; }

        private ObjectField spriteTargetObjectField { get; set; }
        private ObjectField normalSpriteObjectField { get; set; }
        private ObjectField highlightedSpriteObjectField { get; set; }
        private ObjectField pressedSpriteObjectField { get; set; }
        private ObjectField selectedSpriteObjectField { get; set; }
        private ObjectField disabledSpriteObjectField { get; set; }

        private SerializedProperty propertyToggleCommand { get; set; }
        private EnumField toggleCommandEnumField { get; set; }
        private FluidField toggleCommandField { get; set; }
        
        private IVisualElementScheduledItem targetFinder { get; set; }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            spriteTargetFluidField?.Recycle();
            normalSpriteFluidField?.Recycle();
            highlightedSpriteFluidField?.Recycle();
            pressedSpriteFluidField?.Recycle();
            selectedSpriteFluidField?.Recycle();
            disabledSpriteFluidField?.Recycle();
            
            toggleCommandField?.Recycle();
        }

        protected override void FindProperties()
        {
            base.FindProperties();

            propertySpriteTarget = serializedObject.FindProperty("SpriteTarget");
            propertyNormalSprite = serializedObject.FindProperty("NormalSprite");
            propertyHighlightedSprite = serializedObject.FindProperty("HighlightedSprite");
            propertyPressedSprite = serializedObject.FindProperty("PressedSprite");
            propertySelectedSprite = serializedObject.FindProperty("SelectedSprite");
            propertyDisabledSprite = serializedObject.FindProperty("DisabledSprite");
            
            propertyToggleCommand = serializedObject.FindProperty("ToggleCommand");
        }

        protected override void InitializeEditor()
        {
            base.InitializeEditor();

            componentHeader
                .SetComponentNameText(ObjectNames.NicifyVariableName(nameof(UISelectable)))
                .SetIcon(spriteSwapperIconTextures.ToList())
                .SetComponentTypeText("Sprite Swapper")
                // .AddManualButton("")
                .AddYouTubeButton();
            
            spriteTargetObjectField =
                DesignUtils.NewObjectField(propertySpriteTarget, typeof(ReactorSpriteTarget), false)
                    .SetStyleFlexGrow(1)
                    .SetTooltip("Sprite target");
            spriteTargetFluidField =
                FluidField.Get()
                    .SetLabelText("Sprite Target")
                    .SetIcon(spriteTargetIconTextures)
                    .AddFieldContent(spriteTargetObjectField);

            normalSpriteObjectField = DesignUtils.NewObjectField(propertyNormalSprite, typeof(Sprite), false).SetStyleFlexGrow(1).SetTooltip("Sprite set on Normal state");
            normalSpriteFluidField = FluidField.Get().SetLabelText(" Normal").SetElementSize(ElementSize.Tiny).AddFieldContent(normalSpriteObjectField);
            
            highlightedSpriteObjectField = DesignUtils.NewObjectField(propertyHighlightedSprite, typeof(Sprite), false).SetStyleFlexGrow(1).SetTooltip("Sprite set on Highlighted state");
            highlightedSpriteFluidField = FluidField.Get().SetLabelText(" Highlighted").SetElementSize(ElementSize.Tiny).AddFieldContent(highlightedSpriteObjectField);
            
            pressedSpriteObjectField = DesignUtils.NewObjectField(propertyPressedSprite, typeof(Sprite), false).SetStyleFlexGrow(1).SetTooltip("Sprite set on Pressed state");
            pressedSpriteFluidField = FluidField.Get().SetLabelText(" Pressed").SetElementSize(ElementSize.Tiny).AddFieldContent(pressedSpriteObjectField);
            
            selectedSpriteObjectField = DesignUtils.NewObjectField(propertySelectedSprite, typeof(Sprite), false).SetStyleFlexGrow(1).SetTooltip("Sprite set on Selected state");
            selectedSpriteFluidField = FluidField.Get().SetLabelText(" Selected").SetElementSize(ElementSize.Tiny).AddFieldContent(selectedSpriteObjectField);
            
            disabledSpriteObjectField = DesignUtils.NewObjectField(propertyDisabledSprite, typeof(Sprite), false).SetStyleFlexGrow(1).SetTooltip("Sprite set on Disabled state");
            disabledSpriteFluidField = FluidField.Get().SetLabelText(" Disabled").SetElementSize(ElementSize.Tiny).AddFieldContent(disabledSpriteObjectField);
            
            targetFinder = root.schedule.Execute(() =>
            {
                if (castedTarget == null)
                    return;

                if (castedTarget.spriteTarget != null)
                {
                    targetFinder.Pause();
                    return;
                }

                castedTarget.FindTarget();

            }).Every(1000);
        }
        
        protected override void Compose()
        {
            root
                .AddChild(componentHeader)
                .AddChild(DesignUtils.spaceBlock)
                .AddChild(controllerField)
                .AddChild(DesignUtils.spaceBlock2X)
                .AddChild(spriteTargetFluidField)
                .AddChild(DesignUtils.spaceBlock2X)
                .AddChild(normalSpriteFluidField)
                .AddChild(DesignUtils.spaceBlock)
                .AddChild(highlightedSpriteFluidField)
                .AddChild(DesignUtils.spaceBlock)
                .AddChild(pressedSpriteFluidField)
                .AddChild(DesignUtils.spaceBlock)
                .AddChild(selectedSpriteFluidField)
                .AddChild(DesignUtils.spaceBlock)
                .AddChild(disabledSpriteFluidField)
                .AddChild(DesignUtils.endOfLineBlock)
                ;
        }
        
        protected override void ComposeAnimatedContainers() {} //ignored
        protected override void ComposeTabs() {}               //ignored

        protected override void InitializeController()
        {
            controllerObjectField =
                DesignUtils.NewObjectField(propertyController, typeof(UISelectable))
                    .SetTooltip($"{ObjectNames.NicifyVariableName(nameof(UISelectable))} controller")
                    .SetStyleFlexGrow(1);

            toggleCommandEnumField =
                DesignUtils.NewEnumField(propertyToggleCommand)
                    .SetStyleWidth(50, 50, 50)
                    .SetStyleAlignSelf(Align.Center)
                    .SetStyleMarginRight(DesignUtils.k_Spacing);

            void ShowToggleCommand(bool show) =>
                toggleCommandEnumField.SetStyleDisplay(show ? DisplayStyle.Flex : DisplayStyle.None);

            ShowToggleCommand(propertyController.objectReferenceValue != null && ((UISelectable)propertyController.objectReferenceValue).isToggle);
            controllerObjectField.RegisterValueChangedCallback(evt =>
            {
                if (evt.newValue == null)
                {
                    ShowToggleCommand(false);
                    return;
                }

                ShowToggleCommand(((UISelectable)evt.newValue).isToggle);
            });

            controllerField =
                FluidField.Get()
                    .SetLabelText($"Controller")
                    .SetIcon(uiSelectableIconTextures)
                    .SetStyleMinWidth(200)
                    .AddFieldContent
                    (
                        DesignUtils.row
                            .SetStyleFlexGrow(0)
                            .AddChild(toggleCommandEnumField)
                            .AddChild(controllerObjectField)
                    );
        }
    }
}
