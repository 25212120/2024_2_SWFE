using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Tile
{
    public string name; // Ÿ�� �̸� �Ǵ� ID
    public GameObject prefab; // Ÿ�� ������
    public Dictionary<string, List<string>> compatibleTiles; // ���⺰ ȣȯ�Ǵ� Ÿ�� ���
}

public class Cell
{
    public Vector2Int position; // �׸��� �� ��ġ
    public List<Tile> possibleTiles; // ������ Ÿ�� ���
    public bool isCollapsed = false; // Ÿ���� �����Ǿ����� ����
    public GameObject instantiatedTile; // ������ Ÿ�� �ν��Ͻ�
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
    public List<Tile> tileSet; // ����� Ÿ�� ���

    public GameObject tile1Prefab;
    public GameObject tile2Prefab;
    public GameObject tile3Prefab;
    public GameObject tile4Prefab;

    public Transform tileParent; // Ÿ���� �θ� �� Transform

    private Dictionary<Vector2Int, Cell> cells = new Dictionary<Vector2Int, Cell>();
    private HashSet<Vector2Int> activeCells = new HashSet<Vector2Int>(); // ���� Ȱ��ȭ�� �� ��ǥ ���

    private Vector2Int previousPlayerCellPos = new Vector2Int(int.MinValue, int.MinValue);

    void Start()
    {
        InitializeTiles();
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
        // Tile1 ����
        Tile tile1 = new Tile();
        tile1.name = "Tile1";
        tile1.prefab = tile1Prefab;
        tile1.compatibleTiles = new Dictionary<string, List<string>>()
        {
            { "North", new List<string>() { "Any" } },
            { "East", new List<string>() { "Any" } },
            { "South", new List<string>() { "Tile2" } }, // "Any" �߰�
            { "West", new List<string>() { "Tile3" } }   // "Any" �߰�
        };

        // Tile2 ����
        Tile tile2 = new Tile();
        tile2.name = "Tile2";
        tile2.prefab = tile2Prefab;
        tile2.compatibleTiles = new Dictionary<string, List<string>>()
        {
            { "North", new List<string>() {"Tile1" } }, // "Any" �߰�
            { "East", new List<string>() { "Any" } },
            { "South", new List<string>() { "Any" } },
            { "West", new List<string>() { "Any" } }
        };

        // Tile3 ����
        Tile tile3 = new Tile();
        tile3.name = "Tile3";
        tile3.prefab = tile3Prefab;
        tile3.compatibleTiles = new Dictionary<string, List<string>>()
        {
            { "North", new List<string>() { "Any" } },
            { "East", new List<string>() { "Any" } },  // "Any" �߰�
            { "South", new List<string>() { "Any" } },
            { "West", new List<string>() { "Any" } }
        };

        // Tile4 ����
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

        // TileSet�� �߰�
        tileSet = new List<Tile> { tile1, tile2, tile3, tile4 };

        // Ÿ�ϼ� �ʱ�ȭ ����� �α�
        Debug.Log($"Ÿ�ϼ� �ʱ�ȭ �Ϸ�: {tileSet.Count}���� Ÿ���� �ֽ��ϴ�.");
    }

    Vector2Int GetPlayerCellPosition()
    {
        int playerCellX = Mathf.FloorToInt(player.position.x / tileSize);
        int playerCellY = Mathf.FloorToInt(player.position.z / tileSize); // Unity���� Z���� ���� ����
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
                    Cell newCell = new Cell();
                    newCell.position = cellKey;
                    newCell.possibleTiles = new List<Tile>(tileSet);
                    cells.Add(cellKey, newCell);

                    // ���� �߽� ���� Ư�� Ÿ���� ������ ����
                    if (x == 0 && y == 0)
                    {
                        newCell.isCollapsed = true;
                        newCell.possibleTiles = new List<Tile> { tileSet[0] }; // ù ��° Ÿ�Ϸ� ����
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
                        // WFC �˰������� ���� �ر�
                        CollapseCell(newCell);
                    }
                }
            }
        }

        // Ȱ��ȭ�� �� ��� ������Ʈ
        activeCells = newActiveCells;
    }

    void CollapseCell(Cell cell)
    {
        if (cell.isCollapsed) return;

        if (cell.possibleTiles.Count == 0)
        {
            Debug.LogError($"���� ������ Ÿ���� �����ϴ�. ��ġ: {cell.position}");
            return;
        }

        // ������ Ÿ�� �� �ϳ��� ����
        Tile selectedTile = SelectTile(cell);

        if (selectedTile == null)
        {
            Debug.LogError("Ÿ���� ������ �� �����ϴ�. ��Ģ�� Ȯ���ϼ���.");
            return;
        }

        cell.isCollapsed = true;
        cell.possibleTiles = new List<Tile> { selectedTile };

        // ���� ���� ��ġ ���
        Vector3 cellWorldPos = new Vector3(cell.position.x * tileSize, 0, cell.position.y * tileSize);

        // Ÿ�� �ν��Ͻ� ���� (�θ� tileParent�� ����)
        cell.instantiatedTile = Instantiate(
            selectedTile.prefab,
            cellWorldPos,
            Quaternion.identity,
            tileParent // �θ� tileParent�� ����
        );

        // �ֺ� ���� ���� ����
        PropagateConstraints(cell);
    }

    Tile SelectTile(Cell cell)
    {
        // ������ Ÿ�� ��Ͽ��� �������� ����
        int index = Random.Range(0, cell.possibleTiles.Count);
        return cell.possibleTiles[index];
    }

    void PropagateConstraints(Cell cell)
    {
        foreach (Vector2Int dir in Direction.AllDirections)
        {
            Vector2Int neighborPos = cell.position + dir;

            // neighborPos�� Ȱ��ȭ�� �� ���� ���� �ִ��� Ȯ��
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
                // ������ ���� ������ ����
                neighborCell = new Cell();
                neighborCell.position = neighborPos;
                neighborCell.possibleTiles = new List<Tile>(tileSet);
                cells.Add(neighborPos, neighborCell);
            }

            // ���� ���� ���� ���� ������ Ÿ�� ����� ������Ʈ
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
                Debug.LogError($"ȣȯ�Ǵ� Ÿ���� �����ϴ�. ��ġ: {neighborCell.position}, ����: {Direction.VectorToDirection(dir)}");
            }

            // ������ Ÿ���� �ϳ��� �����Ǹ� �ر�
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

        // ���� Ÿ���� �ش� ���⿡�� ȣȯ�Ǵ� Ÿ�� ���
        List<string> currentCompatible = currentTile.compatibleTiles.ContainsKey(dir) ? currentTile.compatibleTiles[dir] : new List<string>() { "Any" };

        // ���� Ÿ���� �ݴ� ���⿡�� ȣȯ�Ǵ� Ÿ�� ���
        List<string> neighborCompatible = neighborTile.compatibleTiles.ContainsKey(oppositeDir) ? neighborTile.compatibleTiles[oppositeDir] : new List<string>() { "Any" };

        // "Any"�� ���ԵǾ� ������ ��� Ÿ�ϰ� ȣȯ
        bool currentMatches = currentCompatible.Contains("Any") || currentCompatible.Contains(neighborTile.name);
        bool neighborMatches = neighborCompatible.Contains("Any") || neighborCompatible.Contains(currentTile.name);

        return currentMatches && neighborMatches;
    }
}
