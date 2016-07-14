using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#else
using Assets.Game.Scripts.Helpers;
#endif

namespace Assets.Game.Scripts.Utility.Skills
{
    public enum TargetEnum
    {
        SingleEnemy,
        AllEnemies,
        Self,
        SingleAlly,
        AllAllies
    }

    public enum TriggerEnum
    {
        None,
        Attack,
        Skill1,
        Skill2
    }

    [Serializable]
    public class SkillData
    {
        [SerializeField] public int Cooldown;

        [SerializeField] public string IconPath;

        [SerializeField] public int Index;

        [SerializeField] public string Name;

        [SerializeField] public TargetEnum Target;

        [SerializeField] public TriggerEnum TriggerName;

        [SerializeField] public float MinimumDistance;

        [SerializeField] public float MaximumDistance;

        [SerializeField] public float DamageMultiplier;

        public Sprite Icon
        {
#if UNITY_EDITOR
            get
            {
                return string.IsNullOrEmpty(IconPath)
                    ? null
                    : AssetDatabase.LoadAssetAtPath(IconPath, typeof(Sprite)) as Sprite;
            }
            set { IconPath = AssetDatabase.GetAssetPath(value); }
#else
            get { return Resources.Load<Sprite>(IconPath.CutResAndExtension()); }
#endif
        }

        public virtual string GetSkillTypeName()
        {
            return string.Empty;
        }

        public virtual SkillData GetCopy()
        {
            return new SkillData
            {
                Index = Index,
                IconPath = IconPath,
                Cooldown = Cooldown,
                Name = Name,
                Target = Target,
                TriggerName = TriggerName,
                MinimumDistance = MinimumDistance,
                MaximumDistance = MaximumDistance
            };
        }
    }
}