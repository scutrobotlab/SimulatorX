// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using static System.String;

namespace Doozy.Runtime.Common.Extensions
{
    public static class StringExtensions
    {
        /// <summary> Remove all whitespaces from string </summary>
        /// <param name="target"> Target string </param>
        public static string RemoveWhitespaces(this string target) =>
            Regex.Replace(target, @"\s+", "");

        /// <summary> Convert all whitespaces to single space </summary>
        /// <param name="target"> Target string </param>
        public static string ConvertWhitespacesToSingleSpaces(this string target) =>
            Regex.Replace(target, @"\s+", " ");

        /// <summary> Reverse back or forward slashes </summary>
        /// <param name="target"> String value </param>
        /// <param name="direction">
        ///         0 - replace forward slash with back
        /// <para/> 1 - replace back with forward slash
        /// </param>
        /// <returns></returns>
        public static string ReverseSlash(this string target, int direction)
        {
            switch (direction)
            {
                case 0:
                    return target.Replace(@"/", @"\");
                case 1:
                    return target.Replace(@"\", @"/");
                default:
                    return target;
            }
        }

        /// <summary>
        ///         Get the left part of the string, up until the character c.
        /// <para/> If c is not found in the string the whole string is returned.
        /// </summary>
        /// <param name="target"> String value to truncate </param>
        /// <param name="c"> Separator </param>
        public static string LeftOf(this string target, char c)
        {
            int ndx = target.IndexOf(c);
            return ndx >= 0
                ? target.Substring(0, ndx)
                : target;
        }

        /// <summary>
        ///         Get the right part of the string, after the character c.
        /// <para/> If c is not found in the string the whole string is returned.
        /// </summary>
        /// <param name="target"> String value to truncate </param>
        /// <param name="c"> Separator </param>
        public static string RightOf(this string target, char c)
        {
            int ndx = target.IndexOf(c);
            return ndx == -1
                ? target
                : target.Substring(ndx + 1);
        }


        /// <summary> Remove the last character from a string </summary>
        /// <param name="target"> String value </param>
        public static string RemoveLastCharacter(this string target) =>
            target.Substring(0, target.Length - 1);

        /// <summary> Remove the last number of characters from a string </summary>
        /// <param name="target"> String value </param>
        /// <param name="numberOfCharactersToRemove"> Number of characters to remove from the end of the target string </param>
        public static string RemoveLast(this string target, int numberOfCharactersToRemove) =>
            target.IsNullOrEmpty() ? string.Empty : target.Substring(0, target.Length - numberOfCharactersToRemove);

        /// <summary> Remove the first character from a string </summary>
        /// <param name="target"> String value </param>
        public static string RemoveFirstCharacter(this string target) =>
            target.Substring(1);

        /// <summary> Remove the first number of characters from a string </summary>
        /// <param name="target"> String value </param>
        /// <param name="numberOfCharactersToRemove"> Number of characters to remove from the start of the target string </param>
        public static string RemoveFirst(this string target, int numberOfCharactersToRemove) =>
            target.Substring(numberOfCharactersToRemove);

        /// <summary> Remove all special characters from the string </summary>
        /// <param name="target"> String value </param>
        /// <returns> The adjusted string </returns>
        public static string RemoveAllSpecialCharacters(this string target)
        {
            var sb = new StringBuilder(target.Length);
            foreach (char c in target.Where(char.IsLetterOrDigit)) sb.Append(c);
            return sb.ToString();
        }

        /// <summary>
        /// Remove all empty lines from a formatted string
        /// <seealso aref="https://stackoverflow.com/questions/7647716/how-to-remove-empty-lines-from-a-formatted-string"/>
        /// </summary>
        /// <param name="target"> String value </param>
        public static string RemoveAllEmptyLines(this string target)
        {
            return Regex.Replace(target, @"^\s*$\n|\r", Empty, RegexOptions.Multiline).TrimEnd();
        }

        /// <summary> Replace Line Feeds </summary>
        /// <param name="target"> String value to remove line feeds from </param>
        /// <returns> System.string </returns>
        public static string ReplaceLineFeeds(this string target) =>
            Regex.Replace(target, @"^[\r\n]+|\.|[\r\n]+$", "");

        /// <summary> Check string is null </summary>
        /// <param name="target"> String to evaluate </param>
        /// <returns> True if string is null else false </returns>
        public static bool IsNull(this string target) =>
            target == null;

        /// <summary> Check if string is null or empty </summary>
        /// <param name="target"> String to evaluate </param>
        /// <returns> True if string is null or is empty else false </returns>
        public static bool IsNullOrEmpty(this string target) =>
            string.IsNullOrEmpty(target);

        /// <summary>
        ///         Check if string length is a certain minimum number of characters.
        /// <para/> Does not ignore leading and trailing white-space.
        /// <para/> null strings will always evaluate to false.
        /// </summary>
        /// <param name="target"> String value to evaluate minimum length </param>
        /// <param name="minCharLength"> Minimum allowable string length </param>
        /// <returns> True if string is of specified minimum length </returns>
        public static bool IsMinLength(this string target, int minCharLength) =>
            target != null &&
            target.Length >= minCharLength;

        /// <summary>
        ///     Check if string length is consists of specified allowable maximum char length.
        ///	<para/> Does not ignore leading and trailing white-space.
        /// <para/> null strings will always evaluate to false.
        /// </summary>
        /// <param name="target"> String value to evaluate maximum length </param>
        /// <param name="maxCharLength"> Maximum allowable string length </param>
        /// <returns> True if string has specified maximum char length </returns>
        public static bool IsMaxLength(this string target, int maxCharLength) =>
            target != null &&
            target.Length <= maxCharLength;

        /// <summary>
        ///         Check if string length satisfies minimum and maximum allowable char length.
        ///	<para/> Does not ignore leading and trailing white-space.
        /// </summary>
        /// <param name="target"> String value to evaluate </param>
        /// <param name="minCharLength"> Minimum char length </param>
        /// <param name="maxCharLength"> Maximum char length </param>
        /// <returns> True if string satisfies minimum and maximum allowable length </returns>
        public static bool IsLength(this string target, int minCharLength, int maxCharLength) =>
            target != null &&
            target.Length >= minCharLength &&
            target.Length <= minCharLength;

        /// <summary> Get the number of characters in string checks if string is null </summary>
        /// <param name="target"> String to evaluate length of </param>
        /// <returns> Total number of chars or null if string is null </returns>
        public static int? GetLength(string target) =>
            target?.Length;


        /// <summary> Extract the left part of the input string limited with the length parameter </summary>
        /// <param name="target"> The input string to take the left part from </param>
        /// <param name="length"> The total number characters to take from the input string </param>
        /// <returns> The substring starting at startIndex 0 until length </returns>
        /// <exception cref="System.ArgumentNullException"> Input is null </exception>
        /// <exception cref="System.ArgumentOutOfRangeException"> Length is smaller than zero or higher than the length of input </exception>
        public static string Left(this string target, int length) =>
            String.IsNullOrEmpty(target)
                ? throw new ArgumentNullException(nameof(target))
                : length < 0 || length > target.Length
                    ? throw new ArgumentOutOfRangeException(nameof(length), "Length cannot be higher than total string length or less than 0")
                    : target.Substring(0, length);

        /// <summary> Extract the right part of the input string limited with the length parameter </summary>
        /// <param name="target"> The input string to take the right part from </param>
        /// <param name="length"> The total number characters to take from the input string </param>
        /// <returns> The substring taken from the input string </returns>
        /// <exception cref="System.ArgumentNullException"> Input is null </exception>
        /// <exception cref="System.ArgumentOutOfRangeException"> Length is smaller than zero or higher than the length of input </exception>
        public static string Right(this string target, int length) =>
            String.IsNullOrEmpty(target)
                ? throw new ArgumentNullException(nameof(target))
                : length < 0 || length > target.Length
                    ? throw new ArgumentOutOfRangeException(nameof(length), "Length cannot be higher than total string length or less than 0")
                    : target.Substring(target.Length - length);


        /// <summary> Check if a string does not start with prefix </summary>
        /// <param name="target"> String value to evaluate</param>
        /// <param name="prefix"> Prefix </param>
        /// <returns> True if string does not match prefix else false, null values will always evaluate to false </returns>
        public static bool DoesNotStartWith(this string target, string prefix) =>
            target == null ||
            prefix == null ||
            !target.StartsWith(prefix, StringComparison.InvariantCulture);

        /// <summary> Check if a string does not end with prefix </summary>
        /// <param name="target"> String value to evaluate</param>
        /// <param name="suffix"> Suffix</param>
        /// <returns> True if string does not match prefix else false, null values will always evaluate to false </returns>
        public static bool DoesNotEndWith(this string target, string suffix) =>
            target == null ||
            suffix == null ||
            !target.EndsWith(suffix, StringComparison.InvariantCulture);

        /// <summary> Remove the first part of the string, if no match found return original string </summary>
        /// <param name="target"> String value to remove prefix from </param>
        /// <param name="prefix"> Prefix </param>
        /// <param name="ignoreCase"> Indicates whether the compare should ignore case </param>
        /// <returns> Trimmed string with no prefix or original string </returns>
        public static string RemovePrefix(this string target, string prefix, bool ignoreCase = true) =>
            !String.IsNullOrEmpty(target) && (ignoreCase
                ? target.StartsWithIgnoreCase(prefix)
                : target.StartsWith(prefix))
                ? target.Substring(prefix.Length, target.Length - prefix.Length)
                : target;

        /// <summary> Remove the end part of the string, if no match found return original string </summary>
        /// <param name="target"> String value to remove suffix from </param>
        /// <param name="suffix"> Suffix </param>
        /// <param name="ignoreCase"> Indicates whether the compare should ignore case </param>
        /// <returns> Trimmed string with no suffix or original string </returns>
        public static string RemoveSuffix(this string target, string suffix, bool ignoreCase = true) =>
            !String.IsNullOrEmpty(target) && (ignoreCase
                ? target.EndsWithIgnoreCase(suffix)
                : target.EndsWith(suffix))
                ? target.Substring(0, target.Length - suffix.Length)
                : Empty;

        /// <summary> Append the suffix to the end of the string if the string does not already end in the suffix </summary>
        /// <param name="target"> String value to append suffix to </param>
        /// <param name="suffix"> Suffix </param>
        /// <param name="ignoreCase"> Indicates whether the compare should ignore case </param>
        public static string AppendSuffixIfMissing(this string target, string suffix, bool ignoreCase = true) =>
            String.IsNullOrEmpty(target) || (ignoreCase
                ? target.EndsWithIgnoreCase(suffix)
                : target.EndsWith(suffix))
                ? target
                : target + suffix;

        /// <summary> Append the prefix to the start of the string if the string does not already start with prefix </summary>
        /// <param name="target"> String value to append prefix to </param>
        /// <param name="prefix"> Prefix </param>
        /// <param name="ignoreCase"> Indicates whether the compare should ignore case </param>
        public static string AppendPrefixIfMissing(this string target, string prefix, bool ignoreCase = true) =>
            String.IsNullOrEmpty(target) || (ignoreCase
                ? target.StartsWithIgnoreCase(prefix)
                : target.StartsWith(prefix))
                ? target
                : prefix + target;

        /// <summary>
        ///     Read in a sequence of words from standard input and capitalize each
        ///     one (make first letter uppercase; make rest lowercase).
        /// </summary>
        /// <param name="target"> String value </param>
        /// <returns> Word with capitalization </returns>
        public static string Capitalize(this string target) =>
            target.Length == 0 ? target : target.Substring(0, 1).ToUpper() + target.Substring(1).ToLower();

        /// <summary> Get the first character in string </summary>
        /// <param name="target"> String value </param>
        /// <returns> System.string </returns>
        public static string FirstCharacter(this string target) =>
            !String.IsNullOrEmpty(target)
                ? target.Length >= 1
                    ? target.Substring(0, 1)
                    : target
                : null;

        /// <summary> Get the last character in string </summary>
        /// <param name="target"> String value </param>
        /// <returns> System.string </returns>
        public static string LastCharacter(this string target) =>
            !String.IsNullOrEmpty(target)
                ? target.Length >= 1
                    ? target.Substring(target.Length - 1, 1)
                    : target
                : null;

        /// <summary> Check if a string ends with another string ignoring the case </summary>
        /// <param name="target"> String value </param>
        /// <param name="suffix"> Suffix </param>
        /// <returns> True or False </returns>
        public static bool EndsWithIgnoreCase(this string target, string suffix) =>
            target == null
                ? throw new ArgumentNullException(nameof(target), "Target parameter is null")
                : suffix == null
                    ? throw new ArgumentNullException(nameof(suffix), "Suffix parameter is null")
                    : target.Length >= suffix.Length && target.EndsWith(suffix, StringComparison.InvariantCultureIgnoreCase);

        /// <summary> Check if a string starts with another string ignoring the case </summary>
        /// <param name="target"> String value </param>
        /// <param name="prefix"> Prefix </param>
        /// <returns> True or False </returns>
        public static bool StartsWithIgnoreCase(this string target, string prefix) =>
            target == null
                ? throw new ArgumentNullException(nameof(target), "Target parameter is null")
                : prefix == null
                    ? throw new ArgumentNullException(nameof(prefix), "Prefix parameter is null")
                    : target.Length >= prefix.Length && target.StartsWith(prefix, StringComparison.InvariantCultureIgnoreCase);

        /// <summary> Replace specified characters with an empty string </summary>
        /// <param name="target"> String value </param>
        /// <param name="chars"> List of characters to replace from the string </param>
        /// <remarks>
        ///     string s = "Friends";
        ///     s = s.Replace('F', 'r','i','s');  //s becomes 'end;
        /// </remarks>
        /// <returns> System.string </returns>
        public static string Replace(this string target, params char[] chars) =>
            chars.Aggregate(target, (current, c) => current.Replace(c.ToString(CultureInfo.InvariantCulture), ""));

        /// <summary> Remove Characters from string</summary>
        /// <param name="target"> String value </param>
        /// <param name="chars"> List of characters to remove from the string </param>
        /// <returns> System.string </returns>
        public static string RemoveChars(this string target, params char[] chars)
        {
            var sb = new StringBuilder(target.Length);
            foreach (char c in target.Where(c => !chars.Contains(c))) sb.Append(c);
            return sb.ToString();
        }

        /// <summary> Validate email address </summary>
        /// <param name="target"> String email address </param>
        /// <returns> True or False </returns>
        public static bool IsEmailAddress(this string target)
        {
            const string pattern = "^[a-zA-Z][\\w\\.-]*[a-zA-Z0-9]@[a-zA-Z0-9][\\w\\.-]*[a-zA-Z0-9]\\.[a-zA-Z][a-zA-Z\\.]*[a-zA-Z]$";
            return Regex.Match(target, pattern).Success;
        }

        /// <summary> Reverse string </summary>
        /// <param name="target"> String value </param>
        /// <returns> System.string </returns>
        public static string Reverse(this string target)
        {
            char[] chars = new char[target.Length];
            for (int i = target.Length - 1, j = 0; i >= 0; --i, ++j) chars[j] = target[i];
            target = new string(chars);
            return target;
        }

        /// <summary> Count number of occurrences in string </summary>
        /// <param name="target"> String value containing text </param>
        /// <param name="stringToMatch"> String or pattern find </param>
        /// <returns> int </returns>
        public static int CountOccurrences(this string target, string stringToMatch) =>
            Regex.Matches(target, stringToMatch, RegexOptions.IgnoreCase).Count;


        /// <summary>
        ///     Check if the String contains only Unicode letters.
        ///     null will return false. An empty String ("") will return false.
        /// </summary>
        /// <param name="target"> String value to check if is Alpha </param>
        /// <returns> True if only contains letters, and is non-null </returns>
        public static bool IsAlpha(this string target) =>
            !String.IsNullOrEmpty(target) && target.Trim()
                .Replace(" ", "")
                .All(char.IsLetter);

        /// <summary>
        ///     Check if the String contains only Unicode letters, digits.
        ///     null will return false. An empty String ("") will return false.
        /// </summary>
        /// <param name="target"> String value to check if is Alpha or Numeric </param>
        public static bool IsAlphaNumeric(this string target) =>
            !String.IsNullOrEmpty(target) && target.Trim()
                .Replace(" ", "")
                .All(char.IsLetterOrDigit);

        /// <summary>
        /// Encrypt a string using the supplied key.
        /// <para/> Encoding is done using RSA encryption.
        /// </summary>
        /// <param name="target"> String value that must be encrypted </param>
        /// <param name="key"> Encryption key </param>
        /// <returns> A string representing a byte array separated by a minus sign </returns>
        /// <exception cref="ArgumentException"> Occurs when stringToEncrypt or key is null or empty </exception>
        public static string Encrypt(this string target, string key)
        {
            var cspParameter = new CspParameters { KeyContainerName = key };
            var rsaServiceProvider = new RSACryptoServiceProvider(cspParameter) { PersistKeyInCsp = true };
            byte[] bytes = rsaServiceProvider.Encrypt(Encoding.UTF8.GetBytes(target), true);
            return BitConverter.ToString(bytes);
        }


        /// <summary>
        /// Decrypt a string using the supplied key.
        /// <para/> Decoding is done using RSA encryption.
        /// </summary>
        /// <param name="target"> String value that must be decrypted </param>
        /// <param name="key"> Decryption key </param>
        /// <returns> The decrypted string or null if decryption failed </returns>
        /// <exception cref="ArgumentException"> Occurs when stringToDecrypt or key is null or empty </exception>
        public static string Decrypt(this string target, string key)
        {
            var cspParameters = new CspParameters { KeyContainerName = key };
            var rsaServiceProvider = new RSACryptoServiceProvider(cspParameters) { PersistKeyInCsp = true };
            string[] decryptArray = target.Split(new[] { "-" }, StringSplitOptions.None);
            byte[] decryptByteArray = Array.ConvertAll(decryptArray, s => Convert.ToByte(byte.Parse(s, NumberStyles.HexNumber)));
            byte[] bytes = rsaServiceProvider.Decrypt(decryptByteArray, true);
            string result = Encoding.UTF8.GetString(bytes);
            return result;
        }

        /// <summary> Calculate the amount of bytes occupied by the input string encoded as the encoding specified </summary>
        /// <param name="target"> The input string to check </param>
        /// <param name="encoding"> The encoding to use </param>
        /// <returns> The total size of the input string in bytes </returns>
        /// <exception cref="System.ArgumentNullException"> Input is null </exception>
        /// <exception cref="System.ArgumentNullException"> Encoding is null </exception>
        public static int GetByteSize(this string target, Encoding encoding) =>
            target == null
                ? throw new ArgumentNullException(nameof(target))
                : encoding == null
                    ? throw new ArgumentNullException(nameof(encoding))
                    : encoding.GetByteCount(target);
    }
}
