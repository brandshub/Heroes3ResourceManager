using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace h3magic
{
    public enum ProfilePropertyType
    {
        Creature, SecondarySkill, Spell, Other,
        // indirect properties
        SpecSecondarySkill, SpecCreature, SpecResource, SpecSpell, SpecCreatureStatic, SpecSpeed, SpecCreatureUpgrade // last two omitted
    }

    public enum SpecialityType
    {
        [Description("Skill")]
        Skill = 0,
        [Description("Creature Bonus")]
        CreatureLevelBonus = 1,
        [Description("Resources")]
        Resource = 2,
        [Description("Spell")]
        Spell = 3,
        [Description("Creature Bonus Static")]
        CreatureStaticBonus = 4,
        [Description("Speed +2")]
        Speed = 5,
        [Description("Creature Upgrade")]
        CreaturesUpgrade = 6,
        Mutara = 7,
        Adrianna = -1,
        Invalid = 100
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
