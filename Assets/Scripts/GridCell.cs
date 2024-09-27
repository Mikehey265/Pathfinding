using UnityEngine;

public class GridCell : MonoBehaviour
{
    private GridManager gridManager;
    private GameObject cell;
    private SpriteRenderer spriteRenderer;

    private Color defaultColor;
    private Color highlightColor = Color.gray;
    private Color startingColor = Color.green;
    private Color goalColor = Color.red;

    private bool bIsSelectedAsStarting;
    private bool bIsSelectedAsGoal;

    public void Initialize(GridManager manager, GameObject cellObject)
    {
        gridManager = manager;
        cell = cellObject;
        spriteRenderer = GetComponent<SpriteRenderer>();
        defaultColor = spriteRenderer.color;
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
}
