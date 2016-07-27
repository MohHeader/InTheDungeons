using System;

namespace Assets.Game.Scripts.Utility.Equipment {
    [Serializable]
    public class EquippedItem {
        public EquipmentItem Item;
        public WearSlotEnum Slot;

        public void Initialize() {
            Item.ForceInitialize();
        }
    }
}