using System;
using UnityEngine;

namespace Assets.Game.Scripts.Battle.Model {
    [Serializable]
    public class Defender : Character {
        [SerializeField] public Vector3 Position;
    }
}