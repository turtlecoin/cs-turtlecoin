//
// Copyright (c) 2018 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

using Canti.Utilities;
using log4net;
using log4net.Config;
using System.IO;
using System.Reflection;

namespace Daemon
{
    class Program
    {
        static int Port = Daemon.P2pPort;
        static void Main(string[] args)
        {
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("Logger.config"));

            Logger.Log(Level.DEBUG, "Starting daemon...");


            // Parse commandline arguments
            if (args.Length >= 1) Port = int.Parse(args[0]);

            // Create a daemon connection
            Daemon Daemon = new Daemon(Port);

            // Start daemon
            Daemon.Start();
        }
    }
}
