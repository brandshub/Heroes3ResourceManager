using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

namespace h3magic
{
    public class LodFile
    {
        protected const uint HEADER = 0x00444f4c;
        protected const int FAT_OFFSET = 0x5c;

        public Stream stream;

        public int FileCount { get; private set; }
        public List<FatRecord> FilesTable { get; private set; }
        public string Name { get; private set; }
        public string Path { get; private set; }

        public Heroes3Master Master { get; private set; }

        public LodFile(Heroes3Master master, FileStream fs)
        {
            Path = fs.Name;
            Name = System.IO.Path.GetFileName(Path).ToLower();

            byte[] temp = new byte[4];
            fs.Position = 8;
            fs.Read(temp, 0, 4);
            stream = fs;

            FileCount = BitConverter.ToInt32(temp, 0);
            FilesTable = new List<FatRecord>(FileCount);
            Master = master;
        }

        public virtual void LoadData(LodFile parent, int count)
        {
            stream.Position = FAT_OFFSET;

            byte[] record = new byte[32];
            for (int i = 0; i < count; i++)
            {
                stream.Read(record, 0, 32);
                FilesTable.Add(new FatRecord(parent, record));
            }
        }

        private int bs(string f)
        {
            var fnames = GetNames();
            return Array.BinarySearch<string>(fnames, f);
        }
        public void AddNewRecord(FatRecord record)
        {
            record.Parent = this;

            int index = IndexOf(record.FileName);
            if (index >= 0)
                return;

            if (string.Compare(record.FileName, FilesTable.Last().FileName) == 1)
            {
                FilesTable.Add(record);
            }
            else
            {
                FilesTable.Insert(~index, record);
            }
            FileCount++;

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
            LoadData(this, FileCount);
        }

        public string[] GetNames()
        {
            return FilesTable.Select(fat => fat.FileName).ToArray();
        }

        public int IndexOf(string fileName)
        {
            string local = fileName;
            int low = 0, hi = FilesTable.Count - 1, now = -1;
            while (low <= hi)
            {
                now = low + ((hi - low) >> 1);
                int res = string.Compare(local, FilesTable[now].FileName, true);
                if (res < 0)
                    hi = now - 1;
                else if (res > 0)
                    low = now + 1;
                else
                    return now;
            }
            return ~low;
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
            return index >= 0 ? FilesTable[index].GetBitmap() : null;
        }

        public byte[] GetRawData(string fileName)
        {
            int index = IndexOf(fileName);
            return index >= 0 ? FilesTable[index].GetRawData() : null;
        }


        public List<FatRecord> Filter(string extension)
        {
            if (extension[0] == '.')
                extension = extension.Substring(1);
            if (extension == "*") return FilesTable.ToList();
            string local = FatRecord.ToggleCase(extension);
            return FilesTable.Where(fat => fat.Extension == local).ToList();
        }

        public virtual bool HasChanges
        {
            get { return FilesTable.Any(f => f.HasChanged); }
        }

        public virtual bool SaveToDisk(string fileName)
        {
            if (HasChanges)
            {
                using (var destination = new FileStream(fileName, FileMode.Create))
                {
                    byte[] buffer = new byte[4096];

                    destination.Position = FilesTable[0].Offset;
                    for (int i = 0; i < FileCount; i++)
                    {
                        string fn = FilesTable[i].FileName;
                        FilesTable[i].SaveToStream(stream, destination);
                    }

                    stream.Position = 0;
                    stream.Read(buffer, 0, FAT_OFFSET);

                    destination.Position = 0;
                    destination.Write(buffer, 0, FAT_OFFSET);
                    for (int i = 0; i < FileCount; i++)
                        destination.Write(FilesTable[i].GetHeader(), 0, 32);

                    destination.Position = 8;
                    destination.Write(BitConverter.GetBytes(FileCount), 0, 4);


                    destination.Flush();
                }
                return true;
            }
            return false;
        }

        public bool SaveToDiskBackupAndSwap(string prefix)
        {
            string newFileName = Path + prefix;

            if (SaveToDisk(newFileName))
            {
                stream.Close();
                File.Move(Path, Path + ".bak." + DateTime.Now.ToString("yyyyMMdd_HHmmss"));
                File.Move(newFileName, Path);
                stream = new FileStream(Path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                return true;
            }
            return false;

        }


        public override string ToString()
        {
            return Name;
        }
    }
}
