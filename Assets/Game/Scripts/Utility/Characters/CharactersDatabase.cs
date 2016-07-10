using System.Linq;
using Assets.Game.Scripts.Utility.Common;

namespace Assets.Game.Scripts.Utility.Characters
{
    public class CharactersDatabase : UtilityDatabase<CharacterData>
    {
        public CharacterData GetCharacterData(int id)
        {
            return Database.SingleOrDefault(_ => _.Id == id);
        }
    }
}