//
// Copyright Dan Bernstein (djb) from Ref10 in SUPERCOP
// Copyright 2012-2013 The CryptoNote Developers
// Copyright 2014-2018 The Monero Developers
// Copyright 2018 The TurtleCoin Developers
//
// Please see the included LICENSE file for more information.

using System;

/* Here be dragons */
namespace Canti.Cryptography.Native
{
    public static partial class ED25519
    {
        /* This calculates (input[0]) + (input[1] * 2^8) + (input[2] * 2^16)
           Takes an optional offset to simulate passing an offseted pointer */
        public static ulong load_3(byte[] input, int offset = 0)
        {
            ulong result;
            result = (ulong)input[0 + offset];
            result |= (ulong)input[1 + offset] << 8;
            result |= (ulong)input[2 + offset] << 16;
            return result;
        }
    
        /* This calculates (input[0]) + (input[1] * 2^8) + (input[2] * 2^16)
           + (input[3] * 2^24)
           Takes an optional offset to simulate passing an offseted pointer */
        public static ulong load_4(byte[] input, int offset = 0)
        {
            ulong result;
            result = (ulong)input[0 + offset];
            result |= (ulong)input[1 + offset] << 8;
            result |= (ulong)input[2 + offset] << 16;
            result |= (ulong)input[3 + offset] << 24;
            return result;
        }

        /* Zeroes an fe (int[10]) */
        public static void fe_0(ref int[] x)
        {
            for (int i = 0; i < 10; i++)
            {
                x[i] = 0;
            }
        }

        /* Sets the first value of an fe (int[10]) to 1, and zeroes the next
           nine */
        public static void fe_1(ref int[] x)
        {
            x[0] = 1;

            for (int i = 1; i < 10; i++)
            {
                x[i] = 0;
            }
        }

        /* Adds two fe's (int[10]) together and stores the result in h */
        public static void fe_add(ref int[] h, int[] f, int[] g)
        {
            for (int i = 0; i < 10; i++)
            {
                h[i] = f[i] + g[i];
            }
        }

        /* If b == 0, do nothing. If b == 1, replace f with g */
        public static void fe_cmov(ref int[] f, int[] g, uint b)
        {
            if (b == 0)
            {
                return;
            }
            else if (b == 1)
            {
                for (int i = 0; i < 10; i++)
                {
                    f[i] = g[i];
                }
            }
            else
            {
                throw new ArgumentException("B must be 0 or 1");
            }
        }

        /* Sets h to f */
        public static void fe_copy(ref int[] h, int[] f)
        {
            for (int i = 0; i < 10; i++)
            {
                h[i] = f[i];
            }
        }

        public static void fe_invert(ref int[] ret, int[] z)
        {
            int[] t0 = new int[10];
            int[] t1 = new int[10];
            int[] t2 = new int[10];
            int[] t3 = new int[10];

            fe_sq(ref t0, z);
            fe_sq(ref t1, t0);
            fe_sq(ref t1, t1);

            fe_mul(ref t1, z, t1);
            fe_mul(ref t0, t0, t1);

            fe_sq(ref t2, t0);

            fe_mul(ref t1, t1, t2);

            fe_sq(ref t2, t1);

            for (int i = 0; i < 4; ++i)
            {
                fe_sq(ref t2, t2);
            }

            fe_mul(ref t1, t2, t1);
            fe_sq(ref t2, t1);

            for (int i = 0; i < 9; ++i)
            {
                fe_sq(ref t2, t2);
            }

            fe_mul(ref t2, t2, t1);

            fe_sq(ref t3, t2);

            for (int i = 0; i < 19; ++i)
            {
                fe_sq(ref t3, t3);
            }

            fe_mul(ref t2, t3, t2);

            fe_sq(ref t2, t2);

            for (int i = 0; i < 9; ++i)
            {
                fe_sq(ref t2, t2);
            }

            fe_mul(ref t1, t2, t1);

            fe_sq(ref t2, t1);

            for (int i = 0; i < 49; ++i)
            {
                fe_sq(ref t2, t2);
            }

            fe_mul(ref t2, t2, t1);

            fe_sq(ref t3, t2);

            for (int i = 0; i < 99; ++i)
            {
                fe_sq(ref t3, t3);
            }

            fe_mul(ref t2, t3, t2);

            fe_sq(ref t2, t2);

            for (int i = 0; i < 49; ++i)
            {
                fe_sq(ref t2, t2);
            }

            fe_mul(ref t1, t2, t1);
            fe_sq(ref t1, t1);

            for (int i = 0; i < 4; ++i)
            {
                fe_sq(ref t1, t1);
            }

            fe_mul(ref ret, t1, t0);
        }

        /* Return 1 if f is in {1,3,5,....,q-2}
           Return 0 if f is in {0,2,4,...,q-1} */
        public static int fe_isnegative(int[] f)
        {
            byte[] s = new byte[32];
            fe_tobytes(ref s, f);

            /* Is it odd? */
            return s[0] & 1;
        }

        /* Is every element of fe_tobytes(f) zero? */
        public static bool fe_isnonzero(int[] f)
        {
            byte[] s = new byte[32];
            fe_tobytes(ref s, f);

            for (int i = 0; i < 32; i++)
            {
                if (s[i] != 0)
                {
                    return true;
                }
            }

            return false;
        }

        /* h = f * g 
           I have tried to simplify where possible, but am not sure how to
           easily simplify this one whilst being mindful of overflow */
        public static void fe_mul(ref int[] h, int[] f, int[] g)
        {
            int f0 = f[0];
            int f1 = f[1];
            int f2 = f[2];
            int f3 = f[3];
            int f4 = f[4];
            int f5 = f[5];
            int f6 = f[6];
            int f7 = f[7];
            int f8 = f[8];
            int f9 = f[9];
            int g0 = g[0];
            int g1 = g[1];
            int g2 = g[2];
            int g3 = g[3];
            int g4 = g[4];
            int g5 = g[5];
            int g6 = g[6];
            int g7 = g[7];
            int g8 = g[8];
            int g9 = g[9];
            int g1_19 = 19 * g1; /* 1.959375*2^29 */
            int g2_19 = 19 * g2; /* 1.959375*2^30; still ok */
            int g3_19 = 19 * g3;
            int g4_19 = 19 * g4;
            int g5_19 = 19 * g5;
            int g6_19 = 19 * g6;
            int g7_19 = 19 * g7;
            int g8_19 = 19 * g8;
            int g9_19 = 19 * g9;
            int f1_2 = 2 * f1;
            int f3_2 = 2 * f3;
            int f5_2 = 2 * f5;
            int f7_2 = 2 * f7;
            int f9_2 = 2 * f9;
            long f0g0    = f0   * (long) g0;
            long f0g1    = f0   * (long) g1;
            long f0g2    = f0   * (long) g2;
            long f0g3    = f0   * (long) g3;
            long f0g4    = f0   * (long) g4;
            long f0g5    = f0   * (long) g5;
            long f0g6    = f0   * (long) g6;
            long f0g7    = f0   * (long) g7;
            long f0g8    = f0   * (long) g8;
            long f0g9    = f0   * (long) g9;
            long f1g0    = f1   * (long) g0;
            long f1g1_2  = f1_2 * (long) g1;
            long f1g2    = f1   * (long) g2;
            long f1g3_2  = f1_2 * (long) g3;
            long f1g4    = f1   * (long) g4;
            long f1g5_2  = f1_2 * (long) g5;
            long f1g6    = f1   * (long) g6;
            long f1g7_2  = f1_2 * (long) g7;
            long f1g8    = f1   * (long) g8;
            long f1g9_38 = f1_2 * (long) g9_19;
            long f2g0    = f2   * (long) g0;
            long f2g1    = f2   * (long) g1;
            long f2g2    = f2   * (long) g2;
            long f2g3    = f2   * (long) g3;
            long f2g4    = f2   * (long) g4;
            long f2g5    = f2   * (long) g5;
            long f2g6    = f2   * (long) g6;
            long f2g7    = f2   * (long) g7;
            long f2g8_19 = f2   * (long) g8_19;
            long f2g9_19 = f2   * (long) g9_19;
            long f3g0    = f3   * (long) g0;
            long f3g1_2  = f3_2 * (long) g1;
            long f3g2    = f3   * (long) g2;
            long f3g3_2  = f3_2 * (long) g3;
            long f3g4    = f3   * (long) g4;
            long f3g5_2  = f3_2 * (long) g5;
            long f3g6    = f3   * (long) g6;
            long f3g7_38 = f3_2 * (long) g7_19;
            long f3g8_19 = f3   * (long) g8_19;
            long f3g9_38 = f3_2 * (long) g9_19;
            long f4g0    = f4   * (long) g0;
            long f4g1    = f4   * (long) g1;
            long f4g2    = f4   * (long) g2;
            long f4g3    = f4   * (long) g3;
            long f4g4    = f4   * (long) g4;
            long f4g5    = f4   * (long) g5;
            long f4g6_19 = f4   * (long) g6_19;
            long f4g7_19 = f4   * (long) g7_19;
            long f4g8_19 = f4   * (long) g8_19;
            long f4g9_19 = f4   * (long) g9_19;
            long f5g0    = f5   * (long) g0;
            long f5g1_2  = f5_2 * (long) g1;
            long f5g2    = f5   * (long) g2;
            long f5g3_2  = f5_2 * (long) g3;
            long f5g4    = f5   * (long) g4;
            long f5g5_38 = f5_2 * (long) g5_19;
            long f5g6_19 = f5   * (long) g6_19;
            long f5g7_38 = f5_2 * (long) g7_19;
            long f5g8_19 = f5   * (long) g8_19;
            long f5g9_38 = f5_2 * (long) g9_19;
            long f6g0    = f6   * (long) g0;
            long f6g1    = f6   * (long) g1;
            long f6g2    = f6   * (long) g2;
            long f6g3    = f6   * (long) g3;
            long f6g4_19 = f6   * (long) g4_19;
            long f6g5_19 = f6   * (long) g5_19;
            long f6g6_19 = f6   * (long) g6_19;
            long f6g7_19 = f6   * (long) g7_19;
            long f6g8_19 = f6   * (long) g8_19;
            long f6g9_19 = f6   * (long) g9_19;
            long f7g0    = f7   * (long) g0;
            long f7g1_2  = f7_2 * (long) g1;
            long f7g2    = f7   * (long) g2;
            long f7g3_38 = f7_2 * (long) g3_19;
            long f7g4_19 = f7   * (long) g4_19;
            long f7g5_38 = f7_2 * (long) g5_19;
            long f7g6_19 = f7   * (long) g6_19;
            long f7g7_38 = f7_2 * (long) g7_19;
            long f7g8_19 = f7   * (long) g8_19;
            long f7g9_38 = f7_2 * (long) g9_19;
            long f8g0    = f8   * (long) g0;
            long f8g1    = f8   * (long) g1;
            long f8g2_19 = f8   * (long) g2_19;
            long f8g3_19 = f8   * (long) g3_19;
            long f8g4_19 = f8   * (long) g4_19;
            long f8g5_19 = f8   * (long) g5_19;
            long f8g6_19 = f8   * (long) g6_19;
            long f8g7_19 = f8   * (long) g7_19;
            long f8g8_19 = f8   * (long) g8_19;
            long f8g9_19 = f8   * (long) g9_19;
            long f9g0    = f9   * (long) g0;
            long f9g1_38 = f9_2 * (long) g1_19;
            long f9g2_19 = f9   * (long) g2_19;
            long f9g3_38 = f9_2 * (long) g3_19;
            long f9g4_19 = f9   * (long) g4_19;
            long f9g5_38 = f9_2 * (long) g5_19;
            long f9g6_19 = f9   * (long) g6_19;
            long f9g7_38 = f9_2 * (long) g7_19;
            long f9g8_19 = f9   * (long) g8_19;
            long f9g9_38 = f9_2 * (long) g9_19;
            long h0 = f0g0+f1g9_38+f2g8_19+f3g7_38+f4g6_19+f5g5_38+f6g4_19+f7g3_38+f8g2_19+f9g1_38;
            long h1 = f0g1+f1g0   +f2g9_19+f3g8_19+f4g7_19+f5g6_19+f6g5_19+f7g4_19+f8g3_19+f9g2_19;
            long h2 = f0g2+f1g1_2 +f2g0   +f3g9_38+f4g8_19+f5g7_38+f6g6_19+f7g5_38+f8g4_19+f9g3_38;
            long h3 = f0g3+f1g2   +f2g1   +f3g0   +f4g9_19+f5g8_19+f6g7_19+f7g6_19+f8g5_19+f9g4_19;
            long h4 = f0g4+f1g3_2 +f2g2   +f3g1_2 +f4g0   +f5g9_38+f6g8_19+f7g7_38+f8g6_19+f9g5_38;
            long h5 = f0g5+f1g4   +f2g3   +f3g2   +f4g1   +f5g0   +f6g9_19+f7g8_19+f8g7_19+f9g6_19;
            long h6 = f0g6+f1g5_2 +f2g4   +f3g3_2 +f4g2   +f5g1_2 +f6g0   +f7g9_38+f8g8_19+f9g7_38;
            long h7 = f0g7+f1g6   +f2g5   +f3g4   +f4g3   +f5g2   +f6g1   +f7g0   +f8g9_19+f9g8_19;
            long h8 = f0g8+f1g7_2 +f2g6   +f3g5_2 +f4g4   +f5g3_2 +f6g2   +f7g1_2 +f8g0   +f9g9_38;
            long h9 = f0g9+f1g8   +f2g7   +f3g6   +f4g5   +f5g4   +f6g3   +f7g2   +f8g1   +f9g0   ;
            long carry0;
            long carry1;
            long carry2;
            long carry3;
            long carry4;
            long carry5;
            long carry6;
            long carry7;
            long carry8;
            long carry9;

            /*
            |h0| <= (1.65*1.65*2^52*(1+19+19+19+19)+1.65*1.65*2^50*(38+38+38+38+38))
              i.e. |h0| <= 1.4*2^60; narrower ranges for h2, h4, h6, h8
            |h1| <= (1.65*1.65*2^51*(1+1+19+19+19+19+19+19+19+19))
              i.e. |h1| <= 1.7*2^59; narrower ranges for h3, h5, h7, h9
            */

            carry0 = (h0 + (long) (1<<25)) >> 26; h1 += carry0; h0 -= carry0 << 26;
            carry4 = (h4 + (long) (1<<25)) >> 26; h5 += carry4; h4 -= carry4 << 26;
            /* |h0| <= 2^25 */
            /* |h4| <= 2^25 */
            /* |h1| <= 1.71*2^59 */
            /* |h5| <= 1.71*2^59 */

            carry1 = (h1 + (long) (1<<24)) >> 25; h2 += carry1; h1 -= carry1 << 25;
            carry5 = (h5 + (long) (1<<24)) >> 25; h6 += carry5; h5 -= carry5 << 25;
            /* |h1| <= 2^24; from now on fits into int32 */
            /* |h5| <= 2^24; from now on fits into int32 */
            /* |h2| <= 1.41*2^60 */
            /* |h6| <= 1.41*2^60 */

            carry2 = (h2 + (long) (1<<25)) >> 26; h3 += carry2; h2 -= carry2 << 26;
            carry6 = (h6 + (long) (1<<25)) >> 26; h7 += carry6; h6 -= carry6 << 26;
            /* |h2| <= 2^25; from now on fits into int32 unchanged */
            /* |h6| <= 2^25; from now on fits into int32 unchanged */
            /* |h3| <= 1.71*2^59 */
            /* |h7| <= 1.71*2^59 */

            carry3 = (h3 + (long) (1<<24)) >> 25; h4 += carry3; h3 -= carry3 << 25;
            carry7 = (h7 + (long) (1<<24)) >> 25; h8 += carry7; h7 -= carry7 << 25;
            /* |h3| <= 2^24; from now on fits into int32 unchanged */
            /* |h7| <= 2^24; from now on fits into int32 unchanged */
            /* |h4| <= 1.72*2^34 */
            /* |h8| <= 1.41*2^60 */

            carry4 = (h4 + (long) (1<<25)) >> 26; h5 += carry4; h4 -= carry4 << 26;
            carry8 = (h8 + (long) (1<<25)) >> 26; h9 += carry8; h8 -= carry8 << 26;
            /* |h4| <= 2^25; from now on fits into int32 unchanged */
            /* |h8| <= 2^25; from now on fits into int32 unchanged */
            /* |h5| <= 1.01*2^24 */
            /* |h9| <= 1.71*2^59 */

            carry9 = (h9 + (long) (1<<24)) >> 25; h0 += carry9 * 19; h9 -= carry9 << 25;
            /* |h9| <= 2^24; from now on fits into int32 unchanged */
            /* |h0| <= 1.1*2^39 */

            carry0 = (h0 + (long) (1<<25)) >> 26; h1 += carry0; h0 -= carry0 << 26;
            /* |h0| <= 2^25; from now on fits into int32 unchanged */
            /* |h1| <= 1.01*2^24 */

            h[0] = (int)h0;
            h[1] = (int)h1;
            h[2] = (int)h2;
            h[3] = (int)h3;
            h[4] = (int)h4;
            h[5] = (int)h5;
            h[6] = (int)h6;
            h[7] = (int)h7;
            h[8] = (int)h8;
            h[9] = (int)h9;
        }

        /* h = -f */
        public static void fe_neg(ref int[] h, int[] f)
        {
            for (int i = 0; i < 10; i++)
            {
                h[i] = -f[i];
            }
        }

