using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Game.Scripts.Utility.Common;
using Assets.Game.Scripts.Utility.Equipment;
using Assets.Game.Scripts.Utility.Skills;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Assets.Game.Scripts.Utility.Characters.Editor {
    public class CharactersDatabaseEditor : CommonDatabaseEditor<UtilityDatabase<CharacterData>, CharacterData>{
        protected override string DatabasePath
        {
            get { return @"Assets/Database/CharactersDB.asset"; }
        }

        protected override Object CreateDatabaseInstance()
        {
            return CreateInstance<CharactersDatabase>();
        }

        protected override string ItemName
        {
            get { return "Character"; }
        }

        [MenuItem("Astaror/Database/Characters CharactersDatabase")]
        public static void Init()
        {
            var window = GetWindow<CharactersDatabaseEditor>(false, "Characters");
            window.minSize = new Vector2(1200, 400);
            window.Show();
        }

        protected override void DisplayEditor(CharacterData element) {
            base.DisplayEditor(element);
            element.Asset = EditorGUILayout.ObjectField(new GUIContent("Asset:"), element.Asset, typeof(GameObject), false) as GameObject;
            element.Icon = (Sprite)EditorGUILayout.ObjectField("Icon:", element.Icon, typeof(Sprite), false);
            element.MainStatValue = (MainStatEnum) EditorGUILayout.EnumPopup(new GUIContent("Main Attribute:"), element.MainStatValue);

            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            element.ActionPoints = EditorGUILayout.IntField("Action points:", element.ActionPoints);
            element.MovementRange = EditorGUILayout.IntField("Movement Range:", element.MovementRange);
            element.DefaultWeaponVisual = (EquipmentTypeEnum) EditorGUILayout.EnumPopup("Default weapon visual", element.DefaultWeaponVisual);
            element.DefaultOffhandVisual = (EquipmentTypeEnum) EditorGUILayout.EnumPopup("Default offhand visual", element.DefaultOffhandVisual);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField(new GUIContent("Starting attributes"));
            EditorGUILayout.BeginHorizontal();
            element.StartStrength = EditorGUILayout.FloatField(new GUIContent("STR:"), element.StartStrength);
            element.StartDexterity = EditorGUILayout.FloatField(new GUIContent("DEX:"), element.StartDexterity);
            element.StartIntelligence = EditorGUILayout.FloatField(new GUIContent("INT:"), element.StartIntelligence);
            element.StartConstitution = EditorGUILayout.FloatField(new GUIContent("CON:"), element.StartConstitution);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField(new GUIContent("Level attributes"));
            EditorGUILayout.BeginHorizontal();
            element.LevelStrength = EditorGUILayout.FloatField(new GUIContent("STR:"), element.LevelStrength);
            element.LevelDexterity = EditorGUILayout.FloatField(new GUIContent("DEX:"), element.LevelDexterity);
            element.LevelIntelligence = EditorGUILayout.FloatField(new GUIContent("INT:"), element.LevelIntelligence);
            element.LevelConstitution = EditorGUILayout.FloatField(new GUIContent("CON:"), element.LevelConstitution);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField(new GUIContent("Grade attribute bonuses"));
            EditorGUILayout.BeginHorizontal();
            element.SecondGradeMultiplier = EditorGUILayout.FloatField(new GUIContent("2*:"), element.SecondGradeMultiplier, GUILayout.Width(200));
            element.ThirdGradeMultiplier = EditorGUILayout.FloatField(new GUIContent("3*:"), element.ThirdGradeMultiplier, GUILayout.Width(200));
            element.ForthGradeMultiplier = EditorGUILayout.FloatField(new GUIContent("4*:"), element.ForthGradeMultiplier, GUILayout.Width(200));
            element.FifthGradeMultiplier = EditorGUILayout.FloatField(new GUIContent("5*:"), element.FifthGradeMultiplier, GUILayout.Width(200));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField(new GUIContent("Equipment slots"));
            EditorGUILayout.BeginVertical();
            if (element.EquipmentSlots == null) element.EquipmentSlots = new List<EquipmentSlot>();
            if (GUILayout.Button("New equipment slot")) {
                element.EquipmentSlots.Add(new EquipmentSlot());
            }

            var slotRemovalList = new List<EquipmentSlot>();

            foreach (var equipmentSlot in element.EquipmentSlots) {
                EditorGUILayout.BeginHorizontal();
                equipmentSlot.Slot = (WearSlotEnum)EditorGUILayout.EnumPopup("Slot:", equipmentSlot.Slot, GUILayout.Width(250));
                if (GUILayout.Button("+", GUILayout.Width(40))) {
                    equipmentSlot.PossibleEquipmentTypes.Add(EquipmentTypeEnum.Fake);
                }
                for (int eqSltId = 0; eqSltId < equipmentSlot.PossibleEquipmentTypes.Count; eqSltId++) {
                    equipmentSlot.PossibleEquipmentTypes[eqSltId] = (EquipmentTypeEnum)EditorGUILayout.EnumPopup("", equipmentSlot.PossibleEquipmentTypes[eqSltId], GUILayout.Width(250));
                }
                if (GUILayout.Button("-", GUILayout.Width(40)))
                {
                    slotRemovalList.Add(equipmentSlot);
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();


            EditorGUILayout.Space();
            EditorGUILayout.LabelField(new GUIContent("Skills"));
            EditorGUILayout.BeginVertical();
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(new GUIContent("New Active Skill")))
            {
                element.Skills.Add(new SkillData());
            }
            if (GUILayout.Button(new GUIContent("Passive Skill")))
            {
                //element.Skills.Add(new PassiveSkillData());
            }
            EditorGUILayout.EndHorizontal();
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
            var list = element.Skills.Select(skillData => SkillEditor(element, skillData)).ToList();
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();

            foreach (var skillData in list)
            {
                if (skillData != null) element.Skills.Remove(skillData);
            }

            foreach (var equipmentSlot in slotRemovalList) {
                if (equipmentSlot != null) element.EquipmentSlots.Remove(equipmentSlot);
            }
        }

        private Vector2 _scrollPosition;

        
        protected SkillData SkillEditor(CharacterData element, SkillData skill)
        {
            EditorGUILayout.Space();
            if (GUILayout.Button(string.Format("- {0} {1}", skill.SkillType, skill.Name)))
            {
                return skill;
            }
            EditorGUILayout.BeginHorizontal();
            skill.Index = EditorGUILayout.IntField("Skill index:", skill.Index);
            skill.SkillType = (SkillTypeEnum)EditorGUILayout.EnumPopup("Skill type:", skill.SkillType);
            skill.Target = (TargetEnum) EditorGUILayout.EnumPopup("Target:", skill.Target);
            skill.TriggerName = (TriggerEnum) EditorGUILayout.EnumPopup("Trigger:", skill.TriggerName);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            skill.Name = EditorGUILayout.TextField("Skill name:", skill.Name);
            skill.Cooldown = EditorGUILayout.IntField("Cooldown (turns):", skill.Cooldown);
            skill.ActionPoints = EditorGUILayout.IntField("Action points:", skill.ActionPoints);
            skill.DamageMultiplier = EditorGUILayout.FloatField("Damage:", skill.DamageMultiplier);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            skill.MinimumDistance = EditorGUILayout.FloatField("Min Distance", skill.MinimumDistance);
            skill.MaximumDistance = EditorGUILayout.FloatField("Max distance", skill.MaximumDistance);
            EditorGUILayout.EndHorizontal();
            skill.Icon = (Sprite)EditorGUILayout.ObjectField("Icon:", skill.Icon, typeof(Sprite), false);

            switch (skill.SkillType)
            {
                case SkillTypeEnum.Melee:
                    skill.InflictDamageTime = EditorGUILayout.FloatField("Damage Time:", skill.InflictDamageTime);
                    break;
                case SkillTypeEnum.Shooting:
                    break;
                case SkillTypeEnum.Spell:
                    skill.SpawnTime = EditorGUILayout.FloatField("Spawn Time:", skill.SpawnTime);
                    skill.SpellType = (SpellTypeEnum)EditorGUILayout.EnumPopup("Skill type:", skill.SpellType);
                    skill.Prefab = (GameObject)EditorGUILayout.ObjectField("Prefab:", skill.Prefab, typeof(GameObject), false);
                    skill.SpellMovementSpeed = EditorGUILayout.FloatField("Movement speed:", skill.SpellMovementSpeed);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return null;
        }
    }
}