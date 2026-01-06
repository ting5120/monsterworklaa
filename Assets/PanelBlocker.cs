using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelBlocker : MonoBehaviour
{
    public static PanelBlocker Instance { get; private set; }

    private readonly List<GameObject> registeredPanels = new List<GameObject>();

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void RegisterPanel(GameObject panel)
    {
        if (!registeredPanels.Contains(panel))
            registeredPanels.Add(panel);
    }

    public bool IsAnyPanelOpen()
    {
        foreach (var p in registeredPanels)
        {
            if (p != null && p.activeInHierarchy)
                return true;
        }
        return false;
    }
}

