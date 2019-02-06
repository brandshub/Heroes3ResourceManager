using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace h3magic
{
    public class HeroesSection
    {

        //private static readonly byte[] firstHero = { 7, 0, 0, 0, 0, 0, 0, 0, 6, 0, 0, 0, 1, 0, 0, 0 };
        private static readonly byte[] firstHero = { 0x2e, 0x70, 0x63, 0x78, 0x00, 0x00, 0x00, 0x00, 0x25, 0x64, 0x2f, 0x25, 0x64, 0x00, 0x00, 0x00 };
        //private static readonly byte[] heroSpecs = { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 2, 0, 0, 0 };
        private static readonly byte[] heroSpecs = { 0x47, 0x7a, 0x49, 0x6e, 0x66, 0x6c, 0x61, 0x74, 0x65, 0x42, 0x75, 0x66, 0x40, 0x40, 0x00, 0x00 };


        public static long HeroOffset1 { get; private set; }
        public static long HeroOffset2 { get; private set; }

        public static long FindHeroOffset1(byte[] data)
        {
            HeroOffset1 = FindPosition(data, firstHero) + 16;
            return HeroOffset1;
        }

        public static long FindHeroOffset2(byte[] data)
        {
            HeroOffset2 = FindPosition(data, heroSpecs) + 16;
            return HeroOffset2;
        }

        public static unsafe long FindPosition(byte[] bytes, byte[] search)
        {
            if (search.Length > bytes.Length)
                return -1;


            int limit = search.Length >> 2;
            int ub = bytes.Length - search.Length;

            fixed (byte* fh = search)
            fixed (byte* bts = bytes)
            {
                int* fb = (int*)fh;

                for (int i = 0x270000; i < ub; i++)
                {
                    int* ip = (int*)(bts + i);
                    if (*ip == *fb)
                    {
                        bool found = true;
                        for (int j = 1; j < limit; j++)
                        {
                            int source = *(ip + j);
                            int dest = *(fb + j);
                            if (source != dest)
                            {
                                found = false;
                                break;
                            }
                        }
                        if (found)
                            return i;
                    }
                }

            }
            return -1;
        }

    }
}

