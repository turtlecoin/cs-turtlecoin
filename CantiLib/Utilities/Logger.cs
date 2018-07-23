//
// Copyright (c) 2018 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace Canti.Utilities
{
    public enum Level : int
    {
        FATAL, ERROR, WARNING, INFO, DEBUG
    }

    public struct Entry
    {
        public DateTime Timestamp { get; set; }
        public Level Level { get; set; }
        public string Content { get; set; }
        public Entry(DateTime Timestamp, Level Level, string Content)
        {
            this.Timestamp = Timestamp;
            this.Level = Level;
            this.Content = Content;
        }
    }

    public class Logger
    {
        public string LogFile { get; private set; }
        public Level LogLevel { get; set; }
        private Queue<Entry> Entries { get; set; }
        private bool Running { get; set; }

        public Logger(string LogFile = null)
        {
            this.LogFile = LogFile;
            LogLevel = 0;
            Entries = new Queue<Entry>();
            Running = true;
        }

        public void Start()
        {
            Thread Thread = new Thread(delegate ()
            {
                while (true)
                {
                    if (Entries.Count == 0)
                    {
                        if (!Running) break;
                    }
                    else
                    {
                        Entry Entry = Entries.Dequeue();
                        if (LogLevel >= Entry.Level)
                        {
                            string Output = string.Format("{0} [{1}] {2}", Entry.Timestamp.ToShortTimeString(), Entry.Level, Entry.Content);
                            Console.WriteLine(Output);
                            if (LogFile != null) File.AppendAllText(LogFile, Output + Environment.NewLine);
                        }
                    }
                    Thread.Sleep(10);
                }
            });
            Thread.Start();
        }

        public void Stop()
        {
            Running = false;
        }

        public void Log(Level Level, string Content, params object[] Params)
        {
            Entries.Enqueue(new Entry(DateTime.Now.ToUniversalTime(), Level, string.Format(Content, Params)));
        }
    }
}
