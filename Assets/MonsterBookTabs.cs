using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MonsterBookTabs : MonoBehaviour
{
    public GameObject pageLv1;
    public GameObject pageLv2;
    public GameObject pageLv3;

    [Header("分頁按鈕")]
    public Button btnLv1;
    public Button btnLv2;
    public Button btnLv3;

    [Header("選中顏色加深參數")]
    [Range(0f, 1f)] public float darkenFactor = 0.3f; // 0:不加深, 1:全黑

    // 儲存按鈕原始顏色
    private Color originalColorLv1;
    private Color originalColorLv2;
    private Color originalColorLv3;

    private void Awake()
    {
        originalColorLv1 = btnLv1.image.color;
        originalColorLv2 = btnLv2.image.color;
        originalColorLv3 = btnLv3.image.color;
    }
    private void Start()
    {
        ShowLv1(); // 預設顯示
    }

    public void ShowLv1()
    {
        pageLv1.SetActive(true);
        pageLv2.SetActive(false);
        pageLv3.SetActive(false);
        UpdateTabColor(btnLv1);

    }

    public void ShowLv2()
    {
        pageLv1.SetActive(false);
        pageLv2.SetActive(true);
        pageLv3.SetActive(false);
        UpdateTabColor(btnLv2);

    }

    public void ShowLv3()
    {
        pageLv1.SetActive(false);
        pageLv2.SetActive(false);
        pageLv3.SetActive(true);
        UpdateTabColor(btnLv3);

    }
   private void UpdateTabColor(Button selectedBtn)
    {
        // 將按鈕恢復原始顏色
        btnLv1.image.color = originalColorLv1;
        btnLv2.image.color = originalColorLv2;
        btnLv3.image.color = originalColorLv3;

        // 選中按鈕加深
        Color selectedColor = selectedBtn.image.color * (1f - darkenFactor);
        selectedBtn.image.color = selectedColor;
    }

    public void OpenPanel()
    {
        gameObject.SetActive(true);
    }
    public void ClosePanel()
    {
        gameObject.SetActive(false);
    }
}

