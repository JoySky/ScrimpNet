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
using System.Diagnostics;
using ScrimpNet.Core.Reflection;

namespace ScrimpNet.Core.Diagnostics
{
    /// <summary>
    /// Generic container for a log message.  All standard messages implmement these fields
    /// </summary>
    [Serializable]
    public abstract class LogMessageBase
    {
        private AssemblyVersion.CallerInfo _callerInfo;
        /// <summary>
        /// Default constructor
        /// </summary>
        public LogMessageBase()
        {
            // set default properties and force loading some properties from .config file
            this.MessageType = this.GetType().FullName;
            this.HostName = System.Environment.MachineName;
            this.TimeStamp = DateTime.Now;
            this.ActivityId = this.ActivityId; // force loading of ActivityId from base.Correlation manager
            this.ApplicationKey = this.ApplicationKey;
            this.Priority = this.Priority;
            this.ActiveEnvironment = this.ActiveEnvironment;
            _callerInfo = new AssemblyVersion().CallerInfoGet();
        }

        private Guid _relatedActivityId = Guid.Empty;
        /// <summary>
        /// Id that enables end-to-end identification of a common thread of messages.  Used the Trace.CorrelationManager
        /// </summary>        
        public Guid ActivityId
        {
            get
            {
                if (_relatedActivityId != Guid.Empty) return _relatedActivityId;
                _relatedActivityId = Trace.CorrelationManager.ActivityId;
                if (_relatedActivityId == Guid.Empty)
                {
                    Trace.CorrelationManager.ActivityId = Guid.NewGuid();
                    _relatedActivityId = Trace.CorrelationManager.ActivityId;
                }
                return _relatedActivityId;
            }
            set
            {
                Trace.CorrelationManager.ActivityId = value;
                _relatedActivityId = value;
            }
        }


        private string _loggerName;
        /// <summary>
        /// Name of logger creating this message.  Often used for identifying which part of application this message came from since logger names often are identified with application functions
        /// </summary>
        public string LoggerName
        {
            get
            {
                return _loggerName;
            }
            set
            {
                _loggerName = value;
            }
        }

        private LogMessageClass _logSourceType = LogMessageClass.ApplicationLogging;
        /// <summary>
        /// Gets or sets the type of the log source.
        /// ApplicationLogging, IISLogging, etc.
        /// </summary>
        public LogMessageClass LogMessageClass
        {
            get { return _logSourceType; }
            set { _logSourceType = value; }
        }

        private string _environmentName =CoreConfig.ActiveEnvironment;
        /// <summary>
        /// Returns the currently active environment the application is running in (dev, qa, production, demo, etc)
        /// </summary>
        public string ActiveEnvironment
        {
            get { return _environmentName; }
            set { _environmentName = value; }
        }

        private string _hostName = System.Environment.MachineName;
        /// <summary>
        /// Gets or sets the name of the host (Machine Name).
        /// </summary>
        public string HostName
        {
            get { return _hostName; }
            set { _hostName = value; }
        }

        /// <summary>
        /// Subsection of application (if any) (e.g. Credit Cards, Travel, etc)
        /// </summary>
        public string SubKey { get; set; }

        private string _applicationKey = CoreConfig.ApplicationKey;
        /// <summary>
        /// Gets the name of the application.  If not explicitly set using &lt;appSettings name="Application.Key"...&gt;
        /// </summary>
        public string ApplicationKey
        {
            get
            {
                if (string.IsNullOrEmpty(_applicationKey) == true)
                {
                    _applicationKey = CoreConfig.ApplicationKey;
                }
                return _applicationKey;
            }
            set
            {
                _applicationKey = value;
            }
        }

        private LogLevel _logLevel = LogLevel.Information;
        /// <summary>
        /// Severity of message being persisted
        /// </summary>
        public LogLevel LogLevel
        {
            get { return _logLevel; }
            set
            {
                _logLevel = value;
                priorityReset();
            }
        }

