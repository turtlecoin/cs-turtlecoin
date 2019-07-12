//
// Copyright (c) 2018-2019 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

using Canti.CryptoNote.Blockchain;
using Newtonsoft.Json.Linq;

namespace Canti.CryptoNote
{
    // API method context for a node's API server
    internal sealed class BlockchainMethods : IMethodContext
    {
        #region Properties and Fields

        #region Private

        // A reference to the associated node
        private Node Node { get; set; }

        #endregion

        #endregion

        #region Methods

        // Verifies a request's version
        public bool CheckVersion(int Version)
        {
            // Returns true if version is at least the minimum version, but not higher than the current version
            return Version >= Node.Globals.API_MINIMUM_VERSION && Version <= Node.Globals.API_CURRENT_VERSION;
        }

        #endregion

        #region API Methods

        [ApiMethod("Height")]
        public string GetHeight()
        {
            // Form response
            var Response = new JObject
            {
                ["current_height"] = Node.Blockchain.Height,
                ["known_height"] = Node.Blockchain.KnownHeight,
                ["synced"] = Node.Blockchain.Height >= Node.Blockchain.KnownHeight
            };
            return Response.ToString();
        }

        [ApiMethod("block")]
        public string GetBlockByHash(string Hash)
        {
            // Query block
            if (!Node.Blockchain.TryGetBlock(Hash, out Block Block))
            {
                return new JObject
                {
                    ["error"] = "Could not find block"
                }.ToString();
            }

            // Form response
            var Response = new JObject
            {
                ["height"] = Block.Height,
                ["hash"] = Block.Hash,
                ["timestamp"] = Block.Timestamp,
                ["nonce"] = Block.Nonce,
                ["major_version"] = Block.MajorVersion,
                ["minor_version"] = Block.MinorVersion,
                ["base_reward"] = Block.BaseReward,
                ["total_fees"] = Block.TotalFees,
                ["base_transaction"] = Block.BaseTransaction.Hash
            };
            return Response.ToString();
        }

        [ApiMethod("block")]
        public string GetBlockByHeight(uint Height)
        {
            // Query block
            if (!Node.Blockchain.TryGetBlock(Height, out Block Block))
            {
                return new JObject
                {
                    ["error"] = "Could not find block"
                }.ToString();
            }

            // Form response
            var Response = new JObject
            {
                ["height"] = Block.Height,
                ["hash"] = Block.Hash,
                ["timestamp"] = Block.Timestamp,
                ["nonce"] = Block.Nonce,
                ["major_version"] = Block.MajorVersion,
                ["minor_version"] = Block.MinorVersion,
                ["base_reward"] = Block.BaseReward,
                ["total_fees"] = Block.TotalFees,
                ["base_transaction"] = Block.BaseTransaction.Hash
            };
            return Response.ToString();
        }

        #endregion

        #region Constructors

        // Initializes this API method context
        internal BlockchainMethods(Node Node)
        {
            this.Node = Node;
        }

        #endregion
    }
}
