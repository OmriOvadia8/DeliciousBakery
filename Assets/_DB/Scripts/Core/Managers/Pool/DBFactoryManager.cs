using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DB_Core
{
    public class DBFactoryManager
    {
        public void CreateAsync<T>(T origin, Vector3 pos, Action<T> onCreated) where T : Object
        {
            var clone = Object.Instantiate(origin, pos, Quaternion.identity);
            onCreated?.Invoke(clone);
        }

        public void MultiCreateAsync<T>(T origin, Vector3 pos, int amount, Action<List<T>> onCreated) where T : Object
        {
            var createdObjects = new List<T>();

            for (var i = 0; i < amount; i++)
            {
                CreateAsync(origin, pos, OnCreated);
            }

            void OnCreated(T createdObject)
            {
                createdObjects.Add(createdObject);

                if (createdObjects.Count == amount)
                {
                    onCreated?.Invoke(createdObjects);
                }
            }
        }
    }
}