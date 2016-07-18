using System;
using Assets.Game.Scripts.Battle.Model;
using DungeonArchitect;
using UniRx;

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
            }
            else {
                throw new Exception("Dungeon config not valid. Use GridDungeonConfig");
            }
        }

        protected override void Initialize()
        {
        }
    }
}