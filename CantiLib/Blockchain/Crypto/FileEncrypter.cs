//
// Copyright (c) The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

using System;
using System.Linq;
using System.IO;

using Canti.Data;
using Canti.Errors;
using Canti.Utilities;

namespace Canti.Blockchain.Crypto
{
    /* T is the type that we will save and load */
    public abstract class FileEncrypter<T>
    {
        /* Unencrypt the file in fileName, using the password password, and
           return either an error, or it encoded into type T */
        public abstract IEither<Error, T> Load(string filename,
                                               string password);

        /* Save the data into filename, with the password password */
        public abstract void Save(string filename, string password, T data);

        /* We use this to check that the file is a wallet file, this bit does
           not get encrypted, and we can check if it exists before decrypting.
           If it isn't, it's not a wallet file. */
        protected static byte[] isAWalletIdentifier =
        {
            0x49, 0x66, 0x20, 0x49, 0x20, 0x70, 0x75, 0x6c, 0x6c, 0x20, 0x74,
            0x68, 0x61, 0x74, 0x20, 0x6f, 0x66, 0x66, 0x2c, 0x20, 0x77, 0x69,
            0x6c, 0x6c, 0x20, 0x79, 0x6f, 0x75, 0x20, 0x64, 0x69, 0x65, 0x3f,
            0x0a, 0x49, 0x74, 0x20, 0x77, 0x6f, 0x75, 0x6c, 0x64, 0x20, 0x62,
            0x65, 0x20, 0x65, 0x78, 0x74, 0x72, 0x65, 0x6d, 0x65, 0x6c, 0x79,
            0x20, 0x70, 0x61, 0x69, 0x6e, 0x66, 0x75, 0x6c, 0x2e
        };

        /* We use this to check if the file has been correctly decoded, i.e.
           is the password correct. This gets encrypted into the file, and
           then when unencrypted the file should start with this - if it
           doesn't, the password is wrong */
        protected static byte[] isCorrectPasswordIdentifier =
        {
            0x59, 0x6f, 0x75, 0x27, 0x72, 0x65, 0x20, 0x61, 0x20, 0x62, 0x69,
            0x67, 0x20, 0x67, 0x75, 0x79, 0x2e, 0x0a, 0x46, 0x6f, 0x72, 0x20,
            0x79, 0x6f, 0x75, 0x2e
        };

        /* Add the magic identifier to the beginning of the input string */
        protected static string AddMagicIdentifier(string input,
                                                   byte[] magicIdentifier)
        {
            byte[] inputBytes = Encoding.StringToByteArray(input);

            inputBytes = AddMagicIdentifier(inputBytes, magicIdentifier);

            return Encoding.ByteArrayToString(inputBytes);
        }

        /* Add the magicIdentifier to the beginning of the input array */
        protected static byte[] AddMagicIdentifier(byte[] input,
                                                   byte[] magicIdentifier)
        {
            /* Output array is input array + magicIndentifier length */
            byte[] output = new byte[magicIdentifier.Length + input.Length];

            /* Copy the magic identifier to the start of the output array */
            Buffer.BlockCopy(magicIdentifier, 0, output, 0, 
                             magicIdentifier.Length);

            /* Copy the input to the end of the output array */
            Buffer.BlockCopy(input, 0, output, magicIdentifier.Length,
                             input.Length);

            return output;
        }

        /* Check that a given byte[] has the magicIdentifier as a prefix */
        protected static bool HasMagicIdentifier(byte[] input,
                                                 byte[] magicIdentifier)
        {
            /* The input must be at least as long as the magic identifier */
            if (input.Length < magicIdentifier.Length)
            {
                return false;
            }

            /* Each byte in input must match the byte in magicIdentifier,
               whilst we're checking the first magicIdentifier bytes */
            for (int i = 0; i < magicIdentifier.Length; i++)
            {
                if (input[i] != magicIdentifier[i])
                {
                    return false;
                }
            }

            return true;
        }

        /* Remove the magicIdentifier prefix from a given byte[] */
        protected static byte[] RemoveMagicIdentifier(byte[] input,
                                                      byte[] magicIdentifier)
        {
            return input.Skip(magicIdentifier.Length).ToArray(); 
        }

        /* Get the bytes of the file pointed to by filePath, and return it.
           If the file doesn't exist, or doesn't have the magic identifier,
           return an error */
        protected static IEither<Error, byte[]> GetFileBytesOrError(string
                                                                    filePath)
        {
            /* Make sure the file exists */
            if (!File.Exists(filePath))
            {
                return Either.Left<Error, byte[]>(
                    Error.FileDoesntExist()
                );
            }

            /* Open the file into a byte array */
            byte[] fileData = File.ReadAllBytes(filePath);

            if (!HasMagicIdentifier(fileData, isAWalletIdentifier))
            {
                return Either.Left<Error, byte[]>(
                    Error.FileNotWalletFile()
                );
            }

            /* Don't want to decrypt the magic identifier, so remove it */
            fileData = RemoveMagicIdentifier(fileData, isAWalletIdentifier);

            return Either.Right<Error, byte[]>(fileData);
        }
    }
}
