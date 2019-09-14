using System;
using System.Runtime.Intrinsics.X86;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using static Canti.Utils;
using Canti.Cryptography.Native;

namespace Tests
{
    [TestClass]
    public class AESTests
    {
        [TestMethod]
        public unsafe void TestAESBSingleRound()
        {
            byte[] expandedKeys1 = HexStringToByteArray(
                "4d54c79ab1e27ae04bfe734df26877d7bab89ecbffe598f87d23bbf5ceddc04d97e893355e4d537c4066085159a5515fcfdc9a3a3fe840b61175a446ea75dfb5d7bc2a7a66a268e5e96607e813b8666b443acaff24742a3bed7feaafd78854e170c10468125e99855850588dada7a7f7baf193b754136d748fca760b20628180ece674cfbd44d22168f105d320909d933336f4ffd5afc27eaf8f02b3b289b4e5c8584b928e0ce9b62871f9feae271d4183237e03b88a53c92c549c62366c141599a9995681d612d1206a26eb1559d61f29998383cffce1178c9d1a2c1ba4e8be22b081a7802960dfa6811fe45cf77741"
            );

            byte[] input1 = HexStringToByteArray(
                "f77f10a5fb1a52a2b466751c3a858dc3"
            );

            /* Test the unsafe version */
            fixed (byte *input = input1, expandedKeys = expandedKeys1)
            {
                AES.AESBSingleRound((uint *)input, (uint *)expandedKeys);

                Assert.AreEqual<string>("d3f15e41a16af6931839a8e818cb9abd", ByteArrayToHexString(input1));
            }

            byte[] expandedKeys2 = HexStringToByteArray(
                "af7a7e743ead40cfbc04e390d359ed70f9bd891c25d72aa78f27dba3d0b2922689550342cc1c5616e234a7478551c31c12716645e5b2d4e6dadd251413f3348a221a71868bfb3f18f79cf62b154374c7d8b1ef504288b360652b8fe8a4ee8017397c38b73121778bafc7e24368dedf0c0048789395d40d1d04595bccac8d48b188e0192f7dcb2eb9a0132d2d7eb748a882b53685414cea08de50d83fdf52eba61c5cbe4b9d03e4b0aa674850b485dec2ca8bb8787f90fa9f254292a11d65825ff30d417a56d898721d507d6b1e6b66aa009cc3082ed1c451215b9e9992abba51d4f6a3655a69e43b4e65b99a415c0a75"
            );

            byte[] input2 = HexStringToByteArray(
                "f6ab72cec832b4070711b7e3b8700d6a"
            );

            fixed (byte *input = input2, expandedKeys = expandedKeys2)
            {
                AES.AESBSingleRound((uint *)input, (uint *)expandedKeys);

                Assert.AreEqual<string>("e59c503c34b319b45b66a344316cd109", ByteArrayToHexString(input2));
            }

            byte[] expandedKeys3 = HexStringToByteArray(
                "6a8fd2a6c65c036133649dab4eb09b54148512fc83f1ea9f61aaa0e260848e6320adb182e16bcae13422161f5ca9e600132aaca1be8873dbe5e641eb18412072d41bcbe74c174b4d557acf6e2aeff71f5ec94249f0e781bfd4f9b0d54172d188a17db5c4e69fa309f53a474ebb9640790296d1006f0bfaa223176e74cf4d51116e476b58e8f801e2ddc458e38abeb46ce49f19a9cb1740be0e021b691d59a221d8cd4ff700a1e301f3c8f3edd94956ff1dbacfc925b57a32ea90575548d2a0d6d8dfb8fcfa2130751f7882b984a0d1fa0e6383d1522f9dba6e84365d3236e9564f088cb6fe7b16236fbe65ef61b61eeb"
            );

            byte[] input3 = HexStringToByteArray(
                "ac2432e2f84f5f244ef7e5d977c9f19e"
            );

            fixed (byte *input = input3, expandedKeys = expandedKeys3)
            {
                AES.AESBSingleRound((uint *)input, (uint *)expandedKeys);

                Assert.AreEqual<string>("16767345c5adc04004b973481f5682c3", ByteArrayToHexString(input3));
            }
        }

