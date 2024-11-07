using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CustomNamespace
{
    [System.Serializable]
    public class TileEdge
    {
        public string edgeType;  // ���� Ÿ���� �� Ÿ��
        public List<string> compatibleEdgeTypes;  // ȣȯ ������ �� Ÿ�� ����Ʈ
    }

    [System.Serializable]
    public class MapTile
    {
        public string name;           // Ÿ�� �̸� �Ǵ� ID
        public GameObject prefab;     // Ÿ�� ������
        public string terrainType;    // ���� Ÿ�� (��: Desert, Forest, Glacier, Volcano)

        // �� Ÿ���� �� (����, ����, ����, ����)
        public TileEdge northEdge;
        public TileEdge eastEdge;
        public TileEdge southEdge;
        public TileEdge westEdge;

        // ������
        public MapTile(string name, GameObject prefab, string terrainType, TileEdge northEdge, TileEdge eastEdge, TileEdge southEdge, TileEdge westEdge)
        {
            this.name = name;
            this.prefab = prefab;
            this.terrainType = terrainType;
            this.northEdge = northEdge;
            this.eastEdge = eastEdge;
            this.southEdge = southEdge;
            this.westEdge = westEdge;
        }
    }

    public class Cell
    {
        public Vector2Int position;               // ���� �׸��� ��ġ
        public List<MapTile> possibleTiles;       // ���� ��ġ�� �� �ִ� ������ Ÿ�� ���
        public bool isCollapsed = false;          // ���� ���� (Ÿ���� ��ġ�Ǿ����� ����)
        public GameObject instantiatedTile;       // ��ġ�� Ÿ�� �ν��Ͻ�

        public int Entropy => possibleTiles.Count; // ��Ʈ���� ��� (������ Ÿ�� ��)
    }

    public static class Direction
    {
        public static readonly Vector2Int North = new Vector2Int(0, 1);  // +Y
        public static readonly Vector2Int East = new Vector2Int(1, 0);   // +X
        public static readonly Vector2Int South = new Vector2Int(0, -1); // -Y
        public static readonly Vector2Int West = new Vector2Int(-1, 0);  // -X

        public static readonly Vector2Int[] AllDirections = { North, East, South, West };

        public static string GetOpposite(string dir)
        {
            switch (dir)
            {
                case "North": return "South";
                case "East": return "West";
                case "South": return "North";
                case "West": return "East";
                default: return null;
            }
        }

        public static string VectorToDirection(Vector2Int vec)
        {
            if (vec == North) return "North";
            if (vec == East) return "East";
            if (vec == South) return "South";
            if (vec == West) return "West";
            return null;
        }
    }

    public class InfiniteWFC : MonoBehaviour
    {
        public Transform player; // �÷��̾��� Transform
        public int tileSize = 20; // Ÿ�� ũ��
        public float perlinScale = 10.0f; // Perlin Noise ������
        public float maxHeight = 5.0f; // �ִ� ����

        private Dictionary<string, List<MapTile>> terrainTileSets = new Dictionary<string, List<MapTile>>();
        private Dictionary<Vector2Int, Cell> cells = new Dictionary<Vector2Int, Cell>();
        private Stack<Cell> backtrackStack = new Stack<Cell>(); // ��Ʈ�� ����

        void Start()
        {
            InitializeTiles(); // Ÿ�� �ʱ�ȭ
            GenerateInitialCells();
        }

        void Update()
        {
            UpdateCells(); // �÷��̾ ������ ������ �� ������Ʈ
        }

        void InitializeTiles()
        {
            // Ÿ�� ������ �ҷ�����
            // desert
            GameObject desertPrefab1 = Resources.Load<GameObject>("sand/Tile3_1");
            GameObject desertPrefab2 = Resources.Load<GameObject>("sand/Tile3_2");
            GameObject desertPrefab3 = Resources.Load<GameObject>("sand/Tile3_3");
            GameObject desertPrefab4 = Resources.Load<GameObject>("sand/Tile3_4");
            GameObject desertPrefab5 = Resources.Load<GameObject>("sand/Tile3_5");
            GameObject desertPrefab6 = Resources.Load<GameObject>("sand/Tile3_6");
            GameObject desertPrefab7 = Resources.Load<GameObject>("sand/Tile3_7");
            GameObject desertPrefab8 = Resources.Load<GameObject>("sand/Tile3_8");
            GameObject desertPrefab9 = Resources.Load<GameObject>("sand/Tile3_9");
            GameObject desertPrefab10 = Resources.Load<GameObject>("sand/Tile3_10");
            GameObject desertPrefab11 = Resources.Load<GameObject>("sand/Tile3_11");
            GameObject desertPrefab12 = Resources.Load<GameObject>("sand/Tile3_12");
            GameObject desertPrefab13 = Resources.Load<GameObject>("sand/Tile3_13");
            GameObject desertPrefab14 = Resources.Load<GameObject>("sand/Tile3_14");
            GameObject desertPrefab15 = Resources.Load<GameObject>("sand/Tile3_15");

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
            GameObject forestPrefab11 = Resources.Load<GameObject>("forest/Tile1_11");
            GameObject forestPrefab12 = Resources.Load<GameObject>("forest/Tile1_12");
            GameObject forestPrefab13 = Resources.Load<GameObject>("forest/Tile1_13");
            GameObject forestPrefab14 = Resources.Load<GameObject>("forest/Tile1_14");
            GameObject forestPrefab15 = Resources.Load<GameObject>("forest/Tile1_15");
            GameObject forestPrefab16 = Resources.Load<GameObject>("forest/Tile1_16");
            GameObject forestPrefab17 = Resources.Load<GameObject>("forest/Tile1_17");
            GameObject forestPrefab18 = Resources.Load<GameObject>("forest/Tile1_18");
            GameObject forestPrefab19 = Resources.Load<GameObject>("forest/Tile1_19");

            // glacier
            GameObject glacierPrefab1 = Resources.Load<GameObject>("snow/Tile4_1");
            GameObject glacierPrefab2 = Resources.Load<GameObject>("snow/Tile4_2");
            GameObject glacierPrefab3 = Resources.Load<GameObject>("snow/Tile4_3");
            GameObject glacierPrefab4 = Resources.Load<GameObject>("snow/Tile4_4");
            GameObject glacierPrefab5 = Resources.Load<GameObject>("snow/Tile4_5");
            GameObject glacierPrefab6 = Resources.Load<GameObject>("snow/Tile4_6");
            GameObject glacierPrefab7 = Resources.Load<GameObject>("snow/Tile4_7");
            GameObject glacierPrefab8 = Resources.Load<GameObject>("snow/Tile4_8");
            GameObject glacierPrefab9 = Resources.Load<GameObject>("snow/Tile4_9");
            GameObject glacierPrefab10 = Resources.Load<GameObject>("snow/Tile4_10");
            GameObject glacierPrefab11 = Resources.Load<GameObject>("snow/Tile4_11");
            GameObject glacierPrefab12 = Resources.Load<GameObject>("snow/Tile4_12");
            GameObject glacierPrefab13 = Resources.Load<GameObject>("snow/Tile4_13");
            GameObject glacierPrefab14 = Resources.Load<GameObject>("snow/Tile4_14");


            //volcano
            GameObject volcanoPrefab1 = Resources.Load<GameObject>("lava/Tile2_1");
            GameObject volcanoPrefab2 = Resources.Load<GameObject>("lava/Tile2_2");
            GameObject volcanoPrefab3 = Resources.Load<GameObject>("lava/Tile2_3");
            GameObject volcanoPrefab4 = Resources.Load<GameObject>("lava/Tile2_4");
            GameObject volcanoPrefab5 = Resources.Load<GameObject>("lava/Tile2_5");
            GameObject volcanoPrefab6 = Resources.Load<GameObject>("lava/Tile2_6");
            GameObject volcanoPrefab7 = Resources.Load<GameObject>("lava/Tile2_7");
            GameObject volcanoPrefab8 = Resources.Load<GameObject>("lava/Tile2_8");
            GameObject volcanoPrefab9 = Resources.Load<GameObject>("lava/Tile2_9");
            GameObject volcanoPrefab10 = Resources.Load<GameObject>("lava/Tile2_10");


            // �� Ÿ���� �� ����
            // forest tile   1: north 2: west 3: south 4: east
            TileEdge a1_1 = new TileEdge { edgeType = "a1_1", compatibleEdgeTypes = new List<string> { "a1_1", "a1_3", "a1_3", "c1_1", "c1_3", "b1_1", "b1_3", "b1_4", "d1_1", "d1_3", "e1_3", "e1_4", "f1_3", "f1_1", "g1_1", "g1_2", "g1_3", "g1_4", "h1_1", "h1_2", "h1_3", "h1_4", "i1_1", "i1_2", "i1_3", "i1_4", "j1_1", "j1_2", "j1_3", "j1_4", "k1_1", "k1_2", "k1_3", "k1_4", "l1_1", "l1_2", "l1_3", "l1_4", "m1_2", "m1_3", "m1_4", "n1_2", "n1_3", "n1_4", "o1_2", "o1_4", "p1_1", "p1_2", "p1_3", "p1_4", "q1_3", "q1_4", "r1_3", "r1_4", "s1_1", "s1_2", "s1_3", "s1_4" } };
            TileEdge a1_2 = new TileEdge { edgeType = "a1_2", compatibleEdgeTypes = new List<string> { "b1_2", "c1_4", "d1_4", "e1_2", "f1_2", "f1_4" } };
            TileEdge a1_3 = new TileEdge { edgeType = "a1_3", compatibleEdgeTypes = new List<string> { "a1_1", "a1_3", "a1_3", "c1_1", "c1_3", "b1_1", "b1_3", "b1_4", "d1_1", "d1_3", "e1_3", "e1_4", "f1_3", "f1_1", "g1_1", "g1_2", "g1_3", "g1_4", "h1_1", "h1_2", "h1_3", "h1_4", "i1_1", "i1_2", "i1_3", "i1_4", "j1_1", "j1_2", "j1_3", "j1_4", "k1_1", "k1_2", "k1_3", "k1_4", "l1_1", "l1_2", "l1_3", "l1_4", "m1_2", "m1_3", "m1_4", "n1_2", "n1_3", "n1_4", "o1_2", "o1_4", "p1_1", "p1_2", "p1_3", "p1_4", "q1_3", "q1_4", "r1_3", "r1_4", "s1_1", "s1_2", "s1_3", "s1_4" } };
            TileEdge a1_4 = new TileEdge { edgeType = "a1_4", compatibleEdgeTypes = new List<string> { "a1_1", "a1_3", "a1_3", "c1_1", "c1_3", "b1_1", "b1_3", "b1_4", "d1_1", "d1_3", "e1_3", "e1_4", "f1_3", "f1_1", "g1_1", "g1_2", "g1_3", "g1_4", "h1_1", "h1_2", "h1_3", "h1_4", "i1_1", "i1_2", "i1_3", "i1_4", "j1_1", "j1_2", "j1_3", "j1_4", "k1_1", "k1_2", "k1_3", "k1_4", "l1_1", "l1_2", "l1_3", "l1_4", "m1_2", "m1_3", "m1_4", "n1_2", "n1_3", "n1_4", "o1_2", "o1_4", "p1_1", "p1_2", "p1_3", "p1_4", "q1_3", "q1_4", "r1_3", "r1_4", "s1_1", "s1_2", "s1_3", "s1_4" } };
            TileEdge b1_1 = new TileEdge { edgeType = "b1_1", compatibleEdgeTypes = new List<string> { "a1_1", "a1_3", "a1_3", "c1_1", "c1_3", "b1_1", "b1_3", "b1_4", "d1_1", "d1_3", "e1_3", "e1_4", "f1_3", "f1_1", "g1_1", "g1_2", "g1_3", "g1_4", "h1_1", "h1_2", "h1_3", "h1_4", "i1_1", "i1_2", "i1_3", "i1_4", "j1_1", "j1_2", "j1_3", "j1_4", "k1_1", "k1_2", "k1_3", "k1_4", "l1_1", "l1_2", "l1_3", "l1_4", "m1_2", "m1_3", "m1_4", "n1_2", "n1_3", "n1_4", "o1_2", "o1_4", "p1_1", "p1_2", "p1_3", "p1_4", "q1_3", "q1_4", "r1_3", "r1_4", "s1_1", "s1_2", "s1_3", "s1_4", "n1_1", "o1_3", "r1_1" } };
            TileEdge b1_2 = new TileEdge { edgeType = "b1_2", compatibleEdgeTypes = new List<string> { "c1_2", "d1_2", "e1_1", "a1_2" } };
            TileEdge b1_3 = new TileEdge { edgeType = "b1_3", compatibleEdgeTypes = new List<string> { "a1_1", "a1_3", "a1_3", "c1_1", "c1_3", "b1_1", "b1_3", "b1_4", "d1_1", "d1_3", "e1_3", "e1_4", "f1_3", "f1_1", "g1_1", "g1_2", "g1_3", "g1_4", "h1_1", "h1_2", "h1_3", "h1_4", "i1_1", "i1_2", "i1_3", "i1_4", "j1_1", "j1_2", "j1_3", "j1_4", "k1_1", "k1_2", "k1_3", "k1_4", "l1_1", "l1_2", "l1_3", "l1_4", "m1_2", "m1_3", "m1_4", "n1_2", "n1_3", "n1_4", "o1_2", "o1_4", "p1_1", "p1_2", "p1_3", "p1_4", "q1_3", "q1_4", "r1_3", "r1_4", "s1_1", "s1_2", "s1_3", "s1_4" } };
            TileEdge b1_4 = new TileEdge { edgeType = "b1_4", compatibleEdgeTypes = new List<string> { "a1_1", "a1_3", "a1_3", "c1_1", "c1_3", "b1_1", "b1_3", "b1_4", "d1_1", "d1_3", "e1_3", "e1_4", "f1_3", "f1_1", "g1_1", "g1_2", "g1_3", "g1_4", "h1_1", "h1_2", "h1_3", "h1_4", "i1_1", "i1_2", "i1_3", "i1_4", "j1_1", "j1_2", "j1_3", "j1_4", "k1_1", "k1_2", "k1_3", "k1_4", "l1_1", "l1_2", "l1_3", "l1_4", "m1_2", "m1_3", "m1_4", "n1_2", "n1_3", "n1_4", "o1_2", "o1_4", "p1_1", "p1_2", "p1_3", "p1_4", "q1_3", "q1_4", "r1_3", "r1_4", "s1_1", "s1_2", "s1_3", "s1_4" } };
            TileEdge c1_1 = new TileEdge { edgeType = "c1_1", compatibleEdgeTypes = new List<string> { "g1_4", "h1_4", "i1_4", "j1_4", "l1_1", "l1_2", "l1_3", "l1_4" } };
            TileEdge c1_2 = new TileEdge { edgeType = "c1_2", compatibleEdgeTypes = new List<string> { "b1_2", "c1_4", "d1_4", "e1_2", "f1_2", "f1_4" } };
            TileEdge c1_3 = new TileEdge { edgeType = "c1_3", compatibleEdgeTypes = new List<string> { "a1_1", "a1_3", "a1_3", "c1_1", "c1_3", "b1_1", "b1_3", "b1_4", "d1_1", "d1_3", "e1_3", "e1_4", "f1_3", "f1_1", "g1_1", "g1_2", "g1_3", "g1_4", "h1_1", "h1_2", "h1_3", "h1_4", "i1_1", "i1_2", "i1_3", "i1_4", "j1_1", "j1_2", "j1_3", "j1_4", "k1_1", "k1_2", "k1_3", "k1_4", "l1_1", "l1_2", "l1_3", "l1_4", "m1_2", "m1_3", "m1_4", "n1_2", "n1_3", "n1_4", "o1_2", "o1_4", "p1_1", "p1_2", "p1_3", "p1_4", "q1_3", "q1_4", "r1_3", "r1_4", "s1_1", "s1_2", "s1_3", "s1_4", "n1_1", "o1_3", "r1_1" } };
            TileEdge c1_4 = new TileEdge { edgeType = "c1_4", compatibleEdgeTypes = new List<string> { "d1_3", "c1_2", "a1_2", "e1_1" } };
            TileEdge d1_1 = new TileEdge { edgeType = "d1_1", compatibleEdgeTypes = new List<string> { "a1_1", "a1_3", "a1_3", "c1_1", "c1_3", "b1_1", "b1_3", "b1_4", "d1_1", "d1_3", "e1_3", "e1_4", "f1_3", "f1_1", "g1_1", "g1_2", "g1_3", "g1_4", "h1_1", "h1_2", "h1_3", "h1_4", "i1_1", "i1_2", "i1_3", "i1_4", "j1_1", "j1_2", "j1_3", "j1_4", "k1_1", "k1_2", "k1_3", "k1_4", "l1_1", "l1_2", "l1_3", "l1_4", "m1_2", "m1_3", "m1_4", "n1_2", "n1_3", "n1_4", "o1_2", "o1_4", "p1_1", "p1_2", "p1_3", "p1_4", "q1_3", "q1_4", "r1_3", "r1_4", "s1_1", "s1_2", "s1_3", "s1_4" } };
            TileEdge d1_2 = new TileEdge { edgeType = "d1_2", compatibleEdgeTypes = new List<string> { "a1_3", "a1_4", "c1_3", "c1_2", "b1_2", "b1_3", "b1_4", "e1_3", "e1_4", "f1_3", "f1_2", "g1_1", "g1_2", "g1_3", "g1_4", "h1_1", "h1_2", "h1_3", "h1_4", "i1_1", "i1_2", "i1_3", "i1_4", "j1_1", "j1_2", "j1_3", "j1_4", "k1_1", "k1_2", "k1_3", "l1_1", "l1_2", "l1_3", "l1_4", "m1_1", "m1_2", "m1_3", "m1_4", "n1_1", "n1_3", "n1_4", "o1_1", "o1_2", "o1_4", "p1_1", "p1_2", "p1_3", "p1_4", "q1_3", "q1_4", "r1_1", "r1_3", "r1_4", "s1_1", "s1_2", "s1_3", "s1_4" } };
            TileEdge d1_3 = new TileEdge { edgeType = "d1_3", compatibleEdgeTypes = new List<string> { "a1_3", "a1_4", "c1_3", "c1_2", "b1_2", "b1_3", "b1_4", "e1_3", "e1_4", "f1_3", "f1_2", "g1_1", "g1_2", "g1_3", "g1_4", "h1_1", "h1_2", "h1_3", "h1_4", "i1_1", "i1_2", "i1_3", "i1_4", "j1_1", "j1_2", "j1_3", "j1_4", "k1_1", "k1_2", "k1_3", "l1_1", "l1_2", "l1_3", "l1_4", "m1_1", "m1_2", "m1_3", "m1_4", "n1_1", "n1_2", "n1_3", "n1_4", "o1_1", "o1_2", "o1_4", "p1_1", "p1_2", "p1_3", "p1_4", "q1_1", "q1_2", "q1_3", "q1_4", "r1_1", "r1_2", "r1_3", "r1_4", "s1_1", "s1_2", "s1_3", "s1_4" } };
            TileEdge d1_4 = new TileEdge { edgeType = "d1_4", compatibleEdgeTypes = new List<string> { "a1_2", "i1_2", "d1_1", "e1_2", "c1_4" } };
            TileEdge e1_1 = new TileEdge { edgeType = "e1_1", compatibleEdgeTypes = new List<string> { "b1_2", "c1_4", "d1_4", "e1_2", "f1_2", "f1_4" } };
            TileEdge e1_2 = new TileEdge { edgeType = "e1_2", compatibleEdgeTypes = new List<string> { "a1_2", "c1_2", "d1_2", "e1_1" } };
            TileEdge e1_3 = new TileEdge { edgeType = "e1_3", compatibleEdgeTypes = new List<string> { "a1_1", "a1_3", "a1_3", "c1_1", "c1_3", "b1_1", "b1_3", "b1_4", "d1_1", "d1_3", "e1_3", "e1_4", "f1_3", "f1_1", "g1_1", "g1_2", "g1_3", "g1_4", "h1_1", "h1_2", "h1_3", "h1_4", "i1_1", "i1_2", "i1_3", "i1_4", "j1_1", "j1_2", "j1_3", "j1_4", "k1_1", "k1_2", "k1_3", "k1_4", "l1_1", "l1_2", "l1_3", "l1_4", "m1_2", "m1_3", "m1_4", "n1_2", "n1_3", "n1_4", "o1_2", "o1_4", "p1_1", "p1_2", "p1_3", "p1_4", "q1_3", "q1_4", "r1_3", "r1_4", "s1_1", "s1_2", "s1_3", "s1_4" } };
            TileEdge e1_4 = new TileEdge { edgeType = "e1_4", compatibleEdgeTypes = new List<string> { "a1_1", "a1_3", "a1_3", "c1_1", "c1_3", "b1_1", "b1_3", "b1_4", "d1_1", "d1_3", "e1_3", "e1_4", "f1_3", "f1_1", "g1_1", "g1_2", "g1_3", "g1_4", "h1_1", "h1_2", "h1_3", "h1_4", "i1_1", "i1_2", "i1_3", "i1_4", "j1_1", "j1_2", "j1_3", "j1_4", "k1_1", "k1_2", "k1_3", "k1_4", "l1_1", "l1_2", "l1_3", "l1_4", "m1_2", "m1_3", "m1_4", "n1_2", "n1_3", "n1_4", "o1_2", "o1_4", "p1_1", "p1_2", "p1_3", "p1_4", "q1_3", "q1_4", "r1_3", "r1_4", "s1_1", "s1_2", "s1_3", "s1_4" } };
            TileEdge f1_1 = new TileEdge { edgeType = "f1_1", compatibleEdgeTypes = new List<string> { "a1_1", "a1_3", "a1_3", "c1_1", "c1_3", "b1_1", "b1_3", "b1_4", "d1_1", "d1_3", "e1_3", "e1_4", "f1_3", "f1_1", "g1_1", "g1_2", "g1_3", "g1_4", "h1_1", "h1_2", "h1_3", "h1_4", "i1_1", "i1_2", "i1_3", "i1_4", "j1_1", "j1_2", "j1_3", "j1_4", "k1_1", "k1_2", "k1_3", "k1_4", "l1_1", "l1_2", "l1_3", "l1_4", "m1_2", "m1_3", "m1_4", "n1_2", "n1_3", "n1_4", "o1_2", "o1_4", "p1_1", "p1_2", "p1_3", "p1_4", "q1_3", "q1_4", "r1_3", "r1_4", "s1_1", "s1_2", "s1_3", "s1_4" } };
            TileEdge f1_2 = new TileEdge { edgeType = "f1_2", compatibleEdgeTypes = new List<string> { "a1_2", "c1_2", "d1_2", "e1_1" } };
            TileEdge f1_3 = new TileEdge { edgeType = "f1_3", compatibleEdgeTypes = new List<string> { "a1_1", "a1_3", "a1_3", "c1_1", "c1_3", "b1_1", "b1_3", "b1_4", "d1_1", "d1_3", "e1_3", "e1_4", "f1_3", "f1_1", "g1_1", "g1_2", "g1_3", "g1_4", "h1_1", "h1_2", "h1_3", "h1_4", "i1_1", "i1_2", "i1_3", "i1_4", "j1_1", "j1_2", "j1_3", "j1_4", "k1_1", "k1_2", "k1_3", "k1_4", "l1_1", "l1_2", "l1_3", "l1_4", "m1_2", "m1_3", "m1_4", "n1_2", "n1_3", "n1_4", "o1_2", "o1_4", "p1_1", "p1_2", "p1_3", "p1_4", "q1_3", "q1_4", "r1_3", "r1_4", "s1_1", "s1_2", "s1_3", "s1_4" } };
            TileEdge f1_4 = new TileEdge { edgeType = "f1_4", compatibleEdgeTypes = new List<string> { "a1_2", "c1_2", "d1_2", "e1_1" } };
            TileEdge g1_1 = new TileEdge { edgeType = "g1_1", compatibleEdgeTypes = new List<string> { "a1_1", "a1_3", "a1_3", "c1_1", "c1_3", "b1_1", "b1_3", "b1_4", "d1_1", "d1_3", "e1_3", "e1_4", "f1_3", "f1_1", "k1_1", "k1_2", "k1_3", "k1_4", "l1_1", "l1_2", "l1_3", "l1_4", "m1_2", "m1_3", "m1_4", "n1_2", "n1_3", "n1_4", "o1_2", "o1_4", "q1_3", "q1_4", "r1_3", "r1_4", "s1_1", "s1_2", "s1_3", "s1_4" } };
            TileEdge g1_2 = new TileEdge { edgeType = "g1_2", compatibleEdgeTypes = new List<string> { "a1_1", "a1_3", "a1_3", "c1_1", "c1_3", "b1_1", "b1_3", "b1_4", "d1_1", "d1_3", "e1_3", "e1_4", "f1_3", "f1_1", "k1_1", "k1_2", "k1_3", "k1_4", "l1_1", "l1_2", "l1_3", "l1_4", "m1_2", "m1_3", "m1_4", "n1_2", "n1_3", "n1_4", "o1_2", "o1_4", "q1_3", "q1_4", "r1_3", "r1_4", "s1_1", "s1_2", "s1_3", "s1_4" } };
            TileEdge g1_3 = new TileEdge { edgeType = "g1_3", compatibleEdgeTypes = new List<string> { "a1_1", "a1_3", "a1_3", "c1_1", "c1_3", "b1_1", "b1_3", "b1_4", "d1_1", "d1_3", "e1_3", "e1_4", "f1_3", "f1_1", "k1_1", "k1_2", "k1_3", "k1_4", "l1_1", "l1_2", "l1_3", "l1_4", "m1_2", "m1_3", "m1_4", "n1_2", "n1_3", "n1_4", "o1_2", "o1_4", "q1_3", "q1_4", "r1_3", "r1_4", "s1_1", "s1_2", "s1_3", "s1_4" } };
            TileEdge g1_4 = new TileEdge { edgeType = "g1_4", compatibleEdgeTypes = new List<string> { "a1_1", "a1_3", "a1_3", "c1_1", "c1_3", "b1_1", "b1_3", "b1_4", "d1_1", "d1_3", "e1_3", "e1_4", "f1_3", "f1_1", "k1_1", "k1_2", "k1_3", "k1_4", "l1_1", "l1_2", "l1_3", "l1_4", "m1_2", "m1_3", "m1_4", "n1_2", "n1_3", "n1_4", "o1_2", "o1_4", "q1_3", "q1_4", "r1_3", "r1_4", "s1_1", "s1_2", "s1_3", "s1_4" } };
            TileEdge h1_1 = new TileEdge { edgeType = "h1_1", compatibleEdgeTypes = new List<string> { "a1_1", "a1_3", "a1_3", "c1_1", "c1_3", "b1_1", "b1_3", "b1_4", "d1_1", "d1_3", "e1_3", "e1_4", "f1_3", "f1_1", "k1_1", "k1_2", "k1_3", "k1_4", "l1_1", "l1_2", "l1_3", "l1_4", "m1_2", "m1_3", "m1_4", "n1_2", "n1_3", "n1_4", "o1_2", "o1_4", "q1_3", "q1_4", "r1_3", "r1_4", "s1_1", "s1_2", "s1_3", "s1_4" } };
            TileEdge h1_2 = new TileEdge { edgeType = "h1_2", compatibleEdgeTypes = new List<string> { "a1_1", "a1_3", "a1_3", "c1_1", "c1_3", "b1_1", "b1_3", "b1_4", "d1_1", "d1_3", "e1_3", "e1_4", "f1_3", "f1_1", "k1_1", "k1_2", "k1_3", "k1_4", "l1_1", "l1_2", "l1_3", "l1_4", "m1_2", "m1_3", "m1_4", "n1_2", "n1_3", "n1_4", "o1_2", "o1_4", "q1_3", "q1_4", "r1_3", "r1_4", "s1_1", "s1_2", "s1_3", "s1_4" } };
            TileEdge h1_3 = new TileEdge { edgeType = "h1_3", compatibleEdgeTypes = new List<string> { "a1_1", "a1_3", "a1_3", "c1_1", "c1_3", "b1_1", "b1_3", "b1_4", "d1_1", "d1_3", "e1_3", "e1_4", "f1_3", "f1_1", "k1_1", "k1_2", "k1_3", "k1_4", "l1_1", "l1_2", "l1_3", "l1_4", "m1_2", "m1_3", "m1_4", "n1_2", "n1_3", "n1_4", "o1_2", "o1_4", "q1_3", "q1_4", "r1_3", "r1_4", "s1_1", "s1_2", "s1_3", "s1_4" } };
            TileEdge h1_4 = new TileEdge { edgeType = "h1_4", compatibleEdgeTypes = new List<string> { "a1_1", "a1_3", "a1_3", "c1_1", "c1_3", "b1_1", "b1_3", "b1_4", "d1_1", "d1_3", "e1_3", "e1_4", "f1_3", "f1_1", "k1_1", "k1_2", "k1_3", "k1_4", "l1_1", "l1_2", "l1_3", "l1_4", "m1_2", "m1_3", "m1_4", "n1_2", "n1_3", "n1_4", "o1_2", "o1_4", "q1_3", "q1_4", "r1_3", "r1_4", "s1_1", "s1_2", "s1_3", "s1_4" } };
            TileEdge i1_1 = new TileEdge { edgeType = "i1_1", compatibleEdgeTypes = new List<string> { "a1_1", "a1_3", "a1_3", "c1_1", "c1_3", "b1_1", "b1_3", "b1_4", "d1_1", "d1_3", "e1_3", "e1_4", "f1_3", "f1_1", "k1_1", "k1_2", "k1_3", "k1_4", "l1_1", "l1_2", "l1_3", "l1_4", "m1_2", "m1_3", "m1_4", "n1_2", "n1_3", "n1_4", "o1_2", "o1_4", "q1_3", "q1_4", "r1_3", "r1_4", "s1_1", "s1_2", "s1_3", "s1_4" } };
            TileEdge i1_2 = new TileEdge { edgeType = "i1_2", compatibleEdgeTypes = new List<string> { "a1_1", "a1_3", "a1_3", "c1_1", "c1_3", "b1_1", "b1_3", "b1_4", "d1_1", "d1_3", "e1_3", "e1_4", "f1_3", "f1_1", "k1_1", "k1_2", "k1_3", "k1_4", "l1_1", "l1_2", "l1_3", "l1_4", "m1_2", "m1_3", "m1_4", "n1_2", "n1_3", "n1_4", "o1_2", "o1_4", "q1_3", "q1_4", "r1_3", "r1_4", "s1_1", "s1_2", "s1_3", "s1_4" } };
            TileEdge i1_3 = new TileEdge { edgeType = "i1_3", compatibleEdgeTypes = new List<string> { "a1_1", "a1_3", "a1_3", "c1_1", "c1_3", "b1_1", "b1_3", "b1_4", "d1_1", "d1_3", "e1_3", "e1_4", "f1_3", "f1_1", "k1_1", "k1_2", "k1_3", "k1_4", "l1_1", "l1_2", "l1_3", "l1_4", "m1_2", "m1_3", "m1_4", "n1_2", "n1_3", "n1_4", "o1_2", "o1_4", "q1_3", "q1_4", "r1_3", "r1_4", "s1_1", "s1_2", "s1_3", "s1_4" } };
            TileEdge i1_4 = new TileEdge { edgeType = "i1_4", compatibleEdgeTypes = new List<string> { "a1_1", "a1_3", "a1_3", "c1_1", "c1_3", "b1_1", "b1_3", "b1_4", "d1_1", "d1_3", "e1_3", "e1_4", "f1_3", "f1_1", "k1_1", "k1_2", "k1_3", "k1_4", "l1_1", "l1_2", "l1_3", "l1_4", "m1_2", "m1_3", "m1_4", "n1_2", "n1_3", "n1_4", "o1_2", "o1_4", "q1_3", "q1_4", "r1_3", "r1_4", "s1_1", "s1_2", "s1_3", "s1_4" } };
            TileEdge j1_1 = new TileEdge { edgeType = "j1_1", compatibleEdgeTypes = new List<string> { "a1_1", "a1_3", "a1_3", "c1_1", "c1_3", "b1_1", "b1_3", "b1_4", "d1_1", "d1_3", "e1_3", "e1_4", "f1_3", "f1_1", "k1_1", "k1_2", "k1_3", "k1_4", "l1_1", "l1_2", "l1_3", "l1_4", "m1_2", "m1_3", "m1_4", "n1_2", "n1_3", "n1_4", "o1_2", "o1_4", "q1_3", "q1_4", "r1_3", "r1_4", "s1_1", "s1_2", "s1_3", "s1_4" } };
            TileEdge j1_2 = new TileEdge { edgeType = "j1_2", compatibleEdgeTypes = new List<string> { "a1_1", "a1_3", "a1_3", "c1_1", "c1_3", "b1_1", "b1_3", "b1_4", "d1_1", "d1_3", "e1_3", "e1_4", "f1_3", "f1_1", "k1_1", "k1_2", "k1_3", "k1_4", "l1_1", "l1_2", "l1_3", "l1_4", "m1_2", "m1_3", "m1_4", "n1_2", "n1_3", "n1_4", "o1_2", "o1_4", "q1_3", "q1_4", "r1_3", "r1_4", "s1_1", "s1_2", "s1_3", "s1_4" } };
            TileEdge j1_3 = new TileEdge { edgeType = "j1_3", compatibleEdgeTypes = new List<string> { "a1_1", "a1_3", "a1_3", "c1_1", "c1_3", "b1_1", "b1_3", "b1_4", "d1_1", "d1_3", "e1_3", "e1_4", "f1_3", "f1_1", "k1_1", "k1_2", "k1_3", "k1_4", "l1_1", "l1_2", "l1_3", "l1_4", "m1_2", "m1_3", "m1_4", "n1_2", "n1_3", "n1_4", "o1_2", "o1_4", "q1_3", "q1_4", "r1_3", "r1_4", "s1_1", "s1_2", "s1_3", "s1_4" } };
            TileEdge j1_4 = new TileEdge { edgeType = "j1_4", compatibleEdgeTypes = new List<string> { "a1_1", "a1_3", "a1_3", "c1_1", "c1_3", "b1_1", "b1_3", "b1_4", "d1_1", "d1_3", "e1_3", "e1_4", "f1_3", "f1_1", "k1_1", "k1_2", "k1_3", "k1_4", "l1_1", "l1_2", "l1_3", "l1_4", "m1_2", "m1_3", "m1_4", "n1_2", "n1_3", "n1_4", "o1_2", "o1_4", "q1_3", "q1_4", "r1_3", "r1_4", "s1_1", "s1_2", "s1_3", "s1_4" } };
            TileEdge k1_1 = new TileEdge { edgeType = "k1_1", compatibleEdgeTypes = new List<string> { "a1_1", "a1_3", "a1_3", "c1_1", "c1_3", "b1_1", "b1_3", "b1_4", "d1_1", "d1_3", "e1_3", "e1_4", "f1_3", "f1_1", "k1_1", "k1_2", "k1_3", "k1_4", "l1_1", "l1_2", "l1_3", "l1_4", "m1_2", "m1_3", "m1_4", "n1_2", "n1_3", "n1_4", "o1_2", "o1_4", "q1_3", "q1_4", "r1_3", "r1_4", "s1_1", "s1_2", "s1_3", "s1_4" } };
            TileEdge k1_2 = new TileEdge { edgeType = "k1_2", compatibleEdgeTypes = new List<string> { "a1_1", "a1_3", "a1_3", "c1_1", "c1_3", "b1_1", "b1_3", "b1_4", "d1_1", "d1_3", "e1_3", "e1_4", "f1_3", "f1_1", "k1_1", "k1_2", "k1_3", "k1_4", "l1_1", "l1_2", "l1_3", "l1_4", "m1_2", "m1_3", "m1_4", "n1_2", "n1_3", "n1_4", "o1_2", "o1_4", "q1_3", "q1_4", "r1_3", "r1_4", "s1_1", "s1_2", "s1_3", "s1_4" } };
            TileEdge k1_3 = new TileEdge { edgeType = "k1_3", compatibleEdgeTypes = new List<string> { "a1_1", "a1_3", "a1_3", "c1_1", "c1_3", "b1_1", "b1_3", "b1_4", "d1_1", "d1_3", "e1_3", "e1_4", "f1_3", "f1_1", "k1_1", "k1_2", "k1_3", "k1_4", "l1_1", "l1_2", "l1_3", "l1_4", "m1_2", "m1_3", "m1_4", "n1_2", "n1_3", "n1_4", "o1_2", "o1_4", "q1_3", "q1_4", "r1_3", "r1_4", "s1_1", "s1_2", "s1_3", "s1_4" } };
            TileEdge k1_4 = new TileEdge { edgeType = "k1_4", compatibleEdgeTypes = new List<string> { "a1_1", "a1_3", "a1_3", "c1_1", "c1_3", "b1_1", "b1_3", "b1_4", "d1_1", "d1_3", "e1_3", "e1_4", "f1_3", "f1_1", "k1_1", "k1_2", "k1_3", "k1_4", "l1_1", "l1_2", "l1_3", "l1_4", "m1_2", "m1_3", "m1_4", "n1_2", "n1_3", "n1_4", "o1_2", "o1_4", "q1_3", "q1_4", "r1_3", "r1_4", "s1_1", "s1_2", "s1_3", "s1_4" } };
            TileEdge l1_1 = new TileEdge { edgeType = "l1_1", compatibleEdgeTypes = new List<string> { "a1_1", "a1_3", "a1_3", "c1_1", "c1_3", "b1_1", "b1_3", "b1_4", "d1_1", "d1_3", "e1_3", "e1_4", "f1_3", "f1_1", "g1_1", "g1_2", "g1_3", "g1_4", "h1_1", "h1_2", "h1_3", "h1_4", "i1_1", "i1_2", "i1_3", "i1_4", "j1_1", "j1_2", "j1_3", "j1_4", "k1_1", "k1_2", "k1_3", "k1_4", "l1_1", "l1_2", "l1_3", "l1_4", "m1_2", "m1_3", "m1_4", "n1_2", "n1_3", "n1_4", "o1_2", "o1_4", "p1_1", "p1_2", "p1_3", "p1_4", "q1_3", "q1_4", "r1_3", "r1_4", "s1_1", "s1_2", "s1_3", "s1_4", "n1_1", "o1_3", "r1_1", "m1_1", "g1_1", "g1_2" } };
            TileEdge l1_2 = new TileEdge { edgeType = "l1_2", compatibleEdgeTypes = new List<string> { "a1_1", "a1_3", "a1_3", "c1_1", "c1_3", "b1_1", "b1_3", "b1_4", "d1_1", "d1_3", "e1_3", "e1_4", "f1_3", "f1_1", "g1_1", "g1_2", "g1_3", "g1_4", "h1_1", "h1_2", "h1_3", "h1_4", "i1_1", "i1_2", "i1_3", "i1_4", "j1_1", "j1_2", "j1_3", "j1_4", "k1_1", "k1_2", "k1_3", "k1_4", "l1_1", "l1_2", "l1_3", "l1_4", "m1_2", "m1_3", "m1_4", "n1_2", "n1_3", "n1_4", "o1_2", "o1_4", "p1_1", "p1_2", "p1_3", "p1_4", "q1_3", "q1_4", "r1_3", "r1_4", "s1_1", "s1_2", "s1_3", "s1_4", "n1_1", "o1_3", "r1_1", "m1_1", "g1_1", "g1_2" } };
            TileEdge l1_3 = new TileEdge { edgeType = "l1_3", compatibleEdgeTypes = new List<string> { "a1_1", "a1_3", "a1_3", "c1_1", "c1_3", "b1_1", "b1_3", "b1_4", "d1_1", "d1_3", "e1_3", "e1_4", "f1_3", "f1_1", "g1_1", "g1_2", "g1_3", "g1_4", "h1_1", "h1_2", "h1_3", "h1_4", "i1_1", "i1_2", "i1_3", "i1_4", "j1_1", "j1_2", "j1_3", "j1_4", "k1_1", "k1_2", "k1_3", "k1_4", "l1_1", "l1_2", "l1_3", "l1_4", "m1_2", "m1_3", "m1_4", "n1_2", "n1_3", "n1_4", "o1_2", "o1_4", "p1_1", "p1_2", "p1_3", "p1_4", "q1_3", "q1_4", "r1_3", "r1_4", "s1_1", "s1_2", "s1_3", "s1_4", "n1_1", "o1_3", "r1_1", "m1_1", "g1_1", "g1_2" } };
            TileEdge l1_4 = new TileEdge { edgeType = "l1_4", compatibleEdgeTypes = new List<string> { "a1_1", "a1_3", "a1_3", "c1_1", "c1_3", "b1_1", "b1_3", "b1_4", "d1_1", "d1_3", "e1_3", "e1_4", "f1_3", "f1_1", "g1_1", "g1_2", "g1_3", "g1_4", "h1_1", "h1_2", "h1_3", "h1_4", "i1_1", "i1_2", "i1_3", "i1_4", "j1_1", "j1_2", "j1_3", "j1_4", "k1_1", "k1_2", "k1_3", "k1_4", "l1_1", "l1_2", "l1_3", "l1_4", "m1_2", "m1_3", "m1_4", "n1_2", "n1_3", "n1_4", "o1_2", "o1_4", "p1_1", "p1_2", "p1_3", "p1_4", "q1_3", "q1_4", "r1_3", "r1_4", "s1_1", "s1_2", "s1_3", "s1_4", "n1_1", "o1_3", "r1_1", "m1_1", "g1_1", "g1_2" } };
            TileEdge m1_1 = new TileEdge { edgeType = "m1_1", compatibleEdgeTypes = new List<string> { "d1_3", "l1_1", "l1_2", "l1_3", "l1_4" } };
            TileEdge m1_2 = new TileEdge { edgeType = "m1_2", compatibleEdgeTypes = new List<string> { "a1_1", "a1_3", "a1_3", "c1_1", "c1_3", "b1_1", "b1_3", "b1_4", "d1_1", "d1_3", "e1_3", "e1_4", "f1_3", "f1_1", "g1_1", "g1_2", "g1_3", "g1_4", "h1_1", "h1_2", "h1_3", "h1_4", "i1_1", "i1_2", "i1_3", "i1_4", "j1_1", "j1_2", "j1_3", "j1_4", "k1_1", "k1_2", "k1_3", "k1_4", "l1_1", "l1_2", "l1_3", "l1_4", "m1_2", "m1_3", "m1_4", "n1_2", "n1_3", "n1_4", "o1_2", "o1_4", "p1_1", "p1_2", "p1_3", "p1_4", "q1_3", "q1_4", "r1_3", "r1_4", "s1_1", "s1_2", "s1_3", "s1_4" } };
            TileEdge m1_3 = new TileEdge { edgeType = "m1_3", compatibleEdgeTypes = new List<string> { "l1_1", "l1_2", "l1_3", "l1_4" } };
            TileEdge m1_4 = new TileEdge { edgeType = "m1_4", compatibleEdgeTypes = new List<string> { "a1_1", "a1_3", "a1_3", "c1_1", "c1_3", "b1_1", "b1_3", "b1_4", "d1_1", "d1_3", "e1_3", "e1_4", "f1_3", "f1_1", "g1_1", "g1_2", "g1_3", "g1_4", "h1_1", "h1_2", "h1_3", "h1_4", "i1_1", "i1_2", "i1_3", "i1_4", "j1_1", "j1_2", "j1_3", "j1_4", "k1_1", "k1_2", "k1_3", "k1_4", "l1_1", "l1_2", "l1_3", "l1_4", "m1_2", "m1_3", "m1_4", "n1_2", "n1_3", "n1_4", "o1_2", "o1_4", "p1_1", "p1_2", "p1_3", "p1_4", "q1_3", "q1_4", "r1_3", "r1_4", "s1_1", "s1_2", "s1_3", "s1_4" } };
            TileEdge n1_1 = new TileEdge { edgeType = "n1_1", compatibleEdgeTypes = new List<string> { "l1_1", "l1_2", "l1_3", "l1_4", "b1_1", "c1_3", "d1_3", "q1_3", "q1_4" } };
            TileEdge n1_2 = new TileEdge { edgeType = "n1_2", compatibleEdgeTypes = new List<string> { "a1_1", "a1_3", "a1_3", "c1_1", "c1_3", "b1_1", "b1_3", "b1_4", "d1_1", "d1_3", "e1_3", "e1_4", "f1_3", "f1_1", "g1_1", "g1_2", "g1_3", "g1_4", "h1_1", "h1_2", "h1_3", "h1_4", "i1_1", "i1_2", "i1_3", "i1_4", "j1_1", "j1_2", "j1_3", "j1_4", "k1_1", "k1_2", "k1_3", "k1_4", "l1_1", "l1_2", "l1_3", "l1_4", "m1_2", "m1_3", "m1_4", "n1_2", "n1_3", "n1_4", "o1_2", "o1_4", "p1_1", "p1_2", "p1_3", "p1_4", "q1_3", "q1_4", "r1_3", "r1_4", "s1_1", "s1_2", "s1_3", "s1_4", "n1_1", "o1_3", "r1_1" } };
            TileEdge n1_3 = new TileEdge { edgeType = "n1_3", compatibleEdgeTypes = new List<string> { "l1_1", "l1_2", "l1_3", "l1_4" } };
            TileEdge n1_4 = new TileEdge { edgeType = "n1_4", compatibleEdgeTypes = new List<string> { "a1_1", "a1_3", "a1_3", "c1_1", "c1_3", "b1_1", "b1_3", "b1_4", "d1_1", "d1_3", "e1_3", "e1_4", "f1_3", "f1_1", "g1_1", "g1_2", "g1_3", "g1_4", "h1_1", "h1_2", "h1_3", "h1_4", "i1_1", "i1_2", "i1_3", "i1_4", "j1_1", "j1_2", "j1_3", "j1_4", "k1_1", "k1_2", "k1_3", "k1_4", "l1_1", "l1_2", "l1_3", "l1_4", "m1_2", "m1_3", "m1_4", "n1_2", "n1_3", "n1_4", "o1_2", "o1_4", "p1_1", "p1_2", "p1_3", "p1_4", "q1_3", "q1_4", "r1_3", "r1_4", "s1_1", "s1_2", "s1_3", "s1_4", "n1_1", "o1_3", "r1_1" } };
            TileEdge o1_1 = new TileEdge { edgeType = "o1_1", compatibleEdgeTypes = new List<string> { "l1_1", "l1_2", "l1_3", "l1_4", "b1_1", "c1_3", "d1_3", "q1_3", "q1_4" } };
            TileEdge o1_2 = new TileEdge { edgeType = "o1_2", compatibleEdgeTypes = new List<string> { "a1_1", "a1_3", "a1_3", "c1_1", "c1_3", "b1_1", "b1_3", "b1_4", "d1_1", "d1_3", "e1_3", "e1_4", "f1_3", "f1_1", "g1_1", "g1_2", "g1_3", "g1_4", "h1_1", "h1_2", "h1_3", "h1_4", "i1_1", "i1_2", "i1_3", "i1_4", "j1_1", "j1_2", "j1_3", "j1_4", "k1_1", "k1_2", "k1_3", "k1_4", "l1_1", "l1_2", "l1_3", "l1_4", "m1_2", "m1_3", "m1_4", "n1_2", "n1_3", "n1_4", "o1_2", "o1_4", "p1_1", "p1_2", "p1_3", "p1_4", "q1_3", "q1_4", "r1_3", "r1_4", "s1_1", "s1_2", "s1_3", "s1_4", "n1_1", "o1_3", "r1_1" } };
            TileEdge o1_3 = new TileEdge { edgeType = "o1_3", compatibleEdgeTypes = new List<string> { "l1_1", "l1_2", "l1_3", "l1_4", "b1_1", "c1_3", "d1_3", "q1_3", "q1_4" } };
            TileEdge o1_4 = new TileEdge { edgeType = "o1_4", compatibleEdgeTypes = new List<string> { "a1_1", "a1_3", "a1_3", "c1_1", "c1_3", "b1_1", "b1_3", "b1_4", "d1_1", "d1_3", "e1_3", "e1_4", "f1_3", "f1_1", "g1_1", "g1_2", "g1_3", "g1_4", "h1_1", "h1_2", "h1_3", "h1_4", "i1_1", "i1_2", "i1_3", "i1_4", "j1_1", "j1_2", "j1_3", "j1_4", "k1_1", "k1_2", "k1_3", "k1_4", "l1_1", "l1_2", "l1_3", "l1_4", "m1_2", "m1_3", "m1_4", "n1_2", "n1_3", "n1_4", "o1_2", "o1_4", "p1_1", "p1_2", "p1_3", "p1_4", "q1_3", "q1_4", "r1_3", "r1_4", "s1_1", "s1_2", "s1_3", "s1_4", "n1_1", "o1_3", "r1_1" } };
            TileEdge p1_1 = new TileEdge { edgeType = "p1_1", compatibleEdgeTypes = new List<string> { "a1_1", "a1_3", "a1_3", "c1_1", "c1_3", "b1_1", "b1_3", "b1_4", "d1_1", "d1_3", "e1_3", "e1_4", "f1_3", "f1_1", "k1_1", "k1_2", "k1_3", "k1_4", "l1_1", "l1_2", "l1_3", "l1_4", "m1_2", "m1_3", "m1_4", "n1_2", "n1_3", "n1_4", "o1_2", "o1_4", "q1_3", "q1_4", "r1_3", "r1_4", "s1_1", "s1_2", "s1_3", "s1_4" } };
            TileEdge p1_2 = new TileEdge { edgeType = "p1_2", compatibleEdgeTypes = new List<string> { "a1_1", "a1_3", "a1_3", "c1_1", "c1_3", "b1_1", "b1_3", "b1_4", "d1_1", "d1_3", "e1_3", "e1_4", "f1_3", "f1_1", "k1_1", "k1_2", "k1_3", "k1_4", "l1_1", "l1_2", "l1_3", "l1_4", "m1_2", "m1_3", "m1_4", "n1_2", "n1_3", "n1_4", "o1_2", "o1_4", "q1_3", "q1_4", "r1_3", "r1_4", "s1_1", "s1_2", "s1_3", "s1_4" } };
            TileEdge p1_3 = new TileEdge { edgeType = "p1_3", compatibleEdgeTypes = new List<string> { "a1_1", "a1_3", "a1_3", "c1_1", "c1_3", "b1_1", "b1_3", "b1_4", "d1_1", "d1_3", "e1_3", "e1_4", "f1_3", "f1_1", "k1_1", "k1_2", "k1_3", "k1_4", "l1_1", "l1_2", "l1_3", "l1_4", "m1_2", "m1_3", "m1_4", "n1_2", "n1_3", "n1_4", "o1_2", "o1_4", "q1_3", "q1_4", "r1_3", "r1_4", "s1_1", "s1_2", "s1_3", "s1_4" } };
            TileEdge p1_4 = new TileEdge { edgeType = "p1_4", compatibleEdgeTypes = new List<string> { "a1_1", "a1_3", "a1_3", "c1_1", "c1_3", "b1_1", "b1_3", "b1_4", "d1_1", "d1_3", "e1_3", "e1_4", "f1_3", "f1_1", "k1_1", "k1_2", "k1_3", "k1_4", "l1_1", "l1_2", "l1_3", "l1_4", "m1_2", "m1_3", "m1_4", "n1_2", "n1_3", "n1_4", "o1_2", "o1_4", "q1_3", "q1_4", "r1_3", "r1_4", "s1_1", "s1_2", "s1_3", "s1_4" } };
            TileEdge q1_1 = new TileEdge { edgeType = "q1_1", compatibleEdgeTypes = new List<string> { "d1_3", "l1_1", "l1_2", "l1_3", "l1_4" } };
            TileEdge q1_2 = new TileEdge { edgeType = "q1_2", compatibleEdgeTypes = new List<string> { "d1_3", "l1_1", "l1_2", "l1_3", "l1_4" } };
            TileEdge q1_3 = new TileEdge { edgeType = "q1_3", compatibleEdgeTypes = new List<string> { "a1_1", "a1_3", "a1_3", "c1_1", "c1_3", "b1_1", "b1_3", "b1_4", "d1_1", "d1_3", "e1_3", "e1_4", "f1_3", "f1_1", "g1_1", "g1_2", "g1_3", "g1_4", "h1_1", "h1_2", "h1_3", "h1_4", "i1_1", "i1_2", "i1_3", "i1_4", "j1_1", "j1_2", "j1_3", "j1_4", "k1_1", "k1_2", "k1_3", "k1_4", "l1_1", "l1_2", "l1_3", "l1_4", "m1_2", "m1_3", "m1_4", "n1_2", "n1_3", "n1_4", "o1_2", "o1_4", "p1_1", "p1_2", "p1_3", "p1_4", "q1_3", "q1_4", "r1_3", "r1_4", "s1_1", "s1_2", "s1_3", "s1_4", "n1_1", "o1_3", "r1_1" } };
            TileEdge q1_4 = new TileEdge { edgeType = "q1_4", compatibleEdgeTypes = new List<string> { "a1_1", "a1_3", "a1_3", "c1_1", "c1_3", "b1_1", "b1_3", "b1_4", "d1_1", "d1_3", "e1_3", "e1_4", "f1_3", "f1_1", "g1_1", "g1_2", "g1_3", "g1_4", "h1_1", "h1_2", "h1_3", "h1_4", "i1_1", "i1_2", "i1_3", "i1_4", "j1_1", "j1_2", "j1_3", "j1_4", "k1_1", "k1_2", "k1_3", "k1_4", "l1_1", "l1_2", "l1_3", "l1_4", "m1_2", "m1_3", "m1_4", "n1_2", "n1_3", "n1_4", "o1_2", "o1_4", "p1_1", "p1_2", "p1_3", "p1_4", "q1_3", "q1_4", "r1_3", "r1_4", "s1_1", "s1_2", "s1_3", "s1_4", "n1_1", "o1_3", "r1_1" } };
            TileEdge r1_1 = new TileEdge { edgeType = "r1_1", compatibleEdgeTypes = new List<string> { "l1_1", "l1_2", "l1_3", "l1_4", "b1_1", "c1_3", "d1_3", "q1_3", "q1_4" } };
            TileEdge r1_2 = new TileEdge { edgeType = "r1_2", compatibleEdgeTypes = new List<string> { "a1_1", "a1_3", "a1_3", "c1_1", "c1_3", "b1_1", "b1_3", "b1_4", "d1_1", "d1_3", "e1_3", "e1_4", "f1_3", "f1_1", "g1_1", "g1_2", "g1_3", "g1_4", "h1_1", "h1_2", "h1_3", "h1_4", "i1_1", "i1_2", "i1_3", "i1_4", "j1_1", "j1_2", "j1_3", "j1_4", "k1_1", "k1_2", "k1_3", "k1_4", "l1_1", "l1_2", "l1_3", "l1_4", "m1_2", "m1_3", "m1_4", "n1_2", "n1_3", "n1_4", "o1_2", "o1_4", "p1_1", "p1_2", "p1_3", "p1_4", "q1_3", "q1_4", "r1_3", "r1_4", "s1_1", "s1_2", "s1_3", "s1_4" } };
            TileEdge r1_3 = new TileEdge { edgeType = "r1_3", compatibleEdgeTypes = new List<string> { "a1_1", "a1_3", "a1_3", "c1_1", "c1_3", "b1_1", "b1_3", "b1_4", "d1_1", "d1_3", "e1_3", "e1_4", "f1_3", "f1_1", "g1_1", "g1_2", "g1_3", "g1_4", "h1_1", "h1_2", "h1_3", "h1_4", "i1_1", "i1_2", "i1_3", "i1_4", "j1_1", "j1_2", "j1_3", "j1_4", "k1_1", "k1_2", "k1_3", "k1_4", "l1_1", "l1_2", "l1_3", "l1_4", "m1_2", "m1_3", "m1_4", "n1_2", "n1_3", "n1_4", "o1_2", "o1_4", "p1_1", "p1_2", "p1_3", "p1_4", "q1_3", "q1_4", "r1_3", "r1_4", "s1_1", "s1_2", "s1_3", "s1_4" } };
            TileEdge r1_4 = new TileEdge { edgeType = "r1_4", compatibleEdgeTypes = new List<string> { "a1_1", "a1_3", "a1_3", "c1_1", "c1_3", "b1_1", "b1_3", "b1_4", "d1_1", "d1_3", "e1_3", "e1_4", "f1_3", "f1_1", "g1_1", "g1_2", "g1_3", "g1_4", "h1_1", "h1_2", "h1_3", "h1_4", "i1_1", "i1_2", "i1_3", "i1_4", "j1_1", "j1_2", "j1_3", "j1_4", "k1_1", "k1_2", "k1_3", "k1_4", "l1_1", "l1_2", "l1_3", "l1_4", "m1_2", "m1_3", "m1_4", "n1_2", "n1_3", "n1_4", "o1_2", "o1_4", "p1_1", "p1_2", "p1_3", "p1_4", "q1_3", "q1_4", "r1_3", "r1_4", "s1_1", "s1_2", "s1_3", "s1_4" } };
            TileEdge s1_1 = new TileEdge { edgeType = "s1_1", compatibleEdgeTypes = new List<string> { "a1_1", "a1_3", "a1_3", "c1_1", "c1_3", "b1_1", "b1_3", "b1_4", "d1_1", "d1_3", "e1_3", "e1_4", "f1_3", "f1_1", "g1_1", "g1_2", "g1_3", "g1_4", "h1_1", "h1_2", "h1_3", "h1_4", "i1_1", "i1_2", "i1_3", "i1_4", "j1_1", "j1_2", "j1_3", "j1_4", "k1_1", "k1_2", "k1_3", "k1_4", "l1_1", "l1_2", "l1_3", "l1_4", "m1_2", "m1_3", "m1_4", "n1_2", "n1_3", "n1_4", "o1_2", "o1_4", "p1_1", "p1_2", "p1_3", "p1_4", "q1_3", "q1_4", "r1_3", "r1_4", "s1_1", "s1_2", "s1_3", "s1_4" } };
            TileEdge s1_2 = new TileEdge { edgeType = "s1_2", compatibleEdgeTypes = new List<string> { "a1_1", "a1_3", "a1_3", "c1_1", "c1_3", "b1_1", "b1_3", "b1_4", "d1_1", "d1_3", "e1_3", "e1_4", "f1_3", "f1_1", "g1_1", "g1_2", "g1_3", "g1_4", "h1_1", "h1_2", "h1_3", "h1_4", "i1_1", "i1_2", "i1_3", "i1_4", "j1_1", "j1_2", "j1_3", "j1_4", "k1_1", "k1_2", "k1_3", "k1_4", "l1_1", "l1_2", "l1_3", "l1_4", "m1_2", "m1_3", "m1_4", "n1_2", "n1_3", "n1_4", "o1_2", "o1_4", "p1_1", "p1_2", "p1_3", "p1_4", "q1_3", "q1_4", "r1_3", "r1_4", "s1_1", "s1_2", "s1_3", "s1_4" } };
            TileEdge s1_3 = new TileEdge { edgeType = "s1_3", compatibleEdgeTypes = new List<string> { "a1_1", "a1_3", "a1_3", "c1_1", "c1_3", "b1_1", "b1_3", "b1_4", "d1_1", "d1_3", "e1_3", "e1_4", "f1_3", "f1_1", "g1_1", "g1_2", "g1_3", "g1_4", "h1_1", "h1_2", "h1_3", "h1_4", "i1_1", "i1_2", "i1_3", "i1_4", "j1_1", "j1_2", "j1_3", "j1_4", "k1_1", "k1_2", "k1_3", "k1_4", "l1_1", "l1_2", "l1_3", "l1_4", "m1_2", "m1_3", "m1_4", "n1_2", "n1_3", "n1_4", "o1_2", "o1_4", "p1_1", "p1_2", "p1_3", "p1_4", "q1_3", "q1_4", "r1_3", "r1_4", "s1_1", "s1_2", "s1_3", "s1_4" } };
            TileEdge s1_4 = new TileEdge { edgeType = "s1_4", compatibleEdgeTypes = new List<string> { "a1_1", "a1_3", "a1_3", "c1_1", "c1_3", "b1_1", "b1_3", "b1_4", "d1_1", "d1_3", "e1_3", "e1_4", "f1_3", "f1_1", "g1_1", "g1_2", "g1_3", "g1_4", "h1_1", "h1_2", "h1_3", "h1_4", "i1_1", "i1_2", "i1_3", "i1_4", "j1_1", "j1_2", "j1_3", "j1_4", "k1_1", "k1_2", "k1_3", "k1_4", "l1_1", "l1_2", "l1_3", "l1_4", "m1_2", "m1_3", "m1_4", "n1_2", "n1_3", "n1_4", "o1_2", "o1_4", "p1_1", "p1_2", "p1_3", "p1_4", "q1_3", "q1_4", "r1_3", "r1_4", "s1_1", "s1_2", "s1_3", "s1_4" } };


            // volcano

            TileEdge a2_1 = new TileEdge { edgeType = "a2_1", compatibleEdgeTypes = new List<string> { "c2_3", "c2_4", "d2_3", "d2_4", "f2_1", "j2_4" } };
            TileEdge a2_2 = new TileEdge { edgeType = "a2_2", compatibleEdgeTypes = new List<string> { "c2_1", "c2_4", "c2_4", "d2_1", "d2_3", "d2_4", "e2_1", "e2_2", "e2_3", "e2_4", "f2_1", "f2_2", "f2_3", "f2_4", "h2_1", "h2_3", "i2_1", "i2_4", "j2_1", "j2_2", "j2_4" } };
            TileEdge a2_3 = new TileEdge { edgeType = "a2_3", compatibleEdgeTypes = new List<string> { "c2_1", "c2_4", "c2_4", "d2_1", "d2_3", "d2_4", "e2_1", "e2_2", "e2_3", "e2_4", "f2_1", "f2_2", "f2_3", "f2_4", "h2_1", "h2_3", "i2_1", "i2_4", "j2_1", "j2_2", "j2_4" } };
            TileEdge a2_4 = new TileEdge { edgeType = "a2_4", compatibleEdgeTypes = new List<string> { "h2_2", "g2_4", "h2_2", "i2_2", "j2_3" } };
            TileEdge b2_1 = new TileEdge { edgeType = "b2_1", compatibleEdgeTypes = new List<string> { "c2_3", "c2_4", "d2_3", "f2_1", "f2_2", "f2_3", "f2_4" } };
            TileEdge b2_2 = new TileEdge { edgeType = "b2_2", compatibleEdgeTypes = new List<string> { "g2_2", "i2_3" } };
            TileEdge b2_3 = new TileEdge { edgeType = "b2_3", compatibleEdgeTypes = new List<string> { "c2_3", "c2_4", "d2_3", "f2_1", "f2_2", "f2_3", "f2_4" } };
            TileEdge b2_4 = new TileEdge { edgeType = "b2_4", compatibleEdgeTypes = new List<string> { "c2_1", "c2_4", "c2_4", "d2_1", "d2_3", "d2_4", "e2_1", "e2_2", "e2_3", "e2_4", "f2_1", "f2_2", "f2_3", "f2_4", "h2_1", "h2_3", "i2_1", "i2_4", "j2_1", "j2_2", "j2_4" } };
            TileEdge c2_1 = new TileEdge { edgeType = "c2_1", compatibleEdgeTypes = new List<string> { "c2_1", "c2_4", "c2_4", "d2_1", "d2_3", "d2_4", "e2_1", "e2_2", "e2_3", "e2_4", "f2_1", "f2_2", "f2_3", "f2_4", "h2_1", "h2_3", "i2_1", "i2_4", "j2_1", "j2_2", "j2_4" } };
            TileEdge c2_2 = new TileEdge { edgeType = "c2_2", compatibleEdgeTypes = new List<string> { "c2_3", "c2_4", "d2_3", "d2_4", "f2_1", "j2_4" } };
            TileEdge c2_3 = new TileEdge { edgeType = "c2_3", compatibleEdgeTypes = new List<string> { "a2_1", "a2_2", "a2_3", "b2_1", "d2_1", "d2_2", "d2_3", "d2_4", "e2_1", "e2_2", "e2_3", "e2_4", "g2_1", "g2_3", "h2_1", "h2_3", "h2_4", "i2_1", "i2_4", "j2_1", "j2_2", "j2_4" } };
            TileEdge c2_4 = new TileEdge { edgeType = "c2_4", compatibleEdgeTypes = new List<string> { "a2_1", "a2_2", "a2_3", "b2_1", "d2_1", "d2_2", "d2_3", "d2_4", "e2_1", "e2_2", "e2_3", "e2_4", "g2_1", "g2_3", "h2_1", "h2_3", "h2_4", "i2_1", "i2_4", "j2_1", "j2_2", "j2_4" } };
            TileEdge d2_1 = new TileEdge { edgeType = "d2_1", compatibleEdgeTypes = new List<string> { "c2_1", "c2_4", "c2_4", "d2_1", "d2_3", "d2_4", "e2_1", "e2_2", "e2_3", "e2_4", "f2_1", "f2_2", "f2_3", "f2_4", "h2_1", "h2_3", "i2_1", "i2_4", "j2_1", "j2_2", "j2_4" } };
            TileEdge d2_2 = new TileEdge { edgeType = "d2_2", compatibleEdgeTypes = new List<string> { "c2_3", "c2_4", "d2_3", "d2_4", "f2_1", "j2_4" } };
            TileEdge d2_3 = new TileEdge { edgeType = "d2_3", compatibleEdgeTypes = new List<string> { "a2_1", "a2_2", "a2_3", "b2_1", "d2_1", "d2_2", "d2_3", "d2_4", "e2_1", "e2_2", "e2_3", "e2_4", "g2_1", "g2_3", "h2_1", "h2_3", "h2_4", "i2_1", "i2_4", "j2_1", "j2_2", "j2_4" } };
            TileEdge d2_4 = new TileEdge { edgeType = "d2_4", compatibleEdgeTypes = new List<string> { "c2_1", "c2_4", "c2_4", "d2_1", "d2_3", "d2_4", "e2_1", "e2_2", "e2_3", "e2_4", "f2_1", "f2_2", "f2_3", "f2_4", "h2_1", "h2_3", "i2_1", "i2_4", "j2_1", "j2_2", "j2_4" } };
            TileEdge e2_1 = new TileEdge { edgeType = "e2_1", compatibleEdgeTypes = new List<string> { "c2_1", "c2_4", "c2_4", "d2_1", "d2_3", "d2_4", "e2_1", "e2_2", "e2_3", "e2_4", "f2_1", "f2_2", "f2_3", "f2_4", "h2_1", "h2_3", "i2_1", "i2_4", "j2_1", "j2_2", "j2_4" } };
            TileEdge e2_2 = new TileEdge { edgeType = "e2_2", compatibleEdgeTypes = new List<string> { "c2_1", "c2_4", "c2_4", "d2_1", "d2_3", "d2_4", "e2_1", "e2_2", "e2_3", "e2_4", "f2_1", "f2_2", "f2_3", "f2_4", "h2_1", "h2_3", "i2_1", "i2_4", "j2_1", "j2_2", "j2_4" } };
            TileEdge e2_3 = new TileEdge { edgeType = "e2_3", compatibleEdgeTypes = new List<string> { "c2_1", "c2_4", "c2_4", "d2_1", "d2_3", "d2_4", "e2_1", "e2_2", "e2_3", "e2_4", "f2_1", "f2_2", "f2_3", "f2_4", "h2_1", "h2_3", "i2_1", "i2_4", "j2_1", "j2_2", "j2_4" } };
            TileEdge e2_4 = new TileEdge { edgeType = "e2_4", compatibleEdgeTypes = new List<string> { "c2_1", "c2_4", "c2_4", "d2_1", "d2_3", "d2_4", "e2_1", "e2_2", "e2_3", "e2_4", "f2_1", "f2_2", "f2_3", "f2_4", "h2_1", "h2_3", "i2_1", "i2_4", "j2_1", "j2_2", "j2_4" } };
            TileEdge f2_1 = new TileEdge { edgeType = "f2_1", compatibleEdgeTypes = new List<string> { "a2_1", "a2_2", "a2_3", "b2_1", "d2_1", "d2_2", "d2_3", "d2_4", "e2_1", "e2_2", "e2_3", "e2_4", "g2_1", "g2_3", "h2_1", "h2_3", "h2_4", "i2_1", "i2_4", "j2_1", "j2_2", "j2_4" } };
            TileEdge f2_2 = new TileEdge { edgeType = "f2_2", compatibleEdgeTypes = new List<string> { "a2_1", "a2_2", "a2_3", "b2_1", "d2_1", "d2_2", "d2_3", "d2_4", "e2_1", "e2_2", "e2_3", "e2_4", "g2_1", "g2_3", "h2_1", "h2_3", "h2_4", "i2_1", "i2_4", "j2_1", "j2_2", "j2_4" } };
            TileEdge f2_3 = new TileEdge { edgeType = "f2_3", compatibleEdgeTypes = new List<string> { "a2_1", "a2_2", "a2_3", "b2_1", "d2_1", "d2_2", "d2_3", "d2_4", "e2_1", "e2_2", "e2_3", "e2_4", "g2_1", "g2_3", "h2_1", "h2_3", "h2_4", "i2_1", "i2_4", "j2_1", "j2_2", "j2_4" } };
            TileEdge f2_4 = new TileEdge { edgeType = "f2_4", compatibleEdgeTypes = new List<string> { "a2_1", "a2_2", "a2_3", "b2_1", "d2_1", "d2_2", "d2_3", "d2_4", "e2_1", "e2_2", "e2_3", "e2_4", "g2_1", "g2_3", "h2_1", "h2_3", "h2_4", "i2_1", "i2_4", "j2_1", "j2_2", "j2_4" } };
            TileEdge g2_1 = new TileEdge { edgeType = "g2_1", compatibleEdgeTypes = new List<string> { "c2_3", "c2_4", "d2_3", "f2_1", "f2_2", "f2_3", "f2_4" } };
            TileEdge g2_2 = new TileEdge { edgeType = "g2_2", compatibleEdgeTypes = new List<string> { "b2_2", "i2_2", "i2_3", "j2_3", "h2_2" } };
            TileEdge g2_3 = new TileEdge { edgeType = "g2_3", compatibleEdgeTypes = new List<string> { "c2_3", "c2_4", "d2_3", "f2_1", "f2_2", "f2_3", "f2_4" } };
            TileEdge g2_4 = new TileEdge { edgeType = "g2_4", compatibleEdgeTypes = new List<string> { "a2_4", "i2_3" } };
            TileEdge h2_1 = new TileEdge { edgeType = "h2_1", compatibleEdgeTypes = new List<string> { "c2_1", "c2_4", "c2_4", "d2_1", "d2_3", "d2_4", "e2_1", "e2_2", "e2_3", "e2_4", "f2_1", "f2_2", "f2_3", "f2_4", "h2_1", "h2_3", "i2_1", "i2_4", "j2_1", "j2_2", "j2_4" } };
            TileEdge h2_2 = new TileEdge { edgeType = "h2_2", compatibleEdgeTypes = new List<string> { "g2_2", "i2_3", "a2_4" } };
            TileEdge h2_3 = new TileEdge { edgeType = "h2_3", compatibleEdgeTypes = new List<string> { "c2_1", "c2_4", "c2_4", "d2_1", "d2_3", "d2_4", "e2_1", "e2_2", "e2_3", "e2_4", "f2_1", "f2_2", "f2_3", "f2_4", "h2_1", "h2_3", "j2_1", "j2_2", "j2_4" } };
            TileEdge h2_4 = new TileEdge { edgeType = "h2_4", compatibleEdgeTypes = new List<string> { "c2_1", "c2_4", "c2_4", "d2_1", "d2_3", "d2_4", "e2_1", "e2_2", "e2_3", "e2_4", "f2_1", "f2_2", "f2_3", "f2_4", "h2_1", "h2_3", "j2_1", "j2_2", "j2_4" } };
            TileEdge i2_1 = new TileEdge { edgeType = "i2_1", compatibleEdgeTypes = new List<string> { "c2_1", "c2_4", "c2_4", "d2_1", "d2_3", "d2_4", "e2_1", "e2_2", "e2_3", "e2_4", "f2_1", "f2_2", "f2_3", "f2_4", "h2_1", "h2_3", "j2_1", "j2_2", "j2_4" } };
            TileEdge i2_2 = new TileEdge { edgeType = "i2_2", compatibleEdgeTypes = new List<string> { "a2_4", "g2_2", "g2_4" } };
            TileEdge i2_3 = new TileEdge { edgeType = "i2_3", compatibleEdgeTypes = new List<string> { "h2_2", "b2_2", "g2_4", "g2_2", "j2_3" } };
            TileEdge i2_4 = new TileEdge { edgeType = "i2_4", compatibleEdgeTypes = new List<string> { "c2_1", "c2_4", "c2_4", "d2_1", "d2_3", "d2_4", "e2_1", "e2_2", "e2_3", "e2_4", "f2_1", "f2_2", "f2_3", "f2_4", "h2_1", "h2_3", "i2_1", "i2_4", "j2_1", "j2_2", "j2_4" } };
            TileEdge j2_1 = new TileEdge { edgeType = "j2_1", compatibleEdgeTypes = new List<string> { "c2_1", "c2_4", "c2_4", "d2_1", "d2_3", "d2_4", "e2_1", "e2_2", "e2_3", "e2_4", "f2_1", "f2_2", "f2_3", "f2_4", "h2_1", "h2_3", "i2_1", "i2_4", "j2_1", "j2_2", "j2_4" } };
            TileEdge j2_2 = new TileEdge { edgeType = "j2_2", compatibleEdgeTypes = new List<string> { "c2_1", "c2_4", "c2_4", "d2_1", "d2_3", "d2_4", "e2_1", "e2_2", "e2_3", "e2_4", "f2_1", "f2_2", "f2_3", "f2_4", "h2_1", "h2_3", "i2_1", "i2_4", "j2_1", "j2_2", "j2_4" } };
            TileEdge j2_3 = new TileEdge { edgeType = "j2_3", compatibleEdgeTypes = new List<string> { "g2_2", "i2_3", "a2_4" } };
            TileEdge j2_4 = new TileEdge { edgeType = "j2_4", compatibleEdgeTypes = new List<string> { "c2_1", "c2_4", "c2_4", "d2_1", "d2_3", "d2_4", "e2_1", "e2_2", "e2_3", "e2_4", "f2_1", "f2_2", "f2_3", "f2_4", "h2_1", "h2_3", "i2_1", "i2_4", "j2_1", "j2_2", "j2_4" } };

            // desert

            TileEdge a3_1 = new TileEdge { edgeType = "a3_1", compatibleEdgeTypes = new List<string> { "b3_3", "m3_2", "o3_3" } };
            TileEdge a3_2 = new TileEdge { edgeType = "a3_2", compatibleEdgeTypes = new List<string> { "b3_1", "b3_2", "b3_3", "c3_1", "c3_2", "c3_3", "c3_4", "d3_3", "d3_4", "e3_1", "e3_2", "f3_1", "f3_2", "f3_3", "f3_4", "g3_1", "g3_2", "g3_3", "g3_4", "h3_1", "h3_2", "h3_3", "h3_4", "i3_1", "i3_2", "i3_3", "i3_4", "j3_1", "j3_2", "j3_3", "j3_4", "k3_1", "k3_2", "k3_4", "l3_1", "l3_2", "l3_3", "l3_4", "m3_1", "m3_3", "m3_4", "n3_1", "n3_2", "n3_3", "n3_4", "o3_1", "o3_2", "o3_4" } };
            TileEdge a3_3 = new TileEdge { edgeType = "a3_3", compatibleEdgeTypes = new List<string> { "g3_1", "g3_3", "e3_1", "i3_1", "i3_2", "i3_3", "i3_4", "l3_3", "l3_2" } };
            TileEdge a3_4 = new TileEdge { edgeType = "a3_4", compatibleEdgeTypes = new List<string> { "a3_1", "b3_4" } };
            TileEdge b3_1 = new TileEdge { edgeType = "b3_1", compatibleEdgeTypes = new List<string> { "b3_2", "c3_1", "c3_2", "c3_3", "c3_4", "e3_1", "e3_2", "f3_1", "f3_2", "f3_4", "g3_1", "g3_3", "h3_3", "h3_4", "i3_1", "i3_2", "i3_3", "i3_4", "j3_1", "j3_2", "j3_3", "j3_4", "k3_1", "k3_2", "k3_4", "l3_1", "l3_2", "l3_3", "l3_4", "m3_1", "m3_3", "m3_14", "h3_1", "h3_2", "h3_3", "h3_4", "o3_1", "o3_2", "o3_4" } };
            TileEdge b3_2 = new TileEdge { edgeType = "b3_2", compatibleEdgeTypes = new List<string> { "b3_2", "c3_1", "c3_2", "c3_3", "c3_4", "e3_1", "e3_2", "f3_1", "f3_2", "f3_4", "g3_1", "g3_3", "h3_3", "h3_4", "i3_1", "i3_2", "i3_3", "i3_4", "j3_1", "j3_2", "j3_3", "j3_4", "k3_1", "k3_2", "k3_4", "l3_1", "l3_2", "l3_3", "l3_4", "m3_1", "m3_3", "m3_14", "h3_1", "h3_2", "h3_3", "h3_4", "o3_1", "o3_2", "o3_4" } };
            TileEdge b3_3 = new TileEdge { edgeType = "b3_3", compatibleEdgeTypes = new List<string> { "a3_1", "b3_4" } };
            TileEdge b3_4 = new TileEdge { edgeType = "b3_4", compatibleEdgeTypes = new List<string> { "a3_4", "m3_2", "o3_3" } };
            TileEdge c3_1 = new TileEdge { edgeType = "c3_1", compatibleEdgeTypes = new List<string> { "b3_2", "c3_1", "c3_2", "c3_3", "c3_4", "e3_1", "e3_2", "f3_1", "f3_2", "f3_4", "g3_1", "g3_3", "h3_3", "h3_4", "i3_1", "i3_2", "i3_3", "i3_4", "j3_1", "j3_2", "j3_3", "j3_4", "k3_1", "k3_2", "k3_4", "l3_1", "l3_2", "l3_3", "l3_4", "m3_1", "m3_3", "m3_14", "h3_1", "h3_2", "h3_3", "h3_4", "o3_1", "o3_2", "o3_4" } };
            TileEdge c3_2 = new TileEdge { edgeType = "c3_2", compatibleEdgeTypes = new List<string> { "b3_2", "c3_1", "c3_2", "c3_3", "c3_4", "e3_1", "e3_2", "f3_1", "f3_2", "f3_4", "g3_1", "g3_3", "h3_3", "h3_4", "i3_1", "i3_2", "i3_3", "i3_4", "j3_1", "j3_2", "j3_3", "j3_4", "k3_1", "k3_2", "k3_4", "l3_1", "l3_2", "l3_3", "l3_4", "m3_1", "m3_3", "m3_14", "h3_1", "h3_2", "h3_3", "h3_4", "o3_1", "o3_2", "o3_4" } };
            TileEdge c3_3 = new TileEdge { edgeType = "c3_3", compatibleEdgeTypes = new List<string> { "b3_2", "c3_1", "c3_2", "c3_3", "c3_4", "e3_1", "e3_2", "f3_1", "f3_2", "f3_4", "g3_1", "g3_3", "h3_3", "h3_4", "i3_1", "i3_2", "i3_3", "i3_4", "j3_1", "j3_2", "j3_3", "j3_4", "k3_1", "k3_2", "k3_4", "l3_1", "l3_2", "l3_3", "l3_4", "m3_1", "m3_3", "m3_14", "h3_1", "h3_2", "h3_3", "h3_4", "o3_1", "o3_2", "o3_4" } };
            TileEdge c3_4 = new TileEdge { edgeType = "c3_4", compatibleEdgeTypes = new List<string> { "b3_2", "c3_1", "c3_2", "c3_3", "c3_4", "e3_1", "e3_2", "f3_1", "f3_2", "f3_4", "g3_1", "g3_3", "h3_3", "h3_4", "i3_1", "i3_2", "i3_3", "i3_4", "j3_1", "j3_2", "j3_3", "j3_4", "k3_1", "k3_2", "k3_4", "l3_1", "l3_2", "l3_3", "l3_4", "m3_1", "m3_3", "m3_14", "h3_1", "h3_2", "h3_3", "h3_4", "o3_1", "o3_2", "o3_4" } };
            TileEdge d3_1 = new TileEdge { edgeType = "d3_1", compatibleEdgeTypes = new List<string> { "e3_1", "e3_2", "f3_4", "g3_3", "i3_1", "i3_2", "i3_3", "i3_4" } };
            TileEdge d3_2 = new TileEdge { edgeType = "d3_2", compatibleEdgeTypes = new List<string> { "e3_1", "e3_2", "f3_4", "g3_3", "i3_1", "i3_2", "i3_3", "i3_4" } };
            TileEdge d3_3 = new TileEdge { edgeType = "d3_3", compatibleEdgeTypes = new List<string> { "g3_1", "g3_3", "e3_1", "i3_1", "i3_2", "i3_3", "i3_4", "l3_3", "l3_2" } };
            TileEdge d3_4 = new TileEdge { edgeType = "d3_4", compatibleEdgeTypes = new List<string> { "g3_1", "g3_3", "e3_1", "i3_1", "i3_2", "i3_3", "i3_4", "l3_3", "l3_2" } };
            TileEdge e3_1 = new TileEdge { edgeType = "e3_1", compatibleEdgeTypes = new List<string> { "b3_1", "b3_2", "b3_3", "c3_1", "c3_2", "c3_3", "c3_4", "d3_3", "d3_4", "e3_1", "e3_2", "f3_1", "f3_2", "f3_3", "f3_4", "g3_1", "g3_2", "g3_3", "g3_4", "h3_1", "h3_2", "h3_3", "h3_4", "i3_1", "i3_2", "i3_3", "i3_4", "j3_1", "j3_2", "j3_3", "j3_4", "k3_1", "k3_2", "k3_4", "l3_1", "l3_2", "l3_3", "l3_4", "m3_1", "m3_3", "m3_4", "n3_1", "n3_2", "n3_3", "n3_4", "o3_1", "o3_2", "o3_4" } };
            TileEdge e3_2 = new TileEdge { edgeType = "e3_2", compatibleEdgeTypes = new List<string> { "b3_1", "b3_2", "b3_3", "c3_1", "c3_2", "c3_3", "c3_4", "d3_3", "d3_4", "e3_1", "e3_2", "f3_1", "f3_2", "f3_3", "f3_4", "g3_1", "g3_2", "g3_3", "g3_4", "h3_1", "h3_2", "h3_3", "h3_4", "i3_1", "i3_2", "i3_3", "i3_4", "j3_1", "j3_2", "j3_3", "j3_4", "k3_1", "k3_2", "k3_4", "l3_1", "l3_2", "l3_3", "l3_4", "m3_1", "m3_3", "m3_4", "n3_1", "n3_2", "n3_3", "n3_4", "o3_1", "o3_2", "o3_4" } };
            TileEdge e3_3 = new TileEdge { edgeType = "e3_3", compatibleEdgeTypes = new List<string> { "e3_1", "e3_2", "f3_4", "g3_3", "i3_1", "i3_2", "i3_3", "i3_4" } };
            TileEdge e3_4 = new TileEdge { edgeType = "e3_4", compatibleEdgeTypes = new List<string> { "e3_1", "e3_2", "f3_4", "g3_3", "i3_1", "i3_2", "i3_3", "i3_4" } };
            TileEdge f3_1 = new TileEdge { edgeType = "f3_1", compatibleEdgeTypes = new List<string> { "b3_2", "c3_1", "c3_2", "c3_3", "c3_4", "e3_1", "e3_2", "f3_1", "f3_2", "f3_4", "g3_1", "g3_3", "h3_3", "h3_4", "i3_1", "i3_2", "i3_3", "i3_4", "j3_1", "j3_2", "j3_3", "j3_4", "k3_1", "k3_2", "k3_4", "l3_1", "l3_2", "l3_3", "l3_4", "m3_1", "m3_3", "m3_14", "h3_1", "h3_2", "h3_3", "h3_4", "o3_1", "o3_2", "o3_4" } };
            TileEdge f3_2 = new TileEdge { edgeType = "f3_2", compatibleEdgeTypes = new List<string> { "b3_1", "b3_2", "b3_3", "c3_1", "c3_2", "c3_3", "c3_4", "d3_3", "d3_4", "e3_1", "e3_2", "f3_1", "f3_2", "f3_3", "f3_4", "g3_1", "g3_2", "g3_3", "g3_4", "h3_1", "h3_2", "h3_3", "h3_4", "i3_1", "i3_2", "i3_3", "i3_4", "j3_1", "j3_2", "j3_3", "j3_4", "k3_1", "k3_2", "k3_4", "l3_1", "l3_2", "l3_3", "l3_4", "m3_1", "m3_3", "m3_4", "n3_1", "n3_2", "n3_3", "n3_4", "o3_1", "o3_2", "o3_4" } };
            TileEdge f3_3 = new TileEdge { edgeType = "f3_3", compatibleEdgeTypes = new List<string> { "g3_1", "g3_3", "e3_1", "i3_1", "i3_2", "i3_3", "i3_4", "l3_3", "l3_2" } };
            TileEdge f3_4 = new TileEdge { edgeType = "f3_4", compatibleEdgeTypes = new List<string> { "b3_1", "b3_2", "b3_3", "c3_1", "c3_2", "c3_3", "c3_4", "d3_3", "d3_4", "e3_1", "e3_2", "f3_1", "f3_2", "f3_3", "f3_4", "g3_1", "g3_2", "g3_3", "g3_4", "h3_1", "h3_2", "h3_3", "h3_4", "i3_1", "i3_2", "i3_3", "i3_4", "j3_1", "j3_2", "j3_3", "j3_4", "k3_1", "k3_2", "k3_4", "l3_1", "l3_2", "l3_3", "l3_4", "m3_1", "m3_3", "m3_4", "n3_1", "n3_2", "n3_3", "n3_4", "o3_1", "o3_2", "o3_4" } };
            TileEdge g3_1 = new TileEdge { edgeType = "g3_1", compatibleEdgeTypes = new List<string> { "b3_1", "b3_2", "b3_3", "c3_1", "c3_2", "c3_3", "c3_4", "d3_3", "d3_4", "e3_1", "e3_2", "f3_1", "f3_2", "f3_3", "f3_4", "g3_1", "g3_2", "g3_3", "g3_4", "h3_1", "h3_2", "h3_3", "h3_4", "i3_1", "i3_2", "i3_3", "i3_4", "j3_1", "j3_2", "j3_3", "j3_4", "k3_1", "k3_2", "k3_4", "l3_1", "l3_2", "l3_3", "l3_4", "m3_1", "m3_3", "m3_4", "n3_1", "n3_2", "n3_3", "n3_4", "o3_1", "o3_2", "o3_4" } };
            TileEdge g3_2 = new TileEdge { edgeType = "g3_2", compatibleEdgeTypes = new List<string> { "g3_1", "g3_3", "e3_1", "i3_1", "i3_2", "i3_3", "i3_4", "l3_3", "l3_2" } };
            TileEdge g3_3 = new TileEdge { edgeType = "g3_3", compatibleEdgeTypes = new List<string> { "b3_1", "b3_2", "b3_3", "c3_1", "c3_2", "c3_3", "c3_4", "d3_3", "d3_4", "e3_1", "e3_2", "f3_1", "f3_2", "f3_3", "f3_4", "g3_1", "g3_2", "g3_3", "g3_4", "h3_1", "h3_2", "h3_3", "h3_4", "i3_1", "i3_2", "i3_3", "i3_4", "j3_1", "j3_2", "j3_3", "j3_4", "k3_1", "k3_2", "k3_4", "l3_1", "l3_2", "l3_3", "l3_4", "m3_1", "m3_3", "m3_4", "n3_1", "n3_2", "n3_3", "n3_4", "o3_1", "o3_2", "o3_4" } };
            TileEdge g3_4 = new TileEdge { edgeType = "g3_4", compatibleEdgeTypes = new List<string> { "g3_1", "g3_3", "e3_1", "i3_1", "i3_2", "i3_3", "i3_4", "l3_3", "l3_2" } };
            TileEdge h3_1 = new TileEdge { edgeType = "h3_1", compatibleEdgeTypes = new List<string> { "g3_1", "g3_3", "e3_1", "i3_1", "i3_2", "i3_3", "i3_4", "l3_3", "l3_2" } };
            TileEdge h3_2 = new TileEdge { edgeType = "h3_2", compatibleEdgeTypes = new List<string> { "g3_1", "g3_3", "e3_1", "i3_1", "i3_2", "i3_3", "i3_4", "l3_3", "l3_2" } };
            TileEdge h3_3 = new TileEdge { edgeType = "h3_3", compatibleEdgeTypes = new List<string> { "b3_2", "c3_1", "c3_2", "c3_3", "c3_4", "e3_1", "e3_2", "f3_1", "f3_2", "f3_4", "g3_1", "g3_3", "h3_3", "h3_4", "i3_1", "i3_2", "i3_3", "i3_4", "j3_1", "j3_2", "j3_3", "j3_4", "k3_1", "k3_2", "k3_4", "l3_1", "l3_2", "l3_3", "l3_4", "m3_1", "m3_3", "m3_14", "h3_1", "h3_2", "h3_3", "h3_4", "o3_1", "o3_2", "o3_4" } };
            TileEdge h3_4 = new TileEdge { edgeType = "h3_4", compatibleEdgeTypes = new List<string> { "b3_2", "c3_1", "c3_2", "c3_3", "c3_4", "e3_1", "e3_2", "f3_1", "f3_2", "f3_4", "g3_1", "g3_3", "h3_3", "h3_4", "i3_1", "i3_2", "i3_3", "i3_4", "j3_1", "j3_2", "j3_3", "j3_4", "k3_1", "k3_2", "k3_4", "l3_1", "l3_2", "l3_3", "l3_4", "m3_1", "m3_3", "m3_14", "h3_1", "h3_2", "h3_3", "h3_4", "o3_1", "o3_2", "o3_4" } };
            TileEdge i3_1 = new TileEdge { edgeType = "i3_1", compatibleEdgeTypes = new List<string> { "d3_2", "d3_4", "e3_3", "e3_4", "f3_1", "k3_3" } };
            TileEdge i3_2 = new TileEdge { edgeType = "i3_2", compatibleEdgeTypes = new List<string> { "b3_2", "c3_1", "c3_2", "c3_3", "c3_4", "e3_1", "e3_2", "f3_1", "f3_2", "f3_4", "g3_1", "g3_3", "h3_3", "h3_4", "i3_1", "i3_2", "i3_3", "i3_4", "j3_1", "j3_2", "j3_3", "j3_4", "k3_1", "k3_2", "k3_4", "l3_1", "l3_2", "l3_3", "l3_4", "m3_1", "m3_3", "m3_14", "h3_1", "h3_2", "h3_3", "h3_4", "o3_1", "o3_2", "o3_4" } };
            TileEdge i3_3 = new TileEdge { edgeType = "i3_3", compatibleEdgeTypes = new List<string> { "b3_2", "c3_1", "c3_2", "c3_3", "c3_4", "e3_1", "e3_2", "f3_1", "f3_2", "f3_4", "g3_1", "g3_3", "h3_3", "h3_4", "i3_1", "i3_2", "i3_3", "i3_4", "j3_1", "j3_2", "j3_3", "j3_4", "k3_1", "k3_2", "k3_4", "l3_1", "l3_2", "l3_3", "l3_4", "m3_1", "m3_3", "m3_14", "h3_1", "h3_2", "h3_3", "h3_4", "o3_1", "o3_2", "o3_4" } };
            TileEdge i3_4 = new TileEdge { edgeType = "i3_4", compatibleEdgeTypes = new List<string> { "b3_2", "c3_1", "c3_2", "c3_3", "c3_4", "e3_1", "e3_2", "f3_1", "f3_2", "f3_4", "g3_1", "g3_3", "h3_3", "h3_4", "i3_1", "i3_2", "i3_3", "i3_4", "j3_1", "j3_2", "j3_3", "j3_4", "k3_1", "k3_2", "k3_4", "l3_1", "l3_2", "l3_3", "l3_4", "m3_1", "m3_3", "m3_14", "h3_1", "h3_2", "h3_3", "h3_4", "o3_1", "o3_2", "o3_4" } };
            TileEdge j3_1 = new TileEdge { edgeType = "j3_1", compatibleEdgeTypes = new List<string> { "f3_4", "i3_1", "i3_2", "i3_3", "i3_4", "g3_1", "g3_3", "l3_1", "l3_4" } };
            TileEdge j3_2 = new TileEdge { edgeType = "j3_2", compatibleEdgeTypes = new List<string> { "b3_2", "c3_1", "c3_2", "c3_3", "c3_4", "e3_1", "e3_2", "f3_1", "f3_2", "f3_4", "g3_1", "g3_3", "h3_3", "h3_4", "i3_1", "i3_2", "i3_3", "i3_4", "j3_1", "j3_2", "j3_3", "j3_4", "k3_1", "k3_2", "k3_4", "l3_1", "l3_2", "l3_3", "l3_4", "m3_1", "m3_3", "m3_14", "h3_1", "h3_2", "h3_3", "h3_4", "o3_1", "o3_2", "o3_4" } };
            TileEdge j3_3 = new TileEdge { edgeType = "j3_3", compatibleEdgeTypes = new List<string> { "b3_2", "c3_1", "c3_2", "c3_3", "c3_4", "e3_1", "e3_2", "f3_1", "f3_2", "f3_4", "g3_1", "g3_3", "h3_3", "h3_4", "i3_1", "i3_2", "i3_3", "i3_4", "j3_1", "j3_2", "j3_3", "j3_4", "k3_1", "k3_2", "k3_4", "l3_1", "l3_2", "l3_3", "l3_4", "m3_1", "m3_3", "m3_14", "h3_1", "h3_2", "h3_3", "h3_4", "o3_1", "o3_2", "o3_4" } };
            TileEdge j3_4 = new TileEdge { edgeType = "j3_4", compatibleEdgeTypes = new List<string> { "f3_4", "i3_1", "i3_2", "i3_3", "i3_4", "g3_1", "g3_3", "l3_1", "l3_4" } };
            TileEdge k3_1 = new TileEdge { edgeType = "k3_1", compatibleEdgeTypes = new List<string> { "b3_1", "b3_2", "b3_3", "c3_1", "c3_2", "c3_3", "c3_4", "d3_3", "d3_4", "e3_1", "e3_2", "f3_1", "f3_2", "f3_3", "f3_4", "g3_1", "g3_2", "g3_3", "g3_4", "h3_1", "h3_2", "h3_3", "h3_4", "i3_1", "i3_2", "i3_3", "i3_4", "j3_1", "j3_2", "j3_3", "j3_4", "k3_1", "k3_2", "k3_4", "l3_1", "l3_2", "l3_3", "l3_4", "m3_1", "m3_3", "m3_4", "n3_1", "n3_2", "n3_3", "n3_4", "o3_1", "o3_2", "o3_4" } };
            TileEdge k3_2 = new TileEdge { edgeType = "k3_2", compatibleEdgeTypes = new List<string> { "b3_2", "c3_1", "c3_2", "c3_3", "c3_4", "e3_1", "e3_2", "f3_1", "f3_2", "f3_4", "g3_1", "g3_3", "h3_3", "h3_4", "i3_1", "i3_2", "i3_3", "i3_4", "j3_1", "j3_2", "j3_3", "j3_4", "k3_1", "k3_2", "k3_4", "l3_1", "l3_2", "l3_3", "l3_4", "m3_1", "m3_3", "m3_14", "h3_1", "h3_2", "h3_3", "h3_4", "o3_1", "o3_2", "o3_4" } };
            TileEdge k3_3 = new TileEdge { edgeType = "k3_3", compatibleEdgeTypes = new List<string> { "e3_1", "e3_2", "f3_4", "g3_3", "i3_1", "i3_2", "i3_3", "i3_4" } };
            TileEdge k3_4 = new TileEdge { edgeType = "k3_4", compatibleEdgeTypes = new List<string> { "b3_2", "c3_1", "c3_2", "c3_3", "c3_4", "e3_1", "e3_2", "f3_1", "f3_2", "f3_4", "g3_1", "g3_3", "h3_3", "h3_4", "i3_1", "i3_2", "i3_3", "i3_4", "j3_1", "j3_2", "j3_3", "j3_4", "k3_1", "k3_2", "k3_4", "l3_1", "l3_2", "l3_3", "l3_4", "m3_1", "m3_3", "m3_14", "h3_1", "h3_2", "h3_3", "h3_4", "o3_1", "o3_2", "o3_4" } };
            TileEdge l3_1 = new TileEdge { edgeType = "l3_1", compatibleEdgeTypes = new List<string> { "b3_1", "b3_2", "b3_3", "c3_1", "c3_2", "c3_3", "c3_4", "d3_3", "d3_4", "e3_1", "e3_2", "f3_1", "f3_2", "f3_3", "f3_4", "g3_1", "g3_2", "g3_3", "g3_4", "h3_1", "h3_2", "h3_3", "h3_4", "i3_1", "i3_2", "i3_3", "i3_4", "j3_1", "j3_2", "j3_3", "j3_4", "k3_1", "k3_2", "k3_4", "l3_1", "l3_2", "l3_3", "l3_4", "m3_1", "m3_3", "m3_4", "n3_1", "n3_2", "n3_3", "n3_4", "o3_1", "o3_2", "o3_4" } };
            TileEdge l3_2 = new TileEdge { edgeType = "l3_2", compatibleEdgeTypes = new List<string> { "b3_1", "b3_2", "b3_3", "c3_1", "c3_2", "c3_3", "c3_4", "d3_3", "d3_4", "e3_1", "e3_2", "f3_1", "f3_2", "f3_3", "f3_4", "g3_1", "g3_2", "g3_3", "g3_4", "h3_1", "h3_2", "h3_3", "h3_4", "i3_1", "i3_2", "i3_3", "i3_4", "j3_1", "j3_2", "j3_3", "j3_4", "k3_1", "k3_2", "k3_4", "l3_1", "l3_2", "l3_3", "l3_4", "m3_1", "m3_3", "m3_4", "n3_1", "n3_2", "n3_3", "n3_4", "o3_1", "o3_2", "o3_4" } };
            TileEdge l3_3 = new TileEdge { edgeType = "l3_3", compatibleEdgeTypes = new List<string> { "f3_4", "i3_1", "i3_2", "i3_3", "i3_4", "g3_1", "g3_3", "l3_1", "l3_4" } };
            TileEdge l3_4 = new TileEdge { edgeType = "l3_4", compatibleEdgeTypes = new List<string> { "f3_4", "i3_1", "i3_2", "i3_3", "i3_4", "g3_1", "g3_3", "l3_1", "l3_4" } };
            TileEdge m3_1 = new TileEdge { edgeType = "m3_1", compatibleEdgeTypes = new List<string> { "b3_2", "c3_1", "c3_2", "c3_3", "c3_4", "e3_1", "e3_2", "f3_1", "f3_2", "f3_4", "g3_1", "g3_3", "h3_3", "h3_4", "i3_1", "i3_2", "i3_3", "i3_4", "j3_1", "j3_2", "j3_3", "j3_4", "k3_1", "k3_2", "k3_4", "l3_1", "l3_2", "l3_3", "l3_4", "m3_1", "m3_3", "m3_14", "h3_1", "h3_2", "h3_3", "h3_4", "o3_1", "o3_2", "o3_4" } };
            TileEdge m3_2 = new TileEdge { edgeType = "m3_2", compatibleEdgeTypes = new List<string> { "a3_1", "b3_4" } };
            TileEdge m3_3 = new TileEdge { edgeType = "m3_3", compatibleEdgeTypes = new List<string> { "b3_2", "c3_1", "c3_2", "c3_3", "c3_4", "e3_1", "e3_2", "f3_1", "f3_2", "f3_4", "g3_1", "g3_3", "h3_3", "h3_4", "i3_1", "i3_2", "i3_3", "i3_4", "j3_1", "j3_2", "j3_3", "j3_4", "k3_1", "k3_2", "k3_4", "l3_1", "l3_2", "l3_3", "l3_4", "m3_1", "m3_3", "m3_14", "h3_1", "h3_2", "h3_3", "h3_4", "o3_1", "o3_2", "o3_4" } };
            TileEdge m3_4 = new TileEdge { edgeType = "m3_4", compatibleEdgeTypes = new List<string> { "b3_2", "c3_1", "c3_2", "c3_3", "c3_4", "e3_1", "e3_2", "f3_1", "f3_2", "f3_4", "g3_1", "g3_3", "h3_3", "h3_4", "i3_1", "i3_2", "i3_3", "i3_4", "j3_1", "j3_2", "j3_3", "j3_4", "k3_1", "k3_2", "k3_4", "l3_1", "l3_2", "l3_3", "l3_4", "m3_1", "m3_3", "m3_14", "h3_1", "h3_2", "h3_3", "h3_4", "o3_1", "o3_2", "o3_4" } };
            TileEdge n3_1 = new TileEdge { edgeType = "n3_1", compatibleEdgeTypes = new List<string> { "g3_1", "g3_3", "e3_1", "i3_1", "i3_2", "i3_3", "i3_4", "l3_3", "l3_2" } };
            TileEdge n3_2 = new TileEdge { edgeType = "n3_2", compatibleEdgeTypes = new List<string> { "b3_2", "c3_1", "c3_2", "c3_3", "c3_4", "e3_1", "e3_2", "f3_1", "f3_2", "f3_4", "g3_1", "g3_3", "h3_3", "h3_4", "i3_1", "i3_2", "i3_3", "i3_4", "j3_1", "j3_2", "j3_3", "j3_4", "k3_1", "k3_2", "k3_4", "l3_1", "l3_2", "l3_3", "l3_4", "m3_1", "m3_3", "m3_14", "h3_1", "h3_2", "h3_3", "h3_4", "o3_1", "o3_2", "o3_4" } };
            TileEdge n3_3 = new TileEdge { edgeType = "n3_3", compatibleEdgeTypes = new List<string> { "b3_2", "c3_1", "c3_2", "c3_3", "c3_4", "e3_1", "e3_2", "f3_1", "f3_2", "f3_4", "g3_1", "g3_3", "h3_3", "h3_4", "i3_1", "i3_2", "i3_3", "i3_4", "j3_1", "j3_2", "j3_3", "j3_4", "k3_1", "k3_2", "k3_4", "l3_1", "l3_2", "l3_3", "l3_4", "m3_1", "m3_3", "m3_14", "h3_1", "h3_2", "h3_3", "h3_4", "o3_1", "o3_2", "o3_4" } };
            TileEdge n3_4 = new TileEdge { edgeType = "n3_4", compatibleEdgeTypes = new List<string> { "b3_2", "c3_1", "c3_2", "c3_3", "c3_4", "e3_1", "e3_2", "f3_1", "f3_2", "f3_4", "g3_1", "g3_3", "h3_3", "h3_4", "i3_1", "i3_2", "i3_3", "i3_4", "j3_1", "j3_2", "j3_3", "j3_4", "k3_1", "k3_2", "k3_4", "l3_1", "l3_2", "l3_3", "l3_4", "m3_1", "m3_3", "m3_14", "h3_1", "h3_2", "h3_3", "h3_4", "o3_1", "o3_2", "o3_4" } };
            TileEdge o3_1 = new TileEdge { edgeType = "o3_1", compatibleEdgeTypes = new List<string> { "b3_2", "c3_1", "c3_2", "c3_3", "c3_4", "e3_1", "e3_2", "f3_1", "f3_2", "f3_4", "g3_1", "g3_3", "h3_3", "h3_4", "i3_1", "i3_2", "i3_3", "i3_4", "j3_1", "j3_2", "j3_3", "j3_4", "k3_1", "k3_2", "k3_4", "l3_1", "l3_2", "l3_3", "l3_4", "m3_1", "m3_3", "m3_14", "h3_1", "h3_2", "h3_3", "h3_4", "o3_1", "o3_2", "o3_4" } };
            TileEdge o3_2 = new TileEdge { edgeType = "o3_2", compatibleEdgeTypes = new List<string> { "b3_2", "c3_1", "c3_2", "c3_3", "c3_4", "e3_1", "e3_2", "f3_1", "f3_2", "f3_4", "g3_1", "g3_3", "h3_3", "h3_4", "i3_1", "i3_2", "i3_3", "i3_4", "j3_1", "j3_2", "j3_3", "j3_4", "k3_1", "k3_2", "k3_4", "l3_1", "l3_2", "l3_3", "l3_4", "m3_1", "m3_3", "m3_14", "h3_1", "h3_2", "h3_3", "h3_4", "o3_1", "o3_2", "o3_4" } };
            TileEdge o3_3 = new TileEdge { edgeType = "o3_3", compatibleEdgeTypes = new List<string> { "a3_1", "b3_4" } };
            TileEdge o3_4 = new TileEdge { edgeType = "o3_4", compatibleEdgeTypes = new List<string> { "b3_2", "c3_1", "c3_2", "c3_3", "c3_4", "e3_1", "e3_2", "f3_1", "f3_2", "f3_4", "g3_1", "g3_3", "h3_3", "h3_4", "i3_1", "i3_2", "i3_3", "i3_4", "j3_1", "j3_2", "j3_3", "j3_4", "k3_1", "k3_2", "k3_4", "l3_1", "l3_2", "l3_3", "l3_4", "m3_1", "m3_3", "m3_14", "h3_1", "h3_2", "h3_3", "h3_4", "o3_1", "o3_2", "o3_4" } };

            // glacier

            TileEdge a4_1 = new TileEdge { edgeType = "a4_1", compatibleEdgeTypes = new List<string> { "a4_2", "a4_4", "b4_3", "b4_4", "c4_1", "c4_2", "d4_1", "d4_2", "d4_3", "e4_1", "e4_2", "f4_2", "g4_2", "g4_3", "h4_2", "i4_1", "i4_2", "i4_4", "j4_2", "j4_3", "l4_1", "l4_2", "l4_3", "l4_4", "m4_1", "m4_3", "n4_1", "n4_2", "n4_3", "n4_4" } };
            TileEdge a4_2 = new TileEdge { edgeType = "a4_2", compatibleEdgeTypes = new List<string> { "a4_2", "a4_4", "b4_3", "b4_4", "c4_1", "c4_2", "d4_1", "d4_2", "d4_3", "e4_1", "e4_2", "f4_2", "g4_2", "g4_3", "h4_2", "i4_1", "i4_2", "i4_4", "j4_2", "j4_3", "l4_1", "l4_2", "l4_3", "l4_4", "m4_1", "m4_3", "n4_1", "n4_2", "n4_3", "n4_4" } };
            TileEdge a4_3 = new TileEdge { edgeType = "a4_3", compatibleEdgeTypes = new List<string> { "b4_3", "c4_2", "e4_1", "e4_2", "i4_4", "j4_4", "k4_2", "k4_4" } };
            TileEdge a4_4 = new TileEdge { edgeType = "a4_4", compatibleEdgeTypes = new List<string> { "a4_2", "a4_4", "b4_3", "b4_4", "c4_1", "c4_2", "d4_1", "d4_2", "d4_3", "e4_1", "e4_2", "f4_2", "g4_2", "g4_3", "h4_2", "i4_1", "i4_2", "i4_4", "j4_2", "j4_3", "l4_1", "l4_2", "l4_3", "l4_4", "m4_1", "m4_3", "n4_1", "n4_2", "n4_3", "n4_4" } };
            TileEdge b4_1 = new TileEdge { edgeType = "b4_1", compatibleEdgeTypes = new List<string> { "b4_3", "c4_2", "e4_1", "e4_2", "i4_4", "j4_4", "k4_2", "k4_4" } };
            TileEdge b4_2 = new TileEdge { edgeType = "b4_2", compatibleEdgeTypes = new List<string> { "e4_2", "i4_4", "j4_4", "k4_2", "k4_4" } };
            TileEdge b4_3 = new TileEdge { edgeType = "b4_3", compatibleEdgeTypes = new List<string> { "a4_3", "d4_4", "e4_4", "f4_1", "g4_1", "g4_4", "h4_1", "j4_1" } };
            TileEdge b4_4 = new TileEdge { edgeType = "b4_4", compatibleEdgeTypes = new List<string> { "a4_2", "a4_4", "b4_3", "b4_4", "c4_1", "c4_2", "d4_1", "d4_2", "d4_3", "e4_1", "e4_2", "f4_2", "g4_2", "g4_3", "h4_2", "i4_1", "i4_2", "i4_4", "j4_2", "j4_3", "l4_1", "l4_2", "l4_3", "l4_4", "m4_1", "m4_3", "n4_1", "n4_2", "n4_3", "n4_4" } };
            TileEdge c4_1 = new TileEdge { edgeType = "c4_1", compatibleEdgeTypes = new List<string> { "a4_2", "a4_4", "b4_3", "b4_4", "c4_1", "c4_2", "d4_1", "d4_2", "d4_3", "e4_1", "e4_2", "f4_2", "g4_2", "g4_3", "h4_2", "i4_1", "i4_2", "i4_4", "j4_2", "j4_3", "l4_1", "l4_2", "l4_3", "l4_4", "m4_1", "m4_3", "n4_1", "n4_2", "n4_3", "n4_4" } };
            TileEdge c4_2 = new TileEdge { edgeType = "c4_2", compatibleEdgeTypes = new List<string> { "a4_3", "d4_4", "e4_4", "f4_1", "g4_1", "g4_4", "h4_1", "j4_1" } };
            TileEdge c4_3 = new TileEdge { edgeType = "c4_3", compatibleEdgeTypes = new List<string> { "e4_2", "i4_4", "j4_4", "k4_2", "k4_4" } };
            TileEdge c4_4 = new TileEdge { edgeType = "c4_4", compatibleEdgeTypes = new List<string> { "e4_2", "i4_4", "j4_4", "k4_2", "k4_4" } };
            TileEdge d4_1 = new TileEdge { edgeType = "d4_1", compatibleEdgeTypes = new List<string> { "a4_2", "a4_4", "b4_3", "b4_4", "c4_1", "c4_2", "d4_1", "d4_2", "d4_3", "e4_1", "e4_2", "f4_2", "g4_2", "g4_3", "h4_2", "i4_1", "i4_2", "i4_4", "j4_2", "j4_3", "l4_1", "l4_2", "l4_3", "l4_4", "m4_1", "m4_3", "n4_1", "n4_2", "n4_3", "n4_4" } };
            TileEdge d4_2 = new TileEdge { edgeType = "d4_2", compatibleEdgeTypes = new List<string> { "a4_2", "a4_4", "b4_3", "b4_4", "c4_1", "c4_2", "d4_1", "d4_2", "d4_3", "e4_1", "e4_2", "f4_2", "g4_2", "g4_3", "h4_2", "i4_1", "i4_2", "i4_4", "j4_2", "j4_3", "l4_1", "l4_2", "l4_3", "l4_4", "m4_1", "m4_3", "n4_1", "n4_2", "n4_3", "n4_4" } };
            TileEdge d4_3 = new TileEdge { edgeType = "d4_3", compatibleEdgeTypes = new List<string> { "a4_2", "a4_4", "b4_3", "b4_4", "c4_1", "c4_2", "d4_1", "d4_2", "d4_3", "e4_1", "e4_2", "f4_2", "g4_2", "g4_3", "h4_2", "i4_1", "i4_2", "i4_4", "j4_2", "j4_3", "l4_1", "l4_2", "l4_3", "l4_4", "m4_1", "m4_3", "n4_1", "n4_2", "n4_3", "n4_4" } };
            TileEdge d4_4 = new TileEdge { edgeType = "d4_4", compatibleEdgeTypes = new List<string> { "b4_3", "c4_2", "e4_1", "e4_2", "i4_4", "j4_4", "k4_2", "k4_4" } };
            TileEdge e4_1 = new TileEdge { edgeType = "e4_1", compatibleEdgeTypes = new List<string> { "a4_3", "d4_4", "e4_4", "f4_1", "g4_1", "g4_4", "h4_1", "j4_1" } };
            TileEdge e4_2 = new TileEdge { edgeType = "e4_2", compatibleEdgeTypes = new List<string> { "c4_3", "c4_4", "f4_3", "f4_4", "j4_1", "j4_3", "m4_2", "m4_4" } };
            TileEdge e4_3 = new TileEdge { edgeType = "e4_3", compatibleEdgeTypes = new List<string> { "e4_2", "i4_4", "j4_4", "k4_2", "k4_4" } };
            TileEdge e4_4 = new TileEdge { edgeType = "e4_4", compatibleEdgeTypes = new List<string> { "b4_3", "c4_2", "e4_1", "e4_2", "i4_4", "j4_4", "k4_2", "k4_4" } };
            TileEdge f4_1 = new TileEdge { edgeType = "f4_1", compatibleEdgeTypes = new List<string> { "b4_3", "c4_2", "e4_1", "e4_2", "i4_4", "j4_4", "k4_2", "k4_4" } };
            TileEdge f4_2 = new TileEdge { edgeType = "f4_2", compatibleEdgeTypes = new List<string> { "a4_3", "d4_4", "e4_4", "f4_1", "g4_1", "g4_4", "h4_1", "j4_1" } };
            TileEdge f4_3 = new TileEdge { edgeType = "f4_3", compatibleEdgeTypes = new List<string> { "e4_2", "i4_4", "j4_4", "k4_2", "k4_4" } };
            TileEdge f4_4 = new TileEdge { edgeType = "f4_4", compatibleEdgeTypes = new List<string> { "e4_2", "i4_4", "j4_4", "k4_2", "k4_4" } };
            TileEdge g4_1 = new TileEdge { edgeType = "g4_1", compatibleEdgeTypes = new List<string> { "b4_3", "c4_2", "e4_1", "e4_2", "i4_4", "j4_4", "k4_2", "k4_4" } };
            TileEdge g4_2 = new TileEdge { edgeType = "g4_2", compatibleEdgeTypes = new List<string> { "a4_3", "d4_4", "e4_4", "f4_1", "g4_1", "g4_4", "h4_1", "j4_1" } };
            TileEdge g4_3 = new TileEdge { edgeType = "g4_3", compatibleEdgeTypes = new List<string> { "a4_2", "a4_4", "b4_3", "b4_4", "c4_1", "c4_2", "d4_1", "d4_2", "d4_3", "e4_1", "e4_2", "f4_2", "g4_2", "g4_3", "h4_2", "i4_1", "i4_2", "i4_4", "j4_2", "j4_3", "l4_1", "l4_2", "l4_3", "l4_4", "m4_1", "m4_3", "n4_1", "n4_2", "n4_3", "n4_4" } };
            TileEdge g4_4 = new TileEdge { edgeType = "g4_4", compatibleEdgeTypes = new List<string> { "b4_3", "c4_2", "e4_1", "e4_2", "i4_4", "j4_4", "k4_2", "k4_4" } };
            TileEdge h4_1 = new TileEdge { edgeType = "h4_1", compatibleEdgeTypes = new List<string> { "e4_2", "i4_4", "j4_4", "k4_2", "k4_4" } };
            TileEdge h4_2 = new TileEdge { edgeType = "h4_2", compatibleEdgeTypes = new List<string> { "a4_2", "a4_4", "b4_3", "b4_4", "c4_1", "c4_2", "d4_1", "d4_2", "d4_3", "e4_1", "e4_2", "f4_2", "g4_2", "g4_3", "h4_2", "i4_1", "i4_2", "i4_4", "j4_2", "j4_3", "l4_1", "l4_2", "l4_3", "l4_4", "m4_1", "m4_3", "n4_1", "n4_2", "n4_3", "n4_4" } };
            TileEdge h4_3 = new TileEdge { edgeType = "h4_3", compatibleEdgeTypes = new List<string> { "a4_2", "a4_4", "b4_3", "b4_4", "c4_1", "c4_2", "d4_1", "d4_2", "d4_3", "e4_1", "e4_2", "f4_2", "g4_2", "g4_3", "h4_2", "i4_1", "i4_2", "i4_4", "j4_2", "j4_3", "l4_1", "l4_2", "l4_3", "l4_4", "m4_1", "m4_3", "n4_1", "n4_2", "n4_3", "n4_4" } };
            TileEdge h4_4 = new TileEdge { edgeType = "h4_4", compatibleEdgeTypes = new List<string> { "e4_2", "i4_4", "j4_4", "k4_2", "k4_4" } };
            TileEdge i4_1 = new TileEdge { edgeType = "i4_1", compatibleEdgeTypes = new List<string> { "a4_2", "a4_4", "b4_3", "b4_4", "c4_1", "c4_2", "d4_1", "d4_2", "d4_3", "e4_1", "e4_2", "f4_2", "g4_2", "g4_3", "h4_2", "i4_1", "i4_2", "i4_4", "j4_2", "j4_3", "l4_1", "l4_2", "l4_3", "l4_4", "m4_1", "m4_3", "n4_1", "n4_2", "n4_3", "n4_4" } };
            TileEdge i4_2 = new TileEdge { edgeType = "i4_2", compatibleEdgeTypes = new List<string> { "a4_2", "a4_4", "b4_3", "b4_4", "c4_1", "c4_2", "d4_1", "d4_2", "d4_3", "e4_1", "e4_2", "f4_2", "g4_2", "g4_3", "h4_2", "i4_1", "i4_2", "i4_4", "j4_2", "j4_3", "l4_1", "l4_2", "l4_3", "l4_4", "m4_1", "m4_3", "n4_1", "n4_2", "n4_3", "n4_4" } };
            TileEdge i4_3 = new TileEdge { edgeType = "i4_3", compatibleEdgeTypes = new List<string> { "e4_2", "i4_4", "j4_4", "k4_2", "k4_4" } };
            TileEdge i4_4 = new TileEdge { edgeType = "i4_4", compatibleEdgeTypes = new List<string> { "c4_3", "c4_4", "f4_3", "f4_4", "j4_1", "j4_3", "m4_2", "m4_4" } };
            TileEdge j4_1 = new TileEdge { edgeType = "j4_1", compatibleEdgeTypes = new List<string> { "e4_2", "i4_4", "j4_4", "k4_2", "k4_4" } };
            TileEdge j4_2 = new TileEdge { edgeType = "j4_2", compatibleEdgeTypes = new List<string> { "a4_3", "d4_4", "e4_4", "f4_1", "g4_1", "g4_4", "h4_1", "j4_1" } };
            TileEdge j4_3 = new TileEdge { edgeType = "j4_3", compatibleEdgeTypes = new List<string> { "a4_2", "a4_4", "b4_3", "b4_4", "c4_1", "c4_2", "d4_1", "d4_2", "d4_3", "e4_1", "e4_2", "f4_2", "g4_2", "g4_3", "h4_2", "i4_1", "i4_2", "i4_4", "j4_2", "j4_3", "l4_1", "l4_2", "l4_3", "l4_4", "m4_1", "m4_3", "n4_1", "n4_2", "n4_3", "n4_4" } };
            TileEdge j4_4 = new TileEdge { edgeType = "j4_4", compatibleEdgeTypes = new List<string> { "c4_3", "c4_4", "f4_3", "f4_4", "j4_1", "j4_3", "m4_2", "m4_4" } };
            TileEdge k4_1 = new TileEdge { edgeType = "k4_1", compatibleEdgeTypes = new List<string> { "a4_2", "a4_4", "b4_3", "b4_4", "c4_1", "c4_2", "d4_1", "d4_2", "d4_3", "e4_1", "e4_2", "f4_2", "g4_2", "g4_3", "h4_2", "i4_1", "i4_2", "i4_4", "j4_2", "j4_3", "l4_1", "l4_2", "l4_3", "l4_4", "m4_1", "m4_3", "n4_1", "n4_2", "n4_3", "n4_4" } };
            TileEdge k4_2 = new TileEdge { edgeType = "k4_2", compatibleEdgeTypes = new List<string> { "c4_3", "c4_4", "f4_3", "f4_4", "j4_1", "j4_3", "m4_2", "m4_4" } };
            TileEdge k4_3 = new TileEdge { edgeType = "k4_3", compatibleEdgeTypes = new List<string> { "a4_2", "a4_4", "b4_3", "b4_4", "c4_1", "c4_2", "d4_1", "d4_2", "d4_3", "e4_1", "e4_2", "f4_2", "g4_2", "g4_3", "h4_2", "i4_1", "i4_2", "i4_4", "j4_2", "j4_3", "l4_1", "l4_2", "l4_3", "l4_4", "m4_1", "m4_3", "n4_1", "n4_2", "n4_3", "n4_4" } };
            TileEdge k4_4 = new TileEdge { edgeType = "k4_4", compatibleEdgeTypes = new List<string> { "a4_3", "d4_4", "e4_4", "f4_1", "g4_1", "g4_4", "h4_1", "j4_1" } };
            TileEdge l4_1 = new TileEdge { edgeType = "l4_1", compatibleEdgeTypes = new List<string> { "a4_2", "a4_4", "b4_3", "b4_4", "c4_1", "c4_2", "d4_1", "d4_2", "d4_3", "e4_1", "e4_2", "f4_2", "g4_2", "g4_3", "h4_2", "i4_1", "i4_2", "i4_4", "j4_2", "j4_3", "l4_1", "l4_2", "l4_3", "l4_4", "m4_1", "m4_3", "n4_1", "n4_2", "n4_3", "n4_4" } };
            TileEdge l4_2 = new TileEdge { edgeType = "l4_2", compatibleEdgeTypes = new List<string> { "a4_2", "a4_4", "b4_3", "b4_4", "c4_1", "c4_2", "d4_1", "d4_2", "d4_3", "e4_1", "e4_2", "f4_2", "g4_2", "g4_3", "h4_2", "i4_1", "i4_2", "i4_4", "j4_2", "j4_3", "l4_1", "l4_2", "l4_3", "l4_4", "m4_1", "m4_3", "n4_1", "n4_2", "n4_3", "n4_4" } };
            TileEdge l4_3 = new TileEdge { edgeType = "l4_3", compatibleEdgeTypes = new List<string> { "a4_2", "a4_4", "b4_3", "b4_4", "c4_1", "c4_2", "d4_1", "d4_2", "d4_3", "e4_1", "e4_2", "f4_2", "g4_2", "g4_3", "h4_2", "i4_1", "i4_2", "i4_4", "j4_2", "j4_3", "l4_1", "l4_2", "l4_3", "l4_4", "m4_1", "m4_3", "n4_1", "n4_2", "n4_3", "n4_4" } };
            TileEdge l4_4 = new TileEdge { edgeType = "l4_4", compatibleEdgeTypes = new List<string> { "a4_2", "a4_4", "b4_3", "b4_4", "c4_1", "c4_2", "d4_1", "d4_2", "d4_3", "e4_1", "e4_2", "f4_2", "g4_2", "g4_3", "h4_2", "i4_1", "i4_2", "i4_4", "j4_2", "j4_3", "l4_1", "l4_2", "l4_3", "l4_4", "m4_1", "m4_3", "n4_1", "n4_2", "n4_3", "n4_4" } };
            TileEdge m4_1 = new TileEdge { edgeType = "m4_1", compatibleEdgeTypes = new List<string> { "a4_2", "a4_4", "b4_3", "b4_4", "c4_1", "c4_2", "d4_1", "d4_2", "d4_3", "e4_1", "e4_2", "f4_2", "g4_2", "g4_3", "h4_2", "i4_1", "i4_2", "i4_4", "j4_2", "j4_3", "l4_1", "l4_2", "l4_3", "l4_4", "m4_1", "m4_3", "n4_1", "n4_2", "n4_3", "n4_4" } };
            TileEdge m4_2 = new TileEdge { edgeType = "m4_2", compatibleEdgeTypes = new List<string> { "e4_2", "i4_4", "j4_4", "k4_2", "k4_4" } };
            TileEdge m4_3 = new TileEdge { edgeType = "m4_3", compatibleEdgeTypes = new List<string> { "a4_2", "a4_4", "b4_3", "b4_4", "c4_1", "c4_2", "d4_1", "d4_2", "d4_3", "e4_1", "e4_2", "f4_2", "g4_2", "g4_3", "h4_2", "i4_1", "i4_2", "i4_4", "j4_2", "j4_3", "l4_1", "l4_2", "l4_3", "l4_4", "m4_1", "m4_3", "n4_1", "n4_2", "n4_3", "n4_4" } };
            TileEdge m4_4 = new TileEdge { edgeType = "m4_4", compatibleEdgeTypes = new List<string> { "e4_2", "i4_4", "j4_4", "k4_2", "k4_4" } };
            TileEdge n4_1 = new TileEdge { edgeType = "n4_1", compatibleEdgeTypes = new List<string> { "a4_2", "a4_4", "b4_3", "b4_4", "c4_1", "c4_2", "d4_1", "d4_2", "d4_3", "e4_1", "e4_2", "f4_2", "g4_2", "g4_3", "h4_2", "i4_1", "i4_2", "i4_4", "j4_2", "j4_3", "l4_1", "l4_2", "l4_3", "l4_4", "m4_1", "m4_3", "n4_1", "n4_2", "n4_3", "n4_4" } };
            TileEdge n4_2 = new TileEdge { edgeType = "n4_2", compatibleEdgeTypes = new List<string> { "a4_2", "a4_4", "b4_3", "b4_4", "c4_1", "c4_2", "d4_1", "d4_2", "d4_3", "e4_1", "e4_2", "f4_2", "g4_2", "g4_3", "h4_2", "i4_1", "i4_2", "i4_4", "j4_2", "j4_3", "l4_1", "l4_2", "l4_3", "l4_4", "m4_1", "m4_3", "n4_1", "n4_3", "n4_4" } };
            TileEdge n4_3 = new TileEdge { edgeType = "n4_3", compatibleEdgeTypes = new List<string> { "a4_2", "a4_4", "b4_3", "b4_4", "c4_1", "c4_2", "d4_1", "d4_2", "d4_3", "e4_1", "e4_2", "f4_2", "g4_2", "g4_3", "h4_2", "i4_1", "i4_2", "i4_4", "j4_2", "j4_3", "l4_1", "l4_2", "l4_3", "l4_4", "m4_1", "m4_3", "n4_1", "n4_2", "n4_3", "n4_4" } };
            TileEdge n4_4 = new TileEdge { edgeType = "n4_4", compatibleEdgeTypes = new List<string> { "a4_2", "a4_4", "b4_3", "b4_4", "c4_1", "c4_2", "d4_1", "d4_2", "d4_3", "e4_1", "e4_2", "f4_2", "g4_2", "g4_3", "h4_2", "i4_1", "i4_2", "i4_4", "j4_2", "j4_3", "l4_1", "l4_2", "l4_3", "l4_4", "m4_1", "m4_3", "n4_1", "n4_2", "n4_3", "n4_4" } };


            // ������ ���� Ÿ�� ����
            List<MapTile> desertTiles = new List<MapTile>
{
    new MapTile("desertTile1", desertPrefab1, "Desert", a3_1, a3_2, a3_4, a3_3),
    new MapTile("desertTile2", desertPrefab2, "Desert", b3_1, b3_2, b3_4, b3_3),
    new MapTile("desertTile3", desertPrefab3, "Desert", c3_1, c3_2, c3_4, c3_3),
    new MapTile("desertTile4", desertPrefab4, "Desert", d3_1, d3_2, d3_4, d3_3),
    new MapTile("desertTile5", desertPrefab5, "Desert", e3_1, e3_2, e3_4, e3_3),
    new MapTile("desertTile6", desertPrefab6, "Desert", f3_1, f3_2, f3_4, f3_3),
    new MapTile("desertTile7", desertPrefab7, "Desert", g3_1, g3_2, g3_4, g3_3),
    new MapTile("desertTile8", desertPrefab8, "Desert", h3_1, h3_2, h3_4, h3_3),
    new MapTile("desertTile9", desertPrefab9, "Desert", i3_1, i3_2, i3_4, i3_3),
    new MapTile("desertTile10", desertPrefab10, "Desert", j3_1, j3_2, j3_4, j3_3),
    new MapTile("desertTile11", desertPrefab11, "Desert", k3_1, k3_2, k3_4, k3_3),
    new MapTile("desertTile12", desertPrefab12, "Desert", l3_1, l3_2, l3_4, l3_3),
    new MapTile("desertTile13", desertPrefab13, "Desert", m3_1, m3_2, m3_4, m3_3),
    new MapTile("desertTile14", desertPrefab14, "Desert", n3_1, n3_2, n3_4, n3_3),
    new MapTile("desertTile15", desertPrefab15, "Desert", o3_1, o3_2, o3_4, o3_3)
};

            List<MapTile> forestTiles = new List<MapTile>
{// �ϵ�����
    new MapTile("forestTile1", forestPrefab1, "Forest", a1_4, a1_3, a1_2, a1_1),
    new MapTile("forestTile2", forestPrefab2, "Forest", b1_4, b1_3, b1_2, b1_1),
    new MapTile("forestTile3", forestPrefab3, "Forest", c1_4, c1_3, c1_2, c1_1),
    new MapTile("forestTile4", forestPrefab4, "Forest", d1_4, d1_3, d1_2, d1_1),
    new MapTile("forestTile5", forestPrefab5, "Forest", e1_4, e1_3, e1_2, e1_1),
    new MapTile("forestTile6", forestPrefab6, "Forest", f1_4, f1_3, f1_2, f1_1),
    new MapTile("forestTile7", forestPrefab7, "Forest", g1_4, g1_3, g1_2, g1_1),
    new MapTile("forestTile8", forestPrefab8, "Forest", h1_4, h1_3, h1_2, h1_1),
    new MapTile("forestTile9", forestPrefab9, "Forest", i1_4, i1_3, i1_2, i1_1),
    new MapTile("forestTile10", forestPrefab10, "Forest", j1_4, j1_3, j1_2, j1_1),
    new MapTile("forestTile11", forestPrefab11, "Forest", k1_4, k1_3, k1_2, k1_1),
    new MapTile("forestTile12", forestPrefab12, "Forest", l1_4, l1_3, l1_2, l1_1),
    new MapTile("forestTile13", forestPrefab13, "Forest", m1_4, m1_3, m1_2, m1_1),
    new MapTile("forestTile14", forestPrefab14, "Forest", n1_4, n1_3, n1_2, n1_1),
    new MapTile("forestTile15", forestPrefab15, "Forest", o1_4, o1_3, o1_2, o1_1),
    new MapTile("forestTile16", forestPrefab16, "Forest", p1_4, p1_3, p1_2, p1_1),
    new MapTile("forestTile17", forestPrefab17, "Forest", q1_4, q1_3, q1_2, q1_1),
    new MapTile("forestTile18", forestPrefab18, "Forest", r1_4, r1_3, r1_2, r1_1),
    new MapTile("forestTile19", forestPrefab19, "Forest", s1_4, s1_3, s1_2, s1_1)

};

            List<MapTile> glacierTiles = new List<MapTile>
{
    new MapTile("glacierTile1", glacierPrefab1, "Glacier", a4_1, a4_2, a4_4, a4_3),
    new MapTile("glacierTile2", glacierPrefab2, "Glacier", b4_1, b4_2, b4_4, b4_3),
    new MapTile("glacierTile3", glacierPrefab3, "Glacier", c4_1, c4_2, c4_4, c4_3),
    new MapTile("glacierTile4", glacierPrefab4, "Glacier", d4_1, d4_2, d4_4, d4_3),
    new MapTile("glacierTile5", glacierPrefab5, "Glacier", e4_1, e4_2, e4_4, e4_3),
    new MapTile("glacierTile6", glacierPrefab6, "Glacier", f4_1, f4_2, f4_4, f4_3),
    new MapTile("glacierTile7", glacierPrefab7, "Glacier", g4_1, g4_2, g4_4, g4_3),
    new MapTile("glacierTile8", glacierPrefab8, "Glacier", h4_1, h4_2, h4_4, h4_3),
    new MapTile("glacierTile9", glacierPrefab9, "Glacier", i4_1, i4_2, i4_4, i4_3),
    new MapTile("glacierTile10", glacierPrefab10, "Glacier", j4_1, j4_2, j4_4, j4_3),
    new MapTile("glacierTile11", glacierPrefab11, "Glacier", k4_1, k4_2, k4_4, k4_3),
    new MapTile("glacierTile12", glacierPrefab12, "Glacier", l4_1, l4_2, l4_4, l4_3),
    new MapTile("glacierTile13", glacierPrefab13, "Glacier", m4_1, m4_2, m4_4, m4_3),
    new MapTile("glacierTile14", glacierPrefab14, "Glacier", n4_1, n4_2, n4_4, n4_3)

};

            List<MapTile> volcanoTiles = new List<MapTile>
{
    new MapTile("volcanoTile1", volcanoPrefab1, "Volcano", a2_1, a2_2, a2_4, a2_3),
    new MapTile("volcanoTile2", volcanoPrefab2, "Volcano", b2_1, b2_2, b2_4, b2_3),
    new MapTile("volcanoTile3", volcanoPrefab3, "Volcano", c2_1, c2_2, c2_4, c2_3),
    new MapTile("volcanoTile4", volcanoPrefab4, "Volcano", d2_1, d2_2, d2_4, d2_3),
    new MapTile("volcanoTile5", volcanoPrefab5, "Volcano", e2_1, e2_2, e2_4, e2_3),
    new MapTile("volcanoTile6", volcanoPrefab6, "Volcano", f2_1, f2_2, f2_4, f2_3),
    new MapTile("volcanoTile7", volcanoPrefab7, "Volcano", g2_1, g2_2, g2_4, g2_3),
    new MapTile("volcanoTile8", volcanoPrefab8, "Volcano", h2_1, h2_2, h2_4, h2_3),
    new MapTile("volcanoTile9", volcanoPrefab9, "Volcano", i2_1, i2_2, i2_4, i2_3),
    new MapTile("volcanoTile10", volcanoPrefab10, "Volcano", j2_1, j2_2, j2_4, j2_3)

};
            // Ÿ�� ��Ʈ�� �ʱ�ȭ (����: Desert, Forest ����� Ÿ�� ����� ����)
            terrainTileSets["Desert"] = new List<MapTile>(desertTiles);
            terrainTileSets["Forest"] = new List<MapTile>(forestTiles);
            terrainTileSets["Glacier"] = new List<MapTile>(glacierTiles);
            terrainTileSets["Volcano"] = new List<MapTile>(volcanoTiles);
        }

        void GenerateInitialCells()
        {
            for (int x = -5; x <= 5; x++)
            {
                for (int y = -5; y <= 5; y++)
                {
                    Vector2Int cellKey = new Vector2Int(x, y);
                    string terrainType = GetTerrainType(Mathf.PerlinNoise(cellKey.x / (perlinScale * 2), cellKey.y / (perlinScale * 2)));
                    if (!terrainTileSets.ContainsKey(terrainType))
                    {
                        Debug.LogError("Terrain type " + terrainType + " not found in terrainTileSets.");
                        continue;
                    }

                    Cell newCell = new Cell
                    {
                        position = cellKey,
                        possibleTiles = new List<MapTile>(terrainTileSets[terrainType])
                    };
                    cells.Add(cellKey, newCell);
                }
            }

            CollapseAllCells();
        }

        void CollapseAllCells()
        {
            while (cells.Values.Any(cell => !cell.isCollapsed))
            {
                Cell nextCell = GetCellWithLowestEntropy();
                float height = Mathf.PerlinNoise(nextCell.position.x / perlinScale, nextCell.position.y / perlinScale) * maxHeight;
                Vector3 cellWorldPos = new Vector3(nextCell.position.x * tileSize, height, nextCell.position.y * tileSize);
                if (!CollapseCell(nextCell, cellWorldPos))
                {
                    backtrackStack.Push(nextCell);
                    Backtrack();
                }
                UpdateNeighborEntropies(nextCell);
            }
        }

        Cell GetCellWithLowestEntropy()
        {
            List<Cell> uncollapsedCells = new List<Cell>(cells.Values.Where(cell => !cell.isCollapsed));
            uncollapsedCells.Sort((a, b) => a.Entropy.CompareTo(b.Entropy));
            return uncollapsedCells[0];
        }

        void UpdateNeighborEntropies(Cell cell)
        {
            foreach (var direction in Direction.AllDirections)
            {
                Vector2Int neighborPos = cell.position + direction;
                if (cells.ContainsKey(neighborPos) && !cells[neighborPos].isCollapsed)
                {
                    UpdateCellEntropy(cells[neighborPos]);
                }
            }
        }

        void UpdateCellEntropy(Cell cell)
        {
            List<MapTile> compatibleTiles = new List<MapTile>(cell.possibleTiles);

            foreach (var direction in Direction.AllDirections)
            {
                Vector2Int neighborPos = cell.position + direction;
                if (cells.ContainsKey(neighborPos))
                {
                    Cell neighborCell = cells[neighborPos];
                    if (neighborCell.isCollapsed && neighborCell.instantiatedTile != null)
                    {
                        MapTile neighborTile = neighborCell.instantiatedTile.GetComponent<TileComponent>().mapTile;
                        string dir = Direction.VectorToDirection(direction);

                        compatibleTiles = compatibleTiles
                            .Where(tile => IsCompatible(tile, neighborTile, dir))
                            .ToList();
                    }
                }
            }

            cell.possibleTiles = compatibleTiles;
        }

        void Backtrack()
        {
            while (backtrackStack.Count > 0)
            {
                Cell backtrackCell = backtrackStack.Pop();
                backtrackCell.isCollapsed = false;
                if (backtrackCell.possibleTiles.Count > 0)
                {
                    backtrackCell.possibleTiles = new List<MapTile>(terrainTileSets[backtrackCell.possibleTiles[0].terrainType]);
                }
                // Remove invalid line with placeholders
                if (backtrackCell.possibleTiles.Count > 0)
                {
                    backtrackCell.possibleTiles = new List<MapTile>(terrainTileSets[backtrackCell.possibleTiles[0].terrainType]);
                }
                if (backtrackCell.instantiatedTile != null)
                {
                    Destroy(backtrackCell.instantiatedTile);
                    backtrackCell.instantiatedTile = null;
                }
            }
        }

        bool CollapseCell(Cell cell, Vector3 cellWorldPos)
        {
            if (cell.isCollapsed) return true;

            List<MapTile> compatibleTiles = new List<MapTile>(cell.possibleTiles);

            foreach (var direction in Direction.AllDirections)
            {
                Vector2Int neighborPos = cell.position + direction;
                if (cells.ContainsKey(neighborPos))
                {
                    Cell neighborCell = cells[neighborPos];
                    if (neighborCell.isCollapsed && neighborCell.instantiatedTile != null)
                    {
                        MapTile neighborTile = neighborCell.instantiatedTile.GetComponent<TileComponent>().mapTile;
                        string dir = Direction.VectorToDirection(direction);

                        compatibleTiles = compatibleTiles
                            .Where(tile => IsCompatible(tile, neighborTile, dir))
                            .ToList();
                    }
                }
            }

            if (compatibleTiles.Count == 0)
            {
                Debug.LogWarning("No compatible tiles found for cell at position: " + cell.position);
                return false;
            }

            MapTile selectedTile = compatibleTiles[Random.Range(0, compatibleTiles.Count)];
            cell.isCollapsed = true;
            cell.possibleTiles = new List<MapTile> { selectedTile };
            InstantiateTile(cell, cellWorldPos, selectedTile);
            return true;
        }

        void InstantiateTile(Cell cell, Vector3 cellWorldPos, MapTile tile)
        {
            if (tile.prefab == null)
            {
                Debug.LogError("Tile prefab is null for tile: " + tile.name);
                return;
            }

            GameObject instantiatedTile = Instantiate(tile.prefab, cellWorldPos, Quaternion.Euler(0, Random.Range(0, 4) * 90, 0));
            cell.instantiatedTile = instantiatedTile;
            TileComponent tileComponent = instantiatedTile.AddComponent<TileComponent>();
            tileComponent.mapTile = tile;
        }

        string GetTerrainType(float value)
        {
            if (value < 0.2f)
                return "Desert";
            else if (value < 0.4f)
                return "Volcano";
            else if (value < 0.6f)
                return "Forest";
            else if (value < 0.8f)
                return "Glacier";
            else
                return "Forest";
        }

        bool IsCompatible(MapTile tileA, MapTile tileB, string direction)
        {
            switch (direction)
            {
                case "North":
                    return tileA.northEdge.compatibleEdgeTypes.Contains(tileB.southEdge.edgeType);
                case "East":
                    return tileA.eastEdge.compatibleEdgeTypes.Contains(tileB.westEdge.edgeType);
                case "South":
                    return tileA.southEdge.compatibleEdgeTypes.Contains(tileB.northEdge.edgeType);
                case "West":
                    return tileA.westEdge.compatibleEdgeTypes.Contains(tileB.eastEdge.edgeType);
                default:
                    return false;
            }
        }

        Vector2Int GetPlayerCellPosition()
        {
            int playerCellX = Mathf.RoundToInt(player.position.x / tileSize);
            int playerCellY = Mathf.RoundToInt(player.position.z / tileSize);
            return new Vector2Int(playerCellX, playerCellY);
        }

        void UpdateCells()
        {
            Vector2Int playerCellPos = GetPlayerCellPosition();
            for (int x = -5; x <= 5; x++)
            {
                for (int y = -5; y <= 5; y++)
                {
                    Vector2Int cellKey = new Vector2Int(playerCellPos.x + x, playerCellPos.y + y);
                    if (!cells.ContainsKey(cellKey))
                    {
                        string terrainType = GetTerrainType(Mathf.PerlinNoise(cellKey.x / (perlinScale * 2), cellKey.y / (perlinScale * 2)));
                        if (!terrainTileSets.ContainsKey(terrainType))
                        {
                            Debug.LogError("Terrain type " + terrainType + " not found in terrainTileSets.");
                            continue;
                        }

                        Cell newCell = new Cell
                        {
                            position = cellKey,
                            possibleTiles = new List<MapTile>(terrainTileSets[terrainType])
                        };
                        cells.Add(cellKey, newCell);

                        float height = Mathf.PerlinNoise(cellKey.x / perlinScale, cellKey.y / perlinScale) * maxHeight;
                        Vector3 cellWorldPos = new Vector3(newCell.position.x * tileSize, height, newCell.position.y * tileSize);

                        if (!CollapseCell(newCell, cellWorldPos))
                        {
                            backtrackStack.Push(newCell);
                            Backtrack();
                            return;
                        }
                        UpdateNeighborEntropies(newCell);
                    }
                }
            }
        }
    }

    public class TileComponent : MonoBehaviour
    {
        public MapTile mapTile;
    }
}
