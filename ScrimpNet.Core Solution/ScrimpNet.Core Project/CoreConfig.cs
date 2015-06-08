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
using ScrimpNet.Cryptography;
using ScrimpNet.Diagnostics;
using System.Reflection;
using ScrimpNet.Configuration;
using System.Configuration;
using ScrimpNet.Diagnostics;

namespace ScrimpNet
{
    /// <summary>
    /// Configuration settings for ScrimpNet library.  Note:  Some settings use direct calls to ConfigurationManager[] since they return
    /// bootstrapping values.  By default, always use ConfigManager instead.
    /// </summary>
    public partial class CoreConfig
    {
        private static KeyFile _keyFile;

        /// <summary>
        ///   Set's the active encryption key to be used the Encryption library.  Key and value
        ///   may contain any of the standard configuration meta data tags. Config Key: ScrimpNet.Encryption.KeyFile
        /// </summary>
        /// <remarks>
        /// Configuration entry may be one of:
        /// <para>Path (relative or fully qualified) to serialized key file including file name and extension</para>
        /// <para>Base64 encrypted serialized key file</para>
        /// <para>Null or empty or not provided - to use Encryption library's internal key file</para>
        /// </remarks>
        public static KeyFile EncryptionKey
        {
            get
            {
                if (_keyFile != null) return _keyFile;

                string configValue = ConfigManager.AppSetting<string>("ScrimpNet.Encryption.KeyFile", string.Empty);
                if (string.IsNullOrEmpty(configValue) == true)
                {
                    byte[] kfBytes = Convert.FromBase64String(CryptoUtils._internalKeyFile_Base64);
                    _keyFile = Serialize.From.Binary<KeyFile>(kfBytes);
                    return _keyFile;
                }

                return null;
            }
            set
            {
                _keyFile = value;
            }
        }


        static string _activeEnvironment = string.Empty;
        /// <summary>
        /// Active environment as configured in .config file (ScrimpNet.Application.Environment)
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
        /// Unique key for this application.  Often used in event logging and error handling (optional: ScrimpNet.Application.Key)
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
