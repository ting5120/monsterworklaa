// 記得也要引用 System 才能使用 Action
using System.Collections;
using System.Collections.Generic;
/*public class BuildingPanelManager : MonoBehaviour
{
    [Header("主面板")]
    public GameObject buildingPanel; // 空格子建築選單 & 建築資訊共用主面板
    public GameObject missionPanel;
    public Button buildingCloseButton;

    [Header("Info Panel")]
    public Button infoButton;
    public GameObject detailPanelRoot;
    public Button infoCloseButton;

    [Header("普通建築面板")]
    public GameObject normalLeftPanel;
    public GameObject normalRightPanel;

    [Header("特殊建築面板")]
    public GameObject fashionStorePanel;
    public GameObject decorationStorePanel;

    [Header("普通建築面板 Prefab")]
    public GameObject normalBuildingPanelPrefab; 


    [Header("背景關閉區域")]
    public GameObject closeBackground;

    [Header("數據來源")]
    public BuildingList buildingList;

    [Header("UI Prefab 與位置")]
    [SerializeField] private GameObject TilePrefab;
    [SerializeField] private Transform contentParent;

    public UIManager uiManager;

    public static BuildingPanelManager Instance;

    void Awake()
    {
        // 初始化面板
        buildingPanel.SetActive(false);
        normalLeftPanel.SetActive(false);
        normalRightPanel.SetActive(false);
        fashionStorePanel.SetActive(false);
        decorationStorePanel.SetActive(false);
        detailPanelRoot.SetActive(false);

        // 綁定按鈕事件
        if (buildingCloseButton != null)
            buildingCloseButton.onClick.AddListener(CloseAllAndRoot);

        if (infoButton != null)
            infoButton.onClick.AddListener(OpenDetailPanel);
        if (infoCloseButton != null)
            infoCloseButton.onClick.AddListener(CloseDetailPanel);

        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }

    void Start()
    {
        // 保留原始邏輯，生成 ScrollView 的建築 Tile
        GenerateBuildingSlots();

        // 將 PanelBlocker 註冊延後到 Start()，確保 PanelBlocker 已 Awake
        if (PanelBlocker.Instance != null)
        {
            PanelBlocker.Instance.RegisterPanel(buildingPanel);
            PanelBlocker.Instance.RegisterPanel(normalLeftPanel);
            PanelBlocker.Instance.RegisterPanel(normalRightPanel);
            PanelBlocker.Instance.RegisterPanel(fashionStorePanel);
            PanelBlocker.Instance.RegisterPanel(decorationStorePanel);
            PanelBlocker.Instance.RegisterPanel(detailPanelRoot);
            PanelBlocker.Instance.RegisterPanel(missionPanel);
        }
        else
        {
            Debug.LogError("[BuildingPanelManager] PanelBlocker.Instance 尚未生成！");
        }

    }

    public void GenerateBuildingSlots()
    {
        if (contentParent == null || TilePrefab == null) return;

        // 清空舊的 UI
        foreach (Transform child in contentParent)
            Destroy(child.gameObject);

        if (buildingList == null || buildingList.allBuildings.Count == 0)
        {
            Debug.LogError("BuildingList 未指定或沒有資料");
            return;
        }

        foreach (BuildingData data in buildingList.allBuildings)
        {
            GameObject newSlot = Instantiate(TilePrefab, contentParent);
            newSlot.name = $"Slot_{data.buildingName}";
            newSlot.SetActive(true);

            BuildingTile tileScript = newSlot.GetComponent<BuildingTile>();
            if (tileScript != null)
            {
               
                tileScript.Initialize(data);
            }
            else
            {
                Debug.LogError("TilePrefab 缺少 BuildingTile 組件！");
            }
        }
    }
    // ========================
    // 空格子建築選單
    // ========================
    public void OpenBuildMenu(int row, int col)
    {
        Debug.Log($"[BuildingPanelManager] OpenBuildMenu called at slot ({row},{col})");

        if (buildingPanel != null)
        {
            buildingPanel.SetActive(true);
            Debug.Log("[BuildingPanelManager] buildingPanel active");
        }
        if (missionPanel != null)
            missionPanel.SetActive(false);

        if (closeBackground != null)
            closeBackground.SetActive(true);

        //  告知 RecruitManager：玩家正式進入建造流程
        if (RecruitManager.Instance != null)
            RecruitManager.Instance.NotifyEnteredManualBuildFlow();

        Debug.Log($"[BuildingPanelManager] 打開空格子建築選單 (Row:{row}, Col:{col})");
    }

    // 新增：刷新所有 Tile 的鎖定狀態
    public void RefreshAllTiles(int playerCoins)
    {
        if (contentParent == null) return;

        foreach (Transform child in contentParent)
        {
            BuildingTile tile = child.GetComponent<BuildingTile>();
            if (tile != null)
            {
                tile.RefreshLockStatus(playerCoins);
            }
        }
    }

    // ========================
    // 建築點擊後的面板
    // ========================
    public void OpenPanel(Building building)
    {
        if (building == null || building.data == null)
        {
            Debug.LogWarning("[BuildingPanelManager] OpenPanel: building or data is null");
            return;
        }

        Debug.Log($"[BuildingPanelManager] OpenPanel called: {building.name}, panelType={building.panelType}");
        Debug.Log($"[BuildingPanelManager] building.data.panelType={building.data.panelType}, building.panelType={building.panelType}");

        // 關閉空格子建築面板
        buildingPanel?.SetActive(false);

        switch (building.panelType)
        {
            case PanelType.Normal:
                Debug.Log("[BuildingPanelManager] Open NormalPanel");
                building.OpenNormalPanel(this.transform);
                break;
            case PanelType.FashionStore:
                Debug.Log("[BuildingPanelManager] Open FashionStorePanel");
                fashionStorePanel?.SetActive(true);
                break;
            case PanelType.DecorationStore:
                Debug.Log("[BuildingPanelManager] Open DecorationStorePanel");
                decorationStorePanel?.SetActive(true);
                break;
        }

        closeBackground?.SetActive(true);
    }

    public void ConfirmBuild(BuildingData data)
    {
        Debug.Log("[BuildingPanelManager] ConfirmBuild: " + data.buildingName);

        // 通知 BuildingManager 進入放置模式
        BuildingManager.Instance.StartPlacementMode(data);

        // 關閉所有 UI 面板
        CloseAllPanels();
    }

   
    // ========================
    // 關閉所有面板
    // ========================
    public void CloseAllPanels()
    {
        normalLeftPanel.SetActive(false);
        normalRightPanel.SetActive(false);
        fashionStorePanel.SetActive(false);
        decorationStorePanel.SetActive(false);
        detailPanelRoot.SetActive(false);
        missionPanel.SetActive(false);

        if (closeBackground != null)
            closeBackground.SetActive(false);
    }

    public void CloseAllAndRoot()
    {
        CloseAllPanels();
        if (buildingPanel != null)
            buildingPanel.SetActive(false);
    }

    // ========================
    // Info Panel 控制
    // ========================
    public void OpenDetailPanel() => detailPanelRoot.SetActive(true);
    public void CloseDetailPanel() => detailPanelRoot.SetActive(false);

    public bool IsAnyPanelOpen()
    {
        return (buildingPanel != null && buildingPanel.activeInHierarchy) ||
               (normalLeftPanel != null && normalLeftPanel.activeInHierarchy) ||
               (normalRightPanel != null && normalRightPanel.activeInHierarchy) ||
               (fashionStorePanel != null && fashionStorePanel.activeInHierarchy) ||
               (decorationStorePanel != null && decorationStorePanel.activeInHierarchy) ||
               (detailPanelRoot != null && detailPanelRoot.activeInHierarchy) ||
               (missionPanel != null && missionPanel.activeInHierarchy);
    }


}*/

