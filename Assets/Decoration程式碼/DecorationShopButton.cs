using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DecorationShopButton : MonoBehaviour
{
    public DecorationSlot targetSlot;                // 對應的格子
    public DecorationShopManager shopManager;        // 指向 ShopManager

    // 點擊購買按鈕時呼叫
    public void OnBuyClicked()
    {
        if (shopManager != null && targetSlot != null)
        {
            // 將購買請求傳給 ShopManager，由它處理金錢判斷、扣錢、背包增加、面板顯示
            shopManager.TryBuyDecoration(targetSlot);
        }
        else
        {
            if (shopManager == null)
                Debug.LogError("DecorationShopManager 未指定！");
            if (targetSlot == null)
                Debug.LogError("targetSlot 未指定！");
        }
    }
}
