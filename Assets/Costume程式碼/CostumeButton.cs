using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[System.Serializable]
/*public class CostumeButton : MonoBehaviour
{
    public Button button;
    public Image buttonImage;
    public CostumeData currentCostume;
    [HideInInspector] public CostumePanelManager panelManager;
    [HideInInspector] public bool isSelected = false;

    private void Awake()
    {
        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OnClick);
        }
    }

    public void OnClick()
    {
        if (panelManager != null)
            panelManager.OnClickCostumeButton(this);
    }

    public void SetCostume(CostumeData data)
    {
        currentCostume = data;

        if (buttonImage != null && data != null)
            buttonImage.sprite = data.costumeImage;

        gameObject.SetActive(true);
    }

    public void SetSelected(bool selected)
    {
        isSelected = selected;
        if (buttonImage != null)
            buttonImage.color = selected ? Color.gray : Color.white;
    }
}*/




public class CostumeButton : MonoBehaviour
{
    [Header("UI 元件")]
    public Button button;
    public Image buttonImage;

    [Header("服飾資料")]
    public CostumeData currentCostume;

    [HideInInspector] public CostumePanelManager panelManager;
    [HideInInspector] public bool isSelected = false;
    [HideInInspector] public Building targetBuilding; // 指向怪物所在的建築物
    private void Awake()
    {
        if (buttonImage == null)
            buttonImage = GetComponent<Image>();

        if (buttonImage == null)
            Debug.LogWarning($"[CostumeButton] {name} 的 buttonImage 是 null！");

        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OnClick);
        }
    }

    /// <summary>
    /// 按下服飾按鈕，通知面板並換裝
    /// </summary>
    public void OnClick()
    {
        // 通知面板處理選取狀態
        panelManager?.OnClickCostumeButton(this);

        // 直接呼叫 Building 換裝
        if (targetBuilding != null && currentCostume != null)
        {
            targetBuilding.EquipCostume(currentCostume.costumeID);
        }
    }

   

    /// <summary>
    /// 設定按鈕對應服飾資料
    /// </summary>
    public void SetCostume(CostumeData data)
    {
        currentCostume = data;

        if (buttonImage == null)
            buttonImage = GetComponent<Image>();

        if (buttonImage != null && data != null)
            buttonImage.sprite = data.costumeImage;

        // 強制顯示按鈕
        gameObject.SetActive(true);
    }

    /// <summary>
    /// 設定按鈕是否選中（UI 顏色）
    /// </summary>
    public void SetSelected(bool selected)
    {
        isSelected = selected;
        if (buttonImage != null)
            buttonImage.color = selected ? Color.gray : Color.white;
    }
}

