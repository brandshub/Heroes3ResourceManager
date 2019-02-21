using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace h3magic
{
    public class Heroes3Master
    {
        public static Heroes3Master Master { get; set; }

        public List<LodFile> ResourceFiles { get; private set; }
        public H3Bitmap H3Bitmap { get { return GetByName("h3bitmap.lod") as H3Bitmap; } }
        public H3Sprite H3Sprite { get { return GetByName("h3sprite.lod") as H3Sprite; } }
        public ExeFile Executable { get; private set; }

        public LodFile GetByName(string name)
        {
            return ResourceFiles.FirstOrDefault(r => r.Name.ToLower() == name);
        }


        public static Heroes3Master LoadInfo(string executablePath)
        {
            Master = new Heroes3Master();
            Master.Executable = new ExeFile(executablePath);
            Master.ResourceFiles = new List<LodFile>();

            string dataDirectory = Path.Combine(Path.GetDirectoryName(executablePath), "Data");
            string h3BitmapLod = Path.Combine(dataDirectory, "h3bitmap.lod");
            string h3SpriteLod = Path.Combine(dataDirectory, "h3sprite.lod");

            foreach (var file in Directory.GetFiles(dataDirectory, "*.lod"))
            {
                FileStream fs = null;
                try
                {
                    fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    string lcfileName = Path.GetFileName(file).ToLower();
                    LodFile lod = null;
                    if (lcfileName == "h3bitmap.lod")
                        lod = new H3Bitmap(fs);
                    else if (lcfileName == "h3sprite.lod")
                        lod = new H3Sprite(fs);
                    else
                        lod = new LodFile(fs);

                    lod.LoadFAT();
                    Master.ResourceFiles.Add(lod);
                }
                catch (Exception ex)
                {
                    fs.Close();
                }
            }

            HeroesManager.LoadInfo(Master.H3Bitmap);
            HeroClass.LoadInfo(Master.H3Bitmap);
            CreatureManager.LoadInfo(Master.H3Bitmap);
            Spell.LoadInfo(Master.H3Bitmap);
            SecondarySkill.LoadInfo(Master.H3Bitmap);
            Speciality.LoadInfo(Master.Executable.Data);
            HeroExeData.LoadInfo(Master.Executable.Data);

            return Master;
        }

        public void SaveHeroExeData()
        {
            if (HeroExeData.UpdateDataInMemory())
                File.WriteAllBytes(Executable.Path, Executable.Data);

        }

        public void Save()
        {
            SaveHeroExeData();
            string h3bitmapFn = H3Bitmap.Path + "new";
            string h3spritefn = H3Sprite.Path + "new";

            if (H3Bitmap.SaveToDisk(h3bitmapFn))
            {
                H3Bitmap.stream.Close();
                File.Move(H3Bitmap.Path, H3Bitmap.Path + ".bak." + DateTime.Now.ToString("yyyyMMdd_hhmmss"));
                File.Move(h3bitmapFn, H3Bitmap.Path);
                H3Bitmap.stream = new FileStream(H3Bitmap.Path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            }

            if (H3Sprite.SaveToDisk(h3spritefn))
            {
                H3Sprite.stream.Close();
                File.Move(H3Sprite.Path, H3Sprite.Path + ".bak." + DateTime.Now.ToString("yyyyMMdd_hhmmss"));
                File.Move(h3spritefn, H3Sprite.Path);
                H3Sprite.stream = new FileStream(H3Sprite.Path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            }
        }
    }
}

