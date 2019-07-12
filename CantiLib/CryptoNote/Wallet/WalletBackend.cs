//
// Copyright (c) 2019 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

using Canti.Cryptography;
using Canti.CryptoNote.Wallet;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;

namespace Canti.CryptoNote
{
    public partial class WalletBackend
    {
        #region Properties and Fields

        #region Public

        // A value that is used to prefix addresses with a certain set of characters
        public ulong Prefix { get; set; }

        // Contains wallet and subwallet information
        [JsonProperty(PropertyName = "subWallets")]
        public WalletContainer WalletData { get; set; }

        // The format of the wallet file, may change in the future
        [JsonProperty(PropertyName = "walletFileFormatVersion")]
        public int WalletVersion { get; set; }

        // Data used to help store the wallet synchronization state
        [JsonProperty(PropertyName = "walletSynchronizer")]
        public WalletSyncState WalletSyncState { get; set; }

        #endregion

        #region Private

        // Wallet file encrypter
        private FileEncrypter Encrypter { get; set; }

        // Wallet file path
        private string FileName { get; set; }

        // Wallet file password
        private string Password { get; set; }

        // Whether or not a wallet container is loaded
        private bool IsLoaded { get; set; }

        #endregion

        #endregion

        #region Methods

        // Creates a new wallet container
        public bool New(string FileName, string Password)
        {
            // Save, in case a wallet is already loaded
            Save();

            // Check if file already exists
            if (File.Exists(FileName)) return false;

            // Re-define cached wallet information
            WalletData = new WalletContainer();
            WalletSyncState = new WalletSyncState();
            WalletVersion = Constants.WALLET_FILE_VERSION;

            // Set wallet as loaded
            IsLoaded = true;

            // Store wallet information
            this.FileName = FileName;
            this.Password = Password;

            // Save new wallet file
            if (Save()) return true;

            // Failed to save wallet container
            else return false;
        }

        // Creates a new subwallet
        public SubWallet CreateSubWallet()
        {
            // Check if a wallet container is loaded
            if (!IsLoaded) return null;

            // Create a subwallet object
            SubWallet SubWallet = new SubWallet();

            // Generate a pair of spend keys
            KeyPair SpendKeys = Crypto.GenerateKeys();
            SubWallet.PrivateSpendKey = SpendKeys.PrivateKey;
            SubWallet.PublicSpendKey = SpendKeys.PublicKey;

            // Derive a pair of view keys from the private spend key
            KeyPair ViewKeys = new NativeCrypto().GenerateViewKeysFromPrivateSpendKey(SpendKeys.PrivateKey);
            SubWallet.PrivateViewKey = ViewKeys.PrivateKey;
            SubWallet.PublicViewKey = ViewKeys.PublicKey;

            // Get wallet's public address
            SubWallet.Address = Addresses.AddressFromKeys(SpendKeys.PublicKey, ViewKeys.PublicKey, Prefix);

            // If there are no other subwallets, set this one to primary
            if (WalletData.SubWallets.Count == 0) SubWallet.IsPrimaryAddress = true;

            // Set wallet's sync timestamp to the current time
            SubWallet.SyncTimestamp = Utils.GetTimestamp();

            // Add subwallet info to wallet container
            WalletData.PublicSpendKeys.Add(SpendKeys.PublicKey);
            WalletData.SubWallets.Add(SubWallet);

            // Save wallet file
            Save();

            // Return created subwallet
            return SubWallet;
        }

        // Imports a wallet from a set of keys or mnemonics
        public void Import(string Key1, string Key2)
        {
            // TODO - Import from keys
        }
        public void Import(string Mnemonics)
        {
            // TODO - Import from mnemonics
        }

        // Loads wallet data from a file
        public bool Load(string FileName, string Password)
        {
            // Attempt to load wallet data from the given file path
            if (!Encrypter.Load(FileName, Password, out byte[] Data))
            {
                return false;
            }

            // Deserialize and cache loaded wallet information
            // TODO - Test this
            System.Console.WriteLine(JsonPrettify(Data.ToString(Constants.WALLET_ENCODING_METHOD)));
            var Wallet = JObject.Parse(Data.ToString(Constants.WALLET_ENCODING_METHOD));
            WalletData = Wallet["subWallets"].ToObject<WalletContainer>();
            WalletVersion = Wallet["walletFileFormatVersion"].ToObject<int>();
            WalletSyncState = Wallet["walletSynchronizer"].ToObject<WalletSyncState>();

            // Load successful
            return true;
        }

        // TODO - delete this
        public static string JsonPrettify(string json)
        {
            using (var stringReader = new StringReader(json))
            using (var stringWriter = new StringWriter())
            {
                var jsonReader = new JsonTextReader(stringReader);
                var jsonWriter = new JsonTextWriter(stringWriter) { Formatting = Formatting.Indented };
                jsonWriter.WriteToken(jsonReader);
                return stringWriter.ToString();
            }
        }

        // Saves wallet data to a file
        public bool Save()
        {
            // Save data to file
            if (IsLoaded)
            {
                try
                {
                    byte[] Data = JsonConvert.SerializeObject(this).GetBytes(Constants.WALLET_ENCODING_METHOD);
                    Encrypter.Save(FileName, Password, Data);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            else return false;
        }

        #endregion

        #region Constructor

        // Initializes a new empty wallet backend instance
        public WalletBackend(ulong AddressPrefix)
        {
            // Set address prefix
            Prefix = AddressPrefix;

            // Initialize encrypter
            Encrypter = new FileEncrypter(Constants.WALLET_FILE_IDENTIFIER, Constants.WALLET_PASSWORD_IDENTIFIER)
            {
                EncodeMethod = Constants.WALLET_ENCODING_METHOD
            };
        }

        #endregion
    }
}
