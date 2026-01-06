using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class DecorationWorldObject : MonoBehaviour
{
    public GameObject controlGroupPrefab;
    public DecorationSlot ownerSlot;

    private GameObject controlGroupInstance;
    private bool isSelected = false;
    private bool isInteracting = false;

    public void SetInteracting(bool value)
    {
        isInteracting = value;
    }

    private void OnMouseDown()
    {
        // 有面板打開就不要操作
        if (PanelBlocker.Instance != null && PanelBlocker.Instance.IsAnyPanelOpen())
            return;

        // 正在被操作（移動 / 縮放）時，不切換選取
        if (isInteracting)
            return;
        //  如果點到的是控制鍵，不處理選取
        //if (IsClickingControlButton())
          //  return;

        ToggleSelection();
    }

    bool IsClickingControlButton()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.GetRayIntersection(ray);

        if (hit.collider == null) return false;

        return hit.collider.GetComponent<DecorationControlButton>() != null
            || hit.collider.GetComponent<DecorationMoveButton>() != null
            || hit.collider.GetComponent<DecorationScaleButton>() != null
            || hit.collider.GetComponent<DecorationConfirmButton>() != null; // ← 新增


    }

    void ToggleSelection()
    {
        if (isSelected) Deselect();
        else Select();
    }

    void Select()
    {
        isSelected = true;

        if (controlGroupInstance == null)
        {
            Vector3 offset = new Vector3(10.0f, 0.5f, 0);
            controlGroupInstance = Instantiate(
                controlGroupPrefab,
                transform.position + offset,
                Quaternion.identity
            );

            controlGroupInstance.transform.SetParent(transform);

            // 指定所有控制鍵 targetDecoration
            var controlBtns = controlGroupInstance.GetComponentsInChildren<DecorationControlButton>();
            foreach (var btn in controlBtns)
                btn.targetDecoration = this;

            var moveBtns = controlGroupInstance.GetComponentsInChildren<DecorationMoveButton>();
            foreach (var btn in moveBtns)
                btn.targetDecoration = this;

            var scaleBtns = controlGroupInstance.GetComponentsInChildren<DecorationScaleButton>();
            foreach (var btn in scaleBtns)
                btn.targetDecoration = this;

            var confirmBtns = controlGroupInstance.GetComponentsInChildren<DecorationConfirmButton>();
            foreach (var btn in confirmBtns)
                btn.targetDecoration = this;

        }

        controlGroupInstance.SetActive(true);
    }

    public void Deselect()
    {
        isSelected = false;

        if (controlGroupInstance != null)
            controlGroupInstance.SetActive(false);
    }


    public void StartMoveMode() { }
    public void EndMoveMode() { }

    public void ReturnToBackpack()
    {
        Debug.Log(" ReturnToBackpack triggered, ownerSlot: " + ownerSlot);

        if (ownerSlot != null)
            ownerSlot.AddOne();

        Destroy(gameObject);
    }
}