//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;
//新俺
/*public class BuildingPanelManager : MonoBehaviour
{
    [Header("主面板")]
    public GameObject buildingPanel; // 空格子建築選單 & 建築資訊共用主面板
    public GameObject missionPanel;
    public Button buildingCloseButton;

    [Header("Info Panel")]
    public Button infoButton;
    public GameObject detailPanelRoot;
    public Button infoCloseButton;

    [Header("特殊建築面板")]
    public GameObject fashionStorePanel;
    public GameObject decorationStorePanel;

    [Header("普通建築面板 Prefab")]
    public GameObject normalBuildingPanelPrefab;

    [Header("Canvas 父物件")]
    public Transform canvasTransform;

    [Header("背景關閉區域")]
    public GameObject closeBackground;

    [Header("數據來源")]
    public BuildingList buildingList;

    [Header("UI Prefab 與位置")]
    [SerializeField] private GameObject TilePrefab;
    [SerializeField] private Transform contentParent;

    public UIManager uiManager;

    public static BuildingPanelManager Instance;

    // 儲存每棟建築對應的普通面板
    private Dictionary<Building, GameObject> normalPanelInstances = new Dictionary<Building, GameObject>();

    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;

        // 初始化面板
        buildingPanel.SetActive(false);
        fashionStorePanel.SetActive(false);
        decorationStorePanel.SetActive(false);
        detailPanelRoot.SetActive(false);
        missionPanel.SetActive(false);
        closeBackground.SetActive(false);

        if (buildingCloseButton != null)
            buildingCloseButton.onClick.AddListener(CloseAllAndRoot);
        if (infoButton != null)
            infoButton.onClick.AddListener(OpenDetailPanel);
        if (infoCloseButton != null)
            infoCloseButton.onClick.AddListener(CloseDetailPanel);
    }

    void Start()
    {
        GenerateBuildingSlots();

        if (PanelBlocker.Instance != null)
        {
            PanelBlocker.Instance.RegisterPanel(buildingPanel);
            PanelBlocker.Instance.RegisterPanel(fashionStorePanel);
            PanelBlocker.Instance.RegisterPanel(decorationStorePanel);
            PanelBlocker.Instance.RegisterPanel(detailPanelRoot);
            PanelBlocker.Instance.RegisterPanel(missionPanel);
        }
        else
        {
            Debug.LogError("[BuildingPanelManager] PanelBlocker.Instance 尚未生成！");
        }
    }

    #region 建築 Tile
    public void GenerateBuildingSlots()
    {
        if (contentParent == null || TilePrefab == null) return;

        foreach (Transform child in contentParent)
            Destroy(child.gameObject);

        if (buildingList == null || buildingList.allBuildings.Count == 0)
        {
            Debug.LogError("BuildingList 未指定或沒有資料");
            return;
        }

        foreach (BuildingData data in buildingList.allBuildings)
        {
            GameObject newSlot = Instantiate(TilePrefab, contentParent);
            newSlot.name = $"Slot_{data.buildingName}";
            newSlot.SetActive(true);

            BuildingTile tileScript = newSlot.GetComponent<BuildingTile>();
            if (tileScript != null)
                tileScript.Initialize(data);
            else
                Debug.LogError("TilePrefab 缺少 BuildingTile 組件！");
        }
    }

    public void RefreshAllTiles(int playerCoins)
    {
        if (contentParent == null) return;

        foreach (Transform child in contentParent)
        {
            BuildingTile tile = child.GetComponent<BuildingTile>();
            if (tile != null)
                tile.RefreshLockStatus(playerCoins);
        }
    }
    #endregion

    #region 空格子建築
    public void OpenBuildMenu(int row, int col)
    {
        buildingPanel?.SetActive(true);
        missionPanel?.SetActive(false);
        closeBackground?.SetActive(true);

        if (RecruitManager.Instance != null)
            RecruitManager.Instance.NotifyEnteredManualBuildFlow();

        Debug.Log($"[BuildingPanelManager] OpenBuildMenu at ({row},{col})");
    }
    #endregion

    #region 建築面板管理
    public void ShowBuildingPanel(Building building)
    {
        if (building == null || building.data == null) return;

        buildingPanel?.SetActive(false);
        closeBackground?.SetActive(true);

        switch (building.panelType)
        {
            case PanelType.Normal:
                ShowNormalPanel(building);
                break;
            case PanelType.FashionStore:
                fashionStorePanel?.SetActive(true);
                break;
            case PanelType.DecorationStore:
                decorationStorePanel?.SetActive(true);
                break;
        }
    }

    private void ShowNormalPanel(Building building)
    {
        if (!normalPanelInstances.TryGetValue(building, out GameObject panel))
        {
            panel = Instantiate(normalBuildingPanelPrefab, canvasTransform);
            panel.name = $"{building.name}_NormalPanel";

            // 初始化面板資料
            var costumePanel = panel.GetComponentInChildren<CostumePanelManager>();
            if (costumePanel != null)
            {
                costumePanel.ownerBuilding = building;
                costumePanel.InitializePanel();
                costumePanel.RefreshPanel();
            }

            normalPanelInstances.Add(building, panel);
        }

        panel.SetActive(true);
        panel.transform.SetAsLastSibling();

        // 固定位置在 Canvas 中心
        var rt = panel.GetComponent<RectTransform>();
        if (rt != null)
        {
            rt.anchoredPosition = Vector2.zero;
            rt.localScale = Vector3.one;
        }

        Debug.Log($"[BuildingPanelManager] NormalPanel shown: {panel.name}");
    }
    #endregion

    #region 面板關閉
    public void CloseAllNormalPanels()
    {
        foreach (var kvp in normalPanelInstances)
            kvp.Value?.SetActive(false);
    }

    public void CloseAllPanels()
    {
        CloseAllNormalPanels();
        fashionStorePanel?.SetActive(false);
        decorationStorePanel?.SetActive(false);
        detailPanelRoot?.SetActive(false);
        missionPanel?.SetActive(false);
        closeBackground?.SetActive(false);
    }

    public void CloseAllAndRoot()
    {
        CloseAllPanels();
        buildingPanel?.SetActive(false);
    }
    #endregion

    #region Info Panel
    public void OpenDetailPanel() => detailPanelRoot.SetActive(true);
    public void CloseDetailPanel() => detailPanelRoot.SetActive(false);
    #endregion

    public bool IsAnyPanelOpen()
    {
        return (buildingPanel != null && buildingPanel.activeInHierarchy) ||
               (fashionStorePanel != null && fashionStorePanel.activeInHierarchy) ||
               (decorationStorePanel != null && decorationStorePanel.activeInHierarchy) ||
               (detailPanelRoot != null && detailPanelRoot.activeInHierarchy) ||
               (missionPanel != null && missionPanel.activeInHierarchy) ||
               normalPanelInstances.Values.Any(p => p.activeInHierarchy);
    }

    public void ConfirmBuild(BuildingData data)
    {
        if (data == null) return;

        BuildingManager.Instance.StartPlacementMode(data);
        CloseAllPanels();
    }
}*/


