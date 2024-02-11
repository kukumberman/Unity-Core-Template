using Game.Core.UI;
using Game.Enums;
using Injection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.Managers
{
    public sealed class HudManager
    {
        public Action<bool> SINGLE_HUD_OPENED;

        [Inject]
        private Injector _injector;

        private Mediator _openedHud;
        private readonly List<Mediator> _additionalHuds;

        private readonly IHudFactory _hudFactory;

        public HudOrientation Orientation
        {
            get => _hudFactory.Orientation;
            set => _hudFactory.Orientation = value;
        }

        public IHudFactory HudFactory => _hudFactory;

        public HudManager(IHudFactory factory)
        {
            _hudFactory = factory;
            _additionalHuds = new List<Mediator>();
        }

        public bool IsShowed<T>()
            where T : Mediator
        {
            return _openedHud is T || _additionalHuds.Exists(temp => temp is T);
        }

        public bool IsShowed(Type type)
        {
            if (type == null)
            {
                throw new NullReferenceException();
            }

            if (!typeof(Mediator).IsAssignableFrom(type))
            {
                throw new ArgumentException();
            }

            if (_openedHud != null && _openedHud.GetType() == type)
            {
                return true;
            }

            for (int i = 0; i < _additionalHuds.Count; i++)
            {
                if (_additionalHuds[i].GetType() == type)
                {
                    return true;
                }
            }

            return false;
        }

        public bool IsSingleOpened()
        {
            return null != _openedHud;
        }

        public T ShowSingle<T>(params object[] args)
            where T : Mediator
        {
            if (null != _openedHud)
            {
                HideSingle();
            }

            _openedHud = (Mediator)Activator.CreateInstance(typeof(T), args);
            _injector.Inject(_openedHud);

            _hudFactory.Create(_openedHud);

            SINGLE_HUD_OPENED.SafeInvoke(true);

            return (T)_openedHud;
        }

        public void HideSingle()
        {
            if (null == _openedHud)
                return;

            _openedHud.InternalHide();
            _openedHud.Unmediate();
            _openedHud = null;

            SINGLE_HUD_OPENED.SafeInvoke(false);
        }

        public void ShowAdditional(Mediator hud)
        {
            _injector.Inject(hud);
            _hudFactory.Create(hud);
            _additionalHuds.Add(hud);
        }

        public T ShowAdditional<T>(params object[] args)
            where T : Mediator
        {
            var opened = _additionalHuds.FirstOrDefault(temp => temp is T);
            if (null != opened)
            {
                Logger.LogWarning(typeof(T) + " is already opened");
                return opened as T;
            }

            var hud = (Mediator)Activator.CreateInstance(typeof(T), args);
            ShowAdditional(hud);
            return (T)hud;
        }

        public void HideAdditional<T>()
            where T : Mediator
        {
            for (int i = _additionalHuds.Count - 1; i >= 0; i--)
            {
                var hud = _additionalHuds[i];

                if (!(hud is T))
                    continue;

                hud.InternalHide();
                hud.Unmediate();
                _additionalHuds.RemoveAt(i);
            }
        }

        public bool HideAdditional(Type type)
        {
            if (type == null)
            {
                throw new NullReferenceException();
            }

            if (!typeof(Mediator).IsAssignableFrom(type))
            {
                throw new ArgumentException();
            }

            for (int i = _additionalHuds.Count - 1; i >= 0; i--)
            {
                var hud = _additionalHuds[i];

                if (hud.GetType() == type)
                {
                    hud.InternalHide();
                    hud.Unmediate();
                    _additionalHuds.RemoveAt(i);
                    return true;
                }
            }

            return false;
        }

        public void HideAllAdditionals()
        {
            for (int i = _additionalHuds.Count - 1; i >= 0; i--)
            {
                var hud = _additionalHuds[i];

                hud.InternalHide();
                hud.Unmediate();
                _additionalHuds.RemoveAt(i);
            }
        }

        public bool TryPop()
        {
            for (int i = _additionalHuds.Count - 1; i >= 0; i--)
            {
                var hud = _additionalHuds[i];

                if (hud.CanPop())
                {
                    hud.InternalHide();
                    hud.Unmediate();
                    _additionalHuds.RemoveAt(i);

                    return true;
                }
            }

            return false;
        }
    }
}
