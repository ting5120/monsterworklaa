using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems; // <<--- 1. 引入 EventSystems 命名空間


public class InputManager : MonoBehaviour
{
        
    [Header("Dependencies")]
    public CameraManager cameraManager; // 拖曳 Main Camera 物件到此欄位
    public UIManager uiManager;         // 新增：拖入 UIManager

    public BuildingPanelManager buildingPanelManager;

    [Header("Touch Settings")]
    // 閾值: 判定為有效滑動的最小像素距離
    public float threshold = 50f;

    [HideInInspector]
    public bool blockCameraInput = false;

    private Vector2 touchStartPos;
    private bool isDragging = false;
   

    void Update()
    {
        if (blockCameraInput)
        {
            isDragging = false; // 阻止本 frame 偵測滑動
            return;
        }

        // --------------------------------------------------------------------------------
        // 檢查 UIManager 中是否有任何 Panel 正在開啟
        // --------------------------------------------------------------------------------
        if (PanelBlocker.Instance != null && PanelBlocker.Instance.IsAnyPanelOpen())
        {
            isDragging = false;
            return;
        }



        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                touchStartPos = touch.position;
                isDragging = true;
            }
            else if (touch.phase == TouchPhase.Ended && isDragging)
            {
                Vector2 dragVector = touch.position - touchStartPos;
                float absX = Mathf.Abs(dragVector.x);
                float absY = Mathf.Abs(dragVector.y);

                // 判斷是主要橫向還是垂直滑動
                if (absX > threshold && absX > absY)
                {
                    // 左右滑動
                    int direction = (dragVector.x > 0) ? -1 : 1; // X 軸方向與 Index 變動方向相反
                    cameraManager.ScrollHorizontal(direction);

                    // TODO: 視覺暗示 - 播放左右滑動音效
                }
                else if (absY > threshold && absY > absX)
                {
                    // 上下滑動
                    int direction = (dragVector.y > 0) ? 1 : -1; // Y 軸方向與 Row Index 變動方向相同
                    cameraManager.ScrollVertical(direction);

                    // TODO: 視覺暗示 - 播放上下切換音效
                }

                isDragging = false;
            }
        }

        // 可選：PC 偵錯輸入 (用滑鼠模擬滑動)
        // 邏輯與上面 TouchPhase.Ended 類似，使用 Input.GetMouseButtonUp(0) 處理

        // --------------------------------------------------------------------------------
        // 新增：滑鼠模擬觸控 (PC Editor 偵錯用)  完整版輸出前刪
        // --------------------------------------------------------------------------------
        else if (Application.isEditor) // 只有在 Unity Editor 中才執行
        {
            if (Input.GetMouseButtonDown(0)) // 滑鼠左鍵按下
            {
                //  除錯點 1：確認 Began 發生
                Debug.Log("Mouse Began at: " + Input.mousePosition);
                touchStartPos = Input.mousePosition;
                isDragging = true;
            }
            else if (Input.GetMouseButtonUp(0) && isDragging) // 滑鼠左鍵放開
            {

                // 除錯點 2：確認 Ended 發生
                Debug.Log("Mouse Ended at: " + Input.mousePosition);

                // 將滑鼠位置視為觸控結束位置
                Vector2 touchEndPos = Input.mousePosition;
                Vector2 dragVector = touchEndPos - touchStartPos;

                //  除錯點 3：檢查滑動距離
                Debug.Log($"Drag Vector: X={dragVector.x}, Y={dragVector.y}");

                float absX = Mathf.Abs(dragVector.x);
                float absY = Mathf.Abs(dragVector.y);

                // 執行與觸控結束時相同的判斷邏輯
                if (absX > threshold && absX > absY)
                {
                    int direction = (dragVector.x > 0) ? -1 : 1;
                    cameraManager.ScrollHorizontal(direction);
                }
                else if (absY > threshold && absY > absX)
                {
                    int direction = (dragVector.y > 0) ? 1 : -1;
                    cameraManager.ScrollVertical(direction);
                }

                isDragging = false;
            }
            // 如果滑鼠正在拖曳，但還沒放開，我們不需要做任何事 (因為只在 Ended 時才判斷)
        }

    }
}
