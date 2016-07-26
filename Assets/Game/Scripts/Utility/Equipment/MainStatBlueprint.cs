using System;
using Assets.Game.Scripts.Utility.Characters;
using UnityEngine;

namespace Assets.Game.Scripts.Utility.Equipment {
    [Serializable]
    public class MainStatBlueprint {
        [SerializeField] public MainStatEnum MainStat;
        [SerializeField] public int Minimum;
        [SerializeField] public int Maximum;
        [SerializeField] public float Probability;
    }
}