//
// Copyright 2011 Hongjun Wu
// Copyright 2018 The TurtleCoin Developers
//
// Please see the included LICENSE file for more information.

using System;

/* JH-256, post NIST modifications (42 rounds) */
namespace Canti.Cryptography.Native
{
    public partial class JH : IHashProvider
    {
        public byte[] Hash(byte[] input)
        {
            HashState state = new HashState();

            Init(state);

            Update(state, input);

            return Final(state);
        }

        /* Before hashing a message, initialize the hash state as H0 */
        private static void Init(HashState state)
        {
            state.H[0] = 1;
            F8(state);
        }

        /* Hash each 512-bit message block, except the last partial block */
        private static void Update(HashState state, byte[] input)
        {
            ulong dataBitLen = (ulong)input.Length * 8;

            state.dataBitLen += dataBitLen;

            /* The starting address for the data to be compressed */
            ulong index = 0;

            /* If there is remaining data in the buffer, fill it to a full
               message block first */

            /* We assume that the size of the data in the buffer is the
               multiple of 8 bits if it is not at the end of a message */

            /* There is data in the buffer, but the incoming data is
               insufficient for a full block */
            if ((state.dataInBuffer > 0) &&
               ((state.dataInBuffer + dataBitLen) < 512))
            {
                int plus = 0;

                if ((dataBitLen & 7) != 0)
                {
                    plus = 1;
                }

                /* Copy (64 - dataInBuffer >> 3) bytes + plus from input to
                   state.buffer at an offset of dataInBuffer >> 3 in buffer */
                Buffer.BlockCopy(input, 0, state.buffer,
                                ((int)(state.dataInBuffer >> 3)),
                                ((int)(64 - (state.dataInBuffer >> 3)) + plus));

                state.dataInBuffer += dataBitLen;
                dataBitLen = 0;
            }

            /* There is data in the buffer, and the incoming data is
               sufficient for a full block */
            if ((state.dataInBuffer > 0) &&
               ((state.dataInBuffer + dataBitLen) >= 512))
            {
                /* Copy (64 - dataInBuffer >> 3) bytes from input to
                   state.buffer at an offset of dataInBuffer >> 3 in buffer */
                Buffer.BlockCopy(input, 0, state.buffer,
                                (int)(state.dataInBuffer >> 3),
                                (int)(64 - (state.dataInBuffer >> 3)));

                index = 64 - (state.dataInBuffer >> 3);

                dataBitLen -= (512 - state.dataInBuffer);

                F8(state);

                state.dataInBuffer = 0;
            }

            /* Hash the remaining full message blocks */
            for (; dataBitLen >= 512; index += 64, dataBitLen -= 512)
            {
                /* Copy 64 bytes from input to buffer at an offset of index */
                Buffer.BlockCopy(input, (int)index, state.buffer, 0, 64);
                F8(state);
            }

            /* Store the partial block into buffer, assume that if part of the
               last byte is not part of the message, then that part consists
               of zero bits */
            if (dataBitLen > 0)
            {
                int plus = 0;

                if ((dataBitLen & 7) != 0)
                {
                    plus = 1;
                }

                /* Copy (dataBitLen & 0x1ff >> 3 + plus) bytes from input to
                   state.buffer, at an offset of index from input */
                Buffer.BlockCopy(input, (int)index, state.buffer, 0,
                                (int)((dataBitLen & 0x1ff) >> 3) + plus);

                state.dataInBuffer = dataBitLen;
            }
        }

        private static byte[] Final(HashState state)
        {
            /* Pad the message when dataBitLen is a multiple of 512 bits,
               then process the padded block */
            if ((state.dataBitLen & 0x1ff) == 0)
            {
                FinalizeBuffer(state, true);
            }
            else
            {
                int index = (int)(state.dataBitLen & 0x1ff) >> 3;

                int offset = index;
                
                if ((state.dataInBuffer & 7) != 0)
                {
                    offset++;
                }

                /* Set the rest of the buffer to zero */
                Array.Clear(state.buffer, offset, state.buffer.Length - offset);

                /* Pad and process the partial block when databitlen is not
                   a multiple of 512 bits, then hash the padded blocks*/
                state.buffer[index] |= (byte)(1 << (int)(7- (state.dataBitLen & 7)));

                F8(state);

                FinalizeBuffer(state, false);
            }

            byte[] output = new byte[32];

            /* Copy 32 bytes from state.H to output, at an offset of 96 in H */
            Buffer.BlockCopy(state.H, 96, output, 0, 32);

            return output;
        }

