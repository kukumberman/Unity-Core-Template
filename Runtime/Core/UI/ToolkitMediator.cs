using System;
using UnityEngine.UIElements;

namespace Game.Core.UI
{
    public abstract class ToolkitMediator<T> : Mediator
        where T : IToolkitHud
    {
        public override Type ViewType => typeof(T);

        private bool _isActive;

        protected VisualElement _element;

        public sealed override void Mediate(object view)
        {
            _element = view as VisualElement;
            _isActive = true;
        }

        public sealed override void Unmediate()
        {
            if (_isActive)
            {
                Hide();
            }

            _element.RemoveFromHierarchy();
        }

        public sealed override void InternalShow()
        {
            Show();
        }

        public sealed override void InternalHide()
        {
            Hide();
        }

        protected abstract void Show();
        protected abstract void Hide();
    }
}
