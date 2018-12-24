using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Canti.Data;
using Canti.Blockchain.Crypto;
using Canti.Utilities;

namespace Tests
{
    [TestClass]
    public class AddressTests
    {
        [TestMethod]
        public void TestAddressFromKeys()
        {
            /* Test from public keys first */
            PublicKey s1 = new PublicKey("452fc1d7637380b6f2615583173779a7c1a183f5454a2b1b7dd94d6762023714");
            PublicKey v1 = new PublicKey("b25532b855490516a1cb300cde3f0936cd84d01495e09adcb5a23cae6e6d302a");

            /* TRTL address */
            string derivedAddress1 = Addresses.AddressFromKeys(s1, v1, 0x3bbb1d);

            Assert.AreEqual<string>("TRTLuyFCzMtHdoTjacqK6A4tEQL7yzPGGCbCRz9NRmyxHPonkiCGVNXFGNfo7bBZWBeB4sxABe3goS4ztq1UhVZFKUGytkv3ubJ",
                                    derivedAddress1);

            /* Monero address */
            string derivedAddress2 = Addresses.AddressFromKeys(s1, v1, 0x12);

            Assert.AreEqual<string>("44FBBvSSEohXbpAHmYjtNCV4SvVioYata5bhmenHbGLW4TnREnSMy1N4nZQrQxB7h2AAf3Px7JNk1dvACpuD652X5kea7nw",
                                    derivedAddress2);

            /* Now test from private keys */
            PrivateKey s2 = new PrivateKey("51ae544866f9a9187c6a81d2d6043848100d909ad9626effbdae336443438701");
            PrivateKey v2 = new PrivateKey("e4fc49878954ff6bfc0e2330ff562dd5aceca69e93d519f6d77153ccef09de0e");

            string derivedAddress3 = Addresses.AddressFromKeys(s2, v2, 0x3bbb1d);

            /* TRTL address */
            Assert.AreEqual<string>("TRTLux5AuhnYj3xRZE6frnL4j5MLeLFgLcq9SAfJALtFN5iWbjBTPzRQBx6y1nVGXUdnJLZks5zJP1Ucfcaxbn3cDLKnx3Y7cTf",
                                    derivedAddress3);

            string derivedAddress4 = Addresses.AddressFromKeys(s2, v2, 0x12);

            /* Monero address */
            Assert.AreEqual<string>("42WJwsziXeM8kty8HnvgNnRMAykDcuzFJF78YG2UPfAGNURDzKsATFeL5asUs1jQidApTyLuFkGyKEw6HbraZKVgCxy89ge",
                                    derivedAddress4);
        }

        [TestMethod]
        public void TestKeysFromAddress()
        {
            /* TRTL address */
            string address1 = "TRTLuzcJgwAet7R1JZqbBkM9yGAw59VJKJKf2j4nuw3BFej9iZ4YiU1b6RwbL8mBZA2oKQxPfSJQN3mcSRKWLMTX6SVjNvwPptH";

            Addresses.KeysFromAddress(address1, 0x3bbb1d).Do(
                err => Assert.Fail($"Failed to get keys from address: {err}"),
                keys => {
                    Assert.AreEqual<PublicKey>(new PublicKey("7a8a21b9e27a1c968b679e57787f2f24059276dc678f335cc256f1e65796e1c5"), keys.spendKey);
                    Assert.AreEqual<PublicKey>(new PublicKey("41239f2acbd51af091bf8d610ac0c3e7e6ae09f7108e42317c177d5a20856676"), keys.viewKey);
                }
            );

            /* Monero address */
            string address2 = "49Mimt7fmK1LJMbXfeByR37nKjGb4vijtRStXgX7wjt3ZQ9prGCH2mVG7JSn9aSQ7KCqHQGkV8UJgF6fpGuiEGiXD9nT7oS";

            Addresses.KeysFromAddress(address2, 0x12).Do(
                err => Assert.Fail($"Failed to get keys from address: {err}"),
                keys => {
                    Assert.AreEqual<PublicKey>(new PublicKey("cc1b1a68c856f47361ed9964f09e72288b23fc1561e7f39227ba933f847ac8c1"), keys.spendKey);
                    Assert.AreEqual<PublicKey>(new PublicKey("b3b70ac5706b885a5420749acf226246bddddb76abb8c55448effd337ae9dc6b"), keys.viewKey);
                }
            );

            /* Invalid length */
            TestCantGetKeys("TRTLuySR8AL5Lg7cz558ch38P1hjJexRnKP6EWzbWvPb8yobvyeRoV5gUurZsXhh8ab1LNzbYEnaJj7j8VeffbahXATmYYdNdD",
                            "Address is incorrect length but was still successfully parsed!");

            /* Invalid checksum */
            TestCantGetKeys("TRTLuxKLb7z1Q474yKPb5ASixcSZNP96V1M2gPTBeDQ6Ud7bHYUdP9W9xSaJkdGrhe2bekCbgWAmeDzov34ttLXrd7Bfu1peU5f",
                            "Address has invalid checksum but was still successfully parsed!");

            /* Invalid prefix */
            TestCantGetKeys("dicKuxQN6k9PRVFmtfo6fyWhRRwbAVirAE1Coyv6cyqd5tUkUa4bHHpFAuzrZuF5U3MjjWPfTu3Cq5TiSmiSU5GhYhSqWVsmuNC",
                            "Address has incorrect prefix but was still successfully parsed!");

            /* Non base58 character (zero) */
            TestCantGetKeys("TRTLuw0Hwu7VE5BUeqpd9sbPY1G3uTGTqS5PmYGRRShJhgnXNtGMeY791Yk6LjZmRMEiR5Ff5WdwNPHgHH7WU9zp2Ee2iAQqhC3",
                            "Address has invalid base58 character but was still successfully parsed!");
        }

        private void TestCantGetKeys(string address, string msg)
        {
            Addresses.KeysFromAddress(address, 0x3bbb1d).Do(
                err => {/* Expected */},
                keys => Assert.Fail(msg)
            );
        }

        [TestMethod]
        public void TestAddressesBothWays()
        {
            for (int i = 0; i < 10; i++)
            {
                WalletKeys w = KeyOps.GenerateWalletKeys();

                /* TRTL, Monero, random different lengths */
                ulong[] prefixes = new ulong[] { 0x3bbb1d, 0x12, 0x0, 0x11, 0x222, 0x3333, 0x44444, 0x555555, 0x6666666 };

                foreach (ulong prefix in prefixes)
                {
                    string derivedAddress1 = Addresses.AddressFromKeys(w.privateSpendKey, w.privateViewKey, prefix);

                    Addresses.KeysFromAddress(derivedAddress1, prefix).Do(
                        error => Assert.Fail($"Failed to parse keys from address: {error}"),
                        keys => {
                            Assert.AreEqual<PublicKey>(w.publicSpendKey, keys.spendKey);
                            Assert.AreEqual<PublicKey>(w.publicViewKey, keys.viewKey);
                        }
                    );

                    string derivedAddress2 = Addresses.AddressFromKeys(w.publicSpendKey, w.publicViewKey, prefix);

                    Addresses.KeysFromAddress(derivedAddress2, prefix).Do(
                        error => Assert.Fail($"Failed to parse keys from address: {error}"),
                        keys => {
                            Assert.AreEqual<PublicKey>(w.publicSpendKey, keys.spendKey);
                            Assert.AreEqual<PublicKey>(w.publicViewKey, keys.viewKey);
                        }
                    );
                }
            }
        }
    }
}
