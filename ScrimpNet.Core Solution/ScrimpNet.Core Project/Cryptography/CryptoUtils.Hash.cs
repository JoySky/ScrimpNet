/**
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
using System.Security.Cryptography;
using ScrimpNet.Text;

namespace ScrimpNet.Cryptography
{
    public static partial class CryptoUtils
    {
        /// <summary>
        /// Format of string return types
        /// </summary>
        public enum StringFormat
        {
            /// <summary>
            /// Base64 value.  Suitable for storing in text fields, .config etc.
            /// </summary>
            Base64,

            /// <summary>
            /// Hex value. Suitable for storing in text fields, .config etc. Note: Hex takes more storage than Base64 but may be faster to generate/decode
            /// </summary>
            Hex,

            /// <summary>
            /// Unicode (UTF-16) representation of bytes.  Suitable for storing in binary fields
            /// </summary>
            Unicode,
        }
        /// <summary>
        /// Helper methods of computing hash keys
        /// </summary>
        public static class Hash
        {
            /// <summary>
            /// Convenience method. Compute a hash of some text using default hash (Sha1, Base64)
            /// </summary>
            /// <param name="plainText">Text that is being hashed</param>
            /// <returns>Base64 representation of hash code</returns>
            public static string Compute(string plainText)
            {
                return Compute(plainText, StringFormat.Base64);
            }

            /// <summary>
            /// Convenience method. Compute hash of some text using default hash. (Sha1)  See remarks to change default.
            /// </summary>
            /// <param name="plainText">Text that is being hashed</param>
            /// <param name="format">How text will represent the hashed value</param>
            /// <returns>String representation of computed hash value</returns>
            /// <remarks>
            /// To change default hash algorithm, simply change the call to another method
            /// <para>Sha1 was chosen as default as a balance between storage and security</para>
            /// </remarks>
            public static string Compute(string plainText, StringFormat format)
            {
                return ComputeSha1(plainText, format);
            }

            /// <summary>
            /// Perform actual hashing on data
            /// </summary>
            /// <param name="algorithm">One of the .Net ...CryptoServiceProviders</param>
            /// <param name="data">Bytes that are going to have their hash value calculated</param>
            /// <returns>Actual hash bytes</returns>
            public static byte[] Compute(HashAlgorithm algorithm, byte[] data)
            {
                return algorithm.ComputeHash(data);
            }

            /// <summary>
            /// Calcualte a hash value on a string
            /// </summary>
            /// <param name="algorithm">One of the .Net ...CryptoServiceProviders</param>
            /// <param name="plainText">Text that is having it's hash calculated on</param>
            /// <param name="format">How the hash code will be represented on the string</param>
            /// <returns>String representation of calculated hash</returns>
            public static string Compute(HashAlgorithm algorithm, string plainText, StringFormat format)
            {
                byte[] sourceBytes = ToBytes(plainText,format);
                byte[] hashBytes = Compute(algorithm, sourceBytes);
                return FromBytes(hashBytes, format);
            }

            /// <summary>
            /// Create a one-way 512bit (64 bytes) hash of some text.  Strongest form of hash.  Suitable for password
            /// </summary>
            /// <param name="plainText">Clear text to hash</param>
            /// <param name="format">Determines how return value will be formatted</param>
            /// <returns>Base64 Hashed results</returns>
            public static string Compute512(string plainText, StringFormat format)
            {
                return Compute(new SHA512Managed(), plainText, format);
            }

            /// <summary>
            /// Create a one-way 128bit (16 byte) hash of some text.  Suitable for file comparison and general security.  Not suitable for absolute non-repudiation tasks
            /// </summary>
            /// <param name="plainText">Text that is going to be hashed</param>
            /// <param name="format">Determines how return string will be formatted</param>
            /// <returns>String representation of hash</returns>
            public static string ComputeMD5(string plainText, StringFormat format)
            {
                return Compute(new MD5CryptoServiceProvider(), plainText, format);
            }

            /// <summary>
            /// Create a one-way 160bit (20 byte) hash of some text.  Suitable for file comparison and general security.  Not suitable for absolute non-repudiation tasks
            /// </summary>
            /// <param name="plainText">Text that is going to be hashed</param>
            /// <param name="format">Determines how return string will be formatted</param>
            /// <returns>String representation of hash</returns
            public static string ComputeSha1(string plainText, StringFormat format)
            {
                return Compute(new SHA1CryptoServiceProvider(), plainText, format);
            }
        }
    }
}
