using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;

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
    public Tile[,] tiles = new Tile[16, 16]; // 청크 타일
    public GameObject chunkObject; // 청크의 부모 오브젝트

    public Chunk(Vector2Int pos, BiomeType biome, GameObject chunkObj)
    {
        position = pos;
        biomeType = biome;
        chunkObject = chunkObj; // 청크의 부모 오브젝트
    }
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
    public bool showFullMap = false; // 전체 맵을 보여줄지 여부를 설정하는 bool 값

    private Dictionary<Vector2Int, Chunk> loadedChunks = new Dictionary<Vector2Int, Chunk>();
    private Queue<ChunkLoadRequest> chunksToLoad = new Queue<ChunkLoadRequest>(); // 로드할 청크 큐
    private HashSet<Vector2Int> chunksToKeep = new HashSet<Vector2Int>();
    private object chunkLock = new object(); // 스레드 안전성을 위한 락 객체

    private Vector2 lastPlayerPosition; // 플레이어의 마지막 위치

    private void Start()
    {
        // 시드 설정
        Random.InitState(seed);

        // 초기 청크 로드: showFullMap이 true이면 전체 맵을 로드
        if (showFullMap)
        {
            GenerateFullMap();
        }
        else
        {
            lastPlayerPosition = Vector2.zero;
            // 청크를 비동기적으로 로드
            Task.Run(() => UpdateChunkVisibilityAsync(lastPlayerPosition));
        }
    }

    // 전체 맵을 생성 (싱글 스레드)
    void GenerateFullMap()
    {
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                Vector2Int chunkPosition = new Vector2Int(x * chunkSize, y * chunkSize);
                BiomeType biome = GetBiomeForChunk(chunkPosition);
                GameObject chunkObject = new GameObject($"Chunk_{chunkPosition.x}_{chunkPosition.y}");
                Chunk newChunk = new Chunk(chunkPosition, biome, chunkObject);
                loadedChunks.Add(chunkPosition, newChunk);

                // 청크 내 타일 생성 (메인 스레드)
                GenerateTilesForChunk(newChunk);
            }
        }
    }

    async Task UpdateChunkVisibilityAsync(Vector2 playerPosition)
    {
        Vector2Int playerChunkPosition = new Vector2Int(
            Mathf.FloorToInt(playerPosition.x / chunkSize),
            Mathf.FloorToInt(playerPosition.y / chunkSize)
        );

        await Task.Run(() =>
        {
            for (int x = -2; x <= 2; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    Vector2Int chunkOffset = new Vector2Int(x, y);
                    Vector2Int chunkPosition = playerChunkPosition + chunkOffset;
                    Vector2Int chunkWorldPosition = chunkPosition * chunkSize;

                    if (!loadedChunks.ContainsKey(chunkWorldPosition))
                    {

                        BiomeType biome = GetBiomeForChunk(chunkPosition);

                        // ChunkLoadRequest 생성
                        ChunkLoadRequest newChunkRequest = new ChunkLoadRequest
                        {
                            position = chunkWorldPosition,
                            biomeType = biome
                        };

                        // 요청을 큐에 추가
                        lock (chunkLock)
                        {
                            chunksToLoad.Enqueue(newChunkRequest);
                        }
                    }
                    else
                    {
                    }
                }
            }
        });

    }

    private void Update()
    {
        lock (chunkLock)
        {
            while (chunksToLoad.Count > 0)
            {
                ChunkLoadRequest request = chunksToLoad.Dequeue();

                // 메인 스레드에서 GameObject와 Chunk 생성
                GameObject chunkObject = new GameObject($"Chunk_{request.position.x}_{request.position.y}");
                Chunk newChunk = new Chunk(request.position, request.biomeType, chunkObject);
                loadedChunks.Add(request.position, newChunk);

                // 타일 생성
                GenerateTilesForChunk(newChunk);
            }
        }

        if (!showFullMap)
        {
            Vector2 playerPosition = new Vector2(player.transform.position.x, player.transform.position.y);
            Vector2Int playerChunkPosition = new Vector2Int(
                Mathf.FloorToInt(playerPosition.x / chunkSize),
                Mathf.FloorToInt(playerPosition.y / chunkSize)
            );

            // 플레이어 주변 청크 계산
            HashSet<Vector2Int> newChunksToKeep = new HashSet<Vector2Int>();
            for (int x = -viewDistance; x <= viewDistance; x++)
            {
                for (int y = -viewDistance; y <= viewDistance; y++)
                {
                    Vector2Int chunkPosition = new Vector2Int(playerChunkPosition.x + x, playerChunkPosition.y + y) * chunkSize;
                    newChunksToKeep.Add(chunkPosition);

                    if (!loadedChunks.ContainsKey(chunkPosition))
                    {
                        // 청크 로드 요청
                        Task.Run(() => UpdateChunkVisibilityAsync(playerPosition));
                    }
                }
            }

            // 언로드할 청크 찾기
            List<Vector2Int> chunksToUnload = new List<Vector2Int>();
            foreach (var chunkPosition in loadedChunks.Keys)
            {
                if (!newChunksToKeep.Contains(chunkPosition))
                {
                    chunksToUnload.Add(chunkPosition);
                }
            }

            // 청크 언로드
            foreach (var chunkPosition in chunksToUnload)
            {
                UnloadChunk(chunkPosition);
            }

            chunksToKeep = newChunksToKeep;
            lastPlayerPosition = playerPosition;
        }
    }


    // 청크 내 타일 생성
    // 타일 생성 로직 디버깅 로그 추가
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
        float noiseValue = Mathf.PerlinNoise((clusterPosition.x + seed) * 0.02f, (clusterPosition.y + seed) * 0.02f);
        if (noiseValue < 0.33f) return BiomeType.Snow;
        else if (noiseValue < 0.66f) return BiomeType.Forest;
        else return BiomeType.Desert;
    }

    void UnloadChunk(Vector2Int chunkPosition)
    {
        if (loadedChunks.ContainsKey(chunkPosition))
        {
            Chunk chunkToUnload = loadedChunks[chunkPosition];

            // 청크 내 모든 타일을 삭제
            foreach (Transform tile in chunkToUnload.chunkObject.transform)
            {
                Destroy(tile.gameObject); // 타일 삭제
            }

            Destroy(chunkToUnload.chunkObject); // 청크 오브젝트 삭제
            loadedChunks.Remove(chunkPosition);
        }
    }
}
