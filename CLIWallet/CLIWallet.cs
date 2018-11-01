//
// Copyright (c) 2018 The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

using System;

using Canti.Utilities;
using Canti.Blockchain;
using Canti.Blockchain.WalletBackend;
using log4net;
using System.Reflection;
using log4net.Config;
using System.IO;

namespace CLIWallet
{
    public static class CLIWallet
    {
        public static void Main(string[] args)
        {
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("Logger.config"));

            Logger.Log(LogLevel.DEBUG, "CLIWallet.Main", "Starting daemon...");

            try
            {
                StartupMsg();

                (bool exit, WalletBackend wallet) = Menu.SelectionScreen();

                if (!exit)
                {
                    Menu.MainLoop(wallet);
                }
            }
            catch (Exception ex)
            {
                Logger.LogException("CLIWallet.Main", ex);
            }

            Logger.Log(LogLevel.DEBUG, "CLIWallet.Main", "Exiting daemon...");
        }

        /* Writes out the coin name, version, and wallet name */
        private static void StartupMsg()
        {
            ConsoleMessage.WriteLine(ConsoleColor.Yellow, $"{Globals.coinName} {Globals.version} {Globals.CLIWalletName}");
        }
    }
}
