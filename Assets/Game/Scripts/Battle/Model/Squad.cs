using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Game.Scripts.Battle.Model
{
    [Serializable]
    public class Squad
    {
        [SerializeField] public List<Character> Characters = new List<Character>();
    }
}
