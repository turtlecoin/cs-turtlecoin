//
// Copyright (c) 2018 The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Canti.Data;
using Canti.Blockchain.Crypto;

namespace Tests
{
    [TestClass]
    public class KeyTests
    {
        [TestMethod]
        public void TestGenerateWalletKeys()
        {
            WalletKeys keys = KeyOps.GenerateWalletKeys();

            var privateSpendKey = keys.spendKeys.privateKey;
            var privateViewKey = keys.viewKeys.privateKey;

            /* All newly generated keys should be a deterministic pair */
            Assert.IsTrue(KeyOps.AreKeysDeterministic(privateSpendKey, privateViewKey));

            var publicSpendKey = KeyOps.PrivateKeyToPublicKey(privateSpendKey);
            var publicViewKey = KeyOps.PrivateKeyToPublicKey(privateViewKey);

            /* We should derive the same public keys from the private keys
               as the ones returned from GenerateWalletKeys() */
            Assert.AreEqual<PublicKey>(publicSpendKey, keys.spendKeys.publicKey);
            Assert.AreEqual<PublicKey>(publicViewKey, keys.viewKeys.publicKey);
        }

        [TestMethod]
        public void TestGenerateKeys()
        {
            KeyPair pair = KeyOps.GenerateKeys();

            PublicKey pub = KeyOps.PrivateKeyToPublicKey(pair.privateKey);

            /* The public key should be the same one as we get get from
               manual derivation of the private key */
            Assert.AreEqual<PublicKey>(pair.publicKey, pub);
        }

        [TestMethod]
        public void TestGenerateDeterministicKeys()
        {
            PrivateKey p = new PrivateKey("677e019be0351443ea8904a6d6786056f7390eb74ba05ec327bccf2d318ca809");

            PublicKey pubViewKey = new PublicKey("09e9c339bd1d751bf88d4d2301daa75576297e2e6b1a133cc0b9f03355421f69");

            PrivateKey privViewKey = new PrivateKey("508ba238625600b2ddec49eef1f114002c76c1436d4a144c3588069c3f72ae02");

            /* Generate view keys from a private spend key seed */
            KeyPair pair = KeyOps.GenerateDeterministicKeys(p);

            Assert.AreEqual<PublicKey>(pubViewKey, pair.publicKey);
            Assert.AreEqual<PrivateKey>(privViewKey, pair.privateKey);
        }

        [TestMethod]
        public void TestAreKeysDeterministic()
        {
            /* A private spend key and private view key derived pair */
            PrivateKey a1 = new PrivateKey("a235851985a974aeafa6db9ed4bd8cd523d3f43048bb69df5dff4dfcc445b107");

            PrivateKey a2 = new PrivateKey("b7636ec1e84039dbb6d93ddbcf0f8e39e418ce13516fa1d8c40717f81e8d370e");

            /* a2 is derived from a1 */
            Assert.IsTrue(KeyOps.AreKeysDeterministic(a1, a2));

            /* Derivation is only one way */
            Assert.IsFalse(KeyOps.AreKeysDeterministic(a2, a1));

            /* Two completely unrelated keys */
            PrivateKey b1 = new PrivateKey("ee87e28220156cde3b901beb74bd089bdb512a3f6cef39fca66f900d4009fb08");

            PrivateKey b2 = new PrivateKey("677e019be0351443ea8904a6d6786056f7390eb74ba05ec327bccf2d318ca809");

            Assert.IsFalse(KeyOps.AreKeysDeterministic(b1, b2));
            Assert.IsFalse(KeyOps.AreKeysDeterministic(b2, b1));
        }

        [TestMethod]
        public void TestPrivateKeyToPublicKey()
        {
            /* A private key and public key pair */
            PrivateKey a1 = new PrivateKey("3a8441fa686f3d0c01b60264a79153c41fdd925ae4a31ffab215648f00758101");

            PublicKey a2 = new PublicKey("8464c6ff818fb7f502e4533c23f6ddb615b3568dbbfe954d07f8dd8fd23aefb3");

            Assert.AreEqual<PublicKey>(a2, KeyOps.PrivateKeyToPublicKey(a1));

            /* Two completely unrelated keys */
            PrivateKey b1 = new PrivateKey("a8219da64ddb1974292a19b6f2a3d3ef411a592191590d21ac59961c8af0c50e");

            PublicKey b2 = new PublicKey("45266e9d6259817e6c38cdd7f774fc38c9596d7400b1728546cf111e342941a5");
    
            Assert.AreNotEqual<PublicKey>(b2, KeyOps.PrivateKeyToPublicKey(b1));
        }
    }
}
