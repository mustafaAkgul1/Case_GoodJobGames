using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using CLUtils;

public class MatchItem : Operator
{
    [Header("General Variables")]
    int testInt2;

    [Header("References")]
    public SpriteRenderer itemImage;

    [Space(10)]
    [Header("! Debug !")]
    public MatchItemStates matchItemState;
    public MatchItemTypes matchItemType;
    public GridTile boundGridTile;
    Transform poolHolder;

    public enum MatchItemStates
    {
        OnHold,
        ActiveOnGrid,
        ActiveOffGrid,
        Disabling
    }

    void Start()
    {
        InitVariables();
    }

    void InitVariables()
    {
        
    }

    public void DisableFromPool(Transform _poolHolder)
    {
        gameObject.SetActive(false);

        poolHolder = _poolHolder;
        transform.SetParent(poolHolder, false);

        matchItemState = MatchItemStates.OnHold;
    }

    public void SpawnOnGridTile(GridTile _gridTile, MatchItemTypes _matchItemType, int _rowIndex)
    {
        gameObject.SetActive(true);

        boundGridTile = _gridTile;
        matchItemType = _matchItemType;

        itemImage.sortingOrder = -_rowIndex;
        HandleGroupSprite();

        transform.SetParent(boundGridTile.transform, false);

        matchItemState = MatchItemStates.ActiveOnGrid;
    }

    void HandleGroupSprite()
    {
        //TODO : change this with group sprites
        itemImage.sprite = DataManager.Instance.matchItemTypesData.matchItemSprites[matchItemType][0];
    }

} // class
