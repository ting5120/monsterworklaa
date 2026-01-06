using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DecorationControlButton : MonoBehaviour
{
    public DecorationWorldObject targetDecoration;

    private void OnMouseDown()
    {

        Debug.Log("Backpack button clicked");

        if (targetDecoration != null)
            targetDecoration.ReturnToBackpack();
    }
}
