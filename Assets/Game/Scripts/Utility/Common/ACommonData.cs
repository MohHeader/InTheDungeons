using System;
using UnityEngine;

namespace Assets.Dungeon.Scripts.Utility.Common {
    [Serializable]
    public abstract class ACommonData {
        [SerializeField]
        public int Id;
        [SerializeField]
        public string Name;
    }
}