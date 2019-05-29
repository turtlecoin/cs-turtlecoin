using System;
using System.Runtime.Intrinsics.X86;
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
            byte[] expandedKeys1 = Encoding.HexStringToByteArray(
                "4d54c79ab1e27ae04bfe734df26877d7bab89ecbffe598f87d23bbf5ceddc04d97e893355e4d537c4066085159a5515fcfdc9a3a3fe840b61175a446ea75dfb5d7bc2a7a66a268e5e96607e813b8666b443acaff24742a3bed7feaafd78854e170c10468125e99855850588dada7a7f7baf193b754136d748fca760b20628180ece674cfbd44d22168f105d320909d933336f4ffd5afc27eaf8f02b3b289b4e5c8584b928e0ce9b62871f9feae271d4183237e03b88a53c92c549c62366c141599a9995681d612d1206a26eb1559d61f29998383cffce1178c9d1a2c1ba4e8be22b081a7802960dfa6811fe45cf77741"
            );

            byte[] input1 = Encoding.HexStringToByteArray(
                "f77f10a5fb1a52a2b466751c3a858dc3"
            );

            AES.AESBSingleRound(expandedKeys1, input1, 0);

            Assert.AreEqual<string>("d3f15e41a16af6931839a8e818cb9abd", Encoding.ByteArrayToHexString(input1));


            byte[] expandedKeys2 = Encoding.HexStringToByteArray(
                "af7a7e743ead40cfbc04e390d359ed70f9bd891c25d72aa78f27dba3d0b2922689550342cc1c5616e234a7478551c31c12716645e5b2d4e6dadd251413f3348a221a71868bfb3f18f79cf62b154374c7d8b1ef504288b360652b8fe8a4ee8017397c38b73121778bafc7e24368dedf0c0048789395d40d1d04595bccac8d48b188e0192f7dcb2eb9a0132d2d7eb748a882b53685414cea08de50d83fdf52eba61c5cbe4b9d03e4b0aa674850b485dec2ca8bb8787f90fa9f254292a11d65825ff30d417a56d898721d507d6b1e6b66aa009cc3082ed1c451215b9e9992abba51d4f6a3655a69e43b4e65b99a415c0a75"
            );

            byte[] input2 = Encoding.HexStringToByteArray(
                "f6ab72cec832b4070711b7e3b8700d6a"
            );

            AES.AESBSingleRound(expandedKeys2, input2, 0);

            Assert.AreEqual<string>("e59c503c34b319b45b66a344316cd109", Encoding.ByteArrayToHexString(input2));


            byte[] expandedKeys3 = Encoding.HexStringToByteArray(
                "6a8fd2a6c65c036133649dab4eb09b54148512fc83f1ea9f61aaa0e260848e6320adb182e16bcae13422161f5ca9e600132aaca1be8873dbe5e641eb18412072d41bcbe74c174b4d557acf6e2aeff71f5ec94249f0e781bfd4f9b0d54172d188a17db5c4e69fa309f53a474ebb9640790296d1006f0bfaa223176e74cf4d51116e476b58e8f801e2ddc458e38abeb46ce49f19a9cb1740be0e021b691d59a221d8cd4ff700a1e301f3c8f3edd94956ff1dbacfc925b57a32ea90575548d2a0d6d8dfb8fcfa2130751f7882b984a0d1fa0e6383d1522f9dba6e84365d3236e9564f088cb6fe7b16236fbe65ef61b61eeb"
            );

            byte[] input3 = Encoding.HexStringToByteArray(
                "ac2432e2f84f5f244ef7e5d977c9f19e"
            );

            AES.AESBSingleRound(expandedKeys3, input3, 0);

            Assert.AreEqual<string>("16767345c5adc04004b973481f5682c3", Encoding.ByteArrayToHexString(input3));
        }

        [TestMethod]
        public void TestAESBPseudoRound()
        {
            byte[] expandedKeys1 = Encoding.HexStringToByteArray(
                "4d54c79ab1e27ae04bfe734df26877d7bab89ecbffe598f87d23bbf5ceddc04d97e893355e4d537c4066085159a5515fcfdc9a3a3fe840b61175a446ea75dfb5d7bc2a7a66a268e5e96607e813b8666b443acaff24742a3bed7feaafd78854e170c10468125e99855850588dada7a7f7baf193b754136d748fca760b20628180ece674cfbd44d22168f105d320909d933336f4ffd5afc27eaf8f02b3b289b4e5c8584b928e0ce9b62871f9feae271d4183237e03b88a53c92c549c62366c141599a9995681d612d1206a26eb1559d61f29998383cffce1178c9d1a2c1ba4e8be22b081a7802960dfa6811fe45cf77741"
            );

            byte[] input1 = Encoding.HexStringToByteArray(
                "f77f10a5fb1a52a2b466751c3a858dc3"
            );

            AES.AESBPseudoRound(expandedKeys1, input1, 0);

            Assert.AreEqual<string>("27bfdeec6a2f52b0e815e04a31b40b7f", Encoding.ByteArrayToHexString(input1));


            byte[] expandedKeys2 = Encoding.HexStringToByteArray(
                "af7a7e743ead40cfbc04e390d359ed70f9bd891c25d72aa78f27dba3d0b2922689550342cc1c5616e234a7478551c31c12716645e5b2d4e6dadd251413f3348a221a71868bfb3f18f79cf62b154374c7d8b1ef504288b360652b8fe8a4ee8017397c38b73121778bafc7e24368dedf0c0048789395d40d1d04595bccac8d48b188e0192f7dcb2eb9a0132d2d7eb748a882b53685414cea08de50d83fdf52eba61c5cbe4b9d03e4b0aa674850b485dec2ca8bb8787f90fa9f254292a11d65825ff30d417a56d898721d507d6b1e6b66aa009cc3082ed1c451215b9e9992abba51d4f6a3655a69e43b4e65b99a415c0a75"
            );

            byte[] input2 = Encoding.HexStringToByteArray(
                "f6ab72cec832b4070711b7e3b8700d6a"
            );

            AES.AESBPseudoRound(expandedKeys2, input2, 0);

            Assert.AreEqual<string>("ad2bd997286ac6a1cc6260a4037448ab", Encoding.ByteArrayToHexString(input2));


            byte[] expandedKeys3 = Encoding.HexStringToByteArray(
                "6a8fd2a6c65c036133649dab4eb09b54148512fc83f1ea9f61aaa0e260848e6320adb182e16bcae13422161f5ca9e600132aaca1be8873dbe5e641eb18412072d41bcbe74c174b4d557acf6e2aeff71f5ec94249f0e781bfd4f9b0d54172d188a17db5c4e69fa309f53a474ebb9640790296d1006f0bfaa223176e74cf4d51116e476b58e8f801e2ddc458e38abeb46ce49f19a9cb1740be0e021b691d59a221d8cd4ff700a1e301f3c8f3edd94956ff1dbacfc925b57a32ea90575548d2a0d6d8dfb8fcfa2130751f7882b984a0d1fa0e6383d1522f9dba6e84365d3236e9564f088cb6fe7b16236fbe65ef61b61eeb"
            );

            byte[] input3 = Encoding.HexStringToByteArray(
                "ac2432e2f84f5f244ef7e5d977c9f19e"
            );

            AES.AESBPseudoRound(expandedKeys3, input3, 0);

            Assert.AreEqual<string>("463367a3ac698a8504d5d3267e7a309d", Encoding.ByteArrayToHexString(input3));
        }

        [TestMethod]
        public void TestExpandKey()
        {
            byte[] key1 = Encoding.HexStringToByteArray(
                "f42b7bbf670bbdc8872b2a74a81de5bbaa754e5dd69ae6176263598220bd7a02"
            );

            byte[] expandedKeys1 = AES.ExpandKey(key1);

            if (Sse2.IsSupported && Aes.IsSupported)
            {
                Assert.AreEqual<string>("f42b7bbf670bbdc8872b2a74a81de5bbaa754e5dd69ae6176263598220bd7a028ff10c08e8fab1c06fd19bb4c7cc7e0f6c3ebd2bbaa45b3cd8c702bef87a78bc574d6949bfb7d889d066433d17aa3d329c929a082636c134fef1c38a068bbb366ea76c26d110b4af0176f79216dccaa0db14eee8fd222fdc03d3ec56055857600cfcbc4dddec08e2dc9aff70ca4635d0af4e7898526c574451bfbb1254e7ec728832fc6d55def48f89440bff43023e2f00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000", Encoding.ByteArrayToHexString(expandedKeys1));
            }
            else
            {
                Assert.AreEqual<string>("f42b7bbf670bbdc8872b2a74a81de5bbaa754e5dd69ae6176263598220bd7a028ff10c08e8fab1c06fd19bb4c7cc7e0f6c3ebd2bbaa45b3cd8c702bef87a78bc574d6949bfb7d889d066433d17aa3d329c929a082636c134fef1c38a068bbb366ea76c26d110b4af0176f79216dccaa0db14eee8fd222fdc03d3ec56055857600cfcbc4dddec08e2dc9aff70ca4635d0af4e7898526c574451bfbb1254e7ec728832fc6d55def48f89440bff43023e2fb539ca8de7559dc9b6ea26dbe20dcaa97f462ff52a98db7aa3dcd085e0deeeaa5424e221b3717fe8059b5933e796939aaf9a976185024c1b26de9c9ec6007234", Encoding.ByteArrayToHexString(expandedKeys1));
            }

            byte[] key2 = Encoding.HexStringToByteArray(
                "8220b9a023265e82716dc1a0bd55efb4ca450d6a1a874d2f175bb79bb18b280a"
            );

            byte[] expandedKeys2 = AES.ExpandKey(key2);

            if (Sse2.IsSupported && Aes.IsSupported)
            {
                Assert.AreEqual<string>("8220b9a023265e82716dc1a0bd55efb4ca450d6a1a874d2f175bb79bb18b280abe14de689d3280eaec5f414a510aaefe1b22e9d101a5a4fe16fe1365a7753b6f21f67634bcc4f6de509bb7940191196a67a33dd36606992d70f88a48d78db127783eba3ac4fa4ce49461fb7095f0e21a4d2fa5712b293c5c5bd1b6148c5c07333afb795efe0135ba6a60cecaff902cd05b4fd4017066e85d2bb75e49a7eb597ac330a3023d3196b857515872a8c174a200000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000", Encoding.ByteArrayToHexString(expandedKeys2));
            }
            else
            {
                Assert.AreEqual<string>("8220b9a023265e82716dc1a0bd55efb4ca450d6a1a874d2f175bb79bb18b280abe14de689d3280eaec5f414a510aaefe1b22e9d101a5a4fe16fe1365a7753b6f21f67634bcc4f6de509bb7940191196a67a33dd36606992d70f88a48d78db127783eba3ac4fa4ce49461fb7095f0e21a4d2fa5712b293c5c5bd1b6148c5c07333afb795efe0135ba6a60cecaff902cd05b4fd4017066e85d2bb75e49a7eb597ac330a3023d3196b857515872a8c174a29937463be951ae66c2e6f02f650da95534e35f4f09d2c9f75e839185f642e527db1b9ff7324a3191f0acc1be95a168eb46a6b6654f747f9211f7ee17e7b50b30", Encoding.ByteArrayToHexString(expandedKeys2));
            }

            byte[] key3 = Encoding.HexStringToByteArray(
                "70dad95358fe1be0f79491573c4acc5f84bd6b19a5c5981e14541e9a5cfb780c"
            );

            byte[] expandedKeys3 = AES.ExpandKey(key3);

            if (Sse2.IsSupported && Aes.IsSupported)
            {
                Assert.AreEqual<string>("70dad95358fe1be0f79491573c4acc5f84bd6b19a5c5981e14541e9a5cfb780c7e66271926983cf9d10cadaeed4661f1d1e784b874221ca66076023c3c8d7a3021bc23f207241f0bd628b2a53b6ed3543378e298475afe3e272cfc021ba1863217f8005d10dc1f56c6f4adf3fd9a7ea767c011c4209aeffa07b613f81c1795caefd274c1ff0e6b9739fac664c460b8c37b107dea5b8a92105c3c81e8402b14220e28e7c8f1268c5fc8dc4a3b0cbcf2f800000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000", Encoding.ByteArrayToHexString(expandedKeys3));
            }
            else
            {
                Assert.AreEqual<string>("70dad95358fe1be0f79491573c4acc5f84bd6b19a5c5981e14541e9a5cfb780c7e66271926983cf9d10cadaeed4661f1d1e784b874221ca66076023c3c8d7a3021bc23f207241f0bd628b2a53b6ed3543378e298475afe3e272cfc021ba1863217f8005d10dc1f56c6f4adf3fd9a7ea767c011c4209aeffa07b613f81c1795caefd274c1ff0e6b9739fac664c460b8c37b107dea5b8a92105c3c81e8402b14220e28e7c8f1268c5fc8dc4a3b0cbcf2f88575f4abdeff66bb82c3e753c2e8f371b52544ed4403c8b28cdf828980637071488ea5089671c3b314b224e0d65ad7914b2bc51b0f280da983f78f200394ff51", Encoding.ByteArrayToHexString(expandedKeys3));
            }
        }
    }
}
