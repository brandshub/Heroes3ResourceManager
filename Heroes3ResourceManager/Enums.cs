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
        Creature = 1,
        Resource = 2,
        Spell = 3,
        Custom = 4,
        Speed = 5,
        CreaturesUpgrade = 6,
        Mutara = 7,
        Adrianna = -1

    };

    public enum ResourceSpeciality
    {
        Mercury = 1,
        Sulphur = 3,
        Crystals = 4,
        Gems = 5,
        Gold350 = 6
    };

}
