//
// Copyright (c) 2018-2019 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

using Canti;

namespace CSTurtleCoin
{
    partial class Daemon
    {
        // All our launch arguments/options
        // Order is argument name, description, expected type, and an action delegate
        private static readonly CommandLineArguments Arguments = new CommandLineArguments()
        {
            // P2P Port
            {
                "p2p-port",
                "Which port to bind P2P server to",
                typeof(int),
                (Port) => P2pPort = Port
            },

            // API Port
            {
                "api-port",
                "Which port to bind API server to",
                typeof(int),
                (Port) => ApiPort = Port
            },

            // Log Level
            {
                "log-level",
                "Which log level to output at",
                typeof(int),
                (Level) =>
                {
                    // Level falls below the minimum value, default to minimum
                    if (Level < (int)LogLevel.NONE)
                    {
                        Configuration.LOG_LEVEL = LogLevel.NONE;
                    }

                    // Level falls above the maximum value, default to maximum
                    else if (Level > (int)LogLevel.MAX)
                    {
                        Configuration.LOG_LEVEL = LogLevel.MAX;
                    }

                    // Level falls within range, set to specified level
                    else Configuration.LOG_LEVEL = (LogLevel)Level;
                }
            },

            // Database Directory
            {
                "database-directory",
                "The directory to store database files in",
                typeof(string),
                (Location) => Configuration.LOCAL_STORAGE_DIRECTORY = Location
            }
        };
    }
}