        private static void FinalizeBuffer(HashState state, bool zeroInitial)
        {
            /* Zero buffer */
            Array.Clear(state.buffer, 0, state.buffer.Length);

            if (zeroInitial)
            {
                state.buffer[0] = 0x80;
            }

            for (int i = 0; i < 8; i++)
            {
                state.buffer[63 - i] = (byte)((state.dataBitLen >> (i * 8)) & 0xff);
            }

            F8(state);
        }

        /* Compression function F8 */
        private static void F8(HashState state)
        {
            /* XOR the message with the first half of H */
            for (int i = 0; i < 64; i++)
            {
                state.H[i] ^= state.buffer[i];
            }
            
            /* Bijective function E8 */
            E8(state);

            /* XOR the message with the last half of H */
            for (int i = 0; i < 64; i++)
            {
                state.H[i + 64] ^= state.buffer[i];
            }
        }

        private static void E8(HashState state)
        {
            /* Initialize the round constant */
            for (int i = 0; i < 64; i++)
            {
                state.roundConstant[i] = RoundConstantZero[i];
            }

            /* Initial group at the beginning of E8, group the H value into
               4-bit elements and store them in A */
            E8InitialGroup(state);

            /* 42 Rounds */
            for (int i = 0; i < 42; i++)
            {
                R8(state);
                UpdateRoundConstant(state);
            }

            /* Degroup at the end of E8: decompose the 4-bit elements of A into
               the 1024 bit H */
            E8FinalDegroup(state);
        }

        private static void E8InitialGroup(HashState state)
        {
            byte[] tmp = new byte[256];

            for (int i = 0; i < 256; i++)
            {
                /*t0 is the i-th bit of H, i = 0, 1, 2, 3, ... , 127*/
                byte t0 = (byte)((state.H[(i      ) >> 3] >> (7 - (i & 7))) & 1);

                /*t1 is the (i+256)-th bit of H*/
                byte t1 = (byte)((state.H[(i + 256) >> 3] >> (7 - (i & 7))) & 1);

                /*t2 is the (i+512)-th bit of H*/
                byte t2 = (byte)((state.H[(i + 512) >> 3] >> (7 - (i & 7))) & 1);

                /*t3 is the (i+768)-th bit of H*/
                byte t3 = (byte)((state.H[(i + 768) >> 3] >> (7 - (i & 7))) & 1);

                tmp[i] = (byte)((t0 << 3) | (t1 << 2) | (t2 << 1) | (t3 << 0));
            }

            /* Padding the odd and even elements separately */
            for (int i = 0; i < 128; i++)
            {
                state.A[i << 1] = tmp[i];
                state.A[(i << 1) + 1] = tmp[i + 128];
            }
        }

