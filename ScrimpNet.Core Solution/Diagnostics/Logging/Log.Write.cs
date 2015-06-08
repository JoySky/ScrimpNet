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
using System.Reflection;
using System.Diagnostics;
using ScrimpNet.Core.Text;


namespace ScrimpNet.Core.Diagnostics
{
    public sealed partial class Log : IDisposable
    {
        #region Debug
        /// <summary>
        /// Write a message to the debug output.  Changed message.LogLevel to Debug
        /// </summary>
        /// <param name="message">Hydrated message to write to log store</param>
        /// <returns>Created message that was submitted to log store or null if Debug is disabled</returns>
        public ApplicationLogMessage Debug(ApplicationLogMessage message)
        {
            try
            {
                message.LogLevel = LogLevel.Debug;
                if (IsDebugEnabled == false)
                {
                    return message;
                }
                if (OnLogDebugEventHandler != null) OnLogDebugEventHandler(message);
            }
            catch (Exception ex)
            {
                Log.LastChanceLog(ex);
            }
            return message;
        }
        /// <summary>
        /// Create a log message at the Debug log level
        /// </summary>
        /// <param name="messageText">Text to write to debug log.  May contain string.format({0}) arguments</param>
        /// <param name="args">Arguments to pass to messageText</param>
        /// <param name="messageNumber">Numerical index associated with this message</param>
        /// <returns>Created message that was submitted to log store or null if Debug is disabled</returns>
        public ApplicationLogMessage Debug(int messageNumber, string messageText, params object[] args)
        {
            ApplicationLogMessage msg = new ApplicationLogMessage();
            try
            {
                if (IsDebugEnabled == false) return null;
                msg = new ApplicationLogMessage(messageNumber, messageText, args);
                msg.MessageNumber = messageNumber;
                msg.LogLevel = LogLevel.Debug;
                return Debug(msg);
            }
            catch (Exception ex)
            {
                Log.LastChanceLog(ex);
            }
            return msg;
        }

        /// <summary>
        /// Create a log message at the Debug log level
        /// </summary>
        /// <param name="messageText">Text to write to debug log.  May contain string.format({0}) arguments</param>
        /// <param name="args">Arguments to pass to messageText</param>
        /// <returns>Created message that was submitted to log store or null if Debug is disabled</returns>
        public ApplicationLogMessage Debug(string messageText, params object[] args)
        {
            ApplicationLogMessage msg = new ApplicationLogMessage();
            try
            {
                if (IsDebugEnabled == false) return null;

                msg.LogLevel = LogLevel.Debug;
                msg.MessageText = TextUtils.StringFormat(messageText, args);
                return Debug(msg);
            }
            catch (Exception ex)
            {
                Log.LastChanceLog(ex);
            }
            return msg;
        }

        /// <summary>
        /// Create a log message at the Debug log level. Message is exception.Message
        /// </summary>
        /// <param name="exception">Exception to write to Debug log</param>
        /// <returns>Created message that was submitted to log store or null if Debug is disabled</returns>
        public ApplicationLogMessage Debug(Exception exception)
        {
            ApplicationLogMessage msg = new ApplicationLogMessage();
            try
            {
                if (IsDebugEnabled == false) return null;

                msg.LogLevel = LogLevel.Debug;
                msg.MessageText = exception.Message;
                msg.Exception = exception;
                return Debug(msg);
            }
            catch (Exception ex)
            {
                Log.LastChanceLog(ex);
            }
            return msg;
        }

        /// <summary>
        /// Create a log message at the Debug log level. Message is exception.Message
        /// </summary>
        /// <param name="exception">Exception to write to Debug log</param>
        /// <param name="messageNumber">Numerical index associated with this message</param>
        /// <returns>Created message that was submitted to log store or null if Debug is disabled</returns>
        public ApplicationLogMessage Debug(Exception exception, int messageNumber)
        {
            ApplicationLogMessage msg = new ApplicationLogMessage();
            try
            {
                if (IsDebugEnabled == false) return null;


                msg.LogLevel = LogLevel.Debug;
                msg.MessageText = exception.Message;
                msg.Exception = exception;
                msg.MessageNumber = messageNumber;
                return Debug(msg);
            }

            catch (Exception ex)
            {
                Log.LastChanceLog(ex);

            }
            return msg;
        }
        /// <summary>
        /// Create a log message at the Debug log level
        /// </summary>
        /// <param name="messageText">Text to write to debug log.  May contain string.format({0}) arguments</param>
        /// <param name="messageNumber">Numerical index associated with this message</param>
        /// <returns>Created message that was submitted to log store or null if Debug is disabled</returns>
        /// <param name="args">Values provided to format specifiers in messageText</param>
        /// <param name="exception">Inner exception to be bound to this message</param>
        public ApplicationLogMessage Debug(Exception exception, int messageNumber, string messageText, params object[] args)
        {
            ApplicationLogMessage msg = new ApplicationLogMessage();
            try
            {
                if (IsDebugEnabled == false) return null;

                msg.LogLevel = LogLevel.Debug;
                msg.MessageText = TextUtils.StringFormat(messageText, args);
                msg.Exception = exception;
                msg.MessageNumber = messageNumber;
                return Debug(msg);
            }

            catch (Exception ex)
            {
                Log.LastChanceLog(ex);

            }
            return msg;
        }

