using System.Collections.Generic;
using UnityEngine;

public class Cell
{
    public Vector2Int position;
    public List<TileState> possibleTileStates = new List<TileState>();
    public TileState collapsedTileState;

    public float Entropy
    {
        get
        {
            float sumWeights = 0f;
            float sumWeightLogWeights = 0f;

            foreach (var state in possibleTileStates)
            {
                float weight = state.tile.weight;
                sumWeights += weight;
                sumWeightLogWeights += weight * Mathf.Log(weight);
            }

            return Mathf.Log(sumWeights) - (sumWeightLogWeights / sumWeights);
        }
    }

    public bool IsCollapsed
    {
        get
        {
            return collapsedTileState != null;
        }
    }
}

public class TileState
{
    public Tile tile;
    public int rotationIndex; // 회전 상태 인덱스
}
