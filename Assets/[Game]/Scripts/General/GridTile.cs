using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using CLUtils;

public class GridTile : Operator, IClickable
{
    [Header("General Variables")]
    int testInt2;

    [Header("References")]
    GameObject test;

    [Space(10)]
    [Header("! Debug !")]
    public MatchItem activeMatchItem;
    public List<GridTile> neighbourTiles = new();
    public Vector2Int gridIndex;

    public void InitGridTile(Vector3 _localPosition, Vector2Int _gridIndex, MatchItemTypes _matchItemType)
    {
        transform.localPosition = _localPosition;
        gridIndex = _gridIndex;

        activeMatchItem = MatchItemPoolManager.Instance.FetchFromPool(); 
        activeMatchItem.SpawnOnGridTile(this, _matchItemType);
    }

    public void SetNeighbours(List<GridTile> _neighbourTiles)
    {
        neighbourTiles = _neighbourTiles;
    }

    //void OnMouseDown() // Another option to handle click
    //{
    //    Clicked();
    //}

    public void Clicked()
    {
        if (!activeMatchItem)
        {
            return;
        }

        TriggerClickFeeling();

        Anounce(EventManager<object[]>.OnGridClicked, this);
    }

    void TriggerClickFeeling()
    {
        //TODO : tween rotation
    }

    public bool CheckNeighbourTypeMatching(MatchItemTypes _matchItemType)
    {
        return (activeMatchItem && activeMatchItem.matchItemType == _matchItemType);
    }

    public void Matched(GridTile _gridTile)
    {
        MatchItemPoolManager.Instance.AddToPool(activeMatchItem);
        activeMatchItem = null;
    }

    public void SetActiveMatchItem(MatchItem _matchItem)
    {
        if (_matchItem == null)
        {
            activeMatchItem = null;
            return;
        }

        if (activeMatchItem)
        {
            Debug.LogError("WTF, i have already a match item from setter : " + activeMatchItem.matchItemType, gameObject);
        }

        activeMatchItem = _matchItem;
        activeMatchItem.ChangeGridTile(this);
    }

    public void SpawnActiveMatchItem(MatchItem _matchItem, MatchItemTypes _matchItemType, float _spawnOffsetter)
    {
        if (activeMatchItem)
        {
            Debug.LogWarning("WTF, i have already a match item from off grid spawn : " + activeMatchItem.matchItemType);
        }

        activeMatchItem = _matchItem;
        activeMatchItem.SpawnOffGridTile(this, _matchItemType, _spawnOffsetter);
    }

} // class
