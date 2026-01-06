using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DecorationWorldManager : MonoBehaviour
{
    public static DecorationWorldManager Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public GameObject SpawnDecoration(GameObject prefab)
    {
        if (prefab == null) return null;

        Vector3 spawnPos = Camera.main.ScreenToWorldPoint(
            new Vector3(Screen.width / 2, Screen.height / 2, 10f)
        );
        spawnPos.z = 0f;

        GameObject obj = Instantiate(prefab, spawnPos, Quaternion.identity);
        return obj;
    }

}
