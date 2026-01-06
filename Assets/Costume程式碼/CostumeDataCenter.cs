using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ===========================
// 資料中心，統一管理已購服飾與裝備
// ===========================

public class CostumeDataCenter : MonoBehaviour
{
    public static CostumeDataCenter Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    [Header("已擁有服飾")]
    public List<CostumeData> ownedCostumes = new List<CostumeData>();

    public void AddCostume(CostumeData data)
    {
        if (data == null || ownedCostumes.Contains(data)) return;
        ownedCostumes.Add(data);
    }

    public bool IsOwned(CostumeData data)
    {
        return data != null && ownedCostumes.Contains(data);
    }

    
}
