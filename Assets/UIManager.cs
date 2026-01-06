using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/*public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
    }

    [Header("建築選擇面板")]
    public GameObject buildingPanel;

    [Header("按鈕")]
    public GameObject toggleOverviewButton; // 永遠可見，但可禁用
    public GameObject moveModeButton;       // 僅全螢幕模式
    public List<GameObject> otherButtons;   // 正常模式下的其他按鈕

    public Image overlayImage;
    public BuildingManager buildingManager;

    public int selectedRow, selectedBuilding;

    [Header("普通建築面板")]
    public GameObject normalLeftPanel;
    public GameObject normalRightPanel;

    [Header("特殊建築面板")]
    public GameObject fashionStorePanel;
    public GameObject decorationStorePanel;


    // 開始招募面板 暫關
    /*[Header("招募面板")]
    public GameObject recruitStartPanel;   
    public GameObject recruitMainPanel;    
    private Building currentBuilding;      // 儲存是哪棟建築要招募*/

// ========================
// 打開建築選擇面板
// ========================
/* public void OpenBuildingSelectionPanel(Building building)
 {
     // 如果招募面板任何一個正在開啟，就不要打開建築面板 暫關
     /*if ((recruitStartPanel != null && recruitStartPanel.activeSelf) ||
         (recruitMainPanel != null && recruitMainPanel.activeSelf))
     {
         Debug.Log("[UIManager] 招募面板開啟中，禁止打開建築面板");
         return;
     }*/

// 先關閉所有面板，避免重疊
/* if (buildingPanel != null) buildingPanel.SetActive(false);

 // 如果 building 為 null，表示只是打開空格子建築選擇面板
 if (building == null)
 {
     if (buildingPanel != null) buildingPanel.SetActive(true);
     Debug.Log("[UIManager] 打開空格子建築選擇面板");
     return;
 }

 // 根據 panelType 顯示對應面板
 switch (building.data.panelType)
 {
     case PanelType.Normal:
         if (normalLeftPanel != null) normalLeftPanel.SetActive(true);
         if (normalRightPanel != null) normalRightPanel.SetActive(true);
         if (fashionStorePanel != null) fashionStorePanel.SetActive(false);
         if (decorationStorePanel != null) decorationStorePanel.SetActive(false);
         break;

     case PanelType.FashionStore:
         if (normalLeftPanel != null) normalLeftPanel.SetActive(false);
         if (normalRightPanel != null) normalRightPanel.SetActive(false);
         if (fashionStorePanel != null) fashionStorePanel.SetActive(true);
         if (decorationStorePanel != null) decorationStorePanel.SetActive(false);
         break;

     case PanelType.DecorationStore:
         if (normalLeftPanel != null) normalLeftPanel.SetActive(false);
         if (normalRightPanel != null) normalRightPanel.SetActive(false);
         if (fashionStorePanel != null) fashionStorePanel.SetActive(false);
         if (decorationStorePanel != null) decorationStorePanel.SetActive(true);
         break;
 }

 // 開啟建築面板
 if (buildingPanel != null) buildingPanel.SetActive(true);

 Debug.Log($"[UIManager] 打開 {building.data.buildingName} 面板 ({building.data.panelType})");
}

public void ConfirmPlaceBuilding(BuildingData data)
{
 if (data == null || buildingManager == null) return;

 buildingManager.StartPlacementMode(data);
 buildingManager.TryPlaceBuilding(selectedRow, selectedBuilding);

 buildingPanel.SetActive(false);
}

// 新增：建築建立完成時顯示起始招募面板 暫關
/*public void ShowRecruitStartPanel(Building building)
{
 currentBuilding = building;

 // 關閉建築面板避免重疊
 if (buildingPanel != null)
     buildingPanel.SetActive(false);

 // 顯示第一個招募開始面板
 if (recruitStartPanel != null)
     recruitStartPanel.SetActive(true);

 // 正式招募面板確保關閉
 if (recruitMainPanel != null)
     recruitMainPanel.SetActive(false);
}

// 新增：按下開始招募按鈕 暫關
public void OnClickStartRecruit()
{
 if (recruitStartPanel != null)
     recruitStartPanel.SetActive(false);

 if (recruitMainPanel != null)
     recruitMainPanel.SetActive(true);

 Debug.Log("[UIManager] 進入正式招募面板");
}*/



