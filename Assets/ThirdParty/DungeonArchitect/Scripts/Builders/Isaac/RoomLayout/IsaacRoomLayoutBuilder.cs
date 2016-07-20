using UnityEngine;
using System.Collections;
using Assets.Game.Scripts.Utility.Common;

namespace DungeonArchitect.Builders.Isaac
{
    public abstract class IsaacRoomLayoutBuilder : MonoBehaviour
    {

        public abstract IsaacRoomLayout GenerateLayout(IsaacRoom room, FastRandom random, int roomWidth, int roomHeight);
    }
}