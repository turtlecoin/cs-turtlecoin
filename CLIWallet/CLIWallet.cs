//
// Copyright (c) 2018 The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

using System;
using System.Linq;
using System.Collections.Generic;

using Canti.Errors;
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

            (bool exit, WalletBackend wallet) = SelectionScreen();

            if (!exit)
            {
                MainLoop(wallet);
            }

            Console.WriteLine("Bye.");
        }

        private static void MainLoop(WalletBackend wallet)
        {
            while (true)
            {
                Console.WriteLine();

                string command = ParseCommand(DefaultCommands.BasicCommands(),
                                              DefaultCommands.AllCommands(),
                                              GetPrompt(wallet));
            }
        }

        /* TODO: Better way to do this */
        private static string GetPrompt(WalletBackend wallet)
        {
            int promptLength = 20;

            string extension = ".wallet";

            string walletName = wallet.filename;

            /* Remove extension if it exists */
            if (walletName.EndsWith(extension))
            {
                walletName = walletName.Remove(
                    walletName.LastIndexOf(extension)
                );
            }

            /* Trim to 20 chars */
            walletName = new string(walletName.Take(promptLength).ToArray());

            return $"\n[{Globals.ticker} {walletName}]: ";
        }

        private static (bool exit, WalletBackend wallet) SelectionScreen()
        {
            while (true)
            {
                LaunchAction action = GetAction();

                if (action == LaunchAction.Exit)
                {
                    return (true, null);
                }

                /* Get the walletbackend, or an error. On error, return
                   to selection screen. */
                switch(LaunchWallet.HandleAction(action))
                {
                    case ILeft<Error> error:
                    {
                        RedMsg.WriteLine(
                            "Failed to start wallet: {error.Value.errorMessage}"
                        );

                        Console.WriteLine("Returning to selection screen.");

                        continue;
                    }
                    case IRight<WalletBackend> wallet:
                    {
                        return (false, wallet.Value);
                    }
                }
            }
        }

        /* Writes out the coin name, version, and wallet name */
        private static void StartupMsg()
        {
            YellowMsg.WriteLine($"{Globals.coinName} {Globals.version} "
                              + $"{Globals.CLIWalletName}\n");
        }

        /* printableCommands = the commands to print on bad input.
           availableCommands = the commands that the inputted string is
                               checked against.

           For example, we print basic commands, but can input both basic
           and advanced commands */
        private static string ParseCommand(
                                IEnumerable<Command> printableCommands,
                                IEnumerable<Command> availableCommands,
                                string prompt)
        {
            string selection = null;

            while (true)
            {
                int i = 1;

                /* Print out each command name, description, and possible
                   number accessor */
                foreach (var command in printableCommands)
                {
                    YellowMsg.Write($" {i}\t");
                    GreenMsg.Write(command.commandName.PadRight(25));
                    Console.WriteLine(command.description);
                    i++;
                }

                /* Write the prompt message */
                YellowMsg.Write(prompt);

                selection = Console.ReadLine().ToLower();

                /* \n == no-op */
                if (selection == "")
                {
                    Console.WriteLine();
                    continue;
                }

                /* Check if they entered a command or an number */
                if (int.TryParse(selection, out int selectionNum))
                {
                    /* We print 1, 2, 3, 4 etc to be user friendly but we want
                       to access it with 0 indexing */
                    selectionNum--;

                    if (selectionNum < 0 ||
                        selectionNum >= availableCommands.Count())
                    {
                        Console.WriteLine("Bad input, expected a command " +
                                          "name, or number from 1 to " +
                                         $"{availableCommands.Count()}\n");

                        continue;
                    }

                    /* Set the selection to the command name chosen via
                       the previously printed numbers */
                    /* TODO: Can we directly select an IEnumerable? */
                    selection = availableCommands.ToList()[selectionNum]
                                                 .commandName;
                }
                else
                {
                    /* Does the inputted command exist? */
                    if (!availableCommands.Any(c => c.commandName == selection))
                    {
                        Console.Write("Unknown command: ");
                        RedMsg.WriteLine($"{selection}\n");
                        continue;
                    }
                }

                return selection;
            }
        }

        /* Get the startup action, e.g. open, create */
        private static LaunchAction GetAction()
        {
            /* Grab the command list */
            var commands = DefaultCommands.StartupCommands();

            string command = ParseCommand(commands, commands,
                                          "\n[What would you like to do?]: ");

            switch(command)
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

    /* Possible actions the user can do on startup */
    public enum LaunchAction
    {
        Open, Create, SeedRestore, KeyRestore, ViewWallet, Exit
    };
}
