using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Game/Decoration")]
public class DecorationData : ScriptableObject
{
    [Header("識別")]
    public string id;            // 例如 "tree", "bench"

    [Header("UI")]
    public Sprite icon;          // 商店 & 背包顯示用

    [Header("世界物件")]
    public GameObject worldPrefab; // 擺設時生成的物件

    public int price;

}

