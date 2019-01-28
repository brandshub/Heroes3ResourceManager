using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;

namespace h3magic
{
    public class LodFile
    {
        private const uint HEADER = 0x00444f4c;
        private const int FAT_OFFSET = 0x5c;

        public Stream stream;

        public int FileCount { get; private set; }
        public List<FatRecord> FilesTable { get; private set; }
        public string Name { get; private set; }
        public string Path { get; private set; }

        public LodFile(FileStream fs)
        {
            Path = fs.Name;
            Name = System.IO.Path.GetFileName(Path).ToLower();

            byte[] temp = new byte[4];
            fs.Read(temp, 0, 4);
            if (BitConverter.ToUInt32(temp, 0) != HEADER)
            {
                fs.Close();
                throw new ArgumentException("not a .LOD file");
            }

            fs.Position = 8;
            fs.Read(temp, 0, 4);
            stream = fs;
            FileCount = BitConverter.ToInt32(temp, 0);
            FilesTable = new List<FatRecord>(FileCount);
        }

        public void LoadFAT(int count)
        {
            stream.Position = FAT_OFFSET;
            byte[] record = new byte[32];
            for (int i = 0; i < count; i++)
            {
                stream.Read(record, 0, 32);
                FilesTable.Add(new FatRecord(record));
            }
        }

        public FatRecord this[string name]
        {
            get
            {
                return GetRecord(name);
            }
        }

        public FatRecord this[int index]
        {
            get
            {
                return FilesTable[index];
            }
        }


        public void LoadFAT()
        {
            LoadFAT(FileCount);
        }

        public string[] GetNames()
        {
            return FilesTable.Select(fat => fat.FileName).ToArray();
        }

        public int IndexOf(string fileName)
        {
            string local = fileName;
            int prev = 0, next = FilesTable.Count - 1, now = -1;
            while (prev <= next)
            {
                now = (next + prev) >> 1;
                int res = string.Compare(local, FilesTable[now].FileName, true);
                if (res > 0)
                    prev = now + 1;
                else if (res < 0)
                    next = now - 1;
                else
                    return now;
            }
            return ~now;
        }

        public FatRecord GetRecord(string fileName)
        {
            int index = IndexOf(fileName);
            return index >= 0 ? FilesTable[index] : null;
        }

        public Bitmap GetBitmap(string fileName)
        {
            if (fileName.ToUpper().Substring(fileName.Length - 3) != "PCX") return null;
            int index = IndexOf(fileName);
            return index >= 0 ? FilesTable[index].GetBitmap(stream) : null;
        }

        public byte[] GetRawData(string fileName)
        {
            int index = IndexOf(fileName);
            return index >= 0 ? FilesTable[index].GetRawData(stream) : null;
        }


        public List<FatRecord> Filter(string extension)
        {
            if (extension[0] == '.')
                extension = extension.Substring(1);
            if (extension == "*") return FilesTable.ToList();
            string local = FatRecord.ToggleCase(extension);
            return FilesTable.Where(fat => fat.Extension == local).ToList();
        }

        public void SaveToDisk(string fileName)
        {

            if (CreatureManager.HasChanges)
                CreatureManager.Save(this);
            if (HeroesManager.Loaded)
                HeroesManager.Save(this);
            if (HeroClassManager.Loaded)
                HeroClassManager.Save(this);

            FileStream fs = new FileStream(fileName, FileMode.Create);
            byte[] buffer = new byte[4096];

            fs.Position = FilesTable[0].Offset;
            for (int i = 0; i < FileCount; i++)
                FilesTable[i].SaveToStream(stream, fs);


            stream.Position = 0;
            stream.Read(buffer, 0, FAT_OFFSET);
            fs.Position = 0;
            fs.Write(buffer, 0, FAT_OFFSET);
            for (int i = 0; i < FileCount; i++)
                fs.Write(FilesTable[i].GetHeader(), 0, 32);


            fs.Flush();
            fs.Close();

        }

        public void SaveToDisk()
        {
            SaveToDisk(Path + "new");
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
