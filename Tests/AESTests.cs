using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Canti.Data;
using Canti.Blockchain.Crypto.AES;

namespace Tests
{
    [TestClass]
    public class AESTests
    {
        [TestMethod]
        public void TestAESBSingleRound()
        {
        }

        [TestMethod]
        public void TestAESBPseudoRound()
        {
        }

        [TestMethod]
        public void TestExpandKey()
        {
            byte[] key1 = Encoding.HexStringToByteArray(
                "f42b7bbf670bbdc8872b2a74a81de5bbaa754e5dd69ae6176263598220bd7a02"
            );

            byte[] expandedKeys1 = AES.ExpandKey(key1);

            Assert.AreEqual<string>("f42b7bbf670bbdc8872b2a74a81de5bbaa754e5dd69ae6176263598220bd7a028ff10c08e8fab1c06fd19bb4c7cc7e0f6c3ebd2bbaa45b3cd8c702bef87a78bc574d6949bfb7d889d066433d17aa3d329c929a082636c134fef1c38a068bbb366ea76c26d110b4af0176f79216dccaa0db14eee8fd222fdc03d3ec56055857600cfcbc4dddec08e2dc9aff70ca4635d0af4e7898526c574451bfbb1254e7ec728832fc6d55def48f89440bff43023e2fb539ca8de7559dc9b6ea26dbe20dcaa97f462ff52a98db7aa3dcd085e0deeeaa5424e221b3717fe8059b5933e796939aaf9a976185024c1b26de9c9ec6007234", Encoding.ByteArrayToHexString(expandedKeys1));


            byte[] key2 = Encoding.HexStringToByteArray(
                "8220b9a023265e82716dc1a0bd55efb4ca450d6a1a874d2f175bb79bb18b280a"
            );

            byte[] expandedKeys2 = AES.ExpandKey(key2);

            Assert.AreEqual<string>("8220b9a023265e82716dc1a0bd55efb4ca450d6a1a874d2f175bb79bb18b280abe14de689d3280eaec5f414a510aaefe1b22e9d101a5a4fe16fe1365a7753b6f21f67634bcc4f6de509bb7940191196a67a33dd36606992d70f88a48d78db127783eba3ac4fa4ce49461fb7095f0e21a4d2fa5712b293c5c5bd1b6148c5c07333afb795efe0135ba6a60cecaff902cd05b4fd4017066e85d2bb75e49a7eb597ac330a3023d3196b857515872a8c174a29937463be951ae66c2e6f02f650da95534e35f4f09d2c9f75e839185f642e527db1b9ff7324a3191f0acc1be95a168eb46a6b6654f747f9211f7ee17e7b50b30", Encoding.ByteArrayToHexString(expandedKeys2));


            byte[] key3 = Encoding.HexStringToByteArray(
                "70dad95358fe1be0f79491573c4acc5f84bd6b19a5c5981e14541e9a5cfb780c"
            );

            byte[] expandedKeys3 = AES.ExpandKey(key3);

            Assert.AreEqual<string>("70dad95358fe1be0f79491573c4acc5f84bd6b19a5c5981e14541e9a5cfb780c7e66271926983cf9d10cadaeed4661f1d1e784b874221ca66076023c3c8d7a3021bc23f207241f0bd628b2a53b6ed3543378e298475afe3e272cfc021ba1863217f8005d10dc1f56c6f4adf3fd9a7ea767c011c4209aeffa07b613f81c1795caefd274c1ff0e6b9739fac664c460b8c37b107dea5b8a92105c3c81e8402b14220e28e7c8f1268c5fc8dc4a3b0cbcf2f88575f4abdeff66bb82c3e753c2e8f371b52544ed4403c8b28cdf828980637071488ea5089671c3b314b224e0d65ad7914b2bc51b0f280da983f78f200394ff51", Encoding.ByteArrayToHexString(expandedKeys3));
        }
    }
}