        /// <summary>
        /// Create a log message at the Debug log level.
        /// </summary>
        /// <param name="exception">Exception to write to Debug log</param>
        /// <param name="messageText">Text to write to debug log.  May contain string.format({0}) arguments</param>
        /// <returns>Created message that was submitted to log store or null if Debug is disabled</returns>
        /// <param name="args">Values to be supplied to format specifiers in messageText</param>
        public ApplicationLogMessage Debug(Exception exception, string messageText, params object[] args)
        {
            ApplicationLogMessage msg = new ApplicationLogMessage();
            try
            {
                if (IsDebugEnabled == false) return null;

                msg.LogLevel = LogLevel.Debug;
                msg.MessageText = TextUtils.StringFormat(messageText, args);
                msg.Exception = exception;
                return Debug(msg);
            }

            catch (Exception ex)
            {
                Log.LastChanceLog(ex);

            }
            return msg;
        }
        #endregion Debug

        #region Trace
        /// <summary>
        /// Write a message to the Trace output.  Changes message.LogLevel to Trace
        /// </summary>
        /// <param name="message">Hydrated message to write to log store</param>
        /// <returns>Created message that was submitted to log store or null if Trace is disabled</returns>
        public ApplicationLogMessage Trace(ApplicationLogMessage message)
        {
            try
            {
                message.LogLevel = LogLevel.Debug;
                if (IsTraceEnabled == false)
                {
                    return message;
                }
                if (OnLogTraceEventHandler != null) OnLogTraceEventHandler(message);
            }

            catch (Exception ex)
            {
                Log.LastChanceLog(ex);

            }

            return message;
        }
        /// <summary>
        /// Create a log message at the Trace log level
        /// </summary>
        /// <param name="messageText">Text to write to Trace log.  May contain string.format({0}) arguments</param>
        /// <param name="args">Arguments to pass to messageText</param>
        /// <param name="messageNumber">Numerical index associated with this message</param>
        /// <returns>Created message that was submitted to log store or null if Trace is disabled</returns>
        public ApplicationLogMessage Trace(int messageNumber, string messageText, params object[] args)
        {
            ApplicationLogMessage msg = new ApplicationLogMessage();
            try
            {
                if (IsTraceEnabled == false) return null;

                msg.LogLevel = LogLevel.Trace;
                msg.MessageText = TextUtils.StringFormat(messageText, args);
                msg.MessageNumber = messageNumber;
                if (OnLogTraceEventHandler != null) OnLogTraceEventHandler(msg);
            }

            catch (Exception ex)
            {
                Log.LastChanceLog(ex);
            }
            return msg;

        }

        /// <summary>
        /// Create a log message at the Trace log level
        /// </summary>
        /// <param name="messageText">Text to write to Trace log.  May contain string.format({0}) arguments</param>
        /// <param name="args">Arguments to pass to messageText</param>
        /// <returns>Created message that was submitted to log store</returns>
        public ApplicationLogMessage Trace(string messageText, params object[] args)
        {
            ApplicationLogMessage msg = new ApplicationLogMessage();
            try
            {
                if (IsTraceEnabled == false) return null;

                msg.LogLevel = LogLevel.Trace;
                msg.MessageText = TextUtils.StringFormat(messageText, args);
                if (OnLogTraceEventHandler != null) OnLogTraceEventHandler(msg);
            }

            catch (Exception ex)
            {
                Log.LastChanceLog(ex);

            }
            return msg;
        }

        /// <summary>
        /// Create a log message at the Trace log level. Message is exception.Message
        /// </summary>
        /// <param name="exception">Exception to write to Trace log</param>
        /// <returns>Created message that was submitted to log store</returns>
        public ApplicationLogMessage Trace(Exception exception)
        {
            ApplicationLogMessage msg = new ApplicationLogMessage();
            try
            {
                if (IsTraceEnabled == false) return null;


                msg.LogLevel = LogLevel.Trace;
                msg.MessageText = exception.Message;
                msg.Exception = exception;
                if (OnLogTraceEventHandler != null) OnLogTraceEventHandler(msg);
            }

            catch (Exception ex)
            {
                Log.LastChanceLog(ex);

            }
            return msg;

        }

