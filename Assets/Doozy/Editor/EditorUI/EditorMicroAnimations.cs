// Copyright (c) 2015 - 2021 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

//.........................
//.....Generated Class.....
//.........................
//.......Do not edit.......
//.........................

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Doozy.Editor.EditorUI.ScriptableObjects.MicroAnimations;
using UnityEngine;

namespace Doozy.Editor.EditorUI
{
    [SuppressMessage("ReSharper", "IdentifierTypo")]
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    [SuppressMessage("ReSharper", "UnusedType.Global")]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public static class EditorMicroAnimations
    {
        public static class EditorUI
        {
            public static class Arrows
            {
                private static EditorDataMicroAnimationGroup s_animationGroup;
                private static EditorDataMicroAnimationGroup animationGroup =>
                    s_animationGroup != null
                        ? s_animationGroup
                        : s_animationGroup = EditorDataMicroAnimationDatabase.GetMicroAnimationGroup("EditorUI", "Arrows");

                public static List<Texture2D> GetTextures(AnimationName animationName) =>
                    animationGroup.GetTextures(animationName.ToString());

                public enum AnimationName
                {
                    ArrowDown,
                    ArrowLeft,
                    ArrowRight,
                    ArrowUp,
                    ChevronDown,
                    ChevronLeft,
                    ChevronRight,
                    ChevronUp
                }


                private static List<Texture2D> s_ArrowDown;
                public static List<Texture2D> ArrowDown => s_ArrowDown = s_ArrowDown ?? GetTextures(AnimationName.ArrowDown);
                private static List<Texture2D> s_ArrowLeft;
                public static List<Texture2D> ArrowLeft => s_ArrowLeft = s_ArrowLeft ?? GetTextures(AnimationName.ArrowLeft);
                private static List<Texture2D> s_ArrowRight;
                public static List<Texture2D> ArrowRight => s_ArrowRight = s_ArrowRight ?? GetTextures(AnimationName.ArrowRight);
                private static List<Texture2D> s_ArrowUp;
                public static List<Texture2D> ArrowUp => s_ArrowUp = s_ArrowUp ?? GetTextures(AnimationName.ArrowUp);
                private static List<Texture2D> s_ChevronDown;
                public static List<Texture2D> ChevronDown => s_ChevronDown = s_ChevronDown ?? GetTextures(AnimationName.ChevronDown);
                private static List<Texture2D> s_ChevronLeft;
                public static List<Texture2D> ChevronLeft => s_ChevronLeft = s_ChevronLeft ?? GetTextures(AnimationName.ChevronLeft);
                private static List<Texture2D> s_ChevronRight;
                public static List<Texture2D> ChevronRight => s_ChevronRight = s_ChevronRight ?? GetTextures(AnimationName.ChevronRight);
                private static List<Texture2D> s_ChevronUp;
                public static List<Texture2D> ChevronUp => s_ChevronUp = s_ChevronUp ?? GetTextures(AnimationName.ChevronUp);

            }

            public static class Components
            {
                private static EditorDataMicroAnimationGroup s_animationGroup;
                private static EditorDataMicroAnimationGroup animationGroup =>
                    s_animationGroup != null
                        ? s_animationGroup
                        : s_animationGroup = EditorDataMicroAnimationDatabase.GetMicroAnimationGroup("EditorUI", "Components");

                public static List<Texture2D> GetTextures(AnimationName animationName) =>
                    animationGroup.GetTextures(animationName.ToString());

                public enum AnimationName
                {
                    CarretRightToDown,
                    Checkmark,
                    EditorColorPalette,
                    EditorFontFamily,
                    EditorLayoutGroup,
                    EditorMicroAnimationGroup,
                    EditorSelectableColorPalette,
                    EditorStyleGroup,
                    EditorTextureGroup,
                    LineMixedValues,
                    RadioCircle,
                    Switch
                }


                private static List<Texture2D> s_CarretRightToDown;
                public static List<Texture2D> CarretRightToDown => s_CarretRightToDown = s_CarretRightToDown ?? GetTextures(AnimationName.CarretRightToDown);
                private static List<Texture2D> s_Checkmark;
                public static List<Texture2D> Checkmark => s_Checkmark = s_Checkmark ?? GetTextures(AnimationName.Checkmark);
                private static List<Texture2D> s_EditorColorPalette;
                public static List<Texture2D> EditorColorPalette => s_EditorColorPalette = s_EditorColorPalette ?? GetTextures(AnimationName.EditorColorPalette);
                private static List<Texture2D> s_EditorFontFamily;
                public static List<Texture2D> EditorFontFamily => s_EditorFontFamily = s_EditorFontFamily ?? GetTextures(AnimationName.EditorFontFamily);
                private static List<Texture2D> s_EditorLayoutGroup;
                public static List<Texture2D> EditorLayoutGroup => s_EditorLayoutGroup = s_EditorLayoutGroup ?? GetTextures(AnimationName.EditorLayoutGroup);
                private static List<Texture2D> s_EditorMicroAnimationGroup;
                public static List<Texture2D> EditorMicroAnimationGroup => s_EditorMicroAnimationGroup = s_EditorMicroAnimationGroup ?? GetTextures(AnimationName.EditorMicroAnimationGroup);
                private static List<Texture2D> s_EditorSelectableColorPalette;
                public static List<Texture2D> EditorSelectableColorPalette => s_EditorSelectableColorPalette = s_EditorSelectableColorPalette ?? GetTextures(AnimationName.EditorSelectableColorPalette);
                private static List<Texture2D> s_EditorStyleGroup;
                public static List<Texture2D> EditorStyleGroup => s_EditorStyleGroup = s_EditorStyleGroup ?? GetTextures(AnimationName.EditorStyleGroup);
                private static List<Texture2D> s_EditorTextureGroup;
                public static List<Texture2D> EditorTextureGroup => s_EditorTextureGroup = s_EditorTextureGroup ?? GetTextures(AnimationName.EditorTextureGroup);
                private static List<Texture2D> s_LineMixedValues;
                public static List<Texture2D> LineMixedValues => s_LineMixedValues = s_LineMixedValues ?? GetTextures(AnimationName.LineMixedValues);
                private static List<Texture2D> s_RadioCircle;
                public static List<Texture2D> RadioCircle => s_RadioCircle = s_RadioCircle ?? GetTextures(AnimationName.RadioCircle);
                private static List<Texture2D> s_Switch;
                public static List<Texture2D> Switch => s_Switch = s_Switch ?? GetTextures(AnimationName.Switch);

            }

            public static class Icons
            {
                private static EditorDataMicroAnimationGroup s_animationGroup;
                private static EditorDataMicroAnimationGroup animationGroup =>
                    s_animationGroup != null
                        ? s_animationGroup
                        : s_animationGroup = EditorDataMicroAnimationDatabase.GetMicroAnimationGroup("EditorUI", "Icons");

                public static List<Texture2D> GetTextures(AnimationName animationName) =>
                    animationGroup.GetTextures(animationName.ToString());

                public enum AnimationName
                {
                    Animator,
                    ApplicationQuit,
                    Atom,
                    AudioMixer,
                    AudioMixerGroup,
                    AutoDisable,
                    Back,
                    Binoculars,
                    BookOpen,
                    Border,
                    ButtonClick,
                    ButtonDoubleClick,
                    ButtonLeftClick,
                    ButtonLongClick,
                    ButtonMiddleClick,
                    ButtonRightClick,
                    CategoryPlus,
                    Check,
                    CircleScrub,
                    Clear,
                    Close,
                    Color,
                    ConnectedDisconnected,
                    Cooldown,
                    Copy,
                    Cut,
                    Debug,
                    DelayBetweenLoops,
                    Deselected,
                    Dice,
                    DisabledEnabled,
                    DisconnectedConnected,
                    Duration,
                    Edit,
                    EditorSettings,
                    Email,
                    EmptyList,
                    EnabledDisabled,
                    EventsOnFinish,
                    EventsOnStart,
                    Export,
                    Facebook,
                    Feather,
                    Filter,
                    FirstFrame,
                    FixedUpdate,
                    Font,
                    GameObject,
                    GenericDatabase,
                    Hide,
                    Hourglass,
                    Idle,
                    Import,
                    Info,
                    Integrations,
                    Label,
                    Landscape,
                    Language,
                    LastFrame,
                    LateUpdate,
                    Link,
                    Load,
                    Location,
                    Locked,
                    LockedUnlocked,
                    Loop,
                    MagicWand,
                    Manual,
                    Minus,
                    More,
                    Music,
                    Navigation,
                    News,
                    OffOn,
                    OneShot,
                    OnOff,
                    Orientation,
                    Paste,
                    PingPong,
                    PingPongOnce,
                    Play,
                    PlayForward,
                    PlayPause,
                    PlayReverse,
                    PlayStop,
                    Plus,
                    PointerDown,
                    PointerEnter,
                    PointerExit,
                    PointerUp,
                    Portrait,
                    Prefab,
                    QuestionMark,
                    RawImage,
                    Recent,
                    Refresh,
                    Reset,
                    Reverse,
                    Save,
                    SaveAs,
                    Scripting,
                    Search,
                    Selectable,
                    SelectableColorGenerator,
                    SelectableStates,
                    Selected,
                    Settings,
                    Shake,
                    Show,
                    SortAz,
                    SortHue,
                    SortZa,
                    Sound,
                    SoundMute,
                    Spring,
                    Sprite,
                    SpriteRenderer,
                    StartDelay,
                    Stop,
                    SupportRequest,
                    Texture,
                    TimeScale,
                    ToggleMixed,
                    ToggleOFF,
                    ToggleON,
                    Tooltip,
                    UIBehaviour,
                    Unity,
                    UnityEvent,
                    Unlink,
                    Unlocked,
                    VisibilityChanged,
                    Windows,
                    Youtube
                }


                private static List<Texture2D> s_Animator;
                public static List<Texture2D> Animator => s_Animator = s_Animator ?? GetTextures(AnimationName.Animator);
                private static List<Texture2D> s_ApplicationQuit;
                public static List<Texture2D> ApplicationQuit => s_ApplicationQuit = s_ApplicationQuit ?? GetTextures(AnimationName.ApplicationQuit);
                private static List<Texture2D> s_Atom;
                public static List<Texture2D> Atom => s_Atom = s_Atom ?? GetTextures(AnimationName.Atom);
                private static List<Texture2D> s_AudioMixer;
                public static List<Texture2D> AudioMixer => s_AudioMixer = s_AudioMixer ?? GetTextures(AnimationName.AudioMixer);
                private static List<Texture2D> s_AudioMixerGroup;
                public static List<Texture2D> AudioMixerGroup => s_AudioMixerGroup = s_AudioMixerGroup ?? GetTextures(AnimationName.AudioMixerGroup);
                private static List<Texture2D> s_AutoDisable;
                public static List<Texture2D> AutoDisable => s_AutoDisable = s_AutoDisable ?? GetTextures(AnimationName.AutoDisable);
                private static List<Texture2D> s_Back;
                public static List<Texture2D> Back => s_Back = s_Back ?? GetTextures(AnimationName.Back);
                private static List<Texture2D> s_Binoculars;
                public static List<Texture2D> Binoculars => s_Binoculars = s_Binoculars ?? GetTextures(AnimationName.Binoculars);
                private static List<Texture2D> s_BookOpen;
                public static List<Texture2D> BookOpen => s_BookOpen = s_BookOpen ?? GetTextures(AnimationName.BookOpen);
                private static List<Texture2D> s_Border;
                public static List<Texture2D> Border => s_Border = s_Border ?? GetTextures(AnimationName.Border);
                private static List<Texture2D> s_ButtonClick;
                public static List<Texture2D> ButtonClick => s_ButtonClick = s_ButtonClick ?? GetTextures(AnimationName.ButtonClick);
                private static List<Texture2D> s_ButtonDoubleClick;
                public static List<Texture2D> ButtonDoubleClick => s_ButtonDoubleClick = s_ButtonDoubleClick ?? GetTextures(AnimationName.ButtonDoubleClick);
                private static List<Texture2D> s_ButtonLeftClick;
                public static List<Texture2D> ButtonLeftClick => s_ButtonLeftClick = s_ButtonLeftClick ?? GetTextures(AnimationName.ButtonLeftClick);
                private static List<Texture2D> s_ButtonLongClick;
                public static List<Texture2D> ButtonLongClick => s_ButtonLongClick = s_ButtonLongClick ?? GetTextures(AnimationName.ButtonLongClick);
                private static List<Texture2D> s_ButtonMiddleClick;
                public static List<Texture2D> ButtonMiddleClick => s_ButtonMiddleClick = s_ButtonMiddleClick ?? GetTextures(AnimationName.ButtonMiddleClick);
                private static List<Texture2D> s_ButtonRightClick;
                public static List<Texture2D> ButtonRightClick => s_ButtonRightClick = s_ButtonRightClick ?? GetTextures(AnimationName.ButtonRightClick);
                private static List<Texture2D> s_CategoryPlus;
                public static List<Texture2D> CategoryPlus => s_CategoryPlus = s_CategoryPlus ?? GetTextures(AnimationName.CategoryPlus);
                private static List<Texture2D> s_Check;
                public static List<Texture2D> Check => s_Check = s_Check ?? GetTextures(AnimationName.Check);
                private static List<Texture2D> s_CircleScrub;
                public static List<Texture2D> CircleScrub => s_CircleScrub = s_CircleScrub ?? GetTextures(AnimationName.CircleScrub);
                private static List<Texture2D> s_Clear;
                public static List<Texture2D> Clear => s_Clear = s_Clear ?? GetTextures(AnimationName.Clear);
                private static List<Texture2D> s_Close;
                public static List<Texture2D> Close => s_Close = s_Close ?? GetTextures(AnimationName.Close);
                private static List<Texture2D> s_Color;
                public static List<Texture2D> Color => s_Color = s_Color ?? GetTextures(AnimationName.Color);
                private static List<Texture2D> s_ConnectedDisconnected;
                public static List<Texture2D> ConnectedDisconnected => s_ConnectedDisconnected = s_ConnectedDisconnected ?? GetTextures(AnimationName.ConnectedDisconnected);
                private static List<Texture2D> s_Cooldown;
                public static List<Texture2D> Cooldown => s_Cooldown = s_Cooldown ?? GetTextures(AnimationName.Cooldown);
                private static List<Texture2D> s_Copy;
                public static List<Texture2D> Copy => s_Copy = s_Copy ?? GetTextures(AnimationName.Copy);
                private static List<Texture2D> s_Cut;
                public static List<Texture2D> Cut => s_Cut = s_Cut ?? GetTextures(AnimationName.Cut);
                private static List<Texture2D> s_Debug;
                public static List<Texture2D> Debug => s_Debug = s_Debug ?? GetTextures(AnimationName.Debug);
                private static List<Texture2D> s_DelayBetweenLoops;
                public static List<Texture2D> DelayBetweenLoops => s_DelayBetweenLoops = s_DelayBetweenLoops ?? GetTextures(AnimationName.DelayBetweenLoops);
                private static List<Texture2D> s_Deselected;
                public static List<Texture2D> Deselected => s_Deselected = s_Deselected ?? GetTextures(AnimationName.Deselected);
                private static List<Texture2D> s_Dice;
                public static List<Texture2D> Dice => s_Dice = s_Dice ?? GetTextures(AnimationName.Dice);
                private static List<Texture2D> s_DisabledEnabled;
                public static List<Texture2D> DisabledEnabled => s_DisabledEnabled = s_DisabledEnabled ?? GetTextures(AnimationName.DisabledEnabled);
                private static List<Texture2D> s_DisconnectedConnected;
                public static List<Texture2D> DisconnectedConnected => s_DisconnectedConnected = s_DisconnectedConnected ?? GetTextures(AnimationName.DisconnectedConnected);
                private static List<Texture2D> s_Duration;
                public static List<Texture2D> Duration => s_Duration = s_Duration ?? GetTextures(AnimationName.Duration);
                private static List<Texture2D> s_Edit;
                public static List<Texture2D> Edit => s_Edit = s_Edit ?? GetTextures(AnimationName.Edit);
                private static List<Texture2D> s_EditorSettings;
                public static List<Texture2D> EditorSettings => s_EditorSettings = s_EditorSettings ?? GetTextures(AnimationName.EditorSettings);
                private static List<Texture2D> s_Email;
                public static List<Texture2D> Email => s_Email = s_Email ?? GetTextures(AnimationName.Email);
                private static List<Texture2D> s_EmptyList;
                public static List<Texture2D> EmptyList => s_EmptyList = s_EmptyList ?? GetTextures(AnimationName.EmptyList);
                private static List<Texture2D> s_EnabledDisabled;
                public static List<Texture2D> EnabledDisabled => s_EnabledDisabled = s_EnabledDisabled ?? GetTextures(AnimationName.EnabledDisabled);
                private static List<Texture2D> s_EventsOnFinish;
                public static List<Texture2D> EventsOnFinish => s_EventsOnFinish = s_EventsOnFinish ?? GetTextures(AnimationName.EventsOnFinish);
                private static List<Texture2D> s_EventsOnStart;
                public static List<Texture2D> EventsOnStart => s_EventsOnStart = s_EventsOnStart ?? GetTextures(AnimationName.EventsOnStart);
                private static List<Texture2D> s_Export;
                public static List<Texture2D> Export => s_Export = s_Export ?? GetTextures(AnimationName.Export);
                private static List<Texture2D> s_Facebook;
                public static List<Texture2D> Facebook => s_Facebook = s_Facebook ?? GetTextures(AnimationName.Facebook);
                private static List<Texture2D> s_Feather;
                public static List<Texture2D> Feather => s_Feather = s_Feather ?? GetTextures(AnimationName.Feather);
                private static List<Texture2D> s_Filter;
                public static List<Texture2D> Filter => s_Filter = s_Filter ?? GetTextures(AnimationName.Filter);
                private static List<Texture2D> s_FirstFrame;
                public static List<Texture2D> FirstFrame => s_FirstFrame = s_FirstFrame ?? GetTextures(AnimationName.FirstFrame);
                private static List<Texture2D> s_FixedUpdate;
                public static List<Texture2D> FixedUpdate => s_FixedUpdate = s_FixedUpdate ?? GetTextures(AnimationName.FixedUpdate);
                private static List<Texture2D> s_Font;
                public static List<Texture2D> Font => s_Font = s_Font ?? GetTextures(AnimationName.Font);
                private static List<Texture2D> s_GameObject;
                public static List<Texture2D> GameObject => s_GameObject = s_GameObject ?? GetTextures(AnimationName.GameObject);
                private static List<Texture2D> s_GenericDatabase;
                public static List<Texture2D> GenericDatabase => s_GenericDatabase = s_GenericDatabase ?? GetTextures(AnimationName.GenericDatabase);
                private static List<Texture2D> s_Hide;
                public static List<Texture2D> Hide => s_Hide = s_Hide ?? GetTextures(AnimationName.Hide);
                private static List<Texture2D> s_Hourglass;
                public static List<Texture2D> Hourglass => s_Hourglass = s_Hourglass ?? GetTextures(AnimationName.Hourglass);
                private static List<Texture2D> s_Idle;
                public static List<Texture2D> Idle => s_Idle = s_Idle ?? GetTextures(AnimationName.Idle);
                private static List<Texture2D> s_Import;
                public static List<Texture2D> Import => s_Import = s_Import ?? GetTextures(AnimationName.Import);
                private static List<Texture2D> s_Info;
                public static List<Texture2D> Info => s_Info = s_Info ?? GetTextures(AnimationName.Info);
                private static List<Texture2D> s_Integrations;
                public static List<Texture2D> Integrations => s_Integrations = s_Integrations ?? GetTextures(AnimationName.Integrations);
                private static List<Texture2D> s_Label;
                public static List<Texture2D> Label => s_Label = s_Label ?? GetTextures(AnimationName.Label);
                private static List<Texture2D> s_Landscape;
                public static List<Texture2D> Landscape => s_Landscape = s_Landscape ?? GetTextures(AnimationName.Landscape);
                private static List<Texture2D> s_Language;
                public static List<Texture2D> Language => s_Language = s_Language ?? GetTextures(AnimationName.Language);
                private static List<Texture2D> s_LastFrame;
                public static List<Texture2D> LastFrame => s_LastFrame = s_LastFrame ?? GetTextures(AnimationName.LastFrame);
                private static List<Texture2D> s_LateUpdate;
                public static List<Texture2D> LateUpdate => s_LateUpdate = s_LateUpdate ?? GetTextures(AnimationName.LateUpdate);
                private static List<Texture2D> s_Link;
                public static List<Texture2D> Link => s_Link = s_Link ?? GetTextures(AnimationName.Link);
                private static List<Texture2D> s_Load;
                public static List<Texture2D> Load => s_Load = s_Load ?? GetTextures(AnimationName.Load);
                private static List<Texture2D> s_Location;
                public static List<Texture2D> Location => s_Location = s_Location ?? GetTextures(AnimationName.Location);
                private static List<Texture2D> s_Locked;
                public static List<Texture2D> Locked => s_Locked = s_Locked ?? GetTextures(AnimationName.Locked);
                private static List<Texture2D> s_LockedUnlocked;
                public static List<Texture2D> LockedUnlocked => s_LockedUnlocked = s_LockedUnlocked ?? GetTextures(AnimationName.LockedUnlocked);
                private static List<Texture2D> s_Loop;
                public static List<Texture2D> Loop => s_Loop = s_Loop ?? GetTextures(AnimationName.Loop);
                private static List<Texture2D> s_MagicWand;
                public static List<Texture2D> MagicWand => s_MagicWand = s_MagicWand ?? GetTextures(AnimationName.MagicWand);
                private static List<Texture2D> s_Manual;
                public static List<Texture2D> Manual => s_Manual = s_Manual ?? GetTextures(AnimationName.Manual);
                private static List<Texture2D> s_Minus;
                public static List<Texture2D> Minus => s_Minus = s_Minus ?? GetTextures(AnimationName.Minus);
                private static List<Texture2D> s_More;
                public static List<Texture2D> More => s_More = s_More ?? GetTextures(AnimationName.More);
                private static List<Texture2D> s_Music;
                public static List<Texture2D> Music => s_Music = s_Music ?? GetTextures(AnimationName.Music);
                private static List<Texture2D> s_Navigation;
                public static List<Texture2D> Navigation => s_Navigation = s_Navigation ?? GetTextures(AnimationName.Navigation);
                private static List<Texture2D> s_News;
                public static List<Texture2D> News => s_News = s_News ?? GetTextures(AnimationName.News);
                private static List<Texture2D> s_OffOn;
                public static List<Texture2D> OffOn => s_OffOn = s_OffOn ?? GetTextures(AnimationName.OffOn);
                private static List<Texture2D> s_OneShot;
                public static List<Texture2D> OneShot => s_OneShot = s_OneShot ?? GetTextures(AnimationName.OneShot);
                private static List<Texture2D> s_OnOff;
                public static List<Texture2D> OnOff => s_OnOff = s_OnOff ?? GetTextures(AnimationName.OnOff);
                private static List<Texture2D> s_Orientation;
                public static List<Texture2D> Orientation => s_Orientation = s_Orientation ?? GetTextures(AnimationName.Orientation);
                private static List<Texture2D> s_Paste;
                public static List<Texture2D> Paste => s_Paste = s_Paste ?? GetTextures(AnimationName.Paste);
                private static List<Texture2D> s_PingPong;
                public static List<Texture2D> PingPong => s_PingPong = s_PingPong ?? GetTextures(AnimationName.PingPong);
                private static List<Texture2D> s_PingPongOnce;
                public static List<Texture2D> PingPongOnce => s_PingPongOnce = s_PingPongOnce ?? GetTextures(AnimationName.PingPongOnce);
                private static List<Texture2D> s_Play;
                public static List<Texture2D> Play => s_Play = s_Play ?? GetTextures(AnimationName.Play);
                private static List<Texture2D> s_PlayForward;
                public static List<Texture2D> PlayForward => s_PlayForward = s_PlayForward ?? GetTextures(AnimationName.PlayForward);
                private static List<Texture2D> s_PlayPause;
                public static List<Texture2D> PlayPause => s_PlayPause = s_PlayPause ?? GetTextures(AnimationName.PlayPause);
                private static List<Texture2D> s_PlayReverse;
                public static List<Texture2D> PlayReverse => s_PlayReverse = s_PlayReverse ?? GetTextures(AnimationName.PlayReverse);
                private static List<Texture2D> s_PlayStop;
                public static List<Texture2D> PlayStop => s_PlayStop = s_PlayStop ?? GetTextures(AnimationName.PlayStop);
                private static List<Texture2D> s_Plus;
                public static List<Texture2D> Plus => s_Plus = s_Plus ?? GetTextures(AnimationName.Plus);
                private static List<Texture2D> s_PointerDown;
                public static List<Texture2D> PointerDown => s_PointerDown = s_PointerDown ?? GetTextures(AnimationName.PointerDown);
                private static List<Texture2D> s_PointerEnter;
                public static List<Texture2D> PointerEnter => s_PointerEnter = s_PointerEnter ?? GetTextures(AnimationName.PointerEnter);
                private static List<Texture2D> s_PointerExit;
                public static List<Texture2D> PointerExit => s_PointerExit = s_PointerExit ?? GetTextures(AnimationName.PointerExit);
                private static List<Texture2D> s_PointerUp;
                public static List<Texture2D> PointerUp => s_PointerUp = s_PointerUp ?? GetTextures(AnimationName.PointerUp);
                private static List<Texture2D> s_Portrait;
                public static List<Texture2D> Portrait => s_Portrait = s_Portrait ?? GetTextures(AnimationName.Portrait);
                private static List<Texture2D> s_Prefab;
                public static List<Texture2D> Prefab => s_Prefab = s_Prefab ?? GetTextures(AnimationName.Prefab);
                private static List<Texture2D> s_QuestionMark;
                public static List<Texture2D> QuestionMark => s_QuestionMark = s_QuestionMark ?? GetTextures(AnimationName.QuestionMark);
                private static List<Texture2D> s_RawImage;
                public static List<Texture2D> RawImage => s_RawImage = s_RawImage ?? GetTextures(AnimationName.RawImage);
                private static List<Texture2D> s_Recent;
                public static List<Texture2D> Recent => s_Recent = s_Recent ?? GetTextures(AnimationName.Recent);
                private static List<Texture2D> s_Refresh;
                public static List<Texture2D> Refresh => s_Refresh = s_Refresh ?? GetTextures(AnimationName.Refresh);
                private static List<Texture2D> s_Reset;
                public static List<Texture2D> Reset => s_Reset = s_Reset ?? GetTextures(AnimationName.Reset);
                private static List<Texture2D> s_Reverse;
                public static List<Texture2D> Reverse => s_Reverse = s_Reverse ?? GetTextures(AnimationName.Reverse);
                private static List<Texture2D> s_Save;
                public static List<Texture2D> Save => s_Save = s_Save ?? GetTextures(AnimationName.Save);
                private static List<Texture2D> s_SaveAs;
                public static List<Texture2D> SaveAs => s_SaveAs = s_SaveAs ?? GetTextures(AnimationName.SaveAs);
                private static List<Texture2D> s_Scripting;
                public static List<Texture2D> Scripting => s_Scripting = s_Scripting ?? GetTextures(AnimationName.Scripting);
                private static List<Texture2D> s_Search;
                public static List<Texture2D> Search => s_Search = s_Search ?? GetTextures(AnimationName.Search);
                private static List<Texture2D> s_Selectable;
                public static List<Texture2D> Selectable => s_Selectable = s_Selectable ?? GetTextures(AnimationName.Selectable);
                private static List<Texture2D> s_SelectableColorGenerator;
                public static List<Texture2D> SelectableColorGenerator => s_SelectableColorGenerator = s_SelectableColorGenerator ?? GetTextures(AnimationName.SelectableColorGenerator);
                private static List<Texture2D> s_SelectableStates;
                public static List<Texture2D> SelectableStates => s_SelectableStates = s_SelectableStates ?? GetTextures(AnimationName.SelectableStates);
                private static List<Texture2D> s_Selected;
                public static List<Texture2D> Selected => s_Selected = s_Selected ?? GetTextures(AnimationName.Selected);
                private static List<Texture2D> s_Settings;
                public static List<Texture2D> Settings => s_Settings = s_Settings ?? GetTextures(AnimationName.Settings);
                private static List<Texture2D> s_Shake;
                public static List<Texture2D> Shake => s_Shake = s_Shake ?? GetTextures(AnimationName.Shake);
                private static List<Texture2D> s_Show;
                public static List<Texture2D> Show => s_Show = s_Show ?? GetTextures(AnimationName.Show);
                private static List<Texture2D> s_SortAz;
                public static List<Texture2D> SortAz => s_SortAz = s_SortAz ?? GetTextures(AnimationName.SortAz);
                private static List<Texture2D> s_SortHue;
                public static List<Texture2D> SortHue => s_SortHue = s_SortHue ?? GetTextures(AnimationName.SortHue);
                private static List<Texture2D> s_SortZa;
                public static List<Texture2D> SortZa => s_SortZa = s_SortZa ?? GetTextures(AnimationName.SortZa);
                private static List<Texture2D> s_Sound;
                public static List<Texture2D> Sound => s_Sound = s_Sound ?? GetTextures(AnimationName.Sound);
                private static List<Texture2D> s_SoundMute;
                public static List<Texture2D> SoundMute => s_SoundMute = s_SoundMute ?? GetTextures(AnimationName.SoundMute);
                private static List<Texture2D> s_Spring;
                public static List<Texture2D> Spring => s_Spring = s_Spring ?? GetTextures(AnimationName.Spring);
                private static List<Texture2D> s_Sprite;
                public static List<Texture2D> Sprite => s_Sprite = s_Sprite ?? GetTextures(AnimationName.Sprite);
                private static List<Texture2D> s_SpriteRenderer;
                public static List<Texture2D> SpriteRenderer => s_SpriteRenderer = s_SpriteRenderer ?? GetTextures(AnimationName.SpriteRenderer);
                private static List<Texture2D> s_StartDelay;
                public static List<Texture2D> StartDelay => s_StartDelay = s_StartDelay ?? GetTextures(AnimationName.StartDelay);
                private static List<Texture2D> s_Stop;
                public static List<Texture2D> Stop => s_Stop = s_Stop ?? GetTextures(AnimationName.Stop);
                private static List<Texture2D> s_SupportRequest;
                public static List<Texture2D> SupportRequest => s_SupportRequest = s_SupportRequest ?? GetTextures(AnimationName.SupportRequest);
                private static List<Texture2D> s_Texture;
                public static List<Texture2D> Texture => s_Texture = s_Texture ?? GetTextures(AnimationName.Texture);
                private static List<Texture2D> s_TimeScale;
                public static List<Texture2D> TimeScale => s_TimeScale = s_TimeScale ?? GetTextures(AnimationName.TimeScale);
                private static List<Texture2D> s_ToggleMixed;
                public static List<Texture2D> ToggleMixed => s_ToggleMixed = s_ToggleMixed ?? GetTextures(AnimationName.ToggleMixed);
                private static List<Texture2D> s_ToggleOFF;
                public static List<Texture2D> ToggleOFF => s_ToggleOFF = s_ToggleOFF ?? GetTextures(AnimationName.ToggleOFF);
                private static List<Texture2D> s_ToggleON;
                public static List<Texture2D> ToggleON => s_ToggleON = s_ToggleON ?? GetTextures(AnimationName.ToggleON);
                private static List<Texture2D> s_Tooltip;
                public static List<Texture2D> Tooltip => s_Tooltip = s_Tooltip ?? GetTextures(AnimationName.Tooltip);
                private static List<Texture2D> s_UIBehaviour;
                public static List<Texture2D> UIBehaviour => s_UIBehaviour = s_UIBehaviour ?? GetTextures(AnimationName.UIBehaviour);
                private static List<Texture2D> s_Unity;
                public static List<Texture2D> Unity => s_Unity = s_Unity ?? GetTextures(AnimationName.Unity);
                private static List<Texture2D> s_UnityEvent;
                public static List<Texture2D> UnityEvent => s_UnityEvent = s_UnityEvent ?? GetTextures(AnimationName.UnityEvent);
                private static List<Texture2D> s_Unlink;
                public static List<Texture2D> Unlink => s_Unlink = s_Unlink ?? GetTextures(AnimationName.Unlink);
                private static List<Texture2D> s_Unlocked;
                public static List<Texture2D> Unlocked => s_Unlocked = s_Unlocked ?? GetTextures(AnimationName.Unlocked);
                private static List<Texture2D> s_VisibilityChanged;
                public static List<Texture2D> VisibilityChanged => s_VisibilityChanged = s_VisibilityChanged ?? GetTextures(AnimationName.VisibilityChanged);
                private static List<Texture2D> s_Windows;
                public static List<Texture2D> Windows => s_Windows = s_Windows ?? GetTextures(AnimationName.Windows);
                private static List<Texture2D> s_Youtube;
                public static List<Texture2D> Youtube => s_Youtube = s_Youtube ?? GetTextures(AnimationName.Youtube);

            }

            public static class Markers
            {
                private static EditorDataMicroAnimationGroup s_animationGroup;
                private static EditorDataMicroAnimationGroup animationGroup =>
                    s_animationGroup != null
                        ? s_animationGroup
                        : s_animationGroup = EditorDataMicroAnimationDatabase.GetMicroAnimationGroup("EditorUI", "Markers");

                public static List<Texture2D> GetTextures(AnimationName animationName) =>
                    animationGroup.GetTextures(animationName.ToString());

                public enum AnimationName
                {
                    CaretDownToRight,
                    CircleToCaretDown,
                    CircleToCaretLeft,
                    CircleToCaretRight,
                    CircleToCaretUp,
                    CircleToHorizontalCircleLine,
                    CircleToHorizontalRoundedSquare,
                    CircleToVerticalCircleLine,
                    CircleToVerticalRoundedSquare
                }


                private static List<Texture2D> s_CaretDownToRight;
                public static List<Texture2D> CaretDownToRight => s_CaretDownToRight = s_CaretDownToRight ?? GetTextures(AnimationName.CaretDownToRight);
                private static List<Texture2D> s_CircleToCaretDown;
                public static List<Texture2D> CircleToCaretDown => s_CircleToCaretDown = s_CircleToCaretDown ?? GetTextures(AnimationName.CircleToCaretDown);
                private static List<Texture2D> s_CircleToCaretLeft;
                public static List<Texture2D> CircleToCaretLeft => s_CircleToCaretLeft = s_CircleToCaretLeft ?? GetTextures(AnimationName.CircleToCaretLeft);
                private static List<Texture2D> s_CircleToCaretRight;
                public static List<Texture2D> CircleToCaretRight => s_CircleToCaretRight = s_CircleToCaretRight ?? GetTextures(AnimationName.CircleToCaretRight);
                private static List<Texture2D> s_CircleToCaretUp;
                public static List<Texture2D> CircleToCaretUp => s_CircleToCaretUp = s_CircleToCaretUp ?? GetTextures(AnimationName.CircleToCaretUp);
                private static List<Texture2D> s_CircleToHorizontalCircleLine;
                public static List<Texture2D> CircleToHorizontalCircleLine => s_CircleToHorizontalCircleLine = s_CircleToHorizontalCircleLine ?? GetTextures(AnimationName.CircleToHorizontalCircleLine);
                private static List<Texture2D> s_CircleToHorizontalRoundedSquare;
                public static List<Texture2D> CircleToHorizontalRoundedSquare => s_CircleToHorizontalRoundedSquare = s_CircleToHorizontalRoundedSquare ?? GetTextures(AnimationName.CircleToHorizontalRoundedSquare);
                private static List<Texture2D> s_CircleToVerticalCircleLine;
                public static List<Texture2D> CircleToVerticalCircleLine => s_CircleToVerticalCircleLine = s_CircleToVerticalCircleLine ?? GetTextures(AnimationName.CircleToVerticalCircleLine);
                private static List<Texture2D> s_CircleToVerticalRoundedSquare;
                public static List<Texture2D> CircleToVerticalRoundedSquare => s_CircleToVerticalRoundedSquare = s_CircleToVerticalRoundedSquare ?? GetTextures(AnimationName.CircleToVerticalRoundedSquare);

            }

            public static class Placeholders
            {
                private static EditorDataMicroAnimationGroup s_animationGroup;
                private static EditorDataMicroAnimationGroup animationGroup =>
                    s_animationGroup != null
                        ? s_animationGroup
                        : s_animationGroup = EditorDataMicroAnimationDatabase.GetMicroAnimationGroup("EditorUI", "Placeholders");

                public static List<Texture2D> GetTextures(AnimationName animationName) =>
                    animationGroup.GetTextures(animationName.ToString());

                public enum AnimationName
                {
                    ComingSoon,
                    Empty,
                    EmptyDatabase,
                    EmptyFile,
                    EmptyListView,
                    EmptyListViewSmall,
                    EmptySearch,
                    EmptySmall,
                    UnderConstruction
                }


                private static List<Texture2D> s_ComingSoon;
                public static List<Texture2D> ComingSoon => s_ComingSoon = s_ComingSoon ?? GetTextures(AnimationName.ComingSoon);
                private static List<Texture2D> s_Empty;
                public static List<Texture2D> Empty => s_Empty = s_Empty ?? GetTextures(AnimationName.Empty);
                private static List<Texture2D> s_EmptyDatabase;
                public static List<Texture2D> EmptyDatabase => s_EmptyDatabase = s_EmptyDatabase ?? GetTextures(AnimationName.EmptyDatabase);
                private static List<Texture2D> s_EmptyFile;
                public static List<Texture2D> EmptyFile => s_EmptyFile = s_EmptyFile ?? GetTextures(AnimationName.EmptyFile);
                private static List<Texture2D> s_EmptyListView;
                public static List<Texture2D> EmptyListView => s_EmptyListView = s_EmptyListView ?? GetTextures(AnimationName.EmptyListView);
                private static List<Texture2D> s_EmptyListViewSmall;
                public static List<Texture2D> EmptyListViewSmall => s_EmptyListViewSmall = s_EmptyListViewSmall ?? GetTextures(AnimationName.EmptyListViewSmall);
                private static List<Texture2D> s_EmptySearch;
                public static List<Texture2D> EmptySearch => s_EmptySearch = s_EmptySearch ?? GetTextures(AnimationName.EmptySearch);
                private static List<Texture2D> s_EmptySmall;
                public static List<Texture2D> EmptySmall => s_EmptySmall = s_EmptySmall ?? GetTextures(AnimationName.EmptySmall);
                private static List<Texture2D> s_UnderConstruction;
                public static List<Texture2D> UnderConstruction => s_UnderConstruction = s_UnderConstruction ?? GetTextures(AnimationName.UnderConstruction);

            }

            public static class Widgets
            {
                private static EditorDataMicroAnimationGroup s_animationGroup;
                private static EditorDataMicroAnimationGroup animationGroup =>
                    s_animationGroup != null
                        ? s_animationGroup
                        : s_animationGroup = EditorDataMicroAnimationDatabase.GetMicroAnimationGroup("EditorUI", "Widgets");

                public static List<Texture2D> GetTextures(AnimationName animationName) =>
                    animationGroup.GetTextures(animationName.ToString());

                public enum AnimationName
                {
                    CircularGaugeFillBackground
                }


                private static List<Texture2D> s_CircularGaugeFillBackground;
                public static List<Texture2D> CircularGaugeFillBackground => s_CircularGaugeFillBackground = s_CircularGaugeFillBackground ?? GetTextures(AnimationName.CircularGaugeFillBackground);

            }


        }


        public static class Mody
        {
            public static class Effects
            {
                private static EditorDataMicroAnimationGroup s_animationGroup;
                private static EditorDataMicroAnimationGroup animationGroup =>
                    s_animationGroup != null
                        ? s_animationGroup
                        : s_animationGroup = EditorDataMicroAnimationDatabase.GetMicroAnimationGroup("Mody", "Effects");

                public static List<Texture2D> GetTextures(AnimationName animationName) =>
                    animationGroup.GetTextures(animationName.ToString());

                public enum AnimationName
                {
                    Running
                }


                private static List<Texture2D> s_Running;
                public static List<Texture2D> Running => s_Running = s_Running ?? GetTextures(AnimationName.Running);

            }

            public static class Icons
            {
                private static EditorDataMicroAnimationGroup s_animationGroup;
                private static EditorDataMicroAnimationGroup animationGroup =>
                    s_animationGroup != null
                        ? s_animationGroup
                        : s_animationGroup = EditorDataMicroAnimationDatabase.GetMicroAnimationGroup("Mody", "Icons");

                public static List<Texture2D> GetTextures(AnimationName animationName) =>
                    animationGroup.GetTextures(animationName.ToString());

                public enum AnimationName
                {
                    ModyAction,
                    ModyModule,
                    ModyTrigger
                }


                private static List<Texture2D> s_ModyAction;
                public static List<Texture2D> ModyAction => s_ModyAction = s_ModyAction ?? GetTextures(AnimationName.ModyAction);
                private static List<Texture2D> s_ModyModule;
                public static List<Texture2D> ModyModule => s_ModyModule = s_ModyModule ?? GetTextures(AnimationName.ModyModule);
                private static List<Texture2D> s_ModyTrigger;
                public static List<Texture2D> ModyTrigger => s_ModyTrigger = s_ModyTrigger ?? GetTextures(AnimationName.ModyTrigger);

            }


        }


        public static class Nody
        {
            public static class Effects
            {
                private static EditorDataMicroAnimationGroup s_animationGroup;
                private static EditorDataMicroAnimationGroup animationGroup =>
                    s_animationGroup != null
                        ? s_animationGroup
                        : s_animationGroup = EditorDataMicroAnimationDatabase.GetMicroAnimationGroup("Nody", "Effects");

                public static List<Texture2D> GetTextures(AnimationName animationName) =>
                    animationGroup.GetTextures(animationName.ToString());

                public enum AnimationName
                {
                    BackFlowIndicator,
                    NodeStateActive,
                    NodeStateRunning
                }


                private static List<Texture2D> s_BackFlowIndicator;
                public static List<Texture2D> BackFlowIndicator => s_BackFlowIndicator = s_BackFlowIndicator ?? GetTextures(AnimationName.BackFlowIndicator);
                private static List<Texture2D> s_NodeStateActive;
                public static List<Texture2D> NodeStateActive => s_NodeStateActive = s_NodeStateActive ?? GetTextures(AnimationName.NodeStateActive);
                private static List<Texture2D> s_NodeStateRunning;
                public static List<Texture2D> NodeStateRunning => s_NodeStateRunning = s_NodeStateRunning ?? GetTextures(AnimationName.NodeStateRunning);

            }

            public static class Icons
            {
                private static EditorDataMicroAnimationGroup s_animationGroup;
                private static EditorDataMicroAnimationGroup animationGroup =>
                    s_animationGroup != null
                        ? s_animationGroup
                        : s_animationGroup = EditorDataMicroAnimationDatabase.GetMicroAnimationGroup("Nody", "Icons");

                public static List<Texture2D> GetTextures(AnimationName animationName) =>
                    animationGroup.GetTextures(animationName.ToString());

                public enum AnimationName
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
                    GraphController,
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
                    ThemeNode,
                    TimeScaleNode,
                    UICanvasNode,
                    UINode,
                    UIPopupNode,
                    UnloadSceneNode,
                    WaitNode
                }


                private static List<Texture2D> s_ActivateLoadedScenesNode;
                public static List<Texture2D> ActivateLoadedScenesNode => s_ActivateLoadedScenesNode = s_ActivateLoadedScenesNode ?? GetTextures(AnimationName.ActivateLoadedScenesNode);
                private static List<Texture2D> s_ApplicationQuitNode;
                public static List<Texture2D> ApplicationQuitNode => s_ApplicationQuitNode = s_ApplicationQuitNode ?? GetTextures(AnimationName.ApplicationQuitNode);
                private static List<Texture2D> s_BackButtonNode;
                public static List<Texture2D> BackButtonNode => s_BackButtonNode = s_BackButtonNode ?? GetTextures(AnimationName.BackButtonNode);
                private static List<Texture2D> s_CustomNode;
                public static List<Texture2D> CustomNode => s_CustomNode = s_CustomNode ?? GetTextures(AnimationName.CustomNode);
                private static List<Texture2D> s_DebugNode;
                public static List<Texture2D> DebugNode => s_DebugNode = s_DebugNode ?? GetTextures(AnimationName.DebugNode);
                private static List<Texture2D> s_EnterNode;
                public static List<Texture2D> EnterNode => s_EnterNode = s_EnterNode ?? GetTextures(AnimationName.EnterNode);
                private static List<Texture2D> s_ExitNode;
                public static List<Texture2D> ExitNode => s_ExitNode = s_ExitNode ?? GetTextures(AnimationName.ExitNode);
                private static List<Texture2D> s_FlowController;
                public static List<Texture2D> FlowController => s_FlowController = s_FlowController ?? GetTextures(AnimationName.FlowController);
                private static List<Texture2D> s_FlowGraph;
                public static List<Texture2D> FlowGraph => s_FlowGraph = s_FlowGraph ?? GetTextures(AnimationName.FlowGraph);
                private static List<Texture2D> s_GameEventNode;
                public static List<Texture2D> GameEventNode => s_GameEventNode = s_GameEventNode ?? GetTextures(AnimationName.GameEventNode);
                private static List<Texture2D> s_GraphController;
                public static List<Texture2D> GraphController => s_GraphController = s_GraphController ?? GetTextures(AnimationName.GraphController);
                private static List<Texture2D> s_GroupNode;
                public static List<Texture2D> GroupNode => s_GroupNode = s_GroupNode ?? GetTextures(AnimationName.GroupNode);
                private static List<Texture2D> s_Infinity;
                public static List<Texture2D> Infinity => s_Infinity = s_Infinity ?? GetTextures(AnimationName.Infinity);
                private static List<Texture2D> s_LoadSceneNode;
                public static List<Texture2D> LoadSceneNode => s_LoadSceneNode = s_LoadSceneNode ?? GetTextures(AnimationName.LoadSceneNode);
                private static List<Texture2D> s_MarkerNode;
                public static List<Texture2D> MarkerNode => s_MarkerNode = s_MarkerNode ?? GetTextures(AnimationName.MarkerNode);
                private static List<Texture2D> s_Minimap;
                public static List<Texture2D> Minimap => s_Minimap = s_Minimap ?? GetTextures(AnimationName.Minimap);
                private static List<Texture2D> s_Nody;
                public static List<Texture2D> Nody => s_Nody = s_Nody ?? GetTextures(AnimationName.Nody);
                private static List<Texture2D> s_One;
                public static List<Texture2D> One => s_One = s_One ?? GetTextures(AnimationName.One);
                private static List<Texture2D> s_PivotNode;
                public static List<Texture2D> PivotNode => s_PivotNode = s_PivotNode ?? GetTextures(AnimationName.PivotNode);
                private static List<Texture2D> s_PortalNode;
                public static List<Texture2D> PortalNode => s_PortalNode = s_PortalNode ?? GetTextures(AnimationName.PortalNode);
                private static List<Texture2D> s_RandomNode;
                public static List<Texture2D> RandomNode => s_RandomNode = s_RandomNode ?? GetTextures(AnimationName.RandomNode);
                private static List<Texture2D> s_SignalNode;
                public static List<Texture2D> SignalNode => s_SignalNode = s_SignalNode ?? GetTextures(AnimationName.SignalNode);
                private static List<Texture2D> s_SoundNode;
                public static List<Texture2D> SoundNode => s_SoundNode = s_SoundNode ?? GetTextures(AnimationName.SoundNode);
                private static List<Texture2D> s_StartNode;
                public static List<Texture2D> StartNode => s_StartNode = s_StartNode ?? GetTextures(AnimationName.StartNode);
                private static List<Texture2D> s_StickyNoteNode;
                public static List<Texture2D> StickyNoteNode => s_StickyNoteNode = s_StickyNoteNode ?? GetTextures(AnimationName.StickyNoteNode);
                private static List<Texture2D> s_SwitchBackNode;
                public static List<Texture2D> SwitchBackNode => s_SwitchBackNode = s_SwitchBackNode ?? GetTextures(AnimationName.SwitchBackNode);
                private static List<Texture2D> s_ThemeNode;
                public static List<Texture2D> ThemeNode => s_ThemeNode = s_ThemeNode ?? GetTextures(AnimationName.ThemeNode);
                private static List<Texture2D> s_TimeScaleNode;
                public static List<Texture2D> TimeScaleNode => s_TimeScaleNode = s_TimeScaleNode ?? GetTextures(AnimationName.TimeScaleNode);
                private static List<Texture2D> s_UICanvasNode;
                public static List<Texture2D> UICanvasNode => s_UICanvasNode = s_UICanvasNode ?? GetTextures(AnimationName.UICanvasNode);
                private static List<Texture2D> s_UINode;
                public static List<Texture2D> UINode => s_UINode = s_UINode ?? GetTextures(AnimationName.UINode);
                private static List<Texture2D> s_UIPopupNode;
                public static List<Texture2D> UIPopupNode => s_UIPopupNode = s_UIPopupNode ?? GetTextures(AnimationName.UIPopupNode);
                private static List<Texture2D> s_UnloadSceneNode;
                public static List<Texture2D> UnloadSceneNode => s_UnloadSceneNode = s_UnloadSceneNode ?? GetTextures(AnimationName.UnloadSceneNode);
                private static List<Texture2D> s_WaitNode;
                public static List<Texture2D> WaitNode => s_WaitNode = s_WaitNode ?? GetTextures(AnimationName.WaitNode);

            }


        }


