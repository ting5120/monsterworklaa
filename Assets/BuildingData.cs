using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BuildingData", menuName = "Building/BuildingData")]
public class BuildingData : ScriptableObject
{
    public string buildingName;
    public Sprite icon;
   
    public string production;
    public string streetLimit;
    public string InteractLimit;
    public int price;        
    public bool unlocked;

    public GameObject placedBuildingPrefab;
    public PanelType panelType;

    [Header("收益設定")]
    public float baseCoinPerSecond = 10f; // 每秒基礎收益，可在每個建築物上自訂

    [HideInInspector] 
    public GameObject placedInstance; // 建築物實體

    [Header("升級設定")]
    public int maxLevel = 3; // 建築最高等級

    // 每一級的數值
    public LevelData[] levels;
}
// 每級建築的資料
[System.Serializable]
public class LevelData
{
    public int level;              // 等級 (1,2,3)
    public float incomePerSecond;  // 這級的每秒產值（可疊加 baseCoinPerSecond）
    public int upgradeCost;        // 升級到下一級需要多少錢
}