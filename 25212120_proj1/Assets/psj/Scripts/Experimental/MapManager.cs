using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using UnityEngine;

public enum BiomeType
{
    Snow,
    Forest,
    Desert
}


public class Chunk
{
    public Vector2Int position; // 청크의 좌표
    public BiomeType biomeType; // 청크의 바이옴 타입
    public GameObject chunkObject; // 청크의 부모 오브젝트

    public Chunk(Vector2Int pos, BiomeType biome, GameObject chunkObj)
    {
        position = pos;
        biomeType = biome;
        chunkObject = chunkObj; // 청크의 부모 오브젝트
    }
}

class UnloadChunkRequest
{
    public Vector2Int position;
} 

class ChunkLoadRequest
{
    public Vector2Int position;
    public BiomeType biomeType;
}

public class MapManager : MonoBehaviour
{
    public GameObject player;  // 플레이어 오브젝트
    public GameObject tilePrefab; // 타일 프리팹
    public int mapWidth = 20; // 맵 너비 (청크 단위)
    public int mapHeight = 20; // 맵 높이 (청크 단위)
    public int seed = 12345;  // 시드 값
    public int chunkSize = 16; // 청크 크기
    public int viewDistance = 2; // 플레이어 주변 청크 로드 거리
    public bool showFullMap = false; // 전체 맵을 보여줄지 여부

    private Dictionary<Vector2Int, Chunk> loadedChunks = new Dictionary<Vector2Int, Chunk>();
    private ConcurrentQueue<ChunkLoadRequest> chunksToLoad = new ConcurrentQueue<ChunkLoadRequest>(); // 로드할 청크 큐
    private ConcurrentQueue<UnloadChunkRequest> chunksToUnload = new ConcurrentQueue<UnloadChunkRequest>();
    private HashSet<Vector2Int> chunksToKeep = new HashSet<Vector2Int>(); // 유지해야 할 청크 좌표 집합

    private Vector2 lastPlayerPosition; // 플레이어의 마지막 위치

    private void Start()
    {
        // 시드 설정
        Random.InitState(seed);

            lastPlayerPosition = Vector2.zero;
            // 비동기적으로 청크 로드
            Task.Run(() => UpdateChunkVisibilityAsync(lastPlayerPosition));
    }

    // 청크 가시성 업데이트 (비동기)
    void UpdateChunkVisibilityAsync(Vector2 playerPosition)
    {
        Vector2Int playerChunkPosition = new Vector2Int(
            Mathf.FloorToInt(playerPosition.x / chunkSize),
            Mathf.FloorToInt(playerPosition.y / chunkSize)
        );

        HashSet<Vector2Int> newChunksToKeep = new HashSet<Vector2Int>();
        List<ChunkLoadRequest> chunkRequests = new List<ChunkLoadRequest>();

        for (int x = -viewDistance; x <= viewDistance; x++)
        {
            for (int y = -viewDistance; y <= viewDistance; y++)
            {
                Vector2Int chunkOffset = new Vector2Int(x, y);
                Vector2Int chunkPosition = playerChunkPosition + chunkOffset;
                Vector2Int chunkWorldPosition = chunkPosition * chunkSize;

                newChunksToKeep.Add(chunkWorldPosition);

                if (!loadedChunks.ContainsKey(chunkWorldPosition))
                {
                    BiomeType biome = GetBiomeForChunk(chunkPosition);

                    // ChunkLoadRequest 생성
                    ChunkLoadRequest newChunkRequest = new ChunkLoadRequest
                    {
                        position = chunkWorldPosition,
                        biomeType = biome
                    };

                    chunkRequests.Add(newChunkRequest);
                }
            }
        }

        // 로드할 청크 요청 큐에 추가
        foreach (var request in chunkRequests)
        {
            chunksToLoad.Enqueue(request);
        }

        // 언로드할 청크 결정
        foreach (var loadedChunkPosition in loadedChunks.Keys)
        {
            if (!newChunksToKeep.Contains(loadedChunkPosition))
            {
                UnloadChunkRequest unloadRequest = new UnloadChunkRequest
                {
                    position = loadedChunkPosition
                };
                chunksToUnload.Enqueue(unloadRequest);
            }
        }
    }

