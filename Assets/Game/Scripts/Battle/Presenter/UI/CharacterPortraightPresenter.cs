using UniRx;
using UnityEngine.UI;

namespace Assets.Game.Scripts.Battle.Presenter.UI
{
    public class CharacterPortraightPresenter : PresenterBase<CharacterPresenter>
    {
        public Image Portraight;
        public SquadPresenter SquadPresenter;
        protected CharacterPresenter CharacterPresenter;

        protected override IPresenter[] Children
        {
            get { return EmptyChildren; }
        }

        protected override void BeforeInitialize(CharacterPresenter argument)
        {
        }

        protected override void Initialize(CharacterPresenter argument)
        {
            CharacterPresenter = argument;
            Portraight.sprite = argument.CharacterData.Icon;
        }

        public void PortraightClicked()
        {
            SquadPresenter.SelectCharacter(CharacterPresenter);
        }
    }
}