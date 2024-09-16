using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public Transform player; // �÷��̾�
    public GameObject chunkPrefab; // ûũ ������
    public int renderDistance = 2; // �ε��� ûũ �Ÿ�
    public int chunkSize = 16; // ûũ ũ��
    public int mapSize = 300; // �� ��ü ũ�� (Ÿ�� ����)

    private Dictionary<Vector2Int, Chunk> loadedChunks = new Dictionary<Vector2Int, Chunk>();

    [Header("Mineral Zone Settings")]
    public int resourceZoneCount = 3; // �ڿ� ���� ���� ����
    public float minDistanceBetweenZones = 50f; // �ڿ� ���� �� �ּ� �Ÿ�
    public GameObject mineralPrefab; // �̳׶� Ÿ�� ������
    public GameObject grassPrefab;

    private List<Vector2Int> resourceZones = new List<Vector2Int>();

    private void Start()
    {
        GenerateResourceZones(); // �ڿ� ���� ����
    }

    private void Update()
    {
        UpdateChunks();
    }

    // �÷��̾� �̵��� ���� ûũ �ε� �� ��ε�
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
                    // ûũ ��ǥ�� ûũ ũ�⸦ ������� ���� ��ǥ�� ��ȯ
                    Vector3 chunkPosition = new Vector3(chunkCoord.x * chunkSize, chunkCoord.y * chunkSize, 0);
                    GameObject newChunkObject = Instantiate(chunkPrefab, chunkPosition, Quaternion.identity);
                    Chunk newChunk = newChunkObject.GetComponent<Chunk>();
                    newChunk.Initialize(chunkCoord, chunkSize, mineralPrefab, grassPrefab, resourceZones);
                    loadedChunks.Add(chunkCoord, newChunk);
                }
            }
        }

        // ���ʿ��� ûũ ��ε�
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


    // �÷��̾� ��ġ�� ûũ ��ǥ�� ��ȯ
    Vector2Int GetChunkCoordFromPosition(Vector3 position)
    {
        int x = Mathf.FloorToInt(position.x / chunkSize);
        int y = Mathf.FloorToInt(position.y / chunkSize);
        return new Vector2Int(x, y);
    }

    // �ڿ� ���� ���� ����
    void GenerateResourceZones()
    {
        // �÷��̾� ��ġ�� �������� �߾� �ڿ� ���� ����
        Vector2Int playerPosition = GetChunkCoordFromPosition(player.position);
        Vector2Int center = playerPosition;
        resourceZones.Add(center);
        CreateResourceZoneMarker(center, "Central Resource Zone");

        // ������ �ڿ� ������ �����ϸ鼭 �Ÿ� ���
        for (int i = 0; i < resourceZoneCount; i++)
        {
            Vector2Int newZone;
            int attempt = 0;
            bool zoneCreated = false; // ���� ���� ���� Ȯ��

            do
            {
                // �������� �ڿ� ���� ����
                newZone = new Vector2Int(
                    playerPosition.x + Random.Range(-mapSize / 2, mapSize / 2),
                    playerPosition.y + Random.Range(-mapSize / 2, mapSize / 2)
                );
                attempt++;

                // �Ÿ� ������ �����Ǹ� ������ �����ϰ� ���� ����
                if (!IsTooCloseToOtherZones(newZone))
                {
                    resourceZones.Add(newZone);
                    CreateResourceZoneMarker(newZone, $"Resource Zone {i + 1}");
                    zoneCreated = true; // ���� ���� �Ϸ�
                    break;
                }

                // 1000�� �õ� �Ŀ��� ������ �������� ���� ���
                if (attempt > 1000)
                {
                    Debug.LogWarning("�ڿ� ���� ���� ����: �ִ� �õ� Ƚ�� �ʰ�");
                    // ������ ��ȭ�Ͽ� ������ ���� ����
                    resourceZones.Add(newZone);
                    CreateResourceZoneMarker(newZone, $"Resource Zone {i + 1} (Forced)");
                    zoneCreated = true;
                    break;
                }

            } while (!zoneCreated); // ������ ������ ������ �ݺ�
        }
    }


    void CreateResourceZoneMarker(Vector2Int position, string name)
    {
        GameObject zoneMarker = new GameObject(name); // �� GameObject ����
        zoneMarker.transform.position = new Vector3(position.x, position.y, 0); // ��ġ ����
    }


    // �ڿ� ���� ���� �� �ּ� �Ÿ� �˻�
    bool IsTooCloseToOtherZones(Vector2Int newZone)
    {
        int resourceZoneRadius = 20; // �ڿ� ������ �ݰ�

        // ù ��° ������ �߾� �ڿ� ������ �����ϰ� ������ �ڿ� �������� �Ÿ��� ���
        for (int i = 0; i < resourceZones.Count; i++)
        {
            Vector2Int zone = resourceZones[i];

            // �ڿ� ���� ���� �߽� �Ÿ� ���
            float centerDistance = Vector2Int.Distance(newZone, zone);

            // ���� ��� �� �Ÿ� ��� (�߽� �Ÿ����� �ݰ��� �� ��)
            float distanceBetweenEdges = centerDistance - (2 * resourceZoneRadius);

            // �ּ� �Ÿ� ������ �������� ������ true ��ȯ
            if (distanceBetweenEdges < minDistanceBetweenZones)
            {
                return true;
            }
            Debug.Log(distanceBetweenEdges);
        }

        return false;
    }


}