        /// <summary>
        /// Create a log message at the Trace log level. Message is exception.Message
        /// </summary>
        /// <param name="exception">Exception to write to Trace log</param>
        /// <param name="messageNumber">Numerical index associated with this message</param>
        /// <returns>Created message that was submitted to log store</returns>
        public ApplicationLogMessage Trace(Exception exception, int messageNumber)
        {
            ApplicationLogMessage msg = new ApplicationLogMessage();
            try
            {
                if (IsTraceEnabled == false) return null;


                msg.LogLevel = LogLevel.Trace;
                msg.MessageText = exception.Message;
                msg.Exception = exception;
                msg.MessageNumber = messageNumber;
                if (OnLogTraceEventHandler != null) OnLogTraceEventHandler(msg);
            }

            catch (Exception ex)
            {
                Log.LastChanceLog(ex);

            }
            return msg;
        }
        /// <summary>
        /// Create a log message at the Trace log level
        /// </summary>
        /// <param name="messageText">Text to write to Trace log.  May contain string.format({0}) arguments</param>
        /// <param name="messageNumber">Numerical index associated with this message</param>
        /// <param name="args">Arguments to pass to messageText</param>
        /// <param name="exception">Exception to associate with this trace message</param>
        public ApplicationLogMessage Trace(Exception exception, int messageNumber, string messageText, params object[] args)
        {
            ApplicationLogMessage msg = new ApplicationLogMessage();
            try
            {
                if (IsTraceEnabled == false) return null;

                msg.LogLevel = LogLevel.Trace;
                msg.MessageText = TextUtils.StringFormat(messageText, args);
                msg.Exception = exception;
                msg.MessageNumber = messageNumber;
                if (OnLogTraceEventHandler != null) OnLogTraceEventHandler(msg);
            }

            catch (Exception ex)
            {
                Log.LastChanceLog(ex);

            }
            return msg;
        }

        /// <summary>
        /// Create a log message at the Trace log level.
        /// </summary>
        /// <param name="exception">Exception to write to Trace log</param>
        /// <param name="messageText">Text to write to Trace log.  May contain string.format({0}) arguments</param>
        /// <param name="args">Arguments to pass to messageText</param>   
        public ApplicationLogMessage Trace(Exception exception, string messageText, params object[] args)
        {
            ApplicationLogMessage msg = new ApplicationLogMessage();
            try
            {
                if (IsTraceEnabled == false) return null;

                msg.LogLevel = LogLevel.Trace;
                msg.MessageText = TextUtils.StringFormat(messageText, args);
                msg.Exception = exception;
                if (OnLogTraceEventHandler != null) OnLogTraceEventHandler(msg);
            }

            catch (Exception ex)
            {
                Log.LastChanceLog(ex);

            }
            return msg;
        }



        /// <summary>
        /// Create a log message at the Trace log level.
        /// </summary>
        /// <param name="direction">Sets 'Enter' or 'Exit'</param>  
        /// <param name="method">Details about the method being traced</param>
        public ApplicationLogMessage Trace(TraceEventType direction, MemberInfo method)
        {
            ApplicationLogMessage msg = new ApplicationLogMessage();
            try
            {
                if (IsTraceEnabled == false) return null;

                msg.LogLevel = LogLevel.Trace;
                //msg.MessageText = TextUtils.StringFormat("{0} {1}.{2}",direction.ToString(),
                //    method.DeclaringType.FullName,method.Name);
                if (OnLogTraceEventHandler != null) OnLogTraceEventHandler(msg);
            }

            catch (Exception ex)
            {
                Log.LastChanceLog(ex);

            }
            return msg;
        }


        /// <summary>
        /// Determines which direction the trace method is called
        /// </summary>
        public enum TraceDirection
        {
            /// <summary>
            /// Entering a method (usually first line of method code)
            /// </summary>
            Enter = 0,

            /// <summary>
            /// Exiting a method (usually last line of method code)
            /// </summary>
            Exit = 1
        }
        #endregion Trace

        #region Information
        /// <summary>
        /// Write a message to the Information output.  Changed message.LogLevel to Information
        /// </summary>
        /// <param name="message">Hydrated message to write to log store</param>
        /// <returns>Created message that was submitted to log store or null if Information is disabled</returns>
        public ApplicationLogMessage Information(ApplicationLogMessage message)
        {
            try
            {
                message.LogLevel = LogLevel.Information;
                if (IsInformationEnabled == false)
                {
                    return message;
                }
                if (OnLogInformationEventHandler != null) OnLogInformationEventHandler(message);
            }

            catch (Exception ex)
            {
                Log.LastChanceLog(ex);

            }
            return message;
        }
        /// <summary>
        /// Create a log message at the Information log level
        /// </summary>
        /// <param name="messageText">Text to write to Information log.  May contain string.format({0}) arguments</param>
        /// <param name="args">Arguments to pass to messageText</param>
        /// <param name="messageNumber">Numerical index associated with this message</param>
        /// <returns>Created message that was submitted to log store or null if Information is disabled</returns>
        public ApplicationLogMessage Information(int messageNumber, string messageText, params object[] args)
        {
            ApplicationLogMessage msg = new ApplicationLogMessage();
            try
            {
                if (IsInformationEnabled == false) return null;

                msg.LogLevel = LogLevel.Information;
                msg.MessageText = TextUtils.StringFormat(messageText, args);
                msg.MessageNumber = messageNumber;
                if (OnLogInformationEventHandler != null) OnLogInformationEventHandler(msg);
            }

            catch (Exception ex)
            {
                Log.LastChanceLog(ex);

            }
            return msg;
        }

