using System.Collections.Generic;
using System.Linq;
using Assets.Dungeon.Scripts.Utility.Common;
using UnityEngine;

namespace Assets.Game.Scripts.Utility.Common {
    public class UtilityDatabase<T>: ScriptableObject where T : ACommonData {
        [SerializeField] protected List<T> Database;
        protected void OnEnable()
        {
            if (Database == null)
                Database = new List<T>();
        }

        public void Add(T item)
        {
            item.Id = Database.Any() ? Database.Max(_ => _.Id) + 1 : 1;
            Database.Add(item);
        }

        public void Remove(T item)
        {
            Database.Remove(item);
        }

        public void RemoveAt(int index)
        {
            Database.RemoveAt(index);
        }

        public int Count
        {
            get { return Database.Count; }
        }

        public T ElementAt(int index)
        {
            return Database.ElementAt(index);
        }

        public void SortAlphabetically()
        {
            Database.Sort((x, y) => string.CompareOrdinal(x.Name, y.Name));
        }
    }
}