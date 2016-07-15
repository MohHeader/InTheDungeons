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
            Dungeon.Config.Seed = Seed;
            (Dungeon.Config as GridDungeonConfig).NumCells = Rooms;
            Dungeon.Build();
            AstarPath.active.Scan();
            SquadPresenter.PropagateArgument(PlayerSquad);
            CameraPresenter.PropagateArgument(SquadPresenter);
            DefendersPresenter.PropagateArgument(DefendersSquad);
        }

        protected override void Initialize()
        {
        }
    }
}