// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using System.Linq;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.ScriptableObjects.Colors;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Runtime.Reactor.Targets;
using Doozy.Runtime.UIElements.Extensions;
using Doozy.Runtime.UIManager.Listeners;
using Doozy.Runtime.UIManager.Visual;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Doozy.Editor.UIManager.Editors.Visual
{
    [CustomEditor(typeof(SignalToSpriteTarget), true)]
    public class SignalToSpriteTargetEditor : UnityEditor.Editor
    {
        private SignalToSpriteTarget castedTarget => (SignalToSpriteTarget)target;
        private IEnumerable<SignalToSpriteTarget> castedTargets => targets.Cast<SignalToSpriteTarget>();

        private static Color accentColor => EditorColors.UIManager.VisualComponent;
        private static EditorSelectableColorInfo selectableAccentColor => EditorSelectableColors.UIManager.VisualComponent;

        private static IEnumerable<Texture2D> componentIconTextures => EditorMicroAnimations.UIManager.Icons.SignalToSpriteTarget;
        private static IEnumerable<Texture2D> spriteTargetIconTextures => EditorMicroAnimations.Reactor.Icons.SpriteTarget;
        
        private VisualElement root { get; set; }
        private FluidComponentHeader componentHeader { get; set; }

        private SerializedProperty propertyStreamId { get; set; }
        private SerializedProperty propertySpriteTarget { get; set; }

        private FluidField streamIdFluidField { get; set; }
        private FluidField spriteTargetFluidField { get; set; }

        private PropertyField streamIdPropertyField { get; set; }
        private ObjectField spriteTargetObjectField { get; set; }
        
        private IVisualElementScheduledItem targetFinder { get; set; }

        public override VisualElement CreateInspectorGUI()
        {
            InitializeEditor();
            Compose();
            return root;
        }
        
        private void OnDestroy()
        {
            componentHeader?.Recycle();
            
            streamIdFluidField?.Recycle();
            spriteTargetFluidField?.Recycle();
        }

        private void FindProperties()
        {
            propertyStreamId = serializedObject.FindProperty("StreamId");
            propertySpriteTarget = serializedObject.FindProperty("SpriteTarget");
        }
        
        private void InitializeEditor()
        {
            FindProperties();

            root = new VisualElement();

            componentHeader =
                FluidComponentHeader.Get()
                    .SetElementSize(ElementSize.Large)
                    .SetAccentColor(accentColor)
                    .SetComponentNameText("Signal To Sprite Target")
                    .SetIcon(componentIconTextures.ToList())
                    .AddManualButton()
                    .AddYouTubeButton();

            streamIdPropertyField = DesignUtils.NewPropertyField(propertyStreamId);
            streamIdFluidField = FluidField.Get().AddFieldContent(streamIdPropertyField);

            spriteTargetObjectField =
                DesignUtils.NewObjectField(propertySpriteTarget, typeof(ReactorSpriteTarget), false)
                    .SetStyleFlexGrow(1)
                    .SetTooltip("Sprite target");
            spriteTargetFluidField =
                FluidField.Get()
                    .SetLabelText("Sprite Target")
                    .SetIcon(spriteTargetIconTextures)
                    .AddFieldContent(spriteTargetObjectField);

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

        private void Compose()
        {
            root
                .AddChild(componentHeader)
                .AddChild(DesignUtils.spaceBlock)
                .AddChild(streamIdFluidField)
                .AddChild(DesignUtils.spaceBlock)
                .AddChild(spriteTargetFluidField)
                .AddChild(DesignUtils.endOfLineBlock)
                ;
        }
    }
}
