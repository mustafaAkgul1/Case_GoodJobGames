using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using CLUtils;

public class GridTile : Operator, IClickable
{
    [Header("! Debug !")]
    public MatchItem activeMatchItem;
    public List<GridTile> neighbourTiles = new();
    public Vector2Int gridIndex;
    Transform _transform;
    FeelingDataSO feelingData;
    Sequence bubbleSequence;

    void Awake()
    {
        _transform = transform;
        feelingData = DataManager.Instance.feelingData;
    }

    public void InitGridTile(Vector3 _localPosition, Vector2Int _gridIndex, MatchItemTypes _matchItemType)
    {
        _transform.localPosition = _localPosition;
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

        Anounce(EventManager<object[]>.OnGridClicked, this);
    }

    public void TriggerClickBubble()
    {
        ResetBubbleTween();

        bubbleSequence = DOTween.Sequence();

        bubbleSequence
            .Join(_transform.DOPunchRotation(Vector3.forward * feelingData.gridBubbleRotationFactor * (Random.value >= 0.5f ? 1f : -1f), feelingData.gridBubbleRotationDuration))
            .Join(_transform.DOPunchScale(Vector3.one * feelingData.gridBubbleScaleFactor, feelingData.gridBubbleScaleDuration))
        ;

        bubbleSequence
            .SetEase(Ease.Linear)
            .SetLink(gameObject)
            ;
    }

    public bool CheckMatchable()
    {
        bool _isMatchable = false;

        for (int i = 0; i < neighbourTiles.Count; i++)
        {
            if (neighbourTiles[i].CompareMatchItemType(activeMatchItem.matchItemType))
            {
                _isMatchable = true;
                break;
            }
        }

        return _isMatchable;
    }

    public bool CompareMatchItemType(MatchItemTypes _matchItemType)
    {
        return (activeMatchItem && activeMatchItem.matchItemType == _matchItemType);
    }

    public void Matched(GridTile _gridTile)
    {
        activeMatchItem.Matched();
        activeMatchItem = null;

        ResetBubbleTween();
    }

    public void SetActiveMatchItem(MatchItem _matchItem, bool _checkForErrorDebug = true)
    {
        if (_matchItem == null)
        {
            activeMatchItem = null;
            return;
        }

        if (_checkForErrorDebug && activeMatchItem)
        {
            Debug.LogError("I have already a match item from setter : " + activeMatchItem.matchItemType, gameObject);
        }

        activeMatchItem = _matchItem;
        activeMatchItem.ChangeGridTile(this);
    }

    public void SpawnActiveMatchItem(MatchItem _matchItem, MatchItemTypes _matchItemType, float _spawnOffsetter)
    {
        if (activeMatchItem)
        {
            Debug.LogWarning("I have already a match item from off grid spawn : " + activeMatchItem.matchItemType);
        }

        activeMatchItem = _matchItem;
        activeMatchItem.SpawnOffGridTile(this, _matchItemType, _spawnOffsetter);
    }

    public void ChangeMatchItemSprite(Sprite _sprite)
    {
        activeMatchItem.ChangeGroupSprite(_sprite);
    }

    void ResetBubbleTween()
    {
        if (bubbleSequence != null)
        {
            bubbleSequence.Kill(true);
            bubbleSequence = null;
        }

        _transform.localEulerAngles = Vector3.zero;
        _transform.localScale = Vector3.one;
    }

} // class
