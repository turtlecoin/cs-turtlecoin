//
// Copyright (c) 2018 The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

using System;

using Canti.Utilities;
using Canti.Blockchain.WalletBackend;

namespace CLIWallet
{
    public static class Utilities
    {
        public static void ConfirmPassword(WalletBackend wallet)
        {
            while(true)
            {
                YellowMsg.Write("Confirm your current password: ");

                string password = Console.ReadLine();

                if (password != wallet.password)
                {
                    RedMsg.WriteLine("Incorrect password! Try again.\n");
                    continue;
                }

                return;
            }
        }
    }
}
