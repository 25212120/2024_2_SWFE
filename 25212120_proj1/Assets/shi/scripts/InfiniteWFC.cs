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
            GameObject desertPrefab1 = Resources.Load<GameObject>("DesertTilePrefab1");
            GameObject desertPrefab2 = Resources.Load<GameObject>("DesertTilePrefab2");
            GameObject forestPrefab1 = Resources.Load<GameObject>("ForestTilePrefab1");
            GameObject forestPrefab2 = Resources.Load<GameObject>("ForestTilePrefab2");
            GameObject glacierPrefab1 = Resources.Load<GameObject>("GlacierTilePrefab1");
            GameObject glacierPrefab2 = Resources.Load<GameObject>("GlacierTilePrefab2");
            GameObject volcanoPrefab1 = Resources.Load<GameObject>("VolcanoTilePrefab1");
            GameObject volcanoPrefab2 = Resources.Load<GameObject>("VolcanoTilePrefab2");

            // 각 타일의 면 정의
            TileEdge mountainEdge = new TileEdge { edgeType = "Mountain", compatibleEdgeTypes = new List<string> { "Mountain", "River" } };
            TileEdge riverEdge = new TileEdge { edgeType = "River", compatibleEdgeTypes = new List<string> { "Mountain", "River", "Plain" } };
            TileEdge plainEdge = new TileEdge { edgeType = "Plain", compatibleEdgeTypes = new List<string> { "Plain", "River" } };

            // 지형별 여러 타일 정의
            List<MapTile> desertTiles = new List<MapTile>
            {
                new MapTile("DesertTile1", desertPrefab1, "Desert", mountainEdge, plainEdge, riverEdge, mountainEdge),
                new MapTile("DesertTile2", desertPrefab2, "Desert", riverEdge, mountainEdge, plainEdge, riverEdge)
            };

            List<MapTile> forestTiles = new List<MapTile>
            {
                new MapTile("ForestTile1", forestPrefab1, "Forest", riverEdge, mountainEdge, plainEdge, riverEdge),
                new MapTile("ForestTile2", forestPrefab2, "Forest", plainEdge, riverEdge, mountainEdge, riverEdge)
            };

            List<MapTile> glacierTiles = new List<MapTile>
            {
                new MapTile("GlacierTile1", glacierPrefab1, "Glacier", plainEdge, riverEdge, mountainEdge, riverEdge),
                new MapTile("GlacierTile2", glacierPrefab2, "Glacier", mountainEdge, plainEdge, riverEdge, mountainEdge)
            };

            List<MapTile> volcanoTiles = new List<MapTile>
            {
                new MapTile("VolcanoTile1", volcanoPrefab1, "Volcano", mountainEdge, riverEdge, plainEdge, mountainEdge),
                new MapTile("VolcanoTile2", volcanoPrefab2, "Volcano", riverEdge, plainEdge, mountainEdge, riverEdge)
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
                            possibleTiles = new List<MapTile>(terrainTileSets["Desert"]) // 기본값 예시로 추가
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
