using UnityEngine;
using System.Collections;

public class GhostController : MonoBehaviour
{
    SpriteRenderer sr;

    float centerX;        // 出生中心點
    float moveRange = 0.6f; // 小範圍半徑（可調）

    Coroutine behaviorRoutine;
    bool isPaused = false;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    // ⭐ 由 StreetManager 呼叫一次
    public void Setup(float leftX, float rightX)
    {
        // 出生點隨機
        centerX = Random.Range(leftX, rightX);

        transform.position = new Vector3(
            centerX,
            transform.position.y,
            transform.position.z
        );
    }

    void OnEnable()
    {
        if (behaviorRoutine != null)
            StopCoroutine(behaviorRoutine);

        behaviorRoutine = StartCoroutine(MainLoop());
    }

    void OnDisable()
    {
        if (behaviorRoutine != null)
            StopCoroutine(behaviorRoutine);
    }

    IEnumerator MainLoop()
    {
        // 隨機延遲出現（更自然）
        yield return new WaitForSeconds(Random.Range(0f, 0.6f));

        // 淡入
        yield return Fade(0, 1, 0.5f);

        while (true)
        {
            if (!isPaused)
            {
                // 在出生點附近選一個目標
                float targetX = centerX + Random.Range(-moveRange, moveRange);
                yield return MoveTo(targetX);

                // 停駐
                yield return new WaitForSeconds(Random.Range(0.8f, 2.5f));

                // 隨機消失
                if (Random.value < 0.25f)
                {
                    yield return Fade(1, 0, 0.5f);
                    gameObject.SetActive(false);
                    yield break;
                }
            }

            yield return null;
        }
    }

    IEnumerator MoveTo(float targetX)
    {
        while (Mathf.Abs(transform.position.x - targetX) > 0.02f)
        {
            if (isPaused) yield break;

            transform.position = Vector3.MoveTowards(
                transform.position,
                new Vector3(targetX, transform.position.y, transform.position.z),
                Time.deltaTime * 0.6f   // 慢一點更像鬼火
            );
            yield return null;
        }
    }

    public void FadeOutAndPause()
    {
        if (!gameObject.activeSelf) return;

        isPaused = true;
        StartCoroutine(Fade(1, 0, 0.4f));
    }

    public void FadeInAndResume()
    {
        isPaused = false;
        StartCoroutine(Fade(0, 1, 0.4f));
    }

    IEnumerator Fade(float from, float to, float duration)
    {
        float t = 0;
        while (t < duration)
        {
            t += Time.deltaTime;
            float a = Mathf.Lerp(from, to, t / duration);
            sr.color = new Color(1, 1, 1, a);
            yield return null;
        }
        sr.color = new Color(1, 1, 1, to);
    }
}

