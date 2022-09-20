using UnityEngine;

[CreateAssetMenu(fileName = "GridData", menuName = "Datas/New Grid Data")]
public class GridDataSO : ScriptableObject
{
    [Header("General Variables")]
    [Range(2, 10)] public int columnSize = 10;
    [Range(2, 10)] public int rowSize = 10;
    [Range(0.25f, 5f)] public float gridSize = 2.25f;
    [Range(1, 6)] public int matchItemColorCount = 6;
}