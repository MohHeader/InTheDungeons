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
        protected ReactiveProperty<CharacterData> CharacterData;

        public ReactiveProperty<int> Level { get; protected set; }

        public ReadOnlyReactiveProperty<float> Strength { get; protected set; }
        public ReadOnlyReactiveProperty<float> Dexterity { get; protected set; }
        public ReadOnlyReactiveProperty<float> Intelligence { get; protected set; }
        public ReadOnlyReactiveProperty<float> Consitution { get; protected set; }

        public ReactiveProperty<float> MaximumActionPoints { get; protected set; }
        public ReactiveProperty<float> ActionPointsRegen { get; protected set; }
        public ReactiveProperty<float> RemainingActionPoint { get; protected set; }

        public CharacterStatusPresenter(CharacterData characterData, int level) {
            CharacterData = new ReactiveProperty<CharacterData>();
            Level = new ReactiveProperty<int>();

            SetupStatCalculations();

            CharacterData.SetValueAndForceNotify(characterData);
            Level.SetValueAndForceNotify(level);

            SetupActionPoints();
        }

        protected void SetupStatCalculations() {
            Strength = CharacterData.CombineLatest(Level, (data, level) => data.StartStrength + data.LevelStrength*level).ToReadOnlyReactiveProperty();
            Dexterity = CharacterData.CombineLatest(Level, (data, level) => data.StartDexterity + data.LevelDexterity*level).ToReadOnlyReactiveProperty();
            Intelligence = CharacterData.CombineLatest(Level, (data, level) => data.StartIntelligence + data.LevelIntelligence*level).ToReadOnlyReactiveProperty();
            Consitution = CharacterData.CombineLatest(Level, (data, level) => data.StartConstitution + data.LevelConstitution*level).ToReadOnlyReactiveProperty();
        }

        protected void SetupActionPoints() {
            MaximumActionPoints = new ReactiveProperty<float>(CharacterData.Value.ActionPoints);
            RemainingActionPoint = new ReactiveProperty<float>(CharacterData.Value.ActionPoints);
            ActionPointsRegen = new ReactiveProperty<float>(CharacterData.Value.ActionPoints);
        }

        public void RegenerateActionPoints() {
            RemainingActionPoint.Value = Mathf.Clamp(RemainingActionPoint.Value + ActionPointsRegen.Value, 0f, MaximumActionPoints.Value);
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