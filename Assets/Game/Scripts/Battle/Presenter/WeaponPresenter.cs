using System;
using System.Linq;
using Assets.Game.Scripts.Helpers;
using Assets.Game.Scripts.Utility.Characters;
using Assets.Game.Scripts.Utility.Equipment;
using UniRx;
using Unity.Linq;
using UnityEngine;

namespace Assets.Game.Scripts.Battle.Presenter {
    public class WeaponPresenter : PresenterBase<CharacterPresenter> {
        public enum WeaponPresenterTypeEnum {
            Weapon,
            Offhand,
            None
        }

        public WeaponPresenterTypeEnum WeaponPresenterType;

        protected override IPresenter[] Children
        {
            get { return EmptyChildren; }
        }

        protected CharacterPresenter CharacterPresenter;

        protected override void BeforeInitialize(CharacterPresenter argument) {
        }

        protected GameObject ParentObject;
        protected GameObject Item;

        private DataLayer DataLayer;

        protected override void Initialize(CharacterPresenter argument) {
            DataLayer = DataLayer.GetInstance();

            CharacterPresenter = argument;

            switch (WeaponPresenterType)
            {
                case WeaponPresenterTypeEnum.Weapon:
                    ParentObject = gameObject.Descendants().FirstOrDefault(_ => _.name == "Weapon");
                    break;
                case WeaponPresenterTypeEnum.Offhand:
                    ParentObject = gameObject.Descendants().FirstOrDefault(_ => _.name == "Offhand");
                    break;
                case WeaponPresenterTypeEnum.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            CharacterPresenter.CharacterData.EquippedItems.ObserveAdd().Subscribe(_ => UpdateItem());
            CharacterPresenter.CharacterData.EquippedItems.ObserveRemove().Subscribe(_ => UpdateItem());
            CharacterPresenter.CharacterData.EquippedItems.ObserveReplace().Subscribe(_ => UpdateItem());

            UpdateItem();
        }

        protected void UpdateItem() {
            var path = string.Empty;
            EquippedItem wearedItem;

            switch (WeaponPresenterType)
            {
                case WeaponPresenterTypeEnum.Weapon:
                    wearedItem = CharacterPresenter.CharacterData.EquippedItems.FirstOrDefault(_ => _.Slot == WearSlotEnum.RightHand);
                    path = wearedItem != null ? wearedItem.Item.AssetPath : DataLayer.DefaultEquipmentDatabase.Database[(int)CharacterPresenter.WeaponVisual];
                    break;
                case WeaponPresenterTypeEnum.Offhand:
                    wearedItem = CharacterPresenter.CharacterData.EquippedItems.FirstOrDefault(_ => _.Slot == WearSlotEnum.LeftHand);
                    path = wearedItem != null ? wearedItem.Item.AssetPath : DataLayer.DefaultEquipmentDatabase.Database[(int)CharacterPresenter.OffhandVisual];
                    break;
                case WeaponPresenterTypeEnum.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (path != string.Empty)
            {
                var asset = Resources.Load(path.CutResAndExtension()) as GameObject;
                UpdateVisual(asset);
            }

        }

        protected void UpdateVisual(GameObject model) {
            if (ParentObject == null) return;

            if (Item != null) {
                Destroy(Item);
            }
            Item = Instantiate(model);
            Item.transform.SetParent(ParentObject.transform, false);
        }
    }
}