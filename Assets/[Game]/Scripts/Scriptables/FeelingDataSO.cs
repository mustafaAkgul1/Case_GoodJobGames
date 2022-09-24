using DG.Tweening;
using UnityEngine;

[CreateAssetMenu(fileName = "FeelingData", menuName = "Datas/Create Feeling Data")]
public class FeelingDataSO : ScriptableObject
{
    [Header("General Variables")]
    [Range(0, 10)] public int offGridMatchItemSpawnOffset = 3;

    [Header("Grid Falling Variables")]
    [Range(1f, 50f)] public float offGridMatchItemFallSpeed = 15f;
    [Range(1f, 50f)] public float onGridMatchItemFallSpeed = 15f;
    public AnimationCurve offGridMatchItemFallEase;
    public AnimationCurve onGridMatchItemFallEase;

    [Header("Grid Bubble Variables")]
    [Range(0.1f, 2f)] public float gridBubbleRotationDuration = 0.4f;
    [Range(0.1f, 2f)] public float gridBubbleScaleDuration = 0.4f;
    [Range(0.1f, 90f)] public float gridBubbleRotationFactor = 12f;
    [Range(-1f, -0.1f)] public float gridBubbleScaleFactor = -0.15f;
}
