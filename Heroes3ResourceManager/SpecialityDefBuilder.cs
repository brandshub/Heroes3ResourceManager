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
        private static List<Speciality> OriginalSpecs { get; set; }

        private static DefFile def32, def44;

        public static void LoadOriginalSpecs(byte[] raw)
        {
            OriginalSpecs = Speciality.LoadInfo(raw, 0);
        }

        public static void LoadDefs(byte[] un32, byte[] un44)
        {
            def32 = new DefFile(null, Decompress(un32));
            def44 = new DefFile(null, Decompress(un44));
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

        public static int FindOriginalSpecIndexFromSpeciality(Speciality newSpec)
        {
            if (OriginalSpecs != null)
            {
                for (int i = 0; i < OriginalSpecs.Count; i++)
                {
                    if (OriginalSpecs[i].Equals(newSpec))
                    {
                        return i;
                    }
                }
            }
            return -1;
        }

        public static int TryUpdateSpecImage(HeroExeData hero, DefFile un32, DefFile un44)
        {
            if (OriginalSpecs == null)
                return -1;

            int index = FindOriginalSpecIndexFromSpeciality(hero.Spec);
            if (index >= 0)
            {
                un44.RetargetSprite(def44, 0, hero.Index, index);
                un32.RetargetSprite(def32, 0, hero.Index, index);
            }
            return index;
        }
    }
}
