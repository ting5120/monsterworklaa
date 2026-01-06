using UnityEngine;

public class UISoundManager : MonoBehaviour
{
    public static UISoundManager Instance;
    public AudioClip clickSound;
    AudioSource audioSource;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayClick()
    {
        audioSource.PlayOneShot(clickSound);
    }
}
