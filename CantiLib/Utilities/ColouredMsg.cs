//
// Copyright (c) 2018, The TurtleCoin Developers
//
// Please see the included LICENSE file for more information.

using System;

namespace Canti.Utilities
{
    public class ColouredMsg
    {
        public static void Write(ConsoleColor colour, string msg)
        {
            Console.ForegroundColor = colour;
            Console.Write(msg);
            Console.ResetColor();
        }

        public static void WriteLine(ConsoleColor colour, string msg)
        {
            Console.ForegroundColor = colour;
            Console.WriteLine(msg);
            Console.ResetColor();
        }
    }

    public class GreenMsg : ColouredMsg
    {
        public static void Write(string msg)
        {
            Write(ConsoleColor.DarkGreen, msg);
        }

        public static void WriteLine(string msg)
        {
            WriteLine(ConsoleColor.DarkGreen, msg);
        }
    }

    public class YellowMsg : ColouredMsg
    {
        public static void Write(string msg)
        {
            Write(ConsoleColor.Yellow, msg);
        }

        public static void WriteLine(string msg)
        {
            WriteLine(ConsoleColor.Yellow, msg);
        }
    }

    public class RedMsg : ColouredMsg
    {
        public static void Write(string msg)
        {
            Write(ConsoleColor.Red, msg);
        }

        public static void WriteLine(string msg)
        {
            WriteLine(ConsoleColor.Red, msg);
        }
    }
}