        public static class Reactor
        {
            public static class Icons
            {
                private static EditorDataMicroAnimationGroup s_animationGroup;
                private static EditorDataMicroAnimationGroup animationGroup =>
                    s_animationGroup != null
                        ? s_animationGroup
                        : s_animationGroup = EditorDataMicroAnimationDatabase.GetMicroAnimationGroup("Reactor", "Icons");

                public static List<Texture2D> GetTextures(AnimationName animationName) =>
                    animationGroup.GetTextures(animationName.ToString());

                public enum AnimationName
                {
                    AnimatorProgressTarget,
                    AudioMixerProgressTarget,
                    ColorAnimation,
                    ColorAnimator,
                    ColorTarget,
                    EditorHeartbeat,
                    Fade,
                    FadeOnOff,
                    FadeToDot,
                    FrameByFrameAnimation,
                    FrameByFrameAnimator,
                    Heartbeat,
                    ImageProgressTarget,
                    Move,
                    MoveOnOff,
                    MoveToDot,
                    Progressor,
                    ProgressorGroup,
                    ProgressTarget,
                    Reactor,
                    ReactorIconToFull,
                    Rotate,
                    RotateOnOff,
                    RotateToDot,
                    Scale,
                    ScaleOnOff,
                    ScaleToDot,
                    SignalProgressTarget,
                    SpriteAnimation,
                    SpriteAnimator,
                    SpriteTarget,
                    TextMeshProProgressTarget,
                    TextProgressTarget,
                    UIAnimation,
                    UIAnimationPreset,
                    UIAnimator,
                    UnityEventProgressTarget
                }


