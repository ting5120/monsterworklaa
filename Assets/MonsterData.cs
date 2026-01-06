using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// ===== enum 放在這裡（不在 class 裡）=====
public enum MonsterLevel
{
    Normal,
    Rare,
    Legendary
}

public enum MonsterAlignment   // 好 / 壞（隱性）
{
    Good,
    Bad
}

[CreateAssetMenu(fileName = "MonsterData", menuName = "Monsters/MonsterData")]
public class MonsterData : ScriptableObject
{
    [Header("基本資訊")]
    public string monsterName;

    [Header("識別")]
    public int ID; // 唯一識別碼

    [TextArea]
    public string description;

    [Header("隱性分類（系統用）")]
    public MonsterLevel level;              //  用 enum 存等級
    public MonsterAlignment alignment;      // 好 / 壞（隱性）

    [Header("數值")]
    public float workEfficiency;
    public float costumeEfficiency;

    [Header("圖片")]
    public Sprite monsterImage;
    public Sprite borderImage;//招募邊框
    public Sprite lockedImage;   // 未解鎖剪影
    public Sprite unlockedImage; // 解鎖後圖片
    public Sprite frameImage;//圖鑑邊框


    [Header("生成物件")]
    public GameObject monsterPrefab; // 預設圖片 prefab，後續換動畫

    [Header("生成設定")]
    //public float spawnScale = 1f;       // 生成大小倍率，1 = 原始大小
    //public Vector3 spawnOffset = new Vector3(0, 1f, 0); // 生成偏移量，相對於建築物
    public Vector3 spawnOffset;

    [Header("服飾 Prefab 對照表")]
    public List<MonsterCostumePrefab> costumePrefabs;

    [HideInInspector]
    public bool isUnlocked = false; // 系統用，是否已解鎖

    [System.Serializable]
    public class MonsterCostumePrefab
    {
        public int costumeID;                 // 對應 CostumeData.id
        public GameObject costumePrefab;      // 穿上這件服飾的妖怪 prefab
    }


    /// <summary>
    /// 取得對應服飾的 prefab
    /// </summary>
    /// <param name="costumeId"></param>
    /// <returns></returns>
    public GameObject GetPrefabByCostumeId(int costumeId)
    {
        if (costumeId < 0) return monsterPrefab; // -1 = 預設

        if (costumePrefabs != null)
        {
            foreach (var entry in costumePrefabs)
            {
                if (entry.costumeID == costumeId)
                    return entry.costumePrefab;
            }
        }

        // 找不到對應服飾，就回傳預設
        return monsterPrefab;
    }

    //收益系統
    public float GetLevelMultiplier()
        {
            switch (level)
            {
                case MonsterLevel.Normal: return 1.15f;
                case MonsterLevel.Rare: return 1.4f;
                case MonsterLevel.Legendary: return 2f;
                default: return 1f;
            }
        }

        public float GetAlignmentMultiplier()
        {
            if (alignment == MonsterAlignment.Good) return 1f;

            switch (level)
            {
                case MonsterLevel.Normal: return 0.3f;
                case MonsterLevel.Rare: return 0.6f;
                case MonsterLevel.Legendary: return 0.8f;
                default: return 1f;
            }
        }
    

}