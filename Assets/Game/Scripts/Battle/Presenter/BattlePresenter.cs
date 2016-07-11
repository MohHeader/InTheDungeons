using System;
using System.Linq;
using Assets.Game.Scripts.Battle.Model;
using UniRx;
using UnityEngine;

namespace Assets.Game.Scripts.Battle.Presenter
{
    public class BattlePresenter : PresenterBase {
        public SquadPresenter SquadPresenter;
        public BattleCameraPresenter CameraPresenter;
        public SelectedCharacterPresenter SelectedCharacterPresenter;

        public DungeonArchitect.Dungeon Dungeon;

        public Squad PlayerSquad;
        public ReactiveProperty<CharacterPresenter> SelectedCharacter;

        protected override IPresenter[] Children
        {
            get
            {
                return new IPresenter[] {
                                            SquadPresenter,
                                            CameraPresenter,
                                            SelectedCharacterPresenter
                                        };
            }
        }

        protected override void BeforeInitialize() {
            SelectedCharacter = new ReactiveProperty<CharacterPresenter>();
            SelectedCharacter.Subscribe(SelectedCharacterChanged);

            CameraPresenter.PropagateArgument(this);
            SelectedCharacterPresenter.PropagateArgument(this);

            Dungeon.Build();
            AstarPath.active.Scan();
            SquadPresenter.PropagateArgument(PlayerSquad);
        }

        private void SelectedCharacterChanged(CharacterPresenter characterPresenter) {
        }

        protected override void Initialize() {
            SelectedCharacter.SetValueAndForceNotify(SquadPresenter.Characters.First());
        }

        public void Update() {
            // For debugging purposes
            if (Input.GetMouseButtonUp(0)) {
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, float.PositiveInfinity)) {
                    var character = hit.transform.GetComponent<CharacterPresenter>();
                    if (character != null)
                        SelectedCharacter.SetValueAndForceNotify(character);
                }
            }
        }
    }
}