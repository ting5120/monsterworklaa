using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StreetManager : MonoBehaviour
{
    [Header("Ghost Settings")]
    public GameObject ghostPrefab;
    public Transform ghostParent;

    [Header("Street Range")]
    public float streetY;
    public float leftX;
    public float rightX;

    [Header("Spawn Control")]
    public int maxGhost = 5;
    public float spawnIntervalMin = 2f;
    public float spawnIntervalMax = 4f;

    List<GhostController> ghosts = new();
    bool isActive = false;

    Coroutine spawnRoutine;

    void OnEnable()
    {
        // 街道被啟用（切到這條街）
        ActivateStreet();
    }

    void OnDisable()
    {
        // 街道被關閉（切走）
        DeactivateStreet();
    }

    // =========================
    // 街道狀態控制
    // =========================

    public void ActivateStreet()
    {
        if (isActive) return;

        isActive = true;

        // 讓現有鬼火繼續
        foreach (var g in ghosts)
        {
            if (g != null && g.gameObject.activeSelf)
                g.FadeInAndResume();
        }

        // 啟動生成流程
        if (spawnRoutine != null)
            StopCoroutine(spawnRoutine);

        spawnRoutine = StartCoroutine(SpawnLoop());
    }

    public void DeactivateStreet()
    {
        if (!isActive) return;

        isActive = false;

        // 停止生成
        if (spawnRoutine != null)
            StopCoroutine(spawnRoutine);

        // 讓現有鬼火淡出暫停
        foreach (var g in ghosts)
        {
            if (g != null && g.gameObject.activeSelf)
                g.FadeOutAndPause();
        }
    }

    // =========================
    // 生怪流程
    // =========================

    IEnumerator SpawnLoop()
    {
        while (isActive)
        {
            CleanGhostList();

            if (ghosts.Count < maxGhost)
            {
                SpawnGhost();
            }

            yield return new WaitForSeconds(
                Random.Range(spawnIntervalMin, spawnIntervalMax)
            );
        }
    }

    void SpawnGhost()
    {
        float x = Random.Range(leftX, rightX);
        Vector3 pos = new Vector3(x, streetY, 0);

        GameObject g = Instantiate(
            ghostPrefab,
            pos,
            Quaternion.identity,
            ghostParent
        );

        GhostController gc = g.GetComponent<GhostController>();

        // ⭐ 關鍵：只呼叫一次 Setup
        gc.Setup(leftX, rightX);

        ghosts.Add(gc);
    }

    // =========================
    // 清理失效鬼火
    // =========================

    void CleanGhostList()
    {
        // 移除已被 SetActive(false) 的 ghost
        ghosts.RemoveAll(g =>
            g == null || !g.gameObject.activeSelf
        );
    }
}


