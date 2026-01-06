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

        if (currentBuildingData != null && currentBuildingData.placedInstance != null)
        {
            // 取得建築物實體
            Building building = currentBuildingData.placedInstance.GetComponent<Building>();
            if (building != null)
            {
                // 將招募的妖怪存回建築物
                building.recruitedMonster = currentMonster;

                // Debug 確認
                Debug.Log($"建築物 {building.data.buildingName} 已招募妖怪：{building.recruitedMonster.monsterName}");
                Debug.Log($"等級: {building.recruitedMonster.level}, 好壞: {building.recruitedMonster.alignment}");

                // ==========================
                // 新增這一行：解鎖圖鑑
                // ==========================
                if (currentMonster != null)
                {
                    MonsterBookManager.Instance.UnlockMonster(currentMonster.ID);
                }
            }

            // 生成在建築物前方
            SpawnMonsterAtBuilding(building);
        }

        recruitMainPanel.SetActive(false);
    }


    /*private void SpawnMonsterAtBuilding(Building building)
    {
        if (building == null || building.recruitedMonster == null)
            return;

        Transform spawnPoint = building.monsterSpawnPoint;
        if (spawnPoint == null)
        {
            Debug.LogWarning("[RecruitManager] monsterSpawnPoint 未設定");
            return;
        }

        // 關鍵：取得 MonsterInstance
        MonsterInstance instance = spawnPoint.GetComponent<MonsterInstance>();
        if (instance == null)
        {
            Debug.LogError("[RecruitManager] MonsterSpawnPoint 上沒有 MonsterInstance");
            return;
        }

        //  初始化妖怪（交給 MonsterInstance 管）
        instance.Init(building.recruitedMonster);

        Debug.Log($"[RecruitManager] 建築 {building.data.buildingName} 初始化妖怪 {building.recruitedMonster.monsterName}");
    }*/

    private void SpawnMonsterAtBuilding(Building building)
    {
        if (building == null || building.recruitedMonster == null)
            return;

        // 確保 spawnPoint 存在
        Transform spawnPoint = building.monsterSpawnPoint;
        if (spawnPoint == null)
        {
            Debug.LogWarning("[RecruitManager] monsterSpawnPoint 未設定");
            return;
        }

        // 使用 Building 的 SpawnMonster 來生成怪物
        building.SpawnMonster(building.recruitedMonster);

        // 這時候 building.monsterInstance 已經指向生成的 MonsterInstance
        if (building.monsterInstance != null)
        {
            Debug.Log($"[RecruitManager] 建築 {building.data.buildingName} 初始化妖怪 {building.recruitedMonster.monsterName}");
        }
        else
        {
            Debug.LogError("[RecruitManager] 生成後 monsterInstance 仍為 null");
        }
    }


    // ==========================
    // 隨機抽取與 UI 刷新
    // ==========================
    private MonsterData GetRandomMonster()
    {
        
        if (allMonsters == null || allMonsters.Count == 0) return null;

        //  先抽等級
        float r = Random.value; // 0~1
        MonsterLevel chosenLevel;
        if (r < normalRate)
        {
            chosenLevel = MonsterLevel.Normal;
        }
        else if (r < normalRate + rareRate)
        {
            chosenLevel = MonsterLevel.Rare;
        }
        else if (r < normalRate + rareRate + legendaryRate)
        {
            chosenLevel = MonsterLevel.Legendary;
        }
        else
        {
            // 以防萬一，保底給 Legendary
            chosenLevel = MonsterLevel.Legendary;
        }

        //  再抽好壞
        float r2 = Random.value;
        MonsterAlignment chosenAlignment = MonsterAlignment.Good;
        switch (chosenLevel)
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

        //  從 allMonsters 過濾該等級池
        var candidates = allMonsters.FindAll(m => m.level == chosenLevel);

        if (candidates.Count == 0) return null;

        //  隨機挑選
        int index = Random.Range(0, candidates.Count);
        MonsterData monster = candidates[index];

        //  套用隱性好壞
        monster.alignment = chosenAlignment;

        return monster;
    

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

    private void UpdateNextButtonState()
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
    }


}

