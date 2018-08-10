//
// Copyright 2018 The TurtleCoin Developers
//
// Please see the included LICENSE file for more information.

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
            using (SHA512 sha = SHA512.Create())
            {
                aes.KeySize = 256;

                /* Hash the password with sha512 to get the AES key, take
                   32 bytes */
                aes.Key = sha.ComputeHash(Encoding.UTF8.GetBytes(password))
                             .Take(32).ToArray();

                aes.BlockSize = 128;

                /*Hash the password with sha512 twice to get the IV, take 
                  16 bytes */
                aes.IV = sha.ComputeHash(sha.ComputeHash(
                    Encoding.UTF8.GetBytes(password)
                )).Take(16).ToArray();

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
            byte[] decryptedBytes;

            using (Aes aes = Aes.Create())
            using (SHA512 sha = SHA512.Create())
            {
                aes.KeySize = 256;

                /* Hash the password with sha512 to get the AES key, take
                   32 bytes */
                aes.Key = sha.ComputeHash(Encoding.UTF8.GetBytes(password))
                             .Take(32).ToArray();

                aes.BlockSize = 128;

                /* Hash the password with sha512 twice to get the IV, take 
                   16 bytes */
                aes.IV = sha.ComputeHash(sha.ComputeHash(
                    Encoding.UTF8.GetBytes(password)
                )).Take(16).ToArray();

                /* Initialize our aes decrypter, using the given Key and IV */
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key,
                                                                 aes.IV);

                string decryptedData;

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
