using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using CLUtils;
using System.Linq;
using Sirenix.OdinInspector;

public class GridManager : Operator
{
    [Header("General Variables")]
    int testInt2;

    [Header("References")]
    public GridTile gridTilePrefab;
    public Transform gridTileHolder;

    [Space(10)]
    [Header("! Debug !")]
    public GridTile[,] gridTiles;
    GridDataSO gridData;
    MatchItemTypesDataSO matchItemTypesData;
    List<MatchItemTypes> activeMatchItemTypes;
    Coroutine shuffleCheckerCor;

    #region Enable/Disable
    void OnEnable()
    {
        EventManager<object[]>.OnGridClicked += OnGridClicked;
    }

    void OnDisable()
    {
        EventManager<object[]>.OnGridClicked -= OnGridClicked;
    }
    #endregion

    void Start()
    {
        gridData = DataManager.Instance.gridData;
        matchItemTypesData = DataManager.Instance.matchItemTypesData;

        SpawnGrid();
        AttachNeighbours();

        HandleShuffle();
        HandleMatchItemGroupSprites();
    }

    void SpawnGrid()
    {
        gridTiles = new GridTile[gridData.columnCount, gridData.rowCount];

        float _gridHalfSize = gridData.gridTileSize * 0.5f;
        float _columnSpawnOffsetX = ((gridData.gridTileSize * -gridData.columnCount) * 0.5f) + _gridHalfSize;

        #region Handle Randomized Color List
        List<MatchItemTypes> _matchItemTypesCache = new List<MatchItemTypes>(matchItemTypesData.matchItemTypes.Length);
        _matchItemTypesCache.AddRange(matchItemTypesData.matchItemTypes);

        activeMatchItemTypes = new List<MatchItemTypes>(gridData.matchItemColorCount);

        for (int i = 0; i < gridData.matchItemColorCount; i++)
        {
            int _rndMatchItemTypeIndex = Random.Range(0, _matchItemTypesCache.Count);

            activeMatchItemTypes.Add(_matchItemTypesCache[_rndMatchItemTypeIndex]);

            _matchItemTypesCache.RemoveAt(_rndMatchItemTypeIndex);
        }
        #endregion

        for (int x = 0; x < gridData.columnCount; x++)
        {
            float _columnSpawnOffsetY = ((gridData.gridTileSize * -gridData.rowCount) * 0.5f) + _gridHalfSize;

            for (int y = 0; y < gridData.rowCount; y++)
            {
                GridTile _gridTile = Instantiate(gridTilePrefab, gridTileHolder);
                _gridTile.gameObject.name += $"{x}_{y}";

                Vector2 _spawnPoint = new Vector2(_columnSpawnOffsetX, _columnSpawnOffsetY);
                int _rndMatchItemTypeIndex = Random.Range(0, activeMatchItemTypes.Count);
                _gridTile.InitGridTile(_spawnPoint, new Vector2Int(x, y), activeMatchItemTypes[_rndMatchItemTypeIndex]);

                gridTiles[x, y] = _gridTile;

                _columnSpawnOffsetY += gridData.gridTileSize;
            }

            _columnSpawnOffsetX += gridData.gridTileSize;
        }
    }

    void AttachNeighbours()
    {
        for (int x = 0; x < gridData.columnCount; x++)
        {
            for (int y = 0; y < gridData.rowCount; y++)
            {
                List<GridTile> _neighbourTiles = new List<GridTile>(4);

                Vector2Int _leftNeighbourIndex = new Vector2Int(x - 1, y);
                Vector2Int _rightNeighbourIndex = new Vector2Int(x + 1, y);
                Vector2Int _upNeighbourIndex = new Vector2Int(x, y + 1);
                Vector2Int _downNeighbourIndex = new Vector2Int(x, y - 1);

                Vector2Int[] _neighbourIndexes = new Vector2Int[4] { _leftNeighbourIndex, _rightNeighbourIndex, _upNeighbourIndex, _downNeighbourIndex };

                for (int i = 0; i < _neighbourIndexes.Length; i++)
                {
                    if ((_neighbourIndexes[i].x >= 0f && _neighbourIndexes[i].x < gridData.columnCount) 
                        && (_neighbourIndexes[i].y >= 0f && _neighbourIndexes[i].y < gridData.rowCount))
                    {
                        _neighbourTiles.Add(gridTiles[_neighbourIndexes[i].x, _neighbourIndexes[i].y]);
                    }
                }

                gridTiles[x, y].SetNeighbours(_neighbourTiles);
            }
        }
    }

