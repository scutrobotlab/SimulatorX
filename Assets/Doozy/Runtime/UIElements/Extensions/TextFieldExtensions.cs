// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using UnityEngine.UIElements;

namespace Doozy.Runtime.UIElements.Extensions
{
    public static class TextFieldExtensions
    {
        public static T SetIsDelayed<T>(this T target, bool isDelayed) where T : TextField
        {
            target.isDelayed = isDelayed;
            return target;
        }
        
        public static T SetIsPasswordField<T>(this T target, bool isPasswordField) where T : TextField
        {
            target.isPasswordField = isPasswordField;
            return target;
        }
        
        public static T SetIsReadOnly<T>(this T target, bool isReadOnly) where T : TextField
        {
            target.isReadOnly = isReadOnly;
            return target;
        }

        public static T SetMultiline<T>(this T target, bool multiline) where T : TextField
        {
            target.multiline = multiline;
            return target;
        }
        
        public static T SetMaxLength<T>(this T target, int maxLength) where T : TextField
        {
            target.maxLength = maxLength;
            return target;
        }
        
        public static T SetDoubleClickSelectsWord<T>(this T target, bool doubleClickSelectsWord) where T : TextField
        {
            target.doubleClickSelectsWord = doubleClickSelectsWord;
            return target;
        }
        
        public static T SetTripleClickSelectsLine<T>(this T target, bool tripleClickSelectsLine) where T : TextField
        {
            target.tripleClickSelectsLine = tripleClickSelectsLine;
            return target;
        }
        
        public static T SetMaskChar<T>(this T target, char maskChar) where T : TextField
        {
            target.maskChar = maskChar;
            return target;
        }
    }
}
