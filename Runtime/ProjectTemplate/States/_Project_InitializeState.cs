using Game.Domain;
using Game.Enums;
using Game.Managers;
using Game.States;
using Injection;
using UnityEngine;
using _RootNamespace_.Enums;

namespace _RootNamespace_.States
{
    public sealed class _Project_InitializeState : GameState
    {
        [Inject]
        private _Project_StartBehaviour _game;

        [Inject]
        private Context _context;

        [Inject]
        private GameStateManager _gameStateManager;

        //[Inject]
        //private _Project_GameConfig _gameConfig;

        public override void Initialize()
        {
            var gameModel = GameModel.Load<_Project_GameModel>(
                null /*_gameConfig*/
            );
            _context.Install(gameModel);

            // todo
            _context.Install(new LocalizationManager(null).CreateFromJson("{}"));

            var hudManagerContainer = new HudManagerContainer()
            {
                UGUI = _context.GetByName("HudManager_UGUI") as HudManager,
                UITK = _context.GetByName("HudManager_UITK") as HudManager,
                Active = null,
            };

            hudManagerContainer.Active = hudManagerContainer.UGUI;
            hudManagerContainer.Active.Orientation = HudOrientation.Default;

            _context.Install(hudManagerContainer);
            _context.Install(hudManagerContainer.Active);

            _context.ApplyInstall();

            var nextState = GetNextState();
            _gameStateManager.SwitchToState(nextState);
        }

        public override void Dispose()
        {
            //
        }

        private GameState GetNextState()
        {
            if (!_game.OverrideInitialState)
            {
                return new _Project_MainMenuState();
            }

            switch (_game.InitialState)
            {
                case StateType.MainMenu:
                    return new _Project_MainMenuState();
                case StateType.Gameplay:
                    return new _Project_GameplayState();
                default:
                    Debug.LogWarning(
                        $"Initial switch for state [{_game.InitialState}] is not implemented"
                    );
                    return new _Project_MainMenuState();
            }
        }
    }
}
