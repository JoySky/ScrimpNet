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
using System.Text;
using System.Diagnostics;
using System.Configuration;
using System.IO;

using System.Threading;
using ScrimpNet.Text;

namespace ScrimpNet.Diagnostics
{
    /// <summary>
    /// Base class for writing messages to persistant store.  By default creates log file in application's executing folder
    /// </summary>
    public sealed partial class Log: IDisposable 
    {
        private static Guid _correlationId = inititializeCorrelationId();

        private static Guid inititializeCorrelationId()
        {
            if (System.Diagnostics.Trace.CorrelationManager.ActivityId == Guid.Empty)
            {
                System.Diagnostics.Trace.CorrelationManager.ActivityId = Guid.NewGuid();
            }
            return System.Diagnostics.Trace.CorrelationManager.ActivityId;
        }

      
        
        private static ReaderWriterLockSlim _lastChanceLock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);

        /// <summary>
        /// Create a new trace and associate it with this log instance.  Uses default category of "Code"
        /// </summary>
        /// <returns>Reference to new tracing probe</returns>
        public TraceProbe NewTrace()
        {
            return NewTrace("Code");
        }

        /// <summary>
        /// Create a new trace that has a specific category associated with it
        /// </summary>
        /// <param name="traceCategory">Associate this trace with a specific category.  Cateogries are used for grouping traces of specific kinds ('Code', 'Database', 'ExternalParties', etc)</param>
        /// <returns>Reference to newly created trace</returns>
        public TraceProbe NewTrace(string traceCategory)
        {
            return new TraceProbe(this, traceCategory);
        }

        private static string _defaultKey = string.Empty;
        private string _applicationKey = string.Empty;

        private LoggerLevels _logLevels;
        

        //private static LoggerLevels toLoggerLevels(SourceLevels levels)
        //{
        //    LoggerLevels retLevel = LoggerLevels.None;
        //    if ((levels & SourceLevels.ActivityTracing) == SourceLevels.ActivityTracing)
        //    {
        //        retLevel |= LoggerLevels.Trace;
        //    }
        //    if ((levels & SourceLevels.Critical) == SourceLevels.Critical)
        //    {
        //        retLevel |= LoggerLevels.Critical;
        //    }
        //    if ((levels & SourceLevels.Error) == SourceLevels.Error)
        //    {
        //        retLevel |= LoggerLevels.Error;
        //    }
        //    if ((levels & SourceLevels.Information) == SourceLevels.Information)
        //    {
        //        retLevel |= LoggerLevels.Information;
        //    }
        //    if ((levels & SourceLevels.Verbose) == SourceLevels.Verbose)
        //    {
        //        retLevel |= LoggerLevels.Verbose;
        //    }
        //    if ((levels & SourceLevels.Warning) == SourceLevels.Warning)
        //    {
        //        retLevel |= LoggerLevels.Warning;
        //    }
        //    if ((levels & SourceLevels.Off) == SourceLevels.Off)
        //    {
        //        retLevel = LoggerLevels.None;
        //    }
        //    if ((levels & SourceLevels.All) == SourceLevels.All)
        //    {
        //        retLevel = LoggerLevels.All;
        //    }
        //    return retLevel;
        //}

        //private static SourceLevels toSourceLevel(LoggerLevels levels)
        //{
        //    SourceLevels retLevel = SourceLevels.Off;
        //    if ((levels & LoggerLevels.Critical) == LoggerLevels.Critical)
        //    {
        //        retLevel |= SourceLevels.Critical;
        //    }
        //    if ((levels & LoggerLevels.Debug) == LoggerLevels.Debug)
        //    {
        //        retLevel |= SourceLevels.Verbose;
        //    }
        //    if ((levels & LoggerLevels.Error) == LoggerLevels.Error)
        //    {
        //        retLevel |= SourceLevels.Error;
        //    }
        //    if ((levels & LoggerLevels.Information) == LoggerLevels.Information)
        //    {
        //        retLevel |= SourceLevels.Information;
        //    }
        //    if ((levels & LoggerLevels.None) == LoggerLevels.None)
        //    {
        //        return SourceLevels.Off;
        //    }
        //    if ((levels & LoggerLevels.Trace) == LoggerLevels.Trace)
        //    {
        //        retLevel |= SourceLevels.ActivityTracing;
        //    }
        //    if ((levels & LoggerLevels.Verbose) == LoggerLevels.Verbose)
        //    {
        //        retLevel |= SourceLevels.Verbose | SourceLevels.Information;
        //    }
        //    if ((levels & LoggerLevels.Vital) == LoggerLevels.Vital)
        //    {
        //        retLevel |= SourceLevels.Warning | SourceLevels.Critical | SourceLevels.Error;
        //    }
        //    if ((levels & LoggerLevels.Warning) == LoggerLevels.Warning)
        //    {
        //        retLevel |= SourceLevels.Warning;
        //    }
        //    if ((levels & LoggerLevels.All) == LoggerLevels.All)
        //    {
        //        retLevel |= SourceLevels.All;
        //    }

