using System;
using UnityEngine;

namespace Game.Core
{
    public sealed class Timer
    {
        private readonly OneListener _tickListener = new OneListener();
        public event Action TICK
        {
            add { _tickListener.Add(value); }
            remove { _tickListener.Remove(value); }
        }

        private readonly OneListener _postTickListener = new OneListener();
        public event Action POST_TICK
        {
            add { _postTickListener.Add(value); }
            remove { _postTickListener.Remove(value); }
        }

        private readonly OneListener _fixedTickListener = new OneListener();
        public event Action FIXED_TICK
        {
            add { _fixedTickListener.Add(value); }
            remove { _fixedTickListener.Remove(value); }
        }

        private readonly OneListener _oneSecondTickListener = new OneListener();

        public event Action ONE_SECOND_TICK
        {
            add { _oneSecondTickListener.Add(value); }
            remove { _oneSecondTickListener.Remove(value); }
        }

        private readonly OneListener _drawGizmosTickListener = new OneListener();

        public event Action DRAW_GIZMOS
        {
            add { _drawGizmosTickListener.Add(value); }
            remove { _drawGizmosTickListener.Remove(value); }
        }

        private float _unscaledTime;
        private float _lastTime;
        private float _deltaTime;
        private float _scaleTime;
        private float _time;

        public Timer()
        {
            _lastTime = GetTime();
            _scaleTime = 1f;
            _deltaTime = 0f;
            _time = 0f;
        }

        public float Time { get { return _time; } }
        public float DeltaTime { get { return _deltaTime; } }
        public float TimeScale { get { return _scaleTime; } set { _scaleTime = Math.Max(0f, value); } }
        public float UnscaladeTime { get { return _unscaledTime; } }

        public void Update()
        {
            var now = GetTime();
            var delta = now - _lastTime;
            _unscaledTime += delta;
            _deltaTime = delta * TimeScale;
            _time += _deltaTime;

            bool isNewSecondTick = Mathf.Floor(now) > Mathf.Floor(_lastTime);

            _lastTime = now;

            _tickListener.Invoke();

            if (isNewSecondTick)
            {
                _oneSecondTickListener.Invoke();
            }
        }

        public void LateUpdate()
        {
            _postTickListener.Invoke();
        }

        public void FixedUpdate()
        {
            _fixedTickListener.Invoke();
        }

        public void OnDrawGizmos()
        {
            _drawGizmosTickListener.Invoke();
        }

        private float GetTime()
        {
            return Environment.TickCount / 1000f;
        }
    }
}