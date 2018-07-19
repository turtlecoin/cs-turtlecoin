//
// Original code modified by The CryptoNote Developers from SUPERCOP ref10
// Copyright 2012-2013 The CryptoNote Developers
// Copyright 2014-2018 The Monero Developers
// Copyright 2018 The TurtleCoin Developers
//
// Please see the included LICENSE file for more information.

using System;

/* Here be dragons */
namespace Canti.Blockchain.Crypto.ED25519
{
    internal static class ED25519
    {
        /* This calculates (input[0]) + (input[1] * 2^8) + (input[2] * 2^16)
           Takes an optional offset to simulate passing an offseted pointer */
        static ulong load_3(byte[] input, int offset = 0)
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
        static ulong load_4(byte[] input, int offset = 0)
        {
            ulong result;
            result = (ulong)input[0 + offset];
            result |= (ulong)input[1 + offset] << 8;
            result |= (ulong)input[2 + offset] << 16;
            result |= (ulong)input[3 + offset] << 24;
            return result;
        }

        /* Zeroes an fe (int[10]) */
        static void fe_0(int[] x)
        {
            for (int i = 0; i < 10; i++)
            {
                x[i] = 0;
            }
        }

        /* Sets the first value of an fe (int[10]) to 1, and zeroes the next
           nine */
        static void fe_1(int[] x)
        {
            x[0] = 1;

            for (int i = 1; i < 10; i++)
            {
                x[i] = 0;
            }
        }

        /* Adds two fe's (int[10]) together and stores the result in h */
        static void fe_add(int[] h, int[] f, int[] g)
        {
            for (int i = 0; i < 10; i++)
            {
                h[i] = f[i] + g[i];
            }
        }

        /* If b == 0, do nothing. If b == 1, replace f with g */
        static void fe_cmov(int[] f, int[] g, uint b)
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
        static void fe_copy(int[] h, int[] f)
        {
            for (int i = 0; i < 10; i++)
            {
                h[i] = f[i];
            }
        }

        static void fe_invert(int[] ret, int[] z)
        {
            int[] t0 = new int[10];
            int[] t1 = new int[10];
            int[] t2 = new int[10];
            int[] t3 = new int[10];

            fe_sq(t0, z);
            fe_sq(t1, t0);
            fe_sq(t1, t1);

            fe_mul(t1, z, t1);
            fe_mul(t0, t0, t1);

            fe_sq(t2, t0);

            fe_mul(t1, t1, t2);

            fe_sq(t2, t1);

            for (int i = 0; i < 4; ++i)
            {
                fe_sq(t2, t2);
            }

            fe_mul(t1, t2, t1);
            fe_sq(t2, t1);

            for (int i = 0; i < 9; ++i)
            {
                fe_sq(t2, t2);
            }

            fe_mul(t2, t2, t1);

            fe_sq(t3, t2);

            for (int i = 0; i < 19; ++i)
            {
                fe_sq(t3, t3);
            }

            fe_mul(t2, t3, t2);

            fe_sq(t2, t2);

            for (int i = 0; i < 9; ++i)
            {
                fe_sq(t2, t2);
            }

            fe_mul(t1, t2, t1);

            fe_sq(t2, t1);

            for (int i = 0; i < 49; ++i)
            {
                fe_sq(t2, t2);
            }

            fe_mul(t2, t2, t1);

            fe_sq(t3, t2);

            for (int i = 0; i < 99; ++i)
            {
                fe_sq(t3, t3);
            }

            fe_mul(t2, t3, t2);

            fe_sq(t2, t2);

            for (int i = 0; i < 49; ++i)
            {
                fe_sq(t2, t2);
            }

            fe_mul(t1, t2, t1);
            fe_sq(t1, t1);

            for (int i = 0; i < 4; ++i)
            {
                fe_sq(t1, t1);
            }

            fe_mul(ret, t1, t0);
        }

        /* Return 1 if f is in {1,3,5,....,q-2}
           Return 0 if f is in {0,2,4,...,q-1} */
        static int fe_isnegative(int[] f)
        {
            byte[] s = new byte[32];
            fe_tobytes(s, f);

            /* Is it odd? */
            return s[0] & 1;
        }

