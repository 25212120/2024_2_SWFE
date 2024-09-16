using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    private int chunkSize;
    private Vector2Int position;
    private GameObject mineralPrefab;
    private GameObject grassPrefab;
    private List<Vector2Int> resourceZones;

    public void Initialize(Vector2Int position, int chunkSize, GameObject mineralPrefab, GameObject grassPrefab, List<Vector2Int> resourceZones)
    {
        this.position = position;
        this.chunkSize = chunkSize;
        this.mineralPrefab = mineralPrefab;
        this.grassPrefab = grassPrefab;
        this.resourceZones = resourceZones;

        GenerateChunk();
    }

    public void GenerateChunk()
    {
        int mineralCount = 0; // ���� ��ġ�� �̳׶� ����
        int minMineralsInZone = 10; // �ڿ� ������ �ּ� �̳׶� ����
        float minDistanceBetweenMinerals = 5f; // �̳׶� �� �ּ� �Ÿ�

        List<Vector2> placedMinerals = new List<Vector2>(); // �̹� ��ġ�� �̳׶� ��ġ ����

        for (int x = 0; x < chunkSize; x++)
        {
            for (int y = 0; y < chunkSize; y++)
            {
                // ûũ ��ġ�� �������� Ÿ���� ���� ��ǥ ���
                Vector3 tilePosition = new Vector3(position.x * chunkSize + x, position.y * chunkSize + y, 0);
                Vector2 worldPosition = new Vector2(position.x * chunkSize + x, position.y * chunkSize + y);

                // �ڿ� ������ ���� �ִ��� Ȯ��
                if (IsInResourceZone(new Vector2Int((int)worldPosition.x, (int)worldPosition.y)))
                {
                    // �ڿ� ������ �׵θ����� Ȯ��
                    if (IsEdgeOfResourceZone(new Vector2Int((int)worldPosition.x, (int)worldPosition.y)))
                    {
                        // �̳׶� �� �ּ� �Ÿ� ���� Ȯ��
                        bool tooCloseToOtherMinerals = false;
                        foreach (Vector2 placedMineral in placedMinerals)
                        {
                            if (Vector2.Distance(placedMineral, worldPosition) < minDistanceBetweenMinerals)
                            {
                                tooCloseToOtherMinerals = true;
                                break;
                            }
                        }

                        // �̳׶� ��ġ ����: �̳׶� �ּ� �Ÿ��� �̳׶� ���� ������ �����ؾ� ��
                        if (!tooCloseToOtherMinerals && (mineralCount < minMineralsInZone || Random.value > 0.95f))
                        {
                            // �̳׶��� Z�� -1�� �����Ͽ� Grass���� �տ� ������
                            Vector3 mineralPosition = new Vector3(tilePosition.x, tilePosition.y, -1); // Z���� ����
                            Instantiate(mineralPrefab, mineralPosition, Quaternion.identity, transform);
                            placedMinerals.Add(worldPosition); // ��ġ�� �̳׶� ��ġ ����
                            mineralCount++;
                        }
                        else
                        {
                            // �׵θ� ���������� �̳׶� �� �ּ� �Ÿ� ���� ���ϸ� Grass ��ġ
                            Instantiate(grassPrefab, tilePosition, Quaternion.identity, transform);
                        }
                    }
                    else
                    {
                        // �ڿ� ������ �߾� �κп��� Grass�� ��ġ
                        Instantiate(grassPrefab, tilePosition, Quaternion.identity, transform);
                    }
                }
                else
                {
                    // �ڿ� ���� ���� Grass�� ä��
                    Instantiate(grassPrefab, tilePosition, Quaternion.identity, transform);
                }
            }
        }

    }


    // �ڿ� ���� ������ �׵θ����� Ȯ���ϴ� �Լ�
    bool IsEdgeOfResourceZone(Vector2Int worldPosition)
    {
        foreach (Vector2Int zone in resourceZones)
        {
            float distance = Vector2Int.Distance(worldPosition, zone);

            // �׵θ� ���� ����
            if (distance > 9f && distance < 12f) // ���� ���� ������ �̳׶��� ��ġ�Ǵ� ������ �ø��ϴ�.
            {
                return true;
            }
        }
        return false;
    }

    // �ڿ� ���� ������ ���ϴ��� �˻�
    bool IsInResourceZone(Vector2Int worldPosition)
    {
        foreach (Vector2Int zone in resourceZones)
        {
            if (Vector2Int.Distance(worldPosition, zone) < 10f) // ���� �Ÿ� �ȿ� ���� ���� �ڿ� ��ġ
            {
                return true;
            }
        }
        return false;
    }

    // ûũ ��ε� ó��
    public void Unload()
    {
        Destroy(gameObject);
    }
}
