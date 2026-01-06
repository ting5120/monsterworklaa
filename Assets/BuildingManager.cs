using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BuildingManager : MonoBehaviour
{
    
    public static BuildingManager Instance { get; private set; }

    // Canvas Transform，用來生成建築面板
    public Transform canvasTransform;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject); 
            return;
        }
        Instance = this; 
    }

    public BuildingData GetCurrentPlacementData()
    {
        return buildingDataToPlace;
    }


    [Header("依賴項")]
    public GridManager gridManager;

    private BuildingData buildingDataToPlace;

    private bool isManualPlacement = false;///


    private bool isMoveMode = false;
    private Vector2Int? selectedSlot = null;  
    public bool IsMoveMode => isMoveMode;

    private void OnEnable()
    {
        SlotHandler.OnSlotClicked += HandleSlotClicked;
    }

    private void OnDisable()
    {
        SlotHandler.OnSlotClicked -= HandleSlotClicked;
    }

    public void StartMoveMode()
    {
        if (isMoveMode)
        {
            ExitMoveMode();
            return;
        }

        isMoveMode = true;
        selectedSlot = null;
        
        if (UIManager.Instance != null)
            UIManager.Instance.ShowOverlay(true);

        Debug.Log("[BuildingManager] 進入建築交換模式");
    }

    public void ExitMoveMode()
    {
        isMoveMode = false;
        selectedSlot = null;
        if (UIManager.Instance != null)
            UIManager.Instance.ShowOverlay(false);

        Debug.Log("[BuildingManager] 離開交換模式");
    }

    
    
    private void HandleSlotClicked(int row, int col)
    {
        if (!isMoveMode)
            return;

        var slot = gridManager.GridSlots[row, col];

        if (!slot.IsBuilt)
        {
            Debug.Log("[Swap] 此格子沒有建築物，無法選取");
            return;
        }

        SpriteRenderer clickedSprite = slot.EmptySlotWorld.GetComponent<SpriteRenderer>();

        Vector2Int clicked = new Vector2Int(row, col);
        if (selectedSlot.HasValue && selectedSlot.Value == clicked)
        {
            ResetHighlight(clickedSprite);
            selectedSlot = null;
            Debug.Log("[Swap] 取消選擇建築物");
            return;
        }

        if (!selectedSlot.HasValue)
        {
            selectedSlot = clicked;
            HighlightSlot(clickedSprite);
            Debug.Log($"[Swap] 已選擇第一個格子 ({row},{col})");
            return;
        }

        Vector2Int first = selectedSlot.Value;

        if (first.x == row && first.y == col)
        {
            Debug.Log("[Swap] 點到同一格，取消第二次選擇");
            return;
        }

        var firstSlot = gridManager.GridSlots[first.x, first.y];
        SpriteRenderer firstSprite = firstSlot.EmptySlotWorld.GetComponent<SpriteRenderer>();

        SwapBuildings(first.x, first.y, row, col);
        Debug.Log($"[Swap] 交換完成 ({first.x},{first.y}) ↔ ({row},{col})");

        // 交換後：第一個位置需還原大小
        ResetHighlight(firstSprite);

        // 交換後：新的位置保持選取放大
        selectedSlot = clicked;
        HighlightSlot(clickedSprite);

        //selectedSlot = new Vector2Int(row, col);

        Debug.Log($"[Swap] 新選取位置為交換後的建築物新位置 ({row},{col})");

    }




    private void SwapBuildings(int r1, int c1, int r2, int c2)
    {
        var slotA = gridManager.GridSlots[r1, c1];
        var slotB = gridManager.GridSlots[r2, c2];

        GameObject buildingA = slotA.CurrentBuilding;
        GameObject buildingB = slotB.CurrentBuilding;

        if (buildingA == null && buildingB == null) return;



        if (buildingA != null) buildingA.transform.SetParent(slotB.EmptySlotWorld.transform);
        if (buildingB != null) buildingB.transform.SetParent(slotA.EmptySlotWorld.transform);

        if (buildingA != null) buildingA.transform.localPosition = Vector3.zero;
        if (buildingB != null) buildingB.transform.localPosition = Vector3.zero;

        SpriteRenderer srA = slotA.EmptySlotWorld?.GetComponent<SpriteRenderer>();
        SpriteRenderer srB = slotB.EmptySlotWorld?.GetComponent<SpriteRenderer>();
        
        if (srA != null && srB != null)
        {
            Sprite temp = srA.sprite;
            srA.sprite = srB.sprite;
            srB.sprite = temp;
        }

        slotA.CurrentBuilding = buildingB;
        slotB.CurrentBuilding = buildingA;

        bool tempBuilt = slotA.IsBuilt;
        slotA.IsBuilt = slotB.IsBuilt;
        slotB.IsBuilt = tempBuilt;

        Debug.Log($"[Swap] ({r1},{c1}) ↔ ({r2},{c2}) 完整交換完成！");
    }

    // ==========================
    // 外觀放大 / 還原（slot SpriteRenderer）
    // ==========================
    private float highlightScale = 1.30f;
    private Dictionary<SpriteRenderer, Vector3> originalScales = new Dictionary<SpriteRenderer, Vector3>();

    private void HighlightSlot(SpriteRenderer sprite)
    {
        if (sprite == null) return;

        if (!originalScales.ContainsKey(sprite))
        {
            originalScales[sprite] = sprite.transform.localScale;
        }

        sprite.transform.localScale = originalScales[sprite] * highlightScale;
    }

    private void ResetHighlight(SpriteRenderer sprite)
    {
        if (sprite == null) return;

        if (originalScales.ContainsKey(sprite))
        {
            sprite.transform.localScale = originalScales[sprite];
            originalScales.Remove(sprite);
        }
    }


    public bool IsPlacementMode => buildingDataToPlace != null;

    public void StartPlacementMode(BuildingData data)
    {
        if (data == null) return;
        buildingDataToPlace = data;
        isManualPlacement = true;  // 標記為手動建立///
        Debug.Log($"[BuildingManager] 進入放置模式：{data.buildingName}");
    }

    public void TryPlaceBuilding(int row, int building)
    {
        if (gridManager.GridSlots[row, building].IsBuilt)
        {
            Debug.Log($"網格 ({row},{building}) 已被佔用，無法放置。");
            return;
        }

        if (!IsPlacementMode)
        {
            Debug.Log("未選中建築物，只開面板，不放置。");
            return;
        }

        gridManager.PlaceBuilding(row, building, buildingDataToPlace);

        // 只有手動建立的建築才觸發招募面板///
        if (isManualPlacement && RecruitManager.Instance != null)
        {
            RecruitManager.Instance.ShowRecruitStartPanel(buildingDataToPlace);
        }

        /*var newBuildingGO = gridManager.GridSlots[row, building].CurrentBuilding;
        if (newBuildingGO != null)
        {
            Building newBuilding = newBuildingGO.GetComponent<Building>();
            if (newBuilding != null)
                UIManager.Instance.ShowRecruitStartPanel(newBuilding);  //  放置後觸發招募開始面板
        }*/

        ExitPlacementMode(true);
        isManualPlacement = false; // 重置旗標///
    }

    public void ExitPlacementMode(bool placementSuccess)
    {
        buildingDataToPlace = null;
    }


}


