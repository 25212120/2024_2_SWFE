using System.Collections.Generic;
using UnityEngine;

public class OccupiedCell_Manager : MonoBehaviour
{
    public HashSet<Vector2Int> occupiedCells = new HashSet<Vector2Int>();

    public HashSet<Vector2Int> GetOccupiedCells()
    {
        return occupiedCells;
    }

    public void AddOccupiedCell(Vector2Int cell)
    {
        occupiedCells.Add(cell);
    }

    public void RemoveOccupiedCell(Vector2Int cell)
    {
        occupiedCells.Remove(cell);
    }
}