        [TestMethod]
        public void TestAESBPseudoRound()
        {
            byte[] expandedKeys1 = HexStringToByteArray(
                "4d54c79ab1e27ae04bfe734df26877d7bab89ecbffe598f87d23bbf5ceddc04d97e893355e4d537c4066085159a5515fcfdc9a3a3fe840b61175a446ea75dfb5d7bc2a7a66a268e5e96607e813b8666b443acaff24742a3bed7feaafd78854e170c10468125e99855850588dada7a7f7baf193b754136d748fca760b20628180ece674cfbd44d22168f105d320909d933336f4ffd5afc27eaf8f02b3b289b4e5c8584b928e0ce9b62871f9feae271d4183237e03b88a53c92c549c62366c141599a9995681d612d1206a26eb1559d61f29998383cffce1178c9d1a2c1ba4e8be22b081a7802960dfa6811fe45cf77741"
            );

            byte[] input1 = HexStringToByteArray(
                "f77f10a5fb1a52a2b466751c3a858dc3"
            );

            AES.AESBPseudoRound(expandedKeys1, input1, 0);

            Assert.AreEqual<string>("27bfdeec6a2f52b0e815e04a31b40b7f", ByteArrayToHexString(input1));


            byte[] expandedKeys2 = HexStringToByteArray(
                "af7a7e743ead40cfbc04e390d359ed70f9bd891c25d72aa78f27dba3d0b2922689550342cc1c5616e234a7478551c31c12716645e5b2d4e6dadd251413f3348a221a71868bfb3f18f79cf62b154374c7d8b1ef504288b360652b8fe8a4ee8017397c38b73121778bafc7e24368dedf0c0048789395d40d1d04595bccac8d48b188e0192f7dcb2eb9a0132d2d7eb748a882b53685414cea08de50d83fdf52eba61c5cbe4b9d03e4b0aa674850b485dec2ca8bb8787f90fa9f254292a11d65825ff30d417a56d898721d507d6b1e6b66aa009cc3082ed1c451215b9e9992abba51d4f6a3655a69e43b4e65b99a415c0a75"
            );

            byte[] input2 = HexStringToByteArray(
                "f6ab72cec832b4070711b7e3b8700d6a"
            );

            AES.AESBPseudoRound(expandedKeys2, input2, 0);

            Assert.AreEqual<string>("ad2bd997286ac6a1cc6260a4037448ab", ByteArrayToHexString(input2));


            byte[] expandedKeys3 = HexStringToByteArray(
                "6a8fd2a6c65c036133649dab4eb09b54148512fc83f1ea9f61aaa0e260848e6320adb182e16bcae13422161f5ca9e600132aaca1be8873dbe5e641eb18412072d41bcbe74c174b4d557acf6e2aeff71f5ec94249f0e781bfd4f9b0d54172d188a17db5c4e69fa309f53a474ebb9640790296d1006f0bfaa223176e74cf4d51116e476b58e8f801e2ddc458e38abeb46ce49f19a9cb1740be0e021b691d59a221d8cd4ff700a1e301f3c8f3edd94956ff1dbacfc925b57a32ea90575548d2a0d6d8dfb8fcfa2130751f7882b984a0d1fa0e6383d1522f9dba6e84365d3236e9564f088cb6fe7b16236fbe65ef61b61eeb"
            );

            byte[] input3 = HexStringToByteArray(
                "ac2432e2f84f5f244ef7e5d977c9f19e"
            );

            AES.AESBPseudoRound(expandedKeys3, input3, 0);

            Assert.AreEqual<string>("463367a3ac698a8504d5d3267e7a309d", ByteArrayToHexString(input3));
        }