        //    return retLevel;
        //}
               

        private static string findKey()
        {
            if (string.IsNullOrEmpty(_defaultKey) == true)
            {
                _defaultKey = CoreConfig.ApplicationKey;
            }
            return _defaultKey;
        }

        #region IsEnabled

        /// <summary>
        /// Determines if a particular LogLevel (serverity of a single message) is enabled to be logged 
        /// </summary>
        /// <param name="level">Message log level to be checked</param>
        /// <returns>True if level is enabled to be logged or false if message cannot be logged in this instance</returns>
        /// <exception cref="ArgumentException">Thrown when level doesn't fall within a known set of elements</exception>
        public bool IsLevelEnabled(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Debug: return ((_logLevels & LoggerLevels.Debug) == LoggerLevels.Debug) && (OnLogDebugEventHandler != null);
                case LogLevel.Trace: return ((_logLevels & LoggerLevels.Trace) == LoggerLevels.Trace) && (OnLogTraceEventHandler != null);
                case LogLevel.Information: return ((_logLevels & LoggerLevels.Information) == LoggerLevels.Information) && (OnLogInformationEventHandler != null);
                case LogLevel.Warning: return ((_logLevels & LoggerLevels.Warning) == LoggerLevels.Warning) && (OnLogWarningEventHandler != null);
                case LogLevel.Error: return ((_logLevels & LoggerLevels.Error) == LoggerLevels.Error) && (OnLogErrorEventHandler != null);
                case LogLevel.Critical: return ((_logLevels & LoggerLevels.Critical) == LoggerLevels.Critical) && (OnLogCriticalEventHandler != null);
                //case LogLevel.Verbose: return IsLevelEnabled(LogLevel.Debug) && IsLevelEnabled(LogLevel.Trace) && IsLevelEnabled(LogLevel.Information);
                //case LogLevel.Vital: return IsLevelEnabled(LogLevel.Warning) && IsLevelEnabled(LogLevel.Error) && IsLevelEnabled(LogLevel.Critical);
                //case LogLevel.All: return IsLevelEnabled(LogLevel.Verbose) && IsLevelEnabled(LogLevel.Vital);
                case LogLevel.Off: return ((level & LogLevel.Off) == LogLevel.Off);
                default:
                    throw new ArgumentException("Unable to identify LogLevel of '{0}'", level.ToString());
            }
        }
        /// <summary>
        /// Get/Sets Debug level logging
        /// </summary>
        public bool IsDebugEnabled
        {
            get
            {
                return IsLevelEnabled(LogLevel.Debug);
            }
            set
            {
                SetLogLevel(LoggerLevels.Debug, value);
            }
        }

        /// <summary>
        /// Sets or clears logging of specific kinds of messages
        /// </summary>
        /// <param name="logLevel">LogLevel that needs to be cleared or set</param>
        /// <param name="value">True to turn on LogLevel.  False to turn off logging at that particular LogLevel</param>
        /// <returns>New active log level</returns>
        public LoggerLevels SetLogLevel(LoggerLevels logLevel, bool value)
        {
            if ((logLevel & LoggerLevels.Debug) == LoggerLevels.Debug)
            {
                if (value == true) // turn on debug
                {
                    _logLevels |= LoggerLevels.Debug;
                    if (OnLogDebugEventHandler == null) OnLogDebugEventHandler += writeDebug;
                }
                else // turn off Debug
                {
                    _logLevels &= ~LoggerLevels.Debug;
                }
            }
            if ((logLevel & LoggerLevels.Trace) == LoggerLevels.Trace)
            {
                if (value == true) // turn on Trace
                {
                    _logLevels |= LoggerLevels.Trace;
                    if (OnLogTraceEventHandler == null) OnLogTraceEventHandler += writeTrace;
                }
                else // turn off Trace
                {
                    _logLevels &= ~LoggerLevels.Trace;
                }
            }

            if ((logLevel & LoggerLevels.Information) == LoggerLevels.Information)
            {
                if (value == true) // turn on Information
                {
                    _logLevels |= LoggerLevels.Information;
                    if (OnLogInformationEventHandler == null) OnLogInformationEventHandler += writeInformation;
                }
                else // turn off Information
                {
                    _logLevels &= ~LoggerLevels.Information;
                }
            }
            if ((logLevel & LoggerLevels.Warning) == LoggerLevels.Warning)
            {
                if (value == true) // turn on Warning
                {
                    _logLevels |= LoggerLevels.Warning;
                    if (OnLogWarningEventHandler == null) OnLogWarningEventHandler += writeWarning;
                }
                else // turn off Warning
                {
                    _logLevels &= ~LoggerLevels.Warning;
                }
            }
            if ((logLevel & LoggerLevels.Error) == LoggerLevels.Error)
            {
                if (value == true) // turn on Error
                {
                    _logLevels |= LoggerLevels.Error;
                    if (OnLogErrorEventHandler == null) OnLogErrorEventHandler += writeError;
                }
                else // turn off Error
                {
                    _logLevels &= ~LoggerLevels.Error;
                }
            }
            if ((logLevel & LoggerLevels.Critical) == LoggerLevels.Critical)
            {
                if (value == true) // turn on Critical
                {
                    _logLevels |= LoggerLevels.Critical;
                    if (OnLogCriticalEventHandler == null) OnLogCriticalEventHandler += writeCritical;
                }
                else // turn off Critical
                {
                    _logLevels &= ~LoggerLevels.Critical;
                }
            }

            //if ((logLevel & LoggerLevels.Verbose) == LoggerLevels.Verbose)
            //{
            //    SetLogLevel(LoggerLevels.Debug | LoggerLevels.Trace | LoggerLevels.Information, value);
            //}
            //if ((logLevel & LoggerLevels.Vital) == LoggerLevels.Vital)
            //{
            //    SetLogLevel(LoggerLevels.Warning | LoggerLevels.Error| LoggerLevels.Critical, value);
            //}
            //if ((logLevel & LoggerLevels.All) == LoggerLevels.All)
            //{
            //    SetLogLevel(LoggerLevels.Vital | LoggerLevels.Verbose, value);
            //}
            //if ((logLevel & LoggerLevels.None) == LoggerLevels.None)
            //{
            //    if (value == true)
            //    {
            //        _logLevels = LoggerLevels.None;
            //    }
            //}
            return _logLevels;
        }

        /// <summary>
        /// Gets / Sets Error level logging
        /// </summary>
        public bool IsErrorEnabled
        {
            get
            {
                return IsLevelEnabled(LogLevel.Error);
            }
            set
            {
                SetLogLevel(LoggerLevels.Error, value);
            }
        }

        /// <summary>
        /// Gets / Sets Critical level logging
        /// </summary>
        public bool IsCriticalEnabled
        {
            get
            {
                return IsLevelEnabled(LogLevel.Critical);
            }
            set
            {
                SetLogLevel(LoggerLevels.Critical, value);
            }
        }

        /// <summary>
        /// Gets / Sets Information level logging
        /// </summary>
        public bool IsInformationEnabled
        {
            get
            {
                return IsLevelEnabled(LogLevel.Information);
            }
            set
            {
                SetLogLevel(LoggerLevels.Information, value);
            }
        }

        /// <summary>
        /// Gets / Sets Trace level logging
        /// </summary>
        public bool IsTraceEnabled
        {
            get
            {
                return IsLevelEnabled(LogLevel.Trace);
            }
            set
            {
                SetLogLevel(LoggerLevels.Trace, value);
            }
        }

        /// <summary>
        /// Gets / Sets Warning level logging
        /// </summary>
        public bool IsWarningEnabled
        {
            get
            {
                return IsLevelEnabled(LogLevel.Warning);
            }
            set
            {
                SetLogLevel(LoggerLevels.Warning, value);
            }
        }

        #endregion IsEnabled


        /// <summary>
        /// Asynchronous buffer that queues up log messages before persisting them to storage
        /// </summary>
        private static MessageBuffer  _logBuffer = new MessageBuffer();

        #region Logging
        /// <summary>
        /// Write message to logging store.  This is the single exit point from ScrimpNet logging into logging provider (TraceSource, NLog, Log4Net, etc)
        /// </summary>
        /// <param name="message">Message to be logged</param>
        public void WriteMessage(object message)  //EXTENSION: Since all logging goes through this method, this method is where customization might be most appropriate
        {
            LogMessageBase msg = message as LogMessageBase;
            if (msg == null)
            {
                Log.LastChanceLog("Cannot convert '{0}' to type 'LogMessageBase'.  Only LogMessageBase objects can be used with this logger",
                    message.GetType().ToString());
                return;
            }
            try
            {
                if (IsLevelEnabled(msg.LogLevel) == false) return; //this is the final check within the system if this message should be logged

                msg.ActivityId = (msg.ActivityId == Guid.Empty) ? _correlationId : msg.ActivityId;
                _logBuffer.Submit(msg); //submit message to internal cache and immediately return               
            }
            catch (Exception ex)
            {
                LastChanceLog(LogLevel.Error, Utils.Expand(ex));
            }
        }

        /// <summary>
        /// Write a message to persistent store
        /// </summary>
        /// <param name="message">Message that will be persisted</param>
        public void WriteMessage(LogMessageBase message)
        {
            if (IsLevelEnabled(message.LogLevel) == false) return;
            message.LoggerName = this.LogName;
            WriteMessage((object)message);            
        }

       

        private void writeTrace(LogMessageBase message)
        {
            if (IsTraceEnabled == false) return;
            message.LogLevel = LogLevel.Trace;
            WriteMessage(message);
        }

        private void writeInformation(LogMessageBase message)
        {
            if (IsInformationEnabled == false) return;
            message.LogLevel = LogLevel.Information;
            WriteMessage(message);
        }

        private void writeCritical(LogMessageBase message)
        {
            if (IsCriticalEnabled == false) return;
            message.LogLevel = LogLevel.Critical;
           // message.RuntimeContext = new RuntimeContext(RuntimeContextOptions.HttpRequestContext | RuntimeContextOptions.MachineContext);
            WriteMessage(message);
        }

        private void writeError(LogMessageBase message)
        {
            if (IsErrorEnabled == false) return;
            message.LogLevel = LogLevel.Error;
            //message.RuntimeContext = new RuntimeContext(RuntimeContextOptions.HttpRequestContext | RuntimeContextOptions.MachineContext);
            WriteMessage(message);
        }

        private void writeWarning(LogMessageBase message)
        {
            if (IsWarningEnabled == false) return;
            message.LogLevel = LogLevel.Warning;
           // message.RuntimeContext = new RuntimeContext(RuntimeContextOptions.HttpRequestContext | RuntimeContextOptions.MachineContext);
            WriteMessage(message);
        }

        private void writeDebug(LogMessageBase message)
        {
            if (IsDebugEnabled == false) return;
            message.LogLevel = LogLevel.Debug;
            WriteMessage(message);
        }
        #endregion Logging
     

        

        /// <summary>
        /// Close log and flush any buffered log messages
        /// </summary>
        /// <param name="isDisposing">True if being called from explict Dispose().  False if being called from destructor</param>
        public void Dispose(bool isDisposing)
        {
            if (isDisposing == true)
            {
                _logBuffer.Dispose();
                _logBuffer = null;
            }
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// IDisposable implementation
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~Log()
        {
            if (_logBuffer != null)
            {
                _logBuffer.Dispose();
            }
            Dispose(false);
        }

      
    }
}