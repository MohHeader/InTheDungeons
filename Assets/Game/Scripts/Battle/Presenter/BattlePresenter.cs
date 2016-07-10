using System.Linq;
using Assets.Game.Scripts.Battle.Model;
using UniRx;

namespace Assets.Game.Scripts.Battle.Presenter
{
    public class BattlePresenter : PresenterBase
    {
        public SquadPresenter SquadPresenter;
        public BattleCameraPresenter CameraPresenter;
        public DungeonArchitect.Dungeon Dungeon;

        public Squad PlayerSquad;
        public ReactiveProperty<CharacterPresenter> SelectedCharacter;

        protected override IPresenter[] Children
        {
            get { return new IPresenter[] {SquadPresenter, CameraPresenter }; }
        }

        protected override void BeforeInitialize()
        {
            SelectedCharacter = new ReactiveProperty<CharacterPresenter>();
            CameraPresenter.PropagateArgument(this);

            Dungeon.Build();
            AstarPath.active.Scan();
            SquadPresenter.PropagateArgument(PlayerSquad);
        }

        protected override void Initialize()
        {
            SelectedCharacter.SetValueAndForceNotify(SquadPresenter.Characters.First());
        }
    }
}