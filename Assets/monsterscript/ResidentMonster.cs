using UnityEngine;

public class ResidentMonster : MonoBehaviour
{
    void Start()
    {
        GetComponent<MonsterWorkAnimation>().StartWork();
    }
}

