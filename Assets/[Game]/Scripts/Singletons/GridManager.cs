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
    MatchItemTypesDataSO matchItemTypesData;

    void Start()
    {
        matchItemTypesData = DataManager.Instance.matchItemTypesData;

        SpawnGrid();
    }

    void SpawnGrid()
    {
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
                Vector2 _spawnPoint = new Vector2(_columnSpawnOffsetX, _columnSpawnOffsetY);

                GridTile _gridTile = Instantiate(gridTilePrefab, gridTileHolder);

                int _rndMatchItemTypeIndex = Random.Range(0, _activeMatchItemTypes.Count);
                _gridTile.InitGridTile(_spawnPoint, x, _activeMatchItemTypes[_rndMatchItemTypeIndex]);

                _columnSpawnOffsetX += gridData.gridSize;
            }

            _columnSpawnOffsetY -= gridData.gridSize;
        }

        _matchItemTypesCache.Clear();
        _activeMatchItemTypes.Clear();
    }


} // class
