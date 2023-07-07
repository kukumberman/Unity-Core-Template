using Core;
using Game.Core.UI;

public abstract class BaseToolkitHudWithModel<T> : IToolkitHud, IObserver
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

    public abstract void OnEnable();

    public abstract void OnDisable();

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
