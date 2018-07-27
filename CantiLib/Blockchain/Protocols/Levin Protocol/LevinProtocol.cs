//
// Copyright (c) 2018 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

using Canti.Blockchain.P2P;
using Canti.Data;
using Canti.Utilities;
using System;
using System.Collections.Generic;

namespace Canti.Blockchain
{
    public partial class LevinProtocol : IProtocol
    {
        // Server connection
        public Server Server;

        // Logger
        public Logger Logger;

        // Peer read status (0 = head, 1 = body)
        private Dictionary<PeerConnection, LevinPeer> Peers = new Dictionary<PeerConnection, LevinPeer>();

        // Entry point
        public LevinProtocol(Server Connection)
        {
            // Set connection
            Server = Connection;

            // Set logger
            Logger = Connection.Logger;

            // Bind event handlers
            Server.OnDataReceived += OnDataReceived;
            Server.OnPeerConnected += OnPeerConnected;
            Server.OnPeerDisconnected += OnPeerDisconnected;
        }

        // Data received
        public void OnDataReceived(object sender, EventArgs e)
        {
            // Get packet data
            Packet Packet = (Packet)sender;
            LevinPeer Peer = Peers[Packet.Peer];

            // Read header
            if (Peer.ReadStatus == PacketReadStatus.Head)
            {
                // Decode header
                try { Peer.Header = BucketHead2.Deserialize(Packet.Data); }
                catch
                {
                    Logger?.Log(Level.DEBUG, "Could not deserialize incoming packet header, incorrect format");
                    return;
                }

                // Set peer data
                Peer.Data = Packet.Data;

                // Debug
                /*Logger?.Log(Level.DEBUG, "Received header:");
                Logger?.Log(Level.DEBUG, " - Signature: {0}", Peers[Packet.Peer].Header.Signature);
                Logger?.Log(Level.DEBUG, " - Payload Size: {0}", Peers[Packet.Peer].Header.PayloadSize);
                Logger?.Log(Level.DEBUG, " - Response Required: {0}", Peers[Packet.Peer].Header.ResponseRequired);
                Logger?.Log(Level.DEBUG, " - Command Code: {0}", Peers[Packet.Peer].Header.CommandCode);
                Logger?.Log(Level.DEBUG, " - Return Code: {0}", Peers[Packet.Peer].Header.ReturnCode);
                Logger?.Log(Level.DEBUG, " - Flags: {0}", Peers[Packet.Peer].Header.Flags);
                Logger?.Log(Level.DEBUG, " - Protocol Version: {0}", Peers[Packet.Peer].Header.ProtocolVersion);*/

                // Check that signature matches
                if (Peer.Header.Signature != GlobalsConfig.LEVIN_SIGNATURE)
                {
                    Logger?.Log(Level.DEBUG, "Incoming packet signature mismatch, expected {0}, received {1}", GlobalsConfig.LEVIN_SIGNATURE, Peers[Packet.Peer].Header.Signature);
                    return;
                }

                // Check packet size
                if (Peer.Header.PayloadSize > GlobalsConfig.LEVIN_MAX_PACKET_SIZE)
                {
                    Logger?.Log(Level.DEBUG, "Incoming packet size is too big, max size is {0}, received {1}", GlobalsConfig.LEVIN_MAX_PACKET_SIZE, Packet.Data.Length);
                    return;
                }

                // Set new read status
                if (Peer.Header.PayloadSize > 0) Peers[Packet.Peer].ReadStatus = PacketReadStatus.Body;
            }

            // Add data to peer buffer if reading message body
            else
            {
                // Add bytes to peer buffer
                byte[] NewData = new byte[Peer.Data.Length + Packet.Data.Length];
                Buffer.BlockCopy(Peer.Data, 0, NewData, 0, Peer.Data.Length);
                Buffer.BlockCopy(Packet.Data, 0, NewData, Peer.Data.Length, Packet.Data.Length);
                Peer.Data = NewData;
            }

            // Check if data size matches payload size and that a header has been decoded
            if (Peer.ReadStatus == PacketReadStatus.Body && (ulong)Peer.Data.Length >= Peer.Header.PayloadSize)
            {
                // Get header
                BucketHead2 Header = Peer.Header;

                // Decode command
                Command Command = new Command
                {
                    CommandCode = Header.CommandCode,
                    IsNotification = !Header.ResponseRequired,
                    IsResponse = (Header.Flags & LEVIN_PACKET_RESPONSE) == LEVIN_PACKET_RESPONSE,
                    Data = Encoding.SplitByteArray(Peers[Packet.Peer].Data, 33, (int)Peers[Packet.Peer].Header.PayloadSize)
                };

                // Check if peer requires a handshake
                if (Command.CommandCode == Commands.Handshake.Id) Commands.Handshake.Invoke(this, Peer, Command); // 1001
                else if (Peer.State == PeerState.Verified)
                {
                    // Invoke other commands
                    if (Command.CommandCode == Commands.Ping.Id) Commands.Ping.Invoke(this, Peer, Command); // 1002
                    else if (Command.CommandCode == Commands.TimedSync.Id) Commands.TimedSync.Invoke(this, Peer, Command); // 1003
                    else if (Command.CommandCode == Commands.NotifyRequestChain.Id) Commands.NotifyRequestChain.Invoke(this, Peer, Command); // 2006
                    else if (Command.CommandCode == Commands.RequestTxPool.Id) Commands.RequestTxPool.Invoke(this, Peer, Command); // 2008
                }

                // Debug
                else if (Peer.State == PeerState.Unverified)
                {
                    Logger?.Log(Level.DEBUG, "[IN] Received command:");
                    Logger?.Log(Level.DEBUG, " - Command Code: {0}", Command.CommandCode);
                    Logger?.Log(Level.DEBUG, " - Is Notification: {0}", Command.IsNotification);
                    Logger?.Log(Level.DEBUG, " - Is Response: {0}", Command.IsResponse);
                    Logger?.Log(Level.DEBUG, " - Data: {0} Bytes", Command.Data.Length);
                    Logger?.Log(Level.DEBUG, Encoding.ByteArrayToHexString(Command.Data));
                }

                // Set new read status and clear previous request
                Peer.ReadStatus = PacketReadStatus.Head;
                Peer.Header = default(BucketHead2);
                Peer.Data = new byte[0];
            }
        }

