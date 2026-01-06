using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class SlotHandler : MonoBehaviour
{
    public int rowIndex;
    public int buildingIndex;

    [Header("依賴項")]
    public GridManager gridManager;
    public BuildingPanelManager panelManager;

    public bool IsBuilt => gridManager != null && gridManager.GridSlots[rowIndex, buildingIndex].IsBuilt;

    public static event System.Action<int, int> OnSlotClicked;
    public static SlotHandler SelectedSlot;

    private void Awake()
    {
        if (gridManager == null)
            gridManager = FindObjectOfType<GridManager>();
        if (panelManager == null)
            panelManager = FindObjectOfType<BuildingPanelManager>();
    }

    private void OnMouseDown()
    {
        if (gridManager == null || panelManager == null) return;

        // 任何 UI 開啟時禁止點擊格子
        if (panelManager.IsAnyPanelOpen())
            return;

        OnSlotClicked?.Invoke(rowIndex, buildingIndex);

        if (BuildingManager.Instance != null && BuildingManager.Instance.IsMoveMode)
            return;

        var slot = gridManager.GridSlots[rowIndex, buildingIndex];

        // ===== 空格子 =====
        if (!IsBuilt)
        {
            SelectedSlot = this;
            panelManager.OpenBuildMenu(rowIndex, buildingIndex);
            return; // 立即 return，避免後續邏輯執行
        }

        // ===== 已建建築 =====
        if (slot.CurrentBuilding != null)
        {
            Building building = slot.CurrentBuilding.GetComponent<Building>();
            if (building == null) return;

            Debug.Log($"[SlotHandler] Clicked {building.name}, panelType={building.panelType}");

            panelManager.ShowBuildingPanel(building);

        }
    }


}

