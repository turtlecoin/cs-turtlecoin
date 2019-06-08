//
// Copyright (c) 2019 Canti, The TurtleCoin Developers
//
// Please see the included LICENSE file for more information.

using System;
using System.Runtime.InteropServices;
using static Canti.Utils;

namespace Canti.Cryptography
{
    public sealed partial class TurtleCoinCrypto : ICryptography
    {
        #region Fast Hash

        public string CN_FastHash(string Data)
        {
            if (Data.Length % 2 != 0) return null;

            IntPtr output = new IntPtr();

            _cn_fast_hash(Data, ref output);

            return Marshal.PtrToStringAnsi(output);
        }
        public string CN_FastHash(byte[] Data)
        {
            return CN_FastHash(ByteArrayToHexString(Data));
        }

        #endregion

        #region Slow Hash

        public string CN_SlowHashV0(string Data)
        {
            if (Data.Length % 2 != 0) return null;

            IntPtr output = new IntPtr();

            _cn_slow_hash_v0(Data, ref output);

            return Marshal.PtrToStringAnsi(output);
        }

        public string CN_SlowHashV1(string Data)
        {
            if (Data.Length % 2 != 0 || Data.Length < MINIMUM_VARIATION_BYTES) return null;

            IntPtr output = new IntPtr();

            _cn_slow_hash_v1(Data, ref output);

            return Marshal.PtrToStringAnsi(output);
        }

        public string CN_SlowHashV2(string Data)
        {
            if (Data.Length % 2 != 0 || Data.Length < MINIMUM_VARIATION_BYTES) return null;

            IntPtr output = new IntPtr();

            _cn_slow_hash_v2(Data, ref output);

            return Marshal.PtrToStringAnsi(output);
        }

        #endregion

        #region Lite Slow Hash

        public string CN_LiteSlowHashV0(string Data)
        {
            if (Data.Length % 2 != 0) return null;

            IntPtr output = new IntPtr();

            _cn_lite_slow_hash_v0(Data, ref output);

            return Marshal.PtrToStringAnsi(output);
        }

        public string CN_LiteSlowHashV1(string Data)
        {
            if (Data.Length % 2 != 0 || Data.Length < MINIMUM_VARIATION_BYTES) return null;

            IntPtr output = new IntPtr();

            _cn_lite_slow_hash_v1(Data, ref output);

            return Marshal.PtrToStringAnsi(output);
        }

        public string CN_LiteSlowHashV2(string Data)
        {
            if (Data.Length % 2 != 0 || Data.Length < MINIMUM_VARIATION_BYTES) return null;

            IntPtr output = new IntPtr();

            _cn_lite_slow_hash_v2(Data, ref output);

            return Marshal.PtrToStringAnsi(output);
        }

        #endregion

        #region Dark Slow Hash

        public string CN_DarkSlowHashV0(string Data)
        {
            if (Data.Length % 2 != 0) return null;

            IntPtr output = new IntPtr();

            _cn_dark_slow_hash_v0(Data, ref output);

            return Marshal.PtrToStringAnsi(output);
        }

        public string CN_DarkSlowHashV1(string Data)
        {
            if (Data.Length % 2 != 0 || Data.Length < MINIMUM_VARIATION_BYTES) return null;

            IntPtr output = new IntPtr();

            _cn_dark_slow_hash_v1(Data, ref output);

            return Marshal.PtrToStringAnsi(output);
        }

        public string CN_DarkSlowHashV2(string Data)
        {
            if (Data.Length % 2 != 0 || Data.Length < MINIMUM_VARIATION_BYTES) return null;

            IntPtr output = new IntPtr();

            _cn_dark_slow_hash_v2(Data, ref output);

            return Marshal.PtrToStringAnsi(output);
        }

        #endregion

        #region Dark Lite Slow Hash

        public string CN_DarkLiteSlowHashV0(string Data)
        {
            if (Data.Length % 2 != 0) return null;

            IntPtr output = new IntPtr();

            _cn_dark_lite_slow_hash_v0(Data, ref output);

            return Marshal.PtrToStringAnsi(output);
        }

