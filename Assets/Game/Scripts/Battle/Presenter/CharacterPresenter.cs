using System;
using System.Collections;
using System.Linq;
using Assets.Game.Scripts.Battle.Model;
using Assets.Game.Scripts.Battle.Presenter.UI;
using Assets.Game.Scripts.Common;
using Assets.Game.Scripts.Helpers;
using Assets.Game.Scripts.Utility.Characters;
using Assets.Game.Scripts.Utility.Skills;
using DG.Tweening;
using Pathfinding;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace Assets.Game.Scripts.Battle.Presenter
{
    public class CharacterPresenter : PresenterBase<Character> {
        public BattleCharacterStatusPresenter StatusPresenter;

        public enum CharacterStateEnum {
            Idle,
            Moving
        }

        public ReactiveProperty<CharacterStateEnum> CharacterState = new ReactiveProperty<CharacterStateEnum>();
        protected Character Character;

        public BattleSkill[] Skills;

        public Seeker Seeker
        {
            get
            {
                if (_seeker == null) _seeker = GetComponent<Seeker>();
                return _seeker;
            }
        }
        protected Animator Animator;
        protected CharacterController Controller;
        protected DynamicGridObstacle Obstacle;

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
            var instance = DataLayer.GetInstance();
            CharacterData = new CharacterStatusPresenter(instance.Database.GetCharacterData(Character.Id), Character.Level);
            var prefab = Instantiate(CharacterData.Asset);
            prefab.transform.SetParent(transform, false);
            Animator = gameObject.FindAnimatorComponent();
            Controller = gameObject.FindCharacterControllerComponent();
            Obstacle = gameObject.FindDynamicGridObstacleComponent();
            if (Obstacle != null) {
                Obstacle.DoUpdateGraphs();
            }
            Skills = CharacterData.Skills.Select(_ => new BattleSkill(_)).ToArray();
            StatusPresenter.PropagateArgument(CharacterData);
        }

        protected override void Initialize(Character argument)
        {
            CharacterState.SetValueAndForceNotify(CharacterStateEnum.Idle);
        }

        #region Movement methods
        // TODO: Возможно имеет смысл разделить представление от логики перемещения

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
                Debug.LogFormat("Path lenght {0}", Path.vectorPath.GetPathLength());
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
            Debug.Log("End Of Path Reached");
            Animator.SetBool("Moving", false);
            AstarPath.active.Scan();
            if (Obstacle != null)
            {
                //Obstacle.enabled = true;
                Obstacle.DoUpdateGraphs();
            }
            // TODO: Hack
            // CharacterData.RemainingActionPoint.Value -= Path.vectorPath.GetPathLength()*10;
            Path = null;
            CharacterState.SetValueAndForceNotify(CharacterStateEnum.Idle);
        }

        #endregion

        #region Skill Player

        public IEnumerator PlayTargetedSkill(SkillData skill, CharacterPresenter target) {
            Animator.SetTrigger(TriggerEnumToTriggerName(skill.TriggerName));
            yield return new WaitForSeconds(0.4f);
            while (!Animator.GetCurrentAnimatorStateInfo(0).IsName("Idle")) {
                yield return new WaitForEndOfFrame();
            }
            target.CharacterData.DealDamage(skill.DamageMultiplier * CharacterData.Damage.Value);
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
    }
}