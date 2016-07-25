using Assets.Game.Scripts.Utility.Equipment;
using UnityEngine;

namespace Assets.Game.Scripts.Utility.Characters
{
    public class DataLayer : MonoBehaviour
    {
        public CharactersDatabase CharactersDatabase;
        public DefaultEquipmentDatabase DefaultEquipmentDatabase;

        private static DataLayer _instance;

        public static DataLayer GetInstance()
        {
            return _instance;
        }

        public void Awake()
        {
            _instance = this;
        }
    }
}