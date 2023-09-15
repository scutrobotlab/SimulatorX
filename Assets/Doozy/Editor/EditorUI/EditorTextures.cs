// Copyright (c) 2015 - 2021 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

//.........................
//.....Generated Class.....
//.........................
//.......Do not edit.......
//.........................

using System.Diagnostics.CodeAnalysis;
using Doozy.Editor.EditorUI.ScriptableObjects.Textures;
using UnityEngine;

namespace Doozy.Editor.EditorUI
{
    [SuppressMessage("ReSharper", "IdentifierTypo")]
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    [SuppressMessage("ReSharper", "UnusedType.Global")]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public static class EditorTextures
    {
        public static class EditorUI
        {
            public static class Icons
            {
                private static EditorDataTextureGroup s_textureGroup;
                private static EditorDataTextureGroup textureGroup =>
                    s_textureGroup
                        ? s_textureGroup
                        : s_textureGroup = EditorDataTextureDatabase.GetTextureGroup("EditorUI","Icons");

                public static Texture2D GetTexture2D(TextureName textureName) =>
                    textureGroup.GetTexture(textureName.ToString());

                public enum TextureName
                {
                    ButtonClick,
                    ButtonDoubleClick,
                    ButtonLeftClick,
                    ButtonLongClick,
                    ButtonMiddleClick,
                    ButtonRightClick,
                    Copy,
                    Cut,
                    Deselected,
                    EventsOnFinish,
                    EventsOnStart,
                    Export,
                    Filter,
                    FrameByFrameAnimation,
                    FrameByFrameAnimator,
                    GameObject,
                    GenericDatabase,
                    Hourglass,
                    EditorColorPalette,
                    EditorFontFamily,
                    EditorLayoutGroup,
                    EditorMicroAnimationGroup,
                    EditorSelectableColorPalette,
                    EditorStyleGroup,
                    EditorTextureGroup,
                    Import,
                    Location,
                    Locked,
                    Paste,
                    PointerDown,
                    PointerEnter,
                    PointerExit,
                    PointerUp,
                    Prefab,
                    SelectableStates,
                    Selected,
                    ToggleMixed,
                    ToggleOFF,
                    ToggleON,
                    UIBehaviour,
                    UISelectable,
                    Unity,
                    UnityEvent,
                    Unlocked,
                    VisibilityChanged
                }
                

                private static Texture2D s_ButtonClick;
                public static Texture2D ButtonClick => s_ButtonClick ? s_ButtonClick : s_ButtonClick = GetTexture2D(TextureName.ButtonClick);
                private static Texture2D s_ButtonDoubleClick;
                public static Texture2D ButtonDoubleClick => s_ButtonDoubleClick ? s_ButtonDoubleClick : s_ButtonDoubleClick = GetTexture2D(TextureName.ButtonDoubleClick);
                private static Texture2D s_ButtonLeftClick;
                public static Texture2D ButtonLeftClick => s_ButtonLeftClick ? s_ButtonLeftClick : s_ButtonLeftClick = GetTexture2D(TextureName.ButtonLeftClick);
                private static Texture2D s_ButtonLongClick;
                public static Texture2D ButtonLongClick => s_ButtonLongClick ? s_ButtonLongClick : s_ButtonLongClick = GetTexture2D(TextureName.ButtonLongClick);
                private static Texture2D s_ButtonMiddleClick;
                public static Texture2D ButtonMiddleClick => s_ButtonMiddleClick ? s_ButtonMiddleClick : s_ButtonMiddleClick = GetTexture2D(TextureName.ButtonMiddleClick);
                private static Texture2D s_ButtonRightClick;
                public static Texture2D ButtonRightClick => s_ButtonRightClick ? s_ButtonRightClick : s_ButtonRightClick = GetTexture2D(TextureName.ButtonRightClick);
                private static Texture2D s_Copy;
                public static Texture2D Copy => s_Copy ? s_Copy : s_Copy = GetTexture2D(TextureName.Copy);
                private static Texture2D s_Cut;
                public static Texture2D Cut => s_Cut ? s_Cut : s_Cut = GetTexture2D(TextureName.Cut);
                private static Texture2D s_Deselected;
                public static Texture2D Deselected => s_Deselected ? s_Deselected : s_Deselected = GetTexture2D(TextureName.Deselected);
                private static Texture2D s_EventsOnFinish;
                public static Texture2D EventsOnFinish => s_EventsOnFinish ? s_EventsOnFinish : s_EventsOnFinish = GetTexture2D(TextureName.EventsOnFinish);
                private static Texture2D s_EventsOnStart;
                public static Texture2D EventsOnStart => s_EventsOnStart ? s_EventsOnStart : s_EventsOnStart = GetTexture2D(TextureName.EventsOnStart);
                private static Texture2D s_Export;
                public static Texture2D Export => s_Export ? s_Export : s_Export = GetTexture2D(TextureName.Export);
                private static Texture2D s_Filter;
                public static Texture2D Filter => s_Filter ? s_Filter : s_Filter = GetTexture2D(TextureName.Filter);
                private static Texture2D s_FrameByFrameAnimation;
                public static Texture2D FrameByFrameAnimation => s_FrameByFrameAnimation ? s_FrameByFrameAnimation : s_FrameByFrameAnimation = GetTexture2D(TextureName.FrameByFrameAnimation);
                private static Texture2D s_FrameByFrameAnimator;
                public static Texture2D FrameByFrameAnimator => s_FrameByFrameAnimator ? s_FrameByFrameAnimator : s_FrameByFrameAnimator = GetTexture2D(TextureName.FrameByFrameAnimator);
                private static Texture2D s_GameObject;
                public static Texture2D GameObject => s_GameObject ? s_GameObject : s_GameObject = GetTexture2D(TextureName.GameObject);
                private static Texture2D s_GenericDatabase;
                public static Texture2D GenericDatabase => s_GenericDatabase ? s_GenericDatabase : s_GenericDatabase = GetTexture2D(TextureName.GenericDatabase);
                private static Texture2D s_Hourglass;
                public static Texture2D Hourglass => s_Hourglass ? s_Hourglass : s_Hourglass = GetTexture2D(TextureName.Hourglass);
                private static Texture2D s_EditorColorPalette;
                public static Texture2D EditorColorPalette => s_EditorColorPalette ? s_EditorColorPalette : s_EditorColorPalette = GetTexture2D(TextureName.EditorColorPalette);
                private static Texture2D s_EditorFontFamily;
                public static Texture2D EditorFontFamily => s_EditorFontFamily ? s_EditorFontFamily : s_EditorFontFamily = GetTexture2D(TextureName.EditorFontFamily);
                private static Texture2D s_EditorLayoutGroup;
                public static Texture2D EditorLayoutGroup => s_EditorLayoutGroup ? s_EditorLayoutGroup : s_EditorLayoutGroup = GetTexture2D(TextureName.EditorLayoutGroup);
                private static Texture2D s_EditorMicroAnimationGroup;
                public static Texture2D EditorMicroAnimationGroup => s_EditorMicroAnimationGroup ? s_EditorMicroAnimationGroup : s_EditorMicroAnimationGroup = GetTexture2D(TextureName.EditorMicroAnimationGroup);
                private static Texture2D s_EditorSelectableColorPalette;
                public static Texture2D EditorSelectableColorPalette => s_EditorSelectableColorPalette ? s_EditorSelectableColorPalette : s_EditorSelectableColorPalette = GetTexture2D(TextureName.EditorSelectableColorPalette);
                private static Texture2D s_EditorStyleGroup;
                public static Texture2D EditorStyleGroup => s_EditorStyleGroup ? s_EditorStyleGroup : s_EditorStyleGroup = GetTexture2D(TextureName.EditorStyleGroup);
                private static Texture2D s_EditorTextureGroup;
                public static Texture2D EditorTextureGroup => s_EditorTextureGroup ? s_EditorTextureGroup : s_EditorTextureGroup = GetTexture2D(TextureName.EditorTextureGroup);
                private static Texture2D s_Import;
                public static Texture2D Import => s_Import ? s_Import : s_Import = GetTexture2D(TextureName.Import);
                private static Texture2D s_Location;
                public static Texture2D Location => s_Location ? s_Location : s_Location = GetTexture2D(TextureName.Location);
                private static Texture2D s_Locked;
                public static Texture2D Locked => s_Locked ? s_Locked : s_Locked = GetTexture2D(TextureName.Locked);
                private static Texture2D s_Paste;
                public static Texture2D Paste => s_Paste ? s_Paste : s_Paste = GetTexture2D(TextureName.Paste);
                private static Texture2D s_PointerDown;
                public static Texture2D PointerDown => s_PointerDown ? s_PointerDown : s_PointerDown = GetTexture2D(TextureName.PointerDown);
                private static Texture2D s_PointerEnter;
                public static Texture2D PointerEnter => s_PointerEnter ? s_PointerEnter : s_PointerEnter = GetTexture2D(TextureName.PointerEnter);
                private static Texture2D s_PointerExit;
                public static Texture2D PointerExit => s_PointerExit ? s_PointerExit : s_PointerExit = GetTexture2D(TextureName.PointerExit);
                private static Texture2D s_PointerUp;
                public static Texture2D PointerUp => s_PointerUp ? s_PointerUp : s_PointerUp = GetTexture2D(TextureName.PointerUp);
                private static Texture2D s_Prefab;
                public static Texture2D Prefab => s_Prefab ? s_Prefab : s_Prefab = GetTexture2D(TextureName.Prefab);
                private static Texture2D s_SelectableStates;
                public static Texture2D SelectableStates => s_SelectableStates ? s_SelectableStates : s_SelectableStates = GetTexture2D(TextureName.SelectableStates);
                private static Texture2D s_Selected;
                public static Texture2D Selected => s_Selected ? s_Selected : s_Selected = GetTexture2D(TextureName.Selected);
                private static Texture2D s_ToggleMixed;
                public static Texture2D ToggleMixed => s_ToggleMixed ? s_ToggleMixed : s_ToggleMixed = GetTexture2D(TextureName.ToggleMixed);
                private static Texture2D s_ToggleOFF;
                public static Texture2D ToggleOFF => s_ToggleOFF ? s_ToggleOFF : s_ToggleOFF = GetTexture2D(TextureName.ToggleOFF);
                private static Texture2D s_ToggleON;
                public static Texture2D ToggleON => s_ToggleON ? s_ToggleON : s_ToggleON = GetTexture2D(TextureName.ToggleON);
                private static Texture2D s_UIBehaviour;
                public static Texture2D UIBehaviour => s_UIBehaviour ? s_UIBehaviour : s_UIBehaviour = GetTexture2D(TextureName.UIBehaviour);
                private static Texture2D s_UISelectable;
                public static Texture2D UISelectable => s_UISelectable ? s_UISelectable : s_UISelectable = GetTexture2D(TextureName.UISelectable);
                private static Texture2D s_Unity;
                public static Texture2D Unity => s_Unity ? s_Unity : s_Unity = GetTexture2D(TextureName.Unity);
                private static Texture2D s_UnityEvent;
                public static Texture2D UnityEvent => s_UnityEvent ? s_UnityEvent : s_UnityEvent = GetTexture2D(TextureName.UnityEvent);
                private static Texture2D s_Unlocked;
                public static Texture2D Unlocked => s_Unlocked ? s_Unlocked : s_Unlocked = GetTexture2D(TextureName.Unlocked);
                private static Texture2D s_VisibilityChanged;
                public static Texture2D VisibilityChanged => s_VisibilityChanged ? s_VisibilityChanged : s_VisibilityChanged = GetTexture2D(TextureName.VisibilityChanged);
                
            }

            public static class Placeholders
            {
                private static EditorDataTextureGroup s_textureGroup;
                private static EditorDataTextureGroup textureGroup =>
                    s_textureGroup
                        ? s_textureGroup
                        : s_textureGroup = EditorDataTextureDatabase.GetTextureGroup("EditorUI","Placeholders");

                public static Texture2D GetTexture2D(TextureName textureName) =>
                    textureGroup.GetTexture(textureName.ToString());

                public enum TextureName
                {
                    TransparencyGrid
                }
                

                private static Texture2D s_TransparencyGrid;
                public static Texture2D TransparencyGrid => s_TransparencyGrid ? s_TransparencyGrid : s_TransparencyGrid = GetTexture2D(TextureName.TransparencyGrid);
                
            }

            public static class Pointers
            {
                private static EditorDataTextureGroup s_textureGroup;
                private static EditorDataTextureGroup textureGroup =>
                    s_textureGroup
                        ? s_textureGroup
                        : s_textureGroup = EditorDataTextureDatabase.GetTextureGroup("EditorUI","Pointers");

                public static Texture2D GetTexture2D(TextureName textureName) =>
                    textureGroup.GetTexture(textureName.ToString());

                public enum TextureName
                {
                    PointerDown,
                    PointerLeft,
                    PointerRight,
                    PointerUp
                }
                

                private static Texture2D s_PointerDown;
                public static Texture2D PointerDown => s_PointerDown ? s_PointerDown : s_PointerDown = GetTexture2D(TextureName.PointerDown);
                private static Texture2D s_PointerLeft;
                public static Texture2D PointerLeft => s_PointerLeft ? s_PointerLeft : s_PointerLeft = GetTexture2D(TextureName.PointerLeft);
                private static Texture2D s_PointerRight;
                public static Texture2D PointerRight => s_PointerRight ? s_PointerRight : s_PointerRight = GetTexture2D(TextureName.PointerRight);
                private static Texture2D s_PointerUp;
                public static Texture2D PointerUp => s_PointerUp ? s_PointerUp : s_PointerUp = GetTexture2D(TextureName.PointerUp);
                
            }

            public static class Widgets
            {
                private static EditorDataTextureGroup s_textureGroup;
                private static EditorDataTextureGroup textureGroup =>
                    s_textureGroup
                        ? s_textureGroup
                        : s_textureGroup = EditorDataTextureDatabase.GetTextureGroup("EditorUI","Widgets");

                public static Texture2D GetTexture2D(TextureName textureName) =>
                    textureGroup.GetTexture(textureName.ToString());

                public enum TextureName
                {
                    CircularGaugeBody,
                    CircularGaugeFillBackground
                }
                

                private static Texture2D s_CircularGaugeBody;
                public static Texture2D CircularGaugeBody => s_CircularGaugeBody ? s_CircularGaugeBody : s_CircularGaugeBody = GetTexture2D(TextureName.CircularGaugeBody);
                private static Texture2D s_CircularGaugeFillBackground;
                public static Texture2D CircularGaugeFillBackground => s_CircularGaugeFillBackground ? s_CircularGaugeFillBackground : s_CircularGaugeFillBackground = GetTexture2D(TextureName.CircularGaugeFillBackground);
                
            }


        }


        public static class Mody
        {
            public static class Icons
            {
                private static EditorDataTextureGroup s_textureGroup;
                private static EditorDataTextureGroup textureGroup =>
                    s_textureGroup
                        ? s_textureGroup
                        : s_textureGroup = EditorDataTextureDatabase.GetTextureGroup("Mody","Icons");

                public static Texture2D GetTexture2D(TextureName textureName) =>
                    textureGroup.GetTexture(textureName.ToString());

                public enum TextureName
                {
                    ModyAction,
                    ModyModule,
                    ModyTrigger
                }
                

                private static Texture2D s_ModyAction;
                public static Texture2D ModyAction => s_ModyAction ? s_ModyAction : s_ModyAction = GetTexture2D(TextureName.ModyAction);
                private static Texture2D s_ModyModule;
                public static Texture2D ModyModule => s_ModyModule ? s_ModyModule : s_ModyModule = GetTexture2D(TextureName.ModyModule);
                private static Texture2D s_ModyTrigger;
                public static Texture2D ModyTrigger => s_ModyTrigger ? s_ModyTrigger : s_ModyTrigger = GetTexture2D(TextureName.ModyTrigger);
                
            }


        }


        public static class Nody
        {
            public static class Icons
            {
                private static EditorDataTextureGroup s_textureGroup;
                private static EditorDataTextureGroup textureGroup =>
                    s_textureGroup
                        ? s_textureGroup
                        : s_textureGroup = EditorDataTextureDatabase.GetTextureGroup("Nody","Icons");

                public static Texture2D GetTexture2D(TextureName textureName) =>
                    textureGroup.GetTexture(textureName.ToString());

                public enum TextureName
                {
                    ActivateLoadedScenesNode,
                    ApplicationQuitNode,
                    BackButtonNode,
                    CustomNode,
                    DebugNode,
                    EnterNode,
                    ExitNode,
                    FlowController,
                    FlowGraph,
                    GameEventNode,
                    GroupNode,
                    Infinity,
                    LoadSceneNode,
                    MarkerNode,
                    Minimap,
                    Nody,
                    One,
                    PivotNode,
                    PortalNode,
                    RandomNode,
                    SignalNode,
                    SoundNode,
                    StartNode,
                    StickyNoteNode,
                    SwitchBackNode,
                    TimescaleNode,
                    UINode,
                    UnloadSceneNode,
                    WaitNode
                }
                

                private static Texture2D s_ActivateLoadedScenesNode;
                public static Texture2D ActivateLoadedScenesNode => s_ActivateLoadedScenesNode ? s_ActivateLoadedScenesNode : s_ActivateLoadedScenesNode = GetTexture2D(TextureName.ActivateLoadedScenesNode);
                private static Texture2D s_ApplicationQuitNode;
                public static Texture2D ApplicationQuitNode => s_ApplicationQuitNode ? s_ApplicationQuitNode : s_ApplicationQuitNode = GetTexture2D(TextureName.ApplicationQuitNode);
                private static Texture2D s_BackButtonNode;
                public static Texture2D BackButtonNode => s_BackButtonNode ? s_BackButtonNode : s_BackButtonNode = GetTexture2D(TextureName.BackButtonNode);
                private static Texture2D s_CustomNode;
                public static Texture2D CustomNode => s_CustomNode ? s_CustomNode : s_CustomNode = GetTexture2D(TextureName.CustomNode);
                private static Texture2D s_DebugNode;
                public static Texture2D DebugNode => s_DebugNode ? s_DebugNode : s_DebugNode = GetTexture2D(TextureName.DebugNode);
                private static Texture2D s_EnterNode;
                public static Texture2D EnterNode => s_EnterNode ? s_EnterNode : s_EnterNode = GetTexture2D(TextureName.EnterNode);
                private static Texture2D s_ExitNode;
                public static Texture2D ExitNode => s_ExitNode ? s_ExitNode : s_ExitNode = GetTexture2D(TextureName.ExitNode);
                private static Texture2D s_FlowController;
                public static Texture2D FlowController => s_FlowController ? s_FlowController : s_FlowController = GetTexture2D(TextureName.FlowController);
                private static Texture2D s_FlowGraph;
                public static Texture2D FlowGraph => s_FlowGraph ? s_FlowGraph : s_FlowGraph = GetTexture2D(TextureName.FlowGraph);
                private static Texture2D s_GameEventNode;
                public static Texture2D GameEventNode => s_GameEventNode ? s_GameEventNode : s_GameEventNode = GetTexture2D(TextureName.GameEventNode);
                private static Texture2D s_GroupNode;
                public static Texture2D GroupNode => s_GroupNode ? s_GroupNode : s_GroupNode = GetTexture2D(TextureName.GroupNode);
                private static Texture2D s_Infinity;
                public static Texture2D Infinity => s_Infinity ? s_Infinity : s_Infinity = GetTexture2D(TextureName.Infinity);
                private static Texture2D s_LoadSceneNode;
                public static Texture2D LoadSceneNode => s_LoadSceneNode ? s_LoadSceneNode : s_LoadSceneNode = GetTexture2D(TextureName.LoadSceneNode);
                private static Texture2D s_MarkerNode;
                public static Texture2D MarkerNode => s_MarkerNode ? s_MarkerNode : s_MarkerNode = GetTexture2D(TextureName.MarkerNode);
                private static Texture2D s_Minimap;
                public static Texture2D Minimap => s_Minimap ? s_Minimap : s_Minimap = GetTexture2D(TextureName.Minimap);
                private static Texture2D s_Nody;
                public static Texture2D Nody => s_Nody ? s_Nody : s_Nody = GetTexture2D(TextureName.Nody);
                private static Texture2D s_One;
                public static Texture2D One => s_One ? s_One : s_One = GetTexture2D(TextureName.One);
                private static Texture2D s_PivotNode;
                public static Texture2D PivotNode => s_PivotNode ? s_PivotNode : s_PivotNode = GetTexture2D(TextureName.PivotNode);
                private static Texture2D s_PortalNode;
                public static Texture2D PortalNode => s_PortalNode ? s_PortalNode : s_PortalNode = GetTexture2D(TextureName.PortalNode);
                private static Texture2D s_RandomNode;
                public static Texture2D RandomNode => s_RandomNode ? s_RandomNode : s_RandomNode = GetTexture2D(TextureName.RandomNode);
                private static Texture2D s_SignalNode;
                public static Texture2D SignalNode => s_SignalNode ? s_SignalNode : s_SignalNode = GetTexture2D(TextureName.SignalNode);
                private static Texture2D s_SoundNode;
                public static Texture2D SoundNode => s_SoundNode ? s_SoundNode : s_SoundNode = GetTexture2D(TextureName.SoundNode);
                private static Texture2D s_StartNode;
                public static Texture2D StartNode => s_StartNode ? s_StartNode : s_StartNode = GetTexture2D(TextureName.StartNode);
                private static Texture2D s_StickyNoteNode;
                public static Texture2D StickyNoteNode => s_StickyNoteNode ? s_StickyNoteNode : s_StickyNoteNode = GetTexture2D(TextureName.StickyNoteNode);
                private static Texture2D s_SwitchBackNode;
                public static Texture2D SwitchBackNode => s_SwitchBackNode ? s_SwitchBackNode : s_SwitchBackNode = GetTexture2D(TextureName.SwitchBackNode);
                private static Texture2D s_TimescaleNode;
                public static Texture2D TimescaleNode => s_TimescaleNode ? s_TimescaleNode : s_TimescaleNode = GetTexture2D(TextureName.TimescaleNode);
                private static Texture2D s_UINode;
                public static Texture2D UINode => s_UINode ? s_UINode : s_UINode = GetTexture2D(TextureName.UINode);
                private static Texture2D s_UnloadSceneNode;
                public static Texture2D UnloadSceneNode => s_UnloadSceneNode ? s_UnloadSceneNode : s_UnloadSceneNode = GetTexture2D(TextureName.UnloadSceneNode);
                private static Texture2D s_WaitNode;
                public static Texture2D WaitNode => s_WaitNode ? s_WaitNode : s_WaitNode = GetTexture2D(TextureName.WaitNode);
                
            }


        }


        public static class Reactor
        {
            public static class Actions
            {
                private static EditorDataTextureGroup s_textureGroup;
                private static EditorDataTextureGroup textureGroup =>
                    s_textureGroup
                        ? s_textureGroup
                        : s_textureGroup = EditorDataTextureDatabase.GetTextureGroup("Reactor","Actions");

                public static Texture2D GetTexture2D(TextureName textureName) =>
                    textureGroup.GetTexture(textureName.ToString());

                public enum TextureName
                {
                    Fade,
                    Move,
                    Rotate,
                    Scale
                }
                

                private static Texture2D s_Fade;
                public static Texture2D Fade => s_Fade ? s_Fade : s_Fade = GetTexture2D(TextureName.Fade);
                private static Texture2D s_Move;
                public static Texture2D Move => s_Move ? s_Move : s_Move = GetTexture2D(TextureName.Move);
                private static Texture2D s_Rotate;
                public static Texture2D Rotate => s_Rotate ? s_Rotate : s_Rotate = GetTexture2D(TextureName.Rotate);
                private static Texture2D s_Scale;
                public static Texture2D Scale => s_Scale ? s_Scale : s_Scale = GetTexture2D(TextureName.Scale);
                
            }

            public static class Icons
            {
                private static EditorDataTextureGroup s_textureGroup;
                private static EditorDataTextureGroup textureGroup =>
                    s_textureGroup
                        ? s_textureGroup
                        : s_textureGroup = EditorDataTextureDatabase.GetTextureGroup("Reactor","Icons");

                public static Texture2D GetTexture2D(TextureName textureName) =>
                    textureGroup.GetTexture(textureName.ToString());

                public enum TextureName
                {
                    AnimatorProgressTarget,
                    AudioMixerProgressTarget,
                    ColorAnimation,
                    ColorAnimator,
                    ColorTarget,
                    EditorHeartbeat,
                    FrameByFrameAnimation,
                    FrameByFrameAnimator,
                    Heartbeat,
                    ImageProgressTarget,
                    ProgressTarget,
                    Reactor,
                    SignalProgressTarget,
                    SpriteAnimation,
                    SpriteAnimator,
                    SpriteTarget,
                    TextMeshProProgressTarget,
                    TextProgressTarget,
                    UIAnimation,
                    UIAnimationData,
                    UIAnimator,
                    UnityEventProgressTarget
                }
                

                private static Texture2D s_AnimatorProgressTarget;
                public static Texture2D AnimatorProgressTarget => s_AnimatorProgressTarget ? s_AnimatorProgressTarget : s_AnimatorProgressTarget = GetTexture2D(TextureName.AnimatorProgressTarget);
                private static Texture2D s_AudioMixerProgressTarget;
                public static Texture2D AudioMixerProgressTarget => s_AudioMixerProgressTarget ? s_AudioMixerProgressTarget : s_AudioMixerProgressTarget = GetTexture2D(TextureName.AudioMixerProgressTarget);
                private static Texture2D s_ColorAnimation;
                public static Texture2D ColorAnimation => s_ColorAnimation ? s_ColorAnimation : s_ColorAnimation = GetTexture2D(TextureName.ColorAnimation);
                private static Texture2D s_ColorAnimator;
                public static Texture2D ColorAnimator => s_ColorAnimator ? s_ColorAnimator : s_ColorAnimator = GetTexture2D(TextureName.ColorAnimator);
                private static Texture2D s_ColorTarget;
                public static Texture2D ColorTarget => s_ColorTarget ? s_ColorTarget : s_ColorTarget = GetTexture2D(TextureName.ColorTarget);
                private static Texture2D s_EditorHeartbeat;
                public static Texture2D EditorHeartbeat => s_EditorHeartbeat ? s_EditorHeartbeat : s_EditorHeartbeat = GetTexture2D(TextureName.EditorHeartbeat);
                private static Texture2D s_FrameByFrameAnimation;
                public static Texture2D FrameByFrameAnimation => s_FrameByFrameAnimation ? s_FrameByFrameAnimation : s_FrameByFrameAnimation = GetTexture2D(TextureName.FrameByFrameAnimation);
                private static Texture2D s_FrameByFrameAnimator;
                public static Texture2D FrameByFrameAnimator => s_FrameByFrameAnimator ? s_FrameByFrameAnimator : s_FrameByFrameAnimator = GetTexture2D(TextureName.FrameByFrameAnimator);
                private static Texture2D s_Heartbeat;
                public static Texture2D Heartbeat => s_Heartbeat ? s_Heartbeat : s_Heartbeat = GetTexture2D(TextureName.Heartbeat);
                private static Texture2D s_ImageProgressTarget;
                public static Texture2D ImageProgressTarget => s_ImageProgressTarget ? s_ImageProgressTarget : s_ImageProgressTarget = GetTexture2D(TextureName.ImageProgressTarget);
                private static Texture2D s_ProgressTarget;
                public static Texture2D ProgressTarget => s_ProgressTarget ? s_ProgressTarget : s_ProgressTarget = GetTexture2D(TextureName.ProgressTarget);
                private static Texture2D s_Reactor;
                public static Texture2D Reactor => s_Reactor ? s_Reactor : s_Reactor = GetTexture2D(TextureName.Reactor);
                private static Texture2D s_SignalProgressTarget;
                public static Texture2D SignalProgressTarget => s_SignalProgressTarget ? s_SignalProgressTarget : s_SignalProgressTarget = GetTexture2D(TextureName.SignalProgressTarget);
                private static Texture2D s_SpriteAnimation;
                public static Texture2D SpriteAnimation => s_SpriteAnimation ? s_SpriteAnimation : s_SpriteAnimation = GetTexture2D(TextureName.SpriteAnimation);
                private static Texture2D s_SpriteAnimator;
                public static Texture2D SpriteAnimator => s_SpriteAnimator ? s_SpriteAnimator : s_SpriteAnimator = GetTexture2D(TextureName.SpriteAnimator);
                private static Texture2D s_SpriteTarget;
                public static Texture2D SpriteTarget => s_SpriteTarget ? s_SpriteTarget : s_SpriteTarget = GetTexture2D(TextureName.SpriteTarget);
                private static Texture2D s_TextMeshProProgressTarget;
                public static Texture2D TextMeshProProgressTarget => s_TextMeshProProgressTarget ? s_TextMeshProProgressTarget : s_TextMeshProProgressTarget = GetTexture2D(TextureName.TextMeshProProgressTarget);
                private static Texture2D s_TextProgressTarget;
                public static Texture2D TextProgressTarget => s_TextProgressTarget ? s_TextProgressTarget : s_TextProgressTarget = GetTexture2D(TextureName.TextProgressTarget);
                private static Texture2D s_UIAnimation;
                public static Texture2D UIAnimation => s_UIAnimation ? s_UIAnimation : s_UIAnimation = GetTexture2D(TextureName.UIAnimation);
                private static Texture2D s_UIAnimationData;
                public static Texture2D UIAnimationData => s_UIAnimationData ? s_UIAnimationData : s_UIAnimationData = GetTexture2D(TextureName.UIAnimationData);
                private static Texture2D s_UIAnimator;
                public static Texture2D UIAnimator => s_UIAnimator ? s_UIAnimator : s_UIAnimator = GetTexture2D(TextureName.UIAnimator);
                private static Texture2D s_UnityEventProgressTarget;
                public static Texture2D UnityEventProgressTarget => s_UnityEventProgressTarget ? s_UnityEventProgressTarget : s_UnityEventProgressTarget = GetTexture2D(TextureName.UnityEventProgressTarget);
                
            }

            public static class Player
            {
                private static EditorDataTextureGroup s_textureGroup;
                private static EditorDataTextureGroup textureGroup =>
                    s_textureGroup
                        ? s_textureGroup
                        : s_textureGroup = EditorDataTextureDatabase.GetTextureGroup("Reactor","Player");

                public static Texture2D GetTexture2D(TextureName textureName) =>
                    textureGroup.GetTexture(textureName.ToString());

                public enum TextureName
                {
                    Duration,
                    FirstFrame,
                    IconCooldown,
                    IconDelayBetweenLoops,
                    IconDuration,
                    IconStartDelay,
                    LastFrame,
                    Loop,
                    OneShot,
                    Pause,
                    PingPong,
                    PingPongOnce,
                    Play,
                    PlayForward,
                    PlayReverse,
                    Reverse,
                    Shake,
                    Spring,
                    Stop
                }
                

                private static Texture2D s_Duration;
                public static Texture2D Duration => s_Duration ? s_Duration : s_Duration = GetTexture2D(TextureName.Duration);
                private static Texture2D s_FirstFrame;
                public static Texture2D FirstFrame => s_FirstFrame ? s_FirstFrame : s_FirstFrame = GetTexture2D(TextureName.FirstFrame);
                private static Texture2D s_IconCooldown;
                public static Texture2D IconCooldown => s_IconCooldown ? s_IconCooldown : s_IconCooldown = GetTexture2D(TextureName.IconCooldown);
                private static Texture2D s_IconDelayBetweenLoops;
                public static Texture2D IconDelayBetweenLoops => s_IconDelayBetweenLoops ? s_IconDelayBetweenLoops : s_IconDelayBetweenLoops = GetTexture2D(TextureName.IconDelayBetweenLoops);
                private static Texture2D s_IconDuration;
                public static Texture2D IconDuration => s_IconDuration ? s_IconDuration : s_IconDuration = GetTexture2D(TextureName.IconDuration);
                private static Texture2D s_IconStartDelay;
                public static Texture2D IconStartDelay => s_IconStartDelay ? s_IconStartDelay : s_IconStartDelay = GetTexture2D(TextureName.IconStartDelay);
                private static Texture2D s_LastFrame;
                public static Texture2D LastFrame => s_LastFrame ? s_LastFrame : s_LastFrame = GetTexture2D(TextureName.LastFrame);
                private static Texture2D s_Loop;
                public static Texture2D Loop => s_Loop ? s_Loop : s_Loop = GetTexture2D(TextureName.Loop);
                private static Texture2D s_OneShot;
                public static Texture2D OneShot => s_OneShot ? s_OneShot : s_OneShot = GetTexture2D(TextureName.OneShot);
                private static Texture2D s_Pause;
                public static Texture2D Pause => s_Pause ? s_Pause : s_Pause = GetTexture2D(TextureName.Pause);
                private static Texture2D s_PingPong;
                public static Texture2D PingPong => s_PingPong ? s_PingPong : s_PingPong = GetTexture2D(TextureName.PingPong);
                private static Texture2D s_PingPongOnce;
                public static Texture2D PingPongOnce => s_PingPongOnce ? s_PingPongOnce : s_PingPongOnce = GetTexture2D(TextureName.PingPongOnce);
                private static Texture2D s_Play;
                public static Texture2D Play => s_Play ? s_Play : s_Play = GetTexture2D(TextureName.Play);
                private static Texture2D s_PlayForward;
                public static Texture2D PlayForward => s_PlayForward ? s_PlayForward : s_PlayForward = GetTexture2D(TextureName.PlayForward);
                private static Texture2D s_PlayReverse;
                public static Texture2D PlayReverse => s_PlayReverse ? s_PlayReverse : s_PlayReverse = GetTexture2D(TextureName.PlayReverse);
                private static Texture2D s_Reverse;
                public static Texture2D Reverse => s_Reverse ? s_Reverse : s_Reverse = GetTexture2D(TextureName.Reverse);
                private static Texture2D s_Shake;
                public static Texture2D Shake => s_Shake ? s_Shake : s_Shake = GetTexture2D(TextureName.Shake);
                private static Texture2D s_Spring;
                public static Texture2D Spring => s_Spring ? s_Spring : s_Spring = GetTexture2D(TextureName.Spring);
                private static Texture2D s_Stop;
                public static Texture2D Stop => s_Stop ? s_Stop : s_Stop = GetTexture2D(TextureName.Stop);
                
            }


        }


        public static class Signals
        {
            public static class Icons
            {
                private static EditorDataTextureGroup s_textureGroup;
                private static EditorDataTextureGroup textureGroup =>
                    s_textureGroup
                        ? s_textureGroup
                        : s_textureGroup = EditorDataTextureDatabase.GetTextureGroup("Signals","Icons");

                public static Texture2D GetTexture2D(TextureName textureName) =>
                    textureGroup.GetTexture(textureName.ToString());

                public enum TextureName
                {
                    MetaSignal,
                    MultiSignalReceiver,
                    Signal,
                    SignalBroadcaster,
                    SignalProvider,
                    SignalReceiver,
                    SignalSender,
                    SignalStream,
                    StreamDatabase
                }
                

                private static Texture2D s_MetaSignal;
                public static Texture2D MetaSignal => s_MetaSignal ? s_MetaSignal : s_MetaSignal = GetTexture2D(TextureName.MetaSignal);
                private static Texture2D s_MultiSignalReceiver;
                public static Texture2D MultiSignalReceiver => s_MultiSignalReceiver ? s_MultiSignalReceiver : s_MultiSignalReceiver = GetTexture2D(TextureName.MultiSignalReceiver);
                private static Texture2D s_Signal;
                public static Texture2D Signal => s_Signal ? s_Signal : s_Signal = GetTexture2D(TextureName.Signal);
                private static Texture2D s_SignalBroadcaster;
                public static Texture2D SignalBroadcaster => s_SignalBroadcaster ? s_SignalBroadcaster : s_SignalBroadcaster = GetTexture2D(TextureName.SignalBroadcaster);
                private static Texture2D s_SignalProvider;
                public static Texture2D SignalProvider => s_SignalProvider ? s_SignalProvider : s_SignalProvider = GetTexture2D(TextureName.SignalProvider);
                private static Texture2D s_SignalReceiver;
                public static Texture2D SignalReceiver => s_SignalReceiver ? s_SignalReceiver : s_SignalReceiver = GetTexture2D(TextureName.SignalReceiver);
                private static Texture2D s_SignalSender;
                public static Texture2D SignalSender => s_SignalSender ? s_SignalSender : s_SignalSender = GetTexture2D(TextureName.SignalSender);
                private static Texture2D s_SignalStream;
                public static Texture2D SignalStream => s_SignalStream ? s_SignalStream : s_SignalStream = GetTexture2D(TextureName.SignalStream);
                private static Texture2D s_StreamDatabase;
                public static Texture2D StreamDatabase => s_StreamDatabase ? s_StreamDatabase : s_StreamDatabase = GetTexture2D(TextureName.StreamDatabase);
                
            }


        }


        public static class UIManager
        {
            public static class Icons
            {
                private static EditorDataTextureGroup s_textureGroup;
                private static EditorDataTextureGroup textureGroup =>
                    s_textureGroup
                        ? s_textureGroup
                        : s_textureGroup = EditorDataTextureDatabase.GetTextureGroup("UIManager","Icons");

                public static Texture2D GetTexture2D(TextureName textureName) =>
                    textureGroup.GetTexture(textureName.ToString());

                public enum TextureName
                {
                    Alert,
                    AlertDatabase,
                    BackButton,
                    ButtonsDatabase,
                    InputToSignal,
                    MultiplayerInfo,
                    PopupDatabase,
                    SignalListener,
                    SignalToAudioSource,
                    SignalToColorTarget,
                    SignalToSpriteTarget,
                    SlidersDatabase,
                    SpriteSwapper,
                    TogglesDatabase,
                    Tooltip,
                    UIButtonListener,
                    UIContainer,
                    UIScrollbar,
                    UISelectableAnimator,
                    UIToggleCheckbox,
                    UIToggleGroup,
                    UIToggleListener,
                    UIToggleRadio,
                    UIToggleSwitch,
                    UIViewListener,
                    ViewsDatabase
                }
                

                private static Texture2D s_Alert;
                public static Texture2D Alert => s_Alert ? s_Alert : s_Alert = GetTexture2D(TextureName.Alert);
                private static Texture2D s_AlertDatabase;
                public static Texture2D AlertDatabase => s_AlertDatabase ? s_AlertDatabase : s_AlertDatabase = GetTexture2D(TextureName.AlertDatabase);
                private static Texture2D s_BackButton;
                public static Texture2D BackButton => s_BackButton ? s_BackButton : s_BackButton = GetTexture2D(TextureName.BackButton);
                private static Texture2D s_ButtonsDatabase;
                public static Texture2D ButtonsDatabase => s_ButtonsDatabase ? s_ButtonsDatabase : s_ButtonsDatabase = GetTexture2D(TextureName.ButtonsDatabase);
                private static Texture2D s_InputToSignal;
                public static Texture2D InputToSignal => s_InputToSignal ? s_InputToSignal : s_InputToSignal = GetTexture2D(TextureName.InputToSignal);
                private static Texture2D s_MultiplayerInfo;
                public static Texture2D MultiplayerInfo => s_MultiplayerInfo ? s_MultiplayerInfo : s_MultiplayerInfo = GetTexture2D(TextureName.MultiplayerInfo);
                private static Texture2D s_PopupDatabase;
                public static Texture2D PopupDatabase => s_PopupDatabase ? s_PopupDatabase : s_PopupDatabase = GetTexture2D(TextureName.PopupDatabase);
                private static Texture2D s_SignalListener;
                public static Texture2D SignalListener => s_SignalListener ? s_SignalListener : s_SignalListener = GetTexture2D(TextureName.SignalListener);
                private static Texture2D s_SignalToAudioSource;
                public static Texture2D SignalToAudioSource => s_SignalToAudioSource ? s_SignalToAudioSource : s_SignalToAudioSource = GetTexture2D(TextureName.SignalToAudioSource);
                private static Texture2D s_SignalToColorTarget;
                public static Texture2D SignalToColorTarget => s_SignalToColorTarget ? s_SignalToColorTarget : s_SignalToColorTarget = GetTexture2D(TextureName.SignalToColorTarget);
                private static Texture2D s_SignalToSpriteTarget;
                public static Texture2D SignalToSpriteTarget => s_SignalToSpriteTarget ? s_SignalToSpriteTarget : s_SignalToSpriteTarget = GetTexture2D(TextureName.SignalToSpriteTarget);
                private static Texture2D s_SlidersDatabase;
                public static Texture2D SlidersDatabase => s_SlidersDatabase ? s_SlidersDatabase : s_SlidersDatabase = GetTexture2D(TextureName.SlidersDatabase);
                private static Texture2D s_SpriteSwapper;
                public static Texture2D SpriteSwapper => s_SpriteSwapper ? s_SpriteSwapper : s_SpriteSwapper = GetTexture2D(TextureName.SpriteSwapper);
                private static Texture2D s_TogglesDatabase;
                public static Texture2D TogglesDatabase => s_TogglesDatabase ? s_TogglesDatabase : s_TogglesDatabase = GetTexture2D(TextureName.TogglesDatabase);
                private static Texture2D s_Tooltip;
                public static Texture2D Tooltip => s_Tooltip ? s_Tooltip : s_Tooltip = GetTexture2D(TextureName.Tooltip);
                private static Texture2D s_UIButtonListener;
                public static Texture2D UIButtonListener => s_UIButtonListener ? s_UIButtonListener : s_UIButtonListener = GetTexture2D(TextureName.UIButtonListener);
                private static Texture2D s_UIContainer;
                public static Texture2D UIContainer => s_UIContainer ? s_UIContainer : s_UIContainer = GetTexture2D(TextureName.UIContainer);
                private static Texture2D s_UIScrollbar;
                public static Texture2D UIScrollbar => s_UIScrollbar ? s_UIScrollbar : s_UIScrollbar = GetTexture2D(TextureName.UIScrollbar);
                private static Texture2D s_UISelectableAnimator;
                public static Texture2D UISelectableAnimator => s_UISelectableAnimator ? s_UISelectableAnimator : s_UISelectableAnimator = GetTexture2D(TextureName.UISelectableAnimator);
                private static Texture2D s_UIToggleCheckbox;
                public static Texture2D UIToggleCheckbox => s_UIToggleCheckbox ? s_UIToggleCheckbox : s_UIToggleCheckbox = GetTexture2D(TextureName.UIToggleCheckbox);
                private static Texture2D s_UIToggleGroup;
                public static Texture2D UIToggleGroup => s_UIToggleGroup ? s_UIToggleGroup : s_UIToggleGroup = GetTexture2D(TextureName.UIToggleGroup);
                private static Texture2D s_UIToggleListener;
                public static Texture2D UIToggleListener => s_UIToggleListener ? s_UIToggleListener : s_UIToggleListener = GetTexture2D(TextureName.UIToggleListener);
                private static Texture2D s_UIToggleRadio;
                public static Texture2D UIToggleRadio => s_UIToggleRadio ? s_UIToggleRadio : s_UIToggleRadio = GetTexture2D(TextureName.UIToggleRadio);
                private static Texture2D s_UIToggleSwitch;
                public static Texture2D UIToggleSwitch => s_UIToggleSwitch ? s_UIToggleSwitch : s_UIToggleSwitch = GetTexture2D(TextureName.UIToggleSwitch);
                private static Texture2D s_UIViewListener;
                public static Texture2D UIViewListener => s_UIViewListener ? s_UIViewListener : s_UIViewListener = GetTexture2D(TextureName.UIViewListener);
                private static Texture2D s_ViewsDatabase;
                public static Texture2D ViewsDatabase => s_ViewsDatabase ? s_ViewsDatabase : s_ViewsDatabase = GetTexture2D(TextureName.ViewsDatabase);
                
            }


        }


        public static class UIMenuIcons
        {
            public static class UIMenu
            {
                private static EditorDataTextureGroup s_textureGroup;
                private static EditorDataTextureGroup textureGroup =>
                    s_textureGroup
                        ? s_textureGroup
                        : s_textureGroup = EditorDataTextureDatabase.GetTextureGroup("UIMenuIcons","UIMenu");

                public static Texture2D GetTexture2D(TextureName textureName) =>
                    textureGroup.GetTexture(textureName.ToString());

                public enum TextureName
                {
                    Button,
                    Checkbox,
                    Component,
                    Container,
                    Content,
                    Custom,
                    Dropdown,
                    GridLayout,
                    HorizontalLayout,
                    InputField,
                    RadialLayout,
                    RadioButton,
                    Scollbar,
                    ScrollView,
                    Slider,
                    Switch,
                    TabButtonBottomLeft,
                    TabButtonBottomRight,
                    TabButtonLeftBottom,
                    TabButtonLeftFloating,
                    TabButtonLeftTop,
                    TabButtonMiddleBottom,
                    TabButtonMiddleFloating,
                    TabButtonMiddleLeft,
                    TabButtonMiddleRight,
                    TabButtonMiddleTop,
                    TabButtonRightBottom,
                    TabButtonRightFloating,
                    TabButtonRightTop,
                    TabButtonTopLeft,
                    TabButtonTopRight,
                    UIMenuHeader128x32,
                    VerticalLayout
                }
                

                private static Texture2D s_Button;
                public static Texture2D Button => s_Button ? s_Button : s_Button = GetTexture2D(TextureName.Button);
                private static Texture2D s_Checkbox;
                public static Texture2D Checkbox => s_Checkbox ? s_Checkbox : s_Checkbox = GetTexture2D(TextureName.Checkbox);
                private static Texture2D s_Component;
                public static Texture2D Component => s_Component ? s_Component : s_Component = GetTexture2D(TextureName.Component);
                private static Texture2D s_Container;
                public static Texture2D Container => s_Container ? s_Container : s_Container = GetTexture2D(TextureName.Container);
                private static Texture2D s_Content;
                public static Texture2D Content => s_Content ? s_Content : s_Content = GetTexture2D(TextureName.Content);
                private static Texture2D s_Custom;
                public static Texture2D Custom => s_Custom ? s_Custom : s_Custom = GetTexture2D(TextureName.Custom);
                private static Texture2D s_Dropdown;
                public static Texture2D Dropdown => s_Dropdown ? s_Dropdown : s_Dropdown = GetTexture2D(TextureName.Dropdown);
                private static Texture2D s_GridLayout;
                public static Texture2D GridLayout => s_GridLayout ? s_GridLayout : s_GridLayout = GetTexture2D(TextureName.GridLayout);
                private static Texture2D s_HorizontalLayout;
                public static Texture2D HorizontalLayout => s_HorizontalLayout ? s_HorizontalLayout : s_HorizontalLayout = GetTexture2D(TextureName.HorizontalLayout);
                private static Texture2D s_InputField;
                public static Texture2D InputField => s_InputField ? s_InputField : s_InputField = GetTexture2D(TextureName.InputField);
                private static Texture2D s_RadialLayout;
                public static Texture2D RadialLayout => s_RadialLayout ? s_RadialLayout : s_RadialLayout = GetTexture2D(TextureName.RadialLayout);
                private static Texture2D s_RadioButton;
                public static Texture2D RadioButton => s_RadioButton ? s_RadioButton : s_RadioButton = GetTexture2D(TextureName.RadioButton);
                private static Texture2D s_Scollbar;
                public static Texture2D Scollbar => s_Scollbar ? s_Scollbar : s_Scollbar = GetTexture2D(TextureName.Scollbar);
                private static Texture2D s_ScrollView;
                public static Texture2D ScrollView => s_ScrollView ? s_ScrollView : s_ScrollView = GetTexture2D(TextureName.ScrollView);
                private static Texture2D s_Slider;
                public static Texture2D Slider => s_Slider ? s_Slider : s_Slider = GetTexture2D(TextureName.Slider);
                private static Texture2D s_Switch;
                public static Texture2D Switch => s_Switch ? s_Switch : s_Switch = GetTexture2D(TextureName.Switch);
                private static Texture2D s_TabButtonBottomLeft;
                public static Texture2D TabButtonBottomLeft => s_TabButtonBottomLeft ? s_TabButtonBottomLeft : s_TabButtonBottomLeft = GetTexture2D(TextureName.TabButtonBottomLeft);
                private static Texture2D s_TabButtonBottomRight;
                public static Texture2D TabButtonBottomRight => s_TabButtonBottomRight ? s_TabButtonBottomRight : s_TabButtonBottomRight = GetTexture2D(TextureName.TabButtonBottomRight);
                private static Texture2D s_TabButtonLeftBottom;
                public static Texture2D TabButtonLeftBottom => s_TabButtonLeftBottom ? s_TabButtonLeftBottom : s_TabButtonLeftBottom = GetTexture2D(TextureName.TabButtonLeftBottom);
                private static Texture2D s_TabButtonLeftFloating;
                public static Texture2D TabButtonLeftFloating => s_TabButtonLeftFloating ? s_TabButtonLeftFloating : s_TabButtonLeftFloating = GetTexture2D(TextureName.TabButtonLeftFloating);
                private static Texture2D s_TabButtonLeftTop;
                public static Texture2D TabButtonLeftTop => s_TabButtonLeftTop ? s_TabButtonLeftTop : s_TabButtonLeftTop = GetTexture2D(TextureName.TabButtonLeftTop);
                private static Texture2D s_TabButtonMiddleBottom;
                public static Texture2D TabButtonMiddleBottom => s_TabButtonMiddleBottom ? s_TabButtonMiddleBottom : s_TabButtonMiddleBottom = GetTexture2D(TextureName.TabButtonMiddleBottom);
                private static Texture2D s_TabButtonMiddleFloating;
                public static Texture2D TabButtonMiddleFloating => s_TabButtonMiddleFloating ? s_TabButtonMiddleFloating : s_TabButtonMiddleFloating = GetTexture2D(TextureName.TabButtonMiddleFloating);
                private static Texture2D s_TabButtonMiddleLeft;
                public static Texture2D TabButtonMiddleLeft => s_TabButtonMiddleLeft ? s_TabButtonMiddleLeft : s_TabButtonMiddleLeft = GetTexture2D(TextureName.TabButtonMiddleLeft);
                private static Texture2D s_TabButtonMiddleRight;
                public static Texture2D TabButtonMiddleRight => s_TabButtonMiddleRight ? s_TabButtonMiddleRight : s_TabButtonMiddleRight = GetTexture2D(TextureName.TabButtonMiddleRight);
                private static Texture2D s_TabButtonMiddleTop;
                public static Texture2D TabButtonMiddleTop => s_TabButtonMiddleTop ? s_TabButtonMiddleTop : s_TabButtonMiddleTop = GetTexture2D(TextureName.TabButtonMiddleTop);
                private static Texture2D s_TabButtonRightBottom;
                public static Texture2D TabButtonRightBottom => s_TabButtonRightBottom ? s_TabButtonRightBottom : s_TabButtonRightBottom = GetTexture2D(TextureName.TabButtonRightBottom);
                private static Texture2D s_TabButtonRightFloating;
                public static Texture2D TabButtonRightFloating => s_TabButtonRightFloating ? s_TabButtonRightFloating : s_TabButtonRightFloating = GetTexture2D(TextureName.TabButtonRightFloating);
                private static Texture2D s_TabButtonRightTop;
                public static Texture2D TabButtonRightTop => s_TabButtonRightTop ? s_TabButtonRightTop : s_TabButtonRightTop = GetTexture2D(TextureName.TabButtonRightTop);
                private static Texture2D s_TabButtonTopLeft;
                public static Texture2D TabButtonTopLeft => s_TabButtonTopLeft ? s_TabButtonTopLeft : s_TabButtonTopLeft = GetTexture2D(TextureName.TabButtonTopLeft);
                private static Texture2D s_TabButtonTopRight;
                public static Texture2D TabButtonTopRight => s_TabButtonTopRight ? s_TabButtonTopRight : s_TabButtonTopRight = GetTexture2D(TextureName.TabButtonTopRight);
                private static Texture2D s_UIMenuHeader128x32;
                public static Texture2D UIMenuHeader128x32 => s_UIMenuHeader128x32 ? s_UIMenuHeader128x32 : s_UIMenuHeader128x32 = GetTexture2D(TextureName.UIMenuHeader128x32);
                private static Texture2D s_VerticalLayout;
                public static Texture2D VerticalLayout => s_VerticalLayout ? s_VerticalLayout : s_VerticalLayout = GetTexture2D(TextureName.VerticalLayout);
                
            }
        }



    }
}