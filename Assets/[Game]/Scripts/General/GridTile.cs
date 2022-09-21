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
    public int rowIndex;
    Transform _transform;

    void Awake()
    {
        _transform = transform;
    }

    public void InitGridTile(Vector3 _localPosition, int _rowIndex, MatchItemTypes _matchItemType)
    {
        transform.localPosition = _localPosition;
        rowIndex = _rowIndex;

        activeMatchItem = MatchItemPoolManager.Instance.FetchFromPool(); 
        activeMatchItem.SpawnOnGridTile(this, _matchItemType, rowIndex);
    }

    public void SetNeighbours(List<GridTile> _neighbourTiles)
    {
        neighbourTiles = _neighbourTiles;
    }

    void OnMouseDown()
    {
        Clicked();
    }

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
        //Debug.Log("Clicked : " + gameObject.name + ", pos : " + transform.localPosition);
    }

    public bool CheckNeighbourTypeMatching(MatchItemTypes _matchItemType)
    {
        return (activeMatchItem && activeMatchItem.matchItemType == _matchItemType);
    }

    public void Matched(GridTile _gridTile)
    {
        //Test
        Destroy(activeMatchItem.gameObject);
        activeMatchItem = null;
    }

} // class
