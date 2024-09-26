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

    public void Select()
    {
        bIsSelected = true;
        spriteRenderer.color = selectedColor;
    }

    public void Deselect()
    {
        bIsSelected = false;
        spriteRenderer.color = defaultColor;
    }

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(1))
        {
            gridManager.RemoveCell(cell);
        }

        if (Input.GetMouseButtonDown(0))
        {
            gridManager.SelectCell(this);
        }
    }

    private void OnMouseEnter()
    {
        if (!bIsSelected)
        {
            spriteRenderer.color = highlightColor;   
        }
    }

    private void OnMouseExit()
    {
        if (!bIsSelected)
        {
            spriteRenderer.color = defaultColor;
        }
    }
}
