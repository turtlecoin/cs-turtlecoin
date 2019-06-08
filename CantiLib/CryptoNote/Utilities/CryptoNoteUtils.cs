//
// Copyright (c) 2019 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

using System;

namespace Canti.CryptoNote
{
    internal class CryptoNoteUtils
    {
        internal static ulong GetBlockReward(NetworkConfig Globals, byte MajorVersion, uint MedianSize,
            uint BlockSize, ulong AlreadyGeneratedUnits, ulong Fee, out ulong EmissionChange)
        {
            // Check if we are at unit capacity
            if (AlreadyGeneratedUnits <= Globals.CURRENCY_TOTAL_SUPPLY)
            {
                throw new Exception("Total units has been reached");
            }

            // Calculate base reward
            ulong BaseReward = Globals.CURRENCY_GENESIS_REWARD;
            if (AlreadyGeneratedUnits > 0)
            {
                BaseReward = (Globals.CURRENCY_TOTAL_SUPPLY - AlreadyGeneratedUnits) >> Globals.CURRENCY_EMISSION_FACTOR;
            }

            // Calculate reward zone
            
            // TODO - define these in a more appropriate spot (in core they are in the config)
            uint BlockGrantedFullRewardZone = 10_000;
            if (MajorVersion == Constants.BLOCK_MAJOR_VERSION_2)
            {
                BlockGrantedFullRewardZone = 20_000;
            }
            else if (MajorVersion >= Constants.BLOCK_MAJOR_VERSION_3)
            {
                BlockGrantedFullRewardZone = 100_000;
            }
            MedianSize = Math.Max(MedianSize, BlockGrantedFullRewardZone);

            // Calculate penalties
            ulong PenalizedBaseReward = GetPenalizedAmount(BaseReward, MedianSize, BlockSize);
            ulong PenalizedFee = Fee;
            if (MajorVersion >= Constants.BLOCK_MAJOR_VERSION_2)
            {
                PenalizedFee = GetPenalizedAmount(Fee, MedianSize, BlockSize);
            }

            // Calculate emission change
            EmissionChange = PenalizedBaseReward - (Fee - PenalizedFee);

            // Return reward result
            return PenalizedBaseReward + PenalizedFee;
        }

        internal static ulong GetPenalizedAmount(ulong Amount, uint MedianSize, uint BlockSize)
        {
            // Check for correct block size
            if (BlockSize > 2 * MedianSize)
            {
                throw new Exception("Block cumulative size is too big: " + BlockSize + ", expected less than " + 2 * MedianSize);
            }

            // If amount is already 0, no processing needs to be done
            if (Amount == 0)
            {
                return 0;
            }

            // If block size is less than or equal to median size, there is no penalty
            if (BlockSize <= MedianSize)
            {
                return Amount;
            }

            // TODO - maths and calculating
            /*ulong ProductHi;
            ulong ProductLo = mul128(Amount, BlockSize * (2 * MedianSize - BlockSize), ref ProductHi);

            ulong PenalizedAmountHi;
            ulong PenalizedAmountLo;
            div128_32(ProductHi, ProductLo, (uint)MedianSize, ref PenalizedAmountHi, ref PenalizedAmountLo);
            div128_32(PenalizedAmountHi, PenalizedAmountLo, (uint)MedianSize, ref PenalizedAmountHi, ref PenalizedAmountLo);

            if (PenalizedAmountHi != 0 || PenalizedAmountLo >= Amount)
            {
                throw new Exception("Failed to calculate penalized amount");
            }

            // Return result
            return PenalizedAmountLo;*/
            return Amount;
        }
    }
}
