//
// Copyright (c) 2018-2019 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using static Canti.Utils;

namespace Canti.CryptoNote
{
    // TODO - discovery needs further testing in general
    public sealed partial class Node
    {
        #region Properties and Fields

        #region Private

        // Contains a list of peer candidates
        private List<PeerCandidate> PeerCandidates { get; set; }

        // Contains a list of previously tried peer candidate IDs
        private List<ulong> PeerBlackList { get; set; }

        // Thread in which new peers are discovered
        private Timer DiscoveryTimer { get; set; }

        // Tells the discovery timer whether or not discovery is already active
        private bool DiscoveryActive { get; set; }

        #endregion

        #endregion

        #region Methods

        // Starts peer discovery
        private void StartDiscovery()
        {
            // Assign variables
            PeerCandidates = new List<PeerCandidate>();
            PeerBlackList = new List<ulong>();

            // Setup discovery timer
            DiscoveryTimer = new Timer(new TimerCallback(AutomatedDiscovery), null, 0, Globals.P2P_DISCOVERY_INTERVAL * 1000);
        }

        // Stops peer discovery
        private void StopDiscovery()
        {
            // Dispose of our timer, clearing resources
            DiscoveryTimer.Dispose();
        }

        // Adds peer list candidates from a local peer list blob
        private void AddPeerCandidates(byte[] Buffer)
        {
            // Do nothing if not running
            if (Stopped) return;

            // Check if discovery is enabled
            if (!Globals.P2P_DISCOVERY_ENABLED) return;

            // Loop through the peer list buffer
            while (Buffer.Length >= 24)
            {
                // Sub the next peer in line
                var PeerBuffer = Buffer.SubBytes(0, 24);
                Buffer = Buffer.SubBytes(24);

                try
                {
                    // Deserialize candidate information
                    uint Ip = ByteArrayToInteger<uint>(PeerBuffer, 0);
                    uint Port = ByteArrayToInteger<uint>(PeerBuffer, 4);
                    ulong Id = ByteArrayToInteger<ulong>(PeerBuffer, 8);
                    ulong LastSeen = ByteArrayToInteger<ulong>(PeerBuffer, 16);

                    // Add to candidate list
                    AddPeerCandidate(new PeerCandidate(Ip, Port, Id, LastSeen));
                }
                catch { }
            }
        }

        // Checks whether or not we should attempt to discover a peer on schedule
        private void AutomatedDiscovery(object _)
        {
            // Do nothing if not running
            if (Stopped) return;

            // Check if we are already at the minimum number of peer connections
            if (GetPeerCount() >= Globals.P2P_MIN_PEER_CONNECTIONS) return;

            // Discover a new peer
            DiscoverNewPeer();
        }

        // Adds a peer to the candidate list or updates an existing candidate
        private void AddPeerCandidate(PeerCandidate Peer)
        {
            // Lock peer candidate list to prevent race conditions
            lock (PeerCandidates)
            {
                // Peer exists in the list already
                if (PeerCandidates.Any(x => x.Id == Peer.Id))
                {
                    // Find peer in the list
                    var LocalPeer = PeerCandidates.First(x => x.Id == Peer.Id);

                    // Update peer's last seen time
                    if (LocalPeer.LastSeen < Peer.LastSeen)
                    {
                        LocalPeer.LastSeen = Peer.LastSeen;
                    }
                }

                // Peer has not yet been added
                else
                {
                    // Add peer to the candidate list
                    PeerCandidates.Add(Peer);
                }
            }
        }

        // Returns a new list of peer candidates (to prevent locking the main list continuously)
        private List<PeerCandidate> GetPeerCandidates()
        {
            // Lock peer candidate list to prevent race conditions
            lock (PeerCandidates)
            {
                // Order candidate list by last seen time, newest first
                return PeerCandidates.OrderByDescending(x => x.LastSeen).ToList();
            }
        }

        // Adds a peer to the list of blacklisted peers
        private void AddBlacklistedPeer(PeerCandidate Peer)
        {
            // Lock blacklist to prevent race conditions
            lock (PeerBlackList)
            {
                // Add the peer id to the list
                PeerBlackList.Add(Peer.Id);
            }
        }

        // Returns whether or not a peer is blacklisted
        private bool IsPeerBlacklisted(PeerCandidate Peer)
        {
            // Lock blacklist to prevent race conditions
            lock (PeerBlackList)
            {
                // Return whether or not the blacklist contains this peer
                return PeerBlackList.Contains(Peer.Id);
            }
        }

        // Returns whether or not a peer is connected already
        private bool IsPeerConnected(PeerCandidate Peer)
        {
            // Check whether the peer candidate is this node
            if (Peer.Id == Id) return true;

            // Lock peer list to prevent race conditions
            lock (PeerList)
            {
                // Check if the connected peer list contains this peer
                if (PeerList.Any(x => x.Id == Peer.Id)) return true;

                // TODO - check if any incoming connections have this address + port
            }

            // Peer is not connected
            return false;
        }

        // Attempts to connect to a new peer on the peer candidate list
        private void DiscoverNewPeer()
        {
            // Do nothing if not running
            if (Stopped) return;

            // Check if discovery is enabled
            if (!Globals.P2P_DISCOVERY_ENABLED) return;

            // Check if discovery is already active
            if (DiscoveryActive) return;
            DiscoveryActive = true;

            // Check if the peer list has space for a new peer
            if (GetPeerCount() >= Globals.P2P_MAX_PEER_CONNECTIONS) return;

            // Get the peer candidate list
            List<PeerCandidate> Candidates = GetPeerCandidates();

            // Check if there are candidates in the list
            if (Candidates.Count == 0) return;

            // Setup variables
            int TryCount = 0;
            int RandomCount = 0;
            int MaxIndex = Math.Min(Candidates.Count - 1, 50);
            List<int> TriedIndexes = new List<int>();

            // Loop to try a selection of candidates
            while (RandomCount < (MaxIndex + 1) * 3 && TryCount < 10 && !Stopped)
            {
                // Increment random count and get net random index
                RandomCount++;
                int RandomIndex = SecureRandom.Integer(0, MaxIndex + 1);
                RandomIndex = (RandomIndex * RandomIndex * RandomIndex) / (MaxIndex * MaxIndex);

                // Check if this index has been tried previously
                if (TriedIndexes.Contains(RandomIndex)) continue;

                // Add this index to the tried indexes list
                TriedIndexes.Add(RandomIndex);

                // Get the peer candidate
                PeerCandidate Peer = Candidates[RandomIndex];

                // Increment try count
                TryCount++;

                // Check if this peer is already connected
                if (IsPeerConnected(Peer)) continue;

                // TODO - check if we are allowing remote connections??

                // Check if this peer has previously been blacklists
                // TODO - have a timeout for blacklisted peers to free them (timeout in config)
                if (IsPeerBlacklisted(Peer)) return;

                // Attempt to add this peer candidate
                Logger.Debug($"[{TryCount}/10] Trying candidate #{RandomIndex} {Peer.Address}:{Peer.Port}, last seen {GetTimeDelta(Peer.LastSeen)} seconds ago");
                if (!AddPeer(Peer.Address, (int)Peer.Port))
                {
                    // Add to failed connection list
                    AddBlacklistedPeer(Peer);
                }
            }

            // Discovery process finished
            DiscoveryActive = false;
        }

        #endregion
    }
}
