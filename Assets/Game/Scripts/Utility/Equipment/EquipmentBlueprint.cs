using System;
using System.Collections.Generic;
using Assets.Dungeon.Scripts.Utility.Common;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

#else
using Assets.Game.Scripts.Helpers;
#endif

namespace Assets.Game.Scripts.Utility.Equipment {
    [Serializable]
    public class EquipmentBlueprint : ACommonData {
        [SerializeField] public string AssetPath;

        [SerializeField] public EquipmentTypeEnum EquipmentType;

        [SerializeField] public string IconPath;

        [SerializeField] public List<MainStatBlueprint> MainStats = new List<MainStatBlueprint>();

        [SerializeField] public int RequiredLevel;

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
    }
}