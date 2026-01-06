using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*public class DecorationScaleButton : MonoBehaviour
{
    public DecorationWorldObject targetDecoration;

    [Header("Scale Settings")]
    public float scaleSpeed = 0.005f;
    public float minScale = 0.3f;
    public float maxScale = 3.0f;

    private bool isScaling = false;
    private float startMouseY;
    private Vector3 startScale;

    private void OnMouseDown()
    {
        Debug.Log(" ScaleButton OnMouseDown 被點到了");

        if (targetDecoration == null)
        {
            Debug.Log(" targetDecoration 是 null");
            return;
        }
        isScaling = true;
        startMouseY = Input.mousePosition.y;
        startScale = targetDecoration.transform.localScale;

        // 告知裝飾物：我正在被操作
        targetDecoration.SetInteracting(true);
    }

    private void OnMouseDrag()
    {
        if (!isScaling || targetDecoration == null) return;

        float deltaY = Input.mousePosition.y - startMouseY;
        float scaleFactor = 1f + deltaY * scaleSpeed;

        float clampedScale = Mathf.Clamp(
            startScale.x * scaleFactor,
            minScale,
            maxScale
        );

        targetDecoration.transform.localScale = Vector3.one * clampedScale;
    }

    private void OnMouseUp()
    {
        isScaling = false;

        if (targetDecoration != null)
            targetDecoration.SetInteracting(false);
    }
}*/

public class DecorationScaleButton : MonoBehaviour
{
    public DecorationWorldObject targetDecoration;

    [Header("Scale Settings")]
    public float scaleSpeed = 0.005f;   // 縮放靈敏度
    public float minScale = 0.3f;
    public float maxScale = 3.0f;

    private bool isScaling = false;
    private float startMouseY;

    private Vector3 prefabOriginalScale;   // 記錄 prefab 原始 localScale
    private float originalAspect;          // 寬高比

    private void OnMouseDown()
    {
        if (targetDecoration == null) return;

        isScaling = true;
        startMouseY = Input.mousePosition.y;

        //告知裝飾：我正在被操作
        targetDecoration.SetInteracting(true);

        // 取得 prefab 原始比例
        SpriteRenderer sr = targetDecoration.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            prefabOriginalScale = targetDecoration.transform.localScale;
            originalAspect = sr.bounds.size.x / sr.bounds.size.y;
        }
        else
        {
            // fallback: 等比例 1:1
            prefabOriginalScale = targetDecoration.transform.localScale;
            originalAspect = 1f;
        }
    }

    private void OnMouseDrag()
    {
        if (!isScaling || targetDecoration == null) return;

        float deltaY = Input.mousePosition.y - startMouseY;

        // 計算縮放倍率
        float scaleFactor = 1 + (deltaY * scaleSpeed);

        // 限制縮放倍率
        float newScaleY = Mathf.Clamp(prefabOriginalScale.y * scaleFactor, minScale, maxScale);
        float newScaleX = newScaleY * originalAspect; // 維持原始寬高比

        targetDecoration.transform.localScale = new Vector3(newScaleX, newScaleY, targetDecoration.transform.localScale.z);
    }

    private void OnMouseUp()
    {
        isScaling = false;

        // 操作結束，一定要解除
        targetDecoration.SetInteracting(false);
    }
}
