using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Game.Scripts.Battle.Model;
using DungeonArchitect;
using DungeonArchitect.Utils;
using UniRx;
using UnityEngine;

namespace Assets.Game.Scripts.Battle.Presenter
{
    public class SquadPresenter : PresenterBase<Squad>
    {
        public GameObject CharacterPrefab;
        public Vector3[] Deltas;
        public List<CharacterPresenter> Characters;
        protected Squad Squad;

        protected override IPresenter[] Children
        {
            get { return EmptyChildren; }
        }

        protected override void BeforeInitialize(Squad argument)
        {
            if (argument == null || argument.Characters == null || argument.Characters.Count == 0) 
                throw new Exception("Squad not configured");

            Squad = argument;
            Characters = new List<CharacterPresenter>();
        }

        protected override void Initialize(Squad argument)
        {
            var dungeon = FindObjectOfType<DungeonArchitect.Dungeon>();
            var grid = (dungeon.ActiveModel as GridDungeonModel);
            var startCell = GridDungeonModelUtils.FindFurthestRooms(grid)[0];
            var roomCenter = MathUtils.GridToWorld(grid.Config.GridCellSize, startCell.CenterF);
            var index = 0;

            foreach (var character in Squad.Characters)
            {
                var instance = Instantiate(CharacterPrefab);
                var characterInstance = instance.GetComponent<CharacterPresenter>();
                characterInstance.PropagateArgument(character);
                Characters.Add(characterInstance);

                var spawnNode = AstarPath.active.GetNearest(roomCenter + Deltas[index]);
                instance.transform.position = (Vector3)spawnNode.node.position;
                index++;
            }
        }
    }
}
