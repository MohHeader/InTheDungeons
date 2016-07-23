using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Game.Scripts.Battle.Model;
using Assets.Game.Scripts.Battle.Presenter.UI;
using DungeonArchitect;
using DungeonArchitect.Utils;
using OrbCreationExtensions;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Game.Scripts.Battle.Presenter {
    public class SquadPresenter : PresenterBase<Squad> {
        public GameObject CharacterPrefab;
        public List<CharacterPresenter> Characters;
        public Vector3[] Deltas;

        public GameObject PortraightPrefab;
        public Transform PortraightsRoot;

        public ReactiveProperty<CharacterPresenter> SelectedCharacter;
        public SelectedCharacterPresenter SelectedCharacterPresenter;

        public ReactiveProperty<SquadStateEnum> SquadState = new ReactiveProperty<SquadStateEnum>(SquadStateEnum.None);

        public BattleSkillPresenter[] Skills;
        protected Squad Squad;

        protected override IPresenter[] Children
        {
            get
            {
                return
                    Skills.Select(_ => (IPresenter) _)
                        .Union(new List<IPresenter> {SelectedCharacterPresenter})
                        .ToArray();
            }
        }

        protected override void BeforeInitialize(Squad argument) {
            SelectedCharacter = new ReactiveProperty<CharacterPresenter>();
            SelectedCharacter.Subscribe(SelectedCharacterChanged);

            if (argument == null || argument.Characters == null || argument.Characters.Count == 0)
                throw new Exception("Squad not configured");

            Squad = argument;
            Characters = new List<CharacterPresenter>();

            var dungeon = FindObjectOfType<DungeonArchitect.Dungeon>();
            var grid = dungeon.ActiveModel as GridDungeonModel;
            var furthestrooms = GridDungeonModelUtils.FindFurthestRooms(grid);
            var startCell = furthestrooms.OrderBy(_ => _.Id).ElementAt(0);
            var roomCenter = MathUtils.GridToWorld(grid.Config.GridCellSize, startCell.CenterF);
            var index = 0;

            foreach (var character in Squad.Characters)
            {
                var instance = Instantiate(CharacterPrefab);
                var characterInstance = instance.GetComponent<CharacterPresenter>();
                characterInstance.CharacterSide = CharacterPresenter.CharacterSideEnum.Attacker;
                characterInstance.ForceInitialize(character);
                Characters.Add(characterInstance);

                var spawnNode = AstarPath.active.GetNearest(roomCenter + Deltas[index]);
                instance.transform.position = (Vector3) spawnNode.node.position;
                index++;
            }
            SelectedCharacterPresenter.PropagateArgument(this);
            foreach (var battleSkillPresenter in Skills)
            {
                battleSkillPresenter.PropagateArgument(this);
            }
        }

        private void SelectedCharacterChanged(CharacterPresenter characterPresenter) {
            // TODO: Show skills???
        }

        protected override void Initialize(Squad argument) {
            SpawnPortraights();
            SquadState.Subscribe(SquadStateChanged);

            foreach (var characterPresenter in Characters)
            {
                characterPresenter.CharacterData.RemainingActionPoint.Subscribe(CheckForActionPointsLeft);
            }
        }

        private void CheckForActionPointsLeft(int remaining) {
            if (SquadState.Value != SquadStateEnum.InProgress || remaining > 0) return;

            var remainingActionPoints = Characters.All(_ => _.CharacterData.RemainingActionPoint.Value <= 0);
            if (remainingActionPoints)
            {
                EndTurn();
            }
        }

        protected void SpawnPortraights() {
            foreach (var characterPresenter in Characters)
            {
                var portraight = Instantiate(PortraightPrefab);
                portraight.transform.SetParent(PortraightsRoot, false);
                var presenter = portraight.GetComponent<CharacterPortraightPresenter>();
                presenter.SquadPresenter = this;
                presenter.ForceInitialize(characterPresenter);
            }
        }

        public void Update() {
            // For debugging purposes
            if (Input.GetMouseButtonUp(0) && !EventSystem.current.IsPointerOverGameObject())
            {
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, float.PositiveInfinity))
                {
                    var character = hit.transform.GetComponent<CharacterPresenter>();
                    if (character != null)
                    {
                        if (CommandSelectCharacter(character)) return;
                        if (CommandSelectSpellTarget(character)) return;
                    }
                    else
                    {
                        if (CommandSetRoutePath(hit)) return;
                    }
                    Debug.LogWarning("Inappropriate action!");
                }
            }
        }

        protected void SelectCharacter(CharacterPresenter character) {
            SelectedCharacter.SetValueAndForceNotify(character);
        }

        #region Player battle commands

        public bool CommandSelectCharacter(CharacterPresenter newSelection) {
            // Условия: выбран персонаж в состоянии Idle + выбираемый персонаж имеет очки действия + выбираемый персонаж находится на той же стороне что и игрок
            if (newSelection == null || !SelectedCharacter.HasValue) return false;
            if (SelectedCharacter.Value.CharacterState.Value != CharacterPresenter.CharacterStateEnum.Idle || newSelection.CharacterState.Value != CharacterPresenter.CharacterStateEnum.Idle)
                return false;
            if (newSelection.CharacterData.RemainingActionPoint.Value <= 0) return false;
            if (newSelection.CharacterSide == CharacterPresenter.CharacterSideEnum.Defender) return false;
            SelectCharacter(newSelection);
            return true;
        }

        public bool CommandSelectSpellTarget(CharacterPresenter newSelection)
        {
            // Условия: выбран персонаж в состоянии SelectingTarget + выбрана подходящая цель + цель находится на дистанции поражения
            if (newSelection == null || !SelectedCharacter.HasValue) return false;
            if (SelectedCharacter.Value.CharacterState.Value != CharacterPresenter.CharacterStateEnum.SelectingTarget || newSelection.CharacterState.Value != CharacterPresenter.CharacterStateEnum.Idle)
                return false;
            if (newSelection.CharacterSide != CharacterPresenter.CharacterSideEnum.Defender) return false;

            var distance = Vector3.Distance(SelectedCharacter.Value.transform.position, newSelection.transform.position).Round(2);
            var skill = SelectedCharacter.Value.SelectedSkill.Value;
            if (distance > skill.MaximumDistance || distance < skill.MinimumDistance) return false;

            SelectedCharacter.Value.UseSelectedSkill(newSelection);
            return true;
        }

        public bool CommandSetRoutePath(RaycastHit hit) {
            // Условия: выбран персонаж в состоянии Idle + нажатие произошло по сетке перемещения
            if (!SelectedCharacter.HasValue) return false;
            if (SelectedCharacter.Value.CharacterState.Value != CharacterPresenter.CharacterStateEnum.Idle) return false;
            var closestNode = AstarPath.active.GetNearest(hit.point).node;
            if (closestNode == null) return false;
            if (!SelectedCharacter.Value.Movement.CanMoveToGridNode(closestNode)) return false;
            StartCoroutine(SelectedCharacter.Value.Movement.MoveToGridNode(closestNode));
            return true;
        }

        #endregion

        #region Battle state management

        private void SquadStateChanged(SquadStateEnum squadStateEnum) {
            switch (squadStateEnum)
            {
                case SquadStateEnum.None:
                    break;
                case SquadStateEnum.Started:
                    foreach (var characterPresenter in Characters)
                    {
                        characterPresenter.StartNewTurn();
                    }
                    SquadState.Value = SquadStateEnum.InProgress;
                    break;
                case SquadStateEnum.InProgress:
                    SelectCharacter(Characters.First());
                    break;
                case SquadStateEnum.Finished:
                    break;
                default:
                    throw new ArgumentOutOfRangeException("squadStateEnum", squadStateEnum, null);
            }
        }

        private void EndTurn() {
            SelectCharacter(null);
            foreach (var characterPresenter in Characters)
            {
                characterPresenter.EndTurn();
            }
            SquadState.Value = SquadStateEnum.Finished;
        }
        #endregion
    }
}