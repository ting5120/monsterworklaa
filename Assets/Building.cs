using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;






//原
/*public class Building : MonoBehaviour
{
    public BuildingData data;               // 建築資料
    public PanelType panelType;             // Panel 類型
    public MonsterData recruitedMonster;    // 招募的妖怪
    public Transform monsterSpawnPoint;     // 怪物生成點

    //[Header("妖怪生成位置微調")]
    //[Tooltip("妖怪生成後往下的偏移量（local Y）")]
    //public float monsterYOffset = -20f;

    [Header("壞妖怪煙霧特效")]
    public GameObject badMonsterSmokePrefab;
    [HideInInspector] public GameObject badMonsterSmokeInstance; // 存放當前壞妖怪煙霧
    [Tooltip("壞妖怪煙霧延遲生成時間（秒）")]
    public float badMonsterSmokeDelay = 10f;  // 可在 Inspector 調整延遲秒數

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

        // 刪掉舊怪物
        if (currentMonsterGO != null)
            Destroy(currentMonsterGO);

        // 刪掉舊煙霧
        ClearBadMonsterSmoke();

        // 生成新的怪物
        GameObject monsterGO = Instantiate(
            data.monsterPrefab,
            monsterSpawnPoint.position,
            Quaternion.identity,
            monsterSpawnPoint
        );

        // 套用每隻妖怪 prefab 的生成偏移
        if (data.spawnOffset != Vector3.zero)
            monsterGO.transform.localPosition += data.spawnOffset;

        //  關鍵：生成後往下偏移（使用 localPosition）
        //monsterGO.transform.localPosition += Vector3.up * monsterYOffset;

        currentMonsterGO = monsterGO;

        // 取得 MonsterInstance 並初始化資料
        MonsterInstance mi = monsterGO.GetComponent<MonsterInstance>();
        if (mi != null)
        {
            mi.Init(data, data.alignment);
            monsterInstance = mi;

            // 如果是壞妖怪，延遲生成煙霧
            if (mi.alignment == MonsterAlignment.Bad)
            {
                //SpawnBadMonsterSmoke();
                SpawnBadMonsterSmoke(data.spawnOffset);

            }
        }
        else
        {
            Debug.LogError("[Building] 怪物 prefab 上沒有 MonsterInstance");
        }

        OnMonsterSpawned?.Invoke(monsterInstance);
    }

    /// <summary>
    /// 換裝：直接換掉整隻 prefab
    /// </summary>
    /// <param name="costumeId">服飾 ID</param>
    public void EquipCostume(int costumeId)
    {
        if (recruitedMonster == null) return;

        GameObject newPrefab = recruitedMonster.GetPrefabByCostumeId(costumeId);
        if (newPrefab == null)
        {
            Debug.LogWarning($"找不到服飾 prefab id={costumeId}");
            return;
        }

        // 刪掉舊怪物
        if (currentMonsterGO != null)
            Destroy(currentMonsterGO);

        // 刪掉舊煙霧
        ClearBadMonsterSmoke();

        // 生成新 prefab
        GameObject monsterGO = Instantiate(
            newPrefab,
            monsterSpawnPoint.position,
            Quaternion.identity,
            monsterSpawnPoint
        );

        //  同樣套用往下偏移
        //monsterGO.transform.localPosition += Vector3.up * monsterYOffset;

        // 套用每隻妖怪 prefab 的生成偏移
        if (recruitedMonster.spawnOffset != Vector3.zero)
            monsterGO.transform.localPosition += recruitedMonster.spawnOffset;


        currentMonsterGO = monsterGO;

        MonsterInstance mi = monsterGO.GetComponent<MonsterInstance>();
        if (mi != null)
        {
            mi.Init(recruitedMonster, recruitedMonster.alignment);
            monsterInstance = mi;

            // 如果是壞妖怪，延遲生成煙霧
            if (mi.alignment == MonsterAlignment.Bad)
            {
                //SpawnBadMonsterSmoke();
                SpawnBadMonsterSmoke(recruitedMonster.spawnOffset);

            }
        }
        else
        {
            Debug.LogError("[Building] 服飾 prefab 上沒有 MonsterInstance");
        }

        OnMonsterSpawned?.Invoke(monsterInstance);
    }

    

    private void SpawnBadMonsterSmoke(Vector3 spawnOffset)
    {
        if (badMonsterSmokePrefab == null || monsterSpawnPoint == null) return;

        // 停止之前的 Coroutine（避免多個煙霧同時生成）
        StopCoroutine("SpawnBadMonsterSmokeCoroutine");
        StartCoroutine(SpawnBadMonsterSmokeCoroutine(spawnOffset));
    }
    /// <summary>
    /// Coroutine：延遲生成壞妖怪煙霧
    /// </summary>

    private IEnumerator SpawnBadMonsterSmokeCoroutine(Vector3 spawnOffset)
    {
        yield return new WaitForSeconds(badMonsterSmokeDelay);

        if (monsterSpawnPoint == null) yield break;

        badMonsterSmokeInstance = Instantiate(
            badMonsterSmokePrefab,
            monsterSpawnPoint.position,
            Quaternion.identity,
            monsterSpawnPoint
        );

        // 套用同樣偏移
        if (spawnOffset != Vector3.zero)
            badMonsterSmokeInstance.transform.localPosition += spawnOffset;
    }

    /// <summary>
    /// 清除壞妖怪煙霧
    /// </summary>
    public void ClearBadMonsterSmoke()
    {
        // 停止延遲生成 Coroutine
        StopCoroutine("SpawnBadMonsterSmokeCoroutine");

        if (badMonsterSmokeInstance != null)
        {
            Destroy(badMonsterSmokeInstance);
            badMonsterSmokeInstance = null;
        }
    }
}*/



