using System.Collections.Generic;
using Assets.Game.Scripts.Battle.Model;
using UniRx;
using UnityEngine;

namespace Assets.Game.Scripts.Battle.Presenter {
    public class DefendersPresenter : PresenterBase<DefenderSquad> {
        public GameObject CharacterPrefab;
        public List<CharacterPresenter> Characters;
        protected DefenderSquad DefenderSquad;

        protected override IPresenter[] Children
        {
            get { return EmptyChildren; }
        }

        protected override void BeforeInitialize(DefenderSquad argument) {
            DefenderSquad = argument;
            foreach (var character in DefenderSquad.Defenders)
            {
                var instance = Instantiate(CharacterPrefab);
                var characterInstance = instance.GetComponent<CharacterPresenter>();
                characterInstance.CharacterSide = CharacterPresenter.CharacterSideEnum.Defender;
                characterInstance.ForceInitialize(character);
                Characters.Add(characterInstance);

                var spawnNode = AstarPath.active.GetNearest(character.Position);
                instance.transform.position = (Vector3)spawnNode.node.position;
            }
        }

        protected override void Initialize(DefenderSquad argument) {
        }
    }
}