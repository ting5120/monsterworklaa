using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;



public class CostumeShopManager : MonoBehaviour
{
    public static CostumeShopManager Instance { get; private set; }

    [Header("面板")]
    public GameObject successPanel;
    public GameObject failPanel;
    public TMP_Text successText;
    public TMP_Text failText;
    public Button successCloseButton;
    public Button failCloseButton;

    [Header("金錢管理")]
    public CoinManager coinManager;

    [Header("服飾購買面板")]
    public GameObject costumePanel;
    public Button costumeCloseButton;

    [Header("購買按鈕列表")]
    public List<CostumePurchaseButton> purchaseButtons;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        Instance = this;
    }

    private void Start()
    {
        // 關閉購買面板
        if (costumeCloseButton != null && costumePanel != null)
            costumeCloseButton.onClick.AddListener(() => costumePanel.SetActive(false));

        if (successCloseButton != null)
            successCloseButton.onClick.AddListener(() => successPanel.SetActive(false));
        if (failCloseButton != null)
            failCloseButton.onClick.AddListener(() => failPanel.SetActive(false));

        // 初始化購買按鈕
        foreach (var button in purchaseButtons)
        {
            button?.InitializeButton();
        }
    }

    /// <summary>
    /// 嘗試購買服飾
    /// </summary>

    public void TryBuyCostume(CostumeData data)
    {
        if (data == null || coinManager == null) return;

        if (CostumeDataCenter.Instance.IsOwned(data)) return;

        if (coinManager.GetTotalCoins() < data.price)
        {
            failText.text = $"無法購買 {data.costumeName}，金額不足";
            failPanel.SetActive(true);
            return;
        }

        coinManager.DeductCoins(data.price);
        CostumeDataCenter.Instance.AddCostume(data);

        successText.text = $"購買成功：{data.costumeName}";
        successPanel.SetActive(true);

        // 更新購買按鈕
        var btn = purchaseButtons.FirstOrDefault(b => b.costumeData == data);
        if (btn != null)
            btn.purchaseButton.interactable = false;

        // 僅新增新服飾到所有已生成建築面板
        BuildingPanelManager.Instance?.AddNewCostumeToAllPanels(data);
    }

}