        [TestMethod]
        public void TestAESPseudoRoundPlatformSpecific()
        {
            if (!Sse2.IsSupported || !Aes.IsSupported)
            {
                Assert.Inconclusive("Intrinsics not available: skipping test");
            }

            byte[] expandedKeys1 = HexStringToByteArray(
                "4d54c79ab1e27ae04bfe734df26877d7bab89ecbffe598f87d23bbf5ceddc04d97e893355e4d537c4066085159a5515fcfdc9a3a3fe840b61175a446ea75dfb5d7bc2a7a66a268e5e96607e813b8666b443acaff24742a3bed7feaafd78854e170c10468125e99855850588dada7a7f7baf193b754136d748fca760b20628180ece674cfbd44d22168f105d320909d933336f4ffd5afc27eaf8f02b3b289b4e5c8584b928e0ce9b62871f9feae271d4183237e03b88a53c92c549c62366c141599a9995681d612d1206a26eb1559d61f29998383cffce1178c9d1a2c1ba4e8be22b081a7802960dfa6811fe45cf77741"
            );

            byte[] input1 = HexStringToByteArray(
                "c3c8762f17d0dc13d185f5f4157f385fa9ec83a147446c4133878fa76b9661f0f19ea6c328549f02041235743096b4d29714582f9dee802fac27558c850f18e3946b135e6ffc9749736231b8a998ddc4dc74e12ca8ef6a06f6697bf0b870d913478d77c9efac870dd4fc39209b1542cec2f46f9354f1531a99d69198c6b1df54bd551c41ca9abbad92a0224e27445932cb3c61abe50e9b067fd4a70ddc415bc8deea5d3372f2118486fc94f42ad0f60382dcadebe34b03f78fa6f9ae7513e87f92887bcc8d0b20efe1baf00c89935c692fe562e6b7b347237178dff301e429ba84010ffeec5d7469804d95ce3035141a798963a00c4c005f6ef04b06783a71ec"
            );

            AES.AESPseudoRound(expandedKeys1, input1);

            Assert.AreEqual<string>("165fa64cc11eae356bf687a980b271732dc9abe2c01fa7c3463f76b8a0bb8749df6330cd981bd5597ea70085de7e17c61eb0bea0a2ad089a1d7b0a4da14571af8a9c322fb6b0864effb7f6005d0eb7df061e25c5cbfcbc40847348f8067a2fd1dcdf129134a57d98160b0469a975c5d9f7c4d7bed9a81d111938191bbf540205bd551c41ca9abbad92a0224e27445932cb3c61abe50e9b067fd4a70ddc415bc8deea5d3372f2118486fc94f42ad0f60382dcadebe34b03f78fa6f9ae7513e87f92887bcc8d0b20efe1baf00c89935c692fe562e6b7b347237178dff301e429ba84010ffeec5d7469804d95ce3035141a798963a00c4c005f6ef04b06783a71ec", ByteArrayToHexString(input1));

            byte[] expandedKeys2 = HexStringToByteArray(
                "af7a7e743ead40cfbc04e390d359ed70f9bd891c25d72aa78f27dba3d0b2922689550342cc1c5616e234a7478551c31c12716645e5b2d4e6dadd251413f3348a221a71868bfb3f18f79cf62b154374c7d8b1ef504288b360652b8fe8a4ee8017397c38b73121778bafc7e24368dedf0c0048789395d40d1d04595bccac8d48b188e0192f7dcb2eb9a0132d2d7eb748a882b53685414cea08de50d83fdf52eba61c5cbe4b9d03e4b0aa674850b485dec2ca8bb8787f90fa9f254292a11d65825ff30d417a56d898721d507d6b1e6b66aa009cc3082ed1c451215b9e9992abba51d4f6a3655a69e43b4e65b99a415c0a75"
            );

            byte[] input2 = HexStringToByteArray(
                "60a57c351976d1afe74e82261b552389245e61073873680bb09739056f88d519dd4349f56b3235dbe88fa60ca7b567b59074f68cf0585f960201802e0f88bd3cc1468a663d855cc20593783c1c075ab8d175763879459f8fed4af4f4dd5c61fd3ace14b54dac72ee9fd4a64234b255d15210f747a2b538ce3ca3388723a7b7023d919059b5766ae2aa6355687880d198121ccc9d6d3ad6f2c06f117ffc1eaad724bd7f334fd229d1c3b2b19e58e319f23eea44876b5fcd80afc0d25f4ef52df49c0bf7781a38f56c28c6a12faa94aed1fc42f379d818eb49a54efaae69684bb5c6920439ae9fbfc71c938017ddbd90f2ff0259e43db89d9066d94550238b7e25"
            );

            AES.AESPseudoRound(expandedKeys2, input2);

            Assert.AreEqual<string>("12ad0d780c051f720ce0e0369aafebc00be3aeb528e3c8d550f6ee64facfc4e0cbd6684a24c306e8b26ac1cd165f3d6bb84c56c865859cc146f6300bf5622ee2768d3e0bd310c383ee2ccb11d2612bb6e2ca52e79cec2bd3d65a357ab3ab55dadf24360ee180751965b19d4b1dc8f476ae4195907bab494bcf6f6d734cd3d0013d919059b5766ae2aa6355687880d198121ccc9d6d3ad6f2c06f117ffc1eaad724bd7f334fd229d1c3b2b19e58e319f23eea44876b5fcd80afc0d25f4ef52df49c0bf7781a38f56c28c6a12faa94aed1fc42f379d818eb49a54efaae69684bb5c6920439ae9fbfc71c938017ddbd90f2ff0259e43db89d9066d94550238b7e25", ByteArrayToHexString(input2));

            byte[] expandedKeys3 = HexStringToByteArray(
                "6a8fd2a6c65c036133649dab4eb09b54148512fc83f1ea9f61aaa0e260848e6320adb182e16bcae13422161f5ca9e600132aaca1be8873dbe5e641eb18412072d41bcbe74c174b4d557acf6e2aeff71f5ec94249f0e781bfd4f9b0d54172d188a17db5c4e69fa309f53a474ebb9640790296d1006f0bfaa223176e74cf4d51116e476b58e8f801e2ddc458e38abeb46ce49f19a9cb1740be0e021b691d59a221d8cd4ff700a1e301f3c8f3edd94956ff1dbacfc925b57a32ea90575548d2a0d6d8dfb8fcfa2130751f7882b984a0d1fa0e6383d1522f9dba6e84365d3236e9564f088cb6fe7b16236fbe65ef61b61eeb"
            );

            byte[] input3 = HexStringToByteArray(
                "5f45d28d6ba351a8f4574d33f257798f6b84e3fc9ca75c3069c48aa1e3261ebb03659dadde3368bcacc39456d8c1d93886f1d0aa9cf9d7652325bdc9a19a3dd68c97a27c0239da28abb58131e2191b48e13eaa1ed981660bc90a1bfd88b13da791deb17abae4e9b2e792530c9813ce34393f3cbac17b26ec6e970539793aafb5e08bfa63001295721fa66db5c92517ef11737e7ee602cdb39037ab5869cf67d60d705fd604dbb1a9f8da5f85de26fa46ad5034dfb71f3f2e2b0ae690b4a779689b88d16c6dd427e32d08ea9585a4a93142daa5c3177780b31fb808c998cbbdd2c70dc6915bc667d9b66adb3c64ea3af1bab58a56c0eb2bd6a5976248dbdb306e"
            );

            AES.AESPseudoRound(expandedKeys3, input3);

            Assert.AreEqual<string>("c35e19878d7ff188fc6e3841dc95a8664f8ba48dd98e25ee1f679947082e7d68b8b79bd18289275e4e6526260cca18061ae6726fe232683e3eb521f9b95726bb2d631d0512c85840cd5bb7ebd147899972b43c8dd7663bbcd058e0a2904d3707b30dbc20184b8031a3cf5dad64bd84d8c5d9b5c269bfee740a54403d10ffc01ee08bfa63001295721fa66db5c92517ef11737e7ee602cdb39037ab5869cf67d60d705fd604dbb1a9f8da5f85de26fa46ad5034dfb71f3f2e2b0ae690b4a779689b88d16c6dd427e32d08ea9585a4a93142daa5c3177780b31fb808c998cbbdd2c70dc6915bc667d9b66adb3c64ea3af1bab58a56c0eb2bd6a5976248dbdb306e", ByteArrayToHexString(input3));
        }

