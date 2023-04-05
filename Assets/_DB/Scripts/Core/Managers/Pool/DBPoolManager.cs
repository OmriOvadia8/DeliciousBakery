using System.Collections.Generic;
using UnityEngine;

namespace DB_Core
{
    public class DBPoolManager
    {
        private Dictionary<PoolNames, DBPool> Pools = new();

        private Transform rootPools;

        public DBPoolManager()
        {
            rootPools = new GameObject("PoolsHolder").transform;
            Object.DontDestroyOnLoad(rootPools);
        }

        public void InitPool(PoolNames poolName, int amount)
        {
            //List Of Originals 
            //Linq where PoolNames == poolName
        }

        public void InitPool(string resourceName, int amount, Transform parentTransform, int maxAmount = 100)
        {
            var original = Resources.Load<DBPoolable>(resourceName);
            InitPool(original, amount, parentTransform, maxAmount);
        }

        public void InitPool(DBPoolable original, int amount, Transform parentTransform, int maxAmount)
        {
            DBManager.Instance.FactoryManager.MultiCreateAsync(original, Vector3.zero, amount,
                delegate (List<DBPoolable> list)
                {
                    foreach (var poolable in list)
                    {
                        poolable.name = original.name;
                        poolable.transform.SetParent(parentTransform, false);
                        poolable.gameObject.SetActive(false);
                    }

                    var pool = new DBPool
                    {
                        AllPoolables = new Queue<DBPoolable>(list),
                        UsedPoolables = new Queue<DBPoolable>(),
                        AvailablePoolables = new Queue<DBPoolable>(list),
                        MaxPoolables = maxAmount
                    };

                    Pools.Add(original.poolName, pool);
                });
        }

        public DBPoolable GetPoolable(PoolNames poolName)
        {
            if (Pools.TryGetValue(poolName, out DBPool pool))
            {
                if (pool.AvailablePoolables.TryDequeue(out DBPoolable poolable))
                {
                    DBDebug.Log($"GetPoolable - {poolName}");

                    poolable.OnTakenFromPool();

                    pool.UsedPoolables.Enqueue(poolable);
                    poolable.gameObject.SetActive(true);
                    return poolable;
                }

                //Create more
                DBDebug.Log($"pool - {poolName} no enough poolables, used poolables {pool.UsedPoolables.Count}");

                return null;
            }

            DBDebug.Log($"pool - {poolName} wasn't initialized");
            return null;
        }


        public void ReturnPoolable(DBPoolable poolable)
        {
            if (Pools.TryGetValue(poolable.poolName, out DBPool pool))
            {
                pool.AvailablePoolables.Enqueue(poolable);
                poolable.OnReturnedToPool();
                poolable.gameObject.SetActive(false);
            }
        }


        public void DestroyPool(PoolNames name)
        {
            if (Pools.TryGetValue(name, out DBPool pool))
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

    public class DBPool
    {
        public Queue<DBPoolable> AllPoolables = new();
        public Queue<DBPoolable> UsedPoolables = new();
        public Queue<DBPoolable> AvailablePoolables = new();

        public int MaxPoolables = 100;
    }

    public enum PoolNames
    {
        MoneyToast = 0,
        SpendMoneyToast = 1
    }
}