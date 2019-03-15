using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace h3magic
{
    public class ExeFile : IDisposable
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public byte[] Data { get; private set; }
        public HeroesSection HeroesSection { get; set; }


        public ExeFile(string executablePath)
        {
            Name = System.IO.Path.GetFileName(executablePath);
            Path = executablePath;
            Data = File.ReadAllBytes(executablePath);
            HeroesSection = new HeroesSection();
        }

        public void Dispose()
        {
            Data = null;
        }
    }
}
