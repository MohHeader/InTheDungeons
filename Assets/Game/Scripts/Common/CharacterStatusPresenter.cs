using System;
using System.Collections.Generic;
using Assets.Game.Scripts.Utility.Characters;
using Assets.Game.Scripts.Utility.Skills;
using UniRx;
using UnityEngine;

namespace Assets.Game.Scripts.Common {
    /// <summary>
    ///     Состояние персонажа, зависит от данных и от уровня
    ///     TODO: добавить звёзды
    /// </summary>
    public class CharacterStatusPresenter {
        public enum CharactersStateEnum {
            Alive,
            Dead
        }

        protected ReactiveProperty<CharacterData> CharacterData;

        public ReactiveProperty<CharactersStateEnum> CharacterState =
            new ReactiveProperty<CharactersStateEnum>(CharactersStateEnum.Alive);

        public CharacterStatusPresenter(CharacterData characterData, int level) {
            CharacterData = new ReactiveProperty<CharacterData>(new CharacterData());
            Level = new ReactiveProperty<int>(1);

            CharacterData.SetValueAndForceNotify(characterData);
            Level.SetValueAndForceNotify(level);
            SetupProperties();

            SetupStatCalculations();

            SetupActionPoints();
            RemainingHealth.Select(_ => _ <= 0).Subscribe(_ =>
            {
                if (_)
                    CharacterState.Value = CharactersStateEnum.Dead;
            });
        }

        public ReactiveProperty<int> Level { get; protected set; }

        public ReactiveProperty<float> Strength { get; protected set; }
        public ReactiveProperty<float> Dexterity { get; protected set; }
        public ReactiveProperty<float> Intelligence { get; protected set; }
        public ReactiveProperty<float> Consitution { get; protected set; }

        public ReactiveProperty<int> MaximumActionPoints { get; protected set; }
        public ReactiveProperty<int> ActionPointsRegen { get; protected set; }
        public ReactiveProperty<int> RemainingActionPoint { get; protected set; }

        public ReactiveProperty<float> MaximumHealth { get; protected set; }
        public ReactiveProperty<float> RemainingHealth { get; protected set; }

        public ReactiveProperty<float> Damage { get; protected set; }
        public ReactiveProperty<int> MovementRange { get; protected set; }

        protected void SetupProperties() {
            Strength = new ReactiveProperty<float>();
            Dexterity = new ReactiveProperty<float>();
            Intelligence = new ReactiveProperty<float>();
            Consitution = new ReactiveProperty<float>();

            MaximumHealth = new ReactiveProperty<float>(1);
            RemainingHealth = new ReactiveProperty<float>(1);

            Damage = new ReactiveProperty<float>();

            MovementRange = new ReactiveProperty<int>(CharacterData.Value.MovementRange);

            switch (CharacterData.Value.MainStatValue)
            {
                case MainStatEnum.Strength:
                    Strength.Subscribe(_ => Damage.Value = _*2f);
                    break;
                case MainStatEnum.Dexterity:
                    Dexterity.Subscribe(_ => Damage.Value = _*2f);
                    break;
                case MainStatEnum.Intelligence:
                    Intelligence.Subscribe(_ => Damage.Value = _*2f);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected void SetupStatCalculations() {
            Level.Subscribe(_ =>
            {
                Strength.Value = Level.Value*CharacterData.Value.LevelStrength + CharacterData.Value.StartStrength;
                Dexterity.Value = Level.Value*CharacterData.Value.LevelDexterity + CharacterData.Value.StartDexterity;
                Intelligence.Value = Level.Value*CharacterData.Value.LevelIntelligence +
                                     CharacterData.Value.StartIntelligence;
                Consitution.Value = Level.Value*CharacterData.Value.LevelConstitution +
                                    CharacterData.Value.StartConstitution;
            });

            Consitution.Subscribe(_ => MaximumHealth.Value = _*25f);
            Consitution.Subscribe(_ => RemainingHealth.Value = _*25f);
        }

        protected void SetupActionPoints() {
            MaximumActionPoints = new ReactiveProperty<int>(CharacterData.Value.ActionPoints);
            RemainingActionPoint = new ReactiveProperty<int>(CharacterData.Value.ActionPoints);
            ActionPointsRegen = new ReactiveProperty<int>(CharacterData.Value.ActionPoints);
            Debug.LogFormat("CharacterData.Value.ActionPoints {0}", CharacterData.Value.ActionPoints);
        }

        public void RegenerateActionPoints() {
            RemainingActionPoint.Value = ActionPointsRegen.Value;
        }

        public void DealDamage(float damageAmount) {
            RemainingHealth.Value = Mathf.Clamp(RemainingHealth.Value - damageAmount, 0f, MaximumHealth.Value);
        }

        #region Inherited properties

        public GameObject Asset
        {
            get { return CharacterData.Value.Asset; }
        }

        public List<SkillData> Skills
        {
            get { return CharacterData.Value.Skills; }
        }

        public Sprite Icon
        {
            get { return CharacterData.Value.Icon; }
        }

        #endregion
    }
}