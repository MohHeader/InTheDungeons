using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Game.Scripts.Battle.Model {
    [Serializable]
    public class DefenderSquad {
        [SerializeField] public List<Defender> Defenders = new List<Defender>();
    }
}