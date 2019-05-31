//
// Copyright (c) 2018-2019 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

using System.Linq;
using System.Threading;

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

        #endregion
    }
}
