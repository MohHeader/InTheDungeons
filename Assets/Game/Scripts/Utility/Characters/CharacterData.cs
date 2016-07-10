using System;
using System.Collections.Generic;
using Assets.Dungeon.Scripts.DataLayer.Skills;
using Assets.Dungeon.Scripts.Utility.Common;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Assets.Game.Scripts.Utility.Characters {
    public enum MainStatEnum
    {
        Strength, Dexterity, Intelligence
    }

    [Serializable]
    public class CharacterData : ACommonData {
        public GameObject Asset
        {
#if UNITY_EDITOR
            get { return string.IsNullOrEmpty(AssetPath) ? null : AssetDatabase.LoadAssetAtPath(AssetPath, typeof(GameObject)) as GameObject; }
            set { AssetPath = AssetDatabase.GetAssetPath(value); }
#else
            get { return Resources.Load<GameObject>(AssetPath); }
#endif
        }

        [SerializeField] public string AssetPath;

        [SerializeField] public MainStatEnum MainStatValue;

        #region Main Stats

        [SerializeField] public float LevelConstitution;

        [SerializeField] public float LevelDexterity;

        [SerializeField] public float LevelIntelligence;

        [SerializeField] public float LevelStrength;

        [SerializeField] public float StartConstitution;

        [SerializeField] public float StartDexterity;

        [SerializeField] public float StartIntelligence;

        [SerializeField] public float StartStrength;

        #endregion

        #region Grades multipliers

        [SerializeField] public float SecondGradeMultiplier;

        [SerializeField] public float ThirdGradeMultiplier;

        [SerializeField] public float ForthGradeMultiplier;

        [SerializeField] public float FifthGradeMultiplier;

        #endregion

        #region Skills

        [SerializeField] public List<SkillData> Skills = new List<SkillData>();

        #endregion
    }
}