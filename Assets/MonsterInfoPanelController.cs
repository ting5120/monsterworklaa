using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MonsterInfoPanelController : MonoBehaviour
{
    public static MonsterInfoPanelController Instance;

    [Header("UI")]
    public Image monsterImage;
    public Image frameImage;
    public TMP_Text nameText;
    public TMP_Text descriptionText;

    private void Awake()
    {
        Instance = this;
        gameObject.SetActive(false); // §@∂}©l¡Ù¬√
    }

    public void ShowMonster(MonsterData data)
    {
        monsterImage.sprite = data.monsterImage;
        frameImage.sprite = data.frameImage;
        nameText.text = data.monsterName;
        descriptionText.text = data.description;

        Open();
    }

    public void Open()
    {
        gameObject.SetActive(true);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
}
