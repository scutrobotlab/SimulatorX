// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using System.Linq;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.ScriptableObjects.Colors;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Runtime.Common.Attributes;
using Doozy.Runtime.Signals;
using Doozy.Runtime.UIElements.Extensions;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable UnusedMember.Local

namespace Doozy.Editor.Signals.Layouts
{
    public sealed class StreamsConsoleWindowLayout : FluidWindowLayout
    {
        public override Color accentColor => EditorColors.Signals.Stream;
        public override EditorSelectableColorInfo selectableAccentColor => EditorSelectableColors.Signals.Stream;
        public override List<Texture2D> animatedIconTextures => EditorMicroAnimations.Signals.Icons.SignalStream;

        private static Color signalColor => EditorColors.Signals.Signal;
        private static EditorSelectableColorInfo signalSelectableColor => EditorSelectableColors.Signals.Signal;

        private static Color streamColor => EditorColors.Signals.Stream;
        private static EditorSelectableColorInfo streamSelectableColor => EditorSelectableColors.Signals.Stream;

        private static IEnumerable<Texture2D> signalTextures => EditorMicroAnimations.Signals.Icons.Signal;
        private static IEnumerable<Texture2D> streamTextures => EditorMicroAnimations.Signals.Icons.SignalStream;
        private static IEnumerable<Texture2D> filterTextures => EditorMicroAnimations.EditorUI.Icons.Filter;

        private VisualElement consolePinnedRows { get; set; }
        private ScrollView consoleScrollableContainer { get; set; }
        private List<StreamsConsoleRow> consoleRows { get; set; }

        private FluidPlaceholder placeholderOffline { get; set; }
        private FluidPlaceholder placeholderNoStreams { get; set; }

        #region Filter

        private ScrollView filtersScrollView { get; set; }

        private Filter streamSignalProviderFilter { get; set; }
        private Filter streamProviderGameObjectFilter { get; set; }
        private Filter streamCategoryFilter { get; set; }
        private Filter streamNameFilter { get; set; }
        private Filter streamInfoMessageFilter { get; set; }
        private Filter streamGuidFilter { get; set; }
        private bool showGuid { get; set; }

        private List<Filter> filters { get; set; }

        public void ClearFilters()
        {
            foreach (Filter filter in filters)
                filter.Clear();
        }

        private void FiltersUpdated(string pattern)
        {
            foreach (StreamsConsoleRow consoleRow in consoleRows)
                ApplyFilter(consoleRow);
        }

        private void ApplyFilter(StreamsConsoleRow consoleRow)
        {
            consoleRow.streamGuidContainer.SetStyleDisplay(showGuid ? DisplayStyle.Flex : DisplayStyle.None);

            consoleRow.ClearFilter();
            consoleRow.ApplyFilter(streamSignalProviderFilter, consoleRow.streamSignalProviderLabel.text);
            if (consoleRow.isHiddenByFilter) return;
            consoleRow.ApplyFilter(streamProviderGameObjectFilter, consoleRow.streamSignalProviderGameObjectLabel.text);
            if (consoleRow.isHiddenByFilter) return;
            consoleRow.ApplyFilter(streamCategoryFilter, consoleRow.streamCategoryLabel.text);
            if (consoleRow.isHiddenByFilter) return;
            consoleRow.ApplyFilter(streamNameFilter, consoleRow.streamNameLabel.text);
            if (consoleRow.isHiddenByFilter) return;
            consoleRow.ApplyFilter(streamInfoMessageFilter, consoleRow.streamInfoMessageLabel.text);
            if (consoleRow.isHiddenByFilter) return;
            consoleRow.ApplyFilter(streamGuidFilter, consoleRow.streamGuidLabel.text);
        }

        #endregion

        private bool initialized { get; set; }

