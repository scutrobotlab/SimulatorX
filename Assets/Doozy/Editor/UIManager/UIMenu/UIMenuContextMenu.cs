// Copyright (c) 2015 - 2021 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

//.........................
//.....Generated Class.....
//.........................
//.......Do not edit.......
//.........................

using UnityEditor;
// ReSharper disable All
namespace Doozy.Editor.UIManager.UIMenu
{
    public static class UIMenuContextMenu
    {
        private const int MENU_ITEM_PRIORITY = 11;
        private const string MENU_PATH = "GameObject/Doozy";

        public static class Component
        {
            private const string TYPE_NAME = "Component";
            private const string TYPE_MENU_PATH = MENU_PATH + "/" + TYPE_NAME + "/";

            public static class Checkbox
            {
                private const string CATEGORY_NAME = "Checkbox";
                private const string CATEGORY_MENU_PATH = TYPE_MENU_PATH + CATEGORY_NAME + "/";

                [MenuItem(CATEGORY_MENU_PATH + "Checkbox (Large)", false, MENU_ITEM_PRIORITY)]
                public static void CreateCheckboxLarge(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "CheckboxLarge");

                [MenuItem(CATEGORY_MENU_PATH + "Checkbox (Normal)", false, MENU_ITEM_PRIORITY)]
                public static void CreateCheckboxNormal(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "CheckboxNormal");

                [MenuItem(CATEGORY_MENU_PATH + "Checkbox (Small)", false, MENU_ITEM_PRIORITY)]
                public static void CreateCheckboxSmall(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "CheckboxSmall");

                [MenuItem(CATEGORY_MENU_PATH + "Checkbox (Tiny)", false, MENU_ITEM_PRIORITY)]
                public static void CreateCheckboxTiny(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "CheckboxTiny");
            }

            public static class FlexButton
            {
                private const string CATEGORY_NAME = "Flex Button";
                private const string CATEGORY_MENU_PATH = TYPE_MENU_PATH + CATEGORY_NAME + "/";

                [MenuItem(CATEGORY_MENU_PATH + "Flex Button (Large)", false, MENU_ITEM_PRIORITY)]
                public static void CreateFlexButtonLarge(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "FlexButtonLarge");

                [MenuItem(CATEGORY_MENU_PATH + "Flex Button (Normal)", false, MENU_ITEM_PRIORITY)]
                public static void CreateFlexButtonNormal(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "FlexButtonNormal");

                [MenuItem(CATEGORY_MENU_PATH + "Flex Button (Small)", false, MENU_ITEM_PRIORITY)]
                public static void CreateFlexButtonSmall(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "FlexButtonSmall");

                [MenuItem(CATEGORY_MENU_PATH + "Flex Button (Tiny)", false, MENU_ITEM_PRIORITY)]
                public static void CreateFlexButtonTiny(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "FlexButtonTiny");
            }

            public static class FlexCheckbox
            {
                private const string CATEGORY_NAME = "Flex Checkbox";
                private const string CATEGORY_MENU_PATH = TYPE_MENU_PATH + CATEGORY_NAME + "/";

                [MenuItem(CATEGORY_MENU_PATH + "Flex Checkbox (Large)", false, MENU_ITEM_PRIORITY)]
                public static void CreateFlexCheckboxLarge(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "FlexCheckboxLarge");

                [MenuItem(CATEGORY_MENU_PATH + "Flex Checkbox (Normal)", false, MENU_ITEM_PRIORITY)]
                public static void CreateFlexCheckboxNormal(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "FlexCheckboxNormal");

                [MenuItem(CATEGORY_MENU_PATH + "Flex Checkbox (Small)", false, MENU_ITEM_PRIORITY)]
                public static void CreateFlexCheckboxSmall(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "FlexCheckboxSmall");

                [MenuItem(CATEGORY_MENU_PATH + "Flex Checkbox (Tiny)", false, MENU_ITEM_PRIORITY)]
                public static void CreateFlexCheckboxTiny(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "FlexCheckboxTiny");
            }

