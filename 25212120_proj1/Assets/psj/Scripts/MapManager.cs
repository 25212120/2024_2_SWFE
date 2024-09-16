using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public Transform player; // 플레이어
    public GameObject chunkPrefab; // 청크 프리팹
    public int renderDistance = 2; // 로드할 청크 거리
    public int chunkSize = 16; // 청크 크기
    public int mapSize = 300; // 맵 전체 크기 (타일 단위)

    private Dictionary<Vector2Int, Chunk> loadedChunks = new Dictionary<Vector2Int, Chunk>();

    [Header("Mineral Zone Settings")]
    public int resourceZoneCount = 3; // 자원 집중 구역 개수
    public float minDistanceBetweenZones = 50f; // 자원 구역 간 최소 거리
    public GameObject mineralPrefab; // 미네랄 타일 프리팹
    public GameObject grassPrefab;

    private List<Vector2Int> resourceZones = new List<Vector2Int>();

    private void Start()
    {
        GenerateResourceZones(); // 자원 구역 생성
    }

    private void Update()
    {
        UpdateChunks();
    }

    // 플레이어 이동에 따른 청크 로드 및 언로드
    void UpdateChunks()
    {
        Vector2Int playerChunkCoord = GetChunkCoordFromPosition(player.position);
        HashSet<Vector2Int> neededChunks = new HashSet<Vector2Int>();

        for (int x = -renderDistance; x <= renderDistance; x++)
        {
            for (int y = -renderDistance; y <= renderDistance; y++)
            {
                Vector2Int chunkCoord = new Vector2Int(playerChunkCoord.x + x, playerChunkCoord.y + y);
                neededChunks.Add(chunkCoord);

                if (!loadedChunks.ContainsKey(chunkCoord))
                {
                    // 청크 좌표를 청크 크기를 기반으로 월드 좌표로 변환
                    Vector3 chunkPosition = new Vector3(chunkCoord.x * chunkSize, chunkCoord.y * chunkSize, 0);
                    GameObject newChunkObject = Instantiate(chunkPrefab, chunkPosition, Quaternion.identity);
                    Chunk newChunk = newChunkObject.GetComponent<Chunk>();
                    newChunk.Initialize(chunkCoord, chunkSize, mineralPrefab, grassPrefab, resourceZones);
                    loadedChunks.Add(chunkCoord, newChunk);
                }
            }
        }

        // 불필요한 청크 언로드
        List<Vector2Int> keys = new List<Vector2Int>(loadedChunks.Keys);
        foreach (Vector2Int coord in keys)
        {
            if (!neededChunks.Contains(coord))
            {
                loadedChunks[coord].Unload();
                Destroy(loadedChunks[coord].gameObject);
                loadedChunks.Remove(coord);
            }
        }
    }


    // 플레이어 위치를 청크 좌표로 변환
    Vector2Int GetChunkCoordFromPosition(Vector3 position)
    {
        int x = Mathf.FloorToInt(position.x / chunkSize);
        int y = Mathf.FloorToInt(position.y / chunkSize);
        return new Vector2Int(x, y);
    }

    // 자원 집중 구역 생성
    void GenerateResourceZones()
    {
        // 플레이어 위치를 기준으로 중앙 자원 구역 생성
        Vector2Int playerPosition = GetChunkCoordFromPosition(player.position);
        Vector2Int center = playerPosition;
        resourceZones.Add(center);
        CreateResourceZoneMarker(center, "Central Resource Zone");

        // 나머지 자원 구역을 생성하면서 거리 계산
        for (int i = 0; i < resourceZoneCount; i++)
        {
            Vector2Int newZone;
            int attempt = 0;
            bool zoneCreated = false; // 구역 생성 여부 확인

            do
            {
                // 랜덤으로 자원 구역 생성
                newZone = new Vector2Int(
                    playerPosition.x + Random.Range(-mapSize / 2, mapSize / 2),
                    playerPosition.y + Random.Range(-mapSize / 2, mapSize / 2)
                );
                attempt++;

                // 거리 조건이 충족되면 구역을 생성하고 루프 종료
                if (!IsTooCloseToOtherZones(newZone))
                {
                    resourceZones.Add(newZone);
                    CreateResourceZoneMarker(newZone, $"Resource Zone {i + 1}");
                    zoneCreated = true; // 구역 생성 완료
                    break;
                }

                // 1000번 시도 후에도 구역을 생성하지 못한 경우
                if (attempt > 1000)
                {
                    Debug.LogWarning("자원 구역 생성 실패: 최대 시도 횟수 초과");
                    // 조건을 완화하여 강제로 구역 생성
                    resourceZones.Add(newZone);
                    CreateResourceZoneMarker(newZone, $"Resource Zone {i + 1} (Forced)");
                    zoneCreated = true;
                    break;
                }

            } while (!zoneCreated); // 구역이 생성될 때까지 반복
        }
    }


    void CreateResourceZoneMarker(Vector2Int position, string name)
    {
        GameObject zoneMarker = new GameObject(name); // 빈 GameObject 생성
        zoneMarker.transform.position = new Vector3(position.x, position.y, 0); // 위치 설정
    }


    // 자원 집중 구역 간 최소 거리 검사
    bool IsTooCloseToOtherZones(Vector2Int newZone)
    {
        int resourceZoneRadius = 20; // 자원 구역의 반경

        // 첫 번째 구역인 중앙 자원 구역을 제외하고 나머지 자원 구역과의 거리만 계산
        for (int i = 0; i < resourceZones.Count; i++)
        {
            Vector2Int zone = resourceZones[i];

            // 자원 구역 간의 중심 거리 계산
            float centerDistance = Vector2Int.Distance(newZone, zone);

            // 구역 경계 간 거리 계산 (중심 거리에서 반경을 뺀 값)
            float distanceBetweenEdges = centerDistance - (2 * resourceZoneRadius);

            // 최소 거리 조건을 충족하지 않으면 true 반환
            if (distanceBetweenEdges < minDistanceBetweenZones)
            {
                return true;
            }
            Debug.Log(distanceBetweenEdges);
        }

        return false;
    }


}
