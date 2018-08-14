//
// Copyright (c) 2018, The TurtleCoin Developers
//
// Please see the included LICENSE file for more information.

using System;

namespace Canti.Utilities
{
    /// <summary>
    ///     Pattern matching interface for LHS of <see cref="IEither{TLeft,TRight}" />
    /// </summary>
    public interface ILeft<out TLeft>
    {
        TLeft Value { get; }
    }

    /// <summary>
    ///     Pattern matching interface for RHS of <see cref="IEither{TLeft,TRight}" />
    /// </summary>
    public interface IRight<out TRight>
    {
        TRight Value { get; }
    }

    /// <summary>
    ///     Either monad converted to non-functional C# idioms
    /// </summary>
    public interface IEither<TLeft, TRight>
    {
        TReturn Select<TReturn>(Func<TLeft, TReturn> ofLeft, Func<TRight, TReturn> ofRight);

        void Do(Action<TLeft> ofLeft, Action<TRight> ofRight);

        IEither<TLeft, TReturn> Fmap<TReturn>(Func<TRight, IEither<TLeft, TReturn>> f);

        bool IsLeft();

        /// <summary>
        ///     Provides the left value
        /// </summary>
        /// <exception cref="InvalidOperationException">if this is a right value</exception>
        TLeft Left();

        /// <summary>
        ///     Provides the right value
        /// </summary>
        /// <exception cref="InvalidOperationException">if this is a left value</exception>
        TRight Right();
    }

    /// <summary>
    ///     Static helper class for Either
    /// </summary>
    public static class Either
    {
        private struct LeftImpl<TLeft, TRight> : IEither<TLeft, TRight>, ILeft<TLeft>
        {
            public TLeft Value { get; }
            public TLeft Left() => Value;
            public TRight Right() => throw new InvalidOperationException();

            public LeftImpl(TLeft value)
            {
                Value = value;
            }

            public TReturn Select<TReturn>(Func<TLeft, TReturn> ofLeft, Func<TRight, TReturn> ofRight)
            {
                if (ofLeft == null)
                    throw new ArgumentNullException(nameof(ofLeft));

                return ofLeft(Value);
            }

            /* Fmap on a left is a no-op, just return the left value, wrapped
               in an either. The Right type of the new either becomes TReturn
               instead of TRight */
            public IEither<TLeft, TReturn> Fmap<TReturn>(Func<TRight, IEither<TLeft, TReturn>> f)
            {
                return Either.Left<TLeft, TReturn>(Value);
            }

            public bool IsLeft()
            {
                return true;
            }

            public void Do(Action<TLeft> ofLeft, Action<TRight> ofRight)
            {
                if (ofLeft == null)
                    throw new ArgumentNullException(nameof(ofLeft));

                ofLeft(Value);
            }
        }

        private struct RightImpl<TLeft, TRight> : IEither<TLeft, TRight>, IRight<TRight>
        {
            public TRight Value { get; }
            public TLeft Left() => throw new InvalidOperationException();
            public TRight Right() => Value;

            public RightImpl(TRight value)
            {
                Value = value;
            }

            public TReturn Select<TReturn>(Func<TLeft, TReturn> ofLeft, Func<TRight, TReturn> ofRight)
            {
                if (ofRight == null)
                    throw new ArgumentNullException(nameof(ofRight));

                return ofRight(Value);
            }

            /* If it's a right, apply the function to the item within */
            public IEither<TLeft, TReturn> Fmap<TReturn>(Func<TRight, IEither<TLeft, TReturn>> f)
            {
                return f(Value);
            }

            public bool IsLeft()
            {
                return false;
            }

            public void Do(Action<TLeft> ofLeft, Action<TRight> ofRight)
            {
                if (ofRight == null)
                    throw new ArgumentNullException(nameof(ofRight));

                ofRight(Value);
            }
        }

        /// <summary>
        ///     Create an Either with Left value
        /// </summary>
        public static IEither<TLeft, TRight> Left<TLeft, TRight>(TLeft value)
        {
            return new LeftImpl<TLeft, TRight>(value);
        }

        /// <summary>
        ///     Create an Either with Right value
        /// </summary>
        public static IEither<TLeft, TRight> Right<TLeft, TRight>(TRight value)
        {
            return new RightImpl<TLeft, TRight>(value);
        }
    }
}