            public static class FlexIconButton
            {
                private const string CATEGORY_NAME = "Flex Icon Button";
                private const string CATEGORY_MENU_PATH = TYPE_MENU_PATH + CATEGORY_NAME + "/";

                [MenuItem(CATEGORY_MENU_PATH + "Flex Icon Button (Large)", false, MENU_ITEM_PRIORITY)]
                public static void CreateFlexIconButtonLarge(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "FlexIconButtonLarge");

                [MenuItem(CATEGORY_MENU_PATH + "Flex Icon Button (Normal)", false, MENU_ITEM_PRIORITY)]
                public static void CreateFlexIconButtonNormal(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "FlexIconButtonNormal");

                [MenuItem(CATEGORY_MENU_PATH + "Flex Icon Button (Small)", false, MENU_ITEM_PRIORITY)]
                public static void CreateFlexIconButtonSmall(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "FlexIconButtonSmall");

                [MenuItem(CATEGORY_MENU_PATH + "Flex Icon Button (Tiny)", false, MENU_ITEM_PRIORITY)]
                public static void CreateFlexIconButtonTiny(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "FlexIconButtonTiny");
            }

            public static class FlexRadio
            {
                private const string CATEGORY_NAME = "Flex Radio";
                private const string CATEGORY_MENU_PATH = TYPE_MENU_PATH + CATEGORY_NAME + "/";

                [MenuItem(CATEGORY_MENU_PATH + "Flex Radio (Large)", false, MENU_ITEM_PRIORITY)]
                public static void CreateFlexRadioLarge(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "FlexRadioLarge");

                [MenuItem(CATEGORY_MENU_PATH + "Flex Radio (Normal)", false, MENU_ITEM_PRIORITY)]
                public static void CreateFlexRadioNormal(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "FlexRadioNormal");

                [MenuItem(CATEGORY_MENU_PATH + "Flex Radio (Small)", false, MENU_ITEM_PRIORITY)]
                public static void CreateFlexRadioSmall(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "FlexRadioSmall");

                [MenuItem(CATEGORY_MENU_PATH + "Flex Radio (Tiny)", false, MENU_ITEM_PRIORITY)]
                public static void CreateFlexRadioTiny(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "FlexRadioTiny");
            }

            public static class FlexSwitch
            {
                private const string CATEGORY_NAME = "Flex Switch";
                private const string CATEGORY_MENU_PATH = TYPE_MENU_PATH + CATEGORY_NAME + "/";

                [MenuItem(CATEGORY_MENU_PATH + "Flex Switch (Large)", false, MENU_ITEM_PRIORITY)]
                public static void CreateFlexSwitchLarge(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "FlexSwitchLarge");

                [MenuItem(CATEGORY_MENU_PATH + "Flex Switch (Normal)", false, MENU_ITEM_PRIORITY)]
                public static void CreateFlexSwitchNormal(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "FlexSwitchNormal");

                [MenuItem(CATEGORY_MENU_PATH + "Flex Switch (Small)", false, MENU_ITEM_PRIORITY)]
                public static void CreateFlexSwitchSmall(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "FlexSwitchSmall");

                [MenuItem(CATEGORY_MENU_PATH + "Flex Switch (Tiny)", false, MENU_ITEM_PRIORITY)]
                public static void CreateFlexSwitchTiny(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "FlexSwitchTiny");
            }

            public static class IconButton
            {
                private const string CATEGORY_NAME = "Icon Button";
                private const string CATEGORY_MENU_PATH = TYPE_MENU_PATH + CATEGORY_NAME + "/";

                [MenuItem(CATEGORY_MENU_PATH + "Icon Button (Large)", false, MENU_ITEM_PRIORITY)]
                public static void CreateIconButtonLarge(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "IconButtonLarge");

                [MenuItem(CATEGORY_MENU_PATH + "Icon Button (Normal)", false, MENU_ITEM_PRIORITY)]
                public static void CreateIconButtonNormal(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "IconButtonNormal");