public class Building : MonoBehaviour
{
    public BuildingData data;               // 建築資料
    public PanelType panelType;             // Panel 類型
    public MonsterData recruitedMonster;    // 招募的妖怪
    public Transform monsterSpawnPoint;     // 怪物生成點

    [Header("壞妖怪煙霧特效")]
    public GameObject badMonsterSmokePrefab;
    [HideInInspector] public GameObject badMonsterSmokeInstance; // 存放當前壞妖怪煙霧
    [Tooltip("壞妖怪煙霧延遲生成時間（秒）")]
    public float badMonsterSmokeDelay = 10f;  // 可在 Inspector 調整延遲秒數

    [HideInInspector]
    public int currentLevel = 1; // 預設從 1 級開始

    [HideInInspector] public CostumeData equippedCostume; // 目前裝備的服飾
    [HideInInspector] public GameObject currentMonsterGO; // 目前場上怪物
    public MonsterInstance monsterInstance;  // 指向生成的 MonsterInstance
    private MonsterWorkAnimation monsterWorkAnim; // 怪物工作動畫

    private Coroutine badSmokeRoutine; // 新增：保存正在運行的煙霧 Coroutine

    public event System.Action<MonsterInstance> OnMonsterSpawned;

    /// <summary>
    /// 生成完整怪物 prefab（含服飾）
    /// </summary>
    public void SpawnMonster(MonsterData data)
    {
        if (data == null || monsterSpawnPoint == null)
            return;

        // 刪掉舊怪物
        if (currentMonsterGO != null)
            Destroy(currentMonsterGO);

        // 刪掉舊煙霧
        ClearBadMonsterSmoke();

        // 生成新的怪物
        GameObject monsterGO = Instantiate(
            data.monsterPrefab,
            monsterSpawnPoint.position,
            Quaternion.identity,
            monsterSpawnPoint
        );

        // 套用每隻妖怪 prefab 的生成偏移
        if (data.spawnOffset != Vector3.zero)
            monsterGO.transform.localPosition += data.spawnOffset;

        currentMonsterGO = monsterGO;

        // 取得 MonsterInstance 並初始化資料
        MonsterInstance mi = monsterGO.GetComponent<MonsterInstance>();
        if (mi != null)
        {
            mi.Init(data, data.alignment);
            monsterInstance = mi;

            // 如果是壞妖怪，延遲生成煙霧
            if (mi.alignment == MonsterAlignment.Bad)
            {
                SpawnBadMonsterSmoke(data.spawnOffset);
            }
        }
        else
        {
            Debug.LogError("[Building] 怪物 prefab 上沒有 MonsterInstance");
        }

        // 新增：抓 MonsterWorkAnimation 並開始工作
        monsterWorkAnim = monsterGO.GetComponent<MonsterWorkAnimation>();
        if (monsterWorkAnim != null)
            monsterWorkAnim.StartWork();

        OnMonsterSpawned?.Invoke(monsterInstance);
    }