        /// <summary>
        /// Create a log message at the Information log level
        /// </summary>
        /// <param name="messageText">Text to write to Information log.  May contain string.format({0}) arguments</param>
        /// <param name="args">Arguments to pass to messageText</param>
        /// <returns>Created message that was submitted to log store</returns>
        public ApplicationLogMessage Information(string messageText, params object[] args)
        {
            ApplicationLogMessage msg = new ApplicationLogMessage();
            try
            {
                if (IsInformationEnabled == false) return null;

                msg.LogLevel = LogLevel.Information;
                msg.MessageText = TextUtils.StringFormat(messageText, args);
                if (OnLogInformationEventHandler != null) OnLogInformationEventHandler(msg);
            }

            catch (Exception ex)
            {
                Log.LastChanceLog(ex);

            }
            return msg;
        }

        /// <summary>
        /// Create a log message at the Information log level. Message is exception.Message
        /// </summary>
        /// <param name="exception">Exception to write to Information log</param>
        /// <returns>Created message that was submitted to log store</returns>
        public ApplicationLogMessage Information(Exception exception)
        {
            ApplicationLogMessage msg = new ApplicationLogMessage();
            try
            {
                if (IsInformationEnabled == false) return null;


                msg.LogLevel = LogLevel.Information;
                msg.MessageText = exception.Message;
                msg.Exception = exception;
                if (OnLogInformationEventHandler != null) OnLogInformationEventHandler(msg);
            }

            catch (Exception ex)
            {
                Log.LastChanceLog(ex);

            }
            return msg;
        }

        /// <summary>
        /// Create a log message at the Information log level. Message is exception.Message
        /// </summary>
        /// <param name="exception">Exception to write to Information log</param>
        /// <param name="messageNumber">Numerical index associated with this message</param>
        /// <returns>Created message that was submitted to log store</returns>
        public ApplicationLogMessage Information(Exception exception, int messageNumber)
        {
            ApplicationLogMessage msg = new ApplicationLogMessage();
            try
            {
                if (IsInformationEnabled == false) return null;


                msg.LogLevel = LogLevel.Information;
                msg.MessageText = exception.Message;
                msg.Exception = exception;
                msg.MessageNumber = messageNumber;
                if (OnLogInformationEventHandler != null) OnLogInformationEventHandler(msg);
            }

            catch (Exception ex)
            {
                Log.LastChanceLog(ex);

            }
            return msg;
        }
        /// <summary>
        /// Create a log message at the Information log level
        /// </summary>
        /// <param name="messageText">Text to write to Information log.  May contain string.format({0}) arguments</param>
        /// <param name="messageNumber">Numerical index associated with this message</param>
        /// <param name="args">Arguments to pass to messageText</param>
        /// <param name="exception">Exception that will be bound to this message</param>
        public ApplicationLogMessage Information(Exception exception, int messageNumber, string messageText, params object[] args)
        {
            ApplicationLogMessage msg = new ApplicationLogMessage();
            try
            {
                if (IsInformationEnabled == false) return null;

                msg.LogLevel = LogLevel.Information;
                msg.MessageText = TextUtils.StringFormat(messageText, args);
                msg.Exception = exception;
                msg.MessageNumber = messageNumber;
                if (OnLogInformationEventHandler != null) OnLogInformationEventHandler(msg);
            }

            catch (Exception ex)
            {
                Log.LastChanceLog(ex);

            }
            return msg;
        }

        /// <summary>
        /// Create a log message at the Information log level.
        /// </summary>
        /// <param name="exception">Exception to write to Information log</param>
        /// <param name="messageText">Text to write to Information log.  May contain string.format({0}) arguments</param>
        /// <param name="args">Arguments to pass to messageText</param>   
        public ApplicationLogMessage Information(Exception exception, string messageText, params object[] args)
        {
            ApplicationLogMessage msg = new ApplicationLogMessage();
            try
            {
                if (IsInformationEnabled == false) return null;

                msg.LogLevel = LogLevel.Information;
                msg.MessageText = TextUtils.StringFormat(messageText, args);
                msg.Exception = exception;
                if (OnLogInformationEventHandler != null) OnLogInformationEventHandler(msg);
            }

            catch (Exception ex)
            {
                Log.LastChanceLog(ex);

            }
            return msg;
        }
        #endregion Information