        public StreamsConsoleWindowLayout()
        {
            AddHeader("Streams Console", "Realtime Streams Visualizer", animatedIconTextures);
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

                placeholderNoStreams =
                    FluidPlaceholder
                        .Get("No active streams detected", EditorMicroAnimations.Signals.Placeholders.OnlineSignal)
                        .SetStyleFlexGrow(1)
                        .Hide();

                consolePinnedRows = new VisualElement().SetStyleMarginBottom(6).SetStyleFlexShrink(0).SetStyleDisplay(DisplayStyle.None);
                consoleScrollableContainer = new ScrollView() { viewDataKey = nameof(consoleScrollableContainer) };
                consoleRows = new List<StreamsConsoleRow>();

                SignalsService.OnStreamAdded += AddStream;
                SignalsService.OnStreamRemoved += RemoveStream;
                EditorApplication.playModeStateChanged += state => UpdatePlayModeDependentElements();

                filters = new List<Filter>
                {
                    (streamSignalProviderFilter = new Filter().SetOnPatternChanged(FiltersUpdated)),
                    (streamProviderGameObjectFilter = new Filter().SetOnPatternChanged(FiltersUpdated)),
                    (streamCategoryFilter = new Filter().SetOnPatternChanged(FiltersUpdated)),
                    (streamNameFilter = new Filter().SetOnPatternChanged(FiltersUpdated)),
                    (streamInfoMessageFilter = new Filter().SetOnPatternChanged(FiltersUpdated)),
                    (streamGuidFilter = new Filter().SetOnPatternChanged(FiltersUpdated))
                };

                FluidField GetFilterField(string labelText, Filter filter)
                {
                    TextField textField = new TextField().ResetLayout();
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
                            .AddFieldContent(textField)
                            .AddInfoElement(buttonClear);

                    field.infoContainer.SetStyleJustifyContent(Justify.FlexEnd);
                    return field;
                }

                FluidContainer streamFiltersContainer =
                    FluidContainer.Get()
                        .SetStyleMargins(DesignUtils.k_Spacing2X, DesignUtils.k_Spacing, DesignUtils.k_Spacing2X, 0)
                        .SetIcon(filterTextures)
                        .SetLabelText("Stream Filters")
                        .SetAccentColor(streamColor)
                        .AddToFluidContent(GetFilterField("Stream Provider", streamSignalProviderFilter))
                        .AddToFluidContent(GetFilterField("Provider GameObject", streamProviderGameObjectFilter))
                        .AddToFluidContent(GetFilterField("Stream Category", streamCategoryFilter))
                        .AddToFluidContent(GetFilterField("Stream Name", streamNameFilter))
                        .AddToFluidContent(GetFilterField("Stream Info Message", streamInfoMessageFilter))
                        .AddToFluidContent(GetFilterField("Stream Key (Guid)", streamGuidFilter));

                filtersScrollView.contentContainer
                    .AddChild(streamFiltersContainer);

                content.Clear();
                content
                    .AddChild(consolePinnedRows)
                    .AddChild(placeholderOffline)
                    .AddChild(placeholderNoStreams)
                    .AddChild(consoleScrollableContainer);

                initialized = true;
            }

            if (consoleRows.Count > 0)
                foreach (StreamsConsoleRow row in consoleRows)
                    row.Dispose();

            consoleRows.Clear();
            consoleScrollableContainer.Clear();

            foreach (SignalStream stream in SignalsService.Streams.Values)
                AddStream(stream);

            UpdatePlayModeDependentElements();
        }

        private void UpdatePlayModeDependentElements()
        {
            consolePinnedRows.SetStyleDisplay(EditorApplication.isPlaying && consolePinnedRows.childCount > 0 ? DisplayStyle.Flex : DisplayStyle.None);
            consoleScrollableContainer.SetStyleDisplay(EditorApplication.isPlaying ? DisplayStyle.Flex : DisplayStyle.None);

            placeholderOffline.Toggle(!EditorApplication.isPlaying);
            placeholderNoStreams.Toggle(EditorApplication.isPlaying && consoleRows.Count == 0);
        }

        private void OnRowPinned(StreamsConsoleRow pinnedRow)
        {
            pinnedRow.RemoveFromHierarchy();
            if (pinnedRow.pinned)
            {
                consolePinnedRows.Insert(0, pinnedRow);
                consolePinnedRows.SetStyleDisplay(DisplayStyle.Flex);
                return;
            }
            consoleScrollableContainer.Insert(0, pinnedRow);
            UpdatePlayModeDependentElements();
        }

        public override void OnDestroy()
        {
            Debug.Log("Streams Console Layout - OnDestroy");
            base.OnDestroy();
            SignalsService.OnStreamAdded -= AddStream;
            SignalsService.OnStreamRemoved -= RemoveStream;

            placeholderOffline?.Recycle();
            placeholderNoStreams?.Recycle();

            foreach (StreamsConsoleRow row in consoleRows)
                row?.Dispose();
        }

        private void AddStream(SignalStream stream)
        {
            Clean();
            var row = StreamsConsoleRow.Get(stream);
            consoleRows.Insert(0, row);
            consoleScrollableContainer.AddChild(row);
            row.OnPinned += OnRowPinned;
            Sort();

            UpdatePlayModeDependentElements();
        }

        private void RemoveStream(SignalStream stream)
        {
            Clean();
            StreamsConsoleRow row = GetRow(stream);
            if (row == null) return;
            consoleRows.Remove(row);
            row.OnPinned -= OnRowPinned;
            row.Recycle();

            UpdatePlayModeDependentElements();
        }

        private void Clean()
        {
            for (int i = consoleRows.Count - 1; i >= 0; i--)
            {
                StreamsConsoleRow row = consoleRows[i];
                SignalStream stream = row?.stream;
                bool isValid = row != null & stream != null & SignalsService.Streams.Values.Contains(stream);
                if (isValid) continue;
                consoleRows.RemoveAt(i);
                row?.Recycle();
            }
        }

        private void Sort()
        {
            consoleRows = consoleRows
                .OrderBy(sv => sv.stream.category)
                .ThenBy(sv => sv.stream.name)
                .ToList();
        }

        private StreamsConsoleRow GetRow(SignalStream stream) =>
            stream == null ? null : consoleRows.FirstOrDefault(row => row.stream == stream);
    }
}
