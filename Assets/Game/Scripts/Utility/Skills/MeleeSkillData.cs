using System;

namespace Assets.Game.Scripts.Utility.Skills
{
    [Serializable]
    public class MeleeSkillData : SkillData
    {
        public override string GetSkillTypeName()
        {
            return "Melee";
        }

    }
}