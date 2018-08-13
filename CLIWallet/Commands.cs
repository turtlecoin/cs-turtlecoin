//
// Copyright (c) 2018 The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

using System.Linq;
using System.Collections.Generic;

using Canti.Blockchain;

namespace CLIWallet
{
    public class Command
    {
        public Command(string commandName, string description)
        {
            this.commandName = commandName;
            this.description = description;
        }

        /* The name of the command, e.g. transfer, balance */
        public string commandName { get; }

        /* The description of the command, e.g. "Send TRTL to someone" */
        public string description { get; }
    }

    public class AdvancedCommand : Command
    {
        public AdvancedCommand(string commandName, string description,
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
        public static List<Command> StartupCommands()
        {
            return new List<Command>
            {
                new Command("open", "Open a wallet already on your system"),
                new Command("create", "Create a new wallet"),
                new Command("seed_restore", "Restore a wallet using a seed phrase of words"),
                new Command("key_restore", "Restore a wallet using a view and spend key"),
                new Command("view_wallet", "Import a view only wallet (Unable to send transactions)"),
                new Command("exit", "Exit the program"),
            };
        }

        public static List<AdvancedCommand> AllCommands()
        {
            return new List<AdvancedCommand>
            {
                /* Basic commands */
                new AdvancedCommand("advanced", "List available advanced commands", true, false),
                new AdvancedCommand("address", "Display your payment address", true, false),
                new AdvancedCommand("balance", $"Display how much {Globals.ticker} you have", true, false),
                new AdvancedCommand("backup", "Print the relevant data needed to restore your wallet", true, false),
                new AdvancedCommand("exit", "Exit and save your wallet", true, false),
                new AdvancedCommand("help", "List this help message", true, false),
                new AdvancedCommand("transfer", $"Send {Globals.ticker} to someone", false, false),
                
                /* Advanced commands */
                new AdvancedCommand("ab_add", "Add a person to your address book", true, true),
                new AdvancedCommand("ab_delete", "Delete a person in your address book", true, true),
                new AdvancedCommand("ab_list", "List everyone in your address book", true, true),
                new AdvancedCommand("ab_send", $"Send {Globals.ticker} to someone in your address book", false, true),
                new AdvancedCommand("change_password", "Change your wallet password", true, true),
                new AdvancedCommand("make_integrated_address", "Make an integrated address from an address + payment ID", true, true),
                new AdvancedCommand("incoming_transfers", "Show incoming transfers", true, true),
                new AdvancedCommand("list_transfers", "Show all transfers", false, true),
                new AdvancedCommand("optimize", "Optimize your wallet to send large amounts", false, true),
                new AdvancedCommand("outgoing_transfers", "Show outgoing transfers", false, true),
                new AdvancedCommand("reset", "Recheck the chain from zero for transactions", true, true),
                new AdvancedCommand("save", "Save your wallet state", true, true),
                new AdvancedCommand("save_csv", "Save all wallet transactions to a CSV file", true, true),
                new AdvancedCommand("send_all", "Send all your balance to someone", false, true),
                new AdvancedCommand("status", "Display sync status and network hashrate", true, true),
            };
        }

        public static IEnumerable<AdvancedCommand> BasicCommands()
        {
            return AllCommands().Where(c => !c.advanced);
        }

        public static IEnumerable<AdvancedCommand> AdvancedCommands()
        {
            return AllCommands().Where(c => c.advanced);
        }
    }
}
