//
// Copyright (c) 2018-2019 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using static Canti.Utils;

namespace Canti.CryptoNote
{
    public sealed partial class Node
    {
        #region Properties and Fields

        #region Private

        // A threading timer that will send a sync packet request every so often
        private Timer SyncTimer { get; set; }

        #endregion

        #endregion

        #region Methods

        // Begins the sync process
        private void StartSync()
        {
            // Start our sync timer
            SyncTimer = new Timer(new TimerCallback(Sync), null, 0, Globals.P2P_TIMED_SYNC_INTERVAL * 1000);
        }

        // Stops the sync process
        private void StopSync()
        {
            // Dispose of our timer, clearing resources
            SyncTimer.Dispose();
        }

        // Sends a timed sync request to all connected peers
        private void Sync(object _)
        {
            // Lock our peer list to prevent any race conditions
            lock (PeerList)
            {
                // Loop through all connected peers
                foreach (var Peer in PeerList.Where(x => x.Validated))
                {
                    // Send this peer a timed sync request
                    RequestTimedSync(Peer);
                }
            }
        }

        // Gets core sync data for handshake and timed sync packets
        private Dictionary<string, dynamic> GetCoreSyncData()
        {
            return new Dictionary<string, dynamic>
            {
                ["current_height"] = (uint)Blockchain.Height,
                ["top_id"] = Blockchain.TopBlockHash
            };
        }

        // Builds a sparse chain array from synced data
        private byte[] GetSparseChain()
        {
            // Get sparse chain hashes from cache
            var Hashes = Blockchain.BuildSparseChain();

            // TODO - Test this?
            byte[] Output = new byte[0];
            foreach (var Hash in Hashes)
            {
                Output.AppendBytes(HexStringToByteArray(Hash));
            }

            // Return output
            return Output;
        }

        // Updates the current observed height
        private void UpdateObservedHeight(uint Height)
        {
            // Check that given height is higher than the current height
            if (Height < Blockchain.KnownHeight) return;

            // Set cache known height
            Blockchain.KnownHeight = Height;

            // Log debug message
            Logger.Debug($"Observed height updated, new height: {Blockchain.KnownHeight}");
        }

        // Handles core sync data sent by peers
        private void HandleSyncData(Peer Peer, PortableStorage SyncData)
        {
            // Update peer height
            Peer.Height = SyncData["current_height"];

            // Check if peer is syncing
            if (Peer.State == PeerState.SYNCHRONIZING) return;

            // Check if we have this block stored
            if (Blockchain.IsBlockStored(SyncData["top_id"]))
            {
                Peer.State = PeerState.NORMAL;
                return;
            }

            // Update observed height
            UpdateObservedHeight(Peer.Height);

            // Get the height difference between the local cache and remote node
            int Diff = (int)Blockchain.KnownHeight - (int)Blockchain.Height;
            int Days = Math.Abs(Diff) / (86_400 / Globals.CURRENCY_DIFFICULTY_TARGET);

            // Print how for behind/ahead we are
            StringBuilder Output = new StringBuilder($"[{Peer.Address}:{Peer.Port} {Peer.P2pPeer.Direction}] ");
            Output.Append($"Your {Globals.CURRENCY_NAME} node is syncing with the network");
            if (Diff >= 0)
            {
                Output.Append($" ({Math.Round((decimal)Blockchain.Height / Peer.Height * 100, 2)}% complete)");

                Output.Append($". You are {Diff} blocks ({Days} days) behind ");
            }
            else
            {
                Output.Append($". You are {Math.Abs(Diff)} blocks ({Days} days) ahead of ");
            }
            Output.Append("the current peer you're connected to. Slow and steady wins the race!");

            // If peer isn't validated, this is first contact
            if (!Peer.Validated)
            {
                Logger.Important(Output);
            }
            else
            {
                Logger.Debug(Output);
            }

            // Log debug message
            Logger.Debug($"Remote top block height: {Peer.Height}, id: {SyncData["top_id"]}");

            // TODO - this is just test code
            /*Blockchain.StoreBlock(new Block()
            {
                Height = Peer.Height,
                Hash = SyncData["top_id"]
            });*/

            // Set new peer state
            Peer.State = PeerState.SYNC_REQUIRED;

            // TODO - handle syncing
            Peer.State = PeerState.SYNCHRONIZING;
        }

        #endregion
    }
}
