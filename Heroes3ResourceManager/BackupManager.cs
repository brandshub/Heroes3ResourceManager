using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace h3magic
{
    public class BackupManager
    {
        public static string[] FileEntriesToBackups = new string[] { Speciality.IMG_FNAME_SMALL, Speciality.IMG_FNAME, HeroesManager.H_SPECS };
        public List<FatRecord> OriginalData { get; set; }
        public Heroes3Master Master { get; private set; }

        public void LoadData(Heroes3Master master)
        {
            Master = master;

            OriginalData = new List<FatRecord>();
            foreach (var name in FileEntriesToBackups)
            {
                string backupName = GetBackupFileName(name);
                FatRecord temp;
                try
                {
                    temp = master.ResolveWith(backupName);
                }
                catch
                {
                    temp = master.ResolveWith(name).Clone(backupName);
                    temp.Parent.AddNewRecord(temp);

                    master.NameToFileMap[backupName.ToLower()] = new List<string> { temp.Parent.Name.ToLower() };
                }
                OriginalData.Add(temp);
            }
        }

        public static string GetBackupFileName(string fatName)
        {
            if (fatName.Length <= 10)
                return "z_" + fatName;

            return fatName.Replace(Path.GetExtension(fatName), ".zbk");
        }

    }
}
