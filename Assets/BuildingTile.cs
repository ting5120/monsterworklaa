//using DummyNamespace;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;




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

}



//判斷失敗版
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
    private bool permanentlyUnlocked;

    public void Initialize(BuildingData buildingData)
    {
        data = buildingData;
        permanentlyUnlocked = false;

        iconImage.sprite = data.icon;
        nameText.text = data.buildingName;
        productionText.text = data.production;
        streetText.text = data.streetLimit;
        priceText.text = "$" + data.price;

        RefreshVisualState();

        Button btn = GetComponent<Button>();
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(OnClickTile);
    }

    private void OnClickTile()
    {
        // === 特殊建築：已建過  永久鎖定 ===
        if (data.panelType != PanelType.Normal && data.hasBeenPlaced)
        {
            Debug.Log("[BuildingTile] 特殊建築已建造，無法再次放置：" + data.buildingName);
            return;
        }

        // === 普通建築：尚未解鎖不可點 ===
        if (data.panelType == PanelType.Normal && !permanentlyUnlocked)
            return;

        // === 金錢判斷（只針對普通建築） ===
        if (data.panelType == PanelType.Normal)
        {
            CoinManager coinManager = FindObjectOfType<CoinManager>();
            if (coinManager == null || coinManager.GetTotalCoins() < data.price)
            {
                Debug.Log("[BuildingTile] 金額不足：" + data.buildingName);
                return;
            }

            coinManager.DeductCoins(data.price);
        }

        // === 成功進入放置流程 ===
        data.unlocked = true;

        if (data.panelType != PanelType.Normal)
            data.hasBeenPlaced = true;

        //  進入放置模式
        BuildingManager.Instance.StartPlacementMode(data);

        // 【關鍵】加回原本「立即放置」的保險流程 
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

        RefreshVisualState();
    }

    public void RefreshLockStatus(int playerCoins)
    {
        if (data == null) return;

        // 普通建築：金錢解鎖
        if (data.panelType == PanelType.Normal && playerCoins >= data.price)
            permanentlyUnlocked = true;

        RefreshVisualState(playerCoins);
    }

    private void RefreshVisualState(int playerCoins = int.MaxValue)
    {
        // === 特殊建築 ===
        if (data.panelType != PanelType.Normal)
        {
            bool locked = data.hasBeenPlaced;
            lockOverlay.SetActive(locked);
            interactText.text = locked ? "鎖定中" : data.InteractLimit;
            return;
        }

        // === 普通建築 ===
        lockOverlay.SetActive(!permanentlyUnlocked);
        interactText.text = permanentlyUnlocked ? data.InteractLimit : "鎖定中";

        priceText.color = permanentlyUnlocked && playerCoins < data.price
            ? Color.red
            : Color.black;
    }
}*/

