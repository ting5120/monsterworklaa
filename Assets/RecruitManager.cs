using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class RecruitManager : MonoBehaviour
{
    public static RecruitManager Instance { get; private set; }

    [Header("面板")]
    public GameObject recruitStartPanel;
    public GameObject recruitMainPanel;
    public GameObject nextButtonOverlay;

    [Header("按鈕")]
    public Button confirmRecruitButton;
    public Button nextMonsterButton;

    [Header("次數顯示")]
    public TMP_Text remainingCountText;

    [Header("妖怪資料")]
    public List<MonsterData> allMonsters;  // 所有可招募妖怪
    private MonsterData currentMonster;    // 當前顯示妖怪
    private MonsterInstance currentMonsterInstance; // 實際招募存給建築物

    [Header("UI 元件")]
    public Image monsterImageUI;
    public Image borderImageUI;
    public TMP_Text nameText;
    public TMP_Text descriptionText;
    public TMP_Text levelText;
    public TMP_Text workEffText;
    public TMP_Text costumeEffText;

    [Header("抽取機率設定")]
    [SerializeField] private float normalRate = 0.7125f;
    [SerializeField] private float rareRate = 0.2156f;
    [SerializeField] private float legendaryRate = 0.02985f;

    // 等級內壞妖怪比例（隱性）
    [SerializeField] private float normalBadRate = 0.0526f;  // 3.75 / 71.25
    [SerializeField] private float rareBadRate = 0.0204f;    // 0.44 / 21.56
    [SerializeField] private float legendaryBadRate = 0.005f; // 0.015 / 2.985

    [Header("設定")]
    [SerializeField] private int maxNextCount = 4;

    [Header("生成妖怪測試用")]
    public GameObject monsterPrefab;
    
    //[Header("生成高度設定")]
    //[SerializeField] private float monsterHeightOffset = 1.0f;



    // 當前剩餘次數
    private int remainingNextCount;

    // 當前正在招募的建築物
    private BuildingData currentBuildingData;
    // 新增：當前正在招募的建築實體
    private Building currentTargetBuilding;

    // 在 Awake() 裡初始化 coinManager
    private CoinManager coinManager;

    // 新增：是否已進入「手動建造流程」
    private bool hasEnteredManualBuildFlow = false;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        recruitStartPanel.SetActive(false);
        recruitMainPanel.SetActive(false);

        coinManager = FindObjectOfType<CoinManager>();
        if (coinManager != null)
        {
            CoinManager.OnCoinChanged += UpdateNextButtonState;
        }
    }

  
    ///  由 BuildingPanelManager 呼叫
    /// 玩家「主動打開建築面板」時通知
    public void NotifyEnteredManualBuildFlow()
    {
        hasEnteredManualBuildFlow = true;
        Debug.Log("[RecruitManager] 已進入手動建造流程");
    }



    // 開始招募面板顯示
    public void ShowRecruitStartPanel(BuildingData buildingData)
    {
        //  尚未進入手動建造流程 → 不顯示
        if (!hasEnteredManualBuildFlow)
        {
            Debug.Log("[RecruitManager] 尚未進入手動建造流程，不顯示招募面板");
            return;
        }

        // 新增：只對普通建築顯示招募面板
        if (buildingData.panelType != PanelType.Normal)
        {
            Debug.Log($"[RecruitManager] 建築 {buildingData.buildingName} panelType={buildingData.panelType} 非 Normal，不顯示招募面板");
            return;
        }
        currentBuildingData = buildingData;

        recruitStartPanel.SetActive(true);
        recruitMainPanel.SetActive(false);
        Debug.Log($"[RecruitManager] 顯示招募開始面板（建築：{buildingData.buildingName}）");
   
    }

    // 點擊 StartPanel 按鈕
    public void OnStartRecruitButton()
    {
        recruitStartPanel.SetActive(false);
        recruitMainPanel.SetActive(true);

        ResetRecruitState();
    }

    // ==========================
    // Recruit Main Panel 邏輯
    // ==========================
    private void ResetRecruitState()
    {
        remainingNextCount = maxNextCount;
        UpdateRemainingUI();
        UpdateNextButtonState();

        // 隨機抽一隻妖怪並刷新 UI
        currentMonster = GetRandomMonster();
        RefreshMonsterUI();
    }

    // 換下一隻妖怪
    public void OnNextMonsterButton()
    {
        if (remainingNextCount <= 0)
            return;

        remainingNextCount--;

        Debug.Log($"[RecruitManager] 換下一隻妖怪，剩餘次數：{remainingNextCount}");

        UpdateRemainingUI();
        UpdateNextButtonState();

        // 隨機抽一隻妖怪並刷新 UI
        currentMonster = GetRandomMonster();
        RefreshMonsterUI();
    }


    // 確認招募

    public void OnConfirmRecruitButton()
    {
        Debug.Log("[RecruitManager] 確認招募按鈕被點擊");

        Building building = currentTargetBuilding;

        // 保底：若不是從資遣進來，才退回舊邏輯
        if (building == null && currentBuildingData != null && currentBuildingData.placedInstance != null)
        {
            building = currentBuildingData.placedInstance.GetComponent<Building>();
        }

        if (building == null)
        {
            Debug.LogError("[RecruitManager] 找不到對應的 Building，招募中止");
            return;
        }

       ////// 生成 MonsterInstance 並決定好壞
        currentMonsterInstance = CreateMonsterInstance(currentMonster);

        // 將招募的妖怪存回建築物
        building.recruitedMonster = currentMonster;
        building.monsterInstance = currentMonsterInstance; /////// 實際 Instance


        Debug.Log($"建築物 {building.data.buildingName} 已招募妖怪：{currentMonster.monsterName}");

        // 解鎖圖鑑（你說這邊不用改 ）
        if (currentMonster != null)
        {
            MonsterBookManager.Instance.UnlockMonster(currentMonster.ID);
        }

        // 生成妖怪
        SpawnMonsterAtBuilding(building);

        recruitMainPanel.SetActive(false);

        // 用完即清（避免下次誤用）
        currentTargetBuilding = null;
    }


    private void SpawnMonsterAtBuilding(Building building)
    {
        if (building == null || building.monsterInstance == null)
            return;

        // 使用 Building 的 SpawnMonster 生成 prefab
        building.SpawnMonster(building.recruitedMonster);

        if (building.monsterInstance != null)
        {
            Debug.Log($"[RecruitManager] 建築 {building.data.buildingName} 初始化妖怪 {building.recruitedMonster.monsterName} 好壞：{building.monsterInstance.alignment}");
        }

    }


    // ==========================
    // 隨機抽取與 UI 刷新
    // ==========================

    private MonsterData GetRandomMonster()
    {
        if (allMonsters == null || allMonsters.Count == 0) return null;

        // 先抽等級
        float r = Random.value;
        MonsterLevel chosenLevel;
        if (r < normalRate) chosenLevel = MonsterLevel.Normal;
        else if (r < normalRate + rareRate) chosenLevel = MonsterLevel.Rare;
        else chosenLevel = MonsterLevel.Legendary;

        // 從 allMonsters 過濾該等級池
        var candidates = allMonsters.FindAll(m => m.level == chosenLevel);
        if (candidates.Count == 0) return null;

        // 隨機挑選
        int index = Random.Range(0, candidates.Count);
        MonsterData monster = candidates[index];

        return monster;
    }

    // 新增一個方法：生成 MonsterInstance 並決定好壞
    private MonsterInstance CreateMonsterInstance(MonsterData data)
    {
        if (data == null) return null;

        // 再抽好壞
        float r2 = Random.value;
        MonsterAlignment chosenAlignment = MonsterAlignment.Good;
        switch (data.level)
        {
            case MonsterLevel.Normal:
                if (r2 < normalBadRate) chosenAlignment = MonsterAlignment.Bad;
                break;
            case MonsterLevel.Rare:
                if (r2 < rareBadRate) chosenAlignment = MonsterAlignment.Bad;
                break;
            case MonsterLevel.Legendary:
                if (r2 < legendaryBadRate) chosenAlignment = MonsterAlignment.Bad;
                break;
        }

        // 生成 Instance
        GameObject go = new GameObject(); // 先暫存物件，不生成 prefab
        MonsterInstance mi = go.AddComponent<MonsterInstance>();
        mi.Init(data, chosenAlignment);
        return mi;
    }



    private void RefreshMonsterUI()
    {
        Debug.Log("[RecruitManager] RefreshMonsterUI 被呼叫");

        if (currentMonster == null)
        {
            Debug.LogWarning("currentMonster 為 null");
            return;
        }

        Debug.Log("顯示妖怪：" + currentMonster.monsterName);
        monsterImageUI.sprite = currentMonster.monsterImage;
        borderImageUI.sprite = currentMonster.borderImage;

        nameText.text = currentMonster.monsterName;
        descriptionText.text = currentMonster.description;

        levelText.text = "妖怪品質：" + GetLevelText(currentMonster.level);
        workEffText.text = "工作效率：" + currentMonster.workEfficiency.ToString("F1");
        costumeEffText.text = "服飾加成：" + currentMonster.costumeEfficiency.ToString("F1");
    }

    // 【資遣用】明確指定是哪一棟建築要重新招募
    public void ShowRecruitStartPanel(Building building)
    {
        if (building == null)
        {
            Debug.LogWarning("[RecruitManager] ShowRecruitStartPanel(building) building 為 null");
            return;
        }

        if (!hasEnteredManualBuildFlow)
        {
            Debug.Log("[RecruitManager] 尚未進入手動建造流程，不顯示招募面板");
            return;
        }

        if (building.data.panelType != PanelType.Normal)
        {
            Debug.Log($"[RecruitManager] 建築 {building.data.buildingName} 非 Normal，不顯示招募面板");
            return;
        }

        // 關鍵：直接記住 Building
        currentTargetBuilding = building;
        currentBuildingData = building.data; // 保留你原本邏輯

        recruitStartPanel.SetActive(true);
        recruitMainPanel.SetActive(false);

        Debug.Log($"[RecruitManager] 顯示招募開始面板（建築實體：{building.data.buildingName}）");
    }

    // ==========================
    // UI 更新
    // ==========================

    private string GetLevelText(MonsterLevel level)
    {
        switch (level)
        {
            case MonsterLevel.Normal:
                return "藍";
            case MonsterLevel.Rare:
                return "紫";
            case MonsterLevel.Legendary:
                return "金";
            default:
                return "";
        }
    }

    private void UpdateRemainingUI()
    {
        if (remainingCountText != null)
        {
            //remainingCountText.text = remainingNextCount.ToString();
            remainingCountText.text = $"招募次數：{remainingNextCount}";

        }
    }

    /*private void UpdateNextButtonState()
    {
        if (nextMonsterButton == null)
            return;

        bool canClick = remainingNextCount > 0;
        nextMonsterButton.interactable = canClick;

        // 顯示或隱藏遮罩
        if (nextButtonOverlay != null)
            nextButtonOverlay.SetActive(!canClick); // 用完次數時顯示遮

        ColorBlock cb = nextMonsterButton.colors;
        if (canClick)
        {
            cb.normalColor = Color.white;   // 正常顏色
            cb.highlightedColor = Color.white;
        }
        else
        {
            cb.normalColor = Color.black;    // 灰掉顏色
            cb.highlightedColor = Color.black;
        }
        nextMonsterButton.colors = cb;
    }*/

    private void UpdateNextButtonState()
    {
        if (nextMonsterButton == null || coinManager == null)
            return;

        // 計算當前跳過次數對應的 b 值
        float b = 0f;
        switch (maxNextCount - remainingNextCount)
        {
            case 0: b = 0.5f; break;
            case 1: b = 0.65f; break;
            case 2: b = 0.75f; break;
            case 3: b = 0.8f; break;
            default: b = 0.8f; break; // 以防剩餘次數小於0
        }

        // 計算當前招募費用
        float a = 1f;
        if (currentMonster != null)
        {
            switch (currentMonster.level)
            {
                case MonsterLevel.Normal: a = 1.15f; break;
                case MonsterLevel.Rare: a = 1.35f; break;
                case MonsterLevel.Legendary: a = 1.65f; break;
            }
        }

        int currentRecruitCost = Mathf.CeilToInt(100f * a * b);

        // 判斷是否可點：剩餘次數 > 0 且玩家金錢足夠
        bool canClick = remainingNextCount > 0 && coinManager.HasEnough(currentRecruitCost);
        nextMonsterButton.interactable = canClick;

        // 顯示或隱藏遮罩
        if (nextButtonOverlay != null)
            nextButtonOverlay.SetActive(!canClick);

        // 調整按鈕顏色
        ColorBlock cb = nextMonsterButton.colors;
        if (canClick)
        {
            cb.normalColor = Color.white;
            cb.highlightedColor = Color.white;
        }
        else
        {
            cb.normalColor = Color.black;
            cb.highlightedColor = Color.black;
        }
        nextMonsterButton.colors = cb;

        // 顯示當前招募費（可選）
        Debug.Log($"[RecruitManager] 當前跳過費用：{currentRecruitCost}，玩家金錢：{coinManager.TotalCoins}");
    }
}

