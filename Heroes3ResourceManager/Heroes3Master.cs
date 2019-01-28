using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace h3magic
{
    public class Heroes3Master
    {
        public static Heroes3Master Master { get; private set; }

        public List<LodFile> ResourceFiles { get; private set; } = new List<LodFile>();
        public LodFile H3Bitmap { get { return GetByName("h3bitmap.lod"); } }
        public LodFile H3Sprite { get { return GetByName("h3sprite.lod"); } }
        public ExeFile Executable { get; private set; }

        public LodFile GetByName(string name)
        {
            return ResourceFiles.FirstOrDefault(r => r.Name.ToLower() == name);
        }


        public static Heroes3Master LoadData(string executablePath)
        {
            Master = new Heroes3Master();
            Master.Executable = new ExeFile(executablePath);

            string dataDirectory = Path.Combine(Path.GetDirectoryName(executablePath), "Data");
            string h3BitmapLod = Path.Combine(dataDirectory, "h3bitmap.lod");
            string h3SpriteLod = Path.Combine(dataDirectory, "h3sprite.lod");


            foreach (var file in Directory.GetFiles(dataDirectory, "*.lod"))
            {
                FileStream fs = null;
                try
                {
                    fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    var lod = new LodFile(fs);
                    lod.LoadFAT();
                    Master.ResourceFiles.Add(lod);
                }
                catch
                {
                    fs.Close();
                }
            }

            return Master;
        }
    }
}

