using System;
using Game.UI.Hud;
using UnityEngine;
using UnityEngine.UI;

namespace _RootNamespace_.UI
{
    public class MainMenuHudViewUGUI : BaseHudWithModel<MainMenuHudModel>, IMainMenuHudView
    {
        public event Action OnPlayClicked;

        [SerializeField]
        private Text _txtFoobar;

        [SerializeField]
        private Button _btnPlay;

        protected override void OnEnable()
        {
            _btnPlay.onClick.AddListener(OnPlayButtonClicked);
        }

        protected override void OnDisable()
        {
            _btnPlay.onClick.RemoveListener(OnPlayButtonClicked);
        }

        protected override void OnModelChanged(MainMenuHudModel model)
        {
            _txtFoobar.text = string.Format("{0}: {1}", model.I18N.LabelFoobar, model.Foobar);
        }

        private void OnPlayButtonClicked()
        {
            OnPlayClicked.SafeInvoke();
        }
    }
}