                [MenuItem(CATEGORY_MENU_PATH + "Icon Button (Small)", false, MENU_ITEM_PRIORITY)]
                public static void CreateIconButtonSmall(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "IconButtonSmall");

                [MenuItem(CATEGORY_MENU_PATH + "Icon Button (Tiny)", false, MENU_ITEM_PRIORITY)]
                public static void CreateIconButtonTiny(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "IconButtonTiny");
            }

            public static class IconButtonAction
            {
                private const string CATEGORY_NAME = "Icon Button Action";
                private const string CATEGORY_MENU_PATH = TYPE_MENU_PATH + CATEGORY_NAME + "/";

                [MenuItem(CATEGORY_MENU_PATH + "Add To Cart", false, MENU_ITEM_PRIORITY)]
                public static void CreateAddToCart(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "AddToCart");

                [MenuItem(CATEGORY_MENU_PATH + "Add To Cart Bag", false, MENU_ITEM_PRIORITY)]
                public static void CreateAddToCartBag(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "AddToCartBag");

                [MenuItem(CATEGORY_MENU_PATH + "Add To Favorites", false, MENU_ITEM_PRIORITY)]
                public static void CreateAddToFavorites(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "AddToFavorites");

                [MenuItem(CATEGORY_MENU_PATH + "Calendar", false, MENU_ITEM_PRIORITY)]
                public static void CreateCalendar(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "Calendar");

                [MenuItem(CATEGORY_MENU_PATH + "Calendar Days", false, MENU_ITEM_PRIORITY)]
                public static void CreateCalendarDays(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "CalendarDays");

                [MenuItem(CATEGORY_MENU_PATH + "Camera", false, MENU_ITEM_PRIORITY)]
                public static void CreateCamera(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "Camera");

                [MenuItem(CATEGORY_MENU_PATH + "Close X Mark", false, MENU_ITEM_PRIORITY)]
                public static void CreateCloseXMark(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "CloseXMark");

                [MenuItem(CATEGORY_MENU_PATH + "Gamepad", false, MENU_ITEM_PRIORITY)]
                public static void CreateGamepad(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "Gamepad");

                [MenuItem(CATEGORY_MENU_PATH + "Gift", false, MENU_ITEM_PRIORITY)]
                public static void CreateGift(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "Gift");

                [MenuItem(CATEGORY_MENU_PATH + "Globe", false, MENU_ITEM_PRIORITY)]
                public static void CreateGlobe(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "Globe");

                [MenuItem(CATEGORY_MENU_PATH + "Id Badge", false, MENU_ITEM_PRIORITY)]
                public static void CreateIdBadge(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "IdBadge");

                [MenuItem(CATEGORY_MENU_PATH + "Key", false, MENU_ITEM_PRIORITY)]
                public static void CreateKey(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "Key");

                [MenuItem(CATEGORY_MENU_PATH + "Language", false, MENU_ITEM_PRIORITY)]
                public static void CreateLanguage(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "Language");

                [MenuItem(CATEGORY_MENU_PATH + "Map", false, MENU_ITEM_PRIORITY)]
                public static void CreateMap(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "Map");

                [MenuItem(CATEGORY_MENU_PATH + "Minus", false, MENU_ITEM_PRIORITY)]
                public static void CreateMinus(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "Minus");

                [MenuItem(CATEGORY_MENU_PATH + "Plus", false, MENU_ITEM_PRIORITY)]
                public static void CreatePlus(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "Plus");

                [MenuItem(CATEGORY_MENU_PATH + "Redo", false, MENU_ITEM_PRIORITY)]
                public static void CreateRedo(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "Redo");

                [MenuItem(CATEGORY_MENU_PATH + "Refresh", false, MENU_ITEM_PRIORITY)]
                public static void CreateRefresh(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "Refresh");

                [MenuItem(CATEGORY_MENU_PATH + "Report Bug", false, MENU_ITEM_PRIORITY)]
                public static void CreateReportBug(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "ReportBug");

                [MenuItem(CATEGORY_MENU_PATH + "Search", false, MENU_ITEM_PRIORITY)]
                public static void CreateSearch(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "Search");

