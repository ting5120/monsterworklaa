using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CostumeOwnershipManager : MonoBehaviour //全局 玩家買過哪些服飾
{
    public static CostumeOwnershipManager Instance { get; private set; }

    // 已擁有的服飾（用 costumeId）
    private HashSet<int> ownedCostumes = new HashSet<int>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    // 是否已擁有
    public bool IsOwned(int costumeID)
    {
        return ownedCostumes.Contains(costumeID);
    }

    public void AddCostume(int costumeID)
    {
        if (!ownedCostumes.Contains(costumeID))
        {
            ownedCostumes.Add(costumeID);
        }
    }
}
