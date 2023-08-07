using System;
using System.Collections.Generic;

namespace Injection
{
    public sealed class Context : IDisposable
    {
        public static Context Current { get; set; }

        private readonly Dictionary<Type, object> _objectsMap;
        private readonly Dictionary<string, object> _objectsByNameMap;

        public Context()
        {
            _objectsMap = new Dictionary<Type, object>(100);
            _objectsMap[typeof(Context)] = this;
            _objectsByNameMap = new(100);
        }

        public Context(Context parent)
        {
            _objectsMap = new Dictionary<Type, object>(parent._objectsMap);
            _objectsMap[typeof(Context)] = this;
            _objectsByNameMap = new(parent._objectsByNameMap);
        }

        public void Dispose()
        {
            foreach (var item in _objectsMap)
            {
                if (this == item.Value)
                    continue;

                if (item.Value is IDisposable)
                {
                    (item.Value as IDisposable).Dispose();
                }
            }
            _objectsMap.Clear();
            _objectsByNameMap.Clear();
        }

        public void Install(params object[] objects)
        {
            foreach (object obj in objects)
            {
                _objectsMap[obj.GetType()] = obj;
            }
        }

        public void InstallByType(object obj, Type type)
        {
            _objectsMap[type] = obj;
        }

        public void InstallByName(string name, object obj)
        {
            _objectsByNameMap[name] = obj;
        }

        public void ApplyInstall()
        {
            var injector = Get<Injector>();
            foreach (object obj in _objectsMap.Values)
            {
                injector.Inject(obj);
            }

            foreach (var obj in _objectsByNameMap.Values)
            {
                injector.Inject(obj);
            }
        }

        public void Uninstall(params object[] objects)
        {
            foreach (object obj in objects)
            {
                _objectsMap.Remove(obj.GetType());
            }
        }

        public void UninstallByName(string name)
        {
            _objectsByNameMap.Remove(name);
        }

        public T Get<T>()
            where T : class
        {
#if UNITY_EDITOR
            if (!_objectsMap.ContainsKey(typeof(T)))
            {
                throw new KeyNotFoundException("Not found " + typeof(T));
            }
#endif

            return (T)_objectsMap[typeof(T)];
        }

        public object Get(Type type)
        {
#if UNITY_EDITOR
            if (!_objectsMap.ContainsKey(type))
            {
                throw new KeyNotFoundException("Not found " + type);
            }
#endif
            return _objectsMap[type];
        }

        public object GetByName(string name)
        {
#if UNITY_EDITOR
            if (!_objectsByNameMap.ContainsKey(name))
            {
                throw new KeyNotFoundException("Not found " + name);
            }
#endif
            return _objectsByNameMap[name];
        }
    }
}
