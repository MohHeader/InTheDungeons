using Assets.Game.Scripts.Battle.Model;
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

        protected override void BeforeInitialize()
        {
            Dungeon.Build();
            AstarPath.active.Scan();
            SquadPresenter.PropagateArgument(PlayerSquad);
            CameraPresenter.PropagateArgument(SquadPresenter);
            DefendersPresenter.PropagateArgument(DefendersSquad);

            AstarPath.active.Scan();
        }

        protected override void Initialize()
        {
        }
    }
}