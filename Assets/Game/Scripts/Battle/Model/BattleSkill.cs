using Assets.Game.Scripts.Utility.Skills;
using UniRx;
using UnityEngine;

namespace Assets.Game.Scripts.Battle.Model
{
    public class BattleSkill
    {
        // TODO: Hack
        public SkillData SkillData;

        public BattleSkill(SkillData argument)
        {
            SkillData = argument.GetCopy();
            RemainingCooldown = new ReactiveProperty<int>(0);
        }

        public Sprite Icon
        {
            get { return SkillData.Icon; }
        }

        public float MinimumDistance
        {
            get { return SkillData.MinimumDistance; }
        }

        public float MaximumDistance
        {
            get { return SkillData.MaximumDistance; }
        }

        public ReactiveProperty<int> RemainingCooldown;
    }
}