                private static List<Texture2D> s_AnimatorProgressTarget;
                public static List<Texture2D> AnimatorProgressTarget => s_AnimatorProgressTarget = s_AnimatorProgressTarget ?? GetTextures(AnimationName.AnimatorProgressTarget);
                private static List<Texture2D> s_AudioMixerProgressTarget;
                public static List<Texture2D> AudioMixerProgressTarget => s_AudioMixerProgressTarget = s_AudioMixerProgressTarget ?? GetTextures(AnimationName.AudioMixerProgressTarget);
                private static List<Texture2D> s_ColorAnimation;
                public static List<Texture2D> ColorAnimation => s_ColorAnimation = s_ColorAnimation ?? GetTextures(AnimationName.ColorAnimation);
                private static List<Texture2D> s_ColorAnimator;
                public static List<Texture2D> ColorAnimator => s_ColorAnimator = s_ColorAnimator ?? GetTextures(AnimationName.ColorAnimator);
                private static List<Texture2D> s_ColorTarget;
                public static List<Texture2D> ColorTarget => s_ColorTarget = s_ColorTarget ?? GetTextures(AnimationName.ColorTarget);
                private static List<Texture2D> s_EditorHeartbeat;
                public static List<Texture2D> EditorHeartbeat => s_EditorHeartbeat = s_EditorHeartbeat ?? GetTextures(AnimationName.EditorHeartbeat);
                private static List<Texture2D> s_Fade;
                public static List<Texture2D> Fade => s_Fade = s_Fade ?? GetTextures(AnimationName.Fade);
                private static List<Texture2D> s_FadeOnOff;
                public static List<Texture2D> FadeOnOff => s_FadeOnOff = s_FadeOnOff ?? GetTextures(AnimationName.FadeOnOff);
                private static List<Texture2D> s_FadeToDot;
                public static List<Texture2D> FadeToDot => s_FadeToDot = s_FadeToDot ?? GetTextures(AnimationName.FadeToDot);
                private static List<Texture2D> s_FrameByFrameAnimation;
                public static List<Texture2D> FrameByFrameAnimation => s_FrameByFrameAnimation = s_FrameByFrameAnimation ?? GetTextures(AnimationName.FrameByFrameAnimation);
                private static List<Texture2D> s_FrameByFrameAnimator;
                public static List<Texture2D> FrameByFrameAnimator => s_FrameByFrameAnimator = s_FrameByFrameAnimator ?? GetTextures(AnimationName.FrameByFrameAnimator);
                private static List<Texture2D> s_Heartbeat;
                public static List<Texture2D> Heartbeat => s_Heartbeat = s_Heartbeat ?? GetTextures(AnimationName.Heartbeat);
                private static List<Texture2D> s_ImageProgressTarget;
                public static List<Texture2D> ImageProgressTarget => s_ImageProgressTarget = s_ImageProgressTarget ?? GetTextures(AnimationName.ImageProgressTarget);
                private static List<Texture2D> s_Move;
                public static List<Texture2D> Move => s_Move = s_Move ?? GetTextures(AnimationName.Move);
                private static List<Texture2D> s_MoveOnOff;
                public static List<Texture2D> MoveOnOff => s_MoveOnOff = s_MoveOnOff ?? GetTextures(AnimationName.MoveOnOff);
                private static List<Texture2D> s_MoveToDot;
                public static List<Texture2D> MoveToDot => s_MoveToDot = s_MoveToDot ?? GetTextures(AnimationName.MoveToDot);
                private static List<Texture2D> s_Progressor;
                public static List<Texture2D> Progressor => s_Progressor = s_Progressor ?? GetTextures(AnimationName.Progressor);
                private static List<Texture2D> s_ProgressorGroup;
                public static List<Texture2D> ProgressorGroup => s_ProgressorGroup = s_ProgressorGroup ?? GetTextures(AnimationName.ProgressorGroup);
                private static List<Texture2D> s_ProgressTarget;
                public static List<Texture2D> ProgressTarget => s_ProgressTarget = s_ProgressTarget ?? GetTextures(AnimationName.ProgressTarget);
                private static List<Texture2D> s_Reactor;
                public static List<Texture2D> Reactor => s_Reactor = s_Reactor ?? GetTextures(AnimationName.Reactor);
                private static List<Texture2D> s_ReactorIconToFull;
                public static List<Texture2D> ReactorIconToFull => s_ReactorIconToFull = s_ReactorIconToFull ?? GetTextures(AnimationName.ReactorIconToFull);
                private static List<Texture2D> s_Rotate;
                public static List<Texture2D> Rotate => s_Rotate = s_Rotate ?? GetTextures(AnimationName.Rotate);
                private static List<Texture2D> s_RotateOnOff;
                public static List<Texture2D> RotateOnOff => s_RotateOnOff = s_RotateOnOff ?? GetTextures(AnimationName.RotateOnOff);
                private static List<Texture2D> s_RotateToDot;
                public static List<Texture2D> RotateToDot => s_RotateToDot = s_RotateToDot ?? GetTextures(AnimationName.RotateToDot);
                private static List<Texture2D> s_Scale;
                public static List<Texture2D> Scale => s_Scale = s_Scale ?? GetTextures(AnimationName.Scale);
                private static List<Texture2D> s_ScaleOnOff;
                public static List<Texture2D> ScaleOnOff => s_ScaleOnOff = s_ScaleOnOff ?? GetTextures(AnimationName.ScaleOnOff);
                private static List<Texture2D> s_ScaleToDot;
                public static List<Texture2D> ScaleToDot => s_ScaleToDot = s_ScaleToDot ?? GetTextures(AnimationName.ScaleToDot);
                private static List<Texture2D> s_SignalProgressTarget;
                public static List<Texture2D> SignalProgressTarget => s_SignalProgressTarget = s_SignalProgressTarget ?? GetTextures(AnimationName.SignalProgressTarget);
                private static List<Texture2D> s_SpriteAnimation;
                public static List<Texture2D> SpriteAnimation => s_SpriteAnimation = s_SpriteAnimation ?? GetTextures(AnimationName.SpriteAnimation);
                private static List<Texture2D> s_SpriteAnimator;
                public static List<Texture2D> SpriteAnimator => s_SpriteAnimator = s_SpriteAnimator ?? GetTextures(AnimationName.SpriteAnimator);
                private static List<Texture2D> s_SpriteTarget;
                public static List<Texture2D> SpriteTarget => s_SpriteTarget = s_SpriteTarget ?? GetTextures(AnimationName.SpriteTarget);
                private static List<Texture2D> s_TextMeshProProgressTarget;
                public static List<Texture2D> TextMeshProProgressTarget => s_TextMeshProProgressTarget = s_TextMeshProProgressTarget ?? GetTextures(AnimationName.TextMeshProProgressTarget);
                private static List<Texture2D> s_TextProgressTarget;
                public static List<Texture2D> TextProgressTarget => s_TextProgressTarget = s_TextProgressTarget ?? GetTextures(AnimationName.TextProgressTarget);
                private static List<Texture2D> s_UIAnimation;
                public static List<Texture2D> UIAnimation => s_UIAnimation = s_UIAnimation ?? GetTextures(AnimationName.UIAnimation);
                private static List<Texture2D> s_UIAnimationPreset;
                public static List<Texture2D> UIAnimationPreset => s_UIAnimationPreset = s_UIAnimationPreset ?? GetTextures(AnimationName.UIAnimationPreset);
                private static List<Texture2D> s_UIAnimator;
                public static List<Texture2D> UIAnimator => s_UIAnimator = s_UIAnimator ?? GetTextures(AnimationName.UIAnimator);
                private static List<Texture2D> s_UnityEventProgressTarget;
                public static List<Texture2D> UnityEventProgressTarget => s_UnityEventProgressTarget = s_UnityEventProgressTarget ?? GetTextures(AnimationName.UnityEventProgressTarget);

            }


        }


