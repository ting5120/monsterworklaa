using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class OpeningSequenceController : MonoBehaviour
{
    GameObject staticTenguInstance;
    [Header("Coin")]
    public CoinManager coinManager;

    [Header("Opening Hide Objects")]
    public GameObject[] backgroundMonsters;

    [Header("Canvas / System Lock")]
    public GameObject canvas2;              // 正式遊戲 UI
    public MonoBehaviour inputManager;       // InputManager
    public MonoBehaviour cameraManager;      // CameraManager

    [Header("Tengu")]
    public GameObject tengu;
    public Animator tenguAnimator;
    public Transform tenguTargetPos;
    public float tenguMoveDuration = 2f;

    [Header("Tengu Static")]
    public GameObject tenguStaticPrefab;
    public Transform tenguStaticSpawnPos;

    [Header("Dialogue")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;

    [Header("Building")]
    public GameObject firstBuildingPrefab;
    public Transform buildingSpawnPos;

    [Header("Reward")]
    public GameObject rewardPanel;

    [Header("Tutorial")]
    public GameObject tutorialPanel;
    public Image tutorialImage;
    public Sprite[] tutorialSprites;


    int step = 0;
    int dialogueIndex = 0;
    string[] currentDialogue;
    int tutorialIndex = 0;

    #region Unity

    void Start()
    {
        // 1️⃣ 鎖住正式遊戲
        canvas2.SetActive(false);
        inputManager.enabled = false;
        cameraManager.enabled = false;

        // 2️⃣ 隱藏背景初始妖怪
        HideBackgroundMonsters();

        // 3️⃣ UI 初始化
        dialoguePanel.SetActive(false);
        rewardPanel.SetActive(false);
        tutorialPanel.SetActive(false);

        // 4️⃣ 天狗顯示（一開始在畫面外）
        tengu.SetActive(true);

        StartCoroutine(StartOpening());
    }

    IEnumerator StartOpening()
    {
        yield return new WaitForSeconds(0.5f);

        NextStep();
    }

    #endregion

    #region Opening Flow



    void NextStep()
    {
        step++;
        Debug.Log($"[Opening] Step {step}");

        switch (step)
        {
            case 1:
                StartCoroutine(TenguWalkIn());
                break;

            case 2:
                ShowDialogue(new string[]
                {
                    "嘿，新來的。",
                    "這裡是志怪打工的世界。"
                });
                break;

            case 3:
                Instantiate(firstBuildingPrefab, buildingSpawnPos.position, Quaternion.identity);
                NextStep();
                break;

            case 4:
                ShowDialogue(new string[]
                {
                    "這是你的第一間建築。",
                    "妖怪會在這裡開始打工。"
                });
                break;

            case 5:
                rewardPanel.SetActive(true);
                break;

            case 6:
                ShowDialogue(new string[]
                {
                    "這 300 寶錢就當作見面禮吧。"
                });
                break;

            case 7:
                ShowTutorial();
                break;

            case 8:
                FinishOpening();
                break;
        }
    }

    #endregion

    #region Tengu

    IEnumerator TenguWalkIn()
    {
        Vector3 startPos = tengu.transform.position;
        Vector3 endPos = tenguTargetPos.position;

        tenguAnimator.Play("Walk");

        float timer = 0f;
        while (timer < tenguMoveDuration)
        {
            timer += Time.deltaTime;
            tengu.transform.position = Vector3.Lerp(startPos, endPos, timer / tenguMoveDuration);
            yield return null;
        }

        // 確保走到定位點
        tengu.transform.position = endPos;

        // 關掉走路天狗
        tengu.SetActive(false);

        // ⭐ 在這裡生成靜態天狗（正確位置）
        Vector3 spawnPos = tenguStaticSpawnPos.position;
        spawnPos.z = 0f; // 防止被鏡頭吃掉

        staticTenguInstance = Instantiate(
        tenguStaticPrefab,
        spawnPos,
        Quaternion.identity
        );


        Debug.Log("靜態天狗生成成功：" + staticTenguInstance.name);

        yield return new WaitForSeconds(0.2f);

        NextStep(); // 進對話
    }


    #endregion

    #region Dialogue

    void ShowDialogue(string[] lines)
    {
        currentDialogue = lines;
        dialogueIndex = 0;

        dialoguePanel.SetActive(true);
        dialogueText.text = currentDialogue[dialogueIndex];
    }

    public void OnClickNextDialogue()
    {
        dialogueIndex++;

        if (dialogueIndex >= currentDialogue.Length)
        {
            dialoguePanel.SetActive(false);
            NextStep();
        }
        else
        {
            dialogueText.text = currentDialogue[dialogueIndex];
        }
    }

    #endregion

    #region Reward

    public void OnClickGetReward()
    {
        coinManager.AddUncollectedCoins(300);
        rewardPanel.SetActive(false);
        NextStep();
    }

    #endregion

    #region Tutorial

    void ShowTutorial()
    {
        tutorialPanel.SetActive(true);
        tutorialIndex = 0;
        tutorialImage.sprite = tutorialSprites[tutorialIndex];
    }

    public void OnClickNextTutorial()
    {
        tutorialIndex++;

        if (tutorialIndex >= tutorialSprites.Length)
        {
            tutorialPanel.SetActive(false);
            NextStep();
        }
        else
        {
            tutorialImage.sprite = tutorialSprites[tutorialIndex];
        }
    }

    #endregion

    #region Opening End

    void HideBackgroundMonsters()
    {
        foreach (GameObject monster in backgroundMonsters)
        {
            monster.SetActive(false);
        }
    }

    void ShowBackgroundMonsters()
    {
        foreach (GameObject monster in backgroundMonsters)
        {
            monster.SetActive(true);
        }
    }

    void FinishOpening()
    {
        // 1️⃣ 妖怪回來
        ShowBackgroundMonsters();

        // 2️⃣ 解鎖系統
        canvas2.SetActive(true);
        inputManager.enabled = true;
        cameraManager.enabled = true;

        // 3️⃣ 移除靜態天狗（⭐關鍵）
        if (staticTenguInstance != null)
        {
            Destroy(staticTenguInstance);
        }

        // 4️⃣ 關掉走路天狗（保險）
        tengu.SetActive(false);

        // 5️⃣ 關掉 Opening
        Destroy(gameObject);
    }


    #endregion
}


