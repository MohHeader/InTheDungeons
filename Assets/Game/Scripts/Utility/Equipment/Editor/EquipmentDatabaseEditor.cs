using Assets.Game.Scripts.Utility.Characters;
using Assets.Game.Scripts.Utility.Characters.Editor;
using Assets.Game.Scripts.Utility.Common;
using UnityEditor;
using UnityEngine;

namespace Assets.Game.Scripts.Utility.Equipment.Editor {
    public class EquipmentDatabaseEditor : CommonDatabaseEditor<UtilityDatabase<EquipmentBlueprint>, EquipmentBlueprint>
    {
        protected override string DatabasePath
        {
            get { return @"Assets/Database/EquipmentDB.asset"; }
        }

        protected override Object CreateDatabaseInstance()
        {
            return CreateInstance<EquipmentDatabase>();
        }

        protected override string ItemName
        {
            get { return "Equipment"; }
        }

        [MenuItem("Astaror/Database/Equipment Database")]
        public static void Init()
        {
            var window = GetWindow<EquipmentDatabaseEditor>(false, "Equipment");
            window.minSize = new Vector2(1200, 400);
            window.Show();
        }

        protected override void DisplayEditor(EquipmentBlueprint element) {
            base.DisplayEditor(element);
            element.Asset = EditorGUILayout.ObjectField("Asset:", element.Asset, typeof(GameObject), false) as GameObject;
            element.Icon = (Sprite)EditorGUILayout.ObjectField("Icon:", element.Icon, typeof(Sprite), false);
            element.EquipmentType = (EquipmentTypeEnum)EditorGUILayout.EnumPopup("Equipment type:", element.EquipmentType);
            element.RequiredLevel = EditorGUILayout.IntField("Required level:", element.RequiredLevel);
            EditorGUILayout.Space();
            if (GUILayout.Button("Add main attribute")) {
                element.MainStats.Add(new MainStatBlueprint());
            }
            int toRemove = -1;

            for (int i = 0; i < element.MainStats.Count; i++) {
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("-", GUILayout.Width(40))) {
                    toRemove = i;
                }
                element.MainStats[i].MainStat = (MainStatEnum) EditorGUILayout.EnumPopup("Attribute", element.MainStats[i].MainStat);
                element.MainStats[i].Minimum = EditorGUILayout.IntField("Minimum", element.MainStats[i].Minimum);
                element.MainStats[i].Maximum = EditorGUILayout.IntField("Maximum", element.MainStats[i].Maximum);
                element.MainStats[i].Probability = EditorGUILayout.FloatField("Probability", element.MainStats[i].Probability);
                EditorGUILayout.EndHorizontal();
            }

            if (toRemove != -1) {
                element.MainStats.RemoveAt(toRemove);
            }
        }
    }
}