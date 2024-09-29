using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AStarPathfinding
{
    private GridManager gridManager;

    public AStarPathfinding(GridManager manager)
    {
        gridManager = manager;
    }

    public List<GridCell> FindPath(GridCell startCell, GridCell goalCell)
    {
        List<GridCell> openList = new List<GridCell>();
        HashSet<GridCell> closedList = new HashSet<GridCell>();
        
        startCell.GCost = 0;
        startCell.HCost = CalculateDistanceCost(startCell, goalCell);
        openList.Add(startCell);

        while (openList.Count > 0)
        {
            GridCell currentCell = openList.OrderBy(c => c.FCost).ThenBy(c => c.HCost).First();

            if (currentCell == goalCell)
            {
                return RetracePath(startCell, goalCell);
            }
            
            openList.Remove(currentCell);
            closedList.Add(currentCell);
            
            foreach (GridCell neighbor in gridManager.GetNeighbors(currentCell))
            {
                if (!neighbor.IsWalkable() || closedList.Contains(neighbor))
                {
                    continue;
                }

                int tentativeGCost = currentCell.GCost + CalculateDistanceCost(currentCell, neighbor);
                if (tentativeGCost < neighbor.GCost || !openList.Contains(neighbor))
                {
                    neighbor.GCost = tentativeGCost;
                    neighbor.HCost = CalculateDistanceCost(neighbor, goalCell);
                    neighbor.Parent = currentCell;

                    if (!openList.Contains(neighbor))
                    {
                        openList.Add(neighbor);
                    }
                }
            }
        }

        return null;
    }
    
    private List<GridCell> RetracePath(GridCell startCell, GridCell endCell)
    {
        List<GridCell> path = new List<GridCell>();
        GridCell currentCell = endCell;

        while (currentCell != startCell)
        {
            path.Add(currentCell);
            currentCell = currentCell.Parent;
        }

        path.Add(startCell);
        path.Reverse();
        return path;
    }

    private int CalculateDistanceCost(GridCell a, GridCell b)
    {
        return Mathf.Abs(a.GridX - b.GridX) + Mathf.Abs(a.GridY - b.GridY); // Manhattan Distance
    }
}
