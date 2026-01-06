using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

/*public class OpeningController : MonoBehaviour
{
    [Header("Assign in Inspector")]
    public GameObject openingPanel;
    public VideoPlayer openingVideo;

    public GameObject canvasO;
    public GameObject canvas1;
    public GameObject canvas2;

    private GameObject[] mainButtons; // 儲存主畫面按鈕物件

    void Start()
    {
        // 抓取主畫面 Canvas 下所有按鈕（包含 inactive）
        Button[] btns = canvas2.GetComponentsInChildren<Button>(true);
        mainButtons = new GameObject[btns.Length];
        for (int i = 0; i < btns.Length; i++)
        {
            mainButtons[i] = btns[i].gameObject;
            mainButtons[i].SetActive(false); // 一開始完全隱藏
        }

        // 開場 Canvas 顯示
        canvasO.SetActive(true);
        canvas1.SetActive(true);  // Canvas 本身可見，但按鈕隱藏
        canvas2.SetActive(true);
    }

    // 點擊 Panel 呼叫
    public void StartGame()
    {
        // 停止影片
        if (openingVideo != null)
        {
            openingVideo.Stop();
            openingVideo.gameObject.SetActive(false);
        }

        // 隱藏開場 Panel / Canvas
        if (openingPanel != null)
            openingPanel.SetActive(false);

        

        // 正式開始 → 顯示主畫面按鈕
        foreach (var btn in mainButtons)
        {
            btn.SetActive(true);
        }

        // 切換到正式遊戲場景
        SceneManager.LoadScene("MainScene"); // 替換成你的主遊戲場景名稱
    }
}*/
