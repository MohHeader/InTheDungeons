using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Game.Scripts.Helpers;
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

        public GameObject TargetIndicatorPrefab;
        protected List<GameObject> TargetIndicators = new List<GameObject>();

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

            // TODO: Find targets and spawn target prefabs
            for (int i = 0; i < TargetIndicators.Count; i++) {
                Destroy(TargetIndicators[i]);
            }

            if (select) {
                var transformArray = SelectedCharacterPresenter.transform.gameObject.GetDefenderCharactersBetween(Skill.MinimumDistance, Skill.MaximumDistance);
                foreach (var targetTransform in transformArray) {
                    var prefab = Instantiate(TargetIndicatorPrefab);
                    prefab.transform.SetParent(targetTransform, false);
                    prefab.transform.localPosition = new Vector3(0f, 0.1f, 0f);
                    TargetIndicators.Add(prefab);
                }
            }
        }

        private void SkillSelection(bool b) {
            if (SelectedCharacterPresenter == null) return;

            SelectedCharacterPresenter.SelectedSkill.Value = SelectingTarget.Value ? Skill : null;
        }

        protected Subject<Unit> SelectionSubject = new Subject<Unit>();

        public void OnSelected() {
            SelectionSubject.OnNext(Unit.Default);
        }
    }
}