//using DummyNamespace;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


/*public class MissionUI : MonoBehaviour
{
    public TextMeshProUGUI titleText;      // 任務名稱
    public TextMeshProUGUI descriptionText; // 任務描述
    public Button actionButton;            // 執行任務的按鈕
    private Mission currentMission;        // 關聯的任務資料

    // 初始化任務 UI
    public void Setup(Mission mission)
    {
        currentMission = mission;
        titleText.text = mission.missionName;
        descriptionText.text = mission.missionDescription;

        actionButton.onClick.RemoveAllListeners();
        actionButton.onClick.AddListener(OnActionButtonClicked);

        UpdateUI();
    }

    // 按下任務按鈕的行為
    private void OnActionButtonClicked()
    {
        if (!currentMission.isCompleted)
        {
            currentMission.onAction?.Invoke();   // 達成前行為
        }
        else if (currentMission.isCompleted && !currentMission.rewardClaimed)
        {
            currentMission.rewardClaimed = true;
            currentMission.onReward?.Invoke();   // 領取行為
        }

        UpdateUI();

        
    }

    // 根據任務狀態更新 UI
    private void UpdateUI()
    {
        var buttonText = actionButton.GetComponentInChildren<TextMeshProUGUI>();

        if (!currentMission.isUnlocked)
        {
            actionButton.interactable = true; // 達成前仍可點選
            buttonText.text = "尚未達成";
        }
        else if (currentMission.isCompleted && !currentMission.rewardClaimed)
        {
            actionButton.interactable = true;
            buttonText.text = "領取";
        }
        else if (currentMission.rewardClaimed)
        {
            actionButton.interactable = false;
            buttonText.text = "已完成";
        }
        else
        {
            actionButton.interactable = true;
            buttonText.text = "開始任務";
        }

    
    }
}*/