    void OnGridClicked(object[] a)
    {
        GridTile _clickedGridTile = (GridTile)a[0];

        if (!_clickedGridTile.CheckMatchable())
        {
            return;
        }

        HandleMatching(_clickedGridTile);
        FillEmptyGrids();
        HandleShuffle();
        HandleMatchItemGroupSprites();
    }

    void HandleMatching(GridTile _clickedGridTile)
    {
        List<GridTile> _matchingNeighbourGridTiles = new();

        FillMatchingDataFromNeighbours(ref _matchingNeighbourGridTiles, _clickedGridTile);

        for (int i = 0; i < _matchingNeighbourGridTiles.Count; i++)
        {
            _matchingNeighbourGridTiles[i].Matched(_clickedGridTile);
        }
    }

    void FillMatchingDataFromNeighbours(ref List<GridTile> _matchingNeighbourGridTiles, GridTile _gridTile)
    {
        if (!_matchingNeighbourGridTiles.Contains(_gridTile))
        {
            _matchingNeighbourGridTiles.Add(_gridTile);
        }

        for (int i = 0; i < _gridTile.neighbourTiles.Count; i++)
        {
            if (_matchingNeighbourGridTiles.Contains(_gridTile.neighbourTiles[i]))
            {
                continue;
            }

            if (!_gridTile.neighbourTiles[i].CompareMatchItemType(_gridTile.activeMatchItem.matchItemType))
            {
                continue;
            }

            _matchingNeighbourGridTiles.Add(_gridTile.neighbourTiles[i]);

            FillMatchingDataFromNeighbours(ref _matchingNeighbourGridTiles, _gridTile.neighbourTiles[i]);
        }
    }

    void FillEmptyGrids()
    {
        for (int x = 0; x < gridData.columnCount; x++)
        {
            for (int y = 0; y < gridData.rowCount; y++)
            {
                if (!gridTiles[x, y].activeMatchItem)
                {
                    GridShifting(new Vector2Int(x, y));
                    break;
                }
            }
        }
    }

    void GridShifting(Vector2Int _gridIndex)
    {
        List<GridTile> _columnGridTiles = new();
        List<GridTile> _emptyGridTiles = new();

        //Detecting shiftable grids
        for (int y = _gridIndex.y; y < gridData.rowCount; y++)
        {
            GridTile _gridTile = gridTiles[_gridIndex.x, y];

            if (!_gridTile.activeMatchItem)
            {
                _emptyGridTiles.Add(_gridTile);
            }

            _columnGridTiles.Add(_gridTile);
        }

        //Shifting grids
        for (int i = 0; i < _emptyGridTiles.Count; i++)
        {
            for (int k = 0; k < _columnGridTiles.Count - 1; k++)
            {
                GridTile _currGridTile = _columnGridTiles[k];
                GridTile _upperGridTile = _columnGridTiles[k + 1];
                MatchItem _currMatchItem = _currGridTile.activeMatchItem;
                MatchItem _upperMatchItem = _upperGridTile.activeMatchItem;
                
                // For prevent data loss on spiral matching
                if (!_currMatchItem)
                {
                    _upperGridTile.SetActiveMatchItem(null);
                    _currGridTile.SetActiveMatchItem(_upperMatchItem);
                }

                if (!_emptyGridTiles.Contains(_upperGridTile))
                {
                    _emptyGridTiles[i] = _upperGridTile;
                }
            }
        }

        //For ordered spawning of new match items
        _emptyGridTiles = _emptyGridTiles.OrderBy(x => x.gridIndex.y).ToList();

        //Filling empty grids
        for (int i = 0; i < _emptyGridTiles.Count; i++)
        {
            CreateOffGridMatchItem(_emptyGridTiles[i], i);
        }
    }

    MatchItem CreateOffGridMatchItem(GridTile _gridTile, float _spawnOffsetter)
    {
        int _rndMatchItemTypeIndex = Random.Range(0, activeMatchItemTypes.Count);

        MatchItem _matchItem = MatchItemPoolManager.Instance.FetchFromPool();
        _gridTile.SpawnActiveMatchItem(_matchItem, activeMatchItemTypes[_rndMatchItemTypeIndex], _spawnOffsetter);

        return _matchItem;
    }

    void HandleShuffle(float _shuffleCheckDelay = 2f)
    {
        if (shuffleCheckerCor != null)
        {
            StopCoroutine(shuffleCheckerCor);
            shuffleCheckerCor = null;
        }

        shuffleCheckerCor = StartCoroutine(Utils.DelayerCor(_shuffleCheckDelay, delegate
        {
            CheckForShuffle();
        }));
    }