        /* Is every element of fe_tobytes(f) zero? */
        static bool fe_isnonzero(int[] f)
        {
            byte[] s = new byte[32];
            fe_tobytes(s, f);

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
        static void fe_mul(int[] h, int[] f, int[] g)
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
        static void fe_neg(int[] h, int[] f)
        {
            for (int i = 0; i < 10; i++)
            {
                h[i] = -f[i];
            }
        }

        /* h = f * f
           Same disclaimer as fe_mul applies. */
        static void fe_sq(int[] h, int[] f)
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
        static void fe_sq2(int[] h, int[] f)
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
        static void fe_sub(int[] h, int[] f, int[] g)
        {
            for (int i = 0; i < 10; i++)
            {
                h[i] = f[i] - g[i];
            }
        }

        static void fe_tobytes(byte[] s, int[] h)
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

        static void ge_add(ge_p1p1 r, ge_p3 p, ge_cached q)
        {
            int[] t0 = new int[10];

            fe_add(r.X, p.Y, p.X);
            fe_sub(r.Y, p.Y, p.X);
            fe_mul(r.Z, r.X, q.YplusX);
            fe_mul(r.Y, r.Y, q.YminusX);
            fe_mul(r.T, q.T2d, p.T);
            fe_mul(r.X, p.Z, q.Z);
            fe_add(t0, r.X, r.X);
            fe_sub(r.X, r.Z, r.Y);
            fe_add(r.Y, r.Z, r.Y);
            fe_add(r.Z, t0, r.T);
            fe_sub(r.T, t0, r.T);
        }

        static void slide(sbyte[] r, byte[] a)
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

        static void ge_dsm_precomp(ge_cached[] r, ge_p3 s)
        {
            ge_p1p1 t = null;
            ge_p3 s2 = null;
            ge_p3 u = null;

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

        static void ge_double_scalarmult_base_vartime(ge_p2 r, byte[] a, ge_p3 A, byte[] b)
        {
            sbyte[] aslide = new sbyte[256];
            sbyte[] bslide = new sbyte[256];

            ge_cached[] Ai = new ge_cached[8]; /* A, 3A, 5A, 7A, 9A, 11A, 13A, 15A */

            ge_p1p1 t = null;
            ge_p3 u = null;

            int i;

            slide(aslide, a);
            slide(bslide, b);
            ge_dsm_precomp(Ai, A);

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
                    ge_madd(t, u, ge_Bi[bslide[i]/2]);
                }
                else if (bslide[i] < 0)
                {
                    ge_p1p1_to_p3(u, t);
                    ge_msub(t, u, ge_Bi[(-bslide[i])/2]);
                }

                ge_p1p1_to_p2(r, t);
            }
        }

        static void ge_double_scalarmult_base_vartime_p3(ge_p3 r3, byte[] a, ge_p3 A, byte[] b)
        {
            sbyte[] aslide = new sbyte[256];
            sbyte[] bslide = new sbyte[256];

            ge_cached[] Ai = new ge_cached[8]; /* A, 3A, 5A, 7A, 9A, 11A, 13A, 15A */

            ge_p1p1 t = null;
            ge_p3 u = null;
            ge_p2 r = null;

            int i;

            slide(aslide, a);
            slide(bslide, b);
            ge_dsm_precomp(Ai, A);

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
                    ge_madd(t, u, ge_Bi[bslide[i]/2]);
                }
                else if (bslide[i] < 0)
                {
                    ge_p1p1_to_p3(u, t);
                    ge_msub(t, u, ge_Bi[(-bslide[i])/2]);
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

        static int ge_frombytes_vartime(ge_p3 h, byte[] s)
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

            fe_1(h.Z);
            fe_sq(u, h.Y);
            fe_mul(v, u, Constants.fe_d);
            fe_sub(u, u, h.Z);       /* u = y^2-1 */
            fe_add(v, v, h.Z);       /* v = dy^2+1 */

            fe_divpowm1(h.X, u, v); /* x = uv^3(uv^7)^((q-5)/8) */

            fe_sq(vxx, h.X);
            fe_mul(vxx, vxx, v);
            fe_sub(check, vxx, u);    /* vx^2-u */

            if (fe_isnonzero(check))
            {
                fe_add(check, vxx, u);  /* vx^2+u */

                if (fe_isnonzero(check))
                {
                    return -1;
                }

                fe_mul(h.X, h.X, Constants.fe_sqrtm1);
            }

            if (fe_isnegative(h.X) != (s[31] >> 7))
            {
                /* If x = 0, the sign must be positive */
                if (!fe_isnonzero(h.X))
                {
                    return -1;
                }

                fe_neg(h.X, h.X);
            }

            fe_mul(h.T, h.X, h.Y);

            return 0;
        }

