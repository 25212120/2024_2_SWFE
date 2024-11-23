using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit_SpawnManager : BaseStructure
{
    [Header("업그레이드에 필요한 자원")]
    [SerializeField] private List<ResourceRequirement> spawnRequirements = new List<ResourceRequirement>(); // 업그레이드에 필요한 자원 리스트

    [Header("유닛 스폰 설정")]
    [SerializeField] private GameObject unitPrefab; // 소환할 유닛의 프리팹
    private Transform spawnPoint; // 유닛이 소환될 위치 
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    protected override void Update()
    {
        // 자원 소비 후 유닛 소환
        if (Input.GetKeyDown(KeyCode.X)) // X 키로 소환
        {
            Spawn();
        }
    }

    public bool Spawn()
    {
        // 스폰에 필요한 자원들을 모두 소모할 수 있는지 확인
        foreach (var requirement in spawnRequirements)
        {
            if (!MaterialManager.Instance.ConsumeResource(requirement.resourceType, requirement.amount))
            {
                // 자원이 부족하면 스폰 실패
                Debug.LogWarning($"{requirement.resourceType} 자원이 부족합니다. 유닛 스폰 실패.");
                return false;
            }
        }

        // 모든 자원 소비가 성공하면 유닛 소환 진행
        PerformSpawn();

        return true;
    }

    private void PerformSpawn()
    {
        if (unitPrefab != null && spawnPoint != null)
        {
            // 스폰 포인트에서 유닛을 소환
            Instantiate(unitPrefab, spawnPoint.position, spawnPoint.rotation);
            Debug.Log("유닛이 소환되었습니다.");
        }
        else
        {
            Debug.LogWarning("유닛 프리팹 또는 스폰 포인트가 설정되지 않았습니다.");
        }
    }

    // 각 타워의 스폰 포인트를 설정하는 메서드
    public void SetSpawnPoint(Vector3 newSpawnPosition)
    {
        spawnPoint.position = newSpawnPosition;
        Debug.Log($"스폰 포인트가 ({newSpawnPosition.x}, {newSpawnPosition.z})로 설정되었습니다.");
    }
}
