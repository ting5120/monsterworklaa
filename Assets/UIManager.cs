using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;





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


