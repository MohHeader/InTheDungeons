using System.Linq;
using Assets.Game.Scripts.Battle.Model;
using Assets.Game.Scripts.Helpers;
using Assets.Game.Scripts.Utility.Characters;
using DG.Tweening;
using Pathfinding;
using UniRx;
using UnityEngine;

namespace Assets.Game.Scripts.Battle.Presenter
{
    public class CharacterPresenter : PresenterBase<Character>
    {
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

        public CharacterData CharacterData;

        protected override IPresenter[] Children
        {
            get { return EmptyChildren; }
        }

        protected override void BeforeInitialize(Character argument) {
            Character = argument;
            var instance = DataLayer.GetInstance();
            CharacterData = instance.Database.GetCharacterData(Character.Id);
            var prefab = Instantiate(CharacterData.Asset);
            prefab.transform.SetParent(transform, false);
            Animator = gameObject.FindAnimatorComponent();
            Controller = gameObject.FindCharacterControllerComponent();
            Obstacle = gameObject.FindDynamicGridObstacleComponent();
            if (Obstacle != null) {
                Obstacle.DoUpdateGraphs();
            }
            Skills = CharacterData.Skills.Select(_ => new BattleSkill(_)).ToArray();
        }

        protected override void Initialize(Character argument)
        {
            CharacterState.SetValueAndForceNotify(CharacterStateEnum.Idle);
        }

        public void MoveTo(Vector3 position) {
            Seeker.StartPath(transform.position, position, PathCallback);
        }

        public void PathCallback(Path newPath)
        {
            if (!newPath.error)
            {
                CharacterState.SetValueAndForceNotify(CharacterStateEnum.Moving);
                //Reset the waypoint counter
                _currentWaypoint = -1;

                Path = newPath;
                Debug.Log(Path.vectorPath.Count);
                //if (Obstacle != null) Obstacle.enabled = false;
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
            var duration = Vector3.Angle(transform.forward, Path.vectorPath[_currentWaypoint]) / RotationSpeed;
            transform.DOLookAt(Path.vectorPath[_currentWaypoint], 0.1f, AxisConstraint.None).OnComplete(MoveCallback);
        }

        protected void MoveFinished() {
            Debug.Log("End Of Path Reached");
            Animator.SetBool("Moving", false);
            if (Obstacle != null)
            {
                //Obstacle.enabled = true;
                Obstacle.DoUpdateGraphs();
            }
            Path = null;
            CharacterState.SetValueAndForceNotify(CharacterStateEnum.Idle);
        }
    }
}