using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingIncomeCalculator : MonoBehaviour
{
    [Header("計算間隔（秒）")]
    public float calculateInterval = 5f;

    private float timer = 0f;

    private CoinManager coinManager;

    private void Start()
    {
        coinManager = FindObjectOfType<CoinManager>();
        if (coinManager == null)
        {
            Debug.LogError("找不到 CoinManager");
        }
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= calculateInterval)
        {
            timer = 0f;
            CalculateAllBuildingsIncome();
        }
    }

    private void CalculateAllBuildingsIncome()
    {
        // 找所有建築物
        Building[] buildings = FindObjectsOfType<Building>();
        float totalIncome = 0f;

        foreach (var building in buildings)
        {
            if (building.recruitedMonster == null) continue;

            // 公式 = 基礎收益 * 等級加成 * 好壞加成
            float baseIncome = building.data.baseCoinPerSecond * calculateInterval;
            float income = baseIncome * building.recruitedMonster.GetLevelMultiplier()
                                         * building.recruitedMonster.GetAlignmentMultiplier();

            Debug.Log($"建築：{building.data.buildingName}, 基礎收益：{baseIncome}, 等級加成：{building.recruitedMonster.GetLevelMultiplier()}, 好壞加成：{building.recruitedMonster.GetAlignmentMultiplier()}, 計算後收益：{income}");

            totalIncome += income;
        }

        if (coinManager != null)
        {
            coinManager.AddUncollectedCoins(totalIncome);
            Debug.Log($"本次累積收益：{totalIncome}, 玩家未收集金幣：{coinManager.GetUncollectedCoins()}");

        }
    }
}

