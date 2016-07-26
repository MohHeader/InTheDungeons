﻿using System;
using System.Collections.Generic;
using Assets.Dungeon.Scripts.Utility.Common;
using Assets.Game.Scripts.Utility.Equipment;
using Assets.Game.Scripts.Utility.Skills;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

#else
using Assets.Game.Scripts.Helpers;
#endif

namespace Assets.Game.Scripts.Utility.Characters {
    public enum MainStatEnum {
        Strength,
        Dexterity,
        Intelligence
    }

    [Serializable]
    public class CharacterData : ACommonData {
        [SerializeField] public string AssetPath;

        [SerializeField] public string IconPath;

        [SerializeField] public MainStatEnum MainStatValue;

        [SerializeField] public EquipmentTypeEnum DefaultWeaponVisual;

        [SerializeField] public EquipmentTypeEnum DefaultOffhandVisual;

        #region Skills

        [SerializeField] public List<SkillData> Skills = new List<SkillData>();

        #endregion

        [SerializeField] public List<EquipmentSlot> EquipmentSlots = new List<EquipmentSlot>();

        public GameObject Asset
        {
#if UNITY_EDITOR
            get
            {
                return string.IsNullOrEmpty(AssetPath)
                    ? null
                    : AssetDatabase.LoadAssetAtPath(AssetPath, typeof(GameObject)) as GameObject;
            }
            set { AssetPath = AssetDatabase.GetAssetPath(value); }
#else
            get { return Resources.Load<GameObject>(AssetPath.CutResAndExtension()); }
#endif
        }

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

        #region Main Stats

        [SerializeField] public float LevelConstitution;
        [SerializeField] public float LevelDexterity;
        [SerializeField] public float LevelIntelligence;
        [SerializeField] public float LevelStrength;
        [SerializeField] public float StartConstitution;
        [SerializeField] public float StartDexterity;
        [SerializeField] public float StartIntelligence;
        [SerializeField] public float StartStrength;
        [SerializeField] public int ActionPoints;
        [SerializeField] public int MovementRange;

        #endregion

        #region Grades multipliers

        [SerializeField] public float SecondGradeMultiplier;
        [SerializeField] public float ThirdGradeMultiplier;
        [SerializeField] public float ForthGradeMultiplier;
        [SerializeField] public float FifthGradeMultiplier;

        #endregion
    }
}