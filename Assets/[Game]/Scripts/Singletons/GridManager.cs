using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using CLUtils;
using System.Linq;

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
    List<MatchItemTypes> _activeMatchItemTypes;
    Coroutine emptyGridFinderCor;
    
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
    }

    void SpawnGrid()
    {
        gridTiles = new GridTile[gridData.columnCount, gridData.rowCount];

        float _gridHalfSize = gridData.gridTileSize * 0.5f;

        float _columnSpawnOffsetX = ((gridData.gridTileSize * -gridData.columnCount) * 0.5f) + _gridHalfSize;

        #region Handle Randomized Color List
        List<MatchItemTypes> _matchItemTypesCache = new List<MatchItemTypes>(matchItemTypesData.matchItemTypes.Length);
        _matchItemTypesCache.AddRange(matchItemTypesData.matchItemTypes);

        _activeMatchItemTypes = new List<MatchItemTypes>(gridData.matchItemColorCount);

        for (int i = 0; i < gridData.matchItemColorCount; i++)
        {
            int _rndMatchItemTypeIndex = Random.Range(0, _matchItemTypesCache.Count);

            _activeMatchItemTypes.Add(_matchItemTypesCache[_rndMatchItemTypeIndex]);

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
                int _rndMatchItemTypeIndex = Random.Range(0, _activeMatchItemTypes.Count);
                _gridTile.InitGridTile(_spawnPoint, new Vector2Int(x, y), _activeMatchItemTypes[_rndMatchItemTypeIndex]);

                gridTiles[x, y] = _gridTile;

                _columnSpawnOffsetY += gridData.gridTileSize;
            }

            _columnSpawnOffsetX += gridData.gridTileSize;
        }

        AttachNeighbours();
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

        List<GridTile> _matchingNeighbourGridTiles = new();

        if (!CheckMatchable(_clickedGridTile))
        {
            return;
        }

        CheckNeighbours(ref _matchingNeighbourGridTiles, _clickedGridTile);

        for (int i = 0; i < _matchingNeighbourGridTiles.Count; i++)
        {
            _matchingNeighbourGridTiles[i].Matched(_clickedGridTile);
        }

        //HandleFillingEmptyGrids(_matchingNeighbourGridTiles);
        FillEmptyGrids();
    }

    bool CheckMatchable(GridTile _gridTile)
    {
        bool _isMatching = false;

        for (int i = 0; i < _gridTile.neighbourTiles.Count; i++)
        {
            if (_gridTile.neighbourTiles[i].CheckNeighbourTypeMatching(_gridTile.activeMatchItem.matchItemType))
            {
                _isMatching = true;
                break;
            }
        }

        return _isMatching;
    }

    void CheckNeighbours(ref List<GridTile> _matchingNeighbourGridTiles, GridTile _gridTile)
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

            if (!_gridTile.neighbourTiles[i].CheckNeighbourTypeMatching(_gridTile.activeMatchItem.matchItemType))
            {
                continue;
            }

            _matchingNeighbourGridTiles.Add(_gridTile.neighbourTiles[i]);

            CheckNeighbours(ref _matchingNeighbourGridTiles, _gridTile.neighbourTiles[i]);
        }
    }

    //void HandleFillingEmptyGrids(List<GridTile> _matchedGridTiles)
    //{
    //    _matchedGridTiles = _matchedGridTiles.OrderByDescending(x => x.gridIndex.y).ToList();

    //    List<MatchItem> _cachedMatchItems = new List<MatchItem>();

    //    foreach (GridTile _gridTile in _matchedGridTiles)
    //    {
    //        int _coordRowCounter = 1;

    //        FetchUpperFilledGridTile(_gridTile, ref _cachedMatchItems, _coordRowCounter);
    //    }
    //}

    //void FetchUpperFilledGridTile(GridTile _gridTile, ref List<MatchItem> _cachedMatchItems, int _coordRowCounter)
    //{
    //    bool _hasMatchItem = false;

    //    while (_gridTile.gridIndex.y - _coordRowCounter >= 0)
    //    {
    //        GridTile _upperGridTile = gridTiles[_gridTile.gridIndex.x, _gridTile.gridIndex.y - _coordRowCounter];
    //        MatchItem _matchItem = _upperGridTile.activeMatchItem;

    //        if (!_matchItem)
    //        {
    //            _coordRowCounter++;
    //            continue;
    //        }

    //        if (_cachedMatchItems.Contains(_matchItem))
    //        {
    //            _coordRowCounter++;
    //            continue;
    //        }

    //        _hasMatchItem = true;
            
    //        _matchItem.ChangeGridTile(_gridTile);
    //        _gridTile.activeMatchItem = _matchItem;
    //        _upperGridTile.activeMatchItem = null;

    //        _cachedMatchItems.Add(_matchItem);

    //        FetchUpperFilledGridTile(_upperGridTile, ref _cachedMatchItems, 1);

    //        break;
    //    }

    //    if (_hasMatchItem)
    //    {
    //        return;
    //    }

    //    //Top grids, creates new match items
    //    int _rndMatchItemTypeIndex1 = Random.Range(0, _activeMatchItemTypes.Count);

    //    MatchItem _matchItem1 = CreateOffGridMatchItem(_gridTile);

    //    _cachedMatchItems.Add(_matchItem1);
    //}

    void FillEmptyGrids()
    {
        if (emptyGridFinderCor != null)
        {
            StopCoroutine(emptyGridFinderCor);
            emptyGridFinderCor = null;
        }

        emptyGridFinderCor = StartCoroutine(FindEmptyGrids());
    }

    IEnumerator FindEmptyGrids()
    {
        for (int x = 0; x < gridData.columnCount; x++)
        {
            for (int y = 0; y < gridData.rowCount; y++)
            {
                if (!gridTiles[x, y].activeMatchItem)
                {
                    yield return StartCoroutine(GridShifting(new Vector2Int(x,y)));
                    break;
                }
            }
        }
    }

    IEnumerator GridShifting(Vector2Int _gridIndex)
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
            yield return new WaitForSeconds(0f);

            for (int k = 0; k < _columnGridTiles.Count - 1; k++)
            {
                MatchItem _shiftedMatchItem = _columnGridTiles[k + 1].activeMatchItem;

                _shiftedMatchItem?.ChangeGridTile(_columnGridTiles[k]);
                _columnGridTiles[k].activeMatchItem = _shiftedMatchItem;
                _columnGridTiles[k + 1].activeMatchItem = null;

                if (!_emptyGridTiles.Contains(_columnGridTiles[k + 1]))
                {
                    _emptyGridTiles[i] = _columnGridTiles[k + 1];
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
        int _rndMatchItemTypeIndex = Random.Range(0, _activeMatchItemTypes.Count);

        MatchItem _matchItem = MatchItemPoolManager.Instance.FetchFromPool();
        _matchItem.SpawnOffGridTile(_gridTile, _activeMatchItemTypes[_rndMatchItemTypeIndex], _spawnOffsetter);
        _gridTile.activeMatchItem = _matchItem;

        return _matchItem;
    }


} // class
