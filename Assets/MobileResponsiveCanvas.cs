using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasScaler))]
public class MobileResponsiveCanvas : MonoBehaviour
{
    // 你指定的設計解析度
    public Vector2 referenceResolution = new Vector2(1920, 1080);

    [Range(0f, 1f)]
    public float matchWhenWider = 1f;   // 寬螢幕 → 偏 Height
    [Range(0f, 1f)]
    public float matchWhenNarrow = 0f;  // 窄螢幕 → 偏 Width

    private void Awake()
    {
        ApplyResponsive();
    }

    void ApplyResponsive()
    {
        CanvasScaler scaler = GetComponent<CanvasScaler>();

        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = referenceResolution;
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;

        float targetRatio = referenceResolution.x / referenceResolution.y;
        float currentRatio = (float)Screen.width / Screen.height;

        // 判斷手機比例
        if (currentRatio > targetRatio)
        {
            // 手機比較寬（20:9、21:9）
            scaler.matchWidthOrHeight = matchWhenWider;
        }
        else
        {
            // 手機比較窄（16:9）
            scaler.matchWidthOrHeight = matchWhenNarrow;
        }
    }
}
