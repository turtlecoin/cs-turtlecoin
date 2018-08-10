//
// Copyright 2018 The TurtleCoin Developers
//
// Please see the included LICENSE file for more information.

using System;
using System.IO;

using Canti.Data;
using Canti.Utilities;
using Canti.Blockchain.Crypto;
using Canti.Blockchain.Crypto.Mnemonics;

namespace Canti.Blockchain.Wallet
{
    public class Wallet
    {
        /* Private constructor so we can validate a few prerequisites before
           creating an instance, rather than throwing exceptions, e.g.
           are we overwriting an old file */
        private Wallet(string filename, string password,
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
        }

        /* Makes a new view wallet */
        private Wallet(string filename, string password,
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
        public static IEither<string, Wallet> NewWallet(string filename,
                                                        string password)
        {
            WalletKeys keys = KeyOps.GenerateWalletKeys();

            return NewWallet(filename, password, keys.privateKeys);
        }

        /* Make a new wallet with the given filename and password, and
           mnemonic seed, returning either the wallet or an error */
        public static IEither<string, Wallet> NewWallet(string filename, 
                                                        string password,
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
        public static IEither<string, Wallet> NewWallet(string filename, 
                                                        string password,
                                                        PrivateKeys privateKeys)
        {
            if (!File.Exists(filename))
            {
                return Either.Left<string, Wallet>(
                    "The filename given already exists! Did you mean to"
                  + "open it?"
                );
            }

            /* Create the wallet instance */
            Wallet wallet = new Wallet(filename, password, privateKeys);

            /* Save it */
            wallet.Save();

            /* Return it */
            return Either.Right<string, Wallet>(wallet);
        }

        /* Make a new view wallet with the given filename and password,
           and public view key, returning either the wallet or an error */
        public static IEither<string, Wallet>
                      NewWallet(string filename, string password,
                                PrivateKey privateViewKey, string address)
        {
            if (!File.Exists(filename))
            {
                return Either.Left<string, Wallet>(
                    "The filename given already exists! Did you mean to "
                  + "open it?"
                );
            }

            /* If we can get the keys from the address, make a new view wallet,
               save it, and return it. Else, return the error */
            return Addresses.KeysFromAddress(address).Fmap(
                publicKeys => {
                    Wallet wallet = new Wallet(
                        filename, password, privateViewKey, publicKeys
                    );

                    wallet.Save();

                    return Either.Right<string, Wallet>(wallet);
                }
            );
        }

        /* Load a wallet from the given filename with the given password,
           returning either the wallet, or an error */
        public static IEither<string, Wallet> Load(string filename,
                                                   string password)
        {
            FileEncrypter<Wallet> fileEncrypter = new JSONWalletEncrypter();
            return fileEncrypter.Load(filename, password);
        }

        /* Save the wallet data to the filename and password previously
           specified when loading the wallet */
        public void Save()
        {
            FileEncrypter<Wallet> fileEncrypter = new JSONWalletEncrypter();
            fileEncrypter.Save(filename, password, this);
        }

        /* The private and public keys of this wallet */
        public WalletKeys keys { get; }

        /* The filename this wallet is stored in */
        private string filename;

        /* The password this wallet has */
        private string password;

        bool isViewWallet = false;
    }
}