        /* h = f * f
           Same disclaimer as fe_mul applies. */
        public static void fe_sq(ref int[] h, int[] f)
        {
            int f0 = f[0];
            int f1 = f[1];
            int f2 = f[2];
            int f3 = f[3];
            int f4 = f[4];
            int f5 = f[5];
            int f6 = f[6];
            int f7 = f[7];
            int f8 = f[8];
            int f9 = f[9];
            int f0_2 = 2 * f0;
            int f1_2 = 2 * f1;
            int f2_2 = 2 * f2;
            int f3_2 = 2 * f3;
            int f4_2 = 2 * f4;
            int f5_2 = 2 * f5;
            int f6_2 = 2 * f6;
            int f7_2 = 2 * f7;
            int f5_38 = 38 * f5; /* 1.959375*2^30 */
            int f6_19 = 19 * f6; /* 1.959375*2^30 */
            int f7_38 = 38 * f7; /* 1.959375*2^30 */
            int f8_19 = 19 * f8; /* 1.959375*2^30 */
            int f9_38 = 38 * f9; /* 1.959375*2^30 */
            long f0f0    = f0   * (long) f0;
            long f0f1_2  = f0_2 * (long) f1;
            long f0f2_2  = f0_2 * (long) f2;
            long f0f3_2  = f0_2 * (long) f3;
            long f0f4_2  = f0_2 * (long) f4;
            long f0f5_2  = f0_2 * (long) f5;
            long f0f6_2  = f0_2 * (long) f6;
            long f0f7_2  = f0_2 * (long) f7;
            long f0f8_2  = f0_2 * (long) f8;
            long f0f9_2  = f0_2 * (long) f9;
            long f1f1_2  = f1_2 * (long) f1;
            long f1f2_2  = f1_2 * (long) f2;
            long f1f3_4  = f1_2 * (long) f3_2;
            long f1f4_2  = f1_2 * (long) f4;
            long f1f5_4  = f1_2 * (long) f5_2;
            long f1f6_2  = f1_2 * (long) f6;
            long f1f7_4  = f1_2 * (long) f7_2;
            long f1f8_2  = f1_2 * (long) f8;
            long f1f9_76 = f1_2 * (long) f9_38;
            long f2f2    = f2   * (long) f2;
            long f2f3_2  = f2_2 * (long) f3;
            long f2f4_2  = f2_2 * (long) f4;
            long f2f5_2  = f2_2 * (long) f5;
            long f2f6_2  = f2_2 * (long) f6;
            long f2f7_2  = f2_2 * (long) f7;
            long f2f8_38 = f2_2 * (long) f8_19;
            long f2f9_38 = f2   * (long) f9_38;
            long f3f3_2  = f3_2 * (long) f3;
            long f3f4_2  = f3_2 * (long) f4;
            long f3f5_4  = f3_2 * (long) f5_2;
            long f3f6_2  = f3_2 * (long) f6;
            long f3f7_76 = f3_2 * (long) f7_38;
            long f3f8_38 = f3_2 * (long) f8_19;
            long f3f9_76 = f3_2 * (long) f9_38;
            long f4f4    = f4   * (long) f4;
            long f4f5_2  = f4_2 * (long) f5;
            long f4f6_38 = f4_2 * (long) f6_19;
            long f4f7_38 = f4   * (long) f7_38;
            long f4f8_38 = f4_2 * (long) f8_19;
            long f4f9_38 = f4   * (long) f9_38;
            long f5f5_38 = f5   * (long) f5_38;
            long f5f6_38 = f5_2 * (long) f6_19;
            long f5f7_76 = f5_2 * (long) f7_38;
            long f5f8_38 = f5_2 * (long) f8_19;
            long f5f9_76 = f5_2 * (long) f9_38;
            long f6f6_19 = f6   * (long) f6_19;
            long f6f7_38 = f6   * (long) f7_38;
            long f6f8_38 = f6_2 * (long) f8_19;
            long f6f9_38 = f6   * (long) f9_38;
            long f7f7_38 = f7   * (long) f7_38;
            long f7f8_38 = f7_2 * (long) f8_19;
            long f7f9_76 = f7_2 * (long) f9_38;
            long f8f8_19 = f8   * (long) f8_19;
            long f8f9_38 = f8   * (long) f9_38;
            long f9f9_38 = f9   * (long) f9_38;
            long h0 = f0f0  +f1f9_76+f2f8_38+f3f7_76+f4f6_38+f5f5_38;
            long h1 = f0f1_2+f2f9_38+f3f8_38+f4f7_38+f5f6_38;
            long h2 = f0f2_2+f1f1_2 +f3f9_76+f4f8_38+f5f7_76+f6f6_19;
            long h3 = f0f3_2+f1f2_2 +f4f9_38+f5f8_38+f6f7_38;
            long h4 = f0f4_2+f1f3_4 +f2f2   +f5f9_76+f6f8_38+f7f7_38;
            long h5 = f0f5_2+f1f4_2 +f2f3_2 +f6f9_38+f7f8_38;
            long h6 = f0f6_2+f1f5_4 +f2f4_2 +f3f3_2 +f7f9_76+f8f8_19;
            long h7 = f0f7_2+f1f6_2 +f2f5_2 +f3f4_2 +f8f9_38;
            long h8 = f0f8_2+f1f7_4 +f2f6_2 +f3f5_4 +f4f4   +f9f9_38;
            long h9 = f0f9_2+f1f8_2 +f2f7_2 +f3f6_2 +f4f5_2;
            long carry0;
            long carry1;
            long carry2;
            long carry3;
            long carry4;
            long carry5;
            long carry6;
            long carry7;
            long carry8;
            long carry9;

            carry0 = (h0 + (long) (1<<25)) >> 26; h1 += carry0; h0 -= carry0 << 26;
            carry4 = (h4 + (long) (1<<25)) >> 26; h5 += carry4; h4 -= carry4 << 26;

            carry1 = (h1 + (long) (1<<24)) >> 25; h2 += carry1; h1 -= carry1 << 25;
            carry5 = (h5 + (long) (1<<24)) >> 25; h6 += carry5; h5 -= carry5 << 25;

            carry2 = (h2 + (long) (1<<25)) >> 26; h3 += carry2; h2 -= carry2 << 26;
            carry6 = (h6 + (long) (1<<25)) >> 26; h7 += carry6; h6 -= carry6 << 26;

            carry3 = (h3 + (long) (1<<24)) >> 25; h4 += carry3; h3 -= carry3 << 25;
            carry7 = (h7 + (long) (1<<24)) >> 25; h8 += carry7; h7 -= carry7 << 25;

            carry4 = (h4 + (long) (1<<25)) >> 26; h5 += carry4; h4 -= carry4 << 26;
            carry8 = (h8 + (long) (1<<25)) >> 26; h9 += carry8; h8 -= carry8 << 26;

            carry9 = (h9 + (long) (1<<24)) >> 25; h0 += carry9 * 19; h9 -= carry9 << 25;

            carry0 = (h0 + (long) (1<<25)) >> 26; h1 += carry0; h0 -= carry0 << 26;

            h[0] = (int)h0;
            h[1] = (int)h1;
            h[2] = (int)h2;
            h[3] = (int)h3;
            h[4] = (int)h4;
            h[5] = (int)h5;
            h[6] = (int)h6;
            h[7] = (int)h7;
            h[8] = (int)h8;
            h[9] = (int)h9;
        }

        /* h = 2 * f * f */
        public static void fe_sq2(ref int[] h, int[] f)
        {
            int f0 = f[0];
            int f1 = f[1];
            int f2 = f[2];
            int f3 = f[3];
            int f4 = f[4];
            int f5 = f[5];
            int f6 = f[6];
            int f7 = f[7];
            int f8 = f[8];
            int f9 = f[9];
            int f0_2 = 2 * f0;
            int f1_2 = 2 * f1;
            int f2_2 = 2 * f2;
            int f3_2 = 2 * f3;
            int f4_2 = 2 * f4;
            int f5_2 = 2 * f5;
            int f6_2 = 2 * f6;
            int f7_2 = 2 * f7;
            int f5_38 = 38 * f5; /* 1.959375*2^30 */
            int f6_19 = 19 * f6; /* 1.959375*2^30 */
            int f7_38 = 38 * f7; /* 1.959375*2^30 */
            int f8_19 = 19 * f8; /* 1.959375*2^30 */
            int f9_38 = 38 * f9; /* 1.959375*2^30 */
            long f0f0    = f0   * (long) f0;
            long f0f1_2  = f0_2 * (long) f1;
            long f0f2_2  = f0_2 * (long) f2;
            long f0f3_2  = f0_2 * (long) f3;
            long f0f4_2  = f0_2 * (long) f4;
            long f0f5_2  = f0_2 * (long) f5;
            long f0f6_2  = f0_2 * (long) f6;
            long f0f7_2  = f0_2 * (long) f7;
            long f0f8_2  = f0_2 * (long) f8;
            long f0f9_2  = f0_2 * (long) f9;
            long f1f1_2  = f1_2 * (long) f1;
            long f1f2_2  = f1_2 * (long) f2;
            long f1f3_4  = f1_2 * (long) f3_2;
            long f1f4_2  = f1_2 * (long) f4;
            long f1f5_4  = f1_2 * (long) f5_2;
            long f1f6_2  = f1_2 * (long) f6;
            long f1f7_4  = f1_2 * (long) f7_2;
            long f1f8_2  = f1_2 * (long) f8;
            long f1f9_76 = f1_2 * (long) f9_38;
            long f2f2    = f2   * (long) f2;
            long f2f3_2  = f2_2 * (long) f3;
            long f2f4_2  = f2_2 * (long) f4;
            long f2f5_2  = f2_2 * (long) f5;
            long f2f6_2  = f2_2 * (long) f6;
            long f2f7_2  = f2_2 * (long) f7;
            long f2f8_38 = f2_2 * (long) f8_19;
            long f2f9_38 = f2   * (long) f9_38;
            long f3f3_2  = f3_2 * (long) f3;
            long f3f4_2  = f3_2 * (long) f4;
            long f3f5_4  = f3_2 * (long) f5_2;
            long f3f6_2  = f3_2 * (long) f6;
            long f3f7_76 = f3_2 * (long) f7_38;
            long f3f8_38 = f3_2 * (long) f8_19;
            long f3f9_76 = f3_2 * (long) f9_38;
            long f4f4    = f4   * (long) f4;
            long f4f5_2  = f4_2 * (long) f5;
            long f4f6_38 = f4_2 * (long) f6_19;
            long f4f7_38 = f4   * (long) f7_38;
            long f4f8_38 = f4_2 * (long) f8_19;
            long f4f9_38 = f4   * (long) f9_38;
            long f5f5_38 = f5   * (long) f5_38;
            long f5f6_38 = f5_2 * (long) f6_19;
            long f5f7_76 = f5_2 * (long) f7_38;
            long f5f8_38 = f5_2 * (long) f8_19;
            long f5f9_76 = f5_2 * (long) f9_38;
            long f6f6_19 = f6   * (long) f6_19;
            long f6f7_38 = f6   * (long) f7_38;
            long f6f8_38 = f6_2 * (long) f8_19;
            long f6f9_38 = f6   * (long) f9_38;
            long f7f7_38 = f7   * (long) f7_38;
            long f7f8_38 = f7_2 * (long) f8_19;
            long f7f9_76 = f7_2 * (long) f9_38;
            long f8f8_19 = f8   * (long) f8_19;
            long f8f9_38 = f8   * (long) f9_38;
            long f9f9_38 = f9   * (long) f9_38;
            long h0 = f0f0  +f1f9_76+f2f8_38+f3f7_76+f4f6_38+f5f5_38;
            long h1 = f0f1_2+f2f9_38+f3f8_38+f4f7_38+f5f6_38;
            long h2 = f0f2_2+f1f1_2 +f3f9_76+f4f8_38+f5f7_76+f6f6_19;
            long h3 = f0f3_2+f1f2_2 +f4f9_38+f5f8_38+f6f7_38;
            long h4 = f0f4_2+f1f3_4 +f2f2   +f5f9_76+f6f8_38+f7f7_38;
            long h5 = f0f5_2+f1f4_2 +f2f3_2 +f6f9_38+f7f8_38;
            long h6 = f0f6_2+f1f5_4 +f2f4_2 +f3f3_2 +f7f9_76+f8f8_19;
            long h7 = f0f7_2+f1f6_2 +f2f5_2 +f3f4_2 +f8f9_38;
            long h8 = f0f8_2+f1f7_4 +f2f6_2 +f3f5_4 +f4f4   +f9f9_38;
            long h9 = f0f9_2+f1f8_2 +f2f7_2 +f3f6_2 +f4f5_2;
            long carry0;
            long carry1;
            long carry2;
            long carry3;
            long carry4;
            long carry5;
            long carry6;
            long carry7;
            long carry8;
            long carry9;

            h0 += h0;
            h1 += h1;
            h2 += h2;
            h3 += h3;
            h4 += h4;
            h5 += h5;
            h6 += h6;
            h7 += h7;
            h8 += h8;
            h9 += h9;

            carry0 = (h0 + (long) (1<<25)) >> 26; h1 += carry0; h0 -= carry0 << 26;
            carry4 = (h4 + (long) (1<<25)) >> 26; h5 += carry4; h4 -= carry4 << 26;

            carry1 = (h1 + (long) (1<<24)) >> 25; h2 += carry1; h1 -= carry1 << 25;
            carry5 = (h5 + (long) (1<<24)) >> 25; h6 += carry5; h5 -= carry5 << 25;

            carry2 = (h2 + (long) (1<<25)) >> 26; h3 += carry2; h2 -= carry2 << 26;
            carry6 = (h6 + (long) (1<<25)) >> 26; h7 += carry6; h6 -= carry6 << 26;

            carry3 = (h3 + (long) (1<<24)) >> 25; h4 += carry3; h3 -= carry3 << 25;
            carry7 = (h7 + (long) (1<<24)) >> 25; h8 += carry7; h7 -= carry7 << 25;

            carry4 = (h4 + (long) (1<<25)) >> 26; h5 += carry4; h4 -= carry4 << 26;
            carry8 = (h8 + (long) (1<<25)) >> 26; h9 += carry8; h8 -= carry8 << 26;

            carry9 = (h9 + (long) (1<<24)) >> 25; h0 += carry9 * 19; h9 -= carry9 << 25;

            carry0 = (h0 + (long) (1<<25)) >> 26; h1 += carry0; h0 -= carry0 << 26;

            h[0] = (int)h0;
            h[1] = (int)h1;
            h[2] = (int)h2;
            h[3] = (int)h3;
            h[4] = (int)h4;
            h[5] = (int)h5;
            h[6] = (int)h6;
            h[7] = (int)h7;
            h[8] = (int)h8;
            h[9] = (int)h9;
        }

        /* h = f - g */
        public static void fe_sub(ref int[] h, int[] f, int[] g)
        {
            for (int i = 0; i < 10; i++)
            {
                h[i] = f[i] - g[i];
            }
        }

        public static void fe_tobytes(ref byte[] s, int[] h)
        {
            int h0 = h[0];
            int h1 = h[1];
            int h2 = h[2];
            int h3 = h[3];
            int h4 = h[4];
            int h5 = h[5];
            int h6 = h[6];
            int h7 = h[7];
            int h8 = h[8];
            int h9 = h[9];
            int q;
            int carry0;
            int carry1;
            int carry2;
            int carry3;
            int carry4;
            int carry5;
            int carry6;
            int carry7;
            int carry8;
            int carry9;

            q = (19 * h9 + (((int) 1) << 24)) >> 25;
            q = (h0 + q) >> 26;
            q = (h1 + q) >> 25;
            q = (h2 + q) >> 26;
            q = (h3 + q) >> 25;
            q = (h4 + q) >> 26;
            q = (h5 + q) >> 25;
            q = (h6 + q) >> 26;
            q = (h7 + q) >> 25;
            q = (h8 + q) >> 26;
            q = (h9 + q) >> 25;

            /* Goal: Output h-(2^255-19)q, which is between 0 and 2^255-20. */
            h0 += 19 * q;
            /* Goal: Output h-2^255 q, which is between 0 and 2^255-20. */

            carry0 = h0 >> 26; h1 += carry0; h0 -= carry0 << 26;
            carry1 = h1 >> 25; h2 += carry1; h1 -= carry1 << 25;
            carry2 = h2 >> 26; h3 += carry2; h2 -= carry2 << 26;
            carry3 = h3 >> 25; h4 += carry3; h3 -= carry3 << 25;
            carry4 = h4 >> 26; h5 += carry4; h4 -= carry4 << 26;
            carry5 = h5 >> 25; h6 += carry5; h5 -= carry5 << 25;
            carry6 = h6 >> 26; h7 += carry6; h6 -= carry6 << 26;
            carry7 = h7 >> 25; h8 += carry7; h7 -= carry7 << 25;
            carry8 = h8 >> 26; h9 += carry8; h8 -= carry8 << 26;
            carry9 = h9 >> 25;               h9 -= carry9 << 25;
                              /* h10 = carry9 */

            /*
            Goal: Output h0+...+2^255 h10-2^255 q, which is between 0 and 2^255-20.
            Have h0+...+2^230 h9 between 0 and 2^255-1;
            evidently 2^255 h10-2^255 q = 0.
            Goal: Output h0+...+2^230 h9.
            */

            s[0] = (byte)(h0 >> 0);
            s[1] = (byte)(h0 >> 8);
            s[2] = (byte)(h0 >> 16);
            s[3] = (byte)((h0 >> 24) | (h1 << 2));
            s[4] = (byte)(h1 >> 6);
            s[5] = (byte)(h1 >> 14);
            s[6] = (byte)((h1 >> 22) | (h2 << 3));
            s[7] = (byte)(h2 >> 5);
            s[8] = (byte)(h2 >> 13);
            s[9] = (byte)((h2 >> 21) | (h3 << 5));
            s[10] = (byte)(h3 >> 3);
            s[11] = (byte)(h3 >> 11);
            s[12] = (byte)((h3 >> 19) | (h4 << 6));
            s[13] = (byte)(h4 >> 2);
            s[14] = (byte)(h4 >> 10);
            s[15] = (byte)(h4 >> 18);
            s[16] = (byte)(h5 >> 0);
            s[17] = (byte)(h5 >> 8);
            s[18] = (byte)(h5 >> 16);
            s[19] = (byte)((h5 >> 24) | (h6 << 1));
            s[20] = (byte)(h6 >> 7);
            s[21] = (byte)(h6 >> 15);
            s[22] = (byte)((h6 >> 23) | (h7 << 3));
            s[23] = (byte)(h7 >> 5);
            s[24] = (byte)(h7 >> 13);
            s[25] = (byte)((h7 >> 21) | (h8 << 4));
            s[26] = (byte)(h8 >> 4);
            s[27] = (byte)(h8 >> 12);
            s[28] = (byte)((h8 >> 20) | (h9 << 6));
            s[29] = (byte)(h9 >> 2);
            s[30] = (byte)(h9 >> 10);
            s[31] = (byte)(h9 >> 18);
        }

        public static void ge_add(ge_p1p1 r, ge_p3 p, ge_cached q)
        {
            int[] t0 = new int[10];

            fe_add(ref r.X, p.Y, p.X);
            fe_sub(ref r.Y, p.Y, p.X);
            fe_mul(ref r.Z, r.X, q.YplusX);
            fe_mul(ref r.Y, r.Y, q.YminusX);
            fe_mul(ref r.T, q.T2d, p.T);
            fe_mul(ref r.X, p.Z, q.Z);
            fe_add(ref t0, r.X, r.X);
            fe_sub(ref r.X, r.Z, r.Y);
            fe_add(ref r.Y, r.Z, r.Y);
            fe_add(ref r.Z, t0, r.T);
            fe_sub(ref r.T, t0, r.T);
        }

