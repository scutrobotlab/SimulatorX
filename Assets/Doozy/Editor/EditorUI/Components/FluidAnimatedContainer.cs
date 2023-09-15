// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Editor.Reactor.Internal;
using Doozy.Editor.UIElements;
using Doozy.Runtime.Reactor;
using Doozy.Runtime.Reactor.Easings;
using Doozy.Runtime.Reactor.Internal;
using Doozy.Runtime.Reactor.Reactions;
using Doozy.Runtime.UIElements.Extensions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace Doozy.Editor.EditorUI.Components
{
    public class FluidAnimatedContainer : VisualElement, IDisposable
    {
        public void Dispose()
        {
            ClearContent();
            reaction?.Recycle();
        }

        //layoutContainer
        //--fluidContainer

        #region State

        public enum State
        {
            Idle,
            IsShowing,
            Visible,
            IsHiding,
            Hidden
        }

        private State m_State;
        public State state
        {
            get => m_State;
            set
            {
                // Debug.Log($"{name}.{nameof(state)}: {state} > {value}");
                reaction?.ClearOnFinishCallback();
                switch (value)
                {
                    case State.Idle:
                        //do nothing
                        break;
                    case State.Visible:
                        fluidContainer.SetStyleDisplay(DisplayStyle.Flex);
                        break;
                    case State.Hidden:
                        fluidContainer.SetStyleDisplay(DisplayStyle.None);
                        if (clearOnHide) ClearContent();
                        break;
                    case State.IsShowing:
                        fluidContainer.SetStyleDisplay(DisplayStyle.Flex);
                        reaction?.SetOnFinishCallback(() => state = State.Visible);
                        break;
                    case State.IsHiding:
                        fluidContainer.SetStyleDisplay(DisplayStyle.Flex);
                        reaction?.SetOnFinishCallback(() => state = State.Hidden);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(value), value, null);
                }
                m_State = value;
                onStateChanged?.Invoke(state);
            }
        }

        public bool isVisible => state == State.Visible || state == State.IsShowing;
        public bool isHidden => state == State.Hidden || state == State.IsHiding;

        #endregion

        #region Direction

        public enum Direction
        {
            Top,
            Bottom,
            Left,
            Right
        }

        public Direction direction { get; private set; }

        #endregion

        //SETTINGS
        public const Ease k_ReactionEase = Ease.OutEasy;
        public const float k_ReactionDuration = 0.25f;

        private float fluidContainerWidth { get; set; }
        private float fluidContainerHeight { get; set; }

        public TemplateContainer templateContainer { get; }
        public VisualElement layoutContainer { get; }
        public VisualElement fluidContainer { get; }

        public FloatReaction reaction { get; private set; }

        public bool clearOnHide { get; set; }
        public UnityAction OnShowCallback;
        public UnityAction OnHideCallback;
        public UnityAction<State> onStateChanged { get; set; }

        public FluidAnimatedContainer()
        {
            Add(templateContainer = EditorLayouts.EditorUI.FluidAnimatedContainer.CloneTree());
            templateContainer
                .SetStyleFlexGrow(1)
                .AddStyle(EditorStyles.EditorUI.FluidAnimatedContainer);

            layoutContainer = templateContainer.Q<VisualElement>(nameof(layoutContainer));
            fluidContainer = layoutContainer.Q<VisualElement>(nameof(fluidContainer));

            reaction = Reaction.Get<FloatReaction>().SetEditorHeartbeat().SetEase(k_ReactionEase).SetDuration(k_ReactionDuration)
                .SetSetter(value =>
                {

                    RecalculateValues();
                    fluidContainer.SetStyleDisplay(value < 0.99f ? DisplayStyle.Flex : DisplayStyle.None);
                    switch (direction)
                    {

                        case Direction.Top:
                        case Direction.Bottom:
                            // Debug.Log($"{reaction} > -{fluidContainerHeight} * {value.Round(2)} = {-fluidContainerHeight * value}");
                            if (fluidContainerHeight == 0) return; //this fixes the 'jumpy' effect when the contents of the container changes during animation
                            Update(-fluidContainerHeight * value);
                            break;
                        case Direction.Left:
                        case Direction.Right:
                            // Debug.Log($"{reaction} > -{fluidContainerHeight} * {value.Round(2)} = {-fluidContainerHeight * value}");
                            if (fluidContainerWidth == 0) return; //this fixes the 'jumpy' effect when the contents of the container changes during animation
                            Update(-fluidContainerWidth * value);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                });

            // Debug.Log($"{nameof(FluidAnimatedContainer)} - reaction type: {reaction.GetType().Name}");

            direction = Direction.Top;
            state = State.Idle;

            fluidContainer.SetStyleDisplay(DisplayStyle.None);
            fluidContainer.visible = false;

            fluidContainer.RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
        }

        private void UpdateContainer()
        {
            RecalculateValues();

            if (reaction is { isActive: true })
                return;

            switch (state)
            {
                case State.Idle:
                case State.IsShowing:
                case State.IsHiding:
                    return;
                case State.Visible:
                    ResetToVisiblePosition();
                    break;
                case State.Hidden:
                    ResetToHiddenPosition();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private bool m_ShowContainer;
        private void OnGeometryChanged(GeometryChangedEvent evt)
        {
            // Debug.Log($"{name}.{nameof(OnGeometryChanged)}: {evt}");
            // RecalculateValues();
            UpdateContainer();
            if (!m_ShowContainer) return;
            if (reaction == null) return;
            if (reaction.isActive) return;
            m_ShowContainer = false;
            ResetToHiddenPosition();
            reaction.Play(PlayDirection.Reverse);
            fluidContainer.visible = true;
        }

        public void ResetToHiddenPosition()
        {
            switch (direction)
            {
                case Direction.Top:
                case Direction.Bottom:
                    Update(-fluidContainerHeight); //reset to hidden
                    break;
                case Direction.Left:
                case Direction.Right:
                    Update(-fluidContainerWidth); //reset to hidden
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void ResetToVisiblePosition()
        {
            switch (direction)
            {
                case Direction.Top:
                case Direction.Bottom:
                    Update(0); //reset to visible
                    break;
                case Direction.Left:
                case Direction.Right:
                    Update(0); //reset to visible
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void RecalculateValues()
        {
            // Debug.Log($"{nameof(RecalculateValues)}");
            fluidContainerWidth = fluidContainer.resolvedStyle.width;
            fluidContainerHeight = fluidContainer.resolvedStyle.height;
        }

        public FluidAnimatedContainer SetDirection(Direction targetDirection)
        {
            if (targetDirection == direction) return this;
            if (reaction.isActive) reaction.SetProgressAtZero();
            direction = targetDirection;
            return this;
        }

        private void Update(float toValue)
        {
            switch (direction)
            {
                case Direction.Top:
                    fluidContainer.SetStyleMarginTop(toValue);
                    break;
                case Direction.Bottom:
                    fluidContainer.SetStyleBottom(toValue);
                    fluidContainer.SetStyleMarginTop(toValue);
                    break;
                case Direction.Left:
                    fluidContainer.SetStyleMarginLeft(toValue);
                    break;
                case Direction.Right:
                    fluidContainer.SetStyleMarginRight(toValue);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }
        }

        public FluidAnimatedContainer Toggle(bool show, bool animateChange = true) =>
            show ? Show(animateChange) : Hide(animateChange);

        public FluidAnimatedContainer Show(bool animateChange = true)
        {
            schedule.Execute(() => OnGeometryChanged(null));
            fluidContainer.visible = true;

            if (!animateChange)
            {
                switch (state)
                {
                    case State.Idle:
                    case State.Hidden:
                        OnShowCallback?.Invoke();
                        break;
                    case State.Visible:
                        //do nothing
                        break;
                    case State.IsShowing:
                    case State.IsHiding:
                        reaction?.Stop();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                Update(0); //reset
                state = State.Visible;
            }
            else //animation
            {
                switch (state)
                {
                    case State.Idle:
                    case State.Hidden:
                        m_ShowContainer = true;
                        OnShowCallback?.Invoke();
                        fluidContainer.visible = false;
                        state = State.IsShowing;
                        break;
                    case State.IsHiding:
                        reaction.Reverse();
                        state = State.IsShowing;
                        break;
                    case State.IsShowing:
                    case State.Visible:
                        //do nothing
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return this;
        }

        public FluidAnimatedContainer Hide(bool animateChange = true)
        {
            schedule.Execute(() => OnGeometryChanged(null));
            if (!animateChange)
            {
                switch (state)
                {
                    case State.Idle:
                    case State.Hidden:
                        //do nothing
                        break;
                    case State.Visible:
                        OnHideCallback?.Invoke();
                        RecalculateValues();
                        break;
                    case State.IsShowing:
                    case State.IsHiding:
                        reaction.Stop();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                switch (direction)
                {
                    case Direction.Top:
                    case Direction.Bottom:
                        Update(-fluidContainerHeight); //reset
                        break;
                    case Direction.Left:
                    case Direction.Right:
                        Update(-fluidContainerWidth); //reset
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                fluidContainer.SetStyleDisplay(DisplayStyle.None);
                state = State.Hidden;
            }
            else //animation
            {
                switch (state)
                {
                    case State.Idle:
                        reaction.Play(PlayDirection.Forward);
                        state = State.IsHiding;
                        break;
                    case State.Visible:
                        RecalculateValues();
                        OnHideCallback?.Invoke();
                        reaction.Play(PlayDirection.Forward);
                        state = State.IsHiding;
                        break;
                    case State.IsShowing:
                        reaction.Reverse();
                        state = State.IsHiding;
                        break;
                    case State.IsHiding:
                    case State.Hidden:
                        //do nothing
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return this;
        }

        public FluidAnimatedContainer AddContent(VisualElement content)
        {
            fluidContainer.AddChild(content);
            return this;
        }

        /// <summary> Recycle and clear the animated container and then reset its layout </summary>
        public FluidAnimatedContainer ClearContent(bool resetLayout = false)
        {
            fluidContainer.RecycleAndClear();
            if (resetLayout) fluidContainer.ResetLayout();
            return this;
        }

        public FluidAnimatedContainer SetClearOnHide(bool clear)
        {
            clearOnHide = clear;
            return this;
        }

        #region OnShowCallback

        public FluidAnimatedContainer AddOnShowCallback(UnityAction callback)
        {
            OnShowCallback += callback;
            return this;
        }

        public FluidAnimatedContainer SetOnShowCallback(UnityAction callback)
        {
            OnShowCallback = callback;
            return this;
        }

        public FluidAnimatedContainer ClearOnShowCallback()
        {
            OnShowCallback = null;
            return this;
        }

        #endregion

        #region OnHideCallback

        public FluidAnimatedContainer AddOnHideCallback(UnityAction callback)
        {
            OnHideCallback += callback;
            return this;
        }

        public FluidAnimatedContainer SetOnHideCallback(UnityAction callback)
        {
            OnHideCallback = callback;
            return this;
        }

        public FluidAnimatedContainer ClearOnHideCallback()
        {
            OnHideCallback = null;
            return this;
        }

        #endregion
    }
}
