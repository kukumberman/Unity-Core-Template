using Game.UI.Hud;
using System;

namespace Game.Core.UI
{
    public abstract class Mediator
    {
        [Obsolete]
        public virtual int HierarchyOrder => -1;

        public virtual bool CanPop()
        {
            return false;
        }

        public abstract Type ViewType { get; }

        public abstract void Mediate(object view);
        public abstract void Unmediate();

        public abstract void InternalShow();
        public abstract void InternalHide();
    }

    public abstract class Mediator<T> : Mediator
        where T : IHud
    {
        private bool _isActive;
        protected T _view;
        public override Type ViewType => typeof(T);
        protected T View => _view;

        public sealed override void Mediate(object view)
        {
            _view = (T)view;
            _isActive = false;
        }

        public sealed override void Unmediate()
        {
            if (_isActive)
            {
                Hide();
            }
            _view.Remove();
            _view = default(T);
        }

        public sealed override void InternalShow()
        {
            _view.IsActive = true;
            Show();
        }

        public sealed override void InternalHide()
        {
            _view.IsActive = false;
            Hide();
        }

        protected abstract void Show();
        protected abstract void Hide();
    }
}
