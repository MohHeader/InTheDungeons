using System;
using Pathfinding;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Game.Scripts.Battle.Presenter {
    public class SelectedCharacterPresenter : PresenterBase<SquadPresenter> {
        private readonly CompositeDisposable _characterDisposables = new CompositeDisposable();

        protected Path LastPath;
        protected CharacterPresenter SelectedCharacter;

        protected GameObject SelectionGameObject;
        public GameObject SelectionPrefab;


        protected override IPresenter[] Children
        {
            get { return EmptyChildren; }
        }

        protected override void BeforeInitialize(SquadPresenter argument) {
        }

        private void SelectedCharacterChanged(CharacterPresenter characterPresenter) {
            _characterDisposables.Clear();
            if (SelectedCharacter != null) SelectedCharacter.Movement.ClearPrevious();

            if (characterPresenter == null) {
                SelectedCharacter = null;
                if (SelectionGameObject != null) SelectionGameObject.SetActive(false);
                return;
            }
            if (SelectionGameObject == null) SelectionGameObject = Instantiate(SelectionPrefab);

            SelectedCharacter = characterPresenter;
            SelectedCharacter.CharacterState.Subscribe(CharacterStateChanged).AddTo(_characterDisposables);
            SelectionGameObject.transform.SetParent(characterPresenter.transform, false);
            SelectionGameObject.transform.localPosition = new Vector3(0f, 0.2f, 0f);
        }

        private void CharacterStateChanged(CharacterPresenter.CharacterStateEnum characterStateEnum) {
            switch (characterStateEnum) {
                case CharacterPresenter.CharacterStateEnum.Idle:
                    ShowPossibleMovements();
                    break;
                case CharacterPresenter.CharacterStateEnum.Moving:
                    SelectedCharacter.Movement.ClearPrevious();
                    break;
                case CharacterPresenter.CharacterStateEnum.SelectingTarget:
                    SelectedCharacter.Movement.ClearPrevious();
                    break;
                case CharacterPresenter.CharacterStateEnum.UsingSkill:
                    SelectedCharacter.Movement.ClearPrevious();
                    break;
                case CharacterPresenter.CharacterStateEnum.Dead:
                    SelectedCharacter.Movement.ClearPrevious();
                    break;
                default:
                    throw new ArgumentOutOfRangeException("characterStateEnum", characterStateEnum, null);
            }
        }

        protected void ShowPossibleMovements() {
            Observable.FromCoroutine(SelectedCharacter.Movement.CalculatePossiblePaths).Subscribe(_ => { }, () => { SelectedCharacter.Movement.BuildPathMesh(); });
        }

        protected override void Initialize(SquadPresenter argument) {
            argument.SelectedCharacter.Subscribe(SelectedCharacterChanged);
        }

        public void Update() {
            if (Input.GetMouseButtonDown(0) && SelectedCharacter != null && !EventSystem.current.IsPointerOverGameObject() && SelectedCharacter.CharacterState.Value == CharacterPresenter.CharacterStateEnum.Idle) {
                RaycastHit hit;
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit))
                {
                    var closestNode = AstarPath.active.GetNearest(hit.point).node;
                    if (SelectedCharacter.Movement.CanMoveToGridNode(closestNode))
                        StartCoroutine(SelectedCharacter.Movement.MoveToGridNode(closestNode));
                }
            }
        }
    }
}