using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace h3magic
{
    public class Heroes3Master : IDisposable
    {
        private Routing routing;

        public static Heroes3Master Master { get; set; }


        public List<LodFile> ResourceFiles { get; private set; }
        public H3Bitmap H3Bitmap
        {
            get
            {
                return GetByName("h3bitmap.lod") as H3Bitmap;
            }
        }
        public H3Sprite H3Sprite { get { return GetByName("h3sprite.lod") as H3Sprite; } }
        public ExeFile Executable { get; private set; }

        public Dictionary<string, List<string>> NameToFileMap { get; private set; }

        private Dictionary<string, LodFile> routingCache = new Dictionary<string, LodFile>();

        public Routing Routing
        {
            get
            {
                return routing ?? Routing.Default;
            }
            set
            {
                routing = value;
            }
        }

        public int CastlesCount
        {
            get
            {
                return 9;
            }
        }

        public LodFile GetByName(string name)
        {
            return ResourceFiles.FirstOrDefault(r => string.Compare(r.Name, name, true) == 0);
        }


        public static Heroes3Master LoadInfo(string executablePath)
        {
            var master = new Heroes3Master();

            master.Executable = new ExeFile(executablePath);
            master.ResourceFiles = new List<LodFile>();

            string dataDirectory = Path.Combine(Path.GetDirectoryName(executablePath), "Data");

            master.LoadAllWithExtension(dataDirectory, ".lod");
            master.LoadAllWithExtension(dataDirectory, ".pac");

            master.BuildMap();
            if (master.GetByName("HotA.lod") != null)
                master.Routing = Routing.Hota;

            master.RefreshData();
            Master = master;
            return Master;
        }


        public void RefreshData()
        {
            routingCache = new Dictionary<string, LodFile>();
            Resource.Unload();
            CreatureAnimationLoop.Unload();
            BitmapCache.UnloadCachedDrawItems();

            HeroesManager.LoadInfo(this);
            HeroClass.LoadInfo(this);
            CreatureManager.LoadInfo(this);
            Spell.LoadInfo(this);
            SecondarySkill.LoadInfo(this);
            StringsData.LoadInfo(this);

            Speciality.LoadInfo(Executable.Data);
            Town.LoadInfo(this);
            HeroExeData.LoadInfo(Executable.Data);
        }


        public LodFile Resolve(string fileName)
        {
            LodFile lodFile;
            if(routingCache.TryGetValue(fileName.ToLower(), out lodFile))
                return lodFile;

            
            lodFile = Routing.Resolve(this, fileName);
            routingCache[fileName.ToLower()] = lodFile;
            return lodFile;
        }

        private void BuildMap()
        {
            var sw = Stopwatch.StartNew();

            int size = ResourceFiles.Sum(r => r.FileCount);
            NameToFileMap = new Dictionary<string, List<string>>(size);

            List<string> temp;
            foreach (var resourceFile in ResourceFiles)
            {
                foreach (var file in resourceFile.FilesTable)
                {
                    string name = file.FileName.ToLower();
                    if (!NameToFileMap.TryGetValue(name, out temp))
                    {
                        temp = new List<string>();
                        NameToFileMap[name] = temp;
                    }
                    temp.Add(resourceFile.Name.ToLower());
                }
            }
            float z = sw.ElapsedMs();
        }

        private void LoadAllWithExtension(string dataDirectory, string extension)
        {
            foreach (var file in Directory.GetFiles(dataDirectory, "*" + extension))
            {
                if (file.EndsWith(extension, StringComparison.OrdinalIgnoreCase))
                {
                    FileStream fs = null;
                    try
                    {
                        fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                        string lcfileName = Path.GetFileName(file).ToLower();
                        LodFile lod = null;
                        if (lcfileName == "h3bitmap.lod")
                            lod = new H3Bitmap(this,fs);
                        else if (lcfileName == "h3sprite.lod")
                            lod = new H3Sprite(this, fs);
                        else
                            lod = new LodFile(this, fs);

                        lod.LoadFAT();
                        ResourceFiles.Add(lod);
                    }
                    catch (Exception ex)
                    {
                        fs.Close();
                    }
                }
            }
        }


        public void SaveHeroExeData()
        {
            if (HeroExeData.Data != null && HeroExeData.Data.Any(f => f.HasChanged))
                File.WriteAllBytes(Executable.Path + ".bak." + DateTime.Now.ToString("yyyyMMdd_HHmmss"), Executable.Data);

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
                File.Move(H3Bitmap.Path, H3Bitmap.Path + ".bak." + DateTime.Now.ToString("yyyyMMdd_HHmmss"));
                File.Move(h3bitmapFn, H3Bitmap.Path);
                H3Bitmap.stream = new FileStream(H3Bitmap.Path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            }

            if (H3Sprite.SaveToDisk(h3spritefn))
            {
                H3Sprite.stream.Close();
                File.Move(H3Sprite.Path, H3Sprite.Path + ".bak." + DateTime.Now.ToString("yyyyMMdd_HHmmss"));
                File.Move(h3spritefn, H3Sprite.Path);
                H3Sprite.stream = new FileStream(H3Sprite.Path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            }
        }

        public void Dispose()
        {
            foreach (var file in ResourceFiles)
                file.stream.Close();

            Executable.Dispose();

            GC.Collect();
        }
    }
}

