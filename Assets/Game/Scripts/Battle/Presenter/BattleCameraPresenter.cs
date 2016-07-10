using UniRx;
using UnityEngine;

namespace Assets.Game.Scripts.Battle.Presenter
{
    public class BattleCameraPresenter : PresenterBase<BattlePresenter>
    {
        public Vector3 Offset;         //Private variable to store the offset distance between the player and camera
        protected override IPresenter[] Children
        {
            get { return EmptyChildren; }
        }

        protected override void BeforeInitialize(BattlePresenter argument)
        {
        }

        protected override void Initialize(BattlePresenter argument)
        {
            argument.SelectedCharacter.Subscribe(SelectedCharacterChanged);
        }

        protected Transform FollowTransform;

        private void SelectedCharacterChanged(CharacterPresenter characterPresenter)
        {
            if (characterPresenter == null)
            {
                FollowTransform = null;
                return;
            }
            FollowTransform = characterPresenter.transform;
        }

        void LateUpdate()
        {
            if (FollowTransform == null) return;
            // Set the position of the camera's transform to be the same as the player's, but offset by the calculated offset distance.
            transform.position = FollowTransform.position + Offset;
        }

    }
}