                [MenuItem(CATEGORY_MENU_PATH + "Send Email", false, MENU_ITEM_PRIORITY)]
                public static void CreateSendEmail(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "SendEmail");

                [MenuItem(CATEGORY_MENU_PATH + "Share", false, MENU_ITEM_PRIORITY)]
                public static void CreateShare(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "Share");

                [MenuItem(CATEGORY_MENU_PATH + "Share Nodes", false, MENU_ITEM_PRIORITY)]
                public static void CreateShareNodes(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "ShareNodes");

                [MenuItem(CATEGORY_MENU_PATH + "Star", false, MENU_ITEM_PRIORITY)]
                public static void CreateStar(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "Star");

                [MenuItem(CATEGORY_MENU_PATH + "Tag", false, MENU_ITEM_PRIORITY)]
                public static void CreateTag(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "Tag");

                [MenuItem(CATEGORY_MENU_PATH + "Undo", false, MENU_ITEM_PRIORITY)]
                public static void CreateUndo(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "Undo");

                [MenuItem(CATEGORY_MENU_PATH + "User", false, MENU_ITEM_PRIORITY)]
                public static void CreateUser(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "User");

                [MenuItem(CATEGORY_MENU_PATH + "Users", false, MENU_ITEM_PRIORITY)]
                public static void CreateUsers(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "Users");
            }

            public static class IconButtonDirection
            {
                private const string CATEGORY_NAME = "Icon Button Direction";
                private const string CATEGORY_MENU_PATH = TYPE_MENU_PATH + CATEGORY_NAME + "/";

                [MenuItem(CATEGORY_MENU_PATH + "Arrow Down", false, MENU_ITEM_PRIORITY)]
                public static void CreateArrowDown(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "ArrowDown");

                [MenuItem(CATEGORY_MENU_PATH + "Arrow Left", false, MENU_ITEM_PRIORITY)]
                public static void CreateArrowLeft(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "ArrowLeft");

                [MenuItem(CATEGORY_MENU_PATH + "Arrow Right", false, MENU_ITEM_PRIORITY)]
                public static void CreateArrowRight(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "ArrowRight");

                [MenuItem(CATEGORY_MENU_PATH + "Arrow Up", false, MENU_ITEM_PRIORITY)]
                public static void CreateArrowUp(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "ArrowUp");

                [MenuItem(CATEGORY_MENU_PATH + "Chevron Down", false, MENU_ITEM_PRIORITY)]
                public static void CreateChevronDown(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "ChevronDown");

                [MenuItem(CATEGORY_MENU_PATH + "Chevron Left", false, MENU_ITEM_PRIORITY)]
                public static void CreateChevronLeft(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "ChevronLeft");

                [MenuItem(CATEGORY_MENU_PATH + "Chevron Right", false, MENU_ITEM_PRIORITY)]
                public static void CreateChevronRight(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "ChevronRight");

                [MenuItem(CATEGORY_MENU_PATH + "Chevron Up", false, MENU_ITEM_PRIORITY)]
                public static void CreateChevronUp(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "ChevronUp");

                [MenuItem(CATEGORY_MENU_PATH + "Circle Arrow Down", false, MENU_ITEM_PRIORITY)]
                public static void CreateCircleArrowDown(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "CircleArrowDown");

                [MenuItem(CATEGORY_MENU_PATH + "Circle Arrow Left", false, MENU_ITEM_PRIORITY)]
                public static void CreateCircleArrowLeft(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "CircleArrowLeft");

                [MenuItem(CATEGORY_MENU_PATH + "Circle Arrow Right", false, MENU_ITEM_PRIORITY)]
                public static void CreateCircleArrowRight(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "CircleArrowRight");

                [MenuItem(CATEGORY_MENU_PATH + "Circle Arrow Up", false, MENU_ITEM_PRIORITY)]
                public static void CreateCircleArrowUp(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "CircleArrowUp");

                [MenuItem(CATEGORY_MENU_PATH + "Circle Chevron Down", false, MENU_ITEM_PRIORITY)]
                public static void CreateCircleChevronDown(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "CircleChevronDown");

