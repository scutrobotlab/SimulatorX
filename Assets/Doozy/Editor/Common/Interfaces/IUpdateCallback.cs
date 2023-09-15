// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using UnityEngine.Events;
namespace Doozy.Editor.Common
{
    public interface IUpdateCallback
    {
        UnityAction onUpdateCallback { get; set; }

        void SetOnUpdateCallback(UnityAction callback);
        void AddOnUpdateCallback(UnityAction callback);
        void RemoveOnUpdateCallback(UnityAction callback);
        void ClearOnUpdateCallback();
    }
}
