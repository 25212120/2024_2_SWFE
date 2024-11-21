using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class WaveFunctionCollapse
{
    private BiomeManager biomeManager;
    private float cellSize; // �� ũ��
    private Stack<WaveState> backtrackStack = new Stack<WaveState>();

    public WaveFunctionCollapse(BiomeManager biomeManager, float cellSize)
    {
        this.biomeManager = biomeManager;
        this.cellSize = cellSize;
    }

    public void GenerateChunk(Chunk chunk, Vector2Int chunkCoord, List<Tile> allTiles, bool isPlayerSpawnChunk)
    {
        // ûũ�� ���� ��ġ ���
        float chunkWorldSize = chunk.width * cellSize;
        Vector2 chunkWorldPosition = new Vector2(
            chunkCoord.x * chunkWorldSize,
            chunkCoord.y * chunkWorldSize
        );

        // ûũ�� �߾� ��ġ ��� (�ɼ�)
        chunkWorldPosition += new Vector2(chunkWorldSize / 2, chunkWorldSize / 2);

        // ����� �α� �߰�
        Debug.Log($"ûũ {chunkCoord}�� chunkWorldSize: {chunkWorldSize}, chunkWorldPosition: {chunkWorldPosition}");

        // ���̿� ����
        string biome = biomeManager.GetBiomeForPosition(chunkWorldPosition);
        biome = biome.Trim().ToLowerInvariant();

        // ������ �� �α� ���
        float noiseValue = biomeManager.GetNoiseValue(chunkWorldPosition);
        Debug.Log($"ûũ {chunkCoord}�� ������ ��: {noiseValue:F4}, ���̿�: {biome}");



        // ���̿ȿ� �´� Ÿ�� ���͸�
        List<Tile> filteredTiles = FilterTilesByBiome(biome, allTiles);
        Debug.Log($"���͸��� Ÿ�� ��: {filteredTiles.Count}");

        if (filteredTiles.Count == 0)
        {
            Debug.LogWarning($"���̿� '{biome}'�� �´� Ÿ���� �����ϴ�. ��� Ÿ���� ����մϴ�.");
            filteredTiles = allTiles;
        }

        if (isPlayerSpawnChunk)
        {
            // ���� ûũ�� �⺻ Ÿ�� ����
            InitializeChunkWithDefaultTiles(chunk, filteredTiles);
        }
        else
        {
            // �Ϲ� ûũ ����
            InitializeChunk(chunk, filteredTiles);

            bool success = RunWFC(chunk);
            if (!success)
            {
                Debug.LogError("WFC�� ����Ͽ� ûũ�� �����ϴ� �� �����߽��ϴ�.");
            }
        }
    }


    // Ÿ���� �ν��Ͻ�ȭ�ϴ� �޼���
    public void InstantiateChunk(Chunk chunk)
    {
        for (int x = 0; x < chunk.width; x++)
        {
            for (int y = 0; y < chunk.height; y++)
            {
                Cell cell = chunk.cells[x, y];
                if (cell.collapsedTileState != null)
                {
                    TileState state = cell.collapsedTileState;
                    if (state.tile.prefab == null)
                    {
                        Debug.LogError($"Ÿ�� �������� null�Դϴ�: {state.tile.tileName} ��ġ ({x}, {y})");
                        continue;
                    }

                    // Ÿ�� ��ġ ��� (ûũ ��ġ + �� ��ġ)
                    Vector3 position = chunk.chunkObject.transform.position + new Vector3(x * cellSize, 0, y * cellSize);

                    GameObject obj = GameObject.Instantiate(
                        state.tile.prefab,
                        position,
                        Quaternion.Euler(0, state.tile.rotationAngles[state.rotationIndex], 0)
                    );

                    obj.name = $"{state.tile.tileName}_{x}_{y}";
                    obj.transform.parent = chunk.chunkObject.transform;

                    // Ÿ�� ���� �α� �߰�
                    Debug.Log($"Ÿ�� ������: {obj.name} ��ġ {position}");
                }
                else
                {
                    Debug.LogWarning($"�� ({x}, {y})�� �Ҵ�� Ÿ���� �����ϴ�.");
                }
            }
        }
    }
    // ���� ûũ�� �⺻ Ÿ�� ���� �޼��� �߰�
    private void InitializeChunkWithDefaultTiles(Chunk chunk, List<Tile> tiles)
    {
        // ûũ�� �� �迭 �ʱ�ȭ
        chunk.cells = new Cell[chunk.width, chunk.height];

        // �⺻ Ÿ�� ���� (��: ù ��° Ÿ��)
        Tile defaultTile = tiles[0];

        for (int x = 0; x < chunk.width; x++)
        {
            for (int y = 0; y < chunk.height; y++)
            {
                Cell cell = new Cell();
                cell.position = new Vector2Int(x, y);

                // �⺻ Ÿ�Ϸ� �� �ر�
                cell.collapsedTileState = new TileState
                {
                    tile = defaultTile,
                    rotationIndex = 0 // ȸ�� ����
                };

                // ������ Ÿ�� ���´� �⺻ Ÿ�� �ϳ��� ����
                cell.possibleTileStates = new List<TileState> { cell.collapsedTileState };

                chunk.cells[x, y] = cell;
            }
        }
    }




    private List<Tile> FilterTilesByBiome(string biome, List<Tile> allTiles)
    {
        List<Tile> matchingTiles = allTiles.FindAll(tile => tile.biomes.Any(b => b.Equals(biome, System.StringComparison.OrdinalIgnoreCase)));
        Debug.Log($"���̿� '{biome}'�� �´� Ÿ�� ��: {matchingTiles.Count}");
        return matchingTiles;
    }


    public void InitializeChunk(Chunk chunk, List<Tile> tiles)
    {
        // ûũ�� �� �迭 �ʱ�ȭ
        chunk.cells = new Cell[chunk.width, chunk.height];

        for (int x = 0; x < chunk.width; x++)
        {
            for (int y = 0; y < chunk.height; y++)
            {
                Cell cell = new Cell();
                cell.position = new Vector2Int(x, y);

                bool isEdge = (x == 0 || y == 0 || x == chunk.width - 1 || y == chunk.height - 1);

                if (isEdge)
                {
                    // ���� ��: `isEdgeTile == true`�� Ÿ�ϸ� ���
                    foreach (var tile in tiles)
                    {
                        if (tile.isEdgeTile)
                        {
                            for (int i = 0; i < tile.rotatedSockets.Count; i++)
                            {
                                cell.possibleTileStates.Add(new TileState
                                {
                                    tile = tile,
                                    rotationIndex = i
                                });
                            }
                        }
                    }
                    Debug.Log($"���� �� ({x}, {y}) ������ Ÿ�� ���� ��: {cell.possibleTileStates.Count}");
                }
                else
                {
                    // ���� ��: ��� Ÿ�� ���
                    foreach (var tile in tiles)
                    {
                        for (int i = 0; i < tile.rotatedSockets.Count; i++)
                        {
                            cell.possibleTileStates.Add(new TileState
                            {
                                tile = tile,
                                rotationIndex = i
                            });
                        }
                    }
                    Debug.Log($"���� �� ({x}, {y}) ������ Ÿ�� ���� ��: {cell.possibleTileStates.Count}");
                }

                chunk.cells[x, y] = cell;
            }
        }
    }

    private bool RunWFC(Chunk chunk)
    {
        int iteration = 0;
        while (true)
        {
            iteration++;
            Debug.Log($"WFC Iteration: {iteration}");

            Cell cell = SelectCellWithLowestEntropy(chunk);
            if (cell == null)
            {
                // ��� ���� �ر���
                Debug.Log("��� ���� �ر��Ǿ����ϴ�.");
                return true;
            }

            // ���� ���� ����
            SaveWaveState(chunk);

            bool success = CollapseCell(cell);
            if (!success)
            {
                Debug.LogWarning($"�� ({cell.position.x}, {cell.position.y}) �ر� ����. ��Ʈ��ŷ�� �õ��մϴ�.");
                // ��Ʈ��ŷ
                bool backtrackSuccess = Backtrack(chunk);
                if (!backtrackSuccess)
                {
                    Debug.LogError("��Ʈ��ŷ ����. WFC �˰����� �����մϴ�.");
                    return false;
                }
            }
            else
            {
                Debug.Log($"�� ({cell.position.x}, {cell.position.y}) �ر� ����: {cell.collapsedTileState.tile.tileName}");

                bool propagateSuccess = PropagateConstraints(chunk, cell);
                if (!propagateSuccess)
                {
                    Debug.LogWarning("���� ���� ����. ��Ʈ��ŷ�� �õ��մϴ�.");
                    // ��Ʈ��ŷ
                    bool backtrackSuccess = Backtrack(chunk);
                    if (!backtrackSuccess)
                    {
                        Debug.LogError("��Ʈ��ŷ ����. WFC �˰����� �����մϴ�.");
                        return false;
                    }
                }
            }
        }
    }


    private Cell SelectCellWithLowestEntropy(Chunk chunk)
    {
        List<Cell> minEntropyCells = new List<Cell>();
        float minEntropy = float.MaxValue;

        foreach (var cell in chunk.cells)
        {
            if (!cell.IsCollapsed)
            {
                float entropy = cell.Entropy;
                if (entropy < minEntropy)
                {
                    minEntropy = entropy;
                    minEntropyCells.Clear();
                    minEntropyCells.Add(cell);
                }
                else if (Mathf.Approximately(entropy, minEntropy))
                {
                    minEntropyCells.Add(cell);
                }
            }
        }

        if (minEntropyCells.Count == 0)
            return null;

        // �������� �� ����
        int randomIndex = Random.Range(0, minEntropyCells.Count);
        return minEntropyCells[randomIndex];
    }

    private bool CollapseCell(Cell cell)
    {
        // ������ ���°� ���� ��� ����
        if (cell.possibleTileStates.Count == 0)
            return false;

        // ����ġ�� ���� Ÿ�� ���� ����
        float totalWeight = cell.possibleTileStates.Sum(state => state.tile.weight);
        float randomValue = Random.Range(0, totalWeight);
        float cumulativeWeight = 0f;

        foreach (var state in cell.possibleTileStates)
        {
            cumulativeWeight += state.tile.weight;
            if (randomValue <= cumulativeWeight)
            {
                cell.collapsedTileState = state;
                cell.possibleTileStates = new List<TileState> { state };
                return true;
            }
        }

        // ���� ���� �� ������ ���� ����
        cell.collapsedTileState = cell.possibleTileStates[0];
        cell.possibleTileStates = new List<TileState> { cell.collapsedTileState };
        return true;
    }

    private bool PropagateConstraints(Chunk chunk, Cell startCell)
    {
        Queue<Cell> propagationQueue = new Queue<Cell>();
        propagationQueue.Enqueue(startCell);

        while (propagationQueue.Count > 0)
        {
            Cell cell = propagationQueue.Dequeue();

            foreach (Vector2Int dir in Directions)
            {
                Vector2Int neighborPos = cell.position + dir;
                if (IsValidPosition(chunk, neighborPos))
                {
                    Cell neighborCell = chunk.cells[neighborPos.x, neighborPos.y];
                    if (neighborCell.IsCollapsed) continue;

                    bool changed = UpdateNeighbor(cell, neighborCell, dir);
                    if (neighborCell.possibleTileStates.Count == 0)
                    {
                        return false; // ���� ������ �����ϴ� Ÿ���� ����
                    }

                    if (changed)
                    {
                        propagationQueue.Enqueue(neighborCell);
                    }
                }
            }
        }

        return true;
    }

    private bool UpdateNeighbor(Cell cell, Cell neighborCell, Vector2Int direction)
    {
        bool changed = false;

        List<TileState> possibleStates = new List<TileState>(neighborCell.possibleTileStates);

        foreach (var neighborState in possibleStates)
        {
            bool isCompatible = false;

            foreach (var cellState in cell.possibleTileStates)
            {
                if (AreSocketsCompatible(cellState, neighborState, direction))
                {
                    isCompatible = true;
                    break;
                }
            }

            if (!isCompatible)
            {
                neighborCell.possibleTileStates.Remove(neighborState);
                changed = true;
            }
        }

        return changed;
    }

    private bool AreSocketsCompatible(TileState cellState, TileState neighborState, Vector2Int direction)
    {
        string dirString = GetDirectionString(direction);
        string oppositeDirString = GetOppositeDirectionString(direction);

        var cellSockets = cellState.tile.rotatedSockets[cellState.rotationIndex];
        var neighborSockets = neighborState.tile.rotatedSockets[neighborState.rotationIndex];

        if (!cellSockets.ContainsKey(dirString) || !neighborSockets.ContainsKey(oppositeDirString))
            return false;

        var cellSocketList = cellSockets[dirString];
        var neighborSocketList = neighborSockets[oppositeDirString];

        return cellSocketList.Intersect(neighborSocketList).Any();
    }



    // ���� ����
    private static readonly Vector2Int[] Directions = {
        new Vector2Int(0, 1),  // ����
        new Vector2Int(1, 0),  // ����
        new Vector2Int(0, -1), // ����
        new Vector2Int(-1, 0)  // ����
    };

    private bool IsValidPosition(Chunk chunk, Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < chunk.width && pos.y >= 0 && pos.y < chunk.height;
    }

    private string GetDirectionString(Vector2Int dir)
    {
        if (dir == new Vector2Int(0, 1)) return "north";
        if (dir == new Vector2Int(1, 0)) return "east";
        if (dir == new Vector2Int(0, -1)) return "south";
        if (dir == new Vector2Int(-1, 0)) return "west";
        return "";
    }

    private string GetOppositeDirectionString(Vector2Int dir)
    {
        if (dir == new Vector2Int(0, 1)) return "south";
        if (dir == new Vector2Int(1, 0)) return "west";
        if (dir == new Vector2Int(0, -1)) return "north";
        if (dir == new Vector2Int(-1, 0)) return "east";
        return "";
    }

    // ��Ʈ��ŷ�� ���� ���� ����
    private void SaveWaveState(Chunk chunk)
    {
        WaveState state = new WaveState();

        foreach (var cell in chunk.cells)
        {
            CellState cellState = new CellState
            {
                position = cell.position,
                possibleTileStates = new List<TileState>(cell.possibleTileStates),
                collapsedTileState = cell.collapsedTileState
            };
            state.cellStates.Add(cellState);
        }

        backtrackStack.Push(state);
    }

    private bool Backtrack(Chunk chunk)
    {
        while (backtrackStack.Count > 0)
        {
            WaveState previousState = backtrackStack.Pop();

            // ���� ���·� ����
            foreach (var cellState in previousState.cellStates)
            {
                Cell cell = chunk.cells[cellState.position.x, cellState.position.y];
                cell.possibleTileStates = new List<TileState>(cellState.possibleTileStates);
                cell.collapsedTileState = cellState.collapsedTileState;
            }

            // ���������� �ر��� �� ����
            Cell lastCollapsedCell = previousState.cellStates
                .Where(cs => cs.collapsedTileState != null)
                .Select(cs => chunk.cells[cs.position.x, cs.position.y])
                .LastOrDefault();

            if (lastCollapsedCell != null)
            {
                // ������ ���� �߿��� ������ ������ Ÿ���� ����
                var previousTileState = lastCollapsedCell.collapsedTileState;
                lastCollapsedCell.possibleTileStates.Remove(previousTileState);
                lastCollapsedCell.collapsedTileState = null;

                if (lastCollapsedCell.possibleTileStates.Count == 0)
                {
                    // �� ������ �� �̻� ������ �� �ִ� Ÿ���� �����Ƿ� ��� ��Ʈ��ŷ
                    continue;
                }

                // ���ο� ���� ����
                SaveWaveState(chunk);

                // �� �ر� �� ���� ���� ��õ�
                bool success = CollapseCell(lastCollapsedCell);
                if (success)
                {
                    success = PropagateConstraints(chunk, lastCollapsedCell);
                    if (success)
                    {
                        return true;
                    }
                }
            }
        }

        // ��Ʈ��ŷ ����
        return false;
    }
}

// ��Ʈ��ŷ�� ���� ���� Ŭ����
public class WaveState
{
    public List<CellState> cellStates = new List<CellState>();
}

public class CellState
{
    public Vector2Int position;
    public List<TileState> possibleTileStates;
    public TileState collapsedTileState;
}