                [MenuItem(CATEGORY_MENU_PATH + "Circle Chevron Left", false, MENU_ITEM_PRIORITY)]
                public static void CreateCircleChevronLeft(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "CircleChevronLeft");

                [MenuItem(CATEGORY_MENU_PATH + "Circle Chevron Right", false, MENU_ITEM_PRIORITY)]
                public static void CreateCircleChevronRight(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "CircleChevronRight");

                [MenuItem(CATEGORY_MENU_PATH + "Circle Chevron Up", false, MENU_ITEM_PRIORITY)]
                public static void CreateCircleChevronUp(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "CircleChevronUp");
            }

            public static class IconButtonMedia
            {
                private const string CATEGORY_NAME = "Icon Button Media";
                private const string CATEGORY_MENU_PATH = TYPE_MENU_PATH + CATEGORY_NAME + "/";

                [MenuItem(CATEGORY_MENU_PATH + "Backward", false, MENU_ITEM_PRIORITY)]
                public static void CreateBackward(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "Backward");

                [MenuItem(CATEGORY_MENU_PATH + "Backward Step", false, MENU_ITEM_PRIORITY)]
                public static void CreateBackwardStep(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "BackwardStep");

                [MenuItem(CATEGORY_MENU_PATH + "Forward", false, MENU_ITEM_PRIORITY)]
                public static void CreateForward(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "Forward");

                [MenuItem(CATEGORY_MENU_PATH + "Forward Step", false, MENU_ITEM_PRIORITY)]
                public static void CreateForwardStep(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "ForwardStep");

                [MenuItem(CATEGORY_MENU_PATH + "Pause", false, MENU_ITEM_PRIORITY)]
                public static void CreatePause(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "Pause");

                [MenuItem(CATEGORY_MENU_PATH + "Play", false, MENU_ITEM_PRIORITY)]
                public static void CreatePlay(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "Play");

                [MenuItem(CATEGORY_MENU_PATH + "Play Pause", false, MENU_ITEM_PRIORITY)]
                public static void CreatePlayPause(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "PlayPause");

                [MenuItem(CATEGORY_MENU_PATH + "Repeat", false, MENU_ITEM_PRIORITY)]
                public static void CreateRepeat(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "Repeat");

                [MenuItem(CATEGORY_MENU_PATH + "Stop", false, MENU_ITEM_PRIORITY)]
                public static void CreateStop(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "Stop");
            }

            public static class IconToggle
            {
                private const string CATEGORY_NAME = "Icon Toggle";
                private const string CATEGORY_MENU_PATH = TYPE_MENU_PATH + CATEGORY_NAME + "/";

                [MenuItem(CATEGORY_MENU_PATH + "Icon Toggle (Large)", false, MENU_ITEM_PRIORITY)]
                public static void CreateIconToggleLarge(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "IconToggleLarge");

                [MenuItem(CATEGORY_MENU_PATH + "Icon Toggle (Normal)", false, MENU_ITEM_PRIORITY)]
                public static void CreateIconToggleNormal(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "IconToggleNormal");

                [MenuItem(CATEGORY_MENU_PATH + "Icon Toggle (Small)", false, MENU_ITEM_PRIORITY)]
                public static void CreateIconToggleSmall(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "IconToggleSmall");

                [MenuItem(CATEGORY_MENU_PATH + "Icon Toggle (Tiny)", false, MENU_ITEM_PRIORITY)]
                public static void CreateIconToggleTiny(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "IconToggleTiny");
            }

            public static class Radio
            {
                private const string CATEGORY_NAME = "Radio";
                private const string CATEGORY_MENU_PATH = TYPE_MENU_PATH + CATEGORY_NAME + "/";

                [MenuItem(CATEGORY_MENU_PATH + "Radio (Large)", false, MENU_ITEM_PRIORITY)]
                public static void CreateRadioLarge(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "RadioLarge");