        public static void slide(ref sbyte[] r, byte[] a)
        {
            int i;
            int b;
            int k;

            for (i = 0; i < 256; ++i)
            {
                r[i] = (sbyte)(1 & (a[i >> 3] >> (i & 7)));
            }

            for (i = 0; i < 256; ++i)
            {
                if (r[i] != 0)
                {
                    for (b = 1; b <= 6 && i + b < 256; ++b)
                    {
                        if (r[i + b] != 0)
                        {
                            if (r[i] + (r[i + b] << b) <= 15)
                            {
                                r[i] += (sbyte)(r[i + b] << b);
                                r[i + b] = 0;
                            }
                            else if (r[i] - (r[i + b] << b) >= -15)
                            {
                                r[i] -= (sbyte)(r[i + b] << b);
                                for (k = i + b; k < 256; ++k)
                                {
                                    if (r[k] == 0)
                                    {
                                        r[k] = 1;
                                        break;
                                    }
                                    r[k] = 0;
                                }
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }
            }
        }

        public static void ge_dsm_precomp(ref ge_dsmp r, ge_p3 s)
        {
            ge_p1p1 t = new ge_p1p1();
            ge_p3 s2 = new ge_p3();
            ge_p3 u = new ge_p3();

            ge_p3_to_cached(r[0], s);
            ge_p3_dbl(t, s);
            ge_p1p1_to_p3(s2, t);

            for (int i = 0; i < 7; i++)
            {
                ge_add(t, s2, r[i]);
                ge_p1p1_to_p3(u, t);
                ge_p3_to_cached(r[i+1], u);
            }
        }

        public static void ge_double_scalarmult_base_vartime(ge_p2 r, byte[] a, ge_p3 A, byte[] b)
        {
            sbyte[] aslide = new sbyte[256];
            sbyte[] bslide = new sbyte[256];

            ge_dsmp Ai = new ge_dsmp(); ; /* A, 3A, 5A, 7A, 9A, 11A, 13A, 15A */

            ge_p1p1 t = new ge_p1p1();
            ge_p3 u = new ge_p3();

            int i;

            slide(ref aslide, a);
            slide(ref bslide, b);
            ge_dsm_precomp(ref Ai, A);

            ge_p2_0(r);

            for (i = 255; i >= 0; --i)
            {
                if (aslide[i] != 0 || bslide[i] != 0)
                {
                    break;
                }
            }

            for (; i >= 0; --i)
            {
                ge_p2_dbl(t, r);

                if (aslide[i] > 0)
                {
                    ge_p1p1_to_p3(u, t);
                    ge_add(t, u, Ai[aslide[i]/2]);
                }
                else if (aslide[i] < 0)
                {
                    ge_p1p1_to_p3(u, t);
                    ge_sub(t, u, Ai[(-aslide[i])/2]);
                }

                if (bslide[i] > 0)
                {
                    ge_p1p1_to_p3(u, t);
                    ge_madd(t, u, GE_BI[bslide[i]/2]);
                }
                else if (bslide[i] < 0)
                {
                    ge_p1p1_to_p3(u, t);
                    ge_msub(t, u, GE_BI[(-bslide[i])/2]);
                }

                ge_p1p1_to_p2(r, t);
            }
        }

        public static void ge_double_scalarmult_base_vartime_p3(ge_p3 r3, byte[] a, ge_p3 A, byte[] b)
        {
            sbyte[] aslide = new sbyte[256];
            sbyte[] bslide = new sbyte[256];

            ge_dsmp Ai = new ge_dsmp(); /* A, 3A, 5A, 7A, 9A, 11A, 13A, 15A */

            ge_p1p1 t = new ge_p1p1();
            ge_p3 u = new ge_p3();
            ge_p2 r = new ge_p2();

            int i;

            slide(ref aslide, a);
            slide(ref bslide, b);
            ge_dsm_precomp(ref Ai, A);

            ge_p2_0(r);

            for (i = 255; i >= 0; --i)
            {
                if (aslide[i] != 0 || bslide[i] != 0)
                {
                    break;
                }
            }

            for (; i >= 0; --i)
            {
                ge_p2_dbl(t, r);

                if (aslide[i] > 0)
                {
                    ge_p1p1_to_p3(u, t);
                    ge_add(t, u, Ai[aslide[i]/2]);
                }
                else if (aslide[i] < 0)
                {
                    ge_p1p1_to_p3(u, t);
                    ge_sub(t, u, Ai[(-aslide[i])/2]);
                }

                if (bslide[i] > 0)
                {
                    ge_p1p1_to_p3(u, t);
                    ge_madd(t, u, GE_BI[bslide[i]/2]);
                }
                else if (bslide[i] < 0)
                {
                    ge_p1p1_to_p3(u, t);
                    ge_msub(t, u, GE_BI[(-bslide[i])/2]);
                }

                if (i == 0)
                {
                    ge_p1p1_to_p3(r3, t);
                }
                else
                {
                    ge_p1p1_to_p2(r, t);
                }
            }
        }

        public static int ge_frombytes_vartime(ge_p3 h, byte[] s)
        {
            int[] u = new int[10];
            int[] v = new int[10];
            int[] vxx = new int[10];
            int[] check = new int[10];

            /* From fe_frombytes.c */

            long h0 = (long)(load_4(s));
            long h1 = (long)(load_3(s, 4) << 6);
            long h2 = (long)(load_3(s, 7) << 5);
            long h3 = (long)(load_3(s, 10) << 3);
            long h4 = (long)(load_3(s, 13) << 2);
            long h5 = (long)(load_4(s, 16));
            long h6 = (long)(load_3(s, 20) << 7);
            long h7 = (long)(load_3(s, 23) << 5);
            long h8 = (long)(load_3(s, 26) << 4);
            long h9 = (long)((load_3(s, 29) & 8388607) << 2);
            long carry0;
            long carry1;
            long carry2;
            long carry3;
            long carry4;
            long carry5;
            long carry6;
            long carry7;
            long carry8;
            long carry9;

            /* Validate the number to be canonical */
            if (h9 == 33554428 && h8 == 268435440 && h7 == 536870880 && h6 == 2147483520 &&
                h5 == 4294967295 && h4 == 67108860 && h3 == 134217720 && h2 == 536870880 &&
                h1 == 1073741760 && h0 >= 4294967277) {
                return -1;
            }

            carry9 = (h9 + (long) (1<<24)) >> 25; h0 += carry9 * 19; h9 -= carry9 << 25;
            carry1 = (h1 + (long) (1<<24)) >> 25; h2 += carry1; h1 -= carry1 << 25;
            carry3 = (h3 + (long) (1<<24)) >> 25; h4 += carry3; h3 -= carry3 << 25;
            carry5 = (h5 + (long) (1<<24)) >> 25; h6 += carry5; h5 -= carry5 << 25;
            carry7 = (h7 + (long) (1<<24)) >> 25; h8 += carry7; h7 -= carry7 << 25;

            carry0 = (h0 + (long) (1<<25)) >> 26; h1 += carry0; h0 -= carry0 << 26;
            carry2 = (h2 + (long) (1<<25)) >> 26; h3 += carry2; h2 -= carry2 << 26;
            carry4 = (h4 + (long) (1<<25)) >> 26; h5 += carry4; h4 -= carry4 << 26;
            carry6 = (h6 + (long) (1<<25)) >> 26; h7 += carry6; h6 -= carry6 << 26;
            carry8 = (h8 + (long) (1<<25)) >> 26; h9 += carry8; h8 -= carry8 << 26;

            h.Y[0] = (int)h0;
            h.Y[1] = (int)h1;
            h.Y[2] = (int)h2;
            h.Y[3] = (int)h3;
            h.Y[4] = (int)h4;
            h.Y[5] = (int)h5;
            h.Y[6] = (int)h6;
            h.Y[7] = (int)h7;
            h.Y[8] = (int)h8;
            h.Y[9] = (int)h9;

            /* End fe_frombytes.c */

            fe_1(ref h.Z);
            fe_sq(ref u, h.Y);
            fe_mul(ref v, u, FE_D);
            fe_sub(ref u, u, h.Z);       /* u = y^2-1 */
            fe_add(ref v, v, h.Z);       /* v = dy^2+1 */

            fe_divpowm1(ref h.X, u, v); /* x = uv^3(uv^7)^((q-5)/8) */

            fe_sq(ref vxx, h.X);
            fe_mul(ref vxx, vxx, v);
            fe_sub(ref check, vxx, u);    /* vx^2-u */

            if (fe_isnonzero(check))
            {
                fe_add(ref check, vxx, u);  /* vx^2+u */

                if (fe_isnonzero(check))
                {
                    return -1;
                }

                fe_mul(ref h.X, h.X, FE_SQRTM1);
            }

            if (fe_isnegative(h.X) != (s[31] >> 7))
            {
                /* If x = 0, the sign must be positive */
                if (!fe_isnonzero(h.X))
                {
                    return -1;
                }

                fe_neg(ref h.X, h.X);
            }

            fe_mul(ref h.T, h.X, h.Y);

            return 0;
        }

        /* r = p + q */
        public static void ge_madd(ge_p1p1 r, ge_p3 p, ge_precomp q)
        {
            int[] t0 = new int[10];

            fe_add(ref r.X, p.Y, p.X);
            fe_sub(ref r.Y, p.Y, p.X);
            fe_mul(ref r.Z, r.X, q.yplusx);
            fe_mul(ref r.Y, r.Y, q.yminusx);
            fe_mul(ref r.T, q.xy2d, p.T);
            fe_add(ref t0, p.Z, p.Z);
            fe_sub(ref r.X, r.Z, r.Y);
            fe_add(ref r.Y, r.Z, r.Y);
            fe_add(ref r.Z, t0, r.T);
            fe_sub(ref r.T, t0, r.T);
        }

        /* r = p - q */
        public static void ge_msub(ge_p1p1 r, ge_p3 p, ge_precomp q)
        {
            int[] t0 = new int[10];

            fe_add(ref r.X, p.Y, p.X);
            fe_sub(ref r.Y, p.Y, p.X);
            fe_mul(ref r.Z, r.X, q.yminusx);
            fe_mul(ref r.Y, r.Y, q.yplusx);
            fe_mul(ref r.T, q.xy2d, p.T);
            fe_add(ref t0, p.Z, p.Z);
            fe_sub(ref r.X, r.Z, r.Y);
            fe_add(ref r.Y, r.Z, r.Y);
            fe_sub(ref r.Z, t0, r.T);
            fe_add(ref r.T, t0, r.T);
        }

        /* r = p */
        public static void ge_p1p1_to_p2(ge_p2 r, ge_p1p1 p)
        {
            fe_mul(ref r.X, p.X, p.T);
            fe_mul(ref r.Y, p.Y, p.Z);
            fe_mul(ref r.Z, p.Z, p.T);
        }

        /* r = p */
        public static void ge_p1p1_to_p3(ge_p3 r, ge_p1p1 p)
        {
            fe_mul(ref r.X, p.X, p.T);
            fe_mul(ref r.Y, p.Y, p.Z);
            fe_mul(ref r.Z, p.Z, p.T);
            fe_mul(ref r.T, p.X, p.Y);
        }

        public static void ge_p2_0(ge_p2 h)
        {
            fe_0(ref h.X);
            fe_1(ref h.Y);
            fe_1(ref h.Z);
        }

        /* r = 2 * p */
        public static void ge_p2_dbl(ge_p1p1 r, ge_p2 p)
        {
            int[] t0 = new int[10];

            fe_sq(ref r.X, p.X);
            fe_sq(ref r.Z, p.Y);
            fe_sq2(ref r.T, p.Z);
            fe_add(ref r.Y, p.X, p.Y);
            fe_sq(ref t0, r.Y);
            fe_add(ref r.Y, r.Z, r.X);
            fe_sub(ref r.Z, r.Z, r.X);
            fe_sub(ref r.X, t0, r.Y);
            fe_sub(ref r.T, r.T, r.Z);
        }

        public static void ge_p3_0(ge_p3 h)
        {
            fe_0(ref h.X);
            fe_1(ref h.Y);
            fe_1(ref h.Z);
            fe_0(ref h.T);
        }

        /* r = 2 * p */
        public static void ge_p3_dbl(ge_p1p1 r, ge_p3 p)
        {
            ge_p2 q = new ge_p2();
            ge_p3_to_p2(q, p);
            ge_p2_dbl(r, q);
        }

        /* r = p */
        public static void ge_p3_to_cached(ge_cached r, ge_p3 p)
        {
            fe_add(ref r.YplusX, p.Y, p.X);
            fe_sub(ref r.YminusX, p.Y, p.X);
            fe_copy(ref r.Z, p.Z);
            fe_mul(ref r.T2d, p.T, FE_D2);
        }

        /* r = p */
        public static void ge_p3_to_p2(ge_p2 r, ge_p3 p)
        {
            fe_copy(ref r.X, p.X);
            fe_copy(ref r.Y, p.Y);
            fe_copy(ref r.Z, p.Z);
        }

        public static void ge_p3_tobytes(ref byte[] s, ge_p3 h)
        {
            int[] recip = new int[10];
            int[] x = new int[10];
            int[] y = new int[10];

            fe_invert(ref recip, h.Z);
            fe_mul(ref x, h.X, recip);
            fe_mul(ref y, h.Y, recip);
            fe_tobytes(ref s, y);
            s[31] ^= (byte)(fe_isnegative(x) << 7);
        }

        public static void ge_precomp_0(ge_precomp h)
        {
            fe_1(ref h.yplusx);
            fe_1(ref h.yminusx);
            fe_0(ref h.xy2d);
        }

        public static byte equal(sbyte b, sbyte c)
        {
            return (byte)(b == c ? 1 : 0);
        }

        public static byte negative(sbyte b)
        {
            return (byte)(b < 0 ? 1 : 0);
        }

        public static void ge_precomp_cmov(ge_precomp t, ge_precomp u, byte b)
        {
            fe_cmov(ref t.yplusx, u.yplusx, b);
            fe_cmov(ref t.yminusx, u.yminusx, b);
            fe_cmov(ref t.xy2d, u.xy2d, b);
        }

        public static void select(ge_precomp t, int pos, sbyte b)
        {
            ge_precomp minust = new ge_precomp();

            byte bnegative = negative(b);
            byte babs = (byte)(b - (((-bnegative) & b) << 1));

            ge_precomp_0(t);

            for (int i = 0; i < 8; i++)
            {
                ge_precomp_cmov(t, GE_BASE[pos,i], equal((sbyte)babs, (sbyte)(i+1)));
            }

            fe_copy(ref minust.yplusx, t.yminusx);
            fe_copy(ref minust.yminusx, t.yplusx);

            fe_neg(ref minust.xy2d, t.xy2d);

            ge_precomp_cmov(t, minust, bnegative);
        }
        
        /* h = a * B
           where a = a[0]+256*a[1]+...+256^31 a[31]
           B is the Ed25519 base point (x,4/5) with x positive. */
        public static void ge_scalarmult_base(ge_p3 h, byte[] a)
        {
            sbyte[] e = new sbyte[64];
            sbyte carry;

            ge_p1p1 r = new ge_p1p1();
            ge_p2 s = new ge_p2();
            ge_precomp t = new ge_precomp();

            for (int i = 0; i < 32; ++i)
            {
                e[2 * i + 0] = (sbyte)((a[i] >> 0) & 15);
                e[2 * i + 1] = (sbyte)((a[i] >> 4) & 15);
            }
            /* each e[i] is between 0 and 15 */
            /* e[63] is between 0 and 7 */

            carry = 0;

            for (int i = 0; i < 63; ++i)
            {
                e[i] += carry;
                carry = (sbyte)(e[i] + 8);
                carry >>= 4;
                e[i] -= (sbyte)(carry << 4);
            }

            e[63] += carry;
            /* each e[i] is between -8 and 8 */

            ge_p3_0(h);

            for (int i = 1; i < 64; i += 2)
            {
                select(t, i / 2, e[i]);
                ge_madd(r, h, t);
                ge_p1p1_to_p3(h, r);
            }

            ge_p3_dbl(r, h);
            ge_p1p1_to_p2(s, r);

            ge_p2_dbl(r, s);
            ge_p1p1_to_p2(s, r);

            ge_p2_dbl(r, s);
            ge_p1p1_to_p2(s, r);

            ge_p2_dbl(r, s);
            ge_p1p1_to_p3(h, r);

            for (int i = 0; i < 64; i += 2)
            {
                select(t, i / 2, e[i]);
                ge_madd(r, h, t);
                ge_p1p1_to_p3(h, r);
            }
        }

        /* r = p - q */
        public static void ge_sub(ge_p1p1 r, ge_p3 p, ge_cached q)
        {
            int[] t0 = new int[10];
            
            fe_add(ref r.X, p.Y, p.X);
            fe_sub(ref r.Y, p.Y, p.X);
            fe_mul(ref r.Z, r.X, q.YminusX);
            fe_mul(ref r.Y, r.Y, q.YplusX);
            fe_mul(ref r.T, q.T2d, p.T);
            fe_mul(ref r.X, p.Z, q.Z);
            fe_add(ref t0, r.X, r.X);
            fe_sub(ref r.X, r.Z, r.Y);
            fe_add(ref r.Y, r.Z, r.Y);
            fe_sub(ref r.Z, t0, r.T);
            fe_add(ref r.T, t0, r.T);
        }

        public static void ge_tobytes(ref byte[] s, ge_p2 h)
        {
            int[] recip = new int[10];
            int[] x = new int[10];
            int[] y = new int[10];

            fe_invert(ref recip, h.Z);
            fe_mul(ref x, h.X, recip);
            fe_mul(ref y, h.Y, recip);
            fe_tobytes(ref s, y);
            s[31] ^= (byte)(fe_isnegative(x) << 7);
        }

        public static void sc_reduce(ref byte[] s)
        {
            long s0 = 2097151 & (long)(load_3(s));
            long s1 = 2097151 & (long)((load_4(s, 2) >> 5));
            long s2 = 2097151 & (long)((load_3(s, 5) >> 2));
            long s3 = 2097151 & (long)((load_4(s, 7) >> 7));
            long s4 = 2097151 & (long)((load_4(s, 10) >> 4));
            long s5 = 2097151 & (long)((load_3(s, 13) >> 1));
            long s6 = 2097151 & (long)((load_4(s, 15) >> 6));
            long s7 = 2097151 & (long)((load_3(s, 18) >> 3));
            long s8 = 2097151 & (long)(load_3(s, 21));
            long s9 = 2097151 & (long)((load_4(s, 23) >> 5));
            long s10 = 2097151 & (long)((load_3(s, 26) >> 2));
            long s11 = 2097151 & (long)((load_4(s, 28) >> 7));
            long s12 = 2097151 & (long)((load_4(s, 31) >> 4));
            long s13 = 2097151 & (long)((load_3(s, 34) >> 1));
            long s14 = 2097151 & (long)((load_4(s, 36) >> 6));
            long s15 = 2097151 & (long)((load_3(s, 39) >> 3));
            long s16 = 2097151 & (long)(load_3(s, 42));
            long s17 = 2097151 & (long)((load_4(s, 44) >> 5));
            long s18 = 2097151 & (long)((load_3(s, 47) >> 2));
            long s19 = 2097151 & (long)((load_4(s, 49) >> 7));
            long s20 = 2097151 & (long)((load_4(s, 52) >> 4));
            long s21 = 2097151 & (long)((load_3(s, 55) >> 1));
            long s22 = 2097151 & (long)((load_4(s, 57) >> 6));
            long s23 = (long)((load_4(s, 60) >> 3));
            long carry0;
            long carry1;
            long carry2;
            long carry3;
            long carry4;
            long carry5;
            long carry6;
            long carry7;
            long carry8;
            long carry9;
            long carry10;
            long carry11;
            long carry12;
            long carry13;
            long carry14;
            long carry15;
            long carry16;

            s11 += s23 * 666643;
            s12 += s23 * 470296;
            s13 += s23 * 654183;
            s14 -= s23 * 997805;
            s15 += s23 * 136657;
            s16 -= s23 * 683901;

            s10 += s22 * 666643;
            s11 += s22 * 470296;
            s12 += s22 * 654183;
            s13 -= s22 * 997805;
            s14 += s22 * 136657;
            s15 -= s22 * 683901;

            s9 += s21 * 666643;
            s10 += s21 * 470296;
            s11 += s21 * 654183;
            s12 -= s21 * 997805;
            s13 += s21 * 136657;
            s14 -= s21 * 683901;

            s8 += s20 * 666643;
            s9 += s20 * 470296;
            s10 += s20 * 654183;
            s11 -= s20 * 997805;
            s12 += s20 * 136657;
            s13 -= s20 * 683901;

            s7 += s19 * 666643;
            s8 += s19 * 470296;
            s9 += s19 * 654183;
            s10 -= s19 * 997805;
            s11 += s19 * 136657;
            s12 -= s19 * 683901;

            s6 += s18 * 666643;
            s7 += s18 * 470296;
            s8 += s18 * 654183;
            s9 -= s18 * 997805;
            s10 += s18 * 136657;
            s11 -= s18 * 683901;

            carry6 = (s6 + (1<<20)) >> 21; s7 += carry6; s6 -= carry6 << 21;
            carry8 = (s8 + (1<<20)) >> 21; s9 += carry8; s8 -= carry8 << 21;
            carry10 = (s10 + (1<<20)) >> 21; s11 += carry10; s10 -= carry10 << 21;
            carry12 = (s12 + (1<<20)) >> 21; s13 += carry12; s12 -= carry12 << 21;
            carry14 = (s14 + (1<<20)) >> 21; s15 += carry14; s14 -= carry14 << 21;
            carry16 = (s16 + (1<<20)) >> 21; s17 += carry16; s16 -= carry16 << 21;

            carry7 = (s7 + (1<<20)) >> 21; s8 += carry7; s7 -= carry7 << 21;
            carry9 = (s9 + (1<<20)) >> 21; s10 += carry9; s9 -= carry9 << 21;
            carry11 = (s11 + (1<<20)) >> 21; s12 += carry11; s11 -= carry11 << 21;
            carry13 = (s13 + (1<<20)) >> 21; s14 += carry13; s13 -= carry13 << 21;
            carry15 = (s15 + (1<<20)) >> 21; s16 += carry15; s15 -= carry15 << 21;

            s5 += s17 * 666643;
            s6 += s17 * 470296;
            s7 += s17 * 654183;
            s8 -= s17 * 997805;
            s9 += s17 * 136657;
            s10 -= s17 * 683901;

            s4 += s16 * 666643;
            s5 += s16 * 470296;
            s6 += s16 * 654183;
            s7 -= s16 * 997805;
            s8 += s16 * 136657;
            s9 -= s16 * 683901;

            s3 += s15 * 666643;
            s4 += s15 * 470296;
            s5 += s15 * 654183;
            s6 -= s15 * 997805;
            s7 += s15 * 136657;
            s8 -= s15 * 683901;

            s2 += s14 * 666643;
            s3 += s14 * 470296;
            s4 += s14 * 654183;
            s5 -= s14 * 997805;
            s6 += s14 * 136657;
            s7 -= s14 * 683901;

            s1 += s13 * 666643;
            s2 += s13 * 470296;
            s3 += s13 * 654183;
            s4 -= s13 * 997805;
            s5 += s13 * 136657;
            s6 -= s13 * 683901;

            s0 += s12 * 666643;
            s1 += s12 * 470296;
            s2 += s12 * 654183;
            s3 -= s12 * 997805;
            s4 += s12 * 136657;
            s5 -= s12 * 683901;
            s12 = 0;

            carry0 = (s0 + (1<<20)) >> 21; s1 += carry0; s0 -= carry0 << 21;
            carry2 = (s2 + (1<<20)) >> 21; s3 += carry2; s2 -= carry2 << 21;
            carry4 = (s4 + (1<<20)) >> 21; s5 += carry4; s4 -= carry4 << 21;
            carry6 = (s6 + (1<<20)) >> 21; s7 += carry6; s6 -= carry6 << 21;
            carry8 = (s8 + (1<<20)) >> 21; s9 += carry8; s8 -= carry8 << 21;
            carry10 = (s10 + (1<<20)) >> 21; s11 += carry10; s10 -= carry10 << 21;

            carry1 = (s1 + (1<<20)) >> 21; s2 += carry1; s1 -= carry1 << 21;
            carry3 = (s3 + (1<<20)) >> 21; s4 += carry3; s3 -= carry3 << 21;
            carry5 = (s5 + (1<<20)) >> 21; s6 += carry5; s5 -= carry5 << 21;
            carry7 = (s7 + (1<<20)) >> 21; s8 += carry7; s7 -= carry7 << 21;
            carry9 = (s9 + (1<<20)) >> 21; s10 += carry9; s9 -= carry9 << 21;
            carry11 = (s11 + (1<<20)) >> 21; s12 += carry11; s11 -= carry11 << 21;

            s0 += s12 * 666643;
            s1 += s12 * 470296;
            s2 += s12 * 654183;
            s3 -= s12 * 997805;
            s4 += s12 * 136657;
            s5 -= s12 * 683901;
            s12 = 0;

            carry0 = s0 >> 21; s1 += carry0; s0 -= carry0 << 21;
            carry1 = s1 >> 21; s2 += carry1; s1 -= carry1 << 21;
            carry2 = s2 >> 21; s3 += carry2; s2 -= carry2 << 21;
            carry3 = s3 >> 21; s4 += carry3; s3 -= carry3 << 21;
            carry4 = s4 >> 21; s5 += carry4; s4 -= carry4 << 21;
            carry5 = s5 >> 21; s6 += carry5; s5 -= carry5 << 21;
            carry6 = s6 >> 21; s7 += carry6; s6 -= carry6 << 21;
            carry7 = s7 >> 21; s8 += carry7; s7 -= carry7 << 21;
            carry8 = s8 >> 21; s9 += carry8; s8 -= carry8 << 21;
            carry9 = s9 >> 21; s10 += carry9; s9 -= carry9 << 21;
            carry10 = s10 >> 21; s11 += carry10; s10 -= carry10 << 21;
            carry11 = s11 >> 21; s12 += carry11; s11 -= carry11 << 21;

            s0 += s12 * 666643;
            s1 += s12 * 470296;
            s2 += s12 * 654183;
            s3 -= s12 * 997805;
            s4 += s12 * 136657;
            s5 -= s12 * 683901;

            carry0 = s0 >> 21; s1 += carry0; s0 -= carry0 << 21;
            carry1 = s1 >> 21; s2 += carry1; s1 -= carry1 << 21;
            carry2 = s2 >> 21; s3 += carry2; s2 -= carry2 << 21;
            carry3 = s3 >> 21; s4 += carry3; s3 -= carry3 << 21;
            carry4 = s4 >> 21; s5 += carry4; s4 -= carry4 << 21;
            carry5 = s5 >> 21; s6 += carry5; s5 -= carry5 << 21;
            carry6 = s6 >> 21; s7 += carry6; s6 -= carry6 << 21;
            carry7 = s7 >> 21; s8 += carry7; s7 -= carry7 << 21;
            carry8 = s8 >> 21; s9 += carry8; s8 -= carry8 << 21;
            carry9 = s9 >> 21; s10 += carry9; s9 -= carry9 << 21;
            carry10 = s10 >> 21; s11 += carry10; s10 -= carry10 << 21;

            s[0] = (byte)(s0 >> 0);
            s[1] = (byte)(s0 >> 8);
            s[2] = (byte)((s0 >> 16) | (s1 << 5));
            s[3] = (byte)(s1 >> 3);
            s[4] = (byte)(s1 >> 11);
            s[5] = (byte)((s1 >> 19) | (s2 << 2));
            s[6] = (byte)(s2 >> 6);
            s[7] = (byte)((s2 >> 14) | (s3 << 7));
            s[8] = (byte)(s3 >> 1);
            s[9] = (byte)(s3 >> 9);
            s[10] = (byte)((s3 >> 17) | (s4 << 4));
            s[11] = (byte)(s4 >> 4);
            s[12] = (byte)(s4 >> 12);
            s[13] = (byte)((s4 >> 20) | (s5 << 1));
            s[14] = (byte)(s5 >> 7);
            s[15] = (byte)((s5 >> 15) | (s6 << 6));
            s[16] = (byte)(s6 >> 2);
            s[17] = (byte)(s6 >> 10);
            s[18] = (byte)((s6 >> 18) | (s7 << 3));
            s[19] = (byte)(s7 >> 5);
            s[20] = (byte)(s7 >> 13);
            s[21] = (byte)(s8 >> 0);
            s[22] = (byte)(s8 >> 8);
            s[23] = (byte)((s8 >> 16) | (s9 << 5));
            s[24] = (byte)(s9 >> 3);
            s[25] = (byte)(s9 >> 11);
            s[26] = (byte)((s9 >> 19) | (s10 << 2));
            s[27] = (byte)(s10 >> 6);
            s[28] = (byte)((s10 >> 14) | (s11 << 7));
            s[29] = (byte)(s11 >> 1);
            s[30] = (byte)(s11 >> 9);
            s[31] = (byte)(s11 >> 17);
        }

        public static void fe_divpowm1(ref int[] r, int[] u, int[] v)
        {
            int[] v3 = new int[10];
            int[] uv7 = new int[10];
            int[] t0 = new int[10];
            int[] t1 = new int[10];
            int[] t2 = new int[10];

            fe_sq(ref v3, v);
            fe_mul(ref v3, v3, v); /* v3 = v^3 */
            fe_sq(ref uv7, v3);
            fe_mul(ref uv7, uv7, v);
            fe_mul(ref uv7, uv7, u); /* uv7 = uv^7 */

            /*fe_pow22523(uv7, uv7);*/

            /* From fe_pow22523.c */

            fe_sq(ref t0, uv7);
            fe_sq(ref t1, t0);
            fe_sq(ref t1, t1);
            fe_mul(ref t1, uv7, t1);
            fe_mul(ref t0, t0, t1);
            fe_sq(ref t0, t0);
            fe_mul(ref t0, t1, t0);
            fe_sq(ref t1, t0);

            for (int i = 0; i < 4; ++i)
            {
                fe_sq(ref t1, t1);
            }

            fe_mul(ref t0, t1, t0);
            fe_sq(ref t1, t0);

            for (int i = 0; i < 9; ++i)
            {
                fe_sq(ref t1, t1);
            }

            fe_mul(ref t1, t1, t0);
            fe_sq(ref t2, t1);

            for (int i = 0; i < 19; ++i)
            {
                fe_sq(ref t2, t2);
            }

            fe_mul(ref t1, t2, t1);

            for (int i = 0; i < 10; ++i)
            {
                fe_sq(ref t1, t1);
            }

            fe_mul(ref t0, t1, t0);
            fe_sq(ref t1, t0);

            for (int i = 0; i < 49; ++i)
            {
                fe_sq(ref t1, t1);
            }

            fe_mul(ref t1, t1, t0);
            fe_sq(ref t2, t1);

            for (int i = 0; i < 99; ++i) {
                fe_sq(ref t2, t2);
            }

            fe_mul(ref t1, t2, t1);

            for (int i = 0; i < 50; ++i)
            {
                fe_sq(ref t1, t1);
            }

            fe_mul(ref t0, t1, t0);
            fe_sq(ref t0, t0);
            fe_sq(ref t0, t0);
            fe_mul(ref t0, t0, uv7);

            /* ref t0 = (uv^7)^((q-5)/8) */
            fe_mul(ref t0, t0, v3);
            fe_mul(ref r, t0, u); /* u^(m+1)v^(-(m+1)) */
        }

        public static void ge_cached_0(ge_cached r)
        {
            fe_1(ref r.YplusX);
            fe_1(ref r.YminusX);
            fe_1(ref r.Z);
            fe_0(ref r.T2d);
        }

        public static void ge_cached_cmov(ge_cached t, ge_cached u, byte b)
        {
            fe_cmov(ref t.YplusX, u.YplusX, b);
            fe_cmov(ref t.YminusX, u.YminusX, b);
            fe_cmov(ref t.Z, u.Z, b);
            fe_cmov(ref t.T2d, u.T2d, b);
        }

        public static void ge_scalarmult(ge_p2 r, byte[] a, ge_p3 A)
        {
            sbyte[] e = new sbyte[64];

            int carry = 0;
            int carry2;

            ge_dsmp Ai = new ge_dsmp(); /* 1 * A, 2 * A, ..., 8 * A */
            ge_p1p1 t = new ge_p1p1();
            ge_p3 u = new ge_p3();

            for (int i = 0; i < 31; i++)
            {
                carry += a[i]; /* 0..256 */
                carry2 = (carry + 8) >> 4; /* 0..16 */
                e[2 * i] = (sbyte)(carry - (carry2 << 4)); /* -8..7 */
                carry = (carry2 + 8) >> 4; /* 0..1 */
                e[2 * i + 1] = (sbyte)(carry2 - (carry << 4)); /* -8..7 */
            }

            carry += a[31]; /* 0..128 */
            carry2 = (carry + 8) >> 4; /* 0..8 */
            e[62] = (sbyte)(carry - (carry2 << 4)); /* -8..7 */
            e[63] = (sbyte)carry2; /* 0..8 */

            ge_p3_to_cached(Ai[0], A);

            for (int i = 0; i < 7; i++)
            {
                ge_add(t, A, Ai[i]);
                ge_p1p1_to_p3(u, t);
                ge_p3_to_cached(Ai[i + 1], u);
            }

            ge_p2_0(r);

            for (int i = 63; i >= 0; i--)
            {
                sbyte b = e[i];
                byte bnegative = negative(b);
                byte babs = (byte)(b - (((-bnegative) & b) << 1));
                ge_cached cur = new ge_cached();
                ge_cached minuscur = new ge_cached();
                ge_p2_dbl(t, r);
                ge_p1p1_to_p2(r, t);
                ge_p2_dbl(t, r);
                ge_p1p1_to_p2(r, t);
                ge_p2_dbl(t, r);
                ge_p1p1_to_p2(r, t);
                ge_p2_dbl(t, r);
                ge_p1p1_to_p3(u, t);
                ge_cached_0(cur);

                for (int j = 0; j < 8; j++)
                {
                    ge_cached_cmov(cur, Ai[j], equal((sbyte)babs, (sbyte)(j+1)));
                }

                fe_copy(ref minuscur.YplusX, cur.YminusX);
                fe_copy(ref minuscur.YminusX, cur.YplusX);
                fe_copy(ref minuscur.Z, cur.Z);
                fe_neg(ref minuscur.T2d, cur.T2d);
                ge_cached_cmov(cur, minuscur, bnegative);
                ge_add(t, u, cur);
                ge_p1p1_to_p2(r, t);
            }
        }

        public static void ge_scalarmult_p3(ge_p3 r3, byte[] a, ge_p3 A)
        {
            sbyte[] e = new sbyte[64];
            int carry = 0;
            int carry2;

            ge_dsmp Ai = new ge_dsmp();
            ge_p1p1 t = new ge_p1p1();
            ge_p3 u = new ge_p3();
            ge_p2 r = new ge_p2();

            for (int i = 0; i < 31; i++)
            {
                carry += a[i]; /* 0..256 */
                carry2 = (carry + 8) >> 4; /* 0..16 */
                e[2 * i] = (sbyte)(carry - (carry2 << 4)); /* -8..7 */
                carry = (carry2 + 8) >> 4; /* 0..1 */
                e[2 * i + 1] = (sbyte)(carry2 - (carry << 4)); /* -8..7 */
            }

            carry += a[31]; /* 0..128 */
            carry2 = (carry + 8) >> 4; /* 0..8 */
            e[62] = (sbyte)(carry - (carry2 << 4)); /* -8..7 */
            e[63] = (sbyte)carry2; /* 0..8 */

            ge_p3_to_cached(Ai[0], A);

            for (int i = 0; i < 7; i++)
            {
                ge_add(t, A, Ai[i]);
                ge_p1p1_to_p3(u, t);
                ge_p3_to_cached(Ai[i + 1], u);
            }

            ge_p2_0(r);
            
            for (int i = 63; i >= 0; i--)
            {
                sbyte b = e[i];
                byte bnegative = negative(b);
                byte babs = (byte)(b - (((-bnegative) & b) << 1));

                ge_cached cur = new ge_cached();
                ge_cached minuscur = new ge_cached();

                ge_p2_dbl(t, r);
                ge_p1p1_to_p2(r, t);
                ge_p2_dbl(t, r);
                ge_p1p1_to_p2(r, t);
                ge_p2_dbl(t, r);
                ge_p1p1_to_p2(r, t);
                ge_p2_dbl(t, r);
                ge_p1p1_to_p3(u, t);
                ge_cached_0(cur);

                for (int j = 0; j < 8; j++)
                {
                    ge_cached_cmov(cur, Ai[j], equal((sbyte)babs, (sbyte)(j+1)));
                }

                fe_copy(ref minuscur.YplusX, cur.YminusX);
                fe_copy(ref minuscur.YminusX, cur.YplusX);
                fe_copy(ref minuscur.Z, cur.Z);
                fe_neg(ref minuscur.T2d, cur.T2d);
                ge_cached_cmov(cur, minuscur, bnegative);
                ge_add(t, u, cur);

                if (i == 0)
                {
                    ge_p1p1_to_p3(r3, t);
                }
                else
                {
                    ge_p1p1_to_p2(r, t);
                }
            }
        }

        public static void ge_double_scalarmult_precomp_vartime2(ge_p2 r, byte[] a, ge_dsmp Ai, byte[] b, ge_dsmp Bi)
        {
            sbyte[] aslide = new sbyte[256];
            sbyte[] bslide = new sbyte[256];

            ge_p1p1 t = new ge_p1p1();
            ge_p3 u = new ge_p3();

            int i;

            slide(ref aslide, a);
            slide(ref bslide, b);

            ge_p2_0(r);

            for (i = 255; i >= 0; --i)
            {
                if (aslide[i] != 0 || bslide[i] != 0)
                {
                    break;
                }
            }

            for (; i > 0; --i)
            {
                ge_p2_dbl(t, r);

                if (aslide[i] > 0)
                {
                    ge_p1p1_to_p3(u, t);
                    ge_add(t, u, Ai[aslide[i]/2]);
                }
                else if (aslide[i] < 0)
                {
                    ge_p1p1_to_p3(u, t);
                    ge_sub(t, u, Ai[(-aslide[i]) / 2]);
                }
            }

            if (bslide[i] > 0)
            {
                ge_p1p1_to_p3(u, t);
                ge_add(t, u, Bi[bslide[i]/2]);
            }
            else if (bslide[i] < 0)
            {
                ge_p1p1_to_p3(u, t);
                ge_sub(t, u, Bi[(-bslide[i])/2]);
            }

            ge_p1p1_to_p2(r, t);
        }

        public static void ge_double_scalarmult_precomp_vartime2_p3(ge_p3 r3, byte[] a, ge_dsmp Ai, byte[] b, ge_dsmp Bi)
        {
            sbyte[] aslide = new sbyte[256];
            sbyte[] bslide = new sbyte[256];

            ge_p1p1 t = new ge_p1p1();
            ge_p3 u = new ge_p3();
            ge_p2 r = new ge_p2();

            int i;

            slide(ref aslide, a);
            slide(ref bslide, b);

            ge_p2_0(r);

            for (i = 255; i >= 0; --i)
            {
                if (aslide[i] != 0 || bslide[i] != 0)
                {
                    break;
                }
            }

            for (; i >= 0; --i)
            {
                ge_p2_dbl(t, r);

                if (aslide[i] > 0)
                {
                    ge_p1p1_to_p3(u, t);
                    ge_add(t, u, Ai[aslide[i]/2]);
                }
                else if (aslide[i] < 0)
                {
                    ge_p1p1_to_p3(u, t);
                    ge_sub(t, u, Ai[(-aslide[i])/2]);
                }

                if (bslide[i] > 0)
                {
                    ge_p1p1_to_p3(u, t);
                    ge_add(t, u, Bi[bslide[i]/2]);
                }
                else if (bslide[i] < 0)
                {
                    ge_p1p1_to_p3(u, t);
                    ge_sub(t, u, Bi[(-bslide[i])/2]);
                }

                if (i == 0)
                {
                    ge_p1p1_to_p3(r3, t);
                }
                else
                {
                    ge_p1p1_to_p2(r, t);
                }
            }
        }

        public static void ge_double_scalarmult_precomp_vartime(ge_p2 r, byte[] a, ge_p3 A, byte[] b, ge_dsmp Bi)
        {
            ge_dsmp Ai = new ge_dsmp();

            ge_dsm_precomp(ref Ai, A);
            ge_double_scalarmult_precomp_vartime2(r, a, Ai, b, Bi);
        }

        public static void ge_mul8(ge_p1p1 r, ge_p2 t)
        {
            ge_p2 u = new ge_p2();

            ge_p2_dbl(r, t);
            ge_p1p1_to_p2(u, r);
            ge_p2_dbl(r, u);
            ge_p1p1_to_p2(u, r);
            ge_p2_dbl(r, u);
        }

        public static void ge_fromfe_frombytes_vartime(ge_p2 r, byte[] s)
        {
            int[] u = new int[10];
            int[] v = new int[10];
            int[] w = new int[10];
            int[] x = new int[10];
            int[] y = new int[10];
            int[] z = new int[10];

            byte sign;

            long h0 = (long)(load_4(s));
            long h1 = (long)(load_3(s, 4) << 6);
            long h2 = (long)(load_3(s, 7) << 5);
            long h3 = (long)(load_3(s, 10) << 3);
            long h4 = (long)(load_3(s, 13) << 2);
            long h5 = (long)(load_4(s, 16));
            long h6 = (long)(load_3(s, 20) << 7);
            long h7 = (long)(load_3(s, 23) << 5);
            long h8 = (long)(load_3(s, 26) << 4);
            long h9 = (long)(load_3(s, 29) << 2);
            long carry0;
            long carry1;
            long carry2;
            long carry3;
            long carry4;
            long carry5;
            long carry6;
            long carry7;
            long carry8;
            long carry9;

            carry9 = (h9 + (long) (1<<24)) >> 25; h0 += carry9 * 19; h9 -= carry9 << 25;
            carry1 = (h1 + (long) (1<<24)) >> 25; h2 += carry1; h1 -= carry1 << 25;
            carry3 = (h3 + (long) (1<<24)) >> 25; h4 += carry3; h3 -= carry3 << 25;
            carry5 = (h5 + (long) (1<<24)) >> 25; h6 += carry5; h5 -= carry5 << 25;
            carry7 = (h7 + (long) (1<<24)) >> 25; h8 += carry7; h7 -= carry7 << 25;

            carry0 = (h0 + (long) (1<<25)) >> 26; h1 += carry0; h0 -= carry0 << 26;
            carry2 = (h2 + (long) (1<<25)) >> 26; h3 += carry2; h2 -= carry2 << 26;
            carry4 = (h4 + (long) (1<<25)) >> 26; h5 += carry4; h4 -= carry4 << 26;
            carry6 = (h6 + (long) (1<<25)) >> 26; h7 += carry6; h6 -= carry6 << 26;
            carry8 = (h8 + (long) (1<<25)) >> 26; h9 += carry8; h8 -= carry8 << 26;

            u[0] = (int)h0;
            u[1] = (int)h1;
            u[2] = (int)h2;
            u[3] = (int)h3;
            u[4] = (int)h4;
            u[5] = (int)h5;
            u[6] = (int)h6;
            u[7] = (int)h7;
            u[8] = (int)h8;
            u[9] = (int)h9;

            /* End fe_frombytes.c */

            fe_sq2(ref v, u); /* 2 * ref u^2 */
            fe_1(ref w);
            fe_add(ref w, v, w); /* ref w = 2 * ref u^2 + 1 */
            fe_sq(ref x, w); /* ref w^2 */
            fe_mul(ref y, FE_MA2, v); /* -2 * A^2 * ref u^2 */
            fe_add(ref x, x, y); /* ref x = ref w^2 - 2 * A^2 * ref u^2 */
            fe_divpowm1(ref r.X, w, x); /* (ref w / ref x)^(m + 1) */
            fe_sq(ref y, r.X);
            fe_mul(ref x, y, x);
            fe_sub(ref y, w, x);
            fe_copy(ref z, FE_MA);

            if (fe_isnonzero(y))
            {
                fe_add(ref y, w, x);

                if (fe_isnonzero(y))
                {
                    goto negative;
                }
                else
                {
                    fe_mul(ref r.X, r.X, FE_FFFB1);
                }
            }
            else
            {
                fe_mul(ref r.X, r.X, FE_FFFB2);
            }

            fe_mul(ref r.X, r.X, u); /* ref u * sqrt(2 * A * (A + 2) * ref w / ref x) */
            fe_mul(ref z, z, v); /* -2 * A * ref u^2 */
            sign = 0;
            goto setsign;

        negative:
            fe_mul(ref x, x, FE_SQRTM1);
            fe_sub(ref y, w, x);

            if (fe_isnonzero(y))
            {
                fe_mul(ref r.X, r.X, FE_FFFB3);
            }
            else
            {
                fe_mul(ref r.X, r.X, FE_FFFB4);
            }

            /* ref r.X = sqrt(A * (A + 2) * ref w / ref x) */
            /* ref z = -A */
            sign = 1;

         setsign:
            if (fe_isnegative(r.X) != sign)
            {
                fe_neg(ref r.X, r.X);
            }

            fe_add(ref r.Z, z, w);
            fe_sub(ref r.Y, z, w);
            fe_mul(ref r.X, r.X, r.Z);
        }

        public static int ge_check_subgroup_precomp_vartime(ge_dsmp p)
        {
            ge_p3 s = new ge_p3();
            ge_p1p1 t = new ge_p1p1();
            ge_p2 u = new ge_p2();
            ge_p3_0(s);
            ge_add(t, s, p[7]);
            ge_p1p1_to_p3(s, t);
            ge_add(t, s, p[0]);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p3(s, t);
            ge_add(t, s, p[2]);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p3(s, t);
            ge_add(t, s, p[3]);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p3(s, t);
            ge_sub(t, s, p[0]);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p3(s, t);
            ge_sub(t, s, p[1]);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p3(s, t);
            ge_sub(t, s, p[0]);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p3(s, t);
            ge_sub(t, s, p[5]);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p3(s, t);
            ge_add(t, s, p[1]);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p3(s, t);
            ge_sub(t, s, p[0]);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p3(s, t);
            ge_sub(t, s, p[1]);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p3(s, t);
            ge_sub(t, s, p[6]);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p3(s, t);
            ge_add(t, s, p[5]);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p3(s, t);
            ge_add(t, s, p[5]);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p3(s, t);
            ge_add(t, s, p[4]);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p3(s, t);
            ge_add(t, s, p[1]);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p3(s, t);
            ge_add(t, s, p[1]);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p3(s, t);
            ge_add(t, s, p[6]);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p3(s, t);
            ge_add(t, s, p[1]);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p3(s, t);
            ge_sub(t, s, p[1]);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p3(s, t);
            ge_sub(t, s, p[2]);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p3(s, t);
            ge_sub(t, s, p[5]);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p3(s, t);
            ge_sub(t, s, p[2]);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p2(u, t);
            ge_p2_dbl(t, u);
            ge_p1p1_to_p3(s, t);
            ge_add(t, s, p[0]);
            fe_sub(ref t.Y, t.Y, t.T);
            return fe_isnonzero(t.Y) ? 1 : 0;
        }

        public static void random_scalar(ref byte[] res)
        {
            byte[] tmp = SecureRandom.Bytes(64);
            sc_reduce(ref tmp);
            res = tmp.SubBytes(0, 32);
        }

        public static void sc_0(ref byte[] s)
        {
            for (int i = 0; i < 32; i++)
            {
                s[i] = 0;
            }
        }

        public static void sc_reduce32(ref byte[] s)
        {
            long s0 = (long)(2097151 & load_3(s));
            long s1 = (long)(2097151 & (load_4(s, 2) >> 5));
            long s2 = (long)(2097151 & (load_3(s, 5) >> 2));
            long s3 = (long)(2097151 & (load_4(s, 7) >> 7));
            long s4 = (long)(2097151 & (load_4(s, 10) >> 4));
            long s5 = (long)(2097151 & (load_3(s, 13) >> 1));
            long s6 = (long)(2097151 & (load_4(s, 15) >> 6));
            long s7 = (long)(2097151 & (load_3(s, 18) >> 3));
            long s8 = (long)(2097151 & load_3(s, 21));
            long s9 = (long)(2097151 & (load_4(s, 23) >> 5));
            long s10 = (long)(2097151 & (load_3(s, 26) >> 2));
            long s11 = (long)((load_4(s, 28) >> 7));
            long s12 = 0;
            long carry0;
            long carry1;
            long carry2;
            long carry3;
            long carry4;
            long carry5;
            long carry6;
            long carry7;
            long carry8;
            long carry9;
            long carry10;
            long carry11;

            carry0 = (s0 + (1<<20)) >> 21; s1 += carry0; s0 -= carry0 << 21;
            carry2 = (s2 + (1<<20)) >> 21; s3 += carry2; s2 -= carry2 << 21;
            carry4 = (s4 + (1<<20)) >> 21; s5 += carry4; s4 -= carry4 << 21;
            carry6 = (s6 + (1<<20)) >> 21; s7 += carry6; s6 -= carry6 << 21;
            carry8 = (s8 + (1<<20)) >> 21; s9 += carry8; s8 -= carry8 << 21;
            carry10 = (s10 + (1<<20)) >> 21; s11 += carry10; s10 -= carry10 << 21;

            carry1 = (s1 + (1<<20)) >> 21; s2 += carry1; s1 -= carry1 << 21;
            carry3 = (s3 + (1<<20)) >> 21; s4 += carry3; s3 -= carry3 << 21;
            carry5 = (s5 + (1<<20)) >> 21; s6 += carry5; s5 -= carry5 << 21;
            carry7 = (s7 + (1<<20)) >> 21; s8 += carry7; s7 -= carry7 << 21;
            carry9 = (s9 + (1<<20)) >> 21; s10 += carry9; s9 -= carry9 << 21;
            carry11 = (s11 + (1<<20)) >> 21; s12 += carry11; s11 -= carry11 << 21;

            s0 += s12 * 666643;
            s1 += s12 * 470296;
            s2 += s12 * 654183;
            s3 -= s12 * 997805;
            s4 += s12 * 136657;
            s5 -= s12 * 683901;
            s12 = 0;

            carry0 = s0 >> 21; s1 += carry0; s0 -= carry0 << 21;
            carry1 = s1 >> 21; s2 += carry1; s1 -= carry1 << 21;
            carry2 = s2 >> 21; s3 += carry2; s2 -= carry2 << 21;
            carry3 = s3 >> 21; s4 += carry3; s3 -= carry3 << 21;
            carry4 = s4 >> 21; s5 += carry4; s4 -= carry4 << 21;
            carry5 = s5 >> 21; s6 += carry5; s5 -= carry5 << 21;
            carry6 = s6 >> 21; s7 += carry6; s6 -= carry6 << 21;
            carry7 = s7 >> 21; s8 += carry7; s7 -= carry7 << 21;
            carry8 = s8 >> 21; s9 += carry8; s8 -= carry8 << 21;
            carry9 = s9 >> 21; s10 += carry9; s9 -= carry9 << 21;
            carry10 = s10 >> 21; s11 += carry10; s10 -= carry10 << 21;
            carry11 = s11 >> 21; s12 += carry11; s11 -= carry11 << 21;

            s0 += s12 * 666643;
            s1 += s12 * 470296;
            s2 += s12 * 654183;
            s3 -= s12 * 997805;
            s4 += s12 * 136657;
            s5 -= s12 * 683901;

            carry0 = s0 >> 21; s1 += carry0; s0 -= carry0 << 21;
            carry1 = s1 >> 21; s2 += carry1; s1 -= carry1 << 21;
            carry2 = s2 >> 21; s3 += carry2; s2 -= carry2 << 21;
            carry3 = s3 >> 21; s4 += carry3; s3 -= carry3 << 21;
            carry4 = s4 >> 21; s5 += carry4; s4 -= carry4 << 21;
            carry5 = s5 >> 21; s6 += carry5; s5 -= carry5 << 21;
            carry6 = s6 >> 21; s7 += carry6; s6 -= carry6 << 21;
            carry7 = s7 >> 21; s8 += carry7; s7 -= carry7 << 21;
            carry8 = s8 >> 21; s9 += carry8; s8 -= carry8 << 21;
            carry9 = s9 >> 21; s10 += carry9; s9 -= carry9 << 21;
            carry10 = s10 >> 21; s11 += carry10; s10 -= carry10 << 21;

            s[0] = (byte)(s0 >> 0);
            s[1] = (byte)(s0 >> 8);
            s[2] = (byte)((s0 >> 16) | (s1 << 5));
            s[3] = (byte)(s1 >> 3);
            s[4] = (byte)(s1 >> 11);
            s[5] = (byte)((s1 >> 19) | (s2 << 2));
            s[6] = (byte)(s2 >> 6);
            s[7] = (byte)((s2 >> 14) | (s3 << 7));
            s[8] = (byte)(s3 >> 1);
            s[9] = (byte)(s3 >> 9);
            s[10] = (byte)((s3 >> 17) | (s4 << 4));
            s[11] = (byte)(s4 >> 4);
            s[12] = (byte)(s4 >> 12);
            s[13] = (byte)((s4 >> 20) | (s5 << 1));
            s[14] = (byte)(s5 >> 7);
            s[15] = (byte)((s5 >> 15) | (s6 << 6));
            s[16] = (byte)(s6 >> 2);
            s[17] = (byte)(s6 >> 10);
            s[18] = (byte)((s6 >> 18) | (s7 << 3));
            s[19] = (byte)(s7 >> 5);
            s[20] = (byte)(s7 >> 13);
            s[21] = (byte)(s8 >> 0);
            s[22] = (byte)(s8 >> 8);
            s[23] = (byte)((s8 >> 16) | (s9 << 5));
            s[24] = (byte)(s9 >> 3);
            s[25] = (byte)(s9 >> 11);
            s[26] = (byte)((s9 >> 19) | (s10 << 2));
            s[27] = (byte)(s10 >> 6);
            s[28] = (byte)((s10 >> 14) | (s11 << 7));
            s[29] = (byte)(s11 >> 1);
            s[30] = (byte)(s11 >> 9);
            s[31] = (byte)(s11 >> 17);
        }

        public static void sc_add(ref byte[] s, byte[] a, byte[] b)
        {
            long a0 = (long)(2097151 & load_3(a));
            long a1 = (long)(2097151 & (load_4(a, 2) >> 5));
            long a2 = (long)(2097151 & (load_3(a, 5) >> 2));
            long a3 = (long)(2097151 & (load_4(a, 7) >> 7));
            long a4 = (long)(2097151 & (load_4(a, 10) >> 4));
            long a5 = (long)(2097151 & (load_3(a, 13) >> 1));
            long a6 = (long)(2097151 & (load_4(a, 15) >> 6));
            long a7 = (long)(2097151 & (load_3(a, 18) >> 3));
            long a8 = (long)(2097151 & load_3(a, 21));
            long a9 = (long)(2097151 & (load_4(a, 23) >> 5));
            long a10 = (long)(2097151 & (load_3(a, 26) >> 2));
            long a11 = (long)((load_4(a, 28) >> 7));
            long b0 = (long)(2097151 & load_3(b));
            long b1 = (long)(2097151 & (load_4(b, 2) >> 5));
            long b2 = (long)(2097151 & (load_3(b, 5) >> 2));
            long b3 = (long)(2097151 & (load_4(b, 7) >> 7));
            long b4 = (long)(2097151 & (load_4(b, 10) >> 4));
            long b5 = (long)(2097151 & (load_3(b, 13) >> 1));
            long b6 = (long)(2097151 & (load_4(b, 15) >> 6));
            long b7 = (long)(2097151 & (load_3(b, 18) >> 3));
            long b8 = (long)(2097151 & load_3(b, 21));
            long b9 = (long)(2097151 & (load_4(b, 23) >> 5));
            long b10 = (long)(2097151 & (load_3(b, 26) >> 2));
            long b11 = (long)((load_4(b, 28) >> 7));
            long s0 = a0 + b0;
            long s1 = a1 + b1;
            long s2 = a2 + b2;
            long s3 = a3 + b3;
            long s4 = a4 + b4;
            long s5 = a5 + b5;
            long s6 = a6 + b6;
            long s7 = a7 + b7;
            long s8 = a8 + b8;
            long s9 = a9 + b9;
            long s10 = a10 + b10;
            long s11 = a11 + b11;
            long s12 = 0;
            long carry0;
            long carry1;
            long carry2;
            long carry3;
            long carry4;
            long carry5;
            long carry6;
            long carry7;
            long carry8;
            long carry9;
            long carry10;
            long carry11;

            carry0 = (s0 + (1<<20)) >> 21; s1 += carry0; s0 -= carry0 << 21;
            carry2 = (s2 + (1<<20)) >> 21; s3 += carry2; s2 -= carry2 << 21;
            carry4 = (s4 + (1<<20)) >> 21; s5 += carry4; s4 -= carry4 << 21;
            carry6 = (s6 + (1<<20)) >> 21; s7 += carry6; s6 -= carry6 << 21;
            carry8 = (s8 + (1<<20)) >> 21; s9 += carry8; s8 -= carry8 << 21;
            carry10 = (s10 + (1<<20)) >> 21; s11 += carry10; s10 -= carry10 << 21;

            carry1 = (s1 + (1<<20)) >> 21; s2 += carry1; s1 -= carry1 << 21;
            carry3 = (s3 + (1<<20)) >> 21; s4 += carry3; s3 -= carry3 << 21;
            carry5 = (s5 + (1<<20)) >> 21; s6 += carry5; s5 -= carry5 << 21;
            carry7 = (s7 + (1<<20)) >> 21; s8 += carry7; s7 -= carry7 << 21;
            carry9 = (s9 + (1<<20)) >> 21; s10 += carry9; s9 -= carry9 << 21;
            carry11 = (s11 + (1<<20)) >> 21; s12 += carry11; s11 -= carry11 << 21;

            s0 += s12 * 666643;
            s1 += s12 * 470296;
            s2 += s12 * 654183;
            s3 -= s12 * 997805;
            s4 += s12 * 136657;
            s5 -= s12 * 683901;
            s12 = 0;

            carry0 = s0 >> 21; s1 += carry0; s0 -= carry0 << 21;
            carry1 = s1 >> 21; s2 += carry1; s1 -= carry1 << 21;
            carry2 = s2 >> 21; s3 += carry2; s2 -= carry2 << 21;
            carry3 = s3 >> 21; s4 += carry3; s3 -= carry3 << 21;
            carry4 = s4 >> 21; s5 += carry4; s4 -= carry4 << 21;
            carry5 = s5 >> 21; s6 += carry5; s5 -= carry5 << 21;
            carry6 = s6 >> 21; s7 += carry6; s6 -= carry6 << 21;
            carry7 = s7 >> 21; s8 += carry7; s7 -= carry7 << 21;
            carry8 = s8 >> 21; s9 += carry8; s8 -= carry8 << 21;
            carry9 = s9 >> 21; s10 += carry9; s9 -= carry9 << 21;
            carry10 = s10 >> 21; s11 += carry10; s10 -= carry10 << 21;
            carry11 = s11 >> 21; s12 += carry11; s11 -= carry11 << 21;

            s0 += s12 * 666643;
            s1 += s12 * 470296;
            s2 += s12 * 654183;
            s3 -= s12 * 997805;
            s4 += s12 * 136657;
            s5 -= s12 * 683901;

            carry0 = s0 >> 21; s1 += carry0; s0 -= carry0 << 21;
            carry1 = s1 >> 21; s2 += carry1; s1 -= carry1 << 21;
            carry2 = s2 >> 21; s3 += carry2; s2 -= carry2 << 21;
            carry3 = s3 >> 21; s4 += carry3; s3 -= carry3 << 21;
            carry4 = s4 >> 21; s5 += carry4; s4 -= carry4 << 21;
            carry5 = s5 >> 21; s6 += carry5; s5 -= carry5 << 21;
            carry6 = s6 >> 21; s7 += carry6; s6 -= carry6 << 21;
            carry7 = s7 >> 21; s8 += carry7; s7 -= carry7 << 21;
            carry8 = s8 >> 21; s9 += carry8; s8 -= carry8 << 21;
            carry9 = s9 >> 21; s10 += carry9; s9 -= carry9 << 21;
            carry10 = s10 >> 21; s11 += carry10; s10 -= carry10 << 21;

            s[0] = (byte)(s0 >> 0);
            s[1] = (byte)(s0 >> 8);
            s[2] = (byte)((s0 >> 16) | (s1 << 5));
            s[3] = (byte)(s1 >> 3);
            s[4] = (byte)(s1 >> 11);
            s[5] = (byte)((s1 >> 19) | (s2 << 2));
            s[6] = (byte)(s2 >> 6);
            s[7] = (byte)((s2 >> 14) | (s3 << 7));
            s[8] = (byte)(s3 >> 1);
            s[9] = (byte)(s3 >> 9);
            s[10] = (byte)((s3 >> 17) | (s4 << 4));
            s[11] = (byte)(s4 >> 4);
            s[12] = (byte)(s4 >> 12);
            s[13] = (byte)((s4 >> 20) | (s5 << 1));
            s[14] = (byte)(s5 >> 7);
            s[15] = (byte)((s5 >> 15) | (s6 << 6));
            s[16] = (byte)(s6 >> 2);
            s[17] = (byte)(s6 >> 10);
            s[18] = (byte)((s6 >> 18) | (s7 << 3));
            s[19] = (byte)(s7 >> 5);
            s[20] = (byte)(s7 >> 13);
            s[21] = (byte)(s8 >> 0);
            s[22] = (byte)(s8 >> 8);
            s[23] = (byte)((s8 >> 16) | (s9 << 5));
            s[24] = (byte)(s9 >> 3);
            s[25] = (byte)(s9 >> 11);
            s[26] = (byte)((s9 >> 19) | (s10 << 2));
            s[27] = (byte)(s10 >> 6);
            s[28] = (byte)((s10 >> 14) | (s11 << 7));
            s[29] = (byte)(s11 >> 1);
            s[30] = (byte)(s11 >> 9);
            s[31] = (byte)(s11 >> 17);
        }

        public static void sc_sub(ref byte[] s, byte[] a, byte [] b)
        {
            long a0 = (long)(2097151 & load_3(a));
            long a1 = (long)(2097151 & (load_4(a, 2) >> 5));
            long a2 = (long)(2097151 & (load_3(a, 5) >> 2));
            long a3 = (long)(2097151 & (load_4(a, 7) >> 7));
            long a4 = (long)(2097151 & (load_4(a, 10) >> 4));
            long a5 = (long)(2097151 & (load_3(a, 13) >> 1));
            long a6 = (long)(2097151 & (load_4(a, 15) >> 6));
            long a7 = (long)(2097151 & (load_3(a, 18) >> 3));
            long a8 = (long)(2097151 & load_3(a, 21));
            long a9 = (long)(2097151 & (load_4(a, 23) >> 5));
            long a10 = (long)(2097151 & (load_3(a, 26) >> 2));
            long a11 = (long)((load_4(a, 28) >> 7));
            long b0 = (long)(2097151 & load_3(b));
            long b1 = (long)(2097151 & (load_4(b, 2) >> 5));
            long b2 = (long)(2097151 & (load_3(b, 5) >> 2));
            long b3 = (long)(2097151 & (load_4(b, 7) >> 7));
            long b4 = (long)(2097151 & (load_4(b, 10) >> 4));
            long b5 = (long)(2097151 & (load_3(b, 13) >> 1));
            long b6 = (long)(2097151 & (load_4(b, 15) >> 6));
            long b7 = (long)(2097151 & (load_3(b, 18) >> 3));
            long b8 = (long)(2097151 & load_3(b, 21));
            long b9 = (long)(2097151 & (load_4(b, 23) >> 5));
            long b10 = (long)(2097151 & (load_3(b, 26) >> 2));
            long b11 = (long)((load_4(b, 28) >> 7));
            long s0 = a0 - b0;
            long s1 = a1 - b1;
            long s2 = a2 - b2;
            long s3 = a3 - b3;
            long s4 = a4 - b4;
            long s5 = a5 - b5;
            long s6 = a6 - b6;
            long s7 = a7 - b7;
            long s8 = a8 - b8;
            long s9 = a9 - b9;
            long s10 = a10 - b10;
            long s11 = a11 - b11;
            long s12 = 0;
            long carry0;
            long carry1;
            long carry2;
            long carry3;
            long carry4;
            long carry5;
            long carry6;
            long carry7;
            long carry8;
            long carry9;
            long carry10;
            long carry11;

            carry0 = (s0 + (1<<20)) >> 21; s1 += carry0; s0 -= carry0 << 21;
            carry2 = (s2 + (1<<20)) >> 21; s3 += carry2; s2 -= carry2 << 21;
            carry4 = (s4 + (1<<20)) >> 21; s5 += carry4; s4 -= carry4 << 21;
            carry6 = (s6 + (1<<20)) >> 21; s7 += carry6; s6 -= carry6 << 21;
            carry8 = (s8 + (1<<20)) >> 21; s9 += carry8; s8 -= carry8 << 21;
            carry10 = (s10 + (1<<20)) >> 21; s11 += carry10; s10 -= carry10 << 21;

            carry1 = (s1 + (1<<20)) >> 21; s2 += carry1; s1 -= carry1 << 21;
            carry3 = (s3 + (1<<20)) >> 21; s4 += carry3; s3 -= carry3 << 21;
            carry5 = (s5 + (1<<20)) >> 21; s6 += carry5; s5 -= carry5 << 21;
            carry7 = (s7 + (1<<20)) >> 21; s8 += carry7; s7 -= carry7 << 21;
            carry9 = (s9 + (1<<20)) >> 21; s10 += carry9; s9 -= carry9 << 21;
            carry11 = (s11 + (1<<20)) >> 21; s12 += carry11; s11 -= carry11 << 21;

            s0 += s12 * 666643;
            s1 += s12 * 470296;
            s2 += s12 * 654183;
            s3 -= s12 * 997805;
            s4 += s12 * 136657;
            s5 -= s12 * 683901;
            s12 = 0;

            carry0 = s0 >> 21; s1 += carry0; s0 -= carry0 << 21;
            carry1 = s1 >> 21; s2 += carry1; s1 -= carry1 << 21;
            carry2 = s2 >> 21; s3 += carry2; s2 -= carry2 << 21;
            carry3 = s3 >> 21; s4 += carry3; s3 -= carry3 << 21;
            carry4 = s4 >> 21; s5 += carry4; s4 -= carry4 << 21;
            carry5 = s5 >> 21; s6 += carry5; s5 -= carry5 << 21;
            carry6 = s6 >> 21; s7 += carry6; s6 -= carry6 << 21;
            carry7 = s7 >> 21; s8 += carry7; s7 -= carry7 << 21;
            carry8 = s8 >> 21; s9 += carry8; s8 -= carry8 << 21;
            carry9 = s9 >> 21; s10 += carry9; s9 -= carry9 << 21;
            carry10 = s10 >> 21; s11 += carry10; s10 -= carry10 << 21;
            carry11 = s11 >> 21; s12 += carry11; s11 -= carry11 << 21;

            s0 += s12 * 666643;
            s1 += s12 * 470296;
            s2 += s12 * 654183;
            s3 -= s12 * 997805;
            s4 += s12 * 136657;
            s5 -= s12 * 683901;

            carry0 = s0 >> 21; s1 += carry0; s0 -= carry0 << 21;
            carry1 = s1 >> 21; s2 += carry1; s1 -= carry1 << 21;
            carry2 = s2 >> 21; s3 += carry2; s2 -= carry2 << 21;
            carry3 = s3 >> 21; s4 += carry3; s3 -= carry3 << 21;
            carry4 = s4 >> 21; s5 += carry4; s4 -= carry4 << 21;
            carry5 = s5 >> 21; s6 += carry5; s5 -= carry5 << 21;
            carry6 = s6 >> 21; s7 += carry6; s6 -= carry6 << 21;
            carry7 = s7 >> 21; s8 += carry7; s7 -= carry7 << 21;
            carry8 = s8 >> 21; s9 += carry8; s8 -= carry8 << 21;
            carry9 = s9 >> 21; s10 += carry9; s9 -= carry9 << 21;
            carry10 = s10 >> 21; s11 += carry10; s10 -= carry10 << 21;

            s[0] = (byte)(s0 >> 0);
            s[1] = (byte)(s0 >> 8);
            s[2] = (byte)((s0 >> 16) | (s1 << 5));
            s[3] = (byte)(s1 >> 3);
            s[4] = (byte)(s1 >> 11);
            s[5] = (byte)((s1 >> 19) | (s2 << 2));
            s[6] = (byte)(s2 >> 6);
            s[7] = (byte)((s2 >> 14) | (s3 << 7));
            s[8] = (byte)(s3 >> 1);
            s[9] = (byte)(s3 >> 9);
            s[10] = (byte)((s3 >> 17) | (s4 << 4));
            s[11] = (byte)(s4 >> 4);
            s[12] = (byte)(s4 >> 12);
            s[13] = (byte)((s4 >> 20) | (s5 << 1));
            s[14] = (byte)(s5 >> 7);
            s[15] = (byte)((s5 >> 15) | (s6 << 6));
            s[16] = (byte)(s6 >> 2);
            s[17] = (byte)(s6 >> 10);
            s[18] = (byte)((s6 >> 18) | (s7 << 3));
            s[19] = (byte)(s7 >> 5);
            s[20] = (byte)(s7 >> 13);
            s[21] = (byte)(s8 >> 0);
            s[22] = (byte)(s8 >> 8);
            s[23] = (byte)((s8 >> 16) | (s9 << 5));
            s[24] = (byte)(s9 >> 3);
            s[25] = (byte)(s9 >> 11);
            s[26] = (byte)((s9 >> 19) | (s10 << 2));
            s[27] = (byte)(s10 >> 6);
            s[28] = (byte)((s10 >> 14) | (s11 << 7));
            s[29] = (byte)(s11 >> 1);
            s[30] = (byte)(s11 >> 9);
            s[31] = (byte)(s11 >> 17);
        }

        /*
        Input:
          a[0]+256*a[1]+...+256^31*a[31] = a
          b[0]+256*b[1]+...+256^31*b[31] = b
          c[0]+256*c[1]+...+256^31*c[31] = c
        Output:
          s[0]+256*s[1]+...+256^31*s[31] = (c-ab) mod l
          where l = 2^252 + 27742317777372353535851937790883648493.
        */
        public static void sc_mulsub(ref byte[] s, byte[] a, byte[] b, byte[] c)
        {
            long a0 = (long)(2097151 & load_3(a));
            long a1 = (long)(2097151 & (load_4(a, 2) >> 5));
            long a2 = (long)(2097151 & (load_3(a, 5) >> 2));
            long a3 = (long)(2097151 & (load_4(a, 7) >> 7));
            long a4 = (long)(2097151 & (load_4(a, 10) >> 4));
            long a5 = (long)(2097151 & (load_3(a, 13) >> 1));
            long a6 = (long)(2097151 & (load_4(a, 15) >> 6));
            long a7 = (long)(2097151 & (load_3(a, 18) >> 3));
            long a8 = (long)(2097151 & load_3(a, 21));
            long a9 = (long)(2097151 & (load_4(a, 23) >> 5));
            long a10 = (long)(2097151 & (load_3(a, 26) >> 2));
            long a11 = (long)((load_4(a, 28) >> 7));
            long b0 = (long)(2097151 & load_3(b));
            long b1 = (long)(2097151 & (load_4(b, 2) >> 5));
            long b2 = (long)(2097151 & (load_3(b, 5) >> 2));
            long b3 = (long)(2097151 & (load_4(b, 7) >> 7));
            long b4 = (long)(2097151 & (load_4(b, 10) >> 4));
            long b5 = (long)(2097151 & (load_3(b, 13) >> 1));
            long b6 = (long)(2097151 & (load_4(b, 15) >> 6));
            long b7 = (long)(2097151 & (load_3(b, 18) >> 3));
            long b8 = (long)(2097151 & load_3(b, 21));
            long b9 = (long)(2097151 & (load_4(b, 23) >> 5));
            long b10 = (long)(2097151 & (load_3(b, 26) >> 2));
            long b11 = (long)((load_4(b, 28) >> 7));
            long c0 = (long)(2097151 & load_3(c));
            long c1 = (long)(2097151 & (load_4(c, 2) >> 5));
            long c2 = (long)(2097151 & (load_3(c, 5) >> 2));
            long c3 = (long)(2097151 & (load_4(c, 7) >> 7));
            long c4 = (long)(2097151 & (load_4(c, 10) >> 4));
            long c5 = (long)(2097151 & (load_3(c, 13) >> 1));
            long c6 = (long)(2097151 & (load_4(c, 15) >> 6));
            long c7 = (long)(2097151 & (load_3(c, 18) >> 3));
            long c8 = (long)(2097151 & load_3(c, 21));
            long c9 = (long)(2097151 & (load_4(c, 23) >> 5));
            long c10 = (long)(2097151 & (load_3(c, 26) >> 2));
            long c11 = (long)((load_4(c, 28) >> 7));
            long s0;
            long s1;
            long s2;
            long s3;
            long s4;
            long s5;
            long s6;
            long s7;
            long s8;
            long s9;
            long s10;
            long s11;
            long s12;
            long s13;
            long s14;
            long s15;
            long s16;
            long s17;
            long s18;
            long s19;
            long s20;
            long s21;
            long s22;
            long s23;
            long carry0;
            long carry1;
            long carry2;
            long carry3;
            long carry4;
            long carry5;
            long carry6;
            long carry7;
            long carry8;
            long carry9;
            long carry10;
            long carry11;
            long carry12;
            long carry13;
            long carry14;
            long carry15;
            long carry16;
            long carry17;
            long carry18;
            long carry19;
            long carry20;
            long carry21;
            long carry22;

            s0 = c0 - a0*b0;
            s1 = c1 - (a0*b1 + a1*b0);
            s2 = c2 - (a0*b2 + a1*b1 + a2*b0);
            s3 = c3 - (a0*b3 + a1*b2 + a2*b1 + a3*b0);
            s4 = c4 - (a0*b4 + a1*b3 + a2*b2 + a3*b1 + a4*b0);
            s5 = c5 - (a0*b5 + a1*b4 + a2*b3 + a3*b2 + a4*b1 + a5*b0);
            s6 = c6 - (a0*b6 + a1*b5 + a2*b4 + a3*b3 + a4*b2 + a5*b1 + a6*b0);
            s7 = c7 - (a0*b7 + a1*b6 + a2*b5 + a3*b4 + a4*b3 + a5*b2 + a6*b1 + a7*b0);
            s8 = c8 - (a0*b8 + a1*b7 + a2*b6 + a3*b5 + a4*b4 + a5*b3 + a6*b2 + a7*b1 + a8*b0);
            s9 = c9 - (a0*b9 + a1*b8 + a2*b7 + a3*b6 + a4*b5 + a5*b4 + a6*b3 + a7*b2 + a8*b1 + a9*b0);
            s10 = c10 - (a0*b10 + a1*b9 + a2*b8 + a3*b7 + a4*b6 + a5*b5 + a6*b4 + a7*b3 + a8*b2 + a9*b1 + a10*b0);
            s11 = c11 - (a0*b11 + a1*b10 + a2*b9 + a3*b8 + a4*b7 + a5*b6 + a6*b5 + a7*b4 + a8*b3 + a9*b2 + a10*b1 + a11*b0);
            s12 = -(a1*b11 + a2*b10 + a3*b9 + a4*b8 + a5*b7 + a6*b6 + a7*b5 + a8*b4 + a9*b3 + a10*b2 + a11*b1);
            s13 = -(a2*b11 + a3*b10 + a4*b9 + a5*b8 + a6*b7 + a7*b6 + a8*b5 + a9*b4 + a10*b3 + a11*b2);
            s14 = -(a3*b11 + a4*b10 + a5*b9 + a6*b8 + a7*b7 + a8*b6 + a9*b5 + a10*b4 + a11*b3);
            s15 = -(a4*b11 + a5*b10 + a6*b9 + a7*b8 + a8*b7 + a9*b6 + a10*b5 + a11*b4);
            s16 = -(a5*b11 + a6*b10 + a7*b9 + a8*b8 + a9*b7 + a10*b6 + a11*b5);
            s17 = -(a6*b11 + a7*b10 + a8*b9 + a9*b8 + a10*b7 + a11*b6);
            s18 = -(a7*b11 + a8*b10 + a9*b9 + a10*b8 + a11*b7);
            s19 = -(a8*b11 + a9*b10 + a10*b9 + a11*b8);
            s20 = -(a9*b11 + a10*b10 + a11*b9);
            s21 = -(a10*b11 + a11*b10);
            s22 = -a11*b11;
            s23 = 0;

            carry0 = (s0 + (1<<20)) >> 21; s1 += carry0; s0 -= carry0 << 21;
            carry2 = (s2 + (1<<20)) >> 21; s3 += carry2; s2 -= carry2 << 21;
            carry4 = (s4 + (1<<20)) >> 21; s5 += carry4; s4 -= carry4 << 21;
            carry6 = (s6 + (1<<20)) >> 21; s7 += carry6; s6 -= carry6 << 21;
            carry8 = (s8 + (1<<20)) >> 21; s9 += carry8; s8 -= carry8 << 21;
            carry10 = (s10 + (1<<20)) >> 21; s11 += carry10; s10 -= carry10 << 21;
            carry12 = (s12 + (1<<20)) >> 21; s13 += carry12; s12 -= carry12 << 21;
            carry14 = (s14 + (1<<20)) >> 21; s15 += carry14; s14 -= carry14 << 21;
            carry16 = (s16 + (1<<20)) >> 21; s17 += carry16; s16 -= carry16 << 21;
            carry18 = (s18 + (1<<20)) >> 21; s19 += carry18; s18 -= carry18 << 21;
            carry20 = (s20 + (1<<20)) >> 21; s21 += carry20; s20 -= carry20 << 21;
            carry22 = (s22 + (1<<20)) >> 21; s23 += carry22; s22 -= carry22 << 21;

            carry1 = (s1 + (1<<20)) >> 21; s2 += carry1; s1 -= carry1 << 21;
            carry3 = (s3 + (1<<20)) >> 21; s4 += carry3; s3 -= carry3 << 21;
            carry5 = (s5 + (1<<20)) >> 21; s6 += carry5; s5 -= carry5 << 21;
            carry7 = (s7 + (1<<20)) >> 21; s8 += carry7; s7 -= carry7 << 21;
            carry9 = (s9 + (1<<20)) >> 21; s10 += carry9; s9 -= carry9 << 21;
            carry11 = (s11 + (1<<20)) >> 21; s12 += carry11; s11 -= carry11 << 21;
            carry13 = (s13 + (1<<20)) >> 21; s14 += carry13; s13 -= carry13 << 21;
            carry15 = (s15 + (1<<20)) >> 21; s16 += carry15; s15 -= carry15 << 21;
            carry17 = (s17 + (1<<20)) >> 21; s18 += carry17; s17 -= carry17 << 21;
            carry19 = (s19 + (1<<20)) >> 21; s20 += carry19; s19 -= carry19 << 21;
            carry21 = (s21 + (1<<20)) >> 21; s22 += carry21; s21 -= carry21 << 21;

            s11 += s23 * 666643;
            s12 += s23 * 470296;
            s13 += s23 * 654183;
            s14 -= s23 * 997805;
            s15 += s23 * 136657;
            s16 -= s23 * 683901;

            s10 += s22 * 666643;
            s11 += s22 * 470296;
            s12 += s22 * 654183;
            s13 -= s22 * 997805;
            s14 += s22 * 136657;
            s15 -= s22 * 683901;

            s9 += s21 * 666643;
            s10 += s21 * 470296;
            s11 += s21 * 654183;
            s12 -= s21 * 997805;
            s13 += s21 * 136657;
            s14 -= s21 * 683901;

            s8 += s20 * 666643;
            s9 += s20 * 470296;
            s10 += s20 * 654183;
            s11 -= s20 * 997805;
            s12 += s20 * 136657;
            s13 -= s20 * 683901;

            s7 += s19 * 666643;
            s8 += s19 * 470296;
            s9 += s19 * 654183;
            s10 -= s19 * 997805;
            s11 += s19 * 136657;
            s12 -= s19 * 683901;

            s6 += s18 * 666643;
            s7 += s18 * 470296;
            s8 += s18 * 654183;
            s9 -= s18 * 997805;
            s10 += s18 * 136657;
            s11 -= s18 * 683901;

            carry6 = (s6 + (1<<20)) >> 21; s7 += carry6; s6 -= carry6 << 21;
            carry8 = (s8 + (1<<20)) >> 21; s9 += carry8; s8 -= carry8 << 21;
            carry10 = (s10 + (1<<20)) >> 21; s11 += carry10; s10 -= carry10 << 21;
            carry12 = (s12 + (1<<20)) >> 21; s13 += carry12; s12 -= carry12 << 21;
            carry14 = (s14 + (1<<20)) >> 21; s15 += carry14; s14 -= carry14 << 21;
            carry16 = (s16 + (1<<20)) >> 21; s17 += carry16; s16 -= carry16 << 21;

            carry7 = (s7 + (1<<20)) >> 21; s8 += carry7; s7 -= carry7 << 21;
            carry9 = (s9 + (1<<20)) >> 21; s10 += carry9; s9 -= carry9 << 21;
            carry11 = (s11 + (1<<20)) >> 21; s12 += carry11; s11 -= carry11 << 21;
            carry13 = (s13 + (1<<20)) >> 21; s14 += carry13; s13 -= carry13 << 21;
            carry15 = (s15 + (1<<20)) >> 21; s16 += carry15; s15 -= carry15 << 21;

            s5 += s17 * 666643;
            s6 += s17 * 470296;
            s7 += s17 * 654183;
            s8 -= s17 * 997805;
            s9 += s17 * 136657;
            s10 -= s17 * 683901;

            s4 += s16 * 666643;
            s5 += s16 * 470296;
            s6 += s16 * 654183;
            s7 -= s16 * 997805;
            s8 += s16 * 136657;
            s9 -= s16 * 683901;

            s3 += s15 * 666643;
            s4 += s15 * 470296;
            s5 += s15 * 654183;
            s6 -= s15 * 997805;
            s7 += s15 * 136657;
            s8 -= s15 * 683901;

            s2 += s14 * 666643;
            s3 += s14 * 470296;
            s4 += s14 * 654183;
            s5 -= s14 * 997805;
            s6 += s14 * 136657;
            s7 -= s14 * 683901;

            s1 += s13 * 666643;
            s2 += s13 * 470296;
            s3 += s13 * 654183;
            s4 -= s13 * 997805;
            s5 += s13 * 136657;
            s6 -= s13 * 683901;

            s0 += s12 * 666643;
            s1 += s12 * 470296;
            s2 += s12 * 654183;
            s3 -= s12 * 997805;
            s4 += s12 * 136657;
            s5 -= s12 * 683901;
            s12 = 0;

            carry0 = (s0 + (1<<20)) >> 21; s1 += carry0; s0 -= carry0 << 21;
            carry2 = (s2 + (1<<20)) >> 21; s3 += carry2; s2 -= carry2 << 21;
            carry4 = (s4 + (1<<20)) >> 21; s5 += carry4; s4 -= carry4 << 21;
            carry6 = (s6 + (1<<20)) >> 21; s7 += carry6; s6 -= carry6 << 21;
            carry8 = (s8 + (1<<20)) >> 21; s9 += carry8; s8 -= carry8 << 21;
            carry10 = (s10 + (1<<20)) >> 21; s11 += carry10; s10 -= carry10 << 21;

            carry1 = (s1 + (1<<20)) >> 21; s2 += carry1; s1 -= carry1 << 21;
            carry3 = (s3 + (1<<20)) >> 21; s4 += carry3; s3 -= carry3 << 21;
            carry5 = (s5 + (1<<20)) >> 21; s6 += carry5; s5 -= carry5 << 21;
            carry7 = (s7 + (1<<20)) >> 21; s8 += carry7; s7 -= carry7 << 21;
            carry9 = (s9 + (1<<20)) >> 21; s10 += carry9; s9 -= carry9 << 21;
            carry11 = (s11 + (1<<20)) >> 21; s12 += carry11; s11 -= carry11 << 21;

            s0 += s12 * 666643;
            s1 += s12 * 470296;
            s2 += s12 * 654183;
            s3 -= s12 * 997805;
            s4 += s12 * 136657;
            s5 -= s12 * 683901;
            s12 = 0;

            carry0 = s0 >> 21; s1 += carry0; s0 -= carry0 << 21;
            carry1 = s1 >> 21; s2 += carry1; s1 -= carry1 << 21;
            carry2 = s2 >> 21; s3 += carry2; s2 -= carry2 << 21;
            carry3 = s3 >> 21; s4 += carry3; s3 -= carry3 << 21;
            carry4 = s4 >> 21; s5 += carry4; s4 -= carry4 << 21;
            carry5 = s5 >> 21; s6 += carry5; s5 -= carry5 << 21;
            carry6 = s6 >> 21; s7 += carry6; s6 -= carry6 << 21;
            carry7 = s7 >> 21; s8 += carry7; s7 -= carry7 << 21;
            carry8 = s8 >> 21; s9 += carry8; s8 -= carry8 << 21;
            carry9 = s9 >> 21; s10 += carry9; s9 -= carry9 << 21;
            carry10 = s10 >> 21; s11 += carry10; s10 -= carry10 << 21;
            carry11 = s11 >> 21; s12 += carry11; s11 -= carry11 << 21;

            s0 += s12 * 666643;
            s1 += s12 * 470296;
            s2 += s12 * 654183;
            s3 -= s12 * 997805;
            s4 += s12 * 136657;
            s5 -= s12 * 683901;

            carry0 = s0 >> 21; s1 += carry0; s0 -= carry0 << 21;
            carry1 = s1 >> 21; s2 += carry1; s1 -= carry1 << 21;
            carry2 = s2 >> 21; s3 += carry2; s2 -= carry2 << 21;
            carry3 = s3 >> 21; s4 += carry3; s3 -= carry3 << 21;
            carry4 = s4 >> 21; s5 += carry4; s4 -= carry4 << 21;
            carry5 = s5 >> 21; s6 += carry5; s5 -= carry5 << 21;
            carry6 = s6 >> 21; s7 += carry6; s6 -= carry6 << 21;
            carry7 = s7 >> 21; s8 += carry7; s7 -= carry7 << 21;
            carry8 = s8 >> 21; s9 += carry8; s8 -= carry8 << 21;
            carry9 = s9 >> 21; s10 += carry9; s9 -= carry9 << 21;
            carry10 = s10 >> 21; s11 += carry10; s10 -= carry10 << 21;

            s[0] = (byte)(s0 >> 0);
            s[1] = (byte)(s0 >> 8);
            s[2] = (byte)((s0 >> 16) | (s1 << 5));
            s[3] = (byte)(s1 >> 3);
            s[4] = (byte)(s1 >> 11);
            s[5] = (byte)((s1 >> 19) | (s2 << 2));
            s[6] = (byte)(s2 >> 6);
            s[7] = (byte)((s2 >> 14) | (s3 << 7));
            s[8] = (byte)(s3 >> 1);
            s[9] = (byte)(s3 >> 9);
            s[10] = (byte)((s3 >> 17) | (s4 << 4));
            s[11] = (byte)(s4 >> 4);
            s[12] = (byte)(s4 >> 12);
            s[13] = (byte)((s4 >> 20) | (s5 << 1));
            s[14] = (byte)(s5 >> 7);
            s[15] = (byte)((s5 >> 15) | (s6 << 6));
            s[16] = (byte)(s6 >> 2);
            s[17] = (byte)(s6 >> 10);
            s[18] = (byte)((s6 >> 18) | (s7 << 3));
            s[19] = (byte)(s7 >> 5);
            s[20] = (byte)(s7 >> 13);
            s[21] = (byte)(s8 >> 0);
            s[22] = (byte)(s8 >> 8);
            s[23] = (byte)((s8 >> 16) | (s9 << 5));
            s[24] = (byte)(s9 >> 3);
            s[25] = (byte)(s9 >> 11);
            s[26] = (byte)((s9 >> 19) | (s10 << 2));
            s[27] = (byte)(s10 >> 6);
            s[28] = (byte)((s10 >> 14) | (s11 << 7));
            s[29] = (byte)(s11 >> 1);
            s[30] = (byte)(s11 >> 9);
            s[31] = (byte)(s11 >> 17);
        }

        /*
        Input:
          a[0]+256*a[1]+...+256^31*a[31] = a
          b[0]+256*b[1]+...+256^31*b[31] = b
        Output:
          s[0]+256*s[1]+...+256^31*s[31] = (ab) mod l
          where l = 2^252 + 27742317777372353535851937790883648493.
        */
        public static void sc_mul(ref byte[] s, byte[] a, byte[] b)
        {
            long a0 = (long)(2097151 & load_3(a));
            long a1 = (long)(2097151 & (load_4(a, 2) >> 5));
            long a2 = (long)(2097151 & (load_3(a, 5) >> 2));
            long a3 = (long)(2097151 & (load_4(a, 7) >> 7));
            long a4 = (long)(2097151 & (load_4(a, 10) >> 4));
            long a5 = (long)(2097151 & (load_3(a, 13) >> 1));
            long a6 = (long)(2097151 & (load_4(a, 15) >> 6));
            long a7 = (long)(2097151 & (load_3(a, 18) >> 3));
            long a8 = (long)(2097151 & load_3(a, 21));
            long a9 = (long)(2097151 & (load_4(a, 23) >> 5));
            long a10 = (long)(2097151 & (load_3(a, 26) >> 2));
            long a11 = (long)((load_4(a, 28) >> 7));
            long b0 = (long)(2097151 & load_3(b));
            long b1 = (long)(2097151 & (load_4(b, 2) >> 5));
            long b2 = (long)(2097151 & (load_3(b, 5) >> 2));
            long b3 = (long)(2097151 & (load_4(b, 7) >> 7));
            long b4 = (long)(2097151 & (load_4(b, 10) >> 4));
            long b5 = (long)(2097151 & (load_3(b, 13) >> 1));
            long b6 = (long)(2097151 & (load_4(b, 15) >> 6));
            long b7 = (long)(2097151 & (load_3(b, 18) >> 3));
            long b8 = (long)(2097151 & load_3(b, 21));
            long b9 = (long)(2097151 & (load_4(b, 23) >> 5));
            long b10 = (long)(2097151 & (load_3(b, 26) >> 2));
            long b11 = (long)((load_4(b, 28) >> 7));
            long s0;
            long s1;
            long s2;
            long s3;
            long s4;
            long s5;
            long s6;
            long s7;
            long s8;
            long s9;
            long s10;
            long s11;
            long s12;
            long s13;
            long s14;
            long s15;
            long s16;
            long s17;
            long s18;
            long s19;
            long s20;
            long s21;
            long s22;
            long s23;
            long carry0;
            long carry1;
            long carry2;
            long carry3;
            long carry4;
            long carry5;
            long carry6;
            long carry7;
            long carry8;
            long carry9;
            long carry10;
            long carry11;
            long carry12;
            long carry13;
            long carry14;
            long carry15;
            long carry16;
            long carry17;
            long carry18;
            long carry19;
            long carry20;
            long carry21;
            long carry22;

            s0 = a0*b0;
            s1 = (a0*b1 + a1*b0);
            s2 = (a0*b2 + a1*b1 + a2*b0);
            s3 = (a0*b3 + a1*b2 + a2*b1 + a3*b0);
            s4 = (a0*b4 + a1*b3 + a2*b2 + a3*b1 + a4*b0);
            s5 = (a0*b5 + a1*b4 + a2*b3 + a3*b2 + a4*b1 + a5*b0);
            s6 = (a0*b6 + a1*b5 + a2*b4 + a3*b3 + a4*b2 + a5*b1 + a6*b0);
            s7 = (a0*b7 + a1*b6 + a2*b5 + a3*b4 + a4*b3 + a5*b2 + a6*b1 + a7*b0);
            s8 = (a0*b8 + a1*b7 + a2*b6 + a3*b5 + a4*b4 + a5*b3 + a6*b2 + a7*b1 + a8*b0);
            s9 = (a0*b9 + a1*b8 + a2*b7 + a3*b6 + a4*b5 + a5*b4 + a6*b3 + a7*b2 + a8*b1 + a9*b0);
            s10 = (a0*b10 + a1*b9 + a2*b8 + a3*b7 + a4*b6 + a5*b5 + a6*b4 + a7*b3 + a8*b2 + a9*b1 + a10*b0);
            s11 = (a0*b11 + a1*b10 + a2*b9 + a3*b8 + a4*b7 + a5*b6 + a6*b5 + a7*b4 + a8*b3 + a9*b2 + a10*b1 + a11*b0);
            s12 = (a1*b11 + a2*b10 + a3*b9 + a4*b8 + a5*b7 + a6*b6 + a7*b5 + a8*b4 + a9*b3 + a10*b2 + a11*b1);
            s13 = (a2*b11 + a3*b10 + a4*b9 + a5*b8 + a6*b7 + a7*b6 + a8*b5 + a9*b4 + a10*b3 + a11*b2);
            s14 = (a3*b11 + a4*b10 + a5*b9 + a6*b8 + a7*b7 + a8*b6 + a9*b5 + a10*b4 + a11*b3);
            s15 = (a4*b11 + a5*b10 + a6*b9 + a7*b8 + a8*b7 + a9*b6 + a10*b5 + a11*b4);
            s16 = (a5*b11 + a6*b10 + a7*b9 + a8*b8 + a9*b7 + a10*b6 + a11*b5);
            s17 = (a6*b11 + a7*b10 + a8*b9 + a9*b8 + a10*b7 + a11*b6);
            s18 = (a7*b11 + a8*b10 + a9*b9 + a10*b8 + a11*b7);
            s19 = (a8*b11 + a9*b10 + a10*b9 + a11*b8);
            s20 = (a9*b11 + a10*b10 + a11*b9);
            s21 = (a10*b11 + a11*b10);
            s22 = a11*b11;
            s23 = 0;

            carry0 = (s0 + (1<<20)) >> 21; s1 += carry0; s0 -= carry0 << 21;
            carry2 = (s2 + (1<<20)) >> 21; s3 += carry2; s2 -= carry2 << 21;
            carry4 = (s4 + (1<<20)) >> 21; s5 += carry4; s4 -= carry4 << 21;
            carry6 = (s6 + (1<<20)) >> 21; s7 += carry6; s6 -= carry6 << 21;
            carry8 = (s8 + (1<<20)) >> 21; s9 += carry8; s8 -= carry8 << 21;
            carry10 = (s10 + (1<<20)) >> 21; s11 += carry10; s10 -= carry10 << 21;
            carry12 = (s12 + (1<<20)) >> 21; s13 += carry12; s12 -= carry12 << 21;
            carry14 = (s14 + (1<<20)) >> 21; s15 += carry14; s14 -= carry14 << 21;
            carry16 = (s16 + (1<<20)) >> 21; s17 += carry16; s16 -= carry16 << 21;
            carry18 = (s18 + (1<<20)) >> 21; s19 += carry18; s18 -= carry18 << 21;
            carry20 = (s20 + (1<<20)) >> 21; s21 += carry20; s20 -= carry20 << 21;
            carry22 = (s22 + (1<<20)) >> 21; s23 += carry22; s22 -= carry22 << 21;

            carry1 = (s1 + (1<<20)) >> 21; s2 += carry1; s1 -= carry1 << 21;
            carry3 = (s3 + (1<<20)) >> 21; s4 += carry3; s3 -= carry3 << 21;
            carry5 = (s5 + (1<<20)) >> 21; s6 += carry5; s5 -= carry5 << 21;
            carry7 = (s7 + (1<<20)) >> 21; s8 += carry7; s7 -= carry7 << 21;
            carry9 = (s9 + (1<<20)) >> 21; s10 += carry9; s9 -= carry9 << 21;
            carry11 = (s11 + (1<<20)) >> 21; s12 += carry11; s11 -= carry11 << 21;
            carry13 = (s13 + (1<<20)) >> 21; s14 += carry13; s13 -= carry13 << 21;
            carry15 = (s15 + (1<<20)) >> 21; s16 += carry15; s15 -= carry15 << 21;
            carry17 = (s17 + (1<<20)) >> 21; s18 += carry17; s17 -= carry17 << 21;
            carry19 = (s19 + (1<<20)) >> 21; s20 += carry19; s19 -= carry19 << 21;
            carry21 = (s21 + (1<<20)) >> 21; s22 += carry21; s21 -= carry21 << 21;

            s11 += s23 * 666643;
            s12 += s23 * 470296;
            s13 += s23 * 654183;
            s14 -= s23 * 997805;
            s15 += s23 * 136657;
            s16 -= s23 * 683901;

            s10 += s22 * 666643;
            s11 += s22 * 470296;
            s12 += s22 * 654183;
            s13 -= s22 * 997805;
            s14 += s22 * 136657;
            s15 -= s22 * 683901;

            s9 += s21 * 666643;
            s10 += s21 * 470296;
            s11 += s21 * 654183;
            s12 -= s21 * 997805;
            s13 += s21 * 136657;
            s14 -= s21 * 683901;

            s8 += s20 * 666643;
            s9 += s20 * 470296;
            s10 += s20 * 654183;
            s11 -= s20 * 997805;
            s12 += s20 * 136657;
            s13 -= s20 * 683901;

            s7 += s19 * 666643;
            s8 += s19 * 470296;
            s9 += s19 * 654183;
            s10 -= s19 * 997805;
            s11 += s19 * 136657;
            s12 -= s19 * 683901;

            s6 += s18 * 666643;
            s7 += s18 * 470296;
            s8 += s18 * 654183;
            s9 -= s18 * 997805;
            s10 += s18 * 136657;
            s11 -= s18 * 683901;

            carry6 = (s6 + (1<<20)) >> 21; s7 += carry6; s6 -= carry6 << 21;
            carry8 = (s8 + (1<<20)) >> 21; s9 += carry8; s8 -= carry8 << 21;
            carry10 = (s10 + (1<<20)) >> 21; s11 += carry10; s10 -= carry10 << 21;
            carry12 = (s12 + (1<<20)) >> 21; s13 += carry12; s12 -= carry12 << 21;
            carry14 = (s14 + (1<<20)) >> 21; s15 += carry14; s14 -= carry14 << 21;
            carry16 = (s16 + (1<<20)) >> 21; s17 += carry16; s16 -= carry16 << 21;

            carry7 = (s7 + (1<<20)) >> 21; s8 += carry7; s7 -= carry7 << 21;
            carry9 = (s9 + (1<<20)) >> 21; s10 += carry9; s9 -= carry9 << 21;
            carry11 = (s11 + (1<<20)) >> 21; s12 += carry11; s11 -= carry11 << 21;
            carry13 = (s13 + (1<<20)) >> 21; s14 += carry13; s13 -= carry13 << 21;
            carry15 = (s15 + (1<<20)) >> 21; s16 += carry15; s15 -= carry15 << 21;

            s5 += s17 * 666643;
            s6 += s17 * 470296;
            s7 += s17 * 654183;
            s8 -= s17 * 997805;
            s9 += s17 * 136657;
            s10 -= s17 * 683901;

            s4 += s16 * 666643;
            s5 += s16 * 470296;
            s6 += s16 * 654183;
            s7 -= s16 * 997805;
            s8 += s16 * 136657;
            s9 -= s16 * 683901;

            s3 += s15 * 666643;
            s4 += s15 * 470296;
            s5 += s15 * 654183;
            s6 -= s15 * 997805;
            s7 += s15 * 136657;
            s8 -= s15 * 683901;

            s2 += s14 * 666643;
            s3 += s14 * 470296;
            s4 += s14 * 654183;
            s5 -= s14 * 997805;
            s6 += s14 * 136657;
            s7 -= s14 * 683901;

            s1 += s13 * 666643;
            s2 += s13 * 470296;
            s3 += s13 * 654183;
            s4 -= s13 * 997805;
            s5 += s13 * 136657;
            s6 -= s13 * 683901;

            s0 += s12 * 666643;
            s1 += s12 * 470296;
            s2 += s12 * 654183;
            s3 -= s12 * 997805;
            s4 += s12 * 136657;
            s5 -= s12 * 683901;
            s12 = 0;

            carry0 = (s0 + (1<<20)) >> 21; s1 += carry0; s0 -= carry0 << 21;
            carry2 = (s2 + (1<<20)) >> 21; s3 += carry2; s2 -= carry2 << 21;
            carry4 = (s4 + (1<<20)) >> 21; s5 += carry4; s4 -= carry4 << 21;
            carry6 = (s6 + (1<<20)) >> 21; s7 += carry6; s6 -= carry6 << 21;
            carry8 = (s8 + (1<<20)) >> 21; s9 += carry8; s8 -= carry8 << 21;
            carry10 = (s10 + (1<<20)) >> 21; s11 += carry10; s10 -= carry10 << 21;

            carry1 = (s1 + (1<<20)) >> 21; s2 += carry1; s1 -= carry1 << 21;
            carry3 = (s3 + (1<<20)) >> 21; s4 += carry3; s3 -= carry3 << 21;
            carry5 = (s5 + (1<<20)) >> 21; s6 += carry5; s5 -= carry5 << 21;
            carry7 = (s7 + (1<<20)) >> 21; s8 += carry7; s7 -= carry7 << 21;
            carry9 = (s9 + (1<<20)) >> 21; s10 += carry9; s9 -= carry9 << 21;
            carry11 = (s11 + (1<<20)) >> 21; s12 += carry11; s11 -= carry11 << 21;

            s0 += s12 * 666643;
            s1 += s12 * 470296;
            s2 += s12 * 654183;
            s3 -= s12 * 997805;
            s4 += s12 * 136657;
            s5 -= s12 * 683901;
            s12 = 0;

            carry0 = s0 >> 21; s1 += carry0; s0 -= carry0 << 21;
            carry1 = s1 >> 21; s2 += carry1; s1 -= carry1 << 21;
            carry2 = s2 >> 21; s3 += carry2; s2 -= carry2 << 21;
            carry3 = s3 >> 21; s4 += carry3; s3 -= carry3 << 21;
            carry4 = s4 >> 21; s5 += carry4; s4 -= carry4 << 21;
            carry5 = s5 >> 21; s6 += carry5; s5 -= carry5 << 21;
            carry6 = s6 >> 21; s7 += carry6; s6 -= carry6 << 21;
            carry7 = s7 >> 21; s8 += carry7; s7 -= carry7 << 21;
            carry8 = s8 >> 21; s9 += carry8; s8 -= carry8 << 21;
            carry9 = s9 >> 21; s10 += carry9; s9 -= carry9 << 21;
            carry10 = s10 >> 21; s11 += carry10; s10 -= carry10 << 21;
            carry11 = s11 >> 21; s12 += carry11; s11 -= carry11 << 21;

            s0 += s12 * 666643;
            s1 += s12 * 470296;
            s2 += s12 * 654183;
            s3 -= s12 * 997805;
            s4 += s12 * 136657;
            s5 -= s12 * 683901;

            carry0 = s0 >> 21; s1 += carry0; s0 -= carry0 << 21;
            carry1 = s1 >> 21; s2 += carry1; s1 -= carry1 << 21;
            carry2 = s2 >> 21; s3 += carry2; s2 -= carry2 << 21;
            carry3 = s3 >> 21; s4 += carry3; s3 -= carry3 << 21;
            carry4 = s4 >> 21; s5 += carry4; s4 -= carry4 << 21;
            carry5 = s5 >> 21; s6 += carry5; s5 -= carry5 << 21;
            carry6 = s6 >> 21; s7 += carry6; s6 -= carry6 << 21;
            carry7 = s7 >> 21; s8 += carry7; s7 -= carry7 << 21;
            carry8 = s8 >> 21; s9 += carry8; s8 -= carry8 << 21;
            carry9 = s9 >> 21; s10 += carry9; s9 -= carry9 << 21;
            carry10 = s10 >> 21; s11 += carry10; s10 -= carry10 << 21;

            s[0] = (byte)(s0 >> 0);
            s[1] = (byte)(s0 >> 8);
            s[2] = (byte)((s0 >> 16) | (s1 << 5));
            s[3] = (byte)(s1 >> 3);
            s[4] = (byte)(s1 >> 11);
            s[5] = (byte)((s1 >> 19) | (s2 << 2));
            s[6] = (byte)(s2 >> 6);
            s[7] = (byte)((s2 >> 14) | (s3 << 7));
            s[8] = (byte)(s3 >> 1);
            s[9] = (byte)(s3 >> 9);
            s[10] = (byte)((s3 >> 17) | (s4 << 4));
            s[11] = (byte)(s4 >> 4);
            s[12] = (byte)(s4 >> 12);
            s[13] = (byte)((s4 >> 20) | (s5 << 1));
            s[14] = (byte)(s5 >> 7);
            s[15] = (byte)((s5 >> 15) | (s6 << 6));
            s[16] = (byte)(s6 >> 2);
            s[17] = (byte)(s6 >> 10);
            s[18] = (byte)((s6 >> 18) | (s7 << 3));
            s[19] = (byte)(s7 >> 5);
            s[20] = (byte)(s7 >> 13);
            s[21] = (byte)(s8 >> 0);
            s[22] = (byte)(s8 >> 8);
            s[23] = (byte)((s8 >> 16) | (s9 << 5));
            s[24] = (byte)(s9 >> 3);
            s[25] = (byte)(s9 >> 11);
            s[26] = (byte)((s9 >> 19) | (s10 << 2));
            s[27] = (byte)(s10 >> 6);
            s[28] = (byte)((s10 >> 14) | (s11 << 7));
            s[29] = (byte)(s11 >> 1);
            s[30] = (byte)(s11 >> 9);
            s[31] = (byte)(s11 >> 17);
        }

        public static void sc_muladd(ref byte[] s, byte[] a, byte[] b, byte[] c)
        {
            long a0 = (long)(2097151 & load_3(a));
            long a1 = (long)(2097151 & (load_4(a, 2) >> 5));
            long a2 = (long)(2097151 & (load_3(a, 5) >> 2));
            long a3 = (long)(2097151 & (load_4(a, 7) >> 7));
            long a4 = (long)(2097151 & (load_4(a, 10) >> 4));
            long a5 = (long)(2097151 & (load_3(a, 13) >> 1));
            long a6 = (long)(2097151 & (load_4(a, 15) >> 6));
            long a7 = (long)(2097151 & (load_3(a, 18) >> 3));
            long a8 = (long)(2097151 & load_3(a, 21));
            long a9 = (long)(2097151 & (load_4(a, 23) >> 5));
            long a10 = (long)(2097151 & (load_3(a, 26) >> 2));
            long a11 = (long)((load_4(a, 28) >> 7));
            long b0 = (long)(2097151 & load_3(b));
            long b1 = (long)(2097151 & (load_4(b, 2) >> 5));
            long b2 = (long)(2097151 & (load_3(b, 5) >> 2));
            long b3 = (long)(2097151 & (load_4(b, 7) >> 7));
            long b4 = (long)(2097151 & (load_4(b, 10) >> 4));
            long b5 = (long)(2097151 & (load_3(b, 13) >> 1));
            long b6 = (long)(2097151 & (load_4(b, 15) >> 6));
            long b7 = (long)(2097151 & (load_3(b, 18) >> 3));
            long b8 = (long)(2097151 & load_3(b, 21));
            long b9 = (long)(2097151 & (load_4(b, 23) >> 5));
            long b10 = (long)(2097151 & (load_3(b, 26) >> 2));
            long b11 = (long)((load_4(b, 28) >> 7));
            long c0 = (long)(2097151 & load_3(c));
            long c1 = (long)(2097151 & (load_4(c, 2) >> 5));
            long c2 = (long)(2097151 & (load_3(c, 5) >> 2));
            long c3 = (long)(2097151 & (load_4(c, 7) >> 7));
            long c4 = (long)(2097151 & (load_4(c, 10) >> 4));
            long c5 = (long)(2097151 & (load_3(c, 13) >> 1));
            long c6 = (long)(2097151 & (load_4(c, 15) >> 6));
            long c7 = (long)(2097151 & (load_3(c, 18) >> 3));
            long c8 = (long)(2097151 & load_3(c, 21));
            long c9 = (long)(2097151 & (load_4(c, 23) >> 5));
            long c10 = (long)(2097151 & (load_3(c, 26) >> 2));
            long c11 = (long)((load_4(c, 28) >> 7));
            long s0;
            long s1;
            long s2;
            long s3;
            long s4;
            long s5;
            long s6;
            long s7;
            long s8;
            long s9;
            long s10;
            long s11;
            long s12;
            long s13;
            long s14;
            long s15;
            long s16;
            long s17;
            long s18;
            long s19;
            long s20;
            long s21;
            long s22;
            long s23;
            long carry0;
            long carry1;
            long carry2;
            long carry3;
            long carry4;
            long carry5;
            long carry6;
            long carry7;
            long carry8;
            long carry9;
            long carry10;
            long carry11;
            long carry12;
            long carry13;
            long carry14;
            long carry15;
            long carry16;
            long carry17;
            long carry18;
            long carry19;
            long carry20;
            long carry21;
            long carry22;

            s0 = c0 + a0*b0;
            s1 = c1 + (a0*b1 + a1*b0);
            s2 = c2 + (a0*b2 + a1*b1 + a2*b0);
            s3 = c3 + (a0*b3 + a1*b2 + a2*b1 + a3*b0);
            s4 = c4 + (a0*b4 + a1*b3 + a2*b2 + a3*b1 + a4*b0);
            s5 = c5 + (a0*b5 + a1*b4 + a2*b3 + a3*b2 + a4*b1 + a5*b0);
            s6 = c6 + (a0*b6 + a1*b5 + a2*b4 + a3*b3 + a4*b2 + a5*b1 + a6*b0);
            s7 = c7 + (a0*b7 + a1*b6 + a2*b5 + a3*b4 + a4*b3 + a5*b2 + a6*b1 + a7*b0);
            s8 = c8 + (a0*b8 + a1*b7 + a2*b6 + a3*b5 + a4*b4 + a5*b3 + a6*b2 + a7*b1 + a8*b0);
            s9 = c9 + (a0*b9 + a1*b8 + a2*b7 + a3*b6 + a4*b5 + a5*b4 + a6*b3 + a7*b2 + a8*b1 + a9*b0);
            s10 = c10 + (a0*b10 + a1*b9 + a2*b8 + a3*b7 + a4*b6 + a5*b5 + a6*b4 + a7*b3 + a8*b2 + a9*b1 + a10*b0);
            s11 = c11 + (a0*b11 + a1*b10 + a2*b9 + a3*b8 + a4*b7 + a5*b6 + a6*b5 + a7*b4 + a8*b3 + a9*b2 + a10*b1 + a11*b0);
            s12 = (a1*b11 + a2*b10 + a3*b9 + a4*b8 + a5*b7 + a6*b6 + a7*b5 + a8*b4 + a9*b3 + a10*b2 + a11*b1);
            s13 = (a2*b11 + a3*b10 + a4*b9 + a5*b8 + a6*b7 + a7*b6 + a8*b5 + a9*b4 + a10*b3 + a11*b2);
            s14 = (a3*b11 + a4*b10 + a5*b9 + a6*b8 + a7*b7 + a8*b6 + a9*b5 + a10*b4 + a11*b3);
            s15 = (a4*b11 + a5*b10 + a6*b9 + a7*b8 + a8*b7 + a9*b6 + a10*b5 + a11*b4);
            s16 = (a5*b11 + a6*b10 + a7*b9 + a8*b8 + a9*b7 + a10*b6 + a11*b5);
            s17 = (a6*b11 + a7*b10 + a8*b9 + a9*b8 + a10*b7 + a11*b6);
            s18 = (a7*b11 + a8*b10 + a9*b9 + a10*b8 + a11*b7);
            s19 = (a8*b11 + a9*b10 + a10*b9 + a11*b8);
            s20 = (a9*b11 + a10*b10 + a11*b9);
            s21 = (a10*b11 + a11*b10);
            s22 = a11*b11;
            s23 = 0;

            carry0 = (s0 + (1<<20)) >> 21; s1 += carry0; s0 -= carry0 << 21;
            carry2 = (s2 + (1<<20)) >> 21; s3 += carry2; s2 -= carry2 << 21;
            carry4 = (s4 + (1<<20)) >> 21; s5 += carry4; s4 -= carry4 << 21;
            carry6 = (s6 + (1<<20)) >> 21; s7 += carry6; s6 -= carry6 << 21;
            carry8 = (s8 + (1<<20)) >> 21; s9 += carry8; s8 -= carry8 << 21;
            carry10 = (s10 + (1<<20)) >> 21; s11 += carry10; s10 -= carry10 << 21;
            carry12 = (s12 + (1<<20)) >> 21; s13 += carry12; s12 -= carry12 << 21;
            carry14 = (s14 + (1<<20)) >> 21; s15 += carry14; s14 -= carry14 << 21;
            carry16 = (s16 + (1<<20)) >> 21; s17 += carry16; s16 -= carry16 << 21;
            carry18 = (s18 + (1<<20)) >> 21; s19 += carry18; s18 -= carry18 << 21;
            carry20 = (s20 + (1<<20)) >> 21; s21 += carry20; s20 -= carry20 << 21;
            carry22 = (s22 + (1<<20)) >> 21; s23 += carry22; s22 -= carry22 << 21;

            carry1 = (s1 + (1<<20)) >> 21; s2 += carry1; s1 -= carry1 << 21;
            carry3 = (s3 + (1<<20)) >> 21; s4 += carry3; s3 -= carry3 << 21;
            carry5 = (s5 + (1<<20)) >> 21; s6 += carry5; s5 -= carry5 << 21;
            carry7 = (s7 + (1<<20)) >> 21; s8 += carry7; s7 -= carry7 << 21;
            carry9 = (s9 + (1<<20)) >> 21; s10 += carry9; s9 -= carry9 << 21;
            carry11 = (s11 + (1<<20)) >> 21; s12 += carry11; s11 -= carry11 << 21;
            carry13 = (s13 + (1<<20)) >> 21; s14 += carry13; s13 -= carry13 << 21;
            carry15 = (s15 + (1<<20)) >> 21; s16 += carry15; s15 -= carry15 << 21;
            carry17 = (s17 + (1<<20)) >> 21; s18 += carry17; s17 -= carry17 << 21;
            carry19 = (s19 + (1<<20)) >> 21; s20 += carry19; s19 -= carry19 << 21;
            carry21 = (s21 + (1<<20)) >> 21; s22 += carry21; s21 -= carry21 << 21;

            s11 += s23 * 666643;
            s12 += s23 * 470296;
            s13 += s23 * 654183;
            s14 -= s23 * 997805;
            s15 += s23 * 136657;
            s16 -= s23 * 683901;

            s10 += s22 * 666643;
            s11 += s22 * 470296;
            s12 += s22 * 654183;
            s13 -= s22 * 997805;
            s14 += s22 * 136657;
            s15 -= s22 * 683901;

            s9 += s21 * 666643;
            s10 += s21 * 470296;
            s11 += s21 * 654183;
            s12 -= s21 * 997805;
            s13 += s21 * 136657;
            s14 -= s21 * 683901;

            s8 += s20 * 666643;
            s9 += s20 * 470296;
            s10 += s20 * 654183;
            s11 -= s20 * 997805;
            s12 += s20 * 136657;
            s13 -= s20 * 683901;

            s7 += s19 * 666643;
            s8 += s19 * 470296;
            s9 += s19 * 654183;
            s10 -= s19 * 997805;
            s11 += s19 * 136657;
            s12 -= s19 * 683901;

            s6 += s18 * 666643;
            s7 += s18 * 470296;
            s8 += s18 * 654183;
            s9 -= s18 * 997805;
            s10 += s18 * 136657;
            s11 -= s18 * 683901;

            carry6 = (s6 + (1<<20)) >> 21; s7 += carry6; s6 -= carry6 << 21;
            carry8 = (s8 + (1<<20)) >> 21; s9 += carry8; s8 -= carry8 << 21;
            carry10 = (s10 + (1<<20)) >> 21; s11 += carry10; s10 -= carry10 << 21;
            carry12 = (s12 + (1<<20)) >> 21; s13 += carry12; s12 -= carry12 << 21;
            carry14 = (s14 + (1<<20)) >> 21; s15 += carry14; s14 -= carry14 << 21;
            carry16 = (s16 + (1<<20)) >> 21; s17 += carry16; s16 -= carry16 << 21;

            carry7 = (s7 + (1<<20)) >> 21; s8 += carry7; s7 -= carry7 << 21;
            carry9 = (s9 + (1<<20)) >> 21; s10 += carry9; s9 -= carry9 << 21;
            carry11 = (s11 + (1<<20)) >> 21; s12 += carry11; s11 -= carry11 << 21;
            carry13 = (s13 + (1<<20)) >> 21; s14 += carry13; s13 -= carry13 << 21;
            carry15 = (s15 + (1<<20)) >> 21; s16 += carry15; s15 -= carry15 << 21;

            s5 += s17 * 666643;
            s6 += s17 * 470296;
            s7 += s17 * 654183;
            s8 -= s17 * 997805;
            s9 += s17 * 136657;
            s10 -= s17 * 683901;

            s4 += s16 * 666643;
            s5 += s16 * 470296;
            s6 += s16 * 654183;
            s7 -= s16 * 997805;
            s8 += s16 * 136657;
            s9 -= s16 * 683901;

            s3 += s15 * 666643;
            s4 += s15 * 470296;
            s5 += s15 * 654183;
            s6 -= s15 * 997805;
            s7 += s15 * 136657;
            s8 -= s15 * 683901;

            s2 += s14 * 666643;
            s3 += s14 * 470296;
            s4 += s14 * 654183;
            s5 -= s14 * 997805;
            s6 += s14 * 136657;
            s7 -= s14 * 683901;

            s1 += s13 * 666643;
            s2 += s13 * 470296;
            s3 += s13 * 654183;
            s4 -= s13 * 997805;
            s5 += s13 * 136657;
            s6 -= s13 * 683901;

            s0 += s12 * 666643;
            s1 += s12 * 470296;
            s2 += s12 * 654183;
            s3 -= s12 * 997805;
            s4 += s12 * 136657;
            s5 -= s12 * 683901;
            s12 = 0;

            carry0 = (s0 + (1<<20)) >> 21; s1 += carry0; s0 -= carry0 << 21;
            carry2 = (s2 + (1<<20)) >> 21; s3 += carry2; s2 -= carry2 << 21;
            carry4 = (s4 + (1<<20)) >> 21; s5 += carry4; s4 -= carry4 << 21;
            carry6 = (s6 + (1<<20)) >> 21; s7 += carry6; s6 -= carry6 << 21;
            carry8 = (s8 + (1<<20)) >> 21; s9 += carry8; s8 -= carry8 << 21;
            carry10 = (s10 + (1<<20)) >> 21; s11 += carry10; s10 -= carry10 << 21;

            carry1 = (s1 + (1<<20)) >> 21; s2 += carry1; s1 -= carry1 << 21;
            carry3 = (s3 + (1<<20)) >> 21; s4 += carry3; s3 -= carry3 << 21;
            carry5 = (s5 + (1<<20)) >> 21; s6 += carry5; s5 -= carry5 << 21;
            carry7 = (s7 + (1<<20)) >> 21; s8 += carry7; s7 -= carry7 << 21;
            carry9 = (s9 + (1<<20)) >> 21; s10 += carry9; s9 -= carry9 << 21;
            carry11 = (s11 + (1<<20)) >> 21; s12 += carry11; s11 -= carry11 << 21;

            s0 += s12 * 666643;
            s1 += s12 * 470296;
            s2 += s12 * 654183;
            s3 -= s12 * 997805;
            s4 += s12 * 136657;
            s5 -= s12 * 683901;
            s12 = 0;

            carry0 = s0 >> 21; s1 += carry0; s0 -= carry0 << 21;
            carry1 = s1 >> 21; s2 += carry1; s1 -= carry1 << 21;
            carry2 = s2 >> 21; s3 += carry2; s2 -= carry2 << 21;
            carry3 = s3 >> 21; s4 += carry3; s3 -= carry3 << 21;
            carry4 = s4 >> 21; s5 += carry4; s4 -= carry4 << 21;
            carry5 = s5 >> 21; s6 += carry5; s5 -= carry5 << 21;
            carry6 = s6 >> 21; s7 += carry6; s6 -= carry6 << 21;
            carry7 = s7 >> 21; s8 += carry7; s7 -= carry7 << 21;
            carry8 = s8 >> 21; s9 += carry8; s8 -= carry8 << 21;
            carry9 = s9 >> 21; s10 += carry9; s9 -= carry9 << 21;
            carry10 = s10 >> 21; s11 += carry10; s10 -= carry10 << 21;
            carry11 = s11 >> 21; s12 += carry11; s11 -= carry11 << 21;

            s0 += s12 * 666643;
            s1 += s12 * 470296;
            s2 += s12 * 654183;
            s3 -= s12 * 997805;
            s4 += s12 * 136657;
            s5 -= s12 * 683901;

            carry0 = s0 >> 21; s1 += carry0; s0 -= carry0 << 21;
            carry1 = s1 >> 21; s2 += carry1; s1 -= carry1 << 21;
            carry2 = s2 >> 21; s3 += carry2; s2 -= carry2 << 21;
            carry3 = s3 >> 21; s4 += carry3; s3 -= carry3 << 21;
            carry4 = s4 >> 21; s5 += carry4; s4 -= carry4 << 21;
            carry5 = s5 >> 21; s6 += carry5; s5 -= carry5 << 21;
            carry6 = s6 >> 21; s7 += carry6; s6 -= carry6 << 21;
            carry7 = s7 >> 21; s8 += carry7; s7 -= carry7 << 21;
            carry8 = s8 >> 21; s9 += carry8; s8 -= carry8 << 21;
            carry9 = s9 >> 21; s10 += carry9; s9 -= carry9 << 21;
            carry10 = s10 >> 21; s11 += carry10; s10 -= carry10 << 21;

            s[0] = (byte)(s0 >> 0);
            s[1] = (byte)(s0 >> 8);
            s[2] = (byte)((s0 >> 16) | (s1 << 5));
            s[3] = (byte)(s1 >> 3);
            s[4] = (byte)(s1 >> 11);
            s[5] = (byte)((s1 >> 19) | (s2 << 2));
            s[6] = (byte)(s2 >> 6);
            s[7] = (byte)((s2 >> 14) | (s3 << 7));
            s[8] = (byte)(s3 >> 1);
            s[9] = (byte)(s3 >> 9);
            s[10] = (byte)((s3 >> 17) | (s4 << 4));
            s[11] = (byte)(s4 >> 4);
            s[12] = (byte)(s4 >> 12);
            s[13] = (byte)((s4 >> 20) | (s5 << 1));
            s[14] = (byte)(s5 >> 7);
            s[15] = (byte)((s5 >> 15) | (s6 << 6));
            s[16] = (byte)(s6 >> 2);
            s[17] = (byte)(s6 >> 10);
            s[18] = (byte)((s6 >> 18) | (s7 << 3));
            s[19] = (byte)(s7 >> 5);
            s[20] = (byte)(s7 >> 13);
            s[21] = (byte)(s8 >> 0);
            s[22] = (byte)(s8 >> 8);
            s[23] = (byte)((s8 >> 16) | (s9 << 5));
            s[24] = (byte)(s9 >> 3);
            s[25] = (byte)(s9 >> 11);
            s[26] = (byte)((s9 >> 19) | (s10 << 2));
            s[27] = (byte)(s10 >> 6);
            s[28] = (byte)((s10 >> 14) | (s11 << 7));
            s[29] = (byte)(s11 >> 1);
            s[30] = (byte)(s11 >> 9);
            s[31] = (byte)(s11 >> 17);
        }

        public static long signum(long a)
        {
            return (a >> 63) - ((-a) >> 63);
        }

        public static int sc_check(byte[] s)
        {
            long s0 = (long)(load_4(s));
            long s1 = (long)(load_4(s, 4));
            long s2 = (long)(load_4(s, 8));
            long s3 = (long)(load_4(s, 12));
            long s4 = (long)(load_4(s, 16));
            long s5 = (long)(load_4(s, 20));
            long s6 = (long)(load_4(s, 24));
            long s7 = (long)(load_4(s, 28));

            return (int)((signum(1559614444 - s0)
                 + (signum(1477600026 - s1) << 1)
                 + (signum(2734136534 - s2) << 2)
                 + (signum(350157278 - s3) << 3)
                 + (signum(-s4) << 4)
                 + (signum(-s5) << 5)
                 + (signum(-s6) << 6)
                 + (signum(268435456 - s7) << 7)) >> 8);
        }

        public static int sc_isnonzero(byte[] s)
        {
            for (int i = 0; i < 32; i++)
            {
                if (s[i] != 0)
                {
                    return 1;
                }
            }

            return 0;
        }
    }
}
