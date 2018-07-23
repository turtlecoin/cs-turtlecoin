//
// Copyright (c) 2018 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Canti.Blockchain.RPC
{
    [Serializable]
    public class HttpRequestResult
    {
        public HttpRequestResult() { }
        [OptionalField]
        public MethodError error = null;
        [OptionalField]
        public UInt32 height = 0;
        [OptionalField]
        public UInt32 network_height = 0;
        [OptionalField]
        public string status = null;
    }

    [Serializable]
    public class RequestResult
    {
        public RequestResult() { }
        [OptionalField]
        public string jsonrpc = null;
        [OptionalField]
        public string id = null;
        [OptionalField]
        public MethodResult result = null;
        [OptionalField]
        public MethodError error = null;
    }

    [Serializable]
    public class MethodResult
    {
        public MethodResult() { }
        [OptionalField]
        public string viewSecretKey = null;
        [OptionalField]
        public string spendSecretKey = null;
        [OptionalField]
        public string spendPublicKey = null;
        [OptionalField]
        public UInt32 blockCount = 0;
        [OptionalField]
        public UInt32 knownBlockCount = 0;
        [OptionalField]
        public string lastBlockHash = null;
        [OptionalField]
        public UInt32 peerCount = 0;
        [OptionalField]
        public string[] addresses = null;
        [OptionalField]
        public string address = null;
        [OptionalField]
        public UInt64 availableBalance = 0;
        [OptionalField]
        public UInt64 lockedAmount = 0;
        [OptionalField]
        public string[] blockHashes = null;
        [OptionalField]
        public List<MethodResultItems> items = null;
        [OptionalField]
        public string[] transactionHashes = null;
        [OptionalField]
        public MethodResultTransaction transaction = null;
        [OptionalField]
        public string transactionHash = null;
        [OptionalField]
        public UInt64 totalOutputCount = 0;
        [OptionalField]
        public UInt64 fusionReadyCount = 0;
        [OptionalField]
        public UInt32 count = 0;
        [OptionalField]
        public string status = null;
    }

    [Serializable]
    public class MethodError
    {
        public MethodError() { }
        [OptionalField]
        public string message = null;
        [OptionalField]
        public string code = null;
        [OptionalField]
        public object data = null;
    }

    [Serializable]
    public class MethodResultItems
    {
        public MethodResultItems() { }
        [OptionalField]
        public string[] transactionHashes = null;
        [OptionalField]
        public string blockHash = null;
        [OptionalField]
        public List<MethodResultTransaction> transactions = null;
    }

    [Serializable]
    public class MethodResultTransaction
    {
        public MethodResultTransaction() { }
        [OptionalField]
        public string transactionHash = null;
        [OptionalField]
        public UInt32 blockIndex = 0;
        [OptionalField]
        public UInt64 timestamp = 0;
        [OptionalField]
        public bool isBase = false;
        [OptionalField]
        public UInt64 unlockTime = 0;
        [OptionalField]
        public Int64 amount = 0;
        [OptionalField]
        public UInt64 fee = 0;
        [OptionalField]
        public string extra = null;
        [OptionalField]
        public string paymentId = null;
        [OptionalField]
        public List<MethodResultTransfer> transfers = null;
    }

    [Serializable]
    public class MethodResultTransfer
    {
        public MethodResultTransfer() { }
        [OptionalField]
        public string address = null;
        [OptionalField]
        public Int64 amount = 0;
    }
}
