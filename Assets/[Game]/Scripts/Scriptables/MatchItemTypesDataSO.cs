using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "MatchItemTypesData", menuName = "Datas/Create Match Item Types Data")]
public class MatchItemTypesDataSO : SerializedScriptableObject
{
    [Header("General Variables")]
    public MatchItemTypes[] matchItemTypes;
    public int[] matchItemGroupValues;
    public Dictionary<MatchItemTypes, List<Sprite>> matchItemSprites = new();
}