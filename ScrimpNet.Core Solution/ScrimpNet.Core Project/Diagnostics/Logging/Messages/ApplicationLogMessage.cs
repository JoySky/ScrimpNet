/*
// ScrimpNet.Core Library
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
using ScrimpNet.Text;

namespace ScrimpNet.Diagnostics
{
    /// <summary>
    /// Defines a single message to be persisted into log.  This class is used in application logging scenarios
    /// </summary>
    [Serializable]
    public partial class ApplicationLogMessage : LogMessageBase
    {
        #region [Constructor(s)]

        /// <summary>
        /// Default constructor
        /// </summary>
        public ApplicationLogMessage()
            : base()
        {
            this.LogMessageClass = LogMessageClass.ApplicationLogging;
        }

        /// <summary>
        /// Create a LogMessage with user defined text
        /// </summary>
        /// <param name="messageNumber">Numerical identifier of this message (e.g. '101', '34334')</param>
        /// <param name="messageText">Textual portion of message being persisted.  May contain string.format({0}) place holders</param>
        /// <param name="args">Arguments to be supplied to <paramref name="messageText"/>></param>
        public ApplicationLogMessage(int messageNumber, string messageText, params object[] args)
            : this()
        {
            MessageText = TextUtils.StringFormat(messageText, args);
            MessageNumber = messageNumber;
        }

        /// <summary>
        /// Create a LogMessage with user defined text
        /// </summary>
        /// <param name="messageText">Textual portion of message being persisted.  May contain string.format({0}) place holders</param>
        /// <param name="args">Arguments to be supplied to <paramref name="messageText"/></param>
        public ApplicationLogMessage(string messageText, params object[] args)
            : this(0, messageText, args)
        {
        }

        #endregion

        /// <summary>
        /// Unique identifier for this message.  Used when comparing messages in different message sinks
        /// </summary>
        public int MessageId { get; set; }




        private int _messageNumber = 0;
        /// <summary>
        /// Identifier of this message (e.g. '101', '34334')
        /// </summary>
        public int MessageNumber { get { return _messageNumber; } set { _messageNumber = value; } }

        private string _title = string.Empty;
        /// <summary>
        /// Short text of message. Also considered "subject" or "label" depending on sending via email or via msmq respectively.  If not explicitly set, 
        /// title will attempt to build return value from set values
        /// </summary>
        public virtual string Title
        {
            get
            {
                if (string.IsNullOrEmpty(_title) == false) return _title;
                _title = TextUtils.StringFormat("{0} {1}({2}) {3:0000} ",
                        this.ApplicationKey,
                        this.LogLevel.ToString(),
                        this.Priority.ToString(),
                        this.MessageNumber
                        );
                if (string.IsNullOrEmpty(MessageText) == false)
                {
                    string shortText = MessageText;
                    if (string.IsNullOrEmpty(shortText) == false)
                    {
                        if (shortText.Length > 60)
                        {
                            _title += shortText.Substring(0, 60) + "...";
                        }
                        else
                        {
                            _title += shortText;
                        }
                    }
                }
                return _title;
            }
            set
            {
                _title = value;
            }
        }

        private Exception _exception = null;
        /// <summary>
        /// Exception (if any) this message is referring to.
        /// </summary>
        [System.Xml.Serialization.XmlElement]
        public Exception Exception { get { return _exception; } set { _exception = value; } }




        RuntimeContext _runtimeContext = null;
        /// <summary>
        /// Place holder for gathering run time information.  Note:  This method is very heavy and should be
        /// used cautiously.  Use <see cref="T:RuntimeContext"/> constructors for information
        /// on capturing contexts.  If this field is populated, logging providers will generally persist
        /// values.
        /// </summary>
        [System.Xml.Serialization.XmlElement]
        public RuntimeContext RuntimeContext
        {
            get { return _runtimeContext; }
            set { _runtimeContext = value; }
        }

        /// <summary>
        /// Generates a standard output string.  EXTENSION:  Modify this class to change the format of the message being persisted
        /// </summary>
        /// <returns>A standard format of this message type</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();            
            try
            {               
                sb.Append(base.ToString());
                if (this.RuntimeContext != null)
                {
                    sb.AppendFormat("    Machine Context: {0}", Environment.NewLine);
                    sb.AppendFormat("      Identity:      {0}{1}", this.RuntimeContext.MachineContext.Identity, Environment.NewLine);
                    sb.AppendFormat("      IP Address(es):{0}{1}", this.RuntimeContext.MachineContext.IPAddressList, Environment.NewLine);
                    if (this.Exception == null)
                    {
                        sb.AppendFormat("      Stack:         {0}{1}", this.RuntimeContext.MachineContext.StackTrace, Environment.NewLine);
                    }
                    else
                    {
                        sb.AppendFormat("      Stack:         {0}{1}", "(see exception)", Environment.NewLine);
                    }
                }                
                sb.AppendFormat("    Title:           {0}{1}", this.Title, Environment.NewLine);

                if (this.Exception != null)
                {
                    sb.AppendFormat("    Exception:      {0}{1}", Utils.Expand(this.Exception), Environment.NewLine);
                }
                else
                {
                    sb.AppendFormat("    Exception:      {0}{1}", "(none)", Environment.NewLine);
                }
                if (this.RuntimeContext != null && string.IsNullOrEmpty(this.RuntimeContext.HttpRequest)==false)
                {
                    sb.AppendFormat("    HTTP Request:    {0}{1}", this.RuntimeContext.HttpRequest, Environment.NewLine);
                }
            }
            catch (Exception ex)
            {
                Log.LastChanceLog(Utils.Expand(ex));
                Log.LastChanceLog(sb.ToString());
            }
            return sb.ToString();
        }
    }
}
