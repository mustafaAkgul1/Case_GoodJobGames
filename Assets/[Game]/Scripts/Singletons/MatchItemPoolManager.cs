using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using CLUtils;

public class MatchItemPoolManager : SingletonOperator<MatchItemPoolManager>
{
    [Header("General Variables")]
    [SerializeField] int poolCount = 250;

    [Header("References")]
    [SerializeField] Transform poolHolder;
    [SerializeField] MatchItem poolerPrefab;

    [Space(10)]
    [Header("! Debug !")]
    Queue<MatchItem> poolQueue;

    public override void Awake()
    {
        base.Awake();

        InitPool();
    }

    void InitPool()
    {
        poolQueue = new Queue<MatchItem>();

        for (int i = 0; i < poolCount; i++)
        {
            MatchItem _poolerActor = Instantiate(poolerPrefab, poolHolder);
            _poolerActor.DisableFromPool(poolHolder);
            poolQueue.Enqueue(_poolerActor);
        }
    }

    public MatchItem FetchFromPool()
    {
        return poolQueue.Count > 0 ? poolQueue.Dequeue() : null;
    }

    public void AddToPool(MatchItem _poolerActor)
    {
        if (!poolQueue.Contains(_poolerActor))
        {
            _poolerActor.DisableFromPool(poolHolder);
            poolQueue.Enqueue(_poolerActor);
        }
    }

} // class