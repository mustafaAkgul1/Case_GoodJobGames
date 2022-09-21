using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using CLUtils;

public class GridManager : Operator
{
    [Header("General Variables")]
    int testInt2;

    [Header("References")]
    public GridDataSO gridData;
    public GridTile gridTilePrefab;
    public Transform gridTileHolder;

    [Space(10)]
    [Header("! Debug !")]
    public GridTile[,] gridTiles;
    MatchItemTypesDataSO matchItemTypesData;

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
        matchItemTypesData = DataManager.Instance.matchItemTypesData;

        SpawnGrid();
    }

    void SpawnGrid()
    {
        gridTiles = new GridTile[gridData.rowSize, gridData.columnSize];

        float _gridHalfSize = gridData.gridSize / 2f;

        float _columnSpawnOffsetY = ((gridData.gridSize * gridData.rowSize) / 2f) - _gridHalfSize;

        #region Handle Randomized Color List
        List<MatchItemTypes> _matchItemTypesCache = new List<MatchItemTypes>(matchItemTypesData.matchItemTypes.Length);
        _matchItemTypesCache.AddRange(matchItemTypesData.matchItemTypes);

        List<MatchItemTypes> _activeMatchItemTypes = new List<MatchItemTypes>(gridData.matchItemColorCount);

        for (int i = 0; i < gridData.matchItemColorCount; i++)
        {
            int _rndMatchItemTypeIndex = Random.Range(0, _matchItemTypesCache.Count);

            _activeMatchItemTypes.Add(_matchItemTypesCache[_rndMatchItemTypeIndex]);

            _matchItemTypesCache.RemoveAt(_rndMatchItemTypeIndex);
        }
        #endregion

        for (int x = 0; x < gridData.rowSize; x++)
        {
            float _columnSpawnOffsetX = ((gridData.gridSize * -gridData.columnSize) / 2f) + _gridHalfSize;

            for (int y = 0; y < gridData.columnSize; y++)
            {
                GridTile _gridTile = Instantiate(gridTilePrefab, gridTileHolder);

                _gridTile.gameObject.name += $"{x}_{y}";

                Vector2 _spawnPoint = new Vector2(_columnSpawnOffsetX, _columnSpawnOffsetY);
                int _rndMatchItemTypeIndex = Random.Range(0, _activeMatchItemTypes.Count);
                _gridTile.InitGridTile(_spawnPoint, x, _activeMatchItemTypes[_rndMatchItemTypeIndex]);

                gridTiles[x, y] = _gridTile;

                _columnSpawnOffsetX += gridData.gridSize;
            }

            _columnSpawnOffsetY -= gridData.gridSize;
        }

        _matchItemTypesCache.Clear();
        _activeMatchItemTypes.Clear();

        HandleNeighboursAttachment();
    }

    void HandleNeighboursAttachment()
    {
        for (int x = 0; x < gridData.rowSize; x++)
        {
            for (int y = 0; y < gridData.columnSize; y++)
            {
                List<GridTile> _neighbourTiles = new List<GridTile>(4);

                Vector2Int _leftNeighbourIndex = new Vector2Int(x - 1, y);
                Vector2Int _rightNeighbourIndex = new Vector2Int(x + 1, y);
                Vector2Int _upNeighbourIndex = new Vector2Int(x, y + 1);
                Vector2Int _downNeighbourIndex = new Vector2Int(x, y - 1);

                Vector2Int[] _neighbourIndexes = new Vector2Int[4] { _leftNeighbourIndex, _rightNeighbourIndex, _upNeighbourIndex, _downNeighbourIndex };

                for (int i = 0; i < _neighbourIndexes.Length; i++)
                {
                    if ((_neighbourIndexes[i].x >= 0f && _neighbourIndexes[i].x < gridData.rowSize) 
                        && (_neighbourIndexes[i].y >= 0f && _neighbourIndexes[i].y < gridData.columnSize))
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


} // class
