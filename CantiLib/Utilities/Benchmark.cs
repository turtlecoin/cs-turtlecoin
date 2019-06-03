//
// Copyright (c) 2019 Canti, The TurtleCoin Developers
//
// Please see the included LICENSE file for more information.

using System;
using System.Diagnostics;

namespace Canti
{
    /// <summary>
    /// Contains basic benchmarking tools
    /// </summary>
    public static class Benchmark
    {
        /// <summary>
        /// Runs a delegate function a number of times
        /// </summary>
        /// <param name="Func">A delegate function to test</param>
        /// <param name="Iterations">The amount of times to invoke the given delegate</param>
        /// <returns>A Timespan representing the amount of time it took the delegate iterations to finish</returns>
        public static TimeSpan Run(Action Func, int Iterations)
        {
            Stopwatch Timer = new Stopwatch();
            Timer.Restart();
            for (int i = 0; i < Iterations; i++) Func.Invoke();
            Timer.Stop();
            return Timer.Elapsed;
        }
    }
}
