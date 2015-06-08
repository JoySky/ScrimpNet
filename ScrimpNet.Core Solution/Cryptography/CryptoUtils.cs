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
//----------------------------------------------------------
// .net libraries
//----------------------------------------------------------
using System;
using System.Security;
using System.IO;
using System.Security.Cryptography;
using System.Text;


namespace ScrimpNet.Core.Cryptography
{
	//================================================================
	// byte[] EncryptByteArray(byte[] bArray)
	// byte[] EncryptByteArray(byte[] bArray, byte[] encryptKey)
	// byte[] EncryptByteArray(byte[] bArray, string encryptKey)
	// byte[] EncryptByteArray(string plainText, string encryptKey)
	// byte[] EncryptByteArray(string plainText)
	// byte[] EncryptByteArray(string plainText,byte[] encryptKey)
	//
	// string EncryptString(string plainText)
	// string EncryptString(string plainText,string encryptKey)
	// string EncryptString(string plainText,byte[] encryptKey)
	//
	//
	// byte[] DecryptByteArray(byte[] encryptedBytes,byte[] encryptKey)
	// byte[] DecryptByteArray(byte[] encryptedBytes)
    //
	// string DecryptString(string encryptBase64String,string encryptKey)
	// string DecryptString(string encryptBase64String)

	//================================================================
	/// <summary>
	/// ScrimpNet data encryption routines.  All methods are static
	/// </summary>
    public static class CryptoUtils
	{

        internal static byte[] _staticKey = new byte[]  {103,167,82,200,175,142,225,14,199,84,152,176,243,108,150,129,122,196,150,43,97,36,52,246,165,210,220,5,189,78,35,144};
        internal static byte[] _staticIv = new byte[] { 70, 94, 170, 226, 97, 175, 116, 97, 125, 186, 65, 48, 225, 61, 18, 163 };


        private static byte[] _activeKey = _staticKey.Clone() as byte[];
        private static byte[] _activeIv = _staticIv.Clone() as byte[];

        static CryptoUtils()
        {
            
        }

        /// <summary>
        /// Currently active key.  Default is _staticKey as defined in CryptoUtils class
        /// </summary>
        public static byte[] ActiveKey
        {
            get
            {
                return CoreConfig.EncryptionKey;
            }
            set
            {
                _activeKey = value;
            }
        }

        /// <summary>
        /// Currently active IV.  Default is _staticIv as defined in CryptoUtils class
        /// </summary>
        public static byte[] ActiveIv
        {
            get
            {
                return CoreConfig.EncryptionIv;
            }
            set
            {
                _activeIv = value;
            }
        }

        /// <summary>
        /// Generate a random key
        /// </summary>
        /// <returns>Base64 encoded key</returns>
        public static string GenerateKey()
        {
            RijndaelManaged encryptor = new RijndaelManaged();
            encryptor.GenerateKey();
            byte[] key = encryptor.Key;
            return Convert.ToBase64String(key);
        }

        /// <summary>
        /// Generate a random insertion vector
        /// </summary>
        /// <returns>Base64 representation of insertion vector</returns>
        public static string GenerateIV()
        {
            RijndaelManaged encryptor = new RijndaelManaged();
            encryptor.GenerateIV();
            byte[] iv = encryptor.IV;
            return Convert.ToBase64String(iv);
        }

        /// <summary>
        /// Encrypts a string using values set in .config file.  Application.AESPassword, Application.AESKey1, Application.AESKey2
        /// </summary>
        /// <param name="value">Plain text string that is to be encrypted</param>
        /// <returns>Encrypted Base64 encoded string or empty string if error or value is null or empty</returns>
        public static String Encrypt(string value)
        {
            return Encrypt(value,ActiveKey, ActiveIv);
        }

        /// <summary>
        /// Decrypt a string using values set in .config file.  Application.AESPassword, Application.AESKey1, Application.AESKey2
        /// </summary>
        /// <param name="value">Plain text string that is to be encrypted</param>
        /// <returns>Encrypted Base64 encoded string or empty string if error or value is null or empty</returns>
        public static String Decrypt(string value)
        {
            return Decrypt(value, ActiveKey, ActiveIv);
        }

        /// <summary>
        /// Encrypt a string using a specific key
        /// </summary>
        /// <param name="plainText">Text to encrypt</param>
        /// <param name="encryptionKey">Base64 encoded encryption key. Value returned from AES.GenerateKey().  Must be 126, 128, or 256 bytes</param>
        /// <param name="iv">Base64 encoded insertion key.  Value returned from AES.GenerateIV().  16 bytes</param>
        /// <returns>Encrypted string or empty string if error</returns>
        public static string Encrypt(string plainText, string encryptionKey, string iv)
        {
            return Encrypt(plainText, Convert.FromBase64String(encryptionKey), Convert.FromBase64String(iv));
        }

