using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelBlink : MonoBehaviour
{
    public CanvasGroup panelGroup; // 拖入面板的 CanvasGroup
    public float blinkDuration = 0.5f; // 每次閃爍時間
    public int blinkCount = 5; // 閃幾次

    private void Start()
    {
        // 可選：啟動閃爍
        StartCoroutine(BlinkPanel());
    }

    public IEnumerator BlinkPanel()
    {
        for (int i = 0; i < blinkCount; i++)
        {
            panelGroup.alpha = 0f; // 隱藏
            yield return new WaitForSeconds(blinkDuration);
            panelGroup.alpha = 1f; // 顯示
            yield return new WaitForSeconds(blinkDuration);
        }
    }
}