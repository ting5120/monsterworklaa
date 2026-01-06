using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DecorationConfirmButton : MonoBehaviour
{
    public DecorationWorldObject targetDecoration;

    private void OnMouseDown()
    {
        if (targetDecoration == null) return;

        targetDecoration.Deselect();
    }
}
