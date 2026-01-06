using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;



public class CostumePanelManager : MonoBehaviour
{
    [Header("按鈕")]
    public CostumeButton cancelButton;
    public List<CostumeButton> costumeButtons = new List<CostumeButton>();

    [HideInInspector] public Building ownerBuilding;

    private void Awake()
    {
        // 自動抓 cancelButton（假設它有特定 tag 或名稱）
        if (cancelButton == null)
        {
            cancelButton = GetComponentsInChildren<CostumeButton>(true)
                .FirstOrDefault(b => b.gameObject.name.ToLower().Contains("cancel"));
        }

        // 自動抓其他 costumeButtons
        costumeButtons = GetComponentsInChildren<CostumeButton>(true)
            .Where(b => b != cancelButton).ToList();

        // 每個按鈕指定 panelManager
        foreach (var btn in costumeButtons)
        {
            btn.panelManager = this;
            btn.gameObject.SetActive(false);
            btn.SetSelected(false);

            // 指向建築物，用來換裝
            btn.targetBuilding = ownerBuilding;
        }

        if (cancelButton != null)
        {
            cancelButton.panelManager = this;
            cancelButton.SetSelected(false);
            
        }
    }

    /// <summary>
    /// 初始化面板，隱藏所有 costume 按鈕
    /// </summary>
    public void InitializePanel()
    {
        foreach (var btn in costumeButtons)
        {
            btn.gameObject.SetActive(false);
            btn.SetSelected(false);
        }

        if (cancelButton != null)
            cancelButton.SetSelected(true);
    }

    /// <summary>
    /// 新增已購服飾到面板
    /// </summary>
    public void AddPurchasedCostume(CostumeData data)
    {
        if (data == null)
        {
            Debug.LogWarning("[AddPurchasedCostume] data 為 null");
            return;
        }

        // 找到第一個空按鈕
        var slot = costumeButtons.FirstOrDefault(b => b.currentCostume == null);
        if (slot == null)
        {
            Debug.LogWarning($"[AddPurchasedCostume] 沒有空按鈕可以放服飾 {data.costumeName}");
            return;
        }

        slot.SetCostume(data);
        slot.targetBuilding = ownerBuilding; // ← 指向建築物換整隻 prefab
        slot.gameObject.SetActive(true);
        slot.transform.SetAsLastSibling();
        slot.SetSelected(false);
    }

    /// <summary>
    /// 點擊服飾按鈕
    /// </summary>
    public void OnClickCostumeButton(CostumeButton btn)
    {
        if (btn == null || ownerBuilding == null) return;

        // 取消所有按鈕選中
        foreach (var b in costumeButtons)
            b.SetSelected(false);

        if (cancelButton != null)
            cancelButton.SetSelected(false);

        // 選中這個按鈕
        btn.SetSelected(true);

        ///// 如果是取消鍵
        if (btn.currentCostume == null || btn.currentCostume.costumeID == 0)
        {
            ownerBuilding.equippedCostume = null;
            // 換回預設 prefab（costumeId = 0）
            ownerBuilding.EquipCostume(0);
        }
        else
        {
            ownerBuilding.equippedCostume = btn.currentCostume;
            ownerBuilding.EquipCostume(btn.currentCostume.costumeID);
        }

        //ownerBuilding.equippedCostume = btn.currentCostume;

        // 直接換整隻 prefab
        //ownerBuilding.EquipCostume(btn.currentCostume.costumeID);
    }

    /// <summary>
    /// 刷新面板，顯示當前裝備
    /// </summary>
    public void RefreshPanel()
    {
        if (ownerBuilding == null) return;

        foreach (var btn in costumeButtons)
            btn.SetSelected(btn.currentCostume == ownerBuilding.equippedCostume);

        if (ownerBuilding.equippedCostume == null && cancelButton != null)
            cancelButton.SetSelected(true);
        else if (cancelButton != null)
            cancelButton.SetSelected(false);
    }

    /// <summary>
    /// 從 CostumeDataCenter 刷新面板
    /// </summary>
    public void RefreshFromDataCenter()
    {
        foreach (var data in CostumeDataCenter.Instance.ownedCostumes)
            AddPurchasedCostume(data);

        // 保留原本裝備選中
        RefreshPanel();
    }
}

