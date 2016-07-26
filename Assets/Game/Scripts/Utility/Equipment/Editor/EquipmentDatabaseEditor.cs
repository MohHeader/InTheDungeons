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
        }
    }
}