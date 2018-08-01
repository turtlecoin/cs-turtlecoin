//
// Copyright (c) 2018 The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Canti.Data;
using Canti.Blockchain.Crypto;
using Canti.Blockchain.Crypto.Mnemonics;

namespace Tests
{
    [TestClass]
    public class MnemonicTests
    {
        [TestMethod]
        public void TestMnemonicToPrivateKey()
        {
            string m1 = "optical rumble bamboo worry auctions width essential request oxygen acoustic wounded gawk ginger ornament sixteen pawnshop pairing soapy rift alley enraged orbit axle binocular bamboo";

            Mnemonics.MnemonicToPrivateKey(m1).Do(
                err => Assert.Fail($"Failed to parse mnemonic seed: {err}"),
                /* The corresponding private key to this mnemonic */
                key => Assert.AreEqual<PrivateKey>(key, new PrivateKey("f41d515ac6e84de566f3a9eb559f3e58db6d8b28906857288f9f9f3809846006"))
            );

            string m2 = "psychic frown wetsuit orders rover rays ruffled initiate adhesive acumen among lectures drunk itches tanks highway evolved amended asked thorn nanny juggled vaults velvet adhesive";

            Mnemonics.MnemonicToPrivateKey(m2).Do(
                err => Assert.Fail($"Failed to parse mnemonic seed: {err}"),
                /* A completely unrelated private key */
                key => Assert.AreNotEqual<PrivateKey>(key, new PrivateKey("17a27c50a43b99505d7934c1f05e748deee52bb813d7b1d0654ac980d1a93304"))
            );

            /* Mnemonic with invalid checksum */
            string m3 = "double hatchet solved bifocals dozen ulcers sickness sneeze unrest deftly molten oven deity spud upgrade shipped vogue razor gopher sailor drowning epoxy nephew oust spud";

            Mnemonics.MnemonicToPrivateKey(m3).Do(
                err => {/* Expected */},
                key => Assert.Fail($"Mnemonic has invalid checksum but was still successfully parsed!")
            );

            /* Not 25 words */
            string m4 = "lol, i'm not valid";

            Mnemonics.MnemonicToPrivateKey(m4).Do(
                err => {/* Expected */},
                key => Assert.Fail($"Mnemonic is invalid length but was still successfully parsed!")
            );

            /* Final word is not in mnemonic dictionary */
            string m5 = "daily utopia pistons null giddy pirate return espionage fossil rustled biweekly fictional sedan jubilee asked ugly mystery paper awning titans point luxury eccentric ecstatic word_not_in_dictionary";

            Mnemonics.MnemonicToPrivateKey(m5).Do(
                err => {/* Expected */},
                key => Assert.Fail($"Mnemonic has invalid word but was still successfully parsed!")
            );
        }

        [TestMethod]
        public void TestPrivateKeyToMnemonic()
        {
            PrivateKey p1 = new PrivateKey("4318e9ea979c6a478b224b233fa1c19fe19508a93caf95899ba90c1e32db1e02");

            string m1 = Mnemonics.PrivateKeyToMnemonic(p1);

            /* The corresponding mnemonic to this private key */
            Assert.AreEqual<string>(m1, "dexterity tolerant sixteen jubilee pamphlet useful looking king odometer rounded listen border inundate aphid reruns lumber noodles bogeys bawled situated union tequila flippant foxes border");

            PrivateKey p2 = new PrivateKey("7abd03833990b87b7787ee1eac16b6b1823cc85690ae686d5a794b2171107e05");

            string m2 = Mnemonics.PrivateKeyToMnemonic(p2);

            /* A completely unrelated mnemonic */
            Assert.AreNotEqual<string>(m2, "point wolf alchemy laboratory duplex tsunami rarest lids rugged factual pepper stunning folding fibula daily textbook timber jailed intended five etched reinvest aloof beer duplex");
        }

        public void TestMnemonicBothWays()
        {
            for (int i = 0; i < 1000; i++)
            {
                /* Generate a random private key */
                PrivateKey p = KeyOps.GenerateKeys().privateKey;

                /* Convert to mnemonic */
                string m = Mnemonics.PrivateKeyToMnemonic(p);

                /* Convert back to a private key */
                Mnemonics.MnemonicToPrivateKey(m).Do(
                    err => Assert.Fail($"Failed to parse mnemonic seed: {err}"),
                    /* Should be the same as the original private key */
                    key => Assert.AreEqual<PrivateKey>(p, key)
                );
            }
        }
    }
}
