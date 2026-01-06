using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class MonsterDismissButton : MonoBehaviour
{
    [Header("資遣按鈕")]
    public Button dismissButton;

    // 當前普通面板所對應的建築
    [HideInInspector] public Building ownerBuilding;

    private CoinManager coinManager;

    private void Awake()
    {
        coinManager = FindObjectOfType<CoinManager>();

        if (dismissButton != null)
        {
            dismissButton.onClick.AddListener(OnDismissButtonClicked);
        }
        else
        {
            Debug.LogWarning("[MonsterDismissButton] dismissButton 尚未設定");
        }
    }

    private void OnEnable()
    {
        CoinManager.OnCoinChanged += RefreshDismissButtonState;
        RefreshDismissButtonState(); // 保險，第一次也算
    }

    private void OnDisable()
    {
        CoinManager.OnCoinChanged -= RefreshDismissButtonState;
    }

    /// <summary>
    /// 刷新資遣按鈕狀態（是否可點）
    /// </summary>
    public void RefreshDismissButtonState()
    {
        if (dismissButton == null || coinManager == null)
            return;
        if (ownerBuilding == null || ownerBuilding.monsterInstance == null || coinManager == null)
            return;

        int dismissCost = CalculateDismissCost(ownerBuilding.monsterInstance);

        bool canAfford = coinManager.HasEnough(dismissCost);
        dismissButton.interactable = canAfford;
    }

    /// <summary>
    /// 點擊資遣按鈕
    /// </summary>
    private void OnDismissButtonClicked()
    {
        if (ownerBuilding == null || ownerBuilding.monsterInstance == null)
            return;

        int dismissCost = CalculateDismissCost(ownerBuilding.monsterInstance);

        // 保底檢查（理論上不會發生，因為按鈕已被鎖）
        if (!coinManager.HasEnough(dismissCost))
        {
            Debug.Log("[MonsterDismissButton] 金錢不足，無法資遣");
            return;
        }

        // 扣錢
        coinManager.DeductCoins(dismissCost);

        Debug.Log($"[MonsterDismissButton] 資遣建築 {ownerBuilding.data.buildingName}，花費 {dismissCost} 寶錢");

        // 刪除壞妖怪煙霧
        if (ownerBuilding.badMonsterSmokeInstance != null)
        {
            Destroy(ownerBuilding.badMonsterSmokeInstance);
            ownerBuilding.badMonsterSmokeInstance = null;
        }

        // 刪除怪物
        if (ownerBuilding.currentMonsterGO != null)
        {
            Destroy(ownerBuilding.currentMonsterGO);
            ownerBuilding.currentMonsterGO = null;
            ownerBuilding.monsterInstance = null;
        }

        ownerBuilding.recruitedMonster = null;

        // 關閉普通面板
        var panel = BuildingPanelManager.Instance?.GetNormalPanel(ownerBuilding);
        if (panel != null)
        {
            panel.SetActive(false);
        }

        // 進入招募流程
        RecruitManager.Instance?.ShowRecruitStartPanel(ownerBuilding);
    }

    /// <summary>
    /// 計算資遣費
    /// </summary>
    private int CalculateDismissCost(MonsterInstance monster)
    {
        float a = 1f;

        switch (monster.monsterData.level)
        {
            case MonsterLevel.Normal:
                a = 1.15f;
                break;
            case MonsterLevel.Rare:
                a = 1.35f;
                break;
            case MonsterLevel.Legendary:
                a = 1.65f;
                break;
        }

        float cost = 100f * a * 0.8f;
        return Mathf.CeilToInt(cost);
    }
}