        [TestMethod]
        public void TestAESPseudoRoundPlatformIndependent()
        {
            byte[] expandedKeys1 = HexStringToByteArray(
                "4d54c79ab1e27ae04bfe734df26877d7bab89ecbffe598f87d23bbf5ceddc04d97e893355e4d537c4066085159a5515fcfdc9a3a3fe840b61175a446ea75dfb5d7bc2a7a66a268e5e96607e813b8666b443acaff24742a3bed7feaafd78854e170c10468125e99855850588dada7a7f7baf193b754136d748fca760b20628180ece674cfbd44d22168f105d320909d933336f4ffd5afc27eaf8f02b3b289b4e5c8584b928e0ce9b62871f9feae271d4183237e03b88a53c92c549c62366c141599a9995681d612d1206a26eb1559d61f29998383cffce1178c9d1a2c1ba4e8be22b081a7802960dfa6811fe45cf77741"
            );

            byte[] input1 = HexStringToByteArray(
                "c3c8762f17d0dc13d185f5f4157f385fa9ec83a147446c4133878fa76b9661f0f19ea6c328549f02041235743096b4d29714582f9dee802fac27558c850f18e3946b135e6ffc9749736231b8a998ddc4dc74e12ca8ef6a06f6697bf0b870d913478d77c9efac870dd4fc39209b1542cec2f46f9354f1531a99d69198c6b1df54bd551c41ca9abbad92a0224e27445932cb3c61abe50e9b067fd4a70ddc415bc8deea5d3372f2118486fc94f42ad0f60382dcadebe34b03f78fa6f9ae7513e87f92887bcc8d0b20efe1baf00c89935c692fe562e6b7b347237178dff301e429ba84010ffeec5d7469804d95ce3035141a798963a00c4c005f6ef04b06783a71ec"
            );

            AES.AESPseudoRound(expandedKeys1, input1, false);

            Assert.AreEqual<string>("165fa64cc11eae356bf687a980b271732dc9abe2c01fa7c3463f76b8a0bb8749df6330cd981bd5597ea70085de7e17c61eb0bea0a2ad089a1d7b0a4da14571af8a9c322fb6b0864effb7f6005d0eb7df061e25c5cbfcbc40847348f8067a2fd1dcdf129134a57d98160b0469a975c5d9f7c4d7bed9a81d111938191bbf540205bd551c41ca9abbad92a0224e27445932cb3c61abe50e9b067fd4a70ddc415bc8deea5d3372f2118486fc94f42ad0f60382dcadebe34b03f78fa6f9ae7513e87f92887bcc8d0b20efe1baf00c89935c692fe562e6b7b347237178dff301e429ba84010ffeec5d7469804d95ce3035141a798963a00c4c005f6ef04b06783a71ec", ByteArrayToHexString(input1));

            byte[] expandedKeys2 = HexStringToByteArray(
                "af7a7e743ead40cfbc04e390d359ed70f9bd891c25d72aa78f27dba3d0b2922689550342cc1c5616e234a7478551c31c12716645e5b2d4e6dadd251413f3348a221a71868bfb3f18f79cf62b154374c7d8b1ef504288b360652b8fe8a4ee8017397c38b73121778bafc7e24368dedf0c0048789395d40d1d04595bccac8d48b188e0192f7dcb2eb9a0132d2d7eb748a882b53685414cea08de50d83fdf52eba61c5cbe4b9d03e4b0aa674850b485dec2ca8bb8787f90fa9f254292a11d65825ff30d417a56d898721d507d6b1e6b66aa009cc3082ed1c451215b9e9992abba51d4f6a3655a69e43b4e65b99a415c0a75"
            );

            byte[] input2 = HexStringToByteArray(
                "60a57c351976d1afe74e82261b552389245e61073873680bb09739056f88d519dd4349f56b3235dbe88fa60ca7b567b59074f68cf0585f960201802e0f88bd3cc1468a663d855cc20593783c1c075ab8d175763879459f8fed4af4f4dd5c61fd3ace14b54dac72ee9fd4a64234b255d15210f747a2b538ce3ca3388723a7b7023d919059b5766ae2aa6355687880d198121ccc9d6d3ad6f2c06f117ffc1eaad724bd7f334fd229d1c3b2b19e58e319f23eea44876b5fcd80afc0d25f4ef52df49c0bf7781a38f56c28c6a12faa94aed1fc42f379d818eb49a54efaae69684bb5c6920439ae9fbfc71c938017ddbd90f2ff0259e43db89d9066d94550238b7e25"
            );

            AES.AESPseudoRound(expandedKeys2, input2, false);

            Assert.AreEqual<string>("12ad0d780c051f720ce0e0369aafebc00be3aeb528e3c8d550f6ee64facfc4e0cbd6684a24c306e8b26ac1cd165f3d6bb84c56c865859cc146f6300bf5622ee2768d3e0bd310c383ee2ccb11d2612bb6e2ca52e79cec2bd3d65a357ab3ab55dadf24360ee180751965b19d4b1dc8f476ae4195907bab494bcf6f6d734cd3d0013d919059b5766ae2aa6355687880d198121ccc9d6d3ad6f2c06f117ffc1eaad724bd7f334fd229d1c3b2b19e58e319f23eea44876b5fcd80afc0d25f4ef52df49c0bf7781a38f56c28c6a12faa94aed1fc42f379d818eb49a54efaae69684bb5c6920439ae9fbfc71c938017ddbd90f2ff0259e43db89d9066d94550238b7e25", ByteArrayToHexString(input2));

            byte[] expandedKeys3 = HexStringToByteArray(
                "6a8fd2a6c65c036133649dab4eb09b54148512fc83f1ea9f61aaa0e260848e6320adb182e16bcae13422161f5ca9e600132aaca1be8873dbe5e641eb18412072d41bcbe74c174b4d557acf6e2aeff71f5ec94249f0e781bfd4f9b0d54172d188a17db5c4e69fa309f53a474ebb9640790296d1006f0bfaa223176e74cf4d51116e476b58e8f801e2ddc458e38abeb46ce49f19a9cb1740be0e021b691d59a221d8cd4ff700a1e301f3c8f3edd94956ff1dbacfc925b57a32ea90575548d2a0d6d8dfb8fcfa2130751f7882b984a0d1fa0e6383d1522f9dba6e84365d3236e9564f088cb6fe7b16236fbe65ef61b61eeb"
            );

            byte[] input3 = HexStringToByteArray(
                "5f45d28d6ba351a8f4574d33f257798f6b84e3fc9ca75c3069c48aa1e3261ebb03659dadde3368bcacc39456d8c1d93886f1d0aa9cf9d7652325bdc9a19a3dd68c97a27c0239da28abb58131e2191b48e13eaa1ed981660bc90a1bfd88b13da791deb17abae4e9b2e792530c9813ce34393f3cbac17b26ec6e970539793aafb5e08bfa63001295721fa66db5c92517ef11737e7ee602cdb39037ab5869cf67d60d705fd604dbb1a9f8da5f85de26fa46ad5034dfb71f3f2e2b0ae690b4a779689b88d16c6dd427e32d08ea9585a4a93142daa5c3177780b31fb808c998cbbdd2c70dc6915bc667d9b66adb3c64ea3af1bab58a56c0eb2bd6a5976248dbdb306e"
            );

            AES.AESPseudoRound(expandedKeys3, input3, false);

            Assert.AreEqual<string>("c35e19878d7ff188fc6e3841dc95a8664f8ba48dd98e25ee1f679947082e7d68b8b79bd18289275e4e6526260cca18061ae6726fe232683e3eb521f9b95726bb2d631d0512c85840cd5bb7ebd147899972b43c8dd7663bbcd058e0a2904d3707b30dbc20184b8031a3cf5dad64bd84d8c5d9b5c269bfee740a54403d10ffc01ee08bfa63001295721fa66db5c92517ef11737e7ee602cdb39037ab5869cf67d60d705fd604dbb1a9f8da5f85de26fa46ad5034dfb71f3f2e2b0ae690b4a779689b88d16c6dd427e32d08ea9585a4a93142daa5c3177780b31fb808c998cbbdd2c70dc6915bc667d9b66adb3c64ea3af1bab58a56c0eb2bd6a5976248dbdb306e", ByteArrayToHexString(input3));
        }