        #region Warning
        /// <summary>
        /// Write a message to the Warning output.  Changed message.LogLevel to Warning
        /// </summary>
        /// <param name="message">Hydrated message to write to log store</param>
        /// <returns>Created message that was submitted to log store or null if Warning is disabled</returns>
        public ApplicationLogMessage Warning(ApplicationLogMessage message)
        {
            try
            {
                message.LogLevel = LogLevel.Warning;
                if (IsWarningEnabled == false)
                {
                    return message;
                }
                message.RuntimeContext = new RuntimeContext(RuntimeContextOptions.HttpRequestContext | RuntimeContextOptions.MachineContext);
                if (OnLogWarningEventHandler != null) OnLogWarningEventHandler(message);
            }
            catch (Exception ex)
            {
                Log.LastChanceLog(ex);

            }
            return message;
        }
        /// <summary>
        /// Create a log message at the Warning log level
        /// </summary>
        /// <param name="messageText">Text to write to Warning log.  May contain string.format({0}) arguments</param>
        /// <param name="args">Arguments to pass to messageText</param>
        /// <param name="messageNumber">Numerical index associated with this message</param>
        /// <returns>Created message that was submitted to log store or null if Warning is disabled</returns>
        public ApplicationLogMessage Warning(int messageNumber, string messageText, params object[] args)
        {
            ApplicationLogMessage msg = new ApplicationLogMessage();
            try
            {
                if (IsWarningEnabled == false) return null;

                msg.RuntimeContext = new RuntimeContext(RuntimeContextOptions.HttpRequestContext | RuntimeContextOptions.MachineContext);
                msg.LogLevel = LogLevel.Warning;
                msg.MessageNumber = messageNumber;
                msg.MessageText = TextUtils.StringFormat(messageText, args);
                if (OnLogWarningEventHandler != null) OnLogWarningEventHandler(msg);
            }

            catch (Exception ex)
            {
                Log.LastChanceLog(ex);

            }
            return msg;
        }

        /// <summary>
        /// Create a log message at the Warning log level
        /// </summary>
        /// <param name="messageText">Text to write to Warning log.  May contain string.format({0}) arguments</param>
        /// <param name="args">Arguments to pass to messageText</param>
        /// <returns>Created message that was submitted to log store</returns>
        public ApplicationLogMessage Warning(string messageText, params object[] args)
        {
            ApplicationLogMessage msg = new ApplicationLogMessage();
            try
            {
                if (IsWarningEnabled == false) return null;

                msg.RuntimeContext = new RuntimeContext(RuntimeContextOptions.HttpRequestContext | RuntimeContextOptions.MachineContext);
                msg.LogLevel = LogLevel.Warning;
                msg.MessageText = TextUtils.StringFormat(messageText, args);

                if (OnLogWarningEventHandler != null) OnLogWarningEventHandler(msg);
            }

            catch (Exception ex)
            {
                Log.LastChanceLog(ex);

            }
            return msg;
        }

        /// <summary>
        /// Create a log message at the Warning log level. Message is exception.Message
        /// </summary>
        /// <param name="exception">Exception to write to Warning log</param>
        /// <returns>Created message that was submitted to log store</returns>
        public ApplicationLogMessage Warning(Exception exception)
        {
            ApplicationLogMessage msg = new ApplicationLogMessage();
            try
            {
                if (IsWarningEnabled == false) return null;


                msg.RuntimeContext = new RuntimeContext(RuntimeContextOptions.HttpRequestContext | RuntimeContextOptions.MachineContext);
                msg.LogLevel = LogLevel.Warning;
                msg.MessageText = exception.Message;
                msg.Exception = exception;
                if (OnLogWarningEventHandler != null) OnLogWarningEventHandler(msg);
            }

            catch (Exception ex)
            {
                Log.LastChanceLog(ex);

            }
            return msg;
        }

        /// <summary>
        /// Create a log message at the Warning log level. Message is exception.Message
        /// </summary>
        /// <param name="exception">Exception to write to Warning log</param>
        /// <param name="messageNumber">Numerical index associated with this message</param>
        /// <returns>Created message that was submitted to log store</returns>
        public ApplicationLogMessage Warning(Exception exception, int messageNumber)
        {
            ApplicationLogMessage msg = new ApplicationLogMessage();
            try
            {
                if (IsWarningEnabled == false) return null;


                msg.RuntimeContext = new RuntimeContext(RuntimeContextOptions.HttpRequestContext | RuntimeContextOptions.MachineContext);
                msg.LogLevel = LogLevel.Warning;
                msg.MessageText = exception.Message;
                msg.Exception = exception;
                msg.MessageNumber = messageNumber;
                if (OnLogWarningEventHandler != null) OnLogWarningEventHandler(msg);
            }
            catch (Exception ex)
            {
                Log.LastChanceLog(ex);

            }
            return msg;
        }
        /// <summary>
        /// Create a log message at the Warning log level
        /// </summary>
        /// <param name="messageText">Text to write to Warning log.  May contain string.format({0}) arguments</param>
        /// <param name="messageNumber">Numerical index associated with this message</param>
        /// <param name="args">Arguments to pass to messageText</param>
        /// <param name="exception">Exception that is associated with this message</param>
        public ApplicationLogMessage Warning(Exception exception, int messageNumber, string messageText, params object[] args)
        {
            ApplicationLogMessage msg = new ApplicationLogMessage();
            try
            {
                if (IsWarningEnabled == false) return null;

                msg.RuntimeContext = new RuntimeContext(RuntimeContextOptions.HttpRequestContext | RuntimeContextOptions.MachineContext);
                msg.LogLevel = LogLevel.Warning;
                msg.MessageText = TextUtils.StringFormat(messageText, args);
                msg.MessageNumber = messageNumber;
                if (OnLogWarningEventHandler != null) OnLogWarningEventHandler(msg);
            }

            catch (Exception ex)
            {
                Log.LastChanceLog(ex);

            }
            return msg;
        }

