using UnityEngine;

public class CameraLetterboxWithUI : MonoBehaviour
{
    public float targetAspect = 16f / 9f; // 1920x1080
    public Canvas[] affectedCanvases;

    void Start()
    {
        ApplyLetterbox();
    }

    void ApplyLetterbox()
    {
        Camera cam = GetComponent<Camera>();

        cam.backgroundColor = Color.black;
        cam.clearFlags = CameraClearFlags.SolidColor;

        float screenAspect = (float)Screen.width / Screen.height;
        float scaleHeight = screenAspect / targetAspect;

        Rect rect = new Rect();

        if (scaleHeight < 1f)
        {
            // 上下黑邊
            rect.width = 1f;
            rect.height = scaleHeight;
            rect.x = 0;
            rect.y = (1f - scaleHeight) / 2f;
        }
        else
        {
            // 左右黑邊（2160x1080 會走這裡）
            float scaleWidth = 1f / scaleHeight;
            rect.width = scaleWidth;
            rect.height = 1f;
            rect.x = (1f - scaleWidth) / 2f;
            rect.y = 0;
        }

        cam.rect = rect;

        // ⭐ 關鍵：同步 UI Canvas
        foreach (Canvas canvas in affectedCanvases)
        {
            if (canvas == null) continue;

            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = cam;
            canvas.pixelPerfect = true;

            RectTransform rt = canvas.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
        }
    }
}
