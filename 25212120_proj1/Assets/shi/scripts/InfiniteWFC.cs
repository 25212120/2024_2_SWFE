using System.Collections.Generic;
using UnityEngine;

public class InfiniteWFC : MonoBehaviour
{
    // Ÿ�� ������ ���� ����
    [System.Serializable]
    public class TileEdge
    {
        public string edgeType;  // Ÿ���� �� Ÿ��
        public List<string> compatibleEdgeTypes;  // ȣȯ ������ �� Ÿ�� ����Ʈ
    }

    [System.Serializable]
    public class MapTile
    {
        public string name;  // Ÿ�� �̸�
        public GameObject prefab;  // Ÿ�� ������
        public string terrainType;  // ���� Ÿ��
        public TileEdge northEdge;  // ���� ��
        public TileEdge eastEdge;  // ���� ��
        public TileEdge southEdge;  // ���� ��
        public TileEdge westEdge;  // ���� ��

        public MapTile(string name, GameObject prefab, string terrainType, TileEdge north, TileEdge east, TileEdge south, TileEdge west)
        {
            this.name = name;
            this.prefab = prefab;
            this.terrainType = terrainType;
            this.northEdge = north;
            this.eastEdge = east;
            this.southEdge = south;
            this.westEdge = west;
        }
    }

    private List<MapTile> tiles = new List<MapTile>();  // ��� Ÿ�� ����Ʈ
    private Dictionary<string, MapTile> tileDictionary; // �̸����� Ÿ���� �����ϱ� ���� ��ųʸ�
    private List<MapTile>[,] grid;  // ���ڿ� ������ Ÿ�� ����Ʈ�� ����

    public int gridWidth = 10;
    public int gridHeight = 10;

    void Start()
    {
        // Ÿ�� ��ųʸ� �ʱ�ȭ
        tileDictionary = new Dictionary<string, MapTile>();
        InitializeTiles();
        foreach (MapTile tile in tiles)
        {
            tileDictionary.Add(tile.name, tile);
        }

        // ���� �ʱ�ȭ
        grid = new List<MapTile>[gridWidth, gridHeight];

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                // ��� ���� ������ ��� Ÿ���� �Ҵ�
                grid[x, y] = new List<MapTile>(tiles);
            }
        }

        // WFC �˰��� ����
        RunWFC();
    }

    void InitializeTiles()
    {
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

    }

    void RunWFC()
    {
        while (true)
        {
            // �ּ� ��Ʈ���� �� ã��
            Vector2Int cellToCollapse = FindLowestEntropyCell();
            if (cellToCollapse == Vector2Int.one * -1)
                break;  // �� �̻� ��ġ�� ���� ���ٸ� ����

            // Ÿ�� ���� �� ��ġ
            CollapseCell(cellToCollapse);

            // ���� ���� ����
            PropagateConstraints(cellToCollapse);
        }
    }

    Vector2Int FindLowestEntropyCell()
    {
        Vector2Int lowestEntropyCell = new Vector2Int(-1, -1);
        int lowestEntropy = int.MaxValue;

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                int entropy = grid[x, y].Count;
                if (entropy > 0 && entropy < lowestEntropy)
                {
                    lowestEntropy = entropy;
                    lowestEntropyCell = new Vector2Int(x, y);
                }
            }
        }
        return lowestEntropyCell;
    }

    void CollapseCell(Vector2Int cell)
    {
        // ������ Ÿ�� �� �ϳ��� �������� �����Ͽ� ���� Ȯ��
        List<MapTile> possibleTiles = grid[cell.x, cell.y];

        MapTile selectedTile = possibleTiles[Random.Range(0, possibleTiles.Count)];

        // ���� �ش� Ÿ�ϸ� �����
        grid[cell.x, cell.y] = new List<MapTile> { selectedTile };

        // ���⿡�� ������ Ÿ���� ��ġ�ϴ� ������ ����
        GameObject instance = Instantiate(selectedTile.prefab, new Vector3(cell.x, 0, cell.y), Quaternion.identity);
        // ���� ������ ���
        instance.transform.localScale = new Vector3(
            Mathf.Abs(instance.transform.localScale.x) * (Random.value > 0.5f ? -1 : 1),
            Mathf.Abs(instance.transform.localScale.y),
            Mathf.Abs(instance.transform.localScale.z) * (Random.value > 0.5f ? -1 : 1)
        );

        // BoxCollider ���� �� MeshCollider �߰�
        BoxCollider boxCollider = instance.GetComponent<BoxCollider>();
        if (boxCollider != null)
        {
            Destroy(boxCollider);
        }
        MeshCollider meshCollider = instance.AddComponent<MeshCollider>();
        meshCollider.convex = true;
        // ���� ������ ���
        Vector3 originalScale = instance.transform.localScale;
        instance.transform.localScale = new Vector3(
            Mathf.Abs(originalScale.x) * (Random.value > 0.5f ? -1 : 1),
            Mathf.Abs(originalScale.y),
            Mathf.Abs(originalScale.z) * (Random.value > 0.5f ? -1 : 1)
        );
    }

    void PropagateConstraints(Vector2Int cell)
    {
        // �� �ֺ��� ���� ������ �����Ͽ� ���ɼ� ���
        Queue<Vector2Int> cellsToUpdate = new Queue<Vector2Int>();
        cellsToUpdate.Enqueue(cell);

        while (cellsToUpdate.Count > 0)
        {
            Vector2Int current = cellsToUpdate.Dequeue();
            if (grid[current.x, current.y] == null || grid[current.x, current.y].Count == 0) { cellsToUpdate.Enqueue(current); continue; }
            if (grid[current.x, current.y].Count == 0) return;
            if (grid[current.x, current.y].Count == 0) return;
            MapTile currentTile = grid[current.x, current.y][0];

            // �� ���⿡ ���ؼ� ȣȯ���� �ʴ� Ÿ�ϵ��� ����
            UpdateNeighbor(current, Vector2Int.up, currentTile.northEdge, cellsToUpdate);
            UpdateNeighbor(current, Vector2Int.right, currentTile.eastEdge, cellsToUpdate);
            UpdateNeighbor(current, Vector2Int.down, currentTile.southEdge, cellsToUpdate);
            UpdateNeighbor(current, Vector2Int.left, currentTile.westEdge, cellsToUpdate);
        }
    }

    void UpdateNeighbor(Vector2Int cell, Vector2Int direction, TileEdge currentEdge, Queue<Vector2Int> cellsToUpdate)
    {
        Vector2Int neighbor = cell + direction;

        if (neighbor.x < 0 || neighbor.x >= gridWidth || neighbor.y < 0 || neighbor.y >= gridHeight)
            return;  // ���� ������ ����� ��� ����

        List<MapTile> possibleTiles = grid[neighbor.x, neighbor.y];
        int initialCount = possibleTiles.Count;

        if (possibleTiles == null) return;
        if (possibleTiles.Count == 0) return;
        possibleTiles.RemoveAll(tile => !currentEdge.compatibleEdgeTypes.Contains(tile.northEdge.edgeType));
        if (possibleTiles.Count == 0) possibleTiles.AddRange(tiles); // ������ Ÿ���� ������ ��ü Ÿ�� �ٽ� �߰�
        if (possibleTiles.Count == 0) return;
        if (possibleTiles.Count == 0) return;

        if (possibleTiles.Count != initialCount)
        {
            // ������ �Ͼ�ٸ� �ٽ� ť�� �߰��Ͽ� �߰����� ���� �ʿ�
            cellsToUpdate.Enqueue(neighbor);
        }
    }
}
