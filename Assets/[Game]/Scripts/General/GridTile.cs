using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using CLUtils;

public class GridTile : Operator
{
    [Header("General Variables")]
    int testInt2;

    [Header("References")]
    GameObject testObj1;

    [Space(10)]
    [Header("! Debug !")]
    public MatchItem activeMatchItem;
    public List<GridTile> neighbourTiles = new();
    public int rowIndex;

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


} // class