                [MenuItem(CATEGORY_MENU_PATH + "Radio (Normal)", false, MENU_ITEM_PRIORITY)]
                public static void CreateRadioNormal(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "RadioNormal");

                [MenuItem(CATEGORY_MENU_PATH + "Radio (Small)", false, MENU_ITEM_PRIORITY)]
                public static void CreateRadioSmall(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "RadioSmall");

                [MenuItem(CATEGORY_MENU_PATH + "Radio (Tiny)", false, MENU_ITEM_PRIORITY)]
                public static void CreateRadioTiny(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "RadioTiny");
            }

            public static class Scrollbar
            {
                private const string CATEGORY_NAME = "Scrollbar";
                private const string CATEGORY_MENU_PATH = TYPE_MENU_PATH + CATEGORY_NAME + "/";

                [MenuItem(CATEGORY_MENU_PATH + "Simple Scrollbar", false, MENU_ITEM_PRIORITY)]
                public static void CreateSimpleScrollbar(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "SimpleScrollbar");
            }

            public static class SimpleButton
            {
                private const string CATEGORY_NAME = "Simple Button";
                private const string CATEGORY_MENU_PATH = TYPE_MENU_PATH + CATEGORY_NAME + "/";

                [MenuItem(CATEGORY_MENU_PATH + "Simple Button (Large)", false, MENU_ITEM_PRIORITY)]
                public static void CreateSimpleButtonLarge(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "SimpleButtonLarge");

                [MenuItem(CATEGORY_MENU_PATH + "Simple Button (Normal)", false, MENU_ITEM_PRIORITY)]
                public static void CreateSimpleButtonNormal(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "SimpleButtonNormal");

                [MenuItem(CATEGORY_MENU_PATH + "Simple Button (Small)", false, MENU_ITEM_PRIORITY)]
                public static void CreateSimpleButtonSmall(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "SimpleButtonSmall");

                [MenuItem(CATEGORY_MENU_PATH + "Simple Button (Tiny)", false, MENU_ITEM_PRIORITY)]
                public static void CreateSimpleButtonTiny(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "SimpleButtonTiny");
            }

            public static class SimpleIconButton
            {
                private const string CATEGORY_NAME = "Simple Icon Button";
                private const string CATEGORY_MENU_PATH = TYPE_MENU_PATH + CATEGORY_NAME + "/";

                [MenuItem(CATEGORY_MENU_PATH + "Simple Icon Button (Large)", false, MENU_ITEM_PRIORITY)]
                public static void CreateSimpleIconButtonLarge(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "SimpleIconButtonLarge");

                [MenuItem(CATEGORY_MENU_PATH + "Simple Icon Button (Normal)", false, MENU_ITEM_PRIORITY)]
                public static void CreateSimpleIconButtonNormal(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "SimpleIconButtonNormal");

                [MenuItem(CATEGORY_MENU_PATH + "Simple Icon Button (Small)", false, MENU_ITEM_PRIORITY)]
                public static void CreateSimpleIconButtonSmall(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "SimpleIconButtonSmall");

                [MenuItem(CATEGORY_MENU_PATH + "Simple Icon Button (Tiny)", false, MENU_ITEM_PRIORITY)]
                public static void CreateSimpleIconButtonTiny(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "SimpleIconButtonTiny");
            }

            public static class Slider
            {
                private const string CATEGORY_NAME = "Slider";
                private const string CATEGORY_MENU_PATH = TYPE_MENU_PATH + CATEGORY_NAME + "/";

                [MenuItem(CATEGORY_MENU_PATH + "Simple Slider", false, MENU_ITEM_PRIORITY)]
                public static void CreateSimpleSlider(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "SimpleSlider");
            }

            public static class Switch
            {
                private const string CATEGORY_NAME = "Switch";
                private const string CATEGORY_MENU_PATH = TYPE_MENU_PATH + CATEGORY_NAME + "/";

                [MenuItem(CATEGORY_MENU_PATH + "Switch (Large)", false, MENU_ITEM_PRIORITY)]
                public static void CreateSwitchLarge(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "SwitchLarge");

