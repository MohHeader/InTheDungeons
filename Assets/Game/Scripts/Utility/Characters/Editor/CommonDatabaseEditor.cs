using Assets.Dungeon.Scripts.Utility.Common;
using Assets.Game.Scripts.Utility.Common;
using UnityEditor;
using UnityEngine;

namespace Assets.Game.Scripts.Utility.Characters.Editor {
    public class CommonDatabaseEditor<T, TU> : EditorWindow where T : UtilityDatabase<TU> where TU : ACommonData, new() {
        protected enum StateEnum
        {
            Blank,
            Edit,
            Add
        }

        protected StateEnum State;

        protected int SelectedIndex;
        protected Vector2 ScrollPos;

        protected virtual string DatabasePath
        {
            get { return null; }
        }

        protected virtual string ItemName
        {
            get { return null; }
        }

        protected UtilityDatabase<TU> Database;

        protected void OnEnable()
        {
            if (Database == null)
                LoadDatabase();

            State = StateEnum.Blank;
        }

        // ReSharper disable once InconsistentNaming
        protected void OnGUI()
        {
            EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
            DisplayListArea();
            DisplayMainArea();
            EditorGUILayout.EndHorizontal();
        }

        protected void LoadDatabase()
        {
            Database = (T)AssetDatabase.LoadAssetAtPath(DatabasePath, typeof(T));

            if (Database == null)
                CreateDatabase();
        }

        protected virtual Object CreateDatabaseInstance()
        {
            return null;
        } 

        protected void CreateDatabase()
        {
            AssetDatabase.CreateAsset(CreateDatabaseInstance(), DatabasePath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Database = (T)AssetDatabase.LoadAssetAtPath(DatabasePath, typeof(T));
        }

        void DisplayListArea()
        {
            if (Database == null) LoadDatabase();

            EditorGUILayout.BeginVertical(GUILayout.Width(250));
            EditorGUILayout.Space();

            ScrollPos = EditorGUILayout.BeginScrollView(ScrollPos, "box", GUILayout.ExpandHeight(true));

            for (int cnt = 0; cnt < Database.Count; cnt++)
            {
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("-", GUILayout.Width(25)))
                {
                    Database.RemoveAt(cnt);
                    Database.SortAlphabetically();
                    EditorUtility.SetDirty(Database);
                    State = StateEnum.Blank;
                    return;
                }

                if (GUILayout.Button(Database.ElementAt(cnt).Name, "box", GUILayout.ExpandWidth(true)))
                {
                    SelectedIndex = cnt;
                    State = StateEnum.Edit;
                }

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndScrollView();

            EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
            EditorGUILayout.LabelField(string.Format("{0}s: {1}", ItemName, Database.Count), GUILayout.Width(100));

            if (GUILayout.Button(string.Format("New {0}", ItemName)))
                State = StateEnum.Add;

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();
        }

        void DisplayMainArea()
        {
            EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(true));
            EditorGUILayout.Space();

            switch (State)
            {
                case StateEnum.Add:
                    DisplayAddMainArea();
                    break;
                case StateEnum.Edit:
                    DisplayEditMainArea();
                    break;
            }

            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();
        }

        protected void DisplayEditMainArea()
        {
            DisplayEditor(Database.ElementAt(SelectedIndex));

            EditorGUILayout.Space();

            if (GUILayout.Button("Save", GUILayout.Width(100)))
            {
                Database.SortAlphabetically();
                EditorUtility.SetDirty(Database);
                State = StateEnum.Blank;
            }
        }

        protected void DisplayAddMainArea()
        {
            var newCharacter = new TU();
            DisplayEditor(newCharacter);
            EditorGUILayout.Space();

            if (GUILayout.Button("Add", GUILayout.Width(100)))
            {
                Database.Add(newCharacter);
                Database.SortAlphabetically();
                EditorUtility.SetDirty(Database);
                State = StateEnum.Blank;
            }
        }

        protected virtual void DisplayEditor(TU element)
        {
            element.Name = EditorGUILayout.TextField(new GUIContent("Name:"), element.Name);
        }
    }
}