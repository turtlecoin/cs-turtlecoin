/*
MIT License

Copyright (c) 2018 CodIsAFish
Copyright (c) 2018 The TurtleCoin Developers

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

using Newtonsoft.Json;

/* Clashes with System.Text Encoding namespace */
using Data = Canti.Data;
using Canti.Utilities;
using Canti.Blockchain.WalletBackend;

namespace Canti.Blockchain.Crypto
{
    /* So using the Wallet class doesn't conflict with the Wallet namespace */
    using WalletBackend;

    public class JSONWalletEncrypter : FileEncrypter<WalletBackend>
    {
        /* Returns either an error message, or the unencrypted file */
        public override IEither<string, WalletBackend> Load(string filePath,
                                                            string password)
        {
            /* Get the bytes if we can. If we can, then we decode the bytes.
               Finally, attempt to convert the bytes to a Wallet object.
               If any step fails, we get the first error and return it. */
            return GetFileBytesOrError(filePath).Fmap(
                fileBytes => DecryptBytesOrError(fileBytes, password)
            ).Fmap(TryDecodeJson);
        }

        public override void Save(string filePath, string password,
                                  WalletBackend wallet)
        {
            /* Serialize our wallet as a json string */
            string jsonFileData = JsonConvert.SerializeObject(wallet);

            /* Add the isCorrectPasswordIdentifier to the start of the json
               string. We will encrypt this, and use it to verify that the
               password is correct when decoded */
            jsonFileData = AddMagicIdentifier(jsonFileData,
                                              isCorrectPasswordIdentifier);

            using (Aes aes = Aes.Create())
            using (SHA256 sha256 = SHA256.Create())
            {
                aes.KeySize = 256;

                /* Hash the password with sha256 to get the AES key */
                aes.Key = sha256.ComputeHash(Encoding.UTF8.GetBytes(password))
                                .ToArray();

                aes.BlockSize = 128;

                byte[] IV = SecureRandom.Bytes(16);

                aes.IV = IV;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key,
                                                                 aes.IV);

                /* Open the output file, overwriting any previous files with
                   the same name */
                using (FileStream fileStream = File.Open(
                    filePath, FileMode.Create
                ))
                /* Initialize the crypto stream using the AES encrypter */
                using (CryptoStream cryptoStream = new CryptoStream(
                    fileStream, encryptor, CryptoStreamMode.Write
                ))
                using (StreamWriter streamWriter = new StreamWriter(
                    cryptoStream
                ))
                {
                    /* Write the isAWalletIdentifier to the file, UNENCRYPTED!
                       This is used when opening a file to verify that it is
                       indeed a wallet file, before unencrypting it */
                    fileStream.Write(
                        isAWalletIdentifier, 0, isAWalletIdentifier.Length
                    );

                    /* Write the IV to the file, UNENCRYPTED! We will use this
                       to unencrypt the file when we reopen it. We need the
                       IV to be different on each creation of the file. */
                    fileStream.Write(IV, 0, IV.Length);

                    /* Write the wallet file data to the file, encrypted. */
                    streamWriter.Write(jsonFileData);
                }
            }
        }

        /* Parse the bytes in input into a Wallet class, or return an error */
        private static IEither<string, WalletBackend>
                       TryDecodeJson(byte[] input)
        {
            try
            {
                return Either.Right<string, WalletBackend>(
                    JsonConvert.DeserializeObject<WalletBackend>(
                        Data.Encoding.ByteArrayToString(input)
                    )
                );
            }
            catch
            {
                return Either.Left<string, WalletBackend>(
                    "Failed to parse wallet file! It appears corrupted."
                );
            }
        }

        /* Decrypt the bytes in input with the password password, using AES
           decryption, or return an error */
        private static IEither<string, byte[]>
                       DecryptBytesOrError(byte[] input, string password)
        {
            byte[] IV = new byte[16];

            if (input.Length < IV.Length)
            {
                return Either.Left<string, byte[]>(
                    "Failed to parse wallet file! It appears corrupted."
                );
            }

            /* Extract IV from input */
            Buffer.BlockCopy(input, 0, IV, 0, IV.Length);

            /* Remove the IV from the inputted bytes, we don't need it any
               more */
            input = RemoveMagicIdentifier(input, IV);

            byte[] decryptedBytes;

            using (Aes aes = Aes.Create())
            using (SHA256 sha256 = SHA256.Create())
            {
                aes.KeySize = 256;

                /* Hash the password with sha256 to get the AES key */
                aes.Key = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));

                aes.BlockSize = 128;

                /* Use the random IV we extracted earlier */
                aes.IV = IV;

                /* Initialize our aes decrypter, using the given Key and IV */
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key,
                                                                 aes.IV);

                string decryptedData;

                try
                {
                    using (MemoryStream memoryStream = new MemoryStream(input))
                    /* Decode the AES encrypted file in a stream */
                    using (CryptoStream cryptoStream = new CryptoStream(
                        memoryStream, decryptor, CryptoStreamMode.Read
                    ))
                    /* Write the decoded data into the string */
                    using (StreamReader streamReader = new StreamReader(
                        cryptoStream
                    ))
                    {
                            decryptedData = streamReader.ReadToEnd();
                    }
                }
                /* This exception will be thrown if the data has invalid
                   padding, which indicates an incorrect password */
                catch (CryptographicException)
                {
                    return Either.Left<string, byte[]>(
                        "Incorrect password! Try again."
                    );
                }

                decryptedBytes = Data.Encoding.StringToByteArray(decryptedData);
            }

            if (!HasMagicIdentifier(decryptedBytes,
                                    isCorrectPasswordIdentifier))
            {
                return Either.Left<string, byte[]>(
                    "Incorrect password! Try again."
                );
            }

            /* Remove the magic identifier from the decrypted bytes, we don't
               need it any more */
            decryptedBytes = RemoveMagicIdentifier(decryptedBytes,
                                                   isCorrectPasswordIdentifier);

            return Either.Right<string, byte[]>(decryptedBytes);
        }
    }
}
