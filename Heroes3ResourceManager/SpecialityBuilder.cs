using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace h3magic
{
    public class SpecialityBuilder
    {
        private static List<Speciality> OriginalSpecs { get; set; }

        private static DefFile def32, def44;
        private static FatRecord h_specs;
        private static string[] spec_rows;
        
        public static void LoadFromMaster(Heroes3Master master)
        {
            FatRecord un32, un44;

            try
            {
                un32 = master.ResolveWith(BackupManager.GetBackupFileName(Speciality.IMG_FNAME_SMALL));
            }
            catch
            {
                un32 = master.ResolveWith(Speciality.IMG_FNAME_SMALL);
            }

            try
            {
                un44 = master.ResolveWith(BackupManager.GetBackupFileName(Speciality.IMG_FNAME));
            }
            catch
            {
                un44 = master.ResolveWith(Speciality.IMG_FNAME);
            }

            try
            {
                h_specs = master.ResolveWith(BackupManager.GetBackupFileName(HeroesManager.H_SPECS));
            }
            catch
            {
                h_specs = master.ResolveWith(HeroesManager.H_SPECS);
            }

            //LoadOriginalSpecs(master.Executable.Data);
            spec_rows = Encoding.Default.GetString(h_specs.GetRawData()).Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            LoadOriginalSpecs(Properties.Resources.allspecs, 0);
            LoadDefsUncompressed(un32.GetRawData(), un44.GetRawData());
        }

        public static void LoadOriginalSpecs(byte[] raw, int offset)
        {
            OriginalSpecs = Speciality.LoadInfo(raw, offset);
        }


        public static void LoadDefsUncompressed(byte[] un32, byte[] un44)
        {
            def32 = new DefFile(null, un32);
            def44 = new DefFile(null, un44);
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

        public static string OriginalSpecText(int index)
        {            
            return spec_rows[index+2];
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
