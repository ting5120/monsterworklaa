using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CostumePurchaseButton : MonoBehaviour
{
    [Header("服飾資料")]
    public CostumeData costumeData;

    [Header("UI")]
    public Button purchaseButton;

    [Header("購買管理器")]
    public CostumeShopManager shopManager;

    private void Start()
    {
        if (purchaseButton != null && shopManager != null)
            purchaseButton.onClick.AddListener(OnClickPurchase);

        InitializeButton();
    }

    public void InitializeButton()
    {
        if (costumeData == null || purchaseButton == null) return;

        purchaseButton.interactable = !CostumeOwnershipManager.Instance.IsOwned(costumeData.costumeID);
    }

    private void OnClickPurchase()
    {
        if (shopManager != null && costumeData != null)
            shopManager.TryBuyCostume(costumeData);
    }
}
