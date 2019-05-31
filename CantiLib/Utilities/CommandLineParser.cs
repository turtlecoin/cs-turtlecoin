//
// Copyright (c) 2018-2019 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Canti
{
    /// <summary>
    /// Contains information about a command line argument/option
    /// </summary>
    public sealed class CommandLineOption
    {
        #region Properties and Fields

        /// <summary>
        /// A description of what this option is for
        /// </summary>
        internal string Description { get; set; }

        /// <summary>
        /// The expected value type for this option
        /// </summary>
        internal Type Type { get; set; }

        /// <summary>
        /// A callback method that is invoked when this option is found
        /// </summary>
        internal dynamic Handler { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new command line argument/option
        /// </summary>
        internal CommandLineOption(string Description, Type Type, Action<dynamic> Handler)
        {
            this.Description = Description;
            this.Type = Type;
            this.Handler = Handler;
        }

        #endregion
    }

    /// <summary>
    /// A dictionary of command line arguments and callbacks
    /// </summary>
    public sealed class CommandLineArguments : Dictionary<string, CommandLineOption>
    {
        #region Methods

        /// <summary>
        /// Adds a command line argument to the list of options
        /// </summary>
        /// <param name="Option">The flag name to look for while parsing</param>
        /// <param name="Description">A description of what this option is for</param>
        /// <param name="Type">The expected value type for this option</param>
        /// <param name="Handler">A callback method that is invoked when this option is found</param>
        public void Add(string Option, string Description, Type Type, Action<dynamic> Handler)
        {
            Add(Option, new CommandLineOption(Description, Type, Handler));
        }

        #endregion

        #region Constructors

        /// <summary>
        /// A dictionary of command line arguments, entries are case-insensitive
        /// </summary>
        public CommandLineArguments() : base(StringComparer.OrdinalIgnoreCase) { }

        #endregion
    }

    /// <summary>
    /// A command line argument parser utility class that decodes launch arguments
    /// </summary>
    public sealed class CommandLineParser
    {
        #region Methods

        /// <summary>
        /// Parses an arguments array and looks for a set of specified command line arguments
        /// </summary>
        /// <param name="Args">A dictionary of commandline arguments and callbacks</param>
        /// <param name="Input">An array of arguments a program was launched with</param>
        /// <returns></returns>
        public static bool Parse(CommandLineArguments Args, string[] Input)
        {
            // Set a read state - this could be an enum, but... *shrug*
            // 0 = option, 1 = value, 2, process
            int ReadState = 0;
            string Option = null;

            // Check for a help command
            if (Input.Contains("-help", StringComparer.OrdinalIgnoreCase))
            {
                // Show argument descriptions
                ShowDescriptions(Args);
                return false;
            }
            else
            {
                // Iterate through all strings in input array
                foreach (string Arg in Input)
                {
                    // Looking for an option
                    if (ReadState == 0 && Arg.StartsWith("-"))
                    {
                        // Get option name
                        Option = Arg.Substring(1);

                        // Ignore if this option is not in our argument list
                        if (!Args.ContainsKey(Option)) continue;

                        // Set read state to look for a value
                        ReadState = 1;
                    }

                    // Looking for a value
                    else if (ReadState == 1)
                    {
                        // Attempt to process this option's value
                        try
                        {
                            // Declare a converted value object
                            dynamic ConvertedValue;

                            // Check if the expected type has a parse method
                            var Parse = Args[Option].Type.GetMethod("Parse", new[] { typeof(string) });
                            if (Parse != null)
                            {
                                ConvertedValue = Parse.Invoke(null, new object[] { Arg });
                            }

                            // Attempt to change type from string directly
                            else
                            {
                                ConvertedValue = Convert.ChangeType(Arg, Args[Option].Type);
                            }

                            // Invoke this option's handler
                            Args[Option].Handler.Invoke(ConvertedValue);

                            // Remove this option entry
                            Args.Remove(Option);

                            // Reset read state to look for next option
                            ReadState = 0;
                        }

                        // Attempt failed, this value was of the wrong type
                        catch
                        {
                            Console.WriteLine($"Invalid value for option flag -{Option} " +
                                $"(expecting type {Args[Option].Type.Name})");
                            return false;
                        }
                    }
                }
            }

            // If read state does not end on an option state, arguments were invalid
            if (ReadState != 0)
            {
                Console.WriteLine($"Could not process command line arguments (invalid syntax)");
                return false;
            }

            // If read state ends on an option state, parsing was successful
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Displays all arguments, descriptions, types, and an example for each argument flag
        /// </summary>
        /// <param name="Args">The dictionary of command line arguments to display</param>
        public static void ShowDescriptions(CommandLineArguments Args)
        {
            // Print header
            Console.WriteLine("Arguments:");

            // Iterate through all given argument options
            foreach (var Arg in Args)
            {
                // Print a new line
                Console.WriteLine();

                // Print name
                Console.WriteLine($"-{Arg.Key}:");

                // Print description
                Console.WriteLine($"  Description: {Arg.Value.Description}");

                // Print expected type
                Console.WriteLine($"  Expected Value Type: {Arg.Value.Type.Name}");

                // Print syntax
                if (Arg.Value.Type.IsValueType)
                {
                    Console.WriteLine($"  Example: -{Arg.Key} {Activator.CreateInstance(Arg.Value.Type)}");
                }
                else if (Arg.Value.Type == typeof(string))
                {
                    Console.WriteLine($"  Example: -{Arg.Key} \"value\"");
                }
            }
        }

        #endregion
    }
}
