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
        public static Bitmap[] CreaturesSmall;

        // Heroes
        public static Bitmap HeroesBackground;


        // Resources
        public static Bitmap ResourcesAll;

        // Spells
        public static Bitmap SpellsAll;
        public static Bitmap SpellsForSpeciality;
        public static Bitmap[] SpellsMagicSchools;
        public static Bitmap[] SpellsMagicSchoolsInactive;

        // Specialities
        public static Bitmap SpecialitiesAll;

        //Towns         
        public static Bitmap TownsGrid;
        public static Bitmap[] Towns;
        public static Bitmap[] TownBackgrounds;
        public static Bitmap[] TownsSmall;


        //DrawItemCache
        public static Bitmap[] DrawItemHeroesListBox;
        public static Bitmap[] DrawItemSpellsListBox;
        public static Bitmap[] DrawItemCreaturesOtherComboBox;


        public static void UnloadCachedDrawItems()
        {
            if (DrawItemHeroesListBox != null)
            {
                foreach (var bmp in DrawItemHeroesListBox.Where(p => p != null))
                    bmp.Dispose();
                DrawItemHeroesListBox = null;
            }
            if (DrawItemSpellsListBox != null)
            {
                foreach (var bmp in DrawItemSpellsListBox.Where(p => p != null))
                    bmp.Dispose();
                DrawItemSpellsListBox = null;
            }
            if (DrawItemCreaturesOtherComboBox != null)
            {
                foreach (var bmp in DrawItemCreaturesOtherComboBox.Where(p => p != null))
                    bmp.Dispose();
                DrawItemCreaturesOtherComboBox = null;
            }

        }



    }
}
