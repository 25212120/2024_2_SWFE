using System.Collections.Generic;
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

    public Chunk(Vector2Int pos, BiomeType biome)
    {
        position = pos;
        biomeType = biome;
    }
}

public class MapManager : MonoBehaviour
{
    public int mapWidth = 20; // 맵 너비 (청크 단위)
    public int mapHeight = 20; // 맵 높이 (청크 단위)
    public GameObject tilePrefab;
    public int seed = 54321;  // 시드 값
    public int chunkSize = 16; // 청크 크기
    public int viewDistance = 2; // 플레이어 주변 청크 로드 거리

    private Dictionary<Vector2Int, Chunk> loadedChunks = new Dictionary<Vector2Int, Chunk>();

    private void Start()
    {
        // 시드 설정
        Random.InitState(seed);
        GenerateMap();
    }

    // 맵 전체 생성
    void GenerateMap()
    {
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                Vector2Int chunkPosition = new Vector2Int(x * chunkSize, y * chunkSize);
                BiomeType biome = GetBiomeForChunk(chunkPosition);
                Chunk newChunk = new Chunk(chunkPosition, biome);
                loadedChunks.Add(chunkPosition, newChunk);

                // 청크 내 타일 생성
                GenerateTilesForChunk(newChunk);
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
        else
        {
            Debug.LogError("SpriteRenderer not found on tilePrefab.");
        }
    }

    // 시드를 기반, Perlin Noise를 사용하여 바이옴 결정
    BiomeType GetBiomeForChunk(Vector2Int chunkPosition)
    {
        int clusterSize = 4;  // 청크 그룹화 크기
        Vector2Int clusterPosition = new Vector2Int(
            Mathf.FloorToInt(chunkPosition.x / clusterSize),
            Mathf.FloorToInt(chunkPosition.y / clusterSize)
        );

        // 곱하는 상수는 Perlin Noise 확대 비율임. 너무 세밀한 변화는 배제하기 위함
        float noiseValue = Mathf.PerlinNoise((clusterPosition.x + seed) * 0.04f, (clusterPosition.y + seed) * 0.04f);
        if (noiseValue < 0.33f) return BiomeType.Snow;
        else if (noiseValue < 0.66f) return BiomeType.Forest;
        else return BiomeType.Desert;
    }

    // 플레이어 주변 청크 로드
    void LoadChunksAroundPlayer(Vector2 playerPosition)
    {
        // 플레이어가 위치한 청크 계산
        Vector2Int playerChunkPosition = new Vector2Int(
            Mathf.FloorToInt(playerPosition.x / chunkSize),
            Mathf.FloorToInt(playerPosition.y / chunkSize)
        );

        // 플레이어 주변 청크 로드
        for (int x = -viewDistance; x <= viewDistance; x++)
        {
            for (int y = -viewDistance; y <= viewDistance; y++)
            {
                Vector2Int relativeChunkPosition = new Vector2Int(x, y);

                Vector2Int chunkPosition = playerChunkPosition + relativeChunkPosition;

                if (!loadedChunks.ContainsKey(chunkPosition))
                {
                    BiomeType biome = GetBiomeForChunk(chunkPosition);
                    Chunk newChunk = new Chunk(chunkPosition * chunkSize, biome);
                    loadedChunks.Add(chunkPosition, newChunk);
                    GenerateTilesForChunk(newChunk);
                }
            }
        }
    }
}
