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
    public int rotation; // 회전 상태 (0, 90, 180, 270)

    public MapTile(string name, GameObject prefab, string terrain, TileEdge top, TileEdge bottom, TileEdge left, TileEdge right)
    {
        tileName = name;
        this.prefab = prefab;
        terrainType = terrain;
        topEdge = top;
        bottomEdge = bottom;
        leftEdge = left;
        rightEdge = right;
        rotation = 0; // 초기 회전 상태
    }

    // 타일 회전 로직
    public void Rotate()
    {
        // 90도 회전 (top -> right -> bottom -> left)
        TileEdge temp = topEdge;
        topEdge = leftEdge;
        leftEdge = bottomEdge;
        bottomEdge = rightEdge;
        rightEdge = temp;

        // 회전 상태 업데이트
        rotation = (rotation + 90) % 360;
    }

    // 이웃 타일과의 호환성 검사
    public bool IsCompatibleWithNeighbor(MapTile neighborTile, Vector2Int direction)
    {
        if (neighborTile == null) return false;

        if (direction == Vector2Int.up) // 위쪽 검사
        {
            return bottomEdge.compatibleEdgeTypes.Contains(neighborTile.topEdge.edgeType);
        }
        else if (direction == Vector2Int.down) // 아래쪽 검사
        {
            return topEdge.compatibleEdgeTypes.Contains(neighborTile.bottomEdge.edgeType);
        }
        else if (direction == Vector2Int.right) // 오른쪽 검사
        {
            return leftEdge.compatibleEdgeTypes.Contains(neighborTile.rightEdge.edgeType);
        }
        else if (direction == Vector2Int.left) // 왼쪽 검사
        {
            return rightEdge.compatibleEdgeTypes.Contains(neighborTile.leftEdge.edgeType);
        }

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
    public int gridSize = 10; // 그리드 크기
    public float tileSpacing = 50.0f; // 타일 간격

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
        // 타일 프리팹을 여기서 로드 (현재는 구현되지 않음)
        // 타일 프리팹 불러오기
        // forest
        GameObject forestPrefab1 = Resources.Load<GameObject>("forest/Tile1_1");
        GameObject forestPrefab2 = Resources.Load<GameObject>("forest/Tile1_2");
        GameObject forestPrefab3 = Resources.Load<GameObject>("forest/Tile1_3");
        GameObject forestPrefab4 = Resources.Load<GameObject>("forest/Tile1_4");
        GameObject forestPrefab5 = Resources.Load<GameObject>("forest/Tile1_5");
        GameObject forestPrefab6 = Resources.Load<GameObject>("forest/Tile1_6");
        GameObject forestPrefab7 = Resources.Load<GameObject>("forest/Tile1_7");
        GameObject forestPrefab8 = Resources.Load<GameObject>("forest/Tile1_8");
        GameObject forestPrefab9 = Resources.Load<GameObject>("forest/Tile1_9");
        GameObject forestPrefab10 = Resources.Load<GameObject>("forest/Tile1_10");

        // 각 타일의 면 정의
        // forest tile   1: north 2: west 3: south 4: east
        TileEdge a1_1 = new TileEdge { edgeType = "a1_1", compatibleEdgeTypes = new List<string> { "a1_1", "a1_2", "a1_4", "c1_1", "c1_4", "b1_2", "b1_4", "d1_4", "f1_1", "f1_2", "f1_4", "f1_3", "g1_1", "g1_2", "g1_3", "g1_4", "h1_1", "h1_3", "i1_1", "i1_2", "j1_1", "j1_2", "j1_3" } };
        TileEdge a1_2 = new TileEdge { edgeType = "a1_2", compatibleEdgeTypes = new List<string> { "a1_1", "a1_2", "a1_4", "c1_1", "c1_4", "b1_2", "b1_4", "d1_4", "f1_1", "f1_2", "f1_4", "f1_3", "g1_1", "g1_2", "g1_3", "g1_4", "h1_1", "h1_3", "i1_1", "i1_2", "j1_1", "j1_2", "j1_3" } };
        TileEdge a1_3 = new TileEdge { edgeType = "a1_3", compatibleEdgeTypes = new List<string> { "a1_3", "b1_1", "c1_2", "c1_3", "d1_1", "d1_2", "d1_3", "e1_3", "e1_4", "e1_1", "e1_2" } };
        TileEdge a1_4 = new TileEdge { edgeType = "a1_4", compatibleEdgeTypes = new List<string> { "a1_1", "a1_2", "a1_4", "c1_1", "c1_4", "b1_2", "b1_4", "d1_4", "f1_1", "f1_2", "f1_4", "f1_3", "g1_1", "g1_2", "g1_3", "g1_4", "h1_1", "h1_3", "i1_1", "i1_2", "j1_1", "j1_2", "j1_3" } };
        TileEdge b1_1 = new TileEdge { edgeType = "b1_1", compatibleEdgeTypes = new List<string> { "a1_3", "b1_1", "c1_2", "c1_3", "d1_1", "d1_2", "d1_3", "e1_3", "e1_4", "e1_1", "e1_2" } };
        TileEdge b1_2 = new TileEdge { edgeType = "b1_2", compatibleEdgeTypes = new List<string> { "a1_1", "a1_2", "a1_4", "c1_1", "c1_4", "b1_2", "b1_4", "d1_4", "f1_1", "f1_2", "f1_4", "f1_3", "g1_1", "g1_2", "g1_3", "g1_4", "h1_1", "h1_3", "i1_1", "i1_2", "j1_1", "j1_2", "j1_3", "h1_2", "h1_4" } };
        TileEdge b1_3 = new TileEdge { edgeType = "b1_3", compatibleEdgeTypes = new List<string> { "a1_3", "b1_1", "c1_2", "c1_3", "d1_1", "d1_2", "d1_3", "e1_3", "e1_4", "e1_1", "e1_2" } };
        TileEdge b1_4 = new TileEdge { edgeType = "b1_4", compatibleEdgeTypes = new List<string> { "a1_1", "a1_2", "a1_4", "c1_1", "c1_4", "b1_2", "b1_4", "d1_4", "f1_1", "f1_2", "f1_4", "f1_3", "g1_1", "g1_2", "g1_3", "g1_4", "h1_1", "h1_3", "i1_1", "i1_2", "j1_1", "j1_2", "j1_3", "h1_2", "h1_4" } };
        TileEdge c1_1 = new TileEdge { edgeType = "c1_1", compatibleEdgeTypes = new List<string> { "g1_4", "h1_4", "i1_4", "j1_4", "l1_1", "l1_2", "l1_3", "l1_4" } };
        TileEdge c1_2 = new TileEdge { edgeType = "c1_2", compatibleEdgeTypes = new List<string> { "a1_3", "b1_1", "c1_2", "c1_3", "d1_1", "d1_2", "d1_3", "e1_3", "e1_4", "e1_1", "e1_2" } };
        TileEdge c1_3 = new TileEdge { edgeType = "c1_3", compatibleEdgeTypes = new List<string> { "a1_3", "b1_1", "c1_2", "c1_3", "d1_1", "d1_2", "d1_3", "e1_3", "e1_4", "e1_1", "e1_2" } };
        TileEdge c1_4 = new TileEdge { edgeType = "c1_4", compatibleEdgeTypes = new List<string> { "a1_1", "a1_2", "a1_4", "c1_1", "c1_4", "b1_2", "b1_4", "d1_4", "f1_1", "f1_2", "f1_4", "f1_3", "g1_1", "g1_2", "g1_3", "g1_4", "h1_1", "h1_3", "i1_1", "i1_2", "j1_1", "j1_2", "j1_3", "h1_2", "h1_4" } };
        TileEdge d1_1 = new TileEdge { edgeType = "d1_1", compatibleEdgeTypes = new List<string> { "a1_3", "b1_1", "c1_2", "c1_3", "d1_1", "d1_2", "d1_3", "e1_3", "e1_4", "e1_1", "e1_2" } };
        TileEdge d1_2 = new TileEdge { edgeType = "d1_2", compatibleEdgeTypes = new List<string> { "a1_3", "b1_1", "c1_2", "c1_3", "d1_1", "d1_2", "d1_3", "e1_3", "e1_4", "e1_1", "e1_2" } };
        TileEdge d1_3 = new TileEdge { edgeType = "d1_3", compatibleEdgeTypes = new List<string> { "a1_3", "b1_1", "c1_2", "c1_3", "d1_1", "d1_2", "d1_3", "e1_3", "e1_4", "e1_1", "e1_2" } };
        TileEdge d1_4 = new TileEdge { edgeType = "d1_4", compatibleEdgeTypes = new List<string> { "a1_1", "a1_2", "a1_4", "c1_1", "c1_4", "b1_2", "b1_4", "d1_4", "f1_1", "f1_2", "f1_4", "f1_3", "g1_1", "g1_2", "g1_3", "g1_4", "h1_1", "h1_3", "i1_1", "i1_2", "j1_1", "j1_2", "j1_3", "h1_2", "h1_4" } };
        TileEdge e1_1 = new TileEdge { edgeType = "e1_1", compatibleEdgeTypes = new List<string> { "a1_3", "b1_1", "c1_2", "c1_3", "d1_1", "d1_2", "d1_3", "e1_3", "e1_4", "e1_1", "e1_2" } };
        TileEdge e1_2 = new TileEdge { edgeType = "e1_2", compatibleEdgeTypes = new List<string> { "a1_3", "b1_1", "c1_2", "c1_3", "d1_1", "d1_2", "d1_3", "e1_3", "e1_4", "e1_1", "e1_2" } };
        TileEdge e1_3 = new TileEdge { edgeType = "e1_3", compatibleEdgeTypes = new List<string> { "a1_3", "b1_1", "c1_2", "c1_3", "d1_1", "d1_2", "d1_3", "e1_3", "e1_4", "e1_1", "e1_2" } };
        TileEdge e1_4 = new TileEdge { edgeType = "e1_4", compatibleEdgeTypes = new List<string> { "a1_3", "b1_1", "c1_2", "c1_3", "d1_1", "d1_2", "d1_3", "e1_3", "e1_4", "e1_1", "e1_2" } };
        TileEdge f1_1 = new TileEdge { edgeType = "f1_1", compatibleEdgeTypes = new List<string> { "a1_1", "a1_2", "a1_4", "c1_1", "c1_4", "b1_2", "b1_4", "d1_4", "f1_1", "f1_2", "f1_4", "f1_3", "g1_1", "g1_2", "g1_3", "g1_4", "h1_1", "h1_3", "i1_1", "i1_2", "j1_1", "j1_2", "j1_3" } };
        TileEdge f1_2 = new TileEdge { edgeType = "f1_2", compatibleEdgeTypes = new List<string> { "a1_1", "a1_2", "a1_4", "c1_1", "c1_4", "b1_2", "b1_4", "d1_4", "f1_1", "f1_2", "f1_4", "f1_3", "g1_1", "g1_2", "g1_3", "g1_4", "h1_1", "h1_3", "i1_1", "i1_2", "j1_1", "j1_2", "j1_3" } };
        TileEdge f1_3 = new TileEdge { edgeType = "f1_3", compatibleEdgeTypes = new List<string> { "a1_1", "a1_2", "a1_4", "c1_1", "c1_4", "b1_2", "b1_4", "d1_4", "f1_1", "f1_2", "f1_4", "f1_3", "g1_1", "g1_2", "g1_3", "g1_4", "h1_1", "h1_3", "i1_1", "i1_2", "j1_1", "j1_2", "j1_3" } };
        TileEdge f1_4 = new TileEdge { edgeType = "f1_4", compatibleEdgeTypes = new List<string> { "a1_1", "a1_2", "a1_4", "c1_1", "c1_4", "b1_2", "b1_4", "d1_4", "f1_1", "f1_2", "f1_4", "f1_3", "g1_1", "g1_2", "g1_3", "g1_4", "h1_1", "h1_3", "i1_1", "i1_2", "j1_1", "j1_2", "j1_3" } };
        TileEdge g1_1 = new TileEdge { edgeType = "g1_1", compatibleEdgeTypes = new List<string> { "a1_1", "a1_2", "a1_4", "c1_1", "c1_4", "b1_2", "b1_4", "d1_4", "f1_1", "f1_2", "f1_4", "f1_3", "g1_1", "g1_2", "g1_3", "g1_4", "h1_1", "h1_3", "i1_1", "i1_2", "j1_1", "j1_2", "j1_3", "h1_2", "h1_4", "i1_3", "i1_4", "j1_4" } };
        TileEdge g1_2 = new TileEdge { edgeType = "g1_2", compatibleEdgeTypes = new List<string> { "a1_1", "a1_2", "a1_4", "c1_1", "c1_4", "b1_2", "b1_4", "d1_4", "f1_1", "f1_2", "f1_4", "f1_3", "g1_1", "g1_2", "g1_3", "g1_4", "h1_1", "h1_3", "i1_1", "i1_2", "j1_1", "j1_2", "j1_3", "h1_2", "h1_4", "i1_3", "i1_4", "j1_4" } };
        TileEdge g1_3 = new TileEdge { edgeType = "g1_3", compatibleEdgeTypes = new List<string> { "a1_1", "a1_2", "a1_4", "c1_1", "c1_4", "b1_2", "b1_4", "d1_4", "f1_1", "f1_2", "f1_4", "f1_3", "g1_1", "g1_2", "g1_3", "g1_4", "h1_1", "h1_3", "i1_1", "i1_2", "j1_1", "j1_2", "j1_3", "h1_2", "h1_4", "i1_3", "i1_4", "j1_4" } };
        TileEdge g1_4 = new TileEdge { edgeType = "g1_4", compatibleEdgeTypes = new List<string> { "a1_1", "a1_2", "a1_4", "c1_1", "c1_4", "b1_2", "b1_4", "d1_4", "f1_1", "f1_2", "f1_4", "f1_3", "g1_1", "g1_2", "g1_3", "g1_4", "h1_1", "h1_3", "i1_1", "i1_2", "j1_1", "j1_2", "j1_3", "h1_2", "h1_4", "i1_3", "i1_4", "j1_4" } };
        TileEdge h1_1 = new TileEdge { edgeType = "h1_1", compatibleEdgeTypes = new List<string> { "a1_1", "a1_2", "a1_4", "c1_1", "c1_4", "b1_2", "b1_4", "d1_4", "f1_1", "f1_2", "f1_4", "f1_3", "g1_1", "g1_2", "g1_3", "g1_4", "h1_1", "h1_3", "i1_1", "i1_2", "j1_1", "j1_2", "j1_3", "h1_2", "h1_4", "i1_3", "i1_4", "j1_4" } };
        TileEdge h1_2 = new TileEdge { edgeType = "h1_2", compatibleEdgeTypes = new List<string> { "a1_1", "a1_2", "a1_4", "c1_1", "c1_4", "b1_2", "b1_4", "d1_4", "f1_1", "f1_2", "f1_4", "f1_3", "g1_1", "g1_2", "g1_3", "g1_4", "h1_1", "h1_3", "i1_1", "i1_2", "j1_1", "j1_2", "j1_3", "h1_2", "h1_4", "i1_3", "i1_4", "j1_4" } };
        TileEdge h1_3 = new TileEdge { edgeType = "h1_3", compatibleEdgeTypes = new List<string> { "a1_1", "a1_2", "a1_4", "c1_1", "c1_4", "b1_2", "b1_4", "d1_4", "f1_1", "f1_2", "f1_4", "f1_3", "g1_1", "g1_2", "g1_3", "g1_4", "h1_1", "h1_3", "i1_1", "i1_2", "j1_1", "j1_2", "j1_3", "h1_2", "h1_4", "i1_3", "i1_4", "j1_4" } };
        TileEdge h1_4 = new TileEdge { edgeType = "h1_4", compatibleEdgeTypes = new List<string> { "a1_1", "a1_2", "a1_4", "c1_1", "c1_4", "b1_2", "b1_4", "d1_4", "f1_1", "f1_2", "f1_4", "f1_3", "g1_1", "g1_2", "g1_3", "g1_4", "h1_1", "h1_3", "i1_1", "i1_2", "j1_1", "j1_2", "j1_3", "h1_2", "h1_4", "i1_3", "i1_4", "j1_4" } };
        TileEdge i1_1 = new TileEdge { edgeType = "i1_1", compatibleEdgeTypes = new List<string> { "a1_1", "a1_2", "a1_4", "c1_1", "c1_4", "b1_2", "b1_4", "d1_4", "f1_1", "f1_2", "f1_4", "f1_3", "g1_1", "g1_2", "g1_3", "g1_4", "h1_1", "h1_3", "i1_1", "i1_2", "j1_1", "j1_2", "j1_3", "h1_2", "h1_4", "i1_3", "i1_4", "j1_4" } };
        TileEdge i1_2 = new TileEdge { edgeType = "i1_2", compatibleEdgeTypes = new List<string> { "a1_1", "a1_2", "a1_4", "c1_1", "c1_4", "b1_2", "b1_4", "d1_4", "f1_1", "f1_2", "f1_4", "f1_3", "g1_1", "g1_2", "g1_3", "g1_4", "h1_1", "h1_3", "i1_1", "i1_2", "j1_1", "j1_2", "j1_3", "h1_2", "h1_4", "i1_3", "i1_4", "j1_4" } };
        TileEdge i1_3 = new TileEdge { edgeType = "i1_3", compatibleEdgeTypes = new List<string> { "a1_1", "a1_2", "a1_4", "c1_1", "c1_4", "b1_2", "b1_4", "d1_4", "f1_1", "f1_2", "f1_4", "f1_3", "g1_1", "g1_2", "g1_3", "g1_4", "h1_1", "h1_3", "i1_1", "i1_2", "j1_1", "j1_2", "j1_3", "h1_2", "h1_4", "i1_3", "i1_4", "j1_4" } };
        TileEdge i1_4 = new TileEdge { edgeType = "i1_4", compatibleEdgeTypes = new List<string> { "a1_1", "a1_2", "a1_4", "c1_1", "c1_4", "b1_2", "b1_4", "d1_4", "f1_1", "f1_2", "f1_4", "f1_3", "g1_1", "g1_2", "g1_3", "g1_4", "h1_1", "h1_3", "i1_1", "i1_2", "j1_1", "j1_2", "j1_3", "h1_2", "h1_4", "i1_3", "i1_4", "j1_4" } };
        TileEdge j1_1 = new TileEdge { edgeType = "j1_1", compatibleEdgeTypes = new List<string> { "a1_1", "a1_2", "a1_4", "c1_1", "c1_4", "b1_2", "b1_4", "d1_4", "f1_1", "f1_2", "f1_4", "f1_3", "g1_1", "g1_2", "g1_3", "g1_4", "h1_1", "h1_3", "i1_1", "i1_2", "j1_1", "j1_2", "j1_3", "h1_2", "h1_4", "i1_3", "i1_4", "j1_4" } };
        TileEdge j1_2 = new TileEdge { edgeType = "j1_2", compatibleEdgeTypes = new List<string> { "a1_1", "a1_2", "a1_4", "c1_1", "c1_4", "b1_2", "b1_4", "d1_4", "f1_1", "f1_2", "f1_4", "f1_3", "g1_1", "g1_2", "g1_3", "g1_4", "h1_1", "h1_3", "i1_1", "i1_2", "j1_1", "j1_2", "j1_3", "h1_2", "h1_4", "i1_3", "i1_4", "j1_4" } };
        TileEdge j1_3 = new TileEdge { edgeType = "j1_3", compatibleEdgeTypes = new List<string> { "a1_1", "a1_2", "a1_4", "c1_1", "c1_4", "b1_2", "b1_4", "d1_4", "f1_1", "f1_2", "f1_4", "f1_3", "g1_1", "g1_2", "g1_3", "g1_4", "h1_1", "h1_3", "i1_1", "i1_2", "j1_1", "j1_2", "j1_3", "h1_2", "h1_4", "i1_3", "i1_4", "j1_4" } };
        TileEdge j1_4 = new TileEdge { edgeType = "j1_4", compatibleEdgeTypes = new List<string> { "a1_1", "a1_2", "a1_4", "c1_1", "c1_4", "b1_2", "b1_4", "d1_4", "f1_1", "f1_2", "f1_4", "f1_3", "g1_1", "g1_2", "g1_3", "g1_4", "h1_1", "h1_3", "i1_1", "i1_2", "j1_1", "j1_2", "j1_3", "h1_2", "h1_4", "i1_3", "i1_4", "j1_4" } };




        // 지형별 여러 타일 정의



        tiles.Add(new MapTile("forestTile1", forestPrefab1, "Forest", a1_4, a1_3, a1_2, a1_1));
        tiles.Add(new MapTile("forestTile2", forestPrefab2, "Forest", b1_4, b1_3, b1_2, b1_1));
        tiles.Add(new MapTile("forestTile3", forestPrefab3, "Forest", c1_4, c1_3, c1_2, c1_1));
        tiles.Add(new MapTile("forestTile4", forestPrefab4, "Forest", d1_4, d1_3, d1_2, d1_1));
        tiles.Add(new MapTile("forestTile5", forestPrefab5, "Forest", e1_4, e1_3, e1_2, e1_1));
        tiles.Add(new MapTile("forestTile6", forestPrefab6, "Forest", f1_4, f1_3, f1_2, f1_1));
        tiles.Add(new MapTile("forestTile7", forestPrefab7, "Forest", g1_4, g1_3, g1_2, g1_1));
        tiles.Add(new MapTile("forestTile8", forestPrefab8, "Forest", h1_4, h1_3, h1_2, h1_1));
        tiles.Add(new MapTile("forestTile9", forestPrefab9, "Forest", i1_4, i1_3, i1_2, i1_1));
        tiles.Add(new MapTile("forestTile10", forestPrefab10, "Forest", j1_4, j1_3, j1_2, j1_1));
        tiles.Add(new MapTile("forestTile11", forestPrefab7, "Forest", g1_4, g1_3, g1_2, g1_1));
        tiles.Add(new MapTile("forestTile12", forestPrefab7, "Forest", g1_4, g1_3, g1_2, g1_1));
        tiles.Add(new MapTile("forestTile13", forestPrefab7, "Forest", g1_4, g1_3, g1_2, g1_1));
        tiles.Add(new MapTile("forestTile14", forestPrefab7, "Forest", g1_4, g1_3, g1_2, g1_1));
        tiles.Add(new MapTile("forestTile15", forestPrefab7, "Forest", g1_4, g1_3, g1_2, g1_1));

        // 각 타일의 회전된 버전 추가
        foreach (MapTile originalTile in tiles.ToList()) // 기존 타일 리스트를 복사하여 회전된 타일을 추가
        {
            for (int i = 1; i < 4; i++) // 90도, 180도, 270도 회전을 위한 루프
            {
                MapTile rotatedTile = new MapTile(
                    originalTile.tileName + "_rot" + (i * 90),
                    originalTile.prefab,
                    originalTile.terrainType,
                    originalTile.rightEdge, // 90도 회전 시 top -> right
                    originalTile.topEdge,   // right -> bottom
                    originalTile.bottomEdge,// left -> top
                    originalTile.leftEdge   // bottom -> left
                );
                rotatedTile.rotation = i * 90; // 회전 상태 설정
                tiles.Add(rotatedTile);
            }
        }

    }

    void InitializeGrid()
    {
        // 가능한 모든 타일로 그리드를 초기화
        grid = new Tile[gridSize, gridSize];
        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                grid[x, y] = new Tile(tiles, new Vector2Int(x, y)); // 가능한 모든 타일 할당
            }
        }
    }

    void CollapseGrid()
    {
        Stack<(Tile tile, List<MapTile> previousTiles)> backtrackStack = new Stack<(Tile, List<MapTile>)>();
        int maxIterations = 10000; // 최대 루프 반복 횟수 설정
        int iterationCount = 0;

        while (!IsGridCollapsed())
        {
            iterationCount++;
            if (iterationCount > maxIterations)
            {
                Debug.LogError("최대 반복 횟수에 도달하여 무한 루프를 방지하기 위해 중지합니다.");
                break;
            }

            Tile currentTile = GetTileWithLowestEntropy();
            if (currentTile == null || currentTile.GetPossibleTiles().Count == 0)
            {
                if (backtrackStack.Count > 0)
                {
                    // 백트래킹 시도
                    Backtrack(backtrackStack);
                }
                else
                {
                    Debug.LogError("백트래킹 가능한 유효한 타일이 없습니다. 백트래킹 스택이 비어 있어 그리드를 초기화합니다.");
                    InitializeGrid(); // 그리드 초기화 후 다시 시작
                    iterationCount = 0; // 반복 횟수 초기화
                }
            }
            else if (!currentTile.IsCollapsed) // 이미 배치된 타일이 아니라면
            {
                // 백트래킹을 위한 가능한 타일 저장
                backtrackStack.Push((currentTile, new List<MapTile>(currentTile.GetPossibleTiles())));

                // 현재 타일을 축소 시도
                currentTile.Collapse(random);

                if (currentTile.GetSelectedTile() == null)
                {
                    Debug.LogError($"위치 {currentTile.Position}에서 타일 축소 후 선택된 타일이 없습니다. 가능한 타일 수: {currentTile.GetPossibleTiles().Count}");
                    continue; // 문제가 있을 경우 다음으로 넘어갑니다.
                }

                Debug.Log($"위치 {currentTile.Position}의 타일을 {currentTile.GetSelectedTile()?.tileName}로 축소했습니다");
                InstantiateTileAtPosition(currentTile);
                PropagateConstraints(currentTile);
            }
        }

        
    }


    void Backtrack(Stack<(Tile tile, List<MapTile> previousTiles)> backtrackStack)
    {
        while (backtrackStack.Count > 0)
        {
            var backtrackData = backtrackStack.Pop();
            Tile backtrackTile = backtrackData.tile;
            List<MapTile> previousTiles = backtrackData.previousTiles;

            // 가능한 타일 복원
            backtrackTile.SetPossibleTiles(previousTiles);

            // 이전에 선택했던 타일을 제외한 나머지 타일들로 재축소 시도
            List<MapTile> newPossibleTiles = backtrackTile.GetPossibleTiles().Where(tile => tile != backtrackTile.GetSelectedTile()).ToList();

            if (newPossibleTiles.Count > 0)
            {
                // 새로운 타일 선택 후 축소
                backtrackTile.SetPossibleTiles(newPossibleTiles);
                backtrackTile.Collapse(random);
                Debug.Log($"위치 {backtrackTile.Position}의 타일을 백트랙킹 후 {backtrackTile.GetSelectedTile()?.tileName}로 재축소했습니다");
                InstantiateTileAtPosition(backtrackTile);
                PropagateConstraints(backtrackTile);
                return; // 백트래킹 성공 시 함수 종료
            }
            else
            {
                Debug.LogWarning($"위치 {backtrackTile.Position}에서 새로운 가능한 타일이 없습니다. 계속해서 백트랙킹을 시도합니다.");
            }
        }

        Debug.LogError("백트랙킹 실패, 적합한 타일을 찾지 못했습니다. 백트래킹 스택이 비어 있어 초기화가 필요합니다.");
    }





    public bool VerifyPlacement(Tile tile, MapTile selectedTile)
    {
        foreach (Vector2Int direction in Tile.Directions)
        {
            Vector2Int neighborPos = tile.Position + direction;
            if (IsValidPosition(neighborPos))
            {
                Tile neighborTile = grid[neighborPos.x, neighborPos.y];
                if (neighborTile.IsCollapsed)
                {
                    MapTile neighborSelectedTile = neighborTile.GetSelectedTile();
                    bool isCompatible = selectedTile.IsCompatibleWithNeighbor(neighborSelectedTile, direction);

                    if (!isCompatible)
                    {
                        Debug.Log($"위치 {tile.Position}의 타일이 방향 {direction}의 위치 {neighborPos}의 이웃과 호환되지 않습니다");
                        return false;
                    }
                }
            }
        }
        return true;
    }

    

    void PropagateConstraints(Tile collapsedTile)
    {
        if (collapsedTile == null)
        {
            Debug.LogError("PropagateConstraints 함수에 전달된 collapsedTile이 null입니다.");
            return;
        }

        Queue<Tile> propagationQueue = new Queue<Tile>();
        propagationQueue.Enqueue(collapsedTile);

        while (propagationQueue.Count > 0)
        {
            Tile currentTile = propagationQueue.Dequeue();
            Vector2Int position = currentTile.Position;

            foreach (Vector2Int direction in Tile.Directions)
            {
                Vector2Int neighborPos = position + direction;
                if (IsValidPosition(neighborPos))
                {
                    Tile neighborTile = grid[neighborPos.x, neighborPos.y];
                    if (neighborTile == null)
                    {
                        Debug.LogError($"Neighbor tile at position {neighborPos} is null.");
                        continue;
                    }

                    if (!neighborTile.IsCollapsed)
                    {
                        List<MapTile> compatibleTiles = new List<MapTile>();
                        MapTile selectedTile = currentTile.GetSelectedTile();

                        if (selectedTile == null)
                        {
                            Debug.LogError($"위치 {currentTile.Position}에서 타일 축소 후 선택된 타일이 없습니다. 가능한 타일 수: {currentTile.GetPossibleTiles().Count}, 가능한 타일 목록: {string.Join(", ", currentTile.GetPossibleTiles().Select(tile => tile.tileName))}");
                            continue;
                        }

                        foreach (MapTile possibleTile in neighborTile.GetPossibleTiles())
                        {
                            if (selectedTile.IsCompatibleWithNeighbor(possibleTile, direction))
                            {
                                compatibleTiles.Add(possibleTile);
                            }
                        }

                        // 가능한 타일이 줄어들었다면 업데이트
                        if (compatibleTiles.Count < neighborTile.GetPossibleTiles().Count)
                        {
                            neighborTile.SetPossibleTiles(compatibleTiles);
                            propagationQueue.Enqueue(neighborTile);
                        }
                    }
                }
            }
        }
    }


    void InstantiateTileAtPosition(Tile tile)
    {
        if (tile.IsCollapsed)
        {
            MapTile selectedTile = tile.GetSelectedTile();
            if (selectedTile != null && selectedTile.prefab != null)
            {
                Vector3 position = new Vector3(tile.Position.x * tileSpacing, 0, tile.Position.y * tileSpacing);

                // 이미 타일이 배치된 곳인지 체크 (기존에 배치된 타일을 지우지 않도록 주의)
                Collider[] existingTiles = Physics.OverlapBox(position, new Vector3(tileSpacing / 2, 0.1f, tileSpacing / 2));
                if (existingTiles.Length > 0)
                {
                    Debug.LogWarning($"위치 {tile.Position}에 이미 타일이 존재합니다. 타일 배치를 건너뜁니다.");
                    return;
                }

                GameObject instantiatedTile = Instantiate(selectedTile.prefab, position, Quaternion.Euler(0, selectedTile.rotation, 0));
                instantiatedTile.SetActive(true);
            }
        }
    }

    void InstantiateTiles()
    {
        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                InstantiateTileAtPosition(grid[x, y]);
            }
        }
    }

    bool IsGridCollapsed()
    {
        foreach (Tile tile in grid)
        {
            if (!tile.IsCollapsed)
                return false;
        }
        return true;
    }

    Tile GetTileWithLowestEntropy()
    {
        List<Tile> lowestEntropyTiles = new List<Tile>();
        int lowestEntropy = int.MaxValue;

        foreach (Tile tile in grid)
        {
            if (!tile.IsCollapsed && tile.Entropy < lowestEntropy)
            {
                lowestEntropy = tile.Entropy;
                lowestEntropyTiles.Clear();
                lowestEntropyTiles.Add(tile);
            }
            else if (!tile.IsCollapsed && tile.Entropy == lowestEntropy)
            {
                lowestEntropyTiles.Add(tile);
            }
        }

        return lowestEntropyTiles.Count > 0 ? lowestEntropyTiles[random.Next(lowestEntropyTiles.Count)] : null;
    }

    bool IsValidPosition(Vector2Int position)
    {
        return position.x >= 0 && position.x < gridSize && position.y >= 0 && position.y < gridSize;
    }

    public class Tile
    {
        public static readonly Vector2Int[] Directions =
        {
            Vector2Int.up,    // 위쪽
            Vector2Int.down,  // 아래쪽
            Vector2Int.right, // 오른쪽
            Vector2Int.left   // 왼쪽
        };

        private List<MapTile> possibleTiles;
        public Vector2Int Position { get; private set; }
        public bool IsCollapsed => possibleTiles.Count == 1;
        public int Entropy => possibleTiles.Count;

        public Tile(List<MapTile> allTiles, Vector2Int position)
        {
            possibleTiles = new List<MapTile>(allTiles);
            Position = position;
        }

        public void Collapse(System.Random random)
        {
            if (possibleTiles.Count > 1)
            {
                int index = random.Next(possibleTiles.Count);
                MapTile selectedTile = possibleTiles[index];
                possibleTiles.Clear();
                possibleTiles.Add(selectedTile);
            }
        }

        public void SetPossibleTiles(List<MapTile> tiles)
        {
            possibleTiles = new List<MapTile>(tiles);
        }

        public List<MapTile> GetPossibleTiles()
        {
            return possibleTiles;
        }

        public MapTile GetSelectedTile()
        {
            return IsCollapsed ? possibleTiles.First() : null;
        }
    }
   



    void OnDrawGizmos()
    {
        if (grid == null) return;

        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                Tile tile = grid[x, y];
                if (tile != null)
                {
                    float entropyNormalized = Mathf.InverseLerp(0, tiles.Count, tile.Entropy);
                    Gizmos.color = Color.Lerp(Color.green, Color.red, entropyNormalized);
                    Vector3 position = new Vector3(x * tileSpacing, 0, y * tileSpacing);
                    Gizmos.DrawWireCube(position, new Vector3(tileSpacing, 0.1f, tileSpacing));
                    UnityEditor.Handles.Label(position + Vector3.up * 0.5f, $"E: {tile.Entropy}");
                }
            }
        }
    }
}
