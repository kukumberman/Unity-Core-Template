using Game;
using UnityEngine;
using _RootNamespace_.Enums;
using _RootNamespace_.States;

namespace _RootNamespace_
{
    public sealed class _Project_StartBehaviour : MonoBehaviour
    {
        [SerializeField]
        private GameStartBehaviour _gameStart;

        [SerializeField]
        private bool _overrideInitialState;

        [SerializeField]
        private StateType _initialState;

        public bool OverrideInitialState => Application.isEditor ? _overrideInitialState : false;

        public StateType InitialState => _initialState;

        private void Awake()
        {
            _gameStart.InitialStateFunc = () =>
            {
                return new _Project_InitializeState();
            };
        }
    }
}
