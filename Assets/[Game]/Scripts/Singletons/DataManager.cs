using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CLUtils;

public class DataManager : SingletonOperator<DataManager>
{
    [Header("References")]
    public MatchItemTypesDataSO matchItemTypesData;
    public GridDataSO gridData;
    public FeelingDataSO feelingData;

} // class
