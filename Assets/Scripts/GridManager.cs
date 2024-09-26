using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GridManager : MonoBehaviour
{
    [SerializeField] private GameObject gridCellPrefab;
    
    private int gridWidth;
    private int gridHeight;
    private float cellSpacing;
    
    private Camera mainCamera;
    private List<GameObject> gridCells = new List<GameObject>();
    private GridCell selectedCell = null;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    public void SetGridParameters(int width, int height, float spacing)
    {
        gridWidth = width;
        gridHeight = height;
        cellSpacing = spacing;
        
        SpawnGrid();
        AdjustCamera();
    }
    
    public void RandomizeGrid(float walkablePercentage)
    {
        ClearGrid();
        SpawnGrid();

        int totalCells = gridWidth * gridHeight;
        int walkableCells = Mathf.RoundToInt(totalCells * walkablePercentage);

        List<GameObject> randomCells = new List<GameObject>(gridCells);

        for (int i = 0; i < totalCells - walkableCells; i++)
        {
            int randomIndex = Random.Range(0, randomCells.Count);
            Destroy(randomCells[randomIndex]);
            randomCells.RemoveAt(randomIndex);
        }
    }

    public void RemoveCell(GameObject cell)
    {
        gridCells.Remove(cell);

        if (selectedCell != null && selectedCell.gameObject == cell)
        {
            selectedCell = null;
        }
        
        Destroy(cell);
    }

    public void SelectCell(GridCell cellToSelect)
    {
        if (selectedCell != null)
        {
            selectedCell.Deselect();
        }

        selectedCell = cellToSelect;
        selectedCell.Select();
    }

    private void SpawnGrid()
    {
        ClearGrid();

        float gridTotalWidth = (gridWidth - 1) * cellSpacing;
        float gridTotalHeight = (gridHeight - 1) * cellSpacing;
        Vector3 startPosition = new Vector3(-gridTotalWidth / 2, -gridTotalHeight / 2, 0);
        
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                Vector3 spawnPosition = new Vector3(x * cellSpacing, y * cellSpacing, 0) + startPosition;
                GameObject cell = Instantiate(gridCellPrefab, spawnPosition, Quaternion.identity, transform);

                GridCell gridCell = cell.AddComponent<GridCell>();
                gridCell.Initialize(this, cell);

                BoxCollider2D col = cell.AddComponent<BoxCollider2D>();
                col.isTrigger = true;
                
                gridCells.Add(cell);
            }
        }
    }

    private void ClearGrid()
    {
        foreach (Transform childCell in transform)
        {
            Destroy(childCell.gameObject);
        }
        gridCells.Clear();

        selectedCell = null;
    }
    
    private void AdjustCamera()
    {
        float verticalSize = (gridHeight * cellSpacing) * 0.6f ;
        float horizontalSize = ((gridWidth * cellSpacing) * 0.6f ) / mainCamera.aspect;

        mainCamera.orthographicSize = Mathf.Max(verticalSize, horizontalSize);
        mainCamera.transform.position = new Vector3(0, 0, -10);
    }
}
