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
    public Vector2Int position; // ûũ�� ��ǥ
    public BiomeType biomeType; // ûũ�� ���̿� Ÿ��
    public GameObject chunkObject; // ûũ�� �θ� ������Ʈ

    public Chunk(Vector2Int pos, BiomeType biome, GameObject chunkObj)
    {
        position = pos;
        biomeType = biome;
        chunkObject = chunkObj; // ûũ�� �θ� ������Ʈ
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
    public GameObject player;  // �÷��̾� ������Ʈ
    public GameObject tilePrefab; // Ÿ�� ������
    public int mapWidth = 20; // �� �ʺ� (ûũ ����)
    public int mapHeight = 20; // �� ���� (ûũ ����)
    public int seed = 12345;  // �õ� ��
    public int chunkSize = 16; // ûũ ũ��
    public int viewDistance = 2; // �÷��̾� �ֺ� ûũ �ε� �Ÿ�
    public bool showFullMap = false; // ��ü ���� �������� ����

    private Dictionary<Vector2Int, Chunk> loadedChunks = new Dictionary<Vector2Int, Chunk>();
    private ConcurrentQueue<ChunkLoadRequest> chunksToLoad = new ConcurrentQueue<ChunkLoadRequest>(); // �ε��� ûũ ť
    private ConcurrentQueue<UnloadChunkRequest> chunksToUnload = new ConcurrentQueue<UnloadChunkRequest>();
    private HashSet<Vector2Int> chunksToKeep = new HashSet<Vector2Int>(); // �����ؾ� �� ûũ ��ǥ ����

    private Vector2 lastPlayerPosition; // �÷��̾��� ������ ��ġ

    private void Start()
    {
        // �õ� ����
        Random.InitState(seed);

            lastPlayerPosition = Vector2.zero;
            // �񵿱������� ûũ �ε�
            Task.Run(() => UpdateChunkVisibilityAsync(lastPlayerPosition));
    }

    // ûũ ���ü� ������Ʈ (�񵿱�)
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

                    // ChunkLoadRequest ����
                    ChunkLoadRequest newChunkRequest = new ChunkLoadRequest
                    {
                        position = chunkWorldPosition,
                        biomeType = biome
                    };

                    chunkRequests.Add(newChunkRequest);
                }
            }
        }

        // �ε��� ûũ ��û ť�� �߰�
        foreach (var request in chunkRequests)
        {
            chunksToLoad.Enqueue(request);
        }

        // ��ε��� ûũ ����
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
                // ���� �����忡�� GameObject�� Chunk ����
                GameObject chunkObject = new GameObject($"Chunk_{request.position.x}_{request.position.y}");
                Chunk newChunk = new Chunk(request.position, request.biomeType, chunkObject);
                loadedChunks.Add(request.position, newChunk);

                // Ÿ�� ����
                GenerateTilesForChunk(newChunk);
            }

        }

        while (chunksToUnload.TryDequeue(out UnloadChunkRequest unloadRequest))
        {
            UnloadChunk(unloadRequest.position);
        }

        // �÷��̾� �̵� �� ûũ ������Ʈ ó��
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

    // ûũ �� Ÿ�� ����
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

    // Ÿ���� ������ ���̿ȿ� �°� ����
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

    // �õ带 ������� �۸� ����� ����Ͽ� ���̿� ����
    BiomeType GetBiomeForChunk(Vector2Int chunkPosition)
    {
        int clusterSize = 4;  // ûũ �׷�ȭ ũ��
        Vector2Int clusterPosition = new Vector2Int(
            Mathf.FloorToInt(chunkPosition.x / clusterSize),
            Mathf.FloorToInt(chunkPosition.y / clusterSize)
        );

        // �õ带 ������� �۸� ������ ������ ����
        float noiseValue = Mathf.PerlinNoise((clusterPosition.x + seed) * 0.06f, (clusterPosition.y + seed) * 0.06f);
        if (noiseValue < 0.33f) return BiomeType.Snow;
        else if (noiseValue < 0.66f) return BiomeType.Forest;
        else return BiomeType.Desert;
    }

    // ûũ ��ε�
    void UnloadChunk(Vector2Int chunkPosition)
    {
        if (loadedChunks.ContainsKey(chunkPosition))
        {
            Chunk chunkToUnload = loadedChunks[chunkPosition];

            // ûũ ������Ʈ ���� (ûũ �� ��� Ÿ�� ����)
            Destroy(chunkToUnload.chunkObject);
            loadedChunks.Remove(chunkPosition);
        }
    }
}
