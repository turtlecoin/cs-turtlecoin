//
// Copyright Dan Bernstein (djb) from Ref10 in SUPERCOP
// Copyright 2012-2013 The CryptoNote Developers
// Copyright 2014-2018 The Monero Developers
// Copyright 2018 The TurtleCoin Developers
//
// Please see the included LICENSE file for more information.

namespace Canti.Cryptography.Native
{
    /* We want to pass by reference, so lets use a class here instead of
       a struct */
    public class ge_p2
    {
        public ge_p2(int[] X, int[] Y, int[] Z)
        {
            this.X = X;
            this.Y = Y;
            this.Z = Z;
        }

        public ge_p2()
        {
            X = new int[10];
            Y = new int[10];
            Z = new int[10];
        }

        public int[] X;
        public int[] Y;
        public int[] Z;
    }

    public class ge_p3
    {
        public ge_p3(int[] X, int[] Y, int[] Z, int[] T)
        {
            this.X = X;
            this.Y = Y;
            this.Z = Z;
            this.T = T;
        }

        public ge_p3()
        {
            X = new int[10];
            Y = new int[10];
            Z = new int[10];
            T = new int[10];
        }

        public int[] X;
        public int[] Y;
        public int[] Z;
        public int[] T;
    }

    /* This has the same members as ge_p3, but a different representation
       I believe */
    public class ge_p1p1
    {
        public ge_p1p1(int[] X, int[] Y, int[] Z, int[] T)
        {
            this.X = X;
            this.Y = Y;
            this.Z = Z;
            this.T = T;
        }

        public ge_p1p1()
        {
            X = new int[10];
            Y = new int[10];
            Z = new int[10];
            T = new int[10];
        }

        public int[] X;
        public int[] Y;
        public int[] Z;
        public int[] T;
    }

    public class ge_precomp
    {
        public ge_precomp(int[] yplusx, int[] yminusx, int[] xy2d)
        {
            this.yplusx = yplusx;
            this.yminusx = yminusx;
            this.xy2d = xy2d;
        }

        public ge_precomp()
        {
            yplusx = new int[10];
            yminusx = new int[10];
            xy2d = new int[10];
        }

        public int[] yplusx;
        public int[] yminusx;
        public int[] xy2d;
    }

    public class ge_cached
    {
        public ge_cached(int[] YplusX, int[] YminusX, int[] Z, int[] T2d)
        {
            this.YplusX = YplusX;
            this.YminusX = YminusX;
            this.Z = Z;
            this.T2d = T2d;
        }

        public ge_cached()
        {
            YplusX = new int[10];
            YminusX = new int[10];
            Z = new int[10];
            T2d = new int[10];
        }

        public int[] YplusX;
        public int[] YminusX;
        public int[] Z;
        public int[] T2d;
    }

    public class ge_dsmp
    {
        public ge_cached[] ge_cached;

        public ge_cached this[int index]
        {
            get
            {
                return ge_cached[index];
            }
            set
            {
                ge_cached[index] = value;
            }
        }

        public ge_dsmp()
        {
            ge_cached = new ge_cached[]
            {
                new ge_cached(),
                new ge_cached(),
                new ge_cached(),
                new ge_cached(),
                new ge_cached(),
                new ge_cached(),
                new ge_cached(),
                new ge_cached()
            };
        }
    }
}
