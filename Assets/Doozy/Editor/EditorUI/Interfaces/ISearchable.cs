// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using UnityEngine.UIElements;

namespace Doozy.Editor.EditorUI
{
    public interface ISearchable
    {
        bool isSearching { get; }
        string searchPattern { get; }
        bool hasSearchResults { get; }
        VisualElement searchResults { get; }
        
        void ClearSearch();
        void Search(string pattern);
        void UpdateSearchResults();
    }
}
