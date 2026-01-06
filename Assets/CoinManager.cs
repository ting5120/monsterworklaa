using System.Collections;
using System.Collections.Generic;


    using TMPro; //Unity有改版所以需要
    using Unity.VisualScripting;
    using UnityEngine;
    using UnityEngine.UI;



public class CoinManager : MonoBehaviour
{
    [Header(" TextMeshPro UI 元件")]
    public TextMeshProUGUI coinTextMoneyUI;   // 顯示玩家總寶錢

    [Header(" 金錢數值設定")]
    private float uncollectedCoins = 0f;     // 掛機累積中的寶錢（可小數）
    private int totalCoins = 0;              // 玩家已收集的總寶錢
    public int coinsPerSecond = 20;           // 每秒掛機產生的寶錢

    // 提供其他系統讀取總金額(任務系統用)
    public int TotalCoins => totalCoins;

    public static System.Action OnCoinChanged;

    private void Start()
    {
        totalCoins = 300; // 初始給玩家 300 金錢
        UpdateUI();
    }

    private void Update()
    {
        // 每秒累積掛機金錢
        uncollectedCoins += coinsPerSecond * Time.deltaTime;

        // 將累積掛機金錢加入總金錢（取整數部分）
        int coinsToAdd = Mathf.FloorToInt(uncollectedCoins);
        if (coinsToAdd > 0)
        {
            totalCoins += coinsToAdd;
            uncollectedCoins -= coinsToAdd;
            UpdateUI();
        }
    }

    private void UpdateUI()
    {
        // 顯示總寶錢（整數）
        coinTextMoneyUI.text = $"寶錢：{totalCoins:N0}";

        OnCoinChanged?.Invoke();

        // 取得 BuildingPanelManager 並刷新解鎖狀態
        BuildingPanelManager panelManager = FindObjectOfType<BuildingPanelManager>();
        if (panelManager != null)
        {
            panelManager.RefreshAllTiles(TotalCoins);
        }
    }

    // 扣除金額（給建築物或服飾系統用）
    public void DeductCoins(int amount)
    {
        if (amount <= 0) return;

        if (totalCoins >= amount)
        {
            totalCoins -= amount;
            UpdateUI();
            Debug.Log($"已扣除 {amount} 寶錢，目前總寶錢：{totalCoins}");
        }
        else
        {
            Debug.Log("[CoinManager] 金額不足，無法扣除");
        }
    }

    // 檢查玩家是否有足夠金錢
    public bool HasEnough(int amount)
    {
        return totalCoins >= amount;
    }

    // 可用於增加掛機金錢（外部系統）
    public void AddUncollectedCoins(float amount)
    {
        uncollectedCoins += amount;
    }

    // 可讀掛機金錢（外部系統）
    public float GetUncollectedCoins()
    {
        return uncollectedCoins;
    }

    public int GetTotalCoins()
    {
        return totalCoins;
    }

}
