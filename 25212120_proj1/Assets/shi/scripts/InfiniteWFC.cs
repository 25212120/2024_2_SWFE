using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomNamespace
{
    [System.Serializable]
    public class TileEdge
    {
        public string edgeType;  // 현재 타일의 면 타입
        public List<string> compatibleEdgeTypes;  // 호환 가능한 면 타입 리스트
    }

    [System.Serializable]
    public class MapTile
    {
        public string name;           // 타일 이름 또는 ID
        public GameObject prefab;     // 타일 프리팹
        public string terrainType;    // 지형 타입 (예: Desert, Forest, Glacier, Volcano)

        // 각 타일의 면 (북쪽, 동쪽, 남쪽, 서쪽)
        public TileEdge northEdge;
        public TileEdge eastEdge;
        public TileEdge southEdge;
        public TileEdge westEdge;

        // 생성자 - 타일 이름, 프리팹, 지형 타입 및 각 방향의 면 설정
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

    public class Cell
    {
        public Vector2Int position;               // 셀의 그리드 위치
        public List<MapTile> possibleTiles;       // 셀에 배치할 수 있는 가능한 타일 목록
        public bool isCollapsed = false;          // 셀의 상태 (타일이 배치되었는지 여부)
        public GameObject instantiatedTile;       // 배치된 타일 인스턴스
        public string terrainType;                // 지형 타입
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
        public Transform player; // 플레이어의 Transform
        public int tileSize = 20; // 타일 크기
        public float perlinScale = 10.0f; // Perlin Noise 스케일
        public float maxHeight = 5.0f; // 최대 높이
        public Transform tileParent; // 타일의 부모가 될 Transform

        // 지형별로 여러 타일을 관리하기 위한 딕셔너리
        private Dictionary<string, List<MapTile>> terrainTileSets = new Dictionary<string, List<MapTile>>();
        private Dictionary<Vector2Int, Cell> cells = new Dictionary<Vector2Int, Cell>();
        private HashSet<Vector2Int> activeCells = new HashSet<Vector2Int>();

        void Start()
        {
            InitializeTiles(); // 타일 초기화
            UpdateCells(); // 초기 타일 생성
        }

        void InitializeTiles()
        {
            // 타일 프리팹 불러오기
            // desert
            GameObject desertPrefab1 = Resources.Load<GameObject>("Tile3_1");
            GameObject desertPrefab2 = Resources.Load<GameObject>("Tile3_2");
            GameObject desertPrefab3 = Resources.Load<GameObject>("Tile3_3");
            GameObject desertPrefab4 = Resources.Load<GameObject>("Tile3_4");
            GameObject desertPrefab5 = Resources.Load<GameObject>("Tile3_5");
            GameObject desertPrefab6 = Resources.Load<GameObject>("Tile3_6");
            GameObject desertPrefab7 = Resources.Load<GameObject>("Tile3_7");
            GameObject desertPrefab8 = Resources.Load<GameObject>("Tile3_8");
            GameObject desertPrefab9 = Resources.Load<GameObject>("Tile3_9");
            GameObject desertPrefab10 = Resources.Load<GameObject>("Tile3_10");
            GameObject desertPrefab11 = Resources.Load<GameObject>("Tile3_11");
            GameObject desertPrefab12 = Resources.Load<GameObject>("Tile3_12");
            GameObject desertPrefab13 = Resources.Load<GameObject>("Tile3_13");
            GameObject desertPrefab14 = Resources.Load<GameObject>("Tile3_14");
            GameObject desertPrefab15 = Resources.Load<GameObject>("Tile3_15");

            // forest
            GameObject forestPrefab1 = Resources.Load<GameObject>("Tile1_1");
            GameObject forestPrefab2 = Resources.Load<GameObject>("Tile1_2");
            GameObject forestPrefab3 = Resources.Load<GameObject>("Tile1_3");
            GameObject forestPrefab4 = Resources.Load<GameObject>("Tile1_4");
            GameObject forestPrefab5 = Resources.Load<GameObject>("Tile1_5");
            GameObject forestPrefab6 = Resources.Load<GameObject>("Tile1_6");
            GameObject forestPrefab7 = Resources.Load<GameObject>("Tile1_7");
            GameObject forestPrefab8 = Resources.Load<GameObject>("Tile1_8");
            GameObject forestPrefab9 = Resources.Load<GameObject>("Tile1_9");
            GameObject forestPrefab10 = Resources.Load<GameObject>("Tile1_01");
            GameObject forestPrefab11 = Resources.Load<GameObject>("Tile1_11");
            GameObject forestPrefab12 = Resources.Load<GameObject>("Tile1_12");
            GameObject forestPrefab13 = Resources.Load<GameObject>("Tile1_13");
            GameObject forestPrefab14 = Resources.Load<GameObject>("Tile1_14");
            GameObject forestPrefab15 = Resources.Load<GameObject>("Tile1_15");
            GameObject forestPrefab16 = Resources.Load<GameObject>("Tile1_16");
            GameObject forestPrefab17 = Resources.Load<GameObject>("Tile1_17");
            GameObject forestPrefab18 = Resources.Load<GameObject>("Tile1_18");
            GameObject forestPrefab19 = Resources.Load<GameObject>("Tile1_19");

            // glacier
            GameObject glacierPrefab1 = Resources.Load<GameObject>("Tile3_1");
            GameObject glacierPrefab2 = Resources.Load<GameObject>("Tile3_2");
            GameObject glacierPrefab3 = Resources.Load<GameObject>("Tile3_3");
            GameObject glacierPrefab4 = Resources.Load<GameObject>("Tile3_4");
            GameObject glacierPrefab5 = Resources.Load<GameObject>("Tile3_5");
            GameObject glacierPrefab6 = Resources.Load<GameObject>("Tile3_6");
            GameObject glacierPrefab7 = Resources.Load<GameObject>("Tile3_7");
            GameObject glacierPrefab8 = Resources.Load<GameObject>("Tile3_8");
            GameObject glacierPrefab9 = Resources.Load<GameObject>("Tile3_9");
            GameObject glacierPrefab10 = Resources.Load<GameObject>("Tile3_10");
            GameObject glacierPrefab11 = Resources.Load<GameObject>("Tile3_11");
            GameObject glacierPrefab12 = Resources.Load<GameObject>("Tile3_12");
            GameObject glacierPrefab13 = Resources.Load<GameObject>("Tile3_13");
            GameObject glacierPrefab14 = Resources.Load<GameObject>("Tile3_14");
            GameObject glacierPrefab15 = Resources.Load<GameObject>("Tile3_15");
            GameObject glacierPrefab16 = Resources.Load<GameObject>("Tile3_16");
            GameObject glacierPrefab17 = Resources.Load<GameObject>("Tile3_17");
            GameObject glacierPrefab18 = Resources.Load<GameObject>("Tile3_18");

            //volcano
            GameObject volcanoPrefab1 = Resources.Load<GameObject>("Tile2_1");
            GameObject volcanoPrefab2 = Resources.Load<GameObject>("Tile2_2");
            GameObject volcanoPrefab3 = Resources.Load<GameObject>("Tile2_3");
            GameObject volcanoPrefab4 = Resources.Load<GameObject>("Tile2_4");
            GameObject volcanoPrefab5 = Resources.Load<GameObject>("Tile2_5");
            GameObject volcanoPrefab6 = Resources.Load<GameObject>("Tile2_6");
            GameObject volcanoPrefab7 = Resources.Load<GameObject>("Tile2_7");
            GameObject volcanoPrefab8 = Resources.Load<GameObject>("Tile2_8");
            GameObject volcanoPrefab9 = Resources.Load<GameObject>("Tile2_9");
            GameObject volcanoPrefab10 = Resources.Load<GameObject>("Tile2_10");
            

            // 각 타일의 면 정의
            // forest tile   1: north 2: east 3: west 4: south
            TileEdge a1_1 = new TileEdge { edgeType = "a1_1", compatibleEdgeTypes = new List<string> { "a1_3", "a1_4","c1_3","c1_2","b1_2","b1_3","b1_4","e1_3","e1_4","f1_3","f1_2","g1_1","g1_2","g1_3","g1_4","h1_1","h1_2","h1_3","h1_4","i1_1","i1_2","i1_3","i1_4","j1_1","j1_2","j1_3","j1_4","k1_1","k1_2","k1_3","l1_1","l1_2","l1_3","l1_4","m1_1","m1_3","m1_4","n1_1","n1_3","n1_4","o1_1","o1_2","o1_4","p1_1","p1_2","p1_3","p1_4","q1_3","q1_4","r1_1","r1_3","r1_4", "s1_1", "s1_2", "s1_3", "s1_4"} };
            TileEdge a1_2 = new TileEdge { edgeType = "a1_2", compatibleEdgeTypes = new List<string> { "c1_4", "d1_4", "e1_1", "f1_1" } };
            TileEdge a1_3 = new TileEdge { edgeType = "a1_3", compatibleEdgeTypes = new List<string> { "a1_3", "a1_4", "c1_3", "c1_2", "b1_2", "b1_3", "b1_4", "e1_3", "e1_4", "f1_3", "f1_2", "g1_1", "g1_2", "g1_3", "g1_4", "h1_1", "h1_2", "h1_3", "h1_4", "i1_1", "i1_2", "i1_3", "i1_4", "j1_1", "j1_2", "j1_3", "j1_4", "k1_1", "k1_2", "k1_3", "l1_1", "l1_2", "l1_3", "l1_4", "m1_1",  "m1_3", "m1_4", "n1_1",  "n1_3", "n1_4", "o1_1", "o1_2", "o1_4", "p1_1", "p1_2", "p1_3", "p1_4", "q1_3", "q1_4", "r1_1", "r1_3", "r1_4", "s1_1", "s1_2", "s1_3", "s1_4" } };
            TileEdge a1_4 = new TileEdge { edgeType = "a1_4", compatibleEdgeTypes = new List<string> { "a1_3", "a1_4", "c1_3", "c1_2", "b1_2", "b1_3", "b1_4", "e1_3", "e1_4", "f1_3", "f1_2", "g1_1", "g1_2", "g1_3", "g1_4", "h1_1", "h1_2", "h1_3", "h1_4", "i1_1", "i1_2", "i1_3", "i1_4", "j1_1", "j1_2", "j1_3", "j1_4", "k1_1", "k1_2", "k1_3", "l1_1", "l1_2", "l1_3", "l1_4", "m1_1", "m1_3", "m1_4", "n1_1", "n1_3", "n1_4", "o1_1", "o1_2", "o1_4", "p1_1", "p1_2", "p1_3", "p1_4", "q1_3", "q1_4", "r1_1", "r1_3", "r1_4", "s1_1", "s1_2", "s1_3", "s1_4" } };
            TileEdge b1_1 = new TileEdge { edgeType = "b1_1", compatibleEdgeTypes = new List<string> { "c1_4", "d1_4", "f1_4", "e1_1" } };
            TileEdge b1_2 = new TileEdge { edgeType = "b1_2", compatibleEdgeTypes = new List<string> { "a1_3", "a1_4", "c1_3", "c1_2", "b1_2", "b1_3", "b1_4", "e1_3", "e1_4", "f1_3", "f1_2", "g1_1", "g1_2", "g1_3", "g1_4", "h1_1", "h1_2", "h1_3", "h1_4", "i1_1", "i1_2", "i1_3", "i1_4", "j1_1", "j1_2", "j1_3", "j1_4", "k1_1", "k1_2", "k1_3", "l1_1", "l1_2", "l1_3", "l1_4", "m1_1",  "m1_3", "m1_4", "n1_1",  "n1_3", "n1_4", "o1_1", "o1_2", "o1_4", "p1_1", "p1_2", "p1_3", "p1_4", "q1_3", "q1_4", "r1_1", "r1_3", "r1_4", "s1_1", "s1_2", "s1_3", "s1_4" } };
            TileEdge b1_3 = new TileEdge { edgeType = "b1_3", compatibleEdgeTypes = new List<string> { "a1_3", "a1_4", "c1_3", "c1_2", "b1_2", "b1_3", "b1_4", "e1_3", "e1_4", "f1_3", "f1_2", "g1_1", "g1_2", "g1_3", "g1_4", "h1_1", "h1_2", "h1_3", "h1_4", "i1_1", "i1_2", "i1_3", "i1_4", "j1_1", "j1_2", "j1_3", "j1_4", "k1_1", "k1_2", "k1_3", "l1_1", "l1_2", "l1_3", "l1_4", "m1_1", "m1_3", "m1_4", "n1_1",  "n1_3", "n1_4", "o1_1", "o1_2", "o1_4", "p1_1", "p1_2", "p1_3", "p1_4", "q1_3", "q1_4", "r1_1", "r1_3", "r1_4", "s1_1", "s1_2", "s1_3", "s1_4" } };
            TileEdge b1_4 = new TileEdge { edgeType = "b1_4", compatibleEdgeTypes = new List<string> { "a1_3", "a1_4", "c1_3", "c1_2", "b1_2", "b1_3", "b1_4", "e1_3", "e1_4", "f1_3", "f1_2", "g1_1", "g1_2", "g1_3", "g1_4", "h1_1", "h1_2", "h1_3", "h1_4", "i1_1", "i1_2", "i1_3", "i1_4", "j1_1", "j1_2", "j1_3", "j1_4", "k1_1", "k1_2", "k1_3", "l1_1", "l1_2", "l1_3", "l1_4", "m1_1", "m1_3", "m1_4", "n1_1",  "n1_3", "n1_4", "o1_1", "o1_2", "o1_4", "p1_1", "p1_2", "p1_3", "p1_4", "q1_3", "q1_4", "r1_1", "r1_3", "r1_4", "s1_1", "s1_2", "s1_3", "s1_4" } };
            TileEdge c1_1 = new TileEdge { edgeType = "c1_1", compatibleEdgeTypes = new List<string> { "d1_4", "e1_1" } };
            TileEdge c1_2 = new TileEdge { edgeType = "c1_2", compatibleEdgeTypes = new List<string> { "l1_1", "l1_2", "l1_3", "l1_4", "m1_3", "g1_4"} };
            TileEdge c1_3 = new TileEdge { edgeType = "c1_3", compatibleEdgeTypes = new List<string> { "a1_3", "a1_4", "c1_3", "c1_2", "b1_2", "b1_3", "b1_4", "e1_3", "e1_4", "f1_3", "f1_2", "g1_1", "g1_2", "g1_3", "g1_4", "h1_1", "h1_2", "h1_3", "h1_4", "i1_1", "i1_2", "i1_3", "i1_4", "j1_1", "j1_2", "j1_3", "j1_4", "k1_1", "k1_2", "k1_3", "l1_1", "l1_2", "l1_3", "l1_4", "m1_1", "m1_2", "m1_3", "m1_4", "n1_1", "n1_2", "n1_3", "n1_4", "o1_1", "o1_2", "o1_4", "p1_1", "p1_2", "p1_3", "p1_4", "q1_3", "q1_4", "r1_1", "r1_3", "r1_4", "s1_1", "s1_2", "s1_3", "s1_4" } };
            TileEdge c1_4 = new TileEdge { edgeType = "c1_4", compatibleEdgeTypes = new List<string> { "i1_2", "b1_1", "d1_1", "e1_2" } };
            TileEdge d1_1 = new TileEdge { edgeType = "d1_1", compatibleEdgeTypes = new List<string> { "c1_4", "a1_2" } };
            TileEdge d1_2 = new TileEdge { edgeType = "d1_2", compatibleEdgeTypes = new List<string> { "a1_3", "a1_4", "c1_3", "c1_2", "b1_2", "b1_3", "b1_4", "e1_3", "e1_4", "f1_3", "f1_2", "g1_1", "g1_2", "g1_3", "g1_4", "h1_1", "h1_2", "h1_3", "h1_4", "i1_1", "i1_2", "i1_3", "i1_4", "j1_1", "j1_2", "j1_3", "j1_4", "k1_1", "k1_2", "k1_3", "l1_1", "l1_2", "l1_3", "l1_4", "m1_1", "m1_2", "m1_3", "m1_4", "n1_1", "n1_3", "n1_4", "o1_1", "o1_2", "o1_4", "p1_1", "p1_2", "p1_3", "p1_4", "q1_3", "q1_4", "r1_1", "r1_3", "r1_4", "s1_1", "s1_2", "s1_3", "s1_4" } };
            TileEdge d1_3 = new TileEdge { edgeType = "d1_3", compatibleEdgeTypes = new List<string> { "a1_3", "a1_4", "c1_3", "c1_2", "b1_2", "b1_3", "b1_4", "e1_3", "e1_4", "f1_3", "f1_2", "g1_1", "g1_2", "g1_3", "g1_4", "h1_1", "h1_2", "h1_3", "h1_4", "i1_1", "i1_2", "i1_3", "i1_4", "j1_1", "j1_2", "j1_3", "j1_4", "k1_1", "k1_2", "k1_3", "l1_1", "l1_2", "l1_3", "l1_4", "m1_1", "m1_2", "m1_3", "m1_4", "n1_1", "n1_2", "n1_3", "n1_4", "o1_1", "o1_2", "o1_4", "p1_1", "p1_2", "p1_3", "p1_4", "q1_1","q1_2", "q1_3", "q1_4", "r1_1", "r1_2", "r1_3", "r1_4", "s1_1", "s1_2", "s1_3", "s1_4" } };
            TileEdge d1_4 = new TileEdge { edgeType = "d1_4", compatibleEdgeTypes = new List<string> { "a1_2", "c1_1", "e1_2" } };
            TileEdge e1_1 = new TileEdge { edgeType = "e1_1", compatibleEdgeTypes = new List<string> { "a1_2", "b1_1", "c1_1", "d1_1" } };
            TileEdge e1_2 = new TileEdge { edgeType = "e1_2", compatibleEdgeTypes = new List<string> { "c1_4", "d1_4" } };
            TileEdge e1_3 = new TileEdge { edgeType = "e1_3", compatibleEdgeTypes = new List<string> { "a1_3", "a1_4", "c1_3", "c1_2", "b1_2", "b1_3", "b1_4", "e1_3", "e1_4", "f1_3", "f1_2", "g1_1", "g1_2", "g1_3", "g1_4", "h1_1", "h1_2", "h1_3", "h1_4", "i1_1", "i1_2", "i1_3", "i1_4", "j1_1", "j1_2", "j1_3", "j1_4", "k1_1", "k1_2", "k1_3", "l1_1", "l1_2", "l1_3", "l1_4", "m1_1", "m1_3", "m1_4", "n1_1", "n1_3", "n1_4", "o1_1", "o1_2", "o1_4", "p1_1", "p1_2", "p1_3", "p1_4", "q1_3", "q1_4", "r1_1", "r1_3", "r1_4", "s1_1", "s1_2", "s1_3", "s1_4" } };
            TileEdge e1_4 = new TileEdge { edgeType = "e1_4", compatibleEdgeTypes = new List<string> { "a1_3", "a1_4", "c1_3", "c1_2", "b1_2", "b1_3", "b1_4", "e1_3", "e1_4", "f1_3", "f1_2", "g1_1", "g1_2", "g1_3", "g1_4", "h1_1", "h1_2", "h1_3", "h1_4", "i1_1", "i1_2", "i1_3", "i1_4", "j1_1", "j1_2", "j1_3", "j1_4", "k1_1", "k1_2", "k1_3", "l1_1", "l1_2", "l1_3", "l1_4", "m1_1", "m1_3", "m1_4", "n1_1", "n1_3", "n1_4", "o1_1", "o1_2", "o1_4", "p1_1", "p1_2", "p1_3", "p1_4", "q1_3", "q1_4", "r1_1", "r1_3", "r1_4", "s1_1", "s1_2", "s1_3", "s1_4" } };
            TileEdge f1_1 = new TileEdge { edgeType = "f1_1", compatibleEdgeTypes = new List<string> { "a1_2", "b1_1", "c1_1", "d1_1", "e1_2" } };
            TileEdge f1_2 = new TileEdge { edgeType = "f1_2", compatibleEdgeTypes = new List<string> { "a1_3", "a1_4", "c1_3", "c1_2", "b1_2", "b1_3", "b1_4", "e1_3", "e1_4", "f1_3", "f1_2", "g1_1", "g1_2", "g1_3", "g1_4", "h1_1", "h1_2", "h1_3", "h1_4", "i1_1", "i1_2", "i1_3", "i1_4", "j1_1", "j1_2", "j1_3", "j1_4", "k1_1", "k1_2", "k1_3", "l1_1", "l1_2", "l1_3", "l1_4", "m1_1", "m1_3", "m1_4", "n1_1", "n1_3", "n1_4", "o1_1", "o1_2", "o1_4", "p1_1", "p1_2", "p1_3", "p1_4", "q1_3", "q1_4", "r1_1", "r1_3", "r1_4", "s1_1", "s1_2", "s1_3", "s1_4" } };
            TileEdge f1_3 = new TileEdge { edgeType = "f1_3", compatibleEdgeTypes = new List<string> { "a1_3", "a1_4", "c1_3", "c1_2", "b1_2", "b1_3", "b1_4", "e1_3", "e1_4", "f1_3", "f1_2", "g1_1", "g1_2", "g1_3", "g1_4", "h1_1", "h1_2", "h1_3", "h1_4", "i1_1", "i1_2", "i1_3", "i1_4", "j1_1", "j1_2", "j1_3", "j1_4", "k1_1", "k1_2", "k1_3", "l1_1", "l1_2", "l1_3", "l1_4", "m1_1", "m1_3", "m1_4", "n1_1", "n1_3", "n1_4", "o1_1", "o1_2", "o1_4", "p1_1", "p1_2", "p1_3", "p1_4", "q1_3", "q1_4", "r1_1", "r1_3", "r1_4", "s1_1", "s1_2", "s1_3", "s1_4" } };
            TileEdge f1_4 = new TileEdge { edgeType = "f1_4", compatibleEdgeTypes = new List<string> { "c1_1", "d1_1", "e1_2" } };
            TileEdge g1_1 = new TileEdge { edgeType = "g1_1", compatibleEdgeTypes = new List<string> { "a1_1", "a1_3", "a1_4", "b1_2", "b1_3", "b1_4", "e1_3", "e1_4", "k1_1", "k1_2", "k1_3", "l1_1", "l1_2", "l1_3", "l1_4", "m1_1", "m1_3", "m1_4", "n1_1", "n1_3", "n1_4", "o1_1", "o1_4", "q1_3", "q1_4", "r1_1", "r1_3", "r1_4", "s1_1", "s1_2", "s1_3", "s1_4" } };
            TileEdge g1_2 = new TileEdge { edgeType = "g1_2", compatibleEdgeTypes = new List<string> { "a1_1", "a1_3", "a1_4", "b1_2", "b1_3", "b1_4", "e1_3", "e1_4", "k1_1", "k1_2", "k1_3", "l1_1", "l1_2", "l1_3", "l1_4", "m1_1", "m1_3", "m1_4", "n1_1", "n1_3", "n1_4", "o1_1", "o1_4", "q1_3", "q1_4", "r1_1", "r1_3", "r1_4", "s1_1", "s1_2", "s1_3", "s1_4" } };
            TileEdge g1_3 = new TileEdge { edgeType = "g1_3", compatibleEdgeTypes = new List<string> { "a1_1", "a1_3", "a1_4", "b1_2", "b1_3", "b1_4", "e1_3", "e1_4", "k1_1", "k1_2", "k1_3", "l1_1", "l1_2", "l1_3", "l1_4", "m1_1", "m1_3", "m1_4", "n1_1", "n1_3", "n1_4", "o1_1", "o1_4", "q1_3", "q1_4", "r1_1", "r1_3", "r1_4", "s1_1", "s1_2", "s1_3", "s1_4" } };
            TileEdge g1_4 = new TileEdge { edgeType = "g1_4", compatibleEdgeTypes = new List<string> { "a1_1", "a1_3", "a1_4", "b1_2", "b1_3", "b1_4", "e1_3", "e1_4", "k1_1", "k1_2", "k1_3", "l1_1", "l1_2", "l1_3", "l1_4", "m1_1", "m1_3", "m1_4", "n1_1", "n1_3", "n1_4", "o1_1", "o1_4", "q1_3", "q1_4", "r1_1", "r1_3", "r1_4", "s1_1", "s1_2", "s1_3", "s1_4" } };
            TileEdge h1_1 = new TileEdge { edgeType = "h1_1", compatibleEdgeTypes = new List<string> { "a1_1", "a1_3", "a1_4", "b1_2", "b1_3", "b1_4", "e1_3", "e1_4", "k1_1", "k1_2", "k1_3", "l1_1", "l1_2", "l1_3", "l1_4", "m1_1", "m1_3", "m1_4", "n1_1", "n1_3", "n1_4", "o1_1", "o1_4", "q1_3", "q1_4", "r1_1", "r1_3", "r1_4", "s1_1", "s1_2", "s1_3", "s1_4" } };
            TileEdge h1_2 = new TileEdge { edgeType = "h1_2", compatibleEdgeTypes = new List<string> { "a1_1", "a1_3", "a1_4", "b1_2", "b1_3", "b1_4", "e1_3", "e1_4", "k1_1", "k1_2", "k1_3", "l1_1", "l1_2", "l1_3", "l1_4", "m1_1", "m1_3", "m1_4", "n1_1", "n1_3", "n1_4", "o1_1", "o1_4", "q1_3", "q1_4", "r1_1", "r1_3", "r1_4", "s1_1", "s1_2", "s1_3", "s1_4" } };
            TileEdge h1_3 = new TileEdge { edgeType = "h1_3", compatibleEdgeTypes = new List<string> { "a1_1", "a1_3", "a1_4", "b1_2", "b1_3", "b1_4", "e1_3", "e1_4", "k1_1", "k1_2", "k1_3", "l1_1", "l1_2", "l1_3", "l1_4", "m1_1", "m1_3", "m1_4", "n1_1", "n1_3", "n1_4", "o1_1", "o1_4", "q1_3", "q1_4", "r1_1", "r1_3", "r1_4", "s1_1", "s1_2", "s1_3", "s1_4" } };
            TileEdge h1_4 = new TileEdge { edgeType = "h1_4", compatibleEdgeTypes = new List<string> { "a1_1", "a1_3", "a1_4", "b1_2", "b1_3", "b1_4", "e1_3", "e1_4", "k1_1", "k1_2", "k1_3", "l1_1", "l1_2", "l1_3", "l1_4", "m1_1", "m1_3", "m1_4", "n1_1", "n1_3", "n1_4", "o1_1", "o1_4", "q1_3", "q1_4", "r1_1", "r1_3", "r1_4", "s1_1", "s1_2", "s1_3", "s1_4" } };
            TileEdge i1_1 = new TileEdge { edgeType = "i1_1", compatibleEdgeTypes = new List<string> { "a1_1", "a1_3", "a1_4", "b1_2", "b1_3", "b1_4", "e1_3", "e1_4", "k1_1", "k1_2", "k1_3", "l1_1", "l1_2", "l1_3", "l1_4", "m1_1", "m1_3", "m1_4", "n1_1", "n1_3", "n1_4", "o1_1", "o1_4", "q1_3", "q1_4", "r1_1", "r1_3", "r1_4", "s1_1", "s1_2", "s1_3", "s1_4" } };
            TileEdge i1_2 = new TileEdge { edgeType = "i1_2", compatibleEdgeTypes = new List<string> { "a1_1", "a1_3", "a1_4", "b1_2", "b1_3", "b1_4", "e1_3", "e1_4", "k1_1", "k1_2", "k1_3", "l1_1", "l1_2", "l1_3", "l1_4", "m1_1", "m1_3", "m1_4", "n1_1", "n1_3", "n1_4", "o1_1", "o1_4", "q1_3", "q1_4", "r1_1", "r1_3", "r1_4", "s1_1", "s1_2", "s1_3", "s1_4" } };
            TileEdge i1_3 = new TileEdge { edgeType = "i1_3", compatibleEdgeTypes = new List<string> { "a1_1", "a1_3", "a1_4", "b1_2", "b1_3", "b1_4", "e1_3", "e1_4", "k1_1", "k1_2", "k1_3", "l1_1", "l1_2", "l1_3", "l1_4", "m1_1", "m1_3", "m1_4", "n1_1", "n1_3", "n1_4", "o1_1", "o1_4", "q1_3", "q1_4", "r1_1", "r1_3", "r1_4", "s1_1", "s1_2", "s1_3", "s1_4" } };
            TileEdge i1_4 = new TileEdge { edgeType = "i1_4", compatibleEdgeTypes = new List<string> { "a1_1", "a1_3", "a1_4", "b1_2", "b1_3", "b1_4", "e1_3", "e1_4", "k1_1", "k1_2", "k1_3", "l1_1", "l1_2", "l1_3", "l1_4", "m1_1", "m1_3", "m1_4", "n1_1", "n1_3", "n1_4", "o1_1", "o1_4", "q1_3", "q1_4", "r1_1", "r1_3", "r1_4", "s1_1", "s1_2", "s1_3", "s1_4" } };
            TileEdge j1_1 = new TileEdge { edgeType = "j1_1", compatibleEdgeTypes = new List<string> { "a1_1", "a1_3", "a1_4", "b1_2", "b1_3", "b1_4", "e1_3", "e1_4", "k1_1", "k1_2", "k1_3", "l1_1", "l1_2", "l1_3", "l1_4", "m1_1", "m1_3", "m1_4", "n1_1", "n1_3", "n1_4", "o1_1", "o1_4", "q1_3", "q1_4", "r1_1", "r1_3", "r1_4", "s1_1", "s1_2", "s1_3", "s1_4" } };
            TileEdge j1_2 = new TileEdge { edgeType = "j1_2", compatibleEdgeTypes = new List<string> { "a1_1", "a1_3", "a1_4", "b1_2", "b1_3", "b1_4", "e1_3", "e1_4", "k1_1", "k1_2", "k1_3", "l1_1", "l1_2", "l1_3", "l1_4", "m1_1", "m1_3", "m1_4", "n1_1", "n1_3", "n1_4", "o1_1", "o1_4", "q1_3", "q1_4", "r1_1", "r1_3", "r1_4", "s1_1", "s1_2", "s1_3", "s1_4" } };
            TileEdge j1_3 = new TileEdge { edgeType = "j1_3", compatibleEdgeTypes = new List<string> { "a1_1", "a1_3", "a1_4", "b1_2", "b1_3", "b1_4", "e1_3", "e1_4", "k1_1", "k1_2", "k1_3", "l1_1", "l1_2", "l1_3", "l1_4", "m1_1", "m1_3", "m1_4", "n1_1", "n1_3", "n1_4", "o1_1", "o1_4", "q1_3", "q1_4", "r1_1", "r1_3", "r1_4", "s1_1", "s1_2", "s1_3", "s1_4" } };
            TileEdge j1_4 = new TileEdge { edgeType = "j1_4", compatibleEdgeTypes = new List<string> { "a1_1", "a1_3", "a1_4", "b1_2", "b1_3", "b1_4", "e1_3", "e1_4", "k1_1", "k1_2", "k1_3", "l1_1", "l1_2", "l1_3", "l1_4", "m1_1", "m1_3", "m1_4", "n1_1", "n1_3", "n1_4", "o1_1", "o1_4", "q1_3", "q1_4", "r1_1", "r1_3", "r1_4", "s1_1", "s1_2", "s1_3", "s1_4" } };
            TileEdge k1_1 = new TileEdge { edgeType = "k1_1", compatibleEdgeTypes = new List<string> { "a1_1", "a1_3", "a1_4", "b1_2", "b1_3", "b1_4", "e1_3", "e1_4",  "l1_1", "l1_2", "l1_3", "l1_4", "m1_1", "m1_3", "m1_4", "n1_1","n1_2", "n1_3", "n1_4", "o1_1", "o1_2", "o1_3", "o1_4", "q1_3", "q1_4", "r1_1", "r1_2", "r1_3", "r1_4", "s1_1", "s1_2", "s1_3", "s1_4" } };
            TileEdge k1_2 = new TileEdge { edgeType = "k1_2", compatibleEdgeTypes = new List<string> { "a1_1", "a1_3", "a1_4", "b1_2", "b1_3", "b1_4", "e1_3", "e1_4", "l1_1", "l1_2", "l1_3", "l1_4", "m1_1", "m1_3", "m1_4", "n1_1", "n1_2", "n1_3", "n1_4", "o1_1", "o1_2", "o1_3", "o1_4", "q1_3", "q1_4", "r1_1", "r1_2", "r1_3", "r1_4", "s1_1", "s1_2", "s1_3", "s1_4" } };
            TileEdge k1_3 = new TileEdge { edgeType = "k1_3", compatibleEdgeTypes = new List<string> { "a1_1", "a1_3", "a1_4", "b1_2", "b1_3", "b1_4", "e1_3", "e1_4", "l1_1", "l1_2", "l1_3", "l1_4", "m1_1", "m1_3", "m1_4", "n1_1", "n1_2", "n1_3", "n1_4", "o1_1", "o1_2", "o1_3", "o1_4", "q1_3", "q1_4", "r1_1", "r1_2", "r1_3", "r1_4", "s1_1", "s1_2", "s1_3", "s1_4" } };
            TileEdge k1_4 = new TileEdge { edgeType = "k1_4", compatibleEdgeTypes = new List<string> { "a1_1", "a1_3", "a1_4", "b1_2", "b1_3", "b1_4", "e1_3", "e1_4", "l1_1", "l1_2", "l1_3", "l1_4", "m1_1", "m1_3", "m1_4", "n1_1", "n1_2", "n1_3", "n1_4", "o1_1", "o1_2", "o1_3", "o1_4", "q1_3", "q1_4", "r1_1", "r1_2", "r1_3", "r1_4", "s1_1", "s1_2", "s1_3", "s1_4" } };
            TileEdge l1_1 = new TileEdge { edgeType = "l1_1", compatibleEdgeTypes = new List<string> { "a1_1", "a1_3", "a1_4", "b1_2", "b1_3", "b1_4", "e1_3", "e1_4", "l1_1", "l1_2", "l1_3", "l1_4", "m1_1", "m1_2", "m1_3", "m1_4", "n1_1", "n1_2", "n1_3", "n1_4", "o1_1", "o1_2", "o1_3", "o1_4", "q1_1", "q1_2", "q1_3", "q1_4", "r1_1", "r1_2", "r1_3", "r1_4", "s1_1", "s1_2", "s1_3", "s1_4" } };
            TileEdge l1_2 = new TileEdge { edgeType = "l1_2", compatibleEdgeTypes = new List<string> { "a1_1", "a1_3", "a1_4", "b1_2", "b1_3", "b1_4", "e1_3", "e1_4", "l1_1", "l1_2", "l1_3", "l1_4", "m1_1", "m1_2", "m1_3", "m1_4", "n1_1", "n1_2", "n1_3", "n1_4", "o1_1", "o1_2", "o1_3", "o1_4", "q1_1", "q1_2", "q1_3", "q1_4", "r1_1", "r1_2", "r1_3", "r1_4", "s1_1", "s1_2", "s1_3", "s1_4" } };
            TileEdge l1_3 = new TileEdge { edgeType = "l1_3", compatibleEdgeTypes = new List<string> { "a1_1", "a1_3", "a1_4", "b1_2", "b1_3", "b1_4", "e1_3", "e1_4", "l1_1", "l1_2", "l1_3", "l1_4", "m1_1", "m1_2", "m1_3", "m1_4", "n1_1", "n1_2", "n1_3", "n1_4", "o1_1", "o1_2", "o1_3", "o1_4", "q1_1", "q1_2", "q1_3", "q1_4", "r1_1", "r1_2", "r1_3", "r1_4", "s1_1", "s1_2", "s1_3", "s1_4" } };
            TileEdge l1_4 = new TileEdge { edgeType = "l1_4", compatibleEdgeTypes = new List<string> { "a1_1", "a1_3", "a1_4", "b1_2", "b1_3", "b1_4", "e1_3", "e1_4", "l1_1", "l1_2", "l1_3", "l1_4", "m1_1", "m1_2", "m1_3", "m1_4", "n1_1", "n1_2", "n1_3", "n1_4", "o1_1", "o1_2", "o1_3", "o1_4", "q1_1", "q1_2", "q1_3", "q1_4", "r1_1", "r1_2", "r1_3", "r1_4", "s1_1", "s1_2", "s1_3", "s1_4" } };
            TileEdge m1_1 = new TileEdge { edgeType = "m1_1", compatibleEdgeTypes = new List<string> { "a1_3", "a1_4", "c1_3", "c1_2", "b1_2", "b1_3", "b1_4", "e1_3", "e1_4", "f1_3", "f1_2", "g1_1", "g1_2", "g1_3", "g1_4", "h1_1", "h1_2", "h1_3", "h1_4", "i1_1", "i1_2", "i1_3", "i1_4", "j1_1", "j1_2", "j1_3", "j1_4", "k1_1", "k1_2", "k1_3", "l1_1", "l1_2", "l1_3", "l1_4", "m1_1", "m1_3", "m1_4", "n1_1", "n1_3", "n1_4", "o1_1", "o1_2", "o1_4", "p1_1", "p1_2", "p1_3", "p1_4", "q1_3", "q1_4", "r1_1", "r1_3", "r1_4", "s1_1", "s1_2", "s1_3", "s1_4" } };
            TileEdge m1_2 = new TileEdge { edgeType = "m1_2", compatibleEdgeTypes = new List<string> { "l1_1", "l1_2", "l1_3", "l1_4", "m1_3", "n1_3" } };
            TileEdge m1_3 = new TileEdge { edgeType = "m1_3", compatibleEdgeTypes = new List<string> { "n1_2", "o1_3", "o1_2", "r1_2", "q1_1", "q1_2" } };
            TileEdge m1_4 = new TileEdge { edgeType = "m1_4", compatibleEdgeTypes = new List<string> { "a1_3", "a1_4", "c1_3", "c1_2", "b1_2", "b1_3", "b1_4", "e1_3", "e1_4", "f1_3", "f1_2", "g1_1", "g1_2", "g1_3", "g1_4", "h1_1", "h1_2", "h1_3", "h1_4", "i1_1", "i1_2", "i1_3", "i1_4", "j1_1", "j1_2", "j1_3", "j1_4", "k1_1", "k1_2", "k1_3", "l1_1", "l1_2", "l1_3", "l1_4", "m1_1", "m1_3", "m1_4", "n1_1", "n1_3", "n1_4", "o1_1", "o1_2", "o1_4", "p1_1", "p1_2", "p1_3", "p1_4", "q1_3", "q1_4", "r1_1", "r1_3", "r1_4", "s1_1", "s1_2", "s1_3", "s1_4" } };
            TileEdge n1_1 = new TileEdge { edgeType = "n1_1", compatibleEdgeTypes = new List<string> { "a1_3", "a1_4", "c1_3", "c1_2", "b1_2", "b1_3", "b1_4", "e1_3", "e1_4", "f1_3", "f1_2", "g1_1", "g1_2", "g1_3", "g1_4", "h1_1", "h1_2", "h1_3", "h1_4", "i1_1", "i1_2", "i1_3", "i1_4", "j1_1", "j1_2", "j1_3", "j1_4", "k1_1", "k1_2", "k1_3", "l1_1", "l1_2", "l1_3", "l1_4", "m1_1", "m1_3", "m1_4", "n1_1", "n1_3", "n1_4", "o1_1", "o1_2", "o1_4", "p1_1", "p1_2", "p1_3", "p1_4", "q1_3", "q1_4", "r1_1", "r1_3", "r1_4", "s1_1", "s1_2", "s1_3", "s1_4" } };
            TileEdge n1_2 = new TileEdge { edgeType = "n1_2", compatibleEdgeTypes = new List<string> { "l1_1", "l1_2", "l1_3", "l1_4", "r1_3", "r1_1", "r1_2", "r1_4", "f1_2", "f1_3", "b1_3", "c1_3", "d1_3", "e1_4", "o1_1", "q1_3", "q1_4" } };
            TileEdge n1_3 = new TileEdge { edgeType = "n1_3", compatibleEdgeTypes = new List<string> { "n1_2", "o1_3", "o1_2", "r1_2", "q1_1", "q1_2" } };
            TileEdge n1_4 = new TileEdge { edgeType = "n1_4", compatibleEdgeTypes = new List<string> { "a1_3", "a1_4", "c1_3", "c1_2", "b1_2", "b1_3", "b1_4", "e1_3", "e1_4", "f1_3", "f1_2", "g1_1", "g1_2", "g1_3", "g1_4", "h1_1", "h1_2", "h1_3", "h1_4", "i1_1", "i1_2", "i1_3", "i1_4", "j1_1", "j1_2", "j1_3", "j1_4", "k1_1", "k1_2", "k1_3", "l1_1", "l1_2", "l1_3", "l1_4", "m1_1", "m1_3", "m1_4", "n1_1", "n1_3", "n1_4", "o1_1", "o1_2", "o1_4", "p1_1", "p1_2", "p1_3", "p1_4", "q1_3", "q1_4", "r1_1", "r1_3", "r1_4", "s1_1", "s1_2", "s1_3", "s1_4" } };
            TileEdge o1_1 = new TileEdge { edgeType = "o1_1", compatibleEdgeTypes = new List<string> { "a1_3", "a1_4", "c1_3", "c1_2", "b1_2", "b1_3", "b1_4", "e1_3", "e1_4", "f1_3", "f1_2", "g1_1", "g1_2", "g1_3", "g1_4", "h1_1", "h1_2", "h1_3", "h1_4", "i1_1", "i1_2", "i1_3", "i1_4", "j1_1", "j1_2", "j1_3", "j1_4", "k1_1", "k1_2", "k1_3", "l1_1", "l1_2", "l1_3", "l1_4", "m1_1", "m1_3", "m1_4", "n1_1", "n1_3", "n1_4", "o1_1", "o1_2", "o1_4", "p1_1", "p1_2", "p1_3", "p1_4", "q1_3", "q1_4", "r1_1", "r1_3", "r1_4", "s1_1", "s1_2", "s1_3", "s1_4" } };
            TileEdge o1_2 = new TileEdge { edgeType = "o1_2", compatibleEdgeTypes = new List<string> { "l1_1", "l1_2", "l1_3", "l1_4", "r1_3", "r1_1", "r1_2", "r1_4", "f1_2", "f1_3", "b1_3", "c1_3", "d1_3", "e1_4", "o1_1", "q1_3", "q1_4" } };
            TileEdge o1_3 = new TileEdge { edgeType = "o1_3", compatibleEdgeTypes = new List<string> { "l1_1", "l1_2", "l1_3", "l1_4", "r1_3", "r1_1", "r1_2", "r1_4", "f1_2", "f1_3", "b1_3", "c1_3", "d1_3", "e1_4", "o1_1", "q1_3", "q1_4" } };
            TileEdge o1_4 = new TileEdge { edgeType = "o1_4", compatibleEdgeTypes = new List<string> { "a1_3", "a1_4", "c1_3", "c1_2", "b1_2", "b1_3", "b1_4", "e1_3", "e1_4", "f1_3", "f1_2", "g1_1", "g1_2", "g1_3", "g1_4", "h1_1", "h1_2", "h1_3", "h1_4", "i1_1", "i1_2", "i1_3", "i1_4", "j1_1", "j1_2", "j1_3", "j1_4", "k1_1", "k1_2", "k1_3", "l1_1", "l1_2", "l1_3", "l1_4", "m1_1", "m1_3", "m1_4", "n1_1", "n1_3", "n1_4", "o1_1", "o1_2", "o1_4", "p1_1", "p1_2", "p1_3", "p1_4", "q1_3", "q1_4", "r1_1", "r1_3", "r1_4", "s1_1", "s1_2", "s1_3", "s1_4" } };
            TileEdge p1_1 = new TileEdge { edgeType = "p1_1", compatibleEdgeTypes = new List<string> { "a1_1", "a1_3", "a1_4", "b1_2", "b1_3", "b1_4", "e1_3", "e1_4", "k1_1", "k1_2", "k1_3", "l1_1", "l1_2", "l1_3", "l1_4", "m1_1", "m1_3", "m1_4", "n1_1", "n1_3", "n1_4", "o1_1", "o1_4", "q1_3", "q1_4", "r1_1", "r1_3", "r1_4", "s1_1", "s1_2", "s1_3", "s1_4" } };
            TileEdge p1_2 = new TileEdge { edgeType = "p1_2", compatibleEdgeTypes = new List<string> { "a1_1", "a1_3", "a1_4", "b1_2", "b1_3", "b1_4", "e1_3", "e1_4", "k1_1", "k1_2", "k1_3", "l1_1", "l1_2", "l1_3", "l1_4", "m1_1", "m1_3", "m1_4", "n1_1", "n1_3", "n1_4", "o1_1", "o1_4", "q1_3", "q1_4", "r1_1", "r1_3", "r1_4", "s1_1", "s1_2", "s1_3", "s1_4" } };
            TileEdge p1_3 = new TileEdge { edgeType = "p1_3", compatibleEdgeTypes = new List<string> { "a1_1", "a1_3", "a1_4", "b1_2", "b1_3", "b1_4", "e1_3", "e1_4", "k1_1", "k1_2", "k1_3", "l1_1", "l1_2", "l1_3", "l1_4", "m1_1", "m1_3", "m1_4", "n1_1", "n1_3", "n1_4", "o1_1", "o1_4", "q1_3", "q1_4", "r1_1", "r1_3", "r1_4", "s1_1", "s1_2", "s1_3", "s1_4" } };
            TileEdge p1_4 = new TileEdge { edgeType = "p1_4", compatibleEdgeTypes = new List<string> { "a1_1", "a1_3", "a1_4", "b1_2", "b1_3", "b1_4", "e1_3", "e1_4", "k1_1", "k1_2", "k1_3", "l1_1", "l1_2", "l1_3", "l1_4", "m1_1", "m1_3", "m1_4", "n1_1", "n1_3", "n1_4", "o1_1", "o1_4", "q1_3", "q1_4", "r1_1", "r1_3", "r1_4", "s1_1", "s1_2", "s1_3", "s1_4" } };
            TileEdge q1_1 = new TileEdge { edgeType = "q1_1", compatibleEdgeTypes = new List<string> { "l1_1", "l1_2", "l1_3", "l1_4", "m1_3", "n1_3" } };
            TileEdge q1_2 = new TileEdge { edgeType = "q1_2", compatibleEdgeTypes = new List<string> { "l1_1", "l1_2", "l1_3", "l1_4", "m1_3", "n1_3" } };
            TileEdge q1_3 = new TileEdge { edgeType = "q1_3", compatibleEdgeTypes = new List<string> { "a1_3", "a1_4", "c1_3", "c1_2", "b1_2", "b1_3", "b1_4", "e1_3", "e1_4", "f1_3", "f1_2", "g1_1", "g1_2", "g1_3", "g1_4", "h1_1", "h1_2", "h1_3", "h1_4", "i1_1", "i1_2", "i1_3", "i1_4", "j1_1", "j1_2", "j1_3", "j1_4", "k1_1", "k1_2", "k1_3", "l1_1", "l1_2", "l1_3", "l1_4", "m1_1", "m1_3", "m1_4", "n1_1", "n1_3", "n1_4", "o1_1", "o1_2", "o1_4", "p1_1", "p1_2", "p1_3", "p1_4", "q1_3", "q1_4", "r1_1", "r1_3", "r1_4", "s1_1", "s1_2", "s1_3", "s1_4" } };
            TileEdge q1_4 = new TileEdge { edgeType = "q1_4", compatibleEdgeTypes = new List<string> { "a1_3", "a1_4", "c1_3", "c1_2", "b1_2", "b1_3", "b1_4", "e1_3", "e1_4", "f1_3", "f1_2", "g1_1", "g1_2", "g1_3", "g1_4", "h1_1", "h1_2", "h1_3", "h1_4", "i1_1", "i1_2", "i1_3", "i1_4", "j1_1", "j1_2", "j1_3", "j1_4", "k1_1", "k1_2", "k1_3", "l1_1", "l1_2", "l1_3", "l1_4", "m1_1", "m1_3", "m1_4", "n1_1", "n1_3", "n1_4", "o1_1", "o1_2", "o1_4", "p1_1", "p1_2", "p1_3", "p1_4", "q1_3", "q1_4", "r1_1", "r1_3", "r1_4", "s1_1", "s1_2", "s1_3", "s1_4" } };
            TileEdge r1_1 = new TileEdge { edgeType = "r1_1", compatibleEdgeTypes = new List<string> { "a1_3", "a1_4", "c1_3", "c1_2", "b1_2", "b1_3", "b1_4", "e1_3", "e1_4", "f1_3", "f1_2", "g1_1", "g1_2", "g1_3", "g1_4", "h1_1", "h1_2", "h1_3", "h1_4", "i1_1", "i1_2", "i1_3", "i1_4", "j1_1", "j1_2", "j1_3", "j1_4", "k1_1", "k1_2", "k1_3", "l1_1", "l1_2", "l1_3", "l1_4", "m1_1", "m1_3", "m1_4", "n1_1", "n1_3", "n1_4", "o1_1", "o1_2", "o1_4", "p1_1", "p1_2", "p1_3", "p1_4", "q1_3", "q1_4", "r1_1", "r1_3", "r1_4", "s1_1", "s1_2", "s1_3", "s1_4" } };
            TileEdge r1_2 = new TileEdge { edgeType = "r1_2", compatibleEdgeTypes = new List<string> { "l1_1", "l1_2", "l1_3", "l1_4", "r1_3", "r1_1", "r1_2", "r1_4", "f1_2", "f1_3", "b1_3", "c1_3", "d1_3", "e1_4", "o1_1", "q1_3", "q1_4" } };
            TileEdge r1_3 = new TileEdge { edgeType = "r1_3", compatibleEdgeTypes = new List<string> { "a1_3", "a1_4", "c1_3", "c1_2", "b1_2", "b1_3", "b1_4", "e1_3", "e1_4", "f1_3", "f1_2", "g1_1", "g1_2", "g1_3", "g1_4", "h1_1", "h1_2", "h1_3", "h1_4", "i1_1", "i1_2", "i1_3", "i1_4", "j1_1", "j1_2", "j1_3", "j1_4", "k1_1", "k1_2", "k1_3", "l1_1", "l1_2", "l1_3", "l1_4", "m1_1", "m1_3", "m1_4", "n1_1", "n1_3", "n1_4", "o1_1", "o1_2", "o1_4", "p1_1", "p1_2", "p1_3", "p1_4", "q1_3", "q1_4", "r1_1", "r1_3", "r1_4", "s1_1", "s1_2", "s1_3", "s1_4" } };
            TileEdge r1_4 = new TileEdge { edgeType = "r1_4", compatibleEdgeTypes = new List<string> { "a1_3", "a1_4", "c1_3", "c1_2", "b1_2", "b1_3", "b1_4", "e1_3", "e1_4", "f1_3", "f1_2", "g1_1", "g1_2", "g1_3", "g1_4", "h1_1", "h1_2", "h1_3", "h1_4", "i1_1", "i1_2", "i1_3", "i1_4", "j1_1", "j1_2", "j1_3", "j1_4", "k1_1", "k1_2", "k1_3", "l1_1", "l1_2", "l1_3", "l1_4", "m1_1", "m1_3", "m1_4", "n1_1", "n1_3", "n1_4", "o1_1", "o1_2", "o1_4", "p1_1", "p1_2", "p1_3", "p1_4", "q1_3", "q1_4", "r1_1", "r1_3", "r1_4", "s1_1", "s1_2", "s1_3", "s1_4" } };
            TileEdge s1_1 = new TileEdge { edgeType = "s1_1", compatibleEdgeTypes = new List<string> { "a1_3", "a1_4", "c1_3", "c1_2", "b1_2", "b1_3", "b1_4", "e1_3", "e1_4", "f1_3", "f1_2", "g1_1", "g1_2", "g1_3", "g1_4", "h1_1", "h1_2", "h1_3", "h1_4", "i1_1", "i1_2", "i1_3", "i1_4", "j1_1", "j1_2", "j1_3", "j1_4", "k1_1", "k1_2", "k1_3", "l1_1", "l1_2", "l1_3", "l1_4", "m1_1", "m1_3", "m1_4", "n1_1", "n1_3", "n1_4", "o1_1", "o1_2", "o1_4", "p1_1", "p1_2", "p1_3", "p1_4", "q1_3", "q1_4", "r1_1", "r1_3", "r1_4", "s1_1", "s1_2", "s1_3", "s1_4" } };
            TileEdge s1_2 = new TileEdge { edgeType = "s1_2", compatibleEdgeTypes = new List<string> { "a1_3", "a1_4", "c1_3", "c1_2", "b1_2", "b1_3", "b1_4", "e1_3", "e1_4", "f1_3", "f1_2", "g1_1", "g1_2", "g1_3", "g1_4", "h1_1", "h1_2", "h1_3", "h1_4", "i1_1", "i1_2", "i1_3", "i1_4", "j1_1", "j1_2", "j1_3", "j1_4", "k1_1", "k1_2", "k1_3", "l1_1", "l1_2", "l1_3", "l1_4", "m1_1", "m1_3", "m1_4", "n1_1", "n1_3", "n1_4", "o1_1", "o1_2", "o1_4", "p1_1", "p1_2", "p1_3", "p1_4", "q1_3", "q1_4", "r1_1", "r1_3", "r1_4", "s1_1", "s1_2", "s1_3", "s1_4" } };
            TileEdge s1_3 = new TileEdge { edgeType = "s1_3", compatibleEdgeTypes = new List<string> { "a1_3", "a1_4", "c1_3", "c1_2", "b1_2", "b1_3", "b1_4", "e1_3", "e1_4", "f1_3", "f1_2", "g1_1", "g1_2", "g1_3", "g1_4", "h1_1", "h1_2", "h1_3", "h1_4", "i1_1", "i1_2", "i1_3", "i1_4", "j1_1", "j1_2", "j1_3", "j1_4", "k1_1", "k1_2", "k1_3", "l1_1", "l1_2", "l1_3", "l1_4", "m1_1", "m1_3", "m1_4", "n1_1", "n1_3", "n1_4", "o1_1", "o1_2", "o1_4", "p1_1", "p1_2", "p1_3", "p1_4", "q1_3", "q1_4", "r1_1", "r1_3", "r1_4", "s1_1", "s1_2", "s1_3", "s1_4" } };
            TileEdge s1_4 = new TileEdge { edgeType = "s1_4", compatibleEdgeTypes = new List<string> { "a1_3", "a1_4", "c1_3", "c1_2", "b1_2", "b1_3", "b1_4", "e1_3", "e1_4", "f1_3", "f1_2", "g1_1", "g1_2", "g1_3", "g1_4", "h1_1", "h1_2", "h1_3", "h1_4", "i1_1", "i1_2", "i1_3", "i1_4", "j1_1", "j1_2", "j1_3", "j1_4", "k1_1", "k1_2", "k1_3", "l1_1", "l1_2", "l1_3", "l1_4", "m1_1", "m1_3", "m1_4", "n1_1", "n1_3", "n1_4", "o1_1", "o1_2", "o1_4", "p1_1", "p1_2", "p1_3", "p1_4", "q1_3", "q1_4", "r1_1", "r1_3", "r1_4", "s1_1", "s1_2", "s1_3", "s1_4" } };


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

            TileEdge a3_1 = new TileEdge { edgeType = "a1_1", compatibleEdgeTypes = new List<string> { "Mountain", "River" } };
            TileEdge a3_2 = new TileEdge { edgeType = "", compatibleEdgeTypes = new List<string> { "Mountain", "River" } };
            TileEdge a3_3 = new TileEdge { edgeType = "Mountain", compatibleEdgeTypes = new List<string> { "Mountain", "River" } };
            TileEdge a3_4 = new TileEdge { edgeType = "Mountain", compatibleEdgeTypes = new List<string> { "Mountain", "River" } };
            TileEdge b3_1 = new TileEdge { edgeType = "River", compatibleEdgeTypes = new List<string> { "Mountain", "River", "Plain" } };
            TileEdge b3_2 = new TileEdge { edgeType = "River", compatibleEdgeTypes = new List<string> { "Mountain", "River", "Plain" } };
            TileEdge b3_3 = new TileEdge { edgeType = "River", compatibleEdgeTypes = new List<string> { "Mountain", "River", "Plain" } };
            TileEdge b3_4 = new TileEdge { edgeType = "River", compatibleEdgeTypes = new List<string> { "Mountain", "River", "Plain" } };
            TileEdge c3_1 = new TileEdge { edgeType = "", compatibleEdgeTypes = new List<string> { "Plain", "River" } };
            TileEdge c3_2 = new TileEdge { edgeType = "Plain", compatibleEdgeTypes = new List<string> { "Plain", "River" } };
            TileEdge c3_3 = new TileEdge { edgeType = "Plain", compatibleEdgeTypes = new List<string> { "Plain", "River" } };
            TileEdge c3_4 = new TileEdge { edgeType = "Plain", compatibleEdgeTypes = new List<string> { "Plain", "River" } };
            TileEdge d3_1 = new TileEdge { edgeType = "Plain", compatibleEdgeTypes = new List<string> { "Plain", "River" } };
            TileEdge d3_2 = new TileEdge { edgeType = "Plain", compatibleEdgeTypes = new List<string> { "Plain", "River" } };
            TileEdge d3_3 = new TileEdge { edgeType = "Plain", compatibleEdgeTypes = new List<string> { "Plain", "River" } };
            TileEdge d3_4 = new TileEdge { edgeType = "Plain", compatibleEdgeTypes = new List<string> { "Plain", "River" } };
            TileEdge e3_1 = new TileEdge { edgeType = "Plain", compatibleEdgeTypes = new List<string> { "Plain", "River" } };
            TileEdge e3_2 = new TileEdge { edgeType = "Plain", compatibleEdgeTypes = new List<string> { "Plain", "River" } };
            TileEdge e3_3 = new TileEdge { edgeType = "Plain", compatibleEdgeTypes = new List<string> { "Plain", "River" } };
            TileEdge e3_4 = new TileEdge { edgeType = "Plain", compatibleEdgeTypes = new List<string> { "Plain", "River" } };
            TileEdge f3_1 = new TileEdge { edgeType = "Plain", compatibleEdgeTypes = new List<string> { "Plain", "River" } };
            TileEdge f3_2 = new TileEdge { edgeType = "Plain", compatibleEdgeTypes = new List<string> { "Plain", "River" } };
            TileEdge f3_3 = new TileEdge { edgeType = "Plain", compatibleEdgeTypes = new List<string> { "Plain", "River" } };
            TileEdge f3_4 = new TileEdge { edgeType = "Plain", compatibleEdgeTypes = new List<string> { "Plain", "River" } };
            TileEdge g3_1 = new TileEdge { edgeType = "Plain", compatibleEdgeTypes = new List<string> { "Plain", "River" } };
            TileEdge g3_2 = new TileEdge { edgeType = "Plain", compatibleEdgeTypes = new List<string> { "Plain", "River" } };
            TileEdge g3_3 = new TileEdge { edgeType = "Plain", compatibleEdgeTypes = new List<string> { "Plain", "River" } };
            TileEdge g3_4 = new TileEdge { edgeType = "Plain", compatibleEdgeTypes = new List<string> { "Plain", "River" } };
            TileEdge h3_1 = new TileEdge { edgeType = "Plain", compatibleEdgeTypes = new List<string> { "Plain", "River" } };
            TileEdge h3_2 = new TileEdge { edgeType = "Plain", compatibleEdgeTypes = new List<string> { "Plain", "River" } };
            TileEdge h3_3 = new TileEdge { edgeType = "Plain", compatibleEdgeTypes = new List<string> { "Plain", "River" } };
            TileEdge h3_4 = new TileEdge { edgeType = "Plain", compatibleEdgeTypes = new List<string> { "Plain", "River" } };
            TileEdge i3_1 = new TileEdge { edgeType = "Plain", compatibleEdgeTypes = new List<string> { "Plain", "River" } };
            TileEdge i3_2 = new TileEdge { edgeType = "Plain", compatibleEdgeTypes = new List<string> { "Plain", "River" } };
            TileEdge i3_3 = new TileEdge { edgeType = "Plain", compatibleEdgeTypes = new List<string> { "Plain", "River" } };
            TileEdge i3_4 = new TileEdge { edgeType = "Plain", compatibleEdgeTypes = new List<string> { "Plain", "River" } };
            TileEdge j3_1 = new TileEdge { edgeType = "Plain", compatibleEdgeTypes = new List<string> { "Plain", "River" } };
            TileEdge j3_2 = new TileEdge { edgeType = "Plain", compatibleEdgeTypes = new List<string> { "Plain", "River" } };
            TileEdge j3_3 = new TileEdge { edgeType = "Plain", compatibleEdgeTypes = new List<string> { "Plain", "River" } };
            TileEdge j3_4 = new TileEdge { edgeType = "Plain", compatibleEdgeTypes = new List<string> { "Plain", "River" } };
            TileEdge k3_1 = new TileEdge { edgeType = "Plain", compatibleEdgeTypes = new List<string> { "Plain", "River" } };
            TileEdge k3_2 = new TileEdge { edgeType = "Plain", compatibleEdgeTypes = new List<string> { "Plain", "River" } };
            TileEdge k3_3 = new TileEdge { edgeType = "Plain", compatibleEdgeTypes = new List<string> { "Plain", "River" } };
            TileEdge k3_4 = new TileEdge { edgeType = "Plain", compatibleEdgeTypes = new List<string> { "Plain", "River" } };
            TileEdge l3_1 = new TileEdge { edgeType = "Plain", compatibleEdgeTypes = new List<string> { "Plain", "River" } };
            TileEdge l3_2 = new TileEdge { edgeType = "Plain", compatibleEdgeTypes = new List<string> { "Plain", "River" } };
            TileEdge l3_3 = new TileEdge { edgeType = "Plain", compatibleEdgeTypes = new List<string> { "Plain", "River" } };
            TileEdge l3_4 = new TileEdge { edgeType = "Plain", compatibleEdgeTypes = new List<string> { "Plain", "River" } };
            TileEdge m3_1 = new TileEdge { edgeType = "Plain", compatibleEdgeTypes = new List<string> { "Plain", "River" } };
            TileEdge m3_2 = new TileEdge { edgeType = "Plain", compatibleEdgeTypes = new List<string> { "Plain", "River" } };
            TileEdge m3_3 = new TileEdge { edgeType = "Plain", compatibleEdgeTypes = new List<string> { "Plain", "River" } };
            TileEdge m3_4 = new TileEdge { edgeType = "Plain", compatibleEdgeTypes = new List<string> { "Plain", "River" } };
            TileEdge n3_1 = new TileEdge { edgeType = "Plain", compatibleEdgeTypes = new List<string> { "Plain", "River" } };
            TileEdge n3_2 = new TileEdge { edgeType = "Plain", compatibleEdgeTypes = new List<string> { "Plain", "River" } };
            TileEdge n3_3 = new TileEdge { edgeType = "Plain", compatibleEdgeTypes = new List<string> { "Plain", "River" } };
            TileEdge n3_4 = new TileEdge { edgeType = "Plain", compatibleEdgeTypes = new List<string> { "Plain", "River" } };
            TileEdge o3_1 = new TileEdge { edgeType = "Plain", compatibleEdgeTypes = new List<string> { "Plain", "River" } };
            TileEdge o3_2 = new TileEdge { edgeType = "Plain", compatibleEdgeTypes = new List<string> { "Plain", "River" } };
            TileEdge o3_3 = new TileEdge { edgeType = "Plain", compatibleEdgeTypes = new List<string> { "Plain", "River" } };
            TileEdge o3_4 = new TileEdge { edgeType = "Plain", compatibleEdgeTypes = new List<string> { "Plain", "River" } };
            TileEdge p3_1 = new TileEdge { edgeType = "Plain", compatibleEdgeTypes = new List<string> { "Plain", "River" } };
            TileEdge p3_2 = new TileEdge { edgeType = "Plain", compatibleEdgeTypes = new List<string> { "Plain", "River" } };
            TileEdge p3_3 = new TileEdge { edgeType = "Plain", compatibleEdgeTypes = new List<string> { "Plain", "River" } };
            TileEdge p3_4 = new TileEdge { edgeType = "Plain", compatibleEdgeTypes = new List<string> { "Plain", "River" } };
            TileEdge q3_1 = new TileEdge { edgeType = "Plain", compatibleEdgeTypes = new List<string> { "Plain", "River" } };
            TileEdge q3_2 = new TileEdge { edgeType = "Plain", compatibleEdgeTypes = new List<string> { "Plain", "River" } };
            TileEdge q3_3 = new TileEdge { edgeType = "Plain", compatibleEdgeTypes = new List<string> { "Plain", "River" } };
            TileEdge q3_4 = new TileEdge { edgeType = "Plain", compatibleEdgeTypes = new List<string> { "Plain", "River" } };
            TileEdge r3_1 = new TileEdge { edgeType = "Plain", compatibleEdgeTypes = new List<string> { "Plain", "River" } };
            TileEdge r3_2 = new TileEdge { edgeType = "Plain", compatibleEdgeTypes = new List<string> { "Plain", "River" } };
            TileEdge r3_3 = new TileEdge { edgeType = "Plain", compatibleEdgeTypes = new List<string> { "Plain", "River" } };
            TileEdge r3_4 = new TileEdge { edgeType = "Plain", compatibleEdgeTypes = new List<string> { "Plain", "River" } };
            TileEdge s3_1 = new TileEdge { edgeType = "Plain", compatibleEdgeTypes = new List<string> { "Plain", "River" } };
            TileEdge s3_2 = new TileEdge { edgeType = "Plain", compatibleEdgeTypes = new List<string> { "Plain", "River" } };
            TileEdge s3_3 = new TileEdge { edgeType = "Plain", compatibleEdgeTypes = new List<string> { "Plain", "River" } };
            TileEdge s3_4 = new TileEdge { edgeType = "Plain", compatibleEdgeTypes = new List<string> { "Plain", "River" } };

            // 지형별 여러 타일 정의
            List<MapTile> desertTiles = new List<MapTile>
            {
                new MapTile("desertTile1", desertPrefab1, "Desert", mountainEdge, plainEdge, riverEdge, mountainEdge),
                new MapTile("desertTile2", desertPrefab2, "Desert", riverEdge, mountainEdge, plainEdge, riverEdge),
                new MapTile("desertTile3", desertPrefab2, "Desert", riverEdge, mountainEdge, plainEdge, riverEdge),
                new MapTile("desertTile4", desertPrefab2, "Desert", riverEdge, mountainEdge, plainEdge, riverEdge),
                new MapTile("desertTile5", desertPrefab2, "Desert", riverEdge, mountainEdge, plainEdge, riverEdge),
                new MapTile("desertTile6", desertPrefab2, "Desert", riverEdge, mountainEdge, plainEdge, riverEdge),
                new MapTile("desertTile7", desertPrefab2, "Desert", riverEdge, mountainEdge, plainEdge, riverEdge),
                new MapTile("desertTile8", desertPrefab2, "Desert", riverEdge, mountainEdge, plainEdge, riverEdge),
                new MapTile("desertTile9", desertPrefab2, "Desert", riverEdge, mountainEdge, plainEdge, riverEdge),
                new MapTile("desertTile10", desertPrefab2, "Desert", riverEdge, mountainEdge, plainEdge, riverEdge),
                new MapTile("desertTile11", desertPrefab2, "Desert", riverEdge, mountainEdge, plainEdge, riverEdge),
                new MapTile("desertTile12", desertPrefab2, "Desert", riverEdge, mountainEdge, plainEdge, riverEdge),
                new MapTile("desertTile13", desertPrefab2, "Desert", riverEdge, mountainEdge, plainEdge, riverEdge),
                new MapTile("desertTile14", desertPrefab2, "Desert", riverEdge, mountainEdge, plainEdge, riverEdge),
                new MapTile("desertTile15", desertPrefab2, "Desert", riverEdge, mountainEdge, plainEdge, riverEdge)
            };

            List<MapTile> forestTiles = new List<MapTile>
            {
                new MapTile("forestTile1", forestPrefab1, "Forest", a1_1, a1_2, a1_4, a1_3),
                new MapTile("forestTile2", forestPrefab2, "Forest", b1_1, b1_2, b1_4, b1_3),
                new MapTile("forestTile3", forestPrefab3, "Forest", c1_1, c1_2, c1_4, c1_3),
                new MapTile("forestTile4", forestPrefab4, "Forest", d1_1, d1_2, d1_4, d1_3),
                new MapTile("forestTile5", forestPrefab5, "Forest", e1_1, e1_2, e1_4, e1_3),
                new MapTile("forestTile6", forestPrefab6, "Forest", f1_1, f1_2, f1_4, f1_3),
                new MapTile("forestTile7", forestPrefab7, "Forest", g1_1, g1_2, g1_4, g1_3),
                new MapTile("forestTile8", forestPrefab8, "Forest", h1_1, h1_2, h1_4, h1_3),
                new MapTile("forestTile9", forestPrefab9, "Forest", i1_1, i1_2, i1_4, i1_3),
                new MapTile("forestTile10", forestPrefab10, "Forest", j1_1, j1_2, j1_4, j1_3),
                new MapTile("forestTile11", forestPrefab11, "Forest", k1_1, k1_2, k1_4, k1_3),
                new MapTile("forestTile12", forestPrefab12, "Forest", l1_1, l1_2, l1_4, l1_3),
                new MapTile("forestTile13", forestPrefab13, "Forest", m1_1, m1_2, m1_4, m1_3),
                new MapTile("forestTile14", forestPrefab14, "Forest", n1_1, n1_2, n1_4, n1_3),
                new MapTile("forestTile15", forestPrefab15, "Forest", o1_1, o1_2, o1_4, o1_3),
                new MapTile("forestTile16", forestPrefab16, "Forest", p1_1, p1_2, p1_4, p1_3),
                new MapTile("forestTile17", forestPrefab17, "Forest", q1_1, q1_2, q1_4, q1_3),
                new MapTile("forestTile18", forestPrefab18, "Forest", r1_1, r1_2, r1_4, r1_3),
                new MapTile("forestTile19", forestPrefab19, "Forest", s1_1, s1_2, s1_4, s1_3)
                
            };

            List<MapTile> glacierTiles = new List<MapTile>
            {
                //new MapTile("glacierTile1", glacierPrefab1, "Glacier", plainEdge, riverEdge, mountainEdge, riverEdge),
                //new MapTile("glacierTile2", glacierPrefab2, "Glacier", mountainEdge, plainEdge, riverEdge, mountainEdge)
            };

            List<MapTile> volcanoTiles = new List<MapTile>
            {
                new MapTile("volcanoTile1", volcanoPrefab1, "Volcano", a2_1, a2_2, a2_4, a2_3),
                new MapTile("volcanoTile2", volcanoPrefab1, "Volcano", a2_1, a2_2, a2_4, a2_3),
                new MapTile("volcanoTile3", volcanoPrefab1, "Volcano", a2_1, a2_2, a2_4, a2_3),
                new MapTile("volcanoTile4", volcanoPrefab1, "Volcano", a2_1, a2_2, a2_4, a2_3),
                new MapTile("volcanoTile5", volcanoPrefab1, "Volcano", a2_1, a2_2, a2_4, a2_3),
                new MapTile("volcanoTile6", volcanoPrefab1, "Volcano", a2_1, a2_2, a2_4, a2_3),
                new MapTile("volcanoTile7", volcanoPrefab1, "Volcano", a2_1, a2_2, a2_4, a2_3),
                new MapTile("volcanoTile8", volcanoPrefab1, "Volcano", a2_1, a2_2, a2_4, a2_3),
                new MapTile("volcanoTile9", volcanoPrefab1, "Volcano", a2_1, a2_2, a2_4, a2_3),
                new MapTile("volcanoTile10", volcanoPrefab1, "Volcano", a2_1, a2_2, a2_4, a2_3)

            };

            // 지형별로 타일 목록을 딕셔너리에 추가
            terrainTileSets["Desert"] = desertTiles;
            terrainTileSets["Forest"] = forestTiles;
            terrainTileSets["Glacier"] = glacierTiles;
            terrainTileSets["Volcano"] = volcanoTiles;
        }

        void UpdateCells()
        {
            Vector2Int playerCellPos = GetPlayerCellPosition();
            HashSet<Vector2Int> newActiveCells = new HashSet<Vector2Int>();

            for (int x = -2; x <= 2; x++)
            {
                for (int y = -2; y <= 2; y++)
                {
                    Vector2Int cellKey = new Vector2Int(playerCellPos.x + x, playerCellPos.y + y);
                    newActiveCells.Add(cellKey);

                    if (!cells.ContainsKey(cellKey))
                    {
                        Cell newCell = new Cell
                        {
                            position = cellKey,
                            possibleTiles = new List<MapTile>(terrainTileSets["Forest"]) // 기본값 예시로 추가
                        };
                        cells.Add(cellKey, newCell);

                        float height = Mathf.PerlinNoise(cellKey.x / perlinScale, cellKey.y / perlinScale) * maxHeight;
                        float terrainValue = Mathf.PerlinNoise(cellKey.x / (perlinScale * 2), cellKey.y / (perlinScale * 2));
                        newCell.terrainType = GetTerrainType(terrainValue);
                        Vector3 cellWorldPos = new Vector3(newCell.position.x * tileSize, height, newCell.position.y * tileSize);

                        CollapseCell(newCell, cellWorldPos);
                    }
                }
            }

            activeCells = newActiveCells;
        }

        string GetTerrainType(float value)
        {
            if (value < 0.2f)
                return "Desert";
            else if (value < 0.4f)
                return "Lava";
            else if (value < 0.6f)
                return "Forest";
            else if (value < 0.8f)
                return "Glacier";
            else
                return "Mountain";
        }

        void CollapseCell(Cell cell, Vector3 cellWorldPos)
        {
            if (cell.isCollapsed) return;

            // 지형 타입 결정
            string terrainType = GetTerrainType(Mathf.PerlinNoise(cell.position.x / perlinScale, cell.position.y / perlinScale));

            // 해당 지형 타입의 타일 목록에서 호환되는 타일 선택
            List<MapTile> compatibleTiles = new List<MapTile>(terrainTileSets[terrainType]);

            foreach (var direction in Direction.AllDirections)
            {
                Vector2Int neighborPos = cell.position + direction;
                if (cells.ContainsKey(neighborPos))
                {
                    Cell neighborCell = cells[neighborPos];
                    string dir = Direction.VectorToDirection(direction);
                    compatibleTiles = compatibleTiles.FindAll(tile => IsCompatible(tile, neighborCell.instantiatedTile.GetComponent<MapTile>(), dir));
                }
            }

            if (compatibleTiles.Count == 0)
            {
                Debug.LogError("호환 가능한 타일이 없습니다.");
                return;
            }

            MapTile selectedTile = compatibleTiles[Random.Range(0, compatibleTiles.Count)];
            cell.isCollapsed = true;
            cell.instantiatedTile = Instantiate(selectedTile.prefab, cellWorldPos, Quaternion.identity, tileParent);
        }

        Vector2Int GetPlayerCellPosition()
        {
            int playerCellX = Mathf.RoundToInt(player.position.x / tileSize);
            int playerCellY = Mathf.RoundToInt(player.position.z / tileSize);
            return new Vector2Int(playerCellX, playerCellY);
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
    }
}
