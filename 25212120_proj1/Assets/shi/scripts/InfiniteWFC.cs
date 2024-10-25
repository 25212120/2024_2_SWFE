using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomNamespace
{
    [System.Serializable]
    public class DirectionCompatibility
    {
        public string direction; // 방향 (North, East, South, West)
        public List<string> compatibleTileNames; // 해당 방향에서 호환되는 타일 이름 목록
    }

    [System.Serializable]
    public class MapTile
    {
        public string name; // 타일 이름 또는 ID
        public GameObject prefab; // 타일 프리팹
        public List<DirectionCompatibility> compatibleTiles; // 방향별 호환되는 타일 목록

        // 생성자 추가 - 타일 이름, 프리팹 및 호환성 설정
        public MapTile(string name, GameObject prefab, List<DirectionCompatibility> compatibleTiles)
        {
            this.name = name;
            this.prefab = prefab;
            this.compatibleTiles = compatibleTiles;
        }
    }

    public class Cell
    {
        public Vector2Int position; // 그리드 내 위치
        public List<MapTile> possibleTiles; // 가능한 타일 목록
        public bool isCollapsed = false; // 타일이 결정되었는지 여부
        public GameObject instantiatedTile; // 생성된 타일 인스턴스
        public string terrainType; // 지형 타입
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
        public List<MapTile> tileSet; // 사용할 타일 목록
        public float perlinScale = 10.0f; // Perlin Noise 스케일
        public float maxHeight = 5.0f; // 최대 높이

        public Transform tileParent; // 타일의 부모가 될 Transform

        private Dictionary<Vector2Int, Cell> cells = new Dictionary<Vector2Int, Cell>();
        private HashSet<Vector2Int> activeCells = new HashSet<Vector2Int>(); // 현재 활성화된 셀 좌표 목록

        private Vector2Int previousPlayerCellPos = new Vector2Int(int.MinValue, int.MinValue);

        void Start()
        {
            InitializeTiles(); // 타일 초기화
            UpdateCells(); // 초기 타일 생성
        }

        void Update()
        {
            Vector2Int currentPlayerCellPos = GetPlayerCellPosition();

            // 플레이어가 새로운 셀로 이동했을 때만 타일 업데이트
            if (currentPlayerCellPos != previousPlayerCellPos)
            {
                UpdateCells();
                previousPlayerCellPos = currentPlayerCellPos;
            }
        }

        void InitializeTiles()
        {
            // 각 타일의 방향별 호환성을 설정하고 타일 생성
            GameObject tile1Prefab = Resources.Load<GameObject>("Tile1Prefab");
            GameObject tile2Prefab = Resources.Load<GameObject>("Tile2Prefab");

            if (tile1Prefab == null || tile2Prefab == null)
            {
                Debug.LogError("Tile prefabs could not be loaded. Check your Resources folder.");
                return;
            }

            List<DirectionCompatibility> tile1Compatibility = new List<DirectionCompatibility>
            {
                new DirectionCompatibility { direction = "North", compatibleTileNames = new List<string> { "Tile2", "Tile3" } },
                new DirectionCompatibility { direction = "East", compatibleTileNames = new List<string> { "Tile2", "Tile4" } },
                new DirectionCompatibility { direction = "South", compatibleTileNames = new List<string> { "Tile1", "Tile3" } },
                new DirectionCompatibility { direction = "West", compatibleTileNames = new List<string> { "Tile2", "Tile4" } }
            };

            List<DirectionCompatibility> tile2Compatibility = new List<DirectionCompatibility>
            {
                new DirectionCompatibility { direction = "North", compatibleTileNames = new List<string> { "Tile1", "Tile3" } },
                new DirectionCompatibility { direction = "East", compatibleTileNames = new List<string> { "Tile1", "Tile4" } },
                new DirectionCompatibility { direction = "South", compatibleTileNames = new List<string> { "Tile1", "Tile2" } },
                new DirectionCompatibility { direction = "West", compatibleTileNames = new List<string> { "Tile3", "Tile4" } }
            };

            MapTile tile1 = new MapTile("Tile1", tile1Prefab, tile1Compatibility);
            MapTile tile2 = new MapTile("Tile2", tile2Prefab, tile2Compatibility);

            tileSet = new List<MapTile> { tile1, tile2 };
        }

        Vector2Int GetPlayerCellPosition()
        {
            // 플레이어 위치를 셀 그리드 좌표로 변환
            int playerCellX = Mathf.RoundToInt(player.position.x / tileSize);
            int playerCellY = Mathf.RoundToInt(player.position.z / tileSize); // Unity에서 Z축이 전진 방향
            return new Vector2Int(playerCellX, playerCellY);
        }

        void UpdateCells()
        {
            Vector2Int playerCellPos = GetPlayerCellPosition();

            // 현재 활성화된 셀 목록 복사
            HashSet<Vector2Int> newActiveCells = new HashSet<Vector2Int>();

            // 플레이어 주변의 셀을 확인하고 필요한 경우 생성
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
                            possibleTiles = new List<MapTile>(tileSet)
                        };
                        cells.Add(cellKey, newCell);

                        // Perlin Noise를 사용하여 높이와 지형 타입을 계산
                        float height = Mathf.PerlinNoise(cellKey.x / perlinScale, cellKey.y / perlinScale) * maxHeight;
                        float terrainValue = Mathf.PerlinNoise(cellKey.x / (perlinScale * 2), cellKey.y / (perlinScale * 2));
                        newCell.terrainType = GetTerrainType(terrainValue);
                        Vector3 cellWorldPos = new Vector3(newCell.position.x * tileSize, height, newCell.position.y * tileSize);

                        // 맵의 중심 셀에 특정 타일을 강제로 설정
                        if (x == 0 && y == 0)
                        {
                            newCell.isCollapsed = true;
                            newCell.possibleTiles = new List<MapTile> { tileSet[0] }; // 첫 번째 타일로 설정
                            newCell.instantiatedTile = Instantiate(
                                tileSet[0].prefab,
                                cellWorldPos,
                                Quaternion.identity,
                                tileParent
                            );
                        }
                        else
                        {
                            // WFC 알고리즘으로 셀을 붕괴
                            CollapseCell(newCell, cellWorldPos);
                        }
                    }
                }
            }

            // 활성화된 셀 목록 업데이트
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

            if (cell.possibleTiles.Count == 0)
            {
                Debug.LogError($"셀의 가능한 타일이 없습니다. 위치: {cell.position}");
                return;
            }

            // 가능한 타일 중 하나를 선택
            MapTile selectedTile = SelectTile(cell);

            if (selectedTile == null)
            {
                Debug.LogError("타일을 선택할 수 없습니다. 규칙을 확인하세요.");
                return;
            }

            // 타일을 인스턴스화하고 해당 위치에 배치
            cell.isCollapsed = true;
            cell.instantiatedTile = Instantiate(selectedTile.prefab, cellWorldPos, Quaternion.identity, tileParent);

            
        }

        // Select a tile from possible ones
        MapTile SelectTile(Cell cell)
        {
            // 타일 선택 논리 추가 (가중치 기반 또는 무작위)
            return cell.possibleTiles[Random.Range(0, cell.possibleTiles.Count)];
        }
    }
}
