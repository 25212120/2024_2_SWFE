/*

using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MapTile
{
    public string tileName;
    public GameObject prefab;
    public string terrainType;
    public TileEdge topEdge;
    public TileEdge bottomEdge;
    public TileEdge leftEdge;
    public TileEdge rightEdge;
    public int rotation;

    public MapTile(string name, GameObject prefab, string terrain, TileEdge top, TileEdge bottom, TileEdge left, TileEdge right)
    {
        tileName = name;
        this.prefab = prefab;
        terrainType = terrain;
        topEdge = top;
        bottomEdge = bottom;
        leftEdge = left;
        rightEdge = right;
        rotation = 0;
    }

    public void Rotate()
    {
        TileEdge temp = topEdge;
        topEdge = leftEdge;
        leftEdge = bottomEdge;
        bottomEdge = rightEdge;
        rightEdge = temp;
        rotation = (rotation + 90) % 360;
    }

    public bool IsCompatibleWithNeighbor(MapTile neighborTile, Vector2Int direction)
    {
        if (neighborTile == null) return true;

        if (direction == Vector2Int.up)
            return bottomEdge.compatibleEdgeTypes.Contains(neighborTile.topEdge.edgeType);
        else if (direction == Vector2Int.down)
            return topEdge.compatibleEdgeTypes.Contains(neighborTile.bottomEdge.edgeType);
        else if (direction == Vector2Int.right)
            return leftEdge.compatibleEdgeTypes.Contains(neighborTile.rightEdge.edgeType);
        else if (direction == Vector2Int.left)
            return rightEdge.compatibleEdgeTypes.Contains(neighborTile.leftEdge.edgeType);

        return false;
    }
}

public class TileEdge
{
    public string edgeType;
    public List<string> compatibleEdgeTypes;
}

public class InfiniteWFC : MonoBehaviour
{
    private List<MapTile> tiles = new List<MapTile>();
    public int gridSize = 10;
    public float tileSpacing = 50.0f;

    private Tile[,] grid;
    private System.Random random = new System.Random();

    void Start()
    {
        LoadTilePrefabs();
        InitializeGrid();
        CollapseGrid();
        InstantiateTiles();
    }

    void LoadTilePrefabs()
    {
        // 실제 타일 프리팹 로드 및 에지 설정
    }

    void InitializeGrid()
    {
        grid = new Tile[gridSize, gridSize];
        for (int x = 0; x < gridSize; x++)
            for (int y = 0; y < gridSize; y++)
                grid[x, y] = new Tile(tiles, new Vector2Int(x, y));
    }

    void CollapseGrid()
    {
        // 백트래킹 및 엔트로피 기반 타일 배치 로직 구현
    }

    void InstantiateTiles()
    {
        for (int x = 0; x < gridSize; x++)
            for (int y = 0; y < gridSize; y++)
                InstantiateTileAtPosition(grid[x, y]);
    }

    void InstantiateTileAtPosition(Tile tile)
    {
        if (!tile.IsCollapsed)
            return;

        MapTile selectedTile = tile.GetSelectedTile();
        if (selectedTile == null || selectedTile.prefab == null)
            return;

        Vector3 position = new Vector3(tile.Position.x * tileSpacing, 0, tile.Position.y * tileSpacing);
        GameObject obj = Instantiate(selectedTile.prefab, position, Quaternion.Euler(0, selectedTile.rotation, 0));
        obj.name = $"Tile_{tile.Position.x}_{tile.Position.y}";
    }

    bool IsValidPosition(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < gridSize && pos.y >= 0 && pos.y < gridSize;
    }

    public class Tile
    {
        public static readonly Vector2Int[] Directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
        public Vector2Int Position { get; private set; }
        public List<MapTile> possibleTiles;
        public bool IsCollapsed => possibleTiles.Count == 1;
        public int Entropy => possibleTiles.Count;

        public Tile(List<MapTile> tiles, Vector2Int position)
        {
            possibleTiles = new List<MapTile>(tiles);
            Position = position;
        }

        public void Collapse(System.Random random)
        {
            MapTile selected = possibleTiles[random.Next(possibleTiles.Count)];
            possibleTiles = new List<MapTile> { selected };
        }

        public void ReducePossibleTilesBasedOnNeighbor(Tile neighbor, Vector2Int direction)
        {
            List<MapTile> compatibleTiles = new List<MapTile>();
            foreach (MapTile tile in possibleTiles)
            {
                if (tile.IsCompatibleWithNeighbor(neighbor.GetSelectedTile(), -direction))
                    compatibleTiles.Add(tile);
            }
            possibleTiles = compatibleTiles;
        }

        public MapTile GetSelectedTile()
        {
            return IsCollapsed ? possibleTiles[0] : null;
        }
    }
}
*/