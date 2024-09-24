using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Tile
{
    public string name; // 타일 이름 또는 ID
    public GameObject prefab; // 타일 프리팹
    public Dictionary<string, List<string>> compatibleTiles; // 방향별 호환되는 타일 목록
}

public class Cell
{
    public Vector2Int position; // 그리드 내 위치
    public List<Tile> possibleTiles; // 가능한 타일 목록
    public bool isCollapsed = false; // 타일이 결정되었는지 여부
    public GameObject instantiatedTile; // 생성된 타일 인스턴스
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
    public List<Tile> tileSet; // 사용할 타일 목록

    public GameObject tile1Prefab;
    public GameObject tile2Prefab;
    public GameObject tile3Prefab;
    public GameObject tile4Prefab;

    public Transform tileParent; // 타일의 부모가 될 Transform

    private Dictionary<Vector2Int, Cell> cells = new Dictionary<Vector2Int, Cell>();
    private HashSet<Vector2Int> activeCells = new HashSet<Vector2Int>(); // 현재 활성화된 셀 좌표 목록

    private Vector2Int previousPlayerCellPos = new Vector2Int(int.MinValue, int.MinValue);

    void Start()
    {
        InitializeTiles();
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
        // Tile1 설정
        Tile tile1 = new Tile();
        tile1.name = "Tile1";
        tile1.prefab = tile1Prefab;
        tile1.compatibleTiles = new Dictionary<string, List<string>>()
        {
            { "North", new List<string>() { "Any" } },
            { "East", new List<string>() { "Any" } },
            { "South", new List<string>() { "Tile2" } }, // "Any" 추가
            { "West", new List<string>() { "Tile3" } }   // "Any" 추가
        };

        // Tile2 설정
        Tile tile2 = new Tile();
        tile2.name = "Tile2";
        tile2.prefab = tile2Prefab;
        tile2.compatibleTiles = new Dictionary<string, List<string>>()
        {
            { "North", new List<string>() {"Tile1" } }, // "Any" 추가
            { "East", new List<string>() { "Any" } },
            { "South", new List<string>() { "Any" } },
            { "West", new List<string>() { "Any" } }
        };

        // Tile3 설정
        Tile tile3 = new Tile();
        tile3.name = "Tile3";
        tile3.prefab = tile3Prefab;
        tile3.compatibleTiles = new Dictionary<string, List<string>>()
        {
            { "North", new List<string>() { "Any" } },
            { "East", new List<string>() { "Any" } },  // "Any" 추가
            { "South", new List<string>() { "Any" } },
            { "West", new List<string>() { "Any" } }
        };

        // Tile4 설정
        Tile tile4 = new Tile();
        tile4.name = "Tile4";
        tile4.prefab = tile4Prefab;
        tile4.compatibleTiles = new Dictionary<string, List<string>>()
        {
            { "North", new List<string>() { "Any" } },
            { "East", new List<string>() { "Any" } },
            { "South", new List<string>() { "Any" } },
            { "West", new List<string>() { "Any" } }
        };

        // TileSet에 추가
        tileSet = new List<Tile> { tile1, tile2, tile3, tile4 };

        // 타일셋 초기화 디버그 로그
        Debug.Log($"타일셋 초기화 완료: {tileSet.Count}개의 타일이 있습니다.");
    }

