//
// Copyright (c) 2018 The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

using System;
using System.Linq;
using System.Collections.Generic;

using Canti.Utilities;
using Canti.Blockchain;

namespace CLIWallet
{
    /* Possible actions the user can do on startup */
    public enum LaunchAction
    {
        Open, Create, SeedRestore, KeyRestore, ViewWallet, Exit
    };

    class CLIWallet
    {
        public static void Main(string[] args)
        {
            StartupMsg();

            while (true)
            {
                LaunchAction action = GetAction();

                if (action == LaunchAction.Exit)
                {
                    Console.WriteLine("Bye.");
                    return;
                }

                /* Get the walletbackend, or an error. On error, return
                   to selection screen. */
                bool exit = LaunchWallet.HandleAction(action).Select(
                    error => {
                        RedMsg.WriteLine(
                            "Failed to start wallet: {error.errorMessage}"
                        );

                        Console.WriteLine("Returning to selection screen.");

                        return false;
                    },

                    wallet => {
                        /* Do something with the wallet */
                        Console.WriteLine("Got wallet");
                        Console.WriteLine("Bye.");

                        return true;
                    }
                );

                /* Else go back to the selection screen */
                if (exit)
                {
                    return;
                }
            }
        }

        /* Writes out the coin name, version, and wallet name */
        private static void StartupMsg()
        {
            YellowMsg.WriteLine($"{Globals.coinName} {Globals.version} "
                              + $"{Globals.CLIWalletName}\n");
        }

        /* Get the startup action, e.g. open, create */
        private static LaunchAction GetAction()
        {
            /* Grab the command list */
            var commands = DefaultCommands.StartupCommands();

            string selection = null;

            while (true)
            {
                int i = 1;

                /* Print out each command name, description, and possible
                   number accessor */
                foreach (var command in commands)
                {
                    YellowMsg.Write($" {i}\t");
                    GreenMsg.Write(command.commandName.PadRight(25));
                    Console.WriteLine(command.description);
                    i++;
                }

                /* Write the prompt message */
                YellowMsg.Write("\n[What would you like to do?]: ");

                selection = Console.ReadLine().ToLower();

                /* Check if they entered a command or an number */
                if (int.TryParse(selection, out int selectionNum))
                {
                    /* We print 1, 2, 3, 4 etc to be user friendly but we want
                       to access it with 0 indexing */
                    selectionNum--;

                    if (selectionNum < 0 || selectionNum >= commands.Count)
                    {
                        Console.WriteLine("Bad input, expected a command " +
                                          "name, or number from 1 to " +
                                         $"{commands.Count}\n");

                        continue;
                    }

                    /* Set the selection to the command name chosen via
                       the previously printed numbers */
                    selection = commands[selectionNum].commandName;
                }
                else
                {
                    /* Does the inputted command exist? */
                    if (!commands.Any(c => c.commandName == selection))
                    {
                        Console.Write("Unknown command: ");
                        RedMsg.WriteLine($"{selection}\n");
                        continue;
                    }
                }

                break;
            }

            switch(selection)
            {
                case "create":
                {
                    return LaunchAction.Create;
                }
                case "open":
                {
                    return LaunchAction.Open;
                }
                case "seed_restore":
                {
                    return LaunchAction.SeedRestore;
                }
                case "key_restore":
                {
                    return LaunchAction.KeyRestore;
                }
                case "view_wallet":
                {
                    return LaunchAction.ViewWallet;
                }
                case "exit":
                {
                    return LaunchAction.Exit;
                }
                /* This should never happen */
                default:
                {
                    throw new NotImplementedException(
                        "Command was defined but not hooked up!"
                    );
                }
            }
        }
    }
}
