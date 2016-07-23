using System;
using System.Collections;
using Assets.Game.Scripts.Battle.Model;
using DungeonArchitect;
using UniRx;
using UnityEngine;

namespace Assets.Game.Scripts.Battle.Presenter
{
    public class BattlePresenter : PresenterBase
    {
        public BattleCameraPresenter CameraPresenter;

        public DungeonArchitect.Dungeon Dungeon;

        public Squad PlayerSquad;
        public DefenderSquad DefendersSquad;
        public SquadPresenter SquadPresenter;
        public DefendersPresenter DefendersPresenter;

        public GameObject AttackerTurnMessage;
        public GameObject DefenderTurnMessage;

        public float ShowTime = 3f;

        public uint Seed = 524145472;
        public int Rooms = 10;

        protected override IPresenter[] Children
        {
            get
            {
                return new IPresenter[]
                {
                    SquadPresenter,
                    CameraPresenter,
                    DefendersPresenter
                };
            }
        }

        protected override void BeforeInitialize() {
            var gridDungeonConfig = Dungeon.Config as GridDungeonConfig;
            if (gridDungeonConfig != null) {
                Dungeon.Config.Seed = Seed;
                gridDungeonConfig.NumCells = Rooms;
                Dungeon.Build();
                AstarPath.active.Scan();
                SquadPresenter.PropagateArgument(PlayerSquad);
                CameraPresenter.PropagateArgument(SquadPresenter);
                DefendersPresenter.PropagateArgument(DefendersSquad);
                SquadPresenter.SquadState.Subscribe(_ => AttackerSquadStateChanged(_));
                DefendersPresenter.SquadState.Subscribe(_ => DefendersSquadStateChanged(_));
            }
            else {
                throw new Exception("Dungeon config not valid. Use GridDungeonConfig");
            }
        }

        private void DefendersSquadStateChanged(SquadStateEnum squadStateEnum) {
            switch (squadStateEnum)
            {
                case SquadStateEnum.None:
                    break;
                case SquadStateEnum.Started:
                    StartCoroutine(ShowMessage(DefenderTurnMessage, ShowTime));
                    break;
                case SquadStateEnum.InProgress:
                    break;
                case SquadStateEnum.Finished:
                    SquadPresenter.SquadState.Value = SquadStateEnum.Started;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("squadStateEnum", squadStateEnum, null);
            }
        }

        private void AttackerSquadStateChanged(SquadStateEnum squadStateEnum) {
            switch (squadStateEnum)
            {
                case SquadStateEnum.None:
                    break;
                case SquadStateEnum.Started:
                    StartCoroutine(ShowMessage(AttackerTurnMessage, ShowTime));
                    break;
                case SquadStateEnum.InProgress:
                    break;
                case SquadStateEnum.Finished:
                    DefendersPresenter.SquadState.Value = SquadStateEnum.Started;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("squadStateEnum", squadStateEnum, null);
            }
        }

        private IEnumerator ShowMessage(GameObject message, float time) {
            yield return new WaitForEndOfFrame();
            message.SetActive(true);
            yield return new WaitForSeconds(time);
            message.SetActive(false);
        }

        protected override void Initialize() {
            SquadPresenter.SquadState.Value = SquadStateEnum.Started;
        }
    }
}