        /// <summary>
        /// Create a log message at the Warning log level.
        /// </summary>
        /// <param name="exception">Exception to write to Warning log</param>
        /// <param name="messageText">Text to write to Warning log.  May contain string.format({0}) arguments</param>
        /// <param name="args">Arguments to pass to messageText</param>   
        public ApplicationLogMessage Warning(Exception exception, string messageText, params object[] args)
        {
            ApplicationLogMessage msg = new ApplicationLogMessage();
            try
            {
                if (IsWarningEnabled == false) return null;

                msg.RuntimeContext = new RuntimeContext(RuntimeContextOptions.HttpRequestContext | RuntimeContextOptions.MachineContext);
                msg.LogLevel = LogLevel.Warning;
                msg.MessageText = TextUtils.StringFormat(messageText, args);
                msg.Exception = exception;
                if (OnLogWarningEventHandler != null) OnLogWarningEventHandler(msg);
            }

            catch (Exception ex)
            {
                Log.LastChanceLog(ex);

            }
            return msg;
        }
        #endregion Warning

        #region Error
        /// <summary>
        /// Write a message to the Error output.  Changed message.LogLevel to Error
        /// </summary>
        /// <param name="message">Hydrated message to write to log store</param>
        /// <returns>Created message that was submitted to log store or null if Error is disabled</returns>
        public ApplicationLogMessage Error(ApplicationLogMessage message)
        {
            try
            {

                if (IsErrorEnabled == false)
                {
                    return message;
                }
                message.LogLevel = LogLevel.Error;
                message.RuntimeContext = new RuntimeContext(RuntimeContextOptions.HttpRequestContext | RuntimeContextOptions.MachineContext);
                if (OnLogErrorEventHandler != null) OnLogErrorEventHandler(message);
            }
            catch (Exception ex)
            {
                Log.LastChanceLog(ex);
            }
            return message;
        }
        /// <summary>
        /// Create a log message at the Error log level
        /// </summary>
        /// <param name="messageText">Text to write to Error log.  May contain string.format({0}) arguments</param>
        /// <param name="args">Arguments to pass to messageText</param>
        /// <param name="messageNumber">Numerical index associated with this message</param>
        /// <returns>Created message that was submitted to log store or null if Error is disabled</returns>
        public ApplicationLogMessage Error(int messageNumber, string messageText, params object[] args)
        {
            ApplicationLogMessage msg = new ApplicationLogMessage();
            try
            {
                if (IsErrorEnabled == false) return null;

                msg.RuntimeContext = new RuntimeContext(RuntimeContextOptions.HttpRequestContext | RuntimeContextOptions.MachineContext);
                msg.LogLevel = LogLevel.Error;
                msg.MessageText = TextUtils.StringFormat(messageText, args);
                msg.MessageNumber = messageNumber;
                if (OnLogErrorEventHandler != null) OnLogErrorEventHandler(msg);
            }

            catch (Exception ex)
            {
                Log.LastChanceLog(ex);

            }
            return msg;
        }

        /// <summary>
        /// Create a log message at the Error log level
        /// </summary>
        /// <param name="messageText">Text to write to Error log.  May contain string.format({0}) arguments</param>
        /// <param name="args">Arguments to pass to messageText</param>
        /// <returns>Created message that was submitted to log store</returns>
        public ApplicationLogMessage Error(string messageText, params object[] args)
        {
            ApplicationLogMessage msg = new ApplicationLogMessage();
            try
            {
                if (IsErrorEnabled == false) return null;

                msg.RuntimeContext = new RuntimeContext(RuntimeContextOptions.HttpRequestContext | RuntimeContextOptions.MachineContext);
                msg.LogLevel = LogLevel.Error;
                msg.MessageText = TextUtils.StringFormat(messageText, args);
                if (OnLogErrorEventHandler != null) OnLogErrorEventHandler(msg);
            }

            catch (Exception ex)
            {
                Log.LastChanceLog(ex);

            }
            return msg;
        }

        /// <summary>
        /// Create a log message at the Error log level. Message is exception.Message
        /// </summary>
        /// <param name="exception">Exception to write to Error log</param>
        /// <returns>Created message that was submitted to log store</returns>
        public ApplicationLogMessage Error(Exception exception)
        {
            ApplicationLogMessage msg = new ApplicationLogMessage();
            try
            {
                if (IsErrorEnabled == false) return null;


                msg.RuntimeContext = new RuntimeContext(RuntimeContextOptions.HttpRequestContext | RuntimeContextOptions.MachineContext);
                msg.LogLevel = LogLevel.Error;
                msg.MessageText = exception.Message;
                msg.Exception = exception;
                if (OnLogErrorEventHandler != null) OnLogErrorEventHandler(msg);
            }

            catch (Exception ex)
            {
                Log.LastChanceLog(ex);

            }
            return msg;
        }