                [MenuItem(CATEGORY_MENU_PATH + "Switch (Normal)", false, MENU_ITEM_PRIORITY)]
                public static void CreateSwitchNormal(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "SwitchNormal");

                [MenuItem(CATEGORY_MENU_PATH + "Switch (Small)", false, MENU_ITEM_PRIORITY)]
                public static void CreateSwitchSmall(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "SwitchSmall");

                [MenuItem(CATEGORY_MENU_PATH + "Switch (Tiny)", false, MENU_ITEM_PRIORITY)]
                public static void CreateSwitchTiny(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "SwitchTiny");
            }
        }

        public static class Container
        {
            private const string TYPE_NAME = "Container";
            private const string TYPE_MENU_PATH = MENU_PATH + "/" + TYPE_NAME + "/";

            public static class Basic
            {
                private const string CATEGORY_NAME = "Basic";
                private const string CATEGORY_MENU_PATH = TYPE_MENU_PATH + CATEGORY_NAME + "/";

                [MenuItem(CATEGORY_MENU_PATH + "Simple Container", false, MENU_ITEM_PRIORITY)]
                public static void CreateSimpleContainer(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "SimpleContainer");

                [MenuItem(CATEGORY_MENU_PATH + "UI View", false, MENU_ITEM_PRIORITY)]
                public static void CreateUIView(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "UIView");
            }

            public static class TabLayouts
            {
                private const string CATEGORY_NAME = "Tab Layouts";
                private const string CATEGORY_MENU_PATH = TYPE_MENU_PATH + CATEGORY_NAME + "/";

                [MenuItem(CATEGORY_MENU_PATH + "Tab Layout (Bottom Left)", false, MENU_ITEM_PRIORITY)]
                public static void CreateTabLayoutBottomLeft(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "TabLayoutBottomLeft");

                [MenuItem(CATEGORY_MENU_PATH + "Tab Layout (Bottom Right)", false, MENU_ITEM_PRIORITY)]
                public static void CreateTabLayoutBottomRight(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "TabLayoutBottomRight");

                [MenuItem(CATEGORY_MENU_PATH + "Tab Layout (Bottom)", false, MENU_ITEM_PRIORITY)]
                public static void CreateTabLayoutBottom(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "TabLayoutBottom");

                [MenuItem(CATEGORY_MENU_PATH + "Tab Layout (Left Bottom)", false, MENU_ITEM_PRIORITY)]
                public static void CreateTabLayoutLeftBottom(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "TabLayoutLeftBottom");

                [MenuItem(CATEGORY_MENU_PATH + "Tab Layout (Left Top)", false, MENU_ITEM_PRIORITY)]
                public static void CreateTabLayoutLeftTop(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "TabLayoutLeftTop");

                [MenuItem(CATEGORY_MENU_PATH + "Tab Layout (Left)", false, MENU_ITEM_PRIORITY)]
                public static void CreateTabLayoutLeft(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "TabLayoutLeft");

                [MenuItem(CATEGORY_MENU_PATH + "Tab Layout (Right Bottom)", false, MENU_ITEM_PRIORITY)]
                public static void CreateTabLayoutRightBottom(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "TabLayoutRightBottom");

                [MenuItem(CATEGORY_MENU_PATH + "Tab Layout (Right Top)", false, MENU_ITEM_PRIORITY)]
                public static void CreateTabLayoutRightTop(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "TabLayoutRightTop");

                [MenuItem(CATEGORY_MENU_PATH + "Tab Layout (Right)", false, MENU_ITEM_PRIORITY)]
                public static void CreateTabLayoutRight(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "TabLayoutRight");

                [MenuItem(CATEGORY_MENU_PATH + "Tab Layout (Top Left)", false, MENU_ITEM_PRIORITY)]
                public static void CreateTabLayoutTopLeft(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "TabLayoutTopLeft");

                [MenuItem(CATEGORY_MENU_PATH + "Tab Layout (Top Right)", false, MENU_ITEM_PRIORITY)]
                public static void CreateTabLayoutTopRight(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "TabLayoutTopRight");

