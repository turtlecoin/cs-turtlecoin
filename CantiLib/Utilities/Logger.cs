//
// Copyright (c) 2018-2019 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace Canti
{
    /// <summary>
    /// An enumerator that specifies which level of output a logger will utilize
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// No logging takes place
        /// </summary>
        NONE = -2,

        /// <summary>
        /// Only messages with the "important" label are shown
        /// </summary>
        IMPORTANT_ONLY = -1,

        /// <summary>
        /// Only messages with the "important", "info", and "error" labels are shown
        /// </summary>
        DEFAULT = 0,

        /// <summary>
        /// Only messages with the "important", "info", "error", and "warning" labels are shown
        /// </summary>
        ENHANCED = 1,

        /// <summary>
        /// All message types are shown, including "debug", and time labels show TWO decimal places
        /// </summary>
        DEBUG = 2,

        /// <summary>
        /// All message types are shown, including "debug", and time labels show SIX decimal places
        /// </summary>
        MAX = 3
    }

    // Enumerates a log message's label name
    internal enum LogLabel
    {
        IMPORTANT,
        INFO,
        ERROR,
        WARNING,
        DEBUG
    }

    // Contains log message information
    internal struct LogMessage
    {
        internal DateTime Timestamp { get; set; }
        internal LogLabel Label { get; set; }
        internal bool Prefix { get; set; }
        internal string Message { get; set; }
    }

    /// <summary>
    /// Logger utility to facilitate logging at different levels and saving to a log file
    /// </summary>
    public sealed class Logger
    {
        #region Properties and Fields

        #region Public

        /// <summary>
        /// What level of logging will be shown
        /// </summary>
        public LogLevel LogLevel { get; set; }

        /// <summary>
        /// (OPTIONAL) A file where all logger output is also written to
        /// </summary>
        public string LogFile { get; set; }

        /// <summary>
        /// If set to true, a time and label prefix is shown alongside each log message
        /// </summary>
        public bool ShowPrefix { get; set; }

        /// <summary>
        /// (OPTIONAL) A custom prefix that will be shown before a label name, if showing prefixes
        /// </summary>
        public string CustomPrefix { get; set; }

        /// <summary>
        /// The default color for logger output
        /// </summary>
        public ConsoleColor InfoColor { get; set; }

        /// <summary>
        /// The color important messages will be shown in when logging
        /// </summary>
        public ConsoleColor ImportantColor { get; set; }

        /// <summary>
        /// The color debug messages will be shown in when logging
        /// </summary>
        public ConsoleColor DebugColor { get; set; }

        /// <summary>
        /// The color warning messages will be shown in when logging
        /// </summary>
        public ConsoleColor WarningColor { get; set; }

        /// <summary>
        /// The color error messages will be shown in when logging
        /// </summary>
        public ConsoleColor ErrorColor { get; set; }

        #endregion

        #region Private

        // The thread our logger works on
        private Thread LoggerThread { get; set; }

        // Event that is set when the logger is stopped
        private ManualResetEvent StopEvent { get; set; }

        // Event that is set when the logger has a message queued
        private ManualResetEvent ReadyEvent { get; set; }

        // A working queue of output messages
        private Queue<LogMessage> MessageQueue { get; set; }

        // A bool letting the logger know if it should ignore the rest of the queue when stopping
        private bool IgnoreQueue { get; set; }

        #endregion

        #endregion

        #region Methods

        #region Public

        /// <summary>
        /// Starts logging operations
        /// </summary>
        public void Start()
        {
            // Start the logging thread
            LoggerThread.Start();
        }

        /// <summary>
        /// Stops logging operations
        /// </summary>
        /// <param name="ForceStop">If this is set to true, all remaining queued messages will be dropped</param>
        public void Stop(bool ForceStop = false)
        {
            // Set whether or not the logger will ignore the remaining queue
            IgnoreQueue = ForceStop;

            // Stop the logging thread
            StopEvent.Set();
            LoggerThread.Join();
        }

        /// <summary>
        /// Logs an "info" message with the specified "important" message color
        /// </summary>
        /// <param name="Input">The value to write</param>
        /// <param name="Params">An array of objects to write using format</param>
        public void Important(object Input, params object[] Params)
        {
            // Check minimum log level
            if (LogLevel < LogLevel.IMPORTANT_ONLY) return;

            // Add message to queue
            AddMessage(LogLabel.IMPORTANT, Input, Params);
        }

        /// <summary>
        /// Logs an "info" message
        /// </summary>
        /// <param name="Input">The value to write</param>
        /// <param name="Params">An array of objects to write using format</param>
        public void WriteLine(object Input, params object[] Params)
        {
            // Check minimum log level
            if (LogLevel < LogLevel.DEFAULT) return;

            // Add message to queue
            AddMessage(LogLabel.INFO, Input, Params);
        }

        /// <summary>
        /// Logs an "error" message
        /// </summary>
        /// <param name="Input">The value to write</param>
        /// <param name="Params">An array of objects to write using format</param>
        public void Error(object Input, params object[] Params)
        {
            // Check minimum log level
            if (LogLevel < LogLevel.DEFAULT) return;

            // Add message to queue
            AddMessage(LogLabel.ERROR, Input, Params);
        }

        /// <summary>
        /// Logs a "warning" message
        /// </summary>
        /// <param name="Input">The value to write</param>
        /// <param name="Params">An array of objects to write using format</param>
        public void Warning(object Input, params object[] Params)
        {
            // Check minimum log level
            if (LogLevel < LogLevel.ENHANCED) return;

            // Add message to queue
            AddMessage(LogLabel.WARNING, Input, Params);
        }

        /// <summary>
        /// Logs a "debug" message
        /// </summary>
        /// <param name="Input">The value to write</param>
        /// <param name="Params">An array of objects to write using format</param>
        public void Debug(object Input, params object[] Params)
        {
            // Check minimum log level
            if (LogLevel < LogLevel.DEBUG) return;

            // Add message to queue
            AddMessage(LogLabel.DEBUG, Input, Params);
        }

        #endregion

        #region Private

        // Adds a log message to the message queue
        private void AddMessage(LogLabel Label, object Input, params object[] Params)
        {
            // Lock the message queue to prevent race conditions
            lock (MessageQueue)
            {
                // Add to message queue
                MessageQueue.Enqueue(new LogMessage
                {
                    Prefix = ShowPrefix,
                    Label = Label,
                    Timestamp = DateTime.Now,
                    Message = string.Format($"{Input}", Params)
                });

                // Signal ready event
                ReadyEvent.Set();
            }
        }

        // Writes a log message to the console, as well as to an optional log file
        // TODO - this is kind of a monolith, it could probably be shortened
        private void Write()
        {
            // Create a wait handle array so we can cancel this thread if need be
            WaitHandle[] Wait = new[] { ReadyEvent, StopEvent };
            while (0 == WaitHandle.WaitAny(Wait))
            {
                // Check if we need to stop and ignore queue
                if (StopEvent.WaitOne(0) && IgnoreQueue) break;

                // Lock the message queue to prevent race conditions
                lock (MessageQueue)
                {
                    // Check if there are any messages in the queue
                    if (MessageQueue.Count == 0)
                    {
                        ReadyEvent.Reset();
                        continue;
                    }

                    // Dequeue a message
                    var Message = MessageQueue.Dequeue();

                    // Check if logging is enabled
                    if (LogLevel <= LogLevel.NONE)
                    {
                        // Reset ready event
                        ReadyEvent.Reset();
                        continue;
                    }

                    // Create an output string builder
                    StringBuilder Output = new StringBuilder(Message.Message);

                    // Add time and label prefix
                    StringBuilder Prefix = new StringBuilder("");
                    if (Message.Prefix)
                    {
                        // Add time
                        if (LogLevel >= LogLevel.MAX)
                        {
                            Prefix.Append($"{Message.Timestamp.ToString("dd-MM-yyyy hh:mm:ss.ffffff")} [");
                        }
                        else if (LogLevel >= LogLevel.DEBUG)
                        {
                            Prefix.Append($"{Message.Timestamp.ToString("dd-MM-yyyy hh:mm:ss.ff")} [");
                        }
                        else
                        {
                            Prefix.Append($"{Message.Timestamp.ToString("dd-MM-yyyy hh:mm:ss")} [");
                        }

                        // Add custom prefix
                        if (!string.IsNullOrEmpty(CustomPrefix))
                        {
                            Prefix.Append($"{CustomPrefix} ");
                        }

                        // Add label
                        if (Message.Label != LogLabel.IMPORTANT)
                        {
                            Prefix.Append($"{Message.Label}]");
                        }
                        else
                        {
                            Prefix.Append($"{LogLabel.INFO}]");
                        }

                        // Pad prefix
                        if (LogLevel >= LogLevel.MAX)
                        {
                            Prefix.Append(' ', 37 - Prefix.Length);
                        }
                        else if (LogLevel >= LogLevel.DEBUG)
                        {
                            Prefix.Append(' ', 33 - Prefix.Length);
                        }
                        else
                        {
                            Prefix.Append(' ', 30 - Prefix.Length);
                        }
                        if (!string.IsNullOrEmpty(CustomPrefix)) Prefix.Append(' ');
                    }
                    Output.Insert(0, Prefix);

                    // Set console color
                    switch (Message.Label)
                    {
                        case LogLabel.IMPORTANT:
                            Console.ForegroundColor = ImportantColor;
                            break;
                        case LogLabel.INFO:
                            Console.ForegroundColor = InfoColor;
                            break;
                        case LogLabel.ERROR:
                            Console.ForegroundColor = ErrorColor;
                            break;
                        case LogLabel.WARNING:
                            Console.ForegroundColor = WarningColor;
                            break;
                        case LogLabel.DEBUG:
                            Console.ForegroundColor = DebugColor;
                            break;
                    }

                    // Write to console
                    Console.WriteLine(Output);

                    // Append to log file
                    if (!string.IsNullOrEmpty(LogFile))
                    {
                        try { File.AppendAllText(LogFile, $"{Output}\n"); }
                        catch { }
                    }

                    // Set console color back to default
                    Console.ForegroundColor = ConsoleColor.Gray;
                }
            }
        }

        #endregion

        #endregion

        #region Constructors

        /// <summary>
        /// Initialized a new logger instance
        /// </summary>
        public Logger()
        {
            // Setup variables
            MessageQueue = new Queue<LogMessage>();
            ReadyEvent = new ManualResetEvent(false);
            StopEvent = new ManualResetEvent(false);
            LoggerThread = new Thread(Write);
            ShowPrefix = true;
        }

        #endregion
    }
}
