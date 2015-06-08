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
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Formatters.Soap;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Xml.Serialization;
using ScrimpNet.Core.IO;

namespace ScrimpNet.Core
{
    /// <summary>
    /// Helper methods to serialize/deserialze objects
    /// </summary>
    public static partial class Serialize
    {
        /// <summary>
        /// Serialize objects into a string
        /// </summary>
        public static class To
        {
            /// <summary>
            /// Convert an object into byte array
            /// </summary>
            /// <param name="obj">Object that is being serialized</param>
            /// <returns>Byte array representation of object</returns>
            public static byte[] Binary(object obj)
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    BinaryFormatter bf1 = new BinaryFormatter();
                    bf1.Serialize(ms, obj.ToBytes());
                    return ms.ToArray();
                }
            }

            /// <summary>
            /// Convert an object into a JSON formatted string
            /// </summary>
            /// <param name="obj">Hydrated object to create</param>
            /// <returns>JSON string representation of an instance of an object</returns>
            public static string Json(object obj)
            {
                DataContractJsonSerializer ser = new DataContractJsonSerializer(obj.GetType());
                using (MemoryStream ms = new MemoryStream())
                {
                    ser.WriteObject(ms, obj);
                    return Encoding.Default.GetString(ms.ToArray());
                }
            } 

            /// <summary>
            /// Convert an object to XML using generic serializer
            /// </summary>
            /// <param name="obj">Object to serialize</param>
            /// <returns>Serialized object in XML format</returns>
            public static string Xml<T>(T obj)
            {
                string s = null;
                using (MemoryStream ms = new MemoryStream(1000))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(T));
                    XmlSerializerNamespaces xmlnsEmpty = new XmlSerializerNamespaces();
                    xmlnsEmpty.Add("", "");
                    serializer.Serialize(ms, obj, xmlnsEmpty);
                    ms.Seek(0, SeekOrigin.Begin);
                    using (StreamReader reader = new StreamReader(ms))
                    {
                        s = reader.ReadToEnd();
                    }
                    ms.Close();
                    serializer = null;
                }
                return s;
            }

            /// <summary>
            /// Creates a SoapXML format
            /// </summary>
            /// <param name="obj">Object to make into SOAP</param>
            /// <returns>String with SoapXML Format</returns>
            public static string Soap(object obj)
            {
                string s = null;
                using (MemoryStream ms = new MemoryStream(1000))
                {
                    SoapFormatter sf = new SoapFormatter(null, new StreamingContext(StreamingContextStates.Persistence));
                    sf.Serialize(ms, obj);
                    s = IOUtils.StreamToString(ms);
                }
                return s;
            }
            /// <summary>
            /// Convert an object to XML using generic serializer
            /// </summary>
            /// <param name="obj">Object to serialize</param>
            /// <returns>Serialized object in XML format</returns>
            public static string Xml(object obj)
            {
                string s = null;
                using (MemoryStream ms = new MemoryStream(1000))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(object));
                    serializer.Serialize(ms, obj);
                    ms.Seek(0, SeekOrigin.Begin);
                    using (StreamReader reader = new StreamReader(ms))
                    {
                        s = reader.ReadToEnd();
                    }
                    serializer = null;
                }
                return s;
            }
        }
    }
}
