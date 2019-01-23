using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace h3magic
{
    public class HeroesSection
    {
        private static readonly byte[] firstHero = { 0x0, 0x0, 0x0, 0x0, 0x7, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x6, 0x0, 0x0, 0x0, 0x1, 0x0, 0x0, 0x0 };

        public static long FindOffset2(Stream stream)
        {
            return FindPosition2(stream, firstHero);
        }

        public static long FindOffset2(byte[] bytes)
        {
            return FindPosition2(bytes, firstHero);
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


        public static unsafe long FindPosition3(Stream stream)
        {
            if (firstHero.Length > stream.Length)
                return -1;
            stream.Position = 0;
            var bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);

            int limit = firstHero.Length >> 2;
            int ub = bytes.Length - firstHero.Length;

            fixed (byte* fh = firstHero)
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
                            if(source != dest)
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

