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
using ScrimpNet.Core.Configuration;
using ScrimpNet.Core.Cryptography;
using ScrimpNet.Core.Diagnostics;
using System.Configuration;
using System.Reflection;

namespace ScrimpNet.Core
{
    public partial class CoreConfig
    {
        /// <summary>
        /// Base64 encoded encryption key.  Config Key: ScrimpNet.Encryption.Key
        /// </summary>
        public static byte[] EncryptionKey
        {
            get
            {
                string configValue = ConfigUtils.AppSetting<string>("ScrimpNet.Encryption.Key",string.Empty);
                if (string.IsNullOrEmpty(configValue) == true)
                {
                    //if (probeForKeyFile() == false)
                    //{
                        return CryptoUtils._staticKey;
                    //}

                }
                else
                {
                    return Convert.FromBase64String(configValue);
                }
               // return null;
            }
        }

        /// <summary>
        /// Base64 encoded encryption insertion vector.  Config Key: ScrimpNet.Encryption.Iv
        /// </summary>
        public static byte[] EncryptionIv
        {
            get
            {
                string configValue = ConfigUtils.AppSetting<string>("ScrimpNet.Encryption.Iv", string.Empty);
                if (string.IsNullOrEmpty(configValue) == true)
                {
                    //if (probeForKeyFile() == false)
                    //{
                        return CryptoUtils._staticIv;
                    //}
                    //throw new ConfigurationErrorsException("");
                }
                else
                {
                    return Convert.FromBase64String(configValue);
                }
            }
        }

        //private static string encryptionKeyFileName
        //{
        //    get
        //    {
        //        return ConfigUtils.AppSetting<string>("ScrimpNet.Encryption.KeyFile", string.Empty);
        //    }
        //}
        //private static byte[] encryptionKeyFileKey
        //{
        //    get
        //    {
        //        string configValue = ConfigUtils.AppSetting<string>("ScrimpNet.Encryption.KeyFileKey", string.Empty);
        //        if (string.IsNullOrEmpty(configValue) == true)
        //        {
        //            return CryptoUtils._staticKey;
        //        }
        //        return Convert.FromBase64String(configValue);
        //    }
        //}


        //private static byte[] encryptionKeyFileIv
        //{
        //    get
        //    {
        //        string configValue = ConfigUtils.AppSetting<string>("ScrimpNet.Encryption.KeyFileIv",string.Empty);
        //        if (string.IsNullOrEmpty(configValue) == true)
        //        {
        //            return CryptoUtils._staticIv;
        //        }
        //        return Convert.FromBase64String(configValue);
        //    }
        //}

        //private static bool probeForKeyFile()
        //{
        //    try
        //    {
        //        SNetKeySet keyFile = SNetKeySet.LoadFromFile(encryptionKeyFileName, encryptionKeyFileKey, encryptionKeyFileIv);
        //        if (keyFile == null) return false; //no keyfile found
        //        _encryptionIv = keyFile.Iv;
        //        _encryptionKey = keyFile.Key;
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        ScrimpNet.Core.Diagnostics.Log.LastChanceLog(ex);
        //        throw;
        //    }
        //}

        static string _activeEnvironment = string.Empty;
        /// <summary>
        /// Active environment as configured in .config file
        /// </summary>
        public static string ActiveEnvironment
        {
            get
            {
                if (string.IsNullOrEmpty(_activeEnvironment) == false)
                {
                    return _activeEnvironment;
                }
                _activeEnvironment = ConfigurationManager.AppSettings["ScrimpNet.Application.Environment"];
                
                if (string.IsNullOrEmpty(_activeEnvironment) == true)
                {
                    _activeEnvironment = System.Environment.MachineName;
                }
                
            

                return _activeEnvironment;
            }
        }

        private static string _applicationKey;
        /// <summary>
        /// Unique key for this application.  Often used in event logging and error handling
        /// </summary>
        public static string ApplicationKey
        {
            get
            {
                if (string.IsNullOrEmpty(_applicationKey) == false)
                {
                    return _applicationKey;
                }

                _applicationKey = ConfigurationManager.AppSettings["ScrimpNet.Application.Key"];
                if (string.IsNullOrEmpty(_applicationKey) == true)
                {
                    Assembly asm = Assembly.GetEntryAssembly();
                    if (asm == null)
                    {
                        asm = Assembly.GetCallingAssembly();
                    }
                    if (asm == null)
                    {
                        asm = Assembly.GetExecutingAssembly();
                    }
                    _applicationKey = asm.GetName().Name;
                }
                return _applicationKey;
            }
        }

 
    }
}
