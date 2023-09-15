// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Runtime.Reactor.Easings;
using Doozy.Runtime.Reactor.Ticker;
using UnityEngine;
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Runtime.Reactor.Internal
{
    public static class ReactionExtensions
    {
        public static T SetDirection<T>(this T target, PlayDirection playDirection) where T : Reaction
        {
            target.direction = playDirection;
            return target;
        }

        public static T SetTargetObject<T>(this T target, object targetObject) where T : Reaction
        {
            if (target.hasTargetObject) Reaction.ReactionByTargetObject.RemoveReaction(target.targetObject, target);
            target.targetObject = targetObject;
            target.hasTargetObject = targetObject != null;
            if (target.hasTargetObject) Reaction.ReactionByTargetObject.AddReaction(targetObject, target);
            return target;
        }

        public static T ClearTargetObject<T>(this T target) where T : Reaction =>
            target.SetTargetObject(null);

        public static T SetObjectId<T>(this T target, object objectId) where T : Reaction
        {
            if (target.hasObjectId) Reaction.ReactionByObjectId.RemoveReaction(target.objectId, target);
            target.objectId = objectId;
            target.hasObjectId = objectId != null;
            if (target.hasObjectId) Reaction.ReactionByObjectId.AddReaction(objectId, target);
            return target;
        }

        public static T ClearObjectId<T>(this T target) where T : Reaction =>
            target.SetObjectId(null);

        public static T SetStringId<T>(this T target, string stringId) where T : Reaction
        {
            if (target.hasStringId) Reaction.ReactionByStringId.RemoveReaction(target.stringId, target);
            target.stringId = stringId;
            target.hasStringId = !string.IsNullOrEmpty(stringId);
            if (target.hasStringId) Reaction.ReactionByStringId.AddReaction(stringId, target);
            return target;
        }

        public static T ClearStringId<T>(this T target) where T : Reaction =>
            target.SetStringId(string.Empty);

        public static T SetIntId<T>(this T target, int intId) where T : Reaction
        {
            if (target.hasIntId) Reaction.ReactionByIntId.RemoveReaction(target.intId, target);
            target.intId = intId;
            target.hasIntId = true;
            if (target.hasIntId) Reaction.ReactionByIntId.AddReaction(intId, target);
            return target;
        }

        public static T ClearIntId<T>(this T target) where T : Reaction
        {
            if (target.hasIntId) Reaction.ReactionByIntId.RemoveReaction(target.intId, target);
            target.intId = Reaction.k_DefaultIntId;
            target.hasIntId = false;
            return target;
        }

        public static T ClearAllIds<T>(this T target) where T : Reaction =>
            target
                .ClearTargetObject()
                .ClearObjectId()
                .ClearStringId()
                .ClearIntId();

        public static T SetRuntimeHeartbeat<T>(this T target) where T : Reaction
        {
            target.SetHeartbeat(new RuntimeHeartbeat());
            return target;
        }

        public static T SetPlayMode<T>(this T target, PlayMode playMode) where T : Reaction
        {
            target.settings.playMode = playMode;
            return target;
        }

        public static T SetEaseMode<T>(this T target, EaseMode easeMode) where T : Reaction
        {
            target.settings.easeMode = easeMode;
            return target;
        }

        public static T SetEase<T>(this T target, Ease ease) where T : Reaction
        {
            target.settings.ease = ease;
            return target;
        }

        public static T SetAnimationCurve<T>(this T target, AnimationCurve curve) where T : Reaction
        {
            target.settings.curve = curve;
            return target;
        }

        public static T SetReactionSettings<T>(this T target, ReactionSettings settings) where T : Reaction
        {
            target.settings = settings ?? new ReactionSettings();
            target.RefreshSettings();
            return target;
        }

        public static T ApplyReactionSettings<T>(this T target, ReactionSettings settings) where T : Reaction
        {
            target.settings = new ReactionSettings(settings);
            target.RefreshSettings();
            return target;
        }

        public static T SetStartDelay<T>(this T target, float startDelay) where T : Reaction
        {
            target.settings.useRandomStartDelay = false;
            target.settings.startDelay = startDelay;
            target.settings.Validate();
            target.startDelay = target.settings.GetStartDelay();
            return target;
        }

        public static T SetRandomStartDelay<T>(this T target, float min, float max, bool useRandomValue = true) where T : Reaction
        {
            target.settings.SetRandomStartDelay(min, max, useRandomValue);
            target.settings.Validate();
            target.startDelay = target.settings.GetStartDelay();
            return target;
        }

        public static T SetDuration<T>(this T target, float duration) where T : Reaction
        {
            target.settings.useRandomDuration = false;
            target.settings.duration = duration;
            target.settings.Validate();
            target.duration = target.settings.GetDuration();
            return target;
        }

        public static T SetRandomDuration<T>(this T target, float min, float max, bool useRandomValue = true) where T : Reaction
        {
            target.settings.SetRandomDuration(min, max, useRandomValue);
            target.settings.Validate();
            target.duration = target.settings.GetDuration();
            return target;
        }

        public static T SetLoops<T>(this T target, int numberOfLoops) where T : Reaction
        {
            target.settings.useRandomLoops = false;
            target.settings.loops = numberOfLoops;
            target.settings.Validate();
            target.loops = target.settings.GetLoops();
            return target;
        }

        public static T SetRandomLoops<T>(this T target, int min, int max, bool useRandomValue = true) where T : Reaction
        {
            target.settings.SetRandomLoops(min, max, useRandomValue);
            target.settings.Validate();
            target.loops = target.settings.GetLoops();
            return target;
        }

        public static T SetLoopDelay<T>(this T target, float loopDelay) where T : Reaction
        {
            target.settings.useRandomLoopDelay = false;
            target.settings.loopDelay = loopDelay;
            target.settings.Validate();
            target.loopDelay = target.settings.GetLoopDelay();
            return target;
        }

        public static T SetRandomLoopDelay<T>(this T target, float min, float max, bool useRandomValue = true) where T : Reaction
        {
            target.settings.SetRandomLoopDelay(min, max, useRandomValue);
            target.settings.Validate();
            target.loopDelay = target.settings.GetLoopDelay();
            return target;
        }

        #region OnPlayCallback

        public static T SetOnPlayCallback<T>(this T target, ReactionCallback callback) where T : Reaction
        {
            if (callback == null) return target;
            target.OnPlayCallback = callback;
            return target;
        }

        public static T AddOnPlayCallback<T>(this T target, ReactionCallback callback) where T : Reaction
        {
            if (callback == null) return target;
            target.OnPlayCallback += callback;
            return target;
        }

        public static T ClearOnPlayCallback<T>(this T target) where T : Reaction
        {
            target.OnPlayCallback = null;
            return target;
        }

        #endregion

        #region OnStopCallback

        public static T SetOnStopCallback<T>(this T target, ReactionCallback callback) where T : Reaction
        {
            if (callback == null) return target;
            target.OnStopCallback = callback;
            return target;
        }

        public static T AddOnStopCallback<T>(this T target, ReactionCallback callback) where T : Reaction
        {
            if (callback == null) return target;
            target.OnStopCallback += callback;
            return target;
        }

        public static T ClearOnStopCallback<T>(this T target) where T : Reaction
        {
            target.OnStopCallback = null;
            return target;
        }

        #endregion

        #region OnFinishCallback

        public static T SetOnFinishCallback<T>(this T target, ReactionCallback callback) where T : Reaction
        {
            if (callback == null) return target;
            target.OnFinishCallback = callback;
            return target;
        }

        public static T AddOnFinishCallback<T>(this T target, ReactionCallback callback) where T : Reaction
        {
            if (callback == null) return target;
            target.OnFinishCallback += callback;
            return target;
        }

        public static T ClearOnFinishCallback<T>(this T target) where T : Reaction
        {
            target.OnFinishCallback = null;
            return target;
        }

        #endregion

        #region OnLoopCallback

        public static T SetOnLoopCallback<T>(this T target, ReactionCallback callback) where T : Reaction
        {
            if (callback == null) return target;
            target.OnLoopCallback = callback;
            return target;
        }

        public static T AddOnLoopCallback<T>(this T target, ReactionCallback callback) where T : Reaction
        {
            if (callback == null) return target;
            target.OnLoopCallback += callback;
            return target;
        }

        public static T ClearOnLoopCallback<T>(this T target) where T : Reaction
        {
            target.OnLoopCallback = null;
            return target;
        }

        #endregion

        #region OnPauseCallback

        public static T SetOnPauseCallback<T>(this T target, ReactionCallback callback) where T : Reaction
        {
            if (callback == null) return target;
            target.OnPauseCallback = callback;
            return target;
        }

        public static T AddOnPauseCallback<T>(this T target, ReactionCallback callback) where T : Reaction
        {
            if (callback == null) return target;
            target.OnPauseCallback += callback;
            return target;
        }

        public static T ClearOnPauseCallback<T>(this T target) where T : Reaction
        {
            target.OnPauseCallback = null;
            return target;
        }

        #endregion

        #region OnResumeCallback

        public static T SetOnResumeCallback<T>(this T target, ReactionCallback callback) where T : Reaction
        {
            if (callback == null) return target;
            target.OnResumeCallback = callback;
            return target;
        }

        public static T AddOnResumeCallback<T>(this T target, ReactionCallback callback) where T : Reaction
        {
            if (callback == null) return target;
            target.OnResumeCallback += callback;
            return target;
        }

        public static T ClearOnResumeCallback<T>(this T target) where T : Reaction
        {
            target.OnResumeCallback = null;
            return target;
        }

        #endregion

        #region OnUpdateCallback

        public static T SetOnUpdateCallback<T>(this T target, ReactionCallback callback) where T : Reaction
        {
            if (callback == null) return target;
            target.OnUpdateCallback = callback;
            return target;
        }

        public static T AddOnUpdateCallback<T>(this T target, ReactionCallback callback) where T : Reaction
        {
            if (callback == null) return target;
            target.OnUpdateCallback += callback;
            return target;
        }

        public static T ClearOnUpdateCallback<T>(this T target) where T : Reaction
        {
            target.OnUpdateCallback = null;
            return target;
        }

        #endregion
        
        public static T ClearCallbacks<T>(this T target) where T : Reaction =>
            target.ClearOnFinishCallback()
                .ClearOnLoopCallback()
                .ClearOnPauseCallback()
                .ClearOnPlayCallback()
                .ClearOnResumeCallback()
                .ClearOnStopCallback()
                .ClearOnUpdateCallback();
    }
}
