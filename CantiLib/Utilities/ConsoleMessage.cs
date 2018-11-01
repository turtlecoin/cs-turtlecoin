//
// Copyright (c) 2018, The TurtleCoin Developers
//
// Please see the included LICENSE file for more information.

using System;

namespace Canti.Utilities
{
    public static class ConsoleMessage
    {
        public static readonly ConsoleColor DefaultColor = ConsoleColor.White;        
        

        public static void Write(string msg)
        {
            Write(ConsoleColor.White, msg);
        }

        public static void Write(ConsoleColor color, string msg, LogLevel logLevel)
        {
            Write(color, msg);

            Logger.Log(logLevel, "ConsoleMessage", msg);
        }

        public static void Write(ConsoleColor color, string msg)
        {
            Console.ForegroundColor = color;
            Console.Write(msg);
            Console.ResetColor();
        }


        public static void WriteLine(string msg)
        {
            WriteLine(DefaultColor, msg);
        }

        public static void WriteLine(ConsoleColor color, string msg, LogLevel logLevel)
        {
            WriteLine(color, msg);

            Logger.Log(logLevel, "ConsoleMessage", msg);
        }

        public static void WriteLine(ConsoleColor color, string msg)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(msg);
            Console.ResetColor();
        }
    }
}