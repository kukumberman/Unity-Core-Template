using System;
using System.Collections.Generic;
using System.Linq;
using Game.Core;
using Game.Domain;
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

        [SerializeField]
        private int _injectionDepth;

        [SerializeField]
        private List<MonoBehaviour> _injectableMonoBehaviours;

        [SerializeField]
        private List<ScriptableObject> _injectableScriptableObjects;

        private Timer _timer;

        public Context Context { get; private set; }

        private void Start()
        {
            _timer = new Timer();

            Application.targetFrameRate = 60;
            QualitySettings.vSyncCount = 0;
            Application.runInBackground = true;

            var context = new Context();
            Context = context;
            Context.Current = context;

#if UNITY_WEBGL && !UNITY_EDITOR
            GameModel.CurrentSaveSystem = new WebglSaveSystem();
#else
            GameModel.CurrentSaveSystem = new FileSaveSystem();
#endif

            var gameView = GetComponent<GameView>();
            var hudManager = new HudManager(new HudFactoryUGUI(gameView.Canvas));
            var hudManagerUITK = new HudManager(new HudFactoryUITK(gameView.Document));

            context.Install(
                new Injector(context),
                new GameStateManager(),
                hudManager,
                new SpriteManager()
            );

            context.InstallByName("HudManager_UGUI", hudManager);
            context.InstallByName("HudManager_UITK", hudManagerUITK);

            context.Install(_timer);
            Install();
            context.ApplyInstall();

            if (InitialStateFunc == null)
            {
                Debug.LogError("Provide initial state getter");
            }

            GameState next = InitialStateFunc();
            context.Get<GameStateManager>().SwitchToState(next);
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
            if (Application.isPlaying && enabled && _timer != null)
            {
                _timer.OnDrawGizmos();
            }
        }

        private void Install()
        {
            Context.Install(this);

            var injectables = new List<UnityEngine.Object>();
            injectables.AddRange(_injectableMonoBehaviours);
            injectables.AddRange(_injectableScriptableObjects);

            var childrens = GetChildrenDeeply(transform, _injectionDepth);
            for (int i = 0, length = childrens.Count; i < length; i++)
            {
                injectables.AddRange(childrens[i].GetComponents<MonoBehaviour>());
            }

            var uniqueInjectables = new HashSet<UnityEngine.Object>(injectables).ToList();

            for (int i = 0, length = uniqueInjectables.Count; i < length; i++)
            {
                var item = uniqueInjectables[i];

                var attributeObject = item.GetType()
                    .GetCustomAttributes(typeof(Injectable), true)
                    .FirstOrDefault();

                if (attributeObject != null)
                {
                    var attribute = attributeObject as Injectable;

                    if (attribute.Type != null)
                    {
                        Context.InstallByType(item, attribute.Type);
                    }
                    else
                    {
                        Context.Install(item);
                    }
                }
                else
                {
                    Context.Install(item);
                }
            }
        }

        private static List<Transform> GetChildrenDeeply(Transform parent, int depth)
        {
            var result = new List<Transform>();

            result.Add(parent);

            if (depth <= 0)
            {
                return result;
            }

            for (int i = 0; i < parent.childCount; i++)
            {
                var child = parent.GetChild(i);
                result.AddRange(GetChildrenDeeply(child, depth - 1));
            }

            return result;
        }
    }
}
