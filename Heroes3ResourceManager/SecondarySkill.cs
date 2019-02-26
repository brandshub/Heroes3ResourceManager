using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace h3magic
{
    public class SecondarySkill
    {
        public const int SPEC_COLNUMBER = 7;
        public const int ALL_COLNUMBER = 12;

        private const string TXT_FNAME = "SSTRAITS.TXT";
        private const string IMG_FNAME = "Secskill.def";

        public static bool Loaded { get; private set; }

        public static List<SecondarySkill> AllSkills = new List<SecondarySkill>();
        public static int[] IndexesOfAllSpecSkills = { 1, 2, 5, 8, 11, 12, 13, 22, 23, 24, 25, 26, 27 };
        public static int[] MagicSchoolSecondarySkillIndexes = new int[] { 17, 16, 14, 15 };

        private static DefFile _defFile = null;
        private static Bitmap _skillTree = null;
        private static Bitmap _skillTree2 = null;
        private static Bitmap _specImage = null;

        public int Index { get; private set; }
        public string Name { get; private set; }

        public static void LoadInfo(LodFile lodFile)
        {
            var rec = lodFile[TXT_FNAME];
            string text = Encoding.Default.GetString(rec.GetRawData(lodFile.stream));
            var rows = text.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 2; i < rows.Length; i++)
            {
                string name = rows[i].Split('\t')[0];
                AllSkills.Add(new SecondarySkill { Index = i - 2, Name = name });
            }
            _defFile = null;
            _specImage = null;
            _skillTree = null;
            _skillTree2 = null;
        }

        public static Bitmap GetImage(LodFile lodFile, int skillIndex, int level)
        {
            return AllSkills[skillIndex].GetImage(lodFile, level);
        }

        public Bitmap GetImage(LodFile lodFile, int level)
        {
            if (_defFile == null)
                _defFile = lodFile.GetRecord(IMG_FNAME).GetDefFile(lodFile);
            return _defFile.GetByAbsoluteNumber(Index * 3 + level + 2);
        }

        public override string ToString()
        {
            return Name;
        }



        public static Bitmap GetSkillTreeForHeroClass(LodFile h3sprite)
        {

            if (_skillTree != null)
                return _skillTree;

            if (_defFile == null)
                _defFile = h3sprite.GetRecord(IMG_FNAME).GetDefFile(h3sprite);

           /* var bmp = new Bitmap((44 + 60) * 4, 44 * 7);
            using (var g = Graphics.FromImage(bmp))
            {
                for (int i = 0; i < 4; i++)
                    for (int j = 0; j < 7; j++)
                        g.DrawImage(_defFile.GetByAbsoluteNumber(3 + (i * 7 + j) * 3), i * 104, 44 * j);
            }*/

            var bmp = new Bitmap(44 * 7, (44 + 24) * 4);

            using (var g = Graphics.FromImage(bmp))
            {
                for (int i = 0; i < 4; i++)
                    for (int j = 0; j < 7; j++)
                        g.DrawImage(_defFile.GetByAbsoluteNumber(3 + (i * 7 + j) * 3), j * 44, 68 * i);
            }
            _skillTree = bmp;
            return _skillTree;
        }

        public static Bitmap GetSkillTree(LodFile h3sprite)
        {
            if (_skillTree2 != null)
                return _skillTree2;

            if (_defFile == null)
                _defFile = h3sprite.GetRecord(IMG_FNAME).GetDefFile(h3sprite.stream);

            int rowCount = 3 * AllSkills.Count / ALL_COLNUMBER;

            var bmp = new Bitmap((44 + 1) * ALL_COLNUMBER, (44 + 1) * rowCount, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            var imageData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            Parallel.For(0, AllSkills.Count * 3, i =>
                {
                    int row = i / ALL_COLNUMBER;
                    int col = i % ALL_COLNUMBER;

                    var img = _defFile.GetByAbsoluteNumber2(3 + row * ALL_COLNUMBER + col);
                    imageData.DrawImage24(col * (44 + 1), row * (44 + 1), 132, img);
                });

            bmp.UnlockBits(imageData);
            _skillTree2 = bmp;

            return _skillTree2;
        }



        public static Bitmap GetSkillsForSpeciality(LodFile h3sprite)
        {
            if (_specImage != null)
                return _specImage;

            if (_defFile == null)
                _defFile = h3sprite.GetRecord(IMG_FNAME).GetDefFile(h3sprite.stream);           
            
            int rowNum = IndexesOfAllSpecSkills.Length / SPEC_COLNUMBER + (IndexesOfAllSpecSkills.Length % SPEC_COLNUMBER == 0 ? 0 : 1);

            var bmp = new Bitmap((44 + 1) * SPEC_COLNUMBER, (44 + 1) * rowNum, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            var imageData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            Parallel.For(0, IndexesOfAllSpecSkills.Length, i =>
            {
                int row = i / SPEC_COLNUMBER;
                int col = i % SPEC_COLNUMBER;

                var img = _defFile.GetByAbsoluteNumber2(3 + IndexesOfAllSpecSkills[i] * 3);
                imageData.DrawImage24(col * (44 + 1), row * (44 + 1), 132, img);
            });

            bmp.UnlockBits(imageData);
            _specImage = bmp;

            return _specImage;
        }

    }
}
