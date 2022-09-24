using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MatchingData", menuName = "Datas/Create Matching Data")]
public class MatchingDataSO : ScriptableObject
{
    [Header("General Variables")]
    [Range(0, 10)] public int offGridMatchItemSpawnOffset = 3;
    [Range(1f, 50f)] public float offGridMatchItemFallSpeed = 15f;
    [Range(1f, 50f)] public float onGridMatchItemFallSpeed = 15f;
    public Ease offGridMatchItemFallEase;
    public Ease onGridMatchItemFallEase;

}
