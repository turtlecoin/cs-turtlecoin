//
// Copyright (c) 2019 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

using Canti.CryptoNote;
using System;

namespace CantiWallet
{
    partial class Wallet
    {
        // Whether or not a wallet has been loaded/cached
        private static bool WalletLoaded { get; set; }

        // Main wallet menu
        private static void ShowMainMenu()
        {
            // Show menu
            Console.WriteLine("Menu:");
            Console.WriteLine(" 1 - Check Balance");
            Console.WriteLine(" 2 - Send Transaction");
            Console.WriteLine(" 3 - Export Keys");
            Console.WriteLine(" 4 - Load Different Wallet");
            Console.WriteLine(" 5 - Save and Exit");

            // Wait for a menu selection
            switch (Console.ReadLine().ToLower())
            {
                case "1":
                    // TODO
                    break;

                case "2":
                    // TODO
                    break;

                case "3":
                    // TODO
                    break;

                case "4":
                    WalletBackend = new WalletBackend(AddressPrefix);
                    WalletLoaded = false;
                    break;

                case "5":
                    SaveAndExit();
                    break;
            }
        }

        // Open wallet menu
        private static void ShowOpenWalletMenu()
        {
            // Show menu
            Console.WriteLine("Menu:");
            Console.WriteLine(" 1 - Create Wallet");
            Console.WriteLine(" 2 - Load Wallet");
            Console.WriteLine(" 3 - Import Wallet");
            Console.WriteLine(" 4 - Exit");

            // Wait for a menu selection
            switch (Console.ReadLine().ToLower())
            {
                case "1":
                    CreateWallet();
                    break;

                case "2":
                    LoadWallet();
                    break;

                case "3":
                    ImportWallet();
                    break;

                case "4":
                    SaveAndExit();
                    break;
            }
        }

        // Shows commandline menu and waits for a menu choice
        private static void ShowMenu()
        {
            // Loop until a stop event is detected
            while (true)
            {
                if (WalletLoaded) ShowMainMenu();
                else ShowOpenWalletMenu();
            }
        }
    }
}
