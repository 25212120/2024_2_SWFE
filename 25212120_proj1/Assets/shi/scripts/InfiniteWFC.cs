using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomNamespace
{
    [System.Serializable]
    public class DirectionCompatibility
    {
        public string direction; // ���� (North, East, South, West)
        public List<string> compatibleTileNames; // �ش� ���⿡�� ȣȯ�Ǵ� Ÿ�� �̸� ���
    }

    [System.Serializable]
    public class MapTile
    {
        public string name; // Ÿ�� �̸� �Ǵ� ID
        public GameObject prefab; // Ÿ�� ������
        public List<DirectionCompatibility> compatibleTiles; // ���⺰ ȣȯ�Ǵ� Ÿ�� ���

        // ������ �߰� - Ÿ�� �̸�, ������ �� ȣȯ�� ����
        public MapTile(string name, GameObject prefab, List<DirectionCompatibility> compatibleTiles)
        {
            this.name = name;
            this.prefab = prefab;
            this.compatibleTiles = compatibleTiles;
        }
    }

    public class Cell
    {
        public Vector2Int position; // �׸��� �� ��ġ
        public List<MapTile> possibleTiles; // ������ Ÿ�� ���
        public bool isCollapsed = false; // Ÿ���� �����Ǿ����� ����
        public GameObject instantiatedTile; // ������ Ÿ�� �ν��Ͻ�
        public string terrainType; // ���� Ÿ��
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
        public List<MapTile> tileSet; // ����� Ÿ�� ���
        public float perlinScale = 10.0f; // Perlin Noise ������
        public float maxHeight = 5.0f; // �ִ� ����

        public Transform tileParent; // Ÿ���� �θ� �� Transform

        private Dictionary<Vector2Int, Cell> cells = new Dictionary<Vector2Int, Cell>();
        private HashSet<Vector2Int> activeCells = new HashSet<Vector2Int>(); // ���� Ȱ��ȭ�� �� ��ǥ ���

        private Vector2Int previousPlayerCellPos = new Vector2Int(int.MinValue, int.MinValue);

        void Start()
        {
            InitializeTiles(); // Ÿ�� �ʱ�ȭ
            UpdateCells(); // �ʱ� Ÿ�� ����
        }

        void Update()
        {
            Vector2Int currentPlayerCellPos = GetPlayerCellPosition();

            // �÷��̾ ���ο� ���� �̵����� ���� Ÿ�� ������Ʈ
            if (currentPlayerCellPos != previousPlayerCellPos)
            {
                UpdateCells();
                previousPlayerCellPos = currentPlayerCellPos;
            }
        }

        void InitializeTiles()
        {
            // �� Ÿ���� ���⺰ ȣȯ���� �����ϰ� Ÿ�� ����
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
            // �÷��̾� ��ġ�� �� �׸��� ��ǥ�� ��ȯ
            int playerCellX = Mathf.RoundToInt(player.position.x / tileSize);
            int playerCellY = Mathf.RoundToInt(player.position.z / tileSize); // Unity���� Z���� ���� ����
            return new Vector2Int(playerCellX, playerCellY);
        }

        void UpdateCells()
        {
            Vector2Int playerCellPos = GetPlayerCellPosition();

            // ���� Ȱ��ȭ�� �� ��� ����
            HashSet<Vector2Int> newActiveCells = new HashSet<Vector2Int>();

            // �÷��̾� �ֺ��� ���� Ȯ���ϰ� �ʿ��� ��� ����
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

                        // Perlin Noise�� ����Ͽ� ���̿� ���� Ÿ���� ���
                        float height = Mathf.PerlinNoise(cellKey.x / perlinScale, cellKey.y / perlinScale) * maxHeight;
                        float terrainValue = Mathf.PerlinNoise(cellKey.x / (perlinScale * 2), cellKey.y / (perlinScale * 2));
                        newCell.terrainType = GetTerrainType(terrainValue);
                        Vector3 cellWorldPos = new Vector3(newCell.position.x * tileSize, height, newCell.position.y * tileSize);

                        // ���� �߽� ���� Ư�� Ÿ���� ������ ����
                        if (x == 0 && y == 0)
                        {
                            newCell.isCollapsed = true;
                            newCell.possibleTiles = new List<MapTile> { tileSet[0] }; // ù ��° Ÿ�Ϸ� ����
                            newCell.instantiatedTile = Instantiate(
                                tileSet[0].prefab,
                                cellWorldPos,
                                Quaternion.identity,
                                tileParent
                            );
                        }
                        else
                        {
                            // WFC �˰������� ���� �ر�
                            CollapseCell(newCell, cellWorldPos);
                        }
                    }
                }
            }

            // Ȱ��ȭ�� �� ��� ������Ʈ
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
                Debug.LogError($"���� ������ Ÿ���� �����ϴ�. ��ġ: {cell.position}");
                return;
            }

            // ������ Ÿ�� �� �ϳ��� ����
            MapTile selectedTile = SelectTile(cell);

            if (selectedTile == null)
            {
                Debug.LogError("Ÿ���� ������ �� �����ϴ�. ��Ģ�� Ȯ���ϼ���.");
                return;
            }

            // Ÿ���� �ν��Ͻ�ȭ�ϰ� �ش� ��ġ�� ��ġ
            cell.isCollapsed = true;
            cell.instantiatedTile = Instantiate(selectedTile.prefab, cellWorldPos, Quaternion.identity, tileParent);

            
        }

        // Select a tile from possible ones
        MapTile SelectTile(Cell cell)
        {
            // Ÿ�� ���� �� �߰� (����ġ ��� �Ǵ� ������)
            return cell.possibleTiles[Random.Range(0, cell.possibleTiles.Count)];
        }
    }
}
