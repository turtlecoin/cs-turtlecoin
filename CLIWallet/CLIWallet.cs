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
    class CLIWallet
    {
        public static void Main(string[] args)
        {
            StartupMsg();

            Action action = GetAction();
        }

        /* Writes out the coin name, version, and wallet name */
        private static void StartupMsg()
        {
            YellowMsg.WriteLine($"{Globals.coinName} {Globals.version} "
                              + $"{Globals.CLIWalletName}\n");
        }

        /* Possible actions the user can do on startup */
        private enum Action
        {
            Open, Create, SeedRestore, KeyRestore, ViewWallet, Exit
        };

        private static Action GetAction()
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
                    return Action.Create;
                }
                case "open":
                {
                    return Action.Open;
                }
                case "seed_restore":
                {
                    return Action.SeedRestore;
                }
                case "key_restore":
                {
                    return Action.KeyRestore;
                }
                case "view_wallet":
                {
                    return Action.ViewWallet;
                }
                case "exit":
                {
                    return Action.Exit;
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
