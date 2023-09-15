using System.Collections.Generic;
using System.Linq;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.ScriptableObjects.Colors;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Runtime.Reactor.Targets;
using Doozy.Runtime.UIElements.Extensions;
using Doozy.Runtime.UIManager.Visual;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
namespace Doozy.Editor.UIManager.Editors.Visual
{
    [CustomEditor(typeof(SignalToColorTarget), true)]
    public class SignalToColorTargetEditor : UnityEditor.Editor
    {
          private SignalToColorTarget castedTarget => (SignalToColorTarget)target;
        private IEnumerable<SignalToColorTarget> castedTargets => targets.Cast<SignalToColorTarget>();

        private static Color accentColor => EditorColors.UIManager.ListenerComponent;
        private static EditorSelectableColorInfo selectableAccentColor => EditorSelectableColors.UIManager.VisualComponent;

        private static IEnumerable<Texture2D> componentIconTextures => EditorMicroAnimations.UIManager.Icons.SignalToColorTarget;
        private static IEnumerable<Texture2D> colorTargetIconTextures => EditorMicroAnimations.Reactor.Icons.ColorTarget;
        
        private VisualElement root { get; set; }
        private FluidComponentHeader componentHeader { get; set; }

        private SerializedProperty propertyStreamId { get; set; }
        private SerializedProperty propertyColorTarget { get; set; }

        private FluidField streamIdFluidField { get; set; }
        private FluidField colorTargetFluidField { get; set; }

        private PropertyField streamIdPropertyField { get; set; }
        private ObjectField colorTargetObjectField { get; set; }
        
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
            colorTargetFluidField?.Recycle();
        }

        private void FindProperties()
        {
            propertyStreamId = serializedObject.FindProperty("StreamId");
            propertyColorTarget = serializedObject.FindProperty("ColorTarget");
        }
        
        private void InitializeEditor()
        {
            FindProperties();

            root = new VisualElement();

            componentHeader =
                FluidComponentHeader.Get()
                    .SetElementSize(ElementSize.Large)
                    .SetAccentColor(accentColor)
                    .SetComponentNameText("Signal To Color Target")
                    .SetIcon(componentIconTextures.ToList())
                    .AddManualButton()
                    .AddYouTubeButton();

            streamIdPropertyField = DesignUtils.NewPropertyField(propertyStreamId);
            streamIdFluidField = FluidField.Get().AddFieldContent(streamIdPropertyField);

            colorTargetObjectField =
                DesignUtils.NewObjectField(propertyColorTarget, typeof(ReactorColorTarget), false)
                    .SetStyleFlexGrow(1)
                    .SetTooltip("Color target");
            colorTargetFluidField =
                FluidField.Get()
                    .SetLabelText("Color Target")
                    .SetIcon(colorTargetIconTextures)
                    .AddFieldContent(colorTargetObjectField);

            targetFinder = root.schedule.Execute(() =>
            {
                if (castedTarget == null)
                    return;

                if (castedTarget.colorTarget != null)
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
                .AddChild(colorTargetFluidField)
                .AddChild(DesignUtils.endOfLineBlock)
                ;
        }
    }
}
