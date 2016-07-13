using System.Linq;
using Assets.Game.Scripts.Utility.Common;
using Assets.Game.Scripts.Utility.Skills;
using UnityEditor;
using UnityEngine;

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

        [MenuItem("Astaror/Database/Characters Database")]
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
            element.ActionPoints = EditorGUILayout.FloatField("Action points:", element.ActionPoints);

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
            EditorGUILayout.LabelField(new GUIContent("Skills"));
            EditorGUILayout.BeginVertical();
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(new GUIContent("Active Skill")))
            {
                element.Skills.Add(new SkillData());
            }
            //if (GUILayout.Button(new GUIContent("Shooting Skill")))
            //{
            //    element.Skills.Add(new ShootSkillData());
            //}
            //if (GUILayout.Button(new GUIContent("Spell Skill")))
            //{
            //    element.Skills.Add(new SpellSkillData());
            //}
            if (GUILayout.Button(new GUIContent("Passive Skill")))
            {
                element.Skills.Add(new PassiveSkillData());
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
        }

        private Vector2 _scrollPosition;

        protected SkillData SkillEditor(CharacterData element, SkillData skill)
        {
            EditorGUILayout.Space();
            if (GUILayout.Button(string.Format("- {0} {1}", skill.GetSkillTypeName(), skill.Name)))
            {
                return skill;
            }
            EditorGUILayout.BeginHorizontal();
            skill.Index = EditorGUILayout.IntField("Skill index:", skill.Index);
            skill.Target = (TargetEnum) EditorGUILayout.EnumPopup("Target:", skill.Target);
            skill.TriggerName = (TriggerEnum) EditorGUILayout.EnumPopup("Trigger:", skill.TriggerName);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            skill.Name = EditorGUILayout.TextField("Skill name:", skill.Name);
            skill.Cooldown = EditorGUILayout.IntField("Cooldown (turns):", skill.Cooldown);
            EditorGUILayout.EndHorizontal();
            skill.Icon = (Sprite)EditorGUILayout.ObjectField("Icon:", skill.Icon, typeof(Sprite), false);
            return null;
        }
    }
}