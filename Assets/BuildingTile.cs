//using DummyNamespace;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;




/*public class BuildingTile : MonoBehaviour
{
    public Image iconImage;
    public TMP_Text nameText;
    public TMP_Text productionText;
    public TMP_Text streetText;
    public TMP_Text priceText;
    public TMP_Text interactText;
    public GameObject lockOverlay;

    private BuildingData data;
    private bool permanentlyUnlocked = false;


    public void Initialize(BuildingData buildingData)
    {
        data = buildingData;
        permanentlyUnlocked = false; // 重置永久解鎖

        iconImage.sprite = data.icon;
        nameText.text = data.buildingName;
        productionText.text = data.production;
        streetText.text = data.streetLimit;
        priceText.text = "$" + data.price;

        lockOverlay.SetActive(!data.unlocked);

        // 初始 lockOverlay 與互動文字
        lockOverlay.SetActive(true);
        interactText.text = "鎖定中";

        

        Button btn = GetComponent<Button>();

        if (btn != null)
        {
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() =>
            {
                if (!permanentlyUnlocked) return; // 用 Tile 自己的解鎖狀態判斷

                //if (!data.unlocked) return;

                // 找到 CoinManager
                CoinManager coinManager = FindObjectOfType<CoinManager>();
                if (coinManager != null)
                {
                    // 檢查玩家是否有足夠金錢
                    if (coinManager.GetTotalCoins() >= data.price)
                    {
                        // 扣除玩家金錢
                        coinManager.DeductCoins(data.price);

                        // 設為解鎖（如果之前還未解鎖）
                        data.unlocked = true;

                        // 更新 lockOverlay 與互動文字
                        lockOverlay.SetActive(!data.unlocked);
                        interactText.text = data.unlocked ? data.InteractLimit : "鎖定中";

                        Debug.Log($"已購買建築：{data.buildingName}，扣除 {data.price} 寶錢");

                        // 通知 BuildingManager 進入放置模式
                        BuildingManager.Instance.StartPlacementMode(data);

                        // 如果之前有選中的格子，立即更新該格子的 Sprite 並放置建築
                        if (SlotHandler.SelectedSlot != null)
                        {
                            var slot = SlotHandler.SelectedSlot;
                            if (slot.gridManager != null)
                            {
                                var gridSlot = slot.gridManager.GridSlots[slot.rowIndex, slot.buildingIndex];
                                if (gridSlot != null && gridSlot.EmptySlotWorld != null)
                                {
                                    SpriteRenderer sr = gridSlot.EmptySlotWorld.GetComponent<SpriteRenderer>();
                                    if (sr != null)
                                    {
                                        sr.sprite = data.icon;
                                        sr.enabled = true;
                                    }
                                    BuildingManager.Instance.TryPlaceBuilding(slot.rowIndex, slot.buildingIndex);
                                }
                            }

                            SlotHandler.SelectedSlot = null;
                        }

                        // 關閉建築面板
                        if (FindObjectOfType<BuildingPanelManager>() is BuildingPanelManager panelManager)
                            panelManager.CloseAllAndRoot();
                    }
                    else
                    {
                        Debug.Log("[BuildingTile] 金額不足，無法購買此建築：" + data.buildingName);
                        // 可額外彈出提示面板或文字提示玩家金額不足
                    }
                }
            });
        }

        else
        {
            Debug.LogError("[BuildingTile] Tile 上找不到 Button 組件！");
        }
    }

    // 新增方法：根據玩家金錢刷新解鎖狀態
    // 根據玩家金錢刷新解鎖狀態
    public void RefreshLockStatus(int playerCoins)
    {
        if (data == null) return;

        // === 解鎖邏輯 ===
        if (!permanentlyUnlocked && playerCoins >= data.price)
        {
            permanentlyUnlocked = true;   // 標記永久解鎖
            lockOverlay.SetActive(false); // 關閉鎖定遮罩
            //data.unlocked = true;
            interactText.text = data.InteractLimit;
        }

        // === lockOverlay 永遠不會重新開啟 ===
        if (permanentlyUnlocked)
        {
            lockOverlay.SetActive(false);
        }

        // === 價格文字顏色判斷 ===
        if (permanentlyUnlocked)
        {
            priceText.color = (playerCoins >= data.price) ? Color.black : Color.red;
        }
        else
        {
            priceText.color = Color.black; // 尚未解鎖的建築文字顏色保持黑色
        }
    }

}*/

public class BuildingTile : MonoBehaviour
{
    public Image iconImage;
    public TMP_Text nameText;
    public TMP_Text productionText;
    public TMP_Text streetText;
    public TMP_Text priceText;
    public TMP_Text interactText;
    public GameObject lockOverlay;

