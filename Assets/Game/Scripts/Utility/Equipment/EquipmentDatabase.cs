﻿using System.Linq;
using Assets.Game.Scripts.Utility.Common;

namespace Assets.Game.Scripts.Utility.Equipment {
    public class EquipmentDatabase : UtilityDatabase<EquipmentBlueprint> {
        public EquipmentBlueprint GetItemBlueprintData(int id)
        {
            return Database.SingleOrDefault(_ => _.Id == id);
        }
    }
}