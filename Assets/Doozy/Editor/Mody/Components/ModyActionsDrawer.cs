// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using System.Linq;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.ScriptableObjects.Colors;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Runtime.Reactor.Internal;
using Doozy.Runtime.UIElements.Extensions;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using ModyModule = Doozy.Runtime.Mody.ModyModule;
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Editor.Mody.Components
{
    public class ModyActionsDrawer : VisualElement
    {
        public List<ModyActionsDrawerItem> items { get; private set; }

        private List<string> availableActions { get; }
        private PopupField<string> popup { get; }
        private FluidButton enableButton { get; }

        private FluidField availableActionsField { get; }
        private FluidField placeholderField { get; }
        private FluidPlaceholder placeholder { get; }

        public bool drawerIsEmpty { get; private set; }

        private static EditorSelectableColorInfo modyActionColor => EditorSelectableColors.Mody.Action;
        private static IEnumerable<Texture2D> arrowLeftTextures => EditorMicroAnimations.EditorUI.Arrows.ArrowLeft;
        private static IEnumerable<Texture2D> arrowDownTextures => EditorMicroAnimations.EditorUI.Arrows.ArrowDown;
        private static IEnumerable<Texture2D> emptyPlaceholderTextures => EditorMicroAnimations.EditorUI.Placeholders.Empty;
        
        private static string drawerTitle => "Available Actions";

        public ModyActionsDrawer()
        {
            this.SetStyleFlexGrow(1);

            placeholder = FluidPlaceholder.Get().SetIcon(emptyPlaceholderTextures).ResizeToHeight(35);
            
            items = new List<ModyActionsDrawerItem>();
            availableActions = new List<string> { string.Empty };
            drawerIsEmpty = true;

            popup =
                new PopupField<string>(availableActions, availableActions[0]).ResetLayout()
                    .SetStyleMinWidth(120);

            enableButton = FluidButton.Get("Enable Action")
                .SetButtonStyle(ButtonStyle.Contained)
                .SetElementSize(ElementSize.Tiny)
                .SetStyleAlignSelf(Align.Center)
                .SetIcon(arrowDownTextures)
                .SetAccentColor(modyActionColor)
                .SetOnClick(() =>
                {
                    if (drawerIsEmpty) return;
                    if (!ContainsAction(popup.value)) return;
                    ModyActionsDrawerItem item = GetDrawerItem(popup.value);
                    if (item != null) item.actionEnabled = true;
                    Update();
                });

            popup.RegisterValueChangedCallback(evt => UpdateEnableButtonTooltip(evt.newValue));
            popup.schedule.Execute(() => UpdateEnableButtonTooltip(popup.value));

            availableActionsField =
                FluidField.Get("Available Actions").SetStyleFlexGrow(0)
                    .SetIcon(EditorMicroAnimations.Mody.Icons.ModyAction)
                    .AddFieldContent(popup)
                    .AddInfoElement(DesignUtils.flexibleSpace)
                    .AddInfoElement(enableButton);

            placeholderField =
                FluidField.Get()
                    .AddFieldContent(placeholder);
            
            placeholderField.AddManipulator(new Clickable(() => placeholder?.Play()));
            
            this
                .AddChild(placeholderField)
                .AddChild(availableActionsField);
            
            Update();
        }

        private ModyActionsDrawerItem GetDrawerItem(string actionName) =>
            items.FirstOrDefault(item => item.actionName.Equals(actionName));

        private bool ContainsAction(string actionName) =>
            items.Any(item => item.actionName.Equals(actionName));

        public ModyActionsDrawer AddItem(ModyActionsDrawerItem item)
        {
            if (item == null) return this;
            if (ContainsAction(item.actionName))
            {
                Debug.LogWarning($"'{item.actionName}' - Duplicate action name found! Check the action names defined in the {nameof(ModyModule)} where this action comes from!");
                return this;
            }
            items.Add(item);
            item.parentDrawer = this;
            UpdateAvailableActions();
            return this;
        }

        public void UpdateAvailableActions()
        {
            availableActions.Clear();

            foreach (ModyActionsDrawerItem item in items.Where(item => !item.actionEnabled))
                availableActions.Add(item.actionName);

            enableButton.iconReaction?.Play();

            drawerIsEmpty = availableActions.Count == 0;
            if (drawerIsEmpty) availableActions.Add(string.Empty);
            popup.value = availableActions[0];
        }

        public void Update()
        {
            UpdateAvailableActions();
            placeholderField.SetStyleDisplay(drawerIsEmpty ? DisplayStyle.Flex : DisplayStyle.None);
            placeholder.Toggle(drawerIsEmpty);
            availableActionsField.SetStyleDisplay(drawerIsEmpty ? DisplayStyle.None : DisplayStyle.Flex);
        }

        public void ItemUpdated(ModyActionsDrawerItem item, bool actionEnabled)
        {
            if (!actionEnabled)
            {
                //trigger arrows animations (left arrow towards the popup and then down arrow)
                enableButton.iconReaction.SetTextures(arrowLeftTextures);
                enableButton.iconReaction.SetOnFinishCallback(() =>
                {
                    enableButton.iconReaction.SetTextures(arrowDownTextures);
                    enableButton.iconReaction.ClearOnFinishCallback();
                });
                enableButton.iconReaction.Play();

            }
            Update();
            if (drawerIsEmpty) return;
            if (!actionEnabled) return;
            popup.schedule.Execute(() =>
            {
                if (availableActions.Contains(item.actionName))
                    popup.value = item.actionName;
            });
        }

        private void UpdateEnableButtonTooltip(string actionName) =>
            enableButton.SetTooltip($"Enable the '{actionName}' action and remove it from the {drawerTitle} drawer");
    }
}
