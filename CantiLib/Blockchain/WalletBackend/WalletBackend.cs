//
// Copyright 2018 The TurtleCoin Developers
//
// Please see the included LICENSE file for more information.

using System;
using System.IO;
using System.Collections.Generic;

using Canti.Data;
using Canti.Errors;
using Canti.Utilities;
using Canti.Blockchain.Crypto;
using Canti.Blockchain.Crypto.Mnemonics;

namespace Canti.Blockchain.WalletBackend
{
    public class WalletBackend
    {
        /* So it can't be called by end users */
        private WalletBackend() {}

        /* Private constructor so we can validate a few prerequisites before
           creating an instance, rather than throwing exceptions, e.g.
           are we overwriting an old file */
        private WalletBackend(string filename, string password,
                              PrivateKeys privateKeys)
        {
            PublicKey publicSpendKey = KeyOps.PrivateKeyToPublicKey(
                privateKeys.spendKey
            );

            PublicKey publicViewKey = KeyOps.PrivateKeyToPublicKey(
                privateKeys.viewKey
            );

            this.filename = filename;
            this.password = password;

            this.keys = new WalletKeys(
                publicSpendKey, privateKeys.spendKey,
                publicViewKey, privateKeys.viewKey
            );

            this.addresses = new List<string>
            {
                Addresses.AddressFromKeys(publicSpendKey, publicViewKey)
            };
        }

        /* Makes a new view wallet */
        private WalletBackend(string filename, string password,
                              PrivateKey privateViewKey, PublicKeys publicKeys)
        {
            this.filename = filename;
            this.password = password;

            this.isViewWallet = true;

            /* View wallets don't have a spend key */
            this.keys = new WalletKeys(
                publicKeys.spendKey, null,
                publicKeys.viewKey, privateViewKey
            );
        }

        /* Make a new wallet with the given filename and password, returning
           either the wallet or an error */
        public static IEither<Error, WalletBackend> NewWallet(string filename,
                                                              string password)
        {
            WalletKeys keys = KeyOps.GenerateWalletKeys();

            return NewWallet(filename, password, keys.GetPrivateKeys());
        }

        /* Make a new wallet with the given filename and password, and
           mnemonic seed, returning either the wallet or an error */
        public static IEither<Error, WalletBackend>
                      NewWallet(string filename, string password,
                                string mnemonicSeed)
        {
            /* Derive the mnemonic into a private spend key if possible */
            return Mnemonics.MnemonicToPrivateKey(mnemonicSeed)
                            .Fmap(privateSpendKey => {
                /* Derive the private view key from the private spend key */
                var privateKeys = new PrivateKeys(
                    privateSpendKey,
                    KeyOps.GenerateDeterministicKeys(privateSpendKey).privateKey
                );

                /* Try and create the new wallet from the private keys */
                return NewWallet(filename, password, privateKeys);
            });
        }

        /* Make a new wallet with the given filename and password, and
           private spend and view key, returning either the wallet or
           an error */
        public static IEither<Error, WalletBackend>
                      NewWallet(string filename, string password,
                                PrivateKeys privateKeys)
        {
            if (!KeyOps.IsValidKey(privateKeys.spendKey)
             || !KeyOps.IsValidKey(privateKeys.viewKey))
            {
                return Either.Left<Error, WalletBackend>(
                    Error.InvalidPrivateKey()
                );
            }

            if (File.Exists(filename))
            {
                return Either.Left<Error, WalletBackend>(
                    Error.FileAlreadyExists()
                );
            }

            /* Create the wallet instance */
            WalletBackend wallet = new WalletBackend(
                filename, password, privateKeys
            );

            /* Save it */
            wallet.Save();

            /* Return it */
            return Either.Right<Error, WalletBackend>(wallet);
        }

        /* Make a new view wallet with the given filename and password,
           and public view key, returning either the wallet or an error */
        public static IEither<Error, WalletBackend>
                      NewWallet(string filename, string password,
                                PrivateKey privateViewKey, string address)
        {
            if (!KeyOps.IsValidKey(privateViewKey))
            {
                return Either.Left<Error, WalletBackend>(
                    Error.InvalidPrivateKey()
                );
            }

            if (File.Exists(filename))
            {
                return Either.Left<Error, WalletBackend>(
                    Error.FileAlreadyExists()
                );
            }

            /* If we can get the keys from the address, make a new view wallet,
               save it, and return it. Else, return the error */
            return Addresses.KeysFromAddress(address).Fmap(
                publicKeys => {
                    WalletBackend wallet = new WalletBackend(
                        filename, password, privateViewKey, publicKeys
                    );

                    wallet.Save();

                    return Either.Right<Error, WalletBackend>(wallet);
                }
            );
        }

        /* Load a wallet from the given filename with the given password,
           returning either the wallet, or an error */
        public static IEither<Error, WalletBackend> Load(string filename,
                                                         string password)
        {
            FileEncrypter<WalletBackend> fileEncrypter
                = new JSONWalletEncrypter();

            return fileEncrypter.Load(filename, password).Fmap(
                /* Loading filename / password from file is dumb. The filename
                   can for sure change - the password maybe could? */
                wallet => {
                    wallet.filename = filename;
                    wallet.password = password;
                    return Either.Right<Error, WalletBackend>(wallet);
                }
            );
        }

        /* Save the wallet data to the filename and password previously
           specified when loading the wallet */
        public void Save()
        {
            FileEncrypter<WalletBackend> fileEncrypter
                = new JSONWalletEncrypter();

            fileEncrypter.Save(filename, password, this);
        }

        /* NOTE: EVERYTHING BELOW MUST HAVE A SETTER OR BE PUBLIC, ELSE
           THE JSON SERIALIZATION WILL NOT SET ITS VALUE
           
           This can of course be used for internal fields we don't want to
           serialize. */

        /* The private and public keys of this wallet */
        public WalletKeys keys { get; set; }

        /* The filename this wallet is stored in */
        public string filename { get; set; }

        /* The password this wallet has */
        public string password { get; set; }

        /* The addresses this wallet contains */
        public List<string> addresses { get; set; }

        /* Is the wallet a view only wallet */
        public bool isViewWallet { get; set; } = false;
    }
}