        [TestMethod]
        public void TestExpandKeyPlatformIndependent()
        {
            byte[] key1 = HexStringToByteArray(
                "f42b7bbf670bbdc8872b2a74a81de5bbaa754e5dd69ae6176263598220bd7a02"
            );

            byte[] expandedKeys1 = AES.ExpandKey(key1, false);

            Assert.AreEqual<string>("f42b7bbf670bbdc8872b2a74a81de5bbaa754e5dd69ae6176263598220bd7a028ff10c08e8fab1c06fd19bb4c7cc7e0f6c3ebd2bbaa45b3cd8c702bef87a78bc574d6949bfb7d889d066433d17aa3d329c929a082636c134fef1c38a068bbb366ea76c26d110b4af0176f79216dccaa0db14eee8fd222fdc03d3ec56055857600cfcbc4dddec08e2dc9aff70ca4635d0af4e7898526c574451bfbb1254e7ec728832fc6d55def48f89440bff43023e2fb539ca8de7559dc9b6ea26dbe20dcaa97f462ff52a98db7aa3dcd085e0deeeaa5424e221b3717fe8059b5933e796939aaf9a976185024c1b26de9c9ec6007234", ByteArrayToHexString(expandedKeys1));

            byte[] key2 = HexStringToByteArray(
                "8220b9a023265e82716dc1a0bd55efb4ca450d6a1a874d2f175bb79bb18b280a"
            );

            byte[] expandedKeys2 = AES.ExpandKey(key2, false);

            Assert.AreEqual<string>("8220b9a023265e82716dc1a0bd55efb4ca450d6a1a874d2f175bb79bb18b280abe14de689d3280eaec5f414a510aaefe1b22e9d101a5a4fe16fe1365a7753b6f21f67634bcc4f6de509bb7940191196a67a33dd36606992d70f88a48d78db127783eba3ac4fa4ce49461fb7095f0e21a4d2fa5712b293c5c5bd1b6148c5c07333afb795efe0135ba6a60cecaff902cd05b4fd4017066e85d2bb75e49a7eb597ac330a3023d3196b857515872a8c174a29937463be951ae66c2e6f02f650da95534e35f4f09d2c9f75e839185f642e527db1b9ff7324a3191f0acc1be95a168eb46a6b6654f747f9211f7ee17e7b50b30", ByteArrayToHexString(expandedKeys2));

            byte[] key3 = HexStringToByteArray(
                "70dad95358fe1be0f79491573c4acc5f84bd6b19a5c5981e14541e9a5cfb780c"
            );

            byte[] expandedKeys3 = AES.ExpandKey(key3, false);

            Assert.AreEqual<string>("70dad95358fe1be0f79491573c4acc5f84bd6b19a5c5981e14541e9a5cfb780c7e66271926983cf9d10cadaeed4661f1d1e784b874221ca66076023c3c8d7a3021bc23f207241f0bd628b2a53b6ed3543378e298475afe3e272cfc021ba1863217f8005d10dc1f56c6f4adf3fd9a7ea767c011c4209aeffa07b613f81c1795caefd274c1ff0e6b9739fac664c460b8c37b107dea5b8a92105c3c81e8402b14220e28e7c8f1268c5fc8dc4a3b0cbcf2f88575f4abdeff66bb82c3e753c2e8f371b52544ed4403c8b28cdf828980637071488ea5089671c3b314b224e0d65ad7914b2bc51b0f280da983f78f200394ff51", ByteArrayToHexString(expandedKeys3));
        }