        public static class Signals
        {
            public static class Icons
            {
                private static EditorDataMicroAnimationGroup s_animationGroup;
                private static EditorDataMicroAnimationGroup animationGroup =>
                    s_animationGroup != null
                        ? s_animationGroup
                        : s_animationGroup = EditorDataMicroAnimationDatabase.GetMicroAnimationGroup("Signals", "Icons");

                public static List<Texture2D> GetTextures(AnimationName animationName) =>
                    animationGroup.GetTextures(animationName.ToString());

                public enum AnimationName
                {
                    MetaSignal,
                    MetaSignalOnOff,
                    MultiSignalReceiver,
                    Signal,
                    SignalBroadcaster,
                    SignalOnOff,
                    SignalProvider,
                    SignalReceiver,
                    SignalSender,
                    SignalStream,
                    StreamDatabase
                }


                private static List<Texture2D> s_MetaSignal;
                public static List<Texture2D> MetaSignal => s_MetaSignal = s_MetaSignal ?? GetTextures(AnimationName.MetaSignal);
                private static List<Texture2D> s_MetaSignalOnOff;
                public static List<Texture2D> MetaSignalOnOff => s_MetaSignalOnOff = s_MetaSignalOnOff ?? GetTextures(AnimationName.MetaSignalOnOff);
                private static List<Texture2D> s_MultiSignalReceiver;
                public static List<Texture2D> MultiSignalReceiver => s_MultiSignalReceiver = s_MultiSignalReceiver ?? GetTextures(AnimationName.MultiSignalReceiver);
                private static List<Texture2D> s_Signal;
                public static List<Texture2D> Signal => s_Signal = s_Signal ?? GetTextures(AnimationName.Signal);
                private static List<Texture2D> s_SignalBroadcaster;
                public static List<Texture2D> SignalBroadcaster => s_SignalBroadcaster = s_SignalBroadcaster ?? GetTextures(AnimationName.SignalBroadcaster);
                private static List<Texture2D> s_SignalOnOff;
                public static List<Texture2D> SignalOnOff => s_SignalOnOff = s_SignalOnOff ?? GetTextures(AnimationName.SignalOnOff);
                private static List<Texture2D> s_SignalProvider;
                public static List<Texture2D> SignalProvider => s_SignalProvider = s_SignalProvider ?? GetTextures(AnimationName.SignalProvider);
                private static List<Texture2D> s_SignalReceiver;
                public static List<Texture2D> SignalReceiver => s_SignalReceiver = s_SignalReceiver ?? GetTextures(AnimationName.SignalReceiver);
                private static List<Texture2D> s_SignalSender;
                public static List<Texture2D> SignalSender => s_SignalSender = s_SignalSender ?? GetTextures(AnimationName.SignalSender);
                private static List<Texture2D> s_SignalStream;
                public static List<Texture2D> SignalStream => s_SignalStream = s_SignalStream ?? GetTextures(AnimationName.SignalStream);
                private static List<Texture2D> s_StreamDatabase;
                public static List<Texture2D> StreamDatabase => s_StreamDatabase = s_StreamDatabase ?? GetTextures(AnimationName.StreamDatabase);

            }

