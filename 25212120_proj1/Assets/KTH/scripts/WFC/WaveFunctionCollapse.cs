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
        // 바이옴 결정
        string biome = biomeManager.GetBiomeForPosition(chunkPosition);

        // 바이옴에 맞는 타일 필터링
        List<Tile> filteredTiles = FilterTilesByBiome(biome, allTiles);

        // 청크 초기화 및 WFC 실행
        InitializeChunk(chunk, filteredTiles);

        bool success = RunWFC(chunk);
        if (!success)
        {
            Debug.LogError("Failed to generate chunk with WFC.");
        }
        else
        {
            // 청크에 타일 인스턴스화
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
                    // 엣지 셀: `isEdgeTile == true`인 타일만 허용
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
                    // 내부 셀: 모든 타일 허용
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
                // 모든 셀이 붕괴됨
                return true;
            }

            // 현재 상태 저장
            SaveWaveState(chunk);

            bool success = CollapseCell(cell);
            if (!success)
            {
                // 백트래킹
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
                    // 백트래킹
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

        // 무작위로 셀 선택
        int randomIndex = Random.Range(0, minEntropyCells.Count);
        return minEntropyCells[randomIndex];
    }

    private bool CollapseCell(Cell cell)
    {
        // 가능한 상태가 없는 경우 실패
        if (cell.possibleTileStates.Count == 0)
            return false;

        // 가중치에 따라 타일 상태 선택
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

        // 선택 실패 시 임의의 상태 선택
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
                        return false; // 제약 조건을 만족하는 타일이 없음
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

    private float cellSize = 50.0f; // 셀 크기

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

                    // 셀 크기를 반영한 좌표 계산
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



    // 방향 벡터
    private static readonly Vector2Int[] Directions = {
        new Vector2Int(0, 1),  // 북쪽
        new Vector2Int(1, 0),  // 동쪽
        new Vector2Int(0, -1), // 남쪽
        new Vector2Int(-1, 0)  // 서쪽
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

    // 백트래킹을 위한 상태 저장
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

            // 이전 상태로 복원
            foreach (var cellState in previousState.cellStates)
            {
                Cell cell = chunk.cells[cellState.position.x, cellState.position.y];
                cell.possibleTileStates = new List<TileState>(cellState.possibleTileStates);
                cell.collapsedTileState = cellState.collapsedTileState;
            }

            // 마지막으로 붕괴된 셀 선택
            Cell lastCollapsedCell = previousState.cellStates
                .Where(cs => cs.collapsedTileState != null)
                .Select(cs => chunk.cells[cs.position.x, cs.position.y])
                .LastOrDefault();

            if (lastCollapsedCell != null)
            {
                // 가능한 상태 중에서 이전에 선택한 타일을 제외
                var previousTileState = lastCollapsedCell.collapsedTileState;
                lastCollapsedCell.possibleTileStates.Remove(previousTileState);
                lastCollapsedCell.collapsedTileState = null;

                if (lastCollapsedCell.possibleTileStates.Count == 0)
                {
                    // 이 셀에서 더 이상 선택할 수 있는 타일이 없으므로 계속 백트래킹
                    continue;
                }

                // 새로운 상태 저장
                SaveWaveState(chunk);

                // 셀 붕괴 및 제약 전파 재시도
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

        // 백트래킹 실패
        return false;
    }
}

// 백트래킹을 위한 상태 클래스
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
