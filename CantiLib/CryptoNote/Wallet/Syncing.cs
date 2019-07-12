//
// Copyright (c) 2019 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

namespace Canti.CryptoNote
{
    public partial class WalletBackend
    {
        #region Properties and Fields

        // TODO - Daemon connection info for syncing
        private int SyncHeight { get; set; }

        #endregion

        #region Methods

        // Starts wallet syncing
        public void Start()
        {
            // TODO - Wallet syncing
        }

        // Stops wallet syncing
        public void Stop()
        {

        }

        #endregion
    }
}