            public static class Placeholders
            {
                private static EditorDataMicroAnimationGroup s_animationGroup;
                private static EditorDataMicroAnimationGroup animationGroup =>
                    s_animationGroup != null
                        ? s_animationGroup
                        : s_animationGroup = EditorDataMicroAnimationDatabase.GetMicroAnimationGroup("Signals", "Placeholders");

                public static List<Texture2D> GetTextures(AnimationName animationName) =>
                    animationGroup.GetTextures(animationName.ToString());

                public enum AnimationName
                {
                    OfflineSignal,
                    OnlineSignal
                }


                private static List<Texture2D> s_OfflineSignal;
                public static List<Texture2D> OfflineSignal => s_OfflineSignal = s_OfflineSignal ?? GetTextures(AnimationName.OfflineSignal);
                private static List<Texture2D> s_OnlineSignal;
                public static List<Texture2D> OnlineSignal => s_OnlineSignal = s_OnlineSignal ?? GetTextures(AnimationName.OnlineSignal);

            }


        }


        public static class UIManager
        {
            public static class Icons
            {
                private static EditorDataMicroAnimationGroup s_animationGroup;
                private static EditorDataMicroAnimationGroup animationGroup =>
                    s_animationGroup != null
                        ? s_animationGroup
                        : s_animationGroup = EditorDataMicroAnimationDatabase.GetMicroAnimationGroup("UIManager", "Icons");

