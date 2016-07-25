using UnityEditor;
using UnityEngine;

namespace Assets.Game.Scripts.Utility.Equipment.Editor {
    public class DefaultEquipmentEditor : EditorWindow{
        [MenuItem("Astaror/Database/Default Weapon Editor")]
        public static void Init()
        {
            var window = GetWindow<DefaultEquipmentEditor>(false, "Default Weapon");
            window.minSize = new Vector2(1200, 400);
            window.Show();
        }

        protected DefaultEquipmentDatabase Database;
        protected string DatabasePath
        {
            get { return @"Assets/Database/DefaultWeapons.asset"; }
        }

        protected void OnEnable()
        {
            if (Database == null)
                LoadDatabase();
        }

        protected void LoadDatabase()
        {
            Database = (DefaultEquipmentDatabase)AssetDatabase.LoadAssetAtPath(DatabasePath, typeof(DefaultEquipmentDatabase));

            if (Database == null)
                CreateDatabase();
        }

        protected void CreateDatabase()
        {
            AssetDatabase.CreateAsset(CreateInstance<DefaultEquipmentDatabase>(), DatabasePath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Database = (DefaultEquipmentDatabase)AssetDatabase.LoadAssetAtPath(DatabasePath, typeof(DefaultEquipmentDatabase));
        }

        // ReSharper disable once InconsistentNaming
        protected void OnGUI()
        {
            if (Database == null) LoadDatabase();

            EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(true));
            for (int eqId = 0; eqId < Database.Database.Length; eqId++)
            {
                EditorGUILayout.BeginHorizontal();
                var id = (EquipmentTypeEnum) eqId;
                EditorGUILayout.LabelField(id.ToString());
                var equipment = Database.Database[eqId];

                var previousObject = AssetDatabase.LoadAssetAtPath(equipment, typeof(GameObject)) as GameObject;
                var gameObject = EditorGUILayout.ObjectField("Prefab", previousObject, typeof(GameObject), false);
                if (gameObject != null)
                {
                    Database.Database[eqId] = AssetDatabase.GetAssetPath(gameObject);
                }
                else
                {
                    Database.Database[eqId] = null;
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();

            if (GUILayout.Button("Save", GUILayout.Width(100)))
            {
                EditorUtility.SetDirty(Database);
            }
        }
    }
}