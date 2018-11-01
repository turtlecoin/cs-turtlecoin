//
// Copyright (c) 2018 The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

using System;
using System.Linq;

using Canti.Utilities;
using Canti.Blockchain.Crypto;
using Canti.Blockchain.WalletBackend;
using Canti.Blockchain.Crypto.Mnemonics;

namespace CLIWallet
{
    public static class CommandImplementations
    {
        public static bool HandleCommand(string command, WalletBackend wallet)
        {
            switch(command)
            {
                case "advanced":
                {
                    Advanced(wallet);
                    break;
                }
                case "address":
                {
                    ConsoleMessage.WriteLine(ConsoleColor.DarkGreen, wallet.addresses[0]);
                    break;
                }
                case "balance":
                {
                    ConsoleMessage.WriteLine(ConsoleColor.Red, "Command not implemented yet...");
                    break;
                }
                case "backup":
                {
                    ExportKeys(wallet);
                    break;
                }
                case "exit":
                {
                    return Exit(wallet);
                }
                case "help":
                {
                    Help(wallet);
                    break;
                }
                case "transfer":
                {
                    ConsoleMessage.WriteLine(ConsoleColor.Red, "Command not implemented yet...");
                    break;
                }
                case "ab_add":
                {
                    ConsoleMessage.WriteLine(ConsoleColor.Red, "Command not implemented yet...");
                    break;
                }
                case "ab_delete":
                {
                    ConsoleMessage.WriteLine(ConsoleColor.Red, "Command not implemented yet...");
                    break;
                }
                case "ab_list":
                {
                    ConsoleMessage.WriteLine(ConsoleColor.Red, "Command not implemented yet...");
                    break;
                }
                case "ab_send":
                {
                    ConsoleMessage.WriteLine(ConsoleColor.Red, "Command not implemented yet...");
                    break;
                }
                case "change_password":
                {
                    ChangePassword(wallet);
                    break;
                }
                case "make_integrated_address":
                {
                    ConsoleMessage.WriteLine(ConsoleColor.Red, "Command not implemented yet...");
                    break;
                }
                case "incoming_transfers":
                {
                    ConsoleMessage.WriteLine(ConsoleColor.Red, "Command not implemented yet...");
                    break;
                }
                case "list_transfers":
                {
                    ConsoleMessage.WriteLine(ConsoleColor.Red, "Command not implemented yet...");
                    break;
                }
                case "optimize":
                {
                    ConsoleMessage.WriteLine(ConsoleColor.Red, "Command not implemented yet...");
                    break;
                }
                case "outgoing_transfers":
                {
                    ConsoleMessage.WriteLine(ConsoleColor.Red, "Command not implemented yet...");
                    break;
                }
                case "reset":
                {
                    ConsoleMessage.WriteLine(ConsoleColor.Red, "Command not implemented yet...");
                    break;
                }
                case "save":
                {
                    Save(wallet);
                    break;
                }
                case "save_csv":
                {
                        ConsoleMessage.WriteLine(ConsoleColor.Red, "Command not implemented yet...");
                        break;
                }
                case "send_all":
                {
                        ConsoleMessage.WriteLine(ConsoleColor.Red, "Command not implemented yet...");
                        break;
                }
                case "status":
                {
                        ConsoleMessage.WriteLine(ConsoleColor.Red, "Command not implemented yet...");
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
            ConsoleMessage.WriteLine(ConsoleColor.Yellow, "Saving wallet and shutting down...");
            wallet.Save();
            return true;
        }

        private static void Save(WalletBackend wallet)
        {
            ConsoleMessage.WriteLine(ConsoleColor.Yellow, "Saving...");
            wallet.Save();
            ConsoleMessage.WriteLine(ConsoleColor.Yellow, "Saved!");
        }

        private static void ChangePassword(WalletBackend wallet)
        {
            Utilities.ConfirmPassword(wallet);

            string newPassword;

            while (true)
            {
                ConsoleMessage.Write(ConsoleColor.Yellow, "Enter your new password: ");

                newPassword = Console.ReadLine();

                ConsoleMessage.Write(ConsoleColor.Yellow, "Confirm your new password: ");

                string confirmedPassword = Console.ReadLine();

                if (newPassword != confirmedPassword)
                {
                    ConsoleMessage.WriteLine(ConsoleColor.Red, "Passwords do not match! Try again.\n");
                    continue;
                }

                break;
            }

            wallet.password = newPassword;

            wallet.Save();

            ConsoleMessage.WriteLine(ConsoleColor.DarkGreen, "\nPassword successfully updated!");
        }

        private static void ExportKeys(WalletBackend wallet)
        {
            Utilities.ConfirmPassword(wallet);

            ConsoleMessage.WriteLine(ConsoleColor.Red, "The below data is PRIVATE and should not be given to anyone!");

            ConsoleMessage.WriteLine(ConsoleColor.Red, "If someone else gains access to these, they can steal all your funds!");

            ConsoleMessage.WriteLine("");

            if (wallet.isViewWallet)
            {
                ConsoleMessage.WriteLine(ConsoleColor.DarkGreen, "Private view key:");
                ConsoleMessage.WriteLine(ConsoleColor.DarkGreen, wallet.keys.privateViewKey.ToString());
                return;
            }

            ConsoleMessage.WriteLine(ConsoleColor.DarkGreen, "Private spend key:");
            ConsoleMessage.WriteLine(ConsoleColor.DarkGreen, wallet.keys.privateSpendKey.ToString());

            ConsoleMessage.WriteLine("");

            ConsoleMessage.WriteLine(ConsoleColor.DarkGreen, "Private view key:");
            ConsoleMessage.WriteLine(ConsoleColor.DarkGreen, wallet.keys.privateViewKey.ToString());

            if (KeyOps.AreKeysDeterministic(wallet.keys.privateSpendKey,
                                            wallet.keys.privateViewKey))
            {
                string mnemonic = Mnemonics.PrivateKeyToMnemonic(
                    wallet.keys.privateSpendKey
                );

                ConsoleMessage.WriteLine(ConsoleColor.DarkGreen, "\nMnemonic seed:");
                ConsoleMessage.WriteLine(ConsoleColor.DarkGreen, mnemonic);
            }
        }

        private static void Help(WalletBackend wallet)
        {
            if (wallet.isViewWallet)
            {
                Menu.PrintCommands(DefaultCommands.BasicViewWalletCommands());
            }
            else
            {
                Menu.PrintCommands(DefaultCommands.BasicCommands());
            }
        }

        private static void Advanced(WalletBackend wallet)
        {
            if (wallet.isViewWallet)
            {
                Menu.PrintCommands(
                    DefaultCommands.AdvancedViewWalletCommands(),
                    /* The offset to print the number from, e.g. help is
                       command numbers 1-7 or whatever, advanced is command
                       numbers 8-19 */
                    DefaultCommands.BasicViewWalletCommands().Count()
                );
            }
            else
            {
                Menu.PrintCommands(
                    DefaultCommands.AdvancedCommands(),
                    /* Offset */
                    DefaultCommands.BasicCommands().Count()
                );
            }
        }
    }
}
