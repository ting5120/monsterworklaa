using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public bool blockMovement = false;

    [Header("Dependencies")]
    public GridManager gridManager; // 拖曳 GridManager 物件到此欄位

    [Header("Street System")]
    public StreetSystem streetSystem;


    [Header("Main Camera Settings")]
    public Camera mainCamera;           // 原本的遊戲攝影機
    public Camera overviewCamera;       // 全螢幕攝影機
    public bool isOverviewMode = false; // 是否處於全螢幕模式

    [Header("Movement Settings")]
    public float snapSpeed = 10f; // 鏡頭對齊速度

    private Vector3 targetPosition;
    private int currentBuildingIndex = 0; // 0 到 9
    private int currentRowIndex = 0;      // 0, 1, 2 (中間列為預設)

    /*void Start()
    {
        // 確保 GridManager 被連結
        if (gridManager == null)
            gridManager = FindObjectOfType<GridManager>();

        // 初始位置：中央列 (0)，第一棟建築物 (0)
        targetPosition = GetNewTargetPosition(0, 0);
        transform.position = targetPosition;
    }*/
    void Start()
    {
        if (gridManager == null)
            gridManager = FindObjectOfType<GridManager>();

        // 初始位置
        targetPosition = GetNewTargetPosition(0, 0);
        if (mainCamera != null)
            mainCamera.transform.position = targetPosition;

        // 確保攝影機狀態
        if (mainCamera != null) mainCamera.gameObject.SetActive(true);
        if (overviewCamera != null) overviewCamera.gameObject.SetActive(false);

        if (streetSystem != null)
            streetSystem.OnStreetChanged(currentRowIndex);

    }

    void Update()
    {
       // Debug.Log("CameraManager Update is running."); // 檢查點 A
        // 平滑移動到目標位置 (Lerp 實現平滑過渡)
        //transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * snapSpeed);

        if (!blockMovement)
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * snapSpeed);
    }

    // 獲取新的目標位置 (X/Y 軸都需考慮)
    private Vector3 GetNewTargetPosition(int buildingIndex, int rowIndex)
    {
        Vector3 pos = gridManager.GetSlotPosition(rowIndex, buildingIndex);
        // Z 軸保持相機的 Z 軸位置 (通常是 -10)
        return new Vector3(pos.x, pos.y, transform.position.z);
    }

    // =========================================================================
    // a. 左右滑動：強制對齊到一棟完整的建築物
    // =========================================================================
    public void ScrollHorizontal(int direction) // direction: +1 (滑動到下一棟), -1 (滑動到前一棟)
    {
        if (isOverviewMode) return; // 全螢幕時不使用滑動

        int newIndex = currentBuildingIndex + direction;

        // 邊界鎖定: 確保索引在 [0, 9] 範圍內
        newIndex = Mathf.Clamp(newIndex, 0, gridManager.numBuildings - 1);

        if (newIndex != currentBuildingIndex)
        {
            currentBuildingIndex = newIndex;
            targetPosition = GetNewTargetPosition(currentBuildingIndex, currentRowIndex);

            if (streetSystem != null)
                streetSystem.OnStreetChanged(currentRowIndex);
        }
    }

    // =========================================================================
    // b. 上下滑動：每一下只能滑到相鄰的那一條街
    // =========================================================================
    public void ScrollVertical(int direction) // direction: +1 (上滑), -1 (下滑)
    {
        int newRowIndex = currentRowIndex + direction;

        // 邊界鎖定: 確保索引在 [0, 2] 範圍內
        newRowIndex = Mathf.Clamp(newRowIndex, 0, gridManager.numRows - 1);

        if (newRowIndex != currentRowIndex)
        {
            currentRowIndex = newRowIndex;
            targetPosition = GetNewTargetPosition(currentBuildingIndex, currentRowIndex);
        }
    }

    // =========================================================
    // 切換全螢幕模式
    // =========================================================
    public void ToggleOverviewMode()
    {
        // 如果正在移動模式，禁止切換全螢幕
        if (BuildingManager.Instance != null && BuildingManager.Instance.IsMoveMode)
        {
            Debug.Log("[CameraManager] 無法切換全螢幕模式：正在移動建築物");
            return;
        }

        isOverviewMode = !isOverviewMode;

        if (mainCamera != null) mainCamera.gameObject.SetActive(!isOverviewMode);
        if (overviewCamera != null) overviewCamera.gameObject.SetActive(isOverviewMode);

        // ====== 找 Canvas 並切換 worldCamera ======
        Canvas canvas = FindObjectOfType<Canvas>(); // 自動抓主 UI Canvas
        if (canvas != null)
        {
            canvas.worldCamera = isOverviewMode ? overviewCamera : mainCamera;
        }

        // UI 顯示/隱藏
        UIManager uiManager = FindObjectOfType<UIManager>();
        if (uiManager != null)
        {
            uiManager.SetOverviewMode(isOverviewMode);
        }
    }
}
