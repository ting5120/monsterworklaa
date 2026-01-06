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

    private void Awake()
    {
        if (dismissButton != null)
        {
            dismissButton.onClick.AddListener(OnDismissButtonClicked);
        }
        else
        {
            Debug.LogWarning("[MonsterDismissButton] dismissButton 尚未設定");
        }
    }

    /// <summary>
    /// 點擊資遣按鈕
    /// </summary>
    private void OnDismissButtonClicked()
    {
        if (ownerBuilding == null)
        {
            Debug.LogWarning("[MonsterDismissButton] ownerBuilding 尚未指定");
            return;
        }

        Debug.Log($"[MonsterDismissButton] 資遣建築 {ownerBuilding.data.buildingName} 的妖怪");

        // 刪除目前的怪物 prefab
        if (ownerBuilding.currentMonsterGO != null)
        {
            Destroy(ownerBuilding.currentMonsterGO);
            ownerBuilding.currentMonsterGO = null;
            ownerBuilding.monsterInstance = null;
        }

        // 將 recruitedMonster 設為 null，等待重新招募
        ownerBuilding.recruitedMonster = null;

        // 暫時關閉普通面板，但不重置 UI 或其他進度
        var panel = BuildingPanelManager.Instance?.GetNormalPanel(ownerBuilding);
        if (panel != null)
        {
            panel.SetActive(false);
        }

        // 呼叫 RecruitManager 顯示招募流程
        if (RecruitManager.Instance != null)
        {
            RecruitManager.Instance.ShowRecruitStartPanel(ownerBuilding.data);
        }
        else
        {
            Debug.LogError("[MonsterDismissButton] RecruitManager.Instance 尚未生成");
        }

    }
}
