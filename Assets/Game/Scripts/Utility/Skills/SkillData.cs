using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

#else
using Assets.Game.Scripts.Helpers;
#endif

namespace Assets.Game.Scripts.Utility.Skills {
    public enum TargetEnum {
        SingleEnemy,
        AllEnemies,
        Self,
        SingleAlly,
        AllAllies
    }

    public enum TriggerEnum {
        None,
        Attack,
        Skill1,
        Skill2
    }

    public enum SpellTypeEnum {
        Projectile,
        Direction
    }

    public enum SkillTypeEnum {
        Melee,
        Shooting,
        Spell
    }

    [Serializable]
    public class SkillData {
        [SerializeField] public int Cooldown;

        [SerializeField] public float DamageMultiplier;

        [SerializeField] public string IconPath;

        [SerializeField] public int Index;

        #region Melee data

        [SerializeField] public float InflictDamageTime;

        #endregion

        [SerializeField] public float MaximumDistance;

        [SerializeField] public float MinimumDistance;

        [SerializeField] public string Name;

        [SerializeField] public SkillTypeEnum SkillType;

        [SerializeField] public TargetEnum Target;

        [SerializeField] public TriggerEnum TriggerName;

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

        public virtual string GetSkillTypeName() {
            return string.Empty;
        }

        public virtual SkillData GetCopy() {
            return new SkillData
            {
                Index = Index,
                IconPath = IconPath,
                Cooldown = Cooldown,
                Name = Name,
                Target = Target,
                TriggerName = TriggerName,
                SkillType = SkillType,
                MinimumDistance = MinimumDistance,
                MaximumDistance = MaximumDistance,
                DamageMultiplier = DamageMultiplier,

                // Spell data

                PrefabPath = PrefabPath,
                SpellType = SpellType,
                SpawnTime = SpawnTime,
                SpellMovementSpeed = SpellMovementSpeed,

                // Melee data

                InflictDamageTime = InflictDamageTime
            };
        }

        #region Spell data

        [SerializeField] public string PrefabPath;

        [SerializeField] public SpellTypeEnum SpellType;

        [SerializeField] public float SpawnTime;

        [SerializeField] public float SpellMovementSpeed;

        public GameObject Prefab
        {
#if UNITY_EDITOR
            get
            {
                return string.IsNullOrEmpty(PrefabPath)
                    ? null
                    : AssetDatabase.LoadAssetAtPath(PrefabPath, typeof(GameObject)) as GameObject;
            }
            set { PrefabPath = AssetDatabase.GetAssetPath(value); }
#else
            get { return Resources.Load<GameObject>(PrefabPath.CutResAndExtension()); }
#endif
        }

        #endregion
    }
}