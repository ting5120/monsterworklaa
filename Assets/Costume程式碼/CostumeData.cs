using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    fileName = "New Costume",
    menuName = "Costume/Costume Data"
)]
public class CostumeData : ScriptableObject
{
    [Header("基本資訊")]
    public int costumeID;     // 唯一ID
    public string costumeName;
    public int price;

    [Header("服飾圖片")]
    public Sprite costumeImage;  // 用於換裝面板按鈕顯示

  
}