        // Peer connected
        public void OnPeerConnected(object sender, EventArgs e)
        {
            // Get peer connection
            PeerConnection Peer = (PeerConnection)sender;

            // Add peer to peer list
            Peers.Add(Peer, new LevinPeer(Peer));
        }

        // Peer disconnected
        public void OnPeerDisconnected(object sender, EventArgs e)
        {
            // Get peer connections
            PeerConnection Peer = (PeerConnection)sender;

            // Remove peer from peer list
            Peers.Remove(Peer);
        }

        // Notifies a peer with a command, no response expected
        public void Notify(PeerConnection Peer, int CommandCode, byte[] Data)
        {
            // Form message header
            BucketHead2 Header = new BucketHead2
            {
                Signature =         GlobalsConfig.LEVIN_SIGNATURE,
                ResponseRequired =  false,
                PayloadSize =       (ulong)Data.Length,
                CommandCode =       (uint)CommandCode,
                ProtocolVersion =   LEVIN_PROTOCOL_VER_1,
                Flags =             LEVIN_PACKET_REQUEST
            };

            // Send header packet
            if (Server.SendMessage(Peer, Header.Serialize()))
            {
                // Send body packet
                Server.SendMessage(Peer, Data);
            }
        }

        // Notifies all peers with a command, no response expected
        public void NotifyAll(int CommandCode, byte[] Data)
        {
            // Form message header
            BucketHead2 Header = new BucketHead2
            {
                Signature =         GlobalsConfig.LEVIN_SIGNATURE,
                ResponseRequired =  false,
                PayloadSize =       (ulong)Data.Length,
                CommandCode =       (uint)CommandCode,
                ProtocolVersion =   GlobalsConfig.LEVIN_VERSION,
                Flags =             LEVIN_PACKET_REQUEST
            };

            // Send header packet
            Server.Broadcast(Header.Serialize());
            
            // Send body packet
            Server.Broadcast(Data);
        }

        // Notifies a peer with a command, no response expected
        public void Reply(LevinPeer Peer, int CommandCode, byte[] Data, bool RequestSuccessul = false, bool ResponseRequired = false)
        {
            // Form message header
            BucketHead2 Header = new BucketHead2
            {
                Signature         = GlobalsConfig.LEVIN_SIGNATURE,
                ResponseRequired  = ResponseRequired,
                PayloadSize       = (ulong)Data.Length,
                CommandCode       = (uint)CommandCode,
                ProtocolVersion   = GlobalsConfig.LEVIN_VERSION,
                Flags             = LEVIN_PACKET_RESPONSE,
                ReturnCode        = RequestSuccessul? LEVIN_RETCODE_SUCCESS : LEVIN_RETCODE_FAILURE
            };

            // Debug
            Logger?.Log(Level.DEBUG, "Sending header:");
            Logger?.Log(Level.DEBUG, " - Signature: {0}", Header.Signature);
            Logger?.Log(Level.DEBUG, " - Payload Size: {0}", Header.PayloadSize);
            Logger?.Log(Level.DEBUG, " - Response Required: {0}", Header.ResponseRequired);
            Logger?.Log(Level.DEBUG, " - Command Code: {0}", Header.CommandCode);
            Logger?.Log(Level.DEBUG, " - Return Code: {0}", Header.ReturnCode);
            Logger?.Log(Level.DEBUG, " - Flags: {0}", Header.Flags);
            Logger?.Log(Level.DEBUG, " - Protocol Version: {0}", Header.ProtocolVersion);

            // Send header packet
            if (Server.SendMessage(Peer.Connection, Header.Serialize()))
            {
                // Send body packet
                Server.SendMessage(Peer.Connection, Data);
            }
        }

        // Encodes a command and returns the raw bytes
        public byte[] Encode(ICommandRequestBase Data)
        {
            // Return the serialized byte array
            return Data.Serialize();
        }
    }
}