using System.Linq;
using UnityEngine;
using UnityEngine.UI;


public class BuildingPanelManager : MonoBehaviour
{
    [Header("主面板")]
    public GameObject buildingPanel; // 空格子建築選單 & 建築資訊共用主面板
    public GameObject missionPanel;
    public Button buildingCloseButton;

    [Header("Info Panel")]
    public Button infoButton;
    public GameObject detailPanelRoot;
    public Button infoCloseButton;

    [Header("特殊建築面板")]
    public GameObject fashionStorePanel;
    public GameObject decorationStorePanel;
    public GameObject monsterBookPanel;
    public GameObject recruitMainPanel;
    public GameObject recruitStartPanel;
    public GameObject backPackPanel;

    [Header("UI Buttons")]
    public GameObject backpackButton;

    [Header("普通建築面板 Prefab")]
    public GameObject normalBuildingPanelPrefab;

    [Header("Canvas 父物件")]
    public Transform canvasTransform;

    [Header("背景關閉區域")]
    public GameObject closeBackground;

    [Header("數據來源")]
    public BuildingList buildingList;

    [Header("UI Prefab 與位置")]
    [SerializeField] private GameObject TilePrefab;
    [SerializeField] private Transform contentParent;

    public UIManager uiManager;

    public static BuildingPanelManager Instance;

