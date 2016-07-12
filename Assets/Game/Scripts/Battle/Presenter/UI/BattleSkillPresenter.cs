using Assets.Game.Scripts.Battle.Model;
using UniRx;
using UnityEngine.UI;

namespace Assets.Game.Scripts.Battle.Presenter.UI
{
    public class BattleSkillPresenter : PresenterBase<SquadPresenter>
    {
        public Image Icon;
        public int SkillIndex;
        protected override IPresenter[] Children
        {
            get { return EmptyChildren; }
        }

        protected override void BeforeInitialize(SquadPresenter argument)
        {
        }

        private void CharacterChanged(CharacterPresenter characterPresenter)
        {
            if (characterPresenter == null || characterPresenter.Skills.Length <= SkillIndex ||
                characterPresenter.Skills[SkillIndex] == null)
            {
                Icon.sprite = null;
                Icon.enabled = false;
            }
            else
            {
                Icon.enabled = true;
                Icon.sprite = characterPresenter.Skills[SkillIndex].Icon;
            }
        }

        protected override void Initialize(SquadPresenter argument)
        {
            argument.SelectedCharacter.Subscribe(CharacterChanged);
        }
    }
}