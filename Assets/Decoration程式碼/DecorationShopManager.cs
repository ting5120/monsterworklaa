using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;

public class DecorationShopManager : MonoBehaviour
{
    [Header("面板")]
    public GameObject successPanel;
    public GameObject failPanel;

    [Header("成功面板 UI")]
    public TMP_Text successText;
    public Button successCloseButton;

    [Header("失敗面板 UI")]
    public TMP_Text failText;
    public Button failCloseButton;

    [Header("景觀購買面板")]
    public GameObject landscapePanel;
    public Button landscapeCloseButton;

    [Header("金錢管理")]
    public CoinManager coinManager;

    void Start()
    {
        // 綁定關閉按鈕事件
        if (successCloseButton != null)
            successCloseButton.onClick.AddListener(() => successPanel.SetActive(false));

        if (failCloseButton != null)
            failCloseButton.onClick.AddListener(() => failPanel.SetActive(false));

        // 景觀面板關閉
        if (landscapeCloseButton != null && landscapePanel != null)
        {
            landscapeCloseButton.onClick.AddListener(() =>
            {
                landscapePanel.SetActive(false);
            });
        }
    }

    /// <summary>
    /// 嘗試購買裝飾物
    /// </summary>
    /// <param name="slot">對應的裝飾物格子</param>
    public void TryBuyDecoration(DecorationSlot slot)
    {
        if (slot == null || coinManager == null)
        {
            Debug.LogError("DecorationSlot 或 CoinManager 為 null");
            return;
        }

        int price = slot.decoration.price;

        if (coinManager.GetTotalCoins() >= price)
        {
            // 金錢足夠 → 顯示成功面板
            if (successPanel != null && successText != null)
            {
                successText.text = $"購買成功：{slot.decoration.name}";
                successPanel.SetActive(true);
            }

            // 扣錢並增加背包
            coinManager.DeductCoins(price);
            slot.AddOne();
        }
        else
        {
            // 金錢不足 → 顯示失敗面板
            if (failPanel != null && failText != null)
            {
                failText.text = $"無法購買 {slot.decoration.name}，金額不足";
                failPanel.SetActive(true);
            }
        }
    }

}
