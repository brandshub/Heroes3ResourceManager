using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace h3magic
{
    public class SpecialityDefBuilder
    {
        public static List<Speciality> OriginalSpecs;
        public static byte[] UN32;
        public static byte[] UN44;

        public static void LoadOriginalSpecs(byte[] raw)
        {
            OriginalSpecs = Speciality.LoadInfo(raw, 0);
            
        }

        public static void LoadDefs(byte[] un32, byte[] un44)
        {
            UN32 = Decompress(un32);
            UN44 = Decompress(un44);
        }

        private static byte[] Decompress(byte[] data)
        {
            using (var compressedStream = new MemoryStream(data))
            using (var zipStream = new GZipStream(compressedStream, CompressionMode.Decompress))
            using (var resultStream = new MemoryStream())
            {
                zipStream.CopyTo(resultStream);
                return resultStream.ToArray();
            }
        }
    }
}
