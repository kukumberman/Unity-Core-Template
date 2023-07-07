using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.UI.Pool
{
    public class MultiplyComponentPoolFactory : MonoBehaviour, IComponentPoolFactory
    {
        [SerializeField]
        private GameObject[] _prefabs;
        [SerializeField]
        private int _count;
        [SerializeField]
        private Transform _content;
        [SerializeField]
        private Transform _poolStorage;

        private readonly Dictionary<Type, HashSet<GameObject>> _instances;
        private Dictionary<Type, Queue<GameObject>> _pool;

        public Transform Content { get { return _content; } }

        public MultiplyComponentPoolFactory()
        {
            _instances = new Dictionary<Type, HashSet<GameObject>>();
            _pool = new Dictionary<Type, Queue<GameObject>>();
        }

        public int CountInstances
        {
            get { return _instances.Count; }
        }

        private void Awake()
        {
            if (_instances.Count > 0)
                return;

            for (int i = 0; i < _count; i++)
            {
                Get<Transform>();
            }
            ReleaseAllInstances();
        }

        public T Get<T>() where T : Component
        {
            return Get<T>(_instances.Count);
        }

        public Component Get(Type type) 
        {
            return Get(type, _instances.Count);
        }

        public T Get<T>(int sublingIndex) where T : Component
        {
            bool isNewInstance = false;

            if (GetPool(typeof(T)).Count == 0)
            {
                GameObject result = GetInstance<T>();
                GetPool(typeof(T)).Enqueue(result);
                isNewInstance = true;
            }

            T resultComponent = GetPool(typeof(T)).Dequeue().GetComponent<T>();
            if (null == resultComponent)
            {
                return resultComponent;
            }

            var go = resultComponent.gameObject;
            var t = resultComponent.transform;
            if (isNewInstance || (_poolStorage != null && _poolStorage != _content))
            {
                t.SetParent(_content, false);
            }

            GetInstances(typeof(T)).Add(go);

            if (!go.activeSelf)
            {
                go.SetActive(true);
            }

            if (t.GetSiblingIndex() != sublingIndex)
            {
                t.SetSiblingIndex(sublingIndex);
            }

            return resultComponent;
        }

        public Component Get (Type type, int sublingIndex)
        {
            bool isNewInstance = false;

            if (GetPool(type).Count == 0)
            {
                GameObject result = GetInstance(type);
                GetPool(type).Enqueue(result);
                isNewInstance = true;
            }

            var resultComponent = GetPool(type).Dequeue().GetComponent(type);
            if (null == resultComponent)
            {
                return resultComponent;
            }

            var go = resultComponent.gameObject;
            var t = resultComponent.transform;
            if (isNewInstance || (_poolStorage != null && _poolStorage != _content))
            {
                t.SetParent(_content, false);
            }

            GetInstances(type).Add(go);

            if (!go.activeSelf)
            {
                go.SetActive(true);
            }

            if (t.GetSiblingIndex() != sublingIndex)
            {
                t.SetSiblingIndex(sublingIndex);
            }

            return resultComponent;
        }

        public GameObject GetInstance<T>() where T : Component
        {
            foreach (var prefab in _prefabs)
            {
                var component = prefab.gameObject.GetComponent(typeof(T));
                if (component != null)
                {
                    return Instantiate(prefab);
                }
            }

            return null;
        }

        public GameObject GetInstance(Type type)
        {
            foreach (var prefab in _prefabs)
            {
                var component = prefab.gameObject.GetComponent(type);
                if (component != null)
                {
                    return Instantiate(prefab);
                }
            }

            return null;
        }

        public void Release<T>(T component) where T : Component
        {
           
            var go = component.gameObject;
            if (GetInstances(typeof(T)).Contains(go))
            {
                go.SetActive(false);
                if (_poolStorage)
                {
                    go.transform.SetParent(_poolStorage, false);
                }
                GetPool(typeof(T)).Enqueue(go);
                GetInstances(typeof(T)).Remove(go);
            }
        }

        public void Release(GameObject gameObject, Type type)
        {
            if (GetInstances(type).Contains(gameObject))
            {
                gameObject.SetActive(false);
                if (_poolStorage)
                {
                    gameObject.transform.SetParent(_poolStorage, false);
                }
                GetPool(type).Enqueue(gameObject);
                GetInstances(type).Remove(gameObject);
            }
        }

        public void ReleaseComponent(Component component)
        {
            Type type = component.GetType();

            var go = component.gameObject;
            if (GetInstances(type).Contains(go))
            {
                go.SetActive(false);
                if (_poolStorage)
                {
                    go.transform.SetParent(_poolStorage, false);
                }

                GetPool(type).Enqueue(go);
                GetInstances(type).Remove(go);
            }
        }

        public void ReleaseAllInstances()
        {
            foreach (var hashSet in _instances)
            {
                foreach (GameObject instance in hashSet.Value)
                {
                    instance.SetActive(false);
                    GetPool(hashSet.Key).Enqueue(instance);
                }
            }

            _instances.Clear();
        }

        public void Dispose()
        {
            ReleaseAllInstances();

            foreach (var queue in _pool.Values)
            {
                foreach (GameObject gameObject in queue)
                {
                    GameObject.Destroy(gameObject);
                }
            }

            _pool.Clear();
        }

        private Queue<GameObject> GetPool(Type type)
        {
            Queue<GameObject> result;

            if (!_pool.TryGetValue(type, out result))
            {
                var newQueue = result = new Queue<GameObject>();
                _pool.Add(type, newQueue);
            }

            return result;
        }

        private HashSet<GameObject> GetInstances(Type type)
        {
            HashSet<GameObject> result;

            if (!_instances.TryGetValue(type, out result))
            {
                var newHashSet = result = new HashSet<GameObject>();
                _instances.Add(type, newHashSet);
            }

            return result;
        }
    }
}
