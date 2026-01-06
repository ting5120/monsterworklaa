using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PanelController : MonoBehaviour
{
    [Header("要控制的面板")]
    public GameObject panel;

    // 開啟面板
    public void OpenPanel()
    {
        if (panel != null)
            panel.SetActive(true);
    }

    // 關閉面板
    public void ClosePanel()
    {
        if (panel != null)
            panel.SetActive(false);
    }
}
