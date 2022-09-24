#region Header
//Created by Mustafa Akgul (CL)
//mustafaakgul248@gmail.com
#endregion

using System.Collections.Generic;
using UnityEngine;

namespace CLUtils
{
    public class TextIndicatorPoolManager : SingletonOperator<TextIndicatorPoolManager>
    {
        [Header("General Variables")]
        [SerializeField] int poolCount = 50;

        [Header("References")]
        [SerializeField] Transform poolHolder;
        [SerializeField] TextIndicator poolerPrefab;

        [Space(10)]
        [Header("! Debug !")]
        Queue<TextIndicator> poolQueue;

        void Start()
        {
            InitPool();
        }

        void InitPool()
        {
            poolQueue = new Queue<TextIndicator>();

            for (int i = 0; i < poolCount; i++)
            {
                TextIndicator _poolerActor = Instantiate(poolerPrefab, poolHolder);
                _poolerActor.DisableFromPool(poolHolder);
                poolQueue.Enqueue(_poolerActor);
            }
        }

        public TextIndicator FetchFromPool()
        {
            //null can be replaced with extra Instantiate to prevent overflow pool
            return poolQueue.Count > 0 ? poolQueue.Dequeue() : null;
        }

        public void AddToPool(TextIndicator _poolerActor)
        {
            if (!poolQueue.Contains(_poolerActor))
            {
                _poolerActor.DisableFromPool(poolHolder);
                poolQueue.Enqueue(_poolerActor);
            }
        }

    } // class
} // namespace
