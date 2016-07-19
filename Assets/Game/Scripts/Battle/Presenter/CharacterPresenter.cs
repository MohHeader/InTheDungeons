using System;
using System.Collections;
using System.Linq;
using Assets.Game.Scripts.Battle.Model;
using Assets.Game.Scripts.Battle.Presenter.Interfaces;
using Assets.Game.Scripts.Battle.Presenter.UI;
using Assets.Game.Scripts.Common;
using Assets.Game.Scripts.Helpers;
using Assets.Game.Scripts.Utility.Characters;
using Assets.Game.Scripts.Utility.Skills;
using DG.Tweening;
using Pathfinding;
using UniRx;
using UnityEngine;

namespace Assets.Game.Scripts.Battle.Presenter
{
    public class CharacterPresenter : PresenterBase<Character>, IActor {
        public BattleCharacterStatusPresenter StatusPresenter;

        public enum CharacterStateEnum {
            Idle,
            Moving,
            SelectingTarget,
            UsingSkill,
            Dead
        }

        public ReactiveProperty<CharacterStateEnum> CharacterState = new ReactiveProperty<CharacterStateEnum>();
        public ReactiveProperty<SkillData> SelectedSkill = new ReactiveProperty<SkillData>();

        protected Character Character;

        public BattleSkill[] Skills;

        public Seeker Seeker
        {
            get { return _seeker ?? (_seeker = GetComponent<Seeker>()); }
        }
        protected Animator Animator;
        protected CharacterController Controller;

        private Seeker _seeker;
        //The AI's speed per second
        public float Speed = 100;
        //The waypoint we are currently moving towards
        private int _currentWaypoint;
        public float RotationSpeed = 360;
        protected Path Path;

        public CharacterStatusPresenter CharacterData;

        protected override IPresenter[] Children
        {
            get { return new IPresenter[]{ StatusPresenter }; }
        }

        protected override void BeforeInitialize(Character argument)
        {
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

        private IEnumerator AliveStateChanged(CharacterStatusPresenter.CharactersStateEnum charactersStateEnum)
        {
            if (charactersStateEnum == CharacterStatusPresenter.CharactersStateEnum.Dead)
            {
                CharacterState.SetValueAndForceNotify(CharacterStateEnum.Dead);
                DestroyImmediate(gameObject.GetComponent<Collider>());
                AstarPath.active.Scan();
                Animator.SetBool("Dead", true);
                yield return new WaitForSeconds(3f);
                DestroyImmediate(StatusPresenter);
                DestroyImmediate(gameObject);
            }
        }

        protected override void Initialize(Character argument)
        {
            CharacterState.SetValueAndForceNotify(CharacterStateEnum.Idle);
            SelectedSkill.Subscribe(_ =>
            {
                CharacterState.SetValueAndForceNotify(_ != null
                    ? CharacterStateEnum.SelectingTarget
                    : CharacterStateEnum.Idle);
            });
        }

        #region Movement methods
        public void MoveTo(Vector3 position) {
            //var p = ABPath.Construct(transform.position, position, PathCallback);
            //AstarPath.StartPath(p);
            var start = AstarPath.active.GetNearest(transform.position);
            var end = AstarPath.active.GetNearest(position);

            Seeker.StartPath((Vector3)start.node.position, (Vector3)end.node.position, PathCallback);
        }

        public void PathCallback(Path newPath)
        {
            if (!newPath.error)
            {
                CharacterState.SetValueAndForceNotify(CharacterStateEnum.Moving);
                //Reset the waypoint counter
                _currentWaypoint = -1;

                Path = newPath;
                Animator.SetBool("Moving", true);
                RotateCallback();
            }
        }

        protected void MoveCallback() {
            var duration = Vector3.Distance(Path.vectorPath[_currentWaypoint], transform.position)/Speed;
            transform.DOMove(Path.vectorPath[_currentWaypoint], duration).OnComplete(RotateCallback);
        }

        protected void RotateCallback() {
            _currentWaypoint ++;
            if (_currentWaypoint >= Path.vectorPath.Count)
            {
                MoveFinished();
                return;
            }
            if (Vector3.Distance(transform.position, Path.vectorPath[_currentWaypoint]) < 0.1f)
            {
                RotateCallback();
            }
            else
            {
                var duration = Vector3.Angle(transform.forward, Path.vectorPath[_currentWaypoint])/RotationSpeed;
                transform.DOLookAt(Path.vectorPath[_currentWaypoint], duration)
                    .OnComplete(MoveCallback);
            }
        }

        protected void MoveFinished() {
            Animator.SetBool("Moving", false);
            AstarPath.active.Scan();

            CharacterData.RemainingActionPoint.Value --;
            Path = null;
            CharacterState.SetValueAndForceNotify(CharacterStateEnum.Idle);
        }

        #endregion

        #region Skill Player

        public IEnumerator PlayTargetedSkill(SkillData skill, CharacterPresenter target) {
            CharacterState.SetValueAndForceNotify(CharacterStateEnum.UsingSkill);

            SelectedSkill.Value = null;
            Animator.SetTrigger(TriggerEnumToTriggerName(skill.TriggerName));
            yield return new WaitForSeconds(0.4f);
            while (!Animator.GetCurrentAnimatorStateInfo(0).IsName("Idle")) {
                yield return new WaitForEndOfFrame();
            }
            target.CharacterData.DealDamage(skill.DamageMultiplier * CharacterData.Damage.Value);

            CharacterState.SetValueAndForceNotify(CharacterStateEnum.Idle);
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