    void CheckForShuffle()
    {
        List<GridTile> _matchingNeighbourGridTiles = new();
        List<MatchItem> _matchItems = new List<MatchItem>(gridData.columnCount * gridData.rowCount);

        int _counter = 0;
        for (int x = 0; x < gridData.columnCount; x++)
        {
            for (int y = 0; y < gridData.rowCount; y++)
            {
                _matchItems.Add(gridTiles[x, y].activeMatchItem);

                if (_matchingNeighbourGridTiles.Count > _counter)
                    continue;

                FillMatchingDataFromNeighbours(ref _matchingNeighbourGridTiles, gridTiles[x, y]);
                _counter++;
            }
        }

        // Check there is a matchable tile to click
        if (_matchingNeighbourGridTiles.Count < (gridData.columnCount * gridData.rowCount))
        {
            //Debug.Log("There is a matchable tile to click, continue without shuffle");
            _matchingNeighbourGridTiles.Clear();
            _matchItems.Clear();
            return;
        }

        ShuffleMatchItems(_matchItems);
    }

    void ShuffleMatchItems(List<MatchItem> _matchItems)
    {
        SpawnTextIndicator("Shuffling");

        System.Random _rnd = new System.Random();
        _matchItems = _matchItems.OrderBy(_item => _rnd.Next()).ToList();

        int _indexer = 0;
        for (int x = 0; x < gridData.columnCount; x++)
        {
            for (int y = 0; y < gridData.rowCount; y++)
            {
                gridTiles[x, y].SetActiveMatchItem(_matchItems[_indexer], false);
                _indexer++;
            }
        }

        //Check again for still no more moves situation
        HandleShuffle(1.5f);
    }

    void HandleMatchItemGroupSprites()
    {
        ////Use for delayed sprite change
        //if (matchItemGroupCheckerCor != null)
        //{
        //    StopCoroutine(matchItemGroupCheckerCor);
        //    matchItemGroupCheckerCor = null;
        //}

        //matchItemGroupCheckerCor = StartCoroutine(Utils.DelayerCor(1f, delegate
        //{

        //}));

        List<GridTile> _matchGroupGridTiles = new();

        for (int x = 0; x < gridData.columnCount; x++)
        {
            for (int y = 0; y < gridData.rowCount; y++)
            {
                FillMatchingDataFromNeighbours(ref _matchGroupGridTiles, gridTiles[x, y]);

                SetMatchItemGroups(_matchGroupGridTiles);
                _matchGroupGridTiles.Clear();
            }
        }
    }

    void SetMatchItemGroups(List<GridTile> _matchGroupGridTiles)
    {
        int _groupSize = _matchGroupGridTiles.Count;

        int _groupsCount = matchItemTypesData.matchItemGroupValues.Length;
        int _groupIndexer = 0;

        for (int i = 0; i < _groupsCount; i++)
        {
            if (_groupSize < matchItemTypesData.matchItemGroupValues[i])
            {
                _groupIndexer = i;
                break;
            }

            if (i == _groupsCount - 1)
            {
                _groupIndexer = _groupsCount;
                break;
            }
        }

        for (int i = 0; i < _groupSize; i++)
        {
            GridTile _gridTile = _matchGroupGridTiles[i];
            _gridTile.ChangeMatchItemSprite(matchItemTypesData.matchItemSprites[_gridTile.activeMatchItem.matchItemType][_groupIndexer]);
        }
    }

    void SpawnTextIndicator(string _message)
    {
        TextIndicator _textIndicator = TextIndicatorPoolManager.Instance.FetchFromPool();

        Vector3 _localPosition = gridTiles[0, gridData.rowCount - 1].transform.position + (Vector3.up * gridData.gridTileSize * 2f);
        _localPosition.x = 0f;

        _textIndicator.SpawnIndicator(_message, transform, _localPosition, TextIndicatorTypes.Normal);
    }

    void SpawnTextIndicator(string _message, GridTile _gridTile)
    {
        TextIndicator _textIndicator = TextIndicatorPoolManager.Instance.FetchFromPool();

        Vector3 _localPosition = _gridTile.transform.position + (Vector3.up * gridData.gridTileSize * 0.5f);
        _localPosition.x = 0f;

        _textIndicator.SpawnIndicator(_message, transform, _localPosition, TextIndicatorTypes.Normal);
    }

    [Button]
    public void DebugShuffling() // For debug purposes
    {
        List<MatchItem> _matchItems = new List<MatchItem>(gridData.columnCount * gridData.rowCount);

        for (int x = 0; x < gridData.columnCount; x++)
        {
            for (int y = 0; y < gridData.rowCount; y++)
            {
                _matchItems.Add(gridTiles[x, y].activeMatchItem);
            }
        }

        ShuffleMatchItems(_matchItems);
    }

} // class
