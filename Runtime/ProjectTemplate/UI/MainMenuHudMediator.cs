using System;
using Core;
using Game.Core.UI;
using Game.Managers;
using Game.States;
using Game.UI.Hud;
using Injection;
using _RootNamespace_.States;

namespace _RootNamespace_.UI
{
    public sealed class MainMenuHudModel : Observable
    {
        public sealed class Localization
        {
            public string LabelFoobar;
        }

        public Localization I18N;

        public int Foobar;
    }

    public interface IMainMenuHudView : IHudWithModel<MainMenuHudModel>
    {
        event Action OnPlayClicked;
    }

    public sealed class MainMenuHudMediator : Mediator<IMainMenuHudView>
    {
        [Inject]
        private LocalizationManager _localizationManager;

        [Inject]
        private GameStateManager _gameStateManager;

        //[Inject]
        //private _Project_GameConfig _gameConfig;

        [Inject]
        private _Project_GameModel _gameModel;

        private MainMenuHudModel _viewModel;

        protected override void Show()
        {
            _gameModel.Foobar += 1;
            _gameModel.Save();

            _localizationManager.OnLanguageChanged += LocalizationManager_OnLanguageChanged;

            _view.OnPlayClicked += View_OnPlayClicked;

            _viewModel = new MainMenuHudModel
            {
                I18N = new MainMenuHudModel.Localization(),
                Foobar = _gameModel.Foobar,
            };

            PopulateLocalizedText();

            _view.Model = _viewModel;
        }

        protected override void Hide()
        {
            _localizationManager.OnLanguageChanged -= LocalizationManager_OnLanguageChanged;

            _view.OnPlayClicked -= View_OnPlayClicked;
        }

        private void LocalizationManager_OnLanguageChanged()
        {
            PopulateLocalizedText();
        }

        private void View_OnPlayClicked()
        {
            _gameStateManager.SwitchToState(new _Project_GameplayState());
        }

        private void PopulateLocalizedText()
        {
            _viewModel.I18N.LabelFoobar = _localizationManager.GetValue("label_foobar");

            _viewModel.SetChanged();
        }
    }
}
