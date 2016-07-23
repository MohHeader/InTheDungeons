﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Game.Scripts.Battle.Components;
using Assets.Game.Scripts.Battle.Model;
using Assets.Game.Scripts.Battle.Model.Skills;
using Assets.Game.Scripts.Battle.Presenter.Interfaces;
using Assets.Game.Scripts.Battle.Presenter.UI;
using Assets.Game.Scripts.Common;
using Assets.Game.Scripts.Helpers;
using Assets.Game.Scripts.Utility.Characters;
using Assets.Game.Scripts.Utility.Skills;
using DG.Tweening;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace Assets.Game.Scripts.Battle.Presenter {
    public class CharacterPresenter : PresenterBase<Character>, IActor {
        public enum CharacterSideEnum {
            Attacker,
            Defender
        }

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

        public CharacterSideEnum CharacterSide;

        public ReactiveProperty<CharacterStateEnum> CharacterState = new ReactiveProperty<CharacterStateEnum>();
        protected CharacterController Controller;
        public GridMovementBehaviour Movement;
        public ReactiveProperty<SkillData> SelectedSkill = new ReactiveProperty<SkillData>();

        public BattleSkill[] Skills;

        public GameObject SpellTarget;
        public BattleCharacterStatusPresenter StatusPresenter;

        protected override IPresenter[] Children
        {
            get
            {
                return new IPresenter[]
                {
                    StatusPresenter
                };
            }
        }

        protected override void BeforeInitialize(Character argument) {
            Movement = GetComponent<GridMovementBehaviour>();
            Character = argument;
            SelectedSkill.Value = null;
            var instance = DataLayer.GetInstance();
            CharacterData = new CharacterStatusPresenter(instance.Database.GetCharacterData(Character.Id),
                Character.Level);
            var prefab = Instantiate(CharacterData.Asset);
            prefab.transform.SetParent(transform, false);
            Animator = gameObject.FindAnimatorComponent();
            Controller = gameObject.FindCharacterControllerComponent();
            Skills = CharacterData.Skills.Select(_ => new BattleSkill(_)).ToArray();
            StatusPresenter.PropagateArgument(CharacterData);
            CharacterData.CharacterState.Subscribe(_ => StartCoroutine(AliveStateChanged(_)));
        }

        private IEnumerator AliveStateChanged(CharacterStatusPresenter.CharactersStateEnum charactersStateEnum) {
            if (charactersStateEnum == CharacterStatusPresenter.CharactersStateEnum.Dead)
            {
                CharacterState.SetValueAndForceNotify(CharacterStateEnum.Dead);
                Destroy(gameObject.GetComponent<Collider>());
                AstarPath.active.Scan();
                Animator.SetBool("Dead", true);
                yield return new WaitForSeconds(3f);
                DestroyImmediate(StatusPresenter);
                DestroyImmediate(gameObject);
            }
        }

        protected override void Initialize(Character argument) {
            CharacterState.SetValueAndForceNotify(CharacterStateEnum.Idle);
            SelectedSkill.Subscribe(_ =>
            {
                CharacterState.SetValueAndForceNotify(_ != null
                    ? CharacterStateEnum.SelectingTarget
                    : CharacterStateEnum.Idle);
            });
            Movement.Initialize();
        }

        #region Skill Player

        public void UseSelectedSkill(CharacterPresenter target) {
            var skill = SelectedSkill.Value;
            if (skill == null) return;

            if (CharacterData.RemainingActionPoint.Value < skill.ActionPoints) return;

            switch (skill.SkillType)
            {
                case SkillTypeEnum.Melee:
                    StartCoroutine(PlayMeleeSkill(skill, target));
                    break;
                case SkillTypeEnum.Shooting:
                    break;
                case SkillTypeEnum.Spell:
                    switch (skill.SpellType)
                    {
                        case SpellTypeEnum.Projectile:
                            StartCoroutine(PlayProjectileSpell(skill, target));
                            break;
                        case SpellTypeEnum.Direction:
                            StartCoroutine(PlayDirectionSpell(skill, target));
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            CharacterData.RemainingActionPoint.Value -= skill.ActionPoints;
            skill.RemainingCooldown = skill.Cooldown;
        }

        public IEnumerator PlayMeleeSkill(SkillData skill, CharacterPresenter target) {
            CharacterState.SetValueAndForceNotify(CharacterStateEnum.UsingSkill);

            transform.LookAt(target.transform);
            Animator.SetTrigger(TriggerEnumToTriggerName(skill.TriggerName));
            var subscriber =
                Observable.Timer(TimeSpan.FromSeconds(skill.InflictDamageTime), Scheduler.MainThreadIgnoreTimeScale)
                    .Subscribe(_ => target.CharacterData.DealDamage(skill.DamageMultiplier*CharacterData.Damage.Value));
            yield return new WaitForSeconds(0.4f);
            while (!Animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
            {
                yield return new WaitForEndOfFrame();
            }

            yield return new WaitForFixedUpdate();
            subscriber.Dispose();
            CharacterState.SetValueAndForceNotify(CharacterStateEnum.Idle);
            SelectedSkill.Value = null;
        }

        public IEnumerator PlayProjectileSpell(SkillData skill, CharacterPresenter target) {
            var isCollided = false;
            CharacterState.SetValueAndForceNotify(CharacterStateEnum.UsingSkill);

            var spellTarget = Instantiate(SpellTarget);
            spellTarget.transform.SetParent(target.transform, false);

            transform.LookAt(target.transform);

            var spellPrefab = Instantiate(skill.Prefab);
            var collisionTrigger = spellTarget.GetComponent<ObservableTriggerTrigger>();
            var subscriber = collisionTrigger.OnTriggerEnterAsObservable().Subscribe(_ =>
            {
                target.CharacterData.DealDamage(skill.DamageMultiplier*CharacterData.Damage.Value);
                isCollided = true;
            });

            spellPrefab.transform.position = transform.position + new Vector3(0f, 1f, 0f);

            Animator.SetTrigger(TriggerEnumToTriggerName(skill.TriggerName));
            yield return new WaitForSeconds(skill.SpawnTime);

            spellPrefab.transform.DOMove(
                spellTarget.GetComponent<CapsuleCollider>().center + spellTarget.transform.position,
                Vector3.Distance(spellTarget.transform.position, spellPrefab.transform.position)/
                skill.SpellMovementSpeed);

            while (!Animator.GetCurrentAnimatorStateInfo(0).IsName("Idle") || !isCollided)
            {
                yield return new WaitForEndOfFrame();
            }

            yield return new WaitForFixedUpdate();
            CharacterState.SetValueAndForceNotify(CharacterStateEnum.Idle);

            subscriber.Dispose();
            DestroyImmediate(spellTarget);
            SelectedSkill.Value = null;
        }

        public IEnumerator PlayDirectionSpell(SkillData skill, CharacterPresenter target) {
            CharacterState.SetValueAndForceNotify(CharacterStateEnum.UsingSkill);

            var disposables = new List<IDisposable>();
            var spellTargets = new List<GameObject>();
            var hitTargets = new List<Transform>();

            var transformArray = gameObject.GetCharactersBetween(skill.MinimumDistance, skill.MaximumDistance);
            foreach (var targetTransform in transformArray)
            {
                var spellTarget = Instantiate(SpellTarget);
                spellTarget.transform.SetParent(targetTransform, false);
                var collisionTrigger = spellTarget.GetComponent<ObservableTriggerTrigger>();
                var copy = targetTransform;
                var subscriber = collisionTrigger.OnTriggerEnterAsObservable().SubscribeWithState(copy, (c, t) =>
                {
                    if (!hitTargets.Contains(t))
                    {
                        t.GetComponent<CharacterPresenter>()
                            .CharacterData.DealDamage(skill.DamageMultiplier*
                                                      CharacterData.Damage
                                                          .Value);
                        hitTargets.Add(t);
                    }
                });
                spellTargets.Add(spellTarget);
                disposables.Add(subscriber);
            }

            transform.LookAt(target.transform);

            Animator.SetTrigger(TriggerEnumToTriggerName(skill.TriggerName));
            yield return new WaitForSeconds(skill.SpawnTime);

            Instantiate(skill.Prefab, transform.position + transform.forward, transform.rotation);

            while (!Animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
            {
                yield return new WaitForEndOfFrame();
            }

            yield return new WaitForFixedUpdate();
            CharacterState.SetValueAndForceNotify(CharacterStateEnum.Idle);

            for (var disposable = 0; disposable < disposables.Count; disposable++)
            {
                disposables[disposable].Dispose();
            }

            for (var spellTarget = 0; spellTarget < spellTargets.Count; spellTarget++)
            {
                DestroyImmediate(spellTargets[spellTarget]);
            }

            SelectedSkill.Value = null;
        }

        protected string TriggerEnumToTriggerName(TriggerEnum trigger) {
            switch (trigger)
            {
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

        public void StartNewTurn() {
            CharacterData.RegenerateActionPoints();
            foreach (var skillData in CharacterData.Skills)
            {
                if (skillData.Cooldown > 0) skillData.Cooldown--;
            }
        }

        public void EndTurn() {
            CharacterData.RemainingActionPoint.Value = 0;
        }

        #endregion

        public void ReduceActionPoints(int amount) {
            CharacterData.RemainingActionPoint.Value = Mathf.Clamp(CharacterData.RemainingActionPoint.Value - amount, 0,
                CharacterData.RemainingActionPoint.Value);
            Debug.LogWarning(CharacterData.RemainingActionPoint.Value);
        }
    }
}