    Vector2Int GetPlayerCellPosition()
    {
        int playerCellX = Mathf.FloorToInt(player.position.x / tileSize);
        int playerCellY = Mathf.FloorToInt(player.position.z / tileSize); // Unity에서 Z축이 전진 방향
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
                    Cell newCell = new Cell();
                    newCell.position = cellKey;
                    newCell.possibleTiles = new List<Tile>(tileSet);
                    cells.Add(cellKey, newCell);

                    // 맵의 중심 셀에 특정 타일을 강제로 설정
                    if (x == 0 && y == 0)
                    {
                        newCell.isCollapsed = true;
                        newCell.possibleTiles = new List<Tile> { tileSet[0] }; // 첫 번째 타일로 설정
                        Vector3 cellWorldPos = new Vector3(newCell.position.x * tileSize, 0, newCell.position.y * tileSize);
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
                        CollapseCell(newCell);
                    }
                }
            }
        }

        // 활성화된 셀 목록 업데이트
        activeCells = newActiveCells;
    }

    void CollapseCell(Cell cell)
    {
        if (cell.isCollapsed) return;

        if (cell.possibleTiles.Count == 0)
        {
            Debug.LogError($"셀의 가능한 타일이 없습니다. 위치: {cell.position}");
            return;
        }

        // 가능한 타일 중 하나를 선택
        Tile selectedTile = SelectTile(cell);

        if (selectedTile == null)
        {
            Debug.LogError("타일을 선택할 수 없습니다. 규칙을 확인하세요.");
            return;
        }

        cell.isCollapsed = true;
        cell.possibleTiles = new List<Tile> { selectedTile };

        // 셀의 월드 위치 계산
        Vector3 cellWorldPos = new Vector3(cell.position.x * tileSize, 0, cell.position.y * tileSize);

        // 타일 인스턴스 생성 (부모를 tileParent로 설정)
        cell.instantiatedTile = Instantiate(
            selectedTile.prefab,
            cellWorldPos,
            Quaternion.identity,
            tileParent // 부모를 tileParent로 설정
        );

        // 주변 셀에 제약 전파
        PropagateConstraints(cell);
    }

    Tile SelectTile(Cell cell)
    {
        // 가능한 타일 목록에서 무작위로 선택
        int index = Random.Range(0, cell.possibleTiles.Count);
        return cell.possibleTiles[index];
    }

    void PropagateConstraints(Cell cell)
    {
        foreach (Vector2Int dir in Direction.AllDirections)
        {
            Vector2Int neighborPos = cell.position + dir;

            // neighborPos가 활성화된 셀 범위 내에 있는지 확인
            if (!activeCells.Contains(neighborPos))
                continue;

            Cell neighborCell;

            if (cells.TryGetValue(neighborPos, out neighborCell))
            {
                if (neighborCell.isCollapsed)
                    continue;
            }
            else
            {
                // 인접한 셀이 없으면 생성
                neighborCell = new Cell();
                neighborCell.position = neighborPos;
                neighborCell.possibleTiles = new List<Tile>(tileSet);
                cells.Add(neighborPos, neighborCell);
            }

            // 현재 셀과 인접 셀의 가능한 타일 목록을 업데이트
            List<Tile> compatibleTiles = new List<Tile>();

            foreach (Tile neighborTile in neighborCell.possibleTiles)
            {
                if (IsCompatible(cell.possibleTiles[0], neighborTile, dir))
                {
                    compatibleTiles.Add(neighborTile);
                }
            }

            if (compatibleTiles.Count > 0)
            {
                neighborCell.possibleTiles = compatibleTiles;
            }
            else
            {
                Debug.LogError($"호환되는 타일이 없습니다. 위치: {neighborCell.position}, 방향: {Direction.VectorToDirection(dir)}");
            }

            // 가능한 타일이 하나로 결정되면 붕괴
            if (neighborCell.possibleTiles.Count == 1)
            {
                CollapseCell(neighborCell);
            }
        }
    }

    bool IsCompatible(Tile currentTile, Tile neighborTile, Vector2Int direction)
    {
        string dir = Direction.VectorToDirection(direction);
        string oppositeDir = Direction.GetOpposite(dir);

        // 현재 타일의 해당 방향에서 호환되는 타일 목록
        List<string> currentCompatible = currentTile.compatibleTiles.ContainsKey(dir) ? currentTile.compatibleTiles[dir] : new List<string>() { "Any" };

        // 인접 타일의 반대 방향에서 호환되는 타일 목록
        List<string> neighborCompatible = neighborTile.compatibleTiles.ContainsKey(oppositeDir) ? neighborTile.compatibleTiles[oppositeDir] : new List<string>() { "Any" };

        // "Any"가 포함되어 있으면 모든 타일과 호환
        bool currentMatches = currentCompatible.Contains("Any") || currentCompatible.Contains(neighborTile.name);
        bool neighborMatches = neighborCompatible.Contains("Any") || neighborCompatible.Contains(currentTile.name);

        return currentMatches && neighborMatches;
    }
}
