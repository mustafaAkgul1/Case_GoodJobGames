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

    public void SpawnOnGridTile(GridTile _gridTile, MatchItemTypes _matchItemType, Vector2Int _coordIndex)
    {
        gameObject.SetActive(true);

        boundGridTile = _gridTile;
        matchItemType = _matchItemType;

        itemImage.sortingOrder = -_coordIndex.x;
        HandleGroupSprite();

        _transform.SetParent(boundGridTile.transform, false);

        matchItemState = MatchItemStates.ActiveOnGrid;
    }

    void HandleGroupSprite()
    {
        //TODO : change this with group sprites
        itemImage.sprite = DataManager.Instance.matchItemTypesData.matchItemSprites[matchItemType][0];
    }

    public void SpawnOffGridTile(GridTile _gridTile, MatchItemTypes _matchItemType, Vector2Int _coordIndex)
    {
        gameObject.SetActive(true);

        boundGridTile = _gridTile;
        matchItemType = _matchItemType;

        itemImage.sortingOrder = -_coordIndex.x;
        HandleGroupSprite();

        _transform.SetParent(boundGridTile.transform);
        _transform.localPosition = Vector3.up * ((((float)_coordIndex.x / DataManager.Instance.gridData.rowSize) * DataManager.Instance.gridData.rowSize) + 3f);

        matchItemState = MatchItemStates.ActiveOffGrid;

        TweenToLocalZero(10f, delegate
        {
            matchItemState = MatchItemStates.ActiveOnGrid;
        });
    }

    public void ChangeGridTile(GridTile _gridTile, Vector2Int _coordIndex)
    {
        boundGridTile = _gridTile;

        itemImage.sortingOrder = -_coordIndex.x;
        HandleGroupSprite();

        _transform.SetParent(boundGridTile.transform);

        matchItemState = MatchItemStates.ActiveOffGrid;

        TweenToLocalZero(10f, delegate
        {
            matchItemState = MatchItemStates.ActiveOnGrid;
        });
    }

    void TweenToLocalZero(float _duration, System.Action _callBack)
    {
        if (movingTween != null)
        {
            movingTween.Kill();
        }

        movingTween = _transform.DOLocalMove(Vector3.zero, _duration)
            .SetEase(Ease.InBack)
            .SetLink(gameObject)
            .SetSpeedBased()
            .OnComplete(delegate
            {
                _callBack?.Invoke();
            });
    }

} // class
