using UnityEngine;

namespace Assets.Game.Scripts.Utility.Characters
{
    public class DataLayer : MonoBehaviour
    {
        public CharactersDatabase Database;
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