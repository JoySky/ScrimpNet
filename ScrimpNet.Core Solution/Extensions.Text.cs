﻿/**
/// ScrimpNet.Core Library
/// Copyright (c) 2005-2010
///
/// This module is Copyright (c) 2005-2010 Steve Powell
/// All rights reserved.
///
/// This library is free software; you can redistribute it and/or
/// modify it under the terms of the Microsoft Public License (Ms-PL)
/// 
/// This library is distributed in the hope that it will be
/// useful, but WITHOUT ANY WARRANTY; without even the implied
/// warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR
/// PURPOSE.  See theMicrosoft Public License (Ms-PL) License for more
/// details.
///
/// You should have received a copy of the Microsoft Public License (Ms-PL)
/// License along with this library; if not you may 
/// find it here: http://www.opensource.org/licenses/ms-pl.html
///
/// Steve Powell, spowell@scrimpnet.com
**/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScrimpNet.Core
{
    public static partial class Extensions
    {
        /// <summary>
        /// Extension method since StringBuilder.AppendLine() does not support format arguments
        /// </summary>
        /// <param name="sb">String builder that updates string</param>
        /// <param name="format">Format of string to append</param>
        /// <param name="args">Arguments to supply to format string</param>
        /// <returns>Formatted string appended to existing string builder with new line appended</returns>
        public static StringBuilder AppendLine(this StringBuilder sb, string format, params object[] args)
        {
            sb.AppendFormat(format, args).AppendLine();
            return sb;
        }


        /// <summary>
        /// (extension) Mask last 4 characters of a string
        /// </summary>
        /// <param name="plainText">Credit card number to be revealed</param>
        /// <returns>String with last four characters masked</returns>
        public static string MaskRight(this string plainText)
        {
            return MaskRight(plainText, 4);
        }

        /// <summary>
        /// (extension) Mask last X characters of a string
        /// </summary>
        /// <param name="plainText">String which to mask right X characters</param>
        /// <param name="places">Number of right hand characters</param>
        /// <returns>String with right X characters masked</returns>
        public static string MaskRight(this string plainText, int places)
        {
            return MaskRight(plainText, 4, '#');
        }

        /// <summary>
        /// (extension) Mask the right most X characters of a string with the specified character
        /// </summary>
        /// <param name="plainText">Text which right most characters should be masked</param>
        /// <param name="places">Number of characters to mask</param>
        /// <param name="character">Character to use as the mask character</param>
        /// <returns>Masked string or input value if null or empty</returns>
        public static string MaskRight(this string plainText, int places, char character)
        {
            if (string.IsNullOrEmpty(plainText) == true || plainText.Length == 0)
                return plainText;

            if (plainText.Length > places)
            {
                places = plainText.Length;
            }

            plainText = plainText.Substring(0, plainText.Length - places);
            return plainText.PadRight(places, character);

        }

        /// <summary>
        /// Return right most characters of string
        /// </summary>
        /// <param name="target">string that is being evaluated</param>
        /// <param name="characterCount">Number of characters from right end of string to return</param>
        /// <returns>Right most characters of string or entired string if characterCount > length</returns>
        public static string Right(this string target, int characterCount)
        {
            if (string.IsNullOrEmpty(target) == true || target.Length <= 1) return target;
            int startPos = target.Length - characterCount;
            if (startPos < 0) return target;
            return target.Substring(startPos);
        }
    }
}
