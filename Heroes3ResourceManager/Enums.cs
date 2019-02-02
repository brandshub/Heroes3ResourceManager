using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace h3magic
{
    public enum ProfilePropertyType
    {
        Creature, SecondarySkill, Spell, Speciality, Other
    }

    public enum SpecialityType
    {
        Skill = 0,
        CreatureLevelBonus = 1,
        Resource = 2,
        Spell = 3,
        CreatureStaticBonus = 4,
        Speed = 5,
        CreaturesUpgrade = 6,
        Mutara = 7,
        Adrianna = -1

    };

    public enum ResourceSpeciality
    {
        Lumber = 0,
        Mercury = 1,
        Stone = 2,
        Sulphur = 3,
        Crystals = 4,
        Gems = 5,
        Gold350 = 6
    };

}
