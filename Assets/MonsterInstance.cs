using System.Collections;
using System.Collections.Generic;
using UnityEngine;



// ===========================================
// MonsterInstance.cs
// ===========================================


/*public class MonsterInstance : MonoBehaviour
{
    [Header("資料")]
    public MonsterData monsterData;

    [Header("目前換裝")]
    public int equippedCostumeId = -1; // -1 = 使用預設

    [Header("生成高度偏移（沿用舊版）")]
    [SerializeField] private float monsterHeightOffset = 1.0f;

    private GameObject currentMonsterGO;

    //強制測試用
    [ContextMenu("Test Equip Costume ID = 1")]
    public void TestEquipCostume1()
    {
        EquipCostume(1);
    }

    /// <summary>
    /// 初始化妖怪（第一次生成）
    /// </summary>
    public void Init(MonsterData data)
    {
        monsterData = data;
        equippedCostumeId = -1;

        // 生成原始怪物 prefab
        GameObject prefab = monsterData.GetPrefabByCostumeId(-1);
        SpawnMonster(prefab);
    }

    /// <summary>
    /// UI 呼叫：換裝
    /// </summary>
    public void EquipCostume(int costumeId)
    {
        Debug.Log($"[EquipCostume] monsterData = {monsterData}");

        equippedCostumeId = costumeId;

        if (monsterData == null)
        {
            Debug.LogWarning("MonsterData 未設定");
            return;
        }

        // 取得整個怪物 prefab（包含服飾）
        GameObject prefab = monsterData.GetPrefabByCostumeId(costumeId);
        if (prefab == null)
        {
            Debug.LogWarning($"找不到對應 prefab id={costumeId}");
            return;
        }

        // 刪掉舊 prefab
        if (currentMonsterGO != null)
        {
            Destroy(currentMonsterGO);
            currentMonsterGO = null;
        }

        // 生成新的 prefab
        Vector3 spawnPos = transform.position + Vector3.up * monsterHeightOffset;
        currentMonsterGO = Instantiate(prefab, spawnPos, Quaternion.identity, transform);
        currentMonsterGO.transform.localScale = Vector3.one * monsterData.spawnScale;

        // 啟動工作動畫
        MonsterWorkAnimation workAnim = currentMonsterGO.GetComponent<MonsterWorkAnimation>();
        if (workAnim != null)
            workAnim.StartWork();
    }

    /// <summary>
    /// 真正負責生成怪物 prefab 的地方（唯一入口）
    /// </summary>
    private void SpawnMonster(GameObject prefab)
    {
        if (prefab == null) return;

        // 先刪掉舊的 prefab
        if (currentMonsterGO != null)
        {
            Destroy(currentMonsterGO);
            currentMonsterGO = null; // 保險做法
        }

        Vector3 spawnPos = transform.position + Vector3.up * monsterHeightOffset;
        Vector3 spawnScale = Vector3.one * monsterData.spawnScale;
        Transform parent = transform;

        // 生成新的 prefab
        currentMonsterGO = Instantiate(prefab, spawnPos, Quaternion.identity, parent);
        currentMonsterGO.transform.localScale = spawnScale;

        // 啟動工作動畫
        MonsterWorkAnimation workAnim = currentMonsterGO.GetComponent<MonsterWorkAnimation>();
        if (workAnim != null)
            workAnim.StartWork();
    }

    /// <summary>
    /// 將服飾 prefab 掛到怪物的指定掛點上
    /// </summary>
    private void ApplyCostume()
    {
        if (currentMonsterGO == null) return;

        // 移除舊服飾
        foreach (Transform child in currentMonsterGO.transform)
        {
            if (child.CompareTag("Costume"))
                Destroy(child.gameObject);
        }

        if (equippedCostumeId < 0) return; // -1 = 不換服飾

        // 取得對應服飾 prefab
        GameObject costumePrefab = monsterData.GetPrefabByCostumeId(equippedCostumeId);
        if (costumePrefab == null)
        {
            Debug.LogWarning($"找不到服飾 prefab id={equippedCostumeId}");
            return;
        }

        // 讀取 CostumeComponent 指定的掛點名稱
        CostumeComponent cc = costumePrefab.GetComponent<CostumeComponent>();
        Transform anchor = currentMonsterGO.transform;
        if (cc != null && !string.IsNullOrEmpty(cc.anchorName))
        {
            Transform t = currentMonsterGO.transform.Find(cc.anchorName);
            if (t != null)
                anchor = t;
            else
                Debug.LogWarning($"找不到掛點 {cc.anchorName} 在 {monsterData.monsterName}");
        }

        // 生成服飾
        GameObject costumeGO = Instantiate(costumePrefab, anchor);
        costumeGO.transform.localPosition = Vector3.zero;
        costumeGO.transform.localRotation = Quaternion.identity;
        costumeGO.transform.localScale = Vector3.one;
        costumeGO.tag = "Costume"; // 下次換裝可以清理
    }
}*/

public class MonsterInstance : MonoBehaviour
{
    public MonsterData monsterData;
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
    public void Init(MonsterData data)
    {
        monsterData = data;
        equippedCostumeId = -1;
    }
}

