using System.Collections.Generic;
using UnityEngine;

public class ChunkManager : MonoBehaviour
{
    public GameObject player; // 플레이어 오브젝트
    public int chunkSize = 10; // 청크 크기 (셀 단위)
    public int gridSize = 3;   // nxn 범위의 청크 관리 (홀수로 설정)
    public float cellSize = 1.0f; // 타일의 크기

    private Dictionary<Vector2Int, Chunk> existingChunks = new Dictionary<Vector2Int, Chunk>(); // 이미 생성된 청크
    private Vector2Int currentPlayerChunk; // 현재 플레이어가 위치한 청크 좌표

    // 필요한 컴포넌트 참조
    private BiomeManager biomeManager;
    private TileLoader tileLoader;

    void Start()
    {
        // 필요한 컴포넌트 가져오기
        biomeManager = FindObjectOfType<BiomeManager>();
        tileLoader = FindObjectOfType<TileLoader>();

        if (biomeManager == null || tileLoader == null)
        {
            Debug.LogError("BiomeManager 또는 TileLoader를 찾을 수 없습니다.");
            return;
        }

        // 변수 값 확인
        Debug.Log($"cellSize: {cellSize}, chunkSize: {chunkSize}");

        // 초기 청크 생성
        InitializeChunks();
    }

    void Update()
    {
        Vector3 playerPosition = player.transform.position;
        Vector2Int playerChunkCoord = GetChunkCoordFromPosition(playerPosition);

        if (playerChunkCoord != currentPlayerChunk)
        {
            Debug.Log($"플레이어가 청크 {currentPlayerChunk}에서 {playerChunkCoord}로 이동했습니다.");
            currentPlayerChunk = playerChunkCoord;
            UpdateChunks();
        }
    }

    private void InitializeChunks()
    {
        // 플레이어의 현재 청크 좌표 계산
        currentPlayerChunk = GetChunkCoordFromPosition(player.transform.position);

        // 중심 청크를 기준으로 nxn 범위 생성
        for (int x = -gridSize / 2; x <= gridSize / 2; x++)
        {
            for (int y = -gridSize / 2; y <= gridSize / 2; y++)
            {
                Vector2Int chunkCoord = new Vector2Int(currentPlayerChunk.x + x, currentPlayerChunk.y + y);
                if (!existingChunks.ContainsKey(chunkCoord))
                {
                    bool isInitial = (chunkCoord == currentPlayerChunk);
                    GenerateAndInstantiateChunk(chunkCoord, isInitial);
                }
            }
        }
    }

    private void UpdateChunks()
    {
        // 플레이어의 현재 청크 좌표를 기준으로 청크 생성
        Vector2Int playerChunkCoord = currentPlayerChunk;

        // 플레이어 주변의 청크를 생성
        for (int x = -gridSize / 2; x <= gridSize / 2; x++)
        {
            for (int y = -gridSize / 2; y <= gridSize / 2; y++)
            {
                Vector2Int chunkCoord = new Vector2Int(playerChunkCoord.x + x, playerChunkCoord.y + y);
                if (!existingChunks.ContainsKey(chunkCoord))
                {
                    GenerateAndInstantiateChunk(chunkCoord, isInitial: false);
                }
            }
        }
    }

    private void GenerateAndInstantiateChunk(Vector2Int chunkCoord, bool isInitial)
    {
        // 청크 생성
        Chunk newChunk = new Chunk(chunkCoord, chunkSize);

        // 청크 위치 설정
        float chunkWorldSize = chunkSize * cellSize; // 청크의 실제 월드 크기
        Vector3 chunkPosition = new Vector3(chunkCoord.x * chunkWorldSize, 0, chunkCoord.y * chunkWorldSize);
        newChunk.chunkObject.transform.position = chunkPosition;

        // 디버그 로그 추가
        Debug.Log($"청크 생성: {chunkCoord}, 위치: {chunkPosition}");

        // 청크를 딕셔너리에 저장
        existingChunks[chunkCoord] = newChunk;

        // WFC 적용
        WaveFunctionCollapse wfc = new WaveFunctionCollapse(biomeManager, cellSize);
        wfc.GenerateChunk(newChunk, chunkCoord, tileLoader.LoadedTiles, isPlayerSpawnChunk: isInitial);

        // 타일 인스턴스화
        wfc.InstantiateChunk(newChunk);
    }

    private Vector2Int GetChunkCoordFromPosition(Vector3 position)
    {
        float chunkWorldSize = chunkSize * cellSize;
        int chunkX = Mathf.FloorToInt(position.x / chunkWorldSize);
        int chunkY = Mathf.FloorToInt(position.z / chunkWorldSize);
        return new Vector2Int(chunkX, chunkY);
    }
}
