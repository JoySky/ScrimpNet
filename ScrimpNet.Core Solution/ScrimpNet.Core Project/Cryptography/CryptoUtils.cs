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
using ScrimpNet.Text;
using ScrimpNet.Web;


namespace ScrimpNet.Cryptography
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
    /// ScrimpNet data encryption routines. Uses RijndaelManaged as default provider
    /// </summary>
    public static partial class CryptoUtils
    {
        internal static string _internalKeyFile_Base64 = "AAEAAAD/////AQAAAAAAAAAMAgAAAEdTY3JpbXBOZXQuQ29yZSwgVmVyc2lvbj0xMC41LjE0LjAsIEN1bHR1cmU9bmV1dHJhbCwgUHVibGljS2V5VG9rZW49bnVsbAUBAAAAI1NjcmltcE5ldC5Db3JlLkNyeXB0b2dyYXBoeS5LZXlGaWxlBAAAAARfa2V5A19pdgpfdmFsaWRGcm9tCF92YWxpZFRvBwcAAAICDQ0CAAAACQMAAAAJBAAAAChSPGIX48xInGPgj+rsPEkPAwAAACAAAAACiRPl9PioimiXxgWOj03qJUjYICP+lzzJGUfJPzluIfUPBAAAACAAAAACV6FQtTCGo8yXbY7eFodEHg5g5YSRek+sjEgDQroLRC8L";
        
        /// <summary>
        /// Currently active key.  Default is _internalKeyFile_Base64 as defined in CryptoUtils class
        /// </summary>
        public static KeyFile ActiveKey
        {
            get
            {
                return CoreConfig.EncryptionKey;
            }
            set
            {
                CoreConfig.EncryptionKey = value;
            }
        }

        
        /// <summary>
        /// Encrypts a string using values set in .config file.  Application.AESPassword, Application.AESKey1, Application.AESKey2
        /// </summary>
        /// <param name="value">Plain text string that is to be encrypted</param>
        /// <returns>Encrypted Base64 encoded string or empty string if error or value is null or empty</returns>
        public static String Encrypt(string value)
        {
            return Encrypt(value,StringFormat.Base64);
        }

        /// <summary>
        /// Encrypts a string using values set in .config file.  Application.AESPassword, Application.AESKey1, Application.AESKey2
        /// </summary>
        /// <param name="value">Plain text string that is to be encrypted</param>
        /// <param name="format">Determines how the data in return string is represented</param>
        /// <returns>Encrypted Base64 encoded string or empty string if error or value is null or empty</returns>
        public static String Encrypt(string value, StringFormat format)
        {
            return Encrypt(value, ActiveKey.Key, ActiveKey.IV,format);
        }

        /// <summary>
        /// Decrypt a string using values set in .config file.  Application.AESPassword, Application.AESKey1, Application.AESKey2
        /// </summary>
        /// <param name="value">Plain text string that is to be encrypted</param>
        /// <returns>Encrypted Base64 encoded string or empty string if error or value is null or empty</returns>
        public static String Decrypt(string value)
        {
            return Decrypt(value,StringFormat.Base64);
        }

        /// <summary>
        /// Decrypt a string using values set in .config file.  Application.AESPassword, Application.AESKey1, Application.AESKey2
        /// </summary>
        /// <param name="value">Plain text string that is to be encrypted</param>
        /// <param name="format">Determines how value has been encoded by encryption</param>
        /// <returns>Encrypted Base64 encoded string or empty string if error or value is null or empty</returns>
        public static String Decrypt(string value,StringFormat format)
        {
            return Decrypt(value, ActiveKey.Key, ActiveKey.IV,format);
        }

        /// <summary>
        /// Encrypt a string using a specific key
        /// </summary>
        /// <param name="plainText">Text to encrypt</param>
        /// <param name="format">How the encrypted text is representing the text.  NOTE: <paramref name="EncryptedText"/> and return formats are the same</param>
        /// <param name="encryptionKey">Base64 encoded encryption key. Value returned from AES.GenerateKey().  Must be 126, 128, or 256 bytes</param>
        /// <param name="iv">Base64 encoded insertion key.  Value returned from AES.GenerateIV().  16 bytes</param>
        /// <returns>Encrypted string or empty string if error</returns>
        public static string Encrypt(string plainText, string encryptionKey, string iv,StringFormat format)
        {
            return Encrypt(plainText, Convert.FromBase64String(encryptionKey), Convert.FromBase64String(iv),format);
        }


        /// <summary>
        /// Encrypt a string using a specific key
        /// </summary>
        /// <param name="plainText">Text to encrypt</param>
        /// <param name="format">How the encrypted text is representing the text.  NOTE: <paramref name="EncryptedText"/> and return formats are the same</param>
        /// <param name="encryptionKey">Encryption key. Must be 126, 128, or 256 bits (16, 24, 32 bytes)</param>
        /// <param name="iv">Insertion vector.  16 bytes</param>
        /// <returns>Encrypted Base64 string or empty string if error</returns>
        public static string Encrypt(string plainText, byte[] encryptionKey, byte[] iv,StringFormat format)
        {
            if (string.IsNullOrEmpty(plainText) == true) return "";
            byte[] plainBytes = ToBytes(plainText, format);
            return FromBytes(Encrypt(plainBytes,encryptionKey,iv),format);
        }

        /// <summary>
        /// Encrypt a data block with a specific key file
        /// </summary>
        /// <param name="plainBytes">Unencrypted bytes</param>
        /// <param name="key">Key file to use for encryption</param>
        /// <returns>Encrypted set of bytes</returns>
        public static byte[] Encrypt(byte[] plainBytes, KeyFile key)
        {
            return Encrypt(plainBytes, key.Key, key.IV);
        }

        /// <summary>
        /// Encrypt an array of bytes using key and insertion vector (also called block size).  NOTE: This is the base
        /// method for encryption methods.  Other encryption methods are convenience overloads
        /// </summary>
        /// <param name="plainBytes">Non-null,non-zero length array of bytes to encrypt</param>
        /// <param name="keyBytes">Key for encryption algorithm. Must be 126, 128, or 256 bits (16, 24, 32 bytes) </param>
        /// <param name="ivBytes">Insertion vector (also called block size). Must be 126, 128, or 256 bits (16, 24, 32 bytes)</param>
        /// <returns>An encrypted byte array</returns>
        [Citation("http://www.codeproject.com/KB/security/DotNetCrypto.aspx", Notes = "Incorporated excellent comments")]
        public static byte[] Encrypt(byte[] plainBytes, byte[] keyBytes, byte[] ivBytes)
        {
            // Create a MemoryStream to accept the encrypted bytes 
            using (MemoryStream ms = new MemoryStream())
            {

                // Create a symmetric algorithm. 
                // We are going to use Rijndael because it is strong and
                // available on all platforms. 
                // You can use other algorithms, to do so substitute the
                // next line with something like 
                //      TripleDES alg = TripleDES.Create(); 
                SymmetricAlgorithm encryptor = null;

                try
                {
                    encryptor = new RijndaelManaged();

                    // explicitly set block sizes since
                    // keys may not be default lengths
                    // Legal values are:
                    //      Key Size:   128, 192, 256
                    //      Block Size: 128, 192, 256
                    //      Bytes        16,  24,  32
                    encryptor.BlockSize = ivBytes.Length * 8;
                    encryptor.KeySize = keyBytes.Length * 8;

                    // Now set the key and the IV. 
                    // We need the IV (Initialization Vector) because
                    // the algorithm is operating in its default 
                    // mode called CBC (Cipher Block Chaining).
                    // The IV is XORed with the first block (8 byte) 
                    // of the data before it is encrypted, and then each
                    // encrypted block is XORed with the 
                    // following block of plaintext.
                    // This is done to make encryption more secure. 

                    // There is also a mode called ECB which does not need an IV,
                    // but it is much less secure. 

                    encryptor.Key = keyBytes;
                    encryptor.IV = ivBytes;

                    // Create a CryptoStream through which we are going to be
                    // pumping our data. 
                    // CryptoStreamMode.Write means that we are going to be
                    // writing data to the stream and the output will be written
                    // in the MemoryStream we have provided. 
                    using (CryptoStream cs = new CryptoStream(ms,
                       encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {

                        // Write the data and make it do the encryption 
                        cs.Write(plainBytes, 0, plainBytes.Length);

                        // Close the crypto stream (or do FlushFinalBlock). 
                        // This will tell it that we have done our encryption and
                        // there is no more data coming in, 
                        // and it is now a good time to apply the padding and
                        // finalize the encryption process. 
                        cs.Close();

                        // Now get the encrypted data from the MemoryStream.
                        // Some people make a mistake of using GetBuffer() here,
                        // which is not the right way. 
                        ms.Flush();
                        byte[] encryptedData = ms.ToArray();
                        ms.Close();
                        return encryptedData;
                    }
                }
                finally
                {
                    if (encryptor != null)
                    {
                        encryptor.Clear();
                    }
                }
            } //using memory stream
        }

        /// <summary>
        /// Decrypt a data block using a specified key
        /// </summary>
        /// <param name="encryptedBytes">Encrypted data block to be decrypted</param>
        /// <param name="key">Key used for decrypting</param>
        /// <returns>Unencrypted bytes</returns>
        public static byte[] Decrypt(byte[] encryptedBytes, KeyFile key)
        {
            return Decrypt(encryptedBytes, key.Key, key.IV);
        }

        /// <summary>
        /// Decrypt a string using a specific key
        /// </summary>
        /// <param name="plainText">Text to decrypt</param>
        /// <param name="format">How the encrypted text is representing the text.  NOTE: <paramref name="EncryptedText"/> and return formats are the same</param>
        /// <param name="encryptionKey">Base64 encoded encryption key. Value returned from AES.GenerateKey().  Must be 126, 128, or 256 bytes</param>
        /// <param name="iv">Base64 encoded insertion key.  Value returned from AES.GenerateIV().  16 bytes</param>
        /// <returns>Decrypted string or empty string if error</returns>
        public static string Decrypt(string plainText, string encryptionKey, string iv, StringFormat format)
        {
            return Decrypt(plainText, Convert.FromBase64String(encryptionKey), Convert.FromBase64String(iv),format);
        }


        /// <summary>
        /// Decrypt a string using a specific key
        /// </summary>
        /// <param name="encryptedText">encrypted text to be decrypted</param>
        /// <param name="format">How the encrypted text is representing the text.  NOTE: <paramref name="EncryptedText"/> and return formats are the same</param>
        /// <param name="keyBytes">Key for encryption algorithm. Must be 126, 128, or 256 bits (16, 24, 32 bytes) </param>
        /// <param name="ivBytes">Insertion vector (also called block size). Must be 126, 128, or 256 bits (16, 24, 32 bytes)</param>
        /// <returns>Decrypted string or empty string if error</returns>
        public static string Decrypt(string encryptedText, byte[] keyBytes, byte[] ivBytes, StringFormat format)
        {
            byte[] encryptedBytes = ToBytes(encryptedText,format);
            byte[] plainBytes = Decrypt(keyBytes, encryptedBytes, ivBytes);
            return FromBytes(plainBytes, format);
        }

        /// <summary>
        ///Decrypt an array of bytes using key and insertion vector (also called block size).  NOTE: This is the base
        /// method for decryption methods.  Other decryption methods are convenience overloads
        /// </summary>
        /// <param name="encryptedBytes">Non-null,non-zero length array of bytes to decrypt</param>
        /// <param name="keyBytes">Key for encryption algorithm. Must be 126, 128, or 256 bits (16, 24, 32 bytes) </param>
        /// <param name="ivBytes">Insertion vector (also called block size). Must be 126, 128, or 256 bits (16, 24, 32 bytes)</param>
        /// <returns>An encrypted byte array</returns>
        public static byte[] Decrypt(byte[] encryptedBytes,byte[] keyBytes, byte[] ivBytes)
        {
            // Create a MemoryStream that is going to accept the
            // decrypted bytes 
            using (MemoryStream ms = new MemoryStream())
            {

                // Create a symmetric algorithm. 
                // We are going to use Rijndael because it is strong and
                // available on all platforms. 
                // You can use other algorithms, to do so substitute the next
                // line with something like 
                //     TripleDES alg = TripleDES.Create(); 
                SymmetricAlgorithm encryptor = null;
                try
                {
                    encryptor = new RijndaelManaged();


                    // explicitly set block sizes since
                    // keys may not be default lengths
                    // Legal values are:
                    //      Key Size:   128, 192, 256
                    //      Block Size: 128, 192, 256
                    //      Bytes        16,  24,  32
                    encryptor.BlockSize = ivBytes.Length * 8;
                    encryptor.KeySize = keyBytes.Length * 8;

                    // Now set the key and the IV. 
                    // We need the IV (Initialization Vector) because the algorithm
                    // is operating in its default 
                    // mode called CBC (Cipher Block Chaining). The IV is XORed with
                    // the first block (8 byte) 
                    // of the data after it is decrypted, and then each decrypted
                    // block is XORed with the previous 
                    // cipher block. This is done to make encryption more secure. 
                    // There is also a mode called ECB which does not need an IV,
                    // but it is much less secure. 
                    encryptor.Key = keyBytes;
                    encryptor.IV = ivBytes;

                    // Create a CryptoStream through which we are going to be
                    // pumping our data. 
                    // CryptoStreamMode.Write means that we are going to be
                    // writing data to the stream 
                    // and the output will be written in the MemoryStream
                    // we have provided. 
                    using (CryptoStream cs = new CryptoStream(ms,
                        encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {

                        // Write the data and make it do the decryption 
                        cs.Write(encryptedBytes, 0, encryptedBytes.Length);

                        // Close the crypto stream (or do FlushFinalBlock). 
                        // This will tell it that we have done our decryption
                        // and there is no more data coming in, 
                        // and it is now a good time to remove the padding
                        // and finalize the decryption process. 
                        cs.Close();

                        // Now get the decrypted data from the MemoryStream. 
                        // Some people make a mistake of using GetBuffer() here,
                        // which is not the right way. 
                        byte[] decryptedData = ms.ToArray();

                        return decryptedData;
                    }
                }
                finally
                {
                    if (encryptor != null)
                    {
                        encryptor.Clear(); //remove any resources this might have
                    }
                }
            } //using memory stream
        }

        private static byte[] ToBytes(string value, StringFormat inputFormat)
        {
            switch (inputFormat)
            {
                case StringFormat.Base64:
                    return Convert.FromBase64String(value);
                case StringFormat.Hex:
                    return Transform.HexStringToBytes(value);
                case StringFormat.Unicode:
                    return Transform.StringToBytes(value);
                default:
                    throw ExceptionFactory.New<InvalidOperationException>("'{0}' is an unknown string format", inputFormat.ToString());
            }
        }

       private static string FromBytes(byte[] source, StringFormat outputFormat)
        {
            switch (outputFormat)
            {
                case StringFormat.Base64:
                    return Convert.ToBase64String(source);
                case StringFormat.Hex:
                    return Transform.BytesToHexString(source);
                case  StringFormat.Unicode:
                    return Transform.StringFromBytes(source);
                default:
                    throw ExceptionFactory.New<InvalidOperationException>("'{0}' is an unknown string format", outputFormat.ToString());
            }
        }
        /// <summary>
        /// Use this method to generate internal static key file for private field: _internalKeyFile_Base64  Generally called from debugger and values cut/pasted into this class
        /// </summary>
        /// <param name="longevity">How long from this instant in time that this key file should remain effective</param>
        /// <returns>A string of code suitable for pasting into this class or .config file.  NOTE: The results of this call are NOT encrypted</returns>
        public static string GenerateInternalKeys_Base64Unencrypted(TimeSpan longevity)
        {
            KeyFile kf = KeyFile.New(DateTime.UtcNow, DateTime.UtcNow+longevity);
            byte[] kfBytes = Serialize.To.Binary(kf);
            return Convert.ToBase64String(kfBytes);
        }

        private static char[] punctuations = "!@#$%^&*()_-+=[{]};:>|./?".ToCharArray();
        /// <summary>
        /// Generate a random password of a given length
        /// </summary>
        /// <param name="length">Total length of password to generate</param>
        /// <param name="numberOfNonAlphanumericCharacters">Number of non-alpha numeric characters to use</param>
        /// <returns>Generated password</returns>
        [Citation(".Net Framework Library: System.Web.Security")]
        public static string GeneratePassword(int length, int numberOfNonAlphanumericCharacters)
        {
            string str;
            int num;

            do
            {
                byte[] data = new byte[length];
                char[] chArray = new char[length];
                int num2 = 0;
                new RNGCryptoServiceProvider().GetBytes(data);
                for (int i = 0; i < length; i++)
                {
                    int num4 = data[i] % 0x57;
                    if (num4 < 10)
                    {
                        chArray[i] = (char)(0x30 + num4);
                    }
                    else if (num4 < 0x24)
                    {
                        chArray[i] = (char)((0x41 + num4) - 10);
                    }
                    else if (num4 < 0x3e)
                    {
                        chArray[i] = (char)((0x61 + num4) - 0x24);
                    }
                    else
                    {
                        chArray[i] = punctuations[num4 - 0x3e];
                        num2++;
                    }
                }
                if (num2 < numberOfNonAlphanumericCharacters)
                {
                    Random random = new Random();
                    for (int j = 0; j < (numberOfNonAlphanumericCharacters - num2); j++)
                    {
                        int num6;
                        do
                        {
                            num6 = random.Next(0, length);
                        }
                        while (!char.IsLetterOrDigit(chArray[num6]));
                        chArray[num6] = punctuations[random.Next(0, punctuations.Length)];
                    }
                }
                str = new string(chArray);
            }
            while (CrossSiteScriptingValidation.IsDangerousString(str, out num));
            return str;
        }
    } //class
} // namespace
