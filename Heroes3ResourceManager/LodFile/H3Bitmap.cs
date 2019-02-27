using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace h3magic
{
    public class H3Bitmap : LodFile
    {
        
        public H3Bitmap(Heroes3Master master, FileStream fs)
            : base(master,fs)
        { }
        public override void LoadData(int count)
        {
            base.LoadData(count);
        }

        public override bool SaveToDisk(string fileName)
        {
            if (CreatureManager.HasChanges)
                CreatureManager.Save(this);

            if (HeroesManager.HasChanges)
                HeroesManager.Save(this);

            if (HeroClass.HasChanges)
                HeroClass.Save(this);

            if (Spell.AllSpells.Any(s => s.HasChanges))
                Spell.Save(this);

            return base.SaveToDisk(fileName);
        }

        public override bool HasChanges
        {
            get
            {
                return base.HasChanges || CreatureManager.HasChanges || HeroesManager.HasChanges || HeroClass.HasChanges || Spell.AllSpells.Any(s => s.HasChanges);
            }
        }
    }
}
