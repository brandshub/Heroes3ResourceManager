using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace h3magic
{
    public class H3Sprite : LodFile
    {
        public const string SPEC_LARGE = "UN44.def";
        public const string SPEC_SMALL = "UN32.def";

        private DefFile un32 = null;
        private DefFile un44 = null;

        public DefFile Un32Def
        {
            get
            {
                if (un32 == null)
                {
                    var rec = GetRecord(SPEC_SMALL);
                    if (rec == null)
                        throw new Exception("Not a h3sprite.lod");
                    un32 = rec.GetDefFile(this);
                }
                return un32;
            }
        }

        public DefFile Un44Def
        {
            get
            {
                if (un44 == null)
                {
                    var rec = GetRecord(SPEC_LARGE);
                    if (rec == null)
                        throw new Exception("Not a h3sprite.lod");

                    un44 = rec.GetDefFile(this);
                }
                return un44;
            }
        }

        public H3Sprite(FileStream fs)
            : base(fs)
        {

        }

        public override bool SaveToDisk(string fileName)
        {
            if (Un44Def.HasChanges || Un32Def.HasChanges)
            {
                base.SaveToDisk(fileName);
                return true;
            }
            return false;
        }

        public override bool HasChanges
        {
            get
            {
                return base.HasChanges || Un32Def.HasChanges || Un44Def.HasChanges;
            }
        }
    }
}
