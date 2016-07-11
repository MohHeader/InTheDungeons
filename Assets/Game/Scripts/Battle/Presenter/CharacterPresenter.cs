﻿using Assets.Game.Scripts.Battle.Model;
using Assets.Game.Scripts.Utility.Characters;
using UniRx;

namespace Assets.Game.Scripts.Battle.Presenter
{
    public class CharacterPresenter : PresenterBase<Character>
    {
        protected Character Character;

        public Seeker Seeker
        {
            get
            {
                if (_seeker == null) _seeker = GetComponent<Seeker>();
                return _seeker;
            }
        }

        private Seeker _seeker;

        protected override IPresenter[] Children
        {
            get { return EmptyChildren; }
        }

        protected override void BeforeInitialize(Character argument)
        {
            
        }

        protected override void Initialize(Character argument)
        {
            Character = argument;
            var instance = DataLayer.GetInstance();
            var prefab = Instantiate(instance.Database.GetCharacterData(Character.Id).Asset);
            prefab.transform.SetParent(transform, false);
        }
    }
}