using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using CLUtils;
using UnityEngine.EventSystems;

public class GridTile : Operator, IPointerClickHandler
{
    [Header("! Debug !")]
    [HideInInspector] public MatchItem activeMatchItem;
    [HideInInspector] public Vector2Int gridIndex;
    [HideInInspector] public List<GridTile> neighbourTiles = new();
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
        activeMatchItem.SpawnFromPool(this, _matchItemType);
    }

    public void SetNeighbours(List<GridTile> _neighbourTiles)
    {
        neighbourTiles = _neighbourTiles;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Clicked();
    }

    void Clicked()
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
            .Join(_transform.DOPunchRotation(feelingData.gridBubbleRotationFactor * (Random.value >= 0.5f ? 1f : -1f) * Vector3.forward, feelingData.gridBubbleRotationDuration))
            .Join(_transform.DOPunchScale(feelingData.gridBubbleScaleFactor * Vector3.one, feelingData.gridBubbleScaleDuration))
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

    public void SetActiveMatchItem(MatchItem _matchItem)
    {
        if (!_matchItem)
        {
            activeMatchItem = null;
            return;
        }
        
        activeMatchItem = _matchItem;
        activeMatchItem.ChangeGridTile(this);
    }

    public void SpawnActiveMatchItem(MatchItem _matchItem, MatchItemTypes _matchItemType, float _spawnOffsetter)
    {
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
