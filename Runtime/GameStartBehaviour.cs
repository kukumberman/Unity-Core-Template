using System;
using Game.Core;
using Game.Managers;
using Game.States;
using Game.UI;
using Game.Core.UI;
using Injection;
using UnityEngine;

namespace Game
{
    public sealed class GameStartBehaviour : MonoBehaviour
    {
        public Func<GameState> InitialStateFunc = null;

        private Timer _timer;

        public Context Context { get; private set; }

        private void Start()
        {
            _timer = new Timer();

            Application.targetFrameRate = 60;
            QualitySettings.vSyncCount = 0;
            Application.runInBackground = true;

            var context = new Context();

            var gameView = GetComponent<GameView>();
            var hudManager = new HudManager(new CanvasHudCreator(gameView.Canvas));
            var toolkitHudManager = new HudManager(new ToolkitHudCreator(gameView.Document));

            context.Install(
                new Injector(context),
                new GameStateManager(),
                hudManager,
                new SpriteManager()
            );

            context.InstallByName("ToolkitHudManager", toolkitHudManager);

            context.Install(GetComponents<Component>());
            context.Install(_timer);
            context.ApplyInstall();

            if (InitialStateFunc == null)
            {
                Debug.LogError("Provide initial state getter");
            }

            GameState next = InitialStateFunc();
            context.Get<GameStateManager>().SwitchToState(next);

            Context = context;
        }

        // csharpier-ignore
        private void OnDestroy()
        {
        }

        public void Reload()
        {
            Context.Get<GameStateManager>().Dispose();
            Context.Dispose();

            Start();
        }

        private void Update()
        {
            _timer.Update();
        }

        private void LateUpdate()
        {
            _timer.LateUpdate();
        }

        private void FixedUpdate()
        {
            _timer.FixedUpdate();
        }

        private void OnDrawGizmos()
        {
            if (Application.isPlaying)
            {
                _timer.OnDrawGizmos();
            }
        }
    }
}
