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
    public int rotation; // ȸ�� ���� (0, 90, 180, 270)

    public MapTile(string name, GameObject prefab, string terrain, TileEdge top, TileEdge bottom, TileEdge left, TileEdge right)
    {
        tileName = name;
        this.prefab = prefab;
        terrainType = terrain;
        topEdge = top;
        bottomEdge = bottom;
        leftEdge = left;
        rightEdge = right;
        rotation = 0; // �ʱ� ȸ�� ����
    }

    // Ÿ�� ȸ�� ����
    public void Rotate()
    {
        // 90�� ȸ�� (top -> right -> bottom -> left)
        TileEdge temp = topEdge;
        topEdge = leftEdge;
        leftEdge = bottomEdge;
        bottomEdge = rightEdge;
        rightEdge = temp;

        // ȸ�� ���� ������Ʈ
        rotation = (rotation + 90) % 360;
    }

    // �̿� Ÿ�ϰ��� ȣȯ�� �˻�
    public bool IsCompatibleWithNeighbor(MapTile neighborTile, Vector2Int direction)
    {
        if (neighborTile == null) return false;

        if (direction == Vector2Int.up) // ���� �˻�
        {
            return bottomEdge.compatibleEdgeTypes.Contains(neighborTile.topEdge.edgeType);
        }
        else if (direction == Vector2Int.down) // �Ʒ��� �˻�
        {
            return topEdge.compatibleEdgeTypes.Contains(neighborTile.bottomEdge.edgeType);
        }
        else if (direction == Vector2Int.right) // ������ �˻�
        {
            return leftEdge.compatibleEdgeTypes.Contains(neighborTile.rightEdge.edgeType);
        }
        else if (direction == Vector2Int.left) // ���� �˻�
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
    public int gridSize = 10; // �׸��� ũ��
    public float tileSpacing = 50.0f; // Ÿ�� ����

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
        // Ÿ�� �������� ���⼭ �ε� (����� �������� ����)
        // Ÿ�� ������ �ҷ�����
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

        // �� Ÿ���� �� ����
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




        // ������ ���� Ÿ�� ����



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

        // �� Ÿ���� ȸ���� ���� �߰�
        foreach (MapTile originalTile in tiles.ToList()) // ���� Ÿ�� ����Ʈ�� �����Ͽ� ȸ���� Ÿ���� �߰�
        {
            for (int i = 1; i < 4; i++) // 90��, 180��, 270�� ȸ���� ���� ����
            {
                MapTile rotatedTile = new MapTile(
                    originalTile.tileName + "_rot" + (i * 90),
                    originalTile.prefab,
                    originalTile.terrainType,
                    originalTile.rightEdge, // 90�� ȸ�� �� top -> right
                    originalTile.topEdge,   // right -> bottom
                    originalTile.bottomEdge,// left -> top
                    originalTile.leftEdge   // bottom -> left
                );
                rotatedTile.rotation = i * 90; // ȸ�� ���� ����
                tiles.Add(rotatedTile);
            }
        }

    }

    void InitializeGrid()
    {
        // ������ ��� Ÿ�Ϸ� �׸��带 �ʱ�ȭ
        grid = new Tile[gridSize, gridSize];
        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                grid[x, y] = new Tile(tiles, new Vector2Int(x, y)); // ������ ��� Ÿ�� �Ҵ�
            }
        }
    }

    void CollapseGrid()
    {
        Stack<(Tile tile, List<MapTile> previousTiles)> backtrackStack = new Stack<(Tile, List<MapTile>)>();
        int maxIterations = 10000; // �ִ� ���� �ݺ� Ƚ�� ����
        int iterationCount = 0;

        while (!IsGridCollapsed())
        {
            iterationCount++;
            if (iterationCount > maxIterations)
            {
                Debug.LogError("�ִ� �ݺ� Ƚ���� �����Ͽ� ���� ������ �����ϱ� ���� �����մϴ�.");
                break;
            }

            Tile currentTile = GetTileWithLowestEntropy();
            if (currentTile == null || currentTile.GetPossibleTiles().Count == 0)
            {
                if (backtrackStack.Count > 0)
                {
                    // ��Ʈ��ŷ �õ�
                    Backtrack(backtrackStack);
                }
                else
                {
                    Debug.LogError("��Ʈ��ŷ ������ ��ȿ�� Ÿ���� �����ϴ�. ��Ʈ��ŷ ������ ��� �־� �׸��带 �ʱ�ȭ�մϴ�.");
                    InitializeGrid(); // �׸��� �ʱ�ȭ �� �ٽ� ����
                    iterationCount = 0; // �ݺ� Ƚ�� �ʱ�ȭ
                }
            }
            else if (!currentTile.IsCollapsed) // �̹� ��ġ�� Ÿ���� �ƴ϶��
            {
                // ��Ʈ��ŷ�� ���� ������ Ÿ�� ����
                backtrackStack.Push((currentTile, new List<MapTile>(currentTile.GetPossibleTiles())));

                // ���� Ÿ���� ��� �õ�
                currentTile.Collapse(random);

                if (currentTile.GetSelectedTile() == null)
                {
                    Debug.LogError($"��ġ {currentTile.Position}���� Ÿ�� ��� �� ���õ� Ÿ���� �����ϴ�. ������ Ÿ�� ��: {currentTile.GetPossibleTiles().Count}");
                    continue; // ������ ���� ��� �������� �Ѿ�ϴ�.
                }

                Debug.Log($"��ġ {currentTile.Position}�� Ÿ���� {currentTile.GetSelectedTile()?.tileName}�� ����߽��ϴ�");
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

            // ������ Ÿ�� ����
            backtrackTile.SetPossibleTiles(previousTiles);

            // ������ �����ߴ� Ÿ���� ������ ������ Ÿ�ϵ�� ����� �õ�
            List<MapTile> newPossibleTiles = backtrackTile.GetPossibleTiles().Where(tile => tile != backtrackTile.GetSelectedTile()).ToList();

            if (newPossibleTiles.Count > 0)
            {
                // ���ο� Ÿ�� ���� �� ���
                backtrackTile.SetPossibleTiles(newPossibleTiles);
                backtrackTile.Collapse(random);
                Debug.Log($"��ġ {backtrackTile.Position}�� Ÿ���� ��Ʈ��ŷ �� {backtrackTile.GetSelectedTile()?.tileName}�� ������߽��ϴ�");
                InstantiateTileAtPosition(backtrackTile);
                PropagateConstraints(backtrackTile);
                return; // ��Ʈ��ŷ ���� �� �Լ� ����
            }
            else
            {
                Debug.LogWarning($"��ġ {backtrackTile.Position}���� ���ο� ������ Ÿ���� �����ϴ�. ����ؼ� ��Ʈ��ŷ�� �õ��մϴ�.");
            }
        }

        Debug.LogError("��Ʈ��ŷ ����, ������ Ÿ���� ã�� ���߽��ϴ�. ��Ʈ��ŷ ������ ��� �־� �ʱ�ȭ�� �ʿ��մϴ�.");
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
                        Debug.Log($"��ġ {tile.Position}�� Ÿ���� ���� {direction}�� ��ġ {neighborPos}�� �̿��� ȣȯ���� �ʽ��ϴ�");
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
            Debug.LogError("PropagateConstraints �Լ��� ���޵� collapsedTile�� null�Դϴ�.");
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
                            Debug.LogError($"��ġ {currentTile.Position}���� Ÿ�� ��� �� ���õ� Ÿ���� �����ϴ�. ������ Ÿ�� ��: {currentTile.GetPossibleTiles().Count}, ������ Ÿ�� ���: {string.Join(", ", currentTile.GetPossibleTiles().Select(tile => tile.tileName))}");
                            continue;
                        }

                        foreach (MapTile possibleTile in neighborTile.GetPossibleTiles())
                        {
                            if (selectedTile.IsCompatibleWithNeighbor(possibleTile, direction))
                            {
                                compatibleTiles.Add(possibleTile);
                            }
                        }

                        // ������ Ÿ���� �پ����ٸ� ������Ʈ
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

                // �̹� Ÿ���� ��ġ�� ������ üũ (������ ��ġ�� Ÿ���� ������ �ʵ��� ����)
                Collider[] existingTiles = Physics.OverlapBox(position, new Vector3(tileSpacing / 2, 0.1f, tileSpacing / 2));
                if (existingTiles.Length > 0)
                {
                    Debug.LogWarning($"��ġ {tile.Position}�� �̹� Ÿ���� �����մϴ�. Ÿ�� ��ġ�� �ǳʶݴϴ�.");
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
            Vector2Int.up,    // ����
            Vector2Int.down,  // �Ʒ���
            Vector2Int.right, // ������
            Vector2Int.left   // ����
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