        /// <summary>
        /// Encrypt a string using a specific key
        /// </summary>
        /// <param name="plainText">Text to encrypt</param>
        /// <param name="encryptionKey">Encryption key. Must be 126, 128, or 256 bytes</param>
        /// <param name="iv">Insertion vector.  16 bytes</param>
        /// <returns>Encrypted string or empty string if error</returns>
        public static string Encrypt(string plainText, byte[] encryptionKey, byte[] iv)
        {
            if (string.IsNullOrEmpty(plainText) == true) return "";
            try
            {
                RijndaelManaged encryptor = new RijndaelManaged();
                using (MemoryStream ms = new MemoryStream())
                {
                    using (ICryptoTransform transform = encryptor.CreateEncryptor(encryptionKey, iv))
                    {
                        using (CryptoStream cs = new CryptoStream(ms, transform, CryptoStreamMode.Write))
                        {

                            using (StreamWriter sw = new StreamWriter(cs))
                            {
                                sw.Write(plainText, 0, plainText.Length);
                                sw.Flush();
                                cs.FlushFinalBlock();
                                ms.Flush();

                                //convert back to a string
                                return Convert.ToBase64String(ms.GetBuffer(), 0, Convert.ToInt32(ms.Length));
                            }

                        }
                    }
                }
            }
            catch (Exception) //ignore errors
            {
                return string.Empty;
            }
        }


        /// <summary>
        /// Decrypt a string using a specific key
        /// </summary>
        /// <param name="plainText">Text to decrypt</param>
        /// <param name="encryptionKey">Base64 encoded encryption key. Value returned from AES.GenerateKey().  Must be 126, 128, or 256 bytes</param>
        /// <param name="iv">Base64 encoded insertion key.  Value returned from AES.GenerateIV().  16 bytes</param>
        /// <returns>Decrypted string or empty string if error</returns>
        public static string Decrypt(string plainText, string encryptionKey, string iv)
        {
            return Decrypt(plainText, Convert.FromBase64String(encryptionKey), Convert.FromBase64String(iv));
        }


        /// <summary>
        /// Decrypt a string using a specific key
        /// </summary>
        /// <param name="plainText">encrypted base64 text</param>
        /// <param name="encryptionKey">Encryption key. Must be 126, 128, or 256 bytes</param>
        /// <param name="iv">Insertion vector.  16 bytes</param>
        /// <returns>Decrypted string or empty string if error</returns>
        public static string Decrypt(string plainText, byte[] encryptionKey, byte[] iv)
        {
            if (String.IsNullOrEmpty(plainText) == true) return "";
            try
            {
                RijndaelManaged decryptor = new RijndaelManaged();
                byte[] buffer = Convert.FromBase64String(plainText);
                using (ICryptoTransform transform = decryptor.CreateDecryptor(encryptionKey, iv))
                {
                    using (MemoryStream ms = new MemoryStream(buffer))
                    {
                        using (CryptoStream cs = new CryptoStream(ms, transform, CryptoStreamMode.Read))
                        {
                            using (StreamReader sr = new StreamReader(cs))
                            {
                                return sr.ReadToEnd();
                            }
                        }
                    }
                }
            }
            catch
            {
                return "";
            }
        }
        /// <summary>
        /// Create a one-way 512 hash of some text
        /// </summary>
        /// <param name="plainText">Clear text to hash</param>
        /// <returns>Base64 Hashed results</returns>
        public static string Hash(string plainText)
        {
            return Hash(plainText, true);
        }
        /// <summary>
        /// Create a one-way 512 hash of some text
        /// </summary>
        /// <param name="plainText">Clear text to hash</param>
        /// <param name="makeBase64">if True then return a Base64 encoded string (for .config files,emails, etc)</param>
        /// <returns>Base64 Hashed results</returns>
        public static string Hash(string plainText,bool makeBase64)
        {
            SHA512Managed sha = new SHA512Managed();
            byte[] result = sha.ComputeHash(UnicodeEncoding.Unicode.GetBytes(plainText));
            if (makeBase64 == true)
            {
               return Convert.ToBase64String(result);
            }
            else
            {
                return UnicodeEncoding.Unicode.GetString(result);
            }
    
        }

	} //class
} // namespace
