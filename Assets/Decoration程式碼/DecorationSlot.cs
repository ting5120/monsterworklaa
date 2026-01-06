using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;  
using UnityEngine.UI;

public class DecorationSlot : MonoBehaviour
{
    public DecorationData decoration;

    public Button backpackButton;
    public Image backpackImage;
    public TMP_Text countText;
    public Sprite normalIcon;
    public Sprite emptyIcon;

    private int count = 0;


    [Header("World Decoration")]
    public GameObject worldPrefab; // 對應的世界裝飾 prefab

    void Start()
    {
        RefreshUI();
    }

    public void AddOne()
    {
        count++;
        RefreshUI();
    }

    void RefreshUI()
    {
        countText.text = count.ToString();

        bool hasAny = count > 0;
        backpackButton.interactable = hasAny;
        //backpackImage.sprite = hasAny ? normalIcon : emptyIcon;
        // 切換圖示
        if (backpackImage != null)
            backpackImage.sprite = hasAny ? normalIcon : emptyIcon;
    }

    public void SpawnToWorld()
    {
        if (count <= 0) return;

        count--;
        RefreshUI();

        if (DecorationWorldManager.Instance != null)
        {
            GameObject obj = DecorationWorldManager.Instance.SpawnDecoration(worldPrefab);

            if (obj != null)
            {
                DecorationWorldObject worldObj =
                    obj.GetComponent<DecorationWorldObject>();

                if (worldObj != null)
                {
                    worldObj.ownerSlot = this; // 關鍵關聯
                }
            }
        }
    }


}

