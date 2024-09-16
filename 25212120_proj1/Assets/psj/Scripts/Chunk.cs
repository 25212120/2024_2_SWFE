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
        int mineralCount = 0; // 현재 배치된 미네랄 개수
        int minMineralsInZone = 10; // 자원 구역당 최소 미네랄 개수
        float minDistanceBetweenMinerals = 5f; // 미네랄 간 최소 거리

        List<Vector2> placedMinerals = new List<Vector2>(); // 이미 배치된 미네랄 위치 저장

        for (int x = 0; x < chunkSize; x++)
        {
            for (int y = 0; y < chunkSize; y++)
            {
                // 청크 위치를 기준으로 타일의 월드 좌표 계산
                Vector3 tilePosition = new Vector3(position.x * chunkSize + x, position.y * chunkSize + y, 0);
                Vector2 worldPosition = new Vector2(position.x * chunkSize + x, position.y * chunkSize + y);

                // 자원 구역에 속해 있는지 확인
                if (IsInResourceZone(new Vector2Int((int)worldPosition.x, (int)worldPosition.y)))
                {
                    // 자원 구역의 테두리인지 확인
                    if (IsEdgeOfResourceZone(new Vector2Int((int)worldPosition.x, (int)worldPosition.y)))
                    {
                        // 미네랄 간 최소 거리 조건 확인
                        bool tooCloseToOtherMinerals = false;
                        foreach (Vector2 placedMineral in placedMinerals)
                        {
                            if (Vector2.Distance(placedMineral, worldPosition) < minDistanceBetweenMinerals)
                            {
                                tooCloseToOtherMinerals = true;
                                break;
                            }
                        }

                        // 미네랄 배치 조건: 미네랄 최소 거리와 미네랄 개수 조건을 만족해야 함
                        if (!tooCloseToOtherMinerals && (mineralCount < minMineralsInZone || Random.value > 0.95f))
                        {
                            // 미네랄을 Z값 -1로 설정하여 Grass보다 앞에 렌더링
                            Vector3 mineralPosition = new Vector3(tilePosition.x, tilePosition.y, -1); // Z값을 조정
                            Instantiate(mineralPrefab, mineralPosition, Quaternion.identity, transform);
                            placedMinerals.Add(worldPosition); // 배치된 미네랄 위치 저장
                            mineralCount++;
                        }
                        else
                        {
                            // 테두리 영역이지만 미네랄 간 최소 거리 충족 못하면 Grass 배치
                            Instantiate(grassPrefab, tilePosition, Quaternion.identity, transform);
                        }
                    }
                    else
                    {
                        // 자원 구역의 중앙 부분에는 Grass만 배치
                        Instantiate(grassPrefab, tilePosition, Quaternion.identity, transform);
                    }
                }
                else
                {
                    // 자원 구역 밖은 Grass로 채움
                    Instantiate(grassPrefab, tilePosition, Quaternion.identity, transform);
                }
            }
        }

    }


    // 자원 집중 구역의 테두리인지 확인하는 함수
    bool IsEdgeOfResourceZone(Vector2Int worldPosition)
    {
        foreach (Vector2Int zone in resourceZones)
        {
            float distance = Vector2Int.Distance(worldPosition, zone);

            // 테두리 범위 조정
            if (distance > 9f && distance < 12f) // 범위 값을 수정해 미네랄이 배치되는 범위를 늘립니다.
            {
                return true;
            }
        }
        return false;
    }

    // 자원 집중 구역에 속하는지 검사
    bool IsInResourceZone(Vector2Int worldPosition)
    {
        foreach (Vector2Int zone in resourceZones)
        {
            if (Vector2Int.Distance(worldPosition, zone) < 10f) // 일정 거리 안에 있을 때만 자원 배치
            {
                return true;
            }
        }
        return false;
    }

    // 청크 언로드 처리
    public void Unload()
    {
        Destroy(gameObject);
    }
}
