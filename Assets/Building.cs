using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class Building : MonoBehaviour
{
    public BuildingData data;               // 建築資料
    public PanelType panelType;             // Panel 類型
    public MonsterData recruitedMonster;    // 招募的妖怪
    public Transform monsterSpawnPoint;     // 怪物生成點

    [HideInInspector]
    public int currentLevel = 1; // 預設從 1 級開始

    [HideInInspector] public CostumeData equippedCostume; // 目前裝備的服飾

    [HideInInspector] public GameObject currentMonsterGO; // 目前場上怪物
    public MonsterInstance monsterInstance;  // 指向生成的 MonsterInstance

    public event System.Action<MonsterInstance> OnMonsterSpawned;

    /// <summary>
    /// 生成完整怪物 prefab（含服飾）
    /// </summary>
    public void SpawnMonster(MonsterData data)
    {
        if (data == null || monsterSpawnPoint == null)
            return;

        // 刪掉舊的 prefab
        if (currentMonsterGO != null)
            Destroy(currentMonsterGO);

       
        // 生成新的完整 prefab
        GameObject monsterGO = Instantiate(
            data.monsterPrefab,
            monsterSpawnPoint.position,
            Quaternion.identity,
            monsterSpawnPoint   // 掛在 spawnPoint 底下
        );

        currentMonsterGO = monsterGO;

        // 取得 MonsterInstance 並初始化資料
        MonsterInstance mi = monsterGO.GetComponent<MonsterInstance>();
        if (mi != null)
        {
            mi.Init(data);
            monsterInstance = mi;
        }
        else
        {
            Debug.LogError("[Building] 怪物 prefab 上沒有 MonsterInstance");
        }

        // 生成完成通知
        OnMonsterSpawned?.Invoke(monsterInstance);
    }

    /// <summary>
    /// 換裝：直接換掉整隻 prefab
    /// </summary>
    /// <param name="costumeId">服飾 ID</param>
    public void EquipCostume(int costumeId)
    {
        if (recruitedMonster == null) return;

        // 找對應服飾 prefab
        GameObject newPrefab = recruitedMonster.GetPrefabByCostumeId(costumeId);
        if (newPrefab == null)
        {
            Debug.LogWarning($"找不到服飾 prefab id={costumeId}");
            return;
        }

        

        // 生成新 prefab 替換舊的（保持 MonsterData）
        if (currentMonsterGO != null)
            Destroy(currentMonsterGO);

        
        GameObject monsterGO = Instantiate(
            newPrefab,
            monsterSpawnPoint.position,
            Quaternion.identity,
            monsterSpawnPoint
        );

        currentMonsterGO = monsterGO;

        MonsterInstance mi = monsterGO.GetComponent<MonsterInstance>();
        if (mi != null)
        {
            // 保持原本的 recruitedMonster 資料
            mi.Init(recruitedMonster);
            monsterInstance = mi;
        }
        else
        {
            Debug.LogError("[Building] 服飾 prefab 上沒有 MonsterInstance");
        }

        // 更新事件
        OnMonsterSpawned?.Invoke(monsterInstance);
    }
}
