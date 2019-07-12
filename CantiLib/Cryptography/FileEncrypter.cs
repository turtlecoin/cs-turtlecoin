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

using System.Linq;
using System.IO;
using System.Text;
using System.Security.Cryptography;

namespace Canti.Cryptography
{
    // T is the type that we will save and load
    public class FileEncrypter
    {
        #region Properties and Fields

        #region Public

        // The encoding method to use for encryption
        public Encoding EncodeMethod { get; set; }

        #endregion

        #region Private

        // An identifier that is added to a file to ensure it's a file containing data we want
        private byte[] FileIdentifier { get; set; }

        // An identifier that is added to a file that ensures a password is correct
        private byte[] PasswordIdentifier { get; set; }

        // The number of iterations to use when hashing the password with PBKDF2
        private const int PBKDF2Iterations = 500_000;

        #endregion

        #endregion

        #region Methods

        #region Public

        // Attempts to unencrypt a file using a specified password
        public bool Load(string FileName, string Password, out byte[] Result)
        {
            // Default result to an empty array
            Result = default;

            // Try to decrypt file bytes
            if (!GetFileBytesOrError(FileName, out byte[] Bytes))
            {
                return false;
            }

            // Try to decrypt bytes
            if (!DecryptBytesOrError(Bytes, Password, out Result))
            {
                return false;
            }

            // Success
            return true;
        }

        // Saves data into a file with a specified password
        public void Save(string FileName, string Password, byte[] Data)
        {
            // Serialize data to json
            string ByteString = Data.ToString(EncodeMethod);

            // Add our password identifier to the json data
            ByteString = AddMagicIdentifier(ByteString, PasswordIdentifier);

            // Create an AES instance and generate a new secure salt
            using var AES = Aes.Create();
            byte[] Salt = SecureRandom.Bytes(16);

            // Use pbkdf2 to generate the AES key
            var PBKDF2 = new Rfc2898DeriveBytes(Password, Salt, PBKDF2Iterations, HashAlgorithmName.SHA256);

            // Setup AES information
            AES.KeySize = 256;
            AES.Key = PBKDF2.GetBytes(16);
            AES.BlockSize = 128;
            AES.Mode = CipherMode.CBC;
            AES.IV = Salt;

            // Create AES encrypter from provided information
            var Encrypter = AES.CreateEncryptor(AES.Key, AES.IV);

            // Open our output file as a filestream
            using var FileStream = File.Open(FileName, FileMode.Create);

            // Initialize a new crypto stream and stream writer
            using var CryptoStream = new CryptoStream(FileStream, Encrypter, CryptoStreamMode.Write);
            using var CryptoWriter = new StreamWriter(CryptoStream);

            // Writer our file identifier to our file (unencrypted)
            FileStream.Write(FileIdentifier, 0, FileIdentifier.Length);

            // Writer our salt to the file (unencrypted)
            FileStream.Write(Salt, 0, Salt.Length);

            // Write our wallet data to the file (encrypted)
            CryptoWriter.Write(ByteString);
        }

        #endregion

        #region Private

        // Adds a magic identifier to the beginning of an input string
        private string AddMagicIdentifier(string Input, byte[] MagicIdentifier)
        {
            return MagicIdentifier.ToString(EncodeMethod) + Input;
        }

        // Checks whether or not a string contains a magic identifier at the start
        private bool HasMagicIdentifier(byte[] Input, byte[] MagicIdentifier)
        {
            // If input length is less than identifier's length, it ain't there
            if (Input.Length < MagicIdentifier.Length)
            {
                return false;
            }

            // Checks each byte at the start of a string
            for (int i = 0; i < MagicIdentifier.Length; i++)
            {
                if (Input[i] != MagicIdentifier[i])
                {
                    return false;
                }
            }

            // Successful, has identifier
            return true;
        }

        // Removes the magic identifier from a byte byte array
        private byte[] RemoveMagicIdentifier(byte[] Input, byte[] MagicIdentifier)
        {
            return Input.Skip(MagicIdentifier.Length).ToArray();
        }

        // Attempts to get bytes from a file
        private bool GetFileBytesOrError(string FilePath, out byte[] Result)
        {
            // Default the result initially
            Result = new byte[0];

            // Ensure the file exists
            if (!File.Exists(FilePath))
            {
                return false;
            }

            // Read all bytes from the given file path
            byte[] FileData;
            try
            {
                FileData = File.ReadAllBytes(FilePath);
            }
            catch
            {
                // File may be in use elsewhere
                return false;
            }

            // Ensure our file identifier is present
            if (!HasMagicIdentifier(FileData, FileIdentifier))
            {
                return false;
            }

            // Remove our file identifier
            Result = RemoveMagicIdentifier(FileData, FileIdentifier);

            // Successful, got file bytes
            return true;
        }

        // Attempts to decrypt a given byte array with a given password
        private bool DecryptBytesOrError(byte[] Input, string Password, out byte[] Result)
        {
            // Default the result initially
            Result = new byte[0];

            // Initialize an array to store our salt in
            byte[] Salt = new byte[16];

            // Ensure the input byte array length is at least the size of our salt
            if (Input.Length < Salt.Length)
            {
                // Wallet corrupted or unable to be read
                return false;
            }

            // Extract salt from input
            Salt = Input.SubBytes(0, Salt.Length);

            // Remove the extracted salt from the input array
            Input = RemoveMagicIdentifier(Input, Salt);

            // Create an AES instance
            using var AES = Aes.Create();

            // Use pbkdf2 to generate the AES key, using the extracted salt
            var PBKDF2 = new Rfc2898DeriveBytes(Password, Salt, PBKDF2Iterations, HashAlgorithmName.SHA256);

            // Setup AES information
            AES.KeySize = 256;
            AES.Key = PBKDF2.GetBytes(16);
            AES.BlockSize = 128;
            AES.Mode = CipherMode.CBC;
            AES.IV = Salt;

            // Create AES decrypter from provided information
            ICryptoTransform Decrypter = AES.CreateDecryptor(AES.Key, AES.IV);

            // Wrap this in a try-catch to capture any decryption errors
            try
            {
                // Create a memory stream to work with our input
                using var MemoryStream = new MemoryStream(Input);

                // Decode the AES encrypted file in a stream
                using var CryptoStream = new CryptoStream(MemoryStream, Decrypter, CryptoStreamMode.Read);

                // Write the decoded data into the string
                using var StreamReader = new StreamReader(CryptoStream);

                // Convert decoded data to a byte array
                string DecryptedData = StreamReader.ReadToEnd();
                byte[] DecryptedBytes = DecryptedData.GetBytes(EncodeMethod);

                // Check it decoded by verifying the password identifier bytes are present
                if (!HasMagicIdentifier(DecryptedBytes, PasswordIdentifier))
                {
                    return false;
                }

                // Remove the magic identifier from the decrypted bytes, we don't need it any more
                Result = RemoveMagicIdentifier(DecryptedBytes, PasswordIdentifier);
                return true;
            }

            // Failed to decrypt, generally means a wrong password
            catch (CryptographicException)
            {
                return false;
            }
        }

        #endregion

        #endregion

        #region Constructor

        // Initializes a new file encrypter with a given set of identifiers
        public FileEncrypter(byte[] FileIdentifier, byte[] PasswordIdentifier)
        {
            this.EncodeMethod = Encoding.Default;
            this.FileIdentifier = FileIdentifier;
            this.PasswordIdentifier = PasswordIdentifier;
        }

        #endregion
    }
}