        /* r = p + q */
        static void ge_madd(ge_p1p1 r, ge_p3 p, ge_precomp q)
        {
            int[] t0 = new int[10];

            fe_add(r.X, p.Y, p.X);
            fe_sub(r.Y, p.Y, p.X);
            fe_mul(r.Z, r.X, q.yplusx);
            fe_mul(r.Y, r.Y, q.yminusx);
            fe_mul(r.T, q.xy2d, p.T);
            fe_add(t0, p.Z, p.Z);
            fe_sub(r.X, r.Z, r.Y);
            fe_add(r.Y, r.Z, r.Y);
            fe_add(r.Z, t0, r.T);
            fe_sub(r.T, t0, r.T);
        }

        /* r = p - q */
        static void ge_msub(ge_p1p1 r, ge_p3 p, ge_precomp q)
        {
            int[] t0 = new int[10];

            fe_add(r.X, p.Y, p.X);
            fe_sub(r.Y, p.Y, p.X);
            fe_mul(r.Z, r.X, q.yminusx);
            fe_mul(r.Y, r.Y, q.yplusx);
            fe_mul(r.T, q.xy2d, p.T);
            fe_add(t0, p.Z, p.Z);
            fe_sub(r.X, r.Z, r.Y);
            fe_add(r.Y, r.Z, r.Y);
            fe_sub(r.Z, t0, r.T);
            fe_add(r.T, t0, r.T);
        }

        /* r = p */
        static void ge_p1p1_to_p2(ge_p2 r, ge_p1p1 p)
        {
            fe_mul(r.X, p.X, p.T);
            fe_mul(r.Y, p.Y, p.Z);
            fe_mul(r.Z, p.Z, p.T);
        }

        /* r = p */
        static void ge_p1p1_to_p3(ge_p3 r, ge_p1p1 p)
        {
            fe_mul(r.X, p.X, p.T);
            fe_mul(r.Y, p.Y, p.Z);
            fe_mul(r.Z, p.Z, p.T);
            fe_mul(r.T, p.X, p.Y);
        }

        static void ge_p2_0(ge_p2 h)
        {
            fe_0(h.X);
            fe_1(h.Y);
            fe_1(h.Z);
        }

        /* r = 2 * p */
        static void ge_p2_dbl(ge_p1p1 r, ge_p2 p)
        {
            int[] t0 = new int[10];

            fe_sq(r.X, p.X);
            fe_sq(r.Z, p.Y);
            fe_sq2(r.T, p.Z);
            fe_add(r.Y, p.X, p.Y);
            fe_sq(t0, r.Y);
            fe_add(r.Y, r.Z, r.X);
            fe_sub(r.Z, r.Z, r.X);
            fe_sub(r.X, t0, r.Y);
            fe_sub(r.T, r.T, r.Z);
        }

        static void ge_p3_0(ge_p3 h)
        {
            fe_0(h.X);
            fe_1(h.Y);
            fe_1(h.Z);
            fe_0(h.T);
        }

        /* r = 2 * p */
        static void ge_p3_dbl(ge_p1p1 r, ge_p3 p)
        {
            ge_p2 q = null;
            ge_p3_to_p2(q, p);
            ge_p2_dbl(r, q);
        }

        /* r = p */
        static void ge_p3_to_cached(ge_cached r, ge_p3 p)
        {
            fe_add(r.YplusX, p.Y, p.X);
            fe_sub(r.YminusX, p.Y, p.X);
            fe_copy(r.Z, p.Z);
            fe_mul(r.T2d, p.T, Constants.fe_d2);
        }
    }
}
