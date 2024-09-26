using System;
using UnityEngine;

public class GridCell : MonoBehaviour
{
    private GridManager gridManager;
    private GameObject cell;
    private SpriteRenderer spriteRenderer;

    private Color defaultColor;
    private Color highlightColor = Color.gray;
    private Color selectedColor = Color.red;

    private bool bIsSelected;

    public void Initialize(GridManager manager, GameObject cellObject)
    {
        gridManager = manager;
        cell = cellObject;
        spriteRenderer = GetComponent<SpriteRenderer>();
        defaultColor = spriteRenderer.color;
    }

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(1))
        {
            if (bIsSelected)
            {
                gridManager.bIsAnyCellSelected = false;
            }
            gridManager.RemoveCell(cell);
        }
    }

    private void OnMouseDown()
    {
        if (Input.GetMouseButtonDown(0) && !gridManager.bIsAnyCellSelected)
        {
            gridManager.bIsAnyCellSelected = true;
            bIsSelected = true;
            
            defaultColor = selectedColor;
            spriteRenderer.color = defaultColor;
        }
    }

    private void OnMouseEnter()
    {
        spriteRenderer.color = highlightColor;
    }

    private void OnMouseExit()
    {
        spriteRenderer.color = defaultColor;
    }
}
