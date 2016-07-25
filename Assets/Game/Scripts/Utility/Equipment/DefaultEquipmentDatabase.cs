using System;
using UnityEngine;

namespace Assets.Game.Scripts.Utility.Equipment {
    public class DefaultEquipmentDatabase : ScriptableObject {
        [SerializeField] public string[] Database;
        protected void OnEnable()
        {
            if (Database != null) return;
            var size = Enum.GetValues(typeof(EquipmentTypeEnum)).Length;

            Database = new string[size];
        }
    }
}