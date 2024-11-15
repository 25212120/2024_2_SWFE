using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class WaveFunctionCollapse
{
    private BiomeManager biomeManager;
    private Stack<WaveState> backtrackStack = new Stack<WaveState>();

    public WaveFunctionCollapse(BiomeManager biomeManager)
    {
        this.biomeManager = biomeManager;
    }

    public void GenerateChunk(Chunk chunk, Vector2 chunkPosition, List<Tile> allTiles)
    {
        // ���̿� ����
        string biome = biomeManager.GetBiomeForPosition(chunkPosition);

        // ���̿ȿ� �´� Ÿ�� ���͸�
        List<Tile> filteredTiles = FilterTilesByBiome(biome, allTiles);

        // ûũ �ʱ�ȭ �� WFC ����
        InitializeChunk(chunk, filteredTiles);

        bool success = RunWFC(chunk);
        if (!success)
        {
            Debug.LogError("Failed to generate chunk with WFC.");
        }
        else
        {
            // ûũ�� Ÿ�� �ν��Ͻ�ȭ
            InstantiateChunk(chunk);
        }
    }

    private List<Tile> FilterTilesByBiome(string biome, List<Tile> allTiles)
    {
        return allTiles.FindAll(tile => tile.biomes.Contains(biome));
    }

    private void InitializeChunk(Chunk chunk, List<Tile> tiles)
    {
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
                }

                chunk.cells[x, y] = cell;
            }
        }
    }



    private bool RunWFC(Chunk chunk)
    {
        while (true)
        {
            Cell cell = SelectCellWithLowestEntropy(chunk);
            if (cell == null)
            {
                // ��� ���� �ر���
                return true;
            }

            // ���� ���� ����
            SaveWaveState(chunk);

            bool success = CollapseCell(cell);
            if (!success)
            {
                // ��Ʈ��ŷ
                bool backtrackSuccess = Backtrack(chunk);
                if (!backtrackSuccess)
                {
                    return false;
                }
            }
            else
            {
                bool propagateSuccess = PropagateConstraints(chunk, cell);
                if (!propagateSuccess)
                {
                    // ��Ʈ��ŷ
                    bool backtrackSuccess = Backtrack(chunk);
                    if (!backtrackSuccess)
                    {
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

        var cellSocketList = cellSockets[dirString];
        var neighborSocketList = neighborSockets[oppositeDirString];

        return cellSocketList.Intersect(neighborSocketList).Any();
    }

    private float cellSize = 50.0f; // �� ũ��

    private void InstantiateChunk(Chunk chunk)
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
                        Debug.LogError($"Tile prefab is null for tile {state.tile.tileName} at position ({x}, {y}).");
                        continue;
                    }

                    // �� ũ�⸦ �ݿ��� ��ǥ ���
                    Vector3 position = new Vector3(x * cellSize, 0, y * cellSize);
                    GameObject obj = GameObject.Instantiate(
                        state.tile.prefab,
                        position,
                        Quaternion.Euler(0, state.tile.rotationAngles[state.rotationIndex], 0)
                    );

                    obj.name = $"{state.tile.tileName}_{x}_{y}";
                    Debug.Log($"Tile instantiated: {obj.name} at position {position}");
                }
            }
        }
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
