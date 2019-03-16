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
        public static Heroes3Master Master { get; set; }

        private Routing routing;

        public List<LodFile> ResourceFiles { get; private set; }

        public ExeFile Executable { get; private set; }

        public Dictionary<string, List<string>> NameToFileMap { get; private set; }

        private Dictionary<string, LodFile> routingCache = new Dictionary<string, LodFile>();

        public BackupManager BackupManager { get; set; }

        public string OriginalDataFolder { get; set; }

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

            master.OriginalDataFolder = Path.Combine(Path.GetDirectoryName(executablePath), "Data");

            master.LoadAllWithExtension(master.OriginalDataFolder, ".lod");
            master.LoadAllWithExtension(master.OriginalDataFolder, ".pac");

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
            BackupManager = new BackupManager();

            Resource.Unload();
            CreatureAnimationLoop.Unload();
            BitmapCache.UnloadCachedDrawItems();

            HeroesManager.LoadInfo(this);
            HeroClass.LoadInfo(this);
            CreatureManager.LoadInfo(this);
            Spell.LoadInfo(this);
            SecondarySkill.LoadInfo(this);
            StringsData.LoadInfo(this);

            Speciality.LoadInfo(this);
            Town.LoadInfo(this);
            HeroExeData.LoadInfo(this);

            BackupManager.LoadData(this);
        }


        public LodFile Resolve(string fileName)
        {
            LodFile lodFile;
            if (routingCache.TryGetValue(fileName.ToLower(), out lodFile))
                return lodFile;


            lodFile = Routing.Resolve(this, fileName);
            if (lodFile == null)
                return null;

            routingCache[fileName.ToLower()] = lodFile;
            return lodFile;
        }

        public FatRecord ResolveWith(string fileName)
        {
            return ResolveWith(fileName, true);
        }


        public FatRecord ResolveWith(string fileName, bool cachingEnabled)
        {
            LodFile lodFile;

            if (cachingEnabled)
            {
                if (routingCache.TryGetValue(fileName.ToLower(), out lodFile))
                    return lodFile[fileName];
            }


            lodFile = Routing.Resolve(this, fileName);
            if (lodFile == null)
                return null;

            if (cachingEnabled)
                routingCache[fileName.ToLower()] = lodFile;

            return lodFile[fileName];
        }

        private void BuildMap()
        {
            var sw = Stopwatch.StartNew();

            int size = ResourceFiles.Sum(r => r.FileCount);
            NameToFileMap = new Dictionary<string, List<string>>(size + BackupManager.FileEntriesToBackups.Length);

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
                        LodFile lod = new LodFile(this, fs);

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


        public void SaveHeroExeData(string backupFolder)
        {

            if (HeroExeData.Data != null && HeroExeData.Data.Any(f => f.HasChanged))
                File.WriteAllBytes(Path.Combine(backupFolder, Executable.Name), Executable.Data);


            if (HeroExeData.UpdateDataInMemory(this))
                File.WriteAllBytes(Executable.Path, Executable.Data);

        }

        public void SaveToDisk()
        {
            DateTime stamp = DateTime.Now;
            string backupFolder = Path.Combine(Directory.GetParent(Executable.Path).ToString(), "HeroesResourceManagerBackups", stamp.ToString("yyyy-MM-dd-HHmmss"));
            Directory.CreateDirectory(backupFolder);

            SaveHeroExeData(backupFolder);

            if (CreatureManager.AnyChanges)
                CreatureManager.SaveLocalChanges(this);

            if (HeroesManager.AnyChanges)
                HeroesManager.SaveLocalChanges(this);

            if (HeroClass.AnyChanges)
                HeroClass.SaveLocalChanges(this);

            if (Spell.AnyChanges)
                Spell.SaveLocalChanges(this);


            foreach (var lodFile in ResourceFiles)
                lodFile.SaveToDiskBackupAndSwap(backupFolder);


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

