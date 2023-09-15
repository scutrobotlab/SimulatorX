// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Doozy.Runtime.UIElements.Extensions
{
    public static class VisualElementExtensions
    {
        #region AddChild, RemoveAllChildren, AddTemplateContainer

        /// <summary> Add a child element to the target element and get back the reference to the target </summary>
        /// <param name="target"> Target VisualElement </param>
        /// <param name="child"> Child VisualElement </param>
        public static T AddChild<T>(this T target, VisualElement child) where T : VisualElement
        {
            target.Add(child);
            return target;
        }

        /// <summary> Remove all children of the target element </summary>
        /// <param name="target"> Target VisualElement </param>
        public static T RemoveAllChildren<T>(this T target) where T : VisualElement
        {
            foreach (VisualElement child in new List<VisualElement>(target.Children()))
                child.RemoveFromHierarchy();
            return target;
        }

        /// <summary> [Editor] Add a template container to the target element and get back the reference to the target </summary>
        /// <param name="target"> Target VisualElement </param>
        /// <param name="layoutPath"> Path to the uxml file </param>
        /// <param name="flexGrow"> Set how much the item will grow relative to the rest of the flexible items inside the same container - style.flexGrow </param>
        public static T AddTemplateContainer<T>(this T target, string layoutPath, float flexGrow = 1) where T : VisualElement
        {
            #if UNITY_EDITOR
            TemplateContainer templateContainer = UnityEditor.AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(layoutPath).CloneTree().SetStyleFlexGrow(flexGrow);
            templateContainer.style.alignSelf = new StyleEnum<Align>(Align.Stretch);
            target.AddChild(templateContainer);
            #else
			Doozy.Runtime.Common.Debugger.LogWarning("This method works only in the Editor");
            #endif
            return target;
        }

        #endregion

        #region AddSpace, AddHorizontalSpace, AddVerticalSpace

        public static T AddSpace<T>(this T target, float width, float height) where T : VisualElement =>
            target.AddChild(new VisualElement().SetName("Space").SetStyleSize(width, height));

        public static T AddHorizontalSpace<T>(this T target, float height) where T : VisualElement =>
            target.AddChild(new VisualElement().SetName("HSpace").SetStyleHeight(height));

        public static T AddVerticalSpace<T>(this T target, float width) where T : VisualElement =>
            target.AddChild(new VisualElement().SetName("VSpace").SetStyleWidth(width));

        #endregion

        #region AddClass, RemoveClass

        /// <summary> [Editor] Add the 'Dark' or 'Light' class name to the target element, depending on the current active Editor Theme, and get back the reference to the target </summary>
        /// <param name="target"> Target VisualElement </param>
        public static T AddCurrentThemeClass<T>(this T target) where T : VisualElement
        {
            #if UNITY_EDITOR
            return target.AddClass(UnityEditor.EditorGUIUtility.isProSkin ? "Dark" : "Light");
            #else
			Doozy.Runtime.Common.Debugger.LogWarning("This method works only in the Editor");
			return target;
            #endif
        }

        /// <summary> Add a class name to the target element and get back the reference to the target </summary>
        /// <param name="target"> Target VisualElement </param>
        /// <param name="className"> Name of the class </param>
        public static T AddClass<T>(this T target, string className) where T : VisualElement
        {
            if (!target.ClassListContains(className))
                target.AddToClassList(className);
            return target;
        }


        /// <summary> Remove a class name from the target element and get back the reference to the target </summary>
        /// <param name="target"> Target VisualElement </param>
        /// <param name="className"> Name of the class </param>
        public static T RemoveClass<T>(this T target, string className) where T : VisualElement
        {
            if (target.ClassListContains(className))
                target.RemoveFromClassList(className);
            return target;
        }

        #endregion

        #region AddStyle, RemoveStyle

        /// <summary> [Editor] Add a style sheet to the target element and get back the reference to the target </summary>
        /// <param name="target"> Target VisualElement </param>
        /// <param name="styleSheet"> StyleSheet reference </param>
        public static T AddStyle<T>(this T target, StyleSheet styleSheet) where T : VisualElement
        {
            if (styleSheet == null) return target;
            target.styleSheets.Add(styleSheet);
            return target;
        }

        /// <summary> [Editor] Add a style sheet to the target element and get back the reference to the target </summary>
        /// <param name="target"> Target VisualElement </param>
        /// <param name="styleSheetPath"> Path to the StyleSheet uss file </param>
        public static T AddStyle<T>(this T target, string styleSheetPath) where T : VisualElement
        {
            #if UNITY_EDITOR
            target.AddStyle(UnityEditor.AssetDatabase.LoadAssetAtPath<StyleSheet>(styleSheetPath));
            #else
			Doozy.Runtime.Common.Debugger.LogWarning("This method works only in the Editor");
            #endif
            return target;
        }

        /// <summary> Remove a style sheet from the target element and get back the reference to the target </summary>
        /// <param name="target"> Target VisualElement </param>
        /// <param name="styleSheet"> StyleSheet reference </param>
        public static T RemoveStyle<T>(this T target, StyleSheet styleSheet) where T : VisualElement
        {
            if (target.styleSheets.Contains(styleSheet))
                target.styleSheets.Remove(styleSheet);
            return target;
        }

        /// <summary> Remove a style sheet from the target element and get back the reference to the target </summary>
        /// <param name="target"> Target VisualElement </param>
        /// <param name="styleSheetName"> StyleSheet name </param>
        public static T RemoveStyle<T>(this T target, string styleSheetName) where T : VisualElement
        {
            StyleSheet styleSheet = null;
            for (var i = 0; i < target.styleSheets.count; i++)
            {
                if (!target.styleSheets[i].name.Equals(styleSheetName)) continue;
                styleSheet = target.styleSheets[i];
                break;
            }

            if (styleSheet != null)
                target.styleSheets.Remove(styleSheet);

            return target;
        }

        #endregion

        #region Name

        /// <summary> Set the name of the target element and get back the reference to the target
        ///	<para/> name
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        /// <param name="value"> Name value </param>
        public static T SetName<T>(this T target, string value) where T : VisualElement
        {
            target.name = value;
            return target;
        }

        /// <summary> Get the name of the target element
        ///	<para/> name
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        public static string GetName<T>(this T target) where T : VisualElement =>
            target.name;

        #endregion

        #region Tooltip

        /// <summary> Set the text to display inside an information box after the user hovers the element for a small amount of time
        /// <para/> tooltip
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        /// <param name="value"> Tooltip value </param>
        public static T SetTooltip<T>(this T target, string value) where T : VisualElement
        {
            target.tooltip = value;
            return target;
        }

        /// <summary> Get the text displayed inside an information box after the user hovers the element for a small amount of time
        /// <para/> tooltip
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        public static string GetTooltip<T>(this T target) where T : VisualElement =>
            target.tooltip;

        #endregion

        #region PickingMode

        /// <summary>
        /// Set if this element can be pick during mouseEvents or IPanel.Pick queries
        /// <para/> pickingMode
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        /// <param name="value"> PickingMode value </param>
        public static T SetPickingMode<T>(this T target, PickingMode value) where T : VisualElement
        {
            target.pickingMode = value;
            return target;
        }

        /// <summary> Get if this element can be pick during mouseEvents or IPanel.Pick queries
        /// <para/> pickingMode
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        public static PickingMode GetPickingMode<T>(this T target) where T : VisualElement =>
            target.pickingMode;

        #endregion

        #region Style - Position

        /// <summary>
        /// Set the element's positioning in its parent container
        /// <para/> style.position
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        /// <param name="value"> Position value </param>
        public static T SetStylePosition<T>(this T target, Position value) where T : VisualElement
        {
            target.style.position = new StyleEnum<Position>(value);
            return target;
        }

        /// <summary>
        /// Get the element's positioning in its parent container
        /// <para/> style.position
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        public static Position GetStylePosition<T>(this T target) where T : VisualElement =>
            target.style.position.value;

        #endregion

        #region Style - Overflow

        /// <summary>
        /// Set how a container behaves if its content overflows its own box
        /// <para/> style.overflow
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        /// <param name="value"> Overflow value </param>
        public static T SetStyleOverflow<T>(this T target, Overflow value) where T : VisualElement
        {
            target.style.overflow = new StyleEnum<Overflow>(value);
            return target;
        }

        /// <summary>
        /// Get how a container behaves if its content overflows its own box
        /// <para/> style.overflow
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        public static Overflow GetStyleOverflow<T>(this T target) where T : VisualElement =>
            target.style.overflow.value;

        #endregion

        #region Style - AlignSelf

        /// <summary>
        /// Similar to align-items, but only for this specific element
        /// <para/> style.alignSelf
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        /// <param name="value"> Align value </param>
        public static T SetStyleAlignSelf<T>(this T target, Align value) where T : VisualElement
        {
            target.style.alignSelf = new StyleEnum<Align>(value);
            return target;
        }

        /// <summary>
        /// Similar to align-items, but only for this specific element
        /// <para/> style.alignSelf
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        public static Align GetStyleAlignSelf<T>(this T target) where T : VisualElement =>
            target.style.alignSelf.value;

        #endregion

        #region Style - AlignItems

        /// <summary>
        /// Set the alignment of children on the cross axis of this container
        /// <para/> style.alignItems
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        /// <param name="value"> Align value </param>
        public static T SetStyleAlignItems<T>(this T target, Align value) where T : VisualElement
        {
            target.style.alignItems = new StyleEnum<Align>(value);
            return target;
        }

        /// <summary>
        /// Get the alignment of children on the cross axis of this container
        /// <para/> style.alignItems
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        public static Align GetStyleAlignItems<T>(this T target) where T : VisualElement =>
            target.style.alignItems.value;

        #endregion

        #region Style - AlignContent

        /// <summary>
        /// Set the alignment of the whole area of children on the cross axis if they span over multiple lines in this container
        /// <para/> style.alignContent
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        /// <param name="value"> Align value </param>
        public static T SetStyleAlignContent<T>(this T target, Align value) where T : VisualElement
        {
            target.style.alignContent = new StyleEnum<Align>(value);
            return target;
        }

        /// <summary>
        /// Get the alignment of the whole area of children on the cross axis if they span over multiple lines in this container
        /// <para/> style.alignContent
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        public static Align GetStyleAlignContent<T>(this T target) where T : VisualElement =>
            target.style.alignContent.value;

        #endregion

        #region Style - JustifyContent

        /// <summary>
        /// Set the justification of children on the main axis of this container
        /// <para/> style.justifyContent
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        /// <param name="value"> Justify value </param>
        public static T SetStyleJustifyContent<T>(this T target, Justify value) where T : VisualElement
        {
            target.style.justifyContent = new StyleEnum<Justify>(value);
            return target;
        }

        /// <summary>
        /// Get the justification of children on the main axis of this container
        /// <para/> style.justifyContent
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        public static Justify GetStyleJustifyContent<T>(this T target) where T : VisualElement =>
            target.style.justifyContent.value;

        #endregion

        #region Style - FlexDirection

        /// <summary> Set the direction of the main axis to layout children in a container
        /// <para/> style.flexDirection
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        /// <param name="value"> FlexDirection value </param>
        public static T SetStyleFlexDirection<T>(this T target, FlexDirection value) where T : VisualElement
        {
            target.style.flexDirection = new StyleEnum<FlexDirection>(value);
            return target;
        }

        /// <summary> Get the direction of the main axis to layout children in a container
        /// <para/> style.flexDirection
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        public static FlexDirection GetStyleFlexDirection<T>(this T target) where T : VisualElement =>
            target.style.flexDirection.value;

        /// <summary> Set the direction of the main axis to layout children in a container
        /// <para/> style.flexDirection
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        /// <param name="value"> StyleKeyword value </param>
        public static T SetStyleFlexDirectionKeyword<T>(this T target, StyleKeyword value) where T : VisualElement
        {
            target.style.flexDirection = new StyleEnum<FlexDirection>(value);
            return target;
        }

        /// <summary> Get the direction of the main axis to layout children in a container
        /// <para/> style.flexDirection
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        public static StyleKeyword GetStyleFlexDirectionKeyword<T>(this T target) where T : VisualElement =>
            target.style.flexDirection.keyword;

        #endregion

        #region Style - FlexGrow

        /// <summary> Set how the target will grow relative to the rest of the flexible items inside the same container
        /// <para/> style.flexGrow
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        /// <param name="value"> 0 = fixed size / 1 = flexible </param>
        public static T SetStyleFlexGrow<T>(this T target, float value) where T : VisualElement
        {
            target.style.flexGrow = value;
            return target;
        }

        /// <summary> Set how the target will grow relative to the rest of the flexible items inside the same container
        /// <para/> style.flexGrow
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        public static float GetStyleFlexGrow<T>(this T target) where T : VisualElement =>
            target.style.flexGrow.value;

        /// <summary> Set how the target will grow relative to the rest of the flexible items inside the same container
        /// <para/> style.flexGrow
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        /// <param name="value"> StyleKeyword value </param>
        public static T SetStyleFlexGrowKeyword<T>(this T target, StyleKeyword value) where T : VisualElement
        {
            target.style.flexGrow = new StyleFloat(value);
            return target;
        }

        /// <summary> Get how the target will grow relative to the rest of the flexible items inside the same container
        /// <para/> style.flexGrow
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        public static StyleKeyword GetStyleFlexGrowKeyword<T>(this T target) where T : VisualElement =>
            target.style.flexGrow.keyword;

        #endregion

        #region Style - FlexShrink

        /// <summary> Set how the target will shrink relative to the rest of the flexible items inside the same container
        /// <para/> style.flexShrink
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        /// <param name="value"> 0 = fixed size / 1 = flexible </param>
        public static T SetStyleFlexShrink<T>(this T target, float value) where T : VisualElement
        {
            target.style.flexShrink = value;
            return target;
        }

        /// <summary> Get how the target will shrink relative to the rest of the flexible items inside the same container
        /// <para/> style.flexShrink
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        public static float GetStyleFlexShrink<T>(this T target) where T : VisualElement =>
            target.style.flexShrink.value;

        /// <summary> Set how the item will shrink relative to the rest of the flexible items inside the same container
        /// <para/> style.flexShrink
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        /// <param name="value"> StyleKeyword value </param>
        public static T SetStyleFlexShrinkKeyword<T>(this T target, StyleKeyword value) where T : VisualElement
        {
            target.style.flexShrink = new StyleFloat(value);
            return target;
        }

        /// <summary> Get how the item will shrink relative to the rest of the flexible items inside the same container
        /// <para/> style.flexShrink
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        public static StyleKeyword GetStyleFlexShrinkKeyword<T>(this T target) where T : VisualElement =>
            target.style.flexShrink.keyword;

        #endregion

        #region Style - FlexWrap

        /// <summary>
        /// Set the placement of children over multiple lines if not enough space is available in this container
        /// <para/> style.flexWrap
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        /// <param name="value"> Wrap value </param>
        public static T SetStyleFlexWrap<T>(this T target, Wrap value) where T : VisualElement
        {
            target.style.flexWrap = value;
            return target;
        }

        /// <summary>
        /// Get the placement of children over multiple lines if not enough space is available in this container
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        public static Wrap GetStyleFlexWrap<T>(this T target) where T : VisualElement =>
            target.style.flexWrap.value;

        #endregion

        #region Style - FlexBasis

        /// <summary>
        /// Set the initial main size of a flex item, on the main flex axis.
        /// The final layout must be smaller or larger,
        /// according to the flex shrinking and growing determined by the flex property.
        /// <para/> style.flexBasis
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        /// <param name="value"> 0 = fixed size / 1 = flexible </param>
        public static T SetStyleFlexBasis<T>(this T target, float value) where T : VisualElement
        {
            target.style.flexBasis = value;
            return target;
        }

        /// <summary>
        /// Get the initial main size of a flex item, on the main flex axis.
        /// The final layout must be smaller or larger,
        /// according to the flex shrinking and growing determined by the flex property.
        /// <para/> style.flexBasis
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        public static float GetStyleFlexBasis<T>(this T target) where T : VisualElement =>
            target.style.flexBasis.value.value;

        /// <summary>
        /// Set the initial main size of a flex item, on the main flex axis.
        /// The final layout must be smaller or larger,
        /// according to the flex shrinking and growing determined by the flex property.
        /// <para/> style.flexBasis
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        /// <param name="value"> StyleLength value </param>
        public static T SetStyleFlexBasisStyleLength<T>(this T target, StyleLength value) where T : VisualElement
        {
            target.style.flexBasis = value;
            return target;
        }

        /// <summary>
        /// Get the initial main size of a flex item, on the main flex axis.
        /// The final layout must be smaller or larger,
        /// according to the flex shrinking and growing determined by the flex property.
        /// <para/> style.flexBasis
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        public static StyleKeyword GetStyleFlexBasisKeyword<T>(this T target) where T : VisualElement =>
            target.style.flexBasis.keyword;

        #endregion

        #region Style - BorderColor

        /// <summary> Set the Colors for all the element's borders (Top, Left, Right, Bottom)
        /// <para/> style.borderLeftColor
        /// <para/> style.borderTopColor
        /// <para/> style.borderRightColor
        /// <para/> style.borderBottomColor
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        /// <param name="left"> Left border color </param>
        /// <param name="top"> Top border color </param>
        /// <param name="right"> Right border color </param>
        /// <param name="bottom"> Bottom border color </param>
        public static T SetStyleBorderColor<T>(this T target, Color left, Color top, Color right, Color bottom) where T : VisualElement
        {
            target.style.borderLeftColor = left;
            target.style.borderTopColor = top;
            target.style.borderRightColor = right;
            target.style.borderBottomColor = bottom;
            return target;
        }

        /// <summary> Set the same Color for all the element's borders (Top, Left, Right, Bottom)
        /// <para/> style.borderLeftColor
        /// <para/> style.borderTopColor
        /// <para/> style.borderRightColor
        /// <para/> style.borderBottomColor
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        /// <param name="value"> Color value </param>
        public static T SetStyleBorderColor<T>(this T target, Color value) where T : VisualElement =>
            target.SetStyleBorderColor(value, value, value, value);

        #region SetStyleBorderLeftColor, GetStyleBorderLeftColor

        /// <summary> Set the Color of the element's Left border
        /// <para/> style.borderLeftColor
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        /// <param name="value"> Color value </param>
        public static T SetStyleBorderLeftColor<T>(this T target, Color value) where T : VisualElement
        {
            target.style.borderLeftColor = value;
            return target;
        }

        /// <summary> Get the Color of the element's left border
        /// <para/> style.borderLeftColor
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        public static Color GetStyleBorderLeftColor<T>(this T target) where T : VisualElement =>
            target.resolvedStyle.borderLeftColor;

        #endregion

        #region SetStyleBorderTopColor, GetStyleBorderTopColor

        /// <summary> Set the Color of the element's Top border
        /// <para/> style.borderTopColor
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        /// <param name="value"> Color value </param>
        public static T SetStyleBorderTopColor<T>(this T target, Color value) where T : VisualElement
        {
            target.style.borderTopColor = value;
            return target;
        }

        /// <summary> Get the Color of the element's left border
        /// <para/> style.borderTopColor
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        public static Color GetStyleBorderTopColor<T>(this T target) where T : VisualElement =>
            target.resolvedStyle.borderTopColor;

        #endregion

        #region SetStyleBorderRightColor, GetStyleBorderRightColor

        /// <summary> Set the Color of the element's Right border
        /// <para/> style.borderRightColor
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        /// <param name="value"> Color value </param>
        public static T SetStyleBorderRightColor<T>(this T target, Color value) where T : VisualElement
        {
            target.style.borderRightColor = value;
            return target;
        }

        /// <summary> Get the Color of the element's left border
        /// <para/> style.borderRightColor
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        public static Color GetStyleBorderRightColor<T>(this T target) where T : VisualElement =>
            target.resolvedStyle.borderRightColor;

        #endregion

        #region SetStyleBorderBottomColor, GetStyleBorderBottomColor

        /// <summary> Set the Color of the element's Bottom border
        /// <para/> style.borderBottomColor
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        /// <param name="value"> Color value </param>
        public static T SetStyleBorderBottomColor<T>(this T target, Color value) where T : VisualElement
        {
            target.style.borderBottomColor = value;
            return target;
        }

        /// <summary> Get the Color of the element's left border
        /// <para/> style.borderBottomColor
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        public static Color GetStyleBorderBottomColor<T>(this T target) where T : VisualElement =>
            target.resolvedStyle.borderBottomColor;

        #endregion

        #endregion

        #region Style - BorderWidth

        /// <summary> Set the border Width for all the element's borders (Top, Left, Right, Bottom)
        /// <para/> style.borderLeftWidth
        /// <para/> style.borderTopWidth
        /// <para/> style.borderRightWidth
        /// <para/> style.borderBottomWidth
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        /// <param name="left"> Left border width </param>
        /// <param name="top"> Top border width </param>
        /// <param name="right"> Right border with </param>
        /// <param name="bottom"> Bottom border width </param>
        public static T SetStyleBorderWidth<T>(this T target, float left, float top, float right, float bottom) where T : VisualElement
        {
            target.style.borderLeftWidth = left;
            target.style.borderTopWidth = top;
            target.style.borderRightWidth = right;
            target.style.borderBottomWidth = bottom;
            return target;
        }

        /// <summary> Set the same border Width for all the element's borders (Top, Left, Right, Bottom)
        /// <para/> style.borderLeftWidth
        /// <para/> style.borderTopWidth
        /// <para/> style.borderRightWidth
        /// <para/> style.borderBottomWidth
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        /// <param name="value"> Width value </param>
        public static T SetStyleBorderWidth<T>(this T target, float value) where T : VisualElement =>
            target.SetStyleBorderWidth(value, value, value, value);

        /// <summary> Set the border Width for all the element's borders (Top, Left, Right, Bottom)
        /// <para/> style.borderLeftWidth
        /// <para/> style.borderTopWidth
        /// <para/> style.borderRightWidth
        /// <para/> style.borderBottomWidth
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        /// <param name="edge"> Edge values for all the border's widths </param>
        public static T SetStyleBorderWidth<T>(this T target, EdgeValues edge) where T : VisualElement =>
            target.SetStyleBorderWidth(edge.Left, edge.Top, edge.Right, edge.Bottom);


        #region SetStyleBorderLeftWidth, GetStyleBorderLeftWidth

        /// <summary> Set the Width value of the element's Left border
        /// <para/> style.borderLeftWidth
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        /// <param name="value"> Width value </param>
        public static T SetStyleBorderLeftWidth<T>(this T target, float value) where T : VisualElement
        {
            target.style.borderLeftWidth = value;
            return target;
        }

        /// <summary> Get the Width value of the element's left border
        /// <para/> style.borderLeftWidth
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        public static float GetStyleBorderLeftWidth<T>(this T target) where T : VisualElement =>
            target.style.borderLeftWidth.value;

        #endregion

        #region SetStyleBorderTopWidth, GetStyleBorderTopWidth

        /// <summary> Set the Width value of the element's Top border
        /// <para/> style.borderTopWidth
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        /// <param name="value"> Width value </param>
        public static T SetStyleBorderTopWidth<T>(this T target, float value) where T : VisualElement
        {
            target.style.borderTopWidth = value;
            return target;
        }

        /// <summary> Get the Width value of the element's left border
        /// <para/> style.borderTopWidth
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        public static float GetStyleBorderTopWidth<T>(this T target) where T : VisualElement =>
            target.style.borderTopWidth.value;

        #endregion

        #region SetStyleBorderRightWidth, GetStyleBorderRightWidth

        /// <summary> Set the Width value of the element's Right border
        /// <para/> style.borderRightWidth
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        /// <param name="value"> Width value </param>
        public static T SetStyleBorderRightWidth<T>(this T target, float value) where T : VisualElement
        {
            target.style.borderRightWidth = value;
            return target;
        }

        /// <summary> Get the Width value of the element's left border
        /// <para/> style.borderRightWidth
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        public static float GetStyleBorderRightWidth<T>(this T target) where T : VisualElement =>
            target.style.borderRightWidth.value;

        #endregion

        #region SetStyleBorderBottomWidth, GetStyleBorderBottomWidth

        /// <summary> Set the Width value of the element's Bottom border
        /// <para/> style.borderBottomWidth
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        /// <param name="value"> Width value </param>
        public static T SetStyleBorderBottomWidth<T>(this T target, float value) where T : VisualElement
        {
            target.style.borderBottomWidth = value;
            return target;
        }

        /// <summary> Get the Width value of the element's left border
        /// <para/> style.borderBottomWidth
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        public static float GetStyleBorderBottomWidth<T>(this T target) where T : VisualElement =>
            target.style.borderBottomWidth.value;

        #endregion

        #endregion

        #region Style - BorderRadius

        /// <summary> Set the border (corner) Radius for all the element's corners (TopLeft, TopRight, BottomRight, BottomLeft)
        /// <para/> style.borderTopLeftRadius
        /// <para/> style.borderTopRightRadius
        /// <para/> style.borderBottomRightRadius
        /// <para/> style.borderBottomLeftRadius
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        /// <param name="topLeft"> TopLeft border (corner) radius </param>
        /// <param name="topRight"> TopRight border (corner) radius </param>
        /// <param name="bottomRight"> BottomRight border (corner) radius </param>
        /// <param name="bottomLeft"> BottomLeft border (corner) radius </param>
        public static T SetStyleBorderRadius<T>(this T target, float topLeft, float topRight, float bottomRight, float bottomLeft) where T : VisualElement
        {
            target.style.borderTopLeftRadius = topLeft;
            target.style.borderTopRightRadius = topRight;
            target.style.borderBottomRightRadius = bottomRight;
            target.style.borderBottomLeftRadius = bottomLeft;
            return target;
        }

        /// <summary> Set the same border (corner) Radius for all the element's corners (TopLeft, TopRight, BottomRight, BottomLeft)
        /// <para/> style.borderTopLeftRadius
        /// <para/> style.borderTopRightRadius
        /// <para/> style.borderBottomRightRadius
        /// <para/> style.borderBottomLeftRadius
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        /// <param name="value"> Radius value </param>
        public static T SetStyleBorderRadius<T>(this T target, float value) where T : VisualElement =>
            target.SetStyleBorderRadius(value, value, value, value);

        /// <summary> Set the border (corner) Radius for all the element's corners (TopLeft, TopRight, BottomRight, BottomLeft)
        /// <para/> style.borderTopLeftRadius
        /// <para/> style.borderTopRightRadius
        /// <para/> style.borderBottomRightRadius
        /// <para/> style.borderBottomLeftRadius
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        /// <param name="edge"> Edge values for all all the element's corners
        /// <para/> edge.left = TopLeft border (corner) radius
        /// <para/> edge.top = TopRight border (corner) radius
        /// <para/> edge.top = BottomRight border (corner) radius
        /// <para/> edge.bottom = BottomLeft border (corner) radius
        /// </param>
        public static T SetStyleBorderRadius<T>(this T target, EdgeValues edge) where T : VisualElement =>
            target.SetStyleBorderRadius(edge.Left, edge.Top, edge.Right, edge.Bottom);

        #region SetStyleBorderTopLeftRadius, GetStyleBorderTopLeftRadius

        /// <summary> Set the border (corner) Radius value of the element's TopLeft corner
        /// <para/> style.borderTopLeftRadius
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        /// <param name="value"> TopLeft border (corner) radius value </param>
        public static T SetStyleBorderTopLeftRadius<T>(this T target, float value) where T : VisualElement
        {
            target.style.borderTopLeftRadius = value;
            return target;
        }

        /// <summary> Get the border (corner) Radius value of the element's TopLeft corner
        /// <para/> style.borderTopLeftRadius
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        public static float GetStyleBorderTopLeftRadius<T>(this T target) where T : VisualElement =>
            target.style.borderTopLeftRadius.value.value;

        #endregion

        #region SetStyleBorderTopRightRadius, GetStyleBorderTopRightRadius

        /// <summary> Set the border (corner) Radius value of the element's TopRight corner
        /// <para/> style.borderTopRightRadius
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        /// <param name="value"> TopRight border (corner) radius value </param>
        public static T SetStyleBorderTopRightRadius<T>(this T target, float value) where T : VisualElement
        {
            target.style.borderTopRightRadius = value;
            return target;
        }

        /// <summary> Get the border (corner) Radius value of the element's TopRight corner
        /// <para/> style.borderTopRightRadius
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        public static float GetStyleBorderTopRightRadius<T>(this T target) where T : VisualElement =>
            target.style.borderTopRightRadius.value.value;

        #endregion

        #region SetStyleBorderBottomRightRadius, GetStyleBorderBottomRightRadius

        /// <summary> Set the border (corner) Radius value of the element's BottomRight corner
        /// <para/> style.borderBottomRightRadius
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        /// <param name="value"> BottomRight border (corner) radius value </param>
        public static T SetStyleBorderBottomRightRadius<T>(this T target, float value) where T : VisualElement
        {
            target.style.borderBottomRightRadius = value;
            return target;
        }

        /// <summary> Get the border (corner) Radius value of the element's BottomRight corner
        /// <para/> style.borderBottomRightRadius
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        public static float GetStyleBorderBottomRightRadius<T>(this T target) where T : VisualElement =>
            target.style.borderBottomRightRadius.value.value;

        #endregion

        #region SetStyleBorderBottomLeftRadius, GetStyleBorderBottomLeftRadius

        /// <summary> Set the border (corner) Radius value of the element's BottomLeft corner
        /// <para/> style.borderBottomLeftRadius
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        /// <param name="value"> BottomLeft border (corner) radius value </param>
        public static T SetStyleBorderBottomLeftRadius<T>(this T target, float value) where T : VisualElement
        {
            target.style.borderBottomLeftRadius = value;
            return target;
        }

        /// <summary> Get the border (corner) Radius value of the element's BottomLeft corner
        /// <para/> style.borderBottomLeftRadius
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        public static float GetStyleBorderBottomLeftRadius<T>(this T target) where T : VisualElement =>
            target.style.borderBottomLeftRadius.value.value;

        #endregion

        #endregion

        #region Style - BackgroundColor

        /// <summary> Set the background color to paint in the element's box
        /// <para/> style.backgroundColor
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        /// <param name="color"> Color value </param>
        public static T SetStyleBackgroundColor<T>(this T target, Color color) where T : VisualElement
        {
            target.style.backgroundColor = color;
            return target;
        }

        /// <summary> Get the background color of element's box
        /// <para/> style.backgroundColor
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        public static Color GetStyleBackgroundColor<T>(this T target) where T : VisualElement =>
            target.resolvedStyle.backgroundColor;

        #endregion

        #region Style - Opacity

        /// <summary> Set the transparency of an element (and its children)
        /// <para/> style.opacity
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        /// <param name="value"> Opacity value (0-1) </param>
        public static T SetStyleOpacity<T>(this T target, float value) where T : VisualElement
        {
            target.style.opacity = value;
            return target;
        }

        /// <summary> Get the transparency of an element (and its children)
        /// <para/> style.opacity
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        public static float GetStyleOpacity<T>(this T target) where T : VisualElement =>
            target.style.opacity.value;

        #endregion

        #region Style - Height

        /// <summary> Set the fixed height of an element for the layout
        /// <para/> style.height
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        /// <param name="height"> Height float value </param>
        public static T SetStyleHeight<T>(this T target, float height) where T : VisualElement
        {
            target.style.height = height;
            return target;
        }

        /// <summary> Set the fixed height of an element for the layout
        /// <para/> style.height
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        /// <param name="styleKeyword"> StyleKeyword </param>
        public static T SetStyleHeight<T>(this T target, StyleKeyword styleKeyword) where T : VisualElement
        {
            target.style.height = new StyleLength(styleKeyword);
            return target;
        }

        /// <summary> Set the fixed height of an element to auto
        /// <para/> style.height
        /// </summary>
        public static T ResetStyleHeight<T>(this T target) where T : VisualElement =>
            target.SetStyleHeight(StyleKeyword.Auto);

        /// <summary> Get the fixed height of an element for the layout
        /// <para/> style.height
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        public static float GetStyleHeight<T>(this T target) where T : VisualElement =>
            target.style.height.value.value;

        /// <summary> Set the fixed MinHeight, Height and MaxHright of an element for the layout
        /// <para/> style.minHeight
        /// <para/> style.height
        /// <para/> style.maxHeight
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        /// <param name="minHeight"> MinHeight float value </param>
        /// <param name="height"> Height float value </param>
        /// <param name="maxHeight"> MaxHeight float value </param>
        public static T SetStyleHeight<T>(this T target, float minHeight, float height, float maxHeight) where T : VisualElement
        {
            return target.SetStyleMinHeight(minHeight)
                .SetStyleHeight(height)
                .SetStyleMaxHeight(maxHeight);
        }

        #endregion

        #region Style - MinHeight

        /// <summary> Set the minimum height for an element, when it is flexible or measures its own size
        /// <para/> style.minHeight
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        /// <param name="height"> MinHeight float value </param>
        public static T SetStyleMinHeight<T>(this T target, float height) where T : VisualElement
        {
            target.style.minHeight = height;
            return target;
        }

        /// <summary> Set the minimum height of an element, when it is flexible or measures its own size
        /// <para/> style.minHeight
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        /// <param name="styleKeyword"> StyleKeyword </param>
        public static T SetStyleMinHeight<T>(this T target, StyleKeyword styleKeyword) where T : VisualElement
        {
            target.style.minHeight = new StyleLength(styleKeyword);
            return target;
        }

        /// <summary> Set the minimum height of an element to auto
        /// <para/> style.minHeight
        /// </summary>
        public static T ResetStyleMinHeight<T>(this T target) where T : VisualElement =>
            target.SetStyleMinHeight(StyleKeyword.Auto);

        /// <summary> Get the minimum height for an element, when it is flexible or measures its own size
        /// <para/> style.minHeight
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        public static float GetStyleMinHeight<T>(this T target) where T : VisualElement =>
            target.style.minHeight.value.value;

        #endregion

        #region Style - MaxHeight

        /// <summary> Set the maximum height for an element, when it is flexible or measures its own size
        /// <para/> style.maxHeight
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        /// <param name="height"> MaxHeight float value </param>
        public static T SetStyleMaxHeight<T>(this T target, float height) where T : VisualElement
        {
            target.style.maxHeight = height;
            return target;
        }

        /// <summary> Set the maximum height of an element, when it is flexible or measures its own size
        /// <para/> style.maxHeight
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        /// <param name="styleKeyword"> StyleKeyword </param>
        public static T SetStyleMaxHeight<T>(this T target, StyleKeyword styleKeyword) where T : VisualElement
        {
            target.style.maxHeight = new StyleLength(styleKeyword);
            return target;
        }

        /// <summary> Set the maximum height of an element to auto
        /// <para/> style.maxHeight
        /// </summary>
        public static T ResetStyleMaxHeight<T>(this T target) where T : VisualElement =>
            target.SetStyleMaxHeight(StyleKeyword.Auto);

        /// <summary> Get the maximum height for an element, when it is flexible or measures its own size
        /// <para/> style.maxHeight
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        public static float GetStyleMaxHeight<T>(this T target) where T : VisualElement =>
            target.style.maxHeight.value.value;

        #endregion

        #region Style - Width

        /// <summary> Set the fixed width of an element for the layout
        /// <para/> style.width
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        /// <param name="width"> Width float value</param>
        public static T SetStyleWidth<T>(this T target, float width) where T : VisualElement
        {
            target.style.width = width;
            return target;
        }

        /// <summary> Set the fixed width of an element for the layout
        /// <para/> style.width
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        /// <param name="styleKeyword"> StyleKeyword </param>
        public static T SetStyleWidth<T>(this T target, StyleKeyword styleKeyword) where T : VisualElement
        {
            target.style.width = new StyleLength(styleKeyword);
            return target;
        }

        /// <summary> Set the fixed width of an element to auto
        /// <para/> style.width
        /// </summary>
        public static T ResetStyleWidth<T>(this T target) where T : VisualElement =>
            target.SetStyleWidth(StyleKeyword.Auto);

        /// <summary> Get the fixed width of an element for the layout
        /// <para/> style.width
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        public static float GetStyleWidth<T>(this T target) where T : VisualElement =>
            target.style.width.value.value;


        /// <summary> Set the fixed MinWidth, Width and MaxWidth of an element for the layout
        /// <para/> style.minWidth
        /// <para/> style.width
        /// <para/> style.maxWidth
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        /// <param name="minWidth"></param>
        /// <param name="width"></param>
        /// <param name="maxWidth"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T SetStyleWidth<T>(this T target, float minWidth, float width, float maxWidth) where T : VisualElement
        {
            return target.SetStyleMinWidth(minWidth)
                .SetStyleWidth(width)
                .SetStyleMaxWidth(maxWidth);
        }

        #endregion

        #region Style - MinWidth

        /// <summary> Set the minimum width for an element, when it is flexible or measures its own size
        /// <para/> style.minWidth
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        /// <param name="width"> MinWidth float value</param>
        public static T SetStyleMinWidth<T>(this T target, float width) where T : VisualElement
        {
            target.style.minWidth = width;
            return target;
        }

        /// <summary> Set the minimum width of an element, when it is flexible or measures its own size
        /// <para/> style.minWidth
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        /// <param name="styleKeyword"> StyleKeyword </param>
        public static T SetStyleMinWidth<T>(this T target, StyleKeyword styleKeyword) where T : VisualElement
        {
            target.style.minWidth = new StyleLength(styleKeyword);
            return target;
        }

        /// <summary> Set the minimum width of an element to auto
        /// <para/> style.minWidth
        /// </summary>
        public static T ResetStyleMinWidth<T>(this T target) where T : VisualElement =>
            target.SetStyleMinWidth(StyleKeyword.Auto);

        /// <summary> Get the minimum width for an element, when it is flexible or measures its own size
        /// <para/> style.minWidth
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        public static float GetStyleMinWidth<T>(this T target) where T : VisualElement =>
            target.style.minWidth.value.value;

        #endregion

        #region Style - MaxWidth

        /// <summary> Set the maximum width for an element, when it is flexible or measures its own size
        /// <para/> style.maxWidth
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        /// <param name="width"> MaxWidth float value</param>
        public static T SetStyleMaxWidth<T>(this T target, float width) where T : VisualElement
        {
            target.style.maxWidth = width;
            return target;
        }

        /// <summary> Set the maximum width of an element, when it is flexible or measures its own size
        /// <para/> style.maxWidth
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        /// <param name="styleKeyword"> StyleKeyword </param>
        public static T SetStyleMaxWidth<T>(this T target, StyleKeyword styleKeyword) where T : VisualElement
        {
            target.style.maxWidth = new StyleLength(styleKeyword);
            return target;
        }

        /// <summary> Set the maximum width of an element to auto
        /// <para/> style.maxWidth
        /// </summary>
        public static T ResetStyleMaxWidth<T>(this T target) where T : VisualElement =>
            target.SetStyleMaxWidth(StyleKeyword.Auto);

        /// <summary> Get the maximum width for an element, when it is flexible or measures its own size
        /// <para/> style.maxWidth
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        public static float GetStyleMaxWidth<T>(this T target) where T : VisualElement =>
            target.style.maxWidth.value.value;

        #endregion

        #region Style - Size (Width and Height) - does not exist natively in Unity

        /// <summary>
        /// Set the fixed values for the width and height of an element for the layout
        /// <para/> style.width
        /// <para/> style.height
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        /// <param name="width"> Width float value </param>
        /// <param name="height"> Height Float value </param>
        public static T SetStyleSize<T>(this T target, float width, float height) where T : VisualElement =>
            target.SetStyleWidth(width).SetStyleHeight(height);

        /// <summary>
        /// Set the same fixed value for the width and height of an element for the layout
        /// <para/> style.width
        /// <para/> style.height
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        /// <param name="value"> Width and Height float value </param>
        public static T SetStyleSize<T>(this T target, float value) where T : VisualElement =>
            target.SetStyleSize(value, value);


        /// <summary>
        /// Set the same fixed value for the width and height of an element for the layout
        /// <para/> style.width
        /// <para/> style.height
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        /// <param name="styleKeyword"> StyleKeyword </param>
        public static T SetStyleSize<T>(this T target, StyleKeyword styleKeyword) where T : VisualElement
        {
            target.SetStyleWidth(styleKeyword);
            target.SetStyleHeight(styleKeyword);
            return target;
        }

        /// <summary>
        /// Set the width and height of an element to auto
        /// <para/> style.width
        /// <para/> style.height
        /// </summary>
        public static T ResetStyleSize<T>(this T target) where T : VisualElement =>
            target.SetStyleSize(StyleKeyword.Auto);

        #endregion

        #region Style - MinSize (MinWidth and MinHeight) - does not exist natively in Unity

        /// <summary>
        /// Set the values for the minimum width and minimum height for an element, when it is flexible or measures its own size
        /// <para/> style.minWidth
        /// <para/> style.minHeight
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        /// <param name="width"> MinWidth float value </param>
        /// <param name="height"> MinHeight Float value </param>
        public static T SetStyleMinSize<T>(this T target, float width, float height) where T : VisualElement =>
            target.SetStyleMinWidth(width).SetStyleMinHeight(height);

        /// <summary>
        /// Set the same values for minimum width and minimum height for an element, when it is flexible or measures its own size
        /// <para/> style.minWidth
        /// <para/> style.minHeight
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        /// <param name="value"> MinWidth and MinHeight float value </param>
        public static T SetStyleMinSize<T>(this T target, float value) where T : VisualElement =>
            target.SetStyleMinWidth(value).SetStyleMinHeight(value);

        /// <summary>
        /// Set the same values for minimum width and minimum height for an element, when it is flexible or measures its own size
        /// <para/> style.minWidth
        /// <para/> style.minHeight
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        /// <param name="styleKeyword"> StyleKeyword </param>
        public static T SetStyleMinSize<T>(this T target, StyleKeyword styleKeyword) where T : VisualElement
        {
            target.SetStyleMinWidth(styleKeyword);
            target.SetStyleMinHeight(styleKeyword);
            return target;
        }

        /// <summary>
        /// Set the minimum width and minimum height of an element to auto
        /// <para/> style.minWidth
        /// <para/> style.minHeight
        /// </summary>
        public static T ResetStyleMinSize<T>(this T target) where T : VisualElement =>
            target.SetStyleMinSize(StyleKeyword.Auto);

        #endregion

        #region Style - MaxSize (MaxWidth and MaxHeight) - does not exist natively in Unity

        /// <summary>
        /// Set the values for the maximum width and maximum height for an element, when it is flexible or measures its own size
        /// <para/> style.maxWidth
        /// <para/> style.maxHeight
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        /// <param name="width"> MaxWidth float value </param>
        /// <param name="height"> MaxHeight Float value </param>
        public static T StyleMaxSize<T>(this T target, float width, float height) where T : VisualElement =>
            target.SetStyleMaxWidth(width).SetStyleMaxHeight(height);

        /// <summary>
        /// Set the same values for the maximum width and maximum height for an element, when it is flexible or measures its own size
        /// <para/> style.maxWidth
        /// <para/> style.maxHeight
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        /// <param name="value"> MaxWidth and MaxHeight float value </param>
        public static T StyleMaxSize<T>(this T target, float value) where T : VisualElement =>
            target.SetStyleMaxWidth(value).SetStyleMaxHeight(value);

        /// <summary>
        /// Set the same values for maximum width and maximum height for an element, when it is flexible or measures its own size
        /// <para/> style.maxWidth
        /// <para/> style.maxHeight
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        /// <param name="styleKeyword"> StyleKeyword </param>
        public static T SetStyleMaxSize<T>(this T target, StyleKeyword styleKeyword) where T : VisualElement
        {
            target.SetStyleMaxWidth(styleKeyword);
            target.SetStyleMaxHeight(styleKeyword);
            return target;
        }

        /// <summary>
        /// Set the maximum width and maximum height of an element to auto
        /// <para/> style.maxWidth
        /// <para/> style.maxHeight
        /// </summary>
        public static T ResetStyleMaxSize<T>(this T target) where T : VisualElement =>
            target.SetStyleMaxSize(StyleKeyword.Auto);

        #endregion

        #region Style - BackgroundImage

        #region Texture2D

        /// <summary> Set the background image to paint in the element's box </summary>
        /// <param name="target"> Target VisualElement </param>
        /// <param name="value"> Texture2D reference </param>
        public static T SetStyleBackgroundImage<T>(this T target, Texture2D value) where T : VisualElement
        {
            target.style.backgroundImage = new StyleBackground(value);
            return target;
        }

        /// <summary> Get the background image Texture2D used to paint in the element's box </summary>
        /// <param name="target"> Target VisualElement </param>
        public static Texture2D GetStyleBackgroundImageTexture2D<T>(this T target) where T : VisualElement =>
            target.style.backgroundImage.value.texture;

        #endregion

        #region VectorImage

        /// <summary> Set the background image to paint in the element's box </summary>
        /// <param name="target"> Target VisualElement </param>
        /// <param name="value"> An asset that represents a vector image </param>
        public static T SetStyleBackgroundImage<T>(this T target, VectorImage value) where T : VisualElement
        {
            target.style.backgroundImage = new StyleBackground(value);
            return target;
        }

        /// <summary> Get the background image VectorImage used to paint in the element's box </summary>
        /// <param name="target"> Target VisualElement </param>
        public static VectorImage GetStyleBackgroundImageVectorImage<T>(this T target) where T : VisualElement =>
            target.style.backgroundImage.value.vectorImage;

        #endregion

        #region VisualElement Background

        /// <summary> Set the background image to paint in the element's box </summary>
        /// <param name="target"> Target VisualElement </param>
        /// <param name="value"> Described a VisualElement background </param>
        public static T SetStyleBackgroundImage<T>(this T target, Background value) where T : VisualElement
        {
            target.style.backgroundImage = new StyleBackground(value);
            return target;
        }

        /// <summary> Get the background image Background value used to paint in the element's box </summary>
        /// <param name="target"> Target VisualElement </param>
        public static Background GetStyleBackgroundImageBackground<T>(this T target) where T : VisualElement =>
            target.style.backgroundImage.value;

        #endregion

        #region VisualElement StyleKeyword

        /// <summary> Set the background image to paint in the element's box </summary>
        /// <param name="target"> Target VisualElement </param>
        /// <param name="value"> Keyword used on style value types </param>
        public static T SetStyleBackgroundImage<T>(this T target, StyleKeyword value) where T : VisualElement
        {
            target.style.backgroundImage = new StyleBackground(value);
            return target;
        }

        /// <summary> Get the background image StyleKeyword value used to paint in the element's box </summary>
        /// <param name="target"> Target VisualElement </param>
        public static StyleKeyword GetStyleBackgroundImageStyleKeyword<T>(this T target) where T : VisualElement =>
            target.style.backgroundImage.keyword;

        #endregion

        #region [Editor] Path methods

        /// <summary> [Editor] Set the background image to paint in the element's box </summary>
        /// <param name="target"> Target VisualElement </param>
        /// <param name="texturePath"> Path to the texture </param>
        public static T SetStyleBackgroundImage<T>(this T target, string texturePath) where T : VisualElement
        {
            #if UNITY_EDITOR
            target.SetStyleBackgroundImage(UnityEditor.AssetDatabase.LoadAssetAtPath<Texture2D>(texturePath));
            #else
			Doozy.Runtime.Common.Debugger.LogWarning("This method works only in the Editor");
            #endif
            return target;
        }

        /// <summary> [Editor] Get the path to the background image Texture2D used to paint in the element's box </summary>
        /// <param name="target"> Target VisualElement </param>
        public static string GetStyleBackgroundImagePath<T>(this T target) where T : VisualElement
        {
            #if UNITY_EDITOR
            return UnityEditor.AssetDatabase.GetAssetPath(target.style.backgroundImage.value.texture);
            #else
			return "This method works only in the Editor";
            #endif
        }

        #endregion

        #endregion

        #region Style - UnityBackgroundImageTintColor

        /// <summary> Set the tinting color for the element's backgroundImage </summary>
        /// <param name="target"> Target VisualElement </param>
        /// <param name="value"> Color value </param>
        public static T SetStyleBackgroundImageTintColor<T>(this T target, Color value) where T : VisualElement
        {
            target.style.unityBackgroundImageTintColor = new StyleColor(value);
            return target;
        }

        /// <summary> Get the tinting color for the element's backgroundImage </summary>
        /// <param name="target"> Target VisualElement </param>
        public static Color GetStyleBackgroundImageTintColor<T>(this T target) where T : VisualElement =>
            target.resolvedStyle.unityBackgroundImageTintColor;

        /// <summary> Set the tinting color for the element's backgroundImage </summary>
        /// <param name="target"> Target VisualElement </param>
        /// <param name="value"> Keyword used on style value types </param>
        public static T SetStyleBackgroundImageTintColor<T>(this T target, StyleKeyword value) where T : VisualElement
        {
            target.style.unityBackgroundImageTintColor = new StyleColor(value);
            return target;
        }

        /// <summary> Get the StyleKeyword value used for the element's backgroundImage </summary>
        /// <param name="target"> Target VisualElement </param>
        public static StyleKeyword GetStyleBackgroundImageTintColorStyleKeyword<T>(this T target) where T : VisualElement =>
            target.style.unityBackgroundImageTintColor.keyword;

        #endregion

        #region Style - UnitySlice (Left, Top, Right, Bottom) - 9-Slice

        /// <summary> Set the size of all the 9-slice's edges when painting an element's background image (Left, Top, Right, Bottom)
        /// <para/> style.unitySliceLeft
        /// <para/> style.unitySliceTop
        /// <para/> style.unitySliceRight
        /// <para/> style.unitySliceBottom
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        /// <param name="left"> Left edge 9-slice </param>
        /// <param name="top">  Top edge 9-slice </param>
        /// <param name="right">  Right edge 9-slice </param>
        /// <param name="bottom">  Bottom edge 9-slice </param>
        public static T SetStyleUnitySlice<T>(this T target, int left, int top, int right, int bottom) where T : VisualElement
        {
            target.style.unitySliceLeft = left;
            target.style.unitySliceTop = top;
            target.style.unitySliceRight = right;
            target.style.unitySliceBottom = bottom;
            return target;
        }

        /// <summary> Set the same size of all the 9-slice's edges when painting an element's background image (Left, Top, Right, Bottom)
        /// <para/> style.unitySliceLeft
        /// <para/> style.unitySliceTop
        /// <para/> style.unitySliceRight
        /// <para/> style.unitySliceBottom
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        /// <param name="value"> Slice value </param>
        public static T SetStyleUnitySlice<T>(this T target, int value) where T : VisualElement =>
            target.SetStyleUnitySlice(value, value, value, value);

        /// <summary> Set the same size of all the 9-slice's edges when painting an element's background image (Left, Top, Right, Bottom)
        /// <para/> style.unitySliceLeft
        /// <para/> style.unitySliceTop
        /// <para/> style.unitySliceRight
        /// <para/> style.unitySliceBottom
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        /// <param name="edge"> Edge 9-slice values </param>
        public static T SetStyleUnitySlice<T>(this T target, EdgeValues edge) where T : VisualElement =>
            target.SetStyleUnitySlice((int)edge.Left, (int)edge.Top, (int)edge.Right, (int)edge.Bottom);

        #region SetStyleUnitySliceLeft, GetStyleUnitySliceLeft

        /// <summary> Set the size of the Left 9-slice's edge when painting an element's background image
        /// <para/> style.unitySliceLeft
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        /// <param name="value"> 9-Slice value </param>
        public static T SetStyleUnitySliceLeft<T>(this T target, int value) where T : VisualElement
        {
            target.style.unitySliceLeft = value;
            return target;
        }

        /// <summary> Get the size of the Left 9-slice's edge when painting an element's background image
        /// <para/> style.unitySliceLeft
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        public static int GetStyleUnitySliceLeft<T>(this T target) where T : VisualElement =>
            target.style.unitySliceLeft.value;

        #endregion

        #region SetStyleUnitySliceTop, GetStyleUnitySliceTop

        /// <summary> Set the size of the Top 9-slice's edge when painting an element's background image
        /// <para/> style.unitySliceTop
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        /// <param name="value"> 9-Slice value </param>
        public static T SetStyleUnitySliceTop<T>(this T target, int value) where T : VisualElement
        {
            target.style.unitySliceTop = value;
            return target;
        }

        /// <summary> Get the size of the Top 9-slice's edge when painting an element's background image
        /// <para/> style.unitySliceTop
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        public static int GetStyleUnitySliceTop<T>(this T target) where T : VisualElement =>
            target.style.unitySliceTop.value;

        #endregion

        #region SetStyleUnitySliceRight, GetStyleUnitySliceRight

        /// <summary> Set the size of the Right 9-slice's edge when painting an element's background image
        /// <para/> style.unitySliceRight
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        /// <param name="value"> 9-Slice value </param>
        public static T SetStyleUnitySliceRight<T>(this T target, int value) where T : VisualElement
        {
            target.style.unitySliceRight = value;
            return target;
        }

        /// <summary> Get the size of the Right 9-slice's edge when painting an element's background image
        /// <para/> style.unitySliceRight
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        public static int GetStyleUnitySliceRight<T>(this T target) where T : VisualElement =>
            target.style.unitySliceRight.value;

        #endregion

        #region SetStyleUnitySliceBottom, GetStyleUnitySliceBottom

        /// <summary> Set the size of the Bottom 9-slice's edge when painting an element's background image
        /// <para/> style.unitySliceBottom
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        /// <param name="value"> 9-Slice value </param>
        public static T SetStyleUnitySliceBottom<T>(this T target, int value) where T : VisualElement
        {
            target.style.unitySliceBottom = value;
            return target;
        }

        /// <summary> Get the size of the Bottom 9-slice's edge when painting an element's background image
        /// <para/> style.unitySliceBottom
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        public static int GetStyleUnitySliceBottom<T>(this T target) where T : VisualElement =>
            target.style.unitySliceBottom.value;

        #endregion

        #endregion

        #region Style - Color

        /// <summary>
        /// Set the color to use when drawing the text of an element
        /// <para/> style.color
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        /// <param name="value"> Color value </param>
        public static T SetStyleColor<T>(this T target, Color value) where T : VisualElement
        {
            target.style.color = value;
            return target;
        }

        /// <summary>
        /// Get the color used when drawing the text of an element
        /// <para/> style.color
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        public static Color GetStyleColor<T>(this T target) where T : VisualElement =>
            target.resolvedStyle.color;

        #endregion

        #region Style - Whitespace

        public static T SetWhiteSpace<T>(this T target, WhiteSpace value) where T : VisualElement
        {
            target.style.whiteSpace = value;
            return target;
        }

        public static WhiteSpace GetWhiteSpace<T>(this T target) where T : VisualElement =>
            target.style.whiteSpace.value;

        #endregion

        #region Style - UnityFont

        public static T SetStyleUnityFont<T>(this T target, Font value) where T : VisualElement
        {
            target.style.unityFont = value;
            return target;
        }

        public static Font GetStyleUnityFont<T>(this T target) where T : VisualElement =>
            target.style.unityFont.value;

        #endregion

        #region Style - FontSize

        /// <summary>
        /// Set the font size to draw the element's text
        /// <para/> style.fontSize
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        /// <param name="value"> Font size value </param>
        public static T SetStyleFontSize<T>(this T target, int value) where T : VisualElement
        {
            target.style.fontSize = value;
            return target;
        }

        /// <summary>
        /// Get the font size used to draw the element's text
        /// <para/> style.fontSize
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        public static float GetStyleFontSize<T>(this T target) where T : VisualElement =>
            target.style.fontSize.value.value;

        #endregion

        #region Style - UnityTextAlign

        /// <summary>
        /// Set the horizontal and vertical text alignment in the element's box
        /// <para/> style.unityTextAlign
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        /// <param name="value"> TextAnchor value </param>
        public static T SetStyleTextAlign<T>(this T target, TextAnchor value) where T : VisualElement
        {
            target.style.unityTextAlign = value;
            return target;
        }

        /// <summary>
        /// Get the horizontal and vertical text alignment in the element's box
        /// <para/> style.unityTextAlign
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        public static TextAnchor GetStyleTextAlign<T>(this T target) where T : VisualElement =>
            target.style.unityTextAlign.value;

        #endregion

        #region Style - Margins

        /// <summary> Set all margins to 0 (zero) </summary>
        /// <param name="target"> Target VisualElement </param>
        public static T ClearMargins<T>(this T target) where T : VisualElement =>
            target.SetStyleMargins(0);

        /// <summary>
        /// Set the space reserved for the all the edges margins during the layout phase (Left, Top, Right, Bottom)
        /// <para/> style.marginLeft
        /// <para/> style.marginTop
        /// <para/> style.marginRight
        /// <para/> style.marginBottom
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        /// <param name="left"> Left edge margin </param>
        /// <param name="top"> Top edge margin </param>
        /// <param name="right"> Right edge margin </param>
        /// <param name="bottom"> Bottom edge margin </param>
        public static T SetStyleMargins<T>(this T target, float left, float top, float right, float bottom) where T : VisualElement
        {
            target.style.marginLeft = left;
            target.style.marginTop = top;
            target.style.marginRight = right;
            target.style.marginBottom = bottom;
            return target;
        }

        /// <summary>
        /// Set the same space reserved for the all the edges margins during the layout phase (Left, Top, Right, Bottom)
        /// <para/> style.marginLeft
        /// <para/> style.marginTop
        /// <para/> style.marginRight
        /// <para/> style.marginBottom
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        /// <param name="value"> Edge margin value </param>
        public static T SetStyleMargins<T>(this T target, float value) where T : VisualElement =>
            target.SetStyleMargins(value, value, value, value);


        /// <summary>
        /// Set the same space reserved for the all the edges margins during the layout phase (Left, Top, Right, Bottom)
        /// <para/> style.marginLeft
        /// <para/> style.marginTop
        /// <para/> style.marginRight
        /// <para/> style.marginBottom
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        /// <param name="edge"> Edge values for margins </param>
        public static T SetStyleMargins<T>(this T target, EdgeValues edge) where T : VisualElement =>
            target.SetStyleMargins(edge.Left, edge.Top, edge.Right, edge.Bottom);

        #region SetStyleMarginLeft, GetStyleMarginLeft

        /// <summary> Set the space reserved for the Left edge margin during the layout phase
        /// <para/> style.marginLeft
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        /// <param name="value"> Left edge margin value </param>
        public static T SetStyleMarginLeft<T>(this T target, float value) where T : VisualElement
        {
            target.style.marginLeft = value;
            return target;
        }

        /// <summary> Get the space reserved for the Left edge margin during the layout phase
        /// <para/> style.marginLeft
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        public static float GetStyleMarginLeft<T>(this T target) where T : VisualElement =>
            target.style.marginLeft.value.value;

        #endregion

        #region SetStyleMarginTop, GetStyleMarginTop

        /// <summary> Set the space reserved for the Top edge margin during the layout phase
        /// <para/> style.marginTop
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        /// <param name="value"> Top edge margin value </param>
        public static T SetStyleMarginTop<T>(this T target, float value) where T : VisualElement
        {
            target.style.marginTop = value;
            return target;
        }

        /// <summary> Get the space reserved for the Top edge margin during the layout phase
        /// <para/> style.marginTop
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        public static float GetStyleMarginTop<T>(this T target) where T : VisualElement =>
            target.style.marginTop.value.value;

        #endregion

        #region SetStyleMarginRight, GetStyleMarginRight

        /// <summary> Set the space reserved for the Right edge margin during the layout phase
        /// <para/> style.marginRight
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        /// <param name="value"> Right edge margin value </param>
        public static T SetStyleMarginRight<T>(this T target, float value) where T : VisualElement
        {
            target.style.marginRight = value;
            return target;
        }

        /// <summary> Get the space reserved for the Right edge margin during the layout phase
        /// <para/> style.marginRight
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        public static float GetStyleMarginRight<T>(this T target) where T : VisualElement =>
            target.style.marginRight.value.value;

        #endregion

        #region SetStyleMarginBottom, GetStyleMarginBottom

        /// <summary> Set the space reserved for the Bottom edge margin during the layout phase
        /// <para/> style.marginBottom
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        /// <param name="value"> Bottom edge margin value </param>
        public static T SetStyleMarginBottom<T>(this T target, float value) where T : VisualElement
        {
            target.style.marginBottom = value;
            return target;
        }

        /// <summary> Get the space reserved for the Bottom edge margin during the layout phase
        /// <para/> style.marginBottom
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        public static float GetStyleMarginBottom<T>(this T target) where T : VisualElement =>
            target.style.marginBottom.value.value;

        #endregion

        #endregion

        #region Style - Padding

        /// <summary> Set all paddings to 0 (zero) </summary>
        /// <param name="target"> Target VisualElement </param>
        public static T ClearPaddings<T>(this T target) where T : VisualElement =>
            target.SetStylePadding(0f);

        /// <summary>
        /// Set the space reserved for all the edges paddings during the layout phase
        /// <para/> style.paddingLeft
        /// <para/> style.paddingTop
        /// <para/> style.paddingRight
        /// <para/> style.paddingBottom
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        /// <param name="left"> Left edge padding </param>
        /// <param name="top"> Top edge padding </param>
        /// <param name="right"> Right edge padding </param>
        /// <param name="bottom"> Bottom edge padding </param>
        public static T SetStylePadding<T>(this T target, float left, float top, float right, float bottom) where T : VisualElement
        {
            target.style.paddingLeft = left;
            target.style.paddingTop = top;
            target.style.paddingRight = right;
            target.style.paddingBottom = bottom;
            return target;
        }

        /// <summary>
        /// Set the same space reserved for all the edges paddings during the layout phase
        /// <para/> style.paddingLeft
        /// <para/> style.paddingTop
        /// <para/> style.paddingRight
        /// <para/> style.paddingBottom
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        /// <param name="value"> Edge padding value </param>
        public static T SetStylePadding<T>(this T target, float value) where T : VisualElement =>
            target.SetStylePadding(value, value, value, value);

        /// <summary>
        /// Set the space reserved for all the edges paddings during the layout phase
        /// <para/> style.paddingLeft
        /// <para/> style.paddingTop
        /// <para/> style.paddingRight
        /// <para/> style.paddingBottom
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        /// <param name="edge"> Edge values for padding </param>
        public static T SetStylePadding<T>(this T target, EdgeValues edge) where T : VisualElement =>
            target.SetStylePadding(edge.Left, edge.Top, edge.Right, edge.Bottom);

        #region SetStylePaddingLeft, GetStylePaddingLeft

        /// <summary> Set the space reserved for the Left edge padding during the layout phase
        /// <para/> style.paddingLeft
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        /// <param name="value"> Left edge padding value </param>
        public static T SetStylePaddingLeft<T>(this T target, float value) where T : VisualElement
        {
            target.style.paddingLeft = value;
            return target;
        }

        /// <summary> Get the space reserved for the Left edge padding during the layout phase
        /// <para/> style.paddingLeft
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        public static float GetStylePaddingLeft<T>(this T target) where T : VisualElement =>
            target.style.paddingLeft.value.value;

        #endregion

        #region SetStylePaddingTop, GetStylePaddingTop

        /// <summary> Set the space reserved for the Top edge padding during the layout phase
        /// <para/> style.paddingTop
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        /// <param name="value"> Top edge padding value </param>
        public static T SetStylePaddingTop<T>(this T target, float value) where T : VisualElement
        {
            target.style.paddingTop = value;
            return target;
        }

        /// <summary> Get the space reserved for the Top edge padding during the layout phase
        /// <para/> style.paddingTop
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        public static float GetStylePaddingTop<T>(this T target) where T : VisualElement =>
            target.style.paddingTop.value.value;

        #endregion

        #region SetStylePaddingRight, GetStylePaddingRight

        /// <summary> Set the space reserved for the Right edge padding during the layout phase
        /// <para/> style.paddingRight
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        /// <param name="value"> Right edge padding value </param>
        public static T SetStylePaddingRight<T>(this T target, float value) where T : VisualElement
        {
            target.style.paddingRight = value;
            return target;
        }

        /// <summary> Get the space reserved for the Right edge padding during the layout phase
        /// <para/> style.paddingRight
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        public static float GetStylePaddingRight<T>(this T target) where T : VisualElement =>
            target.style.paddingRight.value.value;

        #endregion

        #region SetStylePaddingBottom, GetStylePaddingBottom

        /// <summary> Set the space reserved for the Bottom edge padding during the layout phase
        /// <para/> style.paddingBottom
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        /// <param name="value"> Bottom edge padding value </param>
        public static T SetStylePaddingBottom<T>(this T target, float value) where T : VisualElement
        {
            target.style.paddingBottom = value;
            return target;
        }

        /// <summary> Get the space reserved for the Bottom edge padding during the layout phase
        /// <para/> style.paddingBottom
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        public static float GetStylePaddingBottom<T>(this T target) where T : VisualElement =>
            target.style.paddingBottom.value.value;

        #endregion

        #endregion

        #region Resize

        /// <summary>
        /// Resize the element proportionately to the given width
        /// <para/> Adjusts the height automatically
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        /// <param name="referenceWidth"> Target width </param>
        public static T ResizeToWidth<T>(this T target, float referenceWidth) where T : VisualElement
        {
            float width = target.resolvedStyle.width;
            float height = target.resolvedStyle.height;
            float ratio = referenceWidth / width;
            width = referenceWidth;
            height *= ratio;
            return target.SetStyleSize(width, height);
        }

        /// <summary>
        /// Resize the element proportionately to the given height
        /// <para/> Adjusts the width automatically
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        /// <param name="referenceHeight"> Target height </param>
        public static T ResizeToHeight<T>(this T target, float referenceHeight) where T : VisualElement
        {
            float width = target.resolvedStyle.width;
            float height = target.resolvedStyle.height;
            float ratio = referenceHeight / height;
            height = referenceHeight;
            width *= ratio;
            return target.SetStyleSize(width, height);
        }

        /// <summary>
        /// Resize the element, to the given texture size, proportionately to the given width
        /// <para/> Adjusts the height automatically
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        /// <param name="texture"> Target Texture </param>
        /// <param name="referenceWidth"> Target width </param>
        public static T ResizeToTextureWidth<T>(this T target, Texture texture, float referenceWidth) where T : VisualElement
        {
            if (texture == null)
                return target;

            float width = texture.width;
            float height = texture.height;
            float ratio = referenceWidth / width;
            width = referenceWidth;
            height *= ratio;
            return target.SetStyleSize(width, height);
        }

        /// <summary>
        /// Resize the element, to the given texture size, proportionately to the given height
        /// <para/> Adjusts the width automatically
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        /// <param name="texture"> Target Texture </param>
        ///  /// <param name="referenceHeight"> Target height </param>
        public static T ResizeToTextureHeight<T>(this T target, Texture texture, float referenceHeight) where T : VisualElement
        {
            if (texture == null)
                return target;

            float width = texture.width;
            float height = texture.height;
            float ratio = referenceHeight / height;
            height = referenceHeight;
            width *= ratio;
            return target.SetStyleSize(width, height);
        }

        /// <summary>
        /// Resize the element, to its own background image texture size, proportionately to the given width
        /// <para/> Adjusts the height automatically
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        /// <param name="referenceWidth"> Target width </param>
        public static T ResizeToTextureWidth<T>(this T target, float referenceWidth) where T : VisualElement =>
            target.ResizeToTextureWidth(target.GetStyleBackgroundImageTexture2D(), referenceWidth);

        /// <summary>
        /// Resize the element, to its own background image texture size, proportionately to the given height
        /// <para/> Adjusts the width automatically
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        ///  /// <param name="referenceHeight"> Target height </param>
        public static T ResizeToTextureHeight<T>(this T target, float referenceHeight) where T : VisualElement =>
            target.ResizeToTextureHeight(target.GetStyleBackgroundImageTexture2D(), referenceHeight);

        /// <summary> Resize the element to the given texture size and apply a given ratio (to make it bigger or smaller proportionately) </summary>
        /// <param name="target"> Target VisualElement </param>
        /// <param name="texture"> Target Texture </param>
        /// <param name="ratio"> Size change ratio </param>
        public static T ResizeToTextureSize<T>(this T target, Texture texture, float ratio = 1) where T : VisualElement
        {
            bool hasTexture = texture != null;
            float width = hasTexture ? texture.width : 0f;
            float height = hasTexture ? texture.height : 0f;
            ratio = Mathf.Max(0, ratio);
            width *= ratio;
            height *= ratio;
            return target.SetStyleSize(width, height);
        }

        /// <summary> Resize the element to its own background image texture size and apply a given ratio (to make it bigger or smaller proportionately) </summary>
        /// <param name="target"> Target VisualElement </param>
        /// <param name="ratio"> Size change ratio </param>
        public static T ResizeToTextureSize<T>(this T target, float ratio = 1) where T : VisualElement =>
            target.ResizeToTextureSize(target.GetStyleBackgroundImageTexture2D(), ratio);

        #endregion

        /// <summary> Reset all relevant layout settings - Margins, Paddings, Distances - by setting them all to 0 (zero) </summary>
        /// <param name="target"> Target VisualElement </param>
        public static T ResetLayout<T>(this T target) where T : VisualElement =>
            target
                .SetStyleDisplay(DisplayStyle.Flex)
                .ResetStyleMinSize()
                .ResetStyleSize()
                .ResetStyleMaxSize()
                .ClearMargins()
                .ClearPaddings()
                .ClearDistances();

        #region Style - Display

        /// <summary>
        /// Set how an element is displayed in the layout
        /// <para/> style.display
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        /// <param name="value"> DisplayStyle value </param>
        public static T SetStyleDisplay<T>(this T target, DisplayStyle value) where T : VisualElement
        {
            target.style.display = value;
            return target;
        }

        /// <summary>
        /// Get how an element is displayed in the layout
        /// <para/> style.display
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        public static DisplayStyle GetStyleDisplay<T>(this T target) where T : VisualElement =>
            target.style.display.value;

        /// <summary>
        /// Set the element to display normally
        /// <para/> style.display
        /// <para/> DisplayStyle.Flex
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        public static T Show<T>(this T target) where T : VisualElement =>
            target.SetStyleDisplay(DisplayStyle.Flex);

        /// <summary>
        /// Set the element as not visible and absent from the layout
        /// <para/> style.display
        /// <para/> DisplayStyle.None
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        public static T Hide<T>(this T target) where T : VisualElement =>
            target.SetStyleDisplay(DisplayStyle.None);

        #endregion

        #region Style - Left, Top, Right, Bottom - Distance from the element's box

        /// <summary> Set all distances Left, Top, Right, Bottom to 0 (zero) </summary>
        /// <param name="target"> Target VisualElement </param>
        public static T ClearDistances<T>(this T target) where T : VisualElement =>
            target.SetStyleDistance(0, 0, 0, 0);

        #region Distance - set all Left, Top, Right and Bottom

        /// <summary>
        /// Set the Left distance from the element's box during layout
        /// <para/> style.left
        /// <para/> style.top
        /// <para/> style.right
        /// <para/> style.bottom
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        /// <param name="left"> Left distance </param>
        /// <param name="top"> Top distance </param>
        /// <param name="right"> Right distance </param>
        /// <param name="bottom"> Bottom distance </param>
        public static T SetStyleDistance<T>(this T target, float left, float top, float right, float bottom) where T : VisualElement
        {
            target.style.left = left;
            target.style.top = top;
            target.style.right = right;
            target.style.bottom = bottom;
            return target;
        }

        #endregion

        #region Left

        /// <summary>
        /// Set the Left distance from the element's box during layout
        /// <para/> style.left
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        /// <param name="value"> Left distance </param>
        public static T SetStyleLeft<T>(this T target, float value) where T : VisualElement
        {
            target.style.left = value;
            return target;
        }

        /// <summary>
        /// Get the Left distance from the element's box during layout
        /// <para/> style.left
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        public static float GetStyleLeft<T>(this T target) where T : VisualElement =>
            target.style.left.value.value;

        #endregion

        #region Top

        /// <summary>
        /// Set the Top distance from the element's box during layout
        /// <para/> style.left
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        /// <param name="value"> Top distance </param>
        public static T SetStyleTop<T>(this T target, float value) where T : VisualElement
        {
            target.style.top = value;
            return target;
        }

        /// <summary>
        /// Get the Top distance from the element's box during layout
        /// <para/> style.left
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        public static float GetStyleTop<T>(this T target) where T : VisualElement =>
            target.style.top.value.value;

        #endregion

        #region Right

        /// <summary>
        /// Set the Right distance from the element's box during layout
        /// <para/> style.left
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        /// <param name="value"> Right distance </param>
        public static T SetStyleRight<T>(this T target, float value) where T : VisualElement
        {
            target.style.right = value;
            return target;
        }

        /// <summary>
        /// Get the Right distance from the element's box during layout
        /// <para/> style.left
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        public static float GetStyleRight<T>(this T target) where T : VisualElement =>
            target.style.right.value.value;

        #endregion

        #region Bottom

        /// <summary>
        /// Set the Bottom distance from the element's box during layout
        /// <para/> style.left
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        /// <param name="value"> Bottom distance </param>
        public static T SetStyleBottom<T>(this T target, float value) where T : VisualElement
        {
            target.style.bottom = value;
            return target;
        }

        /// <summary>
        /// Get the Bottom distance from the element's box during layout
        /// <para/> style.left
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        public static float GetStyleBottom<T>(this T target) where T : VisualElement =>
            target.style.bottom.value.value;

        #endregion

        #endregion

        /// <summary>
        /// Returns true if the parent is not null
        /// <para/> parent != null
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        public static bool HasParent<T>(this T target) where T : VisualElement =>
            target.parent != null;

        /// <summary>
        /// Returns TRUE if the parent is not null and if style.display is set to DisplayStyle.Flex
        /// <para/> parent != null
        /// <para/> target.style.display.value == DisplayStyle.Flex
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        public static bool IsVisible<T>(this T target) where T : VisualElement =>
            target.HasParent() && target.style.display.value == DisplayStyle.Flex;

        /// <summary>
        /// Set the VisualElement to the enabled state. A disabled VisualElement does not receive most events
        /// <para/> SetEnabled(true)
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        public static T EnableElement<T>(this T target) where T : VisualElement
        {
            target.SetEnabled(true);
            return target;
        }

        /// <summary>
        /// Set the VisualElement to the disabled state. A disabled VisualElement does not receive most events
        /// <para/> SetEnabled(false)
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        public static T DisableElement<T>(this T target) where T : VisualElement
        {
            target.SetEnabled(false);
            return target;
        }

        /// <summary>
        /// Returns TRUE if the VisualElement is enabled locally and if is also enabled in its own hierarchy
        /// <para/> enabledSelf && enabledInHierarchy
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        public static bool IsEnabled<T>(this T target) where T : VisualElement =>
            target.enabledSelf && target.enabledInHierarchy;

        /// <summary>
        /// Replace this VisualElement with another one (while preserving its place in the hierarchy and its children).
        /// Returns a reference to the other VisualElement
        /// </summary>
        /// <param name="target"> Target VisualElement </param>
        /// <param name="other"> VisualElement that will replace the current one (</param>
        public static O ReplaceWith<T, O>(this T target, O other) where T : VisualElement where O : VisualElement
        {
            if (target.childCount > 0) //check if the target has children
            {
                //target has children -> move them to the replacer element
                var children = new List<VisualElement>(target.Children());
                foreach (VisualElement child in children)
                    other.AddChild(child);
            }

            if (!target.HasParent()) return other;
            VisualElement parent = target.parent;                      //get the target parent
            int targetIndexInParentHierarchy = parent.IndexOf(target); //get the 'child index' of the target inside the parent's hierarchy
            target.RemoveFromHierarchy();                              //remove the target from its parent hierarchy
            parent.Insert(targetIndexInParentHierarchy, other);        //insert the element back to the parent at the initial target's index
            return other;                                              //return a reference to the element
        }

        public static bool IsFocused<T>(this T target) where T : VisualElement =>
            target.focusable && target.focusController.focusedElement == target;
    }
}
