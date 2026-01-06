using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class DecorationMoveButton : MonoBehaviour
{
    public DecorationWorldObject targetDecoration;

    private bool isDragging = false;
    private CameraManager cameraManagerBackup;
    private InputManager inputManagerBackup;


    private void OnMouseDown()
    {
        Debug.Log("MouseDown on MoveButton");
        if (targetDecoration != null)
        {
            isDragging = true;

            //  告知裝飾：我正在被操作
            targetDecoration.SetInteracting(true);
            targetDecoration.StartMoveMode(); // 可選：改裝飾物狀態，方便控制

            // 暫停 CameraManager 移動
            cameraManagerBackup = FindObjectOfType<CameraManager>();
            if (cameraManagerBackup != null)
                cameraManagerBackup.blockMovement = true;

            // 暫停 InputManager 偵測滑動
            inputManagerBackup = FindObjectOfType<InputManager>();
            if (inputManagerBackup != null)
                inputManagerBackup.blockCameraInput = true;
        }
    }

    private void OnMouseDrag()
    {
        if (isDragging && targetDecoration != null)
        {
            Debug.Log("Dragging MoveButton");

            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0f; // 2D 層級
            targetDecoration.transform.position = mousePos;
        }
    }

    private void OnMouseUp()
    {
        Debug.Log("MouseUp on MoveButton");

        if (isDragging && targetDecoration != null)
        {
            isDragging = false;
            targetDecoration.EndMoveMode(); // 可選：恢復狀態

            //  操作結束，一定要解除
            targetDecoration.SetInteracting(false);

            // 恢復 CameraManager 移動
            if (cameraManagerBackup != null)
                cameraManagerBackup.blockMovement = false;
            // 恢復 InputManager
            if (inputManagerBackup != null)
                inputManagerBackup.blockCameraInput = false;
        }
    }
}


