using UnityEngine;

public class StreetSystem : MonoBehaviour
{

    public StreetManager[] streets;
    int currentStreet = 0;

    void Start()
    {
        streets[0].ActivateStreet();
    }

    public void OnStreetChanged(int newIndex)
    {
        if (newIndex == currentStreet) return;

        streets[currentStreet].DeactivateStreet();
        streets[newIndex].ActivateStreet();

        currentStreet = newIndex;
    }
}

