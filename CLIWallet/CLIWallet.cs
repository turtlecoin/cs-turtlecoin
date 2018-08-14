//
// Copyright (c) 2018 The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

using System;

using Canti.Utilities;
using Canti.Blockchain;
using Canti.Blockchain.WalletBackend;

namespace CLIWallet
{
    class CLIWallet
    {
        public static void Main(string[] args)
        {
            StartupMsg();

            (bool exit, WalletBackend wallet) = Menu.SelectionScreen();

            if (!exit)
            {
                Menu.MainLoop(wallet);
            }

            Console.WriteLine("Bye.");
        }

        /* Writes out the coin name, version, and wallet name */
        private static void StartupMsg()
        {
            YellowMsg.WriteLine($"{Globals.coinName} {Globals.version} "
                              + $"{Globals.CLIWalletName}");
        }
    }
}