    // 儲存每棟建築對應的普通面板
    private Dictionary<Building, GameObject> normalPanelInstances = new Dictionary<Building, GameObject>();

    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;

        // 初始化面板
        buildingPanel.SetActive(false);
        fashionStorePanel.SetActive(false);
        decorationStorePanel.SetActive(false);
        detailPanelRoot.SetActive(false);
        missionPanel.SetActive(false);
        closeBackground.SetActive(false);

        if (buildingCloseButton != null)
            buildingCloseButton.onClick.AddListener(CloseAllAndRoot);
        if (infoButton != null)
            infoButton.onClick.AddListener(OpenDetailPanel);
        if (infoCloseButton != null)
            infoCloseButton.onClick.AddListener(CloseDetailPanel);
    }

    void Start()
    {
        GenerateBuildingSlots();

        if (PanelBlocker.Instance != null)
        {
            PanelBlocker.Instance.RegisterPanel(buildingPanel);
            PanelBlocker.Instance.RegisterPanel(fashionStorePanel);
            PanelBlocker.Instance.RegisterPanel(decorationStorePanel);
            PanelBlocker.Instance.RegisterPanel(detailPanelRoot);
            PanelBlocker.Instance.RegisterPanel(missionPanel);
            PanelBlocker.Instance.RegisterPanel(monsterBookPanel);
            PanelBlocker.Instance.RegisterPanel(recruitMainPanel);
            PanelBlocker.Instance.RegisterPanel(recruitStartPanel);
            PanelBlocker.Instance.RegisterPanel(backPackPanel);

        }
        else
        {
            Debug.LogError("[BuildingPanelManager] PanelBlocker.Instance 尚未生成！");
        }
    }

    #region 建築 Tile
    public void GenerateBuildingSlots()
    {
        if (contentParent == null || TilePrefab == null) return;

        foreach (Transform child in contentParent)
            Destroy(child.gameObject);

        if (buildingList == null || buildingList.allBuildings.Count == 0)
        {
            Debug.LogError("BuildingList 未指定或沒有資料");
            return;
        }

        foreach (BuildingData data in buildingList.allBuildings)
        {
            GameObject newSlot = Instantiate(TilePrefab, contentParent);
            newSlot.name = $"Slot_{data.buildingName}";
            newSlot.SetActive(true);

            BuildingTile tileScript = newSlot.GetComponent<BuildingTile>();
            if (tileScript != null)
                tileScript.Initialize(data);
            else
                Debug.LogError("TilePrefab 缺少 BuildingTile 組件！");
        }
    }

    public void RefreshAllTiles(int playerCoins)
    {
        if (contentParent == null) return;

        foreach (Transform child in contentParent)
        {
            BuildingTile tile = child.GetComponent<BuildingTile>();
            if (tile != null)
                tile.RefreshLockStatus(playerCoins);
        }
    }
    #endregion

    #region 空格子建築
    public void OpenBuildMenu(int row, int col)
    {
        buildingPanel?.SetActive(true);
        missionPanel?.SetActive(false);
        closeBackground?.SetActive(true);

        if (backpackButton != null)
            backpackButton.SetActive(false);


        if (RecruitManager.Instance != null)
            RecruitManager.Instance.NotifyEnteredManualBuildFlow();

        // 延遲開啟背景，避免立刻擋住點擊
        StartCoroutine(EnableCloseBackgroundNextFrame());

        Debug.Log($"[BuildingPanelManager] OpenBuildMenu at ({row},{col})");
    }
    private IEnumerator EnableCloseBackgroundNextFrame()
    {
        yield return null; // 等待一幀
        closeBackground?.SetActive(true);
    }

    #endregion

    #region 建築面板管理
    public void ShowBuildingPanel(Building building)
    {
        if (building == null || building.data == null) return;

        buildingPanel?.SetActive(false);
        closeBackground?.SetActive(true);

        switch (building.panelType)
        {
            case PanelType.Normal:
                ShowNormalPanel(building);
                break;
            case PanelType.FashionStore:
                fashionStorePanel?.SetActive(true);
                break;
            case PanelType.DecorationStore:
                decorationStorePanel?.SetActive(true);
                break;
        }
    }

    private void ShowNormalPanel(Building building)
    {
        Debug.Log($"[ShowNormalPanel] building.monsterInstance = {building.monsterInstance}");

        if (!normalPanelInstances.TryGetValue(building, out GameObject panel))
        {
            panel = Instantiate(normalBuildingPanelPrefab, canvasTransform);
            panel.name = $"{building.name}_NormalPanel";

            // 初始化面板資料
            var costumePanel = panel.GetComponentInChildren<CostumePanelManager>();
            if (costumePanel != null)
            {
                costumePanel.ownerBuilding = building;

                // 直接刷新面板，顯示已擁有服飾
                costumePanel.RefreshFromDataCenter();

                // 不再直接指派 targetMonster，按鈕點擊改成呼叫 building.EquipCostume()
                foreach (var btn in costumePanel.costumeButtons)
                {
                    btn.panelManager = costumePanel;
                    //btn.targetMonster = null; // 清掉 MonsterInstance 指向
                    btn.button.onClick.RemoveAllListeners();
                    btn.button.onClick.AddListener(() =>
                    {
                        if (btn.currentCostume != null)
                        {
                            building.EquipCostume(btn.currentCostume.costumeID);
                            building.equippedCostume = btn.currentCostume; // 更新建築物目前裝備
                            costumePanel.RefreshPanel(); // 刷新面板顯示
                        }
                    });
                }
            }


            // ===== 初始化升級面板 =====
            var upgradePanel = panel.GetComponentInChildren<UpgradePanelManager>(true);
            if (upgradePanel != null)
            {
                upgradePanel.ownerBuilding = building;
                // 注意：不要呼叫 Refresh()，讓它保持初始狀態

                upgradePanel.InitPanel(); // ← 生成時就初始化 UI

            }

            // ===== 初始化資遣按鈕 =====
            var dismissScript = panel.GetComponentInChildren<MonsterDismissButton>();
            if (dismissScript != null)
            {
                dismissScript.ownerBuilding = building;
                Debug.Log($"[BuildingPanelManager] MonsterDismissButton 綁定建築: {building.data.buildingName}");
            }

            normalPanelInstances.Add(building, panel);
            Debug.Log($"[BuildingPanelManager] 普通面板生成完成: {panel.name}");
        }

        panel.SetActive(true);
        panel.transform.SetAsLastSibling();

        // 固定位置在 Canvas 中心
        var rt = panel.GetComponent<RectTransform>();
        if (rt != null)
        {
            rt.anchoredPosition = Vector2.zero;
            rt.localScale = Vector3.one;
        }

        Debug.Log($"[BuildingPanelManager] NormalPanel shown: {panel.name}");
    }

    // 加在 BuildingPanelManager 類別裡面
    public GameObject GetNormalPanel(Building building)
    {
        if (building == null) return null;

        if (normalPanelInstances.TryGetValue(building, out GameObject panel))
            return panel;

        return null;
    }

    #endregion

    public void RefreshAllCostumePanels()
    {
        foreach (var kvp in normalPanelInstances)
        {
            var panel = kvp.Value;
            if (panel == null) continue;

            var costumePanel = panel.GetComponentInChildren<CostumePanelManager>();
            if (costumePanel != null)
                costumePanel.RefreshFromDataCenter();
        }
    }

    // 新增新服飾到所有已生成面板
    public void AddNewCostumeToAllPanels(CostumeData data)
    {
        foreach (var kvp in normalPanelInstances)
        {
            var costumePanel = kvp.Value.GetComponentInChildren<CostumePanelManager>();
            if (costumePanel != null)
                costumePanel.AddPurchasedCostume(data); // 只新增按鈕
        }
    }


    #region 面板關閉
    public void CloseAllNormalPanels()
    {
        foreach (var kvp in normalPanelInstances)
            kvp.Value?.SetActive(false);

        if (backpackButton != null)
            backpackButton.SetActive(true);

    }

    public void CloseAllPanels()
    {
        CloseAllNormalPanels();
        fashionStorePanel?.SetActive(false);
        decorationStorePanel?.SetActive(false);
        detailPanelRoot?.SetActive(false);
        missionPanel?.SetActive(false);
        closeBackground?.SetActive(false);
    }

    public void CloseAllAndRoot()
    {
        CloseAllPanels();
        buildingPanel?.SetActive(false);
    }
    #endregion

    #region Info Panel
    public void OpenDetailPanel() => detailPanelRoot.SetActive(true);
    public void CloseDetailPanel() => detailPanelRoot.SetActive(false);
    #endregion

    public bool IsAnyPanelOpen()
    {
        return (buildingPanel != null && buildingPanel.activeInHierarchy) ||
               (fashionStorePanel != null && fashionStorePanel.activeInHierarchy) ||
               (decorationStorePanel != null && decorationStorePanel.activeInHierarchy) ||
               (detailPanelRoot != null && detailPanelRoot.activeInHierarchy) ||
               (missionPanel != null && missionPanel.activeInHierarchy) ||
               (monsterBookPanel != null && monsterBookPanel.activeInHierarchy) ||
               (recruitMainPanel != null && recruitMainPanel.activeInHierarchy) ||
               (recruitStartPanel != null && recruitStartPanel.activeInHierarchy) ||
               (backPackPanel != null && backPackPanel.activeInHierarchy) ||


               normalPanelInstances.Values.Any(p => p.activeInHierarchy);
    }

    public void ConfirmBuild(BuildingData data)
    {
        if (data == null) return;

        BuildingManager.Instance.StartPlacementMode(data);
        CloseAllPanels();
    }
}