        [TestMethod]
        public void TestExpandKeyPlatformSpecific()
        {
            if (!Sse2.IsSupported || !Aes.IsSupported)
            {
                Assert.Inconclusive("Intrinsics not available: skipping test");
            }

            byte[] key1 = HexStringToByteArray(
                "f42b7bbf670bbdc8872b2a74a81de5bbaa754e5dd69ae6176263598220bd7a02"
            );

            byte[] expandedKeys1 = AES.ExpandKey(key1);

            /* The intrinsics implementation only fills out the first 176 bytes of the result. */
            Assert.AreEqual<string>("f42b7bbf670bbdc8872b2a74a81de5bbaa754e5dd69ae6176263598220bd7a028ff10c08e8fab1c06fd19bb4c7cc7e0f6c3ebd2bbaa45b3cd8c702bef87a78bc574d6949bfb7d889d066433d17aa3d329c929a082636c134fef1c38a068bbb366ea76c26d110b4af0176f79216dccaa0db14eee8fd222fdc03d3ec56055857600cfcbc4dddec08e2dc9aff70ca4635d0af4e7898526c574451bfbb1254e7ec728832fc6d55def48f89440bff43023e2f00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000", ByteArrayToHexString(expandedKeys1));

            byte[] key2 = HexStringToByteArray(
                "8220b9a023265e82716dc1a0bd55efb4ca450d6a1a874d2f175bb79bb18b280a"
            );

            byte[] expandedKeys2 = AES.ExpandKey(key2);

            Assert.AreEqual<string>("8220b9a023265e82716dc1a0bd55efb4ca450d6a1a874d2f175bb79bb18b280abe14de689d3280eaec5f414a510aaefe1b22e9d101a5a4fe16fe1365a7753b6f21f67634bcc4f6de509bb7940191196a67a33dd36606992d70f88a48d78db127783eba3ac4fa4ce49461fb7095f0e21a4d2fa5712b293c5c5bd1b6148c5c07333afb795efe0135ba6a60cecaff902cd05b4fd4017066e85d2bb75e49a7eb597ac330a3023d3196b857515872a8c174a200000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000", ByteArrayToHexString(expandedKeys2));

            byte[] key3 = HexStringToByteArray(
                "70dad95358fe1be0f79491573c4acc5f84bd6b19a5c5981e14541e9a5cfb780c"
            );

            byte[] expandedKeys3 = AES.ExpandKey(key3);

            Assert.AreEqual<string>("70dad95358fe1be0f79491573c4acc5f84bd6b19a5c5981e14541e9a5cfb780c7e66271926983cf9d10cadaeed4661f1d1e784b874221ca66076023c3c8d7a3021bc23f207241f0bd628b2a53b6ed3543378e298475afe3e272cfc021ba1863217f8005d10dc1f56c6f4adf3fd9a7ea767c011c4209aeffa07b613f81c1795caefd274c1ff0e6b9739fac664c460b8c37b107dea5b8a92105c3c81e8402b14220e28e7c8f1268c5fc8dc4a3b0cbcf2f800000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000", ByteArrayToHexString(expandedKeys3));
        }
    }
}
