using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class WaveFunctionCollapse
{
    private BiomeManager biomeManager;
    private float cellSize; // 셀 크기
    private Stack<WaveState> backtrackStack = new Stack<WaveState>();

    public WaveFunctionCollapse(BiomeManager biomeManager, float cellSize)
    {
        this.biomeManager = biomeManager;
        this.cellSize = cellSize;
    }

    public void GenerateChunk(Chunk chunk, Vector2Int chunkCoord, List<Tile> allTiles, bool isPlayerSpawnChunk)
    {
        // 청크의 월드 위치 계산
        float chunkWorldSize = chunk.width * cellSize;
        Vector2 chunkWorldPosition = new Vector2(
            chunkCoord.x * chunkWorldSize,
            chunkCoord.y * chunkWorldSize
        );

        // 청크의 중앙 위치 사용 (옵션)
        chunkWorldPosition += new Vector2(chunkWorldSize / 2, chunkWorldSize / 2);

        // 디버그 로그 추가
        Debug.Log($"청크 {chunkCoord}의 chunkWorldSize: {chunkWorldSize}, chunkWorldPosition: {chunkWorldPosition}");

        // 바이옴 결정
        string biome = biomeManager.GetBiomeForPosition(chunkWorldPosition);
        biome = biome.Trim().ToLowerInvariant();

        // 노이즈 값 로그 출력
        float noiseValue = biomeManager.GetNoiseValue(chunkWorldPosition);
        Debug.Log($"청크 {chunkCoord}의 노이즈 값: {noiseValue:F4}, 바이옴: {biome}");



        // 바이옴에 맞는 타일 필터링
        List<Tile> filteredTiles = FilterTilesByBiome(biome, allTiles);
        Debug.Log($"필터링된 타일 수: {filteredTiles.Count}");

        if (filteredTiles.Count == 0)
        {
            Debug.LogWarning($"바이옴 '{biome}'에 맞는 타일이 없습니다. 모든 타일을 사용합니다.");
            filteredTiles = allTiles;
        }

        if (isPlayerSpawnChunk)
        {
            // 스폰 청크에 기본 타일 설정
            InitializeChunkWithDefaultTiles(chunk, filteredTiles);
        }
        else
        {
            // 일반 청크 생성
            InitializeChunk(chunk, filteredTiles);

            bool success = RunWFC(chunk);
            if (!success)
            {
                Debug.LogError("WFC를 사용하여 청크를 생성하는 데 실패했습니다.");
            }
        }
    }


    // 타일을 인스턴스화하는 메서드
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
                        Debug.LogError($"타일 프리팹이 null입니다: {state.tile.tileName} 위치 ({x}, {y})");
                        continue;
                    }

                    // 타일 위치 계산 (청크 위치 + 셀 위치)
                    Vector3 position = chunk.chunkObject.transform.position + new Vector3(x * cellSize, 0, y * cellSize);

                    GameObject obj = GameObject.Instantiate(
                        state.tile.prefab,
                        position,
                        Quaternion.Euler(0, state.tile.rotationAngles[state.rotationIndex], 0)
                    );

                    obj.name = $"{state.tile.tileName}_{x}_{y}";
                    obj.transform.parent = chunk.chunkObject.transform;

                    // 타일 생성 로그 추가
                    Debug.Log($"타일 생성됨: {obj.name} 위치 {position}");
                }
                else
                {
                    Debug.LogWarning($"셀 ({x}, {y})에 할당된 타일이 없습니다.");
                }
            }
        }
    }
    // 스폰 청크에 기본 타일 설정 메서드 추가
    private void InitializeChunkWithDefaultTiles(Chunk chunk, List<Tile> tiles)
    {
        // 청크의 셀 배열 초기화
        chunk.cells = new Cell[chunk.width, chunk.height];

        // 기본 타일 선택 (예: 첫 번째 타일)
        Tile defaultTile = tiles[0];

        for (int x = 0; x < chunk.width; x++)
        {
            for (int y = 0; y < chunk.height; y++)
            {
                Cell cell = new Cell();
                cell.position = new Vector2Int(x, y);

                // 기본 타일로 셀 붕괴
                cell.collapsedTileState = new TileState
                {
                    tile = defaultTile,
                    rotationIndex = 0 // 회전 없음
                };

                // 가능한 타일 상태는 기본 타일 하나만 포함
                cell.possibleTileStates = new List<TileState> { cell.collapsedTileState };

                chunk.cells[x, y] = cell;
            }
        }
    }




    private List<Tile> FilterTilesByBiome(string biome, List<Tile> allTiles)
    {
        List<Tile> matchingTiles = allTiles.FindAll(tile => tile.biomes.Any(b => b.Equals(biome, System.StringComparison.OrdinalIgnoreCase)));
        Debug.Log($"바이옴 '{biome}'에 맞는 타일 수: {matchingTiles.Count}");
        return matchingTiles;
    }


    public void InitializeChunk(Chunk chunk, List<Tile> tiles)
    {
        // 청크의 셀 배열 초기화
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
                    Debug.Log($"엣지 셀 ({x}, {y}) 가능한 타일 상태 수: {cell.possibleTileStates.Count}");
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
                    Debug.Log($"내부 셀 ({x}, {y}) 가능한 타일 상태 수: {cell.possibleTileStates.Count}");
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
                // 모든 셀이 붕괴됨
                Debug.Log("모든 셀이 붕괴되었습니다.");
                return true;
            }

            // 현재 상태 저장
            SaveWaveState(chunk);

            bool success = CollapseCell(cell);
            if (!success)
            {
                Debug.LogWarning($"셀 ({cell.position.x}, {cell.position.y}) 붕괴 실패. 백트래킹을 시도합니다.");
                // 백트래킹
                bool backtrackSuccess = Backtrack(chunk);
                if (!backtrackSuccess)
                {
                    Debug.LogError("백트래킹 실패. WFC 알고리즘을 종료합니다.");
                    return false;
                }
            }
            else
            {
                Debug.Log($"셀 ({cell.position.x}, {cell.position.y}) 붕괴 성공: {cell.collapsedTileState.tile.tileName}");

                bool propagateSuccess = PropagateConstraints(chunk, cell);
                if (!propagateSuccess)
                {
                    Debug.LogWarning("제약 전파 실패. 백트래킹을 시도합니다.");
                    // 백트래킹
                    bool backtrackSuccess = Backtrack(chunk);
                    if (!backtrackSuccess)
                    {
                        Debug.LogError("백트래킹 실패. WFC 알고리즘을 종료합니다.");
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

        if (!cellSockets.ContainsKey(dirString) || !neighborSockets.ContainsKey(oppositeDirString))
            return false;

        var cellSocketList = cellSockets[dirString];
        var neighborSocketList = neighborSockets[oppositeDirString];

        return cellSocketList.Intersect(neighborSocketList).Any();
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