                [MenuItem(CATEGORY_MENU_PATH + "Tab Layout (Top)", false, MENU_ITEM_PRIORITY)]
                public static void CreateTabLayoutTop(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "TabLayoutTop");
            }
        }

        public static class Content
        {
            private const string TYPE_NAME = "Content";
            private const string TYPE_MENU_PATH = MENU_PATH + "/" + TYPE_NAME + "/";

            public static class Basic
            {
                private const string CATEGORY_NAME = "Basic";
                private const string CATEGORY_MENU_PATH = TYPE_MENU_PATH + CATEGORY_NAME + "/";

                [MenuItem(CATEGORY_MENU_PATH + "Dark Overlay", false, MENU_ITEM_PRIORITY)]
                public static void CreateDarkOverlay(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "DarkOverlay");

                [MenuItem(CATEGORY_MENU_PATH + "Light Overlay", false, MENU_ITEM_PRIORITY)]
                public static void CreateLightOverlay(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "LightOverlay");

                [MenuItem(CATEGORY_MENU_PATH + "Simple Card", false, MENU_ITEM_PRIORITY)]
                public static void CreateSimpleCard(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "SimpleCard");
            }

            public static class ProductCard
            {
                private const string CATEGORY_NAME = "Product Card";
                private const string CATEGORY_MENU_PATH = TYPE_MENU_PATH + CATEGORY_NAME + "/";

                [MenuItem(CATEGORY_MENU_PATH + "Product Card 01", false, MENU_ITEM_PRIORITY)]
                public static void CreateProductCard01(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "ProductCard01");

                [MenuItem(CATEGORY_MENU_PATH + "Product Card 02", false, MENU_ITEM_PRIORITY)]
                public static void CreateProductCard02(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "ProductCard02");

                [MenuItem(CATEGORY_MENU_PATH + "Product Card 03", false, MENU_ITEM_PRIORITY)]
                public static void CreateProductCard03(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "ProductCard03");

                [MenuItem(CATEGORY_MENU_PATH + "Product Card 04", false, MENU_ITEM_PRIORITY)]
                public static void CreateProductCard04(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "ProductCard04");

                [MenuItem(CATEGORY_MENU_PATH + "Product Card 05", false, MENU_ITEM_PRIORITY)]
                public static void CreateProductCard05(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "ProductCard05");
            }
        }

        public static class Layout
        {
            private const string TYPE_NAME = "Layout";
            private const string TYPE_MENU_PATH = MENU_PATH + "/" + TYPE_NAME + "/";

            public static class Basic
            {
                private const string CATEGORY_NAME = "Basic";
                private const string CATEGORY_MENU_PATH = TYPE_MENU_PATH + CATEGORY_NAME + "/";

                [MenuItem(CATEGORY_MENU_PATH + "Grid", false, MENU_ITEM_PRIORITY)]
                public static void CreateGrid(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "Grid");

                [MenuItem(CATEGORY_MENU_PATH + "Horizontal", false, MENU_ITEM_PRIORITY)]
                public static void CreateHorizontal(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "Horizontal");

                [MenuItem(CATEGORY_MENU_PATH + "Radial", false, MENU_ITEM_PRIORITY)]
                public static void CreateRadial(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "Radial");

                [MenuItem(CATEGORY_MENU_PATH + "Vertical", false, MENU_ITEM_PRIORITY)]
                public static void CreateVertical(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "Vertical");
            }
        }

        public static class Misc
        {
            private const string TYPE_NAME = "Misc";
            private const string TYPE_MENU_PATH = MENU_PATH + "/" + TYPE_NAME + "/";

            public static class Nody
            {
                private const string CATEGORY_NAME = "Nody";
                private const string CATEGORY_MENU_PATH = TYPE_MENU_PATH + CATEGORY_NAME + "/";

                [MenuItem(CATEGORY_MENU_PATH + "Flow Controller", false, MENU_ITEM_PRIORITY)]
                public static void CreateFlowController(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "FlowController");
            }
        }        
    }
}
