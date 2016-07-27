using System;
using System.Collections.Generic;
using Assets.Game.Scripts.Utility.Characters;
using Assets.Game.Scripts.Utility.Common;

namespace Assets.Game.Scripts.Utility.Equipment {
    [Serializable]
    public class EquipmentItem {
        public EquipmentBlueprint Blueprint;

        private bool Initialized;

        public List<MainStatusValue> MainStatusChanges = new List<MainStatusValue>();
        public int RandomSeed;

        public EquipmentItem() {
        }

        public EquipmentItem(EquipmentBlueprint blueprint, int seed = -1) {
            RandomSeed = seed;
            Blueprint = blueprint;
            Initialize();
        }

        public string AssetPath
        {
            get
            {
                if (!Initialized) Initialize();
                return Blueprint.AssetPath;
            }
        }

        public string IconPath
        {
            get
            {
                if (!Initialized) Initialize();
                return Blueprint.AssetPath;
            }
        }

        public EquipmentTypeEnum EquipmentType
        {
            get
            {
                if (!Initialized) Initialize();
                return Blueprint.EquipmentType;
            }
        }

        // TODO: Hack
        public void ForceInitialize() {
            Initialize();
        }

        private void Initialize() {
            if (string.IsNullOrEmpty(Blueprint.AssetPath)) {
                Blueprint = DataLayer.GetInstance().EquipmentDatabase.GetItemBlueprintData(Blueprint.Id);
            }
            var random = RandomSeed < 0 ? new FastRandom() : new FastRandom(RandomSeed);
            for (var msi = 0; msi < Blueprint.MainStats.Count; msi++) {
                var succesfull = random.NextDouble() <= Blueprint.MainStats[msi].Probability;
                if (succesfull) {
                    var msv = new MainStatusValue {
                                                      MainStat = Blueprint.MainStats[msi].MainStat,
                                                      Value = random.Range(Blueprint.MainStats[msi].Minimum, Blueprint.MainStats[msi].Maximum)
                                                  };
                    if (msv.Value != 0) MainStatusChanges.Add(msv);
                }
            }
            Initialized = true;
        }

        [Serializable]
        public class MainStatusValue {
            public MainStatEnum MainStat;
            public int Value;
        }
    }
}