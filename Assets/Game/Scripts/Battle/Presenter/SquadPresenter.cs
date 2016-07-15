using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Game.Scripts.Battle.Model;
using Assets.Game.Scripts.Battle.Presenter.UI;
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

        public ReactiveProperty<CharacterPresenter> SelectedCharacter;
        public SelectedCharacterPresenter SelectedCharacterPresenter;

        public GameObject PortraightPrefab;
        public Transform PortraightsRoot;

        public BattleSkillPresenter[] Skills;

        protected override IPresenter[] Children
        {
            get { return Skills.Select(_ => (IPresenter)_).Union(new List<IPresenter> { SelectedCharacterPresenter}).ToArray(); }
        }

        protected override void BeforeInitialize(Squad argument)
        {
            SelectedCharacter = new ReactiveProperty<CharacterPresenter>();
            SelectedCharacter.Subscribe(SelectedCharacterChanged);

            if (argument == null || argument.Characters == null || argument.Characters.Count == 0) 
                throw new Exception("Squad not configured");

            Squad = argument;
            Characters = new List<CharacterPresenter>();

            var dungeon = FindObjectOfType<DungeonArchitect.Dungeon>();
            var grid = (dungeon.ActiveModel as GridDungeonModel);
            var furthestrooms = GridDungeonModelUtils.FindFurthestRooms(grid);
            foreach (var furthestroom in furthestrooms) {
                Debug.LogFormat("Room id {0}", furthestroom.Id);
            }
            var startCell = furthestrooms.OrderBy(_ => _.Id).ElementAt(0);
            Debug.LogFormat("Selected {0} room for start", startCell.Id);
            var roomCenter = MathUtils.GridToWorld(grid.Config.GridCellSize, startCell.CenterF);
            var index = 0;

            foreach (var character in Squad.Characters)
            {
                var instance = Instantiate(CharacterPrefab);
                var characterInstance = instance.GetComponent<CharacterPresenter>();
                characterInstance.ForceInitialize(character);
                Characters.Add(characterInstance);

                var spawnNode = AstarPath.active.GetNearest(roomCenter + Deltas[index]);
                instance.transform.position = (Vector3)spawnNode.node.position;
                index++;
            }
            SelectedCharacterPresenter.PropagateArgument(this);
            foreach (var battleSkillPresenter in Skills)
            {
                battleSkillPresenter.PropagateArgument(this);
            }
        }

        private void SelectedCharacterChanged(CharacterPresenter characterPresenter)
        {
            // TODO: Show skills???
        }

        protected override void Initialize(Squad argument)
        {
            SpawnPortraights();

            SelectCharacter(Characters.First());
        }

        protected void SpawnPortraights()
        {
            foreach (var characterPresenter in Characters)
            {
                var portraight = Instantiate(PortraightPrefab);
                portraight.transform.SetParent(PortraightsRoot, false);
                var presenter = portraight.GetComponent<CharacterPortraightPresenter>();
                presenter.SquadPresenter = this;
                presenter.ForceInitialize(characterPresenter);
            }
        }

        public void Update()
        {
            // For debugging purposes
            if (Input.GetMouseButtonUp(0))
            {
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, float.PositiveInfinity))
                {
                    var character = hit.transform.GetComponent<CharacterPresenter>();
                    if (Characters.Contains(character))
                        SelectCharacter(character);
                }
            }
        }

        public void SelectCharacter(CharacterPresenter character)
        {
            if (character != null)
                SelectedCharacter.SetValueAndForceNotify(character);
        }
    }
}
