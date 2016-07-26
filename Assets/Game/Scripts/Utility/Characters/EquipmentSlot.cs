using System;
using System.Collections.Generic;
using Assets.Game.Scripts.Utility.Equipment;
using UnityEngine;

namespace Assets.Game.Scripts.Utility.Characters {
    [Serializable]
    public class EquipmentSlot {
        [SerializeField] public WearSlotEnum Slot;
        [SerializeField] public List<EquipmentTypeEnum> PossibleEquipmentTypes;

        public EquipmentSlot() {
            PossibleEquipmentTypes = new List<EquipmentTypeEnum>();
        }
    }
}