        public string CN_DarkLiteSlowHashV1(string Data)
        {
            if (Data.Length % 2 != 0 || Data.Length < MINIMUM_VARIATION_BYTES) return null;

            IntPtr output = new IntPtr();

            _cn_dark_lite_slow_hash_v1(Data, ref output);

            return Marshal.PtrToStringAnsi(output);
        }

        public string CN_DarkLiteSlowHashV2(string Data)
        {
            if (Data.Length % 2 != 0 || Data.Length < MINIMUM_VARIATION_BYTES) return null;

            IntPtr output = new IntPtr();

            _cn_dark_lite_slow_hash_v2(Data, ref output);

            return Marshal.PtrToStringAnsi(output);
        }

        #endregion

        #region Turtle Slow Hash

        public string CN_TurtleSlowHashV0(string Data)
        {
            if (Data.Length % 2 != 0) return null;

            IntPtr output = new IntPtr();

            _cn_turtle_slow_hash_v0(Data, ref output);

            return Marshal.PtrToStringAnsi(output);
        }

        public string CN_TurtleSlowHashV1(string Data)
        {
            if (Data.Length % 2 != 0 || Data.Length < MINIMUM_VARIATION_BYTES) return null;

            IntPtr output = new IntPtr();

            _cn_turtle_slow_hash_v1(Data, ref output);

            return Marshal.PtrToStringAnsi(output);
        }

        public string CN_TurtleSlowHashV2(string Data)
        {
            if (Data.Length % 2 != 0 || Data.Length < MINIMUM_VARIATION_BYTES) return null;

            IntPtr output = new IntPtr();

            _cn_turtle_slow_hash_v2(Data, ref output);

            return Marshal.PtrToStringAnsi(output);
        }

        #endregion

        #region Turtle Lite Slow Hash

        public string CN_TurtleLiteSlowHashV0(string Data)
        {
            if (Data.Length % 2 != 0) return null;

            IntPtr output = new IntPtr();

            _cn_turtle_lite_slow_hash_v0(Data, ref output);

            return Marshal.PtrToStringAnsi(output);
        }

        public string CN_TurtleLiteSlowHashV1(string Data)
        {
            if (Data.Length % 2 != 0 || Data.Length < MINIMUM_VARIATION_BYTES) return null;

            IntPtr output = new IntPtr();

            _cn_turtle_lite_slow_hash_v1(Data, ref output);

            return Marshal.PtrToStringAnsi(output);
        }

        public string CN_TurtleLiteSlowHashV2(string Data)
        {
            if (Data.Length % 2 != 0 || Data.Length < MINIMUM_VARIATION_BYTES) return null;

            IntPtr output = new IntPtr();

            _cn_turtle_lite_slow_hash_v2(Data, ref output);

            return Marshal.PtrToStringAnsi(output);
        }

        #endregion

        #region Soft Shell Slow Hash

        public string CN_SoftShellSlowHashV0(string Data, uint height)
        {
            if (Data.Length % 2 != 0) return null;

            IntPtr output = new IntPtr();

            _cn_soft_shell_slow_hash_v0(Data, height, ref output);

            return Marshal.PtrToStringAnsi(output);
        }

        public string CN_SoftShellSlowHashV1(string Data, uint height)
        {
            if (Data.Length % 2 != 0 || Data.Length < MINIMUM_VARIATION_BYTES) return null;

            IntPtr output = new IntPtr();

            _cn_soft_shell_slow_hash_v1(Data, height, ref output);

            return Marshal.PtrToStringAnsi(output);
        }

        public string CN_SoftShellSlowHashV2(string Data, uint height)
        {
            if (Data.Length % 2 != 0 || Data.Length < MINIMUM_VARIATION_BYTES) return null;

            IntPtr output = new IntPtr();

            _cn_soft_shell_slow_hash_v2(Data, height, ref output);

            return Marshal.PtrToStringAnsi(output);
        }

        #endregion

        #region Chukwa Slow Hash

        public string ChukwaSlowHash(string Data)
        {
            if (Data.Length % 2 != 0) return null;

            IntPtr output = new IntPtr();

            _chukwa_slow_hash(Data, ref output);

            return Marshal.PtrToStringAnsi(output);
        }

        #endregion
    }
}