    private BuildingData data;
    private bool permanentlyUnlocked = false;

    // 新增：特殊建築已被建造過
    private bool hasBeenBuilt = false;

    public void Initialize(BuildingData buildingData)
    {
        data = buildingData;
        permanentlyUnlocked = false; // 重置永久解鎖
        // 特殊建築初始狀態
        if (data.panelType != PanelType.Normal && hasBeenBuilt)
        {
            permanentlyUnlocked = false; // 永遠不可再購買
            lockOverlay.SetActive(true);
            interactText.text = "已建造";
        }
        else
        {
            lockOverlay.SetActive(!data.unlocked);
            interactText.text = "鎖定中";
        }

        iconImage.sprite = data.icon;
        nameText.text = data.buildingName;
        productionText.text = data.production;
        streetText.text = data.streetLimit;
        priceText.text = "$" + data.price;

        Button btn = GetComponent<Button>();

        if (btn != null)
        {
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() =>
            {
                // 特殊建築已建造過，不可再點
                if (data.panelType != PanelType.Normal && hasBeenBuilt) return;

                if (!permanentlyUnlocked) return; // 用 Tile 自己的解鎖狀態判斷

                CoinManager coinManager = FindObjectOfType<CoinManager>();
                if (coinManager != null)
                {
                    if (coinManager.GetTotalCoins() >= data.price)
                    {
                        coinManager.DeductCoins(data.price);

                        // 設為解鎖
                        data.unlocked = true;

                        // 如果是特殊建築，標記已建造
                        if (data.panelType != PanelType.Normal)
                        {
                            hasBeenBuilt = true;
                        }

                        // 更新 lockOverlay 與文字
                        lockOverlay.SetActive(!data.unlocked);
                        interactText.text = data.unlocked ? data.InteractLimit : "鎖定中";

                        Debug.Log($"已購買建築：{data.buildingName}，扣除 {data.price} 寶錢");

                        BuildingManager.Instance.StartPlacementMode(data);

                        if (SlotHandler.SelectedSlot != null)
                        {
                            var slot = SlotHandler.SelectedSlot;
                            if (slot.gridManager != null)
                            {
                                var gridSlot = slot.gridManager.GridSlots[slot.rowIndex, slot.buildingIndex];
                                if (gridSlot != null && gridSlot.EmptySlotWorld != null)
                                {
                                    SpriteRenderer sr = gridSlot.EmptySlotWorld.GetComponent<SpriteRenderer>();
                                    if (sr != null)
                                    {
                                        sr.sprite = data.icon;
                                        sr.enabled = true;
                                    }
                                    BuildingManager.Instance.TryPlaceBuilding(slot.rowIndex, slot.buildingIndex);
                                }
                            }

                            SlotHandler.SelectedSlot = null;
                        }

                        if (FindObjectOfType<BuildingPanelManager>() is BuildingPanelManager panelManager)
                            panelManager.CloseAllAndRoot();
                    }
                    else
                    {
                        Debug.Log("[BuildingTile] 金額不足，無法購買此建築：" + data.buildingName);
                    }
                }
            });
        }
        else
        {
            Debug.LogError("[BuildingTile] Tile 上找不到 Button 組件！");
        }
    }

    // 根據玩家金錢刷新解鎖狀態
    public void RefreshLockStatus(int playerCoins)
    {
        if (data == null) return;

        // 特殊建築已建造，鎖定
        if (data.panelType != PanelType.Normal && hasBeenBuilt)
        {
            permanentlyUnlocked = false;
            lockOverlay.SetActive(true);
            interactText.text = "已建造";
            return; // 直接返回，不改變價格文字顏色
        }

        // === 普通建築解鎖邏輯 ===
        if (!permanentlyUnlocked && playerCoins >= data.price)
        {
            permanentlyUnlocked = true;   // 標記永久解鎖
            lockOverlay.SetActive(false); // 關閉鎖定遮罩
            interactText.text = data.InteractLimit;
        }

        // === lockOverlay 永遠不會重新開啟 ===
        if (permanentlyUnlocked)
        {
            lockOverlay.SetActive(false);
        }

        // === 價格文字顏色判斷（僅普通建築）===
        if (data.panelType == PanelType.Normal)
        {
            if (permanentlyUnlocked)
                priceText.color = (playerCoins >= data.price) ? Color.black : Color.red;
            else
                priceText.color = Color.black; // 尚未解鎖的建築文字顏色保持黑色
        }
    }
}
