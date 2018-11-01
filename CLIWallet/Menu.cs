//
// Copyright (c) 2018 The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using Canti.Errors;
using Canti.Utilities;
using Canti.Blockchain;
using Canti.Blockchain.WalletBackend;

namespace CLIWallet
{
    public static class Menu
    {
        public static void MainLoop(WalletBackend wallet)
        {
            /* Show the available commands */
            if (wallet.isViewWallet)
            {
                PrintCommands(DefaultCommands.BasicViewWalletCommands());
            }
            else
            {
                PrintCommands(DefaultCommands.BasicCommands());
            }

            while (true)
            {
                string command;

                if (wallet.isViewWallet)
                {
                    command = ParseCommand(
                        DefaultCommands.BasicViewWalletCommands(),
                        DefaultCommands.AllViewWalletCommands(),
                        GetPrompt(wallet)
                    );
                }
                else
                {
                    command = ParseCommand(
                        DefaultCommands.BasicCommands(),
                        DefaultCommands.AllCommands(),
                        GetPrompt(wallet)
                    );
                }

                /* If exit command is given, quit */
                if (CommandImplementations.HandleCommand(command, wallet))
                {
                    return;
                }
            }
        }

        private static string GetPrompt(WalletBackend wallet)
        {
            int promptLength = 20;

            /* Remove extension if it exists */
            string walletName = Path.ChangeExtension(wallet.filename, null);

            /* Trim to 20 chars */
            walletName = string.Concat(walletName.Take(promptLength));

            return $"[{Globals.ticker} {walletName}]: ";
        }

        public static (bool exit, WalletBackend wallet) SelectionScreen()
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
                        ConsoleMessage.WriteLine(ConsoleColor.Red, error.Value.errorMessage);

                        ConsoleMessage.WriteLine(ConsoleColor.Yellow, "Returning to selection screen.\n");

                        continue;
                    }
                    case IRight<WalletBackend> wallet:
                    {
                        return (false, wallet.Value);
                    }
                }
            }
        }

        public static void PrintCommands(IEnumerable<Command> commands,
                                         /* The offset to print the command
                                            numbers at, e.g. command 1, 2, 3
                                            we need an offset if we're
                                            printing the advanced commands */
                                         int offset = 0)
        {
            int i = 1 + offset;

            ConsoleMessage.WriteLine("");

            /* Print out each command name, description, and possible
               number accessor */
            foreach (var command in commands)
            {
                ConsoleMessage.Write(ConsoleColor.Yellow, $" {i}\t");
                ConsoleMessage.Write(ConsoleColor.DarkGreen, command.commandName.PadRight(25));
                ConsoleMessage.WriteLine(command.description);
                i++;
            }

            ConsoleMessage.WriteLine("");
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
                /* Write the prompt message */
                ConsoleMessage.Write(ConsoleColor.Yellow, prompt);

                selection = Console.ReadLine().ToLower();

                /* \n == no-op */
                if (selection == "")
                {
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
                        ConsoleMessage.Write(ConsoleColor.Red, "Bad input, expected a command name, or number from ");
                        ConsoleMessage.Write(ConsoleColor.Yellow, "1 ");
                        ConsoleMessage.Write(ConsoleColor.Red, "to ");
                        ConsoleMessage.WriteLine(ConsoleColor.Yellow, availableCommands.Count().ToString());

                        PrintCommands(printableCommands);

                        continue;
                    }

                    /* Set the selection to the command name chosen via
                       the previously printed numbers */
                    selection = availableCommands.ElementAt(selectionNum)
                                                 .commandName;
                }
                else
                {
                    /* Does the inputted command exist? */
                    if (!availableCommands.Any(c => c.commandName == selection))
                    {
                        ConsoleMessage.Write("Unknown command: ");
                        ConsoleMessage.Write(ConsoleColor.Red, $"{selection}\n");

                        PrintCommands(printableCommands);

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

            PrintCommands(commands);

            string command = ParseCommand(commands, commands,
                                          "[What would you like to do?]: ");

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