// ========================
// 更新全螢幕模式 UI
// ========================
/*public void SetOverviewMode(bool isOverview)
{
    if (buildingPanel != null) buildingPanel.SetActive(false);

    // toggleOverviewButton 永遠顯示
    if (toggleOverviewButton != null)
    {
        toggleOverviewButton.SetActive(true);
        UpdateOverviewButtonState(); // 更新是否可按
    }

    if (isOverview)
    {
        // 顯示移動模式按鈕
        if (moveModeButton != null) moveModeButton.SetActive(true);

        // 隱藏其他按鈕
        foreach (var btn in otherButtons)
        {
            if (btn != null) btn.SetActive(false);
        }
    }
    else
    {
        // 離開全螢幕模式，隱藏移動模式按鈕
        if (moveModeButton != null) moveModeButton.SetActive(false);

        // 顯示其他按鈕
        foreach (var btn in otherButtons)
        {
            if (btn != null) btn.SetActive(true);
        }
    }
}

// ========================
// 移動模式開關
// ========================
public void StartMoveMode()
{
    if (buildingManager == null)
    {
        Debug.LogError("[UIManager] buildingManager 未指派");
        return;
    }

    if (buildingManager.IsMoveMode)
    {
        ShowOverlay(false);
        buildingManager.ExitMoveMode();
        UpdateOverviewButtonState();
        return;
    }

    ShowOverlay(true);
    buildingManager.StartMoveMode();
    UpdateOverviewButtonState();
}

// ========================
// 更新 toggleOverviewButton 狀態（可按 / 禁用）
// ========================
public void UpdateOverviewButtonState()
{
    if (toggleOverviewButton == null) return;

    Button btn = toggleOverviewButton.GetComponent<Button>();
    if (btn == null) return;

    if (buildingManager != null && buildingManager.IsMoveMode)
    {
        btn.interactable = false;
        Image img = toggleOverviewButton.GetComponent<Image>();
        if (img != null)
        {
            Color c = img.color;
            c.a = 0.5f; // 半透明表示不可按
            img.color = c;
        }
    }
    else
    {
        btn.interactable = true;
        Image img = toggleOverviewButton.GetComponent<Image>();
        if (img != null)
        {
            Color c = img.color;
            c.a = 1f; // 恢復正常
            img.color = c;
        }
    }
}

// ========================
// 顯示 / 隱藏 overlay
// ========================
public void ShowOverlay(bool show)
{
    if (overlayImage == null) return;
    overlayImage.gameObject.SetActive(show);
    overlayImage.color = show ? new Color(0, 0, 0, 0.3f) : new Color(0, 0, 0, 0f);
}

// ========================
// 檢查是否有面板開啟
// ========================
public bool IsAnyPanelOpen()
{
    if (buildingPanel == null) return false;
    return buildingPanel.activeInHierarchy;
}
}*/



//整合測試


public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("按鈕")]
    public GameObject toggleOverviewButton; // 永遠可見，但可禁用
    public GameObject moveModeButton;       // 僅全螢幕模式
    public List<GameObject> otherButtons;   // 正常模式下的其他按鈕

    public Image overlayImage;
    public BuildingManager buildingManager;

    [Header("背包相關")]
    public GameObject backpackPanel;      // Inspector 指向 BackpackPanel
    public Button backpackMainButton;     // Inspector 指向主畫面背包按鈕


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        // 綁定背包按鈕事件
        if (backpackMainButton != null)
            backpackMainButton.onClick.AddListener(ToggleBackpackPanel);
    }


    // ========================
    // 更新全螢幕模式 UI
    // ========================
    public void SetOverviewMode(bool isOverview)
    {
        if (toggleOverviewButton != null)
        {
            toggleOverviewButton.SetActive(true);
            UpdateOverviewButtonState();
        }

        if (isOverview)
        {
            if (moveModeButton != null) moveModeButton.SetActive(true);
            foreach (var btn in otherButtons)
                if (btn != null) btn.SetActive(false);
        }
        else
        {
            if (moveModeButton != null) moveModeButton.SetActive(false);
            foreach (var btn in otherButtons)
                if (btn != null) btn.SetActive(true);
        }
    }

    // ========================
    // 移動模式開關
    // ========================
    public void StartMoveMode()
    {
        if (buildingManager == null)
        {
            Debug.LogError("[UIManager] buildingManager 未指派");
            return;
        }

        if (buildingManager.IsMoveMode)
        {
            ShowOverlay(false);
            buildingManager.ExitMoveMode();
            UpdateOverviewButtonState();
            return;
        }

        ShowOverlay(true);
        buildingManager.StartMoveMode();
        UpdateOverviewButtonState();
    }

    // ========================
    // 更新 toggleOverviewButton 狀態
    // ========================
    public void UpdateOverviewButtonState()
    {
        if (toggleOverviewButton == null) return;
        Button btn = toggleOverviewButton.GetComponent<Button>();
        if (btn == null) return;

        if (buildingManager != null && buildingManager.IsMoveMode)
        {
            btn.interactable = false;
            var img = toggleOverviewButton.GetComponent<Image>();
            if (img != null)
            {
                Color c = img.color;
                c.a = 0.5f;
                img.color = c;
            }
        }
        else
        {
            btn.interactable = true;
            var img = toggleOverviewButton.GetComponent<Image>();
            if (img != null)
            {
                Color c = img.color;
                c.a = 1f;
                img.color = c;
            }
        }
    }

    // ========================
    // 顯示 / 隱藏 overlay
    // ========================
    public void ShowOverlay(bool show)
    {
        if (overlayImage == null) return;
        overlayImage.gameObject.SetActive(show);
        overlayImage.color = show ? new Color(0, 0, 0, 0.3f) : new Color(0, 0, 0, 0f);
    }

    // ========================
    // 背包面板開關
    // ========================
    public void ToggleBackpackPanel()
    {
        if (backpackPanel != null)
            backpackPanel.SetActive(!backpackPanel.activeSelf);
    }

}


