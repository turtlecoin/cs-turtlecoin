//
// Copyright (c) 2018 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

namespace Canti.Utilities
{
    using System;
    using log4net;

    public enum Level : int
    {
        FATAL,
        ERROR,
        WARNING,
        INFO,
        DEBUG
    }

    public static class Logger
    {
        private static readonly ILog log4Net = LogManager.GetLogger(typeof(Logger));

        public static void Log(Level level, string content, params object[] parameters)
        {
            try
            {
                string message = string.Format("{0} [{1}] {2}", DateTime.Now.ToUniversalTime(), level, string.Format(content, parameters));

                //TODO: Move this elsewhere
                Console.WriteLine(message);

                switch (level)
                {
                    case Level.DEBUG:
                        log4Net.Debug(message);
                        break;
                    case Level.WARNING:
                        log4Net.Warn(message);
                        break;
                    case Level.FATAL:
                        log4Net.Fatal(message);
                        break;
                    case Level.ERROR:
                        log4Net.Error(message);
                        break;
                    case Level.INFO:
                    default:
                        log4Net.Info(message);
                        break;
                }
            }
            catch (Exception ex)
            {
                // Logging failed.  Not much we can do.  Just try to continue.
                ex.Data.Clear();
            }
        }
    }
}