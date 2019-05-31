//
// Copyright (c) 2018-2019 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

using static Canti.Utils;

namespace Canti.CryptoNote
{
    /// <summary>
    /// Contains connection information for a peer candidate
    /// </summary>
    public class PeerCandidate
    {
        /// <summary>
        /// This peer's host address
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// This peer's listening port
        /// </summary>
        public uint Port { get; set; }

        // A remote peer's ID
        internal ulong Id { get; set; }

        // The timestamp a peer was last seen by a node
        internal ulong LastSeen { get; set; }

        /// <summary>
        /// Initializes a new peer candidate
        /// </summary>
        /// <param name="Address">This peer's host address</param>
        /// <param name="Port">This peer's listening port</param>
        public PeerCandidate(string Address, uint Port)
        {
            // Assign variables
            this.Address = Address;
            this.Port = Port;
            Id = 0;
            LastSeen = 0;
        }

        // This constructor is used when adding new peers via discovery
        internal PeerCandidate(uint Ip, uint Port, ulong Id, ulong LastSeen)
        {
            // Assign variables
            Address = IpAddressFromUint(Ip);
            this.Port = Port;
            this.Id = Id;
            this.LastSeen = LastSeen;
        }
    }
}