        /// <summary>
        /// Create a log message at the Error log level. Message is exception.Message
        /// </summary>
        /// <param name="exception">Exception to write to Error log</param>
        /// <param name="messageNumber">Numerical index associated with this message</param>
        /// <returns>Created message that was submitted to log store</returns>
        public ApplicationLogMessage Error(Exception exception, int messageNumber)
        {
            ApplicationLogMessage msg = new ApplicationLogMessage();
            try
            {
                if (IsErrorEnabled == false) return null;


                msg.RuntimeContext = new RuntimeContext(RuntimeContextOptions.HttpRequestContext | RuntimeContextOptions.MachineContext);
                msg.LogLevel = LogLevel.Error;
                msg.MessageText = exception.Message;
                msg.Exception = exception;
                msg.MessageNumber = messageNumber;
                if (OnLogErrorEventHandler != null) OnLogErrorEventHandler(msg);
            }

            catch (Exception ex)
            {
                Log.LastChanceLog(ex);

            }
            return msg;
        }
        /// <summary>
        /// Create a log message at the Error log level
        /// </summary>
        /// <param name="messageText">Text to write to Error log.  May contain string.format({0}) arguments</param>
        /// <param name="messageNumber">Numerical index associated with this message</param>
        /// <param name="args">Arguments to pass to messageText</param>
        /// <param name="exception">Exception that is associated with this message</param>
        public ApplicationLogMessage Error(Exception exception, int messageNumber, string messageText, params object[] args)
        {
            ApplicationLogMessage msg = new ApplicationLogMessage();
            try
            {
                if (IsErrorEnabled == false) return null;

                msg.RuntimeContext = new RuntimeContext(RuntimeContextOptions.HttpRequestContext | RuntimeContextOptions.MachineContext);
                msg.LogLevel = LogLevel.Error;
                msg.MessageText = TextUtils.StringFormat(messageText, args);
                msg.MessageNumber = messageNumber;
                msg.Exception = exception;
                if (OnLogErrorEventHandler != null) OnLogErrorEventHandler(msg);
            }

            catch (Exception ex)
            {
                Log.LastChanceLog(ex);

            }
            return msg;
        }

        /// <summary>
        /// Create a log message at the Error log level.
        /// </summary>
        /// <param name="exception">Exception to write to Error log</param>
        /// <param name="messageText">Text to write to Error log.  May contain string.format({0}) arguments</param>
        /// <param name="args">Arguments to pass to messageText</param>   
        public ApplicationLogMessage Error(Exception exception, string messageText, params object[] args)
        {
            ApplicationLogMessage msg = new ApplicationLogMessage();
            try
            {
                if (IsErrorEnabled == false) return null;

                msg.RuntimeContext = new RuntimeContext(RuntimeContextOptions.HttpRequestContext | RuntimeContextOptions.MachineContext);
                msg.LogLevel = LogLevel.Error;
                msg.Exception = exception;
                msg.MessageText = TextUtils.StringFormat(messageText, args);
                if (OnLogErrorEventHandler != null) OnLogErrorEventHandler(msg);
            }
            catch (Exception ex)
            {
                Log.LastChanceLog(ex);
            }
            return msg;
        }
        #endregion Error

        #region Critical
        /// <summary>
        /// Write a message to the Critical output.  Changed message.LogLevel to Critical
        /// </summary>
        /// <param name="message">Hydrated message to write to log store</param>
        /// <returns>Created message that was submitted to log store or null if Critical is disabled</returns>
        public ApplicationLogMessage Critical(ApplicationLogMessage message)
        {
            try
            {
                message.LogLevel = LogLevel.Critical;
                if (IsCriticalEnabled == false)
                {
                    return message;
                }
                message.RuntimeContext = new RuntimeContext(RuntimeContextOptions.HttpRequestContext | RuntimeContextOptions.MachineContext);
                if (OnLogCriticalEventHandler != null) OnLogCriticalEventHandler(message);
            }

            catch (Exception ex)
            {
                Log.LastChanceLog(ex);

            }
            return message;
        }
        /// <summary>
        /// Create a log message at the Critical log level
        /// </summary>
        /// <param name="messageText">Text to write to Critical log.  May contain string.format({0}) arguments</param>
        /// <param name="args">Arguments to pass to messageText</param>
        /// <param name="messageNumber">Numerical index associated with this message</param>
        /// <returns>Created message that was submitted to log store or null if Critical is disabled</returns>
        public ApplicationLogMessage Critical(int messageNumber, string messageText, params object[] args)
        {
            ApplicationLogMessage msg = new ApplicationLogMessage();
            try
            {
                if (IsCriticalEnabled == false) return null;

                msg.RuntimeContext = new RuntimeContext(RuntimeContextOptions.HttpRequestContext | RuntimeContextOptions.MachineContext);
                msg.LogLevel = LogLevel.Critical;
                msg.MessageText = TextUtils.StringFormat(messageText, args);
                msg.MessageNumber = messageNumber;
                if (OnLogCriticalEventHandler != null) OnLogCriticalEventHandler(msg);
            }
            catch (Exception ex)
            {
                Log.LastChanceLog(ex);

            }
            return msg;
        }

