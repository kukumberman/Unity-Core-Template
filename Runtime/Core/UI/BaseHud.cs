using Core;
using UnityEngine;
using UnityEngine.UIElements;

namespace Game.UI.Hud
{
    public interface IHud
    {
        bool IsActive { get; set; }

        void Remove();
    }

    public abstract class BaseHud : MonoBehaviour, IHud
    {
        public bool IsActive
        {
            get { return gameObject.activeSelf; }
            set { gameObject.SetActive(value); }
        }

        public void Remove()
        {
            Destroy(gameObject);
        }

        protected abstract void OnEnable();
        protected abstract void OnDisable();
    }

    public abstract class BaseHudWithModel<T> : BaseHud, IObserver
        where T : Observable
    {
        private T _model;

        public T Model
        {
            protected get { return _model; }
            set
            {
                if (null != _model)
                {
                    _model.RemoveObserver(this);
                }

                _model = value;
                OnApplyModel(value);

                if (null != _model)
                {
                    _model.AddObserver(this);
                    OnModelChanged(_model);
                }
            }
        }

        protected BaseHudWithModel() { }

        protected abstract void OnModelChanged(T model);

        protected virtual void OnApplyModel(T model) { }

        #region Observer implementation
        public void OnObjectChanged(Observable observable)
        {
            if (observable is T)
            {
                OnModelChanged((T)observable);
            }
            else
            {
                OnModelChanged(Model);
            }
        }
        #endregion
    }

    public abstract class BaseHudUITK : VisualElement, IHud
    {
        public bool IsActive
        {
            get { return style.display != DisplayStyle.None; }
            set { style.display = value ? DisplayStyle.Flex : DisplayStyle.None; }
        }

        public void Remove()
        {
            OnDisable();

            RemoveFromHierarchy();
        }

        protected abstract void OnEnable();
        protected abstract void OnDisable();
    }

    public abstract class BaseHudWithModelUITK<T> : BaseHudUITK, IObserver
        where T : Observable
    {
        private T _model;

        public T Model
        {
            protected get { return _model; }
            set
            {
                if (null != _model)
                {
                    _model.RemoveObserver(this);
                }

                _model = value;
                OnApplyModel(value);

                if (null != _model)
                {
                    _model.AddObserver(this);
                    OnModelChanged(_model);
                }
            }
        }

        protected abstract void OnModelChanged(T model);

        protected virtual void OnApplyModel(T model) { }

        #region Observer implementation
        public void OnObjectChanged(Observable observable)
        {
            if (observable is T)
            {
                OnModelChanged((T)observable);
            }
            else
            {
                OnModelChanged(Model);
            }
        }
        #endregion
    }
}
