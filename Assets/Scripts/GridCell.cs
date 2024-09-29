using TMPro;
using UnityEngine;

public class GridCell : MonoBehaviour
{
    public int GridX { get; private set; }
    public int GridY { get; private set; }
    
    public int GCost { get; set; }  // Cost from the start node.
    public int HCost { get; set; }  // Heuristic cost to the goal node.
    public GridCell Parent { get; set; }  // Parent node in the pathfinding.

    public int FCost => GCost + HCost;  // F = G + H
    
    private GridManager gridManager;
    private GameObject cell;
    private SpriteRenderer spriteRenderer;
    
    private Color defaultColor;
    private Color highlightColor = Color.gray;
    private Color startingColor = Color.green;
    private Color goalColor = Color.red;

    private TextMeshPro coordinateText;
    
    private bool bIsSelectedAsStarting;
    private bool bIsSelectedAsGoal;
    private bool walkable = true;

    public void Initialize(GridManager manager, GameObject cellObject, int x, int y)
    {
        gridManager = manager;
        cell = cellObject;
        spriteRenderer = GetComponent<SpriteRenderer>();
        defaultColor = spriteRenderer.color;
        
        GridX = x;
        GridY = y;

        coordinateText = GetComponentInChildren<TextMeshPro>();
        
        UpdateText();
    }

    public void SelectAsStartingCell()
    {
        bIsSelectedAsStarting = true;
        bIsSelectedAsGoal = false;
        spriteRenderer.color = startingColor;
    }
    
    public void SelectAsGoalCell()
    {
        bIsSelectedAsStarting = false;
        bIsSelectedAsGoal = true;
        spriteRenderer.color = goalColor;
    }

    public void Deselect()
    {
        bIsSelectedAsStarting = false;
        bIsSelectedAsGoal = false;
        spriteRenderer.color = defaultColor;
    }

    public void SetCoordinateTextVisibility(bool isVisible)
    {
        if (coordinateText != null)
        {
            coordinateText.gameObject.SetActive(isVisible);
        }
    }

    public bool IsWalkable() => walkable;

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
        {
            gridManager.SelectStartingCell(this);
        }
        
        if (Input.GetMouseButtonDown(1))
        {
            gridManager.SelectGoalCell(this);
        }

        if (Input.GetMouseButtonDown(2))
        {
            gridManager.RemoveCell(cell);
        }
    }

    private void OnMouseEnter()
    {
        if (!bIsSelectedAsStarting && !bIsSelectedAsGoal)
        {
            spriteRenderer.color = highlightColor;   
        }
    }

    private void OnMouseExit()
    {
        if (!bIsSelectedAsStarting && !bIsSelectedAsGoal)
        {
            spriteRenderer.color = defaultColor;
        }
    }
    
    private void UpdateText()
    {
        if (coordinateText != null)
        {
            coordinateText.text = $"{GridX}, {GridY}";
        }
    }
}