        private MessagePriority? _priority = null;
        /// <summary>
        /// Get and Sets how important the sender of this message considers this message.  Most implementations
        /// will not explictly set this value and allow message to determine priority based on log levels.
        /// </summary>
        public MessagePriority Priority
        {
            get
            {
                if (_priority.HasValue == true) return _priority.Value;
                return priorityReset();

            }
            set
            {
                _priority = value;
            }
        }
        private DateTime _createDate = DateTime.Now;
        /// <summary>
        /// Date/Time when this message was created
        /// </summary>
        public DateTime TimeStamp
        {
            get { return _createDate; }
            set { _createDate = Utils.Date.ToSqlDate(value); }
        }
        /// <summary>
        /// Resets internal priority to a value that matches LogLevel of this message
        /// </summary>
        /// <returns>Messages current priority</returns>
        internal MessagePriority priorityReset()
        {
            switch (this.LogLevel)
            {
                case LogLevel.Critical: _priority = MessagePriority.Highest; break;
                case LogLevel.Error: _priority = MessagePriority.VeryHigh; break;
                case LogLevel.Warning: _priority = MessagePriority.High; break;
                case LogLevel.Information: _priority = MessagePriority.Normal; break;
                case LogLevel.Trace: _priority = MessagePriority.Low; break;
                case LogLevel.Debug: _priority = MessagePriority.VeryLow; break;

                default:
                    _priority = MessagePriority.Normal; break;
            }
            return _priority.Value;
        }

        private string _messageType = string.Empty;
        /// <summary>
        /// .Net type name of this message.  Used for serialization purposes and set in constructors
        /// </summary>
        public string MessageType
        {
            get
            {
                if (string.IsNullOrEmpty(_messageType) == true)
                {
                    _messageType = this.GetType().FullName;
                }
                return _messageType;
            }
            set
            {
                _messageType = value;
            }
        }

        private string _messageText = "";

        /// <summary>
        /// Returns explictly set message text or default of this class (usually ToString())
        /// </summary>
        public string MessageText
        {
            get
            {
                if (_messageText == null)
                {
                    return ToString();
                }
                return _messageText;
            }
            set
            {
                _messageText = value;
            }
        }
        /// <summary>
        /// Detailed information about the callstack of method calling this class
        /// </summary>
        public AssemblyVersion.CallerInfo CallerInfo
        {
            get
            {
                return _callerInfo;
            }
            set
            {
                _callerInfo = value;
            }
        }

        /// <summary>
        /// Standard format for this class
        /// </summary>
        /// <returns>String value of this class</returns>
        public new virtual string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("{0:yyyy-MM-dd HH:mm:ss.fff}{1}", this.TimeStamp, Environment.NewLine);
            sb.AppendFormat("  {0}{1}", this._messageText, Environment.NewLine);
            sb.AppendFormat("    Log Level: {0}{1}", this.LogLevel, Environment.NewLine);
            sb.AppendFormat("     Priority: {0}{1}", this.Priority, System.Environment.NewLine);
            sb.AppendFormat("  Application: {0}{1}", this.ApplicationKey, Environment.NewLine);
            sb.AppendFormat("      Sub Key: {0}{1}", this.SubKey, Environment.NewLine);
            sb.AppendFormat("  Environment: {0}{1}", this.ActiveEnvironment, Environment.NewLine);
            sb.AppendFormat("      Machine: {0}{1}", this.HostName, Environment.NewLine);
            sb.AppendFormat("  Activity Id: {0}{1}", this.ActivityId, Environment.NewLine);
            sb.AppendFormat("  Logger Name: {0}{1}", this.LoggerName, Environment.NewLine);
            sb.AppendFormat("Message Class: {0}{1}", this.LogMessageClass.ToString(), Environment.NewLine);
            sb.AppendFormat(" Message Type: {0}{1}", this.MessageType, Environment.NewLine);
            sb.Append(_callerInfo.ToString());

            return sb.ToString();
        }
    }
}
