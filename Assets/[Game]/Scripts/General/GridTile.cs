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
    public int rowIndex;

    public void InitGridTile(Vector3 _localPosition, int _rowIndex, MatchItemTypes _matchItemType)
    {
        transform.localPosition = _localPosition;

        activeMatchItem = MatchItemPoolManager.Instance.FetchFromPool();

        rowIndex = _rowIndex;
        activeMatchItem.SpawnOnGridTile(this, _matchItemType, rowIndex);
    }


} // class
