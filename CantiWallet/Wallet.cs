//
// Copyright (c) 2019 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

using Canti.CryptoNote;
using System;
using System.IO;

namespace CantiWallet
{
    partial class Wallet
    {
        #region Properties and Fields

        // Wallet address prefix
        private static readonly ulong AddressPrefix = 3914525;

        // Wallet backend instance for handling a wallet file
        private static WalletBackend WalletBackend { get; set; }

        #endregion

        #region Methods

        // Creates a new wallet
        private static void CreateWallet()
        {
            // Get file path to write to
            Console.WriteLine("What would you like to name this wallet file?");
            string FileName = Console.ReadLine();

            // Append ".wallet" filetype if not already written
            if (!FileName.ToLower().EndsWith(".wallet"))
            {
                FileName += ".wallet";
            }

            // Check if file path is valid
            if (File.Exists(FileName))
            {
                Console.WriteLine("File already exists, could not create wallet");
                return;
            }

            // Get password to use for encryption
            Console.WriteLine("Enter a password for this wallet file:");
            string Password = Console.ReadLine();

            // Confirm password
            Console.WriteLine("Re-enter the password to confirm it:");
            if (Console.ReadLine() != Password)
            {
                Console.WriteLine("Passwords did not match, could not create wallet");
                return;
            }

            // Create wallet container
            WalletBackend.New(FileName, Password);

            // Create a subwallet
            var SubWallet = WalletBackend.CreateSubWallet();

            // Set wallet as loaded
            WalletLoaded = true;

            // Print wallet information
            Console.WriteLine($"Address: {SubWallet.Address}");
            Console.WriteLine($"Private Spend Key: {SubWallet.PrivateSpendKey}");
            Console.WriteLine($"Private View Key: {SubWallet.PrivateViewKey}");
        }

        // Loads a wallet from a file
        private static void LoadWallet(string FileName = "")
        {
            // Check if file path is pre-defined
            if (string.IsNullOrEmpty(FileName))
            {
                // Get file path to write to
                Console.WriteLine("What is the file path to the wallet you would like to load?");
                FileName = Console.ReadLine();

                // Append ".wallet" filetype if not already written
                if (!FileName.ToLower().EndsWith(".wallet"))
                {
                    FileName += ".wallet";
                }

                // Check if file path is valid
                if (!File.Exists(FileName))
                {
                    Console.WriteLine("File doesn't exist, could not load wallet");
                    return;
                }
            }

            // Get password to use for decryption
            Console.WriteLine("Enter a password for this wallet file:");
            string Password = Console.ReadLine();

            // Attempt to load wallet file
            if (!WalletBackend.Load(FileName, Password))
            {
                Console.WriteLine("Could not load wallet, please double-check the path and password");
                return;
            }

            // Load successful
            // TODO - better info output here
            Console.WriteLine("Load successful");

            // Set wallet as loaded
            WalletLoaded = true;
        }

        // Imports a wallet
        private static void ImportWallet()
        {

        }

        // Closes the application
        private static void SaveAndExit()
        {
            // Save the wallet
            WalletBackend.Save();

            // Exit the application
            Environment.Exit(0);
        }

        #endregion

        #region Entry Point

        private static void Main(string[] Args)
        {
            // Add exit command detection
            Console.CancelKeyPress += (sender, e) =>
            {
                e.Cancel = true;
                SaveAndExit();
            };

            // Initialize a wallet backend instance
            WalletBackend = new WalletBackend(AddressPrefix);

            // If there is only one commandline argument, first check if it's an existing wallet
            if (Args.Length == 1 && File.Exists(Args[0]))
            {
                LoadWallet(Args[0]);
            }

            // TODO - commandline arguments

            // Start showing the menu
            ShowMenu();
        }

        #endregion
    }
}
