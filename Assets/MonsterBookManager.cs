using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MonsterBookManager : MonoBehaviour
{
    public static MonsterBookManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
    }

    // ----------------------------
    // 已解鎖妖怪 ID
    // ----------------------------
    private HashSet<int> unlockedMonsterIDs = new HashSet<int>();

    // ----------------------------
    // 對應按鈕列表
    // ----------------------------
    [System.Serializable]
    public class MonsterButtonSlot
    {
        public int monsterID;     // 對應 MonsterData 的 ID
        public Button button;     // 圖鑑上的按鈕
        public Image buttonImage; // Button 圖片
        public Sprite unlockedSprite; // 解鎖後圖片
        public Sprite lockedSprite;   // 未解鎖剪影
    }

    public List<MonsterButtonSlot> monsterSlots;

    [Header("圖鑑用妖怪資料（完整）")]
    public List<MonsterData> allMonsterData;


    // 解鎖妖怪
    public void UnlockMonster(int monsterID)
    {
        // 如果已經解鎖過就不做
        if (unlockedMonsterIDs.Contains(monsterID))
            return;

        // 加入已解鎖清單
        unlockedMonsterIDs.Add(monsterID);

        // 找到對應按鈕
        foreach (var slot in monsterSlots)
        {
            if (slot.monsterID == monsterID)
            {
                // 換成解鎖圖片
                slot.buttonImage.sprite = slot.unlockedSprite;

                // 設置可點擊
                slot.button.interactable = true;

                // 設定點擊事件
                slot.button.onClick.RemoveAllListeners();
                slot.button.onClick.AddListener(() => OnClickMonster(slot.monsterID));

                break;
            }
        }
    }

    public void OnClickMonster(int monsterID)
    {
        Debug.Log($"[MonsterBookManager] 點擊妖怪按鈕，ID = {monsterID}");

        // 找到對應的 MonsterData
        MonsterData data = allMonsterData.Find(m => m.ID == monsterID);
        if (data == null)
        {
            Debug.LogWarning($"[MonsterBookManager] 找不到 MonsterData，ID={monsterID}");
            return;
        }

        // 檢查資訊面板 Instance
        if (MonsterInfoPanelController.Instance == null)
        {
            Debug.LogError("[MonsterBookManager] MonsterInfoPanelController.Instance 為 null");
            return;
        }

        // 呼叫資訊面板顯示
        MonsterInfoPanelController.Instance.ShowMonster(data);
    }

}