    private void Update()
    {
        while (chunksToLoad.TryDequeue(out ChunkLoadRequest request))
        {
            if (!loadedChunks.ContainsKey(request.position))
            {
                // 메인 스레드에서 GameObject와 Chunk 생성
                GameObject chunkObject = new GameObject($"Chunk_{request.position.x}_{request.position.y}");
                Chunk newChunk = new Chunk(request.position, request.biomeType, chunkObject);
                loadedChunks.Add(request.position, newChunk);

                // 타일 생성
                GenerateTilesForChunk(newChunk);
            }

        }

        while (chunksToUnload.TryDequeue(out UnloadChunkRequest unloadRequest))
        {
            UnloadChunk(unloadRequest.position);
        }

        // 플레이어 이동 및 청크 업데이트 처리
        if (!showFullMap)
        {
            Vector2 playerPosition = new Vector2(player.transform.position.x, player.transform.position.y);

            if (Vector2.Distance(playerPosition, lastPlayerPosition) > chunkSize / 2)
            {
                Task.Run(() => UpdateChunkVisibilityAsync(playerPosition));
                lastPlayerPosition = playerPosition;
            }
        }
    }

    // 청크 내 타일 생성
    void GenerateTilesForChunk(Chunk chunk)
    {
        for (int x = 0; x < chunkSize; x++)
        {
            for (int y = 0; y < chunkSize; y++)
            {
                Vector2 tilePosition = new Vector2(chunk.position.x + x, chunk.position.y + y);
                GameObject tile = Instantiate(tilePrefab, tilePosition, Quaternion.identity);

                tile.transform.parent = chunk.chunkObject.transform;

                SetTileAppearance(tile, chunk.biomeType);
            }
        }
    }

    // 타일의 외형을 바이옴에 맞게 설정
    void SetTileAppearance(GameObject tile, BiomeType biome)
    {
        SpriteRenderer spriteRenderer = tile.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            switch (biome)
            {
                case BiomeType.Snow:
                    spriteRenderer.color = Color.white;
                    break;
                case BiomeType.Forest:
                    spriteRenderer.color = Color.green;
                    break;
                case BiomeType.Desert:
                    spriteRenderer.color = Color.yellow;
                    break;
            }
        }
    }

    // 시드를 기반으로 퍼린 노이즈를 사용하여 바이옴 결정
    BiomeType GetBiomeForChunk(Vector2Int chunkPosition)
    {
        int clusterSize = 4;  // 청크 그룹화 크기
        Vector2Int clusterPosition = new Vector2Int(
            Mathf.FloorToInt(chunkPosition.x / clusterSize),
            Mathf.FloorToInt(chunkPosition.y / clusterSize)
        );

        // 시드를 기반으로 퍼린 노이즈 스케일 조정
        float noiseValue = Mathf.PerlinNoise((clusterPosition.x + seed) * 0.06f, (clusterPosition.y + seed) * 0.06f);
        if (noiseValue < 0.33f) return BiomeType.Snow;
        else if (noiseValue < 0.66f) return BiomeType.Forest;
        else return BiomeType.Desert;
    }

    // 청크 언로드
    void UnloadChunk(Vector2Int chunkPosition)
    {
        if (loadedChunks.ContainsKey(chunkPosition))
        {
            Chunk chunkToUnload = loadedChunks[chunkPosition];

            // 청크 오브젝트 삭제 (청크 내 모든 타일 포함)
            Destroy(chunkToUnload.chunkObject);
            loadedChunks.Remove(chunkPosition);
        }
    }
}
