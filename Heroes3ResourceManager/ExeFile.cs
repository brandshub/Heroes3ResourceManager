using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace h3magic
{
    public class ExeFile
    {
        public static ExeFile Executable
        {
            get;
            private set;
        }

        public string Name { get; set; }
        public string Path { get; set; }
        public byte[] Data { get; private set; }
        public static void LoadData(string path)
        {
            Executable = new ExeFile
            {
                Name = System.IO.Path.GetFileName(path),
                Path = path,
                Data = File.ReadAllBytes(path)
            };
        }
    }
}
