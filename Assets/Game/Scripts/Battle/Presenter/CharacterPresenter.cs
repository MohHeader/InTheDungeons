﻿using System;
using Assets.Game.Scripts.Battle.Model;
using Assets.Game.Scripts.Helpers;
using Assets.Game.Scripts.Utility.Characters;
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

        public Seeker Seeker
        {
            get
            {
                if (_seeker == null) _seeker = GetComponent<Seeker>();
                return _seeker;
            }
        }
        protected Animator animator;
        private CharacterController controller;

        private Seeker _seeker;
        //The AI's speed per second
        public float speed = 100;
        //The max distance from the AI to a waypoint for it to continue to the next waypoint
        public float nextWaypointDistance = 0.2f;
        //The waypoint we are currently moving towards
        private int currentWaypoint = 0;
        public float rotationSpeed = 360;
        protected Path path;

        protected override IPresenter[] Children
        {
            get { return EmptyChildren; }
        }

        protected override void BeforeInitialize(Character argument) {
            Character = argument;
            var instance = DataLayer.GetInstance();
            var prefab = Instantiate(instance.Database.GetCharacterData(Character.Id).Asset);
            prefab.transform.SetParent(transform, false);
            animator = gameObject.FindAnimatorComponent();
            controller = gameObject.FindCharacterControllerComponent();
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
            Debug.Log("Yay, we got a path back. Did it have an error? " + newPath.error);
            if (!newPath.error)
            {
                //Reset the waypoint counter
                currentWaypoint = 0;

                path = newPath;
                CharacterState.SetValueAndForceNotify(CharacterStateEnum.Moving);
                animator.SetBool("Moving", true);
            }
        }

        void Update()
        {
            if (path == null)
            {
                //We have no path to move after yet
                return;
            }

            if (currentWaypoint >= path.vectorPath.Count)
            {
                Debug.Log("End Of Path Reached");
                animator.SetBool("Moving", false);
                CharacterState.SetValueAndForceNotify(CharacterStateEnum.Idle);
                path = null;
                return;
            }
            //Direction to the next waypoint
            Vector3 dir = (path.vectorPath[currentWaypoint] - transform.position).normalized;
            //transform.LookAt(path.vectorPath[currentWaypoint]);
            // Rotate towards the target
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), rotationSpeed * Time.deltaTime);
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);

            dir *= speed * Time.deltaTime;
            controller.SimpleMove(dir);
            //Check if we are close enough to the next waypoint
            //If we are, proceed to follow the next waypoint
            if (Vector3.Distance(transform.position, path.vectorPath[currentWaypoint]) < nextWaypointDistance)
            {
                currentWaypoint++;
                return;
            }
        }
    }
}