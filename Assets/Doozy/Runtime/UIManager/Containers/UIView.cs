// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using System.Linq;
using Doozy.Runtime.Common.Attributes;
using Doozy.Runtime.Signals;
using Doozy.Runtime.UIManager.Containers.Internal;
using UnityEngine;
using UnityEngine.UI;

namespace Doozy.Runtime.UIManager.Containers
{
    /// <summary>
    /// Container with show and hide capabilities with a category/name id identifier
    /// </summary>
    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(GraphicRaycaster))]
    [RequireComponent(typeof(RectTransform))]
    [AddComponentMenu("Doozy/UI/Containers/UI View")]
    public partial class UIView : UIContainerComponent<UIView>
    {
        /// <summary> Get all the visible views (return all views that are either in the isVisible or isShowing state) </summary>
        public static IEnumerable<UIView> visibleViews =>
            database.Where(view => view.isVisible || view.isShowing);

        /// <summary> Get all the hidden views (return all views that are either in the isHidden or isHiding state) </summary>
        public static IEnumerable<UIView> hiddenViews => database.Where(view => view.isHidden || view.isHiding);

        [ClearOnReload]
        private static SignalStream s_stream;
        /// <summary> Signal stream for this component type </summary>
        public static SignalStream stream => s_stream ??= SignalsService.GetStream(k_StreamCategory, nameof(UIView));

        /// <summary> UIView signal receiver </summary>
        private SignalReceiver receiver { get; set; }

        /// <summary> Category Name Id </summary>
        public UIViewId Id;

        public UIView()
        {
            Id = new UIViewId();
        }

        protected override void Awake()
        {
            base.Awake();
            receiver = new SignalReceiver().SetOnSignalCallback(ProcessSignal);
            stream.ConnectReceiver(receiver);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            stream.DisconnectReceiver(receiver);
        }

        private void ProcessSignal(Signal signal)
        {
            if (!signal.hasValue)
                return;

            if (!(signal.valueAsObject is UIViewSignalData data))
                return;

            if (multiplayerMode & hasMultiplayerInfo &&
                data.playerIndex != playerIndex)
                return;

            if (data.globalCommand) //global command (bypass category and name checks) -> execute
            {
                Execute(data.execute);
                return;
            }
            
            if (!data.viewCategory.Equals(Id.Category)) //check category 
                return;
            
            //category match found - CONTINUE

            if (data.categoryCommand) //category command (bypass name check) -> execute
            {
                Execute(data.execute);
                return;
            }
            
            if (!data.viewName.Equals(Id.Name)) //check name
                return;
            
            //name match found - CONTINUE
            
            Execute(data.execute);
        }

        private void Execute(ShowHideExecute execute)
        {
            switch (execute)
            {
                case ShowHideExecute.Show:
                    Show();
                    break;
                case ShowHideExecute.Hide:
                    Hide();
                    break;
                case ShowHideExecute.InstantShow:
                    InstantShow();
                    break;
                case ShowHideExecute.InstantHide:
                    InstantHide();
                    break;
                case ShowHideExecute.ReverseShow:
                case ShowHideExecute.ReverseHide:
                    //ignored as these states are relevant only for animators
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #region Static Methods

        /// <summary> Get all the registered views with the given category and name </summary>
        /// <param name="category"> UIView category </param>
        /// <param name="name"> UIView name (from the given category) </param>
        public static IEnumerable<UIView> GetViews(string category, string name) =>
            database.Where(view => view.Id.Category.Equals(category)).Where(view => view.Id.Name.Equals(name));

        /// <summary> Get all the registered views with the given category </summary>
        /// <param name="category"> UIView category </param>
        public static IEnumerable<UIView> GetAllViewsInCategory(string category) =>
            database.Where(view => view.Id.Category.Equals(category));

        /// <summary> Show/Hide all the views with the given category and name </summary>
        /// <param name="category"> UIView category </param>
        /// <param name="name"> UIView name (from the given category) </param>
        /// <param name="execute"> What to execute </param>
        /// <param name="playerIndex"> Player index </param>
        internal static void Toggle(string category, string name, ShowHideExecute execute, int playerIndex) =>
            stream.SendSignal(new UIViewSignalData(category, name, execute, playerIndex));

        #region Show

        /// <summary> Show all the views with the given category and name </summary>
        /// <param name="category"> UIView category </param>
        /// <param name="name"> UIView name (from the given category) </param>
        /// <param name="instant"> Should the transition be instant or animated </param>
        /// <param name="playerIndex"> Player index </param>
        public static void Show(string category, string name, bool instant, int playerIndex) =>
            Toggle(category, name, instant ? ShowHideExecute.InstantShow : ShowHideExecute.Show, playerIndex);

        /// <summary> Show all the views with the given category and name </summary>
        /// <param name="category"> UIView category </param>
        /// <param name="name"> UIView name (from the given category) </param>
        /// <param name="instant"> Should the transition be instant or animated </param>
        public static void Show(string category, string name, bool instant = false) =>
            Show(category, name, instant, inputSettings.defaultPlayerIndex);

        /// <summary> Show all the views in the given category (regardless of the view name) </summary>
        /// <param name="category"> UIView category </param>
        /// <param name="instant"> Should the transition be instant or animated </param>
        /// <param name="playerIndex"> Player index </param>
        public static void ShowCategory(string category, bool instant, int playerIndex) =>
            Show(category, string.Empty, instant, playerIndex);

        /// <summary> Show all the views in the given category (regardless of the view name) </summary>
        /// <param name="category"> UIView category </param>
        /// <param name="instant"> Should the transition be instant or animated </param>
        public static void ShowCategory(string category, bool instant = false) =>
            ShowCategory(category, instant, inputSettings.defaultPlayerIndex);

        #endregion

        #region Hide

        /// <summary> Hide all the views with the given category and name </summary>
        /// <param name="category"> UIView category </param>
        /// <param name="name"> UIView name (from the given category) </param>
        /// <param name="instant"> Should the transition be instant or animated </param>
        /// <param name="playerIndex"> Player index </param>
        public static void Hide(string category, string name, bool instant, int playerIndex) =>
            Toggle(category, name, instant ? ShowHideExecute.InstantHide : ShowHideExecute.Hide, playerIndex);

        /// <summary> Hide all the views with the given category and name </summary>
        /// <param name="category"> UIView category </param>
        /// <param name="name"> UIView name (from the given category) </param>
        /// <param name="instant"> Should the transition be instant or animated </param>
        public static void Hide(string category, string name, bool instant = false) =>
            Hide(category, name, instant, inputSettings.defaultPlayerIndex);

        /// <summary> Hide all views in the given category (regardless of the view name) </summary>
        /// <param name="category"> UIView category </param>
        /// <param name="instant"> Should the transition be instant or animated </param>
        /// <param name="playerIndex"> Player index </param>
        public static void HideCategory(string category, bool instant, int playerIndex) =>
            Hide(category, string.Empty, instant, playerIndex);

        /// <summary> Hide all views in the given category (regardless of the view name) </summary>
        /// <param name="category"> UIView category </param>
        /// <param name="instant"> Should the transition be instant or animated </param>
        public static void HideCategory(string category, bool instant = false) =>
            HideCategory(category, instant, inputSettings.defaultPlayerIndex);

        /// <summary> Hide all views regardless of their state </summary>
        /// <param name="instant"> Should the transition be instant or animated </param>
        /// <param name="playerIndex"> Player index </param>
        public static void HideAllViews(bool instant, int playerIndex) =>
            stream.SendSignal(new UIViewSignalData(string.Empty, string.Empty, instant ? ShowHideExecute.InstantHide : ShowHideExecute.Hide, playerIndex));

        /// <summary> Hide all views regardless of their state </summary>
        /// <param name="instant"> Should the transition be instant or animated </param>
        public static void HideAllViews(bool instant = false) =>
            stream.SendSignal(new UIViewSignalData(string.Empty, string.Empty, instant ? ShowHideExecute.InstantHide : ShowHideExecute.Hide, inputSettings.defaultPlayerIndex));

        #endregion

        #endregion
    }
}