        /// <summary>
        /// Create a log message at the Critical log level
        /// </summary>
        /// <param name="messageText">Text to write to Critical log.  May contain string.format({0}) arguments</param>
        /// <param name="args">Arguments to pass to messageText</param>
        /// <returns>Created message that was submitted to log store</returns>
        public ApplicationLogMessage Critical(string messageText, params object[] args)
        {
            ApplicationLogMessage msg = new ApplicationLogMessage();
            try
            {
                if (IsCriticalEnabled == false) return null;

                msg.RuntimeContext = new RuntimeContext(RuntimeContextOptions.HttpRequestContext | RuntimeContextOptions.MachineContext);
                msg.LogLevel = LogLevel.Critical;
                msg.MessageText = TextUtils.StringFormat(messageText, args);
                if (OnLogCriticalEventHandler != null) OnLogCriticalEventHandler(msg);
            }

            catch (Exception ex)
            {
                Log.LastChanceLog(ex);

            }
            return msg;
        }

        /// <summary>
        /// Create a log message at the Critical log level. Message is exception.Message
        /// </summary>
        /// <param name="exception">Exception to write to Critical log</param>
        /// <returns>Created message that was submitted to log store</returns>
        public ApplicationLogMessage Critical(Exception exception)
        {
            ApplicationLogMessage msg = new ApplicationLogMessage();
            try
            {
                if (IsCriticalEnabled == false) return null;


                msg.RuntimeContext = new RuntimeContext(RuntimeContextOptions.HttpRequestContext | RuntimeContextOptions.MachineContext);
                msg.LogLevel = LogLevel.Critical;
                msg.MessageText = exception.Message;
                msg.Exception = exception;
                if (OnLogCriticalEventHandler != null) OnLogCriticalEventHandler(msg);
            }

            catch (Exception ex)
            {
                Log.LastChanceLog(ex);

            }
            return msg;
        }

        /// <summary>
        /// Create a log message at the Critical log level. Message is exception.Message
        /// </summary>
        /// <param name="exception">Exception to write to Critical log</param>
        /// <param name="messageNumber">Numerical index associated with this message</param>
        /// <returns>Created message that was submitted to log store</returns>
        public ApplicationLogMessage Critical(Exception exception, int messageNumber)
        {
            ApplicationLogMessage msg = new ApplicationLogMessage();
            try
            {
                if (IsCriticalEnabled == false) return null;


                msg.RuntimeContext = new RuntimeContext(RuntimeContextOptions.HttpRequestContext | RuntimeContextOptions.MachineContext);
                msg.LogLevel = LogLevel.Critical;
                msg.MessageText = exception.Message;
                msg.Exception = exception;
                msg.MessageNumber = messageNumber;
                if (OnLogCriticalEventHandler != null) OnLogCriticalEventHandler(msg);
            }

            catch (Exception ex)
            {
                Log.LastChanceLog(ex);

            }
            return msg;
        }
        /// <summary>
        /// Create a log message at the Critical log level
        /// </summary>
        /// <param name="messageText">Text to write to Critical log.  May contain string.format({0}) arguments</param>
        /// <param name="messageNumber">Numerical index associated with this message</param>
        /// <param name="args">Arguments to pass to messageText</param>
        /// <param name="exception">Excpetion that is associated with this message</param>
        public ApplicationLogMessage Critical(Exception exception, int messageNumber, string messageText, params object[] args)
        {
            ApplicationLogMessage msg = new ApplicationLogMessage();
            try
            {
                if (IsCriticalEnabled == false) return null;

                msg.RuntimeContext = new RuntimeContext(RuntimeContextOptions.HttpRequestContext | RuntimeContextOptions.MachineContext);
                msg.LogLevel = LogLevel.Critical;
                msg.MessageText = TextUtils.StringFormat(messageText, args);
                msg.MessageNumber = messageNumber;
                msg.Exception = exception;
                if (OnLogCriticalEventHandler != null) OnLogCriticalEventHandler(msg);
            }

            catch (Exception ex)
            {
                Log.LastChanceLog(ex);

            }
            return msg;
        }

        /// <summary>
        /// Create a log message at the Critical log level.
        /// </summary>
        /// <param name="exception">Exception to write to Critical log</param>
        /// <param name="messageText">Text to write to Critical log.  May contain string.format({0}) arguments</param>
        /// <param name="args">Arguments to pass to messageText</param>   
        public ApplicationLogMessage Critical(Exception exception, string messageText, params object[] args)
        {
            ApplicationLogMessage msg = new ApplicationLogMessage();
            try
            {
                if (IsCriticalEnabled == false) return null;

                msg.RuntimeContext = new RuntimeContext(RuntimeContextOptions.HttpRequestContext | RuntimeContextOptions.MachineContext);
                msg.LogLevel = LogLevel.Critical;
                msg.MessageText = TextUtils.StringFormat(messageText, args);
                msg.Exception = exception;
                if (OnLogCriticalEventHandler != null) OnLogCriticalEventHandler(msg);
            }

            catch (Exception ex)
            {
                Log.LastChanceLog(ex);

            }
            return msg;
        }
        #endregion Critical
    }
}