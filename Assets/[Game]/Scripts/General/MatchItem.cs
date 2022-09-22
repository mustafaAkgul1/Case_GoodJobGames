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
    Tween movingTween;
    Transform _transform;

    public enum MatchItemStates
    {
        OnHold,
        ActiveOnGrid,
        ActiveOffGrid,
        Disabling
    }

    void Awake()
    {
        _transform = transform;
    }

    public void DisableFromPool(Transform _poolHolder)
    {
        gameObject.SetActive(false);

        poolHolder = _poolHolder;
        _transform.SetParent(poolHolder, false);

        matchItemState = MatchItemStates.OnHold;
    }

    public void SpawnOnGridTile(GridTile _gridTile, MatchItemTypes _matchItemType)
    {
        gameObject.SetActive(true);

        boundGridTile = _gridTile;
        matchItemType = _matchItemType;

        itemImage.sortingOrder = boundGridTile.gridIndex.y;
        HandleGroupSprite();

        _transform.SetParent(boundGridTile.transform, false);

        matchItemState = MatchItemStates.ActiveOnGrid;
    }

    void HandleGroupSprite()
    {
        //TODO : change this with group sprites
        itemImage.sprite = DataManager.Instance.matchItemTypesData.matchItemSprites[matchItemType][0];
    }

    public void SpawnOffGridTile(GridTile _gridTile, MatchItemTypes _matchItemType, float _spawnOffsetter)
    {
        gameObject.SetActive(true);

        boundGridTile = _gridTile;
        matchItemType = _matchItemType;

        itemImage.sortingOrder = boundGridTile.gridIndex.y;
        HandleGroupSprite();

        _transform.SetParent(boundGridTile.transform, false);

        int _rowCount = DataManager.Instance.gridData.rowCount;
        int _extraOffsetValue = 3;
        float _gridTileSize = DataManager.Instance.gridData.gridTileSize;
        float _gridTileHalfSize = DataManager.Instance.gridData.gridTileSize * 0.5f;
        float _maxGridYValue = _rowCount * _gridTileHalfSize;

        float _resultOffset = _maxGridYValue + ((_spawnOffsetter + _extraOffsetValue) * _gridTileSize);

        Vector3 _position = _transform.position;
        _position.y = _resultOffset;
        _transform.position = _position;

        matchItemState = MatchItemStates.ActiveOffGrid;

        TweenToLocalZero(15f, Ease.Linear, delegate
        {
            matchItemState = MatchItemStates.ActiveOnGrid;
        });
    }

    public void ChangeGridTile(GridTile _gridTile)
    {
        boundGridTile = _gridTile;

        itemImage.sortingOrder = boundGridTile.gridIndex.y;
        HandleGroupSprite();

        _transform.SetParent(boundGridTile.transform);

        matchItemState = MatchItemStates.ActiveOffGrid;

        TweenToLocalZero(15f, Ease.InBack, delegate
        {
            matchItemState = MatchItemStates.ActiveOnGrid;
        });
    }

    void TweenToLocalZero(float _duration, Ease _ease, System.Action _callBack)
    {
        if (movingTween != null)
        {
            movingTween.Kill();
        }

        movingTween = _transform.DOLocalMove(Vector3.zero, _duration)
            .SetEase(_ease)
            .SetLink(gameObject)
            .SetSpeedBased()
            .OnComplete(delegate
            {
                _callBack?.Invoke();
            });
    }

} // class
