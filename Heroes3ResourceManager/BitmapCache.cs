using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace h3magic
{
    public static class BitmapCache
    {
        // Creatures
        public static Bitmap CreaturesAll;
        public static Bitmap CreaturesUnupgraded;

        // Spells
        public static Bitmap SpellsAll;
        public static Bitmap SpellsForSpeciality;
        public static Bitmap[] SpellsMagicSchools;
        public static Bitmap[] SpellsMagicSchoolsInactive;

        //Towns         
        public static Bitmap   TownsGrid;
        public static Bitmap[] Towns;
        public static Bitmap[] TownBackgrounds;
        public static Bitmap[] TownsSmall;


        //DrawItemCache
        public static Bitmap[] DrawItemHeroesListBox;
        public static Bitmap[] DrawItemSpellsListBox;
        public static Bitmap[] DrawItemCreaturesOtherComboBox;


        public static void DisposeImage()
        {
            //TODO
        }

    }
}
