using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class AutoButtonSound : MonoBehaviour
{
    void Awake()
    {
        GetComponent<Button>()
            .onClick
            .AddListener(() =>
            {
                if (AudioManager.Instance != null)
                    AudioManager.Instance.PlayButton();
            });
    }
}
