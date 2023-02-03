using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public class HOGPoolManager
    {
        private Dictionary<string, HOGPool> Pools = new();

        public void InitPool(IHOGPoolable original, int amount, int maxAmount)
        {
            HOGManager.Instance.FactoryManager.MultiCreateAsync(original, Vector3.zero, amount,
                delegate (List<IHOGPoolable> list)
                {
                    foreach (var poolable in list)
                    {
                        poolable.name = original.name;
                    }

                    var pool = new HOGPool
                    {
                        AllPoolables = new Queue<IHOGPoolable>(list),
                        UsedPoolables = new Queue<IHOGPoolable>(),
                        AvailablePoolables = new Queue<IHOGPoolable>(list),
                        MaxPoolables = maxAmount
                    };

                    Pools.Add(original.gameObject.name, pool);
                });
        }

        public IHOGPoolable GetPoolable(string poolName)
        {
            if (Pools.TryGetValue(poolName, out HOGPool pool))
            {
                if (pool.AvailablePoolables.TryDequeue(out IHOGPoolable poolable))
                {
                    poolable.OnTakenFromPool();

                    pool.UsedPoolables.Enqueue(poolable);
                    return poolable;
                }

                //Create more
                Debug.Log($"pool - {poolName} no enough poolables, used poolables {pool.UsedPoolables.Count}");

                return null;
            }

            Debug.Log($"pool - {poolName} wasn't initialized");
            return null;
        }


        public void ReturnPoolable(IHOGPoolable poolable)
        {
            if (Pools.TryGetValue(poolable.poolName, out HOGPool pool))
            {
                pool.AvailablePoolables.Enqueue(poolable);
                poolable.OnReturnedToPool();
            }
        }


        public void DestroyPool(string name)
        {
            if (Pools.TryGetValue(name, out HOGPool pool))
            {
                foreach (var poolable in pool.AllPoolables)
                {
                    poolable.PreDestroy();
                    ReturnPoolable(poolable);
                }

                foreach (var poolable in pool.AllPoolables)
                {
                    Object.Destroy(poolable);
                }

                pool.AllPoolables.Clear();
                pool.AvailablePoolables.Clear();
                pool.UsedPoolables.Clear();

                Pools.Remove(name);
            }
        }
    }

    public class IHOGPoolable : HOGMonoBehaviour
    {
        public string poolName;

        public virtual void OnReturnedToPool()
        {
            this.gameObject.SetActive(false);
        }

        public virtual void OnTakenFromPool()
        {
            this.gameObject.SetActive(true);
        }

        public virtual void PreDestroy()
        {
        }
    }

    public class HOGPool
    {
        public Queue<IHOGPoolable> AllPoolables = new();
        public Queue<IHOGPoolable> UsedPoolables = new();
        public Queue<IHOGPoolable> AvailablePoolables = new();

        public int MaxPoolables = 100;
    }
}