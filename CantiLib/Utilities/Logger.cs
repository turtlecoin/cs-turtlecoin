//
// Copyright (c) 2018 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

namespace Canti.Utilities
{
    using System;
    using log4net;

    public enum LogLevel : int
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

        public static void Log(LogLevel level, string origin, string content, params object[] parameters)
        {
            try
            {
                string message = string.Format("{0} [{1}] {2}", DateTime.Now.ToUniversalTime(), level, string.Format(content, parameters));

                Log(level, origin, message);
            }
            catch (Exception ex)
            {
                // Logging failed.  Not much we can do.  Just try to continue.
                ex.Data.Clear();
            }
        }

        public static void Log(LogLevel level, string origin, string message)
        {
            try
            {
                switch (level)
                {
                    case LogLevel.DEBUG:
                        log4Net.Debug("[" + origin + "] " + message);
                        break;
                    case LogLevel.WARNING:
                        log4Net.Warn("[" + origin + "] " + message);
                        break;
                    case LogLevel.FATAL:
                        log4Net.Fatal("[" + origin + "] " + message);
                        break;
                    case LogLevel.ERROR:
                        log4Net.Error("[" + origin + "] " + message);
                        break;
                    case LogLevel.INFO:
                    default:
                        log4Net.Info("[" + origin + "] " + message);
                        break;
                }
            }
            catch (Exception ex)
            {
                // Logging failed.  Not much we can do.  Just try to continue.
                ex.Data.Clear();
            }
        }

        
        public static void LogException(string origin, Exception exception)
        {
            LogException(origin, "", exception);
        }

        public static void LogException(string origin, string message, Exception exception)
        {
            try
            {
                if (string.IsNullOrEmpty(message))
                {
                    log4Net.Error("[" + origin + "] ", exception);
                }
                else
                {
                    log4Net.Error("[" + origin + "] " + message, exception);
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