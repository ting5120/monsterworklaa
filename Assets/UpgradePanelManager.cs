using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


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
}
