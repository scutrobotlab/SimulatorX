// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using System.Linq;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.Components.Internal;
using Doozy.Editor.EditorUI.ScriptableObjects.Colors;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Editor.UIElements;
using Doozy.Runtime.Common.Attributes;
using Doozy.Runtime.Signals;
using Doozy.Runtime.UIElements.Extensions;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Editor.Signals.Layouts
{
    public sealed class SignalsConsoleWindowLayout : FluidWindowLayout
    {
        public override List<Texture2D> animatedIconTextures => EditorMicroAnimations.Signals.Icons.Signal;
        public override Color accentColor => EditorColors.Signals.Signal;
        public override EditorSelectableColorInfo selectableAccentColor => EditorSelectableColors.Signals.Signal;

        private static Color signalColor => EditorColors.Signals.Signal;
        private static EditorSelectableColorInfo signalSelectableColor => EditorSelectableColors.Signals.Signal;

        private static Color streamColor => EditorColors.Signals.Stream;
        private static EditorSelectableColorInfo streamSelectableColor => EditorSelectableColors.Signals.Stream;

        private static IEnumerable<Texture2D> signalTextures => EditorMicroAnimations.Signals.Icons.Signal;
        private static IEnumerable<Texture2D> streamTextures => EditorMicroAnimations.Signals.Icons.SignalStream;
        private static IEnumerable<Texture2D> filterTextures => EditorMicroAnimations.EditorUI.Icons.Filter;

        public int consoleHistorySteps { get; private set; } = 20;

        private VisualElement toolbarContainer { get; set; }
        private FluidButton clearLogButton { get; set; }

        private ScrollView consoleScrollableContainer { get; set; }
        private List<SignalsConsoleRow> consoleRows { get; set; }

        private FluidPlaceholder placeholderOffline { get; set; }
        private FluidPlaceholder placeholderNoSignals { get; set; }

        #region Filter

        private ScrollView filtersScrollView { get; set; }

        private Filter sourceGameObjectFilter { get; set; }
        private Filter signalProviderFilter { get; set; }
        private Filter signalSenderObjectFilter { get; set; }
        private Filter messageFilter { get; set; }
        private Filter signalValueTypeFilter { get; set; }
        private Filter signalValueFilter { get; set; }
        private Filter streamSignalProviderFilter { get; set; }
        private Filter streamCategoryFilter { get; set; }
        private Filter streamNameFilter { get; set; }
        private Filter streamInfoMessageFilter { get; set; }
        private Filter streamGuidFilter { get; set; }
        private bool showGuid { get; set; }

        private List<Filter> filters { get; set; }
        private bool anyFilterIsActive => filters.Any(f => f.isActive);

        private void ApplyFilters()
        {

            for (int i = consoleRows.Count - 1; i >= 0; i--)
            {
                SignalsConsoleRow row = consoleRows[i];
                if (!DiscardRow(row))
                    continue;
                consoleRows.RemoveAt(i);
                row.Recycle();
            }
        }

        private void ClearFilters()
        {
            filters.ForEach(f => f.Clear());
        }

        private bool DiscardRow(SignalsConsoleRow row)
        {

            return
                !sourceGameObjectFilter.IsMatch(row.sourceGameObjectLabel.text) ||
                !signalProviderFilter.IsMatch(row.signalProviderLabel.text) ||
                !signalSenderObjectFilter.IsMatch(row.signalSenderObjectLabel.text) ||
                !messageFilter.IsMatch(row.messageLabel.text) ||
                !signalValueTypeFilter.IsMatch(row.signalValueTypeLabel.text) ||
                !signalValueFilter.IsMatch(row.signalValueLabel.text) ||
                !streamSignalProviderFilter.IsMatch(row.streamSignalProviderLabel.text) ||
                !streamNameFilter.IsMatch(row.streamNameLabel.text) ||
                !streamCategoryFilter.IsMatch(row.streamCategoryLabel.text) ||
                !streamInfoMessageFilter.IsMatch(row.streamInfoMessageLabel.text) ||
                !streamGuidFilter.IsMatch(row.streamGuidLabel.text);

        }

        #endregion

        private bool initialized { get; set; }

        public SignalsConsoleWindowLayout()
        {
            AddHeader("Signals Console", "Realtime Signals Visualizer", animatedIconTextures);
            Initialize();
        }

        private void Initialize()
        {
            if (!initialized)
            {
                filtersScrollView = new ScrollView().SetStyleAlignSelf(Align.Stretch);

                sideMenu.toolbarContainer
                    .SetStyleDisplay(DisplayStyle.Flex)
                    .SetStyleFlexGrow(1)
                    .AddChild(filtersScrollView);

                sideMenu.buttonsScrollViewContainer
                    .SetStyleDisplay(DisplayStyle.None);

                sideMenu
                    .RemoveSearch()
                    .SetMenuLevel(FluidSideMenu.MenuLevel.Level_2)
                    .SetMenuInfo("Filters", filterTextures)
                    .HideToolbarWhenCollapsed(true)
                    .IsCollapsable(true)
                    .CollapseMenu(false)
                    .SetAccentColor(selectableAccentColor);

                placeholderOffline =
                    FluidPlaceholder
                        .Get("Signals is offline", EditorMicroAnimations.Signals.Placeholders.OfflineSignal)
                        .SetStyleFlexGrow(1)
                        .Hide();

                placeholderNoSignals =
                    FluidPlaceholder
                        .Get("No signals detected", EditorMicroAnimations.Signals.Placeholders.OnlineSignal)
                        .SetStyleFlexGrow(1)
                        .Hide();

                toolbarContainer =
                    DesignUtils.GetToolbarContainer();

                clearLogButton =
                    FluidButton.Get()
                        .SetLabelText("Clear")
                        .SetIcon(EditorMicroAnimations.EditorUI.Icons.Clear)
                        .SetButtonStyle(ButtonStyle.Contained)
                        .SetElementSize(ElementSize.Tiny)
                        .SetOnClick(ClearLog);

                consoleScrollableContainer = new ScrollView() { viewDataKey = nameof(consoleScrollableContainer) };
                consoleRows = new List<SignalsConsoleRow>();

                SignalsService.OnSignal += AddLogEntry;
                EditorApplication.playModeStateChanged += state => UpdatePlayModeDependentElements();

                filters = new List<Filter>
                {
                    (sourceGameObjectFilter = new Filter().SetOnPatternChanged(s => ApplyFilters())),
                    (signalProviderFilter = new Filter().SetOnPatternChanged(s => ApplyFilters())),
                    (signalSenderObjectFilter = new Filter().SetOnPatternChanged(s => ApplyFilters())),
                    (messageFilter = new Filter().SetOnPatternChanged(s => ApplyFilters())),
                    (signalValueTypeFilter = new Filter().SetOnPatternChanged(s => ApplyFilters())),
                    (signalValueFilter = new Filter().SetOnPatternChanged(s => ApplyFilters())),
                    (streamSignalProviderFilter = new Filter().SetOnPatternChanged(s => ApplyFilters())),
                    (streamCategoryFilter = new Filter().SetOnPatternChanged(s => ApplyFilters())),
                    (streamNameFilter = new Filter().SetOnPatternChanged(s => ApplyFilters())),
                    (streamInfoMessageFilter = new Filter().SetOnPatternChanged(s => ApplyFilters())),
                    (streamGuidFilter = new Filter().SetOnPatternChanged(s => ApplyFilters()))
                };

                FluidField GetFilterField(string labelText, Filter filter)
                {
                    TextField textField =
                        new TextField()
                            .ResetLayout()
                            .SetStyleFlexGrow(1)
                            .SetStyleMarginRight(DesignUtils.k_Spacing);

                    FluidButton buttonClear =
                        FluidButton.Get(EditorMicroAnimations.EditorUI.Icons.Clear)
                            .SetElementSize(ElementSize.Tiny)
                            .SetStyleAlignSelf(Align.FlexEnd)
                            .SetOnClick(() => textField.value = string.Empty);

                    textField.RegisterValueChangedCallback(evt => filter.SetPattern(evt.newValue));

                    FluidField field =
                        FluidField.Get(labelText)
                            .SetStyleFlexGrow(1)
                            .SetStyleMinWidth(60)
                            .ClearBackground()
                            .SetElementSize(ElementSize.Small)
                            .AddFieldContent
                            (
                                DesignUtils.row
                                    .AddChild(textField)
                                    .AddChild(buttonClear)
                            );

                    field.infoContainer.SetStyleJustifyContent(Justify.FlexEnd);
                    return field;
                }

                FluidContainer signalFiltersContainer =
                    FluidContainer.Get()
                        .SetStyleMargins(DesignUtils.k_Spacing2X, 0, DesignUtils.k_Spacing2X, DesignUtils.k_Spacing)
                        .SetIcon(filterTextures)
                        .SetLabelText("Signal Filters")
                        .SetAccentColor(signalColor)
                        .AddToFluidContent(GetFilterField("Source GameObject", sourceGameObjectFilter))
                        .AddToFluidContent(GetFilterField("Signal Provider", signalProviderFilter))
                        .AddToFluidContent(GetFilterField("Signal Sender Object", signalSenderObjectFilter))
                        .AddToFluidContent(GetFilterField("Signal Message", messageFilter))
                        .AddToFluidContent(GetFilterField("Signal Value Type", signalValueTypeFilter))
                        .AddToFluidContent(GetFilterField("Signal Value", signalValueFilter));


                FluidField guidFilter = GetFilterField("Stream Key (Guid)", streamGuidFilter);

                FluidToggleSwitch showGuidSwitch =
                    FluidToggleSwitch.Get()
                        .SetToggleAccentColor(streamSelectableColor)
                        .SetTooltip("Show/Hide Stream Key (Guid)")
                        .SetOnValueChanged(evt =>
                        {
                            showGuid = evt.newValue;
                            streamGuidFilter.Clear();
                            ClearFilters();
                        });


                FluidContainer streamFiltersContainer =
                    FluidContainer.Get()
                        .SetStyleMargins(DesignUtils.k_Spacing2X, DesignUtils.k_Spacing, DesignUtils.k_Spacing2X, 0)
                        .SetIcon(filterTextures)
                        .SetLabelText("Stream Filters")
                        .SetAccentColor(streamColor)
                        .AddToFluidContent(GetFilterField("Stream Provider", streamSignalProviderFilter))
                        .AddToFluidContent(GetFilterField("Stream Category", streamCategoryFilter))
                        .AddToFluidContent(GetFilterField("Stream Name", streamNameFilter))
                        .AddToFluidContent(GetFilterField("Stream Info Message", streamInfoMessageFilter))
                        .AddToFluidContent(guidFilter.AddInfoElement(showGuidSwitch));


                filtersScrollView.contentContainer
                    .AddChild(signalFiltersContainer)
                    .AddChild(streamFiltersContainer);

                header
                    .AddChild
                    (
                        toolbarContainer
                            .AddChild(clearLogButton)
                    );

                content.Clear();
                content
                    .AddChild(placeholderOffline)
                    .AddChild(placeholderNoSignals)
                    .AddChild(consoleScrollableContainer);

                initialized = true;
            }

            UpdatePlayModeDependentElements();
        }

        private void UpdatePlayModeDependentElements()
        {
            toolbarContainer.SetStyleDisplay(EditorApplication.isPlaying ? DisplayStyle.Flex : DisplayStyle.None);
            consoleScrollableContainer.SetStyleDisplay(EditorApplication.isPlaying ? DisplayStyle.Flex : DisplayStyle.None);
            placeholderOffline.Toggle(!EditorApplication.isPlaying);
            placeholderNoSignals.Toggle(EditorApplication.isPlaying & consoleRows.Count == 0);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            SignalsService.OnSignal -= AddLogEntry;

            placeholderOffline?.Dispose();
            placeholderNoSignals?.Dispose();

            foreach (SignalsConsoleRow row in consoleRows)
                row?.Dispose();

            consoleRows.Clear();
        }

        private void AddLogEntry(Signal signal)
        {
            if (!anyFilterIsActive)
            {
                AddToConsole(signal);
                return;
            }

            if (sourceGameObjectFilter.isActive &&
                signal.sourceGameObject != null &&
                sourceGameObjectFilter.IsMatch(signal.sourceGameObject.name))
            {
                AddToConsole(signal);
                return;
            }

            if (signalProviderFilter.isActive &&
                signal.signalProvider != null &&
                signalProviderFilter.IsMatch(signal.signalProvider.name))
            {
                AddToConsole(signal);
                return;
            }

            if (signalSenderObjectFilter.isActive &&
                signal.signalSenderObject != null &&
                signalSenderObjectFilter.IsMatch(signal.signalSenderObject.name))
            {
                AddToConsole(signal);
                return;
            }

            if (signalSenderObjectFilter.isActive &&
                signal.signalSenderObject != null &&
                signalSenderObjectFilter.IsMatch(signal.signalSenderObject.name))
            {
                AddToConsole(signal);
                return;
            }

            if (messageFilter.isActive &&
                messageFilter.IsMatch(signal.message))
            {
                AddToConsole(signal);
                return;
            }

            if (signalValueTypeFilter.isActive &&
                signalValueTypeFilter.IsMatch(signal.valueType.ToString()))
            {
                AddToConsole(signal);
                return;
            }

            if (signalValueFilter.isActive &&
                signal.hasValue &&
                signal.valueAsObject != null &&
                signalValueFilter.IsMatch(signal.valueAsObject.ToString()))
            {
                AddToConsole(signal);
                return;
            }

            if (streamSignalProviderFilter.isActive &&
                signal.stream.signalProvider != null &&
                streamSignalProviderFilter.IsMatch(signal.stream.signalProvider.name))
            {
                AddToConsole(signal);
                return;
            }

            if (streamNameFilter.isActive &&
                streamNameFilter.IsMatch(signal.stream.name))
            {
                AddToConsole(signal);
                return;
            }

            if (streamCategoryFilter.isActive &&
                streamCategoryFilter.IsMatch(signal.stream.category))
            {
                AddToConsole(signal);
                return;
            }

            if (streamInfoMessageFilter.isActive &&
                streamInfoMessageFilter.IsMatch(signal.stream.infoMessage))
            {
                AddToConsole(signal);
                return;
            }

            if (streamGuidFilter.isActive &&
                streamGuidFilter.IsMatch(signal.stream.key.ToString()))
            {
                AddToConsole(signal);
                return;
            }

        }

        private void AddToConsole(Signal signal)
        {
            var row = SignalsConsoleRow.Get(signal);
            row.streamGuidContainer.SetStyleDisplay(showGuid ? DisplayStyle.Flex : DisplayStyle.None);
            consoleRows.Insert(0, row);
            consoleScrollableContainer.Insert(0, row);
            UpdatePlayModeDependentElements();
            if (consoleRows.Count > consoleHistorySteps)
                RemoveLastEntry();
        }

        private void RemoveLastEntry()
        {
            SignalsConsoleRow row = consoleRows[consoleRows.Count - 1];
            if (row == null) return;
            row.Recycle();
            consoleRows.Remove(row);
            UpdatePlayModeDependentElements();
        }

        //unused
        private void CleanLog()
        {
            for (int i = consoleRows.Count - 1; i >= 0; i--)
            {
                SignalsConsoleRow row = consoleRows[i];
                SignalStream stream = row?.signal.stream;
                bool isValid = row != null & stream != null & SignalsService.Streams.Values.Contains(stream);
                if (isValid) continue;
                consoleRows.RemoveAt(i);
                if (row != null) consoleScrollableContainer.Remove(row);
                row?.Recycle();
            }
            UpdatePlayModeDependentElements();
        }

        public void ClearLog()
        {
            foreach (SignalsConsoleRow row in consoleRows)
                row?.Recycle();
            consoleRows.Clear();
            consoleScrollableContainer.RecycleAndClear();
            UpdatePlayModeDependentElements();
        }
    }
}