                public static List<Texture2D> GetTextures(AnimationName animationName) =>
                    animationGroup.GetTextures(animationName.ToString());

                public enum AnimationName
                {
                    Alert,
                    AlertDatabase,
                    BackButton,
                    Buttons,
                    ButtonsDatabase,
                    Canvases,
                    DoozyUI,
                    Drawers,
                    GameEventListener,
                    GameEventManager,
                    InputToSignal,
                    KeyMapper,
                    KeyToAction,
                    KeyToGameEvent,
                    MultiplayerInfo,
                    NotificationManager,
                    PopupDatabase,
                    Popups,
                    RadialLayout,
                    SceneDirector,
                    SceneLoader,
                    SignalListener,
                    SignalToAudioSource,
                    SignalToColorTarget,
                    SignalToSpriteTarget,
                    SlidersDatabase,
                    SpriteSwapper,
                    TogglesDatabase,
                    UIButtonListener,
                    UIContainer,
                    UIDrawerListener,
                    UINotification,
                    UIPopupManager,
                    UIScrollbar,
                    UISelectable,
                    UISelectableAnimator,
                    UISlider,
                    UITab,
                    UIToggleCheckbox,
                    UIToggleGroup,
                    UIToggleListener,
                    UIToggleRadio,
                    UIToggleSwitch,
                    UIViewListener,
                    Views,
                    ViewsDatabase
                }