        private static void R8(HashState state)
        {
            byte[] tmp = new byte[256];

            /* The round constant expanded into 256 1-bit elements */
            byte[] roundConstantExpanded = new byte[256];

            /* Expand the round constant into 256 1-bit elements */
            for (int i = 0; i < 256; i++)
            {
                roundConstantExpanded[i] = (byte)((state.roundConstant[i >> 2] >> (3 - (i & 3))) & 1);
            }

            /* Sbox layer, each constant bit selects one Sbox from S0 and S1 */
            for (int i = 0; i < 256; i++)
            {
                /* Constant bits are used to determine which Sbox to use */
                tmp[i] = S[roundConstantExpanded[i], state.A[i]];
            }

            /* MDS Layer */
            for (int i = 0; i < 256; i += 2)
            {
                L(ref tmp[i], ref tmp[i+1]);
            }

            /* The following is the permutation layer P_8 */

            /* Initial swap Pi_8 */
            for (int i = 0; i < 256; i += 4)
            {
                byte t = tmp[i + 2];
                tmp[i + 2] = tmp[i + 3];
                tmp[i + 3] = t;
            }

            /* Permutation P'_8 */
            for (int i = 0; i < 128; i++)
            {
                state.A[i] = tmp[i << 1];
                state.A[i + 128] = tmp[(i << 1) + 1];
            }

            /* Final swap Phi_8 */
            for (int i = 128; i < 256; i += 2)
            {
                byte t = state.A[i];
                state.A[i] = state.A[i+1];
                state.A[i+1] = t;
            }
        }
        
        private static void UpdateRoundConstant(HashState state)
        {
            byte[] tmp = new byte[64];

            /* Sbox layer */
            for (int i = 0; i < 64; i++)
            {
                tmp[i] = S[0, state.roundConstant[i]];
            }

            /* MDS layer */
            for (int i = 0; i < 64; i += 2)
            {
                L(ref tmp[i], ref tmp[i+1]);
            }

            /* The following is the permutation layer P_6 */

            /* Initial swap Pi_6 */
            for (int i = 0; i < 64; i += 4)
            {
                byte t = tmp[i + 2];
                tmp[i + 2] = tmp[i + 3];
                tmp[i + 3] = t;
            }

            /* Permutation P'_6 */
            for (int i = 0; i < 32; i++)
            {
                state.roundConstant[i] = tmp[i << 1];
                state.roundConstant[i + 32] = tmp [(i << 1) + 1];
            }

            /* Final swap Phi_6 */
            for (int i = 32; i < 64; i += 2)
            {
                byte t = state.roundConstant[i];
                state.roundConstant[i] = state.roundConstant[i + 1];
                state.roundConstant[i + 1] = t;
            }
        }

        private static void L(ref byte a, ref byte b)
        {
            b ^= (byte)(((a << 1) ^ (a >> 3) ^ ((a >> 2) & 2)) & 0xf);
            a ^= (byte)(((b << 1) ^ (b >> 3) ^ ((b >> 2) & 2)) & 0xf);
        }

        /* Degroup at the end of E8: it is the inverse of E8InitialGroup.
           The 256 4-bit elements in state.A are degrouped into the 1024-bit
           state.H */
        private static void E8FinalDegroup(HashState state)
        {
            byte[] tmp = new byte[256];

            for (int i = 0; i < 128; i++)
            {
                tmp[i] = state.A[i << 1];
                tmp[i + 128] = state.A[(i << 1) + 1];
            }

            /* Zero out the array H */
            Array.Clear(state.H, 0, state.H.Length);

            for (int i = 0; i < 256; i++)
            {
                byte t0 = (byte)((tmp[i] >> 3) & 1);
                byte t1 = (byte)((tmp[i] >> 2) & 1);
                byte t2 = (byte)((tmp[i] >> 1) & 1);
                byte t3 = (byte)((tmp[i] >> 0) & 1);

                state.H[ i       >> 3] |= (byte)(t0 << (7 - (i & 7)));
                state.H[(i + 256)>> 3] |= (byte)(t1 << (7 - (i & 7)));
                state.H[(i + 512)>> 3] |= (byte)(t2 << (7 - (i & 7)));
                state.H[(i + 768)>> 3] |= (byte)(t3 << (7 - (i & 7)));
            }
        }

        private class HashState
        {
            /* The hash value H */
            public byte[] H = new byte[128];

            /* The temporary round value */
            public byte[] A = new byte[256];

            /* The round constant for one round */
            public byte[] roundConstant = new byte[64];

            /* The message block to be hashed */
            public byte[] buffer = new byte[64];

            /* The size of the message remaining in the buffer */
            public ulong dataInBuffer = 0;

            /* The message size in bits */
            public ulong dataBitLen = 0;
        }
    }
}
