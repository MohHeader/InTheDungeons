using System;
using UnityEngine;

namespace Assets.Game.Scripts.Battle.Model
{
    [Serializable]
    public class Character
    {
        [SerializeField] public int Id;

        [SerializeField] public int Level;
    }
}