using Assets.Game.Scripts.Common;
using Assets.Game.Scripts.Helpers;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Game.Scripts.Battle.Presenter.UI {
    public class BattleCharacterStatusPresenter : PresenterBase<CharacterStatusPresenter> {
        public GameObject HudPrefab;
        protected GameObject Hud;
        protected Transform CharacterTransform;

        protected TextMeshProUGUI StatusText;
        protected Slider HealthSlider;

        protected Rect ScreenRect;

        protected override IPresenter[] Children
        {
            get { return EmptyChildren; }
        }

        protected override void BeforeInitialize(CharacterStatusPresenter argument) {
            ScreenRect = new Rect(0f, 0f, Screen.width, Screen.height);
        }

        protected override void Initialize(CharacterStatusPresenter argument) {
            Hud = Instantiate(HudPrefab);
            var canvas = FindObjectOfType<Canvas>();
            Hud.transform.SetParent(canvas.transform, false);
            StatusText = Hud.FindTextMeshProUguiComponent();
            HealthSlider = Hud.FindSliderComponent();

            argument.MaximumHealth.Subscribe(_ => HealthSlider.maxValue = _);
            argument.RemainingHealth.Subscribe(_ => HealthSlider.value = _);

            StatusText.transform.gameObject.SetActive(false);
        }

        protected void LateUpdate() {
            if (Hud == null) return;
            Hud.transform.position = Camera.main.WorldToScreenPoint(transform.position);
            if (ScreenRect.Contains(Hud.transform.position)) {
                if (!Hud.activeSelf) Hud.SetActive(true);
            }
            else {
                if (Hud.activeSelf) Hud.SetActive(false);
            }
        }
    }
}