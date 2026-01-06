using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


/*public class UpgradePanelManager : MonoBehaviour
{
    [Header("UI 元件")]
    public TextMeshProUGUI buildingNameText;
    public TextMeshProUGUI currentLevelText;
    public TextMeshProUGUI currentIncomeText;
    public TextMeshProUGUI nextLevelText;
    public TextMeshProUGUI nextIncomeText;
    public TextMeshProUGUI upgradeCostText;
    public Button upgradeButton;

    [Header("當前建築資訊重複顯示")]
    public TextMeshProUGUI currentIncomeTextDuplicate; // 第二個顯示每秒收益
    public TextMeshProUGUI currentLevelTextDuplicate;  // 第二個顯示等級

    //[Header("箭頭或指示符號")]
    //public GameObject arrowIndicator; // 可選，用於顯示 → 

    [HideInInspector]
    public Building ownerBuilding; // 建築物，由 BuildingPanelManager 指派

    // 刷新面板UI
    public void InitPanel()
    {
        if (ownerBuilding == null || ownerBuilding.data == null) return;

        BuildingData data = ownerBuilding.data;
        int currentLevel = ownerBuilding.currentLevel;

        // 建築名稱
        buildingNameText.text = data.buildingName;

        // 當前等級與收益 → 這裡加前墜文字
        LevelData currentLevelData = data.levels[currentLevel - 1];
        currentLevelText.text = $"Lv. {currentLevelData.level}";
        currentIncomeText.text = $"每秒收益: {currentLevelData.incomePerSecond}";

        if (currentLevelTextDuplicate != null)
            currentLevelTextDuplicate.text = $"Lv. {currentLevelData.level}";
        if (currentIncomeTextDuplicate != null)
            currentIncomeTextDuplicate.text = $"每秒收益: {currentLevelData.incomePerSecond}";

        // 下一級顯示與升級按鈕
        if (currentLevel < data.maxLevel)
        {
            LevelData nextLevelData = data.levels[currentLevel];
            nextLevelText.text = $"Lv. {nextLevelData.level}";
            nextIncomeText.text = $"每秒收益: {nextLevelData.incomePerSecond}";
            upgradeCostText.text = $"升級費用: {nextLevelData.upgradeCost}";

            upgradeButton.interactable = true; // 後續可加錢不足判斷

            
        }
        else
        {
            nextLevelText.text = "-";
            nextIncomeText.text = "-";
            upgradeCostText.text = "已達該建築最高等級";
            upgradeButton.interactable = false;

           
        }

        // 不論可不可升級，都統一設定顏色狀態
        ColorBlock cb = upgradeButton.colors;
        cb.disabledColor = Color.gray; // 或 Color.black
        upgradeButton.colors = cb;

        // 清掉舊按鈕事件，防止重複
        upgradeButton.onClick.RemoveAllListeners();
        upgradeButton.onClick.AddListener(OnUpgradeButtonClicked);

    }

    // 點擊升級按鈕
    public void OnUpgradeButtonClicked()
    {
        Debug.Log($"[UpgradeButton] ownerBuilding = {ownerBuilding}");

        if (ownerBuilding == null || ownerBuilding.currentLevel >= ownerBuilding.data.maxLevel)
            return;

        // 增加等級（暫時不扣錢，後續可加判斷）
        ownerBuilding.currentLevel++;

        // 刷新UI
        //Refresh();
        InitPanel();
    }
}*/

public class UpgradePanelManager : MonoBehaviour
{
    [Header("UI 元件")]
    public TextMeshProUGUI buildingNameText;
    public TextMeshProUGUI currentLevelText;
    public TextMeshProUGUI currentIncomeText;
    public TextMeshProUGUI nextLevelText;
    public TextMeshProUGUI nextIncomeText;
    public TextMeshProUGUI upgradeCostText;
    public Button upgradeButton;

    [Header("當前建築資訊重複顯示")]
    public TextMeshProUGUI currentIncomeTextDuplicate; // 第二個顯示每秒收益
    public TextMeshProUGUI currentLevelTextDuplicate;  // 第二個顯示等級

    [HideInInspector]
    public Building ownerBuilding; // 建築物，由 BuildingPanelManager 指派

    private CoinManager coinManager;

    private void OnEnable()
    {
        coinManager = FindObjectOfType<CoinManager>();
        if (coinManager != null)
            CoinManager.OnCoinChanged += RefreshUpgradeButtonState;
    }

    private void OnDisable()
    {
        if (coinManager != null)
            CoinManager.OnCoinChanged -= RefreshUpgradeButtonState;
    }

    // 刷新面板UI
    public void InitPanel()
    {
        if (ownerBuilding == null || ownerBuilding.data == null) return;

        BuildingData data = ownerBuilding.data;
        int currentLevel = ownerBuilding.currentLevel;

        // 建築名稱
        buildingNameText.text = data.buildingName;

        // 當前等級與收益 → 這裡加前墜文字
        LevelData currentLevelData = data.levels[currentLevel - 1];
        currentLevelText.text = $"Lv. {currentLevelData.level}";
        currentIncomeText.text = $"每秒收益: {currentLevelData.incomePerSecond}";

        if (currentLevelTextDuplicate != null)
            currentLevelTextDuplicate.text = $"Lv. {currentLevelData.level}";
        if (currentIncomeTextDuplicate != null)
            currentIncomeTextDuplicate.text = $"每秒收益: {currentLevelData.incomePerSecond}";

        // 下一級顯示
        if (currentLevel < data.maxLevel)
        {
            LevelData nextLevelData = data.levels[currentLevel];
            nextLevelText.text = $"Lv. {nextLevelData.level}";
            nextIncomeText.text = $"每秒收益: {nextLevelData.incomePerSecond}";
            upgradeCostText.text = $"升級費用: {nextLevelData.upgradeCost}";
        }
        else
        {
            nextLevelText.text = "-";
            nextIncomeText.text = "-";
            upgradeCostText.text = "已達該建築最高等級";
            upgradeButton.interactable = false;
        }

        // 清掉舊按鈕事件，防止重複
        upgradeButton.onClick.RemoveAllListeners();
        upgradeButton.onClick.AddListener(OnUpgradeButtonClicked);

        // 刷新按鈕狀態（可點與文字顏色）
        RefreshUpgradeButtonState();
    }

    // 刷新升級按鈕狀態
    private void RefreshUpgradeButtonState()
    {
        if (ownerBuilding == null || ownerBuilding.currentLevel >= ownerBuilding.data.maxLevel)
        {
            upgradeButton.interactable = false;
            return;
        }

        LevelData nextLevelData = ownerBuilding.data.levels[ownerBuilding.currentLevel];

        // 判斷玩家金錢是否足夠
        bool canAfford = coinManager != null && coinManager.HasEnough(nextLevelData.upgradeCost);

        // 設定按鈕可點狀態
        upgradeButton.interactable = canAfford;

        // 設定升級費用文字顏色
        upgradeCostText.color = canAfford ? Color.black : Color.red;
    }

    // 點擊升級按鈕
    public void OnUpgradeButtonClicked()
    {
        if (ownerBuilding == null || ownerBuilding.currentLevel >= ownerBuilding.data.maxLevel)
            return;

        LevelData nextLevelData = ownerBuilding.data.levels[ownerBuilding.currentLevel];

        // 檢查金錢足夠
        if (coinManager != null && !coinManager.HasEnough(nextLevelData.upgradeCost))
            return;

        // 扣錢
        coinManager?.DeductCoins(nextLevelData.upgradeCost);

        // 升級
        ownerBuilding.currentLevel++;

        // 刷新UI
        InitPanel();
    }
}
