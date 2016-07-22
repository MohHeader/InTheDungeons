using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Game.Scripts.Battle.Presenter.UI {
    public class CharacterPortraightPresenter : PresenterBase<CharacterPresenter> {
        protected CharacterPresenter CharacterPresenter;
        public Image Portraight;
        public SquadPresenter SquadPresenter;

        protected override IPresenter[] Children
        {
            get { return EmptyChildren; }
        }

        protected override void BeforeInitialize(CharacterPresenter argument) {
        }

        protected override void Initialize(CharacterPresenter argument) {
            CharacterPresenter = argument;
            Portraight.sprite = argument.CharacterData.Icon;
            CharacterPresenter.CharacterState.Subscribe(CharacterStateChanged);
        }

        private void CharacterStateChanged(CharacterPresenter.CharacterStateEnum characterStateEnum) {
            switch (characterStateEnum)
            {
                case CharacterPresenter.CharacterStateEnum.Idle:
                    break;
                case CharacterPresenter.CharacterStateEnum.Moving:
                    break;
                case CharacterPresenter.CharacterStateEnum.SelectingTarget:
                    break;
                case CharacterPresenter.CharacterStateEnum.UsingSkill:
                    break;
                case CharacterPresenter.CharacterStateEnum.Dead:
                    Portraight.color = Color.red;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("characterStateEnum", characterStateEnum, null);
            }
        }

        public void PortraightClicked() {
            SquadPresenter.CommandSelectCharacter(CharacterPresenter);
        }
    }
}