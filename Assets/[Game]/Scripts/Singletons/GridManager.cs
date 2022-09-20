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
    public List<GridTile> gridTiles;
    MatchItemTypesDataSO matchItemTypesData;

    void Start()
    {
        matchItemTypesData = DataManager.Instance.matchItemTypesData;

        SpawnGrid();
    }

    void SpawnGrid()
    {
        gridTiles = new List<GridTile>(gridData.rowSize * gridData.columnSize);
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

                Vector2 _spawnPoint = new Vector2(_columnSpawnOffsetX, _columnSpawnOffsetY);
                int _rndMatchItemTypeIndex = Random.Range(0, _activeMatchItemTypes.Count);
                _gridTile.InitGridTile(_spawnPoint, x, _activeMatchItemTypes[_rndMatchItemTypeIndex]);

                gridTiles.Add(_gridTile);

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
        for (int i = 0; i < gridTiles.Count; i++)
        {
            List<GridTile> _neighbourTiles = new List<GridTile>(4);

            int _leftNeighbourIndex = i - 1;
            int _rightNeighbourIndex = i + 1;
            int _upNeighbourIndex = i + gridData.columnSize;
            int _downNeighbourIndex = i - gridData.columnSize;

            int[] _neighbourIndexes = new int[4] { _leftNeighbourIndex, _rightNeighbourIndex, _upNeighbourIndex, _downNeighbourIndex };

            for (int j = 0; j < _neighbourIndexes.Length; j++)
            {
                if (_neighbourIndexes[j] >= 0 && _neighbourIndexes[j] < gridTiles.Count)
                {
                    _neighbourTiles.Add(gridTiles[_neighbourIndexes[j]]);
                }
            }

            gridTiles[i].SetNeighbours(_neighbourTiles);
        }
    }


} // class
