//
// Copyright (c) 2018 The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

using System;

using Canti.Utilities;
using Canti.Blockchain.WalletBackend;

namespace CLIWallet
{
    public class CommandImplementations
    {
        public static bool HandleCommand(string command, WalletBackend wallet)
        {
            switch(command)
            {
                case "advanced":
                {
                    RedMsg.WriteLine("Command not implemented yet...");
                    break;
                }
                case "address":
                {
                    YellowMsg.WriteLine(wallet.addresses[0]);
                    break;
                }
                case "balance":
                {
                    RedMsg.WriteLine("Command not implemented yet...");
                    break;
                }
                case "backup":
                {
                    RedMsg.WriteLine("Command not implemented yet...");
                    break;
                }
                case "exit":
                {
                    return Exit(wallet);
                }
                case "help":
                {
                    RedMsg.WriteLine("Command not implemented yet...");
                    break;
                }
                case "transfer":
                {
                    RedMsg.WriteLine("Command not implemented yet...");
                    break;
                }
                case "ab_add":
                {
                    RedMsg.WriteLine("Command not implemented yet...");
                    break;
                }
                case "ab_delete":
                {
                    RedMsg.WriteLine("Command not implemented yet...");
                    break;
                }
                case "ab_list":
                {
                    RedMsg.WriteLine("Command not implemented yet...");
                    break;
                }
                case "ab_send":
                {
                    RedMsg.WriteLine("Command not implemented yet...");
                    break;
                }
                case "change_password":
                {
                    RedMsg.WriteLine("Command not implemented yet...");
                    break;
                }
                case "make_integrated_address":
                {
                    RedMsg.WriteLine("Command not implemented yet...");
                    break;
                }
                case "incoming_transfers":
                {
                    RedMsg.WriteLine("Command not implemented yet...");
                    break;
                }
                case "list_transfers":
                {
                    RedMsg.WriteLine("Command not implemented yet...");
                    break;
                }
                case "optimize":
                {
                    RedMsg.WriteLine("Command not implemented yet...");
                    break;
                }
                case "outgoing_transfers":
                {
                    RedMsg.WriteLine("Command not implemented yet...");
                    break;
                }
                case "reset":
                {
                    RedMsg.WriteLine("Command not implemented yet...");
                    break;
                }
                case "save":
                {
                    Save(wallet);
                    break;
                }
                case "save_csv":
                {
                    RedMsg.WriteLine("Command not implemented yet...");
                    break;
                }
                case "send_all":
                {
                    RedMsg.WriteLine("Command not implemented yet...");
                    break;
                }
                case "status":
                {
                    RedMsg.WriteLine("Command not implemented yet...");
                    break;
                }
                /* This should never happen */
                default:
                {
                    throw new NotImplementedException(
                        "Command was defined but not hooked up!"
                    );
                }
            }

            /* Don't exit */
            return false;
        }

        public static bool Exit(WalletBackend wallet)
        {
            YellowMsg.WriteLine("Saving wallet and shutting down...");
            wallet.Save();
            return true;
        }

        public static void Save(WalletBackend wallet)
        {
            YellowMsg.WriteLine("Saving...");
            wallet.Save();
            YellowMsg.WriteLine("Saved!");
        }
    }
}
