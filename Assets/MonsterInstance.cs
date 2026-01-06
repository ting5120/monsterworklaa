using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;



// ===========================================
// MonsterInstance.cs
// ===========================================



public class MonsterInstance : MonoBehaviour
{
    public MonsterData monsterData;
    public MonsterAlignment alignment; // 新增：招募時決定
    public int equippedCostumeId = -1;

    // 已存在的怪物本體
    private GameObject monsterRoot;

    private void Awake()
    {
        monsterRoot = gameObject;
    }

    /// <summary>
    /// 初始化妖怪資料
    /// </summary>
    public void Init(MonsterData data, MonsterAlignment chosenAlignment)
    {
        monsterData = data;
        alignment = chosenAlignment;          // 記錄這個 Instance 的好壞
        monsterData.alignment = chosenAlignment; // 套用好壞到資料
        equippedCostumeId = -1;
    }

}

