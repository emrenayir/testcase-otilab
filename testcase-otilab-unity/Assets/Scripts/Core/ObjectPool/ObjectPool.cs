using System.Collections.Generic;
using UnityEngine;

namespace Core.ObjectPool
{
    public class ObjectPool<T> where T : Component
    {
        private T prefab;
        private Transform parent;
        private Queue<T> pool;
        private List<T> activeObjects;

        public ObjectPool(T prefab, int initialSize, Transform parent = null)
        {
            this.prefab = prefab;
            this.parent = parent;
            pool = new Queue<T>();
            activeObjects = new List<T>();

            Debug.Log($"Initializing object pool for {prefab.name} with {initialSize} objects");
            
            for (int i = 0; i < initialSize; i++)
            {
                CreateNewObject();
            }
        }

        private void CreateNewObject()
        {
            T obj = Object.Instantiate(prefab, prefab.transform.position, prefab.transform.rotation, parent);
            obj.gameObject.SetActive(false);
            pool.Enqueue(obj);
        }

        public T Get()
        {
            if (pool.Count == 0)
            {
                CreateNewObject();
            }

            T obj = pool.Dequeue();
            obj.gameObject.SetActive(true);
            activeObjects.Add(obj);
            return obj;
        }

        public void Return(T obj)
        {
            obj.gameObject.SetActive(false);
            pool.Enqueue(obj);
            activeObjects.Remove(obj);
        }

        public void ReturnAll()
        {
            foreach (T obj in activeObjects.ToArray())
            {
                Return(obj);
            }
        }

        public List<T> GetActiveObjects()
        {
            return activeObjects;
        }
    }
} 