using UnityEngine;

[CreateAssetMenu(fileName = "GridData", menuName = "Datas/New Grid Data")]
public class GridDataSO : ScriptableObject
{
    [Header("General Variables")]
    [Range(1, 6)] public int matchItemColorCount = 6;
    [Range(2, 10)] public int columnCount = 10;
    [Range(2, 10)] public int rowCount = 10;
    [Range(0.25f, 5f)] public float gridTileSize = 2.25f;

    [Header("Gameplay Variables")]
    [Range(0.5f, 5f)] public float shuffleGeneralDelay = 2f;
    [Range(0.5f, 5f)] public float shuffleInnerDelay = 1.5f;
}