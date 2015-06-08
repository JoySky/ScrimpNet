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
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

namespace ScrimpNet.Core.IO
{

    /// <summary>
    /// Classes that extend the System.IO namespace. 
    /// </summary>
    public static class IOUtils
    {
        /// <summary>
        /// Remove file and path reserved characters from string
        /// </summary>
        /// <param name="uncleanFileName">String that may contain reserved characters</param>
        /// <returns>String with any reserved characters removed</returns>
        public static string CleanFileName(string uncleanFileName)
        {
            string validFileName = uncleanFileName.Trim();

            foreach (char invalChar in Path.GetInvalidFileNameChars())
            {
                validFileName = validFileName.Replace(invalChar.ToString(), "");
            }
            foreach (char invalChar in Path.GetInvalidPathChars())
            {
                validFileName = validFileName.Replace(invalChar.ToString(), "");
            }

            if (validFileName.Length > 160) //safe value threshold is 260 but we only use 160
                validFileName = validFileName.Remove(159);

            return validFileName;
        }

        /// <summary>
        /// Get a default directory based on probing config file and/or executing assemblies.  Used primarily in library but may be used by any application
        /// </summary>
        public static string DefaultDirectory
        {
            get
            {
                try
                {
                    string _logFileName = ConfigurationManager.AppSettings["Application.LogFilename"];
                    if (string.IsNullOrEmpty(_logFileName) == false)
                    {
                        return Path.GetDirectoryName(_logFileName);
                    }

                    Assembly asm = Assembly.GetEntryAssembly();
                    if (asm == null)
                    {
                        asm = Assembly.GetCallingAssembly();
                    }
                    if (asm == null)
                    {
                        asm = Assembly.GetExecutingAssembly();
                    }
                    if (asm == null)
                    {
                        asm = Assembly.GetAssembly(typeof(Utils));
                    }

                    string fileRoot = string.Empty;
                    if (asm != null)
                    {
                        fileRoot = asm.Location.Replace("file:///", "").Replace("/", "\\");
                    }
                    if (string.IsNullOrEmpty(fileRoot) == true)
                    {
                        return Environment.CurrentDirectory;
                    }
                    else
                    {
                        return Path.GetDirectoryName(fileRoot);
                    }
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("Unable to determine default directory.  Check inner exception for details", ex);
                }

            }
        }
        /// <summary>
        /// Binary serialize an object to array of bytes.  This is an extension method so it will work on any object in your code after you have a 
        /// reference to ScrimpNet.Core
        /// </summary>
        /// <param name="obj">Object to serialize</param>
        /// <returns>Array of bytes of object or null if obj is null</returns>
        public static byte[] ToBytes(object obj)
        {

            if (obj == null) return null;
            MemoryStream ms = new MemoryStream(500);
            BinaryFormatter bf = new BinaryFormatter(null, new StreamingContext(StreamingContextStates.Clone));
            bf.Serialize(ms, obj);
            ms.Seek(0, SeekOrigin.Begin);
            return ms.ToArray();
        }

        /// <summary>
        /// Get an array of bytes from an operating system file
        /// </summary>
        /// <param name="filePath">Path and filename of file being read into byte array</param>
        /// <returns>Array of bytes</returns>
        public static Byte[] GetByteArrayFromFile(string filePath)
        {
            return File.ReadAllBytes(filePath);

        }

        /// <summary>
        /// Build a stream from a set of bytes
        /// </summary>
        /// <param name="sourceBytes">Bytes that will be the source of the stream</param>
        /// <returns>Stream</returns>
        public static Stream StreamFromBytes(byte[] sourceBytes)
        {
            return new MemoryStream(sourceBytes);
        }
        /// <summary>
        /// Creates a string (not Base64 encoded) from a stream
        /// </summary>
        /// <param name="stream">stream to make into string</param>
        /// <returns>String or null</returns>
        public static string StreamToString(Stream stream)
        {
            string s = null;
            stream.Seek(0, SeekOrigin.Begin);
            using (StreamReader reader = new StreamReader(stream))
            {
                s = reader.ReadToEnd();
            }
            return s;
        }
    }
}