    /// <summary>
    /// 換裝：直接換掉整隻 prefab
    /// </summary>
    public void EquipCostume(int costumeId)
    {
        if (recruitedMonster == null) return;

        GameObject newPrefab = recruitedMonster.GetPrefabByCostumeId(costumeId);
        if (newPrefab == null)
        {
            Debug.LogWarning($"找不到服飾 prefab id={costumeId}");
            return;
        }

        // 刪掉舊怪物
        if (currentMonsterGO != null)
            Destroy(currentMonsterGO);

        // 刪掉舊煙霧
        ClearBadMonsterSmoke();

        // 生成新 prefab
        GameObject monsterGO = Instantiate(
            newPrefab,
            monsterSpawnPoint.position,
            Quaternion.identity,
            monsterSpawnPoint
        );

        // 套用每隻妖怪 prefab 的生成偏移
        if (recruitedMonster.spawnOffset != Vector3.zero)
            monsterGO.transform.localPosition += recruitedMonster.spawnOffset;

        currentMonsterGO = monsterGO;

        MonsterInstance mi = monsterGO.GetComponent<MonsterInstance>();
        if (mi != null)
        {
            mi.Init(recruitedMonster, recruitedMonster.alignment);
            monsterInstance = mi;

            // 如果是壞妖怪，延遲生成煙霧
            if (mi.alignment == MonsterAlignment.Bad)
            {
                SpawnBadMonsterSmoke(recruitedMonster.spawnOffset);
            }
        }
        else
        {
            Debug.LogError("[Building] 服飾 prefab 上沒有 MonsterInstance");
        }

        // 新增：抓 MonsterWorkAnimation 並開始工作
        monsterWorkAnim = monsterGO.GetComponent<MonsterWorkAnimation>();
        if (monsterWorkAnim != null)
            monsterWorkAnim.StartWork();

        OnMonsterSpawned?.Invoke(monsterInstance);
    }

    private void SpawnBadMonsterSmoke(Vector3 spawnOffset)
    {
        if (badMonsterSmokePrefab == null || monsterSpawnPoint == null) return;

        // 停止舊 Coroutine
        if (badSmokeRoutine != null)
        {
            StopCoroutine(badSmokeRoutine);
            badSmokeRoutine = null;
        }

        // 啟動新的 Coroutine
        badSmokeRoutine = StartCoroutine(SpawnBadMonsterSmokeCoroutine(spawnOffset));
    }

    private IEnumerator SpawnBadMonsterSmokeCoroutine(Vector3 spawnOffset)
    {
        yield return new WaitForSeconds(badMonsterSmokeDelay);

        if (monsterSpawnPoint == null) yield break;

        badMonsterSmokeInstance = Instantiate(
            badMonsterSmokePrefab,
            monsterSpawnPoint.position,
            Quaternion.identity,
            monsterSpawnPoint
        );

        if (spawnOffset != Vector3.zero)
            badMonsterSmokeInstance.transform.localPosition += spawnOffset;

        badSmokeRoutine = null; // Coroutine 完成後清空引用

    }

    public void ClearBadMonsterSmoke()
    {
        // 停止延遲生成 Coroutine
        if (badSmokeRoutine != null)
        {
            StopCoroutine(badSmokeRoutine);
            badSmokeRoutine = null;
        }

        if (badMonsterSmokeInstance != null)
        {
            Destroy(badMonsterSmokeInstance);
            badMonsterSmokeInstance = null;
        }
    }
}
