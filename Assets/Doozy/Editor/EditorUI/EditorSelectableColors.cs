// Copyright (c) 2015 - 2021 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

//.........................
//.....Generated Class.....
//.........................
//.......Do not edit.......
//.........................

using System.Diagnostics.CodeAnalysis;
using Doozy.Editor.EditorUI.ScriptableObjects.Colors;
using Doozy.Runtime.Colors;
using UnityEngine;

namespace Doozy.Editor.EditorUI
{
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public static class EditorSelectableColors
    {
        public static class Default
        {
            private static EditorDataSelectableColorPalette s_selectableColorPalette;
            private static EditorDataSelectableColorPalette selectableColorPalette =>
                s_selectableColorPalette != null
                    ? s_selectableColorPalette
                    : s_selectableColorPalette = EditorDataSelectableColorDatabase.GetSelectableColorPalette("Default");

            public static Color GetColor(ColorName colorName, SelectionState state) =>
                selectableColorPalette.GetColor(colorName.ToString(), state);

            public static EditorThemeColor GetThemeColor(ColorName colorName, SelectionState state) =>
                selectableColorPalette.GetThemeColor(colorName.ToString(), state);

            public static EditorSelectableColorInfo GetSelectableColorInfo(ColorName colorName) =>
                selectableColorPalette.GetSelectableColorInfo(colorName.ToString());
            
            public enum ColorName
            {
                Action,
                Add,
                ButtonContainer,
                ButtonIcon,
                ButtonText,
                MenuButtonBackgroundLevel0,
                MenuButtonBackgroundLevel1,
                MenuButtonBackgroundLevel2,
                Remove,
                ToggleOffIcon,
                ToggleOffText,
                UnityTheme,
                UnityThemeInversed
            }
            

            private static EditorSelectableColorInfo s_Action;
            public static EditorSelectableColorInfo Action => s_Action ?? (s_Action = GetSelectableColorInfo(ColorName.Action));
            private static EditorSelectableColorInfo s_Add;
            public static EditorSelectableColorInfo Add => s_Add ?? (s_Add = GetSelectableColorInfo(ColorName.Add));
            private static EditorSelectableColorInfo s_ButtonContainer;
            public static EditorSelectableColorInfo ButtonContainer => s_ButtonContainer ?? (s_ButtonContainer = GetSelectableColorInfo(ColorName.ButtonContainer));
            private static EditorSelectableColorInfo s_ButtonIcon;
            public static EditorSelectableColorInfo ButtonIcon => s_ButtonIcon ?? (s_ButtonIcon = GetSelectableColorInfo(ColorName.ButtonIcon));
            private static EditorSelectableColorInfo s_ButtonText;
            public static EditorSelectableColorInfo ButtonText => s_ButtonText ?? (s_ButtonText = GetSelectableColorInfo(ColorName.ButtonText));
            private static EditorSelectableColorInfo s_MenuButtonBackgroundLevel0;
            public static EditorSelectableColorInfo MenuButtonBackgroundLevel0 => s_MenuButtonBackgroundLevel0 ?? (s_MenuButtonBackgroundLevel0 = GetSelectableColorInfo(ColorName.MenuButtonBackgroundLevel0));
            private static EditorSelectableColorInfo s_MenuButtonBackgroundLevel1;
            public static EditorSelectableColorInfo MenuButtonBackgroundLevel1 => s_MenuButtonBackgroundLevel1 ?? (s_MenuButtonBackgroundLevel1 = GetSelectableColorInfo(ColorName.MenuButtonBackgroundLevel1));
            private static EditorSelectableColorInfo s_MenuButtonBackgroundLevel2;
            public static EditorSelectableColorInfo MenuButtonBackgroundLevel2 => s_MenuButtonBackgroundLevel2 ?? (s_MenuButtonBackgroundLevel2 = GetSelectableColorInfo(ColorName.MenuButtonBackgroundLevel2));
            private static EditorSelectableColorInfo s_Remove;
            public static EditorSelectableColorInfo Remove => s_Remove ?? (s_Remove = GetSelectableColorInfo(ColorName.Remove));
            private static EditorSelectableColorInfo s_ToggleOffIcon;
            public static EditorSelectableColorInfo ToggleOffIcon => s_ToggleOffIcon ?? (s_ToggleOffIcon = GetSelectableColorInfo(ColorName.ToggleOffIcon));
            private static EditorSelectableColorInfo s_ToggleOffText;
            public static EditorSelectableColorInfo ToggleOffText => s_ToggleOffText ?? (s_ToggleOffText = GetSelectableColorInfo(ColorName.ToggleOffText));
            private static EditorSelectableColorInfo s_UnityTheme;
            public static EditorSelectableColorInfo UnityTheme => s_UnityTheme ?? (s_UnityTheme = GetSelectableColorInfo(ColorName.UnityTheme));
            private static EditorSelectableColorInfo s_UnityThemeInversed;
            public static EditorSelectableColorInfo UnityThemeInversed => s_UnityThemeInversed ?? (s_UnityThemeInversed = GetSelectableColorInfo(ColorName.UnityThemeInversed));
            
        }

        public static class EditorUI
        {
            private static EditorDataSelectableColorPalette s_selectableColorPalette;
            private static EditorDataSelectableColorPalette selectableColorPalette =>
                s_selectableColorPalette != null
                    ? s_selectableColorPalette
                    : s_selectableColorPalette = EditorDataSelectableColorDatabase.GetSelectableColorPalette("EditorUI");

            public static Color GetColor(ColorName colorName, SelectionState state) =>
                selectableColorPalette.GetColor(colorName.ToString(), state);

            public static EditorThemeColor GetThemeColor(ColorName colorName, SelectionState state) =>
                selectableColorPalette.GetThemeColor(colorName.ToString(), state);

            public static EditorSelectableColorInfo GetSelectableColorInfo(ColorName colorName) =>
                selectableColorPalette.GetSelectableColorInfo(colorName.ToString());
            
            public enum ColorName
            {
                Amber,
                Blue,
                Cyan,
                DeepOrange,
                DeepPurple,
                Green,
                Indigo,
                LightBlue,
                LightGreen,
                Lime,
                Orange,
                Pink,
                Purple,
                Red,
                Teal,
                Yellow
            }
            

            private static EditorSelectableColorInfo s_Amber;
            public static EditorSelectableColorInfo Amber => s_Amber ?? (s_Amber = GetSelectableColorInfo(ColorName.Amber));
            private static EditorSelectableColorInfo s_Blue;
            public static EditorSelectableColorInfo Blue => s_Blue ?? (s_Blue = GetSelectableColorInfo(ColorName.Blue));
            private static EditorSelectableColorInfo s_Cyan;
            public static EditorSelectableColorInfo Cyan => s_Cyan ?? (s_Cyan = GetSelectableColorInfo(ColorName.Cyan));
            private static EditorSelectableColorInfo s_DeepOrange;
            public static EditorSelectableColorInfo DeepOrange => s_DeepOrange ?? (s_DeepOrange = GetSelectableColorInfo(ColorName.DeepOrange));
            private static EditorSelectableColorInfo s_DeepPurple;
            public static EditorSelectableColorInfo DeepPurple => s_DeepPurple ?? (s_DeepPurple = GetSelectableColorInfo(ColorName.DeepPurple));
            private static EditorSelectableColorInfo s_Green;
            public static EditorSelectableColorInfo Green => s_Green ?? (s_Green = GetSelectableColorInfo(ColorName.Green));
            private static EditorSelectableColorInfo s_Indigo;
            public static EditorSelectableColorInfo Indigo => s_Indigo ?? (s_Indigo = GetSelectableColorInfo(ColorName.Indigo));
            private static EditorSelectableColorInfo s_LightBlue;
            public static EditorSelectableColorInfo LightBlue => s_LightBlue ?? (s_LightBlue = GetSelectableColorInfo(ColorName.LightBlue));
            private static EditorSelectableColorInfo s_LightGreen;
            public static EditorSelectableColorInfo LightGreen => s_LightGreen ?? (s_LightGreen = GetSelectableColorInfo(ColorName.LightGreen));
            private static EditorSelectableColorInfo s_Lime;
            public static EditorSelectableColorInfo Lime => s_Lime ?? (s_Lime = GetSelectableColorInfo(ColorName.Lime));
            private static EditorSelectableColorInfo s_Orange;
            public static EditorSelectableColorInfo Orange => s_Orange ?? (s_Orange = GetSelectableColorInfo(ColorName.Orange));
            private static EditorSelectableColorInfo s_Pink;
            public static EditorSelectableColorInfo Pink => s_Pink ?? (s_Pink = GetSelectableColorInfo(ColorName.Pink));
            private static EditorSelectableColorInfo s_Purple;
            public static EditorSelectableColorInfo Purple => s_Purple ?? (s_Purple = GetSelectableColorInfo(ColorName.Purple));
            private static EditorSelectableColorInfo s_Red;
            public static EditorSelectableColorInfo Red => s_Red ?? (s_Red = GetSelectableColorInfo(ColorName.Red));
            private static EditorSelectableColorInfo s_Teal;
            public static EditorSelectableColorInfo Teal => s_Teal ?? (s_Teal = GetSelectableColorInfo(ColorName.Teal));
            private static EditorSelectableColorInfo s_Yellow;
            public static EditorSelectableColorInfo Yellow => s_Yellow ?? (s_Yellow = GetSelectableColorInfo(ColorName.Yellow));
            
        }

        public static class Mody
        {
            private static EditorDataSelectableColorPalette s_selectableColorPalette;
            private static EditorDataSelectableColorPalette selectableColorPalette =>
                s_selectableColorPalette != null
                    ? s_selectableColorPalette
                    : s_selectableColorPalette = EditorDataSelectableColorDatabase.GetSelectableColorPalette("Mody");

            public static Color GetColor(ColorName colorName, SelectionState state) =>
                selectableColorPalette.GetColor(colorName.ToString(), state);

            public static EditorThemeColor GetThemeColor(ColorName colorName, SelectionState state) =>
                selectableColorPalette.GetThemeColor(colorName.ToString(), state);

            public static EditorSelectableColorInfo GetSelectableColorInfo(ColorName colorName) =>
                selectableColorPalette.GetSelectableColorInfo(colorName.ToString());
            
            public enum ColorName
            {
                Action,
                Module,
                StateActive,
                StateCooldown,
                StateDisabled,
                StateIdle,
                Trigger
            }
            

            private static EditorSelectableColorInfo s_Action;
            public static EditorSelectableColorInfo Action => s_Action ?? (s_Action = GetSelectableColorInfo(ColorName.Action));
            private static EditorSelectableColorInfo s_Module;
            public static EditorSelectableColorInfo Module => s_Module ?? (s_Module = GetSelectableColorInfo(ColorName.Module));
            private static EditorSelectableColorInfo s_StateActive;
            public static EditorSelectableColorInfo StateActive => s_StateActive ?? (s_StateActive = GetSelectableColorInfo(ColorName.StateActive));
            private static EditorSelectableColorInfo s_StateCooldown;
            public static EditorSelectableColorInfo StateCooldown => s_StateCooldown ?? (s_StateCooldown = GetSelectableColorInfo(ColorName.StateCooldown));
            private static EditorSelectableColorInfo s_StateDisabled;
            public static EditorSelectableColorInfo StateDisabled => s_StateDisabled ?? (s_StateDisabled = GetSelectableColorInfo(ColorName.StateDisabled));
            private static EditorSelectableColorInfo s_StateIdle;
            public static EditorSelectableColorInfo StateIdle => s_StateIdle ?? (s_StateIdle = GetSelectableColorInfo(ColorName.StateIdle));
            private static EditorSelectableColorInfo s_Trigger;
            public static EditorSelectableColorInfo Trigger => s_Trigger ?? (s_Trigger = GetSelectableColorInfo(ColorName.Trigger));
            
        }

        public static class Nody
        {
            private static EditorDataSelectableColorPalette s_selectableColorPalette;
            private static EditorDataSelectableColorPalette selectableColorPalette =>
                s_selectableColorPalette != null
                    ? s_selectableColorPalette
                    : s_selectableColorPalette = EditorDataSelectableColorDatabase.GetSelectableColorPalette("Nody");

            public static Color GetColor(ColorName colorName, SelectionState state) =>
                selectableColorPalette.GetColor(colorName.ToString(), state);

            public static EditorThemeColor GetThemeColor(ColorName colorName, SelectionState state) =>
                selectableColorPalette.GetThemeColor(colorName.ToString(), state);

            public static EditorSelectableColorInfo GetSelectableColorInfo(ColorName colorName) =>
                selectableColorPalette.GetSelectableColorInfo(colorName.ToString());
            
            public enum ColorName
            {
                BackFlow,
                Color,
                Input,
                Output
            }
            

            private static EditorSelectableColorInfo s_BackFlow;
            public static EditorSelectableColorInfo BackFlow => s_BackFlow ?? (s_BackFlow = GetSelectableColorInfo(ColorName.BackFlow));
            private static EditorSelectableColorInfo s_Color;
            public static EditorSelectableColorInfo Color => s_Color ?? (s_Color = GetSelectableColorInfo(ColorName.Color));
            private static EditorSelectableColorInfo s_Input;
            public static EditorSelectableColorInfo Input => s_Input ?? (s_Input = GetSelectableColorInfo(ColorName.Input));
            private static EditorSelectableColorInfo s_Output;
            public static EditorSelectableColorInfo Output => s_Output ?? (s_Output = GetSelectableColorInfo(ColorName.Output));
            
        }

        public static class Reactor
        {
            private static EditorDataSelectableColorPalette s_selectableColorPalette;
            private static EditorDataSelectableColorPalette selectableColorPalette =>
                s_selectableColorPalette != null
                    ? s_selectableColorPalette
                    : s_selectableColorPalette = EditorDataSelectableColorDatabase.GetSelectableColorPalette("Reactor");

            public static Color GetColor(ColorName colorName, SelectionState state) =>
                selectableColorPalette.GetColor(colorName.ToString(), state);

            public static EditorThemeColor GetThemeColor(ColorName colorName, SelectionState state) =>
                selectableColorPalette.GetThemeColor(colorName.ToString(), state);

            public static EditorSelectableColorInfo GetSelectableColorInfo(ColorName colorName) =>
                selectableColorPalette.GetSelectableColorInfo(colorName.ToString());
            
            public enum ColorName
            {
                Fade,
                Gray,
                Green,
                Move,
                Red,
                Rotate,
                Scale
            }
            

            private static EditorSelectableColorInfo s_Fade;
            public static EditorSelectableColorInfo Fade => s_Fade ?? (s_Fade = GetSelectableColorInfo(ColorName.Fade));
            private static EditorSelectableColorInfo s_Gray;
            public static EditorSelectableColorInfo Gray => s_Gray ?? (s_Gray = GetSelectableColorInfo(ColorName.Gray));
            private static EditorSelectableColorInfo s_Green;
            public static EditorSelectableColorInfo Green => s_Green ?? (s_Green = GetSelectableColorInfo(ColorName.Green));
            private static EditorSelectableColorInfo s_Move;
            public static EditorSelectableColorInfo Move => s_Move ?? (s_Move = GetSelectableColorInfo(ColorName.Move));
            private static EditorSelectableColorInfo s_Red;
            public static EditorSelectableColorInfo Red => s_Red ?? (s_Red = GetSelectableColorInfo(ColorName.Red));
            private static EditorSelectableColorInfo s_Rotate;
            public static EditorSelectableColorInfo Rotate => s_Rotate ?? (s_Rotate = GetSelectableColorInfo(ColorName.Rotate));
            private static EditorSelectableColorInfo s_Scale;
            public static EditorSelectableColorInfo Scale => s_Scale ?? (s_Scale = GetSelectableColorInfo(ColorName.Scale));
            
        }

        public static class Signals
        {
            private static EditorDataSelectableColorPalette s_selectableColorPalette;
            private static EditorDataSelectableColorPalette selectableColorPalette =>
                s_selectableColorPalette != null
                    ? s_selectableColorPalette
                    : s_selectableColorPalette = EditorDataSelectableColorDatabase.GetSelectableColorPalette("Signals");

            public static Color GetColor(ColorName colorName, SelectionState state) =>
                selectableColorPalette.GetColor(colorName.ToString(), state);

            public static EditorThemeColor GetThemeColor(ColorName colorName, SelectionState state) =>
                selectableColorPalette.GetThemeColor(colorName.ToString(), state);

            public static EditorSelectableColorInfo GetSelectableColorInfo(ColorName colorName) =>
                selectableColorPalette.GetSelectableColorInfo(colorName.ToString());
            
            public enum ColorName
            {
                Provider,
                Receiver,
                Signal,
                Stream
            }
            

            private static EditorSelectableColorInfo s_Provider;
            public static EditorSelectableColorInfo Provider => s_Provider ?? (s_Provider = GetSelectableColorInfo(ColorName.Provider));
            private static EditorSelectableColorInfo s_Receiver;
            public static EditorSelectableColorInfo Receiver => s_Receiver ?? (s_Receiver = GetSelectableColorInfo(ColorName.Receiver));
            private static EditorSelectableColorInfo s_Signal;
            public static EditorSelectableColorInfo Signal => s_Signal ?? (s_Signal = GetSelectableColorInfo(ColorName.Signal));
            private static EditorSelectableColorInfo s_Stream;
            public static EditorSelectableColorInfo Stream => s_Stream ?? (s_Stream = GetSelectableColorInfo(ColorName.Stream));
            
        }

        public static class UIManager
        {
            private static EditorDataSelectableColorPalette s_selectableColorPalette;
            private static EditorDataSelectableColorPalette selectableColorPalette =>
                s_selectableColorPalette != null
                    ? s_selectableColorPalette
                    : s_selectableColorPalette = EditorDataSelectableColorDatabase.GetSelectableColorPalette("UIManager");

            public static Color GetColor(ColorName colorName, SelectionState state) =>
                selectableColorPalette.GetColor(colorName.ToString(), state);

            public static EditorThemeColor GetThemeColor(ColorName colorName, SelectionState state) =>
                selectableColorPalette.GetThemeColor(colorName.ToString(), state);

            public static EditorSelectableColorInfo GetSelectableColorInfo(ColorName colorName) =>
                selectableColorPalette.GetSelectableColorInfo(colorName.ToString());
            
            public enum ColorName
            {
                AudioComponent,
                InputComponent,
                LayoutComponent,
                ListenerComponent,
                Settings,
                UIComponent,
                VisualComponent
            }
            

            private static EditorSelectableColorInfo s_AudioComponent;
            public static EditorSelectableColorInfo AudioComponent => s_AudioComponent ?? (s_AudioComponent = GetSelectableColorInfo(ColorName.AudioComponent));
            private static EditorSelectableColorInfo s_InputComponent;
            public static EditorSelectableColorInfo InputComponent => s_InputComponent ?? (s_InputComponent = GetSelectableColorInfo(ColorName.InputComponent));
            private static EditorSelectableColorInfo s_LayoutComponent;
            public static EditorSelectableColorInfo LayoutComponent => s_LayoutComponent ?? (s_LayoutComponent = GetSelectableColorInfo(ColorName.LayoutComponent));
            private static EditorSelectableColorInfo s_ListenerComponent;
            public static EditorSelectableColorInfo ListenerComponent => s_ListenerComponent ?? (s_ListenerComponent = GetSelectableColorInfo(ColorName.ListenerComponent));
            private static EditorSelectableColorInfo s_Settings;
            public static EditorSelectableColorInfo Settings => s_Settings ?? (s_Settings = GetSelectableColorInfo(ColorName.Settings));
            private static EditorSelectableColorInfo s_UIComponent;
            public static EditorSelectableColorInfo UIComponent => s_UIComponent ?? (s_UIComponent = GetSelectableColorInfo(ColorName.UIComponent));
            private static EditorSelectableColorInfo s_VisualComponent;
            public static EditorSelectableColorInfo VisualComponent => s_VisualComponent ?? (s_VisualComponent = GetSelectableColorInfo(ColorName.VisualComponent));
            
        }
    }
}