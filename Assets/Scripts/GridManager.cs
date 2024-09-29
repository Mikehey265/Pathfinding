using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class GridManager : MonoBehaviour
{
    [SerializeField] private GameObject gridCellPrefab;
    [SerializeField] private GameObject playerPrefab;
    
    [SerializeField] private float moveDelay = 1f;
    [SerializeField] private float speed = 3f;
    
    private int gridWidth;
    private int gridHeight;
    private float cellSpacing;
    
    private Camera mainCamera;
    private List<GameObject> gridCells = new List<GameObject>();
    private GridCell startingCell = null;
    private GridCell goalCell = null;

    private GameObject playerInstance = null;
    private bool bIsCellInteractionEnabled = true;
    private AStarPathfinding pathfinding;

    private void Awake()
    {
        mainCamera = Camera.main;
        pathfinding = new AStarPathfinding(this);
    }
    
    #region GridMethods
    
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

            GameObject cellToRemove = randomCells[randomIndex];
            gridCells.Remove(cellToRemove);
            
            Destroy(randomCells[randomIndex]);
            randomCells.RemoveAt(randomIndex);
        }
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

                cell.name = $"Cell [{x}, {y}]";                
                
                GridCell gridCell = cell.AddComponent<GridCell>();
                gridCell.Initialize(this, cell, x, y);

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

        startingCell = null;
        goalCell = null;
    }
    
    #endregion

    #region PlayerInteraction
    
    public void RemoveCell(GameObject cell)
    {
        if(!bIsCellInteractionEnabled) return;
        
        gridCells.Remove(cell);

        if (startingCell != null && startingCell.gameObject == cell)
        {
            startingCell = null;
        }

        if (goalCell != null && goalCell.gameObject == cell)
        {
            goalCell = null;
        }
        
        Destroy(cell);
    }

    public void SelectStartingCell(GridCell cellToSelect)
    {
        if(!bIsCellInteractionEnabled) return;
        
        if (startingCell != null)
        {
            startingCell.Deselect();
        }

        startingCell = cellToSelect;
        startingCell.SelectAsStartingCell();
    }
    
    public void SelectGoalCell(GridCell cellToSelect)
    {
        if(!bIsCellInteractionEnabled) return;
        
        if (goalCell != null)
        {
            goalCell.Deselect();
        }

        goalCell = cellToSelect;
        goalCell.SelectAsGoalCell();
    }

    public bool IsStartAndGoalSelected()
    {
        return startingCell != null && goalCell != null;
    }

    public void DisableCellInteraction()
    {
        bIsCellInteractionEnabled = false;
    }
    
    public void EnableCellInteraction()
    {
        bIsCellInteractionEnabled = true;
    }

    public void DeselectCells()
    {
        if (startingCell != null)
        {
            startingCell.Deselect();
            startingCell = null;
        }

        if (goalCell != null)
        {
            goalCell.Deselect();
            goalCell = null;
        }
    }
    
    #endregion
    
    #region Pathfinding

    public bool ExecutePathfinding()
    {
        if (startingCell == null || goalCell == null)
        {
            return false;
        }

        List<GridCell> path = pathfinding.FindPath(startingCell, goalCell);
        if (path == null)
        {
            RemovePlayer();
            DeselectCells();
            EnableCellInteraction();
            return false;
        }

        StartCoroutine(MovePlayerAlongPath(path));
        return true;
    }

    private IEnumerator MovePlayerAlongPath(List<GridCell> path)
    {
        if (playerInstance == null)
        {
            SpawnPlayer();
        }
        
        foreach (GridCell targetCell in path)
        {
            Vector3 startPosition = playerInstance.transform.position;
            Vector3 targetPosition = targetCell.transform.position;
            float elapsedTime = 0f;

            while (elapsedTime < 1f)
            {
                elapsedTime += Time.deltaTime * speed;
                playerInstance.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime);
                yield return null;
            }
            
            playerInstance.transform.position = targetPosition;
            yield return new WaitForSeconds(moveDelay);
        }

        Debug.Log("Player reached the goal!");
    }

    public List<GridCell> GetNeighbors(GridCell cell)
    {
        List<GridCell> neighbors = new List<GridCell>();

        Vector2Int[] directions = {
            new Vector2Int(-1, 0), // Left
            new Vector2Int(1, 0),  // Right
            new Vector2Int(0, 1),  // Up
            new Vector2Int(0, -1)  // Down
        };

        foreach (Vector2Int direction in directions)
        {
            GridCell neighbor = GetCellAtPosition(cell.GridX + direction.x, cell.GridY + direction.y);
            if (neighbor != null && neighbor.IsWalkable())
            {
                neighbors.Add(neighbor);
            }
        }

        return neighbors;
    }

    private GridCell GetCellAtPosition(int x, int y)
    {
        return gridCells
            .Select(cell => cell != null ? cell.GetComponent<GridCell>() : null)
            .FirstOrDefault(c => c != null && c.GridX == x && c.GridY == y);
    }

    #endregion
    
    public void SpawnPlayer()
    {
        if(startingCell == null || playerPrefab == null) return;

        playerInstance = Instantiate(playerPrefab, startingCell.transform.position, Quaternion.identity);
    }

    public void RemovePlayer()
    {
        if (playerInstance != null)
        {
            Destroy(playerInstance);
        }
    }

    public void SetGridCoordinatesVisibility(bool isVisible)
    {
        foreach (GameObject cell in gridCells)
        {
            GridCell gridCell = cell.GetComponent<GridCell>();
            if (gridCell != null)
            {
                gridCell.SetCoordinateTextVisibility(isVisible);
            }
        }
    }
    
    private void AdjustCamera()
    {
        float verticalSize = (gridHeight * cellSpacing) * 0.6f ;
        float horizontalSize = ((gridWidth * cellSpacing) * 0.6f ) / mainCamera.aspect;

        mainCamera.orthographicSize = Mathf.Max(verticalSize, horizontalSize);
        mainCamera.transform.position = new Vector3(0, 0, -10);
    }
}