                private static List<Texture2D> s_Alert;
                public static List<Texture2D> Alert => s_Alert = s_Alert ?? GetTextures(AnimationName.Alert);
                private static List<Texture2D> s_AlertDatabase;
                public static List<Texture2D> AlertDatabase => s_AlertDatabase = s_AlertDatabase ?? GetTextures(AnimationName.AlertDatabase);
                private static List<Texture2D> s_BackButton;
                public static List<Texture2D> BackButton => s_BackButton = s_BackButton ?? GetTextures(AnimationName.BackButton);
                private static List<Texture2D> s_Buttons;
                public static List<Texture2D> Buttons => s_Buttons = s_Buttons ?? GetTextures(AnimationName.Buttons);
                private static List<Texture2D> s_ButtonsDatabase;
                public static List<Texture2D> ButtonsDatabase => s_ButtonsDatabase = s_ButtonsDatabase ?? GetTextures(AnimationName.ButtonsDatabase);
                private static List<Texture2D> s_Canvases;
                public static List<Texture2D> Canvases => s_Canvases = s_Canvases ?? GetTextures(AnimationName.Canvases);
                private static List<Texture2D> s_DoozyUI;
                public static List<Texture2D> DoozyUI => s_DoozyUI = s_DoozyUI ?? GetTextures(AnimationName.DoozyUI);
                private static List<Texture2D> s_Drawers;
                public static List<Texture2D> Drawers => s_Drawers = s_Drawers ?? GetTextures(AnimationName.Drawers);
                private static List<Texture2D> s_GameEventListener;
                public static List<Texture2D> GameEventListener => s_GameEventListener = s_GameEventListener ?? GetTextures(AnimationName.GameEventListener);
                private static List<Texture2D> s_GameEventManager;
                public static List<Texture2D> GameEventManager => s_GameEventManager = s_GameEventManager ?? GetTextures(AnimationName.GameEventManager);
                private static List<Texture2D> s_InputToSignal;
                public static List<Texture2D> InputToSignal => s_InputToSignal = s_InputToSignal ?? GetTextures(AnimationName.InputToSignal);
                private static List<Texture2D> s_KeyMapper;
                public static List<Texture2D> KeyMapper => s_KeyMapper = s_KeyMapper ?? GetTextures(AnimationName.KeyMapper);
                private static List<Texture2D> s_KeyToAction;
                public static List<Texture2D> KeyToAction => s_KeyToAction = s_KeyToAction ?? GetTextures(AnimationName.KeyToAction);
                private static List<Texture2D> s_KeyToGameEvent;
                public static List<Texture2D> KeyToGameEvent => s_KeyToGameEvent = s_KeyToGameEvent ?? GetTextures(AnimationName.KeyToGameEvent);
                private static List<Texture2D> s_MultiplayerInfo;
                public static List<Texture2D> MultiplayerInfo => s_MultiplayerInfo = s_MultiplayerInfo ?? GetTextures(AnimationName.MultiplayerInfo);
                private static List<Texture2D> s_NotificationManager;
                public static List<Texture2D> NotificationManager => s_NotificationManager = s_NotificationManager ?? GetTextures(AnimationName.NotificationManager);
                private static List<Texture2D> s_PopupDatabase;
                public static List<Texture2D> PopupDatabase => s_PopupDatabase = s_PopupDatabase ?? GetTextures(AnimationName.PopupDatabase);
                private static List<Texture2D> s_Popups;
                public static List<Texture2D> Popups => s_Popups = s_Popups ?? GetTextures(AnimationName.Popups);
                private static List<Texture2D> s_RadialLayout;
                public static List<Texture2D> RadialLayout => s_RadialLayout = s_RadialLayout ?? GetTextures(AnimationName.RadialLayout);
                private static List<Texture2D> s_SceneDirector;
                public static List<Texture2D> SceneDirector => s_SceneDirector = s_SceneDirector ?? GetTextures(AnimationName.SceneDirector);
                private static List<Texture2D> s_SceneLoader;
                public static List<Texture2D> SceneLoader => s_SceneLoader = s_SceneLoader ?? GetTextures(AnimationName.SceneLoader);
                private static List<Texture2D> s_SignalListener;
                public static List<Texture2D> SignalListener => s_SignalListener = s_SignalListener ?? GetTextures(AnimationName.SignalListener);
                private static List<Texture2D> s_SignalToAudioSource;
                public static List<Texture2D> SignalToAudioSource => s_SignalToAudioSource = s_SignalToAudioSource ?? GetTextures(AnimationName.SignalToAudioSource);
                private static List<Texture2D> s_SignalToColorTarget;
                public static List<Texture2D> SignalToColorTarget => s_SignalToColorTarget = s_SignalToColorTarget ?? GetTextures(AnimationName.SignalToColorTarget);
                private static List<Texture2D> s_SignalToSpriteTarget;
                public static List<Texture2D> SignalToSpriteTarget => s_SignalToSpriteTarget = s_SignalToSpriteTarget ?? GetTextures(AnimationName.SignalToSpriteTarget);
                private static List<Texture2D> s_SlidersDatabase;
                public static List<Texture2D> SlidersDatabase => s_SlidersDatabase = s_SlidersDatabase ?? GetTextures(AnimationName.SlidersDatabase);
                private static List<Texture2D> s_SpriteSwapper;
                public static List<Texture2D> SpriteSwapper => s_SpriteSwapper = s_SpriteSwapper ?? GetTextures(AnimationName.SpriteSwapper);
                private static List<Texture2D> s_TogglesDatabase;
                public static List<Texture2D> TogglesDatabase => s_TogglesDatabase = s_TogglesDatabase ?? GetTextures(AnimationName.TogglesDatabase);
                private static List<Texture2D> s_UIButtonListener;
                public static List<Texture2D> UIButtonListener => s_UIButtonListener = s_UIButtonListener ?? GetTextures(AnimationName.UIButtonListener);
                private static List<Texture2D> s_UIContainer;
                public static List<Texture2D> UIContainer => s_UIContainer = s_UIContainer ?? GetTextures(AnimationName.UIContainer);
                private static List<Texture2D> s_UIDrawerListener;
                public static List<Texture2D> UIDrawerListener => s_UIDrawerListener = s_UIDrawerListener ?? GetTextures(AnimationName.UIDrawerListener);
                private static List<Texture2D> s_UINotification;
                public static List<Texture2D> UINotification => s_UINotification = s_UINotification ?? GetTextures(AnimationName.UINotification);
                private static List<Texture2D> s_UIPopupManager;
                public static List<Texture2D> UIPopupManager => s_UIPopupManager = s_UIPopupManager ?? GetTextures(AnimationName.UIPopupManager);
                private static List<Texture2D> s_UIScrollbar;
                public static List<Texture2D> UIScrollbar => s_UIScrollbar = s_UIScrollbar ?? GetTextures(AnimationName.UIScrollbar);
                private static List<Texture2D> s_UISelectable;
                public static List<Texture2D> UISelectable => s_UISelectable = s_UISelectable ?? GetTextures(AnimationName.UISelectable);
                private static List<Texture2D> s_UISelectableAnimator;
                public static List<Texture2D> UISelectableAnimator => s_UISelectableAnimator = s_UISelectableAnimator ?? GetTextures(AnimationName.UISelectableAnimator);
                private static List<Texture2D> s_UISlider;
                public static List<Texture2D> UISlider => s_UISlider = s_UISlider ?? GetTextures(AnimationName.UISlider);
                private static List<Texture2D> s_UITab;
                public static List<Texture2D> UITab => s_UITab = s_UITab ?? GetTextures(AnimationName.UITab);
                private static List<Texture2D> s_UIToggleCheckbox;
                public static List<Texture2D> UIToggleCheckbox => s_UIToggleCheckbox = s_UIToggleCheckbox ?? GetTextures(AnimationName.UIToggleCheckbox);
                private static List<Texture2D> s_UIToggleGroup;
                public static List<Texture2D> UIToggleGroup => s_UIToggleGroup = s_UIToggleGroup ?? GetTextures(AnimationName.UIToggleGroup);
                private static List<Texture2D> s_UIToggleListener;
                public static List<Texture2D> UIToggleListener => s_UIToggleListener = s_UIToggleListener ?? GetTextures(AnimationName.UIToggleListener);
                private static List<Texture2D> s_UIToggleRadio;
                public static List<Texture2D> UIToggleRadio => s_UIToggleRadio = s_UIToggleRadio ?? GetTextures(AnimationName.UIToggleRadio);
                private static List<Texture2D> s_UIToggleSwitch;
                public static List<Texture2D> UIToggleSwitch => s_UIToggleSwitch = s_UIToggleSwitch ?? GetTextures(AnimationName.UIToggleSwitch);
                private static List<Texture2D> s_UIViewListener;
                public static List<Texture2D> UIViewListener => s_UIViewListener = s_UIViewListener ?? GetTextures(AnimationName.UIViewListener);
                private static List<Texture2D> s_Views;
                public static List<Texture2D> Views => s_Views = s_Views ?? GetTextures(AnimationName.Views);
                private static List<Texture2D> s_ViewsDatabase;
                public static List<Texture2D> ViewsDatabase => s_ViewsDatabase = s_ViewsDatabase ?? GetTextures(AnimationName.ViewsDatabase);

            }

            public static class UIMenu
            {
                private static EditorDataMicroAnimationGroup s_animationGroup;
                private static EditorDataMicroAnimationGroup animationGroup =>
                    s_animationGroup != null
                        ? s_animationGroup
                        : s_animationGroup = EditorDataMicroAnimationDatabase.GetMicroAnimationGroup("UIManager", "UIMenu");

                public static List<Texture2D> GetTextures(AnimationName animationName) =>
                    animationGroup.GetTextures(animationName.ToString());

                public enum AnimationName
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
                    TabButtonMiddleFloating,
                    TabButtonMiddleLeft,
                    TabButtonMiddleRight,
                    TabButtonMiddleTop,
                    TabButtonRightBottom,
                    TabButtonRightFloating,
                    TabButtonRightTop,
                    TabButtonTopLeft,
                    TabButtonTopRight,
                    UIMenuHeader,
                    VerticalLayout
                }


                private static List<Texture2D> s_Button;
                public static List<Texture2D> Button => s_Button = s_Button ?? GetTextures(AnimationName.Button);
                private static List<Texture2D> s_Checkbox;
                public static List<Texture2D> Checkbox => s_Checkbox = s_Checkbox ?? GetTextures(AnimationName.Checkbox);
                private static List<Texture2D> s_Component;
                public static List<Texture2D> Component => s_Component = s_Component ?? GetTextures(AnimationName.Component);
                private static List<Texture2D> s_Container;
                public static List<Texture2D> Container => s_Container = s_Container ?? GetTextures(AnimationName.Container);
                private static List<Texture2D> s_Content;
                public static List<Texture2D> Content => s_Content = s_Content ?? GetTextures(AnimationName.Content);
                private static List<Texture2D> s_Custom;
                public static List<Texture2D> Custom => s_Custom = s_Custom ?? GetTextures(AnimationName.Custom);
                private static List<Texture2D> s_Dropdown;
                public static List<Texture2D> Dropdown => s_Dropdown = s_Dropdown ?? GetTextures(AnimationName.Dropdown);
                private static List<Texture2D> s_GridLayout;
                public static List<Texture2D> GridLayout => s_GridLayout = s_GridLayout ?? GetTextures(AnimationName.GridLayout);
                private static List<Texture2D> s_HorizontalLayout;
                public static List<Texture2D> HorizontalLayout => s_HorizontalLayout = s_HorizontalLayout ?? GetTextures(AnimationName.HorizontalLayout);
                private static List<Texture2D> s_InputField;
                public static List<Texture2D> InputField => s_InputField = s_InputField ?? GetTextures(AnimationName.InputField);
                private static List<Texture2D> s_RadialLayout;
                public static List<Texture2D> RadialLayout => s_RadialLayout = s_RadialLayout ?? GetTextures(AnimationName.RadialLayout);
                private static List<Texture2D> s_RadioButton;
                public static List<Texture2D> RadioButton => s_RadioButton = s_RadioButton ?? GetTextures(AnimationName.RadioButton);
                private static List<Texture2D> s_Scollbar;
                public static List<Texture2D> Scollbar => s_Scollbar = s_Scollbar ?? GetTextures(AnimationName.Scollbar);
                private static List<Texture2D> s_ScrollView;
                public static List<Texture2D> ScrollView => s_ScrollView = s_ScrollView ?? GetTextures(AnimationName.ScrollView);
                private static List<Texture2D> s_Slider;
                public static List<Texture2D> Slider => s_Slider = s_Slider ?? GetTextures(AnimationName.Slider);
                private static List<Texture2D> s_Switch;
                public static List<Texture2D> Switch => s_Switch = s_Switch ?? GetTextures(AnimationName.Switch);
                private static List<Texture2D> s_TabButtonBottomLeft;
                public static List<Texture2D> TabButtonBottomLeft => s_TabButtonBottomLeft = s_TabButtonBottomLeft ?? GetTextures(AnimationName.TabButtonBottomLeft);
                private static List<Texture2D> s_TabButtonBottomRight;
                public static List<Texture2D> TabButtonBottomRight => s_TabButtonBottomRight = s_TabButtonBottomRight ?? GetTextures(AnimationName.TabButtonBottomRight);
                private static List<Texture2D> s_TabButtonLeftBottom;
                public static List<Texture2D> TabButtonLeftBottom => s_TabButtonLeftBottom = s_TabButtonLeftBottom ?? GetTextures(AnimationName.TabButtonLeftBottom);
                private static List<Texture2D> s_TabButtonLeftFloating;
                public static List<Texture2D> TabButtonLeftFloating => s_TabButtonLeftFloating = s_TabButtonLeftFloating ?? GetTextures(AnimationName.TabButtonLeftFloating);
                private static List<Texture2D> s_TabButtonLeftTop;
                public static List<Texture2D> TabButtonLeftTop => s_TabButtonLeftTop = s_TabButtonLeftTop ?? GetTextures(AnimationName.TabButtonLeftTop);
                private static List<Texture2D> s_TabButtonMiddleFloating;
                public static List<Texture2D> TabButtonMiddleFloating => s_TabButtonMiddleFloating = s_TabButtonMiddleFloating ?? GetTextures(AnimationName.TabButtonMiddleFloating);
                private static List<Texture2D> s_TabButtonMiddleLeft;
                public static List<Texture2D> TabButtonMiddleLeft => s_TabButtonMiddleLeft = s_TabButtonMiddleLeft ?? GetTextures(AnimationName.TabButtonMiddleLeft);
                private static List<Texture2D> s_TabButtonMiddleRight;
                public static List<Texture2D> TabButtonMiddleRight => s_TabButtonMiddleRight = s_TabButtonMiddleRight ?? GetTextures(AnimationName.TabButtonMiddleRight);
                private static List<Texture2D> s_TabButtonMiddleTop;
                public static List<Texture2D> TabButtonMiddleTop => s_TabButtonMiddleTop = s_TabButtonMiddleTop ?? GetTextures(AnimationName.TabButtonMiddleTop);
                private static List<Texture2D> s_TabButtonRightBottom;
                public static List<Texture2D> TabButtonRightBottom => s_TabButtonRightBottom = s_TabButtonRightBottom ?? GetTextures(AnimationName.TabButtonRightBottom);
                private static List<Texture2D> s_TabButtonRightFloating;
                public static List<Texture2D> TabButtonRightFloating => s_TabButtonRightFloating = s_TabButtonRightFloating ?? GetTextures(AnimationName.TabButtonRightFloating);
                private static List<Texture2D> s_TabButtonRightTop;
                public static List<Texture2D> TabButtonRightTop => s_TabButtonRightTop = s_TabButtonRightTop ?? GetTextures(AnimationName.TabButtonRightTop);
                private static List<Texture2D> s_TabButtonTopLeft;
                public static List<Texture2D> TabButtonTopLeft => s_TabButtonTopLeft = s_TabButtonTopLeft ?? GetTextures(AnimationName.TabButtonTopLeft);
                private static List<Texture2D> s_TabButtonTopRight;
                public static List<Texture2D> TabButtonTopRight => s_TabButtonTopRight = s_TabButtonTopRight ?? GetTextures(AnimationName.TabButtonTopRight);
                private static List<Texture2D> s_UIMenuHeader;
                public static List<Texture2D> UIMenuHeader => s_UIMenuHeader = s_UIMenuHeader ?? GetTextures(AnimationName.UIMenuHeader);
                private static List<Texture2D> s_VerticalLayout;
                public static List<Texture2D> VerticalLayout => s_VerticalLayout = s_VerticalLayout ?? GetTextures(AnimationName.VerticalLayout);

            }
        }


            
    }
}