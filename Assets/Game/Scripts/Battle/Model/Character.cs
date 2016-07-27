using System;
using System.Collections.Generic;
using Assets.Game.Scripts.Utility.Equipment;
using UnityEngine;

namespace Assets.Game.Scripts.Battle.Model
{
    [Serializable]
    public class Character
    {
        [SerializeField] public int Id;
        [SerializeField] public int Level;
        [SerializeField] public List<EquippedItem> Items = new List<EquippedItem>();
    }
}