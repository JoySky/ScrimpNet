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
using System.Runtime.Serialization;

namespace ScrimpNet.Configuration
{
    /// <summary>
    /// A setting for a specific environment (prod, dev, etc) or machine
    /// </summary>
    [DataContract]
    [Serializable]
    public class ConfigVariant
    {
        /// <summary>
        /// Unique identifier of this version
        /// </summary>
        [DataMember]
        public Guid VariantId { get; set; }

        /// <summary>
        /// All settings in ScrimpNet configuration subsystem has a common identifier.
        /// Often used for setting comparison and merging between environments
        /// </summary>
        [DataMember]
        public Guid GlobalSettingId { get; set; }

        /// <summary>
        /// Specifies specific instance of VariantName.  (e.g. 'localHost','production')
        /// </summary>
        [DataMember]
        public string VariantQualifier { get; set; }

        /// <summary>
        /// Configuration value to return
        /// </summary>
        [DataMember]
        public string VariantValue { get; set; }

        public override string ToString()
        {
            return string.Format("ConfigVariants[{1}]='{2}'",
               VariantQualifier, VariantValue);
        }
    }
}
