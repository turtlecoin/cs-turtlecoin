//
// Copyright (c) 2018, The TurtleCoin Developers
//
// Please see the included LICENSE file for more information.

using System;

namespace Canti.Errors
{
    public class Error
    {
        /* We want all errors to be declared statically in this file for ease
           of finding the definitions */
        private Error() {}

        private Error(string errorMessage, string errorName)
        {
            this.errorMessage = errorMessage;
            this.errorName = errorName;
        }

        /* A human readable error message, for example:
          "Mnemonic seed is not 25 words"  */
        public string errorMessage { get; }
        
        /* A short programmatic name, intended to be matched against, for 
           example: "MNEMONIC_SEED_NOT_25_WORDS". This allows us to reword
           the error messages without making the rpc users have to change
           their implementation */
        public string errorName { get; }

        /* The user is attempting to create a wallet file that already exists,
           don't want to overwrite an existing file, wallet or not. */
        public static Error FileAlreadyExists()
        {
            return new Error(
                "The filename given already exists! Did you mean to open it?",
                "FILE_ALREADY_EXISTS"
            );
        }

        /* The user is attempting to open a wallet file that doesn't exist. */
        public static Error FileDoesntExist()
        {
            return new Error(
                "The filename given doesn't exist! Ensure you entered it "
               + "correctly.",
                "FILE_DOESNT_EXIST"
            );
        }

        /* The file given doesn't have the leading magic bytes which indicate
           the file is a wallet file */
        public static Error FileNotWalletFile()
        {
            return new Error(
                "The filename given does not appear to be a wallet file!",
                "FILE_NOT_WALLET"
            );
        }

        /* The wallet file passed the magic bytes tests, but was unable to
           be decoded from JSON, indicating corruption, or we were unable
           to read 16 bytes for the IV from the file */
        public static Error WalletCorrupted()
        {
            return new Error(
                "Failed to parse wallet file! It appears corrupted.",
                "WALLET_FILE_CORRUPTED"
            );
        }

        /* The wallet is a valid wallet file, but it failed to decode or
           had the incorrect magic bytes once decoded, so the password is
           wrong */
        public static Error IncorrectPassword()
        {
            return new Error(
                "Incorrect password! Try again.",
                "INCORRECT_PASSWORD"
            );
        }

        /* The given address has characters which are not present in the
           base58 alphabet */
        public static Error AddressNotBase58()
        {
            return new Error(
                "Address is not a valid base58 string!",
                "ADDRESS_NOT_BASE_58"
            );
        }

        /* The given address is the wrong length */
        public static Error AddressWrongLength()
        {
            return new Error(
                "Address is not expected length!",
                "ADDRESS_WRONG_LENGTH"
            );
        }

        /* The given address has the wrong prefix, i.e. it doesn't start with
           TRTL, or whatever your coins prefix is. */
        public static Error AddressWrongPrefix()
        {
            return new Error(
                "Address prefix is incorrect!",
                "ADDRESS_WRONG_PREFIX"
            );
        }

        /* The address has the incorrect checksum, indicating it is possibly
           incorrectly copied. To get the checksum, the prefix + spendKey
           + viewKey is hashed with keccak, and the first 4 bytes are taken,
           and appended to the data, which is then encoded with base58 */
        public static Error AddressWrongChecksum()
        {
            return new Error(
                "Address checksum is incorrect!",
                "ADDRESS_WRONG_CHECKSUM"
            );
        }

        /* The inputted mnemonic is the wrong amount of words - it should be
           25 */
        public static Error MnemonicWrongLength(int actualLength)
        {
            string word = actualLength == 1 ? "word" : "words";

            return new Error(
                "Mnemonic seed is wrong length - It should be 25 words " +
               $"long, but it is {actualLength} {word} long!",
                "MNEMONIC_WRONG_LENGTH"
            );
        }

        /* The inputted mnemonic has a word which is not present in the
           English word list */
        public static Error MnemonicWrongWord(string word)
        {
            return new Error(
               $"Mnemonic seed has invalid word - {word} is not " +
                "in the English word list!",
                "MNEMONIC_WRONG_WORD"
            );
        }

        /* The inputted mnemonic has an incorrect checksum. This checksum is
           calculated by taking the first 3 chars from each word, then
           calculating crc32 on it. This gives us a number, which is then
           taken modulus (%) 24, to give us an index within the 24 words.
           This index gives us the checksum word, which is appended to the
           original 24 words. */
        public static Error MnemonicWrongChecksum()
        {
            return new Error(
                "Mnemonic seed has incorrect checksum!",
                "MNEMONIC_WRONG_CHECKSUM"
            );
        }

        /* The mnemonic is invalid. Unfortunately I'm not sure what the bit
           of code that throws this error is actually testing, and if it
           will ever be called or if the previous error tests will catch all
           invalid mnemonics. */
        public static Error InvalidMnemonic()
        {
            return new Error(
                "Invalid mnemonic!",
                "INVALID_MNEMONIC"
            );
        }
    }
}
