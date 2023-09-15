// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using UnityEngine.UIElements;
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Editor.EditorUI.Components.Internal
{
    public class FluidSearchableItem
    {
        public ISearchable searchable { get; }
        public VisualElement visualElement { get; }
        public FluidListView fluidListView { get; }
        public FluidButton selectAssetButton { get; }
            
        public FluidSearchableItem(ISearchable searchable, VisualElement visualElement, FluidListView fluidListView, FluidButton selectAssetButton)
        {
            this.searchable = searchable;
            this.visualElement = visualElement;
            this.fluidListView = fluidListView;
            this.selectAssetButton = selectAssetButton;
        }
    }
}
