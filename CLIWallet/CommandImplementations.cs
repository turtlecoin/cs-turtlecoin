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
                    Menu.PrintCommands(DefaultCommands.AdvancedCommands());
                    break;
                }
                case "address":
                {
                    GreenMsg.WriteLine(wallet.addresses[0]);
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
                    Menu.PrintCommands(DefaultCommands.BasicCommands());
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
                    ChangePassword(wallet);
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

        private static bool Exit(WalletBackend wallet)
        {
            YellowMsg.WriteLine("Saving wallet and shutting down...");
            wallet.Save();
            return true;
        }

        private static void Save(WalletBackend wallet)
        {
            YellowMsg.WriteLine("Saving...");
            wallet.Save();
            YellowMsg.WriteLine("Saved!");
        }

        private static void ChangePassword(WalletBackend wallet)
        {
            while (true)
            {
                YellowMsg.Write("Enter your current password: ");

                string currentPassword = Console.ReadLine();

                if (currentPassword != wallet.password)
                {
                    RedMsg.WriteLine("Incorrect password! Try again.\n");
                    continue;
                }

                break;
            }

            string newPassword;

            while (true)
            {
                YellowMsg.Write("Enter your new password: ");

                newPassword = Console.ReadLine();

                YellowMsg.Write("Confirm your new password: ");

                string confirmedPassword = Console.ReadLine();

                if (newPassword != confirmedPassword)
                {
                    RedMsg.WriteLine("Passwords do not match! Try again.\n");
                    continue;
                }

                break;
            }

            wallet.password = newPassword;

            wallet.Save();

            GreenMsg.WriteLine("\nPassword successfully updated!");
        }
    }
}
