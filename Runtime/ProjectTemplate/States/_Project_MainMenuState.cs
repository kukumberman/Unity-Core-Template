using Game.Managers;
using Game.States;
using Injection;
using _RootNamespace_.UI;

namespace _RootNamespace_.States
{
    public sealed class _Project_MainMenuState : GameState
    {
        [Inject]
        private HudManager _hudManager;

        public override void Initialize()
        {
            _hudManager.ShowAdditional<MainMenuHudMediator>();
        }

        public override void Dispose()
        {
            _hudManager.HideAdditional<MainMenuHudMediator>();
        }
    }
}
