using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace h3magic
{
    public class HeroesSection
    {
        //private static readonly byte[] firstHero = { 0, 0, 0, 0, 7, 0, 0, 0, 0, 0, 0, 0, 6, 0, 0, 0, 1, 0, 0, 0 };
        //private static readonly byte[] heroSpecs = { 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 2, 0, 0, 0 };
        private static readonly byte[] firstHero = { 7, 0, 0, 0, 0, 0, 0, 0, 6, 0, 0, 0, 1, 0, 0, 0 };
        private static readonly byte[] heroSpecs = { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 2, 0, 0, 0 };


        public static long HeroOffset1 { get; private set; }
        public static long HeroOffset2 { get; private set; }

        public static long FindOffset1(byte[] data)
        {
            HeroOffset1 = FindPosition3(data, firstHero) - 4;
            return HeroOffset1;
        }

        public static long FindOffsetX(byte[] data)
        {
            HeroOffset2 = FindPosition3(data, heroSpecs) - 4;
            return HeroOffset2;
        }       

        public static long FindPosition2(Stream stream, byte[] byteSequence)
        {
            if (byteSequence.Length > stream.Length)
                return -1;
            var bytes = new byte[stream.Length];
            stream.Position = 0;
            stream.Read(bytes, 0, bytes.Length);
            byte b0 = byteSequence[0];
            int limit = bytes.Length - byteSequence.Length;
            for (int i = 0; i < limit; i++)
            {
                if (bytes[i] == b0)
                {
                    bool found = true;
                    for (int j = 1; j < byteSequence.Length; j++)
                    {
                        if (bytes[i + j] != byteSequence[j])
                        {
                            found = false;
                            break;
                        }
                    }

                    if (found)
                        return i;
                }
            }
            return -1;
        }

        public static long FindPosition2(byte[] bytes, byte[] byteSequence)
        {
            if (byteSequence.Length > bytes.Length)
                return -1;

            byte b0 = byteSequence[0];
            int limit = bytes.Length - byteSequence.Length;
            for (int i = 0; i < limit; i++)
            {
                if (bytes[i] == b0)
                {
                    bool found = true;
                    for (int j = 1; j < byteSequence.Length; j++)
                    {
                        if (bytes[i + j] != byteSequence[j])
                        {
                            found = false;
                            break;
                        }
                    }

                    if (found)
                        return i;
                }
            }
            return -1;
        }


        public static unsafe long FindPosition3(byte[] bytes, byte[] search)
        {
            if (search.Length > bytes.Length)
                return -1;


            int limit = search.Length >> 2;
            int ub = bytes.Length - search.Length;

            fixed (byte* fh = search)
            fixed (byte* bts = bytes)
            {
                int* fb = (int*)fh;

                for (int i = 0; i < ub; i++)
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

