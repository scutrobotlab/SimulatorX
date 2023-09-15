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
using Doozy.Runtime.UIManager.Containers;
using Doozy.Runtime.UIManager.Visual;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
namespace Doozy.Editor.UIManager.Editors.Visual
{
    [CustomEditor(typeof(UIContainerSpriteSwapper), true)]
    public class UIContainerSpriteSwapperEditor : BaseTargetComponentAnimatorEditor
    {
        public UIContainerSpriteSwapper castedTarget => (UIContainerSpriteSwapper)target;
        public IEnumerable<UIContainerSpriteSwapper> castedTargets => targets.Cast<UIContainerSpriteSwapper>();

        protected override Color accentColor => EditorColors.UIManager.VisualComponent;
        protected override EditorSelectableColorInfo selectableAccentColor => EditorSelectableColors.UIManager.VisualComponent;

        private static IEnumerable<Texture2D> uiContainerIconTextures => EditorMicroAnimations.UIManager.Icons.UIContainer;
        private static IEnumerable<Texture2D> spriteSwapperIconTextures => EditorMicroAnimations.UIManager.Icons.SpriteSwapper;
        private static IEnumerable<Texture2D> spriteTargetIconTextures => EditorMicroAnimations.Reactor.Icons.SpriteTarget;

        private SerializedProperty propertySpriteTarget { get; set; }
        private SerializedProperty propertyShowSprite { get; set; }
        private SerializedProperty propertyHideSprite { get; set; }

        private FluidField spriteTargetFluidField { get; set; }
        private FluidField showSpriteFluidField { get; set; }
        private FluidField hideSpriteFluidField { get; set; }

        private ObjectField spriteTargetObjectField { get; set; }
        private ObjectField showSpriteObjectField { get; set; }
        private ObjectField hideSpriteObjectField { get; set; }

        private IVisualElementScheduledItem targetFinder { get; set; }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            spriteTargetFluidField?.Recycle();
            showSpriteFluidField?.Recycle();
            hideSpriteFluidField?.Recycle();
        }

        protected override void FindProperties()
        {
            base.FindProperties();

            propertySpriteTarget = serializedObject.FindProperty("SpriteTarget");
            propertyShowSprite = serializedObject.FindProperty("ShowSprite");
            propertyHideSprite = serializedObject.FindProperty("HideSprite");
        }

        protected override void InitializeEditor()
        {
            base.InitializeEditor();

            componentHeader
                .SetComponentNameText(ObjectNames.NicifyVariableName(nameof(UIContainer)))
                .SetIcon(spriteSwapperIconTextures.ToList())
                .SetComponentTypeText("Sprite Swapper")
                .AddManualButton()
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

            showSpriteObjectField = DesignUtils.NewObjectField(propertyShowSprite, typeof(Sprite), false).SetStyleFlexGrow(1).SetTooltip("Sprite set on Show");
            showSpriteFluidField = FluidField.Get().SetLabelText(" Show").SetElementSize(ElementSize.Tiny).AddFieldContent(showSpriteObjectField);

            hideSpriteObjectField = DesignUtils.NewObjectField(propertyHideSprite, typeof(Sprite), false).SetStyleFlexGrow(1).SetTooltip("Sprite set on Hide");
            hideSpriteFluidField = FluidField.Get().SetLabelText(" Hide  ").SetElementSize(ElementSize.Tiny).AddFieldContent(hideSpriteObjectField);

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
                .AddChild(showSpriteFluidField)
                .AddChild(DesignUtils.spaceBlock)
                .AddChild(hideSpriteFluidField)
                .AddChild(DesignUtils.endOfLineBlock)
                ;
        }

        protected override void ComposeAnimatedContainers() {} //ignored
        protected override void ComposeTabs() {}               //ignored

        protected override void InitializeController()
        {
            controllerObjectField =
                DesignUtils.NewObjectField(propertyController, typeof(UIContainer))
                    .SetTooltip($"{ObjectNames.NicifyVariableName(nameof(UIContainer))} controller")
                    .SetStyleFlexGrow(1);

            controllerField =
                FluidField.Get()
                    .SetLabelText($"Controller")
                    .SetIcon(uiContainerIconTextures)
                    .SetStyleMinWidth(200)
                    .AddFieldContent(controllerObjectField);
        }
    }
}
