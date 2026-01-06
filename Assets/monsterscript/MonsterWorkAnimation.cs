using UnityEngine;
using System.Collections;

public class MonsterWorkAnimation : MonoBehaviour
{
    private Animator animator;
    private Coroutine workRoutine;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    /// <summary>
    /// 進入打工狀態（由建築 / 招募系統呼叫）
    /// </summary>
    public void StartWork()
    {
        // 進入打工 State
        animator.SetBool("isWorking", true);

        // 避免重複啟動 Coroutine
        if (workRoutine != null)
            StopCoroutine(workRoutine);

        workRoutine = StartCoroutine(WorkLoop());
    }

    /// <summary>
    /// 離開打工狀態（保留給未來用）
    /// </summary>
    public void StopWork()
    {
        animator.SetBool("isWorking", false);

        if (workRoutine != null)
        {
            StopCoroutine(workRoutine);
            workRoutine = null;
        }
    }

    IEnumerator WorkLoop()
    {
        while (true)
        {
            // 隨機決定這一段是在「做事」還是「待機」
            bool doingWork = Random.value > 0.4f;
            animator.SetBool("isDoingWork", doingWork);

            // 做事時間比較長，待機比較短
            float waitTime = doingWork
                ? Random.Range(2.5f, 4f)
                : Random.Range(1.5f, 3f);

            yield return new WaitForSeconds(waitTime);
        }
    }
}


