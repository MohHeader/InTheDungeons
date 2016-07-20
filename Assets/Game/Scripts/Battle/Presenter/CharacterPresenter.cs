using System;
using System.Collections;
using System.Linq;
using Assets.Game.Scripts.Battle.Components;
using Assets.Game.Scripts.Battle.Model;
using Assets.Game.Scripts.Battle.Presenter.Interfaces;
using Assets.Game.Scripts.Battle.Presenter.UI;
using Assets.Game.Scripts.Common;
using Assets.Game.Scripts.Helpers;
using Assets.Game.Scripts.Utility.Characters;
using Assets.Game.Scripts.Utility.Skills;
using UniRx;
using UnityEngine;

namespace Assets.Game.Scripts.Battle.Presenter {
    public class CharacterPresenter : PresenterBase<Character>, IActor {
        public enum CharacterStateEnum {
            Idle,
            Moving,
            SelectingTarget,
            UsingSkill,
            Dead
        }

        protected Animator Animator;

        protected Character Character;

        public CharacterStatusPresenter CharacterData;

        public ReactiveProperty<CharacterStateEnum> CharacterState = new ReactiveProperty<CharacterStateEnum>();
        protected CharacterController Controller;
        public GridMovementBehaviour Movement;
        public ReactiveProperty<SkillData> SelectedSkill = new ReactiveProperty<SkillData>();

        public BattleSkill[] Skills;
        public BattleCharacterStatusPresenter StatusPresenter;

        protected override IPresenter[] Children
        {
            get
            {
                return new IPresenter[] {
                                            StatusPresenter
                                        };
            }
        }

        protected override void BeforeInitialize(Character argument) {
            Movement = GetComponent<GridMovementBehaviour>();
            Character = argument;
            SelectedSkill.Value = null;
            var instance = DataLayer.GetInstance();
            CharacterData = new CharacterStatusPresenter(instance.Database.GetCharacterData(Character.Id), Character.Level);
            var prefab = Instantiate(CharacterData.Asset);
            prefab.transform.SetParent(transform, false);
            Animator = gameObject.FindAnimatorComponent();
            Controller = gameObject.FindCharacterControllerComponent();
            Skills = CharacterData.Skills.Select(_ => new BattleSkill(_)).ToArray();
            StatusPresenter.PropagateArgument(CharacterData);
            CharacterData.CharacterState.Subscribe(_ => StartCoroutine(AliveStateChanged(_)));
        }

        private IEnumerator AliveStateChanged(CharacterStatusPresenter.CharactersStateEnum charactersStateEnum) {
            if (charactersStateEnum == CharacterStatusPresenter.CharactersStateEnum.Dead) {
                CharacterState.SetValueAndForceNotify(CharacterStateEnum.Dead);
                DestroyImmediate(gameObject.GetComponent<Collider>());
                AstarPath.active.Scan();
                Animator.SetBool("Dead", true);
                yield return new WaitForSeconds(3f);
                DestroyImmediate(StatusPresenter);
                DestroyImmediate(gameObject);
            }
        }

        protected override void Initialize(Character argument) {
            CharacterState.SetValueAndForceNotify(CharacterStateEnum.Idle);
            SelectedSkill.Subscribe(_ => {
                                        CharacterState.SetValueAndForceNotify(_ != null
                                            ? CharacterStateEnum.SelectingTarget
                                            : CharacterStateEnum.Idle);
                                    });
            Movement.Initialize();
        }

        #region Skill Player

        public IEnumerator PlayTargetedSkill(SkillData skill, CharacterPresenter target) {
            CharacterState.SetValueAndForceNotify(CharacterStateEnum.UsingSkill);

            transform.LookAt(target.transform);
            Animator.SetTrigger(TriggerEnumToTriggerName(skill.TriggerName));
            yield return new WaitForSeconds(0.4f);
            while (!Animator.GetCurrentAnimatorStateInfo(0).IsName("Idle")) {
                yield return new WaitForEndOfFrame();
            }
            target.CharacterData.DealDamage(skill.DamageMultiplier*CharacterData.Damage.Value);
            yield return new WaitForFixedUpdate();

            CharacterState.SetValueAndForceNotify(CharacterStateEnum.Idle);
            SelectedSkill.Value = null;
        }

        protected string TriggerEnumToTriggerName(TriggerEnum trigger) {
            switch (trigger) {
                case TriggerEnum.None:
                    return "";
                case TriggerEnum.Attack:
                    return "Attack";
                case TriggerEnum.Skill1:
                    return "Skill1";
                case TriggerEnum.Skill2:
                    return "Skill2";
                default:
                    throw new ArgumentOutOfRangeException("trigger", trigger, null);
            }
        }

        #endregion

        #region IActor Implementation

        public void NewTurn() {
            CharacterData.RegenerateActionPoints();
        }

        public void EndTurn() {
            CharacterData.RemainingActionPoint.Value = 0;
        }

        #endregion
    }
}