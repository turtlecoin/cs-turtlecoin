//
// Copyright (c) 2018-2019 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

using Canti;
using Canti.CryptoNote;
using System;
using System.Threading;

namespace CSTurtleCoin
{
    partial class Daemon
    {
        // Our node instance, interfaced for future extensibility
        private static INode Node { get; set; }

        // The ports our node will bind to, set to default values
        private static int P2pPort = Configuration.P2P_DEFAULT_PORT;
        private static int ApiPort = Configuration.API_DEFAULT_PORT;

        // This event is set when the console detects a exit command
        private static ManualResetEvent StopEvent { get; set; }

        // Main application entry point
        private static void Main(string[] Args)
        {
            // Add exit command detection
            StopEvent = new ManualResetEvent(false);
            Console.CancelKeyPress += (sender, e) => {
                e.Cancel = true;
                StopEvent.Set();
            };

            // Parse command line arguments, returns false if any errors occured
            if (!CommandLineParser.Parse(Arguments, Args))
            {
                Console.WriteLine("Could not parse commandline arguments");
                Console.WriteLine("Press any key to exit");
                Console.ReadKey();
                return;
            }

            // Create a node instance
            Node = new Node(Configuration)
            {
                // Assign port values
                P2pPort = P2pPort,
                ApiPort = ApiPort
            };

            // Start the node, returns false if start fails
            if (!Node.Start())
            {
                Console.WriteLine("Could not start node");
                Console.WriteLine("Press any key to exit");
                Console.ReadKey();
                return;
            }

            // Manually add peers
            //Node.AddPeer("127.0.0.1", 11897);

            // Wait for exit command
            StopEvent.WaitOne();

            // Stops the node instance and all associated threads
            Node.Stop();
        }
    }
}
