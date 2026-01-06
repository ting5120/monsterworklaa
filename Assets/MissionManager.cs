//using DummyNamespace;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


/*public class MissionManager : MonoBehaviour
{
    [Header("任務視窗與按鈕")]
    public GameObject missionPanel;//任務視窗
    public Button missioncloseButton;//關閉視窗的按鈕
    public Button missionButton; // 在 Inspector 指派

    public GameObject buildingsPanel;//建築視窗

    [Header("任務列表設定")]
    public GameObject missionItemPrefab;    // 任務項目 Prefab
    public Transform missionListParent;     // 放任務項目的父物件（例如 ScrollView/Content）
    
    [Header("其他物件連結")]
    public CoinManager coinManager;
    

    // 第一個任務狀態
    //private bool firstMissionCompleted = false;
    private List<MissionData> missions = new List<MissionData>();

    void Awake()
    {
        missionPanel.SetActive(false); // 一開始就隱藏
        
    }

    void Start()
    {
        
            if (coinManager == null) Debug.LogError(" CoinManager 沒有被指定！");
            if (missionPanel == null) Debug.LogError(" MissionPanel 沒有被指定！");
            if (missioncloseButton == null) Debug.LogError(" CloseButton 沒有被指定！");
            if (missionButton == null) Debug.LogError(" MissionButton 沒有被指定！");
            // 其他檢查...
        
        // 綁定關閉按鈕事件
        missioncloseButton.onClick.AddListener(CloseMissionPanel);
        missionButton.onClick.AddListener(OpenMissionPanel);

        InitializeMissions();
    }


    void Update()
    {
        CheckMissionsProgress();
    }

    // 初始化任務
    void InitializeMissions()
    {
        // 第一個任務
        missions.Add(new MissionData
        {
            title = "第一個任務：解鎖基礎建設",
            description = "當寶錢達到 100 時，可解鎖第一個可建建築。",
            requirement = 100,
            isUnlocked = true,
            isCompleted = false
        });

        // 第二個任務
        missions.Add(new MissionData
        {
            title = "第二個任務：新的開始",
            description = "完成第一個任務後解鎖此任務。",
            requirement = 0,
            isUnlocked = false,
            isCompleted = false
        });

        foreach (MissionData mission in missions)
        {
            CreateMissionUI(mission);
        }
    }

    // 建立任務項目 UI
    void CreateMissionUI(MissionData mission)
    {
        GameObject obj = Instantiate(missionItemPrefab, missionListParent);
        mission.ui = obj;

        mission.titleText = obj.transform.Find("Title").GetComponent<TextMeshProUGUI>();
        mission.descriptionText = obj.transform.Find("Description").GetComponent<TextMeshProUGUI>();
        mission.actionButton = obj.transform.Find("ActionButton").GetComponent<Button>();
        mission.buttonText = mission.actionButton.transform.Find("Text").GetComponent<TextMeshProUGUI>();

        mission.titleText.text = mission.title;
        mission.descriptionText.text = mission.description;
        mission.buttonText.text = "前往";

        mission.actionButton.onClick.AddListener(() => OnMissionButtonClicked(mission));
    }

    // 檢查任務進度
    void CheckMissionsProgress()
    {
        int currentCoins = coinManager != null ? coinManager.GetTotalCoins() : 0;

        foreach (MissionData mission in missions)
        {
            // 第一個任務檢查條件
            if (mission.title.Contains("第一個") && !mission.isCompleted && currentCoins >= mission.requirement)
            {
                mission.isCompleted = true;
                mission.buttonText.text = "領取";
                Debug.Log("第一個任務完成！");

                //OpenMissionPanel();
            }

            // 第二個任務：第一個完成後才解鎖
            if (mission.title.Contains("第二個") && !mission.isUnlocked && missions[0].isCompleted)
            {
                mission.isUnlocked = true;
                mission.ui.SetActive(true);
                Debug.Log("第二個任務已解鎖！");
            }
        }
    }

    // 按下任務按鈕
    void OnMissionButtonClicked(MissionData mission)
    {
        if (mission.isCompleted && mission.buttonText.text == "領取")
        {
            if (mission.title.Contains("第一個"))
            {
                // 顯示商城面板、隱藏任務視窗與主畫面顯示商城按鈕
                buildingsPanel.SetActive(true);///跳出建築面版待編輯
                CloseMissionPanel();
                Debug.Log("建築已解鎖並顯示！");

                // --- 【關鍵修改點：禁用按鈕】 ---
                mission.actionButton.interactable = false;
                mission.buttonText.text = "已領取"; // 更改文本，提供更清晰的狀態回饋
            }
        }
        else
        {
            // 尚未完成 → 例如前往主畫面
            Debug.Log($"前往任務相關區域：{mission.title}");
            CloseMissionPanel();
        }
    }

    // 顯示任務視窗
    public void OpenMissionPanel()
    {
        missionPanel.SetActive(true);
        buildingsPanel.SetActive(false);///
       
    }

    // 關閉任務視窗
    public void CloseMissionPanel()
    {
        missionPanel.SetActive(false);
    }
}

// 任務資料結構
[System.Serializable]
public class MissionData
{
    public string title;
    public string description;
    public int requirement;
    public bool isUnlocked;
    public bool isCompleted;

    [HideInInspector] public GameObject ui;
    [HideInInspector] public TextMeshProUGUI titleText;
    [HideInInspector] public TextMeshProUGUI descriptionText;
    [HideInInspector] public Button actionButton;
    [HideInInspector] public TextMeshProUGUI buttonText;
}*/







    
 
    


