using UnityEngine;
using DG.Tweening;
using CLUtils;

public class MatchItem : Operator
{
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
    MatchingDataSO matchingData;

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
        matchingData = DataManager.Instance.matchingData;
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
        InitGroupSprite();

        _transform.SetParent(boundGridTile.transform, false);

        matchItemState = MatchItemStates.ActiveOnGrid;
    }

    public void SpawnOffGridTile(GridTile _gridTile, MatchItemTypes _matchItemType, float _spawnOffsetter)
    {
        gameObject.SetActive(true);

        boundGridTile = _gridTile;
        matchItemType = _matchItemType;

        itemImage.sortingOrder = boundGridTile.gridIndex.y;
        InitGroupSprite();

        _transform.SetParent(boundGridTile.transform, false);

        float _gridTileSize = DataManager.Instance.gridData.gridTileSize;
        float _maxGridYValue = DataManager.Instance.gridData.rowCount * (_gridTileSize * 0.5f);
        float _resultOffset = _maxGridYValue + ((_spawnOffsetter + matchingData.offGridMatchItemSpawnOffset) * _gridTileSize);

        Vector3 _position = _transform.position;
        _position.y = _resultOffset;
        _transform.position = _position;

        matchItemState = MatchItemStates.ActiveOffGrid;

        TweenToLocalZero(matchingData.offGridMatchItemFallSpeed, matchingData.offGridMatchItemFallEase, delegate
        {
            matchItemState = MatchItemStates.ActiveOnGrid;
        });
    }

    public void ChangeGridTile(GridTile _gridTile)
    {
        boundGridTile = _gridTile;

        itemImage.sortingOrder = boundGridTile.gridIndex.y;
        InitGroupSprite();

        _transform.SetParent(boundGridTile.transform);

        matchItemState = MatchItemStates.ActiveOffGrid;

        TweenToLocalZero(matchingData.onGridMatchItemFallSpeed, matchingData.onGridMatchItemFallEase, delegate
        {
            matchItemState = MatchItemStates.ActiveOnGrid;
        });
    }

    void InitGroupSprite()
    {
        itemImage.sprite = DataManager.Instance.matchItemTypesData.matchItemSprites[matchItemType][0];
    }

    public void ChangeGroupSprite(Sprite _sprite)
    {
        itemImage.sprite = _sprite;
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
