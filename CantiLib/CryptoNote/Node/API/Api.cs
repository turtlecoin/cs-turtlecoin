//
// Copyright (c) 2018-2019 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

using System;

namespace Canti.CryptoNote
{
    public sealed partial class Node
    {
        #region Properties and Fields

        #region Public

        /// <summary>
        /// The port this node's API server will bind to
        /// </summary>
        public int ApiPort { get; set; }

        #endregion

        #region Private

        // This node's API server
        private ApiServer ApiServer { get; set; }

        #endregion

        #endregion

        #region Methods

        private bool StartApi()
        {
            // Assign API method context
            ApiServer.MethodContexts += new UtilityMethods(this);
            ApiServer.MethodContexts += new BlockchainMethods(this);
            Logger.Debug("API method contexts added");

            // Set to default port if not specified
            if (ApiPort == 0) ApiPort = Globals.API_DEFAULT_PORT;

            // Start the API server
            try
            {
                ApiServer.Start(ApiPort);
            }
            catch (Exception e)
            {
                Logger.Error($"Could not start API server: {e.Message}");
                return false;
            }
            Logger.Debug("API server started");

            // API started
            return true;
        }

        // Stops all API operations
        private void StopApi()
        {
            // Stops the API server
            ApiServer.Stop();
            Logger.Debug("Api listener stopped");
        }

        #endregion
    }
}
