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
using System.Xml.Serialization;
using System.IO;
using System.Runtime.Serialization.Formatters.Soap;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization.Formatters.Binary;
using ScrimpNet.Core.IO;

namespace ScrimpNet.Core
{
    public static partial class Serialize
    {
        /// <summary>
        /// Rehydrate objects from string representation
        /// </summary>
        public static class From
        {
            /// <summary>
            /// Create a new object based on input JSON string
            /// </summary>
            /// <typeparam name="T">Type to convert string into</typeparam>
            /// <param name="json">Hydrated object in JSON format</param>
            /// <returns>newly created object</returns>
            public static T Json<T>(string json)
            {

                using (MemoryStream ms = new MemoryStream(ASCIIEncoding.Default.GetBytes(json)))
                {
                    DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
                    return (T)ser.ReadObject(ms);

                }
            }

            /// <summary>
            /// Create a new object based on input byte array
            /// </summary>
            /// <typeparam name="T">Type to convert byte array to</typeparam>
            /// <param name="binaryObject">Hydrated object in binary format</param>
            /// <returns>Newly created object</returns>
            public static T Binary<T>(byte[] binaryObject)
            {
                IFormatter formatter = new BinaryFormatter();               
                using (Stream stream = IOUtils.StreamFromBytes(binaryObject))
                {
                    T retVal = (T)formatter.Deserialize(stream);
                    stream.Close();
                    return retVal;
                }
            }

            /// <summary>
            /// 
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="serializedObject"></param>
            /// <returns></returns>
            public static T Xml<T>(string serializedObject)
            {
                XmlSerializerNamespaces xmlnsEmpty = new XmlSerializerNamespaces();
                xmlnsEmpty.Add("", "");
                XmlSerializer xs = new XmlSerializer(typeof(T));

                using (MemoryStream memoryStream = new MemoryStream(Transform.StringToUTF8ByteArray(serializedObject)))
                {
                    return (T)xs.Deserialize(memoryStream);
                }
            }

            /// <summary>
            /// Reconstruct object from XML (SOAP format)
            /// </summary>
            /// <param name="soapXml">source string</param>
            /// <returns>Constructed object or null if not found</returns>
            public static object Soap(string soapXml)
            {
                object obj = null;
                using (MemoryStream ms = new MemoryStream((new System.Text.ASCIIEncoding()).GetBytes(soapXml)))
                {
                    ms.Seek(0, SeekOrigin.Begin);
                    SoapFormatter sf = new SoapFormatter(null, new StreamingContext(StreamingContextStates.Persistence));
                    obj = sf.Deserialize(ms);
                }
                return obj;
            }
        }
    }
}
