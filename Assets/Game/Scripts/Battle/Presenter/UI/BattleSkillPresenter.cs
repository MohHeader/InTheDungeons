using Assets.Game.Scripts.Utility.Skills;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
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

        protected CharacterPresenter SelectedCharacterPresenter;
        protected SkillData Skill;

        private void CharacterChanged(CharacterPresenter characterPresenter) {
            SelectedCharacterPresenter = characterPresenter;
            SelectingCharacter.Value = false;
            if (characterPresenter == null || characterPresenter.Skills.Length <= SkillIndex ||
                characterPresenter.Skills[SkillIndex] == null) {
                Skill = null;
                Icon.sprite = null;
                Icon.enabled = false;
            }
            else {
                Skill = characterPresenter.Skills[SkillIndex].SkillData;
                Icon.enabled = true;
                Icon.sprite = characterPresenter.Skills[SkillIndex].Icon;
            }
        }

        protected override void Initialize(SquadPresenter argument)
        {
            argument.SelectedCharacter.Subscribe(CharacterChanged);
            SelectingCharacter.Subscribe(_ => SkillSelection(_));
        }

        private void SkillSelection(bool b) {
            Icon.color = b ? Color.blue : Color.white;
        }

        protected ReactiveProperty<bool> SelectingCharacter = new ReactiveProperty<bool>(false);

        public void OnSelected() {
            SelectingCharacter.Value = !SelectingCharacter.Value;
        }

        protected void Update() {
            if (SelectingCharacter.Value) {
                if (Input.GetMouseButtonUp(0) && !EventSystem.current.IsPointerOverGameObject())
                {
                    var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit, float.PositiveInfinity))
                    {
                        var character = hit.transform.GetComponent<CharacterPresenter>();
                        if (character != null) {
                            var distance = Vector3.Distance(SelectedCharacterPresenter.transform.position, character.transform.position);
                            if (distance <= Skill.MaximumDistance && distance >= Skill.MinimumDistance)
                                StartCoroutine(SelectedCharacterPresenter.PlayTargetedSkill(Skill, character));
                        }
                    }
                }
            }
        }
    }
}