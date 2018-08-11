//
// Copyright (c) 2018 The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

using System.Collections.Generic;

namespace CLIWallet
{
    public class OpenCommand
    {
        public OpenCommand(string commandName, string description)
        {
            this.commandName = commandName;
            this.description = description;
        }

        /* The name of the command, e.g. transfer, balance */
        public string commandName { get; }

        /* The description of the command, e.g. "Send TRTL to someone" */
        public string description { get; }
    }

    public class Command : OpenCommand
    {
        public Command(string commandName, string description,
                       bool viewWalletSupport, bool advanced)
                     : base(commandName, description)
        {
            this.viewWalletSupport = viewWalletSupport;
            this.advanced = advanced;
        }

        /* Can the command be used by a view wallet, e.g. transfer
           cannot but balance can */
        public bool viewWalletSupport { get; }

        /* Is the command an advanced command, that we hide in
           the advanced menu */
        public bool advanced { get; }
    }

    public static class DefaultCommands
    {
        public static List<OpenCommand> StartupCommands()
        {
            return new List<OpenCommand>
            {
                new OpenCommand("open", "Open a wallet already on your system"),
                new OpenCommand("create", "Create a new wallet"),
                new OpenCommand("seed_restore", "Restore a wallet using a seed phrase of words"),
                new OpenCommand("key_restore", "Restore a wallet using a view and spend key"),
                new OpenCommand("view_wallet", "Import a view only wallet (Unable to send transactions)"),
                new OpenCommand("exit", "Exit the program"),
            };
        }

        public static List<Command> AllCommands()
        {
            return new List<Command>
            {
                /* Basic commands */
                new Command("advanced", "List available advanced commands", true, false),
                new Command("address", "Display your payment address", true, false),
                new Command("balance", "Display how much {Globals.ticker} you have", true, false),
                new Command("backup", "Print the relevant data needed to restore your wallet", true, false),
                new Command("exit", "Exit and save your wallet", true, false),
                new Command("help", "List this help message", true, false),
                new Command("transfer", "Send {Globals.ticker} to someone", false, false),
                
                /* Advanced commands */
                new Command("ab_add", "Add a person to your address book", true, true),
                new Command("ab_delete", "Delete a person in your address book", true, true),
                new Command("ab_list", "List everyone in your address book", true, true),
                new Command("ab_send", "Send {Globals.ticker} to someone in your address book", false, true),
                new Command("change_password", "Change your wallet password", true, true),
                new Command("make_integrated_address", "Make an integrated address from an address + payment ID", true, true),
                new Command("incoming_transfers", "Show incoming transfers", true, true),
                new Command("list_transfers", "Show all transfers", false, true),
                new Command("optimize", "Optimize your wallet to send large amounts", false, true),
                new Command("outgoing_transfers", "Show outgoing transfers", false, true),
                new Command("reset", "Recheck the chain from zero for transactions", true, true),
                new Command("save", "Save your wallet state", true, true),
                new Command("save_csv", "Save all wallet transactions to a CSV file", true, true),
                new Command("send_all", "Send all your balance to someone", false, true),
                new Command("status", "Display sync status and network hashrate", true, true),
            };
        }
    }
}
