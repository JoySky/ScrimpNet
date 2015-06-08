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
using System.Reflection;
using System.Threading;

namespace ScrimpNet.Core.Diagnostics
{

    /// <summary>
    /// A sentinel class that can be used in place of explicit method entrance and exit traces.  Typically a TraceProbe is instantiated in a using() statement that wraps an
    /// entire method.  The tracer can automatically log it's exit upon going out of scope
    /// </summary>
    public class TraceProbe : IDisposable
    {
        //private const int CALLER_FRAME = 1;

        private bool _enabled;
        private Log _log;
        private Stopwatch _stopwatch;
        private long _enterTicks = 0;
        private long _exitTicks = 0;
        private string _category = "Code";
        private Guid _tracerId = Guid.NewGuid();
        private Guid _parentTrace = Guid.Empty;
        /// <summary>
        /// Default constructor
        /// </summary>
        internal TraceProbe()
        {

        }

        /// <summary>
        /// Create a tracer that is bound to a specific category.  Categories are used for tracer analysis
        /// </summary>
        /// <param name="log">Name of log that is performing this trace</param>
        /// <param name="tracerCategory">String that will group different kinds of traces (e.g. Code, Database Call, WebService, etc).  Default: 'Code'</param>
        internal TraceProbe(Log log, string tracerCategory)
           
        {
            _category = string.IsNullOrEmpty(tracerCategory) ? "Code" : tracerCategory;
            _log = log;
            _enabled = _log.IsTraceEnabled;
            if (_enabled)
            {
                if (Trace.CorrelationManager.LogicalOperationStack.Count > 0)
                {
                    _parentTrace = Transform.ToGuid(Trace.CorrelationManager.LogicalOperationStack.Peek());
                }
                Trace.CorrelationManager.StartLogicalOperation(_tracerId);
                _enterTicks = DateTime.Now.Ticks;
                _exitTicks = 0;
                sendLogMessage();
                _stopwatch = new Stopwatch();
                _stopwatch.Start();
            }

        }

        /// <summary>
        /// Constructor that associates an active log with a particular trace.  Default TraceProbe category is 'Code'
        /// </summary>
        /// <param name="log"></param>
        public TraceProbe(Log log):this(log,"Code")
        {
        }

        /// <summary>
        /// Constructor that stored the caller's frame which represents the source of the trace
        /// </summary>
        /// <param name="log">Log to assoicate with this trace</param>
        /// <param name="targetFrame">Call stack location of caller</param>
        /// <param name="traceCategory">String that will group different kinds of traces (e.g. Code, Datbase Call, WebService, etc)</param>
        internal TraceProbe(Log log, StackFrame targetFrame, string traceCategory)
        {
            _log = log;
            _enabled = _log.IsTraceEnabled;

            if (_enabled)
            {
                if (Trace.CorrelationManager.LogicalOperationStack.Count > 0)
                {
                    _parentTrace = Transform.ToGuid(Trace.CorrelationManager.LogicalOperationStack.Peek());
                }
                Trace.CorrelationManager.StartLogicalOperation(_tracerId);
                _enterTicks = DateTime.Now.Ticks;
                _exitTicks = 0;
                _category = string.IsNullOrEmpty(traceCategory) ? "Code" : traceCategory;
                _stopwatch = new Stopwatch();
                sendLogMessage();
                _stopwatch.Start();
            }
        }

        private void sendLogMessage()
        {
            TracerLogMessage _logMessage = new TracerLogMessage();
            _logMessage.TracerCategory = this.Category;
            _logMessage.ElapsedTimeMs = 0;
            if (_exitTicks != 0L)
            {
                _logMessage.ElapsedTimeMs = (long)(new TimeSpan(_exitTicks - _enterTicks).TotalMilliseconds);
            }
            _logMessage.EnterTicks = _enterTicks;
            _logMessage.ExitTicks = _exitTicks;
            _logMessage.TracerId = _tracerId;
            _logMessage.ParentOperationId = _parentTrace;
            _log.WriteMessage(_logMessage);
        }

        /// <summary>
        /// If tracing is enabled
        /// </summary>
        public bool IsEnabled
        {
            get { return _enabled; }
        }

        /// <summary>
        /// If stopwatch is running
        /// </summary>
        public bool IsRunning
        {
            get
            {
                if (_enabled == true)
                {
                    return _stopwatch.IsRunning;
                }
                return false;
            }
        }
        /// <summary>
        /// String that can group several traces into a particular type.  E.g. 'Code', 'Db','Feed', 'InternalWs'
        /// </summary>
        public string Category
        {
            get { return _category; }
            set { _category = value; }
        }
        #region IDisposable Members

        /// <summary>
        /// Stop timing and write log entries to file
        /// </summary>
        /// <param name="isDisposing">true if being called from Disposable, false if from Finalize</param>
        public void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                if (_enabled)
                {
                    _exitTicks = DateTime.Now.Ticks;
                    if (Trace.CorrelationManager.LogicalOperationStack.Count > 0)
                    {
                        Trace.CorrelationManager.StopLogicalOperation();
                    }
                    _stopwatch.Stop();
                    sendLogMessage();
                }
            }
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose of things
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Finalizer
        /// </summary>
        ~TraceProbe()
        {
            Dispose(false);
        }
        #endregion
    }
}
