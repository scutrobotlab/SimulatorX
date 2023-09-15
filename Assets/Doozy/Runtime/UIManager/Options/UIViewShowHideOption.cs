// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Runtime.UIManager.Containers;

namespace Doozy.Runtime.UIManager
{
    [Serializable]
    public class UIViewShowHideOption
    {
        public UIViewId Id = new UIViewId();
        public ShowHideMode Mode = ShowHideMode.Animated;

        public void Show(int playerIndex) => UIView.Show(Id.Category, Id.Name, Mode == ShowHideMode.Instant, playerIndex);
        public void Hide(int playerIndex) => UIView.Hide(Id.Category, Id.Name, Mode == ShowHideMode.Instant, playerIndex);
    }
}
