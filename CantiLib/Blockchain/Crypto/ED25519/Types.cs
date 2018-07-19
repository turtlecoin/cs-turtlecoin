namespace Canti.Blockchain.Crypto.ED25519
{
    /* We want to pass by reference, so lets use a class here instead of
       a struct */
    internal class ge_p2
    {
        public int[] X;
        public int[] Y;
        public int[] Z;
    }

    internal class ge_p3
    {
        public int[] X;
        public int[] Y;
        public int[] Z;
        public int[] T;
    }

    /* This has the same members as ge_p3, but a different representation
       I believe */
    internal class ge_p1p1
    {
        public int[] X;
        public int[] Y;
        public int[] Z;
        public int[] T;
    }

    internal class ge_precomp
    {
        public ge_precomp(int[] yplusx, int[] yminusx, int[] xy2d)
        {
            this.yplusx = yplusx;
            this.yminusx = yminusx;
            this.xy2d = xy2d;
        }

        public int[] yplusx;
        public int[] yminusx;
        public int[] xy2d;
    }

    internal class ge_cached
    {
        public int[] YplusX;
        public int[] YminusX;
        public int[] Z;
        public int[] T2d;
    }
}
