using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "BuildingList", menuName = "Game Data/Building List")]
public class BuildingList : ScriptableObject
{
    // 所有建築物數據的列表
    public List<BuildingData> allBuildings = new List<BuildingData>();
}
