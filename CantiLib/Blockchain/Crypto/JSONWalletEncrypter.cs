/*
MIT License

Copyright (c) 2018 Adrian Herridge
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
using System.Security.Cryptography;

using Newtonsoft.Json;

using Canti.Errors;
using Canti.Utilities;

namespace Canti.Blockchain.Crypto
{
    /* So using the WalletBackend class doesn't conflict with the
       WalletBackend namespace */
    using WalletBackend;

    public class JSONWalletEncrypter : FileEncrypter<WalletBackend>
    {
        /* The number of iterations to use when hashing the password with
           PBKDF2 - intended to take roughly 1 second */
        private const int PBKDF2Iterations = 500_000;

        /* Returns either an error message, or the unencrypted file */
        public override IEither<Error, WalletBackend> Load(string filePath,
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

            using (var aes = Aes.Create())
            {
                byte[] salt = SecureRandom.Bytes(16);

                /* Use pbkdf2 to generate the AES key */
                var pbkdf2 = new Rfc2898DeriveBytes(password, salt,
                                                    PBKDF2Iterations);

                /* 16 byte / 256 bit key */
                aes.KeySize = 256;
                aes.Key = pbkdf2.GetBytes(16);

                aes.BlockSize = 128;

                aes.IV = salt;

                var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                /* Open the output file, overwriting any previous files with
                   the same name */
                using (var fileStream = File.Open(filePath, FileMode.Create))
                /* Initialize the crypto stream using the AES encrypter */
                using (var cryptoStream = new CryptoStream(
                    fileStream, encryptor, CryptoStreamMode.Write
                ))
                using (var streamWriter = new StreamWriter(cryptoStream))
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
                    fileStream.Write(salt, 0, salt.Length);

                    /* Write the wallet file data to the file, encrypted. */
                    streamWriter.Write(jsonFileData);
                }
            }
        }

        /* Parse the bytes in input into a Wallet class, or return an error */
        private static IEither<Error, WalletBackend>
                       TryDecodeJson(byte[] input)
        {
            try
            {
                return Either.Right<Error, WalletBackend>(
                    JsonConvert.DeserializeObject<WalletBackend>(
                        Data.Encoding.ByteArrayToString(input)
                    )
                );
            }
            catch (JsonSerializationException e)
            {
                return Either.Left<Error, WalletBackend>(
                    Error.WalletCorrupted(e.ToString())
                );
            }
        }

        /* Decrypt the bytes in input with the password password, using AES
           decryption, or return an error */
        private static IEither<Error, byte[]>
                       DecryptBytesOrError(byte[] input, string password)
        {
            byte[] salt = new byte[16];

            if (input.Length < salt.Length)
            {
                return Either.Left<Error, byte[]>(Error.WalletCorrupted());
            }

            /* Extract salt from input */
            Buffer.BlockCopy(input, 0, salt, 0, salt.Length);

            /* Remove the salt from the inputted bytes, we don't need it any
               more */
            input = RemoveMagicIdentifier(input, salt);

            byte[] decryptedBytes;

            using (var aes = Aes.Create())
            {
                /* Use pbkdf2 to generate the AES key, using the extracted
                   salt */
                var pbkdf2 = new Rfc2898DeriveBytes(password, salt,
                                                    PBKDF2Iterations);

                /* 16 byte / 256 bit key */
                aes.KeySize = 256;
                aes.Key = pbkdf2.GetBytes(16);

                aes.BlockSize = 128;

                /* Use the extracted salt as the IV */
                aes.IV = salt;

                /* Initialize our aes decrypter, using the given Key and IV */
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key,
                                                                 aes.IV);

                string decryptedData;

                try
                {
                    using (var memoryStream = new MemoryStream(input))
                    /* Decode the AES encrypted file in a stream */
                    using (var cryptoStream = new CryptoStream(
                        memoryStream, decryptor, CryptoStreamMode.Read
                    ))
                    /* Write the decoded data into the string */
                    using (var streamReader = new StreamReader(cryptoStream))
                    {
                        decryptedData = streamReader.ReadToEnd();
                    }
                }
                /* This exception will be thrown if the data has invalid
                   padding, which indicates an incorrect password.

                   !! MAKE SURE YOU USE A GENERIC WRONG PASSWORD ERROR HERE !!

                   Otherwise, I believe this can be abused to decrypt the
                   plaintext by using it as a padding oracle attack. */
                catch (CryptographicException)
                {
                    return Either.Left<Error, byte[]>(
                        Error.IncorrectPassword()
                    );
                }

                decryptedBytes = Data.Encoding.StringToByteArray(decryptedData);
            }

            /* Check it decoded by verifying the isCorrectPasswordIdentifier
               bytes are present */
            if (!HasMagicIdentifier(decryptedBytes,
                                    isCorrectPasswordIdentifier))
            {
                return Either.Left<Error, byte[]>(Error.IncorrectPassword());
            }

            /* Remove the magic identifier from the decrypted bytes, we don't
               need it any more */
            decryptedBytes = RemoveMagicIdentifier(decryptedBytes,
                                                   isCorrectPasswordIdentifier);

            return Either.Right<Error, byte[]>(decryptedBytes);
        }
    }
}