//using System;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GridManager : MonoBehaviour
{
    [Header("Grid World Settings")]
    public float buildingWidth = 1.5f;
    public float buildingHeight = 1.5f;
    public float spacing = 100f;

    [Header("Dependencies")]
    public GameObject emptySlotPrefab;
    public Transform gridContainer;

    public int numBuildings = 10;
    public int numRows = 3;

    [System.Serializable]
    public class GridSlot
    {
        public GameObject CurrentBuilding;
        public bool IsBuilt;
        public GameObject EmptySlotWorld;
    }

    public GridSlot[,] GridSlots;

    //  新增：可選擇預先生成哪個建築
    [Header("初始建築設定")]
    public BuildingData defaultFirstBuilding;
    public int startRow = 0;
    public int startColumn = 0;

    private void Awake()
    {
        GridSlots = new GridSlot[numRows, numBuildings];
        InstantiateEmptySlotsWorld();
    }
    private void Start()
    {
        //  新增：一開始自動放一棟建築
        if (defaultFirstBuilding != null)
        {
            // 防止越界
            if (startRow < numRows && startColumn < numBuildings)
            {
                PlaceBuilding(startRow, startColumn, defaultFirstBuilding);
            }
            else
            {
                Debug.LogWarning("[GridManager] 初始建築的位置 (row/column) 超出範圍，請檢查 Inspector 設定！");
            }
        }
    }
    void InstantiateEmptySlotsWorld()
    {
        if (gridContainer == null)
            gridContainer = this.transform;

        for (int r = 0; r < numRows; r++)
        {
            for (int b = 0; b < numBuildings; b++)
            {
                GridSlots[r, b] = new GridSlot();

                float xPos = b * (buildingWidth + spacing) + buildingWidth / 2f;
                float yPos = -(r * (buildingHeight + spacing) + buildingHeight / 2f);
                Vector3 localPos = new Vector3(xPos, yPos, 0f);

                GameObject emptySlotWorld = Instantiate(emptySlotPrefab, gridContainer);
                emptySlotWorld.name = $"Slot_{r}_{b}";
                emptySlotWorld.transform.localPosition = localPos;

                // 設定 SlotHandler
                SlotHandler handler = emptySlotWorld.GetComponent<SlotHandler>();
                if (handler != null)
                {
                    handler.rowIndex = r;
                    handler.buildingIndex = b;
                }

                GridSlots[r, b].EmptySlotWorld = emptySlotWorld;
            }
        }
    }

    public Vector3 GetSlotPosition(int row, int building)
    {
        float localCenterX = building * (buildingWidth + spacing) + buildingWidth / 2f;
        float localCenterY = -(row * (buildingHeight + spacing) + buildingHeight / 2f);
        Vector3 localPos = new Vector3(localCenterX, localCenterY, 0f);

        if (gridContainer != null)
            return gridContainer.TransformPoint(localPos);
        else
            return localPos;
    }

    public void PlaceBuilding(int row, int building, BuildingData data)
    {
        if (!GridSlots[row, building].IsBuilt)
        {
            // 將建築物生成在對應格子 EmptySlotWorld 底下
            GameObject slotGO = GridSlots[row, building].EmptySlotWorld;
            if (slotGO == null)
            {
                Debug.LogWarning($"[PlaceBuilding] 格子 ({row},{building}) 的 EmptySlotWorld 為 null");
                return;
            }

            GameObject newBuilding = Instantiate(data.placedBuildingPrefab, slotGO.transform);
            newBuilding.transform.localPosition = Vector3.zero; // 對齊格子
            newBuilding.transform.localRotation = Quaternion.identity;
            newBuilding.SetActive(true); // 確保生成後啟用

            // ★★★ 這行最重要：把 ScriptableObject 塞進建築物的 Building.cs ★★★
            Building b = newBuilding.GetComponent<Building>();
            if (b != null)
            {
                b.data = data;
                b.panelType = data.panelType;
            }
            // b.InitializeBuilding(data); // 用這個方法初始化，才會正確設定 panelType



            // ===== 新增：將生成的實體存到 BuildingData 內 =====
            data.placedInstance = newBuilding;

            // 更新格子資料
            GridSlots[row, building].CurrentBuilding = newBuilding;
            GridSlots[row, building].IsBuilt = true;

            // 假設 EmptySlotWorld 上有 SpriteRenderer（可選，用於顯示格子底圖）
            SpriteRenderer slotSR = slotGO.GetComponent<SpriteRenderer>();
            if (slotSR != null)
            {
                slotSR.sprite = data.icon;  // 將建築圖片設定給 SpriteRenderer
                slotSR.enabled = true;      // 確保可見
            }

            // === 新增：建築完成，通知 RecruitManager ===
            if (RecruitManager.Instance != null)
            {
                // 只對普通建築自動跳出招募面板
                if (data.panelType == PanelType.Normal)
                {
                    RecruitManager.Instance.ShowRecruitStartPanel(data);
                }
                // RecruitManager.Instance.ShowRecruitStartPanel(data);
            }



            Debug.Log("生成建築物: " + newBuilding.name + ", 來源 prefab: " + data.placedBuildingPrefab.name);

        }
    }
}

// 顯示任務視窗
/*public void OpenBuildingPanel(int row, int building)
{
    // 儲存當前點擊的網格座標
    selectedRow = row;
    selectedBuilding = building;

    // 只有當該位置還沒被佔用時才開啟面板
    if (!GridSlots[row, building].IsBuilt)
    {
        // [新] 觸發事件，通知 BuildingPanelManager 開啟 UI
        OnEmptySlotClicked?.Invoke(row, building);

    }

}



public int GetSelectedRow() { return selectedRow; }//?
public int GetSelectedBuilding() { return selectedBuilding; }//?
*/

