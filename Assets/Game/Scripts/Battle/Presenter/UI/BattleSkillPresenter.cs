using System;
using Assets.Game.Scripts.Utility.Skills;
using OrbCreationExtensions;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Game.Scripts.Battle.Presenter.UI {
    public class BattleSkillPresenter : PresenterBase<SquadPresenter> {
        private readonly CompositeDisposable _characterDisposables = new CompositeDisposable();

        public Image Icon;
        public Image Border;

        public Sprite UnselectedSprite;
        public Sprite SelectedSprite;

        protected CharacterPresenter SelectedCharacterPresenter;

        protected ReactiveProperty<bool> SelectingTarget = new ReactiveProperty<bool>(false);
        protected SkillData Skill;
        public int SkillIndex;

        protected override IPresenter[] Children
        {
            get { return EmptyChildren; }
        }

        protected override void BeforeInitialize(SquadPresenter argument) {
            SelectionSubject.Throttle(TimeSpan.FromMilliseconds(250)).Subscribe(_ => { SelectingTarget.Value = !SelectingTarget.Value; });
        }

        private void CharacterChanged(CharacterPresenter characterPresenter) {
            _characterDisposables.Clear();
            ToggleSkillSelection(false);

            SelectedCharacterPresenter = characterPresenter;
            SelectingTarget.Value = false;
            if (characterPresenter == null || characterPresenter.Skills.Length <= SkillIndex ||
                characterPresenter.Skills[SkillIndex] == null)
            {
                Skill = null;
                Icon.sprite = null;
                Icon.enabled = false;

                Border.sprite = null;
                Border.enabled = false;
            }
            else
            {
                characterPresenter.SelectedSkill.Subscribe(SelectedSkill).AddTo(_characterDisposables);
                Skill = characterPresenter.Skills[SkillIndex].SkillData;
                Icon.enabled = true;
                Icon.sprite = characterPresenter.Skills[SkillIndex].Icon;

                Border.enabled = true;
                Border.sprite = UnselectedSprite;
            }
        }

        protected override void Initialize(SquadPresenter argument) {
            argument.SelectedCharacter.Subscribe(CharacterChanged);
            SelectingTarget.Subscribe(SkillSelection);
        }

        private void SelectedSkill(SkillData skillData) {
            if (skillData == null) ToggleSkillSelection(false);
            else ToggleSkillSelection(skillData == Skill);
        }

        private void ToggleSkillSelection(bool select) {
            Border.sprite = select ? SelectedSprite : UnselectedSprite;
            SelectingTarget.Value = select;
        }

        private void SkillSelection(bool b) {
            if (SelectedCharacterPresenter == null) return;

            SelectedCharacterPresenter.SelectedSkill.Value = SelectingTarget.Value ? Skill : null;
        }

        protected Subject<Unit> SelectionSubject = new Subject<Unit>();

        public void OnSelected() {
            SelectionSubject.OnNext(Unit.Default);
        }

        protected void Update() {
            if (SelectingTarget.Value)
            {
                if (Input.GetMouseButtonUp(0) && !EventSystem.current.IsPointerOverGameObject())
                {
                    var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit, float.PositiveInfinity))
                    {
                        var character = hit.transform.GetComponent<CharacterPresenter>();
                        if (character != null)
                        {
                            var distance = Vector3.Distance(SelectedCharacterPresenter.transform.position,
                                character.transform.position).Round(2);
                            if (distance <= Skill.MaximumDistance && distance >= Skill.MinimumDistance)
                            {
                                switch (Skill.SkillType)
                                {
                                    case SkillTypeEnum.Melee:
                                        StartCoroutine(SelectedCharacterPresenter.PlayMeleeSkill(Skill, character));
                                        break;
                                    case SkillTypeEnum.Shooting:
                                        break;
                                    case SkillTypeEnum.Spell:
                                        switch (Skill.SpellType)
                                        {
                                            case SpellTypeEnum.Projectile:
                                                StartCoroutine(SelectedCharacterPresenter.PlayProjectileSpell(Skill, character));
                                                break;
                                            case SpellTypeEnum.Direction:
                                                StartCoroutine(SelectedCharacterPresenter.PlayDirectionSpell(Skill, character));
                                                break;
                                            default:
                                                throw new ArgumentOutOfRangeException();
                                        }
                                        break;
                                    default:
                                        throw new ArgumentOutOfRangeException();
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}