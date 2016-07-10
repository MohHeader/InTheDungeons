using System;
using UnityEditor;
using UnityEngine;

namespace Assets.Dungeon.Scripts.DataLayer.Skills
{
    public enum TargetEnum
    {
        SingleEnemy,
        AllEnemies,
        Self,
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
        public virtual string GetSkillTypeName()
        {
            return string.Empty;
        }

        [SerializeField] public int Index;

        public Sprite Icon
        {
#if UNITY_EDITOR
            get { return string.IsNullOrEmpty(IconPath) ? null : AssetDatabase.LoadAssetAtPath(IconPath, typeof(Sprite)) as Sprite; }
            set { IconPath = AssetDatabase.GetAssetPath(value); }
#else
            get { return Resources.Load<Sprite>(IconPath); }
#endif
        }

        [SerializeField]
        public string IconPath;

        [SerializeField] public int Cooldown;

        [SerializeField] public string Name;

        [SerializeField] public TargetEnum Target;

        [SerializeField] public TriggerEnum TriggerName;

        public virtual SkillData GetCopy()
        {
            return new SkillData
            {
                Index = Index,
                Icon = Icon,
                Cooldown = Cooldown,
                Name = Name,
                Target = Target,
                TriggerName = TriggerName
            };
        }
    }
}