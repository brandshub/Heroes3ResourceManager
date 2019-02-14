using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace h3magic
{
    public class H3Bitmap : LodFile
    {
        public H3Bitmap(FileStream fs)
            : base(fs)
        { }
        public override void SaveToDisk(string fileName)
        {
            if (CreatureManager.HasChanges)
                CreatureManager.Save(this);

            if (HeroesManager.HasChanges)
                HeroesManager.Save(this);

            if (HeroClass.HasChanges)
                HeroClass.Save(this);

            base.SaveToDisk(fileName);
